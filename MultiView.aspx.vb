Imports Arco.Doma.Library
Imports Arco.Doma.Library.Website

Partial Class MultiView
    Inherits BasePage

    Private mModal As Boolean = False
    Private mWidth As Int32 = 600
    Private llMode As Int32 = 0
    Private llScreenID As Int32 = 0
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
        llMode = QueryStringParser.GetInt("mode")
        llScreenID = QueryStringParser.GetScreenId("SCREEN_ID")

        Dim strScript As StringBuilder = New StringBuilder
        strScript.AppendLine("function SaveObject() {")
        strScript.AppendLine(GetPostBackString(btnSave) & ";")
        strScript.AppendLine("}")

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "detailviewfuncs", strScript.ToString, True)

        If Not Page.IsPostBack Then
            cmdSave.ImageUrl = ThemedImage.GetUrl("savekeepopen.png", Me)
            cmdSaveAndClose.ImageUrl = ThemedImage.GetUrl("release.png", Me)
            lnkClose.ImageUrl = ThemedImage.GetUrl("cancel.png", Me)

            cmdSave.ToolTip = GetDecodedLabel("save")
            cmdSaveAndClose.ToolTip = GetDecodedLabel("savecomplete")
            lnkClose.ToolTip = GetDecodedLabel("close")
        End If

        BindDetail(False)

    End Sub

    Private moItems As List(Of baseObjects.DM_OBJECT)

    Private Sub LoadItems()
        moItems = New List(Of baseObjects.DM_OBJECT)
        Dim objectList As String = QueryStringParser.GetString("OBJECTLIST")

        If Not String.IsNullOrEmpty(objectList) Then
            Dim lsIDs() As String = objectList.Split(";"c)
            Dim llCatID As Int32 = 0
            For Each lsID As String In lsIDs
                If Not String.IsNullOrEmpty(lsID) Then
                    Dim loObj As baseObjects.DM_OBJECT = ObjectRepository.GetObject(Convert.ToInt32(lsID))
                    If loObj.HasCategory AndAlso ((llMode = 1 AndAlso loObj.CanViewMeta) OrElse (llMode = 2 AndAlso loObj.CanModifyMeta)) Then
                        Dim lbAdd As Boolean = True
                        If llCatID = 0 Then
                            llCatID = loObj.Category
                        Else
                            If llCatID <> loObj.Category Then
                                lbAdd = False
                            End If
                        End If
                        If lbAdd AndAlso llMode = 2 AndAlso Not Page.IsPostBack Then
                            loObj = loObj.Lock() 'lock the document -> only the status will be updated
                            If loObj.Status = baseObjects.DM_OBJECT.Object_Status.Production Then 'couldn't be locked, someone else has locked it
                                lbAdd = False
                            End If
                        End If
                        If lbAdd Then

                            moItems.Add(loObj)
                        End If
                    End If
                End If
            Next
        End If
    End Sub
    Private Sub BindDetail(ByVal vbRebind As Boolean)

        LoadItems()

        If moItems.Count = 0 Then
            Exit Sub
        End If


        Dim lcolItems As ScreenItemList
        If llScreenID = 0 Then
            lcolItems = moItems.Item(0).GetDetailScreen(Screen.DetailScreenDisplayMode.BatchEdit, Device.Web, 7).ScreenItems
        Else
            lcolItems = ScreenItemList.GetScreenItems(llScreenID)
        End If

        Dim liFormMode As Screen.DetailScreenDisplayMode
        If llMode = 1 Then           
            liFormMode = Screen.DetailScreenDisplayMode.ReadOnly
        Else         
            liFormMode = Screen.DetailScreenDisplayMode.Edit
        End If

        If liFormMode = Screen.DetailScreenDisplayMode.Edit Then
            cmdSave.Enabled = True
            cmdSaveAndClose.Enabled = True
        Else
            cmdSave.Enabled = False
            cmdSaveAndClose.Enabled = False
        End If

        Dim lcolLabels As Globalisation.LABELList = Globalisation.LABELList.GetCategoryItemsLabelList(moItems.Item(0).Category, Me.EnableIISCaching)


        If Not vbRebind Then
            rows.Controls.Add(New LiteralControl("<table class='SubList'>"))
        End If

        Dim lbFirst As Boolean = True
        Dim idx As Int32 = 0
        For Each loObj As baseObjects.DM_OBJECT In moItems
            If lbFirst Then
                If Not vbRebind Then
                    rows.Controls.Add(New LiteralControl("<tr>"))

                    For Each loitem As Website.ScreenItemList.ScreenItemInfo In lcolItems
                        If loitem.Mode <> ScreenItem.ItemMode.Hidden Then
                            Dim lbAdd As Boolean = True
                            Dim lsCaption As String = "&nbsp;"
                            Select Case loitem.Type
                                Case ScreenItem.ItemType.ObjectProperty
                                    lsCaption = lcolLabels.GetObjectLabel(loitem.PROP_ID, "Property", Language, loitem.NAME)
                                Case ScreenItem.ItemType.Fixed
                                    Select Case loitem.FixedItemID
                                        Case ScreenItem.FixedItem.Category, ScreenItem.FixedItem.Files, ScreenItem.FixedItem.BrowseContent, ScreenItem.FixedItem.Work, ScreenItem.FixedItem.LinkedDossiers, ScreenItem.FixedItem.EditChecks, ScreenItem.FixedItem.EndTab, ScreenItem.FixedItem.Tasks
                                            lbAdd = False
                                        Case Else
                                            lsCaption = GetLabel(Arco.Doma.Library.Website.ScreenItem.GetFixedItemLabelToken(loitem.ITEM_ID))
                                    End Select
                                Case ScreenItem.ItemType.Html, ScreenItem.ItemType.WebControl

                                Case Else
                                    lbAdd = False
                            End Select
                            If lbAdd Then
                                rows.Controls.Add(New LiteralControl("<th>"))
                                rows.Controls.Add(New LiteralControl(lsCaption))
                                If loitem.Mode = ScreenItem.ItemMode.Mandatory Then
                                    rows.Controls.Add(New MandatoryMarker(lsCaption + " " + GetLabel("req")) With {.ID = "mandmark" & idx})
                                End If
                                rows.Controls.Add(New LiteralControl("</th>"))
                                idx += 1
                            End If
                        End If
                    Next

                    rows.Controls.Add(New LiteralControl("</tr>"))


                End If
                lbFirst = False
            End If

            Dim loObjectForm As New DMObjectForm
            If Not vbRebind Then
                loObjectForm.ID = "objectform" & loObj.ID
                loObjectForm.Labels = lcolLabels
                loObjectForm.CategoryID = loObj.Category
                loObjectForm.ScreenID = llScreenID
                loObjectForm.RenderAsRow = True
                loObjectForm.DisplayItems = lcolItems
                loObjectForm.DisplayMode = liFormMode
                loObjectForm.isTableRecord = False
                loObjectForm.RenderValidatorSummary = False
                loObjectForm.ObjectIdentifier = loObj.ID

                rows.Controls.Add(New LiteralControl("<tr>"))

                rows.Controls.Add(loObjectForm)
                rows.Controls.Add(New LiteralControl("</tr>"))
            Else
                loObjectForm = rows.FindControl("objectform" & loObj.ID)
            End If

            If Not Page.IsPostBack Or vbRebind Then
                loObjectForm.DataBind(loObj)
            End If

        Next
        If Not vbRebind Then
            rows.Controls.Add(New LiteralControl("</table>"))
        End If




        Page.Title = "Multi Edit"


    End Sub


    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSave.Click
        SaveItems(False)
    End Sub

    Private Sub UnLockItems()
        LoadItems()
        For Each item As baseObjects.DM_OBJECT In moItems
            item.UnLock()
        Next
    End Sub
    Private Function SaveItems(ByVal vbUnlockAfter As Boolean) As Boolean
        Dim lbRet As Boolean = False
        If Page.IsValid Then
            Dim lbOk As Boolean
            Dim strMessage As String = ""
            Dim idx As Int32 = 0
            LoadItems()

            For Each c As Control In rows.Controls
                If TypeOf (c) Is DMObjectForm Then

                    lbOk = CType(c, DMObjectForm).Save(moItems.Item(idx), vbUnlockAfter)

                    Dim lsMsg As String = CType(c, DMObjectForm).ErrorMessage
                    If Not String.IsNullOrEmpty(lsMsg) Then
                        strMessage &= lsMsg
                    End If
                    If Not lbOk Then
                        Exit For
                    End If
                    idx += 1
                End If
            Next


            If Not String.IsNullOrEmpty(strMessage) Then
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "AlertHandlerMessage", "alert('" & strMessage & "');", True)
            Else
                lbRet = True
            End If

            'rebind the form, things can change
            If lbRet Then
                BindDetail(True)
            End If

        End If
        Return lbRet

    End Function
    Private Sub Reload()
        Response.Redirect("MultiView.aspx" & QueryStringParser.ToString())
    End Sub
    Private Sub Close()
        Page.ClientScript.RegisterStartupScript(Me.GetType, "Close", "Close();", True)
    End Sub
    Protected Sub tlbMain_ButtonClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadToolBarEventArgs) Handles tlbMain.ButtonClick
        Dim btn As Telerik.Web.UI.RadToolBarButton = CType(e.Item, Telerik.Web.UI.RadToolBarButton)
        Select Case btn.CommandName.ToUpper
            Case "SAVE"
                If SaveItems(False) Then
                    Reload()
                End If
            Case "SAVEANDCLOSE"
                If SaveItems(True) Then
                    Close()
                End If
            Case "CLOSE"
                UnLockItems()
                Close()
        End Select
    End Sub



End Class
