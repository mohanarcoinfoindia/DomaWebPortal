Imports System.IO

Partial Class DM_Catalog_Listitems
    Inherits BasePage

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region
    Private listEditItemsRights As Boolean
    Private listManagedListRights As Boolean
    Protected liPage As Int32
    Private moList As Arco.Doma.Library.Lists.List
    Private moListLanguages As Arco.Doma.Library.Lists.ListLanguageList
    Private moCats As Dictionary(Of Integer, String)
    Private lnkbtnCategory As HyperLink
    Private lnkbtnAddNew As HyperLink

    '<tr Class="ListFooter">
    '                <td colspan = "<%=miColCount%>" >
    '                    <asp:HyperLink ID="lnkbtnCategory" Visible="false" class="ButtonLink" runat="server"/>
    '    <ASP:HyperLink ID = "lnkbtnAddNew" Visible="false" Class="ButtonLink" runat="server"></asp:HyperLink>
    '                </td>
    '            </tr>
    Private Property ListID As Integer
        Get
            Return Convert.ToInt32(ViewState("list_id"))
        End Get
        Set(value As Integer)
            ViewState("list_id") = value
        End Set
    End Property

    Public ReadOnly Property CurrentList As Arco.Doma.Library.Lists.List
        Get
            If moList Is Nothing Then
                moList = Arco.Doma.Library.Lists.List.GetList(Me.ListID)
                If moList.ID <> Me.ListID Then
                    Throw New ArgumentException("List with ID " & Me.ListID & " not found")
                End If
            End If
            Return moList
        End Get
    End Property

    Public ReadOnly Property ListLanguages As Arco.Doma.Library.Lists.ListLanguageList
        Get
            If moListLanguages Is Nothing Then moListLanguages = Arco.Doma.Library.Lists.ListLanguageList.GetListLanguageList(CurrentList.ID)
            Return moListLanguages
        End Get
    End Property

    Private Sub SetLabels()
        lnkbtnAddNew.Text = "<span class='icon icon-add-new' ></span>"
        lnkbtnAddNew.ToolTip = GetLabel("cm_add_new")
        lnkbtnCategory.Text = GetLabel("add") & "&nbsp;" & GetLabel("cm_category")
    End Sub

    Private Sub SetButtons()
        lnkbtnCategory = New HyperLink()
        lnkbtnCategory.ID = "lnkbtnCategory"
        lnkbtnCategory.Visible = False
        lnkbtnCategory.CssClass = "ButtonLink"

        lnkbtnAddNew = New HyperLink()
        lnkbtnAddNew.ID = "lnkbtnAddNew"
        lnkbtnAddNew.Visible = False
        lnkbtnAddNew.CssClass = "ButtonLink"
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ListID = QueryStringParser.GetInt("list_id")
            If ListID = 0 Then
                Throw New ArgumentException("List_ID = 0")
            End If
            liPage = 1
        End If

        SetButtons()
        SetLabels()

        Dim liRecsPerPage As Integer
        If UserProfile.RecordsPerPage.Length > 0 Then
            liRecsPerPage = UserProfile.RecordsPerPage
        Else
            liRecsPerPage = ArcoInfoSettings.DefaultRecordsPerPage
        End If

        If Not Request("PAGE") Is Nothing Then
            If Request("PAGE") <> "" Then
                liPage = CType(Request("PAGE"), Int32)
                If liPage = 0 Then
                    liPage = 1
                End If
            End If
        End If

        ListRights()
        DeleteItem()

        If CurrentList.isCategoryDependant AndAlso listEditItemsRights Then
            lnkbtnCategory.NavigateUrl = "javascript:AddListItemCategory(" & CurrentList.ID & ");"
            lnkbtnCategory.Visible = True
            lnkbtnCategory.Text = GetLabel("add") & "&nbsp;" & GetLabel("cm_category")
            InitCategories()
        End If

        Dim sb1 As New StringBuilder
        sb1.AppendLine("function Close() {")
        If Not CurrentList.isHierarchical Then
            sb1.AppendLine("location.href = 'DM_Catalog_LeftPane.aspx'; ")
        Else
            sb1.AppendLine("parent.GotoManagedLists();")
        End If
        sb1.AppendLine(" }")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging1", sb1.ToString, True)

        RenderGrid(liPage, liRecsPerPage)

    End Sub

    Private Sub DeleteItem()
        If Not Request.Form("delitem") Is Nothing Then
            Dim i As Integer = 0
            If Int32.TryParse(Request.Form("delitem"), i) Then
                Arco.Doma.Library.Lists.ListItem.DeleteListItem(i)
            End If
        End If
    End Sub

    Private Sub InitCategories()
        moCats = New Dictionary(Of Integer, String)

        Dim lcolUsedcats As Arco.Doma.Library.Lists.ListItemCatList = Arco.Doma.Library.Lists.ListItemCatList.GetCategoriesForList(CurrentList.ID)
        Dim loCategories As Arco.Doma.Library.baseObjects.OBJECT_CATEGORYList = Arco.Doma.Library.baseObjects.OBJECT_CATEGORYList.GetOBJECT_CATEGORYList(False, Me.EnableIISCaching)
        For Each loUsedcat As Arco.Doma.Library.Lists.ListItemCatList.ListItemCatInfo In lcolUsedcats
            For Each loCategory As Arco.Doma.Library.baseObjects.OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In loCategories
                If loCategory.ID = loUsedcat.CatID Then
                    moCats.Add(loCategory.ID, loCategory.Name)
                End If
            Next
        Next
    End Sub

    Public Sub ListRights()
        If CurrentList.CanEditItems() Then
            listEditItemsRights = True
            lnkbtnAddNew.Visible = True

            If Not CurrentList.isHierarchical Then
                lnkbtnAddNew.NavigateUrl = "javascript:AddNewItem(" & CurrentList.ID & ");"
                If CurrentList.isCategoryDependant Then
                    lnkbtnCategory.NavigateUrl = "javascript:AddListItemCategory(" & CurrentList.ID & ");"
                    lnkbtnCategory.Visible = True
                End If
            Else
                lnkbtnAddNew.NavigateUrl = "javascript:AddNewItemLID2('" & CurrentList.ID & "','" & "0" & "','t0_" & "0" & "');"
            End If


        End If
    End Sub

    Public Function ShowPaging(ByVal liTotalRecs As Integer, ByVal liRecsPerPage As Integer) As String
        Dim lstable As String
        lstable = "<TR class='ListFooter' ><TD COLSPAN=" & miColCount & ">"

        Dim liFirstRec As Integer = ((liPage) * liRecsPerPage) + 1
        Dim liLastRec As Integer = liFirstRec + liRecsPerPage - 1
        If liTotalRecs > 0 Then
            lstable &= GridScroller.GetGridScroller(Me, liPage, liRecsPerPage, liTotalRecs)
        Else
            lstable = lstable & GetDecodedLabel("noresultsfound")
        End If
        lstable = lstable & "</TD></TR>"
        Return lstable

    End Function

    Private Sub AddHeaderCell(sb As StringBuilder, content As String)
        sb.Append("<th>")
        sb.Append(content)
        sb.Append("</th>")
    End Sub

    Private Sub AddHeaderCell(ByVal sb As StringBuilder, ByVal vsContent As String, ByVal vsOrderField As String)
        If Not String.IsNullOrEmpty(vsOrderField) Then
            sb.Append("<th onclick=""javascript:OrderBy('" & vsOrderField & "');"" style=""cursor:pointer"">")
        Else
            sb.Append("<th>")
        End If

        sb.Append(vsContent)
        sb.Append("</th>")
    End Sub

    Private Sub AddCell(ByVal sb As StringBuilder, ByVal vsContent As String)

        sb.Append("<td>")

        sb.Append(vsContent)
        sb.Append("</td>")
    End Sub

    Protected miColCount As Integer = 0
    Private Sub RenderHeaderRow(ByVal sbGrid As StringBuilder)
        sbGrid.Append("<tr >")
        miColCount = 3
        If CurrentList.HasCode Then
            AddHeaderCell(sbGrid, GetLabel("cm_code"), "ITEM_CODE")
            miColCount += 1
        End If
        AddHeaderCell(sbGrid, GetLabel("cm_name"), "ITEM_DESC")
        If CurrentList.MULTI_LINGUAL Then
            For Each loListLanguage As Arco.Doma.Library.Lists.ListLanguageList.ListLanguageInfo In Me.ListLanguages
                AddHeaderCell(sbGrid, GetDecodedLabel("lang" & loListLanguage.LANG_CODE), "DESC_" & loListLanguage.LANG_CODE)
                miColCount += 1
            Next
        End If
        If CurrentList.USE_SYNONYMS Then
            AddHeaderCell(sbGrid, GetDecodedLabel("cm_synonyms"))
            miColCount += 1
        End If

        If CurrentList.isCategoryDependant Then
            AddHeaderCell(sbGrid, GetDecodedLabel("cm_category"))
            miColCount += 1
        End If

        AddHeaderCell(sbGrid, GetDecodedLabel("cm_Active"), "ITEM_STATUS")

        If listEditItemsRights Then
            'edit/delete
            AddHeaderCell(sbGrid, "&nbsp;")
        End If

        sbGrid.Append("</tr>")

    End Sub
    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "ITEM_STATUS", "ITEM_CODE", "ITEM_DESC", "DESC_E", "DESC_N", "DESC_F", "DESC_G"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function
    Private Function GetItemDescription(ByVal loListItem As Arco.Doma.Library.Lists.ListItemList.ListItemInfo) As String
        Dim lsTemp As String = Server.HtmlEncode(loListItem.Description)
        If loListItem.ITEM_DESC_HOMO.Length > 0 Then lsTemp &= " [" & Server.HtmlEncode(loListItem.ITEM_DESC_HOMO) & "]"
        Return lsTemp
    End Function
    Private Function GetItemDescription(ByVal loListItem As Arco.Doma.Library.Lists.ListItem) As String
        Dim lsTemp As String = Server.HtmlEncode(loListItem.Description)
        If loListItem.ITEM_DESC_HOMO.Length > 0 Then lsTemp &= " [" & Server.HtmlEncode(loListItem.ITEM_DESC_HOMO) & "]"
        Return lsTemp
    End Function

    Private moSyns As Arco.Doma.Library.Lists.ListItemSynList
    Private ReadOnly Property Synonyms As Arco.Doma.Library.Lists.ListItemSynList
        Get
            If moSyns Is Nothing Then
                Dim crit As Arco.Doma.Library.Lists.ListItemSynList.Criteria = New Arco.Doma.Library.Lists.ListItemSynList.Criteria
                crit.ITEM_ID_LIST = msItemList
                moSyns = Arco.Doma.Library.Lists.ListItemSynList.GetListItemSynList(crit)
            End If
            Return moSyns
        End Get
    End Property

    Private moItemCats As Arco.Doma.Library.Lists.ListItemCatList
    Private ReadOnly Property ItemCategories As Arco.Doma.Library.Lists.ListItemCatList
        Get
            If moItemCats Is Nothing Then
                moItemCats = Arco.Doma.Library.Lists.ListItemCatList.GetCategoriesForListItems(msItemList)
            End If
            Return moItemCats
        End Get
    End Property

    Private Sub RenderItemRow(ByVal sbGrid As StringBuilder, ByVal loListItem As Arco.Doma.Library.Lists.ListItemList.ListItemInfo, ByVal AlternateRow As Boolean)

        sbGrid.Append("<tr>")

        If CurrentList.HasCode Then
            AddCell(sbGrid, loListItem.ITEM_CODE)
        End If
        AddCell(sbGrid, GetItemDescription(loListItem))
        If CurrentList.MULTI_LINGUAL Then
            For Each loListLanguage As Arco.Doma.Library.Lists.ListLanguageList.ListLanguageInfo In Me.ListLanguages
                Dim lsTransDesc As String = loListItem.Caption(loListLanguage.LANG_CODE, False, False)
                If Not String.IsNullOrEmpty(lsTransDesc) Then
                    Dim lsTransHomo As String = loListItem.Homonym(loListLanguage.LANG_CODE, False)
                    If lsTransHomo <> "" Then
                        lsTransDesc &= " [" & lsTransHomo & "]"
                    End If
                    AddCell(sbGrid, Server.HtmlEncode(lsTransDesc))
                Else
                    AddCell(sbGrid, "&nbsp;")
                End If
            Next
        End If
        If CurrentList.USE_SYNONYMS = True Then
            Dim lsSyns As String = ""
            For Each loSyn As Arco.Doma.Library.Lists.ListItemSynList.ListItemSynInfo In Me.Synonyms
                If loSyn.ITEM_ID = loListItem.ID Then
                    If lsSyns.Length > 0 Then lsSyns &= ", "
                    lsSyns &= Server.HtmlEncode(loSyn.SYN_DESC)
                End If
            Next
            AddCell(sbGrid, lsSyns)
        End If

        If CurrentList.isCategoryDependant Then
            Dim lsCat As String = ""
            For Each loItemCat As Arco.Doma.Library.Lists.ListItemCatList.ListItemCatInfo In ItemCategories
                If loItemCat.ItemID = loListItem.ID Then
                    If lsCat.Length > 0 Then lsCat &= ", "
                    lsCat &= moCats(loItemCat.CatID)
                End If
            Next
            AddCell(sbGrid, Server.HtmlEncode(lsCat))
        End If

        Dim lsStatus As String
        If loListItem.ITEM_STATUS = Arco.Doma.Library.Lists.ListItemList.ITEMSTATUS.Production Then
            lsStatus = GetDecodedLabel("cm_active")
        Else
            lsStatus = GetDecodedLabel("cm_disabled")
        End If
        AddCell(sbGrid, lsStatus)

        If listEditItemsRights Then
            'edit
            Dim editButton As String
            If CurrentList.isHierarchical Then
                editButton = "<a href='javascript:RefreshRightPaneLID2(" & moList.ID & "," & loListItem.ID & ");' class='icon icon-edit' title='" & GetLabel("edit") & "' ></a>"
            Else
                editButton = "<a href='javascript:EditItem(" & loListItem.ID & ");' class='icon icon-edit' title='" & GetLabel("edit") & "' ></a>"
            End If

            'delete
            AddCell(sbGrid, editButton + "<a href='javascript:DeleteItem(" & loListItem.ID & ");' class='icon icon-delete' title='" & GetLabel("delete") & "' ></a>")
        End If

        sbGrid.Append("</tr>")
    End Sub

    Private msItemList As String
    Private Sub RenderGrid(ByVal vlPage As Integer, ByVal liRecsPerPage As Integer)
        Dim sbGrid As StringBuilder = New StringBuilder
        sbGrid.Append(ArcoFormatting.FormatPanelLabel(CurrentList.Name, "<span class='Label'>" & GetLabel("search") & " :</span>&nbsp;&nbsp;<input type=""text"" name=""txtNameFilter"" value=""" & Server.HtmlEncode(Request.Form("txtNameFilter")) & """ size=""100"">&nbsp;&nbsp;<a href='javascript:Close();'><span class='icon icon-close icon-color-light'></span></a>"))
        sbGrid.Append("<table class='List HoverList PaddedTable'>")
        RenderHeaderRow(sbGrid)
        RenderFilterRow(sbGrid)
        Dim loCrit As Arco.Doma.Library.Lists.ListItemList.Criteria = New Arco.Doma.Library.Lists.ListItemList.Criteria()
        loCrit.LIST_ID = CurrentList.ID

        loCrit.IncludeLang = CurrentList.MULTI_LINGUAL
        loCrit.IncludeSyn = CurrentList.USE_SYNONYMS

        If Not String.IsNullOrEmpty(orderby.Value) Then
            loCrit.OrderBy = GetActualOrderBy()
        Else
            loCrit.OrderBy = "ITEM_CODE, ITEM_SEARCH"
        End If



        If Page.IsPostBack Then
            If CurrentList.isCategoryDependant Then
                loCrit.CAT_ID = Convert.ToInt32(Request.Form("txtCatFilter"))
            End If
            loCrit.Filter = Request.Form("txtNameFilter")
            If Not String.IsNullOrEmpty(Request.Form("txtStatusFilter")) Then
                loCrit.ITEM_STATUS = Convert.ToInt32(Request.Form("txtStatusFilter"))
            End If
        End If
        loCrit.Range.FirstItem = GridScroller.FirstRecord(vlPage, liRecsPerPage)
        loCrit.Range.LastItem = GridScroller.LastRecord(vlPage, liRecsPerPage)

        'loCrit.FIRSTRECORD = (TableGridView.PageIndex * TableGridView.PageSize) + 1
        ' loCrit.FIRSTRECORD = (vlPageIndex * TableGridView.PageSize) + 1
        'loCrit.LASTRECORD = loCrit.FIRSTRECORD + (TableGridView.PageSize - 1)

        Dim loListItems As Arco.Doma.Library.Lists.ListItemList = Arco.Doma.Library.Lists.ListItemList.GetListItemList(loCrit)

        While loListItems.Any AndAlso loCrit.Range.FirstItem > 1 AndAlso (loListItems.Count < loCrit.Range.FirstItem)
            loCrit.Range.FirstItem = loCrit.Range.FirstItem - liRecsPerPage
            loCrit.Range.LastItem = loCrit.Range.LastItem - liRecsPerPage
            loListItems = Arco.Doma.Library.Lists.ListItemList.GetListItemList(loCrit)
        End While

        If loListItems.Any Then
            msItemList = GetItemIDsString(loListItems, loCrit)
            Dim i As Integer = 1
            For Each loListItem As Arco.Doma.Library.Lists.ListItemList.ListItemInfo In loListItems
                If i >= loCrit.Range.FirstItem AndAlso i <= loCrit.Range.LastItem Then
                    RenderItemRow(sbGrid, loListItem, (i Mod 2 = 0))
                ElseIf i > loCrit.Range.LastItem Then
                    Exit For
                End If
                i += 1
            Next

        End If
        sbGrid.Append(ShowPaging(loListItems.Count, liRecsPerPage))

        sbGrid.AppendLine("<tr class='ListFooter'><td colspan=" & miColCount & ">")
        sbGrid.AppendLine(RenderControlAsHtmlString(lnkbtnCategory))
        sbGrid.AppendLine(RenderControlAsHtmlString(lnkbtnAddNew))
        sbGrid.AppendLine("</td></tr>")
        sbGrid.Append("</table>")

        plhResults.Text = sbGrid.ToString
    End Sub

    Private Function RenderControlAsHtmlString(control As Control) As String
        Dim sb As StringBuilder = New StringBuilder()
        Dim sw As StringWriter = New StringWriter(sb)
        Dim htmlWriter As Html32TextWriter = New Html32TextWriter(sw)

        control.RenderControl(htmlWriter)

        Return sb.ToString()

    End Function

    Private Function GetItemIDsString(ByVal voListItems As Arco.Doma.Library.Lists.ListItemList, ByVal loCrit As Arco.Doma.Library.Lists.ListItemList.Criteria) As String
        Dim sb As StringBuilder = New StringBuilder
        Dim i As Integer = 1
        For Each loListItem As Arco.Doma.Library.Lists.ListItemList.ListItemInfo In voListItems
            If i >= loCrit.Range.FirstItem AndAlso i <= loCrit.Range.LastItem Then
                If sb.Length > 0 Then sb.Append(",")
                sb.Append(loListItem.ID)
            ElseIf i > loCrit.Range.LastItem Then
                Exit For
            End If
            i += 1
        Next
        Return sb.ToString
    End Function

    Private Sub RenderFilterRow(ByVal sbGrid As StringBuilder)

        sbGrid.Append("<tr class='ListFilter'>")

        If CurrentList.HasCode Then
            AddCell(sbGrid, "&nbsp;")
        End If

        If CurrentList.MULTI_LINGUAL Then
            For Each loListLanguage As Arco.Doma.Library.Lists.ListLanguageList.ListLanguageInfo In Me.ListLanguages
                'AddTextBoxCell(sbGrid, "txtLangFilter" & loListLanguage.LANG_CODE)
                AddCell(sbGrid, "&nbsp;")
            Next
        End If
        AddCell(sbGrid, "&nbsp;")

        If CurrentList.USE_SYNONYMS = True Then
            'AddTextBoxCell(sbGrid, "txtSynFilter")
            AddCell(sbGrid, "&nbsp;")
        End If

        If CurrentList.isCategoryDependant Then
            Dim loCatItems As New List(Of ListItem)()
            loCatItems.Add(New ListItem(GetDecodedLabel("all"), "0"))
            For Each loCat As KeyValuePair(Of Integer, String) In moCats
                loCatItems.Add(New ListItem(loCat.Value, loCat.Key))
            Next
            AddComboCell(sbGrid, "txtCatFilter", loCatItems)
        End If

        Dim loStatusItems As List(Of ListItem) = New List(Of ListItem)()
        loStatusItems.Add(New ListItem(GetDecodedLabel("all"), ""))
        loStatusItems.Add(New ListItem(GetDecodedLabel("cm_active"), "0"))
        loStatusItems.Add(New ListItem(GetDecodedLabel("cm_disabled"), "9"))
        AddComboCell(sbGrid, "txtStatusFilter", loStatusItems)


        'edit/delete
        If listEditItemsRights Then
            AddCell(sbGrid, "&nbsp;")
        End If

        sbGrid.Append("</tr>")
    End Sub

    Private Sub AddComboCell(ByVal sb As StringBuilder, ByVal vsName As String, ByVal voListItems As List(Of ListItem))
        Dim lsValue As String = Request.Form(vsName)
        If lsValue Is Nothing Then lsValue = ""
        Dim sbSelect As StringBuilder = New StringBuilder
        sbSelect.Append("<select name=""" & vsName & """ id=""" & vsName & """ onclick=""javascript:SelectFilter('" & vsName & "','" & lsValue & "');"">")
        For Each loListItem As ListItem In voListItems
            sbSelect.Append("<option value='")
            sbSelect.Append(loListItem.Value)
            If loListItem.Value = lsValue Then
                sbSelect.Append("' selected>")
            Else
                sbSelect.Append("'>")
            End If
            sbSelect.Append(loListItem.Text)
            sbSelect.Append("</option>")
        Next
        sbSelect.Append("</select>")

        AddCell(sb, sbSelect.ToString)
    End Sub

End Class

