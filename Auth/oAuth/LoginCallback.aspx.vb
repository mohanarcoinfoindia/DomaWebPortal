Imports Arco.Doma.Library.Security
Imports Arco.Doma.Library
Imports Arco.Doma.OpenAuth2
Imports Arco.Doma.OpenAuth2.Clients

Partial Class LoginCallback
    Inherits BaseAnonymousPage

    Protected Sub Callback_Load(sender As Object, e As EventArgs) Handles Me.Load

        Dim httpRequest As New HttpRequestWrapper(Request)

        Dim provider As OpenAuthProviderList.ProviderInfo = ClientFactory.GetProvider(httpRequest, OpenAuthProviderList.GetProviders)
        If provider Is Nothing Then
            Arco.Utils.Logging.Debug("oAuth.LoginCallback: couldn't get the provider from querystring: " & Request.Url.Query)
            AccessDenied(3)
            Exit Sub
        End If

        Arco.Utils.Logging.Debug("oAuth.LoginCallback:Provider.Name: " & provider.Name)
        Arco.Utils.Logging.Debug("oAuth.LoginCallback:Provider.AutoRegisterMode: " & provider.AutoRegisterMode)

        Dim client As IAuthenticationClient = ClientFactory.GetClient(provider)
        Dim result As DotNetOpenAuth.AspNet.AuthenticationResult

        Try
            result = client.VerifyAuthentication(New HttpContextWrapper(HttpContext.Current))
        Catch ex As InvalidOperationException
            'match the url exactly with the one in the login page
            Dim redirectUrl As Uri = New Uri(GetClientUrl() & "Auth/oAuth/Logincallback.aspx?__provider__=" & client.ProviderName)
            result = DirectCast(client, OAuth2Client).VerifyAuthentication(New HttpContextWrapper(HttpContext.Current), redirectUrl)
        End Try


        If result.IsSuccessful Then

            Dim mapper As IClaimsMapper = client.GetClaimsMapper()

            'insert or link the user record
            If RegisterUser(mapper, result, provider.AutoRegisterMode) Then

                'don't siwtch session id, regardless of the EnableSessionFixationProtection parameter, so don't use CreateSessionID method
                Dim sessionId As String = Session.SessionID

                BusinessPrincipal.OpenAuthLogin(result.Provider, result.ProviderUserId, CacheToken.GetCacheToken(sessionId), Arco.Licencing.Modules.WEB, UserInfoLoader.GetEndUserInfo(Me), GetTenantId(mapper, result))

                Dim ident As BusinessIdentity = BusinessIdentity.CurrentIdentity

                If ident.IsAuthenticated Then

                    mapper.MapToIdentity(result, ident)

                    AccessOk(ident, client.GetReturnToUrl(httpRequest))


                ElseIf ident.IsBlocked Then
                    AccessDenied(2)
                Else
                    AccessDenied(5)
                End If
            Else
                AccessDenied(4)
            End If
        Else
            AccessDenied(1)
        End If
    End Sub

    Private Sub AccessOk(ident As BusinessIdentity, ByVal returnUrl As String)

        If New UserConsent.UserConsentServiceFactory().GetUserContentService(Settings).GetConsentForm() IsNot Nothing Then
            ident.MustGiveConsent = True
            ident.Save()

            returnUrl = "~/Auth/Consent.aspx?redirect_uri=" & Server.UrlEncode(returnUrl)
        Else
            If Not Arco.Utils.Web.Url.IsLocalUrl(returnUrl) Then
                Arco.Utils.Logging.Debug("oAuth.LoginCallback: AccessOk, non local return url : " & returnUrl)
                returnUrl = ident.GetDefaultWebApplication().Url
            End If
        End If

        Arco.Utils.Logging.Debug("oAuth.LoginCallback: AccessOk, redirecting to : " & returnUrl)

        Response.Redirect(returnUrl, False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    Public Function GetTenantId(ByVal mapper As IClaimsMapper, ByVal result As DotNetOpenAuth.AspNet.AuthenticationResult) As Integer
        Dim tenantIdFromIdp As Integer = mapper.GetTenantId(result)

        If tenantIdFromIdp = 0 Then
            Return GetTenantIdFromUrl()
        End If

        'Dim tenantIdFromUrl As Integer = GetTenantIdFromUrl()
        'If tenantIdFromUrl = 0 OrElse tenantIdFromIdp = tenantIdFromUrl Then
        '    Return tenantIdFromIdp
        'End If

        'what to do here? idp tenant <> url tenant
        'return idp tenant or throw exception?
        Return tenantIdFromIdp

    End Function

    Private Sub AccessDenied(ByVal code As Int32)
        Arco.Utils.Logging.LogSecurity("Access Denied with code " & code)


        GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        ' Response.Redirect("~/Auth/Login.aspx?errorcode=" & code)
    End Sub

    Private Function RegisterUser(ByVal mapper As IClaimsMapper, ByVal result As DotNetOpenAuth.AspNet.AuthenticationResult, ByVal mode As OAuthAutoRegisterMode) As Boolean

        Using logger As New Arco.Utils.BlockLogger("OAuth/LoginCallBack.aspx Registeruser " & mode)


            Dim userLogin As String = mapper.GetUserLogin(result)

            logger.WriteLog("getuser " & userLogin)

            Dim u As ACL.User = ACL.User.GetUser(userLogin)
            Dim isNew As Boolean = False
            If u Is Nothing Then

                If mode = OAuthAutoRegisterMode.Disabled Then
                    logger.WriteLog("Registering users locally is disabled")
                    Return False
                End If

                u = ACL.User.NewUser(userLogin)
                u.USER_DESC = "User registered using " & result.Provider
                u.USER_AUTH_METHOD = ACL.User.AuthenticationMethod.External
                mapper.MapToUser(result, u)

                u = u.Save

                isNew = True


                'add the provider for the user
                ACL.OpenAuthAccount.CreateOpenAuthAccount(u.USER_ACCOUNT, result.Provider, result.ProviderUserId, result.UserName)
            Else
                'existing user
                'don't allow auto adding links in mode Create (we need linkcallback for that)

                If Not ACL.OpenAuthAccount.Exists(result.Provider, result.ProviderUserId, u.USER_ACCOUNT) Then
                    If mode <> OAuthAutoRegisterMode.Full Then
                        logger.WriteLog("OAuth links are not created in Create only mode")
                        Return False
                    End If

                    ACL.OpenAuthAccount.CreateOpenAuthAccount(u.USER_ACCOUNT, result.Provider, result.ProviderUserId, result.UserName)
                End If



                'update fields?
                mapper.MapToUser(result, u)

                u = u.Save
            End If
        End Using


        Return True
    End Function


End Class
