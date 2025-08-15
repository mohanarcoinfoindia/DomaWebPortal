
Partial Class UserControls_PromptDate
    Inherits BasePage

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.Title = GetLabel("date")
        btnCancelUrl.Text = GetLabel("cancel")
        d1.DatePopupButton.ToolTip = GetDecodedLabel("selectdate")
        d1.TimePopupButton.ToolTip = GetDecodedLabel("selecttime")

        d1.DateInput.SelectionOnFocus = Telerik.Web.UI.SelectionOnFocus.None
        d1.DateInput.DateFormat = ArcoFormatting.GetDateTimeFormat(False)

        Dim value As String = QueryStringParser.GetString("d")
        If Not String.IsNullOrEmpty(value) Then
            d1.SelectedDate = Convert.ToDateTime(value)
        End If
        d1.Width = System.Web.UI.WebControls.Unit.Pixel(175)
    End Sub
End Class
