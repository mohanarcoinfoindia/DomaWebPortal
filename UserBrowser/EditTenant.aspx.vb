
Imports Arco.Doma.Library
Imports Arco.Doma.Library.Globalisation
Imports Arco.Doma.Library.Helpers
Imports Arco.Doma.Library.Settings

Partial Class UserBrowser_EditTenant
    Inherits BaseAdminOnlyPage

    Private Property TenantID As Int32
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        scriptManager.Scripts.Add(New ScriptReference("~/Resources/" & Language & "/Messages.js"))

        rowError.Visible = False
        lblName.Text = GetLabel("name")
        lblDesc.Text = GetLabel("description")
        btnOk.Text = GetLabel("save")
        lblStatus.Text = GetLabel("status")
        rdStatus.Items(0).Text = GetLabel("enabled")
        rdStatus.Items(1).Text = GetLabel("disabled")

        lblSeeGlobal.Text = "Global Objects"
        rdSeeGlobal.Items(0).Text = GetLabel("yes")
        rdSeeGlobal.Items(1).Text = GetLabel("no")


        btnCancelUrl.Text = GetLabel("cancel")
        lblTheme.Text = "Theme"
        lblLang.Text = GetLabel("language")
        lblUrl.Text = GetLabel("Url")
        lblFolder.Text = "Root folder"
        lblEmail.Text = GetLabel("mail_emailaddress")
        lblEmailDisplay.Text = GetLabel("mail_emailaddress") & " (Display)"

        TenantID = QueryStringParser.GetInt("ID")

        If Not Page.IsPostBack Then

            For Each t As String In System.IO.Directory.GetDirectories(Server.MapPath("~/App_Themes"))
                Dim themeName As String = System.IO.Path.GetFileName(t)
                If themeName <> "Common" Then
                    drpTheme.Items.Add(New WebControls.ListItem(themeName) With {.Selected = themeName.Equals(ArcoInfoSettings.DefaultTheme, StringComparison.CurrentCultureIgnoreCase)})
                End If
            Next

            For Each loLang As LanguageList.LanguageInfo In LanguageList.GetInterfaceLanguageList()
                drpLang.Items.Add(New WebControls.ListItem(Server.HtmlDecode(loLang.Description), loLang.InterfaceLanguageCode))
            Next
            Dim tenant As Tenant
            If TenantID <> 0 Then

                tenant = Tenant.GetTenant(TenantID)
                txtName.Text = tenant.Name
                txtDesc.Text = tenant.Description
                txtUrl.Text = tenant.Url
                lblId.Text = tenant.ID

                txtFolder.Text = Folder.GetFolder(tenant.RootFolder).GetFullPath
                If Not String.IsNullOrEmpty(tenant.DefaultTheme) Then
                    drpTheme.SelectedValue = tenant.DefaultTheme
                End If
                If Not String.IsNullOrEmpty(tenant.DefaultLanguage) Then
                    drpLang.SelectedValue = tenant.DefaultLanguage
                End If
                Title = tenant.Name

                Dim tenantSettings As List(Of ArcoInfoEntry) = ArcoInfo.GetTentantParameters(tenant.Name, True)
                LoadTenantSetting(tenantSettings, txtEmailSender, "Mail", "DefaultSender")
                LoadTenantSetting(tenantSettings, txtSenderDisplay, "Mail", "DefaultDisplayFrom")

            Else
                Title = GetLabel("new")
                tenant = Tenant.NewTenant()

                drpLang.SelectedValue = Language

            End If
            rdStatus.SelectedIndex = If(tenant.Enabled, 0, 1)
            rdSeeGlobal.SelectedIndex = If(tenant.CanSeeGlobalObjects, 0, 1)

        End If

        LoadAudit()
    End Sub

    Private Sub LoadAudit()
        If TenantID <> 0 Then
            radTab.Tabs(1).Visible = True
            Dim dt As New DataTable()
            dt.Clear()
            dt.Columns.Add("DATE")
            dt.Columns.Add("USER")
            dt.Columns.Add("DESCRIPTION")

            For Each loAudit As Logging.AppAuditList.AppAuditInfo In Logging.AppAuditList.GetAppAuditList(TenantID, "TENANT")
                Dim lsUSer As String = ArcoFormatting.FormatUserName(loAudit.User)
                Dim lsDesc As String = Server.HtmlEncode(loAudit.Description)
                Dim lsDate As String = ArcoFormatting.FormatDateLabel(loAudit.AuditDate, True, True, False)

                dt.Rows.Add(New String() {lsDate, lsUSer, lsDesc})
            Next
            If dt.Rows.Count = 0 Then
                trNoAuditFound.Visible = True
                lblNoAuditFound.Text = GetLabel("noresultsfound")
            Else
                trNoAuditFound.Visible = False
            End If

            repAudit.DataSource = dt
            repAudit.DataBind()
        Else
            radTab.Tabs(1).Visible = False 'hide audit tab
        End If
    End Sub

    Private Sub SetInfoMessage(ByVal t As String)
        lblMsgText.Visible = True
        lblMsgText.Text = t
        lblMsgText.CssClass = "InfoLabel"
    End Sub

    Private Sub SetError(ByVal msg As String)
        lblError.Text = msg
        rowError.Visible = True
    End Sub

    Protected Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        If Not Page.IsValid Then
            Exit Sub
        End If

        If ACL.Rules.isReservedName(txtName.Text) Then
            SetError(txtName.Text & " " & GetLabel("isreservedword"))
            Exit Sub
        End If
        Dim invalidChars As String = Nothing
        If ACL.Rules.ContainsInvalidCharacters(txtName.Text, invalidChars) Then
            SetError(Server.HtmlEncode(txtName.Text) & " " & GetLabel("containsinvalidchars") & ": " & Server.HtmlEncode(invalidChars))
            Exit Sub
        End If

        Dim tn As Tenant
        If TenantID <> 0 Then
            tn = Tenant.GetTenant(TenantID)
            Title = tn.Name

            If Not tn.Name.Equals(txtName.Text, StringComparison.CurrentCultureIgnoreCase) Then
                SetError("Renaming a tenant wil cause tenant specific arcoinfo parameters to no longer be used!")
            End If

        Else

            tn = Tenant.NewTenant()
            Title = GetLabel("new")
        End If


        tn.Name = txtName.Text
        tn.Description = txtDesc.Text
        tn.DefaultTheme = drpTheme.SelectedValue
        tn.DefaultLanguage = drpLang.SelectedValue
        tn.Url = txtUrl.Text
        tn.Enabled = (rdStatus.SelectedItem.Value = "1")
        tn.CanSeeGlobalObjects = (rdSeeGlobal.SelectedItem.Value = "1")

        If Not String.IsNullOrEmpty(txtFolder.Text) Then
            Dim fld As Folder = Folder.GetFolder(0, txtFolder.Text)
            If fld IsNot Nothing Then
                If fld.TenantId <> 0 AndAlso fld.TenantId <> tn.ID Then
                    SetError("Unable to set this folder as tenant root since it belongs to another tenant")
                    Exit Sub
                End If
                tn.RootFolder = fld.ID
            Else
                tn.RootFolder = Folder.GetOrCreateFolderStructure(Nothing, txtFolder.Text, 0)
            End If
        Else
            tn.RootFolder = 0
        End If


        Try
            tn = tn.Save()
        Catch ex As Exception
            SetError(Arco.Utils.ExceptionHelper.GetInnerExceptionMessage(Of Exceptions.DomaException)(ex))
            Exit Sub
        End Try


        Dim tenantSettings As List(Of ArcoInfoEntry) = ArcoInfo.GetTentantParameters(tn.Name, True)

        SaveTenantSetting(tn, tenantSettings, txtEmailSender, "Mail", "DefaultSender")
        SaveTenantSetting(tn, tenantSettings, txtSenderDisplay, "Mail", "DefaultDisplayFrom")
        SetInfoMessage(GetLabel("saveok"))
    End Sub

    Private Sub LoadTenantSetting(ByVal tenantsettings As List(Of ArcoInfoEntry), ByVal txtBox As TextBox, ByVal cat As String, ByVal name As String)
        Dim setting As ArcoInfoEntry =
                   tenantsettings.FirstOrDefault(Function(x) x.Category = cat AndAlso x.ParmName = name)
        If setting IsNot Nothing Then
            txtBox.Text = setting.ParmValue
        End If
    End Sub

    Private Sub SaveTenantSetting(ByVal tn As ITenant, ByVal tenantsettings As List(Of ArcoInfoEntry), ByVal txtBox As TextBox, ByVal cat As String, ByVal name As String)
        Dim setting As ArcoInfoEntry =
           tenantsettings.FirstOrDefault(Function(x) x.Category = cat AndAlso x.ParmName = name)

        If Not String.IsNullOrEmpty(txtBox.Text) Then
            If setting Is Nothing Then
                setting = ArcoInfoEntry.NewEntry(tn)
                setting.Category = cat
                setting.ParmName = name
            End If
            setting.ParmValue = txtBox.Text
            setting.Save()
        Else
            If setting IsNot Nothing Then
                ArcoInfoEntry.DeleteEntry(setting.ID)
            End If
        End If


    End Sub
End Class
