
Public Class DM_SELECT_PRINT
    Inherits BasePage
    Protected Printseltype As Integer
    Protected PrintXml As String = "null"

    Protected Mode As String
    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load


        Mode = QueryStringParser.GetString("mode")

        Printseltype = QueryStringParser.GetInt("printseltype", 1)

        PrintXml = QueryStringParser.GetString("printxml")
        If Not String.IsNullOrEmpty(PrintXml) Then
            PrintXml = EncodingUtils.EncodeJsString(PrintXml)
        End If
        If Mode = "xls" Then
            Page.Title = GetLabel("exportoptions")
            hplSelection.Text = GetLabel("exportsel")
            hplCurrentPage.Text = GetLabel("exportcur")
            hplAll.Text = GetLabel("exportall")
        Else
            Page.Title = GetLabel("printoptions")
            hplSelection.Text = GetLabel("printsel")
            hplCurrentPage.Text = GetLabel("printcur")
            hplAll.Text = GetLabel("printall")
        End If


        hplSelection.NavigateUrl = "javascript:PrintList(0);"
        hplCurrentPage.NavigateUrl = "javascript:PrintList(1);"
        hplAll.NavigateUrl = "javascript:window.PrintList(2);"

        If QueryStringParser.GetBoolean("HASSEL") Then
            plhSelection.Visible = True
        Else
            plhSelection.Visible = False
        End If

    End Sub

End Class

