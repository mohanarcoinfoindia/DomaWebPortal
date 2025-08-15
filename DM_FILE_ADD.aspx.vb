Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Globalisation
Imports Arco.Doma.Library.Mail
Imports Arco.Doma.Library.Routing
Imports Arco.Doma.WebControls.SiteManagement
Imports Arco.Utils
Imports DomaGenTemplateEngine
Imports DomaGenTemplateEngine.baseObjects
Imports Telerik.Web.UI


Partial Class DM_FILE_ADD
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

    Protected ShowUrl As Boolean = True
    Protected ShowOcr As Boolean
    Protected PageGUID As String = ""
    Protected _isReplace As Boolean

    Private Enum eAddAs
        File = 0
        Url = 1
        Comment = 2
        Mail = 3
        Message = 4
    End Enum

    Private Sub LoadSettings()
        If Not Page.IsPostBack Then
            PageGUID = QueryStringParser.GetString("guid")

            If String.IsNullOrEmpty(PageGUID) Then
                PageGUID = Arco.Utils.GUID.CreateGUID
                Server.Transfer(Page.Request.ServerVariables("url") & "?" & Page.Request.ServerVariables("QUERY_STRING") & "&guid=" & PageGUID)
            End If
        End If
    End Sub
    Private Function FileOnClipboard() As Boolean
        Dim myCookie As HttpCookie = Request.Cookies.Get("FileClipBoard")
        Return (Not myCookie Is Nothing AndAlso Not String.IsNullOrEmpty(myCookie.Value))
    End Function

    Private Sub SetLabels()

        btnUploadDesktop.Text = GetLabel("upload")
        btnUploadUrl.Text = GetLabel("upload")
        lblName.Text = GetLabel("name") & ":"
        lblFile.Text = GetLabel("file") & ":"
        lblLang.Text = GetLabel("language") & ":"
        lblPack.Text = GetLabel("package") & ":"
        chkAlwaysOCR.Text = GetLabel("alwaysocr")
        btnCancelUrl.Text = GetLabel("cancel")
        btnCancelDesktop.Text = GetLabel("cancel")
        btnCancelClipboard.Text = GetLabel("cancel")
        lblInvalidFileType.Text = GetLabel("invalidfileextension")

        txtFileUpload.Localization.Select = GetDecodedLabel("dropfilesorclick")
        txtFileUpload.Localization.Remove = ""
    End Sub

#Region " Properties "
    Private Function GetIntFromHiddenField(ByVal hdn As System.Web.UI.HtmlControls.HtmlInputHidden) As Int32
        If Not String.IsNullOrEmpty(hdn.Value) Then
            Return Convert.ToInt32(hdn.Value)
        End If
        Return 0
    End Function
    Public ReadOnly Property ObjectId As Int32
        Get
            Return GetIntFromHiddenField(OBJECT_ID)
        End Get
    End Property
    Public ReadOnly Property FileId As Int32
        Get
            Return GetIntFromHiddenField(FILE_ID)
        End Get
    End Property
    Public ReadOnly Property CatId As Int32
        Get
            Return GetIntFromHiddenField(CAT_ID)
        End Get
    End Property
    Public ReadOnly Property DMParentID As Int32
        Get
            Return GetIntFromHiddenField(DM_PARENT_ID)
        End Get
    End Property
    Public ReadOnly Property CaseId As Int32
        Get
            Return GetIntFromHiddenField(CASE_ID)
        End Get
    End Property
    Public ReadOnly Property LinkToDin As Int32
        Get
            Return GetIntFromHiddenField(LINK_TO_DIN)
        End Get
    End Property
    Public ReadOnly Property LinkAs As Int32
        Get
            Return GetIntFromHiddenField(LINK_AS)
        End Get
    End Property
    Private _file As File
    Public ReadOnly Property CurrentFile As File
        Get
            If _file Is Nothing AndAlso FileId > 0 Then
                _file = File.GetFile(FileId)
            End If
            Return _file
        End Get
    End Property

    Private _obj As DM_OBJECT
    Public ReadOnly Property CurrentObject As DM_OBJECT
        Get
            If _obj Is Nothing Then
                If FileId > 0 Then
                    _obj = CurrentFile.LinkedToObject
                ElseIf ObjectId > 0 Then
                    _obj = ObjectRepository.GetObject(ObjectId)
                End If
            End If
            Return _obj
        End Get
    End Property
    Private _cat As OBJECT_CATEGORY
    Public ReadOnly Property CurrentCategory As OBJECT_CATEGORY
        Get
            If _cat Is Nothing Then
                If FileId > 0 Then
                    _cat = CurrentFile.LinkedToObject.CategoryObject
                ElseIf ObjectId > 0 Then
                    _cat = CurrentObject.CategoryObject
                ElseIf CatId > 0 Then
                    _cat = OBJECT_CATEGORY.GetOBJECT_CATEGORY(CatId)
                End If
            End If
            Return _cat
        End Get
    End Property

    Private ReadOnly Property AddFileAs As eAddAs
        Get
            Dim s As String = ADD_AS.Value
            If Not String.IsNullOrEmpty(s) Then
                Return [Enum].Parse(GetType(eAddAs), s)
                'Return CType(s, eAddAs)
            Else
                Return eAddAs.File
            End If
        End Get
    End Property

    Private _categoryLabels As LABELList = Nothing
    Private ReadOnly Property CategoryLabels As LABELList
        Get
            If _categoryLabels Is Nothing Then
                _categoryLabels = LABELList.GetCategoryItemsLabelList(CurrentCategory.ID, EnableIISCaching)
            End If
            Return _categoryLabels
        End Get
    End Property
