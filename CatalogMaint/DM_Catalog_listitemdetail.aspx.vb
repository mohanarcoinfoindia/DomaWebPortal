
Partial Class DM_Catalog_listitemdetail
    Inherits BasePage
    Public Shared ltDT_Syn As New DataTable()
    Public Shared ltDT_Cat As New DataTable()
    Public Shared ltDT_Rels As New DataTable()
    Private listEditItemsRights As Boolean
    Private ShowDelete As Boolean
    Private ShowDelete_Rels As Boolean
    Private Enum DTCols_Rel
        ITEM1_ID
        DESC1
        LINK_TYPE
        ITEM2_ID
        DESC2

    End Enum

    Private Enum DTCols
        CATNAME
        CATID
    End Enum

#Region " Labels "
    Private Sub SetLabels()
        lblList1.Text = GetDecodedLabel("cm_list")
        lblCode.Text = GetDecodedLabel("cm_code")
        lblUdcCode.Text = GetDecodedLabel("cm_code")
        lblDesc.Text = GetDecodedLabel("description")
        lblheaderDescription.Text = GetDecodedLabel("description")
        lblDescHomo.Text = GetDecodedLabel("cm_homonym")
        lblHeaderHomonym.Text = GetDecodedLabel("cm_homonym")
        lblScopeNote.Text = GetDecodedLabel("cm_scope_note")
        lblHeaderScopeNote.Text = GetDecodedLabel("cm_scope_note")
        lblLanguages.Text = GetDecodedLabel("cm_translations")
        lblGerman.Text = GetDecodedLabel("langg")
        lblEnglish.Text = GetDecodedLabel("lange")
        lblFrench.Text = GetDecodedLabel("langf")
        lblDutch.Text = GetDecodedLabel("langn")

        lblSynonyms.Text = GetDecodedLabel("cm_synonyms")
        lblCategoryDependant.Text = GetDecodedLabel("cm_category_dependant")
        lblRelationships.Text = GetDecodedLabel("cm_relationships")
        imgbtnnew.Title = GetDecodedLabel("cm_new_item")
        imgbtnSave.ToolTip = GetDecodedLabel("but_save")
        imgbtnSaveAndClose.ToolTip = GetDecodedLabel("savecomplete")
        lblPath1.Text = GetDecodedLabel("cm_path")
        lblStatus.Text = GetDecodedLabel("cm_active")
        'imgbtnClose.ToolTip = GetDecodedLabel("delete")
        'lnkOpen.Text = GetDecodedLabel("Open in new window")
        lnkbtnAdd.Text = GetDecodedLabel("add")
        cmdRel1.Text = GetDecodedLabel("add")
        cmdRel2.Text = GetDecodedLabel("add")

        imgbtnSave.Text = "<span class='icon icon-save'></span>"
        imgbtnSaveAndClose.Text = "<span class='icon icon-save-exit'></span>"
    End Sub
#End Region

#Region " Properties "
    Private Property ItemID As Integer
        Get
            Return Convert.ToInt32(ViewState("item_id"))
        End Get
        Set(value As Integer)
            ViewState("item_id") = value            
        End Set
    End Property
    Private Property ListID As Integer
        Get
            Return Convert.ToInt32(ViewState("list_id"))
        End Get
        Set(value As Integer)
            moCurrentList = Nothing
            ViewState("list_id") = value
        End Set
    End Property
    Private Property ParentItemID As Integer
        Get
            Return Convert.ToInt32(ViewState("parent"))
        End Get
        Set(value As Integer)
            moCurrentParent = Nothing
            ViewState("parent") = value
        End Set
    End Property

    Private moCurrentParent As Arco.Doma.Library.Lists.ListItem = Nothing
    Public ReadOnly Property CurrentParent As Arco.Doma.Library.Lists.ListItem
        Get
            If Me.ParentItemID > 0 Then
                If moCurrentParent Is Nothing Then
                    moCurrentParent = Arco.Doma.Library.Lists.ListItem.GetListItem(Me.ParentItemID)
                End If
            End If
            Return moCurrentParent
        End Get
    End Property

    Private moCurrentList As Arco.Doma.Library.Lists.List = Nothing
    Public ReadOnly Property CurrentList As Arco.Doma.Library.Lists.List
        Get
            If moCurrentList Is Nothing Then
                If ListID = 0 AndAlso ItemID <> 0 Then
                    ListID = CurrentItem.LIST_ID
                End If

                moCurrentList = Arco.Doma.Library.Lists.List.GetList(ListID)
                If moCurrentList IsNot Nothing Then
                    lblList.Text = moCurrentList.LIST_TYPE & " - " & moCurrentList.Name
                Else
                    Throw New ArgumentException("List with ID " & ListID & " not found")
                End If
            End If
            Return moCurrentList
        End Get
    End Property
    
    Private moCurrentItem As Arco.Doma.Library.Lists.ListItem = Nothing
    Public ReadOnly Property CurrentItem As Arco.Doma.Library.Lists.ListItem
        Get
            If moCurrentItem Is Nothing Then
                If Me.ItemID > 0 Then
                    moCurrentItem = Arco.Doma.Library.Lists.ListItem.GetListItem(Me.ItemID)
                    If moCurrentItem IsNot Nothing Then
                        Dim lsItem As String = moCurrentItem.Description
                        If moCurrentItem.ITEM_DESC_HOMO.Length > 0 Then
                            lsItem &= " [" & moCurrentItem.ITEM_DESC_HOMO & "]"
                        End If
                        lblRel1.Text = lsItem
                        lblRel2.Text = lsItem
                        'Me.ListID = moCurrentItem.LIST_ID
                        '  Me.ParentItemID = moCurrentItem.ITEM_PARENT_ID

                        cmdSel1.NavigateUrl = "javascript:SearchRelatedItem(" & CurrentList.ID & ",'txtRel2');"
                        cmdSel1.Text = GetDecodedLabel("select")
                        cmdSel1.ImageUrl = "../Images/red1a.gif"

                        cmdSel2.NavigateUrl = "javascript:SearchRelatedItem(" & CurrentList.ID & ",'txtRel1');"
                        cmdSel2.Text = GetDecodedLabel("select")
                        cmdSel2.ImageUrl = "../Images/red1a.gif"

                    Else
                        Throw New ArgumentException("Item with ID " & Me.ItemID & " not found")
                    End If
                Else
                    moCurrentItem = Arco.Doma.Library.Lists.ListItem.NewListItem(Me.ListID)
                End If
            End If
            Return moCurrentItem
        End Get
    End Property

    Private moListLanguages As Arco.Doma.Library.Lists.ListLanguageList
    Public ReadOnly Property ListLanguages As Arco.Doma.Library.Lists.ListLanguageList
        Get
            If moListLanguages Is Nothing Then moListLanguages = Arco.Doma.Library.Lists.ListLanguageList.GetListLanguageList(CurrentList.ID)
            Return moListLanguages
        End Get
    End Property
    Private moIntLangs As Arco.Doma.Library.Globalisation.LanguageList
    Private ReadOnly Property InterfaceLanguages As Arco.Doma.Library.Globalisation.LanguageList
        Get
            If moIntLangs Is Nothing Then moIntLangs = Arco.Doma.Library.Globalisation.LanguageList.GetInterfaceLanguageList()
            Return moIntLangs
        End Get
    End Property
