
Partial Class DM_Catalog_Listaddcategory
    Inherits BasePage

    Public Shared ltDT_Cat As New DataTable()
    Public Shared ltDT As New DataTable()
    Private addEmptyRow As Boolean = False
    Private moList As Arco.Doma.Library.Lists.List = Arco.Doma.Library.Lists.List.NewList()
    Private moItem As Arco.Doma.Library.Lists.ListItem = Arco.Doma.Library.Lists.ListItem.NewListItem(0)
    Private Const DENIED_PAGE As String = "../DM_ACL_DENIED.aspx"
    'Protected Modal As Boolean = False
    Private ShowDelete As Boolean

    Private Enum DTCols
        ID
        CODE
        DESC
    End Enum
    Private Enum DTColsCAT
        CATNAME
        CATID
    End Enum

    Protected Property CatPos() As Integer
        Get
            If ViewState("catpos") IsNot Nothing Then
                Return Convert.ToInt32(ViewState("catpos"))
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            ViewState("catpos") = value
        End Set
    End Property
    Protected Property ListItemPos() As Integer
        Get
            If ViewState("listItemPos") IsNot Nothing Then
                Return Convert.ToInt32(ViewState("listItemPos"))
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            ViewState("listItemPos") = value
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim lsDesc As String = ""
        'Acess granted.

        lblCategory.Text = GetLabel("cm_category")
        lblListItem.Text = GetLabel("cm_list_items")
        lblListItems.Text = GetLabel("cm_list_items")
        lnkbtnAdd.Text = GetLabel("add")
        If Not IsPostBack Then
            If ViewState("item_id") = Nothing Then
                ViewState("item_id") = QueryStringParser.GetString("id")
                selectedids.Value = QueryStringParser.GetString("selectedids")
                ViewState("listid") = QueryStringParser.GetString("listid")
                ViewState("parent") = QueryStringParser.GetString("parent")
            End If
        End If

        Page.Title = GetLabel("add") & GetLabel("category")

        If GetList(ViewState("listid")) Then
            catCreateTemplatedGridView(0)
           
        End If

        If Not IsPostBack Then
            AddCategoryItems()
            If moList.LIST_TYPE = "CONCEPTTREE" Or moList.LIST_TYPE = "THESAURUS" Or moList.LIST_TYPE = "UDCLIST" Then
                imgbtnBack.Visible = True
            End If
        End If

    End Sub

    Public Sub LoadListItems()

        cmbListItems.Items.Clear()
        cmbListItems.Items.Add(New ListItem("", ""))
        If cmbCategory.SelectedIndex > 0 Then
            Dim loCrit As Arco.Doma.Library.Lists.ListItemList.Criteria = New Arco.Doma.Library.Lists.ListItemList.Criteria()
            loCrit.LIST_ID = moList.ID
            loCrit.IncludeLang = moList.MULTI_LINGUAL
            loCrit.IncludeSyn = moList.USE_SYNONYMS
            loCrit.OrderBy = "ITEM_CODE, ITEM_SEARCH"

            For Each loListItem As Arco.Doma.Library.Lists.ListItemList.ListItemInfo In Arco.Doma.Library.Lists.ListItemList.GetListItemList(loCrit)
                If Not Arco.Doma.Library.Lists.ListItemCatList.IsItemLinkedToCat(loListItem.ID, cmbCategory.SelectedValue) Then
                    cmbListItems.Items.Add(New ListItem(loListItem.Description, loListItem.ID))
                End If
            Next
        End If
        

    End Sub

    Public Sub AddCategoryItems()
        If moList.LIST_TYPE = "CODELIST" Or moList.LIST_TYPE = "LIST" Then
            If moList.LIST_CATDEPENDANT Then
                cmbCategory.Items.Clear()
                cmbCategory.Items.Add(New System.Web.UI.WebControls.ListItem("", ""))
                Dim loCategories As Arco.Doma.Library.baseObjects.OBJECT_CATEGORYList = Arco.Doma.Library.baseObjects.OBJECT_CATEGORYList.GetOBJECT_CATEGORYList(False, Me.EnableIISCaching)
                For Each loCategory As Arco.Doma.Library.baseObjects.OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In loCategories
                    Dim flag As Boolean = True
                    For Each row As DataRow In ltDT_Cat.Rows
                        Dim lbl As String = TryCast(row("CATNAME"), String)
                        If lbl.Trim() = loCategory.Name Then
                            flag = False
                        End If
                    Next
                    If flag Then
                        cmbCategory.Items.Add(New System.Web.UI.WebControls.ListItem(loCategory.Name, loCategory.ID))
                    End If
                Next
                If cmbCategory.Items.Count <> 0 Then
                    cmbCategory.SelectedIndex = 0
                End If
            End If
        End If
    End Sub


    Private Function GetList(ByVal vlListID As Integer) As Boolean

        Dim lbOK As Boolean = False

        If vlListID > 0 Then
            moList = Arco.Doma.Library.Lists.List.GetList(vlListID)
            If moList.ID = vlListID Then
                lbOK = True
            End If
        End If

        Return lbOK

    End Function

    Private Function GetItem(ByVal vlItemID As Integer) As Boolean

        Dim lbOK As Boolean = False
        Dim lsItem As String = ""

        If vlItemID > 0 Then
            moItem = Arco.Doma.Library.Lists.ListItem.GetListItem(vlItemID)

            If moItem.ID = vlItemID Then
                lsItem = moItem.Description
                If moItem.ITEM_DESC_HOMO.Length > 0 Then
                    lsItem &= " [" & moItem.ITEM_DESC_HOMO & "]"
                End If
            End If
        Else
            moItem = Arco.Doma.Library.Lists.ListItem.GetListItem(moList.ID)
            lbOK = True

        End If

        Return lbOK

    End Function


    Protected Sub BacktoListItems(ByVal sender As Object, ByVal e As System.EventArgs) Handles imgbtnBack.ServerClick
        GotoListItems()
    End Sub

    Protected Sub GotoListItems()
        RefreshRightPane()
    End Sub

    Private Sub RefreshLeftPane()
        Dim sb As New StringBuilder
        sb.AppendLine("parent.RefreshLeftPane(" & moList.ID & "," & moItem.ID & ",'" & selectedids.Value & "')")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "refresh", sb.ToString, True)
    End Sub

    Private Sub RefreshRightPane()
        Dim sb As New StringBuilder
        sb.AppendLine("parent.RefreshRightPane(" & moList.ID & ")")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "refreshright", sb.ToString, True)
    End Sub


