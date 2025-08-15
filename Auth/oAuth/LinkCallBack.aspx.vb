Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Security
Imports Arco.Doma.OpenAuth2
Imports Arco.Doma.OpenAuth2.Clients

Partial Class LinkCallBack
    Inherits BasePage


    Protected Sub RegisterCallBack_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim providers As OpenAuthProviderList = OpenAuthProviderList.GetProviders

        Dim client As IAuthenticationClient = ClientFactory.GetClient(New HttpRequestWrapper(Request), providers)
        If client Is Nothing Then
            Response.Write(Arco.Web.ErrorHandler.GetErrorForm("No provider found"))
            Exit Sub
        End If
        Dim result As DotNetOpenAuth.AspNet.AuthenticationResult

        Try
            result = client.VerifyAuthentication(New HttpContextWrapper(HttpContext.Current))
        Catch ex As InvalidOperationException
            Dim redirectUrl As Uri = New Uri(GetClientUrl() & "Auth/oAuth/linkcallback.aspx?__provider__=" & client.ProviderName)
            result = DirectCast(client, OAuth2Client).VerifyAuthentication(New HttpContextWrapper(HttpContext.Current), redirectUrl)
        End Try

        Dim returnUrl As String = QueryStringParser.GetUrl("redirect_uri")

        If result.IsSuccessful Then
            'add the provider for the current user
            If OpenAuthAccount.CreateOpenAuthAccount(result.Provider, result.ProviderUserId, result.UserName) Then

                If Not Arco.Utils.Web.Url.IsLocalUrl(returnUrl) Then
                    'redirect to the profile
                    returnUrl = "~/UserBrowser/DM_UserBrowser_AddUser.aspx?fromUP=true"
                End If
                Response.Redirect(returnUrl, False)
                Context.ApplicationInstance.CompleteRequest()
            Else
                Response.Write(Arco.Web.ErrorHandler.GetErrorForm("This " & result.Provider & " account is already linked to another user"))
            End If
        Else
            Response.Write(result.Error.Message)
        End If

    End Sub

End Class
