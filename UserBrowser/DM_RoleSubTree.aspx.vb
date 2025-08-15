Imports Arco.Doma.Library

Partial Class DM_RoleSubTree
    Inherits BasePage
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
      
        Dim loTree As New obout_ASPTreeView_2_NET.Tree
        Dim llParentID As Integer = QueryStringParser.GetInt("parent")

        loTree.SubTree = True

        GetNextLevel(loTree, llParentID)

        If System.IO.Directory.Exists(Page.Server.MapPath(String.Concat("~/App_Themes/", Page.Theme, "/Tree/Icons"))) Then
            loTree.FolderIcons = String.Concat("../App_Themes/", Page.Theme, "/Tree/Icons")
        Else
            loTree.FolderIcons = "../TreeIcons/Icons"
        End If

        loTree.FolderStyle = "../TreeIcons/Styles/Win2003"
        loTree.FolderScript = "../TreeIcons/Script"

        Response.Write(loTree.HTML)

    End Sub

    Private Sub GetNextLevel(ByVal voTree As obout_ASPTreeView_2_NET.Tree, ByVal vlParentID As Integer)
        Dim loCrit As Arco.Doma.Library.ACL.ROLEList.Criteria
        loCrit = New ACL.ROLEList.Criteria(vlParentID)

        Dim lcolChildStructuredRoles As ACL.ROLEList = ACL.ROLEList.GetChildStructureRoles(vlParentID.ToString)
        For Each loItem As ACL.ROLEList.ROLEInfo In lcolChildStructuredRoles
            Dim lcolSubChildStructuredRoles As ACL.ROLEList = ACL.ROLEList.GetChildStructureRoles(loItem.ID.ToString)            
            If lcolSubChildStructuredRoles.Any Then
                ShowNode(voTree, loItem, True)
            Else
                ShowNode(voTree, loItem, False)
            End If

        Next
    End Sub
    Private Sub ShowNode(ByVal voTree As obout_ASPTreeView_2_NET.Tree, ByVal voItem As ACL.ROLEList.ROLEInfo, ByVal ChildRoles As Boolean)

        Dim lsURL As String = "", lsCaption As String = ""

        Dim lbIsParent As Boolean = False

        Dim lsImage As String = "User.svg"
        Dim lsDesc As String = voItem.Name
        lsCaption = "<NOBR><span id='t_" & voItem.ID & "'>"
        lsCaption &= "<a href=javascript:ShowUrl('DM_UserBrowser_RoleDetail.aspx'," & voItem.ID & ",false,'',true) >"
        lsCaption &= lsDesc
        lsCaption &= "</a>"
        lsCaption &= "</NOBR></span>"
        If ChildRoles Then
            lsURL = "DM_RoleSubTree.aspx?parent=" & voItem.ID & ""
        Else
            lsURL = ""
        End If
        voTree.Add("root", voItem.ID, lsCaption, False, lsImage, lsURL)

    End Sub

End Class
