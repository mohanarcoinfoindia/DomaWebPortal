Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Website
Imports Arco.Doma.Library.Search


Public Class DM_DOCUMENTLIST
    Inherits BasePage

    Public Sub New()
        AllowGuestAccess = True
    End Sub

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

#Region " Members"

    Private _queryID As Integer

    Protected _loadQueryFromScreen As Boolean = True
    Protected _loadQueryFromXML As Boolean
    Protected msMode As String
    Private meSelType As DMSelection.SelectionType = DMSelection.SelectionType.Undefined
    Protected JSMessageError As String = ""
    Protected JSMessageSuccess As String = ""
    Protected JSCommand As String = ""
    Protected HeaderText As String = ""
    Private msSelDate As String
    Private miCustomSelID As Int32
    Private _categoryId As Int32
    Private msPrintOptions As String
    Protected RefreshTree As Boolean

    Private _queryXMLProp As String = ""


#End Region

#Region " Page events"
    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        Me.Form.DefaultButton = lnkEnter.UniqueID
        Me.Form.DefaultFocus = "None"
        Me.Form.SubmitDisabledControls = True

        SharedCalendar.RangeMinDate = DateTime.MinValue
        SharedCalendar.RangeMaxDate = DateTime.MaxValue

        PC.ShowPreview = UserProfile.ShowPreview
        SITE.Value = SiteManagement.SitesManager.CurrentSite()

        If Not SetQueryStringData() Then
            Exit Sub
        End If

        LoadResults()
        LoadHeaderInfo()

        SetLabels()

        Dim grpWidth As String = grd.GetSideBarWidth
        If Not String.IsNullOrEmpty(grpWidth) Then
            Dim sb As StringBuilder = New StringBuilder
            sb.Append("$(document).ready(function () {")
            sb.Append("if($('#listControlBodyHead').length && $('#docPageHead').length) {")
            sb.Append("$('#docPageHead').detach().prependTo('#listControlBodyHead');")
            sb.Append("}});")
            ScriptManager.RegisterStartupScript(Page, Page.GetType, "moveDocPageHead" & ClientID, sb.ToString, True)
        End If


        If pnlFilter.Visible AndAlso Not IsPostBack Then
            domasearch.Focus()
        End If
    End Sub
#End Region

