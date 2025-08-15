
Partial Class CaseNotes
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        lblNotes.Case_ID = QueryStringParser.GetInt("CASE_ID")
        lblNotes.DataBind()
    End Sub
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit

        If Not QueryStringParser.GetBoolean("hidetabs") Then
            MasterPageFile = "~/masterpages/Preview.master"
        Else
            MasterPageFile = "~/masterpages/DetailSub.master"
        End If
    End Sub

End Class
