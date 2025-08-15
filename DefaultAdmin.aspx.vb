
Partial Class DefaultAdmin
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        QueryStringParser.Add("menu", "AdminMenu")
        QueryStringParser.Add("hidetree", "true")
        QueryStringParser.Add("defaultaction", "AppServer")
        QueryStringParser.Add("hidecurrentfolder", "true")
        QueryStringParser.Add("hideglobalsearch", "true")

        Dim redirectTo As String = GetRedirectUrl("~/Main.aspx" & QueryStringParser.ToString)

        If Request.Url.AbsolutePath.EndsWith(".aspx", StringComparison.CurrentCultureIgnoreCase) Then
            Server.Transfer(redirectTo, True)
        Else
            'handle the defaultadmin.aspx/test.js loop
            Response.Redirect(redirectTo, False)
            Context.ApplicationInstance.CompleteRequest()
        End If

    End Sub
   

End Class
