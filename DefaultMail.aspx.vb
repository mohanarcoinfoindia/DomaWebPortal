Partial Class DefaultMail
    Inherits BasePage


    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

        QueryStringParser.Add("menu", "MailMenu")
        QueryStringParser.Add("hidetree", "true")
        QueryStringParser.Add("defaultaction", "Inbox")
        QueryStringParser.Add("hidecurrentfolder", "true")
        QueryStringParser.Add("searchscreenmodes", "11,8,10,9,7")

        Dim redirectTo As String = GetRedirectUrl("~/Main.aspx" & QueryStringParser.ToString)


        Response.Redirect(redirectTo, False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

End Class
