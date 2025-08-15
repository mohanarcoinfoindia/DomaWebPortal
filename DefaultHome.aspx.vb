
Partial Class DefaultHome
    Inherits BasePage

    Public Sub New()
        AllowGuestAccess = True
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        QueryStringParser.Add("menu", "HomeMenu")
        QueryStringParser.Add("hidetree", "true")
        '  QueryStringParser.Add("defaultaction", "ShowGlobalSearch")
        QueryStringParser.Add("hidecurrentfolder", "true")
        QueryStringParser.Add("hideglobalsearch", "true")
        Server.Transfer(GetRedirectUrl("Main.aspx" & QueryStringParser.ToString))
    End Sub
End Class