#Region " Private methods"

    Private Sub SetLabels()

        lblSaveQueryHeader.Text = GetLabel("savequery")
        lnkSaveQuery.Text = GetLabel("save")
        lnkCancelQry.Text = GetLabel("cancel")

        rdQryScope.Items(0).Text = GetLabel("public")
        rdQryScope.Items(1).Text = GetLabel("private")
        rdQryScope.Items(2).Text = GetLabel("secured")

        optFolderToUse.Items(0).Text = GetLabel("ontheroot")
        optFolderToUse.Items(1).Text = GetLabel("ontheexecutingfolder")
        optFolderToUse.Items(2).Text = GetLabel("onthisfolder")


    End Sub

    Private mbLastWasSeparator As Boolean = False
    Private Sub AddFolderTextToolbarButton(ByVal voFolder As DM_OBJECT, ByVal voButton As FolderTextToolbarButtons.BaseToolbarButton, ByVal vsScript As StringBuilder)
        If TypeOf voButton Is FolderTextToolbarButtons.SeparatorButton AndAlso mbLastWasSeparator Then
            Exit Sub
        End If

        voButton.DataBind(voFolder)

        voButton.AddToToolbar(tlbFolderText)
        If voButton.Visible Then
            voButton.AppendClientScript(vsScript, Me)

            If TypeOf voButton Is FolderTextToolbarButtons.SeparatorButton Then
                mbLastWasSeparator = True
            Else
                mbLastWasSeparator = False
            End If
        End If
    End Sub

    Private Sub AddFolderTextToolbarButton(ByVal voDrpDwn As Telerik.Web.UI.RadToolBarDropDown, ByVal voFolder As DM_OBJECT, ByVal voButton As FolderTextToolbarButtons.BaseToolbarButton, ByVal vsScript As StringBuilder)
        voButton.DataBind(voFolder)

        voButton.AddToToolbar(voDrpDwn)
        If voButton.Visible Then
            voButton.AppendClientScript(vsScript, Me)
        End If
    End Sub

    Private Sub LoadHeaderInfo()

        Dim lsHeaderText As String
        If meSelType = DMSelection.SelectionType.Undefined Then 'load folder

            Select Case msMode
                Case "links"
                    lsHeaderText = GetDecodedLabel("linkeddocs")
                Case "prevversions"
                    lsHeaderText = GetDecodedLabel("previousversions")
                Case "recyclebin"
                    If Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.isAdmin Then
                        GotoErrorPage()
                    End If
                    lsHeaderText = GetDecodedLabel("recyclebin")

                Case "opencases"
                    lsHeaderText = GetDecodedLabel("opencases")
                Case "mywork"
                    lsHeaderText = GetDecodedLabel("mywork")
                Case "mydossiers"
                    lsHeaderText = GetDecodedLabel("mycases")
                Case "archiveddossiers"
                    lsHeaderText = GetDecodedLabel("archive")
                Case "print"
                    lsHeaderText = "Print"
                Case "whatsnew"
                    lsHeaderText = GetDecodedLabel("whatsnew")
                Case "whatsmodified"
                    lsHeaderText = GetDecodedLabel("whatsmodified")
                Case "myincreation"
                    lsHeaderText = GetDecodedLabel("myincreation")
                Case "mycheckedoutdocs"
                    lsHeaderText = GetDecodedLabel("mycheckedoutdocs")
                Case Else
                    If String.IsNullOrEmpty(PC.PackageID) Then
                        Select Case ResultType
                            Case Screen.ScreenSearchMode.Cases
                                lsHeaderText = GetDecodedLabel("opencases")
                            Case Screen.ScreenSearchMode.MyCases
                                lsHeaderText = GetDecodedLabel("mycases")
                            Case Screen.ScreenSearchMode.ArchivedCases
                                lsHeaderText = GetDecodedLabel("archive")
                            Case Screen.ScreenSearchMode.OpenAndArchivedCases
                                lsHeaderText = GetDecodedLabel("openandarchivedcases")
                            Case Screen.ScreenSearchMode.Work
                                lsHeaderText = GetDecodedLabel("mywork")
                            Case Screen.ScreenSearchMode.Mails, Screen.ScreenSearchMode.MailFollowUp, Screen.ScreenSearchMode.MailDeletedBox, Screen.ScreenSearchMode.MailInbox, Screen.ScreenSearchMode.MailInboxWork, Screen.ScreenSearchMode.MailSentItems, Screen.ScreenSearchMode.MyMails
                                lsHeaderText = GetDecodedLabel("mails")
                            Case Screen.ScreenSearchMode.Files
                                lsHeaderText = GetDecodedLabel("files")
                            Case Else

                                If String.IsNullOrEmpty(domasearch.ObjectType) Then
                                    lsHeaderText = ""
                                Else
                                    lsHeaderText = ""
                                    For Each lsType In domasearch.ObjectType.Split(","c)
                                        If Not String.IsNullOrEmpty(lsHeaderText) Then
                                            lsHeaderText &= " " & GetLabel("and") & " "
                                        End If
                                        lsHeaderText &= GetDecodedLabel(lsType & "s")
                                    Next

                                End If

                        End Select
                    Else
                        Dim lcolPackLabels As Globalisation.LABELList = Globalisation.LABELList.GetCategoryItemsLabelList(CurrentFolder.Category, EnableIISCaching)
                        Dim lsPackName As String = lcolPackLabels.GetObjectLabel(CurrentPackage.ID, "Package", Arco.Security.BusinessIdentity.CurrentIdentity.Language, CurrentPackage.Name)

                        lsHeaderText = lsPackName
                    End If
            End Select

        Else
            Select Case meSelType
                Case DMSelection.SelectionType.Basket
                    lsHeaderText = GetDecodedLabel("basket")
                Case DMSelection.SelectionType.Favorites
                    lsHeaderText = GetDecodedLabel("favorites")
                Case DMSelection.SelectionType.CDRom
                    lsHeaderText = GetDecodedLabel("mycdrom")
                Case DMSelection.SelectionType.Recent
                    lsHeaderText = GetDecodedLabel("recentdocs")
                Case Else
                    lsHeaderText = GetDecodedLabel("documents")
            End Select
        End If

        If String.IsNullOrEmpty(HeaderText) Then
            HeaderText = lsHeaderText
        ElseIf Not String.IsNullOrEmpty(lsHeaderText) Then
            HeaderText &= " : " & lsHeaderText
        End If


        If (grd.ShowFolderText OrElse msMode = "foldertext") AndAlso
            CurrentFolderIsActualFolder() AndAlso CurrentFolder.CanViewMeta Then

            Dim loFolderDetailScreen As Screen = CurrentFolder.GetDetailScreen(Screen.DetailScreenDisplayMode.FolderText, Device.Web, 7)
            pnlFolderText.Visible = True


            Dim loActions As ScreenActionList = loFolderDetailScreen.ScreenActions
            If loFolderDetailScreen.ShowToolbar Then
                If Not String.IsNullOrEmpty(loFolderDetailScreen.Screen_Assembly) AndAlso Not String.IsNullOrEmpty(loFolderDetailScreen.Screen_Class) Then
                    Dim loScreenhandler As IScreenHandler = Arco.ApplicationServer.Library.Shared.PluginManager.CreateInstance(Of IScreenHandler)(loFolderDetailScreen.Screen_Assembly, loFolderDetailScreen.Screen_Class)
                    loScreenhandler.onBeforeToolbarRender(loActions, Screen.DetailScreenDisplayMode.FolderText, CurrentFolder)
                End If
            End If

            If loFolderDetailScreen.ShowToolbar AndAlso loActions.Count > 0 Then
                'show the toolbar                         
                tlbFolderText.Visible = True

                Dim sb As New StringBuilder


                Dim loDrpDwn As Telerik.Web.UI.RadToolBarDropDown = Nothing
                Dim lbInGrouper As Boolean = False

                For Each loAction As ScreenActionList.ScreenActionInfo In loActions
                    If Not lbInGrouper Then
                        Select Case loAction.Type
                            Case ScreenAction.ActionType.Tab
                                    'not supported (yet)
                            Case ScreenAction.ActionType.Separator
                                AddFolderTextToolbarButton(CurrentFolder, New FolderTextToolbarButtons.SeparatorButton, sb)
                            Case ScreenAction.ActionType.CustomAction
                                AddFolderTextToolbarButton(CurrentFolder, New FolderTextToolbarButtons.CustomActionButton(loAction), sb)
                            Case ScreenAction.ActionType.UserEvent
                                AddFolderTextToolbarButton(CurrentFolder, New FolderTextToolbarButtons.UserEventButton(loAction), sb)
                            Case ScreenAction.ActionType.EndGrouper
                                lbInGrouper = False
                            Case ScreenAction.ActionType.StartGrouper
                                lbInGrouper = True
                                loDrpDwn = New Telerik.Web.UI.RadToolBarDropDown
                                loDrpDwn.Text = loAction.Caption

                            Case Else
                                AddFolderTextToolbarButton(CurrentFolder, FolderTextToolbarButtons.ButtonFactory.GetToolbarButton(loAction), sb)
                        End Select
                    Else
                        Select Case loAction.Type
                            Case ScreenAction.ActionType.Tab
                                    'not supported (yet)
                            Case ScreenAction.ActionType.Separator
                                AddFolderTextToolbarButton(loDrpDwn, CurrentFolder, New FolderTextToolbarButtons.SeparatorButton, sb)
                            Case ScreenAction.ActionType.CustomAction
                                AddFolderTextToolbarButton(loDrpDwn, CurrentFolder, New FolderTextToolbarButtons.CustomActionButton(loAction), sb)
                            Case ScreenAction.ActionType.UserEvent
                                AddFolderTextToolbarButton(loDrpDwn, CurrentFolder, New FolderTextToolbarButtons.UserEventButton(loAction), sb)
                            Case ScreenAction.ActionType.EndGrouper
                                tlbFolderText.Items.Add(loDrpDwn)
                                ToolbarTranslator.Translate(loDrpDwn, Me)
                                lbInGrouper = False
                            Case ScreenAction.ActionType.StartGrouper
                                'no nesting allowed
                            Case Else
                                AddFolderTextToolbarButton(loDrpDwn, CurrentFolder, FolderTextToolbarButtons.ButtonFactory.GetToolbarButton(loAction), sb)
                        End Select
                    End If

                Next

                If lbInGrouper Then 'no end grouper supplied
                    If Not loDrpDwn Is Nothing Then
                        tlbFolderText.Items.Add(loDrpDwn)
                        ToolbarTranslator.Translate(loDrpDwn, Me)
                    End If
                End If

                ToolbarTranslator.Translate(tlbFolderText, Me)

                ClientScript.RegisterClientScriptBlock(Me.GetType, "FoldertextToolbarJS", sb.ToString, True)
            Else
                'don't show the toolbar
                tlbFolderText.Visible = False
            End If

            foldertext.DisplayMode = Screen.DetailScreenDisplayMode.FolderText
            foldertext.Labels = CurrentFolder.GetDetailScreenLabels(Me.EnableIISCaching)

            foldertext.Template = ""
            foldertext.DisplayItems = loFolderDetailScreen.ScreenItems.ReplaceAliasProperties(CurrentFolder)
            foldertext.ScreenID = loFolderDetailScreen.ID
            foldertext.ScreenHandlerAssembly = loFolderDetailScreen.Screen_Assembly
            foldertext.ScreenHandlerClass = loFolderDetailScreen.Screen_Class
            If loFolderDetailScreen.Type = Screen.ScreenSourceType.TemplateFile AndAlso loFolderDetailScreen.Source <> "" Then
                foldertext.Template = Arco.Settings.FrameWorkSettings.ReplaceGlobalVars(loFolderDetailScreen.Source, False)
            End If
            foldertext.DataBind(CurrentFolder, True)
        Else
            pnlFolderText.Visible = False
        End If
    End Sub

    Private Property ResultType() As Screen.ScreenSearchMode
        Get
            Dim o As Object = ViewState("ResultType")
            If o Is Nothing Then
                Return Screen.ScreenSearchMode.Objects
            Else
                Return CType(o, Screen.ScreenSearchMode)
            End If
        End Get
        Set(ByVal value As Screen.ScreenSearchMode)
            ViewState("ResultType") = value
        End Set
    End Property

    Private _currentQuery As DMQuery
    Protected ReadOnly Property CurrentSavedQuery As DMQuery
        Get
            If _currentQuery Is Nothing Then
                _currentQuery = GetCurrentSavedQuery()
            End If
            Return _currentQuery
        End Get
    End Property
    Private Function GetCurrentSavedQuery() As DMQuery
        If _queryID = 0 Then
            Return Nothing
        End If

        Dim currentSavedQuery As DMQuery = DMQuery.GetQuery(_queryID)
        If currentSavedQuery Is Nothing Then
            Throw New ArgumentException("Query not found")
        End If
        Dim canModifyPublicQueries As Boolean = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.isAdmin OrElse Folder.GetRoot().HasAccess(ACL.ACL_Access.Access_Level.ACL_Manage_Public_Saved_Queries)
        ' If Not forEdit Then
        If Not (currentSavedQuery.CanUse OrElse currentSavedQuery.CanModify(canModifyPublicQueries)) Then
            currentSavedQuery = Nothing
            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        End If
        'Else
        '    If Not currentSavedQuery.CanModify(canModifyPublicQueries) Then
        '        currentSavedQuery = Nothing
        '        GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        '    End If
        'End If

        Return currentSavedQuery
    End Function

    Private Sub SetCurrentSavedQuery(ByVal queryid As Int32)
        _queryID = queryid
        _currentQuery = Nothing
    End Sub

    Private Sub GotoActualParent()
        If PC.GetParentId(True) > 0 Then
            Dim loParent As DM_OBJECT = CurrentFolder
            While TypeOf loParent Is Shortcut 'load the object the shortcut points too
                If loParent.Object_Reference > 0 Then
                    loParent = CType(loParent, Shortcut).GetReferencedObject
                    SetParent(loParent)
                Else
                    'shortcut not pointing anywhere
                    Response.Write("Illegal shortcut")
                    Response.End()
                End If
            End While
        End If
    End Sub

    Private Sub SetSearchScreen(ByVal screenId As Int32)
        SetSearchScreen(screenId, False)
    End Sub

    Private Sub SetSearchScreen(ByVal screenId As Int32, ByVal force As Boolean)
        'only set it when it's not set yet
        If Not force AndAlso SearchScreen <> 0 Then Exit Sub

        If screenId = 0 OrElse screenId = -999999 Then Exit Sub


        SearchScreen = screenId
    End Sub

    Private Sub SetResultScreen(ByVal screenId As Int32)
        SetResultScreen(screenId, False)
    End Sub

    Private Sub SetResultScreen(ByVal screenId As Int32, ByVal force As Boolean)
        'only set it when it's not set yet
        If Not force AndAlso grd.ResultScreenID <> 0 Then Exit Sub

        If screenId = 0 OrElse screenId = -999999 Then Exit Sub


        grd.ResultScreenID = screenId
    End Sub

    Private Function CalculateScreensToUse() As Boolean
        'try to get from the url first
        If msMode = "query" Then
            SetSearchScreen(QueryStringParser.GetScreenId("DM_SCREEN_ID", Screen.ScreenMode.QUERY))
        Else
            SetResultScreen(QueryStringParser.GetScreenId("DM_SCREEN_ID", Screen.ScreenMode.RESULT))
        End If

        If SearchScreen = 0 Then SetSearchScreen(QueryStringParser.GetScreenId("DM_SEARCH_SCREEN_ID", Screen.ScreenMode.QUERY), True)
        If grd.ResultScreenID = 0 Then grd.ResultScreenID = QueryStringParser.GetScreenId("DM_RESULT_SCREEN_ID", Screen.ScreenMode.RESULT)


        Dim fromSavedQuery As Boolean = (_queryID <> 0)
        Dim forceResultScreenFromQueryScreen As Boolean = fromSavedQuery OrElse SearchScreen <> 0

        'force the query screen
        If fromSavedQuery Then
            SetSearchScreen(CurrentSavedQuery.SearchScreenID, True)
        End If

        If _categoryId > 0 AndAlso CurrentCategory.CatType = OBJECT_CATEGORY.eType.ListItem Then 'listitems -> use from list instead of folder

            SetSearchScreen(CurrentCategory.Default_Search_Screen)
            SetResultScreen(CurrentCategory.Default_Result_Screen)

        ElseIf meSelType = DMSelection.SelectionType.Undefined AndAlso (msMode = "search" OrElse msMode = "advsearch" OrElse msMode = "browse" OrElse msMode = "browsewithsubfolders" OrElse msMode = "query") Then
            'will be used if the searchscreen doesn't include a result screen
            If Not String.IsNullOrEmpty(PC.PackageID) Then

                SetSearchScreen(CurrentPackage.DefaultSearchScreen)

                SetResultScreen(CurrentPackage.DefaultResultScreen)
            End If

            Dim p As DM_OBJECT = GetCurrentParent(True)
            If p Is Nothing OrElse CurrentFolder Is Nothing Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                Return False
            End If

            If Not CurrentFolder.Type.CanHaveChildren Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOPERATIONONOBJECTTYPE)
                Return False
            End If

            If p.ID <> 0 Then
                SetSearchScreen(p.Default_Search_Screen)
                If p.Default_Search_Screen = 0 AndAlso p.Object_Type = "Query" Then
                    Dim qs As QueryShortcut = DirectCast(p, QueryShortcut)
                    ResultType = qs.Query.ResultType
                    SetSearchScreen(qs.Query.SearchScreenID, True)
                End If
                SetResultScreen(p.Default_Result_Screen)

            ElseIf Me.ResultType = Screen.ScreenSearchMode.Objects Then
                SetSearchScreen(Settings.GetValue("DOMA", "RootSearchScreen", 0))
                SetResultScreen(Settings.GetValue("DOMA", "RootResultScreen", 0))
            End If
        End If

        If SearchScreen > 0 Then
            Dim objQueryScreen As Screen = Screen.GetScreen(SearchScreen)
            If objQueryScreen IsNot Nothing Then
                If Not objQueryScreen.CanUse Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                    Return False
                End If
                SetResultScreen(objQueryScreen.ResultScreen, forceResultScreenFromQueryScreen)
            End If
        End If
        If grd.ResultScreenID > 0 Then
            Dim objResultScreen As Screen = Screen.GetScreen(grd.ResultScreenID)
            If objResultScreen Is Nothing Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                Return False
            ElseIf Not objResultScreen.CanUse Then
                GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                Return False
            End If
        End If

        Return True
    End Function

    Public Property SearchScreen() As Int32
        Get
            Dim s As String = txtSearchScreenID.Value
            If Not String.IsNullOrEmpty(s) Then
                Dim i As Int32
                If Int32.TryParse(s, i) Then
                    If i >= 0 Then
                        Return i
                    Else
                        Return 0
                    End If

                Else
                    Return 0
                End If
            Else
                Return 0
            End If

        End Get
        Set(ByVal value As Int32)
            txtSearchScreenID.Value = value.ToString()
        End Set
    End Property

    Private Function SetQueryStringData() As Boolean

        grd.RootID = PC.RootID
        SetCurrentSavedQuery(QueryStringParser.GetInt("QRY_ID"))
        grd.Mode = DocroomListHelpers.GridMode.Normal
        If Not Request.Form("preview") Is Nothing Then
            If (Request.Form("preview") = "Y") Then
                grd.Mode = DocroomListHelpers.GridMode.Preview
            End If
        End If
        msMode = PC.ScreenMode.ToLower

        If msMode = "recyclebin" Then
            If Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.isAdmin Then
                GotoErrorPage()
                Return False
            End If
        End If

        If Not IsPostBack Then
            domasearch.ObjectType = QueryStringParser.GetString("object_type")
            ResultType = CType(QueryStringParser.GetInt("result_type", Screen.ScreenSearchMode.Objects), Screen.ScreenSearchMode)

            QRY_VALUE.Value = QueryStringParser.GetString("QRY_VALUE")
            grd.GlobalSearch = QueryStringParser.GetString("globalsearch")
            _loadQueryFromScreen = Not QueryStringParser.GetBoolean("LOADQRY")

            txtQryID.Value = CStr(_queryID)
            meSelType = CType(QueryStringParser.GetInt("SELTYPE"), DMSelection.SelectionType)
            msSelDate = QueryStringParser.GetString("SELDATE")
            miCustomSelID = QueryStringParser.GetInt("SELID")

            selectiontype.Value = Convert.ToInt32(meSelType).ToString
            selectiondate.Value = msSelDate
            selectionid.Value = miCustomSelID.ToString

            _categoryId = QueryStringParser.GetInt("DM_CAT_ID")
            catid.Value = _categoryId.ToString
        Else
            SetCurrentSavedQuery(Convert.ToInt32(txtQryID.Value))
            meSelType = CType(selectiontype.Value, DMSelection.SelectionType)
            msSelDate = selectiondate.Value
            miCustomSelID = Convert.ToInt32(selectionid.Value)

            _categoryId = Convert.ToInt32(catid.Value)
        End If

        If _queryID <> 0 AndAlso String.IsNullOrEmpty(msMode) Then
            msMode = "advsearch"
        ElseIf meSelType <> DMSelection.SelectionType.Undefined AndAlso msMode = "browse" Then
            msMode = "search"
        End If

        If Not IsPostBack Then
            GotoActualParent()
            If Not CalculateScreensToUse() Then
                Return False
            End If
        End If

        If Settings.GetValue("Interface", "AllowExternalSearchScreens", False) Then
            _queryXMLProp = Request.Form("QRY_XML")
            If Not String.IsNullOrEmpty(_queryXMLProp) Then
                _loadQueryFromScreen = False
                _loadQueryFromXML = True
            End If
        End If

        RefreshTree = QueryStringParser.GetBoolean("RefreshTree")

        If Not IsPostBack Then
            PC.ScreenMode = msMode
        End If

        If msMode = "print" Then

            grd.Mode = DocroomListHelpers.GridMode.Print
            grd.ShowSideBar = False

            msPrintOptions = QueryStringParser.GetString("printoptions")

            Dim lsStartupScript As String = "<script language='javascript'>function PrintAndClose() { if (parent.GetActionPane) { const pane = parent.GetActionPane(); if (pane) { pane.Print(); } } }</script>"

            ClientScript.RegisterStartupScript(Me.GetType, "RESPRINT", lsStartupScript)
            Master.BodyTag.Attributes.Add("onload", "PrintAndClose();")
        End If

        If msMode = "whatsnew" Then
            grd.ShowCalendar = True
            grd.WhatsNewIncludeModifDate = False
        ElseIf msMode = "whatsmodified" Then
            grd.ShowCalendar = True
            grd.WhatsNewIncludeModifDate = True
        End If

        Return True
    End Function

    Private moPack As Package
    Protected ReadOnly Property CurrentPackage As Package
        Get
            If Not String.IsNullOrEmpty(PC.PackageID) AndAlso moPack Is Nothing Then
                Dim packid As Int32
                If Int32.TryParse(PC.PackageID, packid) Then
                    moPack = Package.GetPackage(packid)
                Else
                    moPack = Package.GetPackage(PC.PackageID)
                End If
            End If
            Return moPack
        End Get
    End Property

    Private Function AddPackageToCriteria(ByVal voCrit As DM_OBJECTSearch.Criteria) As DM_OBJECTSearch.Criteria
        If Not String.IsNullOrEmpty(PC.PackageID) Then
            If Not CurrentFolder.CanViewPackage(CurrentPackage.ID) Then
                GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                Return Nothing
            End If
            Select Case CurrentPackage.PackType
                Case Package.ePackType.CasePackage
                    If String.IsNullOrEmpty(CurrentPackage.DocInfo4) Then
                        grd.SetCaseGridFile("CaseOpenAndArchivedGrid.xml")
                    Else
                        grd.SetCaseGridFile(CurrentPackage.DocInfo4)
                    End If


                    voCrit.DM_PARENT_ID = 0
                    voCrit.LinkedToPackID = CurrentPackage.ID
                    voCrit.LinkedToObjectID = PC.GetParentId(False)
                    voCrit.DM_INCLUDESUBFOLDERS = True
                    voCrit.ResultType = DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                Case Package.ePackType.Docroom
                    voCrit.DM_PARENT_ID = 0
                    voCrit.LinkedToPackID = CurrentPackage.ID
                    voCrit.LinkedToObjectID = PC.GetParentId(False)
                    voCrit.DM_INCLUDESUBFOLDERS = True
                    If Not String.IsNullOrEmpty(CurrentPackage.DocInfo4) Then
                        grd.SetObjectGridFile(CurrentPackage.DocInfo4)
                    End If
                Case Package.ePackType.Query, Package.ePackType.CaseQuery
                    If Convert.ToInt32(CurrentPackage.DocInfo2) > 0 Then
                        voCrit = DMQuery.GetQuery(Convert.ToInt32(CurrentPackage.DocInfo2)).GetQueryCriteria(Folder.GetRoot(), CurrentFolder)
                    Else
                        'caml in docinfo3                          
                        voCrit.DM_PARENT_ID = 0
                        voCrit.DM_INCLUDESUBFOLDERS = True
                        voCrit.Query = New XmlQuery(TagReplacer.ReplaceTags(CurrentPackage.DocInfo3, CurrentFolder))
                        If CurrentPackage.PackType = Package.ePackType.CaseQuery Then
                            voCrit.ResultType = DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                        End If
                    End If
                    If CurrentPackage.PackType = Package.ePackType.CaseQuery Then
                        If String.IsNullOrEmpty(CurrentPackage.DocInfo4) Then
                            grd.SetCaseGridFile("CaseOpenAndArchivedGrid.xml")
                        Else
                            grd.SetCaseGridFile(CurrentPackage.DocInfo4)
                        End If
                    Else
                        If Not String.IsNullOrEmpty(CurrentPackage.DocInfo4) Then
                            grd.SetObjectGridFile(CurrentPackage.DocInfo4)
                        End If
                    End If

                    Package.AddContentFilterToSearchCriteria(voCrit, CurrentPackage.PackType, CurrentPackage.AllowedContent)
            End Select

            grd.CanEdit = CurrentFolder.CanAddToPackage(CurrentPackage.ID)

            grd.ShowFolders = True

        End If

        Dim selectionPack As Int32 = QueryStringParser.GetInt("selectionpack")
        If selectionPack > 0 Then
            'filter to package content
            Dim loForPack As Package = Package.GetPackage(selectionPack)
            Package.AddContentFilterToSearchCriteria(voCrit, loForPack.PackType, loForPack.AllowedContent)
        End If

        Return voCrit
    End Function

    Private Function GetNewCriteriaObject() As DM_OBJECTSearch.Criteria
        Return AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetNewGridCriteria(ResultType, GetCurrentParent(True), _categoryId))
    End Function

    Private Function GetBrowseCriteria() As DM_OBJECTSearch.Criteria
        Dim locrit As DM_OBJECTSearch.Criteria = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetNewBrowseGridCriteria(ResultType, GetCurrentParent(True), _categoryId))
        If locrit.ResultType = DM_OBJECTSearch.eResultType.Objects OrElse locrit.ResultType = DM_OBJECTSearch.eResultType.Files Then
            locrit.Object_Type = domasearch.ObjectType
        End If

        Return locrit
    End Function

    Private Function GetWhatsNewCriteria() As DM_OBJECTSearch.Criteria
        Dim locrit As DM_OBJECTSearch.Criteria = GetNewCriteriaObject()
        locrit.Object_Type = domasearch.ObjectType

        Return locrit
    End Function

    Private Sub LoadSearchCriteriaToScreen(ByVal loSearchcrit As DM_OBJECTSearch.Criteria)


        grd.GlobalSearch = loSearchcrit.GlobalSearchTerm

        domasearch.LoadSearchCriteria(loSearchcrit)
        grd.LoadGroupersToGrid(loSearchcrit)

    End Sub

    Private Function GetSearchCriteria(ByVal vbForSave As Boolean) As DM_OBJECTSearch.Criteria
        Dim loSearchCrit As DM_OBJECTSearch.Criteria

        If _loadQueryFromScreen Then 'load from screen
            If _queryID > 0 Then
                SetQueryName(CurrentSavedQuery.Name) 'query based on ...
                mbNeverShowSearchScreen = (CurrentSavedQuery.Type <> DMQuery.QueryType.Normal)
            End If

            loSearchCrit = GetNewCriteriaObject()
            loSearchCrit.SetResultTypeFromScreenSearchMode(domasearch.ResultType)



            If Not vbForSave Then
                domasearch.ApplyToSearchCriteria(loSearchCrit, True) 'add the fields filled in on the search screen

            Else
                domasearch.ApplyToSearchCriteria(loSearchCrit, False)
                'don't save package links
                loSearchCrit.LinkedToPackID = 0
                loSearchCrit.LinkedToObjectID = 0
                loSearchCrit.LinkedToCaseID = 0
                loSearchCrit.LinkedToArchiveCaseID = 0
            End If

            If msMode = "search" Then
                'global search from menu
                loSearchCrit.DM_INCLUDESUBFOLDERS = True
            End If
        ElseIf _loadQueryFromXML Then
            loSearchCrit = DM_OBJECTSearch.Criteria.Deserialize(_queryXMLProp, CurrentFolder)
            'update the screen too
            LoadSearchCriteriaToScreen(DM_OBJECTSearch.Criteria.Deserialize(_queryXMLProp, Nothing))
        Else 'load from db or property expansion


            SetQueryName(CurrentSavedQuery.Name)

            Select Case CurrentSavedQuery.Type
                Case DMQuery.QueryType.PropertyExpansion
                    loSearchCrit = CurrentSavedQuery.GetPropertyExpansionQueryCriteria(QRY_VALUE.Value)

                    mbNeverShowSearchScreen = True

                    SetParentId(loSearchCrit.DM_PARENT_ID)
                    domasearch.ResultType = CurrentSavedQuery.ResultType
                    ResultType = CurrentSavedQuery.ResultType

                    loSearchCrit.SetResultTypeFromScreenSearchMode(domasearch.ResultType)

                    'load the property expansion into the search screen, for uniformity. Assume all fields are on the default search screen
                    LoadSearchCriteriaToScreen(loSearchCrit)
                Case Else
                    mbNeverShowSearchScreen = (CurrentSavedQuery.Type = DMQuery.QueryType.System)
                    loSearchCrit = CurrentSavedQuery.GetQueryCriteria(CurrentFolder, CurrentFolder)
                    SetParentId(loSearchCrit.DM_PARENT_ID)

                    domasearch.ResultType = CurrentSavedQuery.ResultType
                    ResultType = CurrentSavedQuery.ResultType

                    'update the screen too                        
                    LoadSearchCriteriaToScreen(CurrentSavedQuery.GetQueryCriteria(CurrentFolder, Nothing))

            End Select

        End If

        If Not vbForSave Then
            If meSelType <> DMSelection.SelectionType.Undefined Then

                loSearchCrit.SelectionType = meSelType
                loSearchCrit.SelectionDate = msSelDate
                loSearchCrit.SelectionID = miCustomSelID
                loSearchCrit.DM_PARENT_ID = 0 'don't include the folder in basket/favorites/....
                loSearchCrit.DM_INCLUDESUBFOLDERS = True
                grd.ShowFolders = True
            Else
                loSearchCrit.SelectionType = DMSelection.SelectionType.Undefined
            End If
        End If
        Return loSearchCrit
    End Function

    Private mbNeverShowSearchScreen As Boolean

    Protected Function ShowBackToSearchLink() As Boolean
        Return (Not mbNeverShowSearchScreen AndAlso Not ShowSearchScreenWhenListIsVisible())
    End Function

    Protected Function ShowSearchScreenWhenListIsVisible() As Boolean
        Return Not mbNeverShowSearchScreen AndAlso (UserProfile.ShowSearchScreenInList OrElse grd.KeepSearchScreenVisible)
    End Function

    Private Sub SetGridXmlFile(ByVal vbBrowse As Boolean)


        Select Case meSelType
            Case DMSelection.SelectionType.Basket
                grd.SetGridFile("BasketGrid.xml")
            Case DMSelection.SelectionType.Recent
                grd.SetGridFile("RecentDocsGrid.xml")
            Case DMSelection.SelectionType.CDRom
                grd.SetGridFile("CDRomGrid.xml")
            Case DMSelection.SelectionType.Favorites
                grd.SetGridFile("FavoritesGrid.xml")
            Case Else

                If grd.ResultScreenID > 0 Then
                    Dim s As Screen = Screen.GetScreen(grd.ResultScreenID)
                    If s.ID <> 0 AndAlso s.Type = Screen.ScreenSourceType.DefaultMode AndAlso Not String.IsNullOrEmpty(s.Source) Then
                        grd.SetGridFile(Arco.Settings.FrameWorkSettings.ReplaceGlobalVars(s.Source, False))
                        Exit Sub
                    End If
                End If

                If _categoryId > 0 AndAlso CurrentCategory.CatType = OBJECT_CATEGORY.eType.ListItem Then
                    grd.SetGridFile("ListItemGrid.xml")
                Else
                    Select Case Me.ResultType
                        Case Screen.ScreenSearchMode.ArchivedCases
                            grd.SetCaseGridFile("CaseArchiveGrid.xml")
                        Case Screen.ScreenSearchMode.Cases
                            grd.SetCaseGridFile("CaseGrid.xml")
                        Case Screen.ScreenSearchMode.MyCases
                            grd.SetCaseGridFile("MyCasesGrid.xml")
                        Case Screen.ScreenSearchMode.OpenAndArchivedCases
                            grd.SetCaseGridFile("CaseOpenAndArchivedGrid.xml")
                        Case Screen.ScreenSearchMode.Work
                            grd.SetCaseGridFile("MyWorkGrid.xml")
                        Case Screen.ScreenSearchMode.MailInbox
                            grd.SetMailGridFile("MailInbox.xml")
                        Case Screen.ScreenSearchMode.MailInboxWork
                            grd.SetMailGridFile("MailInbox.xml")
                        Case Screen.ScreenSearchMode.MailFollowUp
                            grd.SetMailGridFile("MailFollowUp.xml")
                        Case Screen.ScreenSearchMode.MailSentItems
                            grd.SetMailGridFile("MailOutbox.xml")
                        Case Screen.ScreenSearchMode.MailDeletedBox
                            grd.SetMailGridFile("MailDeletedBox.xml")
                        Case Screen.ScreenSearchMode.MyMails
                            grd.SetMailGridFile("MyMail.xml")
                        Case Screen.ScreenSearchMode.Mails
                            grd.SetMailGridFile("MailGrid.xml")
                        Case Else

                            If vbBrowse Then
                                grd.SetObjectGridFile("BrowseGrid.xml")
                            Else
                                grd.SetObjectGridFile("SearchResultsGrid.xml")
                            End If
                    End Select
                End If
        End Select
    End Sub

    Private Sub ShowDefaultSearchHeader()

        Select Case ResultType
            Case Screen.ScreenSearchMode.ArchivedCases
                ShowSearchHeaderText(GetLabel("search") & " : " & GetLabel("archive"))
            Case Screen.ScreenSearchMode.OpenAndArchivedCases
                ShowSearchHeaderText(GetLabel("search") & " : " & GetDecodedLabel("openandarchivedcases"))
            Case Screen.ScreenSearchMode.Cases
                ShowSearchHeaderText(GetLabel("search") & " : " & GetLabel("opencases"))
            Case Screen.ScreenSearchMode.Work
                ShowSearchHeaderText(GetLabel("search") & " : " & GetLabel("mywork"))
            Case Screen.ScreenSearchMode.MyCases
                ShowSearchHeaderText(GetLabel("search") & " : " & GetLabel("mycases"))
            Case Screen.ScreenSearchMode.Files
                ShowSearchHeaderText(GetLabel("search") & " : " & GetLabel("files"))
            Case Screen.ScreenSearchMode.Mails
                ShowSearchHeaderText(GetLabel("search") & " : " & GetLabel("mail"))
            Case Else 'Screen.ScreenSearchMode.Objects
                Dim objType As String = domasearch.ObjectType
                If String.IsNullOrEmpty(objType) OrElse objType.Contains(",") Then
                    ShowSearchHeaderText(GetLabel("search"))
                Else
                    ShowSearchHeaderText(GetLabel("search") & " : " & GetDecodedLabel(objType & "s"))
                End If

        End Select
    End Sub

    Private Sub ShowSearchHeader(ByVal screen As Screen)
        If screen.ShowToolbar Then
            ShowSearchHeaderText(screen.Caption)
        Else
            HideSearchHeader()
        End If
    End Sub

    Private Sub HideSearchHeader()
        pnlSearchHeader.Visible = False
        lblTemplateHeader.Visible = False
    End Sub

    Private Sub ShowSearchHeaderText(ByVal vsText As String)
        If Not String.IsNullOrEmpty(vsText) Then
            If pnlFixedHeader.Visible Then
                pnlSearchHeader.Visible = True
                lblSearchHeader.Text = Server.HtmlEncode(vsText)
            ElseIf pnlTemplateHeader.Visible Then
                lblTemplateHeader.Visible = True
                lblTemplateHeader.Text = Server.HtmlEncode(vsText) & "<br>"
            End If
        Else
            ShowDefaultSearchHeader()
        End If
    End Sub

    Private Sub ShowResultCount(ByVal viCount As Int32)
        If pnlFixedHeader.Visible Then
            If pnlSearchHeader.Visible Then
                lblSearchHeader.Text &= " (" & GetDecodedLabel("resultcount") & " : " & viCount & ")"
            Else
                'show the header for the count
                pnlSearchHeader.Visible = True
                lblSearchHeader.Text = GetDecodedLabel("resultcount") & " : " & viCount
            End If

        ElseIf pnlTemplateHeader.Visible Then
            lblTemplateHeader.Text = " (" & GetDecodedLabel("resultcount") & " : " & viCount & ")"
        End If
    End Sub

    Private _currentParent As DM_OBJECT
    Private _currentParent2 As DM_OBJECT

    ''' <summary>
    ''' can be the folder or a query
    ''' </summary>
    ''' <returns></returns>
    Public Function GetCurrentParent(ByVal returnRootIDAsDefault As Boolean) As DM_OBJECT

        If returnRootIDAsDefault Then
            If _currentParent Is Nothing Then
                _currentParent = ObjectRepository.GetObject(PC.GetParentId(True))
                If _currentParent Is Nothing OrElse Not _currentParent.IsForUserTenant(True) Then
                    Return Nothing
                End If
            End If
            Return _currentParent
        Else
            If _currentParent2 Is Nothing Then
                _currentParent2 = ObjectRepository.GetObject(PC.GetParentId(False))
                If _currentParent2 Is Nothing OrElse Not _currentParent2.IsForUserTenant(True) Then
                    Return Nothing
                End If
            End If
            Return _currentParent2
        End If

    End Function

    Private Function CurrentFolderIsActualFolder() As Boolean
        Return CurrentFolder IsNot Nothing AndAlso CurrentFolder.ID <> 0
    End Function

    Private _currentFolder As DM_OBJECT

    Public ReadOnly Property CurrentFolder() As DM_OBJECT
        Get
            If _currentFolder Is Nothing Then
                Dim p As DM_OBJECT = GetCurrentParent(True)
                Dim qs As QueryShortcut = TryCast(p, QueryShortcut)

                If qs IsNot Nothing Then
                    ResultType = qs.Query.ResultType
                    _currentFolder = qs.GetExecutionFolder
                Else
                    _currentFolder = p
                End If
            End If
            Return _currentFolder
        End Get
    End Property


    Private _currentCat As OBJECT_CATEGORY

    Public ReadOnly Property CurrentCategory() As OBJECT_CATEGORY
        Get
            If _currentCat Is Nothing AndAlso _categoryId > 0 Then
                _currentCat = OBJECT_CATEGORY.GetOBJECT_CATEGORY(_categoryId)
            End If
            Return _currentCat
        End Get
    End Property

    Private Sub SetParent(ByVal voParent As DM_OBJECT)
        SetParentId(voParent.ID)
        _currentParent = voParent
    End Sub

    Private Sub SetParentId(ByVal vparentid As Integer)
        If vparentid <> PC.GetParentId(True) Then
            PC.ParentID = vparentid
            _currentParent = Nothing
        End If

    End Sub

    Private Sub ShowSearchButtons(ByVal voScreenItems As ScreenItemList)
        Dim lbHasButtons As Boolean = False

        For Each loItem As ScreenItemList.ScreenItemInfo In voScreenItems
            If loItem.Type = ScreenItem.ItemType.Fixed Then
                Select Case loItem.FixedItemID
                    Case ScreenItem.FixedItem.SearchCountSearchButton, ScreenItem.FixedItem.SearchSearchButton, ScreenItem.FixedItem.SearchOpenSearchButton, ScreenItem.FixedItem.SearchSaveSearchButton
                        lbHasButtons = True
                End Select
            End If
        Next
        If Not lbHasButtons Then
            lnkButtons.ShowCountButton = True
            lnkButtons.ShowFindButton = True
            lnkButtons.ShowOpenButton = domasearch.ShowOpenAndSaveButtons
            lnkButtons.ShowSaveButton = domasearch.ShowOpenAndSaveButtons
            pnlFixedButtons.Visible = True
        Else
            HideSearchButtons()
        End If
    End Sub

    Private Sub HideSearchButtons()

        pnlFixedButtons.Visible = False
    End Sub

    Private Sub ToggleFixedSearchScreen(ByVal show As Boolean)
        pnlFixedHeader.Visible = show
        pnlFixedHeaderCloser.Visible = show
        pnlTemplateHeader.Visible = Not show
    End Sub

    Private Sub LoadResults()

        trMessages.Visible = False

        Dim resultListCrit As DM_OBJECTSearch.Criteria = Nothing
        Dim showGrid As Boolean = True

        domasearch.Folder = CurrentFolder
        If msMode = "browse" Then
            domasearch.IncludeSubFolders = False
        Else
            domasearch.IncludeSubFolders = True
        End If

        domasearch.ShowOpenAndSaveButtons = Security.BusinessIdentity.CurrentIdentity().IsAuthenticated AndAlso String.IsNullOrEmpty(PC.PackageID)

        Dim isPropEx As Boolean = False
        Dim isSystemQuery As Boolean = False
        If _queryID <> 0 Then
            isPropEx = (CurrentSavedQuery.Type = DMQuery.QueryType.PropertyExpansion) 'property expansions require the pool properties query screen
            isSystemQuery = (CurrentSavedQuery.Type = DMQuery.QueryType.System)

            domasearch.ShowOpenAndSaveButtons = (CurrentSavedQuery.Type = DMQuery.QueryType.Normal)
        End If
        If Not isPropEx Then
            Dim objScreen As Screen

            If isSystemQuery Then

                Dim screenItemList As ScreenItemList = ScreenItemList.GetEmptyScreenItemList()
                screenItemList.Add(New ScreenItemList.ScreenItemInfo() With {.Type = ScreenItem.ItemType.Fixed, .FixedItemID = ScreenItem.FixedItem.SearchXml})

                objScreen = Screen.GetDefaultScreen(screenItemList, Nothing, Nothing)
                objScreen.SearchMode = ResultType

            ElseIf SearchScreen = 0 Then
                ToggleFixedSearchScreen(True)

                If _categoryId <> 0 Then
                    domasearch.CategoryID = _categoryId
                End If

                Select Case ResultType
                    Case Screen.ScreenSearchMode.Objects, Screen.ScreenSearchMode.Files
                        If _categoryId <> 0 Then
                            objScreen = ScreenSelector.GetDefaultCategorySearchScreen(CurrentCategory, EnableIISCaching)
                        Else
                            objScreen = ScreenSelector.GetDefaultObjectSearchScreen(domasearch.CategoryID, EnableIISCaching)
                        End If
                    Case Screen.ScreenSearchMode.MailInbox, Screen.ScreenSearchMode.MailDeletedBox, Screen.ScreenSearchMode.MailFollowUp, Screen.ScreenSearchMode.MailInboxWork, Screen.ScreenSearchMode.MailSentItems, Screen.ScreenSearchMode.MyMails, Screen.ScreenSearchMode.Mails
                        objScreen = ScreenSelector.GetDefaultMailSearchScreen(ResultType, EnableIISCaching)
                    Case Else ' Case Screen.ScreenSearchMode.ArchivedCases, Screen.ScreenSearchMode.Cases, Screen.ScreenSearchMode.Work
                        objScreen = ScreenSelector.GetDefaultCaseSearchScreen(domasearch.ProcedureID, ResultType, EnableIISCaching)
                End Select
                objScreen.SearchMode = ResultType
                objScreen.ShowToolbar = False
            Else
                objScreen = Screen.GetScreen(SearchScreen)


                If objScreen.Type = Screen.ScreenSourceType.TemplateFile AndAlso Not String.IsNullOrEmpty(objScreen.Source) Then
                    domasearch.ResultType = objScreen.SearchMode
                    domasearch.datasource = objScreen.ScreenItems
                    domasearch.Template = Arco.Settings.FrameWorkSettings.ReplaceGlobalVars(objScreen.Source, False)
                    ToggleFixedSearchScreen(False)

                    HideSearchButtons()
                    objScreen = Nothing 'cancel the rest later
                ElseIf objScreen.Type = Screen.ScreenSourceType.Url AndAlso Not String.IsNullOrEmpty(objScreen.Source) AndAlso (msMode = "query" OrElse msMode = "querycount") Then
                    RedirectToCustomUrl(objScreen.Source)
                    Exit Sub

                End If
                domasearch.CategoryID = objScreen.Category
                domasearch.ProcedureID = objScreen.Procedure

            End If

            If objScreen IsNot Nothing Then
                ToggleFixedSearchScreen(True)

                ResultType = objScreen.SearchMode
                domasearch.datasource = objScreen.ScreenItems
                domasearch.ResultType = objScreen.SearchMode
                domasearch.ValidationScript = objScreen.ValidationScript

                ShowSearchHeader(objScreen)

                ShowSearchButtons(objScreen.ScreenItems)
                domasearch.Labels = objScreen.GetScreenItemLabels(EnableIISCaching)
                domasearch.FormSize = objScreen.FormSize
                domasearch.FormFieldsLayout = objScreen.FormFieldsLayout
            End If


            Dim searchInPackId As Int32 = QueryStringParser.GetInt("selectionpack", 0)
            If searchInPackId = 0 AndAlso CurrentPackage IsNot Nothing Then
                searchInPackId = CurrentPackage.ID
            End If

            If searchInPackId > 0 Then
                'filter to package content
                Dim forPack As Package = Package.GetPackage(searchInPackId)
                For Each contentInfo As PackageAllowedContentList.ContentInfo In forPack.AllowedContent
                    If contentInfo.Content_ID > 0 Then
                        If contentInfo.Content_Type = "Procedure" Then
                            domasearch.LimitToProcedures.Add(contentInfo.Content_ID)
                        Else
                            domasearch.LimitToCategories.Add(contentInfo.Content_ID)
                        End If
                    Else
                        If contentInfo.Content_Type = "Procedure" Then
                            domasearch.LimitToProcedures.Clear()
                        Else
                            For Each loAllCat As OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In OBJECT_CATEGORYList.GetOBJECT_CATEGORYList(contentInfo.Content_Type, False)
                                domasearch.LimitToCategories.Add(loAllCat.ID)
                            Next
                        End If
                    End If
                Next
            End If

        Else
            'property expansion
            ToggleFixedSearchScreen(True)

            Dim propIds As New List(Of Int32)
            Dim fixedPropIds As New List(Of Int32)
            Dim qryRef As QueryShortcut = QueryShortcut.GetQueryShortcutFromQuery(CurrentSavedQuery)

            If qryRef.Default_Result_Screen > 0 Then
                grd.ResultScreenID = qryRef.Default_Result_Screen
            End If

            For Each expandedPropID As Int32 In qryRef.GetAllExpandedProperties
                If DM_PROPERTY.isFixedProperty(expandedPropID) Then
                    fixedPropIds.Add(expandedPropID)
                Else
                    propIds.Add(expandedPropID)
                End If
            Next


            Dim items As ScreenItemList
            If propIds.Count <> 0 Then
                Dim lcolProps As PROPERTYList = PROPERTYList.GetPropertyList(String.Join(",", propIds))
                items = ScreenItemList.GetSearchScreenItems(lcolProps)
            Else
                items = ScreenItemList.GetEmptyScreenItemList()
            End If


            For Each fixedProp As Int32 In fixedPropIds
                Select Case fixedProp
                    Case Condition.FixedField.CreatedBy
                        items.Add(New ScreenItemList.ScreenItemInfo With {.Type = ScreenItem.ItemType.Fixed, .FixedItemID = ScreenItem.FixedItem.CreationUser})
                    Case Condition.FixedField.ModifiedBy
                        items.Add(New ScreenItemList.ScreenItemInfo With {.Type = ScreenItem.ItemType.Fixed, .FixedItemID = ScreenItem.FixedItem.ModifUser})
                    Case Condition.FixedField.Name
                        items.Add(New ScreenItemList.ScreenItemInfo With {.Type = ScreenItem.ItemType.Fixed, .FixedItemID = ScreenItem.FixedItem.Name})
                    Case Condition.FixedField.Status
                        items.Add(New ScreenItemList.ScreenItemInfo With {.Type = ScreenItem.ItemType.Fixed, .FixedItemID = ScreenItem.FixedItem.SearchStatus})
                    Case Condition.FixedField.AssignedTo
                        items.Add(New ScreenItemList.ScreenItemInfo With {.Type = ScreenItem.ItemType.Fixed, .FixedItemID = ScreenItem.FixedItem.AssignedTo})
                            '    'Case Condition.FixedField.Procedure
                            '    lcolitems.Add(New Arco.Doma.Library.ScreenItemList.ScreenItemInfo With {.Type = ScreenItem.ItemType.Fixed, .FixedItemID = ScreenItem.FixedItem.Procedure})
                    Case Condition.FixedField.StepName
                        items.Add(New ScreenItemList.ScreenItemInfo With {.Type = ScreenItem.ItemType.Fixed, .FixedItemID = ScreenItem.FixedItem.StepName})
                        '    'Case Condition.FixedField.Category
                        '    lcolitems.Add(New Arco.Doma.Library.ScreenItemList.ScreenItemInfo With {.Type = ScreenItem.ItemType.Fixed, .FixedItemID = ScreenItem.FixedItem.Category})
                End Select
            Next

            domasearch.datasource = items

            ' domasearch.Labels = Globalisation.LABELList.GetCategoryItemsLabelList(domasearch.CategoryID, Me.EnableIISCaching)
            domasearch.ResultType = ResultType
            ' ShowDefaultSearchHeader()
            HideSearchHeader()
            ShowSearchButtons(domasearch.datasource)
        End If

        grd.CurrentFolder = GetCurrentParent(True)
        grd.ShowFolders = UserProfile.ShowFoldersInList
        grd.ShowQuery = UserProfile.ShowQuery

        grd.InSelectionMode = QueryStringParser.Exists("selectionmode")

        pnlSaveQuery.Visible = False

        Select Case msMode
            Case "print"
                Dim xmlFile As String = QueryStringParser.GetString("printxml")
                If String.IsNullOrEmpty(xmlFile) Then
                    grd.SetGridFile("printgrid.xml")
                Else
                    grd.SetGridFile(xmlFile)
                End If
                grd.PrintOptions = msPrintOptions
                grd.AllowChangeInCriteria = False
                Select Case msPrintOptions
                    Case "0" 'selecton
                        resultListCrit = Security.BusinessIdentity.CurrentIdentity.GetLastQuery()
                        Dim lsOrderBy As String = resultListCrit.ORDERBY

                        resultListCrit = GetNewCriteriaObject()
                        Dim liSelType As DMSelection.SelectionType = CType(QueryStringParser.GetInt("printseltype", DMSelection.SelectionType.Current), DMSelection.SelectionType)
                        resultListCrit.SelectionType = liSelType
                        Select Case liSelType
                            Case DMSelection.SelectionType.CurrentCases
                                resultListCrit.ResultType = DM_OBJECTSearch.eResultType.CaseList
                            Case DMSelection.SelectionType.CurrentCasesByCaseID
                                resultListCrit.ResultType = DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                            Case DMSelection.SelectionType.CurrentMails
                                resultListCrit.ResultType = DM_OBJECTSearch.eResultType.Mails
                        End Select
                        resultListCrit.DM_PARENT_ID = 0
                        resultListCrit.DM_INCLUDESUBFOLDERS = True
                        resultListCrit.ORDERBY = lsOrderBy

                        grd.ShowScroller = False
                        grd.ShowFolders = True
                    Case "1", "2"
                        resultListCrit = Security.BusinessIdentity.CurrentIdentity.GetLastQuery()

                        grd.ShowPager = False
                        If msPrintOptions = "1" Then 'current page
                            grd.CurrentPage = QueryStringParser.GetInt("printpage", 1)
                        Else
                            'all pages
                            grd.ShowScroller = False
                        End If
                End Select
            Case "browse"
                SetGridXmlFile(True)
                resultListCrit = GetBrowseCriteria()
            Case "myincreation"
                grd.SetObjectGridFile("SearchresultsGrid.xml")
                resultListCrit = GetSearchCriteria(False)
                resultListCrit.Status = DM_OBJECT.Object_Status.InCreation
                resultListCrit.CreatedBy = Security.BusinessIdentity.CurrentIdentity.Name
                resultListCrit.DM_INCLUDESUBFOLDERS = True
                resultListCrit.DM_PARENT_ID = 0
            Case "browsewithsubfolders"
                SetGridXmlFile(True)
                resultListCrit = GetBrowseCriteria()
                resultListCrit.DM_INCLUDESUBFOLDERS = True
            Case "whatsnew", "whatsmodified"
                grd.SetObjectGridFile("WhatsNewGrid.xml")
                grd.ShowFolders = True
                resultListCrit = GetWhatsNewCriteria()
            Case "mymail"
                grd.SetMailGridFile("MyMail.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetMyMailGridCriteria())
            Case "mymailwork"
                grd.SetMailGridFile("MyMailWork.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetMyMailWorkGridCriteria())
            Case "mymailfollowup"
                grd.SetMailGridFile("MailFollowUp.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetMyMailFollowUpGridCriteria())
            Case "mymailinbox"
                grd.SetMailGridFile("MailInbox.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetMyMailInboxGridCriteria())
            Case "mymailoutbox"
                grd.SetMailGridFile("MailOutbox.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetMyMailOutboxGridCriteria())
            Case "mymaildeletedbox"
                grd.SetMailGridFile("MailDeletedbox.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetMyMailDeletedboxGridCriteria())
            Case "linkedmails"
                grd.SetMailGridFile("linkedmails.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetLinkedMailsGridCriteria(CurrentFolder.ID))
            Case "mycheckedoutdocs"
                grd.SetGridFile("MyCheckedOutDocsGrid.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetNewMyCheckedOutDocsGridCriteria())
            Case "links"
                grd.SetGridFile("LinkedDocsGrid.xml")
                resultListCrit = DM_OBJECTSearch.Criteria.GetNewLinkedObjectsGridCriteria(CurrentFolder.DIN)
            Case "mywork"
                grd.SetCaseGridFile("MyWorkGrid.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetNewMyWorkGridCriteria(GetCurrentParent(False)))
            Case "opencases"
                grd.SetCaseGridFile("CaseGrid.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetNewOpenDossiersGridCriteria(GetCurrentParent(False)))
            Case "mycases"
                grd.SetCaseGridFile("MyCasesGrid.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetNewMyDossiersGridCriteria(GetCurrentParent(False)))
            Case "archivedcases"
                grd.SetCaseGridFile("CaseArchiveGrid.xml")
                resultListCrit = AddPackageToCriteria(DM_OBJECTSearch.Criteria.GetNewArchivedDossiersGridCriteria(GetCurrentParent(False)))
            Case "recyclebin"
                grd.SetGridFile("RecycleBinGrid.xml")
                grd.ShowFolders = True
                resultListCrit = RecycleBin.GetSearchCriteria
            Case "prevversions"
                grd.SetGridFile("PreviousVersionsGrid.xml")
                resultListCrit = DM_OBJECTSearch.Criteria.GetNewAllVersionsGridCriteria(CurrentFolder.DIN)
            Case "init", "foldertext"
                showGrid = False
                pnlFilter.Visible = False
            Case "query", "querycount"
                showGrid = False
                pnlFilter.Visible = True


                If Not _loadQueryFromScreen AndAlso Not _loadQueryFromXML Then 'load query into the screen from database
                    Dim searchCrit As DM_OBJECTSearch.Criteria = CurrentSavedQuery.GetQueryCriteria(CurrentFolder, Nothing)

                    SetParentId(searchCrit.DM_PARENT_ID)
                    'update the screen too
                    SetQueryName(CurrentSavedQuery.Name)
                    LoadSearchCriteriaToScreen(searchCrit) 'CurrentSavedQuery.GetQueryCriteria(CurrentFolder, Nothing))
                ElseIf _loadQueryFromXML Then
                    'update the screen too
                    LoadSearchCriteriaToScreen(DM_OBJECTSearch.Criteria.Deserialize(_queryXMLProp, Nothing))
                Else
                    If _queryID <> 0 Then 'just load the name of the query
                        SetQueryName(CurrentSavedQuery.Name) ' + expansion
                    End If
                End If
                SetGridXmlFile(False)
                If msMode = "querycount" Then
                    resultListCrit = GetSearchCriteria(False)

                    grd.AddFiltersToCriteria(resultListCrit)
                    resultListCrit.ForceJoinOnDMObject = grd.Layout.ForceJoinOnDMObject
                    ' loResultListCrit.SelectionType = DMSelection.SelectionType.Undefined

                    If String.IsNullOrEmpty(resultListCrit.Object_Type) AndAlso resultListCrit.ResultType = DM_OBJECTSearch.eResultType.Objects Then
                        resultListCrit.Exclude_Object_Type = "Query"
                        If Not UserProfile.ShowFoldersInList Then
                            resultListCrit.Exclude_Object_Type = "Folder"
                        End If
                    End If
                    Try
                        ShowResultCount(DM_OBJECTSearch.GetObjectListCount(resultListCrit))
                    Catch ex As Exception
                        Arco.Utils.Logging.LogError(ex.Message, ex)

                        ShowErrorMessage("An unexpected error has occured executing the request")

                    End Try
                End If
            Case "savequery"
                showGrid = False
                pnlFilter.Visible = False
                pnlSaveQuery.Visible = True

                Dim canEditPublicQuery = (Security.BusinessIdentity.CurrentIdentity.isAdmin OrElse Folder.GetRoot().HasAccess(ACL.ACL_Access.Access_Level.ACL_Manage_Public_Saved_Queries))
                rdQryScope.Items(0).Enabled = canEditPublicQuery
                If _queryID <> 0 Then
                    SetQueryName(CurrentSavedQuery.Name)
                    If String.IsNullOrEmpty(txtQryName.Text) Then 'load name and usebasefolder from db
                        txtQryName.Text = CurrentSavedQuery.Name
                        txtQryDesc.Text = CurrentSavedQuery.Description
                        rdQryScope.SelectedValue = Convert.ToInt32(CurrentSavedQuery.Scope).ToString
                        optFolderToUse.SelectedValue = CStr(CurrentSavedQuery.UseBaseFolder)

                    End If
                    lnkCancelQry.OnClientClick = "javascript:EditQuery(" & CurrentSavedQuery.ID & "," & CurrentSavedQuery.SearchScreenID & "," & Convert.ToInt32(CurrentSavedQuery.ResultType) & ");"
                    lnkQueryACL.Visible = False

                    Select Case CurrentSavedQuery.Scope
                        Case DMQuery.QueryScope.ACLScope
                            lnkQueryACL.Visible = True
                            lnkQueryACL.OnClientClick = "javascript:DoQueryACL(" & CurrentSavedQuery.ID & ");return false;"
                            lnkQueryACL.Text = GetLabel("ctx_aclsetacl")
                    End Select

                    lnkSaveQuery.Visible = CurrentSavedQuery.CanModify(canEditPublicQuery)

                End If
            Case "search", "advsearch"  'search
                resultListCrit = GetSearchCriteria(False)
                SetGridXmlFile(False)

                ResultType = ConvertCriteriaResultType(resultListCrit)

                If msMode = "advsearch" Then
                    grd.ShowBackToSearchLink = ShowBackToSearchLink()
                End If
            Case Else 'no mode given
                pnlFilter.Visible = True
                showGrid = False
        End Select


        If showGrid Then

            resultListCrit.ScreenID = SearchScreen 'audit purposes
            ResultType = ConvertCriteriaResultType(resultListCrit)
            grd.ShowSearchedForCategory = (_categoryId = 0)
            CopyCategoryAndProcedureLimitsToGrid()

            grd.ThrowExceptions = IsPostBack AndAlso pnlFilter.Visible
            Try
                grd.DataBind(resultListCrit)

                pnlFilter.Visible = ShowSearchScreenWhenListIsVisible()
            Catch ex As Exception
                ShowErrorMessage("An unexpected error has occured executing the request")

                Arco.Utils.Logging.LogError(ex.Message, ex)

            End Try
        End If

        'grouper on the search panel
        If pnlFilter.Visible Then
            pnlgrouper.Visible = False
            If grd.GroupingEnabled Then
                For Each node As DocroomListHelpers.GridGroupingInfo.Grouper In grd.GridGrouping.GrouperList
                    If node.UserGrouper AndAlso node.ShowInSearchScreen Then
                        pnlgrouper.Visible = True
                        ' lcolFields = loNode.SelectNodes("field")
                        Dim chboxGrouper As CheckBox = New CheckBox
                        chboxGrouper.AutoPostBack = False
                        chboxGrouper.Text = node.GetLabel
                        Dim sbJS As StringBuilder = New StringBuilder
                        sbJS.Append("javascript:")
                        For Each loFieldNode As DocroomListHelpers.GridGroupingInfo.Grouper.GrouperField In node.Fields
                            sbJS.Append("PC().GetDefaultResultGrid().ToggleGrouper(")
                            sbJS.Append(node.ID)
                            sbJS.Append(",'")
                            sbJS.Append(loFieldNode.GetFieldName(grd.ContentParams))
                            sbJS.Append("');")
                        Next
                        chboxGrouper.Attributes.Add("onclick", sbJS.ToString)

                        chboxGrouper.Checked = grd.IsGroupedOn(node.ID)
                        plhGrouperCheckboxes.Controls.Add(chboxGrouper)
                        plhGrouperCheckboxes.Controls.Add(New LiteralControl("<br/>"))
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub CopyCategoryAndProcedureLimitsToGrid()
        If domasearch.LimitToCategories.Any Then
            If grd.ContentParams.GridLimitedToCategory.Count = 0 Then
                For Each liCat As Int32 In domasearch.LimitToCategories
                    grd.ContentParams.GridLimitedToCategory.Add(liCat)
                Next
            End If
        End If
        If domasearch.LimitToProcedures.Any Then
            If grd.ContentParams.GridLimitedToProcedure.Count = 0 Then
                For Each liProc As Int32 In domasearch.LimitToProcedures
                    grd.ContentParams.GridLimitedToProcedure.Add(liProc)
                Next
            End If
        End If
    End Sub

    Private Function ConvertCriteriaResultType(ByVal searchCriteria As DM_OBJECTSearch.Criteria) As Screen.ScreenSearchMode
        Select Case searchCriteria.ResultType
            Case DM_OBJECTSearch.eResultType.Objects
                Return Screen.ScreenSearchMode.Objects
            Case DM_OBJECTSearch.eResultType.Files
                Return Screen.ScreenSearchMode.Files
            Case DM_OBJECTSearch.eResultType.Mails
                If searchCriteria.MailSearch.MailBoxSubject = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
                    Select Case searchCriteria.MailSearch.MailBoxType
                        Case Mail.MailBoxItem.MailBox.Inbox
                            If searchCriteria.MailSearch.UserHasWork Then
                                Return Screen.ScreenSearchMode.MailInboxWork
                            Else
                                Return Screen.ScreenSearchMode.MailInbox
                            End If
                        Case Mail.MailBoxItem.MailBox.Sent
                            If searchCriteria.MailSearch.JoinMailTracking Then
                                Return Screen.ScreenSearchMode.MailFollowUp
                            Else
                                Return Screen.ScreenSearchMode.MailSentItems
                            End If
                        Case Mail.MailBoxItem.MailBox.Undefined
                            If searchCriteria.MailSearch.MailBoxStatus = Mail.MailBoxItem.MailStatus.Open Then
                                Return Screen.ScreenSearchMode.MyMails
                            Else
                                Return Screen.ScreenSearchMode.MailDeletedBox
                            End If
                        Case Else
                            Return Screen.ScreenSearchMode.Mails
                    End Select
                Else
                    Return Screen.ScreenSearchMode.Mails
                End If

            Case DM_OBJECTSearch.eResultType.ArchivedCasesList
                Return Screen.ScreenSearchMode.ArchivedCases
            Case DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                Return Screen.ScreenSearchMode.OpenAndArchivedCases
            Case DM_OBJECTSearch.eResultType.CaseList
                If searchCriteria.CaseSearch.FilterOnWork Then
                    Return Screen.ScreenSearchMode.Work
                Else
                    If searchCriteria.CreatedBy = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
                        Return Screen.ScreenSearchMode.MyCases
                    Else
                        Return Screen.ScreenSearchMode.Cases
                    End If
                End If
            Case Else
                Return Screen.ScreenSearchMode.Objects

        End Select
    End Function

    Private Sub SetQueryName(ByVal vsName As String)
        grd.QueryName = vsName
        'If mlSearchScreenID = 0 Then
        ShowSearchHeaderText(vsName)
        'End If
    End Sub

    Private Sub ShowErrorMessage(ByVal msg As String)
        '     pnlFilter.Visible = True
        trMessages.Visible = True
        lblErrors.Text = msg


    End Sub

    Private Sub RedirectToCustomUrl(ByVal vsUrl As String)
        If Settings.GetValue("Interface", "AllowExternalSearchScreens", False) Then
            Dim loCrit As DM_OBJECTSearch.Criteria = GetSearchCriteria(False)
            Dim lsScript As String
            lsScript = "<html><body><form name='frmRedirect' method='post' action='" & vsUrl & "'>"
            lsScript = lsScript & "<input type=hidden name='QRY_XML' VALUE='" & loCrit.Serialize & "' />"
            lsScript = lsScript & "<input type=hidden name='DM_SEARCH_SCREEN_ID' value='" & SearchScreen & "'>"
            lsScript = lsScript & "<input type=hidden name='DM_RESULT_SCREEN_ID' value='" & grd.ResultScreenID & "'>"
            lsScript = lsScript & "<script language='javascript'>document.frmRedirect.submit();</script>"
            lsScript = lsScript & "</form></body></html>"

            Response.Write(lsScript)
            Response.End()
        Else
            Arco.Utils.Logging.LogError("AllowExternalSearchScreens is not enabled")

            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOPERATION)
        End If
    End Sub
#End Region

#Region " Saved Queries "
    Protected Sub SaveQuery(ByVal sender As Object, ByVal e As EventArgs)
        If Not IsValid Then
            Return
        End If

        If DMQueryList.GetMyQueryList().Cast(Of DMQueryList.QueryInfo).
                Any(Function(x) x.ID <> _queryID AndAlso x.Name.Equals(txtQryName.Text, StringComparison.CurrentCultureIgnoreCase)) Then

            JSMessageError = GetDecodedLabel("itemalreadyexists")
            Exit Sub
        End If

        Dim canModifyPublicQueries As Boolean = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.isAdmin OrElse Folder.GetRoot().HasAccess(ACL.ACL_Access.Access_Level.ACL_Manage_Public_Saved_Queries)


        If Not canModifyPublicQueries Then
            If CType(rdQryScope.SelectedValue, DMQuery.QueryScope) = DMQuery.QueryScope.PublicScope Then
                rdQryScope.SelectedValue = DMQuery.QueryScope.PrivateScope.ToString
            End If
        End If

        Dim isNew As Boolean = (_queryID = 0)
        Dim q As DMQuery
        If isNew Then
            q = DMQuery.NewQuery()
        Else
            'save
            q = CurrentSavedQuery

            If Not q.CanModify(canModifyPublicQueries) Then Return
        End If
        q.Name = txtQryName.Text
        q.Description = txtQryDesc.Text
        q.Prop_ID = 0
        q.Scope = CType(rdQryScope.SelectedValue, DMQuery.QueryScope)
        q.Query_Xml = GetSearchCriteria(True).Serialize
        q.UseBaseFolder = CType(Convert.ToInt32(optFolderToUse.SelectedValue), DMQuery.FolderToUse)
        q.SearchScreenID = SearchScreen
        q.ResultType = domasearch.ResultType
        If q.UseBaseFolder = DMQuery.FolderToUse.UsePredefined Then
            q.ParentID = PC.GetParentId(False)
        Else
            q.ParentID = 0
        End If
        q = q.Save()


        txtQryID.Value = q.ID.ToString()
        SetQueryName(q.Name)
        If q.Scope = DMQuery.QueryScope.ACLScope AndAlso isNew Then 'allow for acl to be set on a new query
            lnkQueryACL.Visible = True
            lnkQueryACL.OnClientClick = "javascript:DoQueryACL(" & q.ID & ");return false;"
            lnkQueryACL.Text = GetLabel("ctx_aclsetacl")
            JSCommand = "DoQueryACL(" & q.ID & ");"
        Else
            lnkQueryACL.Visible = False
            JSCommand = "EditQuery(" & q.ID & "," & q.SearchScreenID & "," & Convert.ToInt32(q.ResultType) & ")"
        End If

    End Sub
#End Region

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        'add init script
        If pnlFolderText.Visible Then
            Dim ajxMan As Telerik.Web.UI.RadAjaxManager = Telerik.Web.UI.RadAjaxManager.GetCurrent(Page)
            ajxMan.EnableAJAX = True
            ajxMan.AjaxSettings.AddAjaxSetting(foldertext, pnlFolderText)
        End If
        If grd.Mode <> DocroomListHelpers.GridMode.Print Then


            Dim sb As New StringBuilder

            sb.Append(" function RefreshParent(){if (parent.RefreshTreeContent){parent.RefreshTreeContent(PC().GetParentID());}}")
            sb.Append("function InitPage(){")
            sb.Append("if (PC().InitDone()){")

            sb.Append("if (parent){")
            If RefreshTree Then
                sb.Append("RefreshParent();")
            End If
            If (grd.Mode = DocroomListHelpers.GridMode.Normal) Then

                sb.Append("if (parent.SetCaption){")
                sb.Append("	parent.SetCaption(")
                sb.Append(EncodingUtils.EncodeJsString(HeaderText))
                sb.Append(");}")

                sb.Append("if (parent.SetFolder){")
                sb.Append("	parent.SetFolder(PC().GetParentID(),PC().GetPackageID());}")

                Dim menuFile As String
                If CurrentFolderIsActualFolder() AndAlso Not String.IsNullOrEmpty(CurrentFolder.Menu) AndAlso Not CurrentFolder.HasNoAccess() Then
                    menuFile = CurrentFolder.Menu
                Else
                    menuFile = ""
                End If
                sb.Append("if (parent.SetMenu){")
                sb.Append("	parent.SetMenu(")
                sb.Append(EncodingUtils.EncodeJsString(menuFile))
                sb.Append(");}")
            End If
            sb.Append("}")

            If Not String.IsNullOrEmpty(JSMessageError) Then
                sb.Append("PC().ShowError(")
                sb.Append(EncodingUtils.EncodeJsString(JSMessageError))
                sb.Append(");")
            End If
            If Not String.IsNullOrEmpty(JSMessageSuccess) Then
                sb.Append("PC().ShowSuccess(")
                sb.Append(EncodingUtils.EncodeJsString(JSMessageSuccess))
                sb.Append(");")
            End If
            If Not String.IsNullOrEmpty(JSCommand) Then
                sb.Append(JSCommand)
                sb.Append(";")
            End If
            sb.Append("} else {")
            sb.Append("setTimeout('InitPage()',100) ;}")
            sb.Append("}")

            sb.Append("setTimeout('InitPage()',100) ;")

            ScriptManager.RegisterStartupScript(Page, Page.GetType, "init" & ClientID, sb.ToString, True)
        End If
    End Sub

End Class



Namespace FolderTextToolbarButtons
    Public MustInherit Class BaseToolbarButton

        Public MustOverride Sub DataBind(ByVal voObject As DM_OBJECT)

        Public Overridable Sub AddToToolbar(ByVal toolbar As Telerik.Web.UI.RadToolBar)
            If Me.Visible Then
                'class or url
                Dim toolButton As Telerik.Web.UI.RadToolBarButton = New Telerik.Web.UI.RadToolBarButton
                toolButton.ToolTip = Me.Tooltip
                toolButton.Text = Me.Text
                SetToolbarButtonImage(toolButton, DirectCast(toolbar.Page, BasePage))
                'If Not String.IsNullOrEmpty(Me.ImageClass) Then
                '    toolButton.EnableImageSprite = True
                '    toolButton.SpriteCssClass = Me.ImageClass
                'Else
                '    toolButton.ImageUrl = GetImageUrl(toolbar.Page)
                'End If

                toolButton.Enabled = Me.Enabled

                toolButton.PostBack = False
                toolButton.NavigateUrl = Me.NavigateUrl

                toolbar.Items.Add(toolButton)
            End If
        End Sub

        Private Sub SetToolbarButtonImage(ByRef toolbarBtn As Telerik.Web.UI.RadToolBarButton, ByVal page As BasePage)
            If Not String.IsNullOrEmpty(ImageClass) Then
                toolbarBtn.EnableImageSprite = True
                toolbarBtn.SpriteCssClass = ImageClass
            Else
                toolbarBtn.ImageUrl = GetImageUrl(page)
            End If
        End Sub

        Private Function GetImageUrl(ByVal page As System.Web.UI.Page) As String
            'class or url
            If Not String.IsNullOrEmpty(ImageUrl) AndAlso Not ImageUrl.StartsWith("~") Then

                Return ThemedImage.GetUrl(Me.ImageUrl, page)
            Else
                Return ImageUrl
            End If
        End Function

        Public Overridable Sub AddToToolbar(ByVal drpDwn As Telerik.Web.UI.RadToolBarDropDown)
            If Me.Visible Then
                Dim button As New Telerik.Web.UI.RadToolBarButton
                button.ToolTip = Me.Tooltip
                button.Text = Me.Text
                SetToolbarButtonImage(button, drpDwn.Page)
                'button.ImageUrl = GetImageUrl(voDrpDwn.Page)
                button.Enabled = Me.Enabled

                button.PostBack = False
                button.NavigateUrl = Me.NavigateUrl

                drpDwn.Buttons.Add(button)
            End If
        End Sub

        Public Overridable Sub AppendClientScript(ByVal sb As StringBuilder, ByVal voPage As DM_DOCUMENTLIST)

        End Sub

        Public Property NavigateUrl() As String = ""

        Public Property Enabled() As Boolean = True

        Public Property Visible() As Boolean = True

        Public Property Text() As String = ""

        Public Property Tooltip() As String = ""

        Public Property ImageUrl() As String = ""

        Public Property ImageClass() As String = ""

    End Class

    Public Class UserEventButton
        Inherits BaseToolbarButton

        Private moEvent As UserEvent
        Private mActionID As Int32

        Public Overloads Overrides Sub DataBind(ByVal voObject As DM_OBJECT)
            Me.Visible = False
            If voObject.Object_Type = moEvent.Type OrElse moEvent.Type = "Object" Then
                If voObject.HasAccess(ACL.ACL_Access.Access_Level.ACL_Execute_User_Event, moEvent.ID) Then
                    If moEvent.Location = (moEvent.Location Or UserEvent.eLocation.DetailScreen) Then
                        Me.NavigateUrl = "javascript:PC().DoDocumentListActionDoc(10," & mActionID & "," & voObject.ID & ");"
                        Me.Visible = True
                    End If
                End If
            End If
        End Sub


        Public Sub New(ByVal action As ScreenActionList.ScreenActionInfo)

            Me.Tooltip = action.Caption
            mActionID = action.ACTION_ID
            moEvent = UserEvent.GetUserEvent(action.ACTION_ID)
            If Not String.IsNullOrEmpty(action.Icon) Then
                If moEvent.DisplayMode.HasFlag(ButtonDisplayMode.Icon) Then
                    Me.ImageUrl = "~/Images/" & action.Icon
                End If
            End If
            If moEvent.DisplayMode.HasFlag(ButtonDisplayMode.Text) Then
                Me.Text = moEvent.Caption
            End If
            Me.Tooltip = action.Tooltips.GetValue("")
        End Sub
    End Class

    Public Class CustomActionButton
        Inherits BaseToolbarButton
        Private moCustAction As DMCustomAction
        Private mActionID As Int32 = 0

        Public Overloads Overrides Sub DataBind(ByVal voObject As DM_OBJECT)
            Me.Visible = False
            If voObject.Object_Type = moCustAction.Type OrElse moCustAction.Type = "Object" Then
                If voObject.HasAccess(ACL.ACL_Access.Access_Level.ACL_Execute_Custom_Action, moCustAction.ID) Then
                    If moCustAction.Enabled(voObject, IUserEvent.eCallerLocation.DetailScreen) Then
                        Me.Visible = True
                        Me.NavigateUrl = "javascript:PC().DoDocumentListActionDoc(1," & mActionID & "," & voObject.ID & ");"
                    End If
                End If
            End If
        End Sub

        Public Sub New(ByVal voAction As ScreenActionList.ScreenActionInfo)
            Me.ImageUrl = "" '"~/Images/xp-ok.gif"
            Me.Text = voAction.Caption
            Me.Tooltip = voAction.Caption
            mActionID = voAction.ACTION_ID
            moCustAction = DMCustomAction.GetCustomAction(voAction.ACTION_ID)

        End Sub
    End Class

    Public Class SeparatorButton
        Inherits BaseToolbarButton

        Public Overrides Sub AddToToolbar(ByVal voToolbar As Telerik.Web.UI.RadToolBar)
            Dim loButton As Telerik.Web.UI.RadToolBarButton = New Telerik.Web.UI.RadToolBarButton
            loButton.IsSeparator = True
            voToolbar.Items.Add(loButton)
        End Sub
        Public Sub New()

        End Sub

        Public Overloads Overrides Sub DataBind(ByVal voObject As DM_OBJECT)

        End Sub

    End Class

    Public Class AddMessageButton
        Inherits BaseToolbarButton

        Public Overloads Overrides Sub DataBind(ByVal voObject As DM_OBJECT)
            Visible = voObject.CanAddComment
            NavigateUrl = "javascript:PC().AddMessage(" & voObject.ID & ");"
        End Sub

        Public Sub New()
            Tooltip = "addmessage"
            ImageClass = "icon icon-message-add"
        End Sub
    End Class

    Public Class AddCommentButton
        Inherits BaseToolbarButton

        Public Overloads Overrides Sub DataBind(ByVal voObject As DM_OBJECT)
            Visible = voObject.CanAddComment
            NavigateUrl = "javascript:PC().NewComment(" & voObject.ID & ");"
        End Sub

        Public Sub New()
            Tooltip = "addcomment"
            ImageClass = "icon icon-comments-add"
        End Sub
    End Class

    Public Class AddFileButton
        Inherits BaseToolbarButton
        Public Overloads Overrides Sub DataBind(ByVal voObject As DM_OBJECT)
            Me.Visible = voObject.CanAddFile
            Me.NavigateUrl = "javascript:PC().AddFile(" & voObject.ID & ");"
        End Sub

        Public Sub New()
            Me.Tooltip = "addfile" 'get label?
            ImageClass = "icon icon-file-add"
        End Sub
    End Class

    Public Class ACLButton
        Inherits BaseToolbarButton


        Public Overloads Overrides Sub DataBind(ByVal voObject As DM_OBJECT)
            Me.Visible = voObject.CanViewACL
            Me.NavigateUrl = "javascript:PC().ShowObjectACL(" & voObject.ID & ");"
        End Sub
        Public Sub New()
            Me.Tooltip = "aclsetacl"
            ImageClass = "icon icon-acl"
        End Sub
    End Class

    Public Class EditButton
        Inherits BaseToolbarButton

        Private mID As Int32 = 0
        Public Overrides Sub AppendClientScript(ByVal sb As System.Text.StringBuilder, ByVal voPage As DM_DOCUMENTLIST)
            sb.AppendLine("function EditCurrentFolder(){")
            sb.AppendLine("PC().OpenDetailWindow('dm_detail.aspx?DM_OBJECT_ID=" & mID & "&mode=2', " & mID & ");")
            sb.AppendLine("}")
        End Sub
        Public Sub New()
            ImageClass = "icon icon-edit"
            Me.Tooltip = "Edit"
            'Me.CommandName = "Edit"
            Me.NavigateUrl = "javascript:EditCurrentFolder();"
        End Sub

        Public Overloads Overrides Sub DataBind(ByVal voObject As DM_OBJECT)
            mID = voObject.ID
            Me.Visible = voObject.CanModifyMeta

        End Sub
    End Class

    Public Class NewButton
        Inherits BaseToolbarButton

        Public Sub New()
            ImageClass = "icon icon-document-new"
            Tooltip = "new"
            NavigateUrl = "javascript:NewObject();"
        End Sub

        Public Overloads Overrides Sub DataBind(ByVal voObject As DM_OBJECT)
            Visible = voObject.Type.GetAllowedChildObjectTypes.Any
            NavigateUrl = "javascript:PC().AddObject('', " & voObject.ID & ",0);"
        End Sub
    End Class

#Region " Factory "
    Public Class ButtonFactory
        Public Shared Function GetFixedToolbarButton(ByVal voAction As ScreenActionList.ScreenActionInfo) As BaseToolbarButton
            Select Case voAction.FixedActionID
                Case ScreenAction.FixedAction.AddMessage
                    Return New AddMessageButton
                Case ScreenAction.FixedAction.AddComment
                    Return New AddCommentButton
                Case ScreenAction.FixedAction.AddFile
                    Return New AddFileButton
                Case ScreenAction.FixedAction.ACL
                    Return New ACLButton
                Case ScreenAction.FixedAction.NewItem
                    Return New NewButton
                Case ScreenAction.FixedAction.Edit
                    Return New EditButton
                    'todo : create these buttons

                    'Case ScreenAction.FixedAction.BrowseContent
                    '    Return New BrowseContentButton

                    'Case ScreenAction.FixedAction.Comments
                    '    Return New ViewCommentsButton
                    'Case ScreenAction.FixedAction.CopyMoveToFolder
                    '    Return New CopyMoveToFolderButton              

                    'Case ScreenAction.FixedAction.EditWork
                    '    Return New EditWorkButton
                    'Case ScreenAction.FixedAction.Favorites
                    '    Return New FavoritesButton
                    'Case ScreenAction.FixedAction.History
                    '    Return New HistoryButton
                    'Case ScreenAction.FixedAction.LinkedObjects
                    '    Return New LinkedObjectsButton                                

                    'Case ScreenAction.FixedAction.VersionHistory
                    '    Return New VersionHistoryButton              
                    'Case ScreenAction.FixedAction.Basket
                    '    Return New BasketButton
                    'Case ScreenAction.FixedAction.OpenCasesForObject
                    '    Return New OpenCasesForObjectButton
                    'Case ScreenAction.FixedAction.MyWorkForObject
                    '    Return New WorkForObjectButton
                Case Else
                    Return Nothing
            End Select
        End Function
        Public Shared Function GetToolbarButton(ByVal voAction As ScreenActionList.ScreenActionInfo) As BaseToolbarButton
            Select Case voAction.Type
                Case ScreenAction.ActionType.Fixed
                    Return GetFixedToolbarButton(voAction)
                Case ScreenAction.ActionType.Separator
                    Return New SeparatorButton
                Case Else
                    Return Nothing
            End Select
        End Function
    End Class
#End Region
End Namespace

