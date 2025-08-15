Public Class Auth_Logoff
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load


        If Settings.GetValue("Saml", "IsServiceProvider", False) Then
            Response.Redirect("~/Auth/Saml/SSOlogoff.aspx")
            Exit Sub
        End If

        Dim customLogoutUrl As String = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.GetExtendedProperty("LogoutUrl")

        SetDefaultPageTitle()

        Dim authModule As AuthenticationModule = AuthenticationModule.GetModule(Context)
        authModule.Logout(Context)

        If String.IsNullOrEmpty(customLogoutUrl) Then
            customLogoutUrl = Settings.GetValue("Security", "LogoutRedirect", "")
        End If

        If String.IsNullOrEmpty(customLogoutUrl) Then
            authModule.RedirectToLoginPage(Context, True, QueryStringParser.GetUrl("redirect_uri"))
        Else
            Response.Redirect(customLogoutUrl)
        End If
    End Sub

End Class
