Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Telerik.Web.UI

Partial Class DM_CopyMoveToFolder
    Inherits BasePage
    Private loTreeView As Arco.Doma.Treeview.Treeview

    Protected Modal As Boolean
    Private Const FilterBox As Int32 = 0
    Private Const FilterDropDown As Int32 = 2


    Private Sub DocAction(ByVal action As String)
        If String.IsNullOrEmpty(txtSelectedID.Value) Then
            Exit Sub
        End If

        Dim currentSelection As DMSelectionList = DMSelectionList.GetCurrentSelection()

        Dim loParentLink As OBJECT_LINK
        Dim loLinkCrit As OBJECT_LINK.Criteria
        Dim lbError As Boolean = True

        Dim llSelectedID As Int32 = Convert.ToInt32(Mid(txtSelectedID.Value, 2))

        If llSelectedID >= 0 Then
            If currentSelection.Any Then

                For Each loSelInfo As DMSelectionList.SelectionInfo In currentSelection

                    If llSelectedID > 0 AndAlso action <> "Shortcut" Then
                        loLinkCrit = New OBJECT_LINK.Criteria(loSelInfo.Object_ID, llSelectedID)
                        loParentLink = OBJECT_LINK.GetOBJECT_LINK(loLinkCrit)
                        If loParentLink.CHILD_OBJECT_ID > 0 Then
                            'illegal move
                            loSelInfo.Object_ID = llSelectedID
                        End If
                    End If

                    If loSelInfo.Object_ID <> llSelectedID Then
                        If action = "Shortcut" Then
                            Dim loObj As DM_OBJECT = loSelInfo.GetBusinessObject

                            If loObj IsNot Nothing AndAlso loObj.ID > 0 Then
                                Dim loSC As Shortcut = loObj.CreateShortcut(llSelectedID)
                                If loSC.ID > 0 Then
                                    AddToMessage(GetLabel("shortcutcreatedto") & " " & loObj.Name)
                                    lbError = False
                                Else
                                    AddToMessage(loSC.GetLastError.Description) ' GetLabel("illegalshortcut")
                                End If
                            Else
                                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                            End If
                        ElseIf action = "Merge" Then
                            Dim f As DM_OBJECT = loSelInfo.GetBusinessObject
                            If f IsNot Nothing AndAlso f.ID > 0 And f.Status <> DM_OBJECT.Object_Status.Deleted Then
                                If f.MoveChildren(llSelectedID) Then
                                    'all children have been moved
                                    f.Delete()
                                    AddToMessage(f.Name & " " & GetSuccessMessage(action))
                                Else
                                    AddToMessage(f.Name & " " & GetFailedMessage(action))
                                End If
                            Else
                                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                            End If
                        Else
                            Dim f As DM_OBJECT = loSelInfo.GetBusinessObject
                            If f IsNot Nothing AndAlso f.ID > 0 And f.Status <> DM_OBJECT.Object_Status.Deleted Then
                                Dim loTarget As DM_OBJECT = ObjectRepository.GetObject(llSelectedID)
                                lbError = False
                                Select Case action
                                    Case "Move"
                                        f = f.Move(loTarget)
                                    Case Else
                                        f = f.Copy(loTarget)
                                End Select

                                If Not f.HasError Then
                                    AddToMessage(f.Name & " " & GetSuccessMessage(action))
                                Else
                                    lbError = True
                                    Dim lsSubMSG As String = f.GetLastError.Description
                                    AddToMessage(f.Name & " " & GetFailedMessage(action))

                                    If Not String.IsNullOrEmpty(lsSubMSG) Then
                                        lblMessage.Text &= " : " & lsSubMSG
                                    End If
                                End If
                            Else
                                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                            End If
                        End If
                    Else
                        lblMessage.Text &= LibError.GetErrorDescription(LibError.ErrorCode.ERR_INVALIDOPERATION)
                    End If

                    pnlSelection.Visible = False
                    pnlMsg.Visible = True

                Next

                Selection.ClearCurrentSelection()

            Else
                ShowNothingSelectedMessage()
            End If
        Else
            'nothing selected yet
        End If

        If lbError Then
            lblMessage.CssClass = "ErrorMessage"
        Else
            lblMessage.CssClass = "InfoMessage"
        End If
        plhReloadPanel.Visible = Not lbError


    End Sub

    Private Function GetSuccessMessage(ByVal action As String) As String
        Select Case action
            Case "Move"
                Return GetLabel("moved")
            Case "Merge"
                Return GetLabel("merged")
            Case Else
                Return GetLabel("copied")
        End Select
    End Function
    Private Function GetFailedMessage(ByVal action As String) As String
        Select Case action
            Case "Move"
                Return GetLabel("couldnotbemoved")
            Case "Merge"
                Return GetLabel("couldnotbemerged")
            Case Else
                Return GetLabel("couldnotbecopied")
        End Select
    End Function
    Private Sub AddToMessage(ByVal t As String)
        If Not String.IsNullOrEmpty(lblMessage.Text) Then
            lblMessage.Text &= "<br>"
        End If
        lblMessage.Text &= t

    End Sub


    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        InitFilterControl()
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Modal = QueryStringParser.GetBoolean("modal")

        If Not IsPostBack Then
            lnkCopy.Text = GetLabel("copy")
            lnkMove.Text = GetLabel("move")
            lnkShortcut.Text = GetLabel("createshortcut")
            lnkMerge.Text = GetLabel("merge")
        End If

        Dim sourceObject As DM_OBJECT = Nothing
        Dim llID As Int32 = QueryStringParser.GetInt("DM_OBJECT_ID")
        If llID > 0 Then

            sourceObject = ObjectRepository.GetObject(llID)
            If sourceObject Is Nothing OrElse Not sourceObject.CanViewMeta Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                Exit Sub
            End If

            If Not IsPostBack Then
                Selection.ClearCurrentSelection()
                Selection.AddToCurrentSelection(sourceObject)
            End If
        Else
            Dim currentSelection As DMSelectionList = DMSelectionList.GetCurrentSelection()
            If currentSelection.Count = 0 Then
                ShowNothingSelectedMessage()
                Exit Sub
            ElseIf currentSelection.Count = 1 Then
                sourceObject = ObjectRepository.GetObject(currentSelection(0).Object_ID)
            End If
        End If
        If sourceObject IsNot Nothing Then

            lnkMerge.Visible = sourceObject.HasChildren
            lnkMove.Visible = sourceObject.CanMove

            Page.Title = sourceObject.Name & ": " & GetLabel("ctx_copymovetofolder")
        End If

        LoadTree()

    End Sub

    Private Sub ShowNothingSelectedMessage()
        lblMessage.Text = "Nothing selected"
        pnlSelection.Visible = False
        pnlMsg.Visible = True
    End Sub

    Private Sub LoadTree()
        Dim llGotoID As Integer = QueryStringParser.GetInt("goto")

        loTreeView = New Arco.Doma.Treeview.Treeview(Me)


        InitParms()
        loTreeView.SearchWord = GetFilter()
        loTreeView.RootCaption = GetLabel("folders")

        loTreeView.TypeFilter = "Folder"
        loTreeView.ShowQueries = False

        loTreeView.Parent = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.RootFolder

        If llGotoID > 0 AndAlso Not Page.IsPostBack Then
            loTreeView.GotoID = llGotoID.ToString
            loTreeView.BuildSelectionList(False)
        End If

        loTreeView.ShowPackages = False

        TreeView.Text = loTreeView.BuildTree

        CType(tlbSearch.Items(FilterBox).FindControl("txtFilter"), WebControl).Attributes("placeholder") = GetDecodedLabel("searchfolders")
    End Sub

    Private Sub InitFilterControl()
        Dim loTreeLayout As SiteManagement.TreeDefinition = SiteManagement.TreeDefinition.GetSelectFolderTreeLayout
        CType(tlbSearch.Items(FilterDropDown), RadToolBarDropDown).Visible = False

        For Each loFilter As SiteManagement.TreeDefinition.TreeFilters.Filter In loTreeLayout.TreeFilter.Filters
            Dim lbVisible As Boolean = loFilter.Visible
            Dim lbApply As Boolean = True

            If Not String.IsNullOrEmpty(loFilter.Role) Then
                lbApply = Arco.Security.BusinessIdentity.CurrentIdentity.IsInRole(loFilter.Role)
            End If

            If lbVisible And lbApply Then 'add filter control
                CType(tlbSearch.Items(FilterDropDown), RadToolBarDropDown).Visible = True

                Dim btnFilter As New RadToolBarButton With {
                    .AllowSelfUnCheck = True,
                    .Text = loFilter.GetLabel(False),
                    .PostBack = True,
                    .Value = "filter" & loFilter.Index,
                    .NavigateUrl = "javascript:Filter();",
                    .CheckOnClick = True,
                    .EnableViewState = True,
                    .ImageUrl = "./Images/xp-ok.gif"
                }

                CType(tlbSearch.Items(FilterDropDown), RadToolBarDropDown).Buttons.Add(btnFilter)
            End If
        Next

    End Sub
    Private Function GetFilter() As String
        Return CType(tlbSearch.Items(FilterBox).FindControl("txtFilter"), TextBox).Text
    End Function
    Private Sub InitParms()

        Dim loTreeLayout As SiteManagement.TreeDefinition = SiteManagement.TreeDefinition.GetSelectFolderTreeLayout
        loTreeView.TreeFile = loTreeLayout.FileName
        loTreeView.RecursiveACL = (Settings.GetValue("Security", "RecursiveTreeAcl", True) AndAlso loTreeLayout.RecursiveACL)
        loTreeView.RecursiveACLIncludeDocuments = (Settings.GetValue("Security", "RecursiveTreeAclIncludeDocuments", True) AndAlso loTreeLayout.RecursiveACLIncludeDocuments)
        If Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsAuthenticated Then
            loTreeView.ShowMyDocumentsFolder = Settings.GetValue("MyDocuments", "Enabled", True)
            loTreeView.ShowMyFavoriteFolders = Settings.GetValue("Favorites", "Enabled", True)
            loTreeView.ShowMyBasketFolders = Settings.GetValue("Basket", "Enabled", True)
        End If

        loTreeView.OrderBy = loTreeLayout.OrderBy

        loTreeView.ShowSearchAsFlatList = Not loTreeLayout.SearchHierarchical
        loTreeView.MaxSearchResults = loTreeLayout.MaxSearchResults
        loTreeView.MinFilterLength = loTreeLayout.MinFilterLength

        For Each loFilter As SiteManagement.TreeDefinition.TreeFilters.Filter In loTreeLayout.TreeFilter.Filters
            Dim lbApply As Boolean = True
            Dim lbVisible As Boolean = loFilter.Visible
            Dim lbEnabled As Boolean = loFilter.Enabled

            If Not String.IsNullOrEmpty(loFilter.Role) Then
                lbApply = Arco.Security.BusinessIdentity.CurrentIdentity.IsInRole(loFilter.Role)
                If Not lbApply Then
                    'reverse when not in role 
                    If loFilter.reversefilterwhennotinrole Then
                        lbVisible = False
                        lbEnabled = False
                        lbApply = True
                    End If

                End If
            End If

            If lbApply Then
                Dim lbChecked As Boolean = False
                If lbVisible Then 'add filter control
                    Dim btnFilter As RadToolBarButton = CType(tlbSearch.Items(FilterDropDown), RadToolBarDropDown).Buttons.FindButtonByValue("filter" & loFilter.Index)
                    'Dim chk As CheckBox = CType(pnlFilters.FindControl("filter" & loFilter.Index), CheckBox)
                    If Not Page.IsPostBack Then
                        btnFilter.Checked = lbEnabled
                    End If
                    lbChecked = btnFilter.Checked
                    If Not lbChecked Then
                        btnFilter.ImageUrl = ""
                    End If
                End If

                If Not Page.IsPostBack Then
                    If lbEnabled Then
                        loTreeView.AddFilter(CType(loFilter.Index, Int32), loFilter.NormalFilter, loFilter.Recursive)
                    Else
                        If loFilter.ReversedFilter <> "" Then
                            loTreeView.AddDisabledFilter(CType(loFilter.Index, Int32), loFilter.ReversedFilter, loFilter.Recursive)
                        End If
                    End If
                Else
                    If lbVisible Then
                        lbEnabled = lbChecked
                    End If
                    If lbEnabled Then
                        loTreeView.AddFilter(CType(loFilter.Index, Int32), loFilter.NormalFilter, loFilter.Recursive)
                    Else
                        If loFilter.ReversedFilter <> "" Then
                            loTreeView.AddDisabledFilter(CType(loFilter.Index, Int32), loFilter.ReversedFilter, loFilter.Recursive)
                        End If
                    End If
                End If
            End If
        Next

    End Sub

    Protected Sub tlbSearch_ButtonClick(ByVal sender As Object, ByVal e As RadToolBarEventArgs) Handles tlbSearch.ButtonClick
        Select Case CType(e.Item, RadToolBarButton).CommandName
            Case "COPY"
                DocAction("Copy")
            Case "MOVE"
                DocAction("Move")
            Case "SHORTCUT"
                DocAction("Shortcut")
            Case "MERGE"
                DocAction("Merge")
        End Select
    End Sub
End Class
