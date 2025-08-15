Imports System.Threading
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Mail
Imports Arco.Utils.Web
Imports Arco.QueryParser

Public Class DM_VIEW_FILE
    Inherits BasePage

    Public Property Terms As String
    Public Property FromMail As Boolean
    Public Property FromArchive As Boolean
    Public Property Attach As Boolean

    Public Property Rendition As String


    Private _isFileDownload As Boolean

    Private _action As String

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

    Private thisFile As File
    Private thisMailFile As MailFile

    Public Sub New()
        AllowQueryStringTokenLogin = True
        AllowGuestAccess = True
    End Sub

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load

        Dim fileId As Integer = QueryStringParser.GetInt("FILE_ID")


        FromArchive = QueryStringParser.GetBoolean("fromarchive")
        FromMail = QueryStringParser.GetBoolean("frommail")
        '    Attach = QueryStringParser.GetBoolean("attach")
        Dim mailId As Integer = QueryStringParser.GetInt("DM_MAIL_ID")
        Rendition = InputSanitizer.Sanitize(QueryStringParser.GetString("rend"))
        Attach = QueryStringParser.GetBoolean("attach")
        _action = QueryStringParser.GetString("action")



        If Not FromMail Then
            If fileId > 0 Then
                If Not FromArchive Then
                    thisFile = File.GetFile(fileId)
                    If thisFile Is Nothing Then
                        'check if this file is archived in the mean time
                        thisFile = File.GetFileFromArchive(fileId)
                        If thisFile IsNot Nothing Then
                            'get the active version
                            thisFile = File.GetFileByFIN(thisFile.FIN)
                        End If
                    End If

                    If thisFile Is Nothing Then
                        GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                        Return
                    End If

                Else
                    thisFile = File.GetFileFromArchive(fileId)
                    If thisFile Is Nothing Then
                        thisFile = File.GetFile(fileId)

                    End If

                    If thisFile Is Nothing Then
                        GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                        Return
                    End If

                End If
            Else
                Dim llFin As Int32 = QueryStringParser.GetInt("FILE_FIN")
                If llFin > 0 Then
                    thisFile = File.GetFileByFIN(llFin)
                End If

                If thisFile Is Nothing Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                    Return
                End If
            End If

        Else
            thisMailFile = MailFile.GetFile(fileId, mailId)
            If thisMailFile Is Nothing Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                Return
            End If

        End If


        Dim lbACLOk As Boolean
        Dim lsCode As LibError.ErrorCode = LibError.ErrorCode.ERR_NOERROR
        If Not FromMail Then
            lbACLOk = thisFile.CanView
            If Not lbACLOk Then
                lsCode = thisFile.GetLastError.Code
            End If
        Else
            lbACLOk = thisMailFile.CanView
            If Not lbACLOk Then
                lsCode = LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS
            End If
        End If
        If lbACLOk Then
            Terms = QueryStringParser.GetString("terms")
            If String.IsNullOrEmpty(_action) OrElse _action = "init" Then
                DoInitActions()
            ElseIf _action = "viewdoc" Then
                ShowFile()
            End If
        Else
            GotoErrorPage(lsCode)
        End If

    End Sub

    Private Sub TrackMailAttachmentRead()
        Dim mailId As Int32 = QueryStringParser.GetInt("trackmail")
        If mailId > 0 Then
            Dim loMail As DMMail = DMMail.GetMail(mailId)
            loMail.MarkAttachmentRead()
        End If
    End Sub

    Private Sub ShowFile()
        Try
            Dim ext As String

            If Not FromMail Then

                If Rendition = "preview" Then
                    Rendition = thisFile.GetPreviewRendition
                    If Rendition = "nopreview" Then
                        ShowPreviewNotAvailableForm()
                        Exit Sub
                    End If
                End If
                If (String.IsNullOrEmpty(Rendition) OrElse Rendition = "native") AndAlso thisFile.FILE_EXT = "htm" AndAlso Not thisFile.IsRemoteUrl AndAlso Not Attach Then
                    Rendition = "htm"
                End If

                If Rendition = "download" Then
                    Rendition = "native"
                    Attach = True
                End If

                If Rendition = "viewer" AndAlso Not Settings.GetValue("DocumentViewer", "Enabled", False) Then
                    Rendition = "native"
                End If

                ext = thisFile.GetDownloadExtension(Rendition)

                EnsureIsSandBoxed(ext)


                Audit()
                If Not thisFile.Blocked OrElse Attach Then
                    If Rendition = "viewer" Then
                        Attach = False
                        Response.Redirect("~/DocumentViewer/Default.aspx?id=" & thisFile.ID)
                    ElseIf (Rendition = "htm" AndAlso Not thisFile.FILE_EXT.Contains("xls")) Then

                        RenderHtml()
                    ElseIf Rendition = "idx" Then
                        RenderIndex()
                    Else

                        Dim loContent As FileServers.FileContent = thisFile.GetContent(Rendition, Attach)
                        If loContent.Mode = FileServers.FileContent.eMode.Stream Then
                            Dim contentStream As System.IO.Stream = loContent.Stream

                            If contentStream IsNot Nothing Then
                                Dim contentLength As Long = thisFile.GetContentSize(Rendition)
                                Dim downloadFileName As String = thisFile.GetDownloadFileName(ext)
                                _isFileDownload = True
                                Streamer.StreamStream(Page.Request, Page.Response, contentStream, contentLength, downloadFileName, downloadFileName, Attach, Convert.ToDateTime(thisFile.FILE_MODIFDATE), thisFile.Hash)
                            Else
                                ShowFileNotAvailableForm()
                            End If
                        Else
                            RedirectToFileContentUrl(loContent)
                        End If
                    End If
                Else
                    ShowFileBlockedForm()
                End If

            Else
                If Not thisMailFile.Blocked OrElse Attach Then
                    Dim loContent As FileServers.FileContent
                    Try
                        loContent = thisMailFile.GetContent(Rendition, Attach)
                    Catch ex As System.IO.IOException
                        'thisMailFile.TryRegenerate()
                        'now read it again
                        'loContent = thisMailFile.GetContent(Rendition, mbAttach)
                        Throw
                    End Try

                    If loContent.Mode = FileServers.FileContent.eMode.Stream Then
                        ext = thisMailFile.GetDownloadExtension(Rendition)
                        EnsureIsSandBoxed(ext)

                        If ext <> "htm" Then
                            Dim contentStream As System.IO.Stream = loContent.Stream
                            If contentStream IsNot Nothing Then
                                _isFileDownload = True
                                Dim downloadFileName As String = thisMailFile.GetDownloadFileName(thisMailFile.GetDownloadExtension(Rendition))
                                Streamer.StreamStream(Page.Request, Page.Response, contentStream, thisMailFile.GetContentSize(Rendition), downloadFileName, downloadFileName, Attach, Convert.ToDateTime(thisMailFile.FILE_INDEXDATE), thisMailFile.Hash)
                            Else
                                ShowFileNotAvailableForm()
                            End If
                        Else
                            RenderHtmlForMailFile()
                        End If
                    Else
                        RedirectToFileContentUrl(loContent)
                    End If
                Else
                    ShowFileBlockedForm()
                End If

            End If
        Catch tex As ThreadAbortException
            'ignore, client has disconnected
        Catch ex As UnauthorizedAccessException
            Arco.Utils.Logging.LogError("Accedenied error in viewfile.aspx", ex)
            ShowFileNotAvailableForm()
        Catch ex As System.IO.IOException
            Arco.Utils.Logging.LogError("Error in viewfile.aspx", ex)
            ShowFileNotAvailableForm()
        End Try
    End Sub

    Private Sub EnsureIsSandBoxed(ByVal ext As String)

        If Attach OrElse Not Arco.IO.MimeTypes.ShouldSanbox(ext) Then Return

        If Request.UrlReferrer Is Nothing OrElse Request.UrlReferrer.Host <> Request.Url.Host Then
            Throw New UnauthorizedAccessException("Referrer doesn't match, the file is not sandboxed")
        End If
    End Sub

    Private Sub RenderIndex()

        Dim enc As Encoding = Response.ContentEncoding

        Page.Header.Controls.Clear()

        Dim lsContent As String = thisFile.GetString("idx", enc)
        lsContent = lsContent.Replace(vbCrLf, "<br>")
        lsContent = "<htm><head></head><body>" & lsContent & "</body></htm>"

        Response.ContentEncoding = enc

        Response.Write(lsContent)

    End Sub
    Private Sub RenderHtml()

        Dim enc As Encoding = Response.ContentEncoding

        Page.Header.Controls.Clear()

        If String.IsNullOrEmpty(Terms) Then
            Response.Cache.SetCacheability(HttpCacheability.Private)
            Response.Cache.SetETag(thisFile.Hash)
            Response.Cache.SetLastModified(Convert.ToDateTime(thisFile.FILE_MODIFDATE))
            Response.Cache.SetExpires(Arco.SystemClock.Now.AddDays(30))
        End If

        Dim lsContent As String = thisFile.GetString("htm", enc)

        'also make img tags and other tags absolute
        Dim lsImgUrl As String = thisFile.RenditionServer.Url
        If Not String.IsNullOrEmpty(lsImgUrl) Then
            If Not String.IsNullOrEmpty(thisFile.Html) Then
                lsImgUrl = lsImgUrl & thisFile.Html.Substring(0, thisFile.Html.LastIndexOf("\", StringComparison.Ordinal)).Replace("\", "/")
            End If

            lsContent = Arco.Utils.Html.ReplaceRelativeLinksWithAbsoluteLinks(lsContent, lsImgUrl)
        End If

        'force all links to popup (target = _blank)
        lsContent = Arco.Utils.Html.ForceLinksToPopup(lsContent)

        'no need yet, doesn't do anything
        'lsContent = HtmlSanitizer.SanitizeFileContent(lsContent)

        Response.ContentEncoding = enc

        lsContent = ApplyHighlighting(lsContent)

        Response.Write(lsContent)

    End Sub
    Private Sub RenderHtmlForMailFile()

        Dim enc As Encoding = Response.ContentEncoding

        Page.Header.Controls.Clear()

        Dim lsContent As String = thisMailFile.GetString(enc)

        'force all links to popup (target = _blank)
        lsContent = Arco.Utils.Html.ForceLinksToPopup(lsContent)

        'no need yet, doesn't do anything
        'lsContent = HtmlSanitizer.SanitizeFileContent(lsContent)

        Response.ContentEncoding = enc

        lsContent = ApplyHighlighting(lsContent)

        Response.Write(lsContent)

    End Sub
    Private Sub RedirectToFileContentUrl(ByVal file As FileServers.FileContent)
        If Not Attach Then
            Response.Redirect(file.Url)
        Else
            Dim script As String = String.Format("var w = parent.GetContentWindow().window.open({0},""blank"",""width=800,height=600,resizable=yes,scrollbars=yes,status=yes"");w.focus();", EncodingUtils.EncodeJsString(file.Url))
            Page.ClientScript.RegisterStartupScript(Me.GetType, "OpenFileContent", script, True)
        End If
    End Sub

    Private Sub Audit()
        If Settings.GetValue("Audit", "AuditFileViews", True) OrElse thisFile.LinkedToObject.CategoryObject.Sensitive Then
            thisFile.LinkedToObject.MarkRead()
            If Not Attach Then
                thisFile.AddViewedAudit(Rendition)
            Else
                thisFile.AddDownloadAudit()
            End If
        Else
            'always audit download?
            If Attach Then
                thisFile.AddDownloadAudit()
            End If
        End If
    End Sub

    Private Function ApplyHighlighting(ByVal vsContent As String) As String
        If Not String.IsNullOrEmpty(Terms) Then

            Dim loTerms As New TextParser.TermList()
            loTerms.FromString(Terms, "FILE_TXT")

            Dim loHL As New Highlighting
            vsContent = loHL.Highlight(vsContent, loTerms, "FILE_TXT")
            'vsContent = loHL.AddHLMarkers(vsContent)
            'vsContent = Arco.Utils.Html.AddToTagContent(vsContent, "head", loHL.GetHLToolbarHeader())
            'vsContent = Arco.Utils.Html.AddToTagContent(vsContent, "body", Highlighting.GetHLToolbar("/App_Themes/" & Theme & "/img/"))
        End If

        Return vsContent
    End Function

    Private Sub RedirectToMailView(ByVal hideToolbar As Boolean)

        'audit mailview here
        Audit()

        Dim url As String = "./MailClient/View.aspx?DM_MAIL_ID=" & thisFile.ReferenceID & QueryStringParser.CreateQueryStringPart("hidetoolbar", hideToolbar) & QueryStringParser.CreateQueryStringPart("preview", QueryStringParser.Preview)

        Response.Redirect(url, False)
        _isFileDownload = True
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    Private Sub DoInitActions()

        TrackMailAttachmentRead()

        If Not FromMail Then
            If thisFile.isMail AndAlso Attach Then
                Dim lcolFiles As FileList = FileList.GetFileList(New FileList.Criteria With {
                    .FILE_ID = thisFile.ID,
                    .Type = File.File_Type.MailReference
                })
                _isFileDownload = True
                Streamer.StreamFile(Request, Response, lcolFiles.ZIP, "", True)

            ElseIf thisFile.isMail AndAlso Rendition <> "idx" Then
                RedirectToMailView(QueryStringParser.GetBoolean("hidetoolbar"))

            Else

                Dim lbShowToolBar As Boolean = (Not Attach AndAlso Rendition <> "thumb" AndAlso Rendition <> "largethumb" AndAlso UserProfile.ShowFileToolbar AndAlso Not QueryStringParser.GetBoolean("hidetoolbar"))
                Dim leToolbarPosition As VerticalAlign
                If QueryStringParser.GetString("toolbarpos") = "bottom" Then
                    leToolbarPosition = VerticalAlign.Bottom
                Else
                    leToolbarPosition = VerticalAlign.Top
                End If
                If Not thisFile.IsRemoteUrl Then
                    Dim lsRendToShow As String = Rendition
                    If lsRendToShow = "native" AndAlso thisFile.FILE_EXT = "htm" AndAlso Not String.IsNullOrEmpty(thisFile.Html) AndAlso Not Attach Then
                        lsRendToShow = "htm"
                    End If
                    If lsRendToShow = "preview" Then
                        lsRendToShow = thisFile.GetPreviewRendition
                    End If
                    If lsRendToShow = "download" Then
                        lsRendToShow = "native"
                    End If

                    If lsRendToShow = "mail" Then
                        RedirectToMailView(Not lbShowToolBar)
                        Return
                    End If
                    If lsRendToShow = "thumb" AndAlso Not thisFile.HasRendition(lsRendToShow) Then
                        _isFileDownload = True
                        Streamer.StreamFile(Page.Request, Page.Response, Server.MapPath("./Images/FileTypes/" & Icons.GetFileThumbNail(thisFile.FILE_EXT, 0, False)), thisFile.GetDownloadFileName("gif"), Convert.ToDateTime(thisFile.FILE_MODIFDATE), thisFile.Hash)
                        Return
                    End If

                    RedirectToView(lbShowToolBar, lsRendToShow, leToolbarPosition)
                Else
                    If Not Attach Then

                        If Not QueryStringParser.GetBoolean("hidetoolbar") Then
                            ShowIFrame(thisFile.FILE_PATH, "url", "", leToolbarPosition)
                        Else
                            Response.Redirect(thisFile.FILE_PATH)
                        End If

                    Else
                        Response.Redirect(thisFile.FILE_PATH)
                    End If
                End If
            End If
        Else
            RedirectToView(False, "", VerticalAlign.NotSet)
        End If

    End Sub

    Private Function IsOffice(ByVal vsExt As String) As Boolean
        Select Case vsExt
            Case "doc", "docx", "xls", "xlsx", "ppt", "pptx"
                Return True
            Case Else
                Return False
        End Select
    End Function

    Private Function MustAddSecurityToken(ByVal vsExt As String) As Boolean
        Return IsOffice(vsExt)
    End Function

    Private Sub AddSecurityToken()
        Dim lsToken As String = New Arco.Utils.Crypt(Arco.Settings.FrameWorkSettings.GetSetting("SytemEncryptionKey")).Encrypt(Arco.SystemClock.Now.ToString("yyyy-MM-dd HH:mm:ss") & ";" & Session.SessionID)
        Dim cookie As New HttpCookie("sectoken", lsToken)
        cookie.Expires = Arco.SystemClock.Now.AddSeconds(15)
        Response.Cookies.Add(cookie)

    End Sub

    Private Sub RedirectToView(ByVal withToolbar As Boolean, ByVal vsRendToShow As String, ByVal toolbarPosition As VerticalAlign)

        Dim sbUrl As New StringBuilder
        Dim lsExt As String

        sbUrl.Append("DM_VIEW_FILE.aspx?action=viewdoc&FILE_ID=")
        If Not FromMail Then
            sbUrl.Append(thisFile.ID)
            sbUrl.Append(QueryStringParser.CreateQueryStringPart("ATTACH", Attach))
        Else
            sbUrl.Append(thisMailFile.ID)
            sbUrl.Append(QueryStringParser.CreateQueryStringPart("ATTACH", Attach))
            sbUrl.Append("&DM_MAIL_ID=")
            sbUrl.Append(thisMailFile.MAIL_ID)
        End If

        sbUrl.Append(QueryStringParser.CreateQueryStringPart("FROMMAIL", FromMail))
        sbUrl.Append(QueryStringParser.CreateQueryStringPart("rend", Rendition))
        sbUrl.Append(QueryStringParser.CreateQueryStringPart("terms", Server.UrlEncode(Terms)))
        sbUrl.Append(QueryStringParser.CreateQueryStringPart("fromarchive", FromArchive))
        If thisFile IsNot Nothing Then

            sbUrl.Append(QueryStringParser.CreateQueryStringPart("hash", thisFile.Hash))

            lsExt = thisFile.GetDownloadExtension(vsRendToShow)
        Else
            lsExt = thisMailFile.FILE_EXT
        End If
        If MustAddSecurityToken(lsExt) Then AddSecurityToken()
        SiteManagement.SitesManager.AddSiteInformation(sbUrl, False)


        If lsExt = "pdf" Then
            sbUrl.Append("#view=Fit")
        Else
            'todo : check if we still need this
            sbUrl.Append("&ext=.")
            sbUrl.Append(lsExt)
        End If

        If withToolbar Then
            ShowIFrame(sbUrl.ToString, lsExt, vsRendToShow, toolbarPosition)
        Else
            If vsRendToShow <> "Thennopreview" AndAlso IsOffice(lsExt) Then
                Response.Redirect(sbUrl.ToString, False) 'redirect with token, we need to share our security token with office
                _isFileDownload = True
                Context.ApplicationInstance.CompleteRequest()
            Else
                If Not Attach AndAlso Arco.IO.MimeTypes.ShouldSanbox(lsExt) Then
                    ShowSandboxedFile(sbUrl.ToString, lsExt)
                Else
                    ShowFile()

                End If

            End If
        End If
    End Sub

    Private Function ToolBarButtonImgClass(ByVal label As String, ByVal javascript As String, ByVal imgClass As String) As String
        If String.IsNullOrEmpty(imgClass) Then
            Return String.Format("<span style='margin-left:5px;margin-right:5px'><a href='javascript:{0};' style='color:#000000;font-weight:bold;text-decoration:underline;font-size:7.5pt'>{1}</a></span>", javascript, label)
        Else
            Return String.Format("<span style='margin-left:5px;margin-right:5px'><a href='javascript:{0};'>{1}</a></span>", javascript, ThemedImage.GetSpanIconTag(imgClass, label))
        End If
    End Function

    Private Function ToolBarButton(ByVal label As String, ByVal javaScript As String, ByVal img As String) As String
        If String.IsNullOrEmpty(img) Then
            Return String.Format("<span style='margin-left:5px;margin-right:5px'><a href='javascript:{0};' style='color:#000000;font-weight:bold;text-decoration:underline;font-size:7.5pt'>{1}</a></span>", javaScript, label)
        Else
            If Not img.Contains("/") Then
                Return String.Format("<span style='margin-left:5px;margin-right:5px'><a href='javascript:{0};'>{1}</a></span>", javaScript, ThemedImage.GetImageTag(img, Me, label))
            Else
                Return String.Format("<span style='margin-left:5px;margin-right:5px'><a href='javascript:{0};'>{1}</a></span>", javaScript, "<img src='./Images/" & img & "' tooltip='" & label & "'/>")
            End If

        End If
    End Function

    Private Sub ShowIFrame(ByVal url As String, ByVal vsExtToShow As String, ByVal vsRendToShow As String, ByVal veToolbarPosition As VerticalAlign)
        Dim HeaderText As String
        Dim liPrevFile As Int32 = 0
        Dim liNextFile As Int32 = 0
        Dim liCurrFile As Int32
        Dim liCurrFileIndex As Int32 = 1
        Dim liTotalFileCount As Int32 = 0
        Dim i As Int32 = 1
        Dim lbFound As Boolean = False
        Dim llLastId As Int32
        Dim fileTitle As String
        Dim lsRightToolbar As String = ""
        Dim sbLeftToolbar As New StringBuilder
        Dim lsCenterToolbar As String = ""
        Dim canDelete As Boolean
        Dim canModify As Boolean
        Dim mailId As Integer = 0

        If Not thisFile Is Nothing Then
            HeaderText = String.Concat(thisFile.LinkedToObject.Name, " : ", thisFile.Name)
            If Not thisFile.Name.Contains("." & thisFile.FILE_EXT) Then
                fileTitle = String.Concat(thisFile.Name, ".", thisFile.FILE_EXT)
            Else
                fileTitle = thisFile.Name
            End If

            If thisFile.FromArchive Then
                fileTitle &= " (" & thisFile.FILE_CHECKVERSION.toString() & ")"
            End If

            liCurrFile = thisFile.ID
            For Each loFileInfo As FileList.FileInfo In thisFile.LinkedToObject.Files
                If thisFile.ID = loFileInfo.ID Then
                    liCurrFileIndex = i
                    lbFound = True
                    liPrevFile = llLastId
                ElseIf lbFound AndAlso liNextFile = 0 Then
                    liNextFile = loFileInfo.ID
                    Exit For
                End If
                llLastId = loFileInfo.ID
                i = i + 1
            Next

            liTotalFileCount = thisFile.LinkedToObject.FileCount

            Dim loFT As DM_FileTypeList.FileTypeInfo = thisFile.LinkedToObject.GetFileType(thisFile.FILE_EXT)
            canModify = thisFile.CanModify

            'todo : verify/change icons
            lsCenterToolbar = String.Format("<span class='dmViewFileName'>{0}</span>", fileTitle)
            If canModify Then
                sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("editfileproperties"), "ShowFileAction(1)", "icon icon-rename"))
            End If

            If vsExtToShow <> "url" AndAlso loFT.AllowDownloads Then
                sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("download"), "DownloadFile()", "icon icon-download"))
            End If
            Dim canCheckout As Boolean = canModify AndAlso thisFile.CanCheckOut
            Dim canCheckin As Boolean = canModify AndAlso thisFile.CanCheckIn

            If canModify AndAlso thisFile.Type = Arco.Doma.Library.File.File_Type.File Then
                If thisFile.IsRemoteUrl Then
                    sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("editurl"), "ShowFileAction(2)", "icon icon-edit"))
                Else


                    If loFT.FILE_WEBDAVENABLED AndAlso (thisFile.Locked = Arco.Doma.Library.File.LockingStatus.NoLock OrElse thisFile.Locked = Arco.Doma.Library.File.LockingStatus.WebDavLock) Then
                        sbLeftToolbar.Append(ToolBarButton(GetLabel("editfile") & " " & GetLabel("with") & " " & Arco.IO.MimeTypes.GetWebDavApplicationName(loFT.Extension), "EditFile()", Icons.GetFileIcon(thisFile.FILE_EXT)))
                    End If
                    If loFT.FILE_INLINEEDITING AndAlso (thisFile.Locked = Arco.Doma.Library.File.LockingStatus.NoLock OrElse thisFile.Locked = Arco.Doma.Library.File.LockingStatus.RemoteLock2) Then
                        sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("editfile"), "EditFileDirect()", "icon icon-edit"))
                    End If
                End If
                If thisFile.Locked = Arco.Doma.Library.File.LockingStatus.NoLock Then
                    sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("replacefile"), "ReplaceFile()", "icon icon-file-replace"))
                End If

            End If
            If canCheckout Then
                sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("checkout"), "ShowAjaxFileAction(3)", "icon icon-document-CHout"))
            End If
            If canCheckin Then
                If thisFile.FILE_CHECKBY = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
                    sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("checkin"), "ShowFileAction(0)", "icon icon-document-CHin"))
                Else
                    sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("admincheckin"), "ShowFileAction(0)", "icon icon-document-CHin"))
                End If

                If thisFile.FILE_CHECKBY = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
                    sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("cancelcheckout"), "ShowAjaxFileAction(4)", "icon icon-document-CHout-cancel"))
                Else
                    sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("admincancelcheckout"), "ShowAjaxFileAction(4)", "icon icon-document-CHout-cancel"))
                End If
            End If

            If thisFile.Locked = Arco.Doma.Library.File.LockingStatus.WebDavLock AndAlso loFT.FILE_WEBDAVENABLED Then
                sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("collectfile"), "ShowAjaxFileAction(5)", "icon icon-chevron-up-xs icon-color-light"))
            End If

            If Settings.GetValue("Publishing", "Enabled", True) AndAlso Not thisFile.LinkedToObject.IsInRecycleBin Then
                sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("publishing"), "Publishing()", "icon icon-export"))
            End If

            If canModify AndAlso thisFile.CanDelete Then
                canDelete = True
                sbLeftToolbar.Append(ToolBarButtonImgClass(GetLabel("delete"), "DeleteFile()", "icon icon-delete"))
            End If
        Else
            'mailfile
            HeaderText = thisMailFile.Name
            fileTitle = thisMailFile.Name
            liCurrFile = thisMailFile.ID
            mailId = thisMailFile.MAIL_ID
            For Each loMailFile As MailFileList.FileInfo In MailFileList.GetAttachments(thisMailFile.MAIL_ID)
                If thisMailFile.ID = loMailFile.ID Then
                    liCurrFileIndex = i
                    lbFound = True
                    liPrevFile = llLastId
                ElseIf lbFound AndAlso liNextFile = 0 Then
                    liNextFile = loMailFile.ID
                    Exit For
                End If
                llLastId = loMailFile.ID
                i = i + 1
            Next


            lsCenterToolbar = String.Format("<span class='dmViewFileName'>{0}</span>", fileTitle)

            '  sbLeftToolbar.Append(fileTitle)
        End If

        Dim sb As New StringBuilder
        sb.AppendLine("<!DOCTYPE html>")
        sb.AppendLine("<html><head>")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""App_Themes/Common/Stylesheet.css"">")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""App_Themes/Common/Icons.css"">")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""App_Themes/Common/bootstrap.min.css"">")
        sb.AppendLine("<style type=""text/css""> html, body,iframe {height:100%;width:100%; margin:0;} </style>")
        sb.AppendLine("<script language='javascript'>")

        'simulate a ajax request using a popup

        sb.Append("function DoActionUrl(url){")
        sb.Append("if (window.opener){")
        sb.Append("if (window.opener.parent && window.opener.parent.DoActionUrl) {")
        sb.Append("window.opener.parent.DoActionUrl(url);")
        sb.Append("} else {")
        sb.Append("if (window.opener.DoActionUrl) {")
        sb.Append("window.opener.DoActionUrl(url);")
        sb.Append("} else {")
        sb.Append("window.open(url, 'action', 'height=600,width=800,scrollbars=yes,resizable=yes');")
        sb.Append("}}")
        '   sb.Append("window.close();")
        sb.Append("} else {")
        sb.Append("if (window != window.top) {")
        sb.Append("if (window.parent.opener) {")
        sb.Append("window.parent.opener.parent.DoActionUrl(url);")
        sb.Append("window.parent.close();")
        sb.Append("} else {")
        sb.Append("if (parent.DoActionUrl) {")
        sb.Append("parent.DoActionUrl(url);")
        sb.Append("} else {")
        sb.Append("parent.parent.DoActionUrl(url);")
        sb.Append("}")
        sb.Append("}")
        sb.Append("} else {")
        sb.Append("window.open(url, 'action', 'height=600,width=800,scrollbars=yes,resizable=yes');")
        sb.Append("}")
        sb.AppendLine("}}")

        If canDelete Then
            sb.Append("function DeleteFile(){")
            sb.Append("if (!confirm(")
            sb.Append(EncodingUtils.EncodeJsString(GetDecodedLabel("confirmdelete")))
            sb.Append(")) {return;}")
            sb.Append("ShowAjaxFileAction(6)}")
        End If

        sb.Append("function ShowAjaxFileAction(action){")
        sb.Append("var w = window.open('DM_FILEACTIONS.aspx?DM_FILE_ID=")
        sb.Append(liCurrFile)
        sb.Append("&action=' + action, 'FILEACTION', 'height=250,width=450,scrollbars=yes,resizable=yes');")
        sb.Append("}")
        sb.Append("function ShowFileAction(action){")
        sb.Append("var w = window.open('DM_FILEACTIONS.aspx?DM_FILE_ID=")
        sb.Append(liCurrFile)
        sb.Append("&action=' + action, 'FILEACTION', 'height=350,width=500,scrollbars=yes,resizable=yes');")
        sb.Append("w.focus();")
        sb.AppendLine("}")

        sb.Append("function ReplaceFile(){")
        sb.Append("var w = window.open('DM_FILE_ADD.aspx?FILE_ID=")
        sb.Append(liCurrFile)
        sb.Append("', 'AddFile', 'height=600,width=900,scrollbars=yes,resizable=yes');")
        sb.Append("w.focus();")
        sb.AppendLine("}")

        sb.Append("function EditFile() {")
        sb.Append("DoActionUrl('DM_EDIT_FILE.aspx?FILE_ID=")
        sb.Append(liCurrFile)
        sb.Append("'); ")
        sb.AppendLine("}")

        If Settings.GetValue("Publishing", "Enabled", True) Then
            sb.Append("function Publishing() {")
            sb.Append("var w = window.open('EditPublishing.aspx?ITEM_ID=")
            sb.Append(liCurrFile)
            sb.Append("&ITEM_TYPE=File', 'EditPublishing', 'height=600,width=900,scrollbars=yes,resizable=yes');")
            sb.AppendLine("}")
        End If

        sb.Append("function EditFileDirect() {")
        sb.Append("var w = window.open('DM_EDIT_FILE_DIRECT.aspx?FILE_ID=")
        sb.Append(liCurrFile)
        sb.Append("', 'Editfile', 'height=600,width=900,scrollbars=yes,resizable=yes');")
        sb.AppendLine("}")


        sb.Append("function ReloadParent(fileid){") 'reload from popups       
        sb.Append("var currfile = " & liCurrFile & ";")
        sb.Append("if (fileid != null){")
        sb.Append(" if (fileid != 0){")
        sb.Append("  currfile = fileid;")
        sb.Append(" }")
        sb.Append("}")
        sb.Append(" location.href = 'DM_VIEW_FILE.aspx?action=init&FILE_ID=' + currfile + '")
        If FromMail Then
            sb.Append("&DM_MAIL_ID=")
            sb.Append(thisMailFile.MAIL_ID)
            sb.Append(QueryStringParser.CreateQueryStringPart("FROMMAIL", True))
        End If

        sb.Append("&rend=preview")
        sb.Append(QueryStringParser.CreateQueryStringPart("terms", Server.UrlEncode(Terms)))
        sb.Append(QueryStringParser.CreateQueryStringPart("fromarchive", FromArchive))
        sb.Append("&x=' + Math.random();")
        sb.Append("}")
        sb.Append("function DownloadFile(){")
        sb.Append("DoActionUrl('DM_VIEW_FILE.aspx?FILE_ID=")
        sb.Append(liCurrFile)
        sb.Append("&attach=Y")
        sb.Append(QueryStringParser.CreateQueryStringPart("fromarchive", FromArchive))
        sb.Append("&x=' + Math.random());")
        sb.Append("}")
        sb.Append("if (parent){")
        sb.Append(" if (parent.SetHeader){")
        sb.Append("parent.SetHeader(")
        sb.Append(EncodingUtils.EncodeJsString(fileTitle))
        sb.Append(");")
        sb.Append("} } else { ")
        sb.Append("document.title = ")
        sb.Append(EncodingUtils.EncodeJsString(HeaderText))
        sb.Append(";")
        sb.AppendLine("}")
        sb.AppendLine("function Close(){if (parent.CloseDetail) {parent.CloseDetail(true);} else if (parent.parent.CloseDetail) { parent.parent.CloseDetail(true);}}")
        sb.Append("</script>")

        Dim prevFileLink As String
        If liPrevFile > 0 Then
            prevFileLink = "<a href='" & GetScrollLink(liPrevFile, mailId) & "' >" & ThemedImage.GetSpanIconTag("icon icon-page-previous icon-color-light", GetDecodedLabel("prevfile") & " (" & (liCurrFileIndex - 1) & "/" & liTotalFileCount & ")") & "</a>"
        Else
            prevFileLink = ThemedImage.GetSpanIconTag("icon icon-page-previous icon-color-light")
        End If

        Dim nextFileLink As String
        If liNextFile > 0 Then
            nextFileLink = "<a href='" & GetScrollLink(liNextFile, mailId) & "'  >" & ThemedImage.GetSpanIconTag("icon icon-page-next icon-color-light", GetDecodedLabel("nextfile") & " (" & (liCurrFileIndex + 1) & "/" & liTotalFileCount & ")") & "</a>"
        Else
            nextFileLink = ThemedImage.GetSpanIconTag("icon icon-page-next icon-color-light")
        End If

        If liPrevFile > 0 OrElse liNextFile > 0 Then
            lsRightToolbar = String.Concat(prevFileLink, "&nbsp;", nextFileLink)
        End If

        If QueryStringParser.GetBoolean("hidetabs") Then
            lsRightToolbar &= "&nbsp;<a href='javascript:Close();' >" & ThemedImage.GetSpanIconTag("icon icon-close icon-color-light") & "</a>"
        End If

        sb.AppendLine("</head><body style='overflow:hidden'>")

        Dim showAsIFrame As Boolean = vsExtToShow <> "nopreview"
        If veToolbarPosition <> VerticalAlign.Bottom Then
            sb.Append("<div><table class='dmViewToolbar'>")
            sb.Append("<tr><td align='left' nowrap>")
            sb.Append(sbLeftToolbar.ToString)
            sb.Append("</td><td align='center' width='100%'>")
            sb.Append(lsCenterToolbar)
            sb.Append("</td><td align='right' nowrap>")
            sb.Append(lsRightToolbar)
            sb.Append("</td></tr>")
            sb.Append("</table></div>")

            If Not showAsIFrame Then
                sb.AppendLine("<div class='viewFilePane viewTop' style='overflow:auto;'>")
            Else
                sb.AppendLine("<div class='viewFilePane viewTop'>")
            End If
        Else
            If Not showAsIFrame Then
                sb.AppendLine("<div class='viewFilePane viewBottom' style='overflow:auto;'>")
            Else
                sb.AppendLine("<div class='viewFilePane viewBottom'>")
            End If
        End If

        Select Case vsExtToShow
            Case "nopreview"
                sb.Append(GetPreviewNotAvailableForm)
            Case Else

                If Arco.IO.MimeTypes.ShouldSanbox(vsExtToShow) Then
                    If Not Arco.IO.MimeTypes.HasOwnScrolling(vsExtToShow) Then
                        sb.AppendLine("<iframe id='docframe' src=""" & url & """ scrolling='yes' ALIGN='top' style='z-index:999999;border:0px;' sandbox='allow-popups'></iframe>")
                    Else
                        sb.AppendLine("<iframe id='docframe' src=""" & url & """ scrolling='no'  ALIGN='top' style=""z-index:999999;border:0px;height:100%"" sandbox='allow-popups'></iframe>")
                    End If
                Else
                    If Not Arco.IO.MimeTypes.HasOwnScrolling(vsExtToShow) Then
                        sb.AppendLine("<iframe id='docframe' src=""" & url & """ scrolling='yes' ALIGN='top' style='z-index:999999;border:0px;'></iframe>")
                    Else
                        sb.AppendLine("<iframe id='docframe' src=""" & url & """ scrolling='no'  ALIGN='top' style=""z-index:999999;border:0px;height:100%""></iframe>")
                    End If
                End If

        End Select

        sb.AppendLine("</div>")

        If veToolbarPosition = VerticalAlign.Bottom Then
            sb.AppendLine("<div style='position:absolute;bottom:0px;width:100%;height:30px'><table cellspacing='0' cellpadding='0' style='height:30px'>")
            sb.AppendLine("<tr style='style='background-color:#d3d3d3;'><td align='left' nowrap>" & sbLeftToolbar.ToString & "</td><td align='center' width='100%'>" & lsCenterToolbar & "</td><td align='right' nowrap>" & lsRightToolbar & "</td></tr>")
            sb.AppendLine("</table></div>")
        End If

        sb.AppendLine("</body></html>")

        Response.Write(sb.ToString)
        Response.Flush()
    End Sub

    Private Sub ShowSandboxedFile(ByVal url As String, ByVal vsExtToShow As String)
        Dim sb As New StringBuilder
        sb.AppendLine("<!DOCTYPE html>")
        sb.AppendLine("<html><head>")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""App_Themes/Common/Stylesheet.css"">")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""App_Themes/Common/Icons.css"">")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""App_Themes/Common/bootstrap.min.css"">")
        sb.AppendLine("<style type=""text/css""> html, body,iframe {height:100%;width:100%; margin:0;} </style>")

        If Not Arco.IO.MimeTypes.HasOwnScrolling(vsExtToShow) Then
            sb.AppendLine("<iframe id='docframe' src=""" & url & """ scrolling='yes' ALIGN='top' style='z-index:999999;border:0px;' sandbox='allow-popups'></iframe>")
        Else
            sb.AppendLine("<iframe id='docframe' src=""" & url & """ scrolling='no'  ALIGN='top' style=""z-index:999999;border:0px;height:100%"" sandbox='allow-popups'></iframe>")
        End If

        sb.AppendLine("</body></html>")

        Response.Write(sb.ToString)
        Response.Flush()
    End Sub

    Private Function GetScrollLink(ByVal toFileId As Integer, ByVal mailId As Integer) As String

        Dim link As String

        link = "DM_VIEW_FILE.aspx?action=init&FILE_ID=" & toFileId & "&DM_MAIL_ID=" & mailId & QueryStringParser.CreateQueryStringPart("FROMMAIL", FromMail) & QueryStringParser.CreateQueryStringPart("rend", Rendition) & QueryStringParser.CreateQueryStringPart(Terms, Server.UrlEncode(Terms)) & QueryStringParser.CreateQueryStringPart("fromarchive", FromArchive)


        If QueryStringParser.GetBoolean("hidetabs") Then
            link &= "&hidetabs=Y"
        End If
        Return link
    End Function


    Private Function GetNotAvailableForm(ByVal text As String) As String
        Return String.Format("<div style='margin:5;text-align:center;height:300px;line-height:300px'><span style='font-size:16;color:grey'>{0}</span></div>", text)
    End Function

    Private Function GetPreviewNotAvailableForm() As String
        Return GetNotAvailableForm(GetLabel("previewnotavailable"))
    End Function

    Private Sub ShowPreviewNotAvailableForm()
        Response.Write(GetPreviewNotAvailableForm)
    End Sub

    Private Sub ShowFileNotAvailableForm()
        Response.Write(GetNotAvailableForm("File not found"))
    End Sub
    Private Sub ShowFileBlockedForm()
        Response.Write(GetNotAvailableForm("This file was blocked for online viewing"))
    End Sub
    Private Sub ShowErrorForm(ByVal msg As String)
        If Not Attach Then
            Response.Write(Arco.Web.ErrorHandler.GetErrorForm(msg))
        Else
            Response.Write("<script type='text/javascript'>alert(" & EncodingUtils.EncodeJsString(msg) & ");</script>")
        End If
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        If Not _isFileDownload Then MyBase.Render(writer)
    End Sub
End Class