#End Region

    Private Sub FillHiddenFieldsFromQueryString()
        OBJECT_ID.Value = QueryStringParser.GetInt("DM_OBJECT_ID")
        FILE_ID.Value = QueryStringParser.GetInt("FILE_ID")
        CAT_ID.Value = QueryStringParser.GetInt("CAT_ID")
        DM_PARENT_ID.Value = QueryStringParser.GetInt("DM_PARENT_ID")
        SelectedTab.Value = QueryStringParser.GetInt("TAB", 1)

        CASE_ID.Value = QueryStringParser.GetInt("CASE_ID")
        Dim packId As Integer = QueryStringParser.GetInt("PACK_ID", -1)
        If packId >= 0 Then
            PACK_ID.Value = packId
        End If

        LINK_TO_DIN.Value = QueryStringParser.GetInt("LINK_TO_DIN")
        LINK_AS.Value = QueryStringParser.GetInt("LINK_AS")
        ADD_AS.Value = QueryStringParser.GetInt("addas")
    End Sub

    Private canAddFile As Boolean = True

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        LoadSettings()

        If Not Page.IsPostBack Then
            FillHiddenFieldsFromQueryString()

            Dim canAddTemplate As Boolean = False

            Dim initLangCode As String = ""

            If QueryStringParser.GetBoolean("HIDEOCRBOX") Then
                chkAlwaysOCR.Visible = False
            End If

            rowName.Visible = False

            If ObjectId > 0 Then
                canAddTemplate = (AddFileAs = eAddAs.File)
                If Not CurrentObject.CanAddFile Then
                    Response.Write(Arco.Web.ErrorHandler.GetErrorForm(CurrentObject.GetLastError.Description))
                    Exit Sub
                End If
            ElseIf FileId > 0 Then
                'init filename textbox for replace



                initLangCode = CurrentFile.FILE_LANGCODE

                rdlReplaceAction.Items.Clear()

                Dim itm As System.Web.UI.WebControls.ListItem
                Dim canReplace As Boolean = False
                If CurrentFile.CanModify Then
                    itm = New System.Web.UI.WebControls.ListItem(GetLabel("overwriteversion"), "0", True)
                    itm.Selected = True
                    rdlReplaceAction.Items.Add(itm)
                    canReplace = True
                End If

                If Settings.GetValue("Versioning", "EnableFileVersioning", True) AndAlso CurrentFile.CanCheckOut Then
                    itm = New System.Web.UI.WebControls.ListItem(GetLabel("savenewversion"), "1", True)
                    If Not canReplace Then
                        itm.Selected = True
                    End If
                    rdlReplaceAction.Items.Add(itm)
                    rdlReplaceAction.Items.Add(New System.Web.UI.WebControls.ListItem(GetLabel("savenewsubversion"), "2", True))
                    canReplace = True

                End If
                If Not canReplace Then
                    Response.Write(Arco.Web.ErrorHandler.GetErrorForm(CurrentFile.GetLastError.Description))
                    Exit Sub
                End If

                rowOrigFileName.Visible = True
                lblOrigFileName.Text = CurrentFile.GetDownloadFileName()

                canAddTemplate = False 'don't allow replace with a template just yet
            ElseIf CatId > 0 Then
                rowName.Visible = True 'only show for new                   
            End If

            txtFileUpload.TemporaryFolder = Settings.GetUploadPath
            txtFileUpload.TargetFolder = ""
            txtFileUpload.MultipleFileSelection = AsyncUpload.MultipleFileSelection.Disabled
            txtFileUpload.MaxFileInputsCount = 1
            If FileId = 0 AndAlso CurrentCategory IsNot Nothing Then
                If CurrentCategory.FileCardinalityConstraint = ConstraintEnum.AtLeastOne OrElse CurrentCategory.FileCardinalityConstraint = ConstraintEnum.NoConstraint Then
                    txtFileUpload.MultipleFileSelection = AsyncUpload.MultipleFileSelection.Automatic
                    txtFileUpload.MaxFileInputsCount = 0
                End If
            End If
            txtFileUpload.UploadedFilesRendering = AsyncUpload.UploadedFilesRendering.AboveFileInput
            txtFileUpload.TemporaryFileExpiration = TimeSpan.FromMinutes(60)

            Dim allowedExtensions As List(Of String) = GetAllowedExtensions()

            If Not ShowOcr Then
                rowOcr.Visible = False
            End If

            SetAllowedExtensions(allowedExtensions)


            If canAddFile OrElse canAddTemplate Then

                rowFileType.Visible = False
                txtFileUpload.Visible = canAddFile

                If canAddTemplate Then
                    Dim loCrit As New TG_TEMPLATES_LIST.Criteria
                    loCrit.AllowedAt = TG_TEMPLATES.eAllowedAt.AddDocumentFile
                    loCrit.DocTypeID = CurrentCategory.ID

                    Dim lcolTemplates As TG_TEMPLATES_LIST = TG_TEMPLATES_LIST.GetTemplates(loCrit)
                    If lcolTemplates.Any Then
                        rowFileType.Visible = True
                        cmbFileType.Items.Clear()
                        If canAddFile Then
                            cmbFileType.Items.Add(New RadComboBoxItem(GetLabel("file"), "0"))
                        End If

                        For Each t As TG_TEMPLATES_LIST.TG_TEMPLATES_INFO In lcolTemplates
                            cmbFileType.Items.Add(New RadComboBoxItem(t.Caption, t.SeqID.ToString))
                        Next
                    End If
                End If
            Else
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDFILETYPE) 'no filetypes allowed                
            End If

            LoadLanguages(initLangCode)
            LoadFilePackages()
            txtFileName.MaxLength = CType(Settings.GetValue("Storage", "MaxNamesLength", "500"), Int32)
        Else
            'load the allowed extensions to fetch what to show
            GetAllowedExtensions()
        End If

        If FileId > 0 Then
            _isReplace = True
        End If
        If Not _isReplace Then
            Select Case AddFileAs
                Case eAddAs.Message
                    Page.Title = GetLabel("addmessage")
                Case eAddAs.Comment
                    Page.Title = GetLabel("addcomment")
                Case Else
                    Page.Title = GetLabel("ctx_AddFile")
            End Select
        Else
            Page.Title = GetLabel("ReplaceFile")
        End If
        Dim lbIsPaste As Boolean = False
        If Not Page.IsPostBack Then
            lbIsPaste = SelectedTab.Value = 5 AndAlso Not QueryStringParser.GetBoolean("isTABSWITCH")
        End If
        If Not lbIsPaste Then
            AddTab(1, GetLabel("file"))
            If ShowUrl Then
                AddTab(4, GetLabel("url"))
            End If
        End If

        If FileOnClipboard() AndAlso Not String.IsNullOrEmpty(OBJECT_ID.Value) AndAlso OBJECT_ID.Value <> "0" AndAlso AddFileAs = eAddAs.File Then
            AddTab(5, GetLabel("fromclipboard"))
            LoadClipboardPanel()
        End If

        If tabMain.Tabs.Count < 2 Then
            tabMain.Visible = False
        End If

        pnlUploadDesktop.Visible = False
        pnlUploadUrl.Visible = False
        pnlUploadClipboard.Visible = False

        Select Case SelectedTab.Value
            Case "1"
                Page.Form.DefaultButton = btnUploadDesktop.UniqueID

                pnlUploadDesktop.Visible = True
                pnlReplaceOptions.Visible = (_isReplace AndAlso rdlReplaceAction.Items.Count > 1)
            Case "4"
                Page.Form.DefaultButton = btnUploadUrl.UniqueID

                pnlUploadUrl.Visible = True
                pnlReplaceOptions.Visible = (_isReplace AndAlso rdlReplaceAction.Items.Count > 1)
            Case "5"
                pnlUploadClipboard.Visible = True
                LoadClipboardPanel()
        End Select

        SetLabels()
    End Sub

    Private Sub SetAllowedExtensions(ByVal allowedExtensions As List(Of String))
        If allowedExtensions.Any Then
            txtFileUpload.AllowedFileExtensions = allowedExtensions.ToArray

            allowedExtensions.Sort()

            lblFileTypes.Text = ArcoFormatting.AddTooltip("&nbsp;<img src='./Images/Help.png'/>", String.Join(", ", allowedExtensions.ToArray))
        Else
            'hide everything
            canAddFile = False
        End If
    End Sub

    Private Function ExtensionAllowedInMode(ByVal ext As String) As Boolean
        Select Case AddFileAs
            Case eAddAs.Comment
                If Settings.GetValue("Interface", "EnableHtmlComments", False) Then
                    Return ext = "htm" OrElse ext = "txt"
                End If
                Return ext = "txt"
            Case eAddAs.Mail, eAddAs.Message
                Return ext = "eml" OrElse ext = "msg"
            Case Else
                Return True
        End Select
    End Function
    Private Function GetAllowedExtensions() As List(Of String)
        Dim canShowOcrBox = Settings.GetValue("Interface", "ShowOcrBox", True)
        ShowOcr = False
        ShowUrl = False
        Dim allowedExtensions As New List(Of String)
        Dim lcolFileTypes As DM_FileTypeList = DM_FileTypeList.GetFileTypeList()
        Dim laCatOverrulles As New List(Of String)
        If CurrentCategory.ID > 0 Then
            For Each fileType As DM_FileTypeList.FileTypeInfo In lcolFileTypes
                If CurrentCategory.ID = fileType.CatID Then
                    laCatOverrulles.Add(fileType.Extension)
                    If fileType.FILE_MANUAL_UPLOAD AndAlso ExtensionAllowedInMode(fileType.Extension) Then
                        allowedExtensions.Add("." & fileType.Extension)

                        If fileType.FILE_OCR Then
                            ShowOcr = canShowOcrBox
                        End If
                        If fileType.Extension = "url" Then
                            ShowUrl = True
                        End If
                    End If
                End If
            Next
        End If
        If Not CurrentCategory.DisableGlobalFileTypes Then
            For Each fileType As DM_FileTypeList.FileTypeInfo In lcolFileTypes
                If fileType.CatID = 0 AndAlso Not laCatOverrulles.Contains(fileType.Extension) Then
                    If fileType.FILE_MANUAL_UPLOAD AndAlso ExtensionAllowedInMode(fileType.Extension) Then
                        allowedExtensions.Add("." & fileType.Extension)
                        If fileType.FILE_OCR Then
                            ShowOcr = canShowOcrBox
                        End If
                        If fileType.Extension = "url" Then
                            ShowUrl = True
                        End If
                    End If
                End If
            Next
        End If

        AddExtraExtension(allowedExtensions, "tif", "tiff")
        AddExtraExtension(allowedExtensions, "jpg", "jpeg")
        AddExtraExtension(allowedExtensions, "htm", "html")

        Return allowedExtensions
    End Function

    Private Sub AddExtraExtension(ByVal allowedExtensions As List(Of String), ByVal vsExt As String, ByVal vsExtra As String)
        If allowedExtensions.Contains("." & vsExt) AndAlso Not allowedExtensions.Contains("." & vsExtra) Then allowedExtensions.Add("." & vsExtra)
    End Sub

    Private Sub LoadLanguages(ByVal initiallangcode As String)
        If AddFileAs = eAddAs.Url OrElse AddFileAs = eAddAs.File Then
            cmbLangs.Items.Clear()
            Dim defLang As String = Settings.GetValue("Interface", "DefFileLang").ToUpper
            Select Case defLang
                Case "AUTO"
                    cmbLangs.Items.Add(New RadComboBoxItem("", initiallangcode) With {.Selected = True})
                    rowLang.Visible = False
                Case ""
                    cmbLangs.Items.Add(New RadComboBoxItem(""))
                    For Each loLang As LanguageList.LanguageInfo In LanguageList.GetLanguageList
                        Dim langitm As New RadComboBoxItem(loLang.Description, loLang.Code)
                        If initiallangcode = loLang.Code Then
                            langitm.Selected = True
                        End If
                        cmbLangs.Items.Add(langitm)
                    Next
                Case "NONE"
                    cmbLangs.Items.Add(New RadComboBoxItem("", ""))
                    rowLang.Visible = False
                Case Else
                    'fixed language
                    rowLang.Visible = False
                    If Not String.IsNullOrEmpty(initiallangcode) Then
                        cmbLangs.Items.Add(New RadComboBoxItem("", initiallangcode) With {.Selected = True})
                    Else
                        cmbLangs.Items.Add(New RadComboBoxItem("", defLang) With {.Selected = True})
                    End If
            End Select
        Else
            rowLang.Visible = False
        End If
    End Sub

    Private Function GetPackageId(ByVal target As IPackageContainer) As Int32
        If Not String.IsNullOrEmpty(PACK_ID.Value) Then
            Dim packId As Int32
            If Int32.TryParse(PACK_ID.Value, packId) Then
                Return packId
            End If

            If target IsNot Nothing Then
                Dim packInfo As PackageList.PackageInfo = target.GetPackageInfo(PACK_ID.Value)
                If packInfo IsNot Nothing Then
                    Return packInfo.ID
                End If
            End If
            Throw New InvalidOperationException("Package " & PACK_ID.Value & " was not found")
        End If

        Return -1
    End Function

    Private Sub LoadFilePackages()
        If FileId = 0 AndAlso Not CurrentObject Is Nothing AndAlso (AddFileAs = eAddAs.File OrElse AddFileAs = eAddAs.Url) Then
            Dim packID As Int32 = GetPackageId(CurrentObject)

            cmbFilePack.Items.Clear()
            If packID = -1 Then
                cmbFilePack.Items.Add(New RadComboBoxItem(GetLabel("files"), "0") With {.Selected = True})
                For Each p As PackageList.PackageInfo In CurrentObject.Packages
                    If p.IsFilePackage AndAlso CurrentObject.CanAddToPackage(p) Then
                        cmbFilePack.Items.Add(New RadComboBoxItem(CategoryLabels.GetObjectLabel(p.ID, "Package", p.Name), p.ID.ToString) With {.Selected = False})
                    End If
                Next
            ElseIf packID = 0 Then
                cmbFilePack.Items.Add(New RadComboBoxItem(GetLabel("files"), "0") With {.Selected = True})
            Else
                For Each p As PackageList.PackageInfo In CurrentObject.Packages
                    If p.IsFilePackage AndAlso packID = p.ID AndAlso CurrentObject.CanAddToPackage(p) Then
                        cmbFilePack.Items.Add(New RadComboBoxItem(p.Name, p.ID.ToString) With {.Selected = True})

                        If Not String.IsNullOrEmpty(p.DocInfo2) Then
                            SetAllowedExtensions(p.DocInfo2.Split(","c).Select(Function(x) "." & x.Trim()).ToList())
                        End If

                        Exit For
                    End If
                Next
            End If
            If cmbFilePack.Items.Count = 0 Then
                Throw New ArgumentException("Invalid Package ID")
            End If
            rowPack.Visible = (cmbFilePack.Items.Count > 1)
        Else
            'replace or add object or message
            rowPack.Visible = False
        End If
    End Sub
