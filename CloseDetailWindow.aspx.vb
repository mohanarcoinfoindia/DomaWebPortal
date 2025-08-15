
Partial Class CloseDetailWindow
    Inherits BasePage

    Protected _GoToTreeId As Integer

    Public ReadOnly Property PersistSize As Boolean
        Get
            Return Not UserProfile.OpenDetailWindowMaximized
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        _GoToTreeId = QueryStringParser.GetInt("goToTreeId")
    End Sub
End Class
