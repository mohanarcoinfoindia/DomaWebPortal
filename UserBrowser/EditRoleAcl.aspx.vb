
Partial Class UserBrowser_EditRoleAcl
    Inherits UserBrowserBasePage

    Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
        If Not String.IsNullOrEmpty(newUser.SubjectID) Then
            If Arco.Doma.Library.Routing.RoutingSecurity.GetRoutingSecurity(mlRoleID, "Role", newUser.SubjectID, newUser.SubjectType, Arco.Doma.Library.Routing.RoutingSecurity.LevelAccess.Edit, 0, False).Object_ID = 0 Then
                Dim sec As Arco.Doma.Library.Routing.RoutingSecurity = Arco.Doma.Library.Routing.RoutingSecurity.NewRoutingSecurity()
                sec.Object_ID = mlRoleID
                sec.Object_Type = "Role"
                sec.Subject = newUser.SubjectID
                sec.SubjectType = newUser.SubjectType
                sec.Level = Arco.Doma.Library.Routing.RoutingSecurity.LevelAccess.Edit
                sec.Save()
            End If

            newUser.Subject = ""
            Close()
        End If

    End Sub
    Private Sub RefreshCaller()
        Dim sb As New StringBuilder
        sb.AppendLine("parent.Refresh();")
        Page.ClientScript.RegisterStartupScript(GetType(String), "closescript", sb.ToString, True)
    End Sub
    Private Sub Close()
        Dim sb As New StringBuilder
        sb.AppendLine("window.opener.Refresh();")
        sb.AppendLine("Close();")

        Page.ClientScript.RegisterStartupScript(GetType(String), "closescript", sb.ToString, True)
    End Sub
    Private mlRoleID As Integer
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        CheckIsAdmin()
        mlRoleID = QueryStringParser.GetInt("role_id")
        If mlRoleID = 0 Then
            Throw New ArgumentException("role_id not supplied")
        End If

        Translate()

    End Sub
    Private Sub Translate()
        Title = GetLabel("ctx_aclsetacl")
        cmdCancel.Text = GetLabel("cancel")
        cmdSave.Text = GetLabel("add")
    End Sub

End Class
