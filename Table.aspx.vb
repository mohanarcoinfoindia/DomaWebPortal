Imports Arco.Doma.Library
Imports Arco.Doma.Library.Website
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Globalisation

Public Class Table
    Inherits BasePage


    Protected llID As Int32
    Protected llCaseID As Int32
    Protected llTechID As Int32
    Protected llTableID As Int32
    Protected _screenID As Int32
    Protected llParentRowID As Int32
    Protected lbFromArchive As Boolean

    Dim llMode As Int32
    Dim obj As DM_OBJECT
    Dim loCase As Routing.cCase
    Private msOpts As String
    Private mModal As Boolean
    Private mWidth As Int32 = 600
    Public ReadOnly Property Modal() As Boolean
        Get
            Return mModal
        End Get
    End Property
    Public Property Width() As Int32
        Get
            Return (mWidth - 40)
        End Get
        Set(ByVal value As Int32)
            mWidth = value
        End Set
    End Property
    Private Function GetPostBackString(ByVal btn As LinkButton) As String
        Return GetPostBackString(btn, True, "")
    End Function
    Private Function GetPostBackString(ByVal btn As LinkButton, ByVal vbValidate As Boolean, ByVal vsArgument As String) As String
        Dim objPostBackoptions As PostBackOptions
        If vbValidate Then
            objPostBackoptions = New PostBackOptions(btn, vsArgument, "", True, True, True, True, True, btn.ValidationGroup)
        Else
            objPostBackoptions = New PostBackOptions(btn, vsArgument)
        End If
        Return Page.ClientScript.GetPostBackEventReference(objPostBackoptions)

    End Function
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        mModal = QueryStringParser.GetBoolean("modal")
        llID = QueryStringParser.GetInt("DM_OBJECT_ID")
        llCaseID = QueryStringParser.GetInt("CASE_ID")
        llTechID = QueryStringParser.GetInt("TECH_ID")
        llTableID = QueryStringParser.GetInt("DM_TABLE_ID")
        _screenID = QueryStringParser.GetScreenId("SCREEN_ID")
        llParentRowID = QueryStringParser.GetInt("PARENT_ROW_ID")
        msOpts = QueryStringParser.GetString("OPTS")
        llMode = QueryStringParser.GetInt("mode")

        lbFromArchive = QueryStringParser.GetBoolean("fromarchive")

        Dim strScript As New StringBuilder
        strScript.AppendLine("function SaveRows() {")
        strScript.AppendLine(GetPostBackString(btnSave) & ";")
        strScript.AppendLine("}")

        strScript.AppendLine("function DeleteRecord(id)")
        strScript.AppendLine("{")
        strScript.AppendLine("    Page_ClientValidate('');")
        strScript.AppendLine("    if (Page_IsValid){")
        strScript.AppendLine("        if (window.confirm(UIMessages[21])) {")
        strScript.AppendLine("                    document.getElementById('" & hdnRow.ClientID & "').value = id;")
        strScript.AppendLine(GetPostBackString(btnDelete) & ";")
        strScript.AppendLine("        }")
        strScript.AppendLine("    }")
        strScript.AppendLine("}")
        strScript.AppendLine(" function CopyRecord(id)")
        strScript.AppendLine("{")
        strScript.AppendLine("     Page_ClientValidate('CheckMandatoryFields');")
        strScript.AppendLine("    if (Page_IsValid){")
        strScript.AppendLine("                    document.getElementById('" & hdnRow.ClientID & "').value = id;")
        strScript.AppendLine(GetPostBackString(btnCopy) & ";")
        strScript.AppendLine("   }")
        strScript.AppendLine("  }")
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "detailviewfuncs", strScript.ToString, True)

        If Not Page.IsPostBack Then
            lnkNew.ToolTip = GetDecodedLabel("new")
            cmdSave.ToolTip = GetDecodedLabel("save")

            cmdSaveAndClose.ToolTip = GetDecodedLabel("savecomplete")           
            lnkClose.ToolTip = GetDecodedLabel("close")

            lnkNew.ImageUrl = ThemedImage.GetUrl("new.png", Me)
            cmdSave.ImageUrl = ThemedImage.GetUrl("save.png", Me)
            cmdSaveAndClose.ImageUrl = ThemedImage.GetUrl("accept.png", Me)
            lnkClose.ImageUrl = ThemedImage.GetUrl("cancel.png", Me)

        End If

        BindDetail(False)
    End Sub

    Private Sub BindDetail(ByVal vbRebind As Boolean)

        Dim lcolItems As ScreenItemList
        Dim liFormMode As Screen.DetailScreenDisplayMode
        If llMode = 1 Then
            If _screenID = 0 Then

                lcolItems = ScreenItemList.GetDefaultTableScreenItems(llTableID, ScreenItem.ItemMode.ReadOnly)
            End If
            liFormMode = Screen.DetailScreenDisplayMode.ReadOnly
        Else
            If _screenID = 0 Then
                lcolItems = ScreenItemList.GetDefaultTableScreenItems(llTableID, ScreenItem.ItemMode.Writable)
            End If
            liFormMode = Screen.DetailScreenDisplayMode.Edit
        End If

        Dim lcolLabels As LABELList = Nothing
        If llCaseID > 0 Then
            If Not lbFromArchive Then
                If llTechID > 0 Then
                    loCase = Routing.cCase.GetCase(llTechID)
                Else
                    loCase = Routing.cCase.GetCaseByCaseID(llCaseID)
                End If
                obj = loCase.GetPropertyLocationObject(llTableID)
                lcolLabels = LABELList.GetProcedureItemsLabelList(loCase.Proc_ID, Me.EnableIISCaching)
                If liFormMode <> Screen.DetailScreenDisplayMode.ReadOnly Then
                    If Not loCase.CurrentUserHasWork AndAlso Not loCase.CanAdminData Then
                        liFormMode = Screen.DetailScreenDisplayMode.ReadOnly
                    End If
                End If
                If _screenID <> 0 Then
                    lcolItems = ScreenItemList.GetTableScreenItems(_screenID, llTableID, loCase, Device.Web, SiteVersion.Version7)
                End If
            Else
                obj = Routing.HistoryCase.GetHistoryCase(llCaseID)
                lcolLabels = LABELList.GetProcedureItemsLabelList(CType(obj, Arco.Doma.Library.Routing.HistoryCase).Proc_ID, Me.EnableIISCaching)
                liFormMode = Screen.DetailScreenDisplayMode.ReadOnly
                If _screenID <> 0 Then
                    lcolItems = ScreenItemList.GetTableScreenItems(_screenID, llTableID, obj, Device.Web, 7)
                End If
            End If

        Else

            obj = ObjectRepository.GetObject(llID)
            If liFormMode <> Screen.DetailScreenDisplayMode.ReadOnly Then
                If Not obj.CanModifyMeta Then
                    liFormMode = Screen.DetailScreenDisplayMode.ReadOnly
                End If
            End If
            If liFormMode = 1 Then
                If Not obj.CanViewMeta Then
                    Server.Transfer("DM_ACL_DENIED.aspx")
                End If
            End If

            lcolLabels = LABELList.GetCategoryItemsLabelList(obj.Category, Me.EnableIISCaching)
            If _screenID <> 0 Then
                lcolItems = ScreenItemList.GetTableScreenItems(_screenID, llTableID, obj, Device.Web, 7)
            End If
        End If


        If Not obj Is Nothing Then
            Dim loTableInfo As DM_PROPERTY = DM_PROPERTY.GetPROPERTY(llTableID)
            Dim lbCopy As Boolean = True
            Dim lbDelete As Boolean = True

            If String.IsNullOrEmpty(msOpts) Then msOpts = loTableInfo.Definition

            If (msOpts.Length = 3) Then
                If (msOpts.Substring(0, 1) = "N") Then
                    lnkNew.Visible = False
                    tlbMain.Items(1).FindControl("txtNumRows").Visible = False
                    tlbMain.Items(2).Visible = False 'separator
                    lbCopy = False
                End If
                If (msOpts.Substring(1, 1) = "N") Then
                    llMode = 1
                End If
                If (msOpts.Substring(2, 1) = "N") Then
                    lbDelete = False
                End If
            End If



            Dim propInfo As DMObjectPROPERTYList.PROPERTYInfo = obj.GetPropertyInfo(llTableID, llParentRowID)
            If lnkNew.Visible AndAlso Not propInfo.CanAddRow Then
                lnkNew.Enabled = False
                lbCopy = False
            End If
            If lbDelete Then
                lbDelete = propInfo.CanRemoveRow
            End If

            Dim loTable As DM_OBJECT.Table = DirectCast(obj.GetProperty(propInfo), DM_OBJECT.Table)
            If Not vbRebind Then
                rows.Controls.Add(New LiteralControl("<table class='SubList'>"))
            End If

            Dim lcolRows As List(Of DM_OBJECT.Table.TableRow) = loTable.Rows
            If propInfo.PropertyDefinition.Tag2 = 1 Then lcolRows = lcolRows.OrderByDescending(Function(x) x.Row_ID).ToList

            For Each loRow As DM_OBJECT.Table.TableRow In lcolRows
                If loRow.Row_ID > 0 Then
                    Dim loRowForm As New DMObjectForm With {
                        .ModalWindow = Modal
                    }
                    If Not vbRebind Then
                        loRowForm.ID = "rowform" & loRow.Row_ID
                        loRowForm.Labels = lcolLabels
                        loRowForm.CategoryID = obj.Category
                        loRowForm.ScreenID = _screenID
                        loRowForm.RenderAsRow = True
                        loRowForm.DisplayItems = lcolItems
                        loRowForm.DisplayMode = liFormMode
                        loRowForm.isTableRecord = True
                        loRowForm.RenderValidatorSummary = False
                        loRowForm.IsMainScreen = False

                        rows.Controls.Add(New LiteralControl("<tr>"))
                        If lbCopy Then
                            rows.Controls.Add(New LiteralControl("<td><a href='javascript:CopyRecord(" & loRow.Row_ID & ");'>" & ThemedImage.GetImageTag("copy.png", Page, GetLabel("copy")) & "</A></td>"))
                        End If
                        If lbDelete Then
                            rows.Controls.Add(New LiteralControl("<td><a href='javascript:DeleteRecord(" & loRow.Row_ID & ");'>" & ThemedImage.GetImageTag("delete.png", Page, GetLabel("delete")) & "</A></td>"))
                        End If
                        rows.Controls.Add(loRowForm)
                        rows.Controls.Add(New LiteralControl("</tr>"))
                    Else
                        loRowForm = DirectCast(rows.FindControl("rowform" & loRow.Row_ID), DMObjectForm)
                    End If

                    If Not Page.IsPostBack OrElse vbRebind Then                        
                        loRowForm.DataBindTableRow(llTechID, llCaseID, llID, llTableID, loRow.Row_ID, llParentRowID, obj, vbRebind)
                    End If
                ElseIf Not vbRebind Then
                    'header
                    rows.Controls.Add(New LiteralControl("<tr>"))
                    If lbCopy Then
                        rows.Controls.Add(New LiteralControl("<th>&nbsp;</th>"))
                    End If
                    If lbDelete Then
                        rows.Controls.Add(New LiteralControl("<th>&nbsp;</th>"))
                    End If
                    Dim idx As Int32 = 0
                    For Each loitem As Website.ScreenItemList.ScreenItemInfo In lcolItems
                        If loitem.Mode <> ScreenItem.ItemMode.Hidden Then
                            rows.Controls.Add(New LiteralControl("<th>"))
                            Dim lsCaption As String = "&nbsp;"
                            Select Case loitem.Type
                                Case ScreenItem.ItemType.ObjectProperty
                                    lsCaption = lcolLabels.GetObjectLabel(loitem.PROP_ID, "Property", Language, loitem.NAME)
                            End Select
                            rows.Controls.Add(New LiteralControl(lsCaption))
                            If loitem.Mode = ScreenItem.ItemMode.Mandatory Then
                                rows.Controls.Add(New MandatoryMarker(lsCaption + " " + GetLabel("req")) With {.ID = "mandmark" & idx})
                            End If
                            rows.Controls.Add(New LiteralControl("</th>"))
                            idx += 1
                        End If
                    Next

                    rows.Controls.Add(New LiteralControl("</tr>"))
                End If
            Next
            If Not vbRebind Then
                rows.Controls.Add(New LiteralControl("</table>"))
            End If

            Page.Title = lcolLabels.GetObjectLabel(loTableInfo.ID, "Property", Language, loTableInfo.Name)
        End If
    End Sub
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        SaveRows(True)
    End Sub
    Private Function SaveRows(ByVal rebind As Boolean) As Boolean

        If Page.IsValid Then
            Dim lbOk As Boolean
            Dim strMessage As String = ""
            If Settings.GetValue("Interface", "TablesSaveRowPerRow", True) Then
                For Each c As Control In rows.Controls
                    If TypeOf (c) Is DMObjectForm Then
                        If Not loCase Is Nothing Then
                            lbOk = CType(c, DMObjectForm).SaveTableRow(loCase)
                        Else
                            lbOk = CType(c, DMObjectForm).SaveTableRow(obj)

                        End If

                        Dim lsMsg As String = CType(c, DMObjectForm).ErrorMessage
                        If Not String.IsNullOrEmpty(lsMsg) Then
                            strMessage &= lsMsg
                        End If
                        If Not lbOk Then
                            Exit For
                        End If
                    End If
                Next
            Else
                For Each c As Control In rows.Controls
                    If TypeOf (c) Is DMObjectForm Then
                        If Not loCase Is Nothing Then
                            CType(c, DMObjectForm).SaveTableRowForm(loCase)
                        Else
                            CType(c, DMObjectForm).SaveTableRowForm(obj)
                        End If
                    End If
                Next

                If Not loCase Is Nothing Then
                    loCase = loCase.SaveWithOnKeep(True)
                    If loCase.ActionCancelled Then
                        strMessage = loCase.HandlerMessage
                    End If
                Else
                    obj = obj.Save
                    If obj.HasError Then
                        strMessage = obj.GetLastError.Description
                    End If
                End If
            End If


            If Not String.IsNullOrEmpty(strMessage) Then
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "AlertHandlerMessage", "alert('" & strMessage & "');", True)
                Return False
            Else
                'rebind the form, things can change
                If rebind Then BindDetail(True)
                Return True
            End If
        Else
            Return False
        End If


    End Function
    Private Sub Reload()
        Response.Redirect("Table.aspx" & QueryStringParser.ToString())
    End Sub
    Private Sub sClose()
        Page.ClientScript.RegisterStartupScript(Me.GetType, "Close", "GetRadWindow().Close();", True)
    End Sub
    Protected Sub tlbMain_ButtonClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadToolBarEventArgs) Handles tlbMain.ButtonClick
        Dim btn As Telerik.Web.UI.RadToolBarButton = CType(e.Item, Telerik.Web.UI.RadToolBarButton)
        Select Case btn.CommandName.ToUpper
            Case "SAVE"
                If SaveRows(False) Then
                    Reload()
                End If
            Case "SAVEANDCLOSE"
                If SaveRows(False) Then
                    sClose()
                End If
            Case "NEW"
                If SaveRows(False) Then
                    Dim txtNumRows As Telerik.Web.UI.RadNumericTextBox = DirectCast(btn.ToolBar.Items(1).FindControl("txtNumRows"), Telerik.Web.UI.RadNumericTextBox)
                    Dim iRows As Int32
                    If Int32.TryParse(txtNumRows.Value, iRows) Then
                        Try
                            obj.CreateRowsInTable(llTableID, iRows, llParentRowID)
                            obj = obj.Save
                        Catch ex As Exceptions.PropertyValueTooLongException
                            Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "AlertHandlerMessage", "alert('" & ex.Message & "');", True)
                        End Try

                        Reload()
                    End If

                End If
        End Select
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As System.EventArgs) Handles btnDelete.Click
        If SaveRows(False) Then
            Dim llRow As Int32 = Convert.ToInt32(hdnRow.Value)
            If llRow > 0 Then
                obj = obj.RemoveRowFromTable(llTableID, llRow)
                obj = obj.Save
                Reload()
            End If
        End If

    End Sub

    Protected Sub btnCopy_Click(sender As Object, e As System.EventArgs) Handles btnCopy.Click
        If SaveRows(False) Then
            Dim llRow As Int32 = Convert.ToInt32(hdnRow.Value)
            If llRow > 0 Then
                Dim t As DM_OBJECT.Table = DirectCast(obj.GetProperty(llTableID, llParentRowID), DM_OBJECT.Table)
                Dim llNewRowID As Int32 = 0
                obj = t.CopyRow(llRow, llNewRowID)
                obj = obj.Save
                Reload()
            End If
        End If

    End Sub
End Class
