
Partial Class CloseModalWindow
    Inherits BaseAnonymousPage

    Protected _GoToTreeId As Integer

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        _GoToTreeId = QueryStringParser.GetInt("goToTreeId")
    End Sub

End Class
