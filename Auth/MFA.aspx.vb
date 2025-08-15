
Imports System.Activities.Statements
Imports Arco.Doma.EPM.ActionTypes
Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Security
Imports Arco.Doma.WebControls
Imports Org.BouncyCastle.Asn1.X509
Imports Telerik.Web.UI.com.hisoftware.api2

Public Class Auth_MFA
    Inherits BaseAfterLoginPage

    Private _mfas As List(Of MultiFactorAuthenticationList.MfaInfo)

    Private Const ShowCaptchaAfterAttempts As Integer = 3

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        SetMessage("")
        SetLabels()

        SetReturnTo()

        If Not Settings.GetValue("Security", "MFAEnabled", False) Then
            SetMessage("MFA is not enabled for this instance")
            Return
        End If
        Dim identity As BusinessIdentity = BusinessIdentity.CurrentIdentity

        Dim isMfaRequired As Boolean = Settings.GetValue("Security", "MFARequired", False)

        _mfas = MultiFactorAuthenticationList.GetMultiFactorAuthenticationList(identity.Name).Cast(Of MultiFactorAuthenticationList.MfaInfo).Where(Function(x) x.GetProvider(Settings).Enabled).ToList()

        'try automatic enabling of mfa
        If isMfaRequired AndAlso Not _mfas.Any() Then
            For Each provider As IMFAProvider In MFAProviderFactory.GetEnabledProviderTypes(Settings)
                Select Case provider.Type
                    Case "otp"
                        MultiFactorAuthentication.Enable(identity.Name, "otp", "")
                        Exit For
                    Case "email"
                        'use user e-mail as mfa
                        If Not String.IsNullOrEmpty(identity.Email) Then
                            MultiFactorAuthentication.Enable(identity.Name, "email", identity.Email)
                            Exit For
                        Else
                            'user has no e-mail
                        End If
                End Select
            Next

            _mfas = MultiFactorAuthenticationList.GetMultiFactorAuthenticationList(identity.Name).Cast(Of MultiFactorAuthenticationList.MfaInfo).Where(Function(x) x.GetProvider(Settings).Enabled).ToList()


        End If

        If Not Page.IsPostBack Then
            If _mfas.Count = 0 Then
                If isMfaRequired Then
                    'user has to register mfa and has no e-mail
                    'continue?
                    ContinueLogin(txtReturnTo.Value, False)
                Else
                    'continue?
                    ContinueLogin(txtReturnTo.Value, False)
                End If
                Return
            End If
        End If

        If _mfas.Any Then
            BuildForm()
        End If

    End Sub

    Private Property OtpSecretKey As String
        Get
            Return ViewState("os").ToString()
        End Get
        Set(value As String)
            ViewState("os") = value
        End Set
    End Property

    Private Sub BuildForm()
        plhSend.Visible = True


        Dim identity As BusinessIdentity = BusinessIdentity.CurrentIdentity
        Dim showVerifyLink As Boolean = identity.TwoFactorAuthenticationCodeSent

        For Each mfaInfo As MultiFactorAuthenticationList.MfaInfo In _mfas
            Dim provider As IMFAProvider = mfaInfo.GetProvider(Settings)
            Select Case mfaInfo.Provider
                Case "otp"

                    txtSelectedProvider.Value = "otp"

                    If String.IsNullOrEmpty(mfaInfo.Target) Then

                        If Not Page.IsPostBack Then
                            OtpSecretKey = Arco.Utils.RandomCode.Generate(20)
                        End If

                        Dim otpAuthUri As String = provider.GetRegisterAuthUri(identity, OtpSecretKey)
                        Dim generator As New QRCodeGenerator()
                        imgOtpQRCode.ImageUrl = generator.Create(otpAuthUri)
                        plhRegisterOtp.Visible = True
                    Else
                        plhVerifyOtp.Visible = True

                        'app already linked, this will store the secret in the mfacode
                        identity.SendTwoFactorAuthenticationCode(provider, mfaInfo.Target)
                    End If


                    showVerifyLink = True

                Case "email"
                    plhSend.Visible = True
                    lnkSendCodeUsingEmail.Text = String.Format(GetLabel("sendemailto"), provider.FormatTargetForDisplay(mfaInfo.Target))
                    lnkSendCodeUsingEmail.Visible = True
                Case "sms"
                    plhSend.Visible = True
                    lnkSendCodeUsingSms.Text = String.Format(GetLabel("sendsmsto"), provider.FormatTargetForDisplay(mfaInfo.Target))
                    lnkSendCodeUsingSms.Visible = True
                Case "log"
                    plhSend.Visible = True
                    lnkSendCodeUsingLog.Text = "Send code to " & provider.FormatTargetForDisplay(mfaInfo.Target)
                    lnkSendCodeUsingLog.Visible = True

            End Select
        Next



        If showVerifyLink Then

            Dim enduserInfo As EndUserInfo = UserInfoLoader.GetEndUserInfo(Me)

            pnlCaptcha.Visible = pnlCaptcha.Visible OrElse LoginAttempts.GetAttemptCount(enduserInfo, Nothing) >= ShowCaptchaAfterAttempts

            plhVerify.Visible = True

            txtCode.Focus()
            pnlMfa.DefaultButton = "lnkVerifyCode"
        Else
            pnlCaptcha.Visible = False
            plhVerify.Visible = False
        End If


    End Sub
    Protected Overrides ReadOnly Property ErrorPageUrl As String
        Get
            Return "../DM_ACL_DENIED.aspx"
        End Get
    End Property

    Private Sub SetReturnTo()
        If Not Page.IsPostBack Then
            txtReturnTo.Value = QueryStringParser.GetUrl("redirect_uri")
        End If
    End Sub
    Private Sub SetLabels()
        lblCode.Text = "Code" ' GetLabel("code")

        lnkVerifyCode.Text = GetLabel("verifymfa")

        capt.ErrorMessage = GetLabel("captchamatchfailed")
        capt.CaptchaLinkButtonText = GetDecodedLabel("refresh")
    End Sub
    Private Sub SetMessage(ByVal vsMsg As String)
        If Not String.IsNullOrEmpty(vsMsg) Then
            pnlMsg.Visible = True
            If Not String.IsNullOrEmpty(lblMsg.Text) Then
                lblMsg.Text &= "<br/>" & vsMsg
            Else
                lblMsg.Text = vsMsg
            End If
        Else
            lblMsg.Text = ""
            pnlMsg.Visible = False
        End If
    End Sub
    Protected Sub lnkSendCodeUsingEmail_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSendCodeUsingEmail.Click

        SendCode("email")

    End Sub
    Protected Sub lnkSendCodeUsingSms_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSendCodeUsingSms.Click

        SendCode("sms")

    End Sub
    Protected Sub lnkSendCodeUsingLog_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkSendCodeUsingLog.Click

        SendCode("log")

    End Sub
    Private Sub SendCode(ByVal provider As String)
        SendCode(_mfas.FirstOrDefault(Function(x) x.Provider = provider))
    End Sub

    Private Sub SendCode(mfa As MultiFactorAuthenticationList.MfaInfo)
        txtSelectedProvider.Value = mfa.Provider

        Try
            BusinessIdentity.CurrentIdentity().SendTwoFactorAuthenticationCode(mfa.GetProvider(Settings), mfa.Target)
            BuildForm()
        Catch ex As Exception
            Arco.Utils.Logging.LogError("Error sending mfa code", ex)
            SetMessage("Someting went wrong sending the mfa code")
        End Try

    End Sub
    Protected Sub lnkVerifyCode_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkVerifyCode.Click
        If Not Page.IsValid Then
            Exit Sub
        End If
        Dim enduserInfo As EndUserInfo = UserInfoLoader.GetEndUserInfo(Me)

        Dim identity As BusinessIdentity = BusinessIdentity.CurrentIdentity
        Dim isValid As Boolean
        If txtSelectedProvider.Value <> "otp" Then
            isValid = identity.ValidateTwoFactorAuthenticationCode(txtSelectedProvider.Value, txtCode.Text.Trim(), Settings)

        Else
            Dim mfaInfo As MultiFactorAuthenticationList.MfaInfo = _mfas.First(Function(x) x.Provider = "otp")
            Dim provider As IMFAProvider = mfaInfo.GetProvider(Settings)

            If String.IsNullOrEmpty(mfaInfo.Target) Then
                'no authenticator app linfoinked atm
                'validate code against the register code, when ok registere the code as target
                isValid = provider.ValidateCode(txtCode.Text.Trim(), OtpSecretKey)
                If isValid Then
                    'store the code if is valid for the next time
                    Dim mfa As MultiFactorAuthentication = MultiFactorAuthentication.GetMultiFactorAuthentication(ACL.User.AddDummyDomain(identity.Name), "otp")
                    mfa.Target = OtpSecretKey
                    mfa.Save()
                End If
            Else
                'app already linked, validate against the target
                isValid = provider.ValidateCode(txtCode.Text.Trim(), mfaInfo.Target)
            End If


        End If


        If isValid Then
            If identity.MustDoTwoFactorAuthentication Then
                Arco.Utils.Logging.LogSecurity("Successfull Stage 2 log in for user: " & identity.Name, Arco.Utils.LogLevel.Info)
                identity.MustDoTwoFactorAuthentication = False
                identity.Save()
            End If
            LoginAttempts.Remove(enduserInfo)

            ContinueLogin(txtReturnTo.Value, False)
        Else

            Arco.Utils.Logging.LogSecurity("Invalid Stage 2 MFA code for user :" & identity.Name, Arco.Utils.LogLevel.Warning)

            Dim currentAttempt As LoginAttempts.Attempt = LoginAttempts.Add(enduserInfo, Nothing)

            pnlCaptcha.Visible = LoginAttempts.GetAttemptCount(enduserInfo, Nothing) >= ShowCaptchaAfterAttempts

            txtCode.Text = ""
            txtCode.Focus()

            SetMessage(GetLabel("invalidcode"))
        End If

    End Sub

End Class