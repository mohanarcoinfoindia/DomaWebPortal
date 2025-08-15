Imports Telerik.Web.UI

Partial Class DM_USER_SETTINGS
    Inherits BasePage

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'this page is only here for bw compat-> it should be removed
        Response.Redirect("UserBrowser/DM_UserBrowser_AddUser.aspx?fromUP=true")

    End Sub
  
   


End Class
