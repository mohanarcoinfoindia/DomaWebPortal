
Imports Arco.Doma.Library.Website

Partial Class UserBrowser_SitemapEntry
    Inherits BaseAdminOnlyPage
    Public Property ShowV8Options As Boolean
    Private Property _entryId As Integer
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        ShowV8Options = Not String.IsNullOrEmpty(Settings.GetValue("General", "url_v8", ""))

        lblName.Text = GetLabel("name")

        lblDescription.Text = GetLabel("description")
        lblRoles.Text = GetLabel("roles")
        lblParentId.Text = "Parent Id"
        lblGroup.Text = GetLabel("group")
        lblSeq.Text = "Sequence"
        lblUrl.Text = GetLabel("url")
        lblParameters.Text = "Parameters"
        lblIcon.Text = "Icon"
        btnOk.Text = GetLabel("save")
        btnCancelUrl.Text = GetLabel("cancel")

        _entryId = QueryStringParser.GetInt("ID")

        If Not Page.IsPostBack Then


            If _entryId <> 0 Then
                'Title = GetDecodedLabel("edit")
                ShowEntry(SiteApplication.GetApplication(_entryId))
            Else
                Title = GetDecodedLabel("new")
            End If

        End If
    End Sub


    Private Sub ShowEntry(ByVal entry As SiteApplication)
        Title = entry.Name
        txtName.Text = entry.Name
        txtDescription.Text = entry.Description
        txtUrl.Text = entry.Url
        txtParentId.Text = entry.ParentId
        txtSeq.Text = entry.Sequence
        txtRoles.Text = entry.Roles
        txtLabel.SetValue(entry.Label)
        txtParameters.Text = entry.Parameters

        lblId.Text = entry.ID
        chkIsDefault.Checked = entry.IsDefault
        chkAppendQueryStrng.Checked = entry.AppendQueryString
        If ShowV8Options Then
            drpSiteVersion.SelectedValue = Convert.ToInt32(entry.SiteVersion).ToString()

            txtIcon.Text = entry.Icon
            txtGroup.Text = entry.Group
        End If
    End Sub

    Protected Sub btnOk_Click(sender As Object, e As System.EventArgs) Handles btnOk.Click
        If Not Page.IsValid Then
            Return
        End If
        Dim entry As SiteApplication

        If _entryId <> 0 Then
            entry = SiteApplication.GetApplication(_entryId)
            If Not String.IsNullOrEmpty(txtUrl.Text) Then
                For Each currentEntry As SiteApplicationList.ApplicationInfo In SiteApplicationList.GetApplications()
                    If entry.ID <> currentEntry.Id AndAlso currentEntry.Url.Equals(txtUrl.Text, StringComparison.CurrentCultureIgnoreCase) Then
                        SetErrorMessage(txtUrl.Text & " is already used")
                        Return
                    End If
                Next
            End If
        Else
            entry = SiteApplication.NewApplication()
            If Not String.IsNullOrEmpty(txtUrl.Text) Then
                For Each currentEntry As SiteApplicationList.ApplicationInfo In SiteApplicationList.GetApplications()
                    If currentEntry.Url.Equals(txtUrl.Text, StringComparison.CurrentCultureIgnoreCase) Then
                        SetErrorMessage(txtUrl.Text & " is already used")
                        Return
                    End If
                Next
            End If
            If Not ShowV8Options Then
                entry.SiteVersion = 7
            End If
        End If

        entry.Name = txtName.Text
        entry.Description = txtDescription.Text
        entry.Url = txtUrl.Text
        entry.Label = txtLabel.GetValue()
        entry.Parameters = txtParameters.Text

        If ShowV8Options Then
            entry.SiteVersion = Convert.ToInt32(drpSiteVersion.SelectedValue)
            entry.Icon = txtIcon.Text
            entry.Group = txtGroup.Text
        End If

        Dim i As Integer

        If String.IsNullOrEmpty(txtParentId.Text) Then
            txtParentId.Text = "0"
        End If

        If Integer.TryParse(txtParentId.Text, i) Then
            entry.ParentId = i
        End If

        If String.IsNullOrEmpty(txtSeq.Text) Then
            txtSeq.Text = SiteApplicationList.GetApplications().Cast(Of SiteApplicationList.ApplicationInfo).Where(Function(x) x.SiteVersion = entry.SiteVersion).Select(Function(x) x.Sequence).Max() + 1
        End If

        If Integer.TryParse(txtSeq.Text, i) Then
            entry.Sequence = i
        End If
        entry.Roles = txtRoles.Text
        entry.IsDefault = chkIsDefault.Checked
        entry.AppendQueryString = chkAppendQueryStrng.Checked


        entry.Save()

        Page.ClientScript.RegisterStartupScript(Page.ClientScript.[GetType](), "onLoad", "GetRadWindow().close();", True)

    End Sub

    Private Sub SetErrorMessage(ByVal t As String)
        lblMsgText.Visible = True
        lblMsgText.Text = t
        lblMsgText.CssClass = "ErrorLabel"
    End Sub
End Class