#Region "Category Dependant"

#Region " Insert/Delete/Fetch "



    Private Sub FetchRows(ByVal vlPageIndex As Integer)

        Dim loListItems As Arco.Doma.Library.Lists.ListItemList
        Dim loListItem As Arco.Doma.Library.Lists.ListItemList.ListItemInfo
        Dim loCrit As Arco.Doma.Library.Lists.ListItemList.Criteria = New Arco.Doma.Library.Lists.ListItemList.Criteria()

        'Dim loLangCrit As Lists. 
        Dim loCol As DataColumn

        Dim n As Integer = 0

        Dim llMax As Integer = 2
        Dim llPos As Integer = 0

        Dim lsTemp As String = ""


        listItemsGridView.Columns.Clear()


        ltDT = New DataTable()
        loCol = ltDT.Columns.Add("ID")
        loCol.Caption = "ID"
        loCol = ltDT.Columns.Add("CODE")
        loCol.Caption = GetLabel("cm_Code")

        loCol = ltDT.Columns.Add("NAME")
        loCol.Caption = GetLabel("cm_Name")



        loCrit.LIST_ID = moList.ID
        loCrit.Filter = ""

        loCrit.IncludeLang = moList.MULTI_LINGUAL
        loCrit.IncludeSyn = moList.USE_SYNONYMS
        'loCrit.FilterCode = FilterExpression
        loCrit.OrderBy = "ITEM_CODE, ITEM_SEARCH"

        loListItems = Arco.Doma.Library.Lists.ListItemList.GetListItemList(loCrit)
        If cmbCategory.SelectedIndex > 0 Then
            For Each loListItem In loListItems
                llPos += 1
                Dim laRowItems(llMax) As String
                laRowItems(DTCols.ID) = loListItem.ID
                laRowItems(DTCols.CODE) = loListItem.ITEM_CODE
                lsTemp = loListItem.Description
                If loListItem.ITEM_DESC_HOMO.Length > 0 Then lsTemp &= " [" & loListItem.ITEM_DESC_HOMO & "]"
                laRowItems(DTCols.DESC) = lsTemp

                If Arco.Doma.Library.Lists.ListItemCatList.IsItemLinkedToCat(loListItem.ID, cmbCategory.SelectedValue) Then
                    ltDT.Rows.Add(laRowItems)
                End If
                ShowDelete = True
            Next
        End If
       

        If ltDT.Rows.Count = 0 Then

            addEmptyRow = True
            ' Show(dummy)
            ShowDelete = False
            Dim laRowItems(llMax + n) As String
            If moList.LIST_TYPE = "CODELIST" Then
                laRowItems(DTCols.CODE) = GetLabel("noresultsfound")
                laRowItems(DTCols.DESC) = ""
                laRowItems(llMax) = ""
            Else
                laRowItems(DTCols.CODE) = ""
                laRowItems(DTCols.DESC) = ""
                laRowItems(llMax) = GetLabel("noresultsfound")
            End If
            


            ltDT.Rows.Add(laRowItems)
        End If

    End Sub

