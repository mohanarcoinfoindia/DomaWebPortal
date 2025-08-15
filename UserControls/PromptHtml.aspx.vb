
Partial Class UserControls_PromptText
    Inherits BasePage

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.Title = Request.QueryString("Header") 'GetLabel("date")
        btnCancelUrl.Text = GetLabel("cancel")
    End Sub

End Class
