Partial Class Main
    Inherits BaseMasterPage


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        InitializePageTitle()

        Dim titleFromQuerystring As String = QueryStringParser.GetString("title")

        If Not String.IsNullOrEmpty(titleFromQuerystring) Then
            Try
                Page.Title &= " : " & StringEncryptionService.Decrypt(titleFromQuerystring)
            Catch ex As Exception

            End Try
        End If

        keepalive.Visible = Settings.GetValue("Security", "WebKeepAlive", True)

        Dim faviconTags As New LiteralControl()
        faviconTags.Text = GenerateFaviconHeadTags(Me.Page)
        Page.Header.Controls.Add(faviconTags)

    End Sub
End Class


