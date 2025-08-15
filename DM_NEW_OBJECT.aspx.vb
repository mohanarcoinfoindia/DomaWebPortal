Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Website
Imports System.IO
Imports Arco.Doma.Library.Globalisation
Imports Arco.Doma.Library.Routing
Imports DomaGenTemplateEngine
Imports DomaGenTemplateEngine.baseObjects
Imports Telerik.Web.UI

Namespace Doma

    Public Class DM_NEW_OBJECT
        Inherits BasePage

        Private Enum eMode
            SingleDoc = 0
            MultiDoc = 1
        End Enum
        Private meMode As eMode = eMode.SingleDoc

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

        End Sub
        Private Sub Page_Init(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region


        Private _category As OBJECT_CATEGORY = Nothing

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load

            Page.Form.DefaultButton = lnkSave.UniqueID

            'Put user code to initialize the page here
            Dim creationInfo As ObjectCreationInfo = Nothing

            meMode = CType(QueryStringParser.GetInt("mode"), eMode)

            If Not Page.IsPostBack Then
                Dim creationInfoQueryString As String = QueryStringParser.GetString("creationinfo")
                If String.IsNullOrEmpty(creationInfoQueryString) Then
                    txtCatType.Text = QueryStringParser.GetString("CAT_TYPE")
                    txtObjectID.Text = QueryStringParser.GetString("DM_OBJECT_ID")
                    txtParentID.Text = QueryStringParser.GetInt("DM_PARENT_ID").ToString
                    txtCatID.Text = QueryStringParser.GetString("DM_CAT_ID")
                    txtCaseID.Text = QueryStringParser.GetString("CASE_ID")
                    txtPackID.Text = QueryStringParser.GetString("PACK_ID")
                    txtLinkToDIN.Text = QueryStringParser.GetString("LINK_TO_DIN")
                    txtLinkAs.Text = QueryStringParser.GetString("LINK_AS")

                    If Not String.IsNullOrEmpty(txtCatID.Text) Then
                        Dim liCatID As Int32
                        If Arco.Utils.GUID.IsGUID(txtCatID.Text) Then
                            _category = OBJECT_CATEGORY.GetCategoryByGUID(txtCatID.Text)
                            txtCatID.Text = _category.ID.ToString
                        ElseIf Not Int32.TryParse(txtCatID.Text, liCatID) Then
                            _category = OBJECT_CATEGORY.GetOBJECT_CATEGORY(txtCatID.Text, txtCatType.Text)
                            txtCatID.Text = _category.ID.ToString
                        End If
                    End If
                Else
                    creationInfo = GetCreationInfoFromJson(creationInfoQueryString)
                End If


                If meMode = eMode.SingleDoc Then
                    txtObjectName.MaxLength = Settings.GetValue("Storage", "MaxNamesLength", 500)
                    txtObjectName.Focus()
                    rowName.Visible = True
                Else
                    rowName.Visible = False
                End If
            Else
                If Request.ContentType = "application/json" Then
                    creationInfo = GetCreationInfoFromJson(GetRequestBody())
                End If
            End If

            If creationInfo IsNot Nothing Then
                _category = creationInfo.GetCategory()
                If _category Is Nothing Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INVALIDCATEGORY)
                ElseIf _category.DisableManualCreation Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOPERATIONONOBJECTTYPE)
                Else
                    'autosave from json
                    Save(creationInfo)
                End If
                Return
            End If


            If String.IsNullOrEmpty(txtCatID.Text) OrElse txtCatID.Text = "0" Then
                Server.Transfer(GetRedirectUrl("DM_SELECT_OBJ_CAT.aspx?DM_PARENT_ID=" & txtParentID.Text & "&CAT_TYPE=" & txtCatType.Text & "&INPUTSELECTION=" & QueryStringParser.GetString("INPUTSELECTION") & "&PACK_ID=" & txtPackID.Text & "&mode=" & Convert.ToInt32(meMode)))
                Return
            End If

            _category = OBJECT_CATEGORY.GetOBJECT_CATEGORY(Convert.ToInt32(txtCatID.Text))
            If _category Is Nothing Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDCATEGORY)
                Return
            ElseIf _category.DisableManualCreation Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOPERATIONONOBJECTTYPE)
                Return
            End If


            SetLabels()
            Dim loFolder As DM_OBJECT

            If _category.CAT_TYPE <> "ListItem" Then
                If Not String.IsNullOrEmpty(_category.Default_Folder) Then
                    Dim loDefFolderID As Int32 = Folder.CreateFolderStructure(TagReplacer.ReplaceTags(_category.Default_Folder), 0)
                    If loDefFolderID > 0 Then
                        txtParentID.Text = loDefFolderID.ToString 'set the variable direct, it doesn't get set thru the
                    Else
                        If Not String.IsNullOrEmpty(_category.Default_Folder) Then
                            Throw New ApplicationException("Default Folder " & _category.Default_Folder & " not found")
                        End If
                    End If
                End If

                'check if the cat_id is allowed here            
                Dim llParentID As Int32 = Convert.ToInt32(txtParentID.Text)

                If llParentID > 0 Then
                    loFolder = ObjectRepository.GetObject(llParentID)

                    While loFolder.Object_Type = "Shortcut" 'load the object the shortcut points too
                        If loFolder.Object_Reference > 0 Then
                            loFolder = CType(loFolder, Shortcut).GetReferencedObject
                            txtParentID.Text = loFolder.ID.ToString
                        Else
                            'shortcut not pointing anywhere
                            Response.Write("Illegal shortcut")
                            Response.End()
                        End If
                    End While

                Else
                    loFolder = Folder.GetRoot
                End If

                fldField.ShowImages = UserProfile.ShowFolderLinkIcons
                fldField.FolderID = loFolder.ID

                rowFolder.Visible = True
            Else
                loFolder = Folder.GetRoot
                rowFolder.Visible = False
                txtParentID.Text = "0"
            End If


            If Not _category.CanCreateObject(loFolder) Then
                Server.Transfer(GetRedirectUrl("DM_ACL_DENIED.aspx?code=" & _category.GetLastError.Code))
            End If

            If meMode = eMode.MultiDoc Then
                txtTitle.Text = GetLabel("multidocupload") & " : " & Server.HtmlEncode(_category.TranslatedName)
            Else
                txtTitle.Text = GetLabel("insert") & " : " & Server.HtmlEncode(_category.TranslatedName)
            End If

            If _category.Type.CanHaveFiles AndAlso _category.FileCardinalityConstraint <> ConstraintEnum.NotAllowed Then

                txtFileUpload.Localization.Select = GetDecodedLabel("dropfilesorclick")
                txtFileUpload.Localization.Remove = ""

                txtFileUpload.TemporaryFolder = Settings.GetUploadPath()
                txtFileUpload.TargetFolder = ""
                txtFileUpload.UploadedFilesRendering = AsyncUpload.UploadedFilesRendering.BelowFileInput
                txtFileUpload.TemporaryFileExpiration = TimeSpan.FromMinutes(60)


                If Not Page.IsPostBack Then

                    txtFileUpload.MultipleFileSelection = AsyncUpload.MultipleFileSelection.Disabled
                    txtFileUpload.MaxFileInputsCount = 1
                    If meMode = eMode.MultiDoc OrElse _category.FileCardinalityConstraint = ConstraintEnum.AtLeastOne OrElse _category.FileCardinalityConstraint = ConstraintEnum.NoConstraint Then
                        txtFileUpload.MultipleFileSelection = AsyncUpload.MultipleFileSelection.Automatic
                        txtFileUpload.MaxFileInputsCount = 0
                    End If


                    Dim lsAllowedExtensions As List(Of String) = _category.GetAllowedFileTypes(True)


                    Dim lbCanAddFile As Boolean = lsAllowedExtensions.Any
                    If lbCanAddFile Then

                        txtFileUpload.AllowedFileExtensions = lsAllowedExtensions.ToArray
                        lsAllowedExtensions.Sort()

                        lblFileTypes.Text = ArcoFormatting.AddTooltip("&nbsp;<img src='./Images/Help.png'/>", String.Join(", ", lsAllowedExtensions.ToArray))
                    End If

                    Dim loCrit As New TG_TEMPLATES_LIST.Criteria With {
                        .AllowedAt = TG_TEMPLATES.eAllowedAt.NewDocument,
                        .DocTypeID = _category.ID
                    }
                    Dim lcolTemplates As TG_TEMPLATES_LIST = TG_TEMPLATES_LIST.GetTemplates(loCrit)
                    If lcolTemplates.Any Then
                        cmbFileType.Visible = True
                        cmbFileType.Items.Clear()
                        If lsAllowedExtensions.Any Then
                            cmbFileType.Items.Add(New RadComboBoxItem(GetLabel("file"), "0"))
                        End If
                        lbCanAddFile = True
                        For Each t As TG_TEMPLATES_LIST.TG_TEMPLATES_INFO In lcolTemplates
                            cmbFileType.Items.Add(New RadComboBoxItem(t.Caption, t.SeqID.ToString))
                        Next
                    Else
                        cmbFileType.Visible = False
                    End If
                    rowFileUploadLang.Visible = lbCanAddFile

                    If rowFileUploadLang.Visible Then
                        cmbLangs.Items.Clear()
                        Dim lsDefLang As String = Settings.GetValue("Interface", "DefFileLang").ToUpper
                        Select Case lsDefLang
                            Case "AUTO", "NONE"
                                cmbLangs.Items.Add(New RadComboBoxItem("", "") With {.Selected = True})
                                rowFileUploadLang.Visible = False
                            Case ""
                                cmbLangs.Items.Add(New RadComboBoxItem(""))
                                For Each loLang As LanguageList.LanguageInfo In LanguageList.GetLanguageList
                                    Dim langitm As New RadComboBoxItem(loLang.Description, loLang.Code)
                                    cmbLangs.Items.Add(langitm)
                                Next
                            Case Else
                                rowFileUploadLang.Visible = False
                                cmbLangs.Items.Add(New RadComboBoxItem("", lsDefLang) With {.Selected = True})

                        End Select
                    End If
                    If lbCanAddFile Then
                        cmbFilePack.Items.Clear()
                        cmbFilePack.Items.Add(New RadComboBoxItem(GetLabel("files"), "0") With {.Selected = True})
                        For Each p As PackageList.PackageInfo In PackageList.GetCategoryPackageList(_category.ID, False)
                            If p.IsFilePackage AndAlso p.HasAccess(RoutingSecurity.LevelAccess.Edit, Nothing) Then

                                cmbFilePack.Items.Add(New RadComboBoxItem(CategoryLabels.GetObjectLabel(p.ID, "Package", p.Name), p.ID.ToString) With {.Selected = False})
                            End If
                        Next

                        rowFileUploadPack.Visible = (cmbFilePack.Items.Count > 1)
                    Else
                        rowFileUploadPack.Visible = False
                    End If

                    rowFileUpload.Visible = lbCanAddFile

                End If
            Else
                If meMode = eMode.MultiDoc Then
                    'can't do this!
                End If
                rowFileUpload.Visible = False
                rowFileUploadLang.Visible = False
                rowFileUploadPack.Visible = False
            End If


            If meMode = eMode.SingleDoc Then
                If _category.AutoName = AutoNamingMode.EmptyName OrElse _category.AutoName = AutoNamingMode.AutoName Then 'for doc, don't show the name box
                    rowName.Visible = False
                    Dim autoSave As Boolean = True
                    If _category.FileRequired Then
                        'if the file is required but it has an insert proc or an insert screen then go ahead
                        autoSave = (_category.InsertAction > 1 OrElse _category.NewObject(Folder.GetRoot).GetDetailScreen(Screen.DetailScreenDisplayMode.Edit, Device.Web, 7).ScreenItems.Any)
                    End If
                    If autoSave Then
                        Table2.Visible = False
                        Save(True)
                    End If
                Else
                    If _category.AutoName <> AutoNamingMode.RequiredName OrElse rowFileUpload.Visible Then
                        reqName.Enabled = False
                    End If
                End If
            End If

        End Sub
        Private Function GetRequestBody() As String
            Using ms As New MemoryStream()
                Request.InputStream.CopyTo(ms)
                Request.InputStream.Position = 0
                Using reader As New StreamReader(ms)
                    Return reader.ReadToEnd()
                End Using
            End Using

        End Function
        Private Function GetCreationInfoFromJson(ByVal json As String) As ObjectCreationInfo
            If String.IsNullOrEmpty(json) Then Return Nothing

            Arco.Utils.Logging.Debug("ObjectCreationInfo json: " + json)

            Return Newtonsoft.Json.JsonConvert.DeserializeObject(Of ObjectCreationInfo)(json)

        End Function


        Private Function GetCreationInfo(ByVal name As String, ByVal parentId As Integer) As ObjectCreationInfo
            Dim creationInfo As New ObjectCreationInfo
            creationInfo.SetCategory(_category)

            creationInfo.Name = name
            '  creationInfo.Description = txtCaseDesc.Text
            creationInfo.FolderId = parentId
            If FilledIn(txtPackID) Then
                If FilledIn(txtCaseID) Then
                    creationInfo.AddToCases = New List(Of CreationInfo.TargetCaseInfo) From {New CreationInfo.TargetCaseInfo() With {.CaseId = CType(txtCaseID.Text, Int32), .PackageId = CType(txtPackID.Text, Int32)}}
                End If
                If FilledIn(txtObjectID) Then
                    creationInfo.AddToObjects = New List(Of CreationInfo.TargetObjectInfo) From {New CreationInfo.TargetObjectInfo() With {.ObjectId = Convert.ToInt32(txtObjectID.Text), .PackageId = CType(txtPackID.Text, Int32)}}
                End If
            End If
            Dim inputSelection As String = QueryStringParser.GetString("INPUTSELECTION")
            If Not String.IsNullOrEmpty(inputSelection) Then
                creationInfo.SelectionToAddToInputPackage = CType(inputSelection, DMSelection.SelectionType)
            End If

            'don't commit/complete yet
            creationInfo.CommitCreation = False
            creationInfo.CompleteCreation = False
            Return creationInfo
        End Function

        Private _categoryLabels As LABELList = Nothing
        Private ReadOnly Property CategoryLabels As LABELList
            Get
                If _categoryLabels Is Nothing Then
                    _categoryLabels = LABELList.GetCategoryItemsLabelList(_category.ID, EnableIISCaching)
                End If
                Return _categoryLabels
            End Get
        End Property

        Private Sub SetLabels()
            lblName.Text = GetLabel("name")
            lnkSave.Text = GetLabel("insert")
            lnkCancel.Text = GetLabel("cancel")
            lblFile.Text = GetLabel("file")
            lblFolder.Text = GetLabel("folder")
            lblLang.Text = GetLabel("language")
            lblPack.Text = GetLabel("package")
            lblInvalidFileType.Text = GetLabel("invalidfileextension")
        End Sub

        Private Class FileToUpload
            Public Property CachedFile As String
            Public Property FileTitle As String
        End Class
        Private Class ObjectToUpload
            Public Property Title As String
            Public Property CachedFile As String
        End Class
        Protected Sub DoSave(ByVal sender As Object, ByVal e As EventArgs)
            Save(False)
        End Sub
        Private Sub Save(ByVal autosave As Boolean)

            Dim lbCreated As Boolean = False
            Dim gotoDetailScreen As Boolean = False

            Dim gotoFileEditScreen As Boolean = False
            Dim lsName As String
            Dim lsMSG As String = ""
            Dim lastErrorCode As LibError.ErrorCode
            Dim nameOk As Boolean = True
            Dim llParentID As Int32 = Convert.ToInt32(txtParentID.Text)
            '  Dim lsRefreshTree As String = ""
            Dim createdItem As DM_OBJECT = Nothing
            Dim lbMultiCreate As Boolean = False
            Dim createdIDs As New List(Of String)
            '  Dim lbShowInTree As Boolean = False

            Dim laFiles As New List(Of FileToUpload)
            Dim laObjects As New List(Of ObjectToUpload)

            Dim loAddedFile As Arco.Doma.Library.File = Nothing

            Dim lbFileOk As Boolean = autosave OrElse Not (_category.FileCardinalityConstraint = ConstraintEnum.AtLeastOne OrElse _category.FileCardinalityConstraint = ConstraintEnum.ExactOne)
            Dim liFileTemplate As Int32 = 0
            If cmbFileType.Visible Then
                liFileTemplate = Convert.ToInt32(cmbFileType.SelectedValue)
            End If

            If liFileTemplate = 0 Then
                For Each f As UploadedFile In txtFileUpload.UploadedFiles
                    Dim lsFileName As String = f.GetName()

                    If lsFileName.Length > 0 AndAlso f.ContentLength > 0 Then

                        Dim lsUniqueFileName As String = Arco.Utils.GUID.CreateGUIDWithCheckSum() & "." & Arco.IO.File.GetExtension(lsFileName)
                        Dim lsCacheDir As String = Settings.GetUploadPath()
                        Dim loF As FileToUpload = New FileToUpload
                        loF.CachedFile = Path.Combine(lsCacheDir, lsUniqueFileName)
                        f.SaveAs(loF.CachedFile)

                        loF.FileTitle = Arco.IO.File.GetFileName(lsFileName)
                        If loF.FileTitle.Length > 200 Then
                            loF.FileTitle = loF.FileTitle.Substring(0, 200)
                        End If

                        laFiles.Add(loF)

                        lbFileOk = True
                    End If
                Next
            Else
                lbFileOk = True
                meMode = eMode.SingleDoc 'template -> single doc
            End If

            If meMode = eMode.SingleDoc Then
                'create a single object
                lsName = txtObjectName.Text.Trim
                If lsName.Length = 0 AndAlso (_category.AutoName = AutoNamingMode.RequiredName OrElse _category.AutoName = AutoNamingMode.OptionalName) Then
                    If laFiles.Any Then
                        If lbFileOk AndAlso laFiles(0).FileTitle.Length <> 0 Then
                            lsName = laFiles(0).FileTitle
                        End If
                    End If
                End If

                If lsName.Length = 0 Then
                    If _category.AutoName = AutoNamingMode.RequiredName Then
                        lsMSG = lblName.Text & " " & GetLabel("req")
                        nameOk = False
                    Else
                        lsName = "[AUTONAME]"
                    End If
                End If
                If _category.CAT_TYPE = "Folder" AndAlso lsName.Contains("\") Then
                    lbMultiCreate = True
                    For Each lsFolder As String In lsName.Split("\"c)
                        laObjects.Add(New ObjectToUpload() With {.Title = lsFolder})
                    Next
                Else
                    laObjects.Add(New ObjectToUpload() With {.Title = lsName})
                End If
            Else
                'create a document for every file
                nameOk = True
                If laFiles.Any Then
                    For Each loF As FileToUpload In laFiles
                        laObjects.Add(New ObjectToUpload() With {.Title = loF.FileTitle, .CachedFile = loF.CachedFile})
                    Next
                Else
                    lsMSG = GetLabel("file") & " " & GetLabel("req")
                End If
            End If


            For Each loObjectToUpload As ObjectToUpload In laObjects.Where(Function(x) Not String.IsNullOrEmpty(x.Title))

                Dim objectName As String = Nothing
                If loObjectToUpload.Title <> "[AUTONAME]" Then
                    objectName = loObjectToUpload.Title
                End If
                Dim creationInfo As ObjectCreationInfo = GetCreationInfo(objectName, llParentID)

                If nameOk AndAlso _category.CAT_TYPE = "Folder" AndAlso _category.AutoName <> AutoNamingMode.AutoName Then
                    Dim oTest As Folder = Folder.GetFolder(llParentID, objectName)
                    If oTest IsNot Nothing Then
                        nameOk = False
                        If lbMultiCreate Then
                            llParentID = oTest.ID
                        End If
                    Else
                        nameOk = True
                    End If

                End If

                If Not nameOk Then
                    lsMSG = GetDecodedLabel("folderexists")
                    lsMSG = lsMSG.Replace(GetDecodedLabel("folder").ToLower, GetDecodedLabel(_category.CAT_TYPE).ToLower)

                    nameOk = lbMultiCreate
                    If Not nameOk Then
                        txtObjectName.Text = creationInfo.Name
                    End If
                    Continue For
                End If

                If Not lbFileOk Then
                    If String.IsNullOrEmpty(lsMSG) Then
                        lsMSG = GetLabel("file") & " " & GetLabel("req")
                        Exit For
                    End If
                End If

                Dim creationFactory As IObjectFactory = New ObjectFactory()

                Dim o As DM_OBJECT = creationFactory.CreateObject(creationInfo, Nothing)

                If o.ID = 0 OrElse o.HasError Then
                    lastErrorCode = o.GetLastError.Code
                    lsMSG = o.GetLastError.Description
                    If o.GetLastError.Code = LibError.ErrorCode.ERR_FOLDEREXISTS Then
                        lsMSG = lsMSG.Replace(GetDecodedLabel("folder").ToLower, GetDecodedLabel(_category.CAT_TYPE).ToLower)
                    End If
                    Continue For
                End If

                lbCreated = True
                If liFileTemplate = 0 Then
                    Dim lbFirst As Boolean = True
                    Dim lbGiveObjectNameToFile = (meMode = eMode.SingleDoc AndAlso loObjectToUpload.Title <> "[AUTONAME]" AndAlso laFiles.Count = 1)
                    For Each loF As FileToUpload In laFiles
                        If Not String.IsNullOrEmpty(loF.CachedFile) AndAlso (meMode = eMode.SingleDoc OrElse loF.CachedFile = loObjectToUpload.CachedFile) Then
                            If lbGiveObjectNameToFile Then
                                loF.FileTitle = loObjectToUpload.Title
                            End If
                            loAddedFile = o.AddFile(loF.FileTitle, loF.CachedFile, Not lbFirst, "", True, 0, cmbLangs.SelectedValue, True, Convert.ToInt32(cmbFilePack.SelectedValue))
                            'todo, what to do if we can't do this?
                            If loAddedFile Is Nothing Then
                                lbCreated = False
                                lsMSG = o.GetLastError.Description
                                Exit For
                            Else
                                lbFirst = False
                            End If
                        End If
                    Next

                Else
                    'we add a template
                    Dim loTemplate As TG_TEMPLATES = TG_TEMPLATES.GetTemplate(liFileTemplate)

                    Dim loGenerateTemplate As New GenerateTemplate
                    loAddedFile = loGenerateTemplate.GenerateTemplate(o, loTemplate, 0)

                    If loAddedFile IsNot Nothing Then
                        If loTemplate.AutoEdit Then
                            gotoFileEditScreen = loAddedFile.CanModifyContent
                        End If
                    Else
                        lbCreated = False
                        lsMSG = "Error : " & loGenerateTemplate.ErrorMessage
                    End If


                End If

                If lbCreated Then
                    o = o.CommitCreation

                    createdItem = o

                    If o.Case_ID > 0 Then
                        gotoDetailScreen = o.CurrentUserHasWork
                    Else
                        gotoDetailScreen = (o.Properties.Any)
                        If gotoDetailScreen Then
                            gotoDetailScreen = o.GetDetailScreen(Screen.DetailScreenDisplayMode.Edit, Device.Web, 7).ScreenItems.Any
                        End If
                    End If


                    If meMode = eMode.SingleDoc Then
                        llParentID = o.ID
                    End If


                    createdIDs.Add(o.ID.ToString)


                    If Not gotoDetailScreen OrElse gotoFileEditScreen Then
                        If o.Case_ID = 0 Then
                            o.CompleteCreation() 'direct completion (ex. folder)
                        End If
                    End If
                Else
                    o.RollBackCreation()
                End If


            Next

            If lbCreated Then

                Table1.Visible = False

                Dim refreshTree As String
                If UserProfile.ShowTree AndAlso createdItem.ShowInTree Then
                    refreshTree = "RefreshTree(" & txtParentID.Text & ");RefreshOpener();"
                Else
                    refreshTree = "RefreshOpener();"
                End If

                If gotoFileEditScreen Then
                    Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", "document.location.href='" & GetRedirectUrl("DM_EDIT_FILE.aspx?FILE_ID=" & loAddedFile.ID & "&closewindow=Y") & "';", True)
                ElseIf gotoDetailScreen Then 'if properties are found -> goto edit screen                   
                    Dim refreshGridOnClose As String = ""
                    If QueryStringParser.Modal Then
                        refreshGridOnClose = "GetRadWindow().add_close(parent.RefreshAfterModal);"
                    End If
                    If Not (meMode = eMode.MultiDoc AndAlso createdIDs.Count > 1) Then
                        Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", refreshGridOnClose & "document.location.href='" & GetRedirectUrl("dm_detail.aspx?DM_OBJECT_ID=" & createdItem.ID & "&folderid=" & txtParentID.Text & "&catid=" & txtCatID.Text & "&OBJECTLIST=" & String.Join(",", createdIDs) & "&mode=2&refreshtree=" & ToQRS(createdItem.ShowInTree)) & "';", True)
                    Else
                        Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", refreshGridOnClose & "document.location.href='" & GetRedirectUrl("MultiView.aspx?OBJECTLIST=" & String.Join(";", createdIDs) & "&mode=2&refreshtree=" & ToQRS(createdItem.ShowInTree)) & "';", True)
                    End If
                Else
                    Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", refreshTree & "Close();", True)
                End If
            Else
                If Not autosave Then
                    Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", "alert(" & EncodingUtils.EncodeJsString(lsMSG) & ");", True)
                Else
                    GotoErrorPage(lastErrorCode)
                End If

            End If

        End Sub


        Private Sub Save(ByVal creationInfo As ObjectCreationInfo)


            'create a single object

            Dim lbNameOk As Boolean = True

            Dim creationFactory As IObjectFactory = New ObjectFactory()

            Dim o As DM_OBJECT = creationFactory.CreateObject(creationInfo, Nothing)
            'o = o.Create(False, 0, False) 'commit, but don't complete yet!!
            If o.ID = 0 OrElse o.HasError Then
                Dim lsMsg As String = o.GetLastError.Description
                If o.GetLastError.Code = LibError.ErrorCode.ERR_FOLDEREXISTS Then
                    lsMsg = lsMsg.Replace(GetDecodedLabel("folder").ToLower, GetDecodedLabel(_category.CAT_TYPE).ToLower)
                End If
                Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", "PC().ShowError(" & EncodingUtils.EncodeJsString(lsMsg) & ",true);", True)
                Return
            End If



            Dim gotoDetailScreen As Boolean

            If o.Case_ID > 0 Then
                gotoDetailScreen = o.CurrentUserHasWork
            Else
                gotoDetailScreen = (o.Properties.Any)
                If gotoDetailScreen Then
                    gotoDetailScreen = o.GetDetailScreen(Screen.DetailScreenDisplayMode.Edit, Device.Web, 7).ScreenItems.Any
                End If
            End If



            If Not gotoDetailScreen Then
                If o.Case_ID = 0 Then
                    o.CompleteCreation() 'direct completion (ex. folder)
                End If
            End If


            Table1.Visible = False

            Dim refreshTree As String
            If UserProfile.ShowTree Then
                refreshTree = "RefreshTree(" & txtParentID.Text & ");RefreshOpener();"
            Else
                refreshTree = "RefreshOpener();"
            End If

            If gotoDetailScreen Then 'if properties are found -> goto edit screen                   
                Dim refreshGridOnClose As String = ""
                If QueryStringParser.Modal Then
                    refreshGridOnClose = "GetRadWindow().add_close(parent.RefreshAfterModal);"
                End If

                Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", refreshGridOnClose & "document.location.href='" & GetRedirectUrl("dm_detail.aspx?DM_OBJECT_ID=" & o.ID & "&folderid=" & txtParentID.Text & "&catid=" & txtCatID.Text & "&mode=2&refreshtree=" & ToQRS(o.ShowInTree)) & "';", True)

            Else
                Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", refreshTree & "Close();", True)
            End If


        End Sub


        Private Function ToQRS(ByVal value As Boolean) As String
            Return If(value, "Y", "")
        End Function

        Private Function FilledIn(ByVal txtField As WebControls.Label) As Boolean
            Return Not String.IsNullOrEmpty(txtField.Text) AndAlso txtField.Text <> "0"
        End Function
    End Class


End Namespace
