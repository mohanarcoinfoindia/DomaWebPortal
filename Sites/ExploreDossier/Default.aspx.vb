
Partial Class Sites_Default_Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load        
        Response.Redirect("../../Default.aspx?site=ExploreDossier")
    End Sub
End Class
