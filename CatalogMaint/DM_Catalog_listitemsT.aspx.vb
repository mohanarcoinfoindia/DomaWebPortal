Imports Arco.Doma.Library

Partial Class DM_Catalog_listitemsT
    Inherits BasePage

    Private Const DENIED_PAGE As String = "../DM_ACL_DENIED.aspx"
    Private moList As Lists.List = Lists.List.NewList()
    Private moParent As Lists.ListItem = Lists.ListItem.NewListItem(0)
    Protected loTreeView As New obout_ASPTreeView_2_NET.Tree  
    Private Const ROOT As String = "root"


    Protected Enum PopupIndex
        Children = 0
        Edit = 1
        Delete = 2
    End Enum
    Private Sub DeleteItem()
        If Not Request.Form("delitem") Is Nothing Then
            Dim i As Integer = 0
            If Int32.TryParse(Request.Form("delitem"), i) Then
                Arco.Doma.Library.Lists.ListItem.DeleteListItem(i)
            End If
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        sc1.Scripts.Add(New System.Web.UI.ScriptReference("~/Resources/" & Language & "/Messages.js"))


        Dim llListID As Integer = 0
        Dim llParentID As Integer = 0
        Dim lsCaption As String = ""
        Dim lsSelectedIDs As String = ""     
        If Not IsPostBack Then

            llListID = QueryStringParser.GetInt("id")
            llParentID = QueryStringParser.GetInt("parent")
            lsSelectedIDs = QueryStringParser.GetString("selectedids")
            ViewState("list_id") = llListID
            ViewState("parent_id") = llParentID
        Else
            DeleteItem()
        End If


        CM_Tree.Items(0).Text = GetLabel("addnode")
        CM_Tree.Items(1).Text = GetLabel("editopen")
        CM_Tree.Items(2).Text = GetLabel("delete")

        If llListID = 0 Then
            llListID = Val(ViewState("list_id"))
            llParentID = Val(ViewState("parent_id"))
        End If

        If llListID > 0 Then
            If GetList(llListID) Then
                lblList.Text = moList.LIST_TYPE & " - " & moList.Name
                If System.IO.Directory.Exists(Server.MapPath(String.Concat("~/App_Themes/", Theme, "/Tree/Icons"))) Then
                    loTreeView.FolderIcons = String.Concat("../App_Themes/", Theme, "/Tree/Icons")
                Else
                    loTreeView.FolderIcons = "../TreeIcons/Icons"
                End If

                loTreeView.FolderStyle = "../TreeIcons/Styles/Win2003"
                loTreeView.FolderScript = "../TreeIcons/Script"
                loTreeView.ShowRootIcon = False
                loTreeView.ShowRootPlusMinus = True
                loTreeView.EventList = "OnBeforeNodeDrop"

                ListRights()
                CreateJS_ListID()
                CreateJS_TranslatedMSG()
                If llParentID = 0 Then
                    lsCaption = "<a href=javascript:RefreshRightPane(" & moList.ID & ");>"
                    lsCaption &= Server.HtmlEncode(moList.LIST_TYPE & " - " & moList.Name)
                    lsCaption &= "</a>"
                Else
                    lsCaption = Server.HtmlEncode(moList.LIST_TYPE & " - " & moList.Name)
                End If

                loTreeView.AddRootNode(lsCaption, True, "")
                loTreeView.SubTree = False
                If llParentID = 0 Then
                    txtFilter.Visible = True
                    cmdFilter.Visible = True
                Else
                    txtFilter.Visible = False
                    cmdFilter.Visible = False
                End If

                If llParentID > 0 Then
                    If GetParent(llParentID) = True Then
                        ShowNode(loTreeView, True, moParent)
                    End If
                Else
                    If txtFilter.Text = "" Then
                        GetNextLevel(loTreeView, 0)
                    Else
                        GetTerms(loTreeView, txtFilter.Text)
                    End If
                End If

                If lsSelectedIDs <> "" Then
                    loTreeView.SelectedId = lsSelectedIDs
                End If
                TreeView.Text = loTreeView.HTML
                CreateJS()
            Else
                Response.Redirect(DENIED_PAGE)
            End If
        Else
            Response.Redirect(DENIED_PAGE)
        End If
        ClearCache()
    End Sub

    Public Sub ListRights()
        Dim listEditItemsRights As Boolean = moList.CanEditItems()

        If moList.LIST_TYPE = "UDCLIST" Then
            loTreeView.DragAndDropEnable = False
        Else
            If listEditItemsRights Then
                loTreeView.DragAndDropEnable = True
            Else
                loTreeView.DragAndDropEnable = False
            End If
        End If

        If Not listEditItemsRights Then
            TreeViewDiv.Attributes.Remove("oncontextmenu")
        Else
            TreeViewDiv.Attributes.Add("oncontextmenu", "return ShowContextMenu();")
        End If
    End Sub
    Private Sub ClearCache()
        HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(False)
        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        HttpContext.Current.Response.Cache.SetNoStore()
        Response.Cache.SetExpires(Arco.SystemClock.Now)
        Response.Cache.SetValidUntilExpires(True)
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

    Private Function GetParent(ByVal vlParentID As Integer) As Boolean

        Dim lbOK As Boolean = False

        If vlParentID > 0 Then
            moParent = Lists.ListItem.GetListItem(vlParentID)
            If moParent.ID = vlParentID Then
                lbOK = True
            End If
        End If

        Return lbOK

    End Function
    Private Sub CreateJS_ListID()
        Dim lsJS As String = ""
        lsJS = "function GetListID() {" & vbCrLf
        lsJS &= "  return " & moList.ID & ";" & vbCrLf
        lsJS &= "}"
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "OnNodeDrop", vbCrLf & lsJS, True)


    End Sub

    Private Sub CreateJS_TranslatedMSG()
        Dim lsJS As String = ""
        lsJS = "function GetTranslatedMSG() {" & vbCrLf
        lsJS &= "  var msg = """ & GetDecodedLabel("cm_revert_changes_before_node_drop") & """;" & vbCrLf
        lsJS &= "  return msg;" & vbCrLf
        lsJS &= "}"
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "MsgBeforeNodeDrop", vbCrLf & lsJS, True)


    End Sub

    Private Sub CreateJS()

        Dim lsJS As String = ""

        lsJS = "function DMC(sender, args) {" & vbCrLf
        lsJS &= "  var id = new String(document.all.txtSelectedID.value);" & vbCrLf
        lsJS &= "   var CommandName = args.get_item().get_value();" & vbCrLf
        lsJS &= "  var idsubstr = id.substr(0,1);" & vbCrLf
        lsJS &= "  if (idsubstr == 't') {" & vbCrLf
        lsJS &= "    var itemid = id.substr(id.indexOf('_')+1);" & vbCrLf
        lsJS &= "    if (CommandName == 'EditCommand') {" & vbCrLf
        lsJS &= "      RefreshRightPaneLID2('" & moList.ID & "', itemid, 'N', id);" & vbCrLf
        lsJS &= "    }" & vbCrLf
        lsJS &= "    if (CommandName == 'ChildrenCommand') {" & vbCrLf
        lsJS &= "      RefreshRightPaneLID2('" & moList.ID & "', itemid, '', id);" & vbCrLf
        lsJS &= "    }" & vbCrLf
        lsJS &= "    if (CommandName == 'DeleteCommand') {" & vbCrLf
        lsJS &= "      DeleteItem(itemid);" & vbCrLf
        lsJS &= "    }" & vbCrLf
        lsJS &= "  } else {" & vbCrLf
        lsJS &= "     if (CommandName == 'ChildrenCommand') { RefreshRightPaneLID2('" & moList.ID & "', '0', '', id);}" & vbCrLf
        lsJS &= "     else { alert('" & GetDecodedLabel("cm_invalid_operation_on_root") & "'); }" & vbCrLf
        lsJS &= "  }" & vbCrLf
        lsJS &= "}"

        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging1", lsJS, True)

    End Sub

    'Private Sub ContextMenu1_ItemCommand(ByVal sender As Object, _
    '                                     ByVal e As System.Web.UI.WebControls.CommandEventArgs) Handles ContextMenu1.ItemCommand

    '    Dim lsID As String = ""
    '    Dim llItemID As Integer = 0
    '    Dim llParentID As Integer = 0
    '    Dim loItem As Lists.ListItem
    '    Dim loItemRels As Lists.ListItemRelList
    '    Dim loCrit As Lists.ListItemRelList.Criteria

    '    Select Case e.CommandName
    '        Case "DeleteCommand"
    '            lsID = txtSelectedID.Value
    '            If Left(lsID, 1) = "t" Then
    '                lsID = lsID.Substring(1)
    '                llParentID = Val(lsID.Substring(0, lsID.IndexOf("_")))
    '                llItemID = Val(lsID.Substring(lsID.IndexOf("_") + 1))

    '                Try
    '                    loItem = Lists.ListItem.GetListItem(llItemID)
    '                    If loItem.ITEM_ID = llItemID Then
    '                        loCrit = New Lists.ListItemRelList.Criteria
    '                        loCrit.LINK_DIRECT = 1
    '                        loCrit.LINK_TYPE = 1
    '                        loCrit.ITEM2_ID = llItemID
    '                        loItemRels = Lists.ListItemRelList.GetListItemRelList(loCrit)
    '                        If loItemRels.Count > 1 Then
    '                            ' Remove only relationship.
    '                            If llParentID > 0 Then
    '                                ' Remove relationship between parent and current item.
    '                                Try
    '                                    Lists.ListItemRel.DeleteListItemRel(llParentID, llItemID)
    '                                Catch ex As Exception
    '                                    Arco.Utils.Logging.Log("Could not remove relationship between " & llParentID & " and " & llItemID)
    '                                    Arco.Utils.Logging.Log(ex.Message)
    '                                End Try
    '                            Else
    '                                Arco.Utils.Logging.Log("Could not remove item " & llItemID & ". This item has more than one parent.")
    '                            End If
    '                        Else
    '                            ' Remove item.
    '                            Try
    '                                Lists.ListItem.DeleteListItem(llItemID)
    '                            Catch ex As Exception
    '                                Arco.Utils.Logging.Log("Could not remove item " & llItemID & ":")
    '                                Arco.Utils.Logging.Log(ex.Message)
    '                            End Try
    '                        End If
    '                        loItemRels = Nothing
    '                    End If
    '                    loItem.Dispose()

    '                Catch ex As Exception
    '                    Arco.Utils.Logging.Log("Could not remove item " & llItemID & ":")
    '                    Arco.Utils.Logging.Log(ex.Message)
    '                Finally
    '                    loItem = Nothing

    '                    RefreshLeftPane()
    '                    RefreshRightPane()

    '                End Try

    '            End If

    '            If lsID.IndexOf("root") >= 0 Then
    '                Alert(GetDecodedLabel("cm_invalid_operation_on_root"))
    '            End If

    '    End Select

    'End Sub

    Private Sub RefreshLeftPane()

        Dim sb As New StringBuilder

        If (moList.LIST_TYPE = "UDCLIST" Or _
            moList.LIST_TYPE = "THESAURUS" Or _
            moList.LIST_TYPE = "CONCEPTTREE") Then
            sb.AppendLine("parent.RefreshLeftPane(" & moList.ID & ",'0','t0_0')")
            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "refresh", sb.ToString, True)
        End If

    End Sub
    Private Sub RefreshRightPane()

        Dim sb As New StringBuilder

        If (moList.LIST_TYPE = "UDCLIST" Or _
            moList.LIST_TYPE = "THESAURUS" Or _
            moList.LIST_TYPE = "CONCEPTTREE") Then
            sb.AppendLine("parent.RefreshRightPane(" & moList.ID & ")")
            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "refreshright", sb.ToString, True)
        End If

    End Sub

    Private Sub GetTerms(ByVal voTree As obout_ASPTreeView_2_NET.Tree, _
                         ByVal vsFilter As String)

        Dim loItems As Lists.ListItemList
        Dim loItem As Lists.ListItemList.ListItemInfo
        Dim loCrit As Lists.ListItemList.Criteria

        loCrit = New Lists.ListItemList.Criteria
        loCrit.Filter = vsFilter
        loCrit.LIST_ID = moList.ID
        loCrit.IncludeLang = moList.MULTI_LINGUAL
        loCrit.IncludeSyn = moList.USE_SYNONYMS
        loCrit.OrderBy = "ITEM_CODE, ITEM_SEARCH"

        loItems = Lists.ListItemList.GetListItemList(loCrit)
        For Each loItem In loItems
            ShowNode(voTree, False, loItem)
        Next

    End Sub

#Region " Show tree "

    Private Sub GetNextLevel(ByVal voTree As obout_ASPTreeView_2_NET.Tree, _
                             ByVal vlParentID As Integer)

        ' This sub has to be identical in DM_Catalog_listitemsT.aspx.
        ' This sub has to be identical in DM_Catalog_Treelevel.aspx.

        Dim loCrit As Lists.ListItemList.Criteria = New Lists.ListItemList.Criteria()
        loCrit.LIST_ID = moList.ID
        loCrit.OrderBy = "ITEM_CODE, ITEM_SEARCH"
        If vlParentID > 0 Then
            loCrit.Parent = vlParentID
        Else
            loCrit.ITEM_ROOT_NODE = 1
        End If

        Dim loItems As Lists.ListItemList = Lists.ListItemList.GetListItemList(loCrit)

        For Each loItem As Lists.ListItemList.ListItemInfo In loItems
            ShowNode(voTree, False, loItem)
        Next

    End Sub

    Private Sub ShowNode(ByVal voTree As obout_ASPTreeView_2_NET.Tree, _
                         ByVal vbFirst As Boolean, ByVal voItem As Arco.Doma.Library.Lists.IListItem)

        ' This sub has to be identical in DM_Catalog_listitemsT.aspx.
        ' This sub has to be identical in DM_Catalog_Treelevel.aspx.
        Dim rand As New Random
        Dim lsURL As String = "", lsCaption As String = ""
        Dim lsSyns As String = "", lsLangs As String = "", lsRels As String = ""
        Dim lsExtra As String = ""
        Dim lbIsParent As Boolean = False
        Dim lsDesc As String = voItem.Description 'voItem.Caption(msUserLang, True, True)

        If voItem.ITEM_CODE.Length > 0 Then lsDesc = voItem.ITEM_CODE & " " & lsDesc

        If voItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & voItem.Homonym(Language, True) & "]"

        lbIsParent = voItem.HasChildren
        If lbIsParent Then
            lsURL = "DM_Catalog_Treelevel.aspx?parent=" & voItem.ID & "&rnd_str=" & rand.Next
        Else
            lsURL = ""
        End If

        lsCaption = "<NOBR><span>"
        If moList.LIST_TYPE = "THESAURUS" Or moList.LIST_TYPE = "CONCEPTTREE" Or moList.LIST_TYPE = "UDCLIST" Then
            lsCaption &= "<a href=javascript:RefreshRightPaneLID2(" & moList.ID & "," & voItem.ID & ",'N','t" & voItem.ITEM_PARENT_ID & "_" & voItem.ID & "');>"
            lsCaption &= Server.HtmlEncode(lsDesc)
            lsCaption &= "</a>"
        End If

        If voItem.ITEM_SCOPE_NOTE.Length > 0 Then
            lsCaption &= " <i><font color=gray>(" & Server.HtmlEncode(voItem.ScopeNote(Language, True)) & ")</font></i>"
        End If

        lsCaption &= "</span></NOBR>"

        voTree.Add(ROOT, "t" & voItem.ITEM_PARENT_ID & "_" & voItem.ID, lsCaption, False, "folder.svg", lsURL)

    End Sub
#End Region

    Private Sub Alert(ByVal Mesg As String)
        Dim sb As New StringBuilder
        sb.AppendLine("alert('" & Mesg & "');")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "Alert", sb.ToString, True)
    End Sub    

#Region " Drop Node "

    Private Shared Sub SaveLink(ByVal voItem As Arco.Doma.Library.Lists.ListItem, ByVal vlOldParent As Integer, ByVal vlNewParent As Integer, ByVal copy As Boolean)
        If vlNewParent = 0 Then
            ' Destination = root.
            If vlOldParent > 0 And Not copy Then
                ' Delete old link.
                Lists.ListItemRel.DeleteListItemRel(vlOldParent, voItem.ID)
            End If
        Else
            Dim lbAlreadyExists As Boolean
            Dim lbHasIndirectLink As Boolean
            If Lists.ListItemRel.RelIsValid(vlNewParent, voItem.ID, lbAlreadyExists, lbHasIndirectLink) Then
                If vlOldParent > 0 And Not copy Then
                    ' Delete old link.
                    Lists.ListItemRel.DeleteListItemRel(vlOldParent, voItem.ID)
                End If
                ' Add new link.
                Dim loListItemRel As Lists.ListItemRel = Lists.ListItemRel.NewListItemRel(vlNewParent, voItem.ID)
                loListItemRel.LINK_TYPE = 1 ' BT.
                loListItemRel.LINK_DIRECT = True                
                loListItemRel.Save() ' Triggers AfterSaveHandling in ListItemRel.                      
            Else
                If lbAlreadyExists = True Then
                    Throw New ArgumentException("Not valid. Relationship already exists.")
                ElseIf lbHasIndirectLink = True Then
                    Throw New ArgumentException("Not valid. Found indirect link.")
                End If
            End If
        End If
    End Sub
    <System.Web.Services.WebMethod()> _
    Public Shared Function OnBeforeNodeDrop(ByVal src As String, _
                                   ByVal dst As String, _
                                   ByVal copy As Boolean) As String

        Dim llSourceId As Integer = 0
        Dim llDestinationId As Integer = 0
        Dim llSourceParentId As Integer = 0

        If src <> "root" Then

            llSourceId = Val(src.Substring(src.IndexOf("_") + 1))
            llSourceParentId = Val(src.Substring(1, src.IndexOf("_")))

            If dst = "root" Then
                ' Move/Copy to root folder.
                llDestinationId = 0
            Else
                ' Move/Copy to another folder.
                llDestinationId = Val(dst.Substring(dst.IndexOf("_") + 1))
            End If

            'Arco.Utils.Logging.Log("DragDrop " & llSourceParentId & "." & llSourceId & " to " & llDestinationId)

            Dim loSource As Arco.Doma.Library.Lists.ListItem = Arco.Doma.Library.Lists.ListItem.GetListItem(llSourceId)
            If loSource.ID = llSourceId Then
                If llDestinationId = 0 Then
                    ' Root.
                    SaveLink(loSource, llSourceParentId, llDestinationId, copy)
                Else
                    Dim loDest As Arco.Doma.Library.Lists.ListItem = Arco.Doma.Library.Lists.ListItem.GetListItem(llDestinationId)
                    If loDest.ID = llDestinationId Then
                        SaveLink(loSource, llSourceParentId, llDestinationId, copy)
                    Else
                        Throw New ArgumentException("Invalid item_id " & dst)
                    End If
                End If
            Else
                Throw New ArgumentException("Invalid item_id " & src)
            End If

        Else
            Throw New ArgumentException("The root cann't be moved")
        End If

        Return "t" & llDestinationId & "_" & llSourceId
     
    End Function

#End Region
End Class
