
Imports Arco.Doma.WebControls.SiteManagement
Imports Telerik.Web.UI

Public Class DM_SelectFolder
    Inherits BasePage
    Protected IndexMode As Boolean = False
    Protected ConfirmMove As Boolean = False
    Private loTreeView As Arco.Doma.Treeview.Treeview
    Protected Modal As Boolean = False
    Protected ReloadMainPage As Boolean = False


    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        InitFilterControl()
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Page.Title = GetLabel("selectafolder")

        Dim llGotoID As Integer = QueryStringParser.GetInt("goto")
        Modal = QueryStringParser.GetBoolean("modal")
        IndexMode = QueryStringParser.GetBoolean("IndexMode")
        ConfirmMove = QueryStringParser.GetBoolean("confirm")
        ReloadMainPage = QueryStringParser.GetBoolean("reload")

        loTreeView = New Arco.Doma.Treeview.Treeview(Me)
    

        InitParms()
        loTreeView.SearchWord = GetFilter()    
        loTreeView.RootCaption = GetLabel("folders")
     
        loTreeView.TypeFilter = "Folder,Dossier"
      
        loTreeView.GotoID = llGotoID.ToString

        TreeView.Text = loTreeView.BuildTree

        CType(tlbSearch.Items(1).FindControl("txtFilter"), TextBox).Attributes("placeholder") = GetDecodedLabel("searchfolders")
    End Sub

    Private Sub InitFilterControl()
        Dim loTreeLayout As TreeDefinition = TreeDefinition.GetSelectFolderTreeLayout
        CType(tlbSearch.Items(0), RadToolBarDropDown).Visible = False

        For Each loFilter As TreeDefinition.TreeFilters.Filter In loTreeLayout.TreeFilter.Filters
            Dim lbVisible As Boolean = loFilter.Visible
            Dim lbApply As Boolean = True

            If Not String.IsNullOrEmpty(loFilter.Role) Then
                lbApply = Arco.Security.BusinessIdentity.CurrentIdentity.IsInRole(loFilter.Role)
            End If

            If lbVisible And lbApply Then 'add filter control
                CType(tlbSearch.Items(0), RadToolBarDropDown).Visible = True

                Dim btnFilter As RadToolBarButton = New RadToolBarButton
                btnFilter.AllowSelfUnCheck = True
                btnFilter.Text = loFilter.GetLabel(False)
                btnFilter.PostBack = True
                btnFilter.Value = "filter" & loFilter.Index
                btnFilter.NavigateUrl = "javascript:Filter();"
                btnFilter.CheckOnClick = True
                btnFilter.EnableViewState = True
                btnFilter.ImageUrl = "./Images/xp-ok.gif"

                CType(tlbSearch.Items(0), RadToolBarDropDown).Buttons.Add(btnFilter)
            End If
        Next

    End Sub
    Public Function GetReturnTextField() As String
        Return QueryStringParser.GetClientIdString("textf")
    End Function
    Public Function GetReturnValueField() As String
        Return QueryStringParser.GetClientIdString("valuef")
    End Function
    Private Function GetFilter() As String
        Return CType(tlbSearch.Items(1).FindControl("txtFilter"), TextBox).Text
    End Function
    Private Sub InitParms()

        Dim loTreeLayout As TreeDefinition = TreeDefinition.GetSelectFolderTreeLayout

        loTreeView.TreeFile = loTreeLayout.FileName
        loTreeView.RecursiveACL = (Settings.GetValue("Security", "RecursiveTreeAcl", True) AndAlso loTreeLayout.RecursiveACL)
        loTreeView.RecursiveACLIncludeDocuments = (Settings.GetValue("Security", "RecursiveTreeAclIncludeDocuments", True) AndAlso loTreeLayout.RecursiveACLIncludeDocuments)


        loTreeView.ShowMyFavoriteFolders = Settings.GetValue("Favorites", "Enabled", True) AndAlso loTreeLayout.ShowMyFavoritesFolders
        loTreeView.ShowMyBasketFolders = Settings.GetValue("Basket", "Enabled", True) AndAlso loTreeLayout.ShowMyBasketFolders
        loTreeView.ShowMyDocumentsFolder = Settings.GetValue("MyDocuments", "Enabled", True)

        loTreeView.OrderBy = loTreeLayout.OrderBy
        loTreeView.ShowSearchAsFlatList = Not loTreeLayout.SearchHierarchical
        loTreeView.MaxSearchResults = loTreeLayout.MaxSearchResults
        loTreeView.MinFilterLength = loTreeLayout.MinFilterLength


        loTreeView.FilterAndOr = loTreeLayout.TreeFilter.AndOr

        For Each loFilter As TreeDefinition.TreeFilters.Filter In loTreeLayout.TreeFilter.Filters
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
                    Dim btnFilter As RadToolBarButton = CType(tlbSearch.Items(0), RadToolBarDropDown).Buttons.FindButtonByValue("filter" & loFilter.Index)
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
  
End Class
