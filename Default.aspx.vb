Partial Class _Default
    Inherits BasePage

    Public Sub New()
        AllowGuestAccess = True
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim MainLink As String

        'get the mainpage from site.xml
        Dim mainPage As String = SiteManagement.SiteLayout.LoadSiteLayout().Mainpage

        'if it's not set or the default value, only then revert to the defaultapplication
        'this means that for the root and default site the fallback is always used
        'for custom sites like AddToPackage the mainpage from the site.xml will still be used
        If String.IsNullOrEmpty(mainPage) OrElse mainPage.Equals("DefaultLib", StringComparison.CurrentCultureIgnoreCase) Then
            mainPage = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity().GetDefaultWebApplication().Url
        End If


        If mainPage.Contains("?") Then
            Dim mainPageQrs As New QueryStringParser(mainPage.Substring(mainPage.IndexOf("?") + 1))
            Dim mainPageUrl As String = mainPage.Substring(0, mainPage.IndexOf("?"))
            For Each key As String In mainPageQrs.Keys
                QueryStringParser.AddOrReplace(key, mainPageQrs(key))
            Next
            MainLink = mainPageUrl & QueryStringParser.ToString()
        Else
            Dim lsLink As String = mainPage
            If Not lsLink.ToLower.EndsWith(".aspx") Then
                lsLink &= ".aspx"
            End If
            MainLink = lsLink & QueryStringParser.ToString
        End If
        MainLink = GetRedirectUrl(MainLink)
        Response.Redirect(MainLink, False)
        Context.ApplicationInstance.CompleteRequest()

    End Sub
End Class


