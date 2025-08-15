
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Globalisation
Imports Arco.Doma.Library.Search
Imports Arco.Doma.Library.Website
Imports Arco.Doma.WebControls.SiteManagement
Imports Telerik.Web.UI

Namespace Doma

    Partial Class Tree
        Inherits BasePage

        Public Sub New()
            AllowGuestAccess = True
        End Sub

        Dim llParentID As Int32
        Dim llRootID As Int32

        Protected TreeView As Arco.Doma.Treeview.Treeview
        Protected lbDoExpand As Boolean
        Protected ErrMsg As String = ""
        Protected SuccesMsg As String = ""
        Private _showSpecialNodesOnSubRoots As Boolean
        Private _reloadList As Boolean

        Protected filterpanel As System.Web.UI.WebControls.Table
        Protected CurrentItemMode As TreePageDefinition.TreePageItem.eItemType = TreePageDefinition.TreePageItem.eItemType.Tree

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub


        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()

            _treeFile = GetTreeFile()
            If CurrentItemMode = TreePageDefinition.TreePageItem.eItemType.Tree Then
                InitFilterControl()
            End If
        End Sub

#End Region

        Private Function AddXmlExt(ByVal s As String) As String
            If Not s.ToLower.Contains(".xml") Then
                s &= ".xml"
            End If
            Return s
        End Function

        Private _treeFile As String

        Protected Function GetTreeFile() As String
            Dim lsTree As String = QueryStringParser.GetString("tree")
            Dim lsFirstTree As String = ""
            Dim leFirstItemMode As TreePageDefinition.TreePageItem.eItemType
            Dim lbUseDefault As Boolean = False
            If String.IsNullOrEmpty(lsTree) Then

                lbUseDefault = True

            End If
            For Each lnk As TreePageDefinition.TreePageItem In TreePageDefinition.Items
                If lnk.Type <> TreePageDefinition.TreePageItem.eItemType.Link Then
                    Dim lbShow As Boolean = True
                    If Not String.IsNullOrEmpty(lnk.Role) Then
                        lbShow = Arco.Security.BusinessIdentity.CurrentIdentity.IsInRole(lnk.Role)
                    End If
                    If lbShow Then
                        If (lbUseDefault AndAlso lnk.Selected) OrElse (Not String.IsNullOrEmpty(lsTree) AndAlso AddXmlExt(lnk.File) = AddXmlExt(lsTree)) Then
                            lsTree = lnk.File
                            CurrentItemMode = lnk.Type
                            Exit For
                        End If
                        If String.IsNullOrEmpty(lsFirstTree) Then
                            lsFirstTree = lnk.File
                            leFirstItemMode = lnk.Type
                        End If
                    End If
                End If
            Next
            If String.IsNullOrEmpty(lsTree) Then
                lsTree = lsFirstTree
                CurrentItemMode = leFirstItemMode
            End If

            Return AddXmlExt(lsTree)
        End Function

        Private moTreeDef As TreeDefinition

        Protected ReadOnly Property TreeDefinition As TreeDefinition
            Get
                If moTreeDef Is Nothing Then
                    moTreeDef = TreeDefinition.GetTreeLayout(_treeFile)
                End If
                Return moTreeDef
            End Get
        End Property

        Private moTreePageDef As TreePageDefinition

        Protected ReadOnly Property TreePageDefinition As TreePageDefinition
            Get
                If moTreePageDef Is Nothing Then
                    Dim lsTreePage As String = QueryStringParser.GetString("treepage")
                    If String.IsNullOrEmpty(lsTreePage) Then
                        moTreePageDef = TreePageDefinition.GetMainTreePage
                    Else
                        If Not lsTreePage.ToLower.Contains(".xml") Then
                            lsTreePage &= ".xml"
                        End If
                        moTreePageDef = TreePageDefinition.GetTreePageLayout(lsTreePage)
                    End If
                End If
                Return moTreePageDef
            End Get
        End Property

        Private Function GetSelectedId() As Int32

            Dim lsSelID As String = txtSelectedID.Value.Substring(1)
            If lsSelID.IndexOf("_") >= 0 Then
                'it's a property expansion that's been selected
                Dim laSelected() As String = lsSelID.Split("_"c)
                lsSelID = laSelected(0)
            End If

            Return Convert.ToInt32(lsSelID)
        End Function

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
            'Put user code to initialize the page here
            Dim lsSub As String
            Dim lsRootCaption As String
            Dim lsTree As String = ""

            Dim lbGotoRoot As Boolean = QueryStringParser.GetBoolean("gotoroot")

            If CurrentItemMode = TreePageDefinition.TreePageItem.eItemType.Tree Then
                llParentID = QueryStringParser.GetInt("DM_PARENT_ID")


                If String.IsNullOrEmpty(TreeDefinition.Root) Then
                    llRootID = QueryStringParser.GetInt("DM_ROOT_ID")
                Else

                    Dim f As Folder = Folder.GetFolder(TagReplacer.ReplaceTags(TreeDefinition.Root))
                    If f IsNot Nothing Then
                        llRootID = f.ID
                    Else
                        AddToMsg(TreeDefinition.Root & " : " & LibError.GetErrorDescription(LibError.ErrorCode.ERR_INVALIDOBJECT), True)
                        llRootID = 0
                    End If

                End If

                lsRootCaption = GetLabel("folders")
                TreeView = New Arco.Doma.Treeview.Treeview(Me)
                InitParms()

                If llRootID > 0 Then
                    TreeView.Parent = llRootID
                    If llParentID > 0 Then
                        TreeView.GotoID = llParentID.ToString
                    Else
                        TreeView.GotoID = llRootID.ToString
                    End If
                ElseIf llParentID > 0 Then
                    TreeView.Parent = llParentID
                    TreeView.GotoID = llParentID.ToString
                ElseIf Not Page.IsPostBack Then
                    TreeView.GotoID = QueryStringParser.GetString("TREEGOTOID")
                End If

                If llRootID > 0 OrElse llParentID > 0 Then
                    If Not _showSpecialNodesOnSubRoots Then
                        TreeView.ShowMyDocroomNode = False
                        TreeView.ShowListsNode = False
                        TreeView.ShowMyBasketFolders = False
                        TreeView.ShowMyFavoriteFolders = False
                        TreeView.ShowMyDocumentsFolder = False
                    End If
                End If

                radPaneFilter.Visible = True

                If Not String.IsNullOrEmpty(txtSelectedID.Value) Then
                    Dim lsSelID As String = txtSelectedID.Value.Substring(1)
                    If lsSelID.IndexOf("_") >= 0 Then
                        'it's a property expansion that's been selected
                        Dim laSelected() As String = lsSelID.Split("_"c)
                        TreeView.GotoID = laSelected(0)
                    Else
                        TreeView.GotoID = lsSelID
                    End If
                End If


                If TreeDefinition.AdvancedSearchScreen > 0 Then
                    sldZoneAdv.Visible = True
                    'ToolTip="Advanced" CommandName="ToggleAdvanced" ImageUrl="Images/action_down.gif" Position="Right"
                    txtFilter.Buttons.Add(New SearchBoxButton() With {.CommandName = "ToggleAdvanced", .ImageUrl = "Images/action_down.gif", .Position = SearchBoxButtonPosition.Right})

                    ' tlbSearch.Items(3).Visible = True
                    frmAdvanced.datasource = ScreenItemList.GetScreenItems(TreeDefinition.AdvancedSearchScreen)

                    Dim objScreen As Screen = Screen.GetScreen(TreeDefinition.AdvancedSearchScreen)

                    frmAdvanced.CategoryID = objScreen.Category
                    frmAdvanced.ProcedureID = objScreen.Procedure
                    frmAdvanced.FormSize = objScreen.FormSize
                    frmAdvanced.FormFieldsLayout = objScreen.FormFieldsLayout

                    If frmAdvanced.ProcedureID > 0 Then
                        frmAdvanced.Labels = LABELList.GetProcedureItemsLabelList(objScreen.Procedure, Me.EnableIISCaching)
                    Else
                        frmAdvanced.Labels = LABELList.GetCategoryItemsLabelList(objScreen.Category, Me.EnableIISCaching)
                    End If
                Else
                    sldZoneAdv.Visible = False

                    ' tlbSearch.Items(3).Visible = False
                End If
                TreeView.SearchWord = ""


                If IsPostBack Then
                    If TreeDefinition.AdvancedSearchScreen > 0 AndAlso Not String.IsNullOrEmpty(sldZoneAdv.DockedPaneId) Then
                        Dim c As DM_OBJECTSearch.Criteria = DM_OBJECTSearch.Criteria.GetNewGridCriteria()
                        frmAdvanced.ApplyToSearchCriteria(c, Nothing)
                        TreeView.SearchCriteria = c
                    End If
                    TreeView.SearchWord = txtFilter.Text

                End If

                txtGotoID.Value = TreeView.GotoID

                DoActions()
                lsSub = Request("subaction")
                If lsSub = "XP" Then lbDoExpand = True

                TreeView.BuildSelectionList(lbDoExpand)
                TreeView.RootCaption = lsRootCaption
                lsTree = TreeView.BuildTree

                txtFilter.EmptyMessage = GetDecodedLabel("searchfolders")

            Else
                'item is a menu             
                radPaneFilter.Visible = False
            End If
            Dim liCount As Int32 = 0

            Dim liSelectedPane As Int32 = 0
            Dim loPanes As RadPanelBar = Nothing
            For Each lnk As TreePageDefinition.TreePageItem In TreePageDefinition.Items
                Dim lbShow As Boolean = True
                If Not String.IsNullOrEmpty(lnk.Role) Then
                    lbShow = Arco.Security.BusinessIdentity.CurrentIdentity.IsInRole(lnk.Role)
                End If
                If lbShow Then
                    liCount += 1
                End If
            Next

            Dim totalHeight As Int32 = 0

            If liCount > 1 Then

                liCount = 0
                Dim sbOnClientItemClickedScript As New StringBuilder
                sbOnClientItemClickedScript.Append("function OnPanelBarItemClicked(sender, eventArgs){")
                sbOnClientItemClickedScript.Append(" var item = eventArgs.get_item(); ")

                For Each lnk As TreePageDefinition.TreePageItem In TreePageDefinition.Items
                    Dim lbShow As Boolean = True
                    If Not String.IsNullOrEmpty(lnk.Role) Then
                        lbShow = Arco.Security.BusinessIdentity.CurrentIdentity.IsInRole(lnk.Role)
                    End If
                    If lbShow Then
                        If loPanes Is Nothing Then
                            loPanes = New RadPanelBar With {
                                .ID = "pnsTree",
                                .Width = Unit.Percentage(100),
                                .Height = Unit.Percentage(100),
                                .ExpandMode = PanelBarExpandMode.FullExpandedItem,
                                .EnableViewState = False,
                                .BorderStyle = BorderStyle.None,
                                .BorderWidth = 0
                            }
                            loPanes.Style.Add("z-index", "2")

                            loPanes.Style.Add("overflow", "hidden")
                            loPanes.Style.Add("position", "relative")

                            plhTree.Controls.Add(loPanes)
                        End If

                        Dim loPaneItem As New RadPanelItem With {
                            .ImageUrl = lnk.ImageUrl,
                            .Text = lnk.GetLabel(False),
                            .Selected = (_treeFile.Equals(AddXmlExt(lnk.File), StringComparison.CurrentCultureIgnoreCase))
                        }
                        Select Case lnk.Type
                            Case TreePageDefinition.TreePageItem.eItemType.Tree
                                If loPaneItem.Selected Then
                                    loPaneItem.Value = "tree"
                                    loPaneItem.ItemTemplate = New TreePanelTemplate
                                    liSelectedPane = liCount
                                    loPaneItem.Expanded = True
                                Else
                                    QueryStringParser.AddOrReplace("tree", lnk.File)
                                    If lnk.NavigateOnSelect Then
                                        QueryStringParser.Add("gotoroot", "Y")
                                    Else
                                        QueryStringParser.Remove("gotoroot")
                                    End If

                                    loPaneItem.NavigateUrl = "Tree.aspx" & QueryStringParser.ToString
                                    loPaneItem.Expanded = False
                                    loPaneItem.Value = "pnl" & liCount
                                End If
                            Case TreePageDefinition.TreePageItem.eItemType.Menu

                                Dim loMenu As New RadMenu With {
                                    .ClickToOpen = True
                                }
                                LoadMenuItems(loMenu, lnk.File)
                                If Not loMenu.GetAllItems().FirstOrDefault(Function(x) x.Visible) Is Nothing Then
                                    If loPaneItem.Selected Then
                                        loMenu.Flow = ItemFlow.Vertical
                                        loMenu.CssClass = "OverFlowClass"

                                        loMenu.Width = Unit.Percentage(100)

                                        Dim loMenuDiv As New Panel With {
                                            .ID = "treediv",
                                            .ClientIDMode = ClientIDMode.Static
                                        }

                                        loMenuDiv.Controls.Add(loMenu)

                                        loPaneItem.Controls.Add(loMenuDiv)
                                        loPaneItem.Expanded = True 'loPaneItem.Selected

                                    Else
                                        QueryStringParser.AddOrReplace("tree", lnk.File)
                                        loPaneItem.NavigateUrl = "Tree.aspx" & QueryStringParser.ToString
                                        loPaneItem.Expanded = False
                                    End If
                                Else
                                    lbShow = False
                                    loPaneItem.Visible = False
                                End If
                            Case TreePageDefinition.TreePageItem.eItemType.Link
                                loPaneItem.NavigateUrl = lnk.File
                                loPaneItem.Expanded = False

                            Case TreePageDefinition.TreePageItem.eItemType.Item
                                If lnk.SubItems IsNot Nothing Then
                                    For Each subItem In lnk.SubItems
                                        Dim subPaneItem As New RadPanelItem With {
                                            .ImageUrl = subItem.ImageUrl,
                                            .Text = subItem.GetLabel(False)
                                        }
                                        loPaneItem.Items.Add(subPaneItem)

                                        If String.IsNullOrEmpty(subPaneItem.ImageUrl) Then
                                            totalHeight += 20
                                        Else
                                            totalHeight += 24
                                        End If
                                    Next
                                End If
                        End Select

                        loPaneItem.PostBack = False
                        loPanes.Items.Add(loPaneItem)

                        If lbShow Then
                            If String.IsNullOrEmpty(loPaneItem.ImageUrl) Then
                                totalHeight += 22
                            Else
                                totalHeight += 24
                            End If
                            liCount += 1
                        End If
                    End If
                Next

                If liCount > 1 Then
                    sbOnClientItemClickedScript.Append("}")
                End If

            End If


            If liCount = 0 Then

            ElseIf liCount = 1 Then
                If CurrentItemMode = TreePageDefinition.TreePageItem.eItemType.Tree Then
                    If UserProfile.EnableContextMenus Then
                        plhTree.Controls.Add(New Literal With {
                        .Text = "<div oncontextmenu=""return ShowContextMenu(arguments[0]);"">" & lsTree & "</div>"
                    })
                    Else
                        plhTree.Controls.Add(New Literal With {
                     .Text = "<div>" & lsTree & "</div>"})
                    End If

                ElseIf CurrentItemMode = TreePageDefinition.TreePageItem.eItemType.Menu Then
                    Dim loMenu As New RadMenu With {
                    .Flow = ItemFlow.Vertical,
                    .EnableRootItemScroll = True,
                    .Width = Unit.Percentage(100),
                    .CssClass = "OverFlowClass"
                }

                    LoadMenuItems(loMenu, _treeFile)

                    plhTree.Controls.Add(loMenu)

                    For Each mnu As RadMenuItem In loMenu.Items
                        mnu.Width = Unit.Percentage(100)
                    Next
                End If
            Else
                If CurrentItemMode = TreePageDefinition.TreePageItem.eItemType.Tree Then

                    DirectCast(loPanes.Items(liSelectedPane).FindControl("TreeView"), Literal).Text = lsTree

                    radPaneContent.OnClientResized = "ResizePanelBar"
                    radSplitterMain.OnClientLoad = "ResizePanelBar"

                    ClientScript.RegisterClientScriptBlock(Me.GetType, "resizepanelbar", "function ResizePanelBar(){$get('treediv').style.height = ($find('" & radSplitterMain.ClientID & "').GetPaneById('" & radPaneContent.ClientID & "').get_height() - " & totalHeight & ") + 'px';}", True)
                End If

            End If

            Dim sb As New StringBuilder
            If lbGotoRoot AndAlso TreeView IsNot Nothing Then
                sb.AppendLine("SetSelectedId('f" & TreeView.Parent & "');")
                sb.AppendLine("DMC('" & TreeView.DefaultRootAction & "');")
            End If
            sb.AppendLine("function SetFocus(){")
            If TreeView IsNot Nothing AndAlso Not String.IsNullOrEmpty(TreeView.FocusField) Then ' AndAlso Not lbDoExpand AndAlso Not IsPostBack Then
                sb.AppendLine(" try{")
                'src: https://stackoverflow.com/questions/5685589/scroll-to-element-only-if-not-in-view-jquery
                sb.AppendLine("const treeView = $get('" & TreeView.FocusField & "');")
                sb.AppendLine("if (treeView.offset) { if (treeView.offset().top < window.scrollTop()) { $get('html,body').animate({scrollTop: treeView.offset().top}); } ")
                sb.AppendLine("else if (treeView.offset().top + $target.height() > window.scrollTop() + (window.innerHeight || document.documentElement.clientHeight)) {")
                sb.AppendLine(" $('html,body').animate({scrollTop: treeView.offset().top - (window.innerHeight || document.documentElement.clientHeight) + $target.height() + 15}); }")
                sb.AppendLine("}")

                'sb.AppendLine("$get('" & TreeView.FocusField & "').scrollIntoView()")
                sb.AppendLine("}")
                sb.AppendLine("catch(e)")
                sb.AppendLine("{")
                sb.AppendLine("}")
            End If
            sb.AppendLine("}")
            ClientScript.RegisterStartupScript(Me.GetType, "SetFocus", sb.ToString, True)
        End Sub

        Private Class TreePanelTemplate
            Implements ITemplate

            Public Sub InstantiateIn(container As Control) Implements ITemplate.InstantiateIn
                container.Controls.Add(New LiteralControl("<div id=""treediv"" oncontextmenu=""return ShowContextMenu(arguments[0]);"" style=""clear:both;overflow:auto"">"))
                Dim loLit As Literal = New Literal
                loLit.ID = "TreeView"
                loLit.EnableViewState = False
                container.Controls.Add(loLit)
                container.Controls.Add(New LiteralControl("</div>"))
            End Sub
        End Class

        Private Sub InitFilterControl()

            'CType(tlbSearch.Items(0), RadToolBarDropDown).Visible = False
            For Each loFilter As SiteFilter.Filter In TreeDefinition.TreeFilter.Filters
                Dim lbVisible As Boolean = loFilter.Visible
                Dim lbApply As Boolean = String.IsNullOrEmpty(loFilter.Role) OrElse Arco.Security.BusinessIdentity.CurrentIdentity.IsInRole(loFilter.Role)

                If lbVisible And lbApply Then 'add filter control
                    txtFilter.Localization.DefaultItemText = GetLabel("all")
                    'CType(tlbSearch.Items(0), RadToolBarDropDown).Visible = True

                    'Dim btnFilter As RadToolBarButton = New RadToolBarButton
                    'btnFilter.AllowSelfUnCheck = True
                    'btnFilter.Text = loFilter.GetLabel(False)
                    'btnFilter.PostBack = True
                    'btnFilter.Value = "filter" & loFilter.Index
                    'btnFilter.NavigateUrl = "javascript:Filter();"
                    'btnFilter.CheckOnClick = True
                    'btnFilter.EnableViewState = True
                    'btnFilter.ImageUrl = "./Images/xp-ok.gif"

                    'CType(tlbSearch.Items(0), RadToolBarDropDown).Buttons.Add(btnFilter)

                    txtFilter.SearchContext.Items.Add(New SearchContextItem(loFilter.GetLabel(False), "filter" & loFilter.Index))
                End If
            Next

        End Sub
        Private Function GetDefaultRootAction() As String
            Dim value As String = Helpers.TreeActionValueConverter.Convert(Settings.GetValue("DOMA", "RootAction", ""))
            Select Case value.ToLower
                Case "browsewithsubfolders", "search", "advsearch", "page"
                    Return value
                Case "query"
                    Return "Search"
                Case Else
                    Return "Browse"
            End Select
        End Function
        Private Sub InitParms()
            Dim authenticated As Boolean = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity().IsAuthenticated

            TreeView.TreeFile = TreeDefinition.FileName


            TreeView.RecursiveACL = (Settings.GetValue("Security", "RecursiveTreeAcl", True) AndAlso TreeDefinition.RecursiveACL)
            TreeView.RecursiveACLIncludeDocuments = (Settings.GetValue("Security", "RecursiveTreeAclIncludeDocuments", True) AndAlso TreeDefinition.RecursiveACLIncludeDocuments)
            TreeView.DragDrop = TreeDefinition.DragDrop
            TreeView.DefaultRootAction = GetDefaultRootAction()
            TreeView.OrderBy = TreeDefinition.OrderBy
            If Not IsPostBack Then
                TreeView.InitialCollapseRootnode = TreeDefinition.InitialCollapseRootNode
            End If
            TreeView.InitialCollapseMyDocroomNode = TreeDefinition.InitialCollapseMyDocroomNode
            If authenticated Then
                TreeView.ShowMyDocroomNode = TreeDefinition.ShowMyDocroomNode
                TreeView.ShowListsNode = Not TreeDefinition.HiddenSpecialNodes.Contains("lists") 'hack for bw compat
                TreeView.ShowMyFavoriteFolders = TreeDefinition.ShowMyFavoritesFolders AndAlso Settings.GetValue("Favorites", "Enabled", True)
                TreeView.ShowMyBasketFolders = TreeDefinition.ShowMyBasketFolders AndAlso Settings.GetValue("Basket", "Enabled", True)
                TreeView.ShowMyDocumentsFolder = Settings.GetValue("MyDocuments", "Enabled", True)
                TreeView.ShowMailFolders = Settings.GetValue("MailClient", "Enabled", True)
                TreeView.HiddenMyDocroomNodes = TreeDefinition.HiddenSpecialNodes
            End If


            _showSpecialNodesOnSubRoots = TreeDefinition.ShowSpecialNodesOnSubRoots
            TreeView.ShowSearchAsFlatList = Not TreeDefinition.SearchHierarchical
            TreeView.MaxSearchResults = TreeDefinition.MaxSearchResults
            TreeView.TypeFilter = TreeDefinition.TypeFilter
            TreeView.HideRootFolder = TreeDefinition.HideRoot
            TreeView.ShowPackages = True
            TreeView.MinFilterLength = TreeDefinition.MinFilterLength

            TreeView.FilterAndOr = TreeDefinition.TreeFilter.AndOr
            For Each loFilter As SiteFilter.Filter In TreeDefinition.TreeFilter.Filters
                Dim lbApply As Boolean = String.IsNullOrEmpty(loFilter.Role) OrElse Arco.Security.BusinessIdentity.CurrentIdentity.IsInRole(loFilter.Role)
                Dim lbVisible As Boolean = loFilter.Visible
                Dim lbEnabled As Boolean = loFilter.Enabled

                If Not lbApply AndAlso loFilter.reversefilterwhennotinrole Then
                    'reverse when not in role                   
                    lbEnabled = Not lbEnabled
                    lbApply = True
                End If

                If lbApply Then

                    Dim lbChecked As Boolean = False
                    If lbVisible Then 'add filter control

                        'Dim btnFilter As RadToolBarButton = CType(tlbSearch.Items(0), RadToolBarDropDown).Buttons.FindButtonByValue("filter" & loFilter.Index)
                        If Not IsPostBack AndAlso lbEnabled Then
                            For Each item As SearchContextItem In txtFilter.SearchContext.Items
                                If item.Key = "filter" & loFilter.Index Then
                                    item.Selected = True
                                    Exit For
                                End If
                            Next
                            ' btnFilter.Checked = true
                        End If
                        lbChecked = (txtFilter.SearchContext.SelectedItem IsNot Nothing AndAlso txtFilter.SearchContext.SelectedItem.Key = "filter" & loFilter.Index)
                        'If Not lbChecked Then
                        '    btnFilter.ImageUrl = ""
                        'End If
                    End If

                    If Not IsPostBack Then
                        If lbEnabled Then
                            TreeView.AddFilter(CType(loFilter.Index, Int32), loFilter.NormalFilter, loFilter.Recursive)
                        Else
                            If Not String.IsNullOrEmpty(loFilter.ReversedFilter) Then
                                TreeView.AddDisabledFilter(CType(loFilter.Index, Int32), loFilter.ReversedFilter, loFilter.Recursive)
                            End If
                        End If
                    Else
                        If lbVisible Then
                            lbEnabled = lbChecked
                        End If
                        If lbEnabled Then
                            TreeView.AddFilter(CType(loFilter.Index, Int32), loFilter.NormalFilter, loFilter.Recursive)
                        Else
                            If Not String.IsNullOrEmpty(loFilter.ReversedFilter) Then
                                TreeView.AddDisabledFilter(CType(loFilter.Index, Int32), loFilter.ReversedFilter, loFilter.Recursive)
                            End If
                        End If
                    End If
                End If
            Next

        End Sub

        Private Sub PasteInSelection(ByVal selType As DMSelection.SelectionType)
            If IsRootNodeID(txtCutID.Value) Then
                Return
            End If

            Dim laCutIDs() As String = Split(txtCutID.Value, ",", -1, CompareMethod.Text)
            For i As Integer = laCutIDs.GetLowerBound(0) To laCutIDs.GetUpperBound(0)
                Dim cutId As String = laCutIDs(i)
                If Not String.IsNullOrEmpty(cutId) Then
                    If cutId.StartsWith("f") Then
                        cutId = GetObjectIDFromNodeID(cutId)
                        If IsNumeric(cutId) Then
                            Dim pasteID As Int32
                            Try
                                pasteID = Convert.ToInt32(cutId)
                            Catch ex As Exception
                                pasteID = 0
                            End Try

                            If pasteID > 0 Then
                                Selection.AddToSelection(selType, ObjectRepository.GetObject(pasteID))
                            End If
                        End If
                    Else
                        AddToMsg(LibError.GetErrorDescription(LibError.ErrorCode.ERR_INVALIDOPERATIONONOBJECTTYPE), True)
                    End If
                End If
            Next
            txtCutID.Value = ""

        End Sub

        Private Sub AddToMsg(ByVal vsText As String, ByVal vbError As Boolean)
            If vbError Then
                If Not String.IsNullOrEmpty(vsText) Then
                    If Not String.IsNullOrEmpty(ErrMsg) Then
                        ErrMsg &= Environment.NewLine
                    End If
                    ErrMsg &= vsText
                End If
            Else
                If Not String.IsNullOrEmpty(vsText) Then
                    If Not String.IsNullOrEmpty(SuccesMsg) Then
                        SuccesMsg &= Environment.NewLine
                    End If
                    SuccesMsg &= vsText
                End If
            End If

        End Sub
        Private Sub DoActions()
            Dim lsSubAction As String
            Dim llSelectedID As Int32
            Dim lbRights As Boolean
            Dim lsSubMSG As String = ""
            Dim llPasteID As Int32

            lsSubAction = Request.Form("subaction")

            Select Case lsSubAction
                Case "D" 'delete
                    Dim lsSelectedID As String = GetObjectIDFromNodeID(txtSelectedID.Value)
                    If Not lsSelectedID.Contains("_") Then
                        llSelectedID = Convert.ToInt32(lsSelectedID)
                    Else
                        Dim laSelected() As String = lsSelectedID.Split("_"c)
                        llSelectedID = Convert.ToInt32(laSelected(0))
                    End If

                    lbRights = True

                    Dim loObject As DM_OBJECT
                    If llSelectedID > 0 Then


                        Dim loLinkCrit As New OBJECT_LINK.Criteria(0, llSelectedID) With {
                            .DIRECT = 1
                        }

                        Dim loParentLink As OBJECT_LINK = OBJECT_LINK.GetOBJECT_LINK(loLinkCrit)
                        txtGotoID.Value = loParentLink.PARENT_OBJECT_ID.ToString

                    Else
                        lbRights = False
                        lsSubMSG = GetDecodedLabel("NoFolderSelected")
                    End If

                    If lbRights Then

                        loObject = ObjectRepository.GetObject(llSelectedID)
                        Dim llParentID As Int32 = loObject.Parent_ID

                        If loObject.Delete() Then
                            TreeView.GotoID = llParentID.ToString 'goto the parentid after a delete  
                        Else
                            lsSubMSG = loObject.GetLastError.Description
                            lbRights = False
                        End If

                    End If


                    If Not lbRights Then
                        AddToMsg(GetDecodedLabel("accessdenied"), True)
                        AddToMsg(lsSubMSG, True)

                    Else
                        AddToMsg(GetDecodedLabel("deleteok"), False)
                    End If
                Case "PBSK"
                    PasteInSelection(DMSelection.SelectionType.Basket)
                Case "PFAV"
                    PasteInSelection(DMSelection.SelectionType.Favorites)
                Case "PS" 'paste shortcut
                    If Not IsRootNodeID(txtCutID.Value) Then
                        Dim laCutIDs() As String
                        Dim liCutCounter As Int32

                        llSelectedID = GetSelectedId()
                        laCutIDs = Split(txtCutID.Value, ",", -1, CompareMethod.Text)
                        For liCutCounter = laCutIDs.GetLowerBound(0) To laCutIDs.GetUpperBound(0)
                            Dim cutId As String = laCutIDs(liCutCounter)
                            If Not String.IsNullOrEmpty(cutId) Then
                                cutId = GetObjectIDFromNodeID(cutId)
                                If IsNumeric(cutId) Then
                                    Try
                                        llPasteID = Convert.ToInt32(cutId)
                                    Catch ex As Exception
                                        llPasteID = llSelectedID
                                    End Try

                                    If llPasteID <> llSelectedID Then
                                        Dim loLinkCrit As OBJECT_LINK.Criteria = New OBJECT_LINK.Criteria(llPasteID, llSelectedID)
                                        Dim loParentLink As OBJECT_LINK = OBJECT_LINK.GetOBJECT_LINK(loLinkCrit)
                                        If loParentLink.CHILD_OBJECT_ID > 0 Then
                                            'illegal move
                                            llPasteID = llSelectedID
                                        End If
                                    End If

                                    If llPasteID <> llSelectedID Then
                                        Dim loObj As DM_OBJECT = ObjectRepository.GetObject(llPasteID)

                                        Dim s As Shortcut = loObj.CreateShortcut(llSelectedID)

                                        If s.ID = 0 Then
                                            Select Case s.GetLastError.Code
                                                Case LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS
                                                    lsSubMSG = GetDecodedLabel("Notenoughrights")
                                                Case Else
                                                    lsSubMSG = "Unexpected error"
                                            End Select

                                            AddToMsg(GetDecodedLabel("accessdenied"), True)
                                            AddToMsg(lsSubMSG, True)
                                        End If
                                    End If
                                End If
                            End If
                        Next
                        txtCutID.Value = ""

                    End If

                Case "PQ" 'create a Query Link in the tree                   
                    If Not IsRootNodeID(txtCutID.Value) Then
                        Dim laCutIDs() As String
                        Dim liCutCounter As Int32
                        If txtSelectedID.Value.Substring(0, 1) = "f" Then
                            llSelectedID = GetSelectedId()
                            laCutIDs = Split(txtCutID.Value, ",", -1, CompareMethod.Text)
                            For liCutCounter = laCutIDs.GetLowerBound(0) To laCutIDs.GetUpperBound(0)
                                Dim cutId As String = laCutIDs(liCutCounter)
                                If Not String.IsNullOrEmpty(cutId) Then
                                    cutId = GetObjectIDFromNodeID(cutId)
                                    If IsNumeric(cutId) Then
                                        Try
                                            llPasteID = Convert.ToInt32(cutId)
                                        Catch ex As Exception
                                            llPasteID = 0
                                        End Try

                                        If llPasteID > 0 Then

                                            Dim loQuery As DMQuery = DMQuery.GetQuery(llPasteID)
                                            If loQuery.ID > 0 Then
                                                Dim loQryyShortcut As QueryShortcut = loQuery.CreateShortCut(llSelectedID, True)
                                                If loQryyShortcut.ID = 0 Then
                                                    AddToMsg(loQryyShortcut.GetLastError.Description, True)
                                                End If
                                            Else
                                                AddToMsg("Query not found", True)
                                            End If

                                        End If
                                    End If

                                End If
                            Next
                        Else
                            AddToMsg(LibError.GetErrorDescription(LibError.ErrorCode.ERR_INVALIDOPERATIONONOBJECTTYPE), True)
                        End If
                        txtCutID.Value = ""
                    End If
                Case "PIP" 'paste into package                   
                    If Not IsRootNodeID(txtCutID.Value) Then
                        Dim laCutIDs() As String
                        Dim liCutCounter As Int32
                        Dim laValues() As String = Mid(txtSelectedID.Value, 2).Split("_"c)
                        llSelectedID = Convert.ToInt32(laValues(0))

                        Dim llPackID As Int32 = Convert.ToInt32(laValues(1))
                        laCutIDs = Split(txtCutID.Value, ",", -1, CompareMethod.Text)

                        For liCutCounter = laCutIDs.GetLowerBound(0) To laCutIDs.GetUpperBound(0)
                            Dim cutId As String = laCutIDs(liCutCounter)
                            If Not String.IsNullOrEmpty(cutId) Then
                                cutId = GetObjectIDFromNodeID(cutId)
                                If IsNumeric(cutId) Then
                                    Try
                                        llPasteID = Convert.ToInt32(cutId)
                                    Catch ex As Exception
                                        llPasteID = 0
                                    End Try

                                    If llPasteID > 0 AndAlso llSelectedID > 0 AndAlso llPackID > 0 Then
                                        Dim loObjTarget As DM_OBJECT = ObjectRepository.GetObject(llSelectedID)
                                        Dim loObjSource As DM_OBJECT = ObjectRepository.GetObject(llPasteID)

                                        If loObjTarget.CanAddToPackage(llPackID, loObjSource) Then
                                            loObjTarget.AddToPackage(llPackID, loObjSource)
                                            AddToMsg(GetDecodedLabel("addpackagedone"), False)
                                        Else
                                            AddToMsg(loObjTarget.GetLastError.Description, True)
                                        End If

                                    End If
                                End If
                            End If
                        Next

                        txtCutID.Value = ""

                    End If
                Case "P", "PC"  'paste or paste copy
                    If Not IsRootNodeID(txtCutID.Value) Then
                        Dim laCutIDs() As String
                        Dim liCutCounter As Int32

                        llSelectedID = GetSelectedId()
                        laCutIDs = Split(txtCutID.Value, ",", -1, CompareMethod.Text)
                        For liCutCounter = laCutIDs.GetLowerBound(0) To laCutIDs.GetUpperBound(0)
                            Dim cutId As String = laCutIDs(liCutCounter)
                            If Not String.IsNullOrEmpty(cutId) Then
                                cutId = GetObjectIDFromNodeID(cutId)
                                Try
                                    llPasteID = Convert.ToInt32(cutId)
                                Catch ex As Exception
                                    llPasteID = llSelectedID
                                End Try

                                If llSelectedID > 0 Then
                                    If llPasteID <> llSelectedID Then

                                        Dim loLinkCrit As OBJECT_LINK.Criteria = New OBJECT_LINK.Criteria(llPasteID, llSelectedID)
                                        Dim loParentLink As OBJECT_LINK = OBJECT_LINK.GetOBJECT_LINK(loLinkCrit)
                                        If loParentLink.CHILD_OBJECT_ID > 0 Then
                                            'illegal move
                                            llPasteID = llSelectedID
                                        End If
                                    End If
                                End If

                                If llPasteID <> llSelectedID Then

                                    Dim loSource As DM_OBJECT = ObjectRepository.GetObject(llPasteID)
                                    If lsSubAction = "P" Then
                                        loSource = loSource.Move(llSelectedID)
                                    Else
                                        loSource = loSource.Copy(llSelectedID)
                                    End If
                                    If loSource.HasError Then
                                        AddToMsg(loSource.GetLastError.Description, True)
                                    Else
                                        If lsSubAction = "P" Then
                                            AddToMsg(GetDecodedLabel("moveok"), False)
                                            _reloadList = True
                                        Else
                                            AddToMsg(GetDecodedLabel("copyok"), False)
                                        End If

                                    End If
                                Else
                                    AddToMsg(LibError.GetErrorDescription(LibError.ErrorCode.ERR_INVALIDOPERATION), True)
                                    'illegal move
                                End If
                            End If
                        Next

                        txtCutID.Value = ""
                    End If
            End Select

        End Sub
        Private Function IsRootNodeID(ByVal nodeId As String) As Boolean
            Return String.IsNullOrEmpty(nodeId) OrElse nodeId = "0" OrElse nodeId = "f0"
        End Function
        Private Function GetObjectIDFromNodeID(ByVal nodeId As String) As String
            Dim firstChar As String = nodeId.Substring(0, 1)
            If firstChar = "f" OrElse firstChar = "q" Then
                Return nodeId.Substring(1)
            End If

            Return nodeId
        End Function

        Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
            Dim sbAlert As New StringBuilder
            If Not String.IsNullOrEmpty(ErrMsg) Then

                sbAlert.Append(" PC().ShowError(")
                sbAlert.Append(EncodingUtils.EncodeJsString(ErrMsg))
                sbAlert.Append(" );")
            End If
            If Not String.IsNullOrEmpty(SuccesMsg) Then

                sbAlert.Append(" PC().ShowSuccess(")
                sbAlert.Append(EncodingUtils.EncodeJsString(SuccesMsg))
                sbAlert.Append(" );")
            End If
            If _reloadList Then
                sbAlert.Append(" ReloadList();")
            End If
            If sbAlert.Length > 0 Then ClientScript.RegisterStartupScript(Me.GetType, "alerts", sbAlert.ToString, True)
        End Sub

    End Class

End Namespace
