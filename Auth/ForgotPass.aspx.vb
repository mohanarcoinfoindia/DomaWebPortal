Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Mail
Imports Arco.Doma.Library.Security

Partial Class Auth_ForgotPass
    Inherits BaseAnonymousPage

    Protected Sub Auth_ForgotPass_Load(sender As Object, e As EventArgs) Handles Me.Load

        'check if we can do local login
        If Settings.GetValue("Saml", "IsServiceProvider", False, GetTenantFromUrl().Name) OrElse OpenAuthProviderList.HasDefaultAndEnabledProvider() Then
            GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INVALIDOPERATION)
            Return
        End If

        SetDefaultPageTitle()
        SetRedirectUri()

        pnlMsg.Visible = False
        pnlReg.Visible = True

        lblUserName.Text = GetLabel("username")
        lblRequestHeader.Text = GetLabel("forgotpassword")
        lnkSave.Text = GetLabel("requestpassword")
        capt.ErrorMessage = GetLabel("captchamatchfailed")
        capt.CaptchaLinkButtonText = GetDecodedLabel("refresh")
        lblCode.Text = "Code"


        'prevent double click
        lnkSave.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(lnkSave, Nothing) + ";return false;")

        txtUserName.Focus()
    End Sub
    Private Sub SetRedirectUri()
        If Not IsPostBack Then
            txtRedirectUri.Value = QueryStringParser.GetUrl("redirect_uri")
        End If
    End Sub
    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        If Not IsValid Then
            Exit Sub
        End If

        ShowOk()

        Dim loUser As User = ACL.User.GetUser(txtUserName.Text, False, False, True)
        If loUser IsNot Nothing Then
            If Not String.IsNullOrEmpty(loUser.USER_MAIL) Then

                Dim resetCode As String = loUser.ResetPassword()
                If Not String.IsNullOrEmpty(resetCode) Then
                    Try
                        Dim templateId As Int32 = Settings.GetValue("Security", "ForgotPasswordNotificationTemplate", 0)
                        Dim template As NotificationTemplate
                        If templateId = 0 Then
                            Dim link As String = Arco.IO.Directory.AddForwardSlash(GetClientUrl) & "Auth/Resetpass.aspx?code={0}&redirect_uri=" & Page.Server.UrlEncode(txtRedirectUri.Value)
                            template = NotificationTemplates.GetResetPasswordTemplate(link)
                        Else
                            template = NotificationTemplate.GetNotificationTemplate(templateId)
                        End If
                        Dim loSmtp As ISmtpClient = Smtp.GetClient(Settings)
                        Dim loMsg As Net.Mail.MailMessage = Smtp.NewMessage(Settings)
                        loMsg.To.Add(loUser.USER_MAIL)
                        loMsg.Subject = template.GetSubjectToUse(loUser)
                        loMsg.IsBodyHtml = template.BodyIsHtml
                        loMsg.Body = String.Format(template.GetBodyToUse(loUser), resetCode)

                        loSmtp.Send(loMsg)

                        Arco.Utils.Logging.LogSecurity(loUser.USER_LOGIN & " has requested a password reset.")



                    Catch ex As Exception
                        Arco.Utils.Logging.LogError("Error sending email", ex)
                        ShowError("Error sending email : " & ex.Message)
                    End Try

                    Exit Sub
                End If
            End If
            Arco.ApplicationServer.Library.Commands.SendAdminNotification("Security", "Password reset request (failed)", "A password reset was requested for " & loUser.USER_LOGIN & ", but this users password can't be reset", Arco.Notifications.NotificationLevel.Info)
        Else
            Arco.ApplicationServer.Library.Commands.SendAdminNotification("Security", "Password reset request (failed)", "A password reset was requested for " & txtUserName.Text & ", but no matching users were found", Arco.Notifications.NotificationLevel.Info)
        End If
    End Sub

    Private Sub ShowOk()
        pnlMsg.Visible = True
        pnlReg.Visible = False

        Select Case Language
            Case "N"
                lblMsg.Text = "Als deze account bestaat, zal er een e-mail worden verstuurd naar het bijhorende e-mail adres, met een link om uw wachtwoord te herinitialiseren."
            Case "F"
                lblMsg.Text = "Si votre compte existe, un e-mail sera envoyé à l'adresse e-mail associée, contenant un lien pour réinitialiser votre mot de passe."
            Case Else
                lblMsg.Text = "If your account exists, an e-mail will be sent to the associated email address, containing a link to reset your password."
        End Select

        lblMsg.ForeColor = Color.Black
    End Sub

    Private Sub ShowError(ByVal vsText As String)
        pnlMsg.Visible = True
        lblMsg.Text = vsText
        lblMsg.ForeColor = Color.Red
    End Sub
End Class
