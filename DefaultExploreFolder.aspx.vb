Partial Class DefaultExploreFolder
    Inherits BasePage


    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

        QueryStringParser.Add("menu", "ExploreFolderMenu")
        QueryStringParser.Add("hideheader", "true")
        Dim redirectTo As String = GetRedirectUrl("~/Main.aspx" & QueryStringParser.ToString)


        Server.Transfer(redirectTo)


    End Sub



End Class
