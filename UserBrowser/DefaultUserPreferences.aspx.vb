
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Helpers
Imports Arco.Doma.Library.Security

Partial Class UserBrowser_DefaultUserPreferences
    Inherits UserBrowserBasePage

    Private Sub UserBrowser_DefaultUserPreferences_Load(sender As Object, e As EventArgs) Handles Me.Load
        AdminShouldBeGlobalUSer = True

        CheckIsAdmin()

        If Not IsPostBack Then
            lnkSave.Text = GetLabel("save")
            lnkApplyToAll.Text = GetDecodedLabel("applytoallusers")
            lnkApplyToAll.OnClientClick = "return confirm('" & GetDecodedLabel("confirmapplytoallusers") & "');"
            lnkApplyThemeToAll.Text = GetDecodedLabel("applythemetoallusers")
            lnkApplyThemeToAll.OnClientClick = "return confirm('" & GetDecodedLabel("confirmapplythemetoallusers") & "');"
            FillPreferencesCheckboxes()
        End If
    End Sub

    Private Sub FillPreferencesCheckbox(ByVal valueCheckbox As CheckBox, ByVal chkReadOnly As Arco.Doma.WebControls.Controls.CheckBox, ByVal setting As DefaultUserSettings.SettingInfo)
        If setting Is Nothing Then Return

        valueCheckbox.Checked = StringConversions.ToBoolean(setting.Value)
        chkReadOnly.Checked = setting.IsReadonly
    End Sub

    Private Sub FillPreferencesTextbox(ByVal valueTextbox As TextBox, ByVal chkReadOnly As Arco.Doma.WebControls.Controls.CheckBox, ByVal setting As DefaultUserSettings.SettingInfo)
        If setting Is Nothing Then Return

        valueTextbox.Text = setting.Value
        chkReadOnly.Checked = setting.IsReadonly
    End Sub
    Private Sub FillPreferencesCheckboxes()


        chkShowQuery.Visible = BusinessIdentity.CurrentIdentity.isAdmin

        TranslateCheckBoxes()

        Dim settings As DefaultUserSettings = DefaultUserSettings.GetDefaultUserSettings()

        FillPreferencesCheckbox(chkShowFilters, chkShowFiltersReadOnly, settings("ShowFilters"))

        FillPreferencesCheckbox(chkShowFoldersInList, chkShowFoldersInListReadOnly, settings("ShowFoldersInList"))

        FillPreferencesCheckbox(chkShowPreview, chkShowPreviewReadOnly, settings("ShowPreview"))

        FillPreferencesCheckbox(chkShowPreviewInDetailWindows, chkShowPreviewInDetailWindowsReadOnly, settings("ShowPreviewInDetailWindows"))

        FillPreferencesCheckbox(chkShowCurrentFolder, chkShowCurrentFolderReadOnly, settings("ShowCurrentFolder"))

        FillPreferencesCheckbox(chkShowQuery, chkShowQueryReadOnly, settings("ShowQuery"))
        FillPreferencesCheckbox(chkShowSearchInList, chkShowSearchInListReadOnly, settings("SHOWSEARCHINLIST"))
        FillPreferencesCheckbox(chkALWAYSSHOWATTACHMENTS, chkALWAYSSHOWATTACHMENTSReadOnly, settings("AlwaysShowAttachments"))
        FillPreferencesCheckbox(chkShowTree, chkShowTreeReadOnly, settings("ShowTree"))
        FillPreferencesCheckbox(chkShowCurrentFolder, chkShowCurrentFolderReadOnly, settings("ShowCurrentFolder"))
        FillPreferencesCheckbox(chkAUTOOPENFILEINDETAIL, chkAUTOOPENFILEINDETAILReadOnly, settings("AutoOpenFileInDetail"))

        FillPreferencesCheckbox(chkSHOWFOLDERLINKICONS, chkSHOWFOLDERLINKICONSReadonly, settings("ShowFolderLinkIcons"))
        FillPreferencesCheckbox(chkSHOWGLOBALSEARCH, chkSHOWGLOBALSEARCHReadOnly, settings("ShowGlobalSearch"))
        FillPreferencesCheckbox(chkSHOWLOCKEDDOSSIERS, chkSHOWLOCKEDDOSSIERSReadOnly, settings("ShowLockedDossiers"))
        FillPreferencesCheckbox(chkSHOWSUSPENDEDDOSSIERS, chkSHOWSUSPENDEDDOSSIERSReadOnly, settings("ShowSuspendedDossiers"))

        FillPreferencesCheckbox(chkSHOWGRIDSIDEBAR, chkSHOWGRIDSIDEBARReadOnly, settings("ShowGridSideBar"))
        FillPreferencesCheckbox(chkSHOWFILESINSEPARATEWINDOW, chkSHOWFILESINSEPARATEWINDOWReadOnly, settings("ShowFilesInSeparateWindow"))
        FillPreferencesCheckbox(chkSHOWDELEGATEDDOSSIERS, chkSHOWDELEGATEDDOSSIERSReadOnly, settings("HideDelegatedWork"))
        FillPreferencesCheckbox(chkSHOWSUCCESSMESSAGES, chkSHOWSUCCESSMESSAGESReadOnly, settings("ShowSuccessMessages"))
        FillPreferencesCheckbox(chkOPENDETAILWINDOWMAXIMIZED, chkOPENDETAILWINDOWMAXIMIZEDReadONly, settings("OpenDetailWindowMaximized"))

        FillPreferencesCheckbox(chkOPENNEXTCASEONCLOSE, chkOPENNEXTCASEONCLOSEReadOnly, settings("OpenNextCaseOnClose"))

        FillPreferencesTextbox(txtRecsperPage, txtRecsperPageReadOnly, settings("RECSPERPAGE"))

        FillPreferencesCheckbox(chkEnableContextMenus, chkEnableContextMenusReadOnly, settings("EnableContextMenus"))
        FillPreferencesCheckbox(chkShowFileToolbar, chkShowFileToolbarReadOnly, settings("ShowFileToolbar"))
        drpTheme.Items.Clear()

        Dim selectedTheme As String = Me.Settings.GetValue("Interface", "DefaultTheme", "")
        For Each t As String In AllThemes
            drpTheme.Items.Add(New System.Web.UI.WebControls.ListItem(t) With {.Selected = (selectedTheme = t)})
        Next

        Dim selectedBeh As UserProfile.CloseBeheaviour
        Dim behSetting As DefaultUserSettings.SettingInfo = settings("OnCloseEditWindow")

        If behSetting IsNot Nothing Then
            selectedBeh = CType(behSetting.Value, UserProfile.CloseBeheaviour)
            drpOnCloseWindowReadOnly.Checked = behSetting.IsReadonly
        End If

        drpOnCloseWindow.Items.Clear()
        drpOnCloseWindow.Items.Add(New System.Web.UI.WebControls.ListItem(GetDecodedLabel("pref_onclose_keeplocked"), "0") With {.Selected = (selectedBeh = UserProfile.CloseBeheaviour.KeepLocked)})
        drpOnCloseWindow.Items.Add(New System.Web.UI.WebControls.ListItem(GetDecodedLabel("pref_onclose_unlock"), "1") With {.Selected = (selectedBeh = UserProfile.CloseBeheaviour.Unlock)})
        drpOnCloseWindow.Items.Add(New System.Web.UI.WebControls.ListItem(GetDecodedLabel("pref_onclose_confirm"), "2") With {.Selected = (selectedBeh = UserProfile.CloseBeheaviour.Confirm)})

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

    Protected Sub doApplyToAll(ByVal sender As Object, ByVal e As EventArgs)
        If Not Page.IsValid Then
            Exit Sub
        End If

        DoSave(sender, e)


        USER_Setting.DeleteAllUserSettings()

        SetInfoMessage(GetLabel("prefresetforallusers"))
    End Sub

    Protected Sub doApplyThemeToAll(ByVal sender As Object, ByVal e As EventArgs)
        If Not Page.IsValid Then
            Exit Sub
        End If

        DoSave(sender, e)


        USER_Setting.DeleteUserSetting(Nothing, "Theme")

        SetInfoMessage(GetLabel("themesetforallusers"))
    End Sub

    Protected Sub DoSave(ByVal sender As Object, ByVal e As EventArgs)

        If Not Page.IsValid Then
            Exit Sub
        End If


        SavePreferencesCheckbox(chkShowFilters, chkShowFiltersReadOnly, "ShowFilters")

        SavePreferencesCheckbox(chkShowFoldersInList, chkShowFoldersInListReadOnly, "ShowFoldersInList")

        SavePreferencesCheckbox(chkShowPreview, chkShowPreviewReadOnly, "ShowPreview")

        SavePreferencesCheckbox(chkShowPreviewInDetailWindows, chkShowPreviewInDetailWindowsReadOnly, "ShowPreviewInDetailWindows")

        SavePreferencesCheckbox(chkShowCurrentFolder, chkShowCurrentFolderReadOnly, "ShowCurrentFolder")

        SavePreferencesCheckbox(chkShowQuery, chkShowQueryReadOnly, "ShowQuery")
        SavePreferencesCheckbox(chkShowSearchInList, chkShowSearchInListReadOnly, "SHOWSEARCHINLIST")
        SavePreferencesCheckbox(chkALWAYSSHOWATTACHMENTS, chkALWAYSSHOWATTACHMENTSReadOnly, "AlwaysShowAttachments")
        SavePreferencesCheckbox(chkShowTree, chkShowTreeReadOnly, "ShowTree")
        SavePreferencesCheckbox(chkShowCurrentFolder, chkShowCurrentFolderReadOnly, "ShowCurrentFolder")
        SavePreferencesCheckbox(chkAUTOOPENFILEINDETAIL, chkAUTOOPENFILEINDETAILReadOnly, "AutoOpenFileInDetail")

        SavePreferencesCheckbox(chkSHOWFOLDERLINKICONS, chkSHOWFOLDERLINKICONSReadonly, "ShowFolderLinkIcons")
        SavePreferencesCheckbox(chkSHOWGLOBALSEARCH, chkSHOWGLOBALSEARCHReadOnly, "ShowGlobalSearch")
        SavePreferencesCheckbox(chkSHOWLOCKEDDOSSIERS, chkSHOWLOCKEDDOSSIERSReadOnly, "ShowLockedDossiers")
        SavePreferencesCheckbox(chkSHOWSUSPENDEDDOSSIERS, chkSHOWSUSPENDEDDOSSIERSReadOnly, "ShowSuspendedDossiers")

        SavePreferencesCheckbox(chkSHOWGRIDSIDEBAR, chkSHOWGRIDSIDEBARReadOnly, "ShowGridSideBar")
        SavePreferencesCheckbox(chkSHOWFILESINSEPARATEWINDOW, chkSHOWFILESINSEPARATEWINDOWReadOnly, "ShowFilesInSeparateWindow")
        SavePreferencesCheckbox(chkSHOWDELEGATEDDOSSIERS, chkSHOWDELEGATEDDOSSIERSReadOnly, "HideDelegatedWork")
        SavePreferencesCheckbox(chkSHOWSUCCESSMESSAGES, chkSHOWSUCCESSMESSAGESReadOnly, "ShowSuccessMessages")
        SavePreferencesCheckbox(chkOPENDETAILWINDOWMAXIMIZED, chkOPENDETAILWINDOWMAXIMIZEDReadONly, "OpenDetailWindowMaximized")

        SavePreferencesCheckbox(chkOPENNEXTCASEONCLOSE, chkOPENNEXTCASEONCLOSEReadOnly, "OpenNextCaseOnClose")

        SavePreferencesTextbox(txtRecsperPage, txtRecsperPageReadOnly, "RECSPERPAGE")

        SavePreferencesDropdown(drpOnCloseWindow, drpOnCloseWindowReadOnly, "OnCloseEditWindow")

        SavePreferencesCheckbox(chkEnableContextMenus, chkEnableContextMenusReadOnly, "EnableContextMenus")
        SavePreferencesCheckbox(chkShowFileToolbar, chkShowFileToolbarReadOnly, "ShowFileToolbar")

        Me.Settings.SaveValue("Interface", "DefaultTheme", drpTheme.SelectedValue)

        SetInfoMessage(GetLabel("saveok"))
    End Sub

    Private Sub SavePreferencesCheckbox(ByVal valueCheckbox As CheckBox, ByVal chkReadOnly As Arco.Doma.WebControls.Controls.CheckBox, ByVal name As String)
        Dim setting As DefaultUserSetting = GetOrCreateSetting(name)
        setting.Value = StringConversions.FromBoolean(valueCheckbox.Checked)
        setting.IsReadonly = chkReadOnly.Checked
        setting.Save()

    End Sub
    Private Sub SavePreferencesTextbox(ByVal valueTextbox As TextBox, ByVal chkReadOnly As Arco.Doma.WebControls.Controls.CheckBox, ByVal name As String)
        Dim setting As DefaultUserSetting = GetOrCreateSetting(name)
        setting.IsReadonly = chkReadOnly.Checked
        setting.Value = valueTextbox.Text
        setting.Save()

    End Sub

    Private Sub SavePreferencesDropdown(ByVal list As DropDownList, ByVal chkReadOnly As Arco.Doma.WebControls.Controls.CheckBox, ByVal name As String)
        Dim setting As DefaultUserSetting = GetOrCreateSetting(name)
        setting.Value = list.SelectedValue
        setting.IsReadonly = chkReadOnly.Checked
        setting.Save()

    End Sub

    Private Function GetOrCreateSetting(ByVal name As String) As DefaultUserSetting
        Dim setting As DefaultUserSetting = DefaultUserSetting.GetSetting(name)
        If setting Is Nothing Then
            setting = DefaultUserSetting.NewSetting(name)
        End If
        Return setting
    End Function

    Private Sub SetInfoMessage(ByVal t As String)
        lblMsgText.Visible = True
        lblMsgText.Text = t
        lblMsgText.CssClass = "InfoLabel"
    End Sub
    Private Sub SetErrorMessage(ByVal t As String)
        lblMsgText.Visible = True
        lblMsgText.Text = t
        lblMsgText.CssClass = "ErrorLabel"
    End Sub
End Class
