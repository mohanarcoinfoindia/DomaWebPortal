
Partial Class DM_AppServer
    Inherits BaseAdminOnlyPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Response.Redirect("DM_About.aspx")
    End Sub
    
End Class
