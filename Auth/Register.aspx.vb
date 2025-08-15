Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Globalisation
Imports Arco.Doma.Library.Security

Partial Class Auth_Register
    Inherits BaseAnonymousPage

    Private _complexityValidator As PasswordComplexityValidator

    Private Const ShowFirstName As Boolean = False
    Private Const ShowLastName As Boolean = False
    Private Const ShowLanguage As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        pnlMsg.Visible = False

        _complexityValidator = PasswordComplexityValidator.GetValidator(Settings, Nothing)

        SetRedirectUri()
        PrefillEmailIfExists()

        Dim activationCode As String = Nothing
        If Not Page.IsPostBack Then
            activationCode = QueryStringParser.GetCode("code")
        End If
        If Not String.IsNullOrEmpty(activationCode) Then
            'handle old link
            ShowActivationCode("")
            txtActivationCode.Text = activationCode

            ActivateUser(activationCode)

        ElseIf pnlReg.Visible Then

            BuildRegistrationForm()
            If Not Settings.GetValue("Security", "EnableUserRegistration", False) Then
                SetErrorMessage("User registration has not been enabled. Set ArcoInfo.Security.EnableUserRegistration to true")
                lnkRegister.Enabled = False
                Exit Sub
            End If

        End If



    End Sub

    Private Sub PrefillEmailIfExists()
        If Not IsPostBack Then
            Dim email As String = QueryStringParser.GetString("username")
            If Not String.IsNullOrEmpty(email) Then
                txtMail.Text = email
                txtMail.Enabled = False
            End If
        End If
    End Sub
    Private Sub SetRedirectUri()
        If Not IsPostBack Then
            txtRedirectUri.Value = QueryStringParser.GetUrl("redirect_uri")
        End If
    End Sub
    Private Sub OnActivateOk()

        Dim lsRedirect As String = txtRedirectUri.Value
        Dim handler As New AfterLoginHandler()
        handler.ContinueLogin(Me, Context, lsRedirect, True)

    End Sub

    Private Sub BuildRegistrationForm()



        pnlFirstName.Visible = ShowFirstName
        If pnlFirstName.Visible Then
            lblFirstName.Text = GetLabel("ub_firstname")
        End If

        pnlLastName.Visible = ShowLastName
        If pnlLastName.Visible Then
            lblLastName.Text = GetLabel("ub_lastname")
        End If

        pnlLanguage.Visible = ShowLanguage
        If pnlLanguage.Visible Then
            lblLang.Text = GetLabel("language")

            If drpLang.Items.Count = 0 Then
                For Each lang As LanguageList.LanguageInfo In LanguageList.GetInterfaceLanguageList()
                    drpLang.Items.Add(New System.Web.UI.WebControls.ListItem(GetDecodedLabel("lang" & lang.InterfaceLanguageCode), lang.InterfaceLanguageCode) With {.Selected = (lang.InterfaceLanguageCode = Language)})
                Next
            End If
        End If

        lblRegisterHeader.Text = GetLabel("register")
        lnkRegister.Text = GetLabel("register")
        lblMail.Text = GetLabel("registerpagemaildesc")
        lblPassword.Text = GetLabel("password")

        Dim ruleCodes As List(Of PasswordComplexityValidator.ResultCode) = _complexityValidator.GetRuleCodes()
        If ruleCodes.Any Then
            Dim rules As New List(Of String)
            For Each ruleCode As PasswordComplexityValidator.ResultCode In ruleCodes
                Select Case ruleCode
                    Case PasswordComplexityValidator.ResultCode.MaxlengthFailed
                        rules.Add(GetDecodedLabel("maxlen").Replace("#FIELDLEN#", _complexityValidator.MaxLength))
                    Case PasswordComplexityValidator.ResultCode.MinlengthFailed
                        rules.Add(GetDecodedLabel("minlen").Replace("#FIELDLEN#", _complexityValidator.MinLength))
                    Case Else
                        rules.Add(GetDecodedLabel(ruleCode.ToString))
                End Select
            Next
            lblPasswordTooltipImg.ToolTip = "- " & String.Join(Environment.NewLine & "- ", rules)
        End If

        lblConfirmPassword.Text = GetLabel("confirmpassword")

        CompareValidator1.ErrorMessage = GetLabel("passwordmatchfailed")

        capt.ErrorMessage = GetLabel("captchamatchfailed")
        capt.CaptchaLinkButtonText = GetDecodedLabel("refresh")
        lblCode.Text = "Code"

    End Sub

    Protected Sub lnkRegister_Click(sender As Object, e As EventArgs) Handles lnkRegister.Click
        lblMsg.Text = ""
        If Not Settings.GetValue("Security", "EnableUserRegistration", False) Then
            SetErrorMessage("User registration has not been enabled. Set ArcoInfo.Security.EnableUserRegistration to true")
            Exit Sub
        End If

        If Not Page.IsValid Then
            SetErrorMessage(GetLabel("validatorerror"))
            Exit Sub
        End If


        Dim results As List(Of PasswordComplexityValidator.ResultCode) = _complexityValidator.Validate(txtPassword.Text)
        If results.Any Then
            For Each result As PasswordComplexityValidator.ResultCode In results
                Select Case result
                    Case PasswordComplexityValidator.ResultCode.MaxlengthFailed
                        SetErrorMessage(GetLabel("password") & " " & GetLabel("maxlen").Replace("#FIELDLEN#", _complexityValidator.MaxLength))
                    Case PasswordComplexityValidator.ResultCode.MinlengthFailed
                        SetErrorMessage(GetLabel("password") & " " & GetLabel("minlen").Replace("#FIELDLEN#", _complexityValidator.MinLength))
                    Case Else
                        SetErrorMessage(GetLabel("password") & ": " & GetLabel(result.ToString))
                End Select
            Next
            Exit Sub
        End If
        Dim u As User = ACL.User.GetUser(txtMail.Text)
        If u IsNot Nothing Then
            SetErrorMessage(GetLabel("useralreadyregistered"))
            Exit Sub
        End If

        For Each otherUser As USERList.USERSInfo In USERList.GetUSERSList(New USERList.Criteria With {
            .EMAIL = txtMail.Text
        })
            SetErrorMessage(GetLabel("emailunique"))
            Exit Sub
        Next

        u = ACL.User.NewUser(txtMail.Text)

        u.USER_DISPLAY_NAME = txtMail.Text

        u.USER_AUTH_METHOD = ACL.User.AuthenticationMethod.Database
        u.USER_STATUS = ACL.User.UserStatus.Blocked
        If ShowLanguage Then
            u.USER_LANGCODE = drpLang.SelectedValue
        End If

        If ShowFirstName Then
                u.USER_FIRSTNAME = txtFirstName.Text
            End If
            If ShowLastName Then
            u.USER_LASTNAME = txtLastName.Text
        End If

        u.USER_MAIL = txtMail.Text
        u.AddToTenant(GetTenantIdFromUrl())
        u.GenerateActivationCode = True
        u.StartUserHasRegisteredProcedure = True
        u.SetPassword(txtPassword.Text, "")
        u = u.Save

        Try
            Dim activationUrl As String = Arco.IO.Directory.AddForwardSlash(GetClientUrl) & "Auth/Register.aspx?code={0}&redirect_uri=" & Page.Server.UrlEncode(txtRedirectUri.Value)
            u.SendActivationMail(Settings, activationUrl)

            Language = drpLang.SelectedValue

            ShowRegisterOk(u.USER_LOGIN)
        Catch ex As Exception
            'delete the user if we can't send the activation mail
            ACL.User.DeleteUser(txtMail.Text)
            Arco.Utils.Logging.LogError("Error in Register.aspx, lnkReguister_Click", ex)
            SetErrorMessage(ex.Message)
        End Try


    End Sub
    Protected Sub lnActivate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkActivate.Click
        If Not Page.IsValid Then
            Exit Sub
        End If
        Dim activationCode As String = txtActivationCode.Text
        ActivateUser(activationCode)

    End Sub

    Public Sub ActivateUser(ByVal activationCode As String)


        Dim u As User = ACL.User.GetUserByActivationCode(activationCode)

        If u Is Nothing Then
            SetActivationErrorMessage(GetLabel("invalidactivationcode"))
            Return
        End If
        If Not String.IsNullOrEmpty(lblUserNameForActivation.Text) AndAlso lblUserNameForActivation.Text <> ArcoFormatting.FormatUserName(u.USER_LOGIN) Then
            SetActivationErrorMessage(GetLabel("invalidactivationcode"))
            Return
        End If
        If u.USER_STATUS <> ACL.User.UserStatus.Blocked Then
            SetActivationErrorMessage(GetLabel("useralreadyactivated"))
            Return
        End If

        u = u.Activate(activationCode)

        If Not u.PasswordExpired Then
            Dim enduserInfo As EndUserInfo = UserInfoLoader.GetEndUserInfo(Me)
            BusinessPrincipal.Login(u.USER_LOGIN, CacheToken.GetCacheToken(Session.SessionID), Arco.Licencing.Modules.WEB, enduserInfo)
            Dim ident As BusinessIdentity = BusinessIdentity.CurrentIdentity
            If ident.IsAuthenticated Then
                LoginAttempts.Remove(enduserInfo)
                OnActivateOk()

                'If New UserConsent.UserConsentServiceFactory().GetUserContentService(Settings).GetConsentForm() IsNot Nothing Then
                '    ident.MustGiveConsent = True
                '    ident.Save()
                'End If

                ''this should never be true
                'If ident.MustChangedPassword Then
                '    Response.Redirect("~/Auth/Changepass.aspx")
                '    Exit Sub
                'Else
                '    ShowActivatedOk(BusinessIdentity.CurrentIdentity.Language)

                '    'Response.Redirect("~/Default.aspx")
                'End If
            Else
                SetActivationErrorMessage("Unable to log in the activated user")
            End If
        Else
            u.GeneratePasswordResetCode = True
            u = u.Save

            Response.Redirect("~/Auth/Resetpass.aspx?code=" & u.PasswordResetCode)

        End If

    End Sub
    Private Sub SetActivationErrorMessage(ByVal msg As String)
        Dim enduserInfo As EndUserInfo = UserInfoLoader.GetEndUserInfo(Me)

        Dim currentAttempt As LoginAttempts.Attempt = LoginAttempts.Add(enduserInfo, Nothing)
        'show the captcha after the 3rd attempt
        captAuthCode.Visible = LoginAttempts.ShowCaptcha(enduserInfo, Nothing, 3)

        SetErrorMessage(msg)
    End Sub
    Private Sub SetErrorMessage(ByVal vsMsg As String)
        lblMsg.CssClass = "ErrorLabel"
        If Not String.IsNullOrEmpty(lblMsg.Text) Then
            lblMsg.Text &= "<br/>" & vsMsg
        Else
            lblMsg.Text = vsMsg
        End If

        pnlMsg.Visible = True
    End Sub

    Private Sub ShowRegisterOk(ByVal userName As String)

        lblRegOk.Text = GetLabel("userregistersucceeded")

        ShowActivationCode(userName)
    End Sub

    Private Sub ShowActivationCode(ByVal userName As String)

        pnlReg.Visible = False
        pnlActivate.Visible = True
        lblActivationCode.Text = "Code"
        lnkActivate.Text = GetLabel("activate")
        If Not String.IsNullOrEmpty(userName) Then
            lblUserNameForActivation.Text = ArcoFormatting.FormatUserName(userName)
        End If
        Dim enduserInfo As EndUserInfo = UserInfoLoader.GetEndUserInfo(Me)


        captAuthCode.ErrorMessage = GetLabel("captchamatchfailed")
        captAuthCode.CaptchaLinkButtonText = GetDecodedLabel("refresh")
        captAuthCode.Visible = captAuthCode.Visible OrElse LoginAttempts.ShowCaptcha(enduserInfo, Nothing, 3)

    End Sub

End Class
