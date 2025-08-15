Imports Arco.ApplicationServer.Library.Linq
Imports Arco.ApplicationServer.Library.Shared
Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Website
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Globalisation
Imports Arco.Doma.Library.Routing
Imports Arco.Doma.Library.Routing.StepTypes
Imports Arco.Doma.Library.Security
Imports Arco.Doma.WebControls.SiteManagement
Imports Arco.Doma.WebControls.Toolbar
Imports Arco.Settings
Imports Telerik.Web.UI
Imports ToolbarButtons
Imports Arco.Doma.Library.Settings

Partial Class DM_detail
    Inherits BasePage

    Protected _FolderID As Integer
    Protected _ObjectIDList As String = ""
    Protected _requestedMode As Screen.DetailScreenDisplayMode
    Protected _actualMode As Screen.DetailScreenDisplayMode
    Protected _ParentReload As Boolean
    Protected _RefreshTree As Boolean
    Protected thisObject As DM_OBJECT
    Private _screen As Screen
    Private msDefaultTab As String = ""
    Private mbDisableAutoOpen As Boolean
    Private mbAutoOpentab As Boolean
    Protected InitialTitle As String = ""
    Protected ForceNextCaseOnClose As Boolean

    Protected WithEvents btnDelete As New LinkButton

    Public Modal As Boolean

    Private thisCaseInstance As cCase
    Private _fromRefresh As Boolean
    Private _isCase As Boolean
    Protected isLast As Boolean
    Protected isFirst As Boolean
    Protected NextID As String = ""
    Private moActions As ScreenActionList

    Public ReadOnly Property InLine As Boolean
        Get
            Return QueryStringParser.GetBoolean("inline")
        End Get
    End Property
#Region " DetailView "
    Protected Function DetailViewUrl() As String
        Return QueryStringParser.AppendTo("DM_Detailview.aspx")
    End Function
#End Region

#Region " Toolbar "
    Private Function AddToolbar(ByVal vsTabText As String, ByVal prevToolbar As RadToolBar) As RadToolBar
        If Not prevToolbar Is Nothing AndAlso prevToolbar.Items.Count = 0 Then
            tabToolbar.Tabs(tabToolbar.Tabs.Count - 1).Text = vsTabText
            Return prevToolbar
        End If

        Dim currentToolbar As RadToolBar
        Dim tab As New RadTab
        tab.Text = If(Not String.IsNullOrEmpty(vsTabText), vsTabText, "Tab") 'todo : translate
        tabToolbar.Tabs.Add(tab)

        Dim loNewPageView As New RadPageView

        currentToolbar = New RadToolBar
        currentToolbar.ID = "tlb" & mitabCount.ToString
        mitabCount += 1
        currentToolbar.Style.Add("width", "100%")
        'currentToolbar.Style.Add("width", "100%")
        currentToolbar.OnClientButtonClicking = "onDynamicToolbarClick"
        currentToolbar.AutoPostBack = True
        currentToolbar.OnClientMouseOut = "tlbOnMouseOut"
        currentToolbar.OnClientMouseOver = "tlbOnMouseOver"
        'currentToolbar.SingleClick = ToolBarSingleClick.ToolBar

        'If mbLargeButtons Then
        '    loCurrentToolbar.Height = 54
        'End If
        AddHandler currentToolbar.ButtonClick, AddressOf toolbar_OnClick

        ' loNewPageView.Style.Add("background-color", "#fafafa")

        loNewPageView.Controls.Add(currentToolbar)

        RadMultiPage1.PageViews.Add(loNewPageView)

        _toolbars.Add(currentToolbar.ClientID)

        Return currentToolbar

    End Function
    Private mitabCount As Int32 = 1
    Private mbLastWasSeparator As Boolean
    Private _showTabsInDetailPreview As Boolean

    Private Sub AddToolbarButton(ByRef currentToolbar As RadToolBar, ByRef drpdwn As RadToolBarDropDown, ByVal button As BaseToolbarButton, ByVal script As StringBuilder)
        If currentToolbar Is Nothing Then
            currentToolbar = AddToolbar(GetLabel("general"), Nothing)
        End If
        If _isCase Then
            button.DataBind(thisCaseInstance, _actualMode)
        Else
            button.DataBind(thisObject, _actualMode)
        End If

        button.AddToToolbar(currentToolbar, drpdwn)
        If button.Visible Then
            button.ShowTabsInPreview = _showTabsInDetailPreview
            button.AppendClientScript(script)
        End If
    End Sub

    Private Sub AddToolbarButton(ByRef currentToolbar As RadToolBar, ByVal button As BaseToolbarButton, ByVal script As StringBuilder)
        If button Is Nothing Then
            Return
        End If
        If TypeOf button Is SeparatorButton AndAlso mbLastWasSeparator Then
            Return
        End If
        ' voButton.LargeButton = mbLargeButtons

        If currentToolbar Is Nothing Then
            currentToolbar = AddToolbar(GetLabel("general"), Nothing)
        End If
        If _isCase Then
            button.DataBind(thisCaseInstance, _actualMode)
        Else
            button.DataBind(thisObject, _actualMode)
        End If

        button.AddToToolbar(currentToolbar)
        If button.Visible Then
            button.ShowTabsInPreview = _showTabsInDetailPreview
            button.AppendClientScript(script)

            If TypeOf button Is SeparatorButton Then
                mbLastWasSeparator = True
            Else
                mbLastWasSeparator = False
            End If
        End If
    End Sub

    Dim _toolbars As List(Of String)
    Private Sub BuildToolbar(ByVal showScreenToolbar As Boolean, ByVal showAdminToolbar As Boolean)

        _showTabsInDetailPreview = Settings.GetValue("Interface", "ShowTabsInDetailPreview", True)

        Dim currentToolbar As RadToolBar = Nothing 'toolbarDetail
        Dim sb As New StringBuilder(256)
        _toolbars = New List(Of String)

        If showScreenToolbar AndAlso _actualMode <> Screen.DetailScreenDisplayMode.Admin Then
            Dim loDrpDwn As RadToolBarDropDown = Nothing
            Dim lbInGrouper As Boolean = False

            For Each loAction As ScreenActionList.ScreenActionInfo In moActions
                If loAction.SiteVersion = 8 Then
                    Continue For
                End If

                If Not lbInGrouper Then
                    Select Case loAction.Type
                        Case ScreenAction.ActionType.Tab
                            If Not currentToolbar Is Nothing Then
                                ToolbarTranslator.Translate(currentToolbar, Me)
                            End If
                            currentToolbar = AddToolbar(loAction.Caption, currentToolbar)
                        Case ScreenAction.ActionType.Separator
                            AddToolbarButton(currentToolbar, New SeparatorButton(Me), sb)
                        Case ScreenAction.ActionType.CustomAction
                            AddToolbarButton(currentToolbar, New CustomActionButton(loAction, Me), sb)
                        Case ScreenAction.ActionType.UserEvent
                            AddToolbarButton(currentToolbar, New UserEventButton(loAction, Me), sb)
                        Case ScreenAction.ActionType.EndGrouper
                            lbInGrouper = False
                        Case ScreenAction.ActionType.StartGrouper
                            lbInGrouper = True
                            loDrpDwn = GetToolbarGrouper(loAction)

                        Case Else
                            AddToolbarButton(currentToolbar, New ButtonFactory(Settings).GetToolbarButton(loAction, Me), sb)
                    End Select
                Else
                    Select Case loAction.Type
                        Case ScreenAction.ActionType.Tab
                            'not possible
                        Case ScreenAction.ActionType.Separator
                            AddToolbarButton(currentToolbar, loDrpDwn, New SeparatorButton(Me), sb)
                        Case ScreenAction.ActionType.CustomAction
                            AddToolbarButton(currentToolbar, loDrpDwn, New CustomActionButton(loAction, Me), sb)
                        Case ScreenAction.ActionType.UserEvent
                            AddToolbarButton(currentToolbar, loDrpDwn, New UserEventButton(loAction, Me), sb)
                        Case ScreenAction.ActionType.EndGrouper
                            If loDrpDwn.Buttons.Count <> 0 Then
                                currentToolbar.Items.Add(loDrpDwn)
                                ToolbarTranslator.Translate(loDrpDwn, Me)
                            End If
                            lbInGrouper = False
                        Case ScreenAction.ActionType.StartGrouper 'nesting?
                            'loDrpDwn = New RadToolBarDropDown
                            'loDrpDwn.Text = loAction.Caption
                        Case Else
                            AddToolbarButton(currentToolbar, loDrpDwn, New ButtonFactory(Settings).GetToolbarButton(loAction, Me), sb)
                    End Select
                End If


            Next

            If lbInGrouper Then 'no end grouper supplied
                If Not loDrpDwn Is Nothing AndAlso loDrpDwn.Buttons.Count <> 0 Then
                    currentToolbar.Items.Add(loDrpDwn)
                    ToolbarTranslator.Translate(loDrpDwn, Me)
                End If
            End If

            If Not currentToolbar Is Nothing Then
                ToolbarTranslator.Translate(currentToolbar, Me)
            End If
        End If

        If showAdminToolbar Then
            currentToolbar = AddToolbar(GetLabel("administration"), currentToolbar)
            AddToolbarButton(currentToolbar, New ToggleAdminButton(Me), sb)
            AddToolbarButton(currentToolbar, New SeparatorButton(Me), sb)

            If _isCase Then
                If _actualMode = Screen.DetailScreenDisplayMode.Admin Then
                    AddToolbarButton(currentToolbar, New SaveAndKeepOpenButton(Nothing, Me), sb)
                Else
                    AddToolbarButton(currentToolbar, New UnlockAdminButton(Nothing, Me), sb)
                    AddToolbarButton(currentToolbar, New FinishCaseButton(Nothing, Me), sb)
                    AddToolbarButton(currentToolbar, New DeleteCaseButton(Nothing, Me), sb)
                End If
                If thisCaseInstance.Proc_ID <> 0 Then
                    AddToolbarButton(currentToolbar, New MoveCaseButton(Nothing, Me), sb)
                    AddToolbarButton(currentToolbar, New SeparatorButton(Me), sb)
                    AddToolbarButton(currentToolbar, New HistoryButton(Nothing, Me), sb)
                    AddToolbarButton(currentToolbar, New ProcedureImageButton(Nothing, Me), sb)
                End If
                AddToolbarButton(currentToolbar, New SeparatorButton(Me), sb)
                AddToolbarButton(currentToolbar, New EditWorkButton(Nothing, Me), sb)
                AddToolbarButton(currentToolbar, New EditCaseDeadlinesButton(Nothing, Me), sb)

                ToolbarTranslator.Translate(currentToolbar, Me)
            Else

                If _actualMode = Screen.DetailScreenDisplayMode.Admin Then
                    AddToolbarButton(currentToolbar, New SaveAndKeepOpenButton(Nothing, Me), sb)
                Else
                    AddToolbarButton(currentToolbar, New UnlockAdminButton(Nothing, Me), sb)
                    AddToolbarButton(currentToolbar, New DeleteButton(Nothing, Me), sb)
                End If
                AddToolbarButton(currentToolbar, New HistoryButton(Nothing, Me), sb)

                If Settings.GetValue("Versioning", "EnableDocumentVersioning", True) Then
                    AddToolbarButton(currentToolbar, New VersionHistoryButton(Nothing, Me), sb)
                End If

                AddToolbarButton(currentToolbar, New ViewSubscriptionsButton(Nothing, Me), sb)
                    AddToolbarButton(currentToolbar, New ACLButton(Nothing, Me), sb)
                    ToolbarTranslator.Translate(currentToolbar, Me)

                End If
            End If
        'Dim iHeight As Int32 = 32
        rowTabs.Visible = tabToolbar.Tabs.Count > 1

        'If mbLargeButtons Then
        '    iHeight += 22
        'End If

        'toolbarScroll.Height = Unit.Pixel(iHeight)


        'If mbLargeButtons Then
        '    For Each btn As RadToolBarButton In toolbarScroll.Items
        '        btn.ImagePosition = ToolBarImagePosition.BelowText
        '    Next
        'End If

        If rowTabs.Visible Then
            'iHeight += 23
            radPaneNavigation.CssClass = "detailRadPaneNavigationInclRowTabs"
            'helpButtonEyeCatcher.Style.Add("top", "88px")
        End If

        'radPaneNavigation.Height = Unit.Pixel(iHeight)
        'radPaneNavigation.MinHeight = iHeight
        'tblToolbar.Height = Unit.Pixel(iHeight)

        sb.Append("function EnableButtons(){")
        For Each sTlb As String In _toolbars
            sb.Append("Togglebuttons('")
            sb.Append(sTlb)
            sb.Append("',true);")
        Next
        sb.Append("} function DisableButtons(){")
        For Each sTlb As String In _toolbars
            sb.Append("Togglebuttons('")
            sb.Append(sTlb)
            sb.Append("',false);")
        Next
        sb.Append("}")

        If thisObject Is Nothing OrElse thisObject.Object_Type <> "Mail" Then
            ClientScript.RegisterClientScriptBlock(Me.GetType, "ToolbarJS", sb.ToString, True)
            ClientScript.RegisterStartupScript(Me.GetType, "ToolbarJSInit", "Sys.Application.add_load(DisableButtons);", True)
        End If
    End Sub

    Private Function GetToolbarGrouper(ByVal action As ScreenActionList.ScreenActionInfo) As RadToolBarDropDown
        Dim drpDwn As New RadToolBarDropDown With {
            .Text = action.Caption
        }
        If Not String.IsNullOrEmpty(action.Icon) Then
            If Not action.Icon.StartsWith("~") Then
                drpDwn.ImageUrl = ThemedImage.GetUrl(action.Icon, Me)
            Else
                drpDwn.ImageUrl = action.Icon
            End If

        End If
        Return drpDwn
    End Function

    Private Sub BuildHiddenToolbar()

        Dim loScroller As QueryStringParser.ObjectScroller = QueryStringParser.GetObjectScroller(False)

        Dim toolBarScript As New StringBuilder()

        toolBarScript.Append("function EnableButtons(){} function DisableButtons(){}")

        toolBarScript.Append("function GotoCurrent(){LoadContent('")
        toolBarScript.Append(MakeJSUrl(loScroller.CurrentLink.NavigateUrl))
        toolBarScript.Append("&refr=Y&selectedtab=' + GetContentWindow().GetSelectedTab());}")

        ClientScript.RegisterClientScriptBlock(Me.GetType, "ToolbarJS", toolBarScript.ToString(), True)

    End Sub

