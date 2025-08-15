
Partial Class UserBrowser_EditClient
    Inherits BaseAdminOnlyPage

    Private Property AppId As String
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'If Not Settings.GetValue("Security", "EnableIdentityProvider", False) Then
        '    GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        '    Exit Sub
        'End If

        lblName.Text = GetLabel("name")
        lblCallBacks.Text = GetLabel("url") & "(s)"
        btnOk.Text = GetLabel("save")
        btnCancelUrl.Text = GetLabel("cancel")

        AppId = QueryStringParser.GetString("ID")

        If Not Page.IsPostBack Then


            If Not String.IsNullOrEmpty(AppId) Then
                'Title = GetDecodedLabel("edit")
                ShowClient(Arco.Doma.Library.Security.OpenAuthApp.GetApplication(AppId))
            Else
                Title = GetDecodedLabel("new")
            End If

        End If
    End Sub

    Private Sub ShowClient(ByVal client As Arco.Doma.Library.Security.OpenAuthApp)
        Title = client.Name
        txtName.Text = client.Name
        txtAllowedCallBacks.Text = client.AllowedCallBacks
        lblClientID.Text = client.ClientId
        lblClientSecret.Text = client.ClientSecret
        rowClientID.Visible = True
        rowClientSecret.Visible = True
    End Sub

    Protected Sub btnOk_Click(sender As Object, e As System.EventArgs) Handles btnOk.Click
        If Not Page.IsValid Then
            Exit Sub
        End If
        Dim client As Arco.Doma.Library.Security.OpenAuthApp
        If Not String.IsNullOrEmpty(AppId) Then
            client = Arco.Doma.Library.Security.OpenAuthApp.GetApplication(AppId)
        Else
            client = Arco.Doma.Library.Security.OpenAuthApp.NewApplication()
        End If
        client.Name = txtName.Text
        client.AllowedCallBacks = txtAllowedCallBacks.Text

        If client.IsNew Then
            client = client.Save()

            Response.Redirect("EditClient.aspx?ID=" & client.ClientId)

        Else
            client.Save()

            Page.ClientScript.RegisterStartupScript(Page.ClientScript.[GetType](), "onLoad", "GetRadWindow().close();", True)
        End If

    End Sub
End Class
