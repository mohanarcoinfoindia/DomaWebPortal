Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports System.Net.Mail
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Mail
Imports Arco.Doma.Library.Routing
Imports Telerik.Web.UI
Imports AngleSharp.Css

''' <summary>
''' Page to compose emails
''' </summary>
''' <remarks>
''' Input Params : 
''' DM_MAIL_ID : source mail id
''' action (in combination with DM_MAIL_ID) : reply,replyall,forward
''' DM_OBJECT_ID : object to mail
''' mailsel : DMSelection.SelectionType to mail
''' DM_ADD_TO_OBJECT_ID = target object id, object in which to link the mail
''' DM_ADD_TO_OBJECT_FILE_TYPE = filetype as which to link the mail (mailcomment or mail)
''' TEMPLATE_ID : Template to use
''' </remarks>
Public Class MailClient_Compose2
    Inherits BasePage

    Dim mlCnt As Int32

#Region " Parameters "
    Protected Property SourceSelection() As DMSelection.SelectionType
        Get
            Try
                If Not ViewState("SourceSelection") Is Nothing Then
                    Return CType(ViewState("SourceSelection"), DMSelection.SelectionType)
                Else
                    Return DMSelection.SelectionType.Undefined
                End If
            Catch ex As Exception
                Return DMSelection.SelectionType.Undefined
            End Try

        End Get
        Set(ByVal value As DMSelection.SelectionType)
            ViewState("SourceSelection") = value
        End Set
    End Property
    Protected Property MailAction() As String
        Get
            Return ViewState("MailAction").ToString
        End Get
        Set(ByVal value As String)
            ViewState("MailAction") = value
        End Set
    End Property

    Protected Property AllowExternalTo() As Boolean
        Get
            Dim b As Boolean = False
            Boolean.TryParse(ViewState("AllowExternalTo"), b)
            Return b
        End Get
        Set(ByVal value As Boolean)
            ViewState("AllowExternalTo") = value
        End Set
    End Property

    Protected Property AllowExternalCc() As Boolean
        Get
            Dim b As Boolean = False
            Boolean.TryParse(ViewState("AllowExternalCc"), b)
            Return b
        End Get
        Set(ByVal value As Boolean)
            ViewState("AllowExternalCc") = value
        End Set
    End Property

    Protected Property AllowExternalBcc() As Boolean
        Get
            Dim b As Boolean = False
            Boolean.TryParse(ViewState("AllowExternalBcc"), b)
            Return b
        End Get
        Set(ByVal value As Boolean)
            ViewState("AllowExternalBcc") = value
        End Set
    End Property

    Protected Property Track() As Boolean
        Get
            Dim b As Boolean = False
            Boolean.TryParse(ViewState("Track"), b)
            Return b
        End Get
        Set(ByVal value As Boolean)
            ViewState("Track") = value
        End Set
    End Property

    Protected Property SendNotification() As Boolean
        Get
            Dim b As Boolean = False
            Boolean.TryParse(ViewState("SendNotification"), b)
            Return b
        End Get
        Set(ByVal value As Boolean)
            ViewState("SendNotification") = value
        End Set
    End Property

    Protected Property Archive() As Boolean
        Get
            Dim b As Boolean = False
            Boolean.TryParse(ViewState("Archive"), b)
            Return b
        End Get
        Set(ByVal value As Boolean)
            ViewState("Archive") = value
        End Set
    End Property

    Protected Property SourceCaseTechID() As Int32
        Get

            Return Convert.ToInt32(ViewState("SourceCaseTechID"))
        End Get
        Set(ByVal value As Int32)
            ViewState("SourceCaseTechID") = value
        End Set
    End Property
    Private moSourceCase As cCase
    Protected ReadOnly Property SourceCase() As cCase
        Get
            If moSourceCase Is Nothing AndAlso SourceCaseTechID > 0 Then
                moSourceCase = cCase.GetCase(SourceCaseTechID)
            End If
            Return moSourceCase
        End Get
    End Property
    Private moSourceObject As DM_OBJECT
    Protected ReadOnly Property SourceObject() As DM_OBJECT
        Get
            If moSourceObject Is Nothing AndAlso SourceObjectID > 0 Then
                moSourceObject = ObjectRepository.GetObject(SourceObjectID)
            End If
            Return moSourceObject
        End Get
    End Property
    Protected Property SourceObjectID() As Int32
        Get

            Return Convert.ToInt32(ViewState("SourceObjectID"))

        End Get
        Set(ByVal value As Int32)
            ViewState("SourceObjectID") = value
        End Set
    End Property
    Protected Property TargetFileType() As Int32
        Get

            Return Convert.ToInt32(ViewState("TargetFileType"))

        End Get
        Set(ByVal value As Int32)
            ViewState("TargetFileType") = value
        End Set
    End Property
    Protected Property TargetPackId() As Int32
        Get

            Return Convert.ToInt32(ViewState("TargetPackId"))

        End Get
        Set(ByVal value As Int32)
            ViewState("TargetPackId") = value
        End Set
    End Property
    Protected Property TargetObjectID() As Int32
        Get

            Return Convert.ToInt32(ViewState("TargetObjectID"))

        End Get
        Set(ByVal value As Int32)
            ViewState("TargetObjectID") = value
        End Set
    End Property
    Protected Property SourceMailID() As Int32
        Get

            Return Convert.ToInt32(ViewState("SourceMailID"))

        End Get
        Set(ByVal value As Int32)
            ViewState("SourceMailID") = value
        End Set
    End Property
    Protected Property TemplateID() As Int32
        Get

            Return Convert.ToInt32(ViewState("TemplateID"))

        End Get
        Set(ByVal value As Int32)
            ViewState("TemplateID") = value
        End Set
    End Property
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not Settings.GetValue("MailClient", "Enabled", True) Then
            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOPERATION)
            Return
        End If




        lblError.Text = ""
        lblError.Visible = False

        tlbMain.Style.Add("width", "100%")

        txtFileUpload.Localization.Remove = ""
        txtFileUpload.Localization.Select = GetDecodedLabel("dropfilesorclick")

        If Not Page.IsPostBack Then
            InitParams()

            If TargetObjectID > 0 Then
                Select Case TargetFileType
                    Case 4
                        'Check if object allows adding messages
                        Dim loObject As DM_OBJECT = ObjectRepository.GetObject(TargetObjectID)
                        If Not loObject.CanAddMessage(TargetPackId) Then
                            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                            Return
                        End If
                    Case Else
                        Dim loObject As DM_OBJECT = ObjectRepository.GetObject(TargetObjectID)
                        If Not loObject.CanAddFile(TargetPackId) Then
                            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                            Return
                        End If
                End Select


            End If

            Dim allowedExtensions As List(Of String) = GetAllowedExtensions()

            If allowedExtensions.Any Then
                txtFileUpload.TemporaryFolder = Settings.GetUploadPath
                txtFileUpload.TargetFolder = ""
                txtFileUpload.MultipleFileSelection = AsyncUpload.MultipleFileSelection.Automatic

                'If FileId = 0 AndAlso Not CurrentObject Is Nothing Then
                '    If CurrentCategory.FileCardinalityConstraint = ConstraintEnum.AtLeastOne OrElse CurrentCategory.FileCardinalityConstraint = ConstraintEnum.NoConstraint Then
                '        txtFileUpload.MultipleFileSelection = AsyncUpload.MultipleFileSelection.Automatic
                '        txtFileUpload.MaxFileInputsCount = 0
                '    End If
                'End If
                txtFileUpload.UploadedFilesRendering = AsyncUpload.UploadedFilesRendering.AboveFileInput
                txtFileUpload.TemporaryFileExpiration = TimeSpan.FromMinutes(60)


                txtFileUpload.AllowedFileExtensions = allowedExtensions.ToArray

                allowedExtensions.Sort()

                lblFileTypes.Text = ArcoFormatting.AddTooltip("&nbsp;<img src='../Images/Help.png'/>", String.Join(", ", allowedExtensions.ToArray))
            Else
                'hide everything
                trATT.Visible = True
            End If

            lblAtt.Visible = False
            BuildDocMessage()
            CreateForm()
        End If

        SetFromAddress()

        Translate()
    End Sub
    Private Function GetAllowedExtensions() As List(Of String)
        Dim allowedExtensions As New List(Of String)

        For Each fileType As DM_FileTypeList.FileTypeInfo In DM_FileTypeList.GetFileTypeList()
            If fileType.CatID = 0 Then
                If fileType.FILE_MANUAL_UPLOAD Then
                    allowedExtensions.Add("." & fileType.Extension)

                End If
            End If
        Next

        AddExtraExtension(allowedExtensions, "tif", "tiff")
        AddExtraExtension(allowedExtensions, "jpg", "jpeg")
        AddExtraExtension(allowedExtensions, "htm", "html")

        Return allowedExtensions
    End Function
    Private Sub AddExtraExtension(ByVal allowedExtensions As List(Of String), ByVal vsExt As String, ByVal vsExtra As String)
        If allowedExtensions.Contains("." & vsExt) AndAlso Not allowedExtensions.Contains("." & vsExtra) Then allowedExtensions.Add("." & vsExtra)
    End Sub
    Private Sub Translate()

        Try
            If TargetObjectID > 0 AndAlso TargetFileType = 4 Then
                Title = GetLabel("addmessage")
            Else
                Title = GetLabel("sendmail")
            End If
            lblFromHeader.Text = GetLabel("mail_from")
            lblTo.Text = GetLabel("mail_to")
            lblCC.Text = GetLabel("mail_cc")
            lblBCC.Text = GetLabel("mail_bcc")

            lblSubject.Text = GetLabel("mail_subject")
            'cmdAdd.Text = GetLabel("mail_add")
            chkZip.Text = GetLabel("mail_zipatt")
            lblHyperlinkFiles.Text = GetLabel("mail_files")
            tlbMain.Items(0).Text = GetLabel("mail_send")
            lblAtt.Text = GetLabel("mail_attachments")
            cmdAddAtt.Text = GetLabel("add")

            CType(btnchkReplyRequired.FindControl("chkReplyRequired"), Controls.CheckBox).Attributes("title") = GetDecodedLabel("requireresponse")
            CType(btnchkReplyRequired.FindControl("lblReplyRequired"), Label).Text = GetDecodedLabel("requireresponse")
            'CType(tlbMain.Items(0).FindControl("lblSendEmail"), Label).Text = GetDecodedLabel("mail_send")
            chkIncludeDelegates.Text = GetLabel("includedelegations")
            If Track Then
                lblCC.Text = GetLabel("mail_external")
            End If
        Catch ex As Exception

        End Try
    End Sub

    Public Property ReplyRequired As Boolean
        Get
            If Not btnchkReplyRequired.Visible Then
                Return Template.ReplyRequired = CheckBoxDisplayMode.Always
            End If
            Return CType(btnchkReplyRequired.FindControl("chkReplyRequired"), Controls.CheckBox).Checked
        End Get
        Set(value As Boolean)
            CType(btnchkReplyRequired.FindControl("chkReplyRequired"), Controls.CheckBox).Checked = value
        End Set
    End Property
    Public ReadOnly Property IncludeDelegates As Boolean
        Get
            If Not chkIncludeDelegates.Visible Then
                Return Template.IncludeDelegates = CheckBoxDisplayMode.Always
            End If
            Return chkIncludeDelegates.Checked
        End Get

    End Property

    Private Function RecipientsSelected() As Boolean
        Return txtTo.Entries.Count <> 0 OrElse txtCC.Entries.Count <> 0 OrElse txtBCC.Entries.Count <> 0
    End Function

    Private Sub SendMail()
        Dim lbOK_To As Boolean
        Dim lbOK_Cc As Boolean = True
        Dim lbOK_Bcc As Boolean = True
        Dim lsToEmails As New List(Of String), lsCcEmails As New List(Of String), lsBccEmails As New List(Of String)
        Dim lsToExternalEmails As New List(Of String), lsCCExternalEmails As New List(Of String), lsBCCExternalEmails As New List(Of String)
        Dim lsToAccounts As New List(Of String), lsCCAccounts As New List(Of String), lsBccAccounts As New List(Of String)

        Dim lsNotifEmails As New List(Of String)


        maTempFiles = New List(Of String)

        If Not RecipientsSelected() Then
            ShowError(GetLabel("mail_recipients") & " " & GetLabel("req"))
            Exit Sub
        End If

        lbOK_To = CheckRecipient(txtTo, lsToEmails, lsToAccounts, lsToExternalEmails, lsNotifEmails, True, Not Track, True)
        If txtCC.Text.Trim.Length > 0 Then
            If Track Then
                lbOK_Cc = CheckRecipient(txtCC, lsToEmails, lsToAccounts, lsToExternalEmails, lsNotifEmails, False, True, False)
            Else
                lbOK_Cc = CheckRecipient(txtCC, lsCcEmails, lsCCAccounts, lsCCExternalEmails, Nothing, False, True, True)
            End If
        End If
        If txtBCC.Text.Trim.Length > 0 Then lbOK_Bcc = CheckRecipient(txtBCC, lsBccEmails, lsCCAccounts, lsBCCExternalEmails, Nothing, False, True, True)

        If Not (lbOK_To AndAlso lbOK_Cc AndAlso lbOK_Bcc) Then
            Exit Sub
        End If


        Dim lsRedirectUrl As String = ""

        Dim mailLink As Int32 = 0
        Dim ArchiveOK As Boolean = False

        If Archive Then
            Dim loMail As DMMail
            If SourceMailID > 0 Then
                Dim sourceMail As DMMail = DMMail.GetMail(SourceMailID)
                If Not sourceMail.CanView Then
                    ShowError("Access Denied")
                    Return
                End If

                Select Case MailAction
                    Case "FORWARD"
                        If Not sourceMail.CanForward Then
                            ShowError("Access Denied")
                            Return
                        End If
                        loMail = DMMail.NewMail(sourceMail, TrackItem.MailDirection.Forward)
                    Case Else 'reply
                        If Not sourceMail.CanReply Then
                            ShowError("Access Denied")
                            Return
                        End If
                        loMail = DMMail.NewMail(sourceMail, TrackItem.MailDirection.Reply)
                End Select
            Else
                loMail = DMMail.NewMail(Template)
            End If

            loMail.ReplyRequired = ReplyRequired

            loMail.From = New DMMail.MailRecipient With {.Account = Security.BusinessIdentity.CurrentIdentity.Name}
            loMail.Subject = txtSubject.Text

            loMail.MailTo.AddFromString(ListToString(lsToAccounts))
            loMail.MailTo.AddFromString(ListToString(lsToExternalEmails))

            loMail.CC.AddFromString(ListToString(lsCCAccounts))
            loMail.CC.AddFromString(ListToString(lsCCExternalEmails))

            loMail.BCC.AddFromString(ListToString(lsBccAccounts))
            loMail.BCC.AddFromString(ListToString(lsBCCExternalEmails))

            If Not String.IsNullOrEmpty(RadEditor1.Text) Then
                loMail.Abstract = Arco.Utils.AbstractExtraction.ExtractFromString(RadEditor1.Text)
            End If

            loMail.Subject = txtSubject.Text
            loMail = loMail.Save()

            mailLink = loMail.ID

            If loMail.ID > 0 Then
                loMail.AddBody(HtmlSanitizer.Sanitize(RadEditor1.Content), True)

                If SourceMailID = 0 Then
                    'the fixed message will be added automatically if it's a forward/reply, so only add for a new mail
                    loMail.AddFixedMessage(lblTemplate.Text, True)
                End If

                AddAttachmentsToDMMail(loMail)

                If TargetObjectID > 0 Then 'index as file on an existing object
                    'add to existing object
                    Dim loObject As DM_OBJECT = ObjectRepository.GetObject(TargetObjectID)
                    Select Case TargetFileType
                        Case 4
                            loObject.AddMessage(loMail, TargetPackId)
                        Case Else
                            loObject.AddMail(loMail, TargetPackId)
                    End Select

                End If

                loMail.RaiseInsertComplete(True, True)
                ArchiveOK = True

                'todo : 
                'can we block recipients in the handler?
                'if so we need to reload the recipients
            Else
                'unable to archive
                ShowError("Unable to archive")
            End If
            'End If
        Else
            ArchiveOK = True
        End If 'end archive check

        If ArchiveOK Then

            If SourceCaseTechID > 0 Then
                Template.PublishLinks(ListToString(lsToAccounts), SourceCase.TargetObject)
                Template.PublishLinks(ListToString(lsCCAccounts), SourceCase.TargetObject)
                Template.PublishLinks(ListToString(lsBccAccounts), SourceCase.TargetObject)
            ElseIf SourceObjectID > 0 Then
                Template.PublishLinks(ListToString(lsToAccounts), SourceObject)
                Template.PublishLinks(ListToString(lsCCAccounts), SourceObject)
                Template.PublishLinks(ListToString(lsBccAccounts), SourceObject)
            End If

            Dim loSmtp As ISmtpClient = Nothing

            If Not String.IsNullOrEmpty(Settings.GetValue("Mail", "Server")) Then
                loSmtp = Smtp.GetClient(Settings)
            End If
            If Archive Then
                If loSmtp IsNot Nothing Then
                    If SendNotification AndAlso lsNotifEmails.Any Then

                        Try
                            Using loNotifMessage As MailMessage = Template.CreateNotificationMessage(Settings, ListToString(lsNotifEmails), mailLink, Language, "[" & Template.Name & "] " & txtSubject.Text)
                                loSmtp.Send(loNotifMessage)
                            End Using
                        Catch ex As Exception
                            ShowError(ex)
                            ArchiveOK = False
                        End Try


                        lsToEmails = lsToEmails.Except(lsNotifEmails).ToList
                    End If

                    If ArchiveOK AndAlso lsToEmails.Any Then
                        'send the full message to the external emails and to the other mails if sendnotif is turned off                     
                        Try
                            Using loMessage As MailMessage = CreateMailMessage(ListToString(lsToEmails), ListToString(lsCcEmails), ListToString(lsBccEmails))
                                loSmtp.Send(loMessage)
                            End Using
                        Catch ex As Exception
                            ShowError(ex)
                            ArchiveOK = False
                        End Try
                    End If
                End If
            Else
                If loSmtp IsNot Nothing Then

                    Try
                        Using loMessage As MailMessage = CreateMailMessage(ListToString(lsToEmails), ListToString(lsCcEmails), ListToString(lsBccEmails))
                            loSmtp.Send(loMessage)
                        End Using
                    Catch ex As Exception
                        ShowError(ex)
                        ArchiveOK = False
                    End Try

                Else
                    ArchiveOK = False
                    ShowError("No Smtp Server configured")
                End If
            End If
        End If

        If ArchiveOK Then
            Dim allRecipients As List(Of String) = lsToAccounts.Union(lsCCAccounts).Union(lsBccAccounts).Union(lsToExternalEmails).Union(lsCCExternalEmails).Union(lsBCCExternalEmails).ToList

            If SourceCaseTechID > 0 Then
                'audit todo
            ElseIf SourceObjectID > 0 Then
                SourceObject.AddAudit("MAIL", "Sent " & Template.Name & " to " & ListToString(allRecipients, ", "), Template.ID, 0)
            ElseIf SourceSelection <> DMSelection.SelectionType.Undefined Then
                For Each loSel As DMSelectionList.SelectionInfo In Selection.GetSelection(SourceSelection)
                    If loSel.Object_ID > 0 Then
                        ObjectRepository.GetObject(loSel.Object_ID).AddAudit("MAIL", "Sent " & Template.Name & " to " & ListToString(allRecipients, ", "), Template.ID, 0)
                    End If
                Next
            End If
        End If

        'cleanup the temp files, even if the archiving fails
        For Each sTempFile As String In maTempFiles
            Arco.IO.File.DeleteFile(sTempFile)
        Next

        If ArchiveOK Then
            If Not String.IsNullOrEmpty(lsRedirectUrl) Then
                Response.Redirect(lsRedirectUrl)
            Else
                Page.ClientScript.RegisterStartupScript(Me.GetType, "closeme", "Close();", True)
            End If
        End If

    End Sub

    Private Function ListToString(ByVal list As IEnumerable(Of String)) As String
        Return ListToString(list, ";")
    End Function
    Private Function ListToString(ByVal list As IEnumerable(Of String), ByVal separator As String) As String
        Return String.Join(separator, list)
    End Function
    Private Sub ShowError(ByVal ex As Exception)
        Arco.Utils.Logging.LogError("Error sending e-mail", ex)
        ShowError("There was an error sending the e-mail")
    End Sub
    Private Sub ShowError(ByVal msg As String)
        lblError.Text = msg
        lblError.Visible = True
    End Sub

    Protected Sub cmdAddAtt_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdAddAtt.Click

        Dim uploadFileCollection As UploadedFileCollection = txtFileUpload.UploadedFiles

        If uploadFileCollection.Count <> 0 Then
            For Each file As UploadedFile In uploadFileCollection
                'Dim file As HttpPostedFile = f.
                If Not String.IsNullOrEmpty(file.FileName) AndAlso file.ContentLength > 0 Then
                    Dim fileStr As String = Settings.GetUploadPath & Arco.Utils.GUID.CreateGUID & "." & Arco.IO.File.GetExtension(file.FileName)

                    file.SaveAs(fileStr)

                    Dim itm As System.Web.UI.WebControls.ListItem = New System.Web.UI.WebControls.ListItem(Arco.IO.File.GetFileNameWithExtension(file.FileName), "ATT" & txtFileUpload.UniqueID & ";" & fileStr) With {.Selected = True}
                    chkListAtt.Items.Add(itm)
                End If
            Next

        End If

    End Sub

    Protected Sub imgHyperlink_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles imgHyperlink.Click

        Dim llFileID As Int32
        Dim lsURL As String

        If cmbHyperlinks.SelectedIndex >= 0 Then
            llFileID = Convert.ToInt32(cmbHyperlinks.Items(cmbHyperlinks.SelectedIndex).Value)

            Try
                Dim loFile As File = File.GetFile(llFileID)
                If loFile.IsRemoteUrl = False Then
                    lsURL = loFile.GetUrl(False)
                Else
                    lsURL = loFile.FILE_PATH
                End If
                RadEditor1.Content &= " <A HREF=" & Chr(34) & lsURL & Chr(34) & ">" & Chr(34) & Server.HtmlEncode(loFile.Name) & Chr(34) & "</A>"
            Catch ex As Exception
            End Try

        End If

    End Sub

    Private Sub BuildCase(ByVal voCase As cCase, ByVal vbAllowAttachment As Boolean)
        If voCase.TargetObject.ID > 0 Then
            BuildDoc(voCase.TargetObject, vbAllowAttachment)
        End If
    End Sub

    Private Sub BuildDoc(ByVal vlObjectID As Int32,
                         ByVal vbAllowAttachment As Boolean)
        BuildDoc(ObjectRepository.GetObject(vlObjectID), vbAllowAttachment)
    End Sub

    Private Sub BuildDoc(ByVal vlObject As DM_OBJECT,
                         ByVal allowAttachment As Boolean)
        Try

            If vlObject.Object_Type = "Shortcut" Then
                vlObject = CType(vlObject, Shortcut).GetReferencedObject
            End If
            If vlObject.ID > 0 Then
                If vlObject.CanViewFiles Then
                    Dim lsURL As String = vlObject.GetUrl(False)
                    cmbHyperlinksDocroom.Items.Add(vlObject.Name)
                    cmbHyperlinksDocroom.Items(cmbHyperlinksDocroom.Items.Count - 1).Value = vlObject.ID.ToString
                    RadEditor1.Links.Add(New EditorLink(vlObject.Name, lsURL))
                    mlCnt += 1
                    If vlObject.FileCount > 0 Then
                        If TemplateID > 0 Then
                            Dim lsatts As String = ShowAttachmentsToEmail(vlObject)
                            If Not String.IsNullOrEmpty(lsatts) Then
                                If Not String.IsNullOrEmpty(pnlFixedAttachments.Text) Then
                                    pnlFixedAttachments.Text &= ", "
                                End If
                                pnlFixedAttachments.Text &= lsatts
                                lblAtt.Visible = True
                            End If
                        End If

                        For Each loFileInfo As FileList.FileInfo In vlObject.Files
                            Dim loFile1 As File = File.GetFile(loFileInfo.ID)
                            If (loFile1.PackageID = 0 OrElse vlObject.CanViewPackage(loFile1.PackageID)) Then
                                If allowAttachment AndAlso
                                    Not loFile1.IsRemoteUrl Then

                                    Dim add As Boolean = True
                                    Dim addSelected As Boolean = True
                                    If TemplateID <> 0 AndAlso
                                        Template.AttachmentList.Cast(Of NotificationAttachmentList.AttachmentInfo).Any(Function(x) x.SendUsing = NotificationAttachment.SendMethod.Attach) Then
                                        'we have attachments configured
                                        'set the linked attachments to not selected
                                        addSelected = False
                                        For Each att As NotificationAttachmentList.AttachmentInfo In Template.AttachmentList.Cast(Of NotificationAttachmentList.AttachmentInfo).Where(Function(x) x.SendUsing = NotificationAttachment.SendMethod.Attach)
                                            Select Case att.ItemType
                                                Case NotificationAttachment.eItemType.AllFiles
                                                    add = False
                                                Case NotificationAttachment.eItemType.MainFile
                                                    If Not loFile1.isAttachment Then
                                                        add = False
                                                    End If
                                            End Select
                                        Next
                                    End If

                                    If add Then
                                        chkListAtt.Items.Add(Server.HtmlEncode(loFile1.GetDownloadFileName()))
                                        chkListAtt.Items(chkListAtt.Items.Count - 1).Value = loFile1.ID.ToString

                                        chkListAtt.Items(chkListAtt.Items.Count - 1).Selected = addSelected
                                    End If
                                End If

                                'add the file as url option (?)
                                lsURL = loFile1.GetUrl(True)
                                Dim loLink As New EditorLink
                                loLink.Name = Arco.IO.File.TrimIllegalChars(loFile1.Name) & "." & loFile1.FILE_EXT
                                loLink.Href = lsURL
                                RadEditor1.Links(mlCnt - 1).ChildLinks.Add(loLink)
                                cmbHyperlinks.Items.Add(loFile1.Name & "." & loFile1.FILE_EXT)
                                cmbHyperlinks.Items(cmbHyperlinks.Items.Count - 1).Value = loFile1.ID.ToString
                            End If
                        Next
                    End If
                End If
            End If
        Catch ex As Exception
            ShowError(ex.ToString)
        End Try

    End Sub

    Private Sub InitParams()

        SourceObjectID = QueryStringParser.GetInt("DM_OBJECT_ID")
        SourceCaseTechID = QueryStringParser.GetInt("RTCASE_TECH_ID")
        TargetObjectID = QueryStringParser.GetInt("DM_ADD_TO_OBJECT_ID")
        If TargetObjectID > 0 Then
            TargetFileType = QueryStringParser.GetInt("DM_ADD_TO_OBJECT_FILE_TYPE")
            TargetPackId = QueryStringParser.GetInt("DM_ADD_TO_PACK_ID")
        End If

        TemplateID = QueryStringParser.GetInt("TEMPLATE_ID")

        If TemplateID = 0 Then
            Dim defaultTemplateId As String = Settings.GetValue("Mail", "DefaultSendTemplate", "")
            If Not String.IsNullOrEmpty(defaultTemplateId) Then
                Dim parsedId As Integer
                If Integer.TryParse(defaultTemplateId, parsedId) Then
                    TemplateID = parsedId
                Else
                    moTemplate = NotificationTemplate.GetNotificationTemplateByName(defaultTemplateId)
                    If moTemplate IsNot Nothing Then
                        TemplateID = moTemplate.ID
                    End If
                End If
            End If
        End If

        SourceMailID = QueryStringParser.GetInt("DM_MAIL_ID")

        If SourceMailID <> 0 Then
            MailAction = QueryStringParser.GetString("ACTION").ToUpper
        Else
            MailAction = ""
        End If
        If QueryStringParser.Exists("mailsel") Then
            SourceSelection = CType(QueryStringParser.GetString("mailsel"), DMSelection.SelectionType)
        End If
    End Sub

    Private Sub BuildDocMessage()
        Dim lbAllowAttachment As Boolean = AllowAttachments()

        pnlFixedAttachments.Text = ""

        If SourceObjectID > 0 Then
            BuildDoc(SourceObject, lbAllowAttachment)
        ElseIf SourceCaseTechID > 0 Then
            BuildCase(SourceCase, lbAllowAttachment)
        ElseIf SourceSelection <> DMSelection.SelectionType.Undefined Then
            For Each loSel As DMSelectionList.SelectionInfo In Selection.GetSelection(SourceSelection)
                If loSel.Object_ID > 0 Then
                    BuildDoc(loSel.Object_ID, lbAllowAttachment)
                End If
            Next
        End If

        If cmbHyperlinksDocroom.Items.Count = 0 Then
            lblHyperlinkDocroom.Visible = False
            cmbHyperlinksDocroom.Visible = False
            imgHyperlinkDocroom.Visible = False
        Else
            lblHyperlinkDocroom.Visible = True
            cmbHyperlinksDocroom.Visible = True
            imgHyperlinkDocroom.Visible = True
        End If

        If cmbHyperlinks.Items.Count = 0 Then
            lblHyperlinkFiles.Visible = False
            cmbHyperlinks.Visible = False
            imgHyperlink.Visible = False
        Else
            lblHyperlinkFiles.Visible = True
            cmbHyperlinks.Visible = True
            imgHyperlink.Visible = True
        End If

        If Not lbAllowAttachment Then
            trATT.Visible = False

            chkListAtt.Visible = False
            cmdAddAtt.Visible = False


        End If
    End Sub

    Private Function GetSelectedAtt() As String
        Dim lsSelected As String = ""
        For i = 0 To chkListAtt.Items.Count - 1
            If chkListAtt.Items(i).Selected Then
                lsSelected &= "|" & chkListAtt.Items(i).Value
                If chkListAtt.Items(i).Value.StartsWith("ATT") Then
                    ' File from PC
                    lsSelected &= "=" & chkListAtt.Items(i).Text
                End If
            End If
        Next
        If lsSelected.Length > 0 Then lsSelected = lsSelected.Substring(1)

        Return lsSelected
    End Function

    Private Function AllowAttachments() As Boolean
        If TemplateID > 0 Then
            Return Template.AllowAttachments
        Else
            Return Settings.GetValue("Mail", "AllowAttachments", True)
        End If
    End Function

    Private maTempFiles As List(Of String)


    Private Function GetFullMailBody() As String
        Dim lsBody As String = RadEditor1.Content
        If Not String.IsNullOrEmpty(lblTemplate.Text) Then
            If lsBody.Length > 0 Then
                lsBody &= "<br />"
            End If
            lsBody &= lblTemplate.Text
        End If
        Return HtmlSanitizer.Sanitize(lsBody)
    End Function

    Private fromEmail As String
    Private fromDisplayName As String
    Private replyTo As String
    Private replyToDisplay As String

    Private Sub SetFromAddress()
        fromEmail = Settings.GetValue("Mail", "DefaultSender")
        fromDisplayName = Settings.GetValue("Mail", "DefaultDisplayFrom")


        If SourceCaseTechID > 0 Then
            fromEmail = Template.GetFromMailToUse(SourceCase, fromEmail)
            fromDisplayName = Template.GetFromDisplayToUse(SourceCase, fromDisplayName)
            replyTo = Template.GetReplyToToUse(SourceCase)
        ElseIf SourceObjectID > 0 Then
            fromEmail = Template.GetFromMailToUse(SourceObject, fromEmail)
            fromDisplayName = Template.GetFromDisplayToUse(SourceObject, fromDisplayName)
            replyTo = Template.GetReplyToToUse(SourceObject)
        Else
            fromEmail = Template.GetFromMailToUse(Nothing, fromEmail)
            fromDisplayName = Template.GetFromDisplayToUse(Nothing, fromDisplayName)
            replyTo = Template.GetReplyToToUse(Nothing)
        End If

        If String.IsNullOrEmpty(replyTo) Then
            replyTo = Security.BusinessIdentity.CurrentIdentity.Email
            replyToDisplay = Security.BusinessIdentity.CurrentIdentity.DisplayName
        Else
            replyToDisplay = Nothing
        End If


        If String.IsNullOrEmpty(fromDisplayName) Then
            lblFrom.Text = fromEmail
        Else
            lblFrom.Text = fromDisplayName & " <" & fromEmail & ">"
        End If

        If Not String.IsNullOrEmpty(replyTo) AndAlso Not replyTo.Equals(fromEmail, StringComparison.CurrentCultureIgnoreCase) Then
            lblFrom.Text &= " (" & replyTo & ")"
        End If

    End Sub

    Private Function CreateMailMessage(ByVal vsToEmails As String,
                                ByVal vsCcEmails As String,
                                ByVal vsBccEmails As String) As MailMessage



        Dim msg As MailMessage


        msg = Smtp.NewMessage(fromEmail, fromDisplayName)
        For Each replyToAddress As String In replyTo.Split(New String() {";", vbCrLf}, StringSplitOptions.RemoveEmptyEntries)

            msg.ReplyToList.Add(New MailAddress(replyToAddress, replyToDisplay))
        Next
        msg.IsBodyHtml = True
        msg.Body = GetFullMailBody()

        ' --- To
        For Each toAddress As String In vsToEmails.Split(New String() {";", vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            msg.To.Add(New MailAddress(toAddress))
        Next

        ' --- Cc
        For Each ccAddress As String In vsCcEmails.Split(New String() {";", vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            msg.CC.Add(New MailAddress(ccAddress))
        Next

        ' --- Bcc
        For Each bccAddress As String In vsBccEmails.Split(New String() {";", vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            msg.Bcc.Add(New MailAddress(bccAddress))
        Next

        AddAttachmentsToEmail(msg)

        ' --- Subject
        msg.Subject = txtSubject.Text

        Return msg

    End Function

    Private Sub AddAttachmentsToDMMail(ByVal voMail As DMMail)

        Dim lsTemp As String
        Dim i As Int32
        Dim lsTempPath As String = Settings.GetUploadPath
        Dim lsAtt As String = GetSelectedAtt()
        If lsAtt.Length > 0 Then
            Dim lsaAtt As String() = Split(lsAtt, "|")
            If lsaAtt.Length > 0 Then
                For i = 0 To lsaAtt.Length - 1
                    lsTemp = lsaAtt(i).Trim
                    If lsTemp.StartsWith("ATT") Then
                        ' File from PC
                        lsaAtt(i) = lsTemp.Substring(lsTemp.IndexOf("=") + 1)
                        Dim lsFile As String = lsTemp.Substring(lsTemp.IndexOf(";") + 1)
                        lsFile = lsFile.Substring(0, lsFile.IndexOf("="))

                        voMail.AddAttachment(System.IO.Path.GetFileName(lsaAtt(i)), lsFile, False)
                        maTempFiles.Add(lsFile)
                    ElseIf lsTemp.StartsWith("M") Then
                        'mail file
                        Dim loMailFile As MailFile = MailFile.GetFile(Convert.ToInt32(lsaAtt(i).Substring(1).Trim), voMail.ID)
                        If loMailFile IsNot Nothing Then
                            voMail.LinkMailFile(loMailFile.ID)
                        End If
                    Else
                        ' Doma file
                        Dim loFile As File = File.GetFile(Convert.ToInt32(lsaAtt(i).Trim))
                        If loFile.ID > 0 Then
                            Dim lsFile As String
                            If loFile.Type = File.File_Type.File Then
                                lsFile = loFile.SaveToDisk(lsTempPath)
                                maTempFiles.Add(lsFile)
                                voMail.AddAttachment(loFile.Name, lsFile, False)
                            ElseIf loFile.isMail Then
                                For Each loMailFileinfo As MailFileList.FileInfo In MailFileList.GetFileList(loFile.ReferenceID)
                                    voMail.LinkMailFile(loMailFileinfo.ID)
                                Next loMailFileinfo
                            End If
                        End If
                    End If
                Next
            End If
        End If
        If Template.ID > 0 Then
            If SourceCaseTechID > 0 Then
                AddTemplateAttachmentsToDMMail(voMail, SourceCase.TargetObject)
            ElseIf SourceObjectID > 0 Then
                AddTemplateAttachmentsToDMMail(voMail, SourceObject)
            ElseIf SourceSelection <> DMSelection.SelectionType.Undefined Then
                For Each loSel As DMSelectionList.SelectionInfo In Selection.GetSelection(Me.SourceSelection)
                    If loSel.Object_ID > 0 Then
                        AddTemplateAttachmentsToDMMail(voMail, loSel.GetBusinessObject)
                    End If
                Next
            End If
        End If

    End Sub

    Private Sub AddTemplateAttachmentsToDMMail(ByVal voMail As DMMail, ByVal obj As DM_OBJECT)
        'attachments from template

        Dim tempPath As String = Settings.GetUploadPath
        For Each attachment As NotificationAttachmentList.AttachmentInfo In Template.AttachmentList
            If attachment.SendUsing = NotificationAttachment.SendMethod.Attach Then
                Select Case attachment.ItemType
                    Case NotificationAttachment.eItemType.AllFiles, NotificationAttachment.eItemType.MainFile
                        For Each fileInfo As FileList.FileInfo In obj.Files
                            If Not fileInfo.isAttachment OrElse attachment.ItemType = NotificationAttachment.eItemType.AllFiles Then
                                If fileInfo.PackageID = 0 OrElse obj.CanViewPackage(fileInfo.PackageID) Then
                                    Dim file As File = File.GetFile(fileInfo.ID)
                                    If file.ID > 0 Then
                                        Dim fileStr As String
                                        If file.Type = File.File_Type.File Then
                                            fileStr = file.SaveToDisk(tempPath, attachment.Rendition)
                                            If String.IsNullOrEmpty(fileStr) AndAlso Not String.IsNullOrEmpty(attachment.Rendition) Then
                                                fileStr = file.SaveToDisk(tempPath)
                                            End If
                                            maTempFiles.Add(fileStr)
                                            voMail.AddAttachment(file.Name, fileStr, False)
                                        ElseIf file.isMail Then
                                            For Each loMailFileinfo As MailFileList.FileInfo In MailFileList.GetFileList(file.ReferenceID)
                                                voMail.LinkMailFile(loMailFileinfo.ID)
                                            Next loMailFileinfo
                                        End If
                                    End If
                                End If
                            End If
                        Next

                End Select

            End If
        Next

    End Sub

    Private Sub AddAttachmentsToEmail(ByVal voMessage As MailMessage)
        Dim lsZipFile As String = ""
        Dim lsTempPath As String = Settings.GetUploadPath
        Dim lsaFiles As String() : ReDim lsaFiles(0)
        Dim lsaFilesDesc As String() : ReDim lsaFilesDesc(0)
        Dim i As Int32
        Dim iOffset As Int32 = 0
        Dim lsTemp As String
        Dim lsAtt As String = GetSelectedAtt()
        If lsAtt.Length > 0 Then
            Dim lsaAtt As String() = Split(lsAtt, "|")
            If lsaAtt.Length > 0 Then
                Dim s As System.IO.Stream

                If chkZip.Checked Then
                    ReDim lsaFiles(0 To lsaAtt.Length - 1)
                    ReDim lsaFilesDesc(0 To lsaAtt.Length - 1)
                    i = 0
                    Do
                        i = i + 1
                        lsZipFile = lsTempPath & i.ToString & ".zip"
                    Loop While Arco.IO.File.Exists(lsZipFile)
                End If

                For i = 0 To lsaAtt.Length - 1
                    lsTemp = lsaAtt(i).Trim
                    If lsTemp.StartsWith("ATT") Then
                        ' File from PC
                        lsaAtt(i) = lsTemp.Substring(lsTemp.IndexOf("=") + 1)
                        Dim lsFile As String = lsTemp.Substring(lsTemp.IndexOf(";") + 1)
                        lsFile = lsFile.Substring(0, lsFile.IndexOf("="))
                        If chkZip.Checked Then
                            lsaFiles(i + iOffset) = lsFile
                            lsaFilesDesc(i + iOffset) = System.IO.Path.GetFileName(lsaAtt(i))
                        Else
                            s = Arco.IO.File.GetFileStream(lsFile)
                            voMessage.Attachments.Add(New Attachment(s, System.IO.Path.GetFileName(lsaAtt(i))))
                        End If
                        maTempFiles.Add(lsFile)
                    ElseIf lsTemp.StartsWith("M") Then
                        'mail file
                        Dim loMailFile As MailFile = MailFile.GetFile(Convert.ToInt32(lsaAtt(i).Substring(1).Trim), 0)
                        If loMailFile IsNot Nothing Then
                            If loMailFile.isAttachment Then
                                If chkZip.Checked Then
                                    Dim lsFile As String = loMailFile.SaveToDisk(lsTempPath)
                                    lsaFiles(i + iOffset) = lsFile
                                    lsaFilesDesc(i + iOffset) = loMailFile.GetDownloadFileName() 'Arco.Doma.FileManager.File.TrimIllegalChars(loMailFile.FILE_TITLE) & "." & loMailFile.FILE_EXT                                
                                Else
                                    voMessage.Attachments.Add(New Attachment(loMailFile.GetStream, loMailFile.GetDownloadFileName()))
                                End If

                            End If
                        End If
                    Else
                        ' Doma file
                        Dim loFile As File = File.GetFile(Convert.ToInt32(lsaAtt(i).Trim))

                        If loFile.ID > 0 Then
                            If loFile.Type = Arco.Doma.Library.File.File_Type.File Then
                                If chkZip.Checked Then
                                    Dim lsTempFile As String = loFile.SaveToDisk(lsTempPath)
                                    lsaFiles(i + iOffset) = lsTempFile
                                    lsaFilesDesc(i + iOffset) = loFile.GetDownloadFileName()  'Arco.Doma.FileManager.File.TrimIllegalChars(loFileList(0).FILE_TITLE) & "." & loFileList(0).FILE_EXT                                
                                Else
                                    voMessage.Attachments.Add(New Attachment(loFile.GetStream, loFile.GetDownloadFileName()))
                                End If
                            ElseIf loFile.isMail Then
                                For Each loMailFileinfo As MailFileList.FileInfo In MailFileList.GetFileList(loFile.ReferenceID)
                                    Dim loMailFile As MailFile = MailFile.GetFile(loMailFileinfo.ID, loFile.ReferenceID)
                                    If Not loMailFile.isAttachment Then
                                        Dim loDMMail As DMMail = DMMail.GetMail(loFile.ReferenceID)
                                        Dim lsTempFile As String = lsTempPath & loMailFile.ID.ToString & ".htm"
                                        lsTempFile = loDMMail.SaveCompleteMailAsHtml(lsTempFile)
                                        If chkZip.Checked Then
                                            lsaFiles(i + iOffset) = lsTempFile
                                            lsaFilesDesc(i + iOffset) = Arco.IO.FileName.CreateFileName(loDMMail.Subject, loMailFile.FILE_EXT) 'Arco.Doma.FileManager.File.TrimIllegalChars(loDMMail.Subject) & "." & loMailFile.FILE_EXT
                                            iOffset = iOffset + 1
                                            ReDim Preserve lsaFiles(i + iOffset)
                                            ReDim Preserve lsaFilesDesc(i + iOffset)
                                        Else
                                            s = Arco.IO.File.GetFileStream(lsTempFile)
                                            voMessage.Attachments.Add(New Attachment(s, Arco.IO.FileName.CreateFileName(loDMMail.Subject, loMailFile.FILE_EXT)))
                                        End If
                                        maTempFiles.Add(lsTempFile)
                                    Else
                                        If chkZip.Checked Then
                                            Dim lsTempFile As String = loMailFile.SaveToDisk(lsTempPath)


                                            lsaFiles(i + iOffset) = lsTempFile
                                            lsaFilesDesc(i + iOffset) = loMailFile.GetDownloadFileName() 'Arco.Doma.FileManager.File.TrimIllegalChars(loMailFile.FILE_TITLE) & "." & loMailFile.FILE_EXT
                                            iOffset = iOffset + 1
                                            ReDim Preserve lsaFiles(i + iOffset)
                                            ReDim Preserve lsaFilesDesc(i + iOffset)
                                        Else
                                            voMessage.Attachments.Add(New Attachment(loMailFile.GetStream, loMailFile.GetDownloadFileName))
                                        End If

                                    End If
                                Next ' loMailFileinfo
                            End If
                        End If
                    End If
                Next

                If chkZip.Checked Then
                    Try
                        Arco.Utils.ZIP.Zip(lsaFiles, lsZipFile, lsaFilesDesc)
                        s = Arco.IO.File.GetFileStream(lsZipFile)
                        voMessage.Attachments.Add(New Attachment(s, "Attachments.zip"))
                        maTempFiles.Add(lsZipFile)
                    Catch ex As Exception
                    End Try
                End If

            End If
        End If

        'don't zip them for now
        If Template.ID > 0 Then
            If SourceCaseTechID > 0 Then
                AddTemplateAttachmentsToEmail(voMessage, SourceCase.TargetObject)
            ElseIf SourceObjectID > 0 Then
                AddTemplateAttachmentsToEmail(voMessage, SourceObject)
            ElseIf SourceSelection <> DMSelection.SelectionType.Undefined Then
                For Each loSel As DMSelectionList.SelectionInfo In Selection.GetSelection(Me.SourceSelection)
                    If loSel.Object_ID > 0 Then
                        AddTemplateAttachmentsToEmail(voMessage, loSel.GetBusinessObject)
                    End If
                Next
            End If
        End If

    End Sub

    Private Function ShowAttachmentsToEmail(ByVal voObject As DM_OBJECT) As String
        Dim lsAtts As String = ""
        For Each loAtt As NotificationAttachmentList.AttachmentInfo In Template.AttachmentList
            If loAtt.SendUsing = NotificationAttachment.SendMethod.Attach Then
                Select Case loAtt.ItemType
                    Case NotificationAttachment.eItemType.AllFiles, NotificationAttachment.eItemType.MainFile
                        For Each loFileInfo As FileList.FileInfo In voObject.Files
                            If Not loFileInfo.isAttachment OrElse loAtt.ItemType = NotificationAttachment.eItemType.AllFiles Then
                                If loFileInfo.PackageID = 0 OrElse voObject.CanViewPackage(loFileInfo.PackageID) Then
                                    If Not String.IsNullOrEmpty(lsAtts) Then
                                        lsAtts &= ", "
                                    End If
                                    lsAtts &= loFileInfo.GetDownloadFileName()
                                End If
                            End If
                        Next
                End Select
            End If
        Next
        Return lsAtts
    End Function

    Private Sub AddTemplateAttachmentsToEmail(ByVal voMessage As MailMessage, ByVal voObject As DM_OBJECT)
        'attachments from template
        For Each loAtt As NotificationAttachmentList.AttachmentInfo In Template.AttachmentList
            If loAtt.SendUsing = NotificationAttachment.SendMethod.Attach Then
                Select Case loAtt.ItemType
                    Case NotificationAttachment.eItemType.AllFiles, NotificationAttachment.eItemType.MainFile
                        Dim lsTempPath As String = Settings.GetUploadPath

                        For Each loFileInfo As FileList.FileInfo In voObject.Files
                            If Not loFileInfo.isAttachment OrElse loAtt.ItemType = NotificationAttachment.eItemType.AllFiles Then
                                Dim loFile As File = File.GetFile(loFileInfo.ID)
                                If loFile.ID > 0 Then
                                    If loFile.PackageID = 0 OrElse voObject.CanViewPackage(loFile.PackageID) Then
                                        If loFile.Type = File.File_Type.File Then
                                            Dim lsExt As String = loFile.GetDownloadExtension(loAtt.Rendition)
                                            Dim s As System.IO.Stream = loFile.GetStream(loAtt.Rendition)
                                            If s Is Nothing AndAlso Not String.IsNullOrEmpty(loAtt.Rendition) Then
                                                lsExt = loFile.FILE_EXT
                                                s = loFile.GetStream
                                            End If
                                            voMessage.Attachments.Add(New Attachment(s, loFile.GetDownloadFileName(lsExt)))
                                        ElseIf loFile.isMail Then
                                            For Each loMailFileinfo As MailFileList.FileInfo In MailFileList.GetFileList(loFile.ReferenceID)
                                                Dim loMailFile As MailFile = MailFile.GetFile(loMailFileinfo.ID, loFile.ReferenceID)
                                                If Not loMailFile.isAttachment Then
                                                    Dim loDMMail As DMMail = DMMail.GetMail(loFile.ReferenceID)
                                                    Dim lsTempFile As String = lsTempPath & loMailFile.ID.ToString & ".htm"
                                                    lsTempFile = loDMMail.SaveCompleteMailAsHtml(lsTempFile)


                                                    voMessage.Attachments.Add(New Attachment(Arco.IO.File.GetFileStream(lsTempFile), Arco.IO.FileName.CreateFileName(loDMMail.Subject, loMailFile.FILE_EXT)))

                                                    maTempFiles.Add(lsTempFile)
                                                Else
                                                    voMessage.Attachments.Add(New Attachment(loMailFile.GetStream, loMailFile.GetDownloadFileName()))
                                                End If
                                            Next loMailFileinfo
                                        End If
                                    End If
                                End If
                            End If
                        Next

                End Select
            End If
        Next

    End Sub

    Private Function CheckRecipient(ByVal voTextBox As RadAutoCompleteBox,
                                      ByVal allEmailList As List(Of String), ByVal accountList As List(Of String),
                                    ByVal allExternalEmailList As List(Of String),
                                    ByVal notificationEmails As List(Of String), ByVal canAddDelegates As Boolean, ByVal canBeExternal As Boolean, ByVal canBeInternal As Boolean) As Boolean

        For Each entry As AutoCompleteBoxEntry In voTextBox.Entries
            Dim lsValue As String = entry.Value
            If String.IsNullOrEmpty(lsValue) Then lsValue = entry.Text
            If Not String.IsNullOrEmpty(lsValue) Then
                For Each lsEntry As String In lsValue.Split(New String() {";", vbCrLf}, StringSplitOptions.RemoveEmptyEntries).Select(Function(x) x.Trim)
                    If IsEmail(lsEntry) Then
                        '  If (Track AndAlso voTextBox.ID = "txtTo") Then
                        If Not canBeExternal Then
                            ShowError(GetDecodedLabel("externalmailnotallowed") & ": " & Server.HtmlEncode(lsEntry))
                            Return False
                        End If
                        AddEmailToPersonalAddressBook(lsEntry, lsEntry)
                        AddToRecipientList(allEmailList, lsEntry)
                        AddToRecipientList(allExternalEmailList, lsEntry)
                    Else
                        If canBeInternal Then
                            If lsEntry.StartsWith("(Role) ") Then
                                Dim loRole As Role = Role.GetRole(lsEntry.Substring(7))
                                If loRole Is Nothing Then
                                    ShowError(GetLabel("noresultsfound") & ": " & Server.HtmlEncode(lsEntry))
                                    Return False
                                End If
                                AddRolToRecipientsList(loRole.ID, allEmailList, accountList, notificationEmails, canAddDelegates)

                            Else
                                If Not AddUserToRecipientList(lsEntry, allEmailList, accountList, notificationEmails, False, canAddDelegates) Then
                                    Return False
                                End If

                            End If
                        Else
                            ShowError(Server.HtmlEncode(lsEntry) & " " & GetLabel("namail"))
                            Return False
                        End If

                    End If
                Next

            End If
        Next

        Return True

    End Function

    Private Sub AddRolToRecipientsList(ByVal vlRoleID As Int32, ByVal allEmailList As List(Of String), ByVal accountList As List(Of String), ByVal notificationEmails As List(Of String), ByVal canAddDelegates As Boolean)
        For Each loRoleMemberInfo As RoleMemberList.RoleMemberInfo In RoleMemberList.GetRoleMemberList(vlRoleID, True)
            If loRoleMemberInfo.MEMBERTYPE = "User" Then
                AddUserToRecipientList(loRoleMemberInfo.MEMBER, allEmailList, accountList, notificationEmails, True, canAddDelegates)
            End If
        Next
    End Sub

    Private Function AddUserToRecipientList(ByVal vsAccount As String, ByVal allEmailList As List(Of String), ByVal accountList As List(Of String), ByVal notificationEmails As List(Of String), ByVal vbSkipErrors As Boolean, ByVal canAddDelegates As Boolean) As Boolean
        If accountList.Contains(vsAccount) Then Return True

        Dim lsCheckUser As User = ACL.User.GetUser(vsAccount)
        If lsCheckUser IsNot Nothing AndAlso lsCheckUser.MatchesTenant Then

            AddToRecipientList(allEmailList, lsCheckUser.USER_MAIL)
            AddToRecipientList(accountList, lsCheckUser.USER_LOGIN)
            If canAddDelegates AndAlso IncludeDelegates Then
                For Each loDel As WorkExceptionList.WorkExceptionInfo In WorkExceptionList.GetDelegatedFrom(lsCheckUser.USER_LOGIN, "User", True)
                    If loDel.Subject_Type = "User" Then
                        AddUserToRecipientList(loDel.Subject_ID, allEmailList, accountList, notificationEmails, vbSkipErrors, False)
                    End If
                Next
            End If
            If Not String.IsNullOrEmpty(lsCheckUser.USER_MAIL) Then
                AddToRecipientList(notificationEmails, lsCheckUser.USER_MAIL)
            ElseIf Not SendNotification AndAlso Not Archive AndAlso Not vbSkipErrors Then
                ShowError("No email address found for user: " & vsAccount)
                Return False
            End If
        ElseIf Not vbSkipErrors Then
            ShowError(vsAccount & " " & GetLabel("navalidvalue"))
            Return False
        End If

        Return True

    End Function

    Private Sub AddToRecipientList(ByVal toList As List(Of String), ByVal value As String)
        If toList IsNot Nothing AndAlso Not String.IsNullOrEmpty(value) AndAlso Not toList.Contains(value) Then
            toList.Add(value)
        End If

    End Sub

    Private Sub AddEmailToPersonalAddressBook(ByVal vsName As String, ByVal vsEmail As String)

        If String.IsNullOrWhiteSpace(vsEmail) Then
            Exit Sub
        End If
        If vsName.Length = 0 Then vsName = vsEmail
        Try
            Dim crit As New DM_MailingUser.Criteria(vsName) With {
                .MailSynch = DM_MailingUser.Mail_Sync.NotSynchronized,
                .MailEmail = vsEmail,
                .MailLoginName = Security.BusinessIdentity.CurrentIdentity.Name
            }
            Dim loUser As DM_MailingUser = DM_MailingUser.GetMailingUser(crit)
            If loUser.ID = 0 Then
                loUser = DM_MailingUser.NewMailingUser(vsName)
                loUser.MailEmail = vsEmail
                loUser.MailSync = 0
                loUser.MailLoginName = Security.BusinessIdentity.CurrentIdentity.Name
                loUser.Save()
            End If

        Catch ex As Exception
            Response.Write(ex.Message)
        End Try

    End Sub
    Private Function IsEmail(ByVal vsEmail As String) As Boolean
        Return Not vsEmail.Contains("\") AndAlso vsEmail.Contains("@") AndAlso vsEmail.Contains(".")
    End Function

    Private Function CreateTop(ByVal vsFrom As String,
                               ByVal vsDate As DateTime,
                               ByVal vsTo As String,
                               ByVal vsCc As String,
                               ByVal vsSubject As String,
                               ByVal vsText As String, ByVal voOrigAtts As MailFileList) As String



        Dim lsTemp As String = "<BR/><BR/><BR/><hr/>"

        If Not String.IsNullOrEmpty(vsFrom) Then lsTemp &= "<BR/><b>" & GetLabel("mail_from") & " </b>" & ArcoFormatting.FormatEmails(vsFrom)
        If Not String.IsNullOrEmpty(vsDate) Then lsTemp &= "<BR/><b>" & GetLabel("date") & ": </b>" & ArcoFormatting.FormatDateTime(vsDate, False)
        If Not String.IsNullOrEmpty(vsTo) Then lsTemp &= "<BR/><b>" & GetLabel("mail_to") & ": </b>" & ArcoFormatting.FormatEmails(vsTo)
        If Not String.IsNullOrEmpty(vsCc) Then lsTemp &= "<BR/><b>" & GetLabel("mail_cc") & ": </b>" & ArcoFormatting.FormatEmails(vsCc)
        If Not String.IsNullOrEmpty(vsSubject) Then lsTemp &= "<BR/><b>" & GetLabel("mail_subject") & " </b>" & Server.HtmlEncode(vsSubject)
        lsTemp &= "<BR/><BR/>" & RenderBodyWithAttachmentsInline(vsText, voOrigAtts)

        Return lsTemp

    End Function
    Private Function RenderBodyWithAttachmentsInline(ByVal vsBody As String, ByVal voAtts As MailFileList) As String
        Dim iPos As Int32
        Dim iPosEnd As Int32
        Dim dict As Dictionary(Of String, String) = New Dictionary(Of String, String)
        iPos = vsBody.IndexOf("src=" & Chr(34) & "cid:")
        If iPos > 0 Then
            Do
                iPosEnd = vsBody.IndexOf(Chr(34), iPos + 7)
                If iPosEnd > 0 Then
                    Dim lsReplace As String = vsBody.Substring(iPos, iPosEnd - iPos + 1)
                    If Not dict.ContainsKey(lsReplace) Then
                        Dim lsName As String = lsReplace.Substring(lsReplace.IndexOf("cid:") + 4)
                        If Not String.IsNullOrEmpty(lsName) Then
                            Dim iAt As Int32 = lsName.IndexOf("@")
                            If iAt > 0 Then
                                lsName = lsName.Substring(0, iAt)
                                For Each loAt As MailFileList.FileInfo In voAtts
                                    If loAt.Name = lsName Then
                                        Dim lsReplaced As String = "src=" & Chr(34) & "../DM_VIEW_FILE.aspx?FILE_ID=" & loAt.ID & "&frommail=Y" & Chr(34)
                                        dict.Add(lsReplace, lsReplaced)
                                    End If
                                Next

                            End If

                        End If
                    End If

                End If

                iPos = vsBody.IndexOf("src=" & Chr(34) & "cid:", iPos + 1)
            Loop While iPos > 0

            For Each d As KeyValuePair(Of String, String) In dict
                vsBody = vsBody.Replace(d.Key, d.Value)
            Next
        End If

        Return HtmlSanitizer.Sanitize(vsBody)

    End Function
    Private moTemplate As NotificationTemplate
    Protected ReadOnly Property Template As NotificationTemplate
        Get
            If moTemplate Is Nothing Then
                If TemplateID <> 0 Then
                    moTemplate = NotificationTemplate.GetNotificationTemplate(TemplateID)
                Else
                    moTemplate = NotificationTemplate.GetDefaultEmailTemplate()

                    If TargetObjectID <> 0 Then
                        moTemplate.Archive = True
                        If TargetFileType = 4 Then
                            moTemplate.Track = True
                        End If

                    End If

                End If
            End If
            Return moTemplate
        End Get
    End Property
    Private Sub CreateForm()

        Dim lsFrom As String = ""
        Dim lsDate As DateTime
        Dim lsTo As String = ""
        Dim lsCc As String = ""
        Dim lsOriginalSubject As String = ""
        Dim lsSubject As String = ""
        Dim lsText As String = ""

        Dim sourceMail As DMMail = Nothing
        If SourceMailID > 0 Then
            sourceMail = DMMail.GetMail(SourceMailID)
            If sourceMail IsNot Nothing Then
                If Not sourceMail.CanView Then
                    sourceMail = Nothing
                    SourceMailID = 0
                End If
            End If
        End If


        If sourceMail IsNot Nothing Then

            If Not Page.IsPostBack Then
                sourceMail.MarkReadForCurrentUser()
            End If

            Dim lcolAtts As MailFileList = MailFileList.GetAttachments(sourceMail.ID)

            If TemplateID = 0 Then 'use the same mail when forwarding/replying?
                moTemplate = Nothing
                TemplateID = sourceMail.Template_ID
            End If

            Try
                lsFrom = sourceMail.From.ToString
                lsDate = sourceMail.Mail_Date
                lsTo = sourceMail.MailTo.ToString
                lsCc = sourceMail.CC.ToString
                lsOriginalSubject = sourceMail.OriginalSubject
                lsSubject = sourceMail.Subject
                lsText = sourceMail.Body
            Catch ex As Exception
                Response.Write(ex.Message)
            End Try

            If MailAction = "REPLY" Then

                AddSenderToRecipientBox(sourceMail)

                txtSubject.Text = "RE: " & lsOriginalSubject
            ElseIf MailAction = "REPLYALL" Then

                AddSenderToRecipientBox(sourceMail)
                FillRecipientBox(txtTo, sourceMail.MailTo)
                FillRecipientBox(txtCC, sourceMail.CC)
                FillRecipientBox(txtBCC, sourceMail.BCC)


                txtSubject.Text = "RE: " & lsOriginalSubject
            ElseIf MailAction = "FORWARD" Then
                txtSubject.Text = "FW: " & lsOriginalSubject
                ForwardAttachments(lcolAtts)
            End If
            If sourceMail.HtmlBody Then
                lblTemplate.Text = HtmlSanitizer.Sanitize(sourceMail.FixedMessage)
            Else
                lblTemplate.Text = ArcoFormatting.FormatText(sourceMail.FixedMessage)
            End If

            RadEditor1.Content = CreateTop(lsFrom, lsDate, lsTo, lsCc, lsSubject, lsText, lcolAtts)


            btnchkReplyRequired.Visible = True
        Else 'new from template

            If Template.ID <> 0 Then
                Dim source As Object
                If SourceCaseTechID > 0 Then
                    source = SourceCase
                ElseIf SourceObjectID > 0 Then
                    source = SourceObject
                Else
                    source = Nothing
                End If

                'todo : check formatbody also for fixed body
                lblTemplate.Text = FormatBody(Template.GetFixedBodyToUse(Language, source))
                RadEditor1.Content = FormatBody(Template.GetBodyToUse(Language, source))
                txtSubject.Text = ArcoFormatting.FormatText(Template.GetSubjectToUse(Language, source))

                FillRecipientBox(txtTo, Template.GetMailToToUse(source))
                FillRecipientBox(txtCC, Template.GetCCToUse(source))
                FillRecipientBox(txtBCC, Template.GetBCCToUse(source))

                'add the links
                Dim lsLinkSection As String = Template.GetAttachmentLinksToUse(Language, Nothing, source, "", False, True)
                If Not String.IsNullOrEmpty(lsLinkSection) Then
                    If Not String.IsNullOrEmpty(lblTemplate.Text) Then
                        lblTemplate.Text &= "<br/>" & lsLinkSection
                    Else
                        lblTemplate.Text = lsLinkSection
                    End If
                End If

                If Template.Track AndAlso Template.Reply Then
                    Select Case Template.ReplyRequired
                        Case CheckBoxDisplayMode.AskDefaultOn
                            btnchkReplyRequired.Visible = True
                            ReplyRequired = True
                        Case CheckBoxDisplayMode.AskDefaultOff
                            btnchkReplyRequired.Visible = True
                            ReplyRequired = False
                        Case Else
                            btnchkReplyRequired.Visible = False
                    End Select

                Else
                    btnchkReplyRequired.Visible = False
                End If


                Select Case Template.IncludeDelegates
                    Case CheckBoxDisplayMode.AskDefaultOn
                        chkIncludeDelegates.Visible = True
                        chkIncludeDelegates.Checked = True
                    Case CheckBoxDisplayMode.AskDefaultOff
                        chkIncludeDelegates.Visible = True
                        chkIncludeDelegates.Checked = False
                    Case Else
                        chkIncludeDelegates.Visible = False
                End Select

            Else
                'empty template
                lblTemplate.Text = ""
                RadEditor1.Content = ""
                txtSubject.Text = ""
            End If
        End If

        Track = Template.Track
        Archive = Template.Archive
        If Archive Then
            SendNotification = Template.SendNotification
        Else
            SendNotification = False
        End If




        If Track Then
            AllowExternalTo = False
            AllowExternalCc = True
            'we need to show the cc for the external to
            trCC.Visible = True
        Else

            AllowExternalTo = Template.ExternalRecipients
            AllowExternalCc = Template.ExternalRecipients
            AllowExternalBcc = Template.ExternalRecipients

            trCC.Visible = Template.CC

        End If
        trBCC.Visible = Template.BCC

        If AllowExternalTo Then txtTo.WebServiceSettings.Method = "GetContactsWithExternal"
        If AllowExternalCc Then txtCC.WebServiceSettings.Method = "GetContactsWithExternal"
        If AllowExternalBcc Then txtBCC.WebServiceSettings.Method = "GetContactsWithExternal"

        If Template.AllowAttachments Then
            lblAtt.Visible = True
            pnlAttachments.Visible = True

        End If

        If Not String.IsNullOrEmpty(lblTemplate.Text) Then
            pnlTemplate.Visible = True
            RadEditor1.Height = Unit.Pixel(290)
        Else
            pnlTemplate.Visible = True
            RadEditor1.Height = Unit.Pixel(390)
        End If


    End Sub
    Private Sub AddSenderToRecipientBox(ByVal loMail As DMMail)
        Dim box As RadAutoCompleteBox
        If Not IsEmail(loMail.From.ToString) OrElse Not Track Then
            box = txtTo
        Else
            box = txtCC
        End If
        Dim entry As AutoCompleteBoxEntry = GetAutoCompleteBoxEntry(loMail.From)
        box.Entries.Add(entry)
        If loMail.ReplyRequired Then
            box.Enabled = False
        End If
    End Sub
    Private Function GetAutoCompleteBoxEntry(ByVal rec As DMMail.MailRecipient) As AutoCompleteBoxEntry
        Dim entry As AutoCompleteBoxEntry
        If Not String.IsNullOrEmpty(rec.Account) Then
            entry = New AutoCompleteBoxEntry(ArcoFormatting.FormatUserName(rec.Account, False, False), rec.Account)
        Else
            entry = New AutoCompleteBoxEntry(rec.Email)
        End If
        entry.Enabled = False
        Return entry
    End Function
    Private Sub FillRecipientBox(ByVal box As RadAutoCompleteBox, ByVal recs As DMMail.MailRecipients)

        For Each rec As DMMail.MailRecipient In recs

            Dim entry As AutoCompleteBoxEntry = GetAutoCompleteBoxEntry(rec)
            'check for existence, standard contains doesn't work
            Dim addEntry As Boolean = True
            For Each currentEntry As AutoCompleteBoxEntry In box.Entries
                If currentEntry.Value.Equals(entry.Value) Then
                    addEntry = False
                    Exit For
                End If
            Next
            If addEntry Then
                box.Entries.Add(entry)
            End If
        Next

    End Sub
    Private Sub FillRecipientBox(ByVal box As RadAutoCompleteBox, ByVal values As String)
        If String.IsNullOrEmpty(values) Then
            Exit Sub
        End If

        For Each val As String In values.Split(New String() {";", vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            box.Entries.Add(GetAutoCompleteBoxEntry(New DMMail.MailRecipient(val)))
        Next

    End Sub
    Private Function FormatBody(ByVal body As String) As String
        If Not Template.BodyIsHtml Then
            Return ArcoFormatting.FormatText(body)
        Else
            Return body
        End If
    End Function
    Private Sub ForwardAttachments(ByVal voAtts As MailFileList)
        If voAtts Is Nothing Then
            Exit Sub
        End If

        For Each loAtt As MailFileList.FileInfo In voAtts
            Dim loMailFile As MailFile = MailFile.GetFile(loAtt.ID, 0)
            chkListAtt.Items.Add(Server.HtmlEncode(loMailFile.Name) & "." & loMailFile.FILE_EXT)
            chkListAtt.Items(chkListAtt.Items.Count - 1).Value = "M" & loMailFile.ID
            chkListAtt.Items(chkListAtt.Items.Count - 1).Selected = True
        Next

    End Sub


    Protected Sub imgHyperlinkDocroom_Click(ByVal sender As Object, ByVal e As ImageClickEventArgs) Handles imgHyperlinkDocroom.Click

        If cmbHyperlinksDocroom.SelectedIndex >= 0 Then
            Dim llDocID As Int32 = Convert.ToInt32(cmbHyperlinksDocroom.Items(cmbHyperlinksDocroom.SelectedIndex).Value)
            Dim loDoc As DM_OBJECT = ObjectRepository.GetObject(llDocID)
            RadEditor1.Content &= " <A HREF=" & Chr(34) & loDoc.GetUrl(False) & Chr(34) & ">" & Chr(34) & loDoc.Name & Chr(34) & "</A>"
        End If

    End Sub

    Protected Sub tlbMain_ButtonClick(ByVal sender As Object, ByVal e As RadToolBarEventArgs) Handles tlbMain.ButtonClick
        Dim btn As RadToolBarButton = CType(e.Item, RadToolBarButton)
        Select Case btn.CommandName.ToUpper
            Case "SEND"
                SendMail()
        End Select
    End Sub
End Class