#End Region
    Private Sub LoadItemToForm()
        If CurrentList.LIST_TYPE = "UDCLIST" Then
            If CurrentItem.ITEM_ROOT_NODE Then
                rowUdcCode.Visible = False
                rowCode.Visible = True
                txtCode.Text = CurrentItem.ITEM_CODE
            Else
                rowUdcCode.Visible = True
                rowCode.Visible = False

                lblUdcParentCode.Text = CurrentItem.ITEM_CODE.Substring(0, CurrentItem.ITEM_CODE.LastIndexOf(".")).ToUpper
                Dim TCode As String = CurrentItem.ITEM_CODE.Substring(CurrentItem.ITEM_CODE.LastIndexOf("."), CurrentItem.ITEM_CODE.Length - CurrentItem.ITEM_CODE.LastIndexOf("."))
                TCode = TCode.Replace(".", "")
                Dim str() As String = TCode.Split(" ")
                If str.Length > 1 Then
                    txtUdcChildCode.Text = str(0).ToString()

                    For i = 1 To str.Length - 1 Step 1
                        txtUdcCodeExtension.Text &= str(i).ToString() & " "
                    Next
                    txtUdcCodeExtension.Text = txtUdcCodeExtension.Text.Trim()
                Else
                    txtUdcChildCode.Text = TCode.Trim()
                End If
                txtUdcChildCode.Width = 100
                txtCode.Text = lblUdcParentCode.Text & "." & txtUdcChildCode.Text.Trim() & " " & txtUdcCodeExtension.Text.Trim()
            End If
        Else
            txtCode.Text = CurrentItem.ITEM_CODE
        End If


        txtDesc.Text = CurrentItem.Description
        txtDescHomo.Text = CurrentItem.ITEM_DESC_HOMO
        txtScopeNote.Text = CurrentItem.ITEM_SCOPE_NOTE
        If CurrentList.MULTI_LINGUAL Then
            GetLangTransList()
            rowLangTrans.Visible = True

            If CurrentList.LIST_TYPE = "CODELIST" Or CurrentList.LIST_TYPE = "LIST" Or CurrentList.LIST_TYPE = "UDCLIST" Then
                rowNativeHomo.Visible = False
                rowNativeScopeNote.Visible = False
                lblheaderDescription.Visible = True
                lblHeaderHomonym.Visible = False
                lblHeaderScopeNote.Visible = False
            Else
                rowNativeHomo.Visible = False
                rowNativeScopeNote.Visible = False
                lblheaderDescription.Visible = True
                lblHeaderHomonym.Visible = True
                lblHeaderScopeNote.Visible = True
            End If
        Else
            rowLangTrans.Visible = False
            If CurrentList.LIST_TYPE = "CODELIST" Or CurrentList.LIST_TYPE = "LIST" Or CurrentList.LIST_TYPE = "UDCLIST" Then
                rowNativeHomo.Visible = False
                rowNativeScopeNote.Visible = False
            Else
                rowNativeHomo.Visible = True
                rowNativeScopeNote.Visible = True
            End If
        End If

        If CurrentList.isHierarchical Then
            rowPath.Visible = True
            lblPath.Text = Arco.Doma.Library.Lists.ListItem.GetPath(CurrentItem)
        End If

        If CurrentItem.ITEM_STATUS > 0 Then
            cbItemStatus.Checked = False
        Else
            cbItemStatus.Checked = True
        End If
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim lsDesc As String = ""
        ' Acess granted.
        SetLabels()

        If Not IsPostBack Then
            ItemID = QueryStringParser.GetInt("id")
            ParentItemID = QueryStringParser.GetInt("parent")
            ListID = QueryStringParser.GetInt("listid")
            selectedids.Value = QueryStringParser.GetString("selectedids")
        End If

        If Not CurrentList.CanEditItems() Then
            GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        End If

        If ItemID > 0 Then  ' Update Mode.

            AddNewItemUpdate()
            If Not IsPostBack Then
                Page.Title = GetDecodedLabel("cm_list_title_update")              
                LoadItemToForm()
            End If

            If CurrentList.USE_SYNONYMS Then
                rowSynonyms.Visible = True
                synCreateTemplatedGridView()
            End If

            If CurrentList.isCategoryDependant Then
                rowCategoryDependantList.Visible = True
                catCreateTemplatedGridView(0)
            End If

            If CurrentList.LIST_TYPE = "CONCEPTTREE" Or CurrentList.LIST_TYPE = "THESAURUS" Then
                rowRelationships.Visible = True
                CreateTemplatedGridView_Rels()            
            End If
            ListRights()

        Else
            ' Insert mode.
            Page.Title = GetDecodedLabel("cm_list_title_insert")
          
            AddNewItemInsert()
            If Not Page.IsPostBack Then
                If CurrentList.MULTI_LINGUAL Then
                    GetLangTransList()
                    rowLangTrans.Visible = True

                    If CurrentList.LIST_TYPE = "CODELIST" Or CurrentList.LIST_TYPE = "LIST" Or CurrentList.LIST_TYPE = "UDCLIST" Then
                        rowNativeHomo.Visible = False
                        rowNativeScopeNote.Visible = False
                        lblheaderDescription.Visible = True
                        lblHeaderHomonym.Visible = False
                        lblHeaderScopeNote.Visible = False
                    Else
                        rowNativeHomo.Visible = False
                        rowNativeScopeNote.Visible = False
                        lblheaderDescription.Visible = True
                        lblHeaderHomonym.Visible = True
                        lblHeaderScopeNote.Visible = True
                    End If
                Else
                    rowLangTrans.Visible = False
                    If CurrentList.LIST_TYPE = "CODELIST" Or CurrentList.LIST_TYPE = "LIST" Or CurrentList.LIST_TYPE = "UDCLIST" Then
                        rowNativeHomo.Visible = False
                        rowNativeScopeNote.Visible = False
                    Else
                        rowNativeHomo.Visible = True
                        rowNativeScopeNote.Visible = True
                    End If
                End If
                If CurrentList.isHierarchical Then
                    rowPath.Visible = True
                    If Not CurrentParent Is Nothing Then
                        lblPath.Text = Arco.Doma.Library.Lists.ListItem.GetPath(CurrentParent) & "\" & CurrentParent.Caption(Language, True, False, False)
                        If lblPath.Text.IndexOf("\") = 0 And lblPath.Text.Length > 0 Then
                            lblPath.Text = lblPath.Text.Remove(0, 1)
                        End If
                        If CurrentList.LIST_TYPE = "UDCLIST" Then
                            rowUdcCode.Visible = True
                            rowCode.Visible = False
                            lblUdcParentCode.Text = CurrentParent.ITEM_CODE
                            txtUdcChildCode.Width = 100
                        Else
                            rowUdcCode.Visible = False
                            rowCode.Visible = True
                        End If
                    Else
                        lblPath.Text = "\"
                    End If
                End If
            End If

        End If

        If CurrentList.LIST_TYPE = "CONCEPTTREE" OrElse CurrentList.LIST_TYPE = "THESAURUS" Then
            tblRelBuilder.Visible = True
        End If

        If Not CurrentList.HasCode Then
            txtCode.Enabled = False
            rowCode.Visible = False
            txtDesc.Focus()
        Else
            txtCode.Focus()
            If CurrentList.LIST_TYPE = "UDCLIST" Then
                txtCode.Attributes.Add("onkeydown", "validateNumber(event,this)")
                If rowUdcCode.Visible Then
                    txtUdcChildCode.Focus()
                End If
            End If
        End If          
            If Not IsPostBack Then
                AddCategoryItems()
            If CurrentList.isHierarchical Then
                imgbtnBack.Visible = True
            End If
            End If
    End Sub
    Protected Sub ListRights()
        '   listEditItemsRights = CurrentList.CanEditItems()

        '   If listEditItemsRights Then
        '  imgbtnnew.Visible = True
        'imgbtnSave.Visible = True
        'imgbtnSaveAndClose.Visible = True
        If CurrentList.LIST_TYPE = "CONCEPTTREE" Or CurrentList.LIST_TYPE = "THESAURUS" Then
                tblRelBuilder.Visible = True
            End If
        '   End If

        'If Not listEditItemsRights Then
        '    imgbtnnew.Visible = False
        '    imgbtnSave.Visible = False
        '    imgbtnSaveAndClose.Visible = False
        '    If CurrentList.USE_SYNONYMS Then
        '        synGridView.Columns(4).Visible = False
        '    End If
        '    If CurrentList.isCategoryDependant Then
        '        catGridView.Columns(2).Visible = False
        '        lnkbtnAdd.Visible = False
        '    End If
        '    If CurrentList.LIST_TYPE = "CONCEPTTREE" Or CurrentList.LIST_TYPE = "THESAURUS" Then
        '        tblRelBuilder.Visible = False
        '        TableGridViewRels.Columns(6).Visible = False
        '    End If
        'End If
    End Sub
    Public Sub AddCategoryItems()
        If CurrentList.isCategoryDependant Then
            cmbCat.Items.Clear()
            cmbCat.Items.Add(New ListItem("", ""))
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
                    cmbCat.Items.Add(New ListItem(loCategory.Name, loCategory.ID))
                End If
            Next
            If cmbCat.Items.Count <> 0 Then
                cmbCat.SelectedIndex = 0
            End If
        End If
    End Sub


    Public Sub GetLangTransList()      
        For Each loLanguage As Arco.Doma.Library.Globalisation.LanguageList.LanguageInfo In Me.InterfaceLanguages
            Dim lbFound As Boolean = False

            Dim lsCode As String = loLanguage.Code
            For Each loListLanguage As Arco.Doma.Library.Lists.ListLanguageList.ListLanguageInfo In Me.ListLanguages
                If loListLanguage.LANG_CODE = loLanguage.InterfaceLanguageCode Then
                    Dim lsDesc As String = ""
                    Dim lsHomo As String = ""
                    Dim lsScopeNote As String = ""
                    If ItemID > 0 Then
                        Dim loItemLang As Arco.Doma.Library.Lists.ListItemLanguage = Arco.Doma.Library.Lists.ListItemLanguage.GetListItemLanguage(ItemID, loListLanguage.LANG_CODE)
                        lsDesc = loItemLang.ITEM_DESC
                        lsHomo = loItemLang.ITEM_DESC_HOMO
                        lsScopeNote = loItemLang.ITEM_SCOPE_NOTE
                    End If
                    Select Case loListLanguage.LANG_CODE
                        Case "G"                          
                            txtGerman.Text = lsDesc
                            If CurrentList.LIST_TYPE = "THESAURUS" OrElse CurrentList.LIST_TYPE = "CONCEPTTREE" Then
                                txtGermanHomo.Text = lsHomo
                                txtGermanScopeNote.Text = lsScopeNote
                            Else
                                txtGermanHomo.Visible = False
                                txtGermanScopeNote.Visible = False
                            End If
                            rowGerman.Visible = True
                        Case "E"                          
                            txtEnglish.Text = lsDesc
                            If CurrentList.LIST_TYPE = "THESAURUS" OrElse CurrentList.LIST_TYPE = "CONCEPTTREE" Then
                                txtEnglishHomo.Text = lsHomo
                                txtEnglishScopeNote.Text = lsScopeNote
                            Else
                                txtEnglishHomo.Visible = False
                                txtEnglishScopeNote.Visible = False
                            End If
                            rowEnglish.Visible = True

                        Case "F"                           
                            txtFrench.Text = lsDesc
                            If CurrentList.LIST_TYPE = "THESAURUS" OrElse CurrentList.LIST_TYPE = "CONCEPTTREE" Then
                                txtFrenchHomo.Text = lsHomo
                                txtFrenchScopeNote.Text = lsScopeNote
                            Else
                                txtFrenchHomo.Visible = False
                                txtFrenchScopeNote.Visible = False
                            End If
                            rowFrench.Visible = True
                        Case "N"                            
                            txtDutch.Text = lsDesc
                            If CurrentList.LIST_TYPE = "THESAURUS" OrElse CurrentList.LIST_TYPE = "CONCEPTTREE" Then
                                txtDutchHomo.Text = lsHomo
                                txtDutchScopeNote.Text = lsScopeNote
                            Else
                                txtDutchHomo.Visible = False
                                txtDutchScopeNote.Visible = False
                            End If
                            rowDutch.Visible = True
                    End Select
                    Exit For
                End If
            Next           
        Next
      
    End Sub


    Protected Sub BacktoListItems(ByVal sender As Object, ByVal e As System.EventArgs) Handles imgbtnBack.ServerClick
        GotoListItems()
    End Sub

    Protected Sub GotoListItems()
        RefreshRightPane()
    End Sub

    Private Sub RefreshLeftPane()

        Dim sb As New StringBuilder
        sb.AppendLine("parent.RefreshLeftPane(" & CurrentList.ID & "," & CurrentItem.ID & ",'" & selectedids.Value & "');")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "refresh", sb.ToString, True)
    End Sub

    Private Sub RefreshRightPane()
        Dim sb As New StringBuilder
        sb.AppendLine("parent.RefreshRightPane(" & CurrentList.ID & ");")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "refreshright", sb.ToString, True)
    End Sub
    Private Sub Close()
        Dim sb As New StringBuilder
        sb.AppendLine("Close();")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "close", sb.ToString, True)
    End Sub

    Public Overrides Function GetLabel(ByVal vsLabel As String) As String

        Dim lsNewLabel As String = "CM_" & vsLabel.Replace(" ", "_")

        Try
            lsNewLabel = Arco.Web.ResourceManager.GetString(lsNewLabel.ToLower, Arco.Security.BusinessIdentity.CurrentIdentity.Language)
        Catch ex As Exception
            lsNewLabel = ""
        End Try

        If lsNewLabel.Length = 0 Then
            Try
                lsNewLabel = Arco.Web.ResourceManager.GetString(vsLabel.ToLower, Arco.Security.BusinessIdentity.CurrentIdentity.Language)
            Catch ex As Exception
                lsNewLabel = ""
            End Try
        End If

        If lsNewLabel.Length = 0 Then
            lsNewLabel = vsLabel
        End If

        Return lsNewLabel

    End Function
    Public Overrides Function GetDecodedLabel(ByVal vsLabel As String) As String

        Dim lsNewLabel As String = vsLabel

        Try
            lsNewLabel = Arco.Web.ResourceManager.GetString(lsNewLabel.ToLower, Arco.Security.BusinessIdentity.CurrentIdentity.Language, False)
        Catch ex As Exception
            lsNewLabel = ""
        End Try

        If lsNewLabel.Length = 0 Then
            Try
                lsNewLabel = Arco.Web.ResourceManager.GetString(vsLabel.ToLower, Arco.Security.BusinessIdentity.CurrentIdentity.Language, False)
            Catch ex As Exception
                lsNewLabel = ""
            End Try
        End If

        If lsNewLabel.Length = 0 Then
            lsNewLabel = vsLabel
        End If

        Return lsNewLabel

    End Function


    Protected Sub AddNewItemUpdate()
        If CurrentItem.ID > 0 Then 'force loading also

            Dim sb As New StringBuilder
            If CurrentList.isHierarchical Then
                sb.AppendLine("function AddNewItem(){//update")
                sb.AppendLine("parent.AddNewItemLID2(" & CurrentList.ID & "," & findparent(CurrentItem) & ",'" & selectedids.Value & "');")
                sb.AppendLine("}")
            Else
                sb.AppendLine("function AddNewItem(){")
                sb.AppendLine("parent.AddNewItem('" & CurrentList.ID & "'); ")
                sb.AppendLine("}")
            End If
            Page.ClientScript.RegisterStartupScript(GetType(String), "AddNewItem", sb.ToString, True)

        End If

    End Sub

    Protected Sub AddNewItemInsert()
        If CurrentList.ID > 0 Then

            Dim sb As New StringBuilder
            If CurrentList.isHierarchical Then
               
                sb.AppendLine("function AddNewItem(){//insert")
                sb.AppendLine("parent.AddNewItemLID2(" & CurrentList.ID & "," & ParentItemID & ",'" & selectedids.Value & "');")
                sb.AppendLine("}")
            Else
                sb.AppendLine("function AddNewItem(){")
                sb.AppendLine("parent.AddNewItem('" & CurrentList.ID & "'); ")
                sb.AppendLine("}")
            End If
            Page.ClientScript.RegisterStartupScript(GetType(String), "AddNewItem", sb.ToString, True)

        End If

    End Sub

    Protected Sub DoSave(ByVal sender As Object, ByVal e As System.EventArgs)

        Save(False)
    End Sub

    Protected Sub DoSaveClose(ByVal sender As Object, ByVal e As System.EventArgs)
        Save(True)
    End Sub

    Private Sub Save(ByVal vbDoClose As Boolean)

        ClearError()

        Dim lbIsNew As Boolean = CurrentItem.IsNew

        Dim lbProceed As Boolean = True
        Dim lbNotUnique, lbHomonymIsMissing, lbLeadTermAlreadyExists As Boolean
       
        Dim udccode As String = ""      
        Dim regexCode As Regex = New Regex("^\d+$") ' Allows only numbers
        udccode = lblUdcParentCode.Text & "." & txtUdcChildCode.Text.Trim() & " " & txtUdcCodeExtension.Text.Replace(".", "").Trim()
        If txtDesc.Text.Trim().Length = 0 Then
            ShowError(GetDecodedLabel("cm_enter_description"))
            lbProceed = False
        ElseIf txtCode.Text.Trim().Length = 0 AndAlso (CurrentList.HasCode) AndAlso rowCode.Visible Then
            ShowError(GetDecodedLabel("cm_enter_code"))
            lbProceed = False     
        ElseIf txtUdcChildCode.Text.Trim().Length = 0 AndAlso rowUdcCode.Visible AndAlso (CurrentList.LIST_TYPE = "UDCLIST") Then
            ShowError(GetDecodedLabel("cm_enter_code"))
            lbProceed = False
        ElseIf Not regexCode.Match(txtUdcChildCode.Text.Trim()).Success AndAlso Not CurrentItem.ITEM_ROOT_NODE AndAlso CurrentList.LIST_TYPE = "UDCLIST" AndAlso Not lbIsNew Then
            ShowError(GetDecodedLabel("cm_invalid_code"))
            lbProceed = False
        ElseIf Not regexCode.Match(txtCode.Text.Trim()).Success AndAlso rowCode.Visible AndAlso CurrentList.LIST_TYPE = "UDCLIST" Then
            ShowError(GetDecodedLabel("cm_invalid_code"))
            lbProceed = False
        ElseIf txtCode.Text.Length > 30 Or udccode.Length > 30 Then
            ShowError(GetDecodedLabel("cm_code_exceeds_limit"))
            lbProceed = False
        Else
            If CurrentList.LIST_TYPE = "CONCEPTTREE" Or CurrentList.LIST_TYPE = "CODELIST" Then
                lbProceed = Arco.Doma.Library.Lists.ListItem.IsCodeUnique(CurrentItem.ID, CurrentList.ID, txtCode.Text.Trim())
                If Not lbProceed Then
                    ShowError(GetDecodedLabel("cm_code_not_unique"))
                End If
            End If
            If CurrentList.LIST_TYPE = "UDCLIST" Then

                If lbIsNew And rowCode.Visible Then
                    lbProceed = Arco.Doma.Library.Lists.ListItem.IsCodeUnique(CurrentItem.ID, CurrentItem.LIST_ID, txtCode.Text.Trim())
                ElseIf lbIsNew And rowUdcCode.Visible Then
                    lbProceed = Arco.Doma.Library.Lists.ListItem.IsCodeUnique(CurrentItem.ID, CurrentItem.LIST_ID, udccode)
                ElseIf Not lbIsNew And CurrentItem.ITEM_ROOT_NODE Then
                    lbProceed = Arco.Doma.Library.Lists.ListItem.IsCodeUnique(CurrentItem.ID, CurrentItem.LIST_ID, txtCode.Text.Trim())
                ElseIf Not lbIsNew And Not CurrentItem.ITEM_ROOT_NODE Then
                    lbProceed = Arco.Doma.Library.Lists.ListItem.IsCodeUnique(CurrentItem.ID, CurrentItem.LIST_ID, udccode)
                End If
                If lbProceed = False Then
                    ShowError(GetDecodedLabel("cm_code_not_unique"))
                End If
            End If

            If lbProceed = True Then
                If CurrentList.isHierarchical Then
                    If lbIsNew Then
                        ' Insert mode.
                        CurrentItem.ITEM_PARENT_ID = ParentItemID
                    End If
                End If

                Select Case CurrentList.LIST_TYPE
                    Case "UDCLIST"
                        ' Insert mode.

                        If lbIsNew AndAlso rowCode.Visible Then
                            CurrentItem.ITEM_CODE = txtCode.Text.Trim()
                        ElseIf lbIsNew AndAlso rowUdcCode.Visible Then
                            CurrentItem.ITEM_CODE = udccode.Trim()
                        ElseIf Not lbIsNew AndAlso CurrentItem.ITEM_ROOT_NODE Then
                            CurrentItem.ITEM_CODE = txtCode.Text.Trim()
                        ElseIf Not lbIsNew And Not CurrentItem.ITEM_ROOT_NODE Then
                            CurrentItem.ITEM_CODE = udccode.Trim()
                        End If
                        CurrentItem.ITEM_ROOT_NODE = (CurrentItem.ITEM_CODE.IndexOf(".") = -1)
                    Case Else
                        If lbIsNew Then
                            ' Insert mode.
                            CurrentItem.ITEM_ROOT_NODE = (ParentItemID = 0)
                        End If
                        CurrentItem.ITEM_CODE = txtCode.Text.Trim()
                End Select

                lbProceed = Arco.Doma.Library.Lists.ListItem.IsDescriptionValid(CurrentItem.ID, CurrentItem.LIST_ID, CurrentItem.ITEM_PARENT_ID, CurrentItem.ITEM_ROOT_NODE, txtDesc.Text.Trim(), txtDescHomo.Text.Trim(), lbNotUnique, lbHomonymIsMissing, lbLeadTermAlreadyExists)
                If lbProceed Then
                    Try
                        CurrentItem.Description = txtDesc.Text.Trim()
                        CurrentItem.ITEM_DESC_HOMO = txtDescHomo.Text.Trim()
                        CurrentItem.ITEM_SCOPE_NOTE = txtScopeNote.Text.Trim()
                        If cbItemStatus.Checked Then
                            CurrentItem.ITEM_STATUS = Arco.Doma.Library.Lists.ListItem.ITEMSTATUS.Production
                        Else
                            CurrentItem.ITEM_STATUS = Arco.Doma.Library.Lists.ListItem.ITEMSTATUS.Disabled
                        End If

                        If lbIsNew Then
                            ' Insert mode.
                            CurrentItem.ITEM_TYPE = If(txtDescHomo.Text.Trim().Length > 0, "HOMO", "")
                        Else
                            If txtDescHomo.Text.Trim().Length > 0 Then
                                ' Homonym
                                Select Case CurrentItem.ITEM_TYPE
                                    Case "MBT", "HOMOMBT"
                                        CurrentItem.ITEM_TYPE = "HOMOMBT"
                                    Case "", "HOMO"
                                        CurrentItem.ITEM_TYPE = "HOMO"
                                End Select
                            Else
                                ' No homonym.
                                Select Case CurrentItem.ITEM_TYPE
                                    Case "MBT", "HOMOMBT"
                                        CurrentItem.ITEM_TYPE = "MBT"
                                    Case "", "HOMO"
                                        CurrentItem.ITEM_TYPE = ""
                                End Select
                            End If
                        End If



                        If Not CurrentList.LIST_TYPE = "UDCLIST" Then
                            moCurrentItem = CurrentItem.Save()
                            ItemID = moCurrentItem.ID
                        Else

                            If Not lbIsNew Then
                                Dim loUdcChilds As Arco.Doma.Library.Lists.ListItemList = Arco.Doma.Library.Lists.ListItemList.GetUDCChildren(CurrentItem.LIST_ID, CurrentItem.ITEM_CODE)
                                If loUdcChilds.Any Then
                                    Dim loNewParentCode As String = ""
                                    Dim loChildCode As String = ""
                                    Dim lbCodeExceeds As Boolean = False
                                    If rowCode.Visible Then
                                        loNewParentCode = txtCode.Text.Trim() & "."
                                    ElseIf rowUdcCode.Visible Then
                                        loNewParentCode = udccode.Trim()
                                    End If
                                    'Loop for checking the child nodes dont exceed 30 characters
                                    For Each loChild As Arco.Doma.Library.Lists.ListItemList.ListItemInfo In loUdcChilds
                                        Dim lochilditem As Arco.Doma.Library.Lists.ListItem = Arco.Doma.Library.Lists.ListItem.GetListItem(loChild.ID)
                                        If rowCode.Visible Then
                                            loChildCode = ReplaceFirst(lochilditem.ITEM_CODE.ToLower(), CurrentItem.ITEM_CODE.ToLower() & ".", loNewParentCode.ToLower())
                                        ElseIf rowUdcCode.Visible Then
                                            loChildCode = ReplaceFirst(lochilditem.ITEM_CODE.ToLower(), CurrentItem.ITEM_CODE.ToLower(), loNewParentCode.ToLower())
                                        End If
                                        If loChildCode.Length > 30 Then
                                            lbCodeExceeds = True
                                            Exit For
                                        End If
                                    Next

                                    If lbCodeExceeds Then
                                        ShowError(GetDecodedLabel("cm_child_code_exceeds_limit"))
                                        Return
                                    Else
                                        Dim loUdcChilds1 As Arco.Doma.Library.Lists.ListItemList = Arco.Doma.Library.Lists.ListItemList.GetUDCChildren(CurrentItem.LIST_ID, CurrentItem.ITEM_CODE)
                                        'Loop for renaming  the child nodes
                                        For Each loChild As Arco.Doma.Library.Lists.ListItemList.ListItemInfo In loUdcChilds1
                                            Dim lochilditem As Arco.Doma.Library.Lists.ListItem = Arco.Doma.Library.Lists.ListItem.GetListItem(loChild.ID)
                                            If rowCode.Visible Then
                                                lochilditem.ITEM_CODE = ReplaceFirst(lochilditem.ITEM_CODE.ToLower(), CurrentItem.ITEM_CODE.ToLower() & ".", loNewParentCode.ToLower())
                                            ElseIf rowUdcCode.Visible Then
                                                lochilditem.ITEM_CODE = ReplaceFirst(lochilditem.ITEM_CODE.ToLower(), CurrentItem.ITEM_CODE.ToLower(), loNewParentCode.ToLower())
                                            End If

                                            lochilditem = lochilditem.Save()
                                        Next
                                        moCurrentItem = CurrentItem.Save()
                                        ItemID = moCurrentItem.ID
                                    End If

                                Else
                                    If (udccode.Trim.Length <= 30 Or txtCode.Text.Trim.Length) Then
                                        moCurrentItem = CurrentItem.Save()
                                        ItemID = moCurrentItem.ID
                                    Else
                                        ShowError(GetDecodedLabel("cm_code_exceeds_limit"))
                                        Return
                                    End If
                                End If
                            Else 'new
                                If (udccode.Trim.Length <= 30 Or txtCode.Text.Trim.Length) Then
                                    moCurrentItem = CurrentItem.Save()
                                    ItemID = moCurrentItem.ID
                                Else
                                    ShowError(GetDecodedLabel("cm_code_exceeds_limit"))
                                    Return
                                End If
                            End If

                        End If
                        ' RefreshRightPane()
                        'Save Language Translations
                        For Each loLanguage As Arco.Doma.Library.Globalisation.LanguageList.LanguageInfo In Me.InterfaceLanguages

                            For Each loListLanguage As Arco.Doma.Library.Lists.ListLanguageList.ListLanguageInfo In Me.ListLanguages
                                If loListLanguage.LANG_CODE = loLanguage.InterfaceLanguageCode Then
                                    Dim loItemLang As Arco.Doma.Library.Lists.ListItemLanguage
                                    If lbIsNew Then
                                        loItemLang = Arco.Doma.Library.Lists.ListItemLanguage.NewListItemLanguage(CurrentItem.ID, loListLanguage.LANG_CODE)
                                    Else
                                        loItemLang = Arco.Doma.Library.Lists.ListItemLanguage.GetListItemLanguage(CurrentItem.ID, loListLanguage.LANG_CODE)
                                        If loItemLang.ITEM_ID = 0 Then loItemLang = Arco.Doma.Library.Lists.ListItemLanguage.NewListItemLanguage(CurrentItem.ID, loListLanguage.LANG_CODE)
                                    End If

                                    Select Case loListLanguage.LANG_CODE
                                        Case "G"
                                            loItemLang.ITEM_DESC = txtGerman.Text.Trim()
                                            loItemLang.ITEM_DESC_HOMO = txtGermanHomo.Text.Trim()
                                            loItemLang.ITEM_SCOPE_NOTE = txtGermanScopeNote.Text.Trim()

                                            loItemLang = loItemLang.Save()
                                        Case "E"

                                            loItemLang.ITEM_DESC = txtEnglish.Text.Trim()
                                            loItemLang.ITEM_DESC_HOMO = txtEnglishHomo.Text.Trim()
                                            loItemLang.ITEM_SCOPE_NOTE = txtEnglishScopeNote.Text.Trim()
                                            loItemLang = loItemLang.Save()
                                        Case "F"
                                            loItemLang.ITEM_DESC = txtFrench.Text.Trim()
                                            loItemLang.ITEM_DESC_HOMO = txtFrenchHomo.Text.Trim()
                                            loItemLang.ITEM_SCOPE_NOTE = txtFrenchScopeNote.Text.Trim()
                                            loItemLang = loItemLang.Save()
                                        Case "N"

                                            loItemLang.ITEM_DESC = txtDutch.Text.Trim()
                                            loItemLang.ITEM_DESC_HOMO = txtDutchHomo.Text.Trim()
                                            loItemLang.ITEM_SCOPE_NOTE = txtDutchScopeNote.Text.Trim()
                                            loItemLang = loItemLang.Save()

                                    End Select
                                    Exit For
                                End If
                            Next

                        Next
                        If vbDoClose Then
                            If CurrentList.isHierarchical Then
                                RefreshLeftPane()
                                RefreshRightPane()
                            Else
                                Close()
                            End If
                        Else
                            If CurrentList.isHierarchical Then
                                lblRel1.Text = CurrentItem.Description
                                lblRel2.Text = CurrentItem.Description

                                RefreshLeftPane()
                            End If
                            If CurrentItem.ID > 0 And lbIsNew Then
                                Try
                                    Dim sb As New StringBuilder
                                    If CurrentList.isHierarchical Then
                                        If ParentItemID > 0 Then
                                            sb.AppendLine("Sys.Application.add_load(function (){RefreshRightPaneLID2('" & CurrentList.ID & "','" & CurrentItem.ID & "','t" & CurrentItem.ITEM_PARENT_ID & "_" & CurrentItem.ID & "');});")
                                        Else
                                            sb.AppendLine("Sys.Application.add_load(function (){RefreshRightPaneLID2(" & CurrentList.ID & "," & CurrentItem.ID & ",'t0_" & CurrentItem.ID & "');});")
                                        End If
                                    End If
                                    Page.ClientScript.RegisterStartupScript(GetType(String), "refresh", sb.ToString, True)
                                Catch ex As Exception
                                    ShowError(ex.ToString)
                                End Try
                            ElseIf Not lbIsNew Then
                                If CurrentList.LIST_TYPE = "CONCEPTTREE" Or CurrentList.LIST_TYPE = "THESAURUS" Then
                                    CreateTemplatedGridView_Rels()
                                End If
                            End If
                            MsgPanel.Visible = False
                        End If

                    Catch ex As Exception
                        ShowError(ex.ToString)
                    End Try
                Else
                    If lbNotUnique Then
                        ShowError(GetDecodedLabel("cm_desc_not_unique"))
                    ElseIf lbHomonymIsMissing Then
                        ShowError(GetDecodedLabel("cm_homonym_missing"))
                    ElseIf lbLeadTermAlreadyExists Then
                        ShowError(GetDecodedLabel("cm_desc_exists_without_synonym"))
                    End If
                End If
            End If
        End If

    End Sub