#End Region

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init



        ParseQueryString()

        AddParentReload()

        If InLine Then
            radSplitbarMain.Visible = False
            radPaneRightContent.Visible = False
            splitAction.Visible = False
            paneActions.Visible = False
        End If
        InitialTitle = """"""

        Dim loLoaded As BusinessBase
        Try
            loLoaded = QueryStringParser.CurrentDMObject
        Catch ex As Exception
            Arco.Utils.Logging.LogError("Error loading CurrentDMObject", ex)
            GotoErrorPage(LibError.ErrorCode.ERR_UNEXPECTED)
            Return
        End Try

        If loLoaded Is Nothing Then
            If QueryStringParser.CaseTechID > 0 Then
                'try to get to the next case          
                RedirectToNextObject(True, False)
            Else
                'If Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsAuthenticated Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                'Else

                'End If
            End If
            Return
        End If


        Dim lsTitle As String = "no name"
        Dim helpText As String = Nothing

        If TypeOf (loLoaded) Is DM_OBJECT Then
            thisObject = CType(loLoaded, DM_OBJECT)
            If TypeOf thisObject Is Shortcut Then
                Dim objref As DM_OBJECT = CType(thisObject, Shortcut).GetReferencedObject
                QueryStringParser.AddOrReplace("DM_OBJECT_ID", objref.ID.ToString)
                QueryStringParser.AddOrReplace("DM_OBJECT_TYPE", objref.Object_Type)
                QueryStringParser.AddOrReplace("folderid", objref.Parent_ID.ToString)
                RedirectToUrl("DM_Detail.aspx" & QueryStringParser.ToString)
                Return
            End If

            If TypeOf thisObject Is HistoryCase Then
                _actualMode = Screen.DetailScreenDisplayMode.ReadOnly

                If Not thisObject.CanViewMeta AndAlso Not thisObject.CanViewFiles Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                    Return
                End If
            Else
                If thisObject.IsInRecycleBin AndAlso Not isAdmin() Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                    Return
                End If

                If Not thisObject.CanViewMeta AndAlso Not thisObject.CanViewFiles Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                    Return
                End If


                If Not IsPostBack AndAlso _requestedMode = Screen.DetailScreenDisplayMode.ReadOnly Then
                    'open in edit even when asked in read-only in certain cases
                    If thisObject.IsInCreationByCurrentUser() OrElse thisObject.IsLockedByCurrentUser() OrElse (thisObject.Status = DM_OBJECT.Object_Status.Production AndAlso thisObject.CategoryObject.AutoEdit) Then
                        _actualMode = Screen.DetailScreenDisplayMode.Edit
                        QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Mode, "2")
                    End If
                End If

            End If



        Else
            thisCaseInstance = CType(loLoaded, cCase)


            If thisCaseInstance.Tech_ID = 0 Then
                RedirectToNextObject(False, False)
                Return
            End If

            _isCase = True

            If _requestedMode <> Screen.DetailScreenDisplayMode.Admin AndAlso thisCaseInstance.CurrentStep IsNot Nothing AndAlso thisCaseInstance.IsWaitingForChildCases() Then

                Dim subCases As List(Of cCase) = thisCaseInstance.GetChildCases()
                If subCases.Count > 0 Then
                    Dim subCaseWithWork As cCase = subCases.FirstOrDefault(Function(x) x.CurrentUserHasWorkRecursive())
                    If subCaseWithWork IsNot Nothing Then
                        GotoSubCase(subCaseWithWork)
                    Else
                        GotoSubCase(subCases.First())

                    End If
                    Return
                End If
            End If

            thisObject = thisCaseInstance.TargetObject 'todo : remove this line


            If _requestedMode <> Screen.DetailScreenDisplayMode.Admin Then
                If thisCaseInstance.CurrentUserHasWork Then
                    thisCaseInstance = thisCaseInstance.UnSuspend
                    thisCaseInstance = thisCaseInstance.LockCase
                    If thisCaseInstance.IsMyCase Then
                        _actualMode = Screen.DetailScreenDisplayMode.Edit
                    Else
                        'show the case in readonly
                        _actualMode = Screen.DetailScreenDisplayMode.ReadOnly
                    End If


                Else
                    If _requestedMode = Screen.DetailScreenDisplayMode.Edit AndAlso _fromRefresh Then
                        'no longer write access to the case after refresh->goto next or close
                        RedirectToNextObject(False, False)
                    Else
                        If thisCaseInstance.CanView Then
                            _actualMode = Screen.DetailScreenDisplayMode.ReadOnly
                        Else
                            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                            Return
                        End If
                    End If
                End If


            End If


        End If




        _screen = QueryStringParser.GetRequestedScreen(If(_isCase, thisCaseInstance.Proc_ID, 0))

        If _screen IsNot Nothing Then
            'match to see the requested screen is for the current mode
            Select Case _screen.Mode
                Case Screen.ScreenMode.DETAIL, Screen.ScreenMode.CASEARCHIVE
                    If _requestedMode = Screen.DetailScreenDisplayMode.Edit Then
                        _actualMode = Screen.DetailScreenDisplayMode.ReadOnly
                    End If
                Case Screen.ScreenMode.ADMIN

                    _actualMode = Screen.DetailScreenDisplayMode.Admin

                Case Screen.ScreenMode.INSERT, Screen.ScreenMode.INSERTUPDATEDETAIL, Screen.ScreenMode.UPDATE
                    If thisCaseInstance IsNot Nothing Then
                        _screen = Nothing
                    End If
                Case Screen.ScreenMode.STEPDETAIL
                    If thisCaseInstance Is Nothing Then
                        _screen = Nothing
                    End If
                Case Else
                    'not a screen type for a detail screen
                    _screen = Nothing
            End Select

        End If

        If _screen Is Nothing Then


            If Not _isCase Then
                _screen = thisObject.GetDetailScreen(_actualMode, Device.Web, 7)
            Else
                _screen = thisCaseInstance.GetDetailScreen(_actualMode, Device.Web, 7)
            End If



            If _screen Is Nothing Then
                Response.Write(GetLabel("noscreendefined"))
                Response.End()
            End If
        End If



        If Not IsPostBack AndAlso _actualMode <> Screen.DetailScreenDisplayMode.Admin AndAlso _screen.Type = Screen.ScreenSourceType.Url Then
            Dim lsUrl As String = _screen.Source
            If _isCase Then
                lsUrl = TagReplacer.ReplaceTags(lsUrl, thisCaseInstance)
            Else
                lsUrl = TagReplacer.ReplaceTags(lsUrl, thisObject)
            End If
            RedirectToUrl(QueryStringParser.AppendTo(lsUrl))
            Return
        End If

        moActions = _screen.ScreenActions
        If Not String.IsNullOrEmpty(_screen.Screen_Assembly) AndAlso Not String.IsNullOrEmpty(_screen.Screen_Class) Then
            Dim loScreenhandler As IScreenHandler = PluginManager.CreateInstance(Of IScreenHandler)(_screen.Screen_Assembly, _screen.Screen_Class)
            If _isCase Then
                loScreenhandler.onBeforeToolbarRender(moActions, _actualMode, thisCaseInstance)
            Else
                loScreenhandler.onBeforeToolbarRender(moActions, _actualMode, thisObject)
            End If
            BusinessIdentity.CurrentIdentity().RunNormal()
        End If
        If _isCase Then
            moActions.ApplyFilters(thisCaseInstance)

            If Not String.IsNullOrEmpty(_screen.Title) Then
                lsTitle = TagReplacer.ReplaceTags(_screen.Title, thisCaseInstance)
            Else
                lsTitle = thisCaseInstance.Name
            End If

        Else
            moActions.ApplyFilters(thisObject)

            If Not String.IsNullOrEmpty(_screen.Title) Then
                lsTitle = TagReplacer.ReplaceTags(_screen.Title, thisObject)
            Else
                lsTitle = thisObject.Name
            End If

        End If


        If (_actualMode = Screen.DetailScreenDisplayMode.Admin) Then
            lsTitle = "[Admin] " & lsTitle
        End If
        Title = lsTitle

        Dim showAdminToolbar As Boolean
        If _isCase Then
            showAdminToolbar = Not thisCaseInstance.IsVirtual AndAlso thisCaseInstance.CanAdminister
        Else
            showAdminToolbar = thisObject.CanAdminData()
        End If

        AddButtons()


        If _screen.ShowToolbar = ToolbarVisibility.Always OrElse (_screen.ShowToolbar = ToolbarVisibility.AdminOnly AndAlso showAdminToolbar) Then
            BuildToolbar(_screen.ShowToolbar = ToolbarVisibility.Always, showAdminToolbar)
            CreateScroller()
        Else
            BuildHiddenToolbar()
            radPaneNavigation.Visible = False
        End If
        SetupHelp()
        AddDetailWindowScripts()

        InitialTitle = EncodingUtils.EncodeJsString(Title)

        BuildPaneLayout()


        If _actualMode <> _requestedMode Then
            QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Mode, Convert.ToInt32(_actualMode))
        End If

        If Not IsPostBack Then

            MetaDataPane.ContentUrl = DetailViewUrl()
            PreviewPane.ContentUrl = "about:blank"

            AutoOpenPreviewContent()
            InitTitle("")

            SetUpToolbar()

        End If


        'for BW compat
        If thisObject.Status = DM_OBJECT.Object_Status.InCreation AndAlso thisObject.Case_ID = 0 Then
            ClientScript.RegisterClientScriptBlock(GetType(String), "DetailJS", "var isInCreation = true;", True)
        Else
            ClientScript.RegisterClientScriptBlock(GetType(String), "DetailJS", "var isInCreation = false;", True)
        End If

        AddBodyAttributes()

    End Sub

    Private Sub GotoSubCase(ByVal subCase As cCase)

        QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Tech_Id, subCase.Tech_ID.ToString)
        QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Case_Id, subCase.Case_ID.ToString)
        QueryStringParser.Remove("TECH_ID")
        QueryStringParser.Remove("CASE_ID")
        QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Object_Id, subCase.DM_Object_ID.ToString)
        QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Mode, Convert.ToInt32(_requestedMode).ToString)
        RedirectToUrl("DM_Detail.aspx" & QueryStringParser.ToString)

    End Sub

    Public Property HelpText As String

    Private Sub SetupHelp()

        Dim helpLabel As Label
        If _actualMode = Screen.DetailScreenDisplayMode.Edit Then
            If thisCaseInstance IsNot Nothing AndAlso thisCaseInstance.CurrentStep IsNot Nothing Then
                helpLabel = thisCaseInstance.CurrentStep.GetTooltipLabel()
                If helpLabel IsNot Nothing Then
                    HelpText = helpLabel.TranslatedLabel(Language, Nothing)
                End If
            End If
            If String.IsNullOrEmpty(HelpText) Then
                helpLabel = _screen.GetTooltipLabel()
                If helpLabel IsNot Nothing Then
                    HelpText = helpLabel.TranslatedLabel(Language, Nothing)
                End If
            End If

            If Not String.IsNullOrEmpty(HelpText) Then
                Dim formatter As New Arco.Doma.Library.TextFormatters.NullTextFormatter()
                If thisCaseInstance IsNot Nothing Then
                    HelpText = TagReplacer.ReplaceTags(HelpText, New LanguageCode(Language), thisCaseInstance, Nothing, formatter, True, True, 0, 0, True)
                Else
                    HelpText = TagReplacer.ReplaceTags(HelpText, New LanguageCode(Language), thisObject, Nothing, formatter, True, True, 0, 0, True)
                End If
            End If
        End If

        Dim helpButton As RadToolBarButton = GetHelpButton()
        If Not String.IsNullOrEmpty(HelpText) Then

            helpWindow.Visible = True

            If helpButton IsNot Nothing Then
                cellScroll.Width = Unit.Pixel(cellScroll.Width.Value + 18)
                helpButton.Visible = True
                helpButton.NavigateUrl = "javascript:ShowHelp();"
                '  helpButton.ToolTip = helpText
                helpButton.EnableImageSprite = True
                helpButton.SpriteCssClass = "icon icon-help icon-md"

            End If

            Dim sbScript As New StringBuilder
            sbScript.Append("function ShowHelp(){$('#")
            sbScript.Append(helpWindow.ClientID)
            sbScript.Append("').modal('show');}")
            sbScript.Append("function HideHelp(){$('#")
            sbScript.Append(helpWindow.ClientID)
            sbScript.Append("').modal('hide');}")
            ClientScript.RegisterClientScriptBlock(GetType(String), "ShowHelp", sbScript.ToString, True)

            'helpButtonEyeCatcher.Text = "Klik hier voor hulp bij dit scherm"   'todo: translate

        Else
            helpWindow.Visible = False
            'helpButtonEyeCatcher.Visible = False
            If helpButton IsNot Nothing Then
                helpButton.Visible = False
            End If
        End If

    End Sub

    Private Sub AddButtons()

        btnDelete.ID = "btnDelete"
        btnDelete.Visible = False
        AddHandler btnDelete.Click, AddressOf btnDelete_Click
        Controls.Add(btnDelete)
    End Sub

    Private Sub RedirectToNextObject(ByVal force As Boolean, ByVal onlywhenScrolling As Boolean)

        If Not onlywhenScrolling OrElse QueryStringParser.ScrollingMode Then
            If force OrElse UserProfile.OpenNextCaseOnClose Then

                Dim loScroller As QueryStringParser.ObjectScroller = QueryStringParser.GetObjectScroller(False)
                If loScroller.NextItemID > 0 Then
                    RedirectToUrl(loScroller.NextLink.NavigateUrl)
                Else
                    CloseWindow()
                End If
            Else
                CloseWindow()
            End If
        Else
            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
        End If

    End Sub

