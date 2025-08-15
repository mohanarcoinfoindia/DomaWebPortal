Imports Telerik.Web.UI
Imports Arco.Doma.WebControls.SiteManagement
Imports System.Web.Services
Imports Arco.Doma.OpenAuth2
Imports System.Web.Script.Services
Imports Arco.Doma.Library.Website
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects

Partial Class Main
    Inherits BaseMainPage


    Public Sub New()
        AllowGuestAccess = True
    End Sub

    Protected ReadOnly Property SearchBox() As String
        Get
            Dim itm As RadMenuItem = mnuMain.FindItemByValue("globalsearch")
            If Not itm Is Nothing Then
                Return itm.FindControl("txtFilter").ClientID
            End If
            Return ""
        End Get
    End Property

    Private Function StringToSearchModes(ByVal s As String) As List(Of Screen.ScreenSearchMode)
        Dim ar() As String = s.Split(","c)
        Dim loModeList As New List(Of Screen.ScreenSearchMode)
        For Each a As String In ar
            loModeList.Add(CType(a, Screen.ScreenSearchMode))
        Next
        Return loModeList
    End Function


    Private Sub ApplyMainPageQueryString()
        Dim pageConfig As String = QueryStringParser.GetString("page")
        If Not String.IsNullOrEmpty(pageConfig) Then

            Dim mainpage As MainPageList.PageInfo = MainPageList.GetMainPages().Cast(Of MainPageList.PageInfo).FirstOrDefault(Function(x) x.Name.Equals(pageConfig, StringComparison.CurrentCultureIgnoreCase))
            If mainpage IsNot Nothing Then
                Dim configParams As NameValueCollection = HttpUtility.ParseQueryString(mainpage.Querystring)
                For Each key As String In configParams.Keys
                    QueryStringParser.Add(key, TagReplacer.ReplaceTags(configParams(key)))
                Next

                QueryStringParser.Remove("page")
            End If

        End If
    End Sub
    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)

        ApplyMainPageQueryString()

        If Not String.IsNullOrEmpty(hdnTenant.Value) Then
            Arco.Doma.Library.Security.BusinessPrincipal.SwitchTenant(Convert.ToInt32(hdnTenant.Value))
            hdnTenant.Value = ""
        End If

        If Not String.IsNullOrEmpty(QueryStringParser.GetString("TARGET_PACK_ID")) OrElse Not String.IsNullOrEmpty(QueryStringParser.GetString("TARGET_DOSSIER_TYPE")) Then
            QueryStringParser.Add("selectionmode", True)
            If Not String.IsNullOrEmpty(QueryStringParser.GetString("TARGET_PACK_ID")) Then
                QueryStringParser.Add("selectionpack", QueryStringParser.GetString("TARGET_PACK_ID"))
            End If
        End If

        If CurrentFolder Is Nothing Then
            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
            Return
        End If

        If Not IsAjaxRequest() Then
            'resync the external user prefs if needed
            SyncExternalUserPreferences()

            QueryStringParser.AddOrReplace("DM_PARENT_ID", CurrentFolder.ID.ToString)
            QueryStringParser.AddOrReplace("DM_ROOT_ID", PC.RootID.ToString)

            Dim lbHideHeader As Boolean = QueryStringParser.GetBoolean("hideheader", False)

            BuildMenu(QueryStringParser.GetString("menu"), False)

            Dim lbHideTree As Boolean = QueryStringParser.GetBoolean("hidetree", False)
            If Not lbHideTree And UserProfile.ShowTree Then
                Dim lbHasTree As Boolean = False

                Dim lsTree As String = QueryStringParser.GetString("tree")
                Dim lsFirstTree As String = ""

                For Each lnk As TreePageDefinition.TreePageItem In TreePageDefinition.Items
                    Dim lbShow As Boolean = True
                    If Not String.IsNullOrEmpty(lnk.Role) Then
                        lbShow = Arco.Security.BusinessIdentity.CurrentIdentity.IsInRole(lnk.Role)
                    End If
                    If lbShow Then
                        lbHasTree = True
                        'link to tree
                        If CurrentFolder.ID = 0 Then
                            If (Not String.IsNullOrEmpty(lsTree) AndAlso AddXmlExt(lnk.File) = AddXmlExt(lsTree)) OrElse (String.IsNullOrEmpty(lsTree) AndAlso lnk.Selected) Then
                                lsTree = lnk.File
                                Exit For
                            End If
                            If String.IsNullOrEmpty(lsFirstTree) AndAlso lnk.Type = TreePageDefinition.TreePageItem.eItemType.Tree Then
                                lsFirstTree = lnk.File
                            End If
                        End If
                    End If
                Next
                If lbHasTree Then
                    paneTree.ContentUrl = GetRedirectUrl("Tree.aspx" & QueryStringParser.ToString)

                    If CurrentFolder.ID = 0 Then
                        'link parent to tree parent
                        If String.IsNullOrEmpty(lsTree) Then lsTree = lsFirstTree
                        If Not String.IsNullOrEmpty(lsTree) Then
                            Dim treeDef As TreeDefinition = TreeDefinition.GetTreeLayout(lsTree)
                            If Not String.IsNullOrEmpty(treeDef.Root) Then
                                Dim f As Arco.Doma.Library.Folder = Arco.Doma.Library.Folder.GetFolder(Arco.Doma.Library.TagReplacer.ReplaceTags(treeDef.Root))
                                If f IsNot Nothing Then
                                    PC.ParentID = f.ID
                                    QueryStringParser.Add("DM_PARENT_ID", f.ID.ToString)
                                End If
                            End If
                        End If
                    End If

                Else
                    HideTreePane()
                End If
            Else
                HideTreePane()
            End If

            BuildFolderLink()
            AddToggleNewButton(False)

            Dim defaultAction As String = InputSanitizer.Sanitize(QueryStringParser.GetString("defaultaction"))
            If mnuMain.SelectedItem Is Nothing AndAlso String.IsNullOrEmpty(defaultAction) Then
                Dim screenMode As String
                If Not QueryStringParser.Exists("screenmode") Then
                    screenMode = GetDefaultScreenMode()
                    QueryStringParser.Add("screenmode", screenMode)
                Else
                    'get and handle the Command suffix
                    screenMode = MapAction(QueryStringParser.GetString("screenmode"), CurrentFolder.ID = 0)
                    QueryStringParser.Replace("screenmode", screenMode)
                End If

                Select Case screenMode.ToLower
                    Case "page"
                        paneList.ContentUrl = GetRedirectUrl("Page.aspx" & QueryStringParser.ToString)
                    Case "openinline"
                        QueryStringParser.AddOrReplace("inline", "1")
                        QueryStringParser.AddOrReplace("mode", "1")
                        paneList.ContentUrl = GetRedirectUrl("dm_detail.aspx" & QueryStringParser.ToString)
                    Case "editinline"
                        QueryStringParser.AddOrReplace("inline", "1")
                        QueryStringParser.AddOrReplace("mode", "2")
                        paneList.ContentUrl = GetRedirectUrl("dm_detail.aspx" & QueryStringParser.ToString)
                    Case Else
                        paneList.ContentUrl = GetRedirectUrl("DM_DocumentList.aspx" & QueryStringParser.ToString)
                End Select


            ElseIf Not String.IsNullOrEmpty(defaultAction) Then
                Dim sbDefault As New StringBuilder

                sbDefault.AppendLine("   function GotoDefault() { var prm = Sys.WebForms.PageRequestManager.getInstance();")
                sbDefault.AppendLine(" if (prm.get_isInAsyncPostBack())return;")
                sbDefault.AppendLine(defaultAction & "();}")

                If mnuMain.Visible Then
                    sbDefault.AppendLine("Sys.Application.add_load(SelectFirstMenuItem);")
                Else
                    sbDefault.AppendLine("Sys.Application.add_load(GotoDefault);")
                End If
                If Not Page.IsCallback Then
                    Page.ClientScript.RegisterStartupScript(Me.GetType, "GotoDefault", sbDefault.ToString, True)
                End If
            ElseIf mnuMain.Visible AndAlso Not mnuMain.SelectedItem Is Nothing AndAlso Not Page.IsCallback Then
                Page.ClientScript.RegisterStartupScript(Me.GetType, "GotoDefault", "Sys.Application.add_load(SelectFirstMenuItem);", True)
            End If

            paneMenu.Visible = True
            If lbHideHeader Then
                headMain.Visible = False
                'paneMenu.Height = Unit.Pixel(MenuHeight)
                paneMenu.CssClass += " paneMenuHeight"
            Else
                headMain.Visible = True
                'paneMenu.Height = Unit.Pixel(HeaderHeight + MenuHeight)
                paneMenu.CssClass += " paneMenuAndHeaderHeight"
            End If


            If Not mnuMain.Visible AndAlso QueryStringParser.GetBoolean("hidecurrentfolder", False) Then
                'no menu shown
                If lbHideHeader Then
                    paneMenu.Visible = False
                Else
                    'paneMenu.Height = Unit.Pixel(HeaderHeight)
                    paneMenu.CssClass += " paneHeaderHeight"
                    tblMenu.Visible = False
                End If
            End If

            Dim showSideBar As Boolean = False

            paneSideMenu.Visible = showSideBar
            headMain.ShowLogo = Not showSideBar
            headMain.ShowSelectedOnly = showSideBar

            If showSideBar Then
                'logo must have the same height as the header
                'paneSideBarLogo.Height = Unit.Pixel(HeaderHeight)
                paneSideBarLogo.CssClass += " paneHeaderHeight"
                paneSideMenu.Width = Unit.Pixel(SideBarWidth)
            End If

        End If
    End Sub

    'Public Const MenuHeight As Integer = 30
    'Public Const HeaderHeight As Integer = 42
    Public Const SideBarWidth As Integer = 72

    Private Function IsAjaxRequest() As Boolean
        Return radAjx1.IsAjaxRequest OrElse ajxMenu.IsAjaxRequest
    End Function
    Private Sub HideTreePane()
        paneTree.ContentUrl = "Blank.htm"
        paneTree.Visible = False
    End Sub
    Private Shared Function AddXmlExt(ByVal s As String) As String
        If Not s.ToLower.Contains(".xml") Then
            s &= ".xml"
        End If
        Return s
    End Function

    Private Function GetDefaultScreenMode() As String
        If CurrentFolder.ID = 0 Then
            Return GetDefaultRootAction()
        End If

        Return MapAction(CurrentFolder.Default_Tree_Action, False)

    End Function
    Private Function GetDefaultRootAction() As String
        Return MapAction(Settings.GetValue("DOMA", "RootAction", ""), True)
    End Function

    Private Function MapAction(ByVal value As String, ByVal forRoot As Boolean) As String
        value = Arco.Doma.Library.Helpers.TreeActionValueConverter.Convert(value)

        Select Case value.ToLower
            Case "search"
                Return "query"
            Case "browsewithsubfolders", "query", "advsearch", "page"
                Return value
            Case "", "open", "add", "explore"
                Return "browse"
            Case Else
                If forRoot Then
                    Return "browse"
                Else
                    Return value
                End If
        End Select
    End Function

    Private moTreePageDef As TreePageDefinition

    Protected ReadOnly Property TreePageDefinition As TreePageDefinition
        Get
            If moTreePageDef Is Nothing Then
                Dim treePage As String = QueryStringParser.GetString("treepage")
                If Not String.IsNullOrEmpty(treePage) AndAlso Not treePage.ToLower.Contains(".xml") Then
                    treePage &= ".xml"
                End If
                If String.IsNullOrEmpty(treePage) Then
                    moTreePageDef = TreePageDefinition.GetMainTreePage
                Else
                    moTreePageDef = TreePageDefinition.GetTreePageLayout(treePage)
                End If
            End If
            Return moTreePageDef
        End Get
    End Property

    Protected Property HasNewButton As Boolean
        Get
            If Not ViewState("HasNewButton") Is Nothing Then
                Return CType(ViewState("HasNewButton"), Boolean)
            Else
                Return False
            End If

        End Get
        Set(ByVal value As Boolean)
            ViewState("HasNewButton") = value
        End Set
    End Property
    Private _currfolder As DM_OBJECT
    Private ReadOnly Property CurrentFolder As DM_OBJECT
        Get
            If _currfolder Is Nothing Then _currfolder = ObjectRepository.GetObject(PC.GetParentId(True))
            Return _currfolder
        End Get
    End Property
    Private Sub AddToggleNewButton(ByVal vbCallBack As Boolean)
        Dim lbHasNewButton As Boolean
        If Not vbCallBack Then
            lbHasNewButton = (Not mnuMain.Items.FindItemByValue("New") Is Nothing)
            HasNewButton = lbHasNewButton
        Else
            lbHasNewButton = HasNewButton
        End If
        If lbHasNewButton Then

            Dim bCanAdd As Boolean
            Dim packid As Int32
            If String.IsNullOrEmpty(PC.PackageID) OrElse PC.PackageID = "0" Then
                bCanAdd = CurrentFolder.CanAddChild("", True)
            ElseIf Int32.TryParse(PC.PackageID, packid) Then
                bCanAdd = CurrentFolder.CanAddToPackage(packid)
            Else
                bCanAdd = CurrentFolder.CanAddToPackage(PC.PackageID)
            End If

            If bCanAdd Then
                hdnCanAdd.Value = "1"
            Else
                hdnCanAdd.Value = "0"
            End If

            If Not vbCallBack Then
                radAjx1.Controls.Add(New LiteralControl("<script language='javascript'>Sys.Application.add_load(ToggleNewButton);</script>"))
            End If
        End If
    End Sub
    Private Function EnableFolderLinkNavigation() As Boolean
        Return String.IsNullOrEmpty(hdnCaption.Value)
    End Function
    Public Function ShowCurrentFolder() As Boolean
        Dim lbHideCurrentFolder As Boolean = QueryStringParser.GetBoolean("hidecurrentfolder", False)
        If Not lbHideCurrentFolder AndAlso UserProfile.ShowCurrentFolder Then
            Return True
        Else
            Return False
        End If
    End Function
    Private Sub BuildFolderLink()
        If ShowCurrentFolder() Then
            Dim lnkFolder As New FolderLink
            If EnableFolderLinkNavigation() Then
                lnkFolder.ShowFolderEasyBrowser = Settings.GetValue("Interface", "FolderEasyBrowser", False)
                lnkFolder.JavaScriptOpenFunction = "ChangeContentFolder"
            Else
                lnkFolder.ShowFolderEasyBrowser = False
            End If
            lnkFolder.Page = Me
            lnkFolder.FolderID = CurrentFolder.ID
            lnkFolder.TopMostFolderID = PC.RootID
            lnkFolder.ShowImages = UserProfile.ShowFolderLinkIcons
            lblFolderLink.Text = lnkFolder.GetLinkContent(True, hdnCaption.Value)
            lblFolderLink.Visible = True
        Else
            lblFolderLink.Visible = False
        End If

    End Sub

    Protected Sub radAjx1_AjaxRequest(sender As Object, e As AjaxRequestEventArgs) Handles radAjx1.AjaxRequest
        BuildFolderLink()

        AddToggleNewButton(True)

    End Sub
    Protected Sub ajxMenu_AjaxRequest(sender As Object, e As AjaxRequestEventArgs) Handles ajxMenu.AjaxRequest
        BuildMenu(hdnMenu.Value, True)

    End Sub
    Private Sub BuildMenu(ByVal menu As String, ByVal vbCallBack As Boolean)
        mnuMain.Items.Clear()

        If Not vbCallBack Then
            hdnMenu.Value = ""
        Else
            If String.IsNullOrEmpty(menu) Then
                'revert to default
                menu = QueryStringParser.GetString("menu")
            End If
        End If

        If Not String.IsNullOrEmpty(menu) Then
            mnuMain.Visible = True
            If Not menu.ToLower.Contains(".xml") Then
                menu &= ".xml"
            End If

            LoadMenuItems(mnuMain, menu)

            If Not QueryStringParser.GetBoolean("hideglobalsearch", False) Then
                ShowGlobalSearch(mnuMain, False)
            End If
            '   ShowUserMenu(mnuMain)

            Dim searchScreenModes As String = QueryStringParser.GetString("searchscreenmodes")
            Dim loModeList As List(Of Screen.ScreenSearchMode)
            If Not String.IsNullOrEmpty(searchScreenModes) Then
                loModeList = StringToSearchModes(searchScreenModes)
            Else
                loModeList = New List(Of Screen.ScreenSearchMode) From {Screen.ScreenSearchMode.Objects, Screen.ScreenSearchMode.Files, Screen.ScreenSearchMode.Mails}
            End If
            LinkSearchScreens(mnuMain, "Search", loModeList)

            mnuMain.Visible = mnuMain.Items.Count <> 0

        Else
            mnuMain.Visible = False
        End If
    End Sub



    <WebMethod(EnableSession:=True)>
    <ScriptMethod(UseHttpGet:=True)>
    Public Shared Sub SyncExternalUserPreferences()

        Dim currentIdentity As Arco.Doma.Library.Security.BusinessIdentity = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity

        Dim provider As String = currentIdentity.GetExtendedProperty("oauthprovider")
        Dim accesstoken As String = currentIdentity.GetExtendedProperty("accesstoken")
        If String.IsNullOrEmpty(provider) OrElse String.IsNullOrEmpty(accesstoken) Then
            Return
        End If

        Try
            Dim client As IAuthenticationClient = ClientFactory.GetClient(provider)

            Dim data As IDictionary(Of String, String) = client.FetchUserData(accesstoken)


            client.GetClaimsMapper().MapToIdentity(data, currentIdentity)
        Catch ex As Exception
            'error mapping user preferences, nothing we can do untill the next user login
        End Try

    End Sub
End Class