#Region " Synonyms "

#Region " Insert/Update/Delete/Fetch "

    Private Sub DeleteRow(ByVal vlRowIndex As Integer)

        Dim loRow As GridViewRow = synGridView.Rows(vlRowIndex)
        Dim llSynID As Integer = Val(DirectCast(loRow.FindControl(ltDT_Syn.Columns(0).ColumnName), Label).Text)

        If llSynID > 0 Then
            Try
                Arco.Doma.Library.Lists.ListItemSyn.DeleteListItemSyn(llSynID)
                synCreateTemplatedGridView()
                synGridView.FooterRow.Visible = True
                ' RefreshLeftPane()
            Catch ex As Exception
                ShowError(ex.Message)
            End Try
        End If

    End Sub

    Private Sub InsertRow()

        Dim loRow As GridViewRow = synGridView.HeaderRow
        Dim loListItemSyn As Arco.Doma.Library.Lists.ListItemSyn
        Dim laParameters As New ArrayList()
        Dim lbOK As Boolean = False

        For i As Integer = 0 To ltDT_Syn.Columns.Count - 1
            Dim loType As Type

            loType = loRow.FindControl(ltDT_Syn.Columns(i).ColumnName).GetType

            Select Case loType.FullName
                Case "System.Web.UI.WebControls.TextBox"
                    Dim lsValue As String = DirectCast(loRow.FindControl(ltDT_Syn.Columns(i).ColumnName), TextBox).Text.Trim()
                    laParameters.Add(lsValue)
                Case "System.Web.UI.WebControls.DropDownList"
                    Dim lsValue As String = DirectCast(loRow.FindControl(ltDT_Syn.Columns(i).ColumnName), DropDownList).SelectedValue
                    laParameters.Add(lsValue)
            End Select

        Next

        Try
            If Convert.ToString(laParameters(1)).Length > 0 Then


                If Not Arco.Doma.Library.Lists.ListItemSyn.SynonymExists(CurrentItem.ID, laParameters(1), laParameters(2)) = True Then

                    loListItemSyn = Arco.Doma.Library.Lists.ListItemSyn.NewListItemSyn("")

                    loListItemSyn.ITEM_ID = CurrentItem.ID
                    loListItemSyn.SYN_DESC = laParameters(1)

                    If CurrentList.MULTI_LINGUAL Then
                        loListItemSyn.LANG_CODE = laParameters(2)
                    Else
                        loListItemSyn.LANG_CODE = "E"
                    End If

                    loListItemSyn.Save()

                    synCreateTemplatedGridView()

                    'synGridView.FooterRow.Visible = True
                    lbOK = True
                    MsgPanel.Visible = False
                Else
                    ShowError(GetDecodedLabel("cm_synonym_not_unique"))
                End If
            Else
                ShowError(GetDecodedLabel("cm_please_enter_synonym"))
            End If
        Catch ex As Exception
            ShowError(ex.ToString)
        Finally
            loListItemSyn = Nothing
        End Try

        'If lbOK = True Then RefreshLeftPane()

    End Sub

    Private Sub UpdateRow(ByVal vlRowIndex As Integer)

        Dim loRow As GridViewRow = synGridView.Rows(vlRowIndex)
        Dim loListItemSyn As Arco.Doma.Library.Lists.ListItemSyn
        Dim laParameters As New ArrayList()
        Dim lbOK As Boolean = False

        Try
            For i As Integer = 0 To ltDT_Syn.Columns.Count - 1
                Dim loType As Type
                loType = loRow.FindControl(ltDT_Syn.Columns(i).ColumnName).GetType
                Select Case loType.FullName
                    Case "System.Web.UI.WebControls.TextBox"
                        Dim field_value As String = DirectCast(loRow.FindControl(ltDT_Syn.Columns(i).ColumnName), TextBox).Text
                        laParameters.Add(field_value)
                    Case "System.Web.UI.WebControls.CheckBox"
                        Dim field_value As Boolean = DirectCast(loRow.FindControl(ltDT_Syn.Columns(i).ColumnName), CheckBox).Checked
                        laParameters.Add(field_value)
                    Case "System.Web.UI.WebControls.DropDownList"
                        Dim field_value As String = DirectCast(loRow.FindControl(ltDT_Syn.Columns(i).ColumnName), DropDownList).SelectedValue
                        laParameters.Add(field_value)
                End Select
            Next
            Try

                loListItemSyn = Arco.Doma.Library.Lists.ListItemSyn.GetListItemSyn(laParameters(0))
                If loListItemSyn.SYN_ID = laParameters(0) Then
                    loListItemSyn.SYN_ID = laParameters(0)
                    loListItemSyn.SYN_DESC = laParameters(1)
                    loListItemSyn.LANG_CODE = laParameters(2)
                    loListItemSyn.ITEM_ID = CurrentItem.ID
                    loListItemSyn.Save()
                    lbOK = True
                End If               
            Catch ex As Exception
                ShowError(ex.ToString)          
            End Try
            synGridView.EditIndex = -1
            synCreateTemplatedGridView()
            synGridView.FooterRow.Visible = True
            Session("InsertFlag") = If(CInt(Session("InsertFlag")) = 1, 0, 1)
        Catch ex As Exception
            ShowError(ex.ToString)
        End Try

        If lbOK = True Then RefreshLeftPane()

    End Sub

    Private Sub FetchSynonyms()

        Dim loListItemSyns As Arco.Doma.Library.Lists.ListItemSynList
        Dim loListItemSyn As Arco.Doma.Library.Lists.ListItemSynList.ListItemSynInfo
        Dim loCrit As Arco.Doma.Library.Lists.ListItemSynList.Criteria = New Arco.Doma.Library.Lists.ListItemSynList.Criteria()
        Dim loCol As DataColumn

        synGridView.Columns.Clear()

        ltDT_Syn = New DataTable()
        loCol = ltDT_Syn.Columns.Add("ID")
        loCol = ltDT_Syn.Columns.Add("NAME")
        'loCol.Caption = GetDecodedLabel("Synonym")
        loCol.MaxLength = 200
        loCol = ltDT_Syn.Columns.Add("LANGCODE")
        loCol.Caption = GetDecodedLabel("language")

        loCrit.ITEM_ID = CurrentItem.ID

        loListItemSyns = Arco.Doma.Library.Lists.ListItemSynList.GetListItemSynList(loCrit)

        For Each loListItemSyn In loListItemSyns
            Dim laRowItems(2) As String
            laRowItems(0) = loListItemSyn.SYN_ID
            laRowItems(1) = Server.HtmlEncode(loListItemSyn.SYN_DESC)
            laRowItems(2) = loListItemSyn.LANG_CODE
            ltDT_Syn.Rows.Add(laRowItems)
        Next
        ShowDelete = True
      
        If loListItemSyns.Count = 0 Then
            ' Show dummy
            ShowDelete = False
            Dim laRowItems1(2) As String
            laRowItems1(0) = 0
            laRowItems1(1) = GetDecodedLabel("cm_please_enter_synonym")

            ltDT_Syn.Rows.Add(laRowItems1)
        End If

    End Sub