#Region " Panes "

    Dim _layout As ScreenLayout = Nothing
    Private Sub BuildPaneLayout()
        _layout = _screen.GetScreenLayout()

        Select Case _layout.MetaPosition
            Case Screen.ScreenMetaDataPosition.Left, Screen.ScreenMetaDataPosition.Right
                radSplitterContent.Orientation = Orientation.Vertical
                MetaDataPane.Width = Unit.Percentage(_layout.MetaPercent)
                PreviewPane.Width = Unit.Percentage(100 - _layout.MetaPercent)

                MetaDataPane.Height = Unit.Percentage(100)
                PreviewPane.Height = Unit.Percentage(100)

            Case Screen.ScreenMetaDataPosition.Bottom, Screen.ScreenMetaDataPosition.Top

                radSplitterContent.Orientation = Orientation.Horizontal

                MetaDataPane.Height = Unit.Percentage(_layout.MetaPercent)
                PreviewPane.Height = Unit.Percentage(100 - _layout.MetaPercent)


                MetaDataPane.Width = Unit.Percentage(100)
                PreviewPane.Width = Unit.Percentage(100)

        End Select

    End Sub
    Public ReadOnly Property MetaDataPane As RadPane
        Get
            Select Case _layout.MetaPosition
                Case Screen.ScreenMetaDataPosition.Left, Screen.ScreenMetaDataPosition.Top
                    Return radPaneLeftContent
                Case Screen.ScreenMetaDataPosition.Right, Screen.ScreenMetaDataPosition.Bottom
                    Return radPaneRightContent
            End Select

            Return Nothing
        End Get
    End Property
    Public ReadOnly Property PreviewPane As RadPane
        Get
            Select Case _layout.MetaPosition
                Case Screen.ScreenMetaDataPosition.Left, Screen.ScreenMetaDataPosition.Top
                    Return radPaneRightContent
                Case Screen.ScreenMetaDataPosition.Right, Screen.ScreenMetaDataPosition.Bottom
                    Return radPaneLeftContent
            End Select
            Return Nothing
        End Get
    End Property


