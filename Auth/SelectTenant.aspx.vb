
Imports Arco.Doma.Library
Imports Arco.Doma.Library.Security
Imports Telerik.Web.UI

Partial Class Auth_SelectTenant
    Inherits BaseAfterLoginPage

    Private Sub Auth_SelectTenant_Load(sender As Object, e As EventArgs) Handles Me.Load

        lblTenant.Text = GetLabel("Tenant")
        lnkSelect.Text = GetLabel("Select")

        cmbTenant.Width = Unit.Pixel(250)


        SetReturnTo()
        LoadTenants()

        lnkSelect.Focus()

    End Sub
    Private Sub SetReturnTo()
        If Not Page.IsPostBack Then

            txtReturnTo.Value = QueryStringParser.GetUrl("redirect_uri")
        End If
    End Sub
    Private Sub LoadTenants()

        If IsPostBack Then
            Exit Sub
        End If


        If BusinessIdentity.CurrentIdentity.IsGlobal Then
            cmbTenant.Items.Add(New RadComboBoxItem("Global", "0"))
        End If

        For Each tenant As ITenant In TenantList.GetTenantsForCurrentUser()
            If tenant.Enabled Then
                cmbTenant.Items.Add(New RadComboBoxItem(tenant.Name, tenant.Id.ToString()))
            End If
        Next

        'autoselect
        If Not Page.IsPostBack Then
            Select Case cmbTenant.Items.Count
                Case 0
                    'no tenant is available for this user
                    pnlSelect.Visible = False
                    pnlMsg.Visible = True
                    lblMsg.Text = "No tenant is available"
                Case 1
                    SwitchTenant(Convert.ToInt32(cmbTenant.Items(0).Value))
                Case Else
                    Dim tenantIdfromUrl As String = QueryStringParser.GetString("tenantid")
                    If Not String.IsNullOrEmpty(tenantIdfromUrl) Then
                        For Each item As RadComboBoxItem In cmbTenant.Items
                            If item.Value = tenantIdfromUrl Then
                                SwitchTenant(Convert.ToInt32(tenantIdfromUrl))
                                Exit For
                            End If
                        Next
                    End If
            End Select

        End If

    End Sub

    Private Sub lnkSelect_Click(sender As Object, e As EventArgs) Handles lnkSelect.Click
        If Not Page.IsValid Then
            Exit Sub
        End If

        Dim selectedTenantId As Integer = Convert.ToInt32(cmbTenant.SelectedValue)

        SwitchTenant(selectedTenantId)

    End Sub

    Private Sub SwitchTenant(ByVal tenantId As Integer)
        BusinessPrincipal.SwitchTenant(tenantId)


        ContinueLogin(txtReturnTo.Value, False)
    End Sub

End Class
