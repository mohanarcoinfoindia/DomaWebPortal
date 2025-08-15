Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Globalisation

Partial Class UserBrowser_EditRoleLabel
    Inherits UserBrowserBasePage
    Private _roleID As Int32
    Private _labelObjectType As String

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not isAdmin() Then
            GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
            Exit Sub
        End If

        lblE.Text = GetLabel("lange")
        lblF.Text = GetLabel("langf")
        lblN.Text = GetLabel("langn")
        lblG.Text = GetLabel("langg")
        Title = "Label"
        btnOk.Text = GetLabel("save")
        btnCancelUrl.Text = GetLabel("cancel")

        _roleID = QueryStringParser.GetInt("ROLE_ID")
        _labelObjectType = QueryStringParser.GetString("type")
        If String.IsNullOrEmpty(_labelObjectType) Then
            _labelObjectType = "Role"
        End If
        If _roleID = 0 Then
            GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INVALIDOBJECT)
            Exit Sub
        End If

        Dim lrRole As Role = Role.GetRole(_roleID)
        If lrRole Is Nothing Then
            GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INVALIDOBJECT)
            Exit Sub
        End If

        If Not (Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal OrElse lrRole.TenantId = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.Id) Then
            GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INVALIDOBJECT)
            Exit Sub
        End If

        If Not Page.IsPostBack Then
            Dim loLabel As Label = Label.GetObjectLabel(_roleID, _labelObjectType)
            If loLabel IsNot Nothing Then
                    txtE.Text = loLabel.LABEL_ENGLISH
                    txtN.Text = loLabel.LABEL_DUTCH
                    txtF.Text = loLabel.LABEL_FRENCH
                    txtG.Text = loLabel.LABEL_GERMAN
                End If
            End If
    End Sub

    Protected Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        Dim loLabel As Label = Label.GetObjectLabel(_roleID, _labelObjectType)
        If loLabel Is Nothing Then
            loLabel = Label.NewLabel()
            loLabel.LABEL_OBJECT_ID = _roleID
            loLabel.LABEL_OBJECT_TYPE = _labelObjectType
            loLabel.LABEL_TYPE = Label.LabelType.ApplicationLabel
        End If

        loLabel.LABEL_ENGLISH = txtE.Text
        loLabel.LABEL_DUTCH = txtN.Text
        loLabel.LABEL_FRENCH = txtF.Text
        loLabel.LABEL_GERMAN = txtG.Text

        loLabel = loLabel.Save

        Page.ClientScript.RegisterStartupScript(Me.GetType, "onLoad", "Close();", True)
    End Sub
End Class