#Region " Tab Functions "

    Private Sub AddTab(ByVal viID As Int32, ByVal vsCaption As String)
        Dim loTab As RadTab = New RadTab
        loTab.Text = vsCaption
        If viID.ToString = SelectedTab.Value Then
            loTab.Selected = True
        Else
            loTab.NavigateUrl = "javascript:GotoTab('" & viID.ToString & "');"
        End If
        tabMain.Tabs.Add(loTab)
    End Sub
#End Region

#Region " Loading functions "

    Private Sub LoadClipboardPanel()

        If Not Page.IsPostBack Then
            Dim llSrcFileID As Int32 = 0
            Dim llMailSrcFileID As Int32 = 0
            Dim lsAction As String = ""

            Dim myCookie As HttpCookie = Request.Cookies.Get("FileClipBoard")
            If Not myCookie Is Nothing AndAlso Not String.IsNullOrEmpty(myCookie.Value) Then
                If myCookie.Value.Substring(0, 1) = "M" Then
                    llMailSrcFileID = Convert.ToInt32(myCookie.Value.Substring(1))
                Else
                    llSrcFileID = Convert.ToInt32(myCookie.Value)
                End If
            End If


            myCookie = Request.Cookies.Get("FileClipBoardAction")
            If Not myCookie Is Nothing AndAlso Not String.IsNullOrEmpty(myCookie.Value) Then
                lsAction = myCookie.Value
            End If
            If String.IsNullOrEmpty(lsAction) Then
                Exit Sub
            End If

            If llSrcFileID <> 0 Then

                Dim loSrcFile As File = File.GetFile(llSrcFileID)
                If Not loSrcFile.CanView Then
                    GotoErrorPage(loSrcFile.GetLastError.Code)
                    Exit Sub
                End If
                txtPasteFileName.Value = loSrcFile.Name
                btnUploadCLipboard.Text = GetDecodedLabel("paste")


            ElseIf llMailSrcFileID <> 0 Then
                Dim loMailSrcFile As MailFile = MailFile.GetFile(llMailSrcFileID)
                'If Not loMailSrcFile.CanView Then
                '    GotoErrorPage(loMailSrcFile.GetLastError.Code)
                '    Exit Sub
                'End If
                txtPasteFileName.Value = loMailSrcFile.Name
                btnUploadCLipboard.Text = GetDecodedLabel("paste")
            End If
        End If

    End Sub