#End Region

#Region "Synonyms TableGridView "

    Protected Sub synGridView_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs) Handles synGridView.RowDeleting
        DeleteRow(e.RowIndex)
    End Sub

    Protected Sub synGridView_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles synGridView.PageIndexChanging
        synCreateTemplatedGridView()
        synGridView.PageIndex = e.NewPageIndex
        synGridView.DataBind()
        synGridView.FooterRow.Visible = True
    End Sub

    Protected Sub synGridView_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles synGridView.RowCommand
        Select Case e.CommandName
            Case "Insert"
                InsertRow()
        End Select
    End Sub

    Protected Sub synGridView_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles synGridView.RowCreated
        If e.Row.RowType = DataControlRowType.DataRow Or _
           e.Row.RowType = DataControlRowType.Header Or _
           e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(1).Style.Add("display", "none")
            e.Row.Cells(3).Style.Add("display", "none")
        End If
    End Sub

    Private molangs As Arco.Doma.Library.Globalisation.LanguageList

    Private Function GetLanguageDesc(ByVal vsCode As String) As String
        If molangs Is Nothing Then
            molangs = Arco.Doma.Library.Globalisation.LanguageList.GetInterfaceLanguageList()
        End If
        Dim lsDesc As String = vsCode
        For Each lolang As Arco.Doma.Library.Globalisation.LanguageList.LanguageInfo In molangs
            If lolang.InterfaceLanguageCode = vsCode Then

                lsDesc = lolang.Description
                Exit For
            End If
        Next
        Return lsDesc
    End Function

    Private Sub synCreateTemplatedGridView()

        Dim field_dropdown As New DropDownList
        Dim field_dropdownFooter As New DropDownList
        Dim loListLanguages As Arco.Doma.Library.Lists.ListLanguageList
        Dim loListLanguage As Arco.Doma.Library.Lists.ListLanguageList.ListLanguageInfo

        ' Fill the table which is to bound to the GridView.
        FetchSynonyms()

        ' language = mandatory
        'field_dropdown.Items.Add(New ListItem("", ""))
        'field_dropdownFooter.Items.Add(New ListItem("", ""))

        loListLanguages = Arco.Doma.Library.Lists.ListLanguageList.GetListLanguageList(CurrentList.ID)
        For Each loListLanguage In loListLanguages
            field_dropdown.Items.Add(New ListItem(GetLanguageDesc(loListLanguage.LANG_CODE), loListLanguage.LANG_CODE))
            field_dropdownFooter.Items.Add(New ListItem(GetLanguageDesc(loListLanguage.LANG_CODE), loListLanguage.LANG_CODE))
        Next
        loListLanguages = Nothing

        ' Add templated fields to the GridView.
        Dim BtnTmpField As New TemplateField()

        BtnTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, "", "") ' Nothing
        BtnTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, "", "") ' Nothing
        BtnTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, "", "") ' Arrow
        synGridView.Columns.Add(BtnTmpField)

        For i As Integer = 0 To ltDT_Syn.Columns.Count - 1
            Dim ItemTmpField As New TemplateField()
            ItemTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, "", "")
            ItemTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, ltDT_Syn.Columns(i).ColumnName, ltDT_Syn.Columns(i).DataType.Name)
            ItemTmpField.ItemStyle.Wrap = True
            Select Case i
                Case 2 ' DropDown
                    ItemTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, ltDT_Syn.Columns(i).ColumnName, "Dropdownhead", field_dropdownFooter)
                Case Else
                    ItemTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, ltDT_Syn.Columns(i).ColumnName, "STRINGHEAD", ltDT_Syn.Columns(i).MaxLength)
            End Select
            ItemTmpField.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
            synGridView.Columns.Add(ItemTmpField)
        Next

        ' Add templated fields to the GridView.

        BtnTmpField = New TemplateField()

        If Not ShowDelete Then
            BtnTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, "", "") ' Edit/Delete
        Else
            BtnTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, "", "deleteonly") ' Edit/Delete
        End If

        BtnTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, "", "AddLinkHead") ' Nothing
        BtnTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, "", "") ' Save
        BtnTmpField.ItemStyle.Width = Unit.Pixel(30)
        BtnTmpField.ItemStyle.HorizontalAlign = HorizontalAlign.Right
        BtnTmpField.HeaderStyle.HorizontalAlign = HorizontalAlign.Right


        synGridView.Columns.Add(BtnTmpField)

        ' Bind and display the data.

        synGridView.DataSource = ltDT_Syn
        synGridView.DataBind()
        'synGridView.Columns(3).Visible = False
        synGridView.Columns(0).Visible = False
        synGridView.HeaderStyle.CssClass = "ListContent"
        synGridView.FooterRow.Visible = True
        synGridView.FooterRow.Cells(2).Focus()
        synGridView.EmptyDataText = GetDecodedLabel("noresultsfound")
    End Sub

