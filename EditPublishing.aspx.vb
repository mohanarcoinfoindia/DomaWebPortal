Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Publishing
Imports Telerik.Web.UI

Partial Class EditPublishing
    Inherits BasePage
    Protected ItemID As Int32
    Protected ItemType As String = ""
    Protected moPublishedItem As PublishedItem


    Protected Sub lstRules_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles lstRules.RowDeleting
        Dim g As GridView = CType(sender, GridView)
        Dim selectedRow As GridViewRow = g.Rows(e.RowIndex)
        Dim selectedCell As TableCell = selectedRow.Cells(0)

        PublishedItemRule.DeletePublishedItemRule(Convert.ToInt32(selectedCell.Text))

        LoadPublishItem()
        ShowRules()
        e.Cancel = True 'cancel the original delete
    End Sub
    Protected Sub lstRules_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles lstRules.RowCommand
        Select Case e.CommandName

            Case "cmdDeleteItem"
                Dim index As Integer = Convert.ToInt32(e.CommandArgument)
                Dim selectedRow As GridViewRow = lstRules.Rows(index)
                Dim selectedCell As TableCell = selectedRow.Cells(0)

                PublishedItemRule.DeletePublishedItemRule(Convert.ToInt32(selectedCell.Text))

                LoadPublishItem()
                ShowRules()
        End Select

    End Sub
    Protected Sub lstRules_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles lstRules.RowDataBound
        Dim r As GridViewRow = e.Row

        r.Cells(eCell.ID).Visible = False
        r.Cells(eCell.SubjectUnformatted).Visible = False

        If r.RowType = DataControlRowType.DataRow Then

            Dim lsSubjectCaption As String
            Dim lsSubjectType As String = r.Cells(eCell.SubjectType).Text
            If lsSubjectType = "User" Then
                lsSubjectCaption = ArcoFormatting.FormatUserName(r.Cells(eCell.SubjectUnformatted).Text)
            Else
                lsSubjectCaption = r.Cells(eCell.SubjectUnformatted).Text
            End If
            r.Cells(eCell.SubjectCaption).Text = lsSubjectCaption

            Dim lsExp As String = CType(e.Row.DataItem, PublishedItemRuleList.RuleInfo).ExpiryDate
            Dim dp As RadDateTimePicker = DirectCast(r.FindControl("dpExp"), RadDateTimePicker)
            If Not dp Is Nothing Then

                If Not String.IsNullOrEmpty(lsExp) Then
                    Dim datValue As DateTime
                    If DateTime.TryParse(lsExp, datValue) Then
                        dp.SelectedDate = datValue
                    End If
                End If
            End If
            Dim l As Label = DirectCast(r.FindControl("lblExpDate"), Label)
            If Not l Is Nothing Then
                l.Text = ArcoFormatting.FormatDateLabel(lsExp, True, False, False)
            End If

        End If
    End Sub

    Private Enum eCell
        ID = 0
        SubjectCaption = 1
        SubjectType = 2
        SubjectUnformatted = 3
        '  Expires = 4
        '   DeleteButton = 5
    End Enum
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit

        If Not QueryStringParser.GetBoolean("inline") Then
            Me.MasterPageFile = "~/masterpages/ToolWindow.master"
        Else
            Me.MasterPageFile = "~/masterpages/DetailSub.master"
        End If
    End Sub
    Private Sub LoadPublishItem()
        Dim lbCanPublish As Boolean
        Select Case ItemType
            Case "File"
                Dim loFile As File = File.GetFile(ItemID)
                lbCanPublish = loFile.CanView 'todo
                If lbCanPublish Then
                    lblObjectType.Text = GetLabel("file")
                    lblObjectName.Text = loFile.Name
                    moPublishedItem = PublishedItem.GetPublishedItem(loFile.FIN, "File", True)
                End If

            Case Else
                Dim loObject As DM_OBJECT = ObjectRepository.GetObject(ItemID)
                lbCanPublish = loObject.CanViewMeta 'todo
                If lbCanPublish Then
                    lblObjectType.Text = GetLabel(loObject.Object_Type)
                    lblObjectName.Text = loObject.Name
                    moPublishedItem = PublishedItem.GetPublishedItem(loObject.DIN, "Object", True)
                End If
        End Select


        If lbCanPublish Then
            If Not String.IsNullOrEmpty(moPublishedItem.GUID) Then
                lblUrl.Text = ConvertRelativeUrlToAbsoluteUrl(moPublishedItem.GetUrl(""))
            Else
                lblUrl.Text = ""
            End If
        Else
            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        End If
    End Sub
    Public Function ConvertRelativeUrlToAbsoluteUrl(ByVal relativeUrl As String) As String

        If (Request.IsSecureConnection) Then
            Return String.Format("https://{0}{1}", Request.Url.Host, Page.ResolveUrl(relativeUrl))
        Else
            Return String.Format("http://{0}{1}", Request.Url.Host, Page.ResolveUrl(relativeUrl))
        End If
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not Settings.GetValue("Publishing", "Enabled", True) Then
            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
            Return
        End If

        Page.Title = GetDecodedLabel("publishing")

        lblAdd.Text = GetLabel("add")
        lblExpires.Text = GetLabel("expirydate")

        ItemType = QueryStringParser.GetString("ITEM_TYPE")

        ItemID = QueryStringParser.GetInt("ITEM_ID")


        If ItemID > 0 Then
            LoadPublishItem()

            If Page.IsPostBack Then
                If Not String.IsNullOrEmpty(newUser.Subject) Then
                    If String.IsNullOrEmpty(moPublishedItem.GUID) Then
                        moPublishedItem = moPublishedItem.Save
                    End If
                    Dim lbFound As Boolean = PublishedItemRuleList.GetRules(moPublishedItem.GUID, False, False).Cast(Of PublishedItemRuleList.RuleInfo).Distinct _
                        .Any(Function(x) x.Subject_ID.Equals(newUser.SubjectID) AndAlso x.Subject_Type.Equals(newUser.SubjectType))

                    If Not lbFound Then
                        Dim loNewRule As PublishedItemRule = PublishedItemRule.NewPublishedItemRule(moPublishedItem.GUID)
                        loNewRule.Subject_ID = newUser.SubjectID
                        loNewRule.Subject_Type = newUser.SubjectType
                        If dpExpNew.SelectedDate.HasValue Then
                            loNewRule.ExpiryDate = dpExpNew.SelectedDate.Value.ToString(ArcoFormatting.GetDateTimeFormatForSave(False))
                        End If

                        loNewRule.Save()
                    End If
                    ShowRules()
                    newUser.Clear()

                End If
            Else
                If Not String.IsNullOrEmpty(moPublishedItem.GUID) Then
                    ShowRules()
                End If
            End If

        Else
            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        End If

    End Sub

    Private Sub ShowRules()
        lstRules.DataSource = moPublishedItem.FullRulesList
        lstRules.DataBind()

    End Sub
    Protected Sub lstRules_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles lstRules.RowCancelingEdit
        lstRules.EditIndex = -1
        ShowRules()
    End Sub

    Protected Sub lstRules_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles lstRules.RowEditing
        lstRules.EditIndex = e.NewEditIndex
        ShowRules()
    End Sub

    Protected Sub lstRules_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles lstRules.RowUpdating

        Dim selectedRow As GridViewRow = lstRules.Rows(e.RowIndex)
        Dim selectedCell As TableCell = selectedRow.Cells(0)

        Dim ruleid As Int32 = Convert.ToInt32(selectedCell.Text)

        Dim rule As PublishedItemRule = PublishedItemRule.GetPublishedItemRule(ruleid)
        If rule IsNot Nothing Then
            Dim dp As RadDateTimePicker = DirectCast(lstRules.Rows(e.RowIndex).FindControl("dpExp"), RadDateTimePicker)
            Dim lsValue As String = ""
            If (dp.SelectedDate.HasValue) Then
                lsValue = dp.SelectedDate.Value.ToString(ArcoFormatting.GetDateTimeFormatForSave(False))
            End If
            rule.ExpiryDate = lsValue
            rule.Save()
        End If

        e.Cancel = True 'cancel the original update
        lstRules.EditIndex = -1
        ShowRules()
    End Sub
End Class