#End Region

    Private Sub AddBodyAttributes()
        If Not UserProfile.OpenDetailWindowMaximized Then
            body.Attributes.Add("onunload", "PC().PersistDetailWindowSize();")
            body.Attributes.Add("onresize", "PC().PersistDetailWindowSize();")
        End If
        If Not Modal Then
            body.Attributes.Add("onbeforeunload", "return CheckBeforeUnload();")
        End If
        body.Attributes.Add("scroll", "no")
        body.Attributes.Add("style", "height:100%;margin:0px;overflow:hidden;")

        Dim sb As New StringBuilder(128)
        If InLine Then
            sb.Append(" var ShowUnLoadMessage = false;")
        Else
            sb.Append(" var ShowUnLoadMessage = true;")
        End If

        sb.Append("function SuppressUnLoadMessage(){ var unloadshowing = ShowUnLoadMessage; ShowUnLoadMessage = false;")
        sb.Append("if (unloadshowing){setTimeout(function(){ShowUnLoadMessage = true},300);}}")
        sb.Append("function CheckBeforeUnload(){ if (ShowUnLoadMessage && _Mode==2 && document.hasFocus()) {")

        If thisObject.Status = DM_OBJECT.Object_Status.InCreation AndAlso thisObject.Case_ID = 0 Then
            sb.Append("return " & EncodingUtils.EncodeJsString(GetDecodedLabel("increationwarning")) & ";")
        ElseIf thisObject.CategoryObject IsNot Nothing AndAlso Not thisObject.CategoryObject.AllowConcurrentEditing Then
            sb.Append("return " & EncodingUtils.EncodeJsString(GetDecodedLabel("lockedwarning")) & ";")
        End If

        sb.Append(" }}")

        ClientScript.RegisterClientScriptBlock(GetType(String), "onOnloadMessage", sb.ToString, True)
    End Sub
    ''' <summary>
    ''' automatically opens the correct url in the preview pane (for objects)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AutoOpenPreviewContent()

        If mbDisableAutoOpen Then
            mbAutoOpentab = False
        Else
            mbAutoOpentab = UserProfile.AutoOpenFileInDetail
        End If

        If TypeOf thisObject Is MailShortcut Then
            mbAutoOpentab = False
        End If

        If mbAutoOpentab Then
            mbAutoOpentab = False
            Select Case _screen.DefaultPreview
                Case 0 'nothing
                    ' mbAutoOpentab = False
                Case Else
                    If msDefaultTab = "File" OrElse msDefaultTab = "Browse" Then
                        msDefaultTab = ""
                    End If
                    Dim lbOpenFile As Boolean = False
                    Dim liPackID As Int32 = 0
                    If String.IsNullOrEmpty(msDefaultTab) Then
                        If _screen.DefaultPreview > 1 AndAlso thisObject.Category <> 0 Then
                            For Each p As PackageList.PackageInfo In thisObject.Packages
                                If p.ID = _screen.DefaultPreview Then
                                    If p.IsFilePackage Then
                                        liPackID = p.ID
                                        lbOpenFile = thisObject.CanViewPackage(p)
                                    End If
                                    Exit For
                                End If
                            Next
                        Else
                            lbOpenFile = True
                            'todo : open file package or any file?
                        End If
                    End If

                    If lbOpenFile Then

                        If thisObject.CanViewFiles Then
                            Dim lbHideFile As Boolean = QueryStringParser.GetBoolean("hidefile", False)

                            If thisObject.FileCount > 0 AndAlso Not lbHideFile Then
                                For Each loFileInfo As FileList.FileInfo In thisObject.Files.AsQueryable.OrderBy("FILE_ATT ASC,FILE_TITLE asc")
                                    If loFileInfo.PackageID = liPackID Then
                                        Dim lsPrevRend As String = thisObject.GetFileType(loFileInfo.FILE_EXT).Preview_Rendition
                                        If lsPrevRend <> "nopreview" AndAlso lsPrevRend <> "download" Then

                                            Dim lsUrl As String = GetRedirectUrl("DM_PREVIEW.aspx?FILE_ID=" & loFileInfo.ID.ToString & "&rend=preview" & QueryStringParser.CreateQueryStringPart("fromarchive", loFileInfo.FILE_STATUS = File.File_Status.Archived))
                                            If Settings.GetValue("Interface", "ShowTabsInDetailPreview", True) Then
                                                lsUrl &= "&tabnode=Sub"
                                            Else
                                                lsUrl &= "&hidetabs=Y"
                                            End If

                                            If Not UserProfile.ShowFilesInSeparateWindow Then
                                                PreviewPane.ContentUrl = lsUrl
                                                InitTitle(loFileInfo.Name)
                                                mbAutoOpentab = True
                                            Else
                                                'autoopen preview in separate window
                                                Dim sbAutoOpen As New StringBuilder(128)

                                                sbAutoOpen.Append("function AutoOpenFile(){PC().OpenWindow(")
                                                sbAutoOpen.Append(EncodingUtils.EncodeJsString(lsUrl))
                                                sbAutoOpen.Append(", 'MySubDetailWindow', 'height=600,width=800,scrollbars=yes,resizable=yes,toolbar=yes,status=yes');}Sys.Application.add_load(AutoOpenFile);")
                                                ClientScript.RegisterStartupScript(Me.GetType, "autoopenpreview", sbAutoOpen.ToString, True)
                                            End If
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If
                        End If
                    End If
            End Select

        End If


        PreviewPane.Collapsed = Not mbAutoOpentab
        If mbAutoOpentab AndAlso _screen.ScreenItems.Count = 0 Then
            MetaDataPane.Collapsed = True
        End If

    End Sub

