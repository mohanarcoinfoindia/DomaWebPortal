Partial Class DefaultRouting
    Inherits BasePage

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

        QueryStringParser.Add("menu", "RoutingMenu")
        QueryStringParser.Add("hidetree", "true")
        QueryStringParser.Add("defaultaction", "MyWork")
        QueryStringParser.Add("hidecurrentfolder", "true")
        QueryStringParser.Add("searchscreenmodes", "4,3,6,2,13")
        Dim redirectTo As String = GetRedirectUrl("~/Main.aspx" & QueryStringParser.ToString)

        Response.Redirect(redirectTo, False)
        Context.ApplicationInstance.CompleteRequest()

    End Sub
End Class
