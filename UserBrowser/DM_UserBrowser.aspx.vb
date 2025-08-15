Partial Class DM_UserBrowser
    Inherits UserBrowserBasePage


    Protected Sub DM_UserBrowser_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not isAdmin() Then
            radPaneRightContent.ContentUrl = GetRedirectUrl("DM_UserBrowser_Roles.aspx")
        Else
            radPaneRightContent.ContentUrl = GetRedirectUrl("DM_UserBrowser_RightPane.aspx?drpStatus=Valid")
        End If
        radPaneLeftContent.ContentUrl = GetRedirectUrl("DM_UserBrowser_LeftPane.aspx")
    End Sub
End Class
