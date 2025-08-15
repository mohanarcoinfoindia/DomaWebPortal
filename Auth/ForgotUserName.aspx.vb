Imports Arco.Doma.Library
Imports Arco.Doma.Library.Mail
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Security

Partial Class Auth_ForgotUserName
    Inherits BaseAnonymousPage

    Protected Sub Auth_ForgotUserName_Load(sender As Object, e As EventArgs) Handles Me.Load

        'check if we can do local login
        If Settings.GetValue("Saml", "IsServiceProvider", False, GetTenantFromUrl().Name) OrElse OpenAuthProviderList.HasDefaultAndEnabledProvider() Then
            GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INVALIDOPERATION)
            Return
        End If

        SetDefaultPageTitle()

        pnlMsg.Visible = False
        pnlReg.Visible = True

        lblEmail.Text = GetLabel("mail")
        lblRequestHeader.Text = GetLabel("forgotusername")
        lnkSave.Text = GetLabel("requestpassword")
        emailValidator.ErrorMessage = GetLabel("namail")
        capt.ErrorMessage = GetLabel("captchamatchfailed")
        capt.CaptchaLinkButtonText = GetDecodedLabel("refresh")
        lblCode.Text = "Code"


        'prevent double click
        lnkSave.Attributes.Add("onclick", " this.disabled = true; " + ClientScript.GetPostBackEventReference(lnkSave, Nothing) + ";return false;")

        txtEmail.Focus()
    End Sub

    Protected Sub lnkSave_Click(sender As Object, e As EventArgs) Handles lnkSave.Click
        If Not IsValid Then
            ShowError(GetLabel("validatorerror"))
            Exit Sub
        End If

        ShowOk()

        Dim users As USERList = USERList.GetUSERSList(New USERList.Criteria With {.EMAIL = txtEmail.Text, .OnlyDBUsers = True})
        If users.Any Then
            For Each u As USERList.USERSInfo In users

                Dim loUser As User = ACL.User.GetUser(u.USER_LOGIN)

                Arco.Utils.Logging.LogSecurity(loUser.USER_MAIL & " requested his user name (" & loUser.USER_LOGIN & ")")
                Try

                    Dim templateId As Int32 = Settings.GetValue("Security", "ForgotUserNameNotificationTemplate", 0)
                    Dim template As NotificationTemplate
                    If templateId = 0 Then
                        template = NotificationTemplates.GetForgotUserNameTemplate()
                    Else
                        template = NotificationTemplate.GetNotificationTemplate(templateId)
                    End If
                    Dim loSmtp As ISmtpClient = Smtp.GetClient(Settings)
                    Dim loMsg As Net.Mail.MailMessage = Smtp.NewMessage(Settings)
                    loMsg.To.Add(txtEmail.Text)
                    loMsg.Subject = template.GetSubjectToUse(loUser)
                    loMsg.IsBodyHtml = template.BodyIsHtml
                    loMsg.Body = template.GetBodyToUse(loUser)

                    loSmtp.Send(loMsg)

                Catch ex As Exception
                    ShowError("Error sending email : " & ex.Message)
                    Arco.Utils.Logging.LogSecurity("Error sending email during forgot username for " & txtEmail.Text & " : " & ex.Message)
                End Try

            Next
        Else
            Dim msg As String = "A Forgot username was requested for " & txtEmail.Text & ", but no matching users were found"
            Arco.Utils.Logging.LogSecurity(msg)
            Arco.ApplicationServer.Library.Commands.SendAdminNotification("Security", "Forgot username request (failed)", msg, Arco.Notifications.NotificationLevel.Warning)
        End If
    End Sub

    Private Sub ShowOk()
        pnlMsg.Visible = True
        pnlReg.Visible = False

        Dim email As String = New EmailMFAProvider().FormatTargetForDisplay(Server.HtmlEncode(txtEmail.Text))

        Select Case Language
            Case "N"
                lblMsg.Text = String.Format("Een e-mail is verstuurd naar {0} met uw gebruikersnaam.", email)
            Case "F"
                lblMsg.Text = String.Format("Un resetCode a été envoyé par courriel à {0} avec votre nom d'utilisateur.", email)
            Case Else
                lblMsg.Text = String.Format("An e-mail has been sent to {0}, containing your username.", email)
        End Select

        lblMsg.ForeColor = Color.Black
    End Sub

    Private Sub ShowError(ByVal vsText As String)
        pnlMsg.Visible = True
        lblMsg.Text = vsText
        lblMsg.ForeColor = Color.Red
    End Sub
End Class