#End Region

#End Region

#Region "Category Dependant"

#Region " Insert/Delete/Fetch "

    Private Sub FetchRows(ByVal vlPageIndex As Integer)
        Dim loCol As DataColumn
        Dim llMax As Integer = 1

        catGridView.Columns.Clear()

        ltDT_Cat = New DataTable()

        loCol = ltDT_Cat.Columns.Add("CATNAME")
        loCol.Caption = "Category"
        loCol.MaxLength = 200
        loCol = ltDT_Cat.Columns.Add("CATID")

        Dim laRowItems(llMax) As String
        Dim loItemCategories As Arco.Doma.Library.Lists.ListItemCatList = Arco.Doma.Library.Lists.ListItemCatList.GetCategoriesForListItem(CurrentItem.ID)
        If loItemCategories.Any Then
            Dim loCategories As Arco.Doma.Library.baseObjects.OBJECT_CATEGORYList = Arco.Doma.Library.baseObjects.OBJECT_CATEGORYList.GetOBJECT_CATEGORYList(False, Me.EnableIISCaching)
            For Each loItemCat As Arco.Doma.Library.Lists.ListItemCatList.ListItemCatInfo In loItemCategories
                For Each loCategory As Arco.Doma.Library.baseObjects.OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In loCategories
                    If loItemCat.CatID = loCategory.ID Then
                        laRowItems(DTCols.CATID) = loCategory.ID
                        laRowItems(DTCols.CATNAME) = loCategory.Name
                        ltDT_Cat.Rows.Add(laRowItems)
                        Exit For
                    End If
                Next
            Next
        End If

    End Sub