#Region " Scroller "
    Private Function MakeJSUrl(ByVal vsRelativeUrl As String) As String
        If Request.ApplicationPath = "/" Then
            Return ResolveClientUrl(vsRelativeUrl)
        Else
            Return Request.ApplicationPath & "/" & ResolveClientUrl(vsRelativeUrl)
        End If
    End Function

    Private Function GetHelpButton() As RadToolBarButton
        Return CType(toolbarScroll.Items.FindItemByValue("help"), RadToolBarButton)
    End Function
    Private Function GetCloseButton() As RadToolBarButton
        Return CType(toolbarScroll.Items.FindItemByValue("close"), RadToolBarButton)
    End Function
    Private Sub CreateScroller()
        Dim scroller As QueryStringParser.ObjectScroller = QueryStringParser.GetObjectScroller(False)
        If scroller.hasItems Then
            Title = Title & " (" & scroller.CurrentItem & "/" & scroller.ItemCount & ")"
        End If


        Dim btnPrevious As RadToolBarButton = CType(toolbarScroll.Items.FindItemByValue("prev"), RadToolBarButton)
        Dim btnNext As RadToolBarButton = CType(toolbarScroll.Items.FindItemByValue("next"), RadToolBarButton)
        Dim closeButton As RadToolBarButton = GetCloseButton()


        If Not Modal Then
            cellScroll.Width = Unit.Pixel(90)

            closeButton.Visible = True
            closeButton.Attributes.Add("onclick", "SuppressUnLoadMessage();")
            closeButton.NavigateUrl = "javascript:CloseButton();"
            closeButton.ToolTip = GetDecodedLabel("close")
            closeButton.EnableImageSprite = True
            closeButton.SpriteCssClass = "icon icon-close icon-md"

            'helpButtonEyeCatcher.Style.Add("right", "41px")

        Else
            cellScroll.Width = Unit.Pixel(72)

            closeButton.Visible = False
        End If

        btnPrevious.EnableImageSprite = True
        btnNext.EnableImageSprite = True
        btnPrevious.SpriteCssClass = "icon icon-arrow-left icon-md"
        btnNext.SpriteCssClass = "icon icon-arrow-right icon-md"

        Dim lsScrollNextDoc As String = ""
        Dim lsScrollNext As String = ""
        Dim lsScrollPrevious As String = ""
        Dim lsScrollPreviousDoc As String = ""

        If scroller.hasItems Then
            btnPrevious.Enabled = scroller.PreviousLink.Enabled
            btnPrevious.NavigateUrl = "javascript:ScrollPrevious();"
            If btnPrevious.Enabled Then
                btnPrevious.ToolTip = Server.HtmlDecode(GetLabel("previous")) & " (" & (scroller.CurrentItem - 1) & "/" & scroller.ItemCount & ")"
                lsScrollPrevious = MakeJSUrl(scroller.PreviousLink.NavigateUrl)
                lsScrollPreviousDoc = MakeJSUrl(scroller.PreviousLink.NavigateUrlWithoutMode)
            End If
            btnPrevious.Attributes.Add("onclick", "SuppressUnLoadMessage();")

            btnNext.Enabled = scroller.NextLink.Enabled
            btnNext.NavigateUrl = "javascript:ScrollNext();"
            If btnNext.Enabled Then
                'remove the ~/ at the start
                lsScrollNextDoc = MakeJSUrl(scroller.NextLink.NavigateUrlWithoutMode)
                lsScrollNext = MakeJSUrl(scroller.NextLink.NavigateUrl)

                btnNext.ToolTip = Server.HtmlDecode(GetLabel("next")) & " (" & (scroller.CurrentItem + 1) & "/" & scroller.ItemCount & ")"
            End If
            btnNext.Attributes.Add("onclick", "SuppressUnLoadMessage();")

            isFirst = scroller.isFirst
            isLast = scroller.isLast
            NextID = scroller.NextItemID
        Else
            btnPrevious.Enabled = False
            btnNext.Enabled = False
        End If
        Dim sbScript As New StringBuilder(128)

        If Not Modal Then
            sbScript.Append("function OpenUrl(url) {LoadContent(url);}")

        Else
            sbScript.Append("function OpenUrl(url) {if (url.indexOf('modal=') < 0) { if (url.indexOf('?') > 0) { url = url + '&modal=Y';} else { url = url + '?modal=Y';}}  LoadContent(url);}")

        End If

        sbScript.Append("function GotoCurrent(){LoadContent('")
        sbScript.Append(MakeJSUrl(scroller.CurrentLink.NavigateUrl))
        sbScript.Append("&refr=Y&selectedtab=' + GetContentWindow().GetSelectedTab());}")

        sbScript.Append("function EditCurrent(){LoadContent('")
        sbScript.Append(MakeJSUrl(scroller.CurrentLink.NavigateUrlWithoutMode))
        sbScript.Append("&mode=2&selectedtab=' + GetContentWindow().GetSelectedTab());}")

        sbScript.Append("function ToggleAdmin(screenid){")
        If _actualMode = Screen.DetailScreenDisplayMode.Admin Then
            If Not _isCase Then
                sbScript.Append("LoadContent('")
                sbScript.Append(MakeJSUrl(scroller.CurrentLink.NavigateUrlWithoutMode))
                sbScript.Append("&mode=1');")
            Else
                sbScript.Append("LoadContent('")
                sbScript.Append(MakeJSUrl(scroller.CurrentLink.NavigateUrlWithoutMode))
                sbScript.Append("&mode=2');")
            End If
        Else
            sbScript.Append("LoadContent('")
            sbScript.Append(MakeJSUrl(scroller.CurrentLink.NavigateUrlWithoutMode))
            sbScript.Append("&mode=4&screen_id=' + screenid);")
        End If
        sbScript.Append("}")

        If closeButton.Visible Then
            sbScript.Append("function CloseButton(){AutoUnlock();CloseWindow();}")
        End If


        sbScript.Append("function Scroll(url){AutoUnlock();LoadContent(url);}")


        sbScript.Append("function ScrollNext(){")
        If Not String.IsNullOrEmpty(lsScrollNext) Then
            sbScript.Append("Scroll('")
            sbScript.Append(lsScrollNext)
            sbScript.Append("');")
        Else
            sbScript.Append("CloseWindow();")
        End If
        sbScript.Append("}")

        sbScript.Append("function ScrollPrevious(){")
        If Not String.IsNullOrEmpty(lsScrollPrevious) Then
            sbScript.Append("Scroll('")
            sbScript.Append(lsScrollPrevious)
            sbScript.Append("');")
        Else
            sbScript.Append("CloseWindow();")
        End If
        sbScript.Append("}")


        sbScript.Append("function NextDoc(mode){")
        If Not String.IsNullOrEmpty(lsScrollNextDoc) Then
            sbScript.Append("LoadContent('")
            sbScript.Append(lsScrollNextDoc)
            sbScript.Append("&mode=' + mode);")
        Else
            sbScript.Append("CloseWindow();")
        End If
        sbScript.Append("}")

        sbScript.Append("function PreviousDoc(mode)")
        sbScript.Append("{")
        If Not String.IsNullOrEmpty(lsScrollPreviousDoc) Then
            sbScript.Append("LoadContent('")
            sbScript.Append(lsScrollPreviousDoc)
            sbScript.Append("&mode=' + mode);")
        Else
            sbScript.Append("CloseWindow();")
        End If
        sbScript.Append("}")

        sbScript.Append("function GotoNextCaseAfterClose(removeobjid,removetechid){")
        If Not String.IsNullOrEmpty(lsScrollNext) Then
            sbScript.Append("LoadContent('")
            sbScript.Append(lsScrollNext)
            sbScript.Append("&")
            sbScript.Append(QueryStringParser.ObjectScroller.RemoveObjectFromListTag)
            sbScript.Append("=' + removeobjid + '&")
            sbScript.Append(QueryStringParser.ObjectScroller.RemoveCaseFromListTag)
            sbScript.Append("=' + removetechid);")
        Else
            sbScript.Append("CloseWindow();")
        End If
        sbScript.Append("}")

        sbScript.Append("function GotoPreviousCaseAfterClose(removeobjid,removetechid){")
        If Not String.IsNullOrEmpty(lsScrollPrevious) Then
            sbScript.Append("LoadContent('")
            sbScript.Append(lsScrollPrevious)
            sbScript.Append("&")
            sbScript.Append(QueryStringParser.ObjectScroller.RemoveObjectFromListTag)
            sbScript.Append("=' + removeobjid + '&")
            sbScript.Append(QueryStringParser.ObjectScroller.RemoveCaseFromListTag)
            sbScript.Append("=' + removetechid);")
        Else
            sbScript.Append("CloseWindow();")
        End If
        sbScript.Append("}")

        ClientScript.RegisterClientScriptBlock(GetType(String), "DetailScrollNext", sbScript.ToString, True)


    End Sub

    Private Sub CreateHelpButton()
        Dim helpButton As RadToolBarButton = GetHelpButton()
        If Not String.IsNullOrEmpty(HelpText) Then
            cellScroll.Width = Unit.Pixel(cellScroll.Width.Value + 18)
            helpWindow.Visible = True
            helpButton.Visible = True
            helpButton.NavigateUrl = "javascript:ShowHelp();"
            '  helpButton.ToolTip = helpText
            helpButton.EnableImageSprite = True
            helpButton.SpriteCssClass = "icon icon-help icon-md"

            Dim sbScript As New StringBuilder
            sbScript.Append("function ShowHelp(){$('#")
            sbScript.Append(helpWindow.ClientID)
            sbScript.Append("').modal('show');}")
            ClientScript.RegisterClientScriptBlock(GetType(String), "ShowHelp", sbScript.ToString, True)

            'helpButtonEyeCatcher.Text = "Klik hier voor hulp bij dit scherm"   'todo: translate

        Else
            helpWindow.Visible = False
            helpButton.Visible = False
            'helpButtonEyeCatcher.Visible = False
        End If
    End Sub


    Private Sub AddDetailWindowScripts()
        Dim sbScript As New StringBuilder()
        If Modal Then
            sbScript.Append("function OnModalWindowClose(sender, args){")
            sbScript.Append("AutoUnlock();")
            sbScript.Append("}")

            sbScript.Append("function CloseWindow() {    ShowUnLoadMessage = false;    PC().GotoLink(""CloseModalWindow.aspx?goToTreeId=" & _FolderID.ToString() & """);}")
        Else
            sbScript.Append("function CloseWindow() {    ShowUnLoadMessage = false;    PC().GotoLink(""CloseDetailWindow.aspx?goToTreeId=" & _FolderID.ToString() & """);}")
        End If

        sbScript.Append("function AutoUnlock(){")
        If _actualMode = Screen.DetailScreenDisplayMode.Edit Then
            If _isCase OrElse (thisObject.CategoryObject IsNot Nothing AndAlso Not thisObject.CategoryObject.AllowConcurrentEditing) Then
                If UserProfile.OnCloseEditWindow = UserProfile.CloseBeheaviour.Confirm Then
                    sbScript.Append("if (confirm(" & EncodingUtils.EncodeJsString(GetDecodedLabel("lockedconfirm")) & ")){")
                End If
                If UserProfile.OnCloseEditWindow = UserProfile.CloseBeheaviour.Confirm OrElse UserProfile.OnCloseEditWindow = UserProfile.CloseBeheaviour.Unlock Then
                    If _isCase Then
                        sbScript.Append("DocroomListFunctions.UnlockCase(" & thisCaseInstance.Tech_ID & ");")
                    Else
                        sbScript.Append("DocroomListFunctions.UnlockDocument(" & thisObject.ID & ");")
                    End If
                End If
                If UserProfile.OnCloseEditWindow = UserProfile.CloseBeheaviour.Confirm Then
                    sbScript.Append("}")
                End If
            End If
        End If

        sbScript.Append("}")

        sbScript.Append("function DeleteObject(){")
        sbScript.Append("SuppressUnLoadMessage();")
        sbScript.Append(GetPostBackString(btnDelete, False, ""))
        sbScript.Append("}")

        ClientScript.RegisterClientScriptBlock(GetType(String), "DetailWindowScripts", sbScript.ToString, True)
    End Sub

    Private Function GetPostBackString(ByVal btn As LinkButton) As String
        Return GetPostBackString(btn, True, "")
    End Function
    Private Function GetPostBackString(ByVal btn As LinkButton, ByVal vbValidate As Boolean, ByVal vsArgument As String) As String
        Dim objPostBackoptions As PostBackOptions
        If vbValidate Then
            objPostBackoptions = New PostBackOptions(btn, vsArgument, "", True, True, True, True, True, btn.ValidationGroup)
        Else
            objPostBackoptions = New PostBackOptions(btn, vsArgument)
        End If
        Return ClientScript.GetPostBackEventReference(objPostBackoptions) & ";"

    End Function
