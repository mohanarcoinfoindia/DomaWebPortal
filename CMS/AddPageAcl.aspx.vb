
Imports Arco.Doma.Library.baseObjects

Partial Class CMS_AddPageAcl
    Inherits BaseAdminOnlyPage

    Private _page As Arco.Doma.CMS.Data.Page

    Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
        If Not String.IsNullOrEmpty(newUser.SubjectID) Then
            If Arco.Doma.Library.Routing.RoutingSecurity.GetRoutingSecurity(_page.ID, "Page", newUser.SubjectID, newUser.SubjectType, Arco.Doma.Library.Routing.RoutingSecurity.LevelAccess.View, 0, False).Object_ID = 0 Then
                Dim sec As Arco.Doma.Library.Routing.RoutingSecurity = Arco.Doma.Library.Routing.RoutingSecurity.NewRoutingSecurity()
                sec.Object_ID = _page.ID
                sec.Object_Type = "Page"
                sec.Subject = newUser.SubjectID
                sec.SubjectType = newUser.SubjectType
                sec.Level = Arco.Doma.Library.Routing.RoutingSecurity.LevelAccess.View
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

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        CheckIsAdmin()
        Dim pageId As Integer = QueryStringParser.GetInt("pageid")


        If pageId <= 0 Then
            GotoErrorPage(LibError.ErrorCode.ERR_UNEXPECTED)
            Return
        End If

        _page = Arco.Doma.CMS.Data.Page.GetPage(pageId)
        If _page Is Nothing Then
            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
            Return
        End If
        If Not _page.CanEdit Then
            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
            Return
        End If

        Translate()

    End Sub
    Private Sub Translate()
        Title = GetLabel("ctx_aclsetacl")
        cmdCancel.Text = GetLabel("cancel")
        cmdSave.Text = GetLabel("add")
    End Sub
End Class
