Imports Arco.Doma.Library
Imports System.Xml

Partial Class DM_Catalog_Tree
    Inherits BasePage

    Private Const DENIED_PAGE As String = "../DM_ACL_DENIED.aspx"
    Dim llParentID As Int32 = 0
    Dim llRootID As Int32 = 0
    Protected mlPropID As Integer = 0
    Protected mlObjID As Integer = 0

    Private mlCurrentListID As Int32 = 0
    Private moCurrentList As Lists.List

    Protected moTreeView As New obout_ASPTreeView_2_NET.Tree
    
    Protected mbMultiSelect As Boolean = False
    Protected mbRefreshOnChange As Boolean = False
    Private lsHierarchy As String = ""   
    Protected mbCodeInDesc As Boolean = True
    Private mbAutoSelect As Boolean = False
    Private moObj As baseObjects.DM_OBJECT = Nothing
    Private mbSelectLowestNodeOnly As Boolean = False

    Protected ReadOnly Property CurrentObject As baseObjects.DM_OBJECT
        Get
            If moObj Is Nothing Then
                moObj = ObjectRepository.GetObject(mlObjID)
            End If
            Return moObj
        End Get
    End Property

    Protected ReadOnly Property CurrentList() As Lists.List
        Get
            If mlCurrentListID = 0 Then
                Response.Write("no list id supplied")
                Response.End()
            End If
            If moCurrentList Is Nothing Then
                moCurrentList = Lists.List.GetList(mlCurrentListID)
                If moCurrentList.ID <> mlCurrentListID Then
                    Response.Write("list id not found")
                    Response.End()

                End If
            End If
            Return moCurrentList
        End Get
    End Property

    Private Sub sSetLabels()
        lblSearch.Text = GetLabel("search")
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.Form.DefaultButton = lnkSearch.UniqueID


        sSetLabels()

        If Not IsPostBack Then
            If ViewState("list_id") = Nothing Then
                mlCurrentListID = QueryStringParser.GetInt("id")
                ViewState("list_id") = mlCurrentListID
            End If
        Else
            mlCurrentListID = ViewState("list_id")
        End If
        mbCodeInDesc = QueryStringParser.GetBoolean("codeindesc")

        If Not Page.IsPostBack Then
            txtFilter.Text = QueryStringParser.GetString("autovalidate")
        End If

        mlPropID = Val(Request("PROP_ID"))
        mlObjID = Val(Request("DM_OBJECT_ID"))


        Dim loProp As Arco.Doma.Library.baseObjects.DM_PROPERTY = Arco.Doma.Library.baseObjects.DM_PROPERTY.GetPROPERTY(CType(mlPropID, Int32))

        mbRefreshOnChange = loProp.PROP_ROC
        mbMultiSelect = (loProp.VALUE_NUMBER = 1)
        mbSelectLowestNodeOnly = Arco.Doma.Library.PropertyTypes.ManagedListHandler.OnlyLowestNodeCanBeSelected(loProp)

        ' Overwrite multiselect 
        If Not Request("multiselect") Is Nothing Then
            If Request("multiselect").ToLower = "n" Then
                mbMultiSelect = False
            ElseIf Request("multiselect").ToLower = "y" Then
                mbMultiSelect = True
            End If
        End If

        If Not mbMultiSelect Then
            selectspan.Visible = False
        End If
        HiddenFields.Text &= "<INPUT TYPE=HIDDEN NAME='PROP_ID' VALUE='" & mlPropID & "'>"
        HiddenFields.Text &= "<INPUT TYPE=HIDDEN NAME='DM_OBJECT_ID' VALUE='" & mlObjID & "'>"


        'cmdAdd.Visible = CurrentList.LIST_ALLOWNEW
        'cmdAdd.OnClientClick = "NewItem('" & CurrentList.LIST_ID & "')"

        If CurrentList.isHierarchical Then
            ' pnlTree.Visible = True
            pnlList.Visible = False

            If System.IO.Directory.Exists(Page.Server.MapPath(String.Concat("~/App_Themes/", Page.Theme, "/Tree/Icons"))) Then
                moTreeView.FolderIcons = String.Concat("../App_Themes/", Page.Theme, "/Tree/Icons")
            Else
                moTreeView.FolderIcons = "../TreeIcons/Icons"
            End If

            moTreeView.FolderStyle = "../TreeIcons/Styles/Win2003"
            moTreeView.FolderScript = "../TreeIcons/Script"           
            moTreeView.SubTree = False
            txtFilter.Visible = True
            'cmdFilter.Visible = True
            If txtFilter.Text = "" Then
                GetNextLevel(moTreeView, 0)
            Else
                GetTerms(moTreeView, txtFilter.Text)
            End If
            TreeView.Text = moTreeView.HTML
        Else
            pnlList.Visible = True
            '   pnlTree.Visible = False
            GetTermsList(txtFilter.Text)
        End If


        'If mbAutoSelect Then
        '    Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "autoselect", "Autoselect(1);", True)
        'Else
        'End If
        Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "yourselection", "UpdateYourSelection();", True)
    End Sub