#End Region

    Private Sub InitTitle(ByVal vsTitle As String)
        ClientScript.RegisterStartupScript(GetType(String), "SetFirstFileTitle", "InitHeader(" & EncodingUtils.EncodeJsString(vsTitle) & ");", True)
    End Sub

    Private Sub ParseQueryString()

        _ObjectIDList = QueryStringParser.GetIdList(QueryStringParser.Parameters.Object_List)
        _FolderID = QueryStringParser.GetInt("folderid", -1)
        _requestedMode = QueryStringParser.DetailDisplayMode
        _actualMode = _requestedMode
        ForceNextCaseOnClose = QueryStringParser.GetBoolean("forcenext")
        _ParentReload = QueryStringParser.GetBoolean("parentreload")
        _RefreshTree = QueryStringParser.GetBoolean("refreshtree")
        mbDisableAutoOpen = QueryStringParser.GetBoolean("disableautoopen")
        _fromRefresh = QueryStringParser.GetBoolean("refr")
        QueryStringParser.Remove("refr")
        Modal = QueryStringParser.Modal
    End Sub

    Private Sub AddParentReload()

        Dim sb As New StringBuilder(64)
        sb.Append("function ReloadParentPage(refreshtree){RefreshParentPage(" & _FolderID & ",refreshtree);}")
        If _ParentReload Then
            If Not _RefreshTree Then
                sb.Append("window.onload = function(){ ReloadParentPage(false);};")
            Else
                sb.Append("window.onload = function(){ ReloadParentPage(true);};")
            End If
        End If

        ClientScript.RegisterClientScriptBlock(Me.GetType, "ReloadParent", sb.ToString, True)

    End Sub



    Protected Sub SetUpToolbar()

        If _isCase Then
            btnChkNextCase.Visible = True

            Dim chkOpenNextCaseOnClose As Controls.CheckBox = CType(btnChkNextCase.FindControl("chkOpenNextCaseOnClose"), Controls.CheckBox)

            chkOpenNextCaseOnClose.Checked = UserProfile.OpenNextCaseOnClose
            chkOpenNextCaseOnClose.Enabled = Not UserProfile.IsReadOnly("OPENNEXTCASEONCLOSE")
            chkOpenNextCaseOnClose.Attributes("title") = GetDecodedLabel("opennextcaseonclose")
        Else
            btnChkNextCase.Visible = False

        End If


    End Sub
    Private _redirect As Boolean

    Public Sub New()
        AllowGuestAccess = True
    End Sub

    Private Sub RedirectToUrl(ByVal vsUrl As String)
        Response.Redirect(vsUrl, False)
        Context.ApplicationInstance.CompleteRequest()
        _redirect = True
    End Sub
#Region " Reloading "
    Private Sub FullReload(ByVal vlObjectID As Int32, ByVal vbReloadListScreen As Boolean)
        If vlObjectID <> thisObject.ID Then
            QueryStringParser.AddOrReplace("DM_OBJECT_ID", vlObjectID.ToString)
        End If
        If vbReloadListScreen Then
            QueryStringParser.AddOrReplace("parentreload", "true")
        Else
            QueryStringParser.Remove("parentreload")
        End If
        RedirectToUrl("dm_detail.aspx" & QueryStringParser.ToString())
    End Sub

    Private Sub FullReload(ByVal vbReloadListScreen As Boolean)
        FullReload(thisObject.ID, vbReloadListScreen)
    End Sub
    Private Sub RemoveIDFromScrollList(ByVal vlObjectID As Int32)
        If Not String.IsNullOrEmpty(_ObjectIDList) Then
            Dim lsNew As String = ""
            Dim laIds() As String = _ObjectIDList.Split(","c)
            For Each s As String In laIds
                If s <> vlObjectID.ToString Then
                    If lsNew = "" Then
                        lsNew = s
                    Else
                        lsNew = lsNew & "," & s
                    End If
                End If
            Next
            _ObjectIDList = lsNew
            QueryStringParser.AddOrReplace("objectlist", _ObjectIDList)
        End If
    End Sub
    Private Sub AddIDToScrollList(ByVal vlObjectID As Int32)
        If Not String.IsNullOrEmpty(_ObjectIDList) Then
            _ObjectIDList = vlObjectID & "," & _ObjectIDList
        Else
            _ObjectIDList = vlObjectID.ToString
        End If
        QueryStringParser.AddOrReplace("objectlist", _ObjectIDList)
    End Sub

