Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports System.Linq
Imports Arco.Doma.Library.Mail
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Website

Partial Class DM_UserBrowser_AddUser
    Inherits UserBrowserBasePage
#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
        If ClientQueryString.Contains("new=true") Then
            SubListMainHeaderT_EditButton.Visible = False
        End If
    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Public Enum TabPages
        Details = 0
        Tenants = 1
        MemberOf = 2
        Preferencs = 3
        Delegates = 4
        Notifications = 5
        Subscriptions = 6
        MFA = 7
        OAuth = 8
        Licencing = 9
        Audit = 10
    End Enum


    Dim _login As String = ""

    Public Property FromUserProfile As Boolean
    Public Property MultiTenant As Boolean
    Public Property ShowRoleTenant As Boolean
    Protected ReadOnly Property UrlWithoutTab As String
        Get
            If FromUserProfile Then
                Return EncodingUtils.EncodeJsString("DM_UserBrowser_AddUser.aspx?fromUP=true", False)
            End If
            Return EncodingUtils.EncodeJsString("DM_UserBrowser_AddUser.aspx?login=" & _login & "&new=false&fromUP=false", False)
        End Get
    End Property
    Public Property UpdateModus As Boolean
        Get
            Dim o As Object = ViewState("updatemodus")
            If o Is Nothing Then
                Return False
            End If
            Return CType(o, Boolean)
        End Get
        Set(value As Boolean)
            ViewState("updatemodus") = value
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Me.Form.DefaultButton = lnkSave.UniqueID

        If FromUserProfile Then
            _login = Arco.Security.BusinessIdentity.CurrentIdentity.Name
        Else
            _login = QueryStringParser.GetString("login")
        End If

        MultiTenant = Settings.GetValue("General", "MultiTenant", False) ' AndAlso Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal
        ShowRoleTenant = MultiTenant AndAlso Security.BusinessIdentity.CurrentIdentity.IsGlobal


        If Not FromUserProfile Then
            CheckIsAdmin()
        Else 'User settings
            Page.Title = GetLabel("myprefs")
        End If

        If Not Page.IsPostBack AndAlso Not FromUserProfile AndAlso Request.UrlReferrer IsNot Nothing Then
            ViewState("RefUrl") = Request.UrlReferrer.ToString()
        End If


        LoadLabels()
        LoadDropdowns()

        If Not Page.IsPostBack Then
            If FromUserProfile OrElse Not QueryStringParser.GetBoolean("new") Then
                LoadUser(_login)
                UpdateModus = True
            Else
                UpdateModus = False
            End If

            Dim liTabID As Integer = QueryStringParser.GetInt("tabindex")
            RadMultiPage1.SelectedIndex = liTabID
            radTab.SelectedIndex = liTabID
        End If


        If Not UpdateModus Then
            'new user
            RequiredFieldValidator1.Enabled = True
            RequiredFieldValidator2.Enabled = True


            ShowHidePasswordFields(True)
            rOldPassWord.Visible = False
            lnkImpersonate.Visible = False
            chkIsTenantUser.Checked = Not Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal
            If Not Page.IsPostBack Then
                ShowAuthMethod(Nothing)
            End If

        Else
            'update exisitng user
            RequiredFieldValidator1.Enabled = False
            RequiredFieldValidator2.Enabled = False



            ctlDelegates.SubjectType = "User"
            ctlDelegates.SubjectID = Account

            If Not FromUserProfile AndAlso Not isCurrentUser() AndAlso Settings.GetValue("Security", "EnableImpersonation", True) AndAlso Security.BusinessIdentity.CurrentIdentity.isAdmin Then
                lnkImpersonate.Visible = True
                lnkImpersonate.Text = GetLabel("impersonate")
                lnkImpersonate.OnClientClick = "return confirm(" & EncodingUtils.EncodeJsString(GetDecodedLabel("confirmimpersonate")) & ");"
            Else
                lnkImpersonate.Visible = False
            End If
        End If

        CheckDomainSettings()
        CheckFocus()

        SetVisibility()

        If UpdateModus AndAlso Not IsPostBack Then
            FillPreferencesCheckboxes()
        End If


        lnkCancel.Visible = Not FromUserProfile

        Dim sScript As New StringBuilder
        If Not FromUserProfile Then
            sScript.Append("function DoCancel(){")
            'If FromUserProfile Then
            '    sScript.Append("Close();")
            'Else
            Dim lsReturnTo As String = ViewState("RefUrl").ToString
            If Not lsReturnTo.ToLower.Contains("dm_userbrowser_rightpane") Then
                lsReturnTo = "dm_userbrowser_rightpane.aspx"
            End If
            sScript.Append("location.href= ")
            sScript.Append(EncodingUtils.EncodeJsString(lsReturnTo))
            sScript.Append(";")
            '  End If
            sScript.Append("}")
        End If

        sScript.Append("function TransferWork() {")
        sScript.Append("var w = window.open(")
        sScript.Append(EncodingUtils.EncodeJsString("TransferWork.aspx?subject_type=User&subject_id=" & Server.UrlEncode(Account)))
        sScript.Append(", 'TransferWork','width=800,height=600,resizable=yes,scrollbars=yes,status=yes');")
        sScript.Append("}")
        sScript.Append("function Refresh() {")
        sScript.Append("$get('" & hdnIsRefresh.ClientID & "').value = 'Y';")
        sScript.Append("document.forms[0].submit();")
        sScript.Append("}")


        If hdnIsRefresh.Value = "Y" Then
            hdnIsRefresh.Value = ""
            SaveSubscriptions()
            If Not String.IsNullOrEmpty(hndDelSubs.Value) Then
                Subscription.DeleteSubscription(Convert.ToInt32(hndDelSubs.Value))
                hndDelSubs.Value = ""
            End If
            LoadSubscriptions()

            thisUser = ACL.User.GetUser(txtLoginName.Text)
            LoadRoles()
        End If

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "AddUserScript", sScript.ToString, True)

    End Sub

    Private Function GetWorkCount() As Int32
        If FromUserProfile Then
            Return Routing.WorkList.GetWorkList(New Routing.WorkList.Criteria With {
            .GetCountOnly = True,
            .ApplyWorkExceptions = True,
            .ShowLockedDossiers = False
        }).Count
        End If
        'todo
        Return 0
    End Function
    Private Sub SetVisibility()
        If UpdateModus Then
            lnkUnlockWorkflows.Visible = Not FromUserProfile OrElse GetWorkCount() <> 0
            TransferWorkButton.Visible = IsAdmin() AndAlso lnkUnlockWorkflows.Visible

            lnkDelete.Visible = Not FromUserProfile AndAlso IsAdmin()
            radTab.Tabs(TabPages.Licencing).Visible = Not FromUserProfile AndAlso Not MultiTenant
            radTab.Tabs(TabPages.Audit).Visible = Not FromUserProfile AndAlso IsAdmin()
        Else
            RadMultiPage1.BorderWidth = Unit.Pixel(0)
            radTab.Tabs(TabPages.Details).Visible = False 'details         
            radTab.Tabs(TabPages.Tenants).Visible = False
            radTab.Tabs(TabPages.MemberOf).Visible = False 'memberof
            radTab.Tabs(TabPages.Preferencs).Visible = False 'preferences
            radTab.Tabs(TabPages.Delegates).Visible = False 'delegates
            radTab.Tabs(TabPages.Notifications).Visible = False 'o
            radTab.Tabs(TabPages.Subscriptions).Visible = False
            radTab.Tabs(TabPages.MFA).Visible = False
            radTab.Tabs(TabPages.OAuth).Visible = False
            radTab.Tabs(TabPages.Licencing).Visible = False
            radTab.Tabs(TabPages.Audit).Visible = False
            TransferWorkButton.Visible = False
            lnkUnlockWorkflows.Visible = False
            lnkDelete.Visible = False
        End If

        radTab.Tabs(TabPages.Details).Visible = EnableContactInfo OrElse (Not FromUserProfile AndAlso EnableSpecialCode)

        If radTab.Tabs(TabPages.Details).Visible Then
            If Not FromUserProfile Then
                rowSpecialCode.Visible = EnableSpecialCode
            Else
                rowSpecialCode.Visible = False
            End If

            If Not EnableContactInfo Then
                HideContactInfo()
            End If

        Else
            If Not IsPostBack Then
                radTab.Tabs(TabPages.MemberOf).Selected = True
                RadMultiPage1.SelectedIndex = TabPages.MemberOf
            End If

        End If


        trStatus.Visible = Not FromUserProfile

    End Sub

    Private Sub HideContactInfo()
        rowFirstName.Visible = False
        rowLastName.Visible = False
        rowPhone.Visible = False
        rowFax.Visible = False
        rowAddress1.Visible = False
        rowAddress2.Visible = False
        rowCity.Visible = False
        rowZip.Visible = False
        rowState.Visible = False
        rowCountry.Visible = False
        rowCompany.Visible = False

    End Sub

    Public ReadOnly Property EnableSpecialCode As Boolean
        Get
            Return Settings.GetValue("Users", "EnableSpecialCode", False)
        End Get
    End Property

    Public ReadOnly Property EnableContactInfo As Boolean
        Get
            Return Settings.GetValue("Users", "EnableContactInfo", False)
        End Get
    End Property

    Protected ReadOnly Property Account As String
        Get
            Return ACL.User.AddDummyDomain(txtLoginName.Text)
        End Get
    End Property
    Private Function isCurrentUser() As Boolean
        Return isCurrentUser(txtLoginName.Text)
    End Function
    Private Function isCurrentUser(ByVal vsLogin As String) As Boolean
        If ACL.User.AddDummyDomain(vsLogin) = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
            Return True
        Else
            Return False
        End If
    End Function
    Private moProf As UserProfile

    Public Sub New()

    End Sub

    Private ReadOnly Property UserProfileToEdit() As UserProfile
        Get
            If isCurrentUser() Then
                Return UserProfile
            Else
                If moProf Is Nothing Then
                    moProf = UserProfile.GetUserProfile(Account)
                End If
                Return moProf
            End If
        End Get

    End Property
    Private Sub FillPreferencesCheckboxes()


        chkShowQuery.Visible = Security.BusinessIdentity.CurrentIdentity.isAdmin

        TranslateCheckBoxes()

        ' Dim enabled As Boolean = Not UserProfileToEdit.IsReadOnly
        With UserProfileToEdit
            chkShowFilters.Checked = .ShowFilters
            chkShowFilters.Enabled = Not .IsReadOnly("ShowFilters")

            chkShowFoldersInList.Checked = .ShowFoldersInList
            chkShowFoldersInList.Enabled = Not .IsReadOnly("ShowFoldersInList")

            chkShowPreview.Checked = .ShowPreview
            chkShowPreview.Enabled = Not .IsReadOnly("ShowPreview")

            chkShowPreviewInDetailWindows.Checked = .ShowPreviewInDetailWindows
            chkShowPreviewInDetailWindows.Enabled = Not .IsReadOnly("ShowPreviewInDetailWindows")

            chkShowCurrentFolder.Checked = .ShowCurrentFolder
            chkShowCurrentFolder.Enabled = Not .IsReadOnly("ShowCurrentFolder")

            chkShowQuery.Checked = .ShowQuery
            chkShowQuery.Enabled = Not .IsReadOnly("ShowQuery")

            chkShowSearchInList.Checked = .ShowSearchScreenInList
            chkShowSearchInList.Enabled = Not .IsReadOnly("ShowSearchScreenInList")

            chkALWAYSSHOWATTACHMENTS.Checked = .AlwaysShowAttachments
            chkALWAYSSHOWATTACHMENTS.Enabled = Not .IsReadOnly("AlwaysShowAttachments")

            chkShowTree.Checked = .ShowTree
            chkShowTree.Enabled = Not .IsReadOnly("ShowTree")

            chkAUTOOPENFILEINDETAIL.Checked = .AutoOpenFileInDetail
            chkAUTOOPENFILEINDETAIL.Enabled = Not .IsReadOnly("AutoOpenFileInDetail")

            chkSHOWFOLDERLINKICONS.Checked = .ShowFolderLinkIcons
            chkSHOWFOLDERLINKICONS.Enabled = Not .IsReadOnly("ShowFolderLinkIcons")

            chkSHOWGLOBALSEARCH.Checked = .ShowGlobalSearch
            chkSHOWGLOBALSEARCH.Enabled = Not .IsReadOnly("ShowGlobalSearch")

            chkSHOWLOCKEDDOSSIERS.Checked = .ShowLockedDossiers
            chkSHOWLOCKEDDOSSIERS.Enabled = Not .IsReadOnly("ShowLockedDossiers")

            chkSHOWSUSPENDEDDOSSIERS.Checked = .ShowSuspendedDossiers
            chkSHOWSUSPENDEDDOSSIERS.Enabled = Not .IsReadOnly("ShowSuspendedDossiers")

            chkSHOWGRIDSIDEBAR.Checked = .ShowGridSideBar
            chkSHOWGRIDSIDEBAR.Enabled = Not .IsReadOnly("ShowGridSideBar")

            chkSHOWFILESINSEPARATEWINDOW.Checked = .ShowFilesInSeparateWindow
            chkSHOWFILESINSEPARATEWINDOW.Enabled = Not .IsReadOnly("ShowFilesInSeparateWindow")

            chkSHOWDELEGATEDDOSSIERS.Checked = Not .HideDelegatedWork
            chkSHOWDELEGATEDDOSSIERS.Enabled = Not .IsReadOnly("HideDelegatedWork")

            chkSHOWSUCCESSMESSAGES.Checked = .ShowSuccessMessages
            chkSHOWSUCCESSMESSAGES.Enabled = Not .IsReadOnly("ShowSuccessMessages")

            chkOPENDETAILWINDOWMAXIMIZED.Checked = .OpenDetailWindowMaximized
            chkOPENDETAILWINDOWMAXIMIZED.Enabled = Not .IsReadOnly("OpenDetailWindowMaximized")

            chkOPENNEXTCASEONCLOSE.Checked = .OpenNextCaseOnClose
            chkOPENNEXTCASEONCLOSE.Enabled = Not .IsReadOnly("OpenNextCaseOnClose")

            chkEnableContextMenus.Checked = .EnableContextMenus
            chkEnableContextMenus.Enabled = Not .IsReadOnly("EnableContextMenus")


            chkShowFileToolbar.Checked = .ShowFileToolbar
            chkShowFileToolbar.Enabled = Not .IsReadOnly("ShowFileToolbar")

            txtRecsperPage.Text = .RecordsPerPage
            txtRecsperPage.Enabled = Not .IsReadOnly("RecordsPerPage")
        End With

        'notifications
        chkSelectAllNotifs.Text = GetLabel("toggleselection")
        chkSelectAllNotifs.Attributes.Add("onclick", "checkAll(this,'" & chkFilteredNotifs.ClientID & "');")
        chkFilteredNotifs.Items.Clear()
        Dim lcolAllnotifs As NotificationTemplateList = NotificationTemplateList.GetNotificationTemplateList()
        If lcolAllnotifs.Any Then
            Dim notifLabels As Globalisation.LABELList = Globalisation.LABELList.GetNotificationTemplatesLabelList(EnableIISCaching)
            Dim lcolFilteredNotifs As NotificationUserFilterList = NotificationUserFilterList.GetFilters(Account)
            Dim hasChecked As Boolean = False
            For Each loNotif As NotificationTemplateList.NotificationTemplateInfo In lcolAllnotifs
                If Not loNotif.Archive Then
                    Dim checked As Boolean = Not lcolFilteredNotifs.Contains(loNotif.ID)
                    If checked Then
                        hasChecked = True
                    End If
                    chkFilteredNotifs.Items.Add(New WebControls.ListItem(notifLabels.GetObjectLabel(loNotif.ID, "Notification", loNotif.Name), loNotif.ID.ToString) With {.Selected = checked})
                End If
            Next
            chkSelectAllNotifs.Checked = hasChecked
        Else
            radTab.Tabs(TabPages.Notifications).Visible = False
        End If


        Dim selectedTheme As String = UserProfileToEdit.Theme
        If String.IsNullOrEmpty(selectedTheme) Then
            If thisUser.IsTenantUser Then
                Dim userTenant As TenantMembership = thisUser.Tenants.FirstOrDefault()
                If userTenant IsNot Nothing Then
                    selectedTheme = Tenant.GetTenant(userTenant.TenantId).DefaultTheme
                End If
            End If
            If String.IsNullOrEmpty(selectedTheme) Then
                selectedTheme = ArcoInfoSettings.DefaultTheme
            End If
        End If

        drpTheme.Items.Clear()
        For Each t As String In AllThemes
            drpTheme.Items.Add(New System.Web.UI.WebControls.ListItem(t) With {.Selected = (selectedTheme.Equals(t, StringComparison.CurrentCultureIgnoreCase))})
        Next
        drpTheme.Enabled = Not UserProfileToEdit.IsReadOnly("Theme")

        drpOnCloseWindow.Items.Clear()
        drpOnCloseWindow.Items.Add(New System.Web.UI.WebControls.ListItem(GetDecodedLabel("pref_onclose_keeplocked"), "0") With {.Selected = (UserProfileToEdit.OnCloseEditWindow = Arco.Doma.Library.UserProfile.CloseBeheaviour.KeepLocked)})
        drpOnCloseWindow.Items.Add(New System.Web.UI.WebControls.ListItem(GetDecodedLabel("pref_onclose_unlock"), "1") With {.Selected = (UserProfileToEdit.OnCloseEditWindow = Arco.Doma.Library.UserProfile.CloseBeheaviour.Unlock)})
        drpOnCloseWindow.Items.Add(New System.Web.UI.WebControls.ListItem(GetDecodedLabel("pref_onclose_confirm"), "2") With {.Selected = (UserProfileToEdit.OnCloseEditWindow = Arco.Doma.Library.UserProfile.CloseBeheaviour.Confirm)})
        drpOnCloseWindow.Enabled = Not UserProfileToEdit.IsReadOnly("OnCloseEditWindow")

        drpDensity.Items.Clear()
        Dim densityOptions As Array = [Enum].GetValues(GetType(LayoutDensity))
        For Each densityOption As LayoutDensity In densityOptions
            drpDensity.Items.Add(New WebControls.ListItem(GetLabel(densityOption.ToString()), CInt(densityOption).ToString()) With {.Selected = (UserProfileToEdit.LayoutDensity = densityOption)})
        Next
        drpDensity.Enabled = Not UserProfileToEdit.IsReadOnly("LayoutDensity")

    End Sub
    Private Sub TranslateCheckBox(ByRef chk As CheckBox, ByVal vsName As String)
        Dim lsLabel As String
        Try
            lsLabel = GetLabel("pref_" & vsName)
        Catch ex As Exception
            lsLabel = ""
        End Try
        If Not String.IsNullOrEmpty(lsLabel) Then
            chk.Text = lsLabel
        End If
    End Sub

    Private Sub TranslateCheckBoxes()
        TranslateCheckBox(chkShowFilters, "showfilters")
        TranslateCheckBox(chkShowFoldersInList, "ShowFoldersInList")
        TranslateCheckBox(chkShowPreview, "ShowPreview")
        TranslateCheckBox(chkShowPreviewInDetailWindows, "ShowPreview")
        TranslateCheckBox(chkShowCurrentFolder, "ShowCurrentFolder")
        TranslateCheckBox(chkShowQuery, "ShowQuery")
        TranslateCheckBox(chkShowSearchInList, "ShowSearchScreenInList")
        TranslateCheckBox(chkALWAYSSHOWATTACHMENTS, "AlwaysShowAttachments")
        TranslateCheckBox(chkShowTree, "ShowTree")
        TranslateCheckBox(chkAUTOOPENFILEINDETAIL, "AutomaticlyOpenFileInDetail")
        TranslateCheckBox(chkSHOWFOLDERLINKICONS, "ShowFolderLinkIcons")
        TranslateCheckBox(chkSHOWGLOBALSEARCH, "SHOWGLOBALSEARCH")
        TranslateCheckBox(chkShowQuery, "showquery")
        TranslateCheckBox(chkSHOWLOCKEDDOSSIERS, "showlockedcases")
        TranslateCheckBox(chkSHOWSUSPENDEDDOSSIERS, "showsuspendedcases")
        TranslateCheckBox(chkSHOWGRIDSIDEBAR, "showgridsidebar")
        TranslateCheckBox(chkSHOWFILESINSEPARATEWINDOW, "showfilesinseparatewindow")
        TranslateCheckBox(chkSHOWDELEGATEDDOSSIERS, "defaultshowdelegatedwork")
        TranslateCheckBox(chkSHOWSUCCESSMESSAGES, "showsuccessmessages")
        TranslateCheckBox(chkOPENDETAILWINDOWMAXIMIZED, "opendetailwindowmaximized")
        TranslateCheckBox(chkEnableContextMenus, "enablecontextmenus")
        TranslateCheckBox(chkShowFileToolbar, "showfiletoolbar")
        chkOPENNEXTCASEONCLOSE.Text = GetDecodedLabel("opennextcaseonclose")
        lblRecsPerpage.Text = GetLabel("recsperpage")
        lblOnCloseWindow.Text = GetLabel("pref_onclosewindow")
    End Sub

    Protected Function GetTenantLabel(ByVal tnid As Integer) As String
        If tnid = 0 Then Return GetLabel("all")
        Return GetTenantName(tnid)

    End Function
    Private Sub CheckDomainSettings()


        If Not IsPostBack Then
            Dim lsDomainname As String = ""
            trDomain.Visible = True
            If UpdateModus Then
                If txtLoginName.Text.Contains("\") Then
                    lsDomainname = txtLoginName.Text.Substring(0, txtLoginName.Text.IndexOf("\"))
                Else
                    trDomain.Visible = False
                End If
            Else
                lsDomainname = Arco.Security.BusinessIdentity.CurrentIdentity.Domain
            End If


            drpdwnDomains.Items.Add(New Telerik.Web.UI.RadComboBoxItem("", ""))

            For Each dom As DOMAINList.DOMAINSInfo In DOMAINList.GetDOMAINSList()
                Dim newItem As New Telerik.Web.UI.RadComboBoxItem(dom.DOMAIN_NAME, dom.DOMAIN_NAME)
                If dom.DOMAIN_NAME = lsDomainname Then
                    newItem.Selected = True
                End If
                drpdwnDomains.Items.Add(newItem)
            Next

            If drpdwnDomains.Items.Count = 1 Then
                trDomain.Visible = False
            End If

        End If

    End Sub
    Private Sub CheckFocus()
        If UpdateModus Then
            txtLoginName.ReadOnly = True
            txtLoginName.BackColor = Color.Transparent
            txtLoginName.BorderStyle = BorderStyle.None
            SetFocus(txtDisplayName)
            drpdwnDomains.Enabled = False
        Else
            SetFocus(txtLoginName)

        End If
    End Sub


    Protected Sub doUnlock(ByVal sender As Object, ByVal e As EventArgs)
        SetInfoMessage(Routing.BatchUnlock.UnlockCases(Account) & " " & GetLabel("workflowsunlocked"))
    End Sub
    Private Sub DoCancel()
        Dim lsReturnTo As String = ViewState("RefUrl").ToString
        If Not lsReturnTo.ToLower.Contains("dm_userbrowser_rightpane") Then
            lsReturnTo = "dm_userbrowser_rightpane.aspx"
        End If
        Response.Redirect(lsReturnTo)
    End Sub
    Protected Sub doDelete(ByVal sender As Object, ByVal e As EventArgs)
        Dim s As String = ACL.User.DeleteUserWithCheck(Account)
        If Not String.IsNullOrEmpty(s) Then
            SetErrorMessage(s)
        Else
            DoCancel()
        End If
    End Sub
    Private Sub SetInfoMessage(ByVal t As String)
        lblMessage.Visible = True
        lblMsgText.Text = t
        lblMsgText.CssClass = "InfoLabel"
    End Sub
    Private Sub SetErrorMessage(ByVal t As String)

        lblMessage.Visible = True
        If Not String.IsNullOrEmpty(lblMsgText.Text) Then
            lblMsgText.Text &= "<br/>" & t
        Else
            lblMsgText.Text = t
        End If
        lblMsgText.CssClass = "ErrorLabel"
    End Sub

    Protected Sub DoSave(ByVal sender As Object, ByVal e As EventArgs)
        SetInfoMessage("")

        If Not Page.IsValid Then
            SetErrorMessage(GetLabel("validatorerror"))
            Exit Sub
        End If

        Dim luUser As User
        _login = txtLoginName.Text.Trim()

        If Not UpdateModus Then
            'insert:           

            'precheck, will be checked on save also
            If Rules.isReservedName(_login) Then
                SetErrorMessage(Server.HtmlEncode(_login) & " " & GetLabel("isreservedword"))
                Exit Sub
            End If
            Dim invalidChar As String = Nothing
            If Rules.ContainsInvalidCharacters(_login, invalidChar) Then
                SetErrorMessage(Server.HtmlEncode(_login) & " " & GetLabel("containsinvalidchars") & ": " & Server.HtmlEncode(invalidChar))
                Exit Sub
            End If


            If drpdwnDomains.Items.Any AndAlso Not String.IsNullOrEmpty(drpdwnDomains.SelectedItem.Text) Then
                _login = drpdwnDomains.SelectedItem.Text & "\" & _login
            End If

            If ACL.User.Exists(_login) Then
                SetErrorMessage(String.Format(LibError.GetErrorDescription(LibError.ErrorCode.ERR_ITEMEXISTS), GetLabel("user").ToLower))
                Exit Sub
            End If

            luUser = ACL.User.NewUser(_login)

            If String.IsNullOrEmpty(txtNewPassWord.Text) Then
                SetErrorMessage(GetLabel("password") & " " & GetLabel("req"))
                Exit Sub
            End If

            If Not SetPassword(luUser) Then
                Exit Sub
            End If


            RequiredFieldValidator1.Enabled = False
            RequiredFieldValidator2.Enabled = False



        Else
            'update:
            luUser = ACL.User.GetUser(_login)
            If Not SetPassword(luUser) Then
                Exit Sub
            End If
        End If

        '  luUser.USER_LOGIN = _login
        luUser.USER_DISPLAY_NAME = InputSanitizer.Sanitize(txtDisplayName.Text, True, True)
        luUser.USER_AUTH_METHOD = CType(drpAuthMethod.SelectedValue, User.AuthenticationMethod)

        luUser.USER_MAIL = txtEmail.Text
        luUser.USER_DESC = InputSanitizer.Sanitize(txtDescription.Text, True, True)
        luUser.USER_STATUS = CType(Convert.ToInt32(drpdwnStatus.SelectedItem.Value), User.UserStatus)
        luUser.USER_LANGCODE = drpdwnLanguage.SelectedItem.Value

        If EnableContactInfo Then
            luUser.USER_FIRSTNAME = InputSanitizer.Sanitize(txtFirstName.Text, True, True)
            luUser.USER_LASTNAME = InputSanitizer.Sanitize(txtLastName.Text, True, True)

            luUser.USER_PHONE = InputSanitizer.Sanitize(txtPhone.Text)
            luUser.USER_FAX = InputSanitizer.Sanitize(txtFax.Text)
            luUser.USER_COMPANY = InputSanitizer.Sanitize(txtCompany.Text, True, True)
            luUser.USER_ADDRESS1 = InputSanitizer.Sanitize(txtAddress1.Text, True, True)
            luUser.USER_ADDRESS2 = InputSanitizer.Sanitize(txtAddress2.Text, True, True)
            luUser.USER_CITY = InputSanitizer.Sanitize(txtCity.Text, True, True)
            luUser.USER_ZIP = InputSanitizer.Sanitize(txtZip.Text)
            luUser.USER_STATE = InputSanitizer.Sanitize(txtState.Text, True, True)
            luUser.USER_COUNTRY = InputSanitizer.Sanitize(txtCountry.Text, True, True)
        End If

        If EnableSpecialCode Then
            luUser.USER_SPECIALCODE = InputSanitizer.Sanitize(txtSpecialCode.Text)
        End If

        luUser.EnableNotifications = chkNotifications.Checked
        luUser.PasswordLocked = chkLockPass.Checked
        luUser.PasswordExpired = chkExpPass.Checked
        luUser.PasswordNeverExpires = chkNeverExpPass.Checked

        If Not FromUserProfile AndAlso MultiTenant Then

            If chkIsTenantUser.Checked Then
                luUser.IsTenantUser = True
                If Not UpdateModus Then
                    luUser.AddToTenant(Security.BusinessIdentity.CurrentIdentity.Tenant)
                End If
            Else
                luUser.IsTenantUser = False
            End If

        End If

        Try

            luUser = luUser.Save()

            SetInfoMessage(GetLabel("saveok"))

            Dim lbCanHavePass As Boolean = (luUser.USER_AUTH_METHOD = ACL.User.AuthenticationMethod.Database OrElse (luUser.USER_AUTH_METHOD = ACL.User.AuthenticationMethod.WindowsDomain AndAlso Not Settings.GetValue("Security", "ValidateAgainstWindowsDomain", True)))
            ShowHidePasswordFields(lbCanHavePass)

            txtLoginName.Text = luUser.USER_LOGIN

            NotificationUserFilter.DeleteAllNotificationUserFilter(luUser.USER_ACCOUNT)
            For Each itm As WebControls.ListItem In chkFilteredNotifs.Items
                If Not itm.Selected Then
                    NotificationUserFilter.AddNotificationUserFilter(Convert.ToInt32(itm.Value), luUser.USER_ACCOUNT)
                End If
            Next

            SaveSubscriptions()
            LoadSubscriptions()

            ctlDelegates.SubjectType = "User"
            ctlDelegates.SubjectID = Account
            ctlDelegates.Reload()

            If UpdateModus Then

                If radTab.Tabs(TabPages.Licencing).Visible Then
                    Try
                        Licensing.License.SetUserNamed(luUser.USER_LOGIN, Arco.Licencing.Modules.WEB, chkNamedWeb.Checked)
                    Catch ex As Exception
                        chkNamedWeb.Checked = False
                        SetErrorMessage(Arco.Utils.ExceptionHelper.GetInnerMostException(ex).Message)
                    End Try

                    Try
                        Licensing.License.SetUserNamed(luUser.USER_LOGIN, Arco.Licencing.Modules.WEBAPI, chkNamedWebAPI.Checked)
                    Catch ex As Exception
                        chkNamedWebAPI.Checked = False
                        SetErrorMessage(Arco.Utils.ExceptionHelper.GetInnerMostException(ex).Message)
                    End Try
                End If


                UserProfileToEdit.ShowFilters = chkShowFilters.Checked
                UserProfileToEdit.ShowFoldersInList = chkShowFoldersInList.Checked
                UserProfileToEdit.ShowPreview = chkShowPreview.Checked
                UserProfileToEdit.ShowPreviewInDetailWindows = chkShowPreviewInDetailWindows.Checked
                UserProfileToEdit.ShowCurrentFolder = chkShowCurrentFolder.Checked
                UserProfileToEdit.ShowQuery = chkShowQuery.Checked
                UserProfileToEdit.ShowSearchScreenInList = chkShowSearchInList.Checked
                UserProfileToEdit.AlwaysShowAttachments = chkALWAYSSHOWATTACHMENTS.Checked
                UserProfileToEdit.ShowTree = chkShowTree.Checked
                UserProfileToEdit.AutoOpenFileInDetail = chkAUTOOPENFILEINDETAIL.Checked
                UserProfileToEdit.ShowFolderLinkIcons = chkSHOWFOLDERLINKICONS.Checked
                UserProfileToEdit.ShowGlobalSearch = chkSHOWGLOBALSEARCH.Checked
                UserProfileToEdit.ShowLockedDossiers = chkSHOWLOCKEDDOSSIERS.Checked
                UserProfileToEdit.ShowSuspendedDossiers = chkSHOWSUSPENDEDDOSSIERS.Checked
                UserProfileToEdit.ShowGridSideBar = chkSHOWGRIDSIDEBAR.Checked
                UserProfileToEdit.ShowFilesInSeparateWindow = chkSHOWFILESINSEPARATEWINDOW.Checked
                UserProfileToEdit.HideDelegatedWork = Not chkSHOWDELEGATEDDOSSIERS.Checked
                UserProfileToEdit.ShowSuccessMessages = chkSHOWSUCCESSMESSAGES.Checked
                UserProfileToEdit.OpenDetailWindowMaximized = chkOPENDETAILWINDOWMAXIMIZED.Checked
                UserProfileToEdit.OpenNextCaseOnClose = chkOPENNEXTCASEONCLOSE.Checked
                UserProfileToEdit.EnableContextMenus = chkEnableContextMenus.Checked
                UserProfileToEdit.ShowFileToolbar = chkShowFileToolbar.Checked

                If Not String.IsNullOrEmpty(txtRecsperPage.Text) Then
                    Dim i As Int32
                    If Int32.TryParse(txtRecsperPage.Text, i) Then
                        If i > 100 Then
                            i = 100
                        End If
                        UserProfileToEdit.RecordsPerPage = i.ToString
                        txtRecsperPage.Text = i.ToString
                    Else
                        txtRecsperPage.Text = UserProfileToEdit.RecordsPerPage
                    End If
                Else
                    UserProfileToEdit.RecordsPerPage = ""
                End If

                UserProfileToEdit.OnCloseEditWindow = CType(drpOnCloseWindow.SelectedValue, UserProfile.CloseBeheaviour)
                UserProfileToEdit.Theme = drpTheme.SelectedValue
                UserProfileToEdit.LayoutDensity = CType(drpDensity.SelectedValue, LayoutDensity)


                If isCurrentUser() Then

                    Security.BusinessIdentity.CurrentIdentity.Language = drpdwnLanguage.SelectedItem.Value
                    Security.BusinessIdentity.CurrentIdentity.Save()

                    Response.Cookies("DefaultLang").Value = drpdwnLanguage.SelectedItem.Value
                End If


                If FromUserProfile Then
                    Page.ClientScript.RegisterStartupScript(Me.GetType, "closewindow", "Close();", True)
                End If
            Else


                Response.Redirect("DM_UserBrowser_AddUser.aspx?login=" & luUser.USER_ACCOUNT & "&new=false&fromUP=false")
                Exit Sub
            End If

        Catch ex As Exception
            If InStr(luUser.USER_ERROR.ToUpper, "EXISTS") > 0 Then
                UpdateModus = True
                SetErrorMessage(luUser.USER_ERROR)
                LoadUser(_login)
            Else
                Arco.Utils.Logging.LogError(ex.Message, ex)

                SetErrorMessage("An unexpected error has occured executing the request")
            End If
        End Try
        If Not String.IsNullOrEmpty(luUser.USER_ERROR) Then
            SetErrorMessage(luUser.USER_ERROR)
        End If
        CheckFocus()
    End Sub

    Private Sub SaveSubscriptions()
        For Each row As RepeaterItem In repSubscriptins.Items
            Dim chk As CheckBox = DirectCast(row.FindControl("chkSelected"), CheckBox)
            If chk IsNot Nothing Then
                Dim hdn As WebControls.HiddenField = DirectCast(row.FindControl("hdnId"), WebControls.HiddenField)
                Dim subsid As Int32 = Convert.ToInt32(hdn.Value)
                Dim subsc As Subscription = Subscription.GetSubscription(subsid)
                If subsc.ID = subsid Then
                    If subsc.Subject_Type = "User" Then
                        If chk.Checked Then
                            subsc.Status = Subscription.SubscriptionStatus.Enabled
                        Else
                            subsc.Status = Subscription.SubscriptionStatus.Disabled
                        End If
                        subsc.Save()
                    Else
                        If Not chk.Checked Then
                            Subscriptions.SubscriptionUserFilter.AddSubscriptionUserFilter(subsc.ID, Account)
                        Else
                            Subscriptions.SubscriptionUserFilter.DeleteSubscriptionUserFilter(subsc.ID, Account)
                        End If
                    End If
                End If
            End If
        Next

    End Sub

    Private Sub ShowHidePasswordFields(ByVal vbOn As Boolean)
        If vbOn Then
            If Not FromUserProfile Then
                'admin mode              
                rOldPassWord.Visible = False
                trLockPass.Visible = True
                rNewPassWord.Visible = True
                rConfirmNewPassWord.Visible = True

                If chkLockPass.Checked Then
                    chkExpPass.Enabled = False
                    chkExpPass.Checked = False

                    chkNeverExpPass.Enabled = False
                    chkNeverExpPass.Checked = True
                Else
                    chkExpPass.Enabled = True

                    If chkExpPass.Checked Then
                        chkNeverExpPass.Enabled = False
                        chkNeverExpPass.Checked = False
                    End If
                    chkNeverExpPass.Enabled = True
                End If

            Else
                rOldPassWord.Visible = Not chkLockPass.Checked
                trLockPass.Visible = False
                rNewPassWord.Visible = Not chkLockPass.Checked
                rConfirmNewPassWord.Visible = Not chkLockPass.Checked
            End If

            txtOldPassword.Attributes.Add("autocomplete", "off")
            txtNewPassWord.Attributes.Add("autocomplete", "off")
            txtConfirmNewPassword.Attributes.Add("autocomplete", "off")
        Else

            rOldPassWord.Visible = False
            trLockPass.Visible = False
            rNewPassWord.Visible = False
            rConfirmNewPassWord.Visible = False
        End If
    End Sub


    Private Sub ShowAuthMethod(ByVal user As ACL.User)

        Dim authMethod As ACL.User.AuthenticationMethod
        Dim showLdap As Boolean

        If user IsNot Nothing Then
            authMethod = user.USER_AUTH_METHOD
            showLdap = authMethod = User.AuthenticationMethod.LDAP OrElse user.USER_DESC.ToUpper.StartsWith("LDAP:")
        Else
            authMethod = User.AuthenticationMethod.Database
            showLdap = False
        End If
        drpAuthMethod.Items.Clear()
        If authMethod = ACL.User.AuthenticationMethod.WindowsDomain AndAlso Not Settings.GetValue("Security", "ValidateAgainstWindowsDomain", True) Then
            'only integrated authentication is allowed
            drpAuthMethod.Items.Add(New Telerik.Web.UI.RadComboBoxItem("Windows", Convert.ToInt32(ACL.User.AuthenticationMethod.WindowsDomain).ToString))
            rAuthMethod.Visible = False
        Else
            rAuthMethod.Visible = Not FromUserProfile
            drpAuthMethod.Items.Add(New Telerik.Web.UI.RadComboBoxItem("Database", Convert.ToInt32(ACL.User.AuthenticationMethod.Database).ToString) With {.Selected = (authMethod = ACL.User.AuthenticationMethod.Database)})
            If showLdap Then
                drpAuthMethod.Items.Add(New Telerik.Web.UI.RadComboBoxItem("Ldap", Convert.ToInt32(ACL.User.AuthenticationMethod.LDAP).ToString) With {.Selected = (authMethod = ACL.User.AuthenticationMethod.LDAP)})
            End If
            If authMethod = ACL.User.AuthenticationMethod.WindowsDomain OrElse Settings.GetValue("Security", "ValidateAgainstWindowsDomain", True) Then
                'allow windows authentication with passthrough
                drpAuthMethod.Items.Add(New Telerik.Web.UI.RadComboBoxItem("Windows", Convert.ToInt32(ACL.User.AuthenticationMethod.WindowsDomain).ToString) With {.Selected = (authMethod = ACL.User.AuthenticationMethod.WindowsDomain)})
            End If
            If authMethod = ACL.User.AuthenticationMethod.External OrElse Not String.IsNullOrEmpty(Settings.GetValue("Security", "ExternalAuthenticationClass", "")) Then
                drpAuthMethod.Items.Add(New Telerik.Web.UI.RadComboBoxItem("External", Convert.ToInt32(ACL.User.AuthenticationMethod.External).ToString) With {.Selected = (authMethod = ACL.User.AuthenticationMethod.External)})
            End If
        End If
    End Sub
    Dim thisUser As User
    Private Sub LoadUser(ByVal login As String)
        thisUser = ACL.User.GetUser(login.ToUpper)
        If thisUser Is Nothing Then
            Response.Write(login.ToUpper & " not found")
            Response.End()
            Exit Sub
        End If

        If Not FromUserProfile Then
            If Not Security.BusinessIdentity.CurrentIdentity.IsGlobal Then ' todo AndAlso thisUser.TenantId <> Security.BusinessIdentity.CurrentIdentity.Tenant.Id Then

                If Not thisUser.IsTenantUser OrElse Not thisUser.Tenants.Any(Function(x) x.TenantId = Security.BusinessIdentity.CurrentIdentity.Tenant.Id AndAlso x.Status = TenantMembershipStatus.Member) Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                    Exit Sub
                End If
            End If
        End If



        txtLoginName.Text = thisUser.USER_LOGIN
        txtDisplayName.Text = thisUser.USER_DISPLAY_NAME
        txtEmail.Text = thisUser.USER_MAIL
        txtDescription.Text = thisUser.USER_DESC

        If EnableContactInfo Then
            txtFirstName.Text = thisUser.USER_FIRSTNAME
            txtLastName.Text = thisUser.USER_LASTNAME
            txtPhone.Text = thisUser.USER_PHONE
            txtFax.Text = thisUser.USER_FAX
            txtCompany.Text = thisUser.USER_COMPANY
            txtAddress1.Text = thisUser.USER_ADDRESS1
            txtAddress2.Text = thisUser.USER_ADDRESS2
            txtCity.Text = thisUser.USER_CITY
            txtZip.Text = thisUser.USER_ZIP
            txtState.Text = thisUser.USER_STATE
            txtCountry.Text = thisUser.USER_COUNTRY
        End If

        If EnableSpecialCode Then
            txtSpecialCode.Text = thisUser.USER_SPECIALCODE
        End If

        chkNotifications.Checked = thisUser.EnableNotifications
        chkLockPass.Checked = thisUser.PasswordLocked
        chkExpPass.Checked = thisUser.PasswordExpired
        chkNeverExpPass.Checked = thisUser.PasswordNeverExpires

        If Settings.GetValue("General", "MultiTenant", False) Then
            chkIsTenantUser.Checked = thisUser.IsTenantUser
        End If

        If thisUser.IsTempLocked Then
            SetErrorMessage("This user has been temporarily locked, save to unlock")
        End If

        Dim lbCanHavePass As Boolean = thisUser.USER_AUTH_METHOD = ACL.User.AuthenticationMethod.Database OrElse (thisUser.USER_AUTH_METHOD = ACL.User.AuthenticationMethod.WindowsDomain AndAlso Not Settings.GetValue("Security", "ValidateAgainstWindowsDomain", True))
        ShowHidePasswordFields(lbCanHavePass)

        ShowAuthMethod(thisUser)

        Dim tmpItem As Telerik.Web.UI.RadComboBoxItem
        Dim i As Integer
        For i = 0 To drpdwnStatus.Items.Count - 1
            tmpItem = drpdwnStatus.Items(i)
            If Convert.ToInt32(tmpItem.Value) = thisUser.USER_STATUS Then
                drpdwnStatus.SelectedIndex = i
            End If
        Next
        For i = 0 To drpdwnLanguage.Items.Count - 1
            tmpItem = drpdwnLanguage.Items(i)
            If tmpItem.Value = thisUser.USER_LANGCODE Then
                drpdwnLanguage.SelectedIndex = i

            End If
        Next

        chkNamedWeb.Checked = Licensing.License.UserIsNamed(thisUser.USER_LOGIN, Arco.Licencing.Modules.WEB)
        chkNamedWebAPI.Checked = Licensing.License.UserIsNamed(thisUser.USER_LOGIN, Arco.Licencing.Modules.WEBAPI)

        LoadTenants()

        LoadRoles()

        LoadGroups()


        LoadSubscriptions()
        LoadMFA()
        LoadOAuth()
        LoadAudit()
    End Sub

    Private Sub LoadRoles()
        Dim dt As New DataTable()
        dt.Columns.Add("TYPE")
        dt.Columns.Add("NAME")
        dt.Columns.Add("TenantId", GetType(Int32))


        Dim hasRoles As Boolean = False

        If Not FromUserProfile Then

            Dim crit As New RoleMemberList.Criteria With {
                .MEMBER = thisUser.USER_ACCOUNT,
                .TYPE = "User"
            }
            For Each role As RoleMemberList.RoleMemberInfo In RoleMemberList.GetRoleMemberList(crit)
                dt.Rows.Add(New String() {"Role", role.ROLENAME, role.TenantId}) 'todo desc
                hasRoles = True
            Next
        Else

            For Each role As String In Security.BusinessIdentity.CurrentIdentity.Roles
                dt.Rows.Add(New String() {"Role", role, 0}) 'todo tenantid +  desc
                hasRoles = True
            Next
        End If

        If Not hasRoles Then
            trRoleFooter.Visible = True
            tdNoRolesFound.ColSpan = If(ShowRoleTenant, 3, 2)
            lblNoRolesFound.Text = GetLabel("up_norolesfound")
        End If

        dt.DefaultView.Sort = "NAME ASC"
        repRoles.DataSource = dt
        repRoles.DataBind()

    End Sub

    Private Sub LoadGroups()

        Dim groups As List(Of String)

        If Not FromUserProfile Then
            groups = thisUser.Groups
        Else
            groups = New List(Of String)
            For Each grp As String In Security.BusinessIdentity.CurrentIdentity.Groups
                groups.Add(grp)
            Next
        End If

        If groups.Count <> 0 Then
            repGroups.Visible = True

            Dim dt As New DataTable()
            dt.Columns.Add("TYPE")
            dt.Columns.Add("NAME")


            For Each grp As String In groups
                dt.Rows.Add(New String() {"Group", grp}) 'todo desc
            Next

            dt.DefaultView.Sort = "NAME ASC"
            repGroups.DataSource = dt
            repGroups.DataBind()
        Else
            repGroups.Visible = False
        End If

    End Sub

    Private Sub LoadTenants()


        If MultiTenant AndAlso Security.BusinessIdentity.CurrentIdentity.IsGlobal Then

            chkIsTenantUser.Enabled = Not FromUserProfile
            rowTenant.Visible = True
        Else
            rowTenant.Visible = False
        End If


        If Not chkIsTenantUser.Checked Then
            radTab.Tabs(TabPages.Tenants).Visible = False
            Exit Sub
        Else
            radTab.Tabs(TabPages.Tenants).Visible = True
        End If
        Dim tenants As TenantList = TenantList.GetTenants(_login)
        If tenants.Count > 0 Then
            repTenants.DataSource = tenants
            repTenants.DataBind()
        Else
            trTenantFooter.Visible = True
            lblNoTenantsFound.Text = "no tenants found"
        End If

    End Sub

    Private Sub LoadAudit()

        If Not radTab.Tabs(TabPages.Audit).Visible Then
            Exit Sub
        End If
        Dim dt As New DataTable()
        dt.Clear()
        dt.Columns.Add("DATE")
        dt.Columns.Add("USER")
        dt.Columns.Add("DESCRIPTION")

        For Each loAudit As Logging.AppAuditList.AppAuditInfo In Logging.AppAuditList.GetUserAuditList(_login)
            Dim lsUSer As String = ArcoFormatting.FormatUserName(loAudit.User)
            Dim lsDesc As String = loAudit.Description
            Dim lsDate As String = ArcoFormatting.FormatDateLabel(loAudit.AuditDate, True, True, False)

            'handle old data
            If loAudit.Action = "ADDMEMBER" AndAlso loAudit.ObjectType = "ROLE" AndAlso Not lsDesc.Contains("to role") Then
                Dim role As Role = Role.GetRole(loAudit.ObjectID)
                If role IsNot Nothing Then
                    lsDesc &= " to role [" & role.Name & "]"
                Else
                    lsDesc &= " to role [" & loAudit.ObjectID & "]"
                End If
            End If

            dt.Rows.Add(New String() {lsDate, lsUSer, Server.HtmlEncode(lsDesc)})
        Next
        If dt.Rows.Count = 0 Then
            trNoAuditFound.Visible = True
            lblNoAuditFound.Text = GetLabel("noresultsfound")
        Else
            trNoAuditFound.Visible = False
        End If

        repAudit.DataSource = dt
        repAudit.DataBind()

    End Sub
    Protected Function CanEditSubscription(dataItem As Object) As Boolean
        Dim loSub As SubscriptionList.SubscriptionInfo = CType(dataItem, SubscriptionList.SubscriptionInfo)
        Return loSub.Subject_Type = "User"
    End Function
    Private Sub LoadMFA()

        If Not Page.IsPostBack Then
            If Not Settings.GetValue("Security", "MFAEnabled", False) OrElse thisUser.USER_AUTH_METHOD <> ACL.User.AuthenticationMethod.Database Then
                radTab.Tabs(TabPages.MFA).Visible = False
                Return
            End If
        End If

        Dim providers As IEnumerable(Of Security.IMFAProvider) = Security.MFAProviderFactory.GetEnabledProviderTypes(Settings)
        If Not providers.Any() Then
            radTab.Tabs(TabPages.MFA).Visible = False
            Return
        End If

        Dim userMFAs As MultiFactorAuthenticationList = MultiFactorAuthenticationList.GetMultiFactorAuthenticationList(Account)
        Dim dt As New DataTable()
        dt.Columns.Add("Name")
        dt.Columns.Add("Enabled", GetType(Boolean))
        dt.Columns.Add("Target")
        For Each provider As Security.IMFAProvider In providers
            Dim userMfa As MultiFactorAuthenticationList.MfaInfo = userMFAs(provider.Type)
            If userMfa IsNot Nothing Then

                Dim target As String = userMfa.Target

                Select Case provider.Type
                    Case "email"
                        'bw compat
                        If String.IsNullOrEmpty(target) Then
                            target = "No e-mail address set"
                        End If
                    Case "otp"
                        If String.IsNullOrEmpty(target) Then
                            target = "App not registered yet"
                        Else
                            target = "App registered"
                        End If
                End Select



                dt.Rows.Add(New String() {provider.Type, True, target})
            Else
                If provider.Type = "email" AndAlso String.IsNullOrEmpty(txtEmail.Text) Then
                    'don't show
                Else
                    dt.Rows.Add(New String() {provider.Type, False, ""})
                End If

            End If
        Next

        repMFA.DataSource = dt
        repMFA.DataBind()

    End Sub

    Protected Sub lnkEnableMfa_Click(sender As Object, e As EventArgs)
        If FromUserProfile Then Return

        Dim link As LinkButton = DirectCast(sender, LinkButton)
        Dim target As String
        Select Case link.CommandArgument
            Case "email"
                target = txtEmail.Text
                'should not happen
                If String.IsNullOrEmpty(target) Then
                    SetErrorMessage("No e-mail address set")
                    Return
                End If
            Case Else
                target = ""
        End Select
        Dim mfa As MultiFactorAuthentication = MultiFactorAuthentication.Enable(Account, link.CommandArgument, target)
        LoadMFA()
    End Sub
    Protected Sub lnkDisableMfa_Click(sender As Object, e As EventArgs)
        If FromUserProfile Then Return

        Dim link As LinkButton = DirectCast(sender, LinkButton)
        MultiFactorAuthentication.Disable(Account, link.CommandArgument)
        LoadMFA()
    End Sub

    Private Sub LoadOAuth()
        Dim providers As Security.OpenAuthProviderList = Security.OpenAuthProviderList.GetProviders

        If providers.Any Then
            Dim accounts As OpenAuthAccountList = OpenAuthAccountList.GetAccounts(Account, "", "")
            Dim dt As New DataTable()
            dt.Columns.Add("Name")
            dt.Columns.Add("Image")
            dt.Columns.Add("Connected", GetType(Boolean))
            dt.Columns.Add("ProviderUserName")
            dt.Columns.Add("Description")

            For Each provider As Security.OpenAuthProviderList.ProviderInfo In providers
                Dim description As String
                If Not String.IsNullOrEmpty(provider.Description) Then
                    description = provider.Description
                Else
                    description = provider.Name
                End If

                Dim linkedaccount As OpenAuthAccountList.AccountInfo = accounts(provider.Name)
                If linkedaccount IsNot Nothing Then
                    dt.Rows.Add(New String() {provider.Name, ResolveClientUrl(provider.ImageUrl), True, linkedaccount.ProviderUserName, description})
                Else
                    dt.Rows.Add(New String() {provider.Name, ResolveClientUrl(provider.ImageUrl), False, "", description})
                End If

            Next

            repOAuth.DataSource = dt
            repOAuth.DataBind()
        Else
            radTab.Tabs(TabPages.OAuth).Visible = False
        End If
    End Sub
    Private Sub LoadSubscriptions()
        Dim subscriptionList As SubscriptionList = SubscriptionList.GetSubscriptionListForUser(txtLoginName.Text)
        If subscriptionList.Any Then
            Dim dt As DataTable = New DataTable()
            dt.Columns.Add("ID")
            dt.Columns.Add("ICON")
            dt.Columns.Add("TEXT")
            dt.Columns.Add("WHEN")
            dt.Columns.Add("TO")
            dt.Columns.Add("CANEDIT", GetType(Boolean))
            dt.Columns.Add("CHECKED", GetType(Boolean))

            Dim filters As IEnumerable(Of Subscriptions.SubscriptionUserFilterList.FilterInfo) = Subscriptions.SubscriptionUserFilterList.GetFilters(Account).Cast(Of Subscriptions.SubscriptionUserFilterList.FilterInfo)()
            Dim jobs As Jobs.JobList = Arco.Doma.Library.Jobs.JobList.GetJobsWithClass("Arco.Doma.Library.BatchJobs.SendSubscriptions")
            For Each loSub As SubscriptionList.SubscriptionInfo In subscriptionList
                Dim icon As String = "." + ThemedImage.GetTreeIconClientUrl(Subscription.GetSubscriptionIcon(loSub.Object_Type), Me)
                Dim title As String = loSub.Object_Label
                Dim schedule As String
                If loSub.Status = baseObjects.Subscription.SubscriptionStatus.Enabled Then
                    If loSub.JobID <> 0 Then
                        Dim loJob As Jobs.JobList.JobInfo = jobs.Cast(Of Jobs.JobList.JobInfo).FirstOrDefault(Function(x) x.ID = loSub.JobID)
                        If loJob IsNot Nothing Then
                            schedule = loJob.Schedule.GetIntervalLabel
                            Select Case loJob.Status
                                Case Arco.Doma.Library.Jobs.Job.JobStatus.Blocked
                                    schedule &= " (" & MyBase.GetDecodedLabel("blocked") & ")"
                                Case Arco.Doma.Library.Jobs.Job.JobStatus.Disabled
                                    schedule &= " (" & MyBase.GetDecodedLabel("disabled") & ")"
                            End Select
                        Else
                            schedule = "Job not found"
                        End If
                    Else
                        schedule = "Never"
                    End If
                Else
                    schedule = GetLabel("disabled")
                End If
                Dim lsTo As String
                Dim lbCanEdit As Boolean
                Dim lbChecked As Boolean
                If loSub.Subject_Type <> "User" Then
                    lsTo = ArcoFormatting.FormatUserName("(" & loSub.Subject_Type & ") " & loSub.Subject_ID)
                    lbCanEdit = False
                    lbChecked = Not filters.Any(Function(x) x.SubscriptionId = loSub.ID)
                Else
                    lsTo = "" 'don't show
                    lbCanEdit = True
                    lbChecked = (loSub.Status = baseObjects.Subscription.SubscriptionStatus.Enabled)
                End If

                If Not String.IsNullOrEmpty(title) Then dt.Rows.Add(New String() {loSub.ID, icon, title, schedule, lsTo, lbCanEdit, lbChecked})
            Next
            repSubscriptins.DataSource = dt
            repSubscriptins.DataBind()
        Else
            radTab.Tabs(TabPages.Subscriptions).Visible = False
        End If


    End Sub

    Private Function GetPassWordComplexityValidator() As Security.PasswordComplexityValidator
        Dim complexityValidator As Security.PasswordComplexityValidator
        If FromUserProfile Then
            complexityValidator = Security.PasswordComplexityValidator.GetValidator(Settings, Security.BusinessIdentity.CurrentIdentity.Name)
            complexityValidator.IllegalPasswords.Add(txtOldPassword.Text)
        Else
            complexityValidator = Security.PasswordComplexityValidator.GetValidator(Settings, Nothing)
        End If

        Return complexityValidator
    End Function
    Private Function SetPassword(ByVal loUser As User) As Boolean
        If Not (String.IsNullOrEmpty(txtNewPassWord.Text) AndAlso String.IsNullOrEmpty(txtConfirmNewPassword.Text)) Then


            Dim complexityValidator As Security.PasswordComplexityValidator = GetPassWordComplexityValidator()

            Dim results As List(Of Security.PasswordComplexityValidator.ResultCode) = complexityValidator.Validate(txtNewPassWord.Text)
            If results.Any Then
                For Each result As Security.PasswordComplexityValidator.ResultCode In results
                    Select Case result
                        Case Security.PasswordComplexityValidator.ResultCode.MaxlengthFailed
                            SetErrorMessage(GetLabel("password") & " " & GetLabel("maxlen").Replace("#FIELDLEN#", complexityValidator.MaxLength.ToString))
                        Case Security.PasswordComplexityValidator.ResultCode.MinlengthFailed
                            SetErrorMessage(GetLabel("password") & " " & GetLabel("minlen").Replace("#FIELDLEN#", complexityValidator.MinLength.ToString))
                        Case Else
                            SetErrorMessage(GetLabel("password") & ": " & GetLabel(result.ToString))
                    End Select
                Next
                Return False
            End If

            If Not FromUserProfile OrElse loUser.MatchPassword(txtOldPassword.Text) Then
                If txtNewPassWord.Text.Equals(txtConfirmNewPassword.Text) Then
                    loUser.SetPassword(txtNewPassWord.Text)
                    txtOldPassword.Text = ""
                    txtNewPassWord.Text = ""
                    txtConfirmNewPassword.Text = ""
                    Return True
                Else
                    SetErrorMessage(GetLabel("passwordmatchfailed"))
                    Return False
                End If
            Else
                SetErrorMessage(GetLabel("invalidoldpassword"))
                Return False
            End If
        Else
            Return True
        End If
    End Function

    Private Sub LoadLabels()
        lblLoginName.Text = GetLabel("ub_loginname")
        lblDisplayName.Text = GetLabel("ub_displayname")

        lblPassword.Text = GetLabel("password")

        Dim complexityValidator As Security.PasswordComplexityValidator = GetPassWordComplexityValidator()
        Dim ruleCodes As List(Of Security.PasswordComplexityValidator.ResultCode) = complexityValidator.GetRuleCodes()
        If ruleCodes.Any Then
            Dim rules As New List(Of String)
            For Each ruleCode As Security.PasswordComplexityValidator.ResultCode In ruleCodes
                Select Case ruleCode
                    Case Security.PasswordComplexityValidator.ResultCode.MaxlengthFailed
                        rules.Add(GetDecodedLabel("maxlen").Replace("#FIELDLEN#", complexityValidator.MaxLength))
                    Case Security.PasswordComplexityValidator.ResultCode.MinlengthFailed
                        rules.Add(GetDecodedLabel("minlen").Replace("#FIELDLEN#", complexityValidator.MinLength))
                    Case Else
                        rules.Add(GetDecodedLabel(ruleCode.ToString))
                End Select
            Next

            lblPasswordTooltipImg.ToolTip = "- " & String.Join(Environment.NewLine & "- ", rules)
        End If

        lblOldPassword.Text = GetLabel("oldpassword")
        lblConfirmPassword.Text = GetLabel("confirmpassword")
        lblEmail.Text = GetLabel("mail_emailaddress")
        lblDescription.Text = GetLabel("description")
        lblAuthentication.Text = GetLabel("authentication")
        lblStatus.Text = GetLabel("Status")
        lblFirstName.Text = GetLabel("ub_firstname")
        lblLastName.Text = GetLabel("ub_lastname")
        lblPhone.Text = GetLabel("ub_phone")
        lblFax.Text = GetLabel("ub_fax")
        lblCompany.Text = GetLabel("ub_company")
        lblAdress1.Text = GetLabel("ub_address1")
        lblAdress2.Text = GetLabel("ub_address2")
        lblCity.Text = GetLabel("ub_city")
        lblZip.Text = GetLabel("ub_zip")
        lblState.Text = GetLabel("ub_state")
        lblCountry.Text = GetLabel("ub_country")
        lblSpecialCode.Text = GetLabel("ub_specialcode")

        lblLanguage.Text = GetLabel("language")
        lblDomains.Text = GetLabel("domain")
        lblTenant.Text = GetLabel("tenantuser")
        lnkSave.Text = GetLabel("save")
        lnkCancel.Text = GetLabel("close")

        radTab.Tabs(TabPages.Details).Text = GetLabel("details")
        radTab.Tabs(TabPages.MemberOf).Text = GetDecodedLabel("memberof")
        radTab.Tabs(TabPages.Preferencs).Text = GetLabel("prefs")
        radTab.Tabs(TabPages.Delegates).Text = GetLabel("delegates")
        radTab.Tabs(TabPages.Notifications).Text = GetLabel("notifications")
        radTab.Tabs(TabPages.Subscriptions).Text = GetLabel("subscriptions")
        radTab.Tabs(TabPages.MFA).Text = "Multi-Factor Authentication" ' GetLabel("mfa")
        radTab.Tabs(TabPages.Audit).Text = "Audit"

        TransferWorkButton.Text = GetLabel("Transfer Work")
        lnkUnlockWorkflows.Text = GetLabel("unlockworkflows")
        lnkDelete.Text = GetLabel("delete")
        chkLockPass.Text = GetLabel("lockpassword")
        chkExpPass.Text = GetLabel("exppassword")
        chkNeverExpPass.Text = GetLabel("neverexppassword")
        chkNotifications.Text = GetLabel("enablenotifs")

        lblDensity.Text = GetLabel("density")
    End Sub

    Private Sub LoadDropdowns()
        If Page.IsPostBack Then
            Exit Sub
        End If

        drpdwnStatus.Items.Add(New Telerik.Web.UI.RadComboBoxItem(Server.HtmlDecode(GetLabel("enabled")), "1")) 'valid
        drpdwnStatus.Items.Add(New Telerik.Web.UI.RadComboBoxItem(Server.HtmlDecode(GetLabel("disabled")), "2")) 'blocked        

        Dim defaultLang As String
        If Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal Then
            defaultLang = Security.BusinessIdentity.CurrentIdentity.Language
        Else
            defaultLang = Security.BusinessIdentity.CurrentIdentity.Tenant.DefaultLanguage
            If String.IsNullOrEmpty(defaultLang) Then
                defaultLang = Security.BusinessIdentity.CurrentIdentity.Language
            End If
        End If
        For Each loLang As Globalisation.LanguageList.LanguageInfo In Globalisation.LanguageList.GetInterfaceLanguageList()
            drpdwnLanguage.Items.Add(New Telerik.Web.UI.RadComboBoxItem(Server.HtmlDecode(loLang.Description), loLang.InterfaceLanguageCode) With {.Selected = (defaultLang = .Value)})
        Next
    End Sub

    Protected Sub lnkConnectOAuth_Click(sender As Object, e As EventArgs)
        If FromUserProfile Then Return

        Dim link As LinkButton = DirectCast(sender, LinkButton)
        Dim client As DotNetOpenAuth.AspNet.IAuthenticationClient = Arco.Doma.OpenAuth2.ClientFactory.GetClient(link.CommandArgument)

        Dim url As Uri = New Uri(Security.BusinessIdentity.CurrentIdentity.BaseUrl & "Auth/oAuth/linkcallback.aspx?__provider__=" & client.ProviderName)

        client.RequestAuthentication(New HttpContextWrapper(HttpContext.Current), url)
    End Sub
    Protected Sub lnkDisConnectOAuth_Click(sender As Object, e As EventArgs)
        If FromUserProfile Then Return

        Dim link As LinkButton = DirectCast(sender, LinkButton)

        OpenAuthAccount.DeleteOpenAuthAccount(link.CommandArgument, Nothing, Security.BusinessIdentity.CurrentIdentity.Name)

        LoadOAuth()
    End Sub


    Protected Sub lnkImpersonate_Click(sender As Object, e As EventArgs)
        Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Impersonate(_login)

        Dim sScript As New StringBuilder


        sScript.Append("parent.parent.location.href= ")
        sScript.Append(EncodingUtils.EncodeJsString("../default.aspx"))
        sScript.Append(";")


        Page.ClientScript.RegisterStartupScript(Me.GetType, "CompleteCloseScript", sScript.ToString, True)
    End Sub

    Private Sub DM_UserBrowser_AddUser_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender

        If FromUserProfile AndAlso ctlDelegates.Count = 0 Then
            If Not Settings.GetValue("Interface", "UserCanSetDelegates", False) AndAlso Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.CanManageDelegations Then
                radTab.Tabs(TabPages.Delegates).Visible = False
            End If
        End If
    End Sub

    Private Sub DM_UserBrowser_AddUser_PreInit(sender As Object, e As EventArgs) Handles Me.PreInit

        FromUserProfile = QueryStringParser.GetBoolean("fromUP")

        If FromUserProfile Then
            MasterPageFile = "~/masterpages/Toolwindow.master"
        Else
            MasterPageFile = "~/masterpages/Empty.master"
        End If
    End Sub
End Class

