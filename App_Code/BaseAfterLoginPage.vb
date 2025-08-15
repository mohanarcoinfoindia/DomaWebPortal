Imports Arco.Doma.Library.Security
Imports Arco.Doma.Library.Settings
Imports Arco.Utils.Web
Imports DotNetOpenAuth.Messaging
Imports DotNetOpenAuth.OAuth2
Imports DotNetOpenAuth.OAuth2.Messages



Public Class BaseAfterLoginPage
    Inherits BasePage


    Protected Sub ContinueLogin(ByVal redirectUri As String, ByVal checkforConsent As Boolean)

        Dim handler As New AfterLoginHandler()
        handler.ContinueLogin(Me, Context, redirectUri, checkforConsent)

    End Sub
End Class

Public Class AfterLoginHandler
    Public Sub ContinueLogin(ByVal page As BasePage, ByVal context As HttpContext, ByVal returnToUrl As String, ByVal checkforConsent As Boolean)

        Dim identity As BusinessIdentity = BusinessIdentity.CurrentIdentity

        If checkforConsent AndAlso New UserConsent.UserConsentServiceFactory().GetUserContentService(page.Settings).GetConsentForm() IsNot Nothing Then

            Dim ident As BusinessIdentity = BusinessIdentity.CurrentIdentity
            ident.MustGiveConsent = True
            ident.Save()

            returnToUrl = "Consent.aspx?redirect_uri=" & page.Server.UrlEncode(returnToUrl)
        ElseIf identity.MustChangePassword Then
            returnToUrl = "ChangePass.aspx?redirect_uri=" & page.Server.UrlEncode(returnToUrl)
        ElseIf identity.MustDoTwoFactorAuthentication Then
            returnToUrl = "MfA.aspx?redirect_uri=" & page.Server.UrlEncode(returnToUrl)
        ElseIf identity.MustSelectTenant Then
            returnToUrl = "selecttenant.aspx?redirect_uri=" & page.Server.UrlEncode(returnToUrl)
        Else
            Dim oauthEndUserRequest = page.Session("oauthrequest")
            If oauthEndUserRequest IsNot Nothing Then
                Arco.Utils.Logging.Debug("AfterLoginHandler:Redirecting for oauthRequest")
                page.Session("oauthrequest") = Nothing
                Dim authServerHost As New Arco.Doma.OpenAuth2.AuthorizationServer(page.Settings)
                Dim authServer As New AuthorizationServer(authServerHost)
                Dim approval As EndUserAuthorizationSuccessResponseBase = authServer.PrepareApproveAuthorizationRequest(oauthEndUserRequest,
                    BusinessIdentity.CurrentIdentity.Name)

                If identity.Tenant.Id <> 0 Then
                    approval.ExtraData.Add("tenantid", identity.Tenant.Id)
                End If

                Dim authResponse As OutgoingWebResponse = authServer.Channel.PrepareResponse(approval)
                authResponse.Respond(New HttpContextWrapper(context))
                Return
            ElseIf Not String.IsNullOrEmpty(returnToUrl) Then

                Arco.Utils.Logging.Debug("AfterLoginHandler: Request to redirect to " & returnToUrl)
                If Not IsLocalUrl(returnToUrl) Then

                    Dim trustedReturnUrl As ReturnUrl = GetTrustedReturnUrl(returnToUrl, page.Settings)

                    If trustedReturnUrl IsNot Nothing Then
                        'remote url, always add otp
                        If trustedReturnUrl.RequiresOtp Then

                            Dim otp As String = OneTimeSessionId.Generate(identity.CacheKey)

                            Arco.Utils.Logging.Debug("Created otp " & otp & " for " & identity.CacheKey)
                            If returnToUrl.Contains("?") Then
                                returnToUrl &= "&otp=" & otp
                            Else
                                returnToUrl &= "?otp=" & otp
                            End If
                        End If

                    Else
                        'not allowed url, default
                        returnToUrl = GetDefaultReturnUrl(identity, page.Settings)
                    End If
                End If


            Else
                Arco.Utils.Logging.Debug("AfterLoginHandler: Redirecting to default local url for the user")
                returnToUrl = GetDefaultReturnUrl(identity, page.Settings)
            End If

        End If

        Arco.Utils.Logging.Debug("AfterLoginHandler: Redirecting to " & returnToUrl)

        page.Response.Redirect(returnToUrl, False)
        context.ApplicationInstance.CompleteRequest()

    End Sub

    Private Function GetDefaultReturnUrl(ByVal identity As BusinessIdentity, ByVal settings As ISettings) As String
        Dim siteVersion As Integer = settings.GetValue("General", "DefaultSiteVersion", 7)

        Dim defaultApp As Arco.Doma.Library.Website.UserApplication = identity.GetDefaultWebApplication(siteVersion)

        If siteVersion = 8 Then
            Dim url As String = defaultApp.Url
            Dim v8Url As String = settings.GetValue("General", "url_v8", "")
            If Not String.IsNullOrEmpty(v8Url) AndAlso Not url.StartsWith(v8Url, StringComparison.CurrentCultureIgnoreCase) Then
                If url.StartsWith("/") Then
                    url = url.Substring(1)
                End If
                url = Arco.IO.Directory.AddForwardSlash(v8Url) & url
            End If
            Return url
        Else
            Return defaultApp.Url
        End If


    End Function


    Private Function IsLocalUrl(ByVal value As String) As Boolean
        Return Url.IsLocalUrl(value)
    End Function
    Private Function GetTrustedReturnUrl(ByVal value As String, ByVal settings As ISettings) As ReturnUrl

        If IsV8Url(value, settings) Then
            Return New ReturnUrl() With {.RequiresOtp = True}
        End If

        Dim oAuthAppList As OpenAuthAppList = OpenAuthAppList.GetApplications()
        For Each app As OpenAuthAppList.AppInfo In OpenAuthAppList.GetApplications()
            'only check apps with explicit allowedcallbacks
            If String.IsNullOrEmpty(app.AllowedCallBacks) Then Continue For

            If New Arco.Doma.OpenAuth2.ClientDescription(app).IsCallbackAllowed(New Uri(value)) Then
                Return New ReturnUrl() With {.RequiresOtp = True} 'always otp for non local apps
            End If
        Next
        Return Nothing

    End Function

    Private Function IsV8Url(ByVal value As String, ByVal settings As ISettings) As Boolean
        Dim v8Url As String = settings.GetValue("General", "url_v8", "")
        If String.IsNullOrEmpty(v8Url) Then Return False

        Return value.StartsWith(v8Url, StringComparison.CurrentCultureIgnoreCase)
    End Function
    'Private Function IsTrustedReturnUrl(ByVal context As HttpContext, ByVal value As String) As Boolean

    '    Return context.Session("AllowRemoteAuthRedirect") IsNot Nothing

    '    Return False
    'End Function

    Private Class ReturnUrl

        Public Property RequiresOtp As Boolean

    End Class
End Class