#End Region

#Region "Category TableGridView "

    Protected Sub AddCategory(ByVal sender As Object, ByVal e As System.EventArgs)
        If cmbCat.SelectedIndex > 0 Then
            Arco.Doma.Library.Lists.ListItemCat.AddListItemCat(CurrentItem.ID, Convert.ToInt32(cmbCat.SelectedItem.Value))
            catCreateTemplatedGridView(0)
            AddCategoryItems()
            MsgPanel.Visible = False
        Else
            ShowError(GetDecodedLabel("selcat"))
        End If
    End Sub
    Protected Sub catGridView_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs) Handles catGridView.RowDeleting

        Dim selectedRow As GridViewRow = catGridView.Rows(e.RowIndex)
        Dim catid As String = DirectCast(selectedRow.Cells(1).Controls(0), Label).Text
        'mesg.InnerText = catid
        Arco.Doma.Library.Lists.ListItemCat.DeleteListItemCat(CurrentItem.ID, Convert.ToInt32(catid))
        catCreateTemplatedGridView(0)
        AddCategoryItems()
    End Sub
    Protected Sub catGridView_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs) Handles catGridView.PageIndexChanging
        catCreateTemplatedGridView(e.NewPageIndex)
        catGridView.PageIndex = e.NewPageIndex
        catGridView.DataBind()
        catGridView.FooterRow.Visible = True
    End Sub
    Private Sub catCreateTemplatedGridView(ByVal vlNewPageIndex As Integer)
        FetchRows(vlNewPageIndex)

        'Dim field_dropdownFooter As New DropDownList
        'field_dropdownFooter.Items.Add(New ListItem("", ""))
        'Dim loCategories As baseObjects.OBJECT_CATEGORYList = baseObjects.OBJECT_CATEGORYList.GetOBJECT_CATEGORYList(False, Me.EnableIISCaching)

        'For Each loCategory As baseObjects.OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In loCategories
        '    field_dropdownFooter.Items.Add(New ListItem(loCategory.Name, loCategory.ID))
        'Next

        For i As Integer = 0 To ltDT_Cat.Columns.Count - 1
            Dim ItemTmpField As New TemplateField()
            Select Case i
                Case 1
                    ItemTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, "", ltDT_Cat.Columns(i).DataType.Name)
                    ItemTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, ltDT_Cat.Columns(i).ColumnName, ltDT_Cat.Columns(i).DataType.Name)
                    ItemTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, ltDT_Cat.Columns(i).ColumnName, "", "")
                Case 0
                    ItemTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, "", ltDT_Cat.Columns(i).DataType.Name)
                    ItemTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, ltDT_Cat.Columns(i).ColumnName, ltDT_Cat.Columns(i).DataType.Name)
                    ItemTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, ltDT_Cat.Columns(i).ColumnName, "", "")
            End Select
            catGridView.Columns.Add(ItemTmpField)
        Next

        Dim BtnTmpField As New TemplateField()
        BtnTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, "", "DeleteOnly") ' Delete Only
        BtnTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, "", "") ' Nothing
        BtnTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, "", "") ' Nothing
        BtnTmpField.ItemStyle.Width = Unit.Pixel(22)
        BtnTmpField.ItemStyle.HorizontalAlign = HorizontalAlign.Right


        catGridView.Columns.Add(BtnTmpField)

        ' Bind and display the data.
        If ltDT_Cat.Rows.Count() >= 0 Then
            catGridView.DataSource = ltDT_Cat
            catGridView.Columns(1).Visible = False
            'catGridView.FooterRow.Visible = True
            'footer.Visible = True
            catGridView.HeaderStyle.CssClass = "ListContent"
            If ltDT_Cat.Rows.Count() = 0 Then
                catGridView.EmptyDataText = GetDecodedLabel("noresultsfound")
            End If
        End If
        catGridView.DataBind()

    End Sub

