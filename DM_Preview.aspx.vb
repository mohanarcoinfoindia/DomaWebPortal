
Partial Class DM_Preview
    Inherits BasePage

    Protected msQryParams As String

    Public Sub New()
        AllowGuestAccess = True
    End Sub
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        msQryParams = QueryStringParser.ToString

        If Not QueryStringParser.GetBoolean("hidetabs") Then
            Me.MasterPageFile = "~/masterpages/Preview.master"
        Else
            Me.MasterPageFile = "~/masterpages/DetailSub.master"

        End If
    End Sub

End Class
