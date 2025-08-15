Imports Arco.Doma.Library

Partial Class DM_Catalog_TreeT
    Inherits BasePage

    Private moList As Lists.List = Lists.List.NewList()
    Private msUserLang As String = ""
    Private moObj As baseObjects.DM_OBJECT = Nothing
    Private mlObjID As Int32 = 0
    Private mbSelectLowestNodeOnly As Boolean = False
    Private mbCodeInDesc As Boolean = False
    ' Private mbShowPath As Boolean = False

    Protected ReadOnly Property CurrentObject As baseObjects.DM_OBJECT
        Get
            If moObj Is Nothing Then
                moObj = ObjectRepository.GetObject(mlObjID)
            End If
            Return moObj
        End Get
    End Property
   
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.AddHeader("pragma", "no-cache")
        Response.AddHeader("cache-control", "private")
        Response.CacheControl = "no-cache"

        Dim loTree As New obout_ASPTreeView_2_NET.Tree
        Dim llParentID As Integer = 0
        Dim llListID As Integer = 0

        msUserLang = Arco.Security.BusinessIdentity.CurrentIdentity.Language

        llListID = QueryStringParser.GetInt("id")
        llParentID = QueryStringParser.GetInt("parent")
        mlObjID = QueryStringParser.GetInt("DM_OBJECT_ID")
        mbSelectLowestNodeOnly = QueryStringParser.GetBoolean("SELLOWEST")
        mbCodeInDesc = QueryStringParser.GetBoolean("codeindesc")

        loTree.SubTree = True

        If llListID > 0 Then
            If GetList(llListID) = True Then
                GetNextLevel(loTree, llParentID)
            End If
        End If

        If System.IO.Directory.Exists(Page.Server.MapPath(String.Concat("~/App_Themes/", Page.Theme, "/Tree/Icons"))) Then
            loTree.FolderIcons = String.Concat("../App_Themes/", Page.Theme, "/Tree/Icons")
        Else
            loTree.FolderIcons = "../TreeIcons/Icons"
        End If

        loTree.FolderStyle = "../TreeIcons/Styles/Win2003"
        loTree.FolderScript = "../TreeIcons/Script"

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
                             ByVal vlParentID As Integer)

        Dim loCrit As Lists.ListItemList.Criteria = New Lists.ListItemList.Criteria()
        loCrit.LIST_ID = moList.ID
        loCrit.OrderBy = "ITEM_CODE, ITEM_SEARCH"
        If vlParentID > 0 Then
            loCrit.Parent = vlParentID
        Else
            loCrit.ITEM_ROOT_NODE = 1
        End If
        loCrit.CAT_ID = CurrentObject.Category
        loCrit.ActiveOnly = True
        Dim loItems As Lists.ListItemList = Lists.ListItemList.GetListItemList(loCrit)

        For Each loItem As Lists.ListItemList.ListItemInfo In loItems
            ShowNode(voTree, False, loItem)
        Next

    End Sub


    Private Sub ShowNode(ByVal voTree As obout_ASPTreeView_2_NET.Tree, _
                          ByVal vbFirst As Boolean, ByVal voItem As Lists.ListItemList.ListItemInfo)

        Dim lsURL As String = "", lsCaption As String = ""
        Dim lbIsParent As Boolean = False
        Dim lsHierarchy As String = fsHierarchy(voItem)

        Dim lsDesc As String = voItem.Caption(Language, True, True, True)
        Dim lsRetDesc As String = voItem.Caption(Language, True, mbCodeInDesc, True)

        If mbCodeInDesc Then
            lsRetDesc &= lsHierarchy
        End If
 
        If voItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & voItem.Homonym(Arco.Security.BusinessIdentity.CurrentIdentity.Language, True) & "]"


        If voItem.HasChildren Then
            lsURL = "DM_Catalog_TreeT.aspx?id=" & moList.ID & "&parent=" & voItem.ID & "&SELLOWEST=" & mbSelectLowestNodeOnly
            If mbCodeInDesc Then
                lsURL &= "&codeindesc=Y"
            End If
        Else
            lsURL = ""
        End If

        lsCaption = "<NOBR><span>"
        Dim lsImage As String = "folder.svg"
        If Not voItem.HasChildren OrElse Not mbSelectLowestNodeOnly Then
            lsCaption &= "<a href='javascript:Select(" & voItem.ID & "," & Chr(34) & fsEncode2(lsRetDesc).Replace("\", "\\") & Chr(34) & ");'>"
            ' lsCaption &= "<input type='checkbox' class='chk' id='chk_" & voItem.ITEM_ID & "' name='chk_" & voItem.ITEM_ID & "'><a href='javascript:Select(" & voItem.ITEM_ID & "," & Chr(34) & fsEncode2(lsRetDesc).Replace("\", "\\") & Chr(34) & ");'>"

            lsCaption &= fsEncode(lsDesc)
            lsCaption &= "</a>"
        Else
            lsCaption &= fsEncode(lsDesc)
            lsImage = "inactive_folder.png"
        End If

        If voItem.ITEM_SCOPE_NOTE.Length > 0 Then
            lsCaption &= " <i><font color=gray>(" & fsEncode(voItem.ScopeNote(Arco.Security.BusinessIdentity.CurrentIdentity.Language, True)) & ")</font></i>"
        End If

        '  lsCaption &= lsHierarchy



        'lsCaption &= " <a href=javascript:ShowDetails('" & voItem.ITEM_ID & "');>"
        'lsCaption &= "<img ID=A" & voItem.ITEM_PARENT_ID & "_" & voItem.ITEM_ID & " alt='Details' border=0 src=../Images/help.png>"
        'lsCaption &= "</a>"

        lsCaption &= "</span></NOBR>"

        'lsCaption &= "<INPUT TYPE=HIDDEN NAME='VAL" & voItem.ITEM_PARENT_ID & "_" & voItem.ITEM_ID & "' VALUE=" & Chr(34) & voItem.ITEM_ID & Chr(34) & ">"
        'lsCaption &= "<INPUT TYPE=HIDDEN NAME='DESC" & voItem.ITEM_PARENT_ID & "_" & voItem.ITEM_ID & "' VALUE=" & Chr(34) & fsEncode2(lsRetDesc) & Chr(34) & ">"
        voTree.Add("root", "t" & voItem.ITEM_PARENT_ID & "_" & voItem.ID, lsCaption, False, lsImage, lsURL)


    End Sub

    Private Function fsHierarchy(ByVal voItem As Lists.IListItem) As String

        Dim lsOut As String = Lists.ListItem.GetPath(voItem)

        If lsOut.Trim() <> "" Then
            'lsOut = " <i><font color=gray>[" & lsOut & "]</font></i>"
            lsOut = " [" & lsOut & "]"
        End If
        Return lsOut
       

    End Function

    Private Function fsEncode(ByVal vsText As String) As String

        ' This function has to be identical in DM_Catalog_Tree.aspx.
        ' This function has to be identical in DM_Catalog_TreeT.aspx.

        Return Server.HtmlEncode(vsText)

    End Function

    Private Function fsEncode2(ByVal vsText As String) As String

        ' This function has to be identical in DM_Catalog_Tree.aspx.
        ' This function has to be identical in DM_Catalog_TreeT.aspx.

        'Return vsText.Replace("'", "\'")
        'todo : replace chr(34) ?
        Return Server.HtmlEncode(vsText)

    End Function



#End Region

  

End Class