#End Region

#End Region

#Region "Relationships"
#Region "Insert/Update/Delete/Fetch Relationships"


    Private Sub DeleteRow_Rel(ByVal vlRowIndex As Integer)

        Dim loRow As GridViewRow = TableGridViewRels.Rows(vlRowIndex)
        Dim llID1 As Integer = DirectCast(loRow.FindControl(ltDT_Rels.Columns(DTCols_Rel.ITEM1_ID).ColumnName), Label).Text
        Dim llID2 As Integer = DirectCast(loRow.FindControl(ltDT_Rels.Columns(DTCols_Rel.ITEM2_ID).ColumnName), Label).Text

        If llID1 > 0 And llID2 > 0 Then
            Arco.Doma.Library.Lists.ListItemRel.DeleteListItemRel(llID1, llID2)
            CreateTemplatedGridView_Rels()
            TableGridViewRels.FooterRow.Visible = False ' True 
            RefreshLeftPane()
        End If

    End Sub

    Private Sub FetchRelationships()

        Dim loCol As DataColumn

        TableGridViewRels.Columns.Clear()

        ltDT_Rels = New DataTable()
        ltDT_Rels.Columns.Add("ID1")
        loCol = ltDT_Rels.Columns.Add("DESCRIPTION1")
        loCol.Caption = GetDecodedLabel("description") & " 1"

        loCol = ltDT_Rels.Columns.Add("LINK_TYPE")
        loCol.Caption = GetDecodedLabel("link") & " " & GetDecodedLabel("cm_type")

        loCol = ltDT_Rels.Columns.Add("ID2")
        loCol = ltDT_Rels.Columns.Add("DESCRIPTION2")
        loCol.Caption = GetDecodedLabel("description") & " 2"

        FetchRelationships(1)
        FetchRelationships(2)

        If ltDT_Rels.Rows.Count = 0 OrElse CurrentItem.ID = 0 Then
            ' Show dummy
            Dim laRowItems(4) As String
            laRowItems(DTCols_Rel.ITEM1_ID) = "0"
            laRowItems(DTCols_Rel.DESC1) = GetDecodedLabel("cm_please_enter_relationship")
            laRowItems(DTCols_Rel.LINK_TYPE) = ""
            laRowItems(DTCols_Rel.ITEM2_ID) = "0"
            laRowItems(DTCols_Rel.DESC2) = ""

            ltDT_Rels.Rows.Add(laRowItems)
            ShowDelete_Rels = False
        End If

    End Sub

    Private Sub FetchRelationShips(ByVal Index As Integer)

        Dim loRels As Arco.Doma.Library.Lists.ListItemRelList = Nothing
        Dim loRel As Arco.Doma.Library.Lists.ListItemRelList.ListItemRelInfo
        Dim loCrit As Arco.Doma.Library.Lists.ListItemRelList.Criteria
        Dim loListRelTypes As Arco.Doma.Library.Lists.ListRelTypeList = Nothing
        Dim loListRelType As Arco.Doma.Library.Lists.ListRelTypeList.ListRelTypeInfo
        Dim loCritRelType As Arco.Doma.Library.Lists.ListRelTypeList.Criteria = New Arco.Doma.Library.Lists.ListRelTypeList.Criteria()
        Dim loListItem As Arco.Doma.Library.Lists.ListItem
        Dim lsDesc As String = ""

        If CurrentList.LIST_TYPE = "CONCEPTTREE" Or CurrentList.LIST_TYPE = "THESAURUS" Then
            loCritRelType.LIST_ID = CurrentList.ID
            If loCritRelType.LIST_ID > 0 Then
                loListRelTypes = Arco.Doma.Library.Lists.ListRelTypeList.GetListRelTypeList(loCritRelType)
            End If
        End If

        ' Get direct links.
        loCrit = New Arco.Doma.Library.Lists.ListItemRelList.Criteria
        If Index = 1 Then
            loCrit.ITEM1_ID = CurrentItem.ID
        Else
            loCrit.ITEM2_ID = CurrentItem.ID
        End If
        loCrit.LINK_DIRECT = 1
        loRels = Arco.Doma.Library.Lists.ListItemRelList.GetListItemRelList(loCrit)
        For Each loRel In loRels
            Dim laRowItems(4) As String
            laRowItems(DTCols_Rel.ITEM1_ID) = loRel.ITEM1_ID
            laRowItems(DTCols_Rel.ITEM2_ID) = loRel.ITEM2_ID
            If loRel.LINK_TYPE = 1 Then
                laRowItems(DTCols_Rel.LINK_TYPE) = "BT"
            ElseIf loRel.LINK_TYPE = 2 Then
                laRowItems(DTCols_Rel.LINK_TYPE) = "RT"
            ElseIf CurrentList.LIST_TYPE = "CONCEPTTREE" Or CurrentList.LIST_TYPE = "THESAURUS" Then
                For Each loListRelType In loListRelTypes
                    If loListRelType.REL_ID = loRel.LINK_TYPE Then
                        laRowItems(DTCols_Rel.LINK_TYPE) = loListRelType.REL_DESC
                        Exit For
                    End If
                Next
            End If
            If Index = 1 Then
                lsDesc = CurrentItem.Description
                If CurrentItem.ITEM_CODE.Length > 0 Then lsDesc = CurrentItem.ITEM_CODE & " " & lsDesc
                If CurrentItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & CurrentItem.ITEM_DESC_HOMO & "]"
                laRowItems(DTCols_Rel.DESC1) = Server.HtmlEncode(lsDesc)
                loListItem = Arco.Doma.Library.Lists.ListItem.GetListItem(loRel.ITEM2_ID)
                If loListItem.ID = loRel.ITEM2_ID Then
                    lsDesc = loListItem.Description
                    If loListItem.ITEM_CODE.Length > 0 Then lsDesc = loListItem.ITEM_CODE & " " & lsDesc
                    If loListItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & loListItem.ITEM_DESC_HOMO & "]"
                    laRowItems(DTCols_Rel.DESC2) = Server.HtmlEncode(lsDesc)
                End If
            Else
                lsDesc = CurrentItem.Description
                If CurrentItem.ITEM_CODE.Length > 0 Then lsDesc = CurrentItem.ITEM_CODE & " " & lsDesc
                If CurrentItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & CurrentItem.ITEM_DESC_HOMO & "]"
                laRowItems(DTCols_Rel.DESC2) = Server.HtmlEncode(lsDesc)
                loListItem = Arco.Doma.Library.Lists.ListItem.GetListItem(loRel.ITEM1_ID)
                If loListItem.ID = loRel.ITEM1_ID Then
                    lsDesc = loListItem.Description
                    If loListItem.ITEM_CODE.Length > 0 Then lsDesc = loListItem.ITEM_CODE & " " & lsDesc
                    If loListItem.ITEM_DESC_HOMO.Length > 0 Then lsDesc &= " [" & loListItem.ITEM_DESC_HOMO & "]"
                    laRowItems(DTCols_Rel.DESC1) = Server.HtmlEncode(lsDesc)
                End If
            End If
            ltDT_Rels.Rows.Add(laRowItems)
            ShowDelete_Rels = True
        Next
        loRels = Nothing

    End Sub

#End Region