#Region " Show tree "
    ' when search keyword is null

    Private Sub GetNextLevel(ByVal voTree As obout_ASPTreeView_2_NET.Tree, _
                             ByVal vlParentID As Integer)

        Dim loCrit As Lists.ListItemList.Criteria = New Lists.ListItemList.Criteria()
        loCrit.LIST_ID = CurrentList.ID
        loCrit.OrderBy = "ITEM_CODE, ITEM_SEARCH"
        loCrit.CAT_ID = CurrentObject.Category
        loCrit.ActiveOnly = True
        If vlParentID > 0 Then
            loCrit.Parent = vlParentID
        Else
            loCrit.ITEM_ROOT_NODE = 1
        End If

        Dim loItems As Lists.ListItemList = Lists.ListItemList.GetListItemList(loCrit)

        For Each loItem As Lists.ListItemList.ListItemInfo In loItems
            ShowNode(voTree, False, loItem, False)

        Next

    End Sub

    Protected liPage As Int32 = 0

    Private Sub GetTermsList(ByVal vsFilter As String)
        Dim liRecsPerPage As Int32 = 100
        Dim liFirstRec As Int32
        Dim liLastRec As Int32
        liPage = 1
        If Not Request("PAGE") Is Nothing Then
            If Request("PAGE") <> "" Then
                liPage = CType(Request("PAGE"), Int32)
            End If
        End If

        liFirstRec = ((liPage - 1) * liRecsPerPage) + 1
        liLastRec = liFirstRec + liRecsPerPage - 1


        ' This sub has to be identical in DM_Catalog_Tree.aspx.
        ' This sub has to be identical in DM_Catalog_TreeT.aspx.

        Dim loItems As Lists.ListItemList
        Dim loItem As Lists.ListItemList.ListItemInfo
        Dim loCrit As Lists.ListItemList.Criteria = New Lists.ListItemList.Criteria

        loCrit.Range = ListRangeRequest.Range(liFirstRec, liLastRec, 1000)

        loCrit.Filter = vsFilter
        loCrit.LIST_ID = CurrentList.ID
        loCrit.IncludeLang = CurrentList.MULTI_LINGUAL
        loCrit.IncludeSyn = CurrentList.USE_SYNONYMS
        loCrit.OrderBy = "ITEM_CODE, ITEM_SEARCH"
        loCrit.CAT_ID = CurrentObject.Category
        loCrit.ActiveOnly = True

        loItems = Lists.ListItemList.GetListItemList(loCrit)

        If loItems.Count = 1 Then
            mbAutoSelect = True
        End If

        Dim lstable As StringBuilder = New StringBuilder
        lstable.Append("<table class='List'><tr><th>&nbsp;</th><th></th></tr>")
        Dim i As Int32 = 1
        For Each loItem In loItems
            If i >= liFirstRec And i <= liLastRec Then
                ShowListItem(lstable, loItem)
            End If
            i += 1
        Next

        If loItems.Any Then
            Dim lbPrev As Boolean
            Dim lbNext As Boolean
            Dim lbFirst As Boolean = False
            Dim liLastPage As Int32 = 0
            If liPage > 1 Then
                lbPrev = True
            Else
                lbFirst = True
                lbPrev = False
            End If

            If liLastRec > loItems.Count Then
                liLastRec = loItems.Count
            End If

            If liLastRec < loItems.Count Then
                lbNext = True
            Else
                lbNext = False
            End If
            liLastPage = (loItems.Count / liRecsPerPage)
            If loItems.Count Mod liRecsPerPage > 0 Then
                liLastPage += 1
            End If


            lstable.Append("<tr class='ListFooter'>")
            lstable.Append("<td colspan='2'>")
        
            lstable.Append("<a " & fsScrollLink("javascript:Goto(" & (liPage - 1) & ");", lbPrev) & " class='ButtonLink' " & fsDisabled(lbPrev) & ">" & GetLabel("previous") & "</a>")

            lstable.Append("&nbsp;" & PageScroller.GetPageScroller(liPage, liLastPage).Render("Goto") & "&nbsp;")

            lstable.Append(" <a " & fsScrollLink("javascript:Goto(" & (liPage + 1) & ");", lbNext) & " class='ButtonLink' " & fsDisabled(lbNext) & ">" & GetLabel("next") & "</a>")

         
            lstable.Append(" </td>")
            lstable.Append(" </tr>")
        End If
        lstable.Append("</TABLE>")

        plhResults.Text = lstable.ToString

    End Sub

    Private Function fsDisabled(ByVal vbEnabled As Boolean) As String
        If Not vbEnabled Then
            Return "disabled='disabled'"
        Else
            Return ""
        End If
    End Function
    Private Function fsScrollLink(ByVal vsLink As String, ByVal vbEnabled As Boolean) As String
        If Not vbEnabled Then
            Return ""
        Else
            Return "href='" & vsLink & "'"
        End If
    End Function
    Private Sub GetTerms(ByVal voTree As obout_ASPTreeView_2_NET.Tree, _
                         ByVal vsFilter As String)

        ' This sub has to be identical in DM_Catalog_Tree.aspx.
        ' This sub has to be identical in DM_Catalog_TreeT.aspx.

        Dim loItems As Lists.ListItemList
        Dim loItem As Lists.ListItemList.ListItemInfo
        Dim loCrit As Lists.ListItemList.Criteria

        loCrit = New Lists.ListItemList.Criteria
        loCrit.Filter = vsFilter
        loCrit.LIST_ID = CurrentList.ID
        loCrit.IncludeLang = CurrentList.MULTI_LINGUAL
        loCrit.IncludeSyn = CurrentList.USE_SYNONYMS
        loCrit.OrderBy = "ITEM_CODE, ITEM_SEARCH"
        loCrit.CAT_ID = CurrentObject.Category

        loItems = Lists.ListItemList.GetListItemList(loCrit)

        If loItems.Count = 1 Then
            mbAutoSelect = True
        End If

        For Each loItem In loItems
            ShowNode(voTree, False, loItem, True)
        Next

    End Sub

    Private Sub ShowListItem(ByVal sbTable As StringBuilder, ByVal voItem As Lists.ListItemList.ListItemInfo)

        Dim lsCaption As String = ""

        Dim lbIsParent As Boolean = False

        Dim lsDesc As String = voItem.Caption(Language, True, True) '& lsHierarchy
        Dim lsRetDesc As String = voItem.Caption(Language, True, mbCodeInDesc, True)

        If voItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & voItem.Homonym(Arco.Security.BusinessIdentity.CurrentIdentity.Language, True) & "]"

        sbTable.Append("<tr><td width='100%'>")
        sbTable.Append("<a href='javascript:Select(" & voItem.ID & "," & EncodingUtils.EncodeJsString(lsRetDesc) & ");'>")
        sbTable.Append(fsEncode(lsDesc))
        sbTable.Append("</a>")
      

        If voItem.ITEM_SCOPE_NOTE.Length > 0 Then
            sbTable.Append(" <i><font color=gray>(" & fsEncode(voItem.ScopeNote(Arco.Security.BusinessIdentity.CurrentIdentity.Language, True)) & ")</font></i>")
        End If

        sbTable.Append("</tr>")

    End Sub

    Private Sub ShowNode(ByVal voTree As obout_ASPTreeView_2_NET.Tree, _
                          ByVal vbFirst As Boolean, ByVal voItem As Lists.ListItemList.ListItemInfo, ByVal vbFromSearch As Boolean)

        Dim lsURL As String = "", lsCaption As String = ""

        Dim lbIsParent As Boolean = False
        Dim lsHierarchy As String = fsHierarchy(voItem)

        Dim lsDesc As String = voItem.Caption(Language, True, True)
        Dim lsRetDesc As String = voItem.Caption(Language, True, mbCodeInDesc, True)
        If mbCodeInDesc Then
            lsRetDesc &= lsHierarchy
        End If

        If vbFromSearch Then
            lsDesc &= lsHierarchy
        End If
     
        If voItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & voItem.Homonym(Arco.Security.BusinessIdentity.CurrentIdentity.Language, True) & "]"


        If CurrentList.isHierarchical Then
            If voItem.HasChildren Then
                lsURL = "DM_Catalog_TreeT.aspx?id=" & CurrentList.ID & "&parent=" & voItem.ID & "&SELLOWEST=" & mbSelectLowestNodeOnly & "&DM_OBJECT_ID=" & mlObjID
                If mbCodeInDesc Then
                    lsURL &= "&codeindesc=Y"
                End If
                'If vbFromSearch Then
                '    lsURL &= "&showpath=Y"
                'End If
            Else
                lsURL = ""
            End If
        Else
            lsURL = ""
        End If


        lsCaption = "<NOBR><span>"
        Dim lsImage As String = "folder.svg"
        If Not voItem.HasChildren OrElse Not mbSelectLowestNodeOnly Then
            lsCaption &= "<a href='javascript:Select(" & voItem.ID & "," & EncodingUtils.EncodeJsString(lsRetDesc) & ");'>"

            lsCaption &= fsEncode(lsDesc)
            lsCaption &= "</a>"
        Else
            lsCaption &= fsEncode(lsDesc)
            lsImage = "inactive_folder.png"
        End If

        If voItem.ITEM_SCOPE_NOTE.Length > 0 Then
            lsCaption &= " <i><font color=gray>(" & fsEncode(voItem.ScopeNote(Arco.Security.BusinessIdentity.CurrentIdentity.Language, True)) & ")</font></i>"
        End If



        lsCaption &= "</span></NOBR>"


        voTree.Add("root", "t" & voItem.ITEM_PARENT_ID & "_" & voItem.ID, lsCaption, False, lsImage, lsURL)

    End Sub

    Private Function fsHierarchy(ByVal voItem As Lists.IListItem) As String
        If CurrentList.isHierarchical Then
            Dim lsOut As String = Lists.ListItem.GetPath(voItem)

            If lsOut.Trim() <> "" Then
                ' lsOut = " <i><font color=gray>[" & lsOut & "]</font></i>"
                lsOut = " [" & lsOut & "]"
            End If
            Return lsOut
        Else
            Return ""
        End If
    End Function

    Private Function fsEncode(ByVal vsText As String) As String

        ' This function has to be identical in DM_Catalog_Tree.aspx.
        ' This function has to be identical in DM_Catalog_TreeT.aspx.

        Return Server.HtmlEncode(vsText)

    End Function

#End Region



End Class