#End Region

#Region " Toolbar click "
    Private Sub CloseCase(ByVal viItemObjectIDRemove As Int32, ByVal viItemTechIDRemove As Int32)
        If Not ForceNextCaseOnClose AndAlso Not UserProfile.OpenNextCaseOnClose Then
            CloseWindow()
        Else
            ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "Sys.Application.add_load(function() {GotoNextCaseAfterClose(" & viItemObjectIDRemove & "," & viItemTechIDRemove & ");});", True)
        End If
    End Sub
    Private Sub HandlerToolbarClick(ByVal vsCommandName As String, ByVal vsArg As String)
        Select Case vsCommandName.ToUpper
            Case "NEWCOPY"
                If thisObject.CanCopyTo(thisObject.Parent) Then
                    If GetLabel("duplicateof") <> "" Then
                        thisObject.Name = GetLabel("duplicateof").Trim() & " " & thisObject.Name
                    End If
                    Dim loCopy As DM_OBJECT = thisObject.CreateDuplicate
                    AddIDToScrollList(loCopy.ID)
                    QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Mode, "2") 'force edit
                    FullReload(loCopy.ID, False)
                Else
                    GotoErrorPage(thisObject.GetLastError().Code)
                End If

            Case "CHECKOUT"
                If TypeOf (thisObject) Is Document Then
                    Dim doc As Document = CType(thisObject, Document)
                    If doc.CanCheckOut Then
                        Dim llOldID As Int32 = doc.ID
                        doc = doc.Checkout(hdnComment.Value)
                        If Not doc.HasError Then
                            RemoveIDFromScrollList(llOldID)
                            AddIDToScrollList(doc.ID)
                            FullReload(doc.ID, True)
                        Else
                            ClientScript.RegisterStartupScript(Me.GetType(), "HandlerCancel", "ShowMessage(" & EncodingUtils.EncodeJsString(doc.GetLastError.Description) & ",""error"");", True)
                        End If
                    Else
                        GotoErrorPage(doc.GetLastError().Code)
                    End If
                End If
            Case "CANCELCHECKOUT"
                If TypeOf (thisObject) Is Document Then
                    Dim doc As Document = CType(thisObject, Document)
                    RemoveIDFromScrollList(doc.ID)
                    doc = doc.CancelCheckout()
                    AddIDToScrollList(doc.ID)
                    FullReload(doc.ID, True)
                End If
            Case "ADDFILECLIPBOARD"
                Dim FileID As Nullable(Of Integer) = Nothing
                Dim Action As String = ""

                If Not Request.Cookies("FileClipBoard") Is Nothing Then FileID = CType(Request.Cookies("FileClipBoard").Value, Integer)
                If Not Request.Cookies("FileClipBoardAction") Is Nothing Then Action = Request.Cookies("FileClipBoardAction").Value

                Select Case Action
                    Case "Copy"
                        If FileID.HasValue Then
                            File.GetFile(FileID.Value).Copy(thisObject.ID, True)
                        End If
                    Case "Cut"
                        If FileID.HasValue Then
                            File.GetFile(FileID.Value).Move(thisObject.ID)
                        End If
                End Select

                Response.Cookies("FileClipBoard").Expires = Now.AddDays(-1)
                Response.Cookies("FileClipBoardAction").Expires = Now.AddDays(-1)

                FullReload(True)

            Case "PROMOTETODOCUMENT"
                If TypeOf (thisObject) Is MailShortcut Then
                    Dim obj As MailShortcut = CType(thisObject, MailShortcut)
                    Dim doc As Document = obj.PromoteToDocument()
                    If doc.HasError Then
                        ClientScript.RegisterStartupScript(Me.GetType(), "HandlerCancel", "ShowMessage(" & EncodingUtils.EncodeJsString(doc.GetLastError.Description) & ",""error"");", True)
                    Else
                        FullReload(doc.ID, True)
                    End If
                End If
            Case "FINISHCASE"
                If _isCase AndAlso thisCaseInstance.CanAdminCaseStatus OrElse (thisCaseInstance.CurrentProcedure.CaseCreatorCanFinishCase AndAlso thisCaseInstance.CaseData.Created_By = Arco.Security.BusinessIdentity.CurrentIdentity.Name) Then
                    thisCaseInstance.DispatchStop(True, False)
                    CloseCase(thisCaseInstance.DM_Object_ID, thisCaseInstance.Tech_ID)
                End If
            Case "RELEASE"

                If _isCase AndAlso thisCaseInstance.CurrentUserHasWork Then
                    Dim vlToStep As Int32 = 0

                    Dim llStepID As Int32 = thisCaseInstance.Step_ID
                    Dim llTechID As Int32 = thisCaseInstance.Tech_ID
                    Dim llObjID As Int32 = thisCaseInstance.DM_Object_ID
                    Dim lbOpenObjectAfterClose As Boolean = False

                    thisCaseInstance = thisCaseInstance.Dispatch(vlToStep)

                    Dim lbOpenSameCase As Boolean = False

                    If Not thisCaseInstance.IsClosed Then 'not ended
                        If Settings.GetValue("Interface", "AutoFlow", True) OrElse llStepID = thisCaseInstance.Step_ID Then
                            lbOpenSameCase = thisCaseInstance.CurrentUserHasWork
                        End If
                    Else
                        lbOpenObjectAfterClose = False 'false for now always
                        If lbOpenObjectAfterClose Then
                            lbOpenObjectAfterClose = Settings.GetValue("Interface", "AutoFlow", True)
                            If lbOpenObjectAfterClose Then
                                Dim oTest As DM_OBJECT = ObjectRepository.GetObject(llObjID)
                                lbOpenObjectAfterClose = (oTest.CanViewMeta OrElse oTest.CanViewFiles) 'can we view the created object
                            End If

                        End If
                    End If

                    If lbOpenSameCase Then
                        ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "LoadContent('dm_detail.aspx" & QueryStringParser.ToString & "');", True)
                    ElseIf lbOpenObjectAfterClose Then
                        QueryStringParser.Remove("TECH_ID")
                        QueryStringParser.Remove("CASE_ID")
                        QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Object_Id, llObjID.ToString)
                        QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Mode, "1")
                        RedirectToUrl("dm_detail.aspx" & QueryStringParser.ToString)
                    Else
                        CloseCase(llObjID, llTechID)
                    End If
                End If
            Case "DELETECASE"
                If _isCase AndAlso thisCaseInstance.Delete Then
                    CloseCase(thisCaseInstance.DM_Object_ID, thisCaseInstance.Tech_ID)
                End If
            Case "DELETE"
                If Not _isCase Then
                    If thisObject.Delete() Then
                        If thisObject.Case_ID > 0 Then
                            FullReload(True)
                            'case started -> todo : redirect
                        Else
                            CloseWindow()
                        End If
                    Else
                        ClientScript.RegisterStartupScript(Me.GetType(), "HandlerCancel", "ShowMessage(" & EncodingUtils.EncodeJsString(thisObject.GetLastError.Description) & ",""error"");", True)
                    End If
                End If
            Case "TOGGLEADMIN"
                If _actualMode = Screen.DetailScreenDisplayMode.Admin Then
                    If Not _isCase Then
                        QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Mode, "1")
                    Else
                        QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Mode, "2")
                    End If
                Else
                    QueryStringParser.AddOrReplace(QueryStringParser.Parameters.Mode, "4")
                End If
                RedirectToUrl("DM_Detail.aspx" & QueryStringParser.ToString())
            Case "RESTORETO"
                If TypeOf thisObject Is HistoryCase AndAlso Not String.IsNullOrEmpty(vsArg) Then
                    Dim hc As HistoryCase = CType(thisObject, HistoryCase)
                    Dim c As cCase = hc.Restore(Convert.ToInt32(vsArg))
                    If Not c Is Nothing Then
                        RedirectToUrl("dm_detail.aspx?RTCASE_TECH_ID=" & c.Tech_ID & "&mode=2")
                    End If
                End If
            Case "TRANSFERWORK"
                If _isCase AndAlso Not String.IsNullOrEmpty(vsArg) Then
                    thisCaseInstance.StepExecutor = vsArg
                    thisCaseInstance = thisCaseInstance.Save
                    CloseWindow()
                End If
        End Select
    End Sub
    Private Sub CloseWindow()
        If Not Modal Then
            RedirectToUrl("CloseDetailWindow.aspx?goToTreeId=" & _FolderID.ToString())
        Else
            RedirectToUrl("CloseModalWindow.aspx?goToTreeId=" & _FolderID.ToString())
        End If
    End Sub

    Protected Sub toolbar_OnClick(ByVal sender As Object, ByVal e As RadToolBarEventArgs)
        Dim b As RadToolBarButton = CType(e.Item, RadToolBarButton)
        HandlerToolbarClick(b.CommandName, b.CommandArgument)
    End Sub

    Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs)
        HandlerToolbarClick("DELETE", Nothing)
    End Sub

