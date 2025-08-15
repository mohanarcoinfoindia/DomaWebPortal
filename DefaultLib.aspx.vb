Partial Class DefaultLib
    Inherits BasePage


    Public Sub New()
        AllowGuestAccess = True
    End Sub
    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)

        QueryStringParser.Add("menu", "LibraryMenu")

        Dim redirectTo As String = GetRedirectUrl("~/Main.aspx" & QueryStringParser.ToString)

        Response.Redirect(redirectTo, False)
        Context.ApplicationInstance.CompleteRequest()

    End Sub


End Class