#End Region

#Region " Upload functions "

    Private Sub DoFeedbackError(ByVal vsMsg As String)

        lblMessage.Text &= vsMsg

        'remove the dofeedbackok
        While pnlUploadMsg.Controls.Count > 1
            pnlUploadMsg.Controls.RemoveAt(1)
        End While

        ShowControlsForFeedback()

    End Sub
    Private Sub DoFeedbackOk(ByVal fileID As Int32)
        Dim loLit As New LiteralControl With {
            .Text = "<script language='javascript'>doFeedback(" & fileID & ");</script>"
        }

        pnlUploadMsg.Controls.Add(loLit)
        ShowControlsForFeedback()
    End Sub

    Private Sub ShowControlsForFeedback()

        pnlUploadMsg.Visible = True
        pnlReplaceOptions.Visible = False
        rowOrigFileName.Visible = False
    End Sub


    Private Sub UploadTemplate(ByVal viTemplateID As Integer)

        Dim loDocument As DM_OBJECT = ObjectRepository.GetObject(ObjectId)
        loDocument = loDocument.UpdateModificationDate
        'we add a template
        Dim loTemplate As TG_TEMPLATES = TG_TEMPLATES.GetTemplate(viTemplateID)

        Dim loGenerateTemplate As New GenerateTemplate
        Dim loAddedFile As File = loGenerateTemplate.GenerateTemplate(loDocument, loTemplate, 0)
        If Not loAddedFile Is Nothing Then
            If loTemplate.AutoEdit Then
                If loAddedFile.CanModifyContent Then
                    Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", "document.location.href='" & GetRedirectUrl("DM_EDIT_FILE.aspx?FILE_ID=" & loAddedFile.ID & "&closewindow=Y") & "';", True)
                Else
                    DoFeedbackOk(loAddedFile.ID)
                End If
            Else
                DoFeedbackOk(loAddedFile.ID)
            End If
        Else
            DoFeedbackError(loGenerateTemplate.ErrorMessage)
        End If


    End Sub
    Private Function UploadFile(ByVal vsTitle As String, ByVal vsFilePath As String, ByVal addAs As eAddAs, ByVal vsLangCode As String, ByVal doOCR As Boolean) As Boolean
        Dim lsFeedback As String = ""


        Dim loAddedFile As File = Nothing

        If FileId > 0 Then 'replace file        

            _obj = CurrentObject.UpdateModificationDate
            Select Case rdlReplaceAction.SelectedValue
                Case "0" 'override -> with default beheaviour?
                    If File.TrySetLockingStatus(CurrentFile, File.LockingStatus.DirectLock) Then
                        Dim lsOrigTitle As String = CurrentFile.Name
                        Select Case addAs
                            Case eAddAs.Url
                                loAddedFile = CurrentFile.ReplaceUrl(vsFilePath, vsFilePath, vsLangCode, False)
                            Case Else
                                loAddedFile = CurrentFile.ReplaceFile(vsFilePath, vsTitle, vsLangCode, True, False)
                        End Select
                        If loAddedFile.HasError Then
                            lsFeedback = loAddedFile.GetLastError.Description
                            loAddedFile.Name = lsOrigTitle
                        End If
                        loAddedFile = loAddedFile.SetLockingStatus(File.LockingStatus.NoLock)
                    Else
                        lsFeedback = CurrentFile.GetLastError.Description
                    End If
                Case "1", "2" 'new major
                    Dim lbNewMajor As Boolean = False
                    Dim lbNewMinor As Boolean = False
                    Dim lbError As Boolean = False
                    If rdlReplaceAction.SelectedValue = "1" Then
                        lbNewMajor = True
                    Else
                        lbNewMinor = True
                    End If
                    If CurrentFile.Status <> File.File_Status.InProgress Then
                        _file = CurrentFile.Checkout("", False)
                        If _file Is Nothing Then
                            lbError = True
                            lsFeedback = "Unable to checkout"
                        ElseIf _file.HasError Then
                            lbError = True
                            lsFeedback = _file.GetLastError.Description
                        End If
                    End If
                    If Not lbError Then
                        Select Case addAs
                            Case eAddAs.Url
                                loAddedFile = CurrentFile.ReplaceUrl(vsFilePath, vsFilePath, vsLangCode, False)
                            Case Else
                                loAddedFile = CurrentFile.ReplaceFile(vsFilePath, vsTitle, vsLangCode, True, False)
                        End Select
                        If loAddedFile.HasError Then
                            lsFeedback = loAddedFile.GetLastError.Description
                            loAddedFile = loAddedFile.CancelCheckout
                        Else
                            loAddedFile = loAddedFile.CheckIn(lbNewMajor, lbNewMinor, "")
                        End If
                    Else
                        lsFeedback = CurrentFile.GetLastError.Description
                    End If

            End Select

        ElseIf ObjectId > 0 Then 'add file                
            _obj = CurrentObject.UpdateModificationDate
            'call addfile  with all params of addmainfile
            Select Case addAs
                Case eAddAs.Url
                    loAddedFile = CurrentObject.AddUrl(vsFilePath, vsFilePath, False, "", False, CType(cmbFilePack.SelectedValue, Int32))
                Case eAddAs.Comment
                    loAddedFile = CurrentObject.AddComment(vsTitle, vsFilePath, True)
                Case eAddAs.Mail
                    loAddedFile = CurrentObject.AddMail(vsTitle, vsFilePath, True, False)
                Case eAddAs.Message
                    loAddedFile = CurrentObject.AddMessage(vsTitle, vsFilePath, "", True, False)
                Case Else
                    loAddedFile = CurrentObject.AddFile(vsTitle, vsFilePath, False, "", True, 0, vsLangCode, False, CType(cmbFilePack.SelectedValue, Int32))
            End Select
            If loAddedFile Is Nothing Then
                lsFeedback = CurrentObject.GetLastError.Description
            ElseIf loAddedFile.HasError Then
                lsFeedback = loAddedFile.GetLastError.Description
            End If
        Else 'If llCatID > 0 Then
            _obj = Document.NewDocument(CurrentCategory.ID, DMParentID)
            _obj.Name = vsTitle
            _obj = _obj.Create(True, 0, False) 'commit, but don't complete yet!!
            If _obj.ID > 0 Then

                Select Case addAs
                    Case eAddAs.Url
                        loAddedFile = CurrentObject.AddUrl(vsFilePath, vsFilePath, False, "", False, 0)
                    Case eAddAs.Comment
                        loAddedFile = CurrentObject.AddComment(vsTitle, vsFilePath, True)
                    Case eAddAs.Mail
                        loAddedFile = CurrentObject.AddMail(vsTitle, vsFilePath, True, False)
                    Case eAddAs.Message
                        loAddedFile = CurrentObject.AddMessage(vsTitle, vsFilePath, "", True, False)
                    Case Else
                        loAddedFile = CurrentObject.AddFile(vsTitle, vsFilePath, False, "", True, 0, vsLangCode, False, 0)
                End Select
                If loAddedFile Is Nothing Then
                    lsFeedback = CurrentObject.GetLastError.Description
                ElseIf loAddedFile.HasError Then
                    lsFeedback = loAddedFile.GetLastError.Description
                Else

                    If Not String.IsNullOrEmpty(PACK_ID.Value) AndAlso CaseId > 0 Then
                        Dim targetCase As cCase = cCase.GetCaseByCaseID(CaseId)
                        targetCase.AddToPackage(GetPackageId(targetCase), CurrentObject)
                    End If
                    If LinkToDin > 0 AndAlso LinkAs > 0 Then
                        Dim loTargetObj As DM_OBJECT = ObjectRepository.GetObjectByDIN(LinkToDin)
                        CurrentObject.LinkTo(loTargetObj, LinkAs)
                    End If
                    If CurrentObject.Properties.Any OrElse CurrentObject.Case_ID > 0 Then 'if properties are found -> goto edit screen
                        Page.ClientScript.RegisterStartupScript(GetType(String), "NewObject", "document.location.href='" & GetRedirectUrl("dm_detail.aspx?DM_OBJECT_ID=" & CurrentObject.ID & "&folderid=" & CurrentObject.Parent_ID & "&catid=" & CurrentObject.Category & "&mode=2&isnew=Y&refreshtree=Y") & "'", True)
                    Else
                        _obj = CurrentObject.CompleteCreation
                    End If
                End If
            Else
                lsFeedback = CurrentObject.GetLastError.Description
            End If
        End If

        If String.IsNullOrEmpty(lsFeedback) Then
            If loAddedFile Is Nothing Then
                lsFeedback = CurrentObject.GetLastError.Description
                DoFeedbackError(lsFeedback)
                Return False
            Else
                Dim loIdxParams As New FileIndexingParameters(FileId > 0) With {.OCR = doOCR}
                loAddedFile.Index(loIdxParams, FileBase.eIndexingMode.Auto)
                DoFeedbackOk(loAddedFile.ID)
                Return True
            End If
        Else
            DoFeedbackError(lsFeedback)
            Return False
        End If

    End Function

    Private Function UploadFileFromCache(ByVal lsCachedFile As String, ByVal addAs As eAddAs, ByVal vsLangCode As String, ByVal doOCR As Boolean, ByVal filetitle As String) As Boolean

        If String.IsNullOrEmpty(filetitle) Then
            filetitle = Arco.IO.File.GetFileName(lsCachedFile)
        End If

        Return UploadFile(filetitle, lsCachedFile, addAs, vsLangCode, doOCR)

    End Function
