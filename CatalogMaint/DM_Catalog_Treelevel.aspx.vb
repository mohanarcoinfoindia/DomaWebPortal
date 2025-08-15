Imports obout_ASPTreeView_2_NET
Imports Arco.Doma.Library

Partial Class DM_Catalog_Treelevel
    Inherits BasePage

    Private moList As Lists.List = Lists.List.NewList()
    Private Const ROOT As String = "root"
    '   Private listEditItemsRights As Boolean
    Private listManagedListRights As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim loTree As New obout_ASPTreeView_2_NET.Tree
        Dim llParentID As Integer = 0
        Dim llListID As Integer = 0


        llParentID = QueryStringParser.GetInt("parent")

        loTree.SubTree = True

        Dim loParent As Arco.Doma.Library.Lists.ListItem = Arco.Doma.Library.Lists.ListItem.GetListItem(llParentID)

        If GetList(loParent.LIST_ID) Then
            GetNextLevel(loTree, loParent)
        End If

        If System.IO.Directory.Exists(Server.MapPath(String.Concat("~/App_Themes/", Theme, "/Tree/Icons"))) Then
            loTree.FolderIcons = String.Concat("../App_Themes/", Theme, "/Tree/Icons")
        Else
            loTree.FolderIcons = "../TreeIcons/Icons"
        End If
        loTree.FolderStyle = "../TreeIcons/Styles/Win2003"
        loTree.FolderScript = "../TreeIcons/Script"

        If moList.LIST_TYPE = "UDCLIST" Then
            loTree.DragAndDropEnable = False
        Else
            loTree.DragAndDropEnable = moList.CanEditItems()
        End If

        Response.Write(loTree.HTML)

    End Sub

    Private Function GetList(ByVal vlListID As Integer) As Boolean

        Dim lbOK As Boolean = False
        If vlListID > 0 Then
            moList = Lists.List.GetList(vlListID)
            If moList.ID = vlListID Then
                lbOK = True
            End If
        End If
        Return lbOK

    End Function

#Region " Show tree "

    Private Sub GetNextLevel(ByVal voTree As obout_ASPTreeView_2_NET.Tree, _
                             ByVal voParent As Arco.Doma.Library.Lists.ListItem)

        ' This sub has to be identical in DM_Catalog_listitemsT.aspx.
        ' This sub has to be identical in DM_Catalog_Treelevel.aspx.

        Dim loCrit As Arco.Doma.Library.Lists.ListItemList.Criteria = New Arco.Doma.Library.Lists.ListItemList.Criteria
        loCrit.LIST_ID = moList.ID
        loCrit.Parent = voParent.ID
      
        loCrit.OrderBy = "ITEM_CODE, ITEM_SEARCH"
        Dim loItems As Arco.Doma.Library.Lists.ListItemList = Arco.Doma.Library.Lists.ListItemList.GetListItemList(loCrit)
        For Each loItem As Arco.Doma.Library.Lists.ListItemList.ListItemInfo In loItems
            ShowNode(voTree, False, loItem, voParent.ID)
        Next
    End Sub

    Private Sub ShowNode(ByVal voTree As obout_ASPTreeView_2_NET.Tree, _
                         ByVal vbFirst As Boolean, loItem As Arco.Doma.Library.Lists.ListItemList.ListItemInfo, ByVal vlParentID As Integer)

        ' This sub has to be identical in DM_Catalog_listitemsT.aspx.
        ' This sub has to be identical in DM_Catalog_Treelevel.aspx.
        Dim rand As New Random
        Dim lsURL As String = "", lsCaption As String = ""
        Dim lsSyns As String = "", lsLangs As String = "", lsRels As String = ""
        Dim lsExtra As String = ""

        Dim lsDesc As String = loItem.Description
        If loItem.ITEM_CODE.Length > 0 Then lsDesc = loItem.ITEM_CODE & " " & lsDesc
        If loItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & loItem.ITEM_DESC_HOMO & "]"


        If loItem.HasChildren Then
            lsURL = "DM_Catalog_Treelevel.aspx?id=" & moList.ID & "&parent=" & loItem.ID & "&rnd_str=" & rand.Next
        Else
            lsURL = ""
        End If

        lsCaption = "<NOBR><span>"

        If moList.LIST_TYPE = "THESAURUS" Or moList.LIST_TYPE = "CONCEPTTREE" Or moList.LIST_TYPE = "UDCLIST" Then
            lsCaption &= "<a href=javascript:RefreshRightPaneLID2(" & moList.ID & "," & loItem.ID & ",'N','t" & vlParentID & "_" & loItem.ID & "');>"
            lsCaption &= Server.HtmlEncode(lsDesc)
            lsCaption &= "</a>"
        End If

        If loItem.ITEM_SCOPE_NOTE.Length > 0 Then
            lsCaption &= " <i><font color=gray>(" & Server.HtmlEncode(loItem.ITEM_SCOPE_NOTE) & ")</font></i>"
        End If

        lsCaption &= "</span></NOBR>"

        voTree.Add(ROOT, "t" & vlParentID & "_" & loItem.ID, lsCaption, False, "folder.svg", lsURL)

    End Sub

  

   
#End Region

End Class
