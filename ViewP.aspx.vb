Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Publishing
Imports Arco.Doma.Library.Security

Partial Class ViewP
    Inherits BaseAnonymousPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim lsGUID As String = Request.QueryString("GUID")
        If Not Settings.GetValue("Publishing", "Enabled", True) Then
            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
            Return
        End If
        If Not String.IsNullOrEmpty(lsGUID) Then
            Dim loPublishedItem As PublishedItem = PublishedItem.GetPublishedItem(lsGUID)
            If loPublishedItem IsNot Nothing Then
                Dim isPublishedToEveryone As Boolean = loPublishedItem.ActiveRulesList.Cast(Of PublishedItemRuleList.RuleInfo).
                    Any(Function(x) x.Subject_ID = "Everyone" AndAlso x.Subject_Type = "Role")

                If Not isPublishedToEveryone Then
                    If loPublishedItem.ActiveRulesList.Any Then
                        Dim loGlobal As New AuthenticationModule
                        loGlobal.Login(False) 'do single page login (?)         
                        If Response.IsRequestBeingRedirected Then
                            Exit Sub
                        End If

                        'check if there is an active rule for the current user (after the login)
                        If Not loPublishedItem.ActiveUserRulesList.Any Then
                            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                            Exit Sub
                        End If

                    Else
                        'no rule found at all
                        GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                        Exit Sub
                    End If
                Else
                    GuestBusinessPrincipal.Login(CacheToken.GetEmptyToken(), UserInfoLoader.GetEndUserInfo(Me.Context))
                End If

                Select Case loPublishedItem.Item_Type
                    Case "File"

                        Dim attach As Boolean = False

                        Dim loFile As File = File.GetFileByFIN(loPublishedItem.Item_ID)
                        If loFile IsNot Nothing Then

                            Dim lsRend As String = "native"

                            If loFile.Blocked Then attach = True 'for download

                            Dim ext As String = loFile.GetDownloadExtension(lsRend)
                            If Not attach AndAlso Arco.IO.MimeTypes.ShouldSanbox(ext) Then
                                Dim isSandBoxed As Boolean = QueryStringParser.GetBoolean("sb")
                                If Not isSandBoxed Then
                                    RenderSandbox("Viewp.aspx?GUID=" & loPublishedItem.GUID & "&sb=Y", ext)
                                    Return
                                Else
                                    If Request.UrlReferrer Is Nothing OrElse Request.UrlReferrer.Host <> Request.Url.Host Then
                                        GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                                        Exit Sub
                                    End If
                                End If
                            End If

                            If Settings.GetValue("Audit", "AuditFileViews", True) OrElse loFile.LinkedToObject.CategoryObject.Sensitive Then

                                    loFile.AddViewedAudit(lsRend, True)
                                End If


                                loPublishedItem.Status = ItemStatus.Read
                                loPublishedItem.Save()

                                TrackMailAttachmentRead()



                            If ext <> "htm" Then

                                Dim fileName As String = loFile.GetDownloadFileName(ext)
                                Arco.Utils.Web.Streamer.StreamStream(Request, Response, loFile.GetStream(lsRend), loFile.GetContentSize(lsRend), fileName, fileName, attach, Convert.ToDateTime(loFile.FILE_MODIFDATE), loFile.Hash)
                            Else
                                RenderHtml(loFile)
                            End If


                        Else
                            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                        End If
                End Select
            Else
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
            End If
        Else
            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
        End If
    End Sub

    Private Sub TrackMailAttachmentRead()
        Dim mailId As Int32 = QueryStringParser.GetInt("trackmail")
        If mailId > 0 Then
            Dim loMail As Mail.DMMail = Mail.DMMail.GetMail(mailId)
            loMail.MarkAttachmentRead()
        End If
    End Sub

    Private Sub RenderSandbox(ByVal url As String, ByVal ext As String)
        Dim sb As New StringBuilder
        sb.AppendLine("<!DOCTYPE html>")
        sb.AppendLine("<html><head>")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""App_Themes/Common/Stylesheet.css"">")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""App_Themes/Common/Icons.css"">")
        sb.AppendLine("<link rel=""stylesheet"" type=""text/css"" href=""App_Themes/Common/bootstrap.min.css"">")
        sb.AppendLine("<style type=""text/css""> html, body,iframe {height:100%;width:100%; margin:0;} </style>")
        sb.AppendLine("</head><body style='overflow:hidden'>")

        If Not Arco.IO.MimeTypes.HasOwnScrolling(ext) Then
            sb.AppendLine("<iframe id='docframe' src=""" & url & """ scrolling='yes' ALIGN='top' style='z-index:999999;border:0px;' sandbox='allow-popups'></iframe>")
        Else
            sb.AppendLine("<iframe id='docframe' src=""" & url & """ scrolling='no'  ALIGN='top' style=""z-index:999999;border:0px;height:100%"" sandbox='allow-popups'></iframe>")
        End If
        sb.AppendLine("</body></html>")

        Response.Write(sb.ToString)
        Response.Flush()
    End Sub

    Private Sub RenderHtml(ByVal file As File)

        Dim enc As Encoding = Response.ContentEncoding

        Page.Header.Controls.Clear()


        Response.Cache.SetCacheability(HttpCacheability.Private)
        Response.Cache.SetETag(file.Hash)
        Response.Cache.SetLastModified(Convert.ToDateTime(file.FILE_MODIFDATE))
        Response.Cache.SetExpires(Arco.SystemClock.Now.AddDays(30))


        Dim lsContent As String = file.GetString("htm", enc)

        'force all links to popup (target = _blank)
        lsContent = Arco.Utils.Html.ForceLinksToPopup(lsContent)

        Response.ContentEncoding = enc

        Response.Write(lsContent)

    End Sub

End Class
