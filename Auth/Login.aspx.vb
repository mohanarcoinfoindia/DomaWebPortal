
Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Security

Imports DotNetOpenAuth.OAuth2
Imports DotNetOpenAuth.OAuth2.Messages

Partial Class Auth_Login
    Inherits BaseAnonymousPage

    Protected MixedLogin As Boolean

    Private _allowLogin As Boolean = True
    Private _cookiesAreDisabled As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ClearMessage()
        SetDefaultPageTitle()
        SetLabels()
        ReadSettings()
        SetRedirectUri()

        Dim enduserInfo As EndUserInfo = UserInfoLoader.GetEndUserInfo(Me)

        pnlCaptcha.Visible = pnlCaptcha.Visible OrElse LoginAttempts.ShowCaptcha(enduserInfo, txtUsername.Text, Settings.GetValue("Security", "MaxLoginAttempts", 0))

        If Not IsPostBack Then

            SetSessionCookieValidator(Session.SessionID)
            Dim errorCode As Int32 = QueryStringParser.GetInt("errorcode")
            Select Case errorCode
                Case 1
                    SetMessage(GetLabel("invalidusername"))
                Case 2
                    SetMessage(GetLabel("accountdisabled"))
                Case 3
                    SetMessage("An unexpected error has occurred")
            End Select


            Dim authServer As New AuthorizationServer(New Arco.Doma.OpenAuth2.AuthorizationServer)
            Dim oauthEnduserRequest As EndUserAuthorizationRequest = authServer.ReadAuthorizationRequest(New HttpRequestWrapper(Request))

            Session("oauthrequest") = oauthEnduserRequest

            Dim isV8Login As Boolean = IsV8Url(txtRedirectUri.Value)
            If oauthEnduserRequest IsNot Nothing OrElse isV8Login Then

                'see if we're already authenticated 
                Dim authModule As AuthenticationModule = AuthenticationModule.GetModule(Context)
                If authModule.TryLogin(Context) Then
                    OnLoginOk()
                    Return
                End If

            End If

            'auto use remote idps if needed
            If isV8Login Then
                If Settings.GetValue("Saml", "IsServiceProvider", False, GetTenantFromUrl().Name) Then

                    RedirectToUrl(Context, "~/Auth/Saml/SSOClient.aspx?action=login&redirect_uri=" & txtRedirectUri.Value)
                    Return
                End If

                For Each Provider As OpenAuthProviderList.ProviderInfo In OpenAuthProviderList.GetProviders()
                    If Provider.IsDefault AndAlso Provider.Enabled Then
                        Dim client As Arco.Doma.OpenAuth2.IAuthenticationClient = Arco.Doma.OpenAuth2.ClientFactory.GetClient(Provider)

                        Dim callbackUrl As New Uri(GetClientUrl() & "Auth/oAuth/Logincallback.aspx?__provider__=" & client.ProviderName & "&redirect_uri=" + txtRedirectUri.Value)

                        client.RequestAuthentication(New HttpContextWrapper(Context), callbackUrl)
                        Return
                    End If
                Next
            End If

        Else
            If Not AreSessionCookiesEnabled() Then
                _cookiesAreDisabled = True
                SetMessage("This application requires cookies to work")
            End If
        End If

        'check if we can do local login
        If Settings.GetValue("Saml", "IsServiceProvider", False, GetTenantFromUrl().Name) OrElse OpenAuthProviderList.HasDefaultAndEnabledProvider() Then
            _allowLogin = False
            SetMessage("This application doesn't allow local login")
        End If


        LoadOAuthButtons()
        LoadDomains()

        PrefillEmailIfExists()

        txtUsername.Focus()

        If Not _allowLogin Then
            lnkLogin.Visible = False
            lblUserName.Visible = False
            txtUsername.Visible = False
            lblPassword.Visible = False
            txtPassword.Visible = False
            lblLoginHeader.Visible = False
            pnlDomain.Visible = False
            pnlExtra.Visible = False

        End If

    End Sub

    Private Function IsV8Url(ByVal value As String) As Boolean
        If String.IsNullOrEmpty(value) Then Return False

        Dim v8Url As String = Settings.GetValue("General", "url_v8", "")
        If String.IsNullOrEmpty(v8Url) Then Return False

        Return value.StartsWith(v8Url, StringComparison.CurrentCultureIgnoreCase)
    End Function

    Private Sub SetSessionCookieValidator(ByVal sessionId As String)
        cookievalidator.Value = Arco.Utils.PasswordStorage.CreateHash(sessionId)
        'txtSession.Value = sessionid
    End Sub
    Private Function AreSessionCookiesEnabled() As Boolean
        Return Arco.Utils.PasswordStorage.VerifyPassword(Session.SessionID, cookievalidator.Value)
        '  Return txtSession.Value = Session.SessionID
    End Function

    Private Sub PrefillEmailIfExists()
        If Not IsPostBack Then
            txtUsername.Text = QueryStringParser.GetString("username")
        End If
    End Sub
    Private Sub SetRedirectUri()
        If Not IsPostBack Then
            txtRedirectUri.Value = QueryStringParser.GetUrl("redirect_uri")
        End If
        If lnkRegister.Visible Then
            lnkRegister.NavigateUrl = "Register.aspx?redirect_uri=" & HttpUtility.UrlEncode(txtRedirectUri.Value)
        End If
        lnkForgotPass.NavigateUrl = "ForgotPass.aspx?redirect_uri=" & HttpUtility.UrlEncode(txtRedirectUri.Value)
    End Sub

    Private Sub LoadDomains()
        MixedLogin = True

        pnlDomain.Visible = MixedLogin

        If MixedLogin Then
            If Not IsPostBack Then
                cmbDomain.Items.Add(New WebControls.ListItem(""))
                For Each loDom As DOMAINList.DOMAINSInfo In DOMAINList.GetDOMAINSList
                    cmbDomain.Items.Add(New WebControls.ListItem(loDom.DOMAIN_NAME))
                Next
            End If
            If cmbDomain.Items.Count = 1 Then
                'only internal _database domain
                pnlDomain.Visible = False
            End If

        End If
    End Sub

    Private Sub SetMessage(ByVal vsMsg As String)

        pnlMsg.Visible = True
        lblMsg.Text = lblMsg.Text & vsMsg
    End Sub
    Private Sub ClearMessage()
        pnlMsg.Visible = False
        lblMsg.Text = ""
    End Sub

    Private Sub ReadSettings()
        MixedLogin = Settings.GetValue("Interface", "MixedLogin", False)
        'Dim fieldWidth As Unit = Unit.Pixel(400)


        txtUsername.CssClass += " authInput"
        txtPassword.CssClass += " authInput"
        cmbDomain.CssClass += " authInput"
        lnkLogin.CssClass += " authInput"
        pnlExtra.CssClass += " authInput"
        capt.CssClass += " authInput"

        lnkRegister.Visible = Settings.GetValue("Security", "EnableUserRegistration", False)
        lnkForgotUserName.Visible = Settings.GetValue("Security", "ShowForgotUserName", False)
        InitPersistCheckbox()

    End Sub

    Private Sub InitPersistCheckbox()
        If Settings.GetValue("Security", "RememberMe", True) Then
            chkPersist.Visible = True
            If Not IsPostBack Then
                chkPersist.Checked = Settings.GetValue("Security", "RememberMeDefault", False)
            End If
        Else
            chkPersist.Visible = False
            chkPersist.Checked = False
        End If
    End Sub
    Private Sub SetLabels()
        langListE.Visible = True
        langListF.Visible = True
        langListN.Visible = True
        Select Case Language
            Case "N"
                langListN.Visible = False
            Case "F"
                langListF.Visible = False
            Case "E"
                langListE.Visible = False
        End Select
        lblLoginHeader.Text = GetLabel("login")
        lblUserName.Text = GetLabel("username")
        lblPassword.Text = GetLabel("password")
        lblDomain.Text = GetLabel("domain")
        lnkLogin.Text = GetLabel("login")
        chkPersist.Text = GetLabel("rememberme")
        lnkRegister.Text = GetLabel("register")
        lnkForgotPass.Text = GetLabel("forgotpassword")
        lnkForgotUserName.Text = GetLabel("forgotusername")


        btnLangE.ToolTip = GetDecodedLabel("lange")
        btnLangF.ToolTip = GetDecodedLabel("langf")
        btnLangN.ToolTip = GetDecodedLabel("langn")
        btnLangEc.ToolTip = GetDecodedLabel("lange")
        btnLangFc.ToolTip = GetDecodedLabel("langf")
        btnLangNc.ToolTip = GetDecodedLabel("langn")

        capt.ErrorMessage = GetLabel("captchamatchfailed")
        capt.CaptchaLinkButtonText = GetDecodedLabel("refresh")
    End Sub

    Private Sub SetPersistCookie()
        Const cookieName As String = "rememberme"
        If chkPersist.Checked Then
            Dim token As AuthorizationToken = AuthorizationToken.Create(Settings.GetValue("Security", "RememberMeDuration", 30))

            Response.Cookies(cookieName).Expires = token.Expires
            Response.Cookies(cookieName).Value = token.Value
            Response.Cookies(cookieName).Path = If(Not String.IsNullOrEmpty(Request.ApplicationPath), Request.ApplicationPath, "/")

        Else
            Response.Cookies(cookieName).Expires = Now.AddDays(-1)
        End If

    End Sub
    Private Sub CheckLogin(ByVal vsUserName As String)
        Dim enduserInfo As EndUserInfo = UserInfoLoader.GetEndUserInfo(Me)

        If BusinessIdentity.CurrentIdentity.IsAuthenticated Then
            LoginAttempts.Remove(enduserInfo)
            OnLoginOk()
        ElseIf BusinessIdentity.CurrentIdentity.IsBlocked Then
            SetMessage(GetLabel("accountdisabled"))
        Else
            Dim liMaxRetries As Int32 = Settings.GetValue("Security", "MaxLoginAttempts", 0)
            Dim currentAttempt As LoginAttempts.Attempt = LoginAttempts.Add(enduserInfo, vsUserName)
            'show the captcha after the 3rd attempt
            pnlCaptcha.Visible = LoginAttempts.ShowCaptcha(enduserInfo, vsUserName, liMaxRetries)

            'if maxloginattempts is set, temp lock for 30 minutes

            If liMaxRetries > 0 AndAlso currentAttempt.Count >= liMaxRetries Then
                Dim u As User = ACL.User.GetUser(vsUserName)
                If u IsNot Nothing AndAlso u.USER_AUTH_METHOD = ACL.User.AuthenticationMethod.Database Then
                    u.TempBlock(Arco.SystemClock.Now.AddMinutes(10))
                    SetMessage(GetLabel("accountdisabled"))
                    Exit Sub
                End If
            End If

            SetMessage(GetLabel("invalidusername"))
        End If
    End Sub

    Private Sub OnLoginOk()

        '  Dim oauthEnduserRequest As EndUserAuthorizationRequest = Session("oauthrequest")
        SetPersistCookie()

        Dim lsRedirect As String = txtRedirectUri.Value


        Dim handler As New AfterLoginHandler()
        handler.ContinueLogin(Me, Context, lsRedirect, True)

    End Sub


    Protected Sub lnkLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkLogin.Click
        'perform login
        If Not IsValid OrElse _cookiesAreDisabled OrElse Not _allowLogin Then
            Exit Sub
        End If

        Dim lsUsername As String = GetUserName()

        Dim oAuthReq As Object = Session("oauthrequest")

        Dim sessionId As String
        If oAuthReq Is Nothing Then
            sessionId = CreateSessionID()
        Else
            'don't switch session id in case of IDP authentication
            sessionId = Session.SessionID
            'no need to set it again
            ' Session("oauthrequest") = oAuthReq
        End If

        SetSessionCookieValidator(sessionId)

        Dim cacheToken As CacheToken = CacheToken.GetCacheToken(sessionId)
        BusinessPrincipal.Login(lsUsername,
                                                           txtPassword.Text,
                                                           cacheToken,
                                                           Arco.Licencing.Modules.WEB,
                                                           UserInfoLoader.GetEndUserInfo(Me),
                                                           GetTenantIdFromUrl)

        CheckLogin(lsUsername)

    End Sub

    Private Function GetUserName() As String

        If txtUsername.Text.Contains("\") Then
            Return ACL.User.RemoveDummyDomain(txtUsername.Text)
        End If
        Dim lsDomain As String
        If MixedLogin Then
            lsDomain = cmbDomain.SelectedValue
            If String.IsNullOrEmpty(lsDomain) Then
                Return txtUsername.Text
            Else
                Return lsDomain & "\" & txtUsername.Text
            End If
        Else
            Return txtUsername.Text
        End If

    End Function

    Private Sub RedirectToUrl(ByVal ctx As HttpContext, ByVal url As String)
        ctx.Response.Redirect(url, False)
        ctx.ApplicationInstance.CompleteRequest()
    End Sub

#Region " Language "
    Protected Sub btnLangE_Click(sender As Object, e As ImageClickEventArgs) Handles btnLangE.Click
        SetLang("E")
    End Sub

    Protected Sub btnLangF_Click(sender As Object, e As ImageClickEventArgs) Handles btnLangF.Click
        SetLang("F")
    End Sub

    Protected Sub btnLangN_Click(sender As Object, e As ImageClickEventArgs) Handles btnLangN.Click
        SetLang("N")
    End Sub

    Protected Sub btnLangEc_Click(sender As Object, e As EventArgs) Handles btnLangEc.Click
        SetLang("E")
    End Sub

    Protected Sub btnLangFc_Click(sender As Object, e As EventArgs) Handles btnLangFc.Click
        SetLang("F")
    End Sub

    Protected Sub btnLangNc_Click(sender As Object, e As EventArgs) Handles btnLangNc.Click
        SetLang("N")
    End Sub

    Private Sub SetLang(ByVal vsLang As String)
        If Not _cookiesAreDisabled Then
            Language = vsLang
            SetLabels()
            LoadOAuthButtons()
        End If
    End Sub
#End Region

#Region " oAuth "
    Private Sub LoadOAuthButtons()
        If Session("oauthrequest") IsNot Nothing Then
            'no oauth when we're in an oauth
            'when supported we need to take into account links to itself
            pnlOAuth.Visible = False
            Exit Sub
        End If

        Dim providers As OpenAuthProviderList = OpenAuthProviderList.GetProviders

        If providers.Any Then
            pnlOAuth.Visible = True
            pnlOAuthButtons.Controls.Clear()
            Dim isFirstButton As Boolean = True
            For Each provider As OpenAuthProviderList.ProviderInfo In providers
                If Not isFirstButton Then
                    pnlOAuthButtons.Controls.Add(New LiteralControl("&nbsp;"))
                Else
                    isFirstButton = False
                End If
                pnlOAuthButtons.Controls.Add(GetOAuthLoginButton(provider))
            Next
        Else
            pnlOAuth.Visible = False
        End If
    End Sub

    Private Function GetOAuthLoginButton(ByVal provider As OpenAuthProviderList.ProviderInfo) As ImageButton
        Dim description As String
        If Not String.IsNullOrEmpty(provider.Description) Then
            description = provider.Description
        Else
            description = provider.Name
        End If
        Dim button As New ImageButton
        button.ToolTip = GetLabel("loginusing") & " " & description
        button.CommandArgument = provider.Name
        button.ImageUrl = provider.ImageUrl
        AddHandler button.Click, AddressOf btnOAuth_Click
        Return button
    End Function
    Protected Sub btnOAuth_Click(sender As Object, e As EventArgs)
        Dim button As ImageButton = DirectCast(sender, ImageButton)
        Dim client As DotNetOpenAuth.AspNet.IAuthenticationClient = Arco.Doma.OpenAuth2.ClientFactory.GetClient(button.CommandArgument)

        Dim url As Uri = New Uri(GetClientUrl() & "Auth/oAuth/Logincallback.aspx?__provider__=" & client.ProviderName)


        client.RequestAuthentication(New HttpContextWrapper(HttpContext.Current), url)
    End Sub

#End Region

End Class
