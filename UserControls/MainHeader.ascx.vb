Imports Arco.Doma.Library.Security
Imports Arco.Doma.Library

Partial Class UserControls_MainHeader
    Inherits BaseUserControl

    Private _currentIdentity As BusinessIdentity
    Protected Function isTabActive(ByVal tab As SiteMapNode) As Boolean
        Return SiteMap.CurrentNode IsNot Nothing AndAlso (SiteMap.CurrentNode.Equals(tab) OrElse SiteMap.CurrentNode.IsDescendantOf(tab))
    End Function


    Public Property ShowLogo As Boolean = True
    Public Property ShowSelectedOnly As Boolean


    Protected Sub repLvl1_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles repLvl1.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim loNode As SiteMapNode = CType(e.Item.DataItem, SiteMapNode)
            If AddSitemapNode(loNode) Then
                Dim ctrls As List(Of Control) = GetMenuItem(loNode)
                If ctrls IsNot Nothing Then
                    For Each c As Control In ctrls
                        e.Item.Controls.Add(c)
                    Next
                End If
            End If
        End If
    End Sub
    Private Function AddSitemapNode(ByVal node As SiteMapNode) As Boolean
        Dim lbAdd As Boolean = True
        If node.Roles IsNot Nothing AndAlso node.Roles.Count <> 0 Then
            lbAdd = _currentIdentity.IsInOneOfRoles(node.Roles.Cast(Of String))
        End If
        '  If Not String.IsNullOrEmpty(node("licence")) Then
        'todo
        '    lbAdd = Arco.Doma.Library.Licensing.License.GetLicencedApplications(False).Contains(node("licence"))
        '   End If
        If Not String.IsNullOrEmpty(node("AdminOnly")) Then
            lbAdd = _currentIdentity.isAdmin
        End If
        Return lbAdd
    End Function

    Private Function GetMenuItem(ByVal loNode As SiteMapNode) As List(Of Control)
        Dim hasChildren = loNode.HasChildNodes
        Dim ctrls As New List(Of Control)
        If Not hasChildren Then
            ctrls.Add(GetBasicMenuItem(loNode, False))
            Return ctrls
        End If
        Dim dropdownMenu = GetDropDownMenu(loNode)
        ctrls.Add(dropdownMenu)
        Return ctrls
    End Function

    Private Function GetTitle(ByVal node As SiteMapNode) As String
        Return Arco.Web.ResourceManager.ReplaceLabeltags(node.Title)
    End Function
    Private Function GetBasicMenuItem(ByVal node As SiteMapNode, ByVal isDropDown As Boolean) As Control

        If ShowSelectedOnly AndAlso Not isTabActive(node) Then
            Return Nothing
        End If
        Dim ctrl = New Panel With {.CssClass = "MainMenu_NavItem nav-item"}

        Dim link As New HtmlGenericControl("a") With {
            .InnerText = GetTitle(node)
        }

        ctrl.Controls.Add(link)

        If Not ShowSelectedOnly Then
            If isTabActive(node) Then
                link.Attributes.Add("class", "nav-link MainMenu_NavLink MainMenu_Selected")
            Else
                link.Attributes.Add("class", "nav-link MainMenu_NavLink MainMenu_UnSelected")
            End If
        Else
            link.Attributes.Add("class", "nav-link MainMenu_NavLink MainMenu_Text")
        End If

        If isDropDown Then
            ctrl.CssClass += " dropdown"
        End If
        link.Attributes.Add("href", ResolveUrl(node.Url))

        Return ctrl
    End Function

    Private Function GetDropDownMenu(ByVal node As SiteMapNode) As Control
        Dim panel = GetBasicMenuItem(node, True)

        Dim ddPanel = GetDropDownPanel(node)
        panel.Controls.Add(ddPanel)
        Return panel
    End Function

    Private Function GetDropDownPanel(ByVal loNode As SiteMapNode) As Control
        Dim ddPanel = New Panel With {.CssClass = "MainMenu_DropDown dropdown-menu"}



        If Not HasGrandChildren(loNode) Then
            For Each childNode As SiteMapNode In loNode.ChildNodes
                Dim ddItem = GetDropDownItem(childNode, False)
                ddPanel.Controls.Add(ddItem)
            Next
        Else
            ddPanel.CssClass += If(loNode.ChildNodes.Count > 10, " dd-columns-10", " dd-columns-" + loNode.ChildNodes.Count.ToString())
            Dim flexbox = New Panel With {.CssClass = "d-flex flex-row flex-wrap"}
            For Each childNode As SiteMapNode In loNode.ChildNodes
                Dim ddSubMenu = New Panel With {.CssClass = "MainMenu_DropDownSubmenu"}
                Dim ddHeader = GetDropDownItem(childNode, False)
                ddSubMenu.Controls.Add(ddHeader)
                For Each grandchildNode As SiteMapNode In childNode.ChildNodes
                    Dim ddItem = GetDropDownItem(grandchildNode, True)
                    ddSubMenu.Controls.Add(ddItem)
                Next
                flexbox.Controls.Add(ddSubMenu)
            Next
            ddPanel.Controls.Add(flexbox)
        End If

        Dim borderColorTriangle = New Label() With {.CssClass = "MainMenu_Triangle TriangleBorderColor"}
        Dim backgroundColorTriangle = New Label() With {.CssClass = "MainMenu_Triangle TriangleBackgroundColor"}
        ddPanel.Controls.Add(borderColorTriangle)
        ddPanel.Controls.Add(backgroundColorTriangle)

        Return ddPanel
    End Function

    Private Function GetDropDownItem(ByVal node As SiteMapNode, ByVal isSmall As Boolean) As Control
        Dim ddItem = New HtmlGenericControl("a") With {.InnerText = GetTitle(node)}
        ddItem.Attributes.Add("href", ResolveUrl(node.Url))
        ddItem.Attributes.Add("class", "dropdown-item" + If(isSmall, " MainMenu_DropDownItemSmall", ""))
        Return ddItem
    End Function

    Private Function HasGrandChildren(ByVal loNode As SiteMapNode) As Boolean
        For Each childNode As SiteMapNode In loNode.ChildNodes
            If childNode.HasChildNodes Then
                Return True
            End If
        Next
        Return False
    End Function

    Protected Function GetTooltip(ByVal loNode As SiteMapNode) As List(Of Control)

        Dim ctrls As New List(Of Control)

        Dim button As New Telerik.Web.UI.RadButton With {
            .NavigateUrl = loNode.Url,
            .Height = Unit.Pixel(65),
            .Width = Unit.Pixel(80),
            .ButtonType = Telerik.Web.UI.RadButtonType.LinkButton,
             .Text = GetTitle(loNode),
            .AutoPostBack = False,
            .Skin = "MetroTouch"
        }

        button.Style.Add("padding-top", "30px")

        If isTabActive(loNode) Then
            button.Font.Bold = True
            ' button.CssClass = button.cs
            '  button.Style.Add("background-color", "#F6781F")
        End If


        Select Case button.Text
            Case "Admin"
                button.Icon.PrimaryIconUrl = ThemedImage.GetUrl("archivedcase.png", Me)
            Case Else
                button.Icon.PrimaryIconCssClass = "rbOpen"
        End Select

        button.Icon.PrimaryIconLeft = Unit.Pixel(30)
        button.Icon.PrimaryIconTop = Unit.Pixel(30)

        ctrls.Add(button)

        Return ctrls
    End Function
    'Protected Sub repTooltip_ItemDataBound(ByVal sender As Object, ByVal e As DataListItemEventArgs) Handles repTooltip.ItemDataBound
    '    If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
    '        Dim loNode As SiteMapNode = CType(e.Item.DataItem, SiteMapNode)

    '        If AddSitemapNode(loNode) Then
    '            Dim ctrls As List(Of Control) = GetTooltip(loNode)
    '            For Each c As Control In ctrls
    '                e.Item.Controls.Add(c)
    '            Next
    '        Else
    '            e.Item.Visible = False
    '        End If


    '    End If
    'End Sub

    Private Function GetDisplayUserName() As String
        If Not Arco.Settings.FrameWorkSettings.isCDRom AndAlso Not Arco.Settings.FrameWorkSettings.isReadOnly Then
            Return _currentIdentity.DisplayName
        Else
            Return String.Empty
        End If
    End Function
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        _currentIdentity = BusinessIdentity.CurrentIdentity
        If ShowLogo Then
            mainLogo.Visible = True
        Else
            mainLogo.Visible = False
        End If

        ShowHelpImage()

        pnlGuest.Visible = False

        If Arco.Settings.FrameWorkSettings.isCDRom Then
            imgWorkWrapper.Visible = False
            imgProfile.Visible = False
            tltProfile.Visible = False
        ElseIf Not _currentIdentity.IsAuthenticated Then
            'guest
            imgWorkWrapper.Visible = False
            pnlProfile.Visible = False
            pnlGuest.Visible = True
            btnLogin.Text = GetLabel("login")
        Else
            'logged in user
            ShowWorkListImage()
            ShowProfileImage()
        End If

    End Sub
    Private Sub ShowHelpImage()
        If Not Settings.GetValue("Interface", "ShowHelpButton", True) Then
            imgHelpWrapper.Visible = False
            Exit Sub
        End If


        imgHelp.ImageUrl = ThemedImage.GetUrl("help.svg", Me)

        Dim helpPageUrl As String = TagReplacer.ReplaceTags(Settings.GetValue("Interface", "HelpLink", "~/Help/WebHelp_#LANGCODE#.pdf", True))
        If Not String.IsNullOrEmpty(helpPageUrl) Then
            lnkHelp.Text = GetLabel("helplink")
            lnkHelp.NavigateUrl = helpPageUrl
            lnkHelp.Target = "_blank"
        Else
            pnlHelpLink.Visible = False
        End If

        Dim legalPageUrl As String = TagReplacer.ReplaceTags(Settings.GetValue("Interface", "LegalLink", "", True))
        If Not String.IsNullOrEmpty(legalPageUrl) Then
            lnkLegal.Text = GetLabel("legallink")
            lnkLegal.NavigateUrl = legalPageUrl
            lnkLegal.Target = "_blank"
        Else
            pnlLegalLink.Visible = False
        End If

    End Sub
    Private Sub ShowWorkListImage()
        If Not Settings.GetValue("Interface", "ShowNewWorkButton", True) Then
            imgWorkWrapper.Visible = False
            Exit Sub
        End If

        imgWork.ImageUrl = ThemedImage.GetUrl("work_menu.svg", Me)
        imgWork.ToolTip = GetLabel("mywork")
        Dim workCount As Int32 = GetNewWorkCOunt()
        If workCount <> 0 Then
            imgWorkWrapper.Attributes.Add("data-badge", workCount.ToString)
        End If

    End Sub
    Private Function GetNewWorkCOunt() As Int32
        Return Routing.WorkList.GetWorkList(New Routing.WorkList.Criteria With {
            .isNew = True,
            .GetCountOnly = True,
            .ApplyWorkExceptions = True,
            .ShowLockedDossiers = False
        }).Count

    End Function
    Private Sub ShowProfileImage()
        Dim userName As String = GetDisplayUserName()
        If _currentIdentity.IsGlobal AndAlso _currentIdentity.Tenant.Id <> 0 Then
            userName &= " (" & _currentIdentity.Tenant.Name & ")"
        End If
        lblUserNameMain.Text = Server.HtmlEncode(userName)

        If _currentIdentity.Impersonator IsNot Nothing Then
            pnlImpersonator.Visible = True
            lnkImpersonator.Text = "(" & ArcoFormatting.FormatUserName(_currentIdentity.Impersonator) & ")"
        Else
            pnlImpersonator.Visible = False
        End If

        imgProfile.ImageUrl = ThemedImage.GetUrl("profile_menu.svg", Me)

        ' imgProfileLogo.Height = Unit.Pixel(100)

        If _currentIdentity.IsAuthenticated AndAlso (Settings.GetValue("Interface", "MixedLogin", False) OrElse _currentIdentity.AuthenticationMethod <> ACL.User.AuthenticationMethod.WindowsDomain) Then
            lnkLogoff.Text = GetLabel("logoff")
        Else
            lnkLogoff.Visible = False
        End If

        ShowUserPreferencesLink()
        ShowTenantSwitch()
    End Sub

    Public Sub ShowUserPreferencesLink()
        Dim hasProfile = Not _currentIdentity.IsVirtual
        Dim externalProfile As Boolean = False
        Dim identityProfileUrl As String = _currentIdentity.GetExtendedProperty("ProfileUrl")
        If String.IsNullOrEmpty(identityProfileUrl) Then
            identityProfileUrl = "~/UserBrowser/DM_UserBrowser_AddUser.aspx?fromUP=true"
        Else
            hasProfile = True
            externalProfile = Not String.IsNullOrEmpty(_currentIdentity.GetExtendedProperty("oauthprovider"))
        End If


        If hasProfile Then
            lnkPrefs.Visible = True
            lnkPrefs.Text = GetLabel("myprefs")
            lnkPrefs.NavigateUrl = "javascript:MyPrefs(""" & ResolveClientUrl(identityProfileUrl) & """," & EncodingUtils.EncodeJsBool(externalProfile) & ");"
        Else
            lnkPrefs.Visible = False
        End If
    End Sub

    Private Sub ShowTenantSwitch()

        If Not Settings.GetValue("General", "MultiTenant", False) Then
            pnlTenant.Visible = False
            pnlSelectTenant.Visible = False
            Exit Sub
        End If

        pnlTenant.Visible = True

        Dim showSelectTenant As Boolean
        If _currentIdentity.IsGlobal Then
            lblTenant.Text = "Global"
            showSelectTenant = True
        Else
            lblTenant.Text = _currentIdentity.Tenant.Name
            showSelectTenant = TenantList.GetTenantsForCurrentUser().Count > 1
            ' cmbTenant.Items.Add(New Telerik.Web.UI.RadComboBoxItem("Global", "0") With {.Selected = _currentIdentity.Tenant.IsGlobal})
        End If

        'For Each tenant As TenantList.TenantInfo In TenantList.GetTenantsForCurrentUser
        '    If tenant.Enabled Then
        '        tenantCount += 1
        '        ' cmbTenant.Items.Add(New Telerik.Web.UI.RadComboBoxItem(tenant.Name, tenant.Id) With {.Selected = (tenant.Id = _currentIdentity.Tenant.Id)})
        '    End If
        'Next

        lnkSelectTenant.Text = GetLabel("switchtenant")

        If showSelectTenant Then
            pnlSelectTenant.Visible = True
            lnkSelectTenant.NavigateUrl = "~/Auth/SelectTenant.aspx"
        Else
            pnlSelectTenant.Visible = False
        End If
        'If tenantCount > 1 Then
        '    lnkTenant.Enabled = True
        '    lnkTenant.NavigateUrl = "~/Auth/SelectTenant.aspx"
        'Else
        '    lnkTenant.Enabled = False
        'End If


    End Sub

    Protected Sub lnkImpersonator_Click(sender As Object, e As EventArgs)
        _currentIdentity = _currentIdentity.RevertToSelf()

        Response.Redirect(_currentIdentity.GetDefaultWebApplication(7).Url)
    End Sub



End Class
