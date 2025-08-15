
Partial Class UserControls_PromptText
    Inherits BasePage

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.Title = Request.QueryString("Header") 'GetLabel("date")
        btnCancelUrl.Text = GetLabel("cancel")
        d1.Width = ArcoFormatting.DefaultControlWidth
        d1.Rows = 5
    End Sub

End Class
