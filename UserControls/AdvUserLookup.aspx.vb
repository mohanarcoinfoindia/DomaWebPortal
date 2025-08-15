
Public Class UserControls_AdvUserLookup
    Inherits BasePage

    Protected RefreshOnChange As Boolean
    Protected MultiSelect As Boolean
    Protected PropID As Int32
    Protected UsersOnly As Boolean
    Protected DomainName As String
    Protected AutoSelect As Boolean
    Protected MaxGroups As Int32 = 20
    Protected MaxUsers As Int32 = 100
    Protected Modal As Boolean
    Protected RowID As Int32
    Private Function fsJS(ByVal vsText As String) As String

        vsText = vsText.Replace("\", "\\")
        vsText = vsText.Replace("'", "\'")

        Return vsText

    End Function
    Protected Function JSIdentifier() As String
        Return PropID & "_" & RowID
    End Function

    Private Class Item
        Public Property Value As String
        Public Property Description As String
        Public Property Caption As String
    End Class

    Private Sub sLoad()
        PropID = QueryStringParser.GetInt("PROP_ID")
        RefreshOnChange = QueryStringParser.GetBoolean("ROC")
        MultiSelect = QueryStringParser.GetBoolean("MS")
        UsersOnly = QueryStringParser.GetBoolean("Extract")
        Modal = QueryStringParser.GetBoolean("modal")
        RowID = QueryStringParser.GetInt("ROW_ID")
      
        If Not Modal Then
            Me.Form.DefaultFocus = txtFilter.ClientID
        End If
        Me.Form.DefaultButton = lnkFilter.UniqueID

        If Not Page.IsPostBack Then

            If PropID > 0 Then
                Dim loProp As Arco.Doma.Library.baseObjects.DM_PROPERTY = Arco.Doma.Library.baseObjects.DM_PROPERTY.GetPROPERTY(PropID)
                Dim lcolLabels As Arco.Doma.Library.Globalisation.LABELList
                If loProp.ProcID > 0 Then
                    lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetProcedureItemsLabelList(loProp.ProcID, Me.EnableIISCaching)
                Else
                    lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetCategoryItemsLabelList(loProp.CatID, Me.EnableIISCaching)
                End If

                Page.Title = lcolLabels.GetObjectLabel(loProp.ID, "Property", Arco.Security.BusinessIdentity.CurrentIdentity.Language, loProp.Name)
            Else
                Page.Title = "User lookup"
            End If



            Dim lbDefaultShowUsers As Boolean
            Dim lbDefaultShowGroups As Boolean
            Dim lbDefaultShowRoles As Boolean

            Dim lsAutovalidate = Request("autovalidate")
            If Not String.IsNullOrEmpty(lsAutovalidate) Then
                txtFilter.Text = lsAutovalidate
            End If

            lnkFilter.Text = "<span class='icon icon-search' title='" & GetLabel("search") & "' />"
            chkUsers.Text = GetLabel("users")
            chkRoles.Text = GetLabel("roles")
            chkGroups.Text = GetLabel("groups")
            lbDefaultShowUsers = Settings.GetValue("UserBrowser", "DefaultShowUsers", True)
            lbDefaultShowGroups = Settings.GetValue("UserBrowser", "DefaultShowGroups", True)
            lbDefaultShowRoles = Settings.GetValue("UserBrowser", "DefaultShowRoles", True)

            Dim lcolDomains As Arco.Doma.Library.ACL.DOMAINList = Arco.Doma.Library.ACL.DOMAINList.GetDOMAINSList(True)
            cmbDomains.Items.Clear()
            cmbDomains.Items.Add("")
            For Each loDomain As Arco.Doma.Library.ACL.DOMAINList.DOMAINSInfo In lcolDomains
                cmbDomains.Items.Add(loDomain.DOMAIN_NAME)
            Next



            If Not UsersOnly Then
                chkRoles.Visible = True
                chkGroups.Visible = True
                chkUsers.Visible = True
                chkRoles.Checked = lbDefaultShowRoles
                chkGroups.Checked = lbDefaultShowGroups
                chkUsers.Checked = lbDefaultShowUsers


            Else
                chkRoles.Visible = False
                chkGroups.Visible = False
                chkUsers.Visible = False
                chkRoles.Checked = False
                chkGroups.Checked = False
                chkUsers.Checked = True
            End If

            'if there is only the dummy domain
            If cmbDomains.Items.Count = 2 Then
                chkGroups.Visible = False
                chkGroups.Checked = False
                cmbDomains.Visible = False
            End If
        End If

        DomainName = cmbDomains.SelectedItem.Text

        Dim liResCount As Int32 = 0
        Dim lsValue As String = ""
        Dim lsCaption As String = ""

        If chkRoles.Checked Then
            Dim lbIncludeEveryone As Boolean = QueryStringParser.GetBoolean("EVR", True)
            Dim roles As Arco.Doma.Library.ACL.ROLEList = Arco.Doma.Library.ACL.ROLEList.GetROLEList(txtFilter.Text, lbIncludeEveryone)
            Dim lcolRoleLabels As Arco.Doma.Library.Globalisation.LABELList = Arco.Doma.Library.Globalisation.LABELList.GetRolesLabelList(EnableIISCaching)
            liResCount += roles.Count
            Dim loItems As New List(Of Item)
            For Each role As Arco.Doma.Library.ACL.ROLEList.ROLEInfo In roles
                loItems.Add(New Item() With {.Value = "(Role) " & role.Name, .Caption = Arco.Doma.Library.ACL.Role.GetDisplayValue(role.ID, role.Name, lcolRoleLabels), .Description = role.Description})
            Next

            For Each itm As Item In loItems.OrderBy(Function(x) x.Caption)
                Dim loTR As New TableRow
                Dim loTD As New TableCell

                Dim spanIconRole As LiteralControl = New LiteralControl("<span class='icon icon-user-role icon-color-light' title='" & GetLabel("role") & "' />")

                loTD.Controls.Add(spanIconRole)
                loTR.Controls.Add(loTD)

                Dim loAdd As HyperLink = New HyperLink
                loAdd.NavigateUrl = "javascript:Select(" & EncodingUtils.EncodeJsString(itm.Value) & "," & EncodingUtils.EncodeJsString(itm.Caption) & ");"
                loAdd.Text = Server.HtmlEncode(itm.Caption)

                loTD = New TableCell
                loTD.Controls.Add(loAdd)
                loTR.Controls.Add(loTD)

                loTD = New TableCell
                loTD.Text = Server.HtmlEncode(itm.Description)
                loTR.Controls.Add(loTD)

                SubjectList.Controls.Add(loTR)

                lsValue = itm.Value
                lsCaption = itm.Caption
            Next

        End If

        If DomainName <> "_DATABASE" AndAlso chkGroups.Checked Then
            Dim groups As Arco.Doma.Library.ACL.GROUPList = Arco.Doma.Library.ACL.GROUPList.GetGROUPSList(DomainName, txtFilter.Text)
            Dim iGrp As Int32 = 0
            liResCount += groups.Count
            For Each group As Arco.Doma.Library.ACL.GROUPList.GROUPSInfo In groups


                If iGrp < MaxGroups Then
                    Dim loTR As New TableRow
                    Dim loTD As New TableCell

                    Dim spanIconGroup = New LiteralControl("<span class='icon icon-user-group icon-color-light' title='" & GetLabel("group") & "' />")
                    loTD.Controls.Add(spanIconGroup)
                    loTR.Controls.Add(loTD)

                    Dim loAdd As New HyperLink
                    lsCaption = group.GROUP_NAME
                    lsValue = "(Group) " & group.GROUP_NAME
                    loAdd.NavigateUrl = "javascript:Select(" & EncodingUtils.EncodeJsString(lsValue) & "," & EncodingUtils.EncodeJsString(lsCaption) & ");"
                    loAdd.Text = Server.HtmlEncode(lsCaption)

                    loTD = New TableCell
                    loTD.Controls.Add(loAdd)
                    loTR.Controls.Add(loTD)

                    loTD = New TableCell
                    loTD.Text = Server.HtmlEncode(group.GROUP_DESC)
                    loTR.Controls.Add(loTD)


                    SubjectList.Controls.Add(loTR)
                    iGrp = iGrp + 1
                Else
                    Dim loTR As New TableRow
                    Dim loTD As New TableCell

                    loTD.ColumnSpan = 2
                    loTD.Text = "Please Filter...."
                    loTR.Controls.Add(loTD)

                    SubjectList.Controls.Add(loTR)
                    Exit For

                End If
            Next
        End If

        If chkUsers.Checked Then
            Dim users As Arco.Doma.Library.ACL.USERList = Arco.Doma.Library.ACL.USERList.GetUSERSList(DomainName, txtFilter.Text)
            Dim iUsr As Int32 = 0
            liResCount += users.Count
            Dim loItems As New List(Of Item)
            Dim lbShowFilterMore As Boolean = False
            For Each loUser As Arco.Doma.Library.ACL.USERList.USERSInfo In users
                If loUser.USER_STATUS = Arco.Doma.Library.ACL.User.UserStatus.Valid Then
                    If iUsr < MaxUsers Then
                        Dim lsLogin As String = loUser.USER_ACCOUNT

                        loItems.Add(New Item() With {.Value = lsLogin, .Caption = ArcoFormatting.FormatUserName(lsLogin), .Description = loUser.USER_DESC})
                        iUsr = iUsr + 1
                    Else
                        lbShowFilterMore = True
                        Exit For
                    End If
                End If
            Next

            For Each itm As Item In loItems.OrderBy(Function(x) x.Caption)
                Dim loTR As New TableRow
                Dim loTD As New TableCell

                Dim spanIconUser = New LiteralControl("<span class='icon icon-user-profile icon-color-light' title='" & GetLabel("user") & "' />")

                loTD.Controls.Add(spanIconUser)
                loTR.Controls.Add(loTD)

                Dim loAdd As New HyperLink
                loAdd.NavigateUrl = "javascript:Select(" & EncodingUtils.EncodeJsString(itm.Value) & "," & EncodingUtils.EncodeJsString(Server.HtmlDecode(itm.Caption)) & ");"
                loAdd.Text = itm.Caption

                loTD = New TableCell
                loTD.Controls.Add(loAdd)
                loTR.Controls.Add(loTD)

                loTD = New TableCell
                loTD.Text = Server.HtmlEncode(itm.Description)
                loTR.Controls.Add(loTD)

                SubjectList.Controls.Add(loTR)

                lsValue = itm.Value
                lsCaption = itm.Caption

            Next
            If lbShowFilterMore Then
                Dim loTR As New TableRow
                Dim loTD As New TableCell
                loTD.ColumnSpan = 2
                loTD.Text = "Please Filter...."
                loTR.Controls.Add(loTD)

                SubjectList.Controls.Add(loTR)
            End If         
        End If

        If liResCount = 1 AndAlso (Not Page.IsPostBack OrElse Not MultiSelect) Then
            Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "autoselect", "AutoSelect(" & EncodingUtils.EncodeJsString(lsValue) & "," & EncodingUtils.EncodeJsString(Server.HtmlDecode(lsCaption)) & ");", True)
        Else
            Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "yourselection", "UpdateYourSelection();", True)
        End If
    End Sub

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
        sLoad()
    End Sub
End Class