#End Region

#Region "Category TableGridView "

    Protected Sub AddCategory(ByVal sender As Object, ByVal e As System.EventArgs)
        If cmbCategory.SelectedIndex > 0 Then            
            If cmbListItems.SelectedIndex > 0 Then
                Arco.Doma.Library.Lists.ListItemCat.AddListItemCat(Convert.ToInt32(cmbListItems.SelectedItem.Value), Convert.ToInt32(cmbCategory.SelectedItem.Value))
                catCreateTemplatedGridView(0)
                AddCategoryItems()
                MsgPanel.Visible = False
                cmbCategory.SelectedIndex = CatPos
                ListItemPos = cmbListItems.SelectedIndex
                LoadListItems()
            Else
                ShowError(GetLabel("cm_please_enter_list_items"))
            End If
        Else
            ShowError(GetLabel("selcat"))
        End If
    End Sub

    Protected Sub ShowListItemsByCategory(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbCategory.SelectedIndexChanged
        catCreateTemplatedGridView(0)
        LoadListItems()
        CatPos = cmbCategory.SelectedIndex
    End Sub

    Protected Sub catGridView_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs) Handles listItemsGridView.RowDeleting

        Dim selectedRow As GridViewRow = listItemsGridView.Rows(e.RowIndex)
        Dim itemid As String = DirectCast(selectedRow.Cells(0).Controls(0), Label).Text

        If cmbCategory.SelectedIndex > 0 And itemid.Length > 0 Then
            Arco.Doma.Library.Lists.ListItemCat.DeleteListItemCat(Convert.ToInt32(itemid), Convert.ToInt32(cmbCategory.SelectedValue))
            catCreateTemplatedGridView(0)
            LoadListItems()
            cmbCategory.SelectedIndex = CatPos
        End If
    End Sub

    Protected Sub catGridView_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles listItemsGridView.PageIndexChanging
        catCreateTemplatedGridView(e.NewPageIndex)
        listItemsGridView.PageIndex = e.NewPageIndex
        listItemsGridView.DataBind()
        listItemsGridView.FooterRow.Visible = True
    End Sub

    Private Sub catCreateTemplatedGridView(ByVal vlNewPageIndex As Integer)
        FetchRows(vlNewPageIndex)

        For i As Integer = 0 To ltDT.Columns.Count - 1
            Dim ItemTmpField As New TemplateField()
            Select Case i
                Case 2
                    ItemTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, ltDT.Columns(i).Caption, ltDT.Columns(i).DataType.Name)
                    ItemTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, ltDT.Columns(i).ColumnName, ltDT.Columns(i).DataType.Name)
                    ItemTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, ltDT.Columns(i).ColumnName, "", "")
                Case 1
                    ItemTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, ltDT.Columns(i).Caption, ltDT.Columns(i).DataType.Name)
                    ItemTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, ltDT.Columns(i).ColumnName, ltDT.Columns(i).DataType.Name)
                    ItemTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, ltDT.Columns(i).ColumnName, "", "")
                Case 0
                    ItemTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, ltDT.Columns(i).Caption, ltDT.Columns(i).DataType.Name)
                    ItemTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, ltDT.Columns(i).ColumnName, ltDT.Columns(i).DataType.Name)
                    ItemTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, ltDT.Columns(i).ColumnName, "", "")
            End Select
            listItemsGridView.Columns.Add(ItemTmpField)
        Next

        Dim BtnTmpField As New TemplateField()
        If Not ShowDelete Then
            BtnTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, "", "") ' Delete Only
        Else
            BtnTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, "", "DeleteOnly") ' Delete Only
        End If
        BtnTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, "", "") ' Nothing
        BtnTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, "", "") ' Nothing
        BtnTmpField.ItemStyle.Width = Unit.Pixel(22)
        BtnTmpField.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        listItemsGridView.Columns.Add(BtnTmpField)

        ' Bind and display the data.
        If ltDT.Rows.Count() >= 0 Then
            listItemsGridView.DataSource = ltDT           
            listItemsGridView.Columns(0).Visible = False
            If moList.LIST_TYPE = "LIST" Then
                listItemsGridView.Columns(1).Visible = False
            End If
        End If
        listItemsGridView.DataBind()

    End Sub

#End Region

#End Region

#Region " Error handling "

    Private Sub ShowError(ByVal vsText As String)
        msg_lbl.Text = vsText
        MsgPanel.Visible = True
    End Sub

    Protected Sub Msg_button_Click(ByVal sender As Object, ByVal e As EventArgs)
        MsgPanel.Visible = False
    End Sub

#End Region

End Class