#End Region

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        If Not _redirect Then
            Dim sb As StringBuilder = New StringBuilder
            AddJSVar(sb, "_Mode", Convert.ToInt32(_actualMode).ToString, False)
            AddJSVar(sb, "docName", InitialTitle, False)
            Try

                AddJSVar(sb, "splitterMain", radSplitterMain.ClientID, True)
                AddJSVar(sb, "splitterContent", radSplitterContent.UniqueID, True)
                AddJSVar(sb, "paneActions", paneActions.UniqueID, True)
                AddJSVar(sb, "panePreviewContent", PreviewPane.UniqueID, True)
                AddJSVar(sb, "paneMetaData", MetaDataPane.UniqueID, True)
                AddJSVar(sb, "winPreviewContent", PreviewPane.ID, True)
                AddJSVar(sb, "winMetaData", MetaDataPane.ID, True)
                AddJSVar(sb, "winActions", paneActions.ID, True)

            Catch ex As Exception

            End Try


            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType, "vars" & ClientID, sb.ToString, True)

            If InLine Then
                sb = New StringBuilder(64)
                sb.Append(" function InitPage(){ if (PC().InitDone()){if (parent){if (parent.SetFolder){parent.SetFolder(")
                sb.Append(thisObject.ID)
                sb.Append(",0);}}} else {setTimeout('InitPage()',100) ;}}setTimeout('InitPage()',100) ;")
                ScriptManager.RegisterStartupScript(Page, Page.GetType, "init" & ClientID, sb.ToString, True)
            End If


        End If
    End Sub

    Private Sub AddJSVar(ByVal sb As StringBuilder, ByVal name As String, ByVal value As String, ByVal addQuotes As Boolean)
        sb.Append("var ")
        sb.Append(name)
        sb.Append(" = ")
        If addQuotes Then
            sb.Append(EncodingUtils.EncodeJsString(value))
        Else
            sb.Append(value)
        End If
        sb.Append(";")
    End Sub
End Class

Namespace ToolbarButtons


#Region " Factory "
    Public Class ButtonFactory

        Private ReadOnly _settings As ISettings
        Public Sub New(ByVal settings As ISettings)
            _settings = settings
        End Sub
        Public Function GetFixedToolbarButton(ByVal voAction As ScreenActionList.ScreenActionInfo, ByVal page As BasePage) As BaseToolbarButton
            Select Case voAction.FixedActionID
                Case ScreenAction.FixedAction.AddComment
                    Return New AddCommentButton(voAction, page)
                Case ScreenAction.FixedAction.AddMessage
                    Return New AddMessageButton(voAction, page)
                Case ScreenAction.FixedAction.AddMail
                    Return New AddMailButton(voAction, page)
                Case ScreenAction.FixedAction.AddFile
                    Return New AddFileButton(voAction, page)
                Case ScreenAction.FixedAction.AddFileAsMessage
                    Return New AddFileAsMessageButton(voAction, page)
                Case ScreenAction.FixedAction.ScanFile
                    Return New ScanFileButton(voAction, page)
                Case ScreenAction.FixedAction.ACL
                    Return New ACLButton(voAction, page)
                Case ScreenAction.FixedAction.Mails
                    Return New LinkedMailsButton(voAction, page)
                Case ScreenAction.FixedAction.BrowseContent
                    Return New BrowseContentButton(voAction, page)
                Case ScreenAction.FixedAction.Cancel
                    Return New CancelButton(voAction, page)
                Case ScreenAction.FixedAction.CheckInOut
                    If Not _settings.GetValue("Versioning", "EnableDocumentVersioning", True) Then Return Nothing

                    Return New CheckInOutButton(voAction, page)
                Case ScreenAction.FixedAction.Comments
                    Return New ViewCommentsButton(voAction, page)
                Case ScreenAction.FixedAction.Messages
                    Return New ViewMessagesButton(voAction, page)
                Case ScreenAction.FixedAction.CopyMoveToFolder
                    Return New CopyMoveToFolderButton(voAction, page)
                Case ScreenAction.FixedAction.Delete
                    Return New DeleteButton(voAction, page)
                Case ScreenAction.FixedAction.DeleteCase
                    Return New DeleteCaseButton(voAction, page)
                Case ScreenAction.FixedAction.Edit
                    Return New EditButton(voAction, page)
                Case ScreenAction.FixedAction.EditWork
                    Return New EditWorkButton(voAction, page)
                Case ScreenAction.FixedAction.EditDeadlines
                    Return New EditCaseDeadlinesButton(voAction, page)
                Case ScreenAction.FixedAction.Favorites
                    If Not _settings.GetValue("Favorites", "Enabled", True) Then
                        Return Nothing
                    End If
                    Return New FavoritesButton(voAction, page)
                Case ScreenAction.FixedAction.FinishCase
                    Return New FinishCaseButton(voAction, page)
                Case ScreenAction.FixedAction.History
                    Return New HistoryButton(voAction, page)
                Case ScreenAction.FixedAction.LinkedObjects
                    Return New LinkedObjectsButton(voAction, page)
                Case ScreenAction.FixedAction.MoveCase
                    Return New MoveCaseButton(voAction, page)
                Case ScreenAction.FixedAction.RestoreCase
                    Return New RestoreCaseButton(voAction, page)
                Case ScreenAction.FixedAction.NewCopy
                    Return New NewCopyButton(voAction, page)
                Case ScreenAction.FixedAction.NewItem
                    Return New NewButton(voAction, page)
                Case ScreenAction.FixedAction.ProcedureImage
                    Return New ProcedureImageButton(voAction, page)
                Case ScreenAction.FixedAction.PromoteMailToDocument
                    Return New PromoteMailToDocumentButton(voAction, page)
                Case ScreenAction.FixedAction.Reject
                    Return New RejectButton(voAction, page)
                Case ScreenAction.FixedAction.Release
                    Return New ReleaseButton(voAction, page)
                Case ScreenAction.FixedAction.ReleaseTo
                    Return New ReleaseToButton(voAction, page)
                Case ScreenAction.FixedAction.SaveAndClose
                    Return New SaveAndCloseButton(voAction, page)
                Case ScreenAction.FixedAction.SaveAndCloseWindow
                    Return New SaveAndCloseWindowButton(voAction, page)
                Case ScreenAction.FixedAction.SaveAndKeepOpen
                    Return New SaveAndKeepOpenButton(voAction, page)
                Case ScreenAction.FixedAction.SaveAndNext
                    Return New SaveAndNextButton(voAction, page)
                Case ScreenAction.FixedAction.Suspend
                    Return New SuspendButton(voAction, page)
                Case ScreenAction.FixedAction.TransferWork
                    Return New TransferWorkButton(voAction, page)
                Case ScreenAction.FixedAction.Unlock
                    Return New UnlockButton(voAction, page)
                Case ScreenAction.FixedAction.VersionHistory
                    If Not _settings.GetValue("Versioning", "EnableDocumentVersioning", True) Then Return Nothing
                    Return New VersionHistoryButton(voAction, page)
                Case ScreenAction.FixedAction.Home
                    Return New HomeButton(voAction, page)
                Case ScreenAction.FixedAction.Basket
                    If Not _settings.GetValue("Basket", "Enabled", True) Then
                        Return Nothing
                    End If
                    Return New BasketButton(voAction, page)
                Case ScreenAction.FixedAction.OpenCasesForObject
                    Return New OpenCasesForObjectButton(voAction, page)
                Case ScreenAction.FixedAction.MyWorkForObject
                    Return New WorkForObjectButton(voAction, page)
                Case ScreenAction.FixedAction.CloseDossier
                    Return New CloseDossierButton(voAction, page)
                Case ScreenAction.FixedAction.ClearForm
                    Return New ClearFormButton(voAction, page)
                Case ScreenAction.FixedAction.AddNote
                    Return New AddNoteButton(voAction, page)
                Case ScreenAction.FixedAction.Notes
                    Return New NotesButton(voAction, page)
                Case ScreenAction.FixedAction.Subscribe
                    Return New SubscribeButton(voAction, page)
                Case ScreenAction.FixedAction.ViewSubscriptions
                    Return New ViewSubscriptionsButton(voAction, page)
                Case Else
                    Return Nothing
            End Select
        End Function
        Public Function GetToolbarButton(ByVal voAction As ScreenActionList.ScreenActionInfo, ByVal page As BasePage) As BaseToolbarButton
            Select Case voAction.Type
                Case ScreenAction.ActionType.Fixed
                    Return GetFixedToolbarButton(voAction, page)
                Case ScreenAction.ActionType.Separator
                    Return New SeparatorButton(page)
                Case Else
                    Return Nothing
            End Select
        End Function
    End Class
#End Region
End Namespace