#End Region

#Region " Buttons "

    Protected Sub btnUploadDesktop_Click1(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUploadDesktop.Click

        Dim fileTemplate As Int32 = 0
        If cmbFileType.Visible Then
            fileTemplate = Convert.ToInt32(cmbFileType.SelectedValue)
        End If
        If fileTemplate = 0 Then
            Dim uploadFileCollection As UploadedFileCollection = txtFileUpload.UploadedFiles

            If uploadFileCollection.Count <> 0 Then
                Dim fileTypeList As DM_FileTypeList = DM_FileTypeList.GetFileTypeList
                Dim success As Boolean = True

                For Each f As UploadedFile In uploadFileCollection
                    Dim fileName As String = f.GetName()

                    If fileName.Length = 0 Then
                        DoFeedbackError("Nothing to upload")
                        Exit For
                    End If
                    If f.ContentLength = 0 Then
                        DoFeedbackError(GetLabel("emptyfilesnotallowed"))
                        Exit For
                    End If
                    If Not fileTypeList.isValidExtension(Arco.IO.File.GetExtension(fileName)) Then
                        DoFeedbackError("Invalid file type")
                        Exit For
                    End If

                    Dim lsUniqueFileName As String = Arco.Utils.GUID.CreateGUIDWithCheckSum() & "." & Arco.IO.File.GetExtension(fileName)
                    Dim lsFileTitle As String = txtFileName.Text
                    If String.IsNullOrEmpty(lsFileTitle) Then
                        lsFileTitle = Arco.IO.File.GetFileName(fileName).RestrictTo(200)
                        If lsFileTitle.Length > 200 Then
                            lsFileTitle = lsFileTitle.Substring(0, 200)
                        End If
                    End If
                    Dim lsCacheDir As String = Settings.GetUploadPath
                    Dim lsCachedFile As String = System.IO.Path.Combine(lsCacheDir, lsUniqueFileName)
                    f.SaveAs(lsCachedFile)
                    If Not UploadFileFromCache(lsCachedFile, AddFileAs, cmbLangs.SelectedValue, (Not chkAlwaysOCR.Visible OrElse chkAlwaysOCR.Checked), lsFileTitle) Then
                        success = False
                        Exit For
                    End If

                Next

                pnlUploadDesktop.Visible = Not success


            End If
        Else
            UploadTemplate(fileTemplate)
        End If

    End Sub

    Protected Sub btnUploadUrl_Click1(ByVal sender As Object, ByVal e As EventArgs) Handles btnUploadUrl.Click
        Dim lsUrl As String = txtUrl.Text.Trim
        If lsUrl.Length = 0 Then
            Exit Sub
        End If

        UploadFile(lsUrl, lsUrl, eAddAs.Url, "", False)

        pnlUploadUrl.Visible = False

    End Sub

    Protected Sub btnUploadCLipboard_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUploadCLipboard.Click
        Dim llSrcFileID As Int32 = 0
        Dim llMailSrcFileID As Int32 = 0
        Dim clipboardAction As String = ""
        Dim lsFeedback As String = ""

        Dim myCookie As HttpCookie = Request.Cookies.Get("FileClipBoard")
        If Not myCookie Is Nothing AndAlso Not String.IsNullOrEmpty(myCookie.Value) Then
            If myCookie.Value.Substring(0, 1) = "M" Then
                llMailSrcFileID = Convert.ToInt32(myCookie.Value.Substring(1))
            Else
                llSrcFileID = Convert.ToInt32(myCookie.Value)
            End If
        End If

        myCookie = Request.Cookies.Get("FileClipBoardAction")
        If Not myCookie Is Nothing AndAlso Not String.IsNullOrEmpty(myCookie.Value) Then
            clipboardAction = myCookie.Value
        End If
        If String.IsNullOrEmpty(clipboardAction) Then
            Exit Sub
        End If

        Dim llNewFileID As Int32 = 0
        Dim targetObject As DM_OBJECT = ObjectRepository.GetObject(ObjectId)
        Dim packID As Int32 = GetPackageId(Nothing)
        If packID = -1 Then
            packID = 0
        ElseIf packID > 0 AndAlso Not targetObject.CanAddToPackage(packID) Then
            DoFeedbackError(targetObject.GetLastError.Description)
            Exit Sub
        End If

        If llSrcFileID <> 0 Then
            Dim loSrcFile As File = File.GetFile(llSrcFileID)

            If clipboardAction = "Copy" Then
                loSrcFile = loSrcFile.Copy(targetObject, True)

                If loSrcFile.GetLastError.Code = LibError.ErrorCode.ERR_NOERROR Then
                    llNewFileID = loSrcFile.ID
                Else
                    lsFeedback = loSrcFile.GetLastError.Description
                End If
            ElseIf clipboardAction = "Cut" Then
                If loSrcFile.OBJECT_ID <> targetObject.ID Then
                    loSrcFile = loSrcFile.Move(targetObject)
                End If

                If loSrcFile.GetLastError.Code = LibError.ErrorCode.ERR_NOERROR Then
                    llNewFileID = loSrcFile.ID
                Else
                    lsFeedback = loSrcFile.GetLastError.Description
                End If
            End If
            If llNewFileID > 0 Then
                loSrcFile.PackageID = packID
                If Not String.IsNullOrWhiteSpace(txtPasteFileName.Value) Then
                    loSrcFile.Name = txtPasteFileName.Value
                End If
                loSrcFile.Save()
            End If

        ElseIf llMailSrcFileID <> 0 Then
            Dim loSrcFile As MailFile = MailFile.GetFile(llMailSrcFileID)
            If clipboardAction = "Copy" Then

                Dim loFile As File = loSrcFile.CopyToObject(targetObject, True)
                If Not loFile Is Nothing AndAlso loFile.ID > 0 Then
                    llNewFileID = loSrcFile.ID
                    loFile.PackageID = packID
                    If Not String.IsNullOrWhiteSpace(txtPasteFileName.Value) Then
                        loFile.Name = txtPasteFileName.Value
                        loFile.Save()
                    End If
                Else
                    lsFeedback = targetObject.GetLastError.Description
                End If
            End If

        End If
        pnlUploadClipboard.Visible = False

        If Not String.IsNullOrEmpty(lsFeedback) Then
            DoFeedbackError(lsFeedback)
        Else
            DoFeedbackOk(llNewFileID)
        End If
    End Sub
#End Region


End Class