#Region "TableGridView Relationships"

    
    Protected Sub TableGridViewRels_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)
        DeleteRow_Rel(e.RowIndex)
    End Sub

    Protected Sub TableGridViewRels_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles TableGridViewRels.RowCreated

        If e.Row.RowType = DataControlRowType.DataRow Or _
           e.Row.RowType = DataControlRowType.Header Or _
           e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(DTCols_Rel.ITEM1_ID + 1).Style.Add("display", "none") ' ITEM1_ID invisible.
            e.Row.Cells(DTCols_Rel.ITEM2_ID + 1).Style.Add("display", "none") ' ITEM2_ID invisible.
        End If
    End Sub

    Private Sub CreateTemplatedGridView_Rels()

        Dim loDDL As New DropDownList
        Dim loDDLFooter As New DropDownList
        Dim loListRelTypes As Arco.Doma.Library.Lists.ListRelTypeList
        Dim loListRelType As Arco.Doma.Library.Lists.ListRelTypeList.ListRelTypeInfo
        Dim loCrit As Arco.Doma.Library.Lists.ListRelTypeList.Criteria = New Arco.Doma.Library.Lists.ListRelTypeList.Criteria()
        Dim loDDLItems1 As New DropDownList
        Dim loDDLItems2 As New DropDownList
        'Dim loItems As Lists.ListItemList
        'Dim loItem As Lists.ListItemList.ListItemInfo
        'Dim loCritItem As Lists.ListItemList.Criteria
        Dim lsDesc As String = ""

        If Not Page.IsPostBack Then
            cmbRel1.Items.Clear()
            cmbRel2.Items.Clear()


            ' Broader Term
            'loDDL.Items.Add(New ListItem("BT", "1"))
            'loDDLFooter.Items.Add(New ListItem("BT", "1"))
            cmbRel1.Items.Add(New ListItem("BT", "1"))
            cmbRel2.Items.Add(New ListItem("BT", "1"))

            ' Related Term
            'loDDL.Items.Add(New ListItem("RT", "2"))
            'loDDLFooter.Items.Add(New ListItem("RT", "2"))
            cmbRel1.Items.Add(New ListItem("RT", "2"))
            cmbRel2.Items.Add(New ListItem("RT", "2"))

            If CurrentList.LIST_TYPE = "CONCEPTTREE" Or CurrentList.LIST_TYPE = "THESARAUS" Then
                loCrit.LIST_ID = CurrentList.ID
                If loCrit.LIST_ID > 0 Then
                    loListRelTypes = Arco.Doma.Library.Lists.ListRelTypeList.GetListRelTypeList(loCrit)
                    For Each loListRelType In loListRelTypes
                        'loDDL.Items.Add(New ListItem(loListRelType.REL_DESC, loListRelType.REL_ID))
                        'loDDLFooter.Items.Add(New ListItem(loListRelType.REL_DESC, loListRelType.REL_ID))
                        cmbRel1.Items.Add(New ListItem(loListRelType.REL_DESC, loListRelType.REL_ID))
                        cmbRel2.Items.Add(New ListItem(loListRelType.REL_DESC, loListRelType.REL_ID))
                    Next
                End If
            End If
        End If


        ' Fill the table which is to bound to the GridView.
        FetchRelationships()

        Dim BtnTmpField As New TemplateField()
        BtnTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, "", "") ' Delete
        BtnTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, "", "") ' Nothing
        BtnTmpField.EditItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.EditItem, "", "") ' Nothing
        BtnTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, "", "") ' Arrow
        TableGridViewRels.Columns.Add(BtnTmpField)
        ' Add templated fields to the GridView.


        For i As Integer = 0 To ltDT_Rels.Columns.Count - 1
            Dim ItemTmpField As New TemplateField()
            ItemTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, ltDT_Rels.Columns(i).Caption, ltDT_Rels.Columns(i).DataType.Name)
            ItemTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, ltDT_Rels.Columns(i).ColumnName, ltDT_Rels.Columns(i).DataType.Name)
            Select Case i
                Case DTCols_Rel.DESC1
                    'ItemTmpField.EditItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, ltDT_Rels.Columns(i).ColumnName, ltDT_Rels.Columns(i).DataType.Name)
                    ItemTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, ltDT_Rels.Columns(i).ColumnName, "Dropdown", loDDLItems1)
                    ItemTmpField.HeaderStyle.Width = Unit.Pixel(205)
                Case DTCols_Rel.DESC2
                    'ItemTmpField.EditItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, ltDT_Rels.Columns(i).ColumnName, ltDT_Rels.Columns(i).DataType.Name)
                    ItemTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, ltDT_Rels.Columns(i).ColumnName, "Dropdown", loDDLItems2)
                    ItemTmpField.HeaderStyle.Width = Unit.Pixel(205)
                Case DTCols_Rel.LINK_TYPE  'Dropdown.
                    'ItemTmpField.EditItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, ltDT_Rels.Columns(i).ColumnName, ltDT_Rels.Columns(i).DataType.Name)
                    ItemTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, ltDT_Rels.Columns(i).ColumnName, "Dropdown", loDDLFooter)
                    ItemTmpField.ItemStyle.HorizontalAlign = HorizontalAlign.Center
                    ItemTmpField.HeaderStyle.Width = Unit.Pixel(100)
                Case Else
                    'ItemTmpField.EditItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, ltDT_Rels.Columns(i).ColumnName, ltDT_Rels.Columns(i).DataType.Name)
                    ItemTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, ltDT_Rels.Columns(i).ColumnName, ltDT_Rels.Columns(i).DataType.Name)
            End Select
            TableGridViewRels.Columns.Add(ItemTmpField)
        Next

        BtnTmpField = New TemplateField()
        If Not ShowDelete_Rels Then
            BtnTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, "", "") ' Nothing
        Else
            BtnTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, "", "DeleteOnly") ' Delete
        End If


        BtnTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, "", "") ' Nothing
        BtnTmpField.EditItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.EditItem, "", "") ' Nothing
        BtnTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, "", "") ' Arrow
        BtnTmpField.ItemStyle.Width = Unit.Pixel(20)
        BtnTmpField.ItemStyle.HorizontalAlign = HorizontalAlign.Center
        TableGridViewRels.Columns.Add(BtnTmpField)


        ' Bind and display the data.

        TableGridViewRels.DataSource = ltDT_Rels
        TableGridViewRels.DataBind()
        TableGridViewRels.Columns(0).Visible = False
        TableGridViewRels.ShowFooter = False

    End Sub

#End Region

    Protected Sub cmdRel1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRel1.Click
        AddRelClicked(txtRel2, cmbRel1, False)
      
    End Sub

    Protected Sub cmdRel2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdRel2.Click

        AddRelClicked(txtRel1, cmbRel2, True)
    End Sub

    Private Sub AddRelClicked(ByVal txtItem As TextBox, ByVal cmbType As DropDownList, ByVal vbReverse As Boolean)
        Dim llLinkType As Integer = 0
        Dim llItem As Integer = 0
        Dim lsDesc As String = ""
        If txtItem.Text.Trim().Length > 0 Then
            lsDesc = txtItem.Text
            Dim loItem As Arco.Doma.Library.Lists.ListItem = Arco.Doma.Library.Lists.ListItem.GetListItemByDesc(CurrentList.ID, lsDesc, "")
            llItem = loItem.ID
            If CurrentItem.ID <> llItem Then
                If llItem > 0 Then
                    llLinkType = cmbType.SelectedValue
                    Dim llItem1 As Integer
                    Dim llItem2 As Integer

                    If vbReverse Then
                        llItem1 = llItem
                        llItem2 = CurrentItem.ID
                    Else
                        llItem1 = CurrentItem.ID
                        llItem2 = llItem
                    End If

                    If AddRel(llItem1, llItem2, llLinkType) = True Then
                        txtItem.Text = ""
                        cmbType.SelectedValue = "1"
                        CreateTemplatedGridView_Rels()
                        RefreshLeftPane()
                        MsgPanel.Visible = False
                    End If
                Else
                    ShowError(GetDecodedLabel("cm_wrong_id"))
                End If
            Else
                ShowError(GetDecodedLabel("cm_wrong_id"))
            End If
        Else
            ShowError(GetDecodedLabel("cm_enter_description"))
        End If
    End Sub

    Private Function AddRel(ByVal vlItem1 As Integer, _
                            ByVal vlItem2 As Integer, _
                            ByVal vlLinkType As Integer) As Boolean

        Dim lbAlreadyExists As Boolean = False
        Dim lbHasIndirectLink As Boolean = False
        Dim loListItemRel As Arco.Doma.Library.Lists.ListItemRel
        Dim lbOK As Boolean = False

        If Arco.Doma.Library.Lists.ListItemRel.RelIsValid(vlItem1, vlItem2, lbAlreadyExists, lbHasIndirectLink) Then
            loListItemRel = Arco.Doma.Library.Lists.ListItemRel.NewListItemRel(vlItem1, vlItem2)
            loListItemRel.LINK_TYPE = vlLinkType
            loListItemRel.LINK_DIRECT = True           
            loListItemRel.Save() ' Triggers AfterSaveHandling in ListItemRel.          
            lbOK = True
        Else
            If lbAlreadyExists = True Then
                ShowError(GetDecodedLabel("cm_relationship_already_exists"))
            ElseIf lbHasIndirectLink = True Then
                ShowError(GetDecodedLabel("cm_found_indirect_link"))
            End If
        End If

        Return lbOK

    End Function

#End Region


    Private Function findparent(ByVal voItem As Arco.Doma.Library.Lists.IListItem) As String
        Dim outParent As Integer = 0
        Dim loCrit As Arco.Doma.Library.Lists.ListItemRelList.Criteria = New Arco.Doma.Library.Lists.ListItemRelList.Criteria
        loCrit.ITEM1_ID = 0
        loCrit.ITEM2_ID = voItem.ID
        loCrit.LINK_TYPE = 1
        loCrit.LINK_DIRECT = 1
        Dim lcolParents As Arco.Doma.Library.Lists.ListItemRelList = Arco.Doma.Library.Lists.ListItemRelList.GetListItemRelList(loCrit)        
        Dim llparent As Int32 = 0
        While Not lcolParents Is Nothing AndAlso lcolParents.Any
            outParent = lcolParents.Item(0).ITEM1_ID
            Exit While
        End While

        Return outParent
    End Function

    Public Function ReplaceFirst(ByVal text As String, ByVal search As String, ByVal replace As String) As String
        Dim pos As Integer = text.IndexOf(search)
        If pos < 0 Then
            Return text
        End If
        Return text.Substring(0, pos) + replace + text.Substring(pos + search.Length)
    End Function

#Region " Error handling "
    Private Sub ClearError()
        msg_lbl.Text = ""
        MsgPanel.Visible = False
    End Sub
    Private Sub ShowError(ByVal vsText As String)
        msg_lbl.Text = vsText
        MsgPanel.Visible = True
    End Sub

    Protected Sub Msg_button_Click(ByVal sender As Object, ByVal e As EventArgs)
        MsgPanel.Visible = False
    End Sub

#End Region

End Class
