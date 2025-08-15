Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Search
Imports Arco.Doma.Library.Security
Imports System.Xml
Imports Arco.Doma.WebControls.DocroomListHelpers
Imports Arco.Doma.WebControls.DocroomListHelpers.ContentProviders
Imports Arco.Doma.Library.Website
Imports Telerik.Web.UI
Imports System.IO
Imports Arco.Doma.WebControls.Formatters

Public Class DM_Listcontrol
    Inherits DocroomList

#Region " Properties and Variables "


    Private mbShowFolders As Boolean
    Private mbInSelectionMode As Boolean
    Protected HeaderContextMenu As RadContextMenu
    Private mbMenuLoaded As Boolean
    Protected msObjectList As New StringBuilder
    Protected moFileColumns As XmlNodeList
    Protected Results As DM_OBJECTSearch
    Protected ListRows As New StringBuilder
    Protected TermsList As String = ""
    Private _loadedXMLFile As String = ""
    Private msGridXMLFileProp As String
    Protected mbShowGrouper As Boolean
    Private mbForceDMObjectJoin As Boolean
    Private miForceJoinOnProperyStore As Int32
    Private _fileExpandShown As Boolean
    'Protected _fileOnMainRowShown As Boolean
    Private _showAttachmentsOnSecondRow As Boolean
    Private mlRootID As Int32
    Private _parentID As Int32
    Private _currentFolder As DM_OBJECT
    Private _showCalendar As Boolean
    Private mbShowBackToSearchLink As Boolean

    Private msPrintOptions As String = ""
    Private mbWhatsNewIncludeModifDate As Boolean = True
    Private _hiddenColumns As HashSet(Of String)
    Private _showSearchedTree As Boolean = True
    Private _showScroller As Boolean = True
    Private _showQueryList As Boolean = True
    Private mbAllowChangeInCriteria As Boolean = True
    Private mlLinkToCaseID As Int32
    Private mlLinkToObjectID As Int32
    Private mlLinkToPackID As Int32
    Private msQueryName As String = ""

    Private _autoOpenFirstHit As Boolean
    Private miFirstHit As Int32
    Private _hasMultiSelect As Boolean
    Private mcolMailFiles As Arco.Doma.Library.Mail.MailFileList

    Public Property AutoOpenFirstHit As Boolean
        Set(value As Boolean)
            _autoOpenFirstHit = value
        End Set
        Private Get
            Return _autoOpenFirstHit
        End Get
    End Property

    Public ReadOnly Property EnableFooTable As Boolean
        Get
            Return False AndAlso Not AjaxEnabled AndAlso NumberOfResults > 0
        End Get
    End Property

    Protected Overrides ReadOnly Property ToolbarControl As RadToolBar
        Get
            'Return DirectCast(FindControl("tlbGrid"), RadToolBar)
            Return tlbGrid
        End Get
    End Property

    Public WriteOnly Property QueryName() As String
        Set(ByVal value As String)
            msQueryName = value
        End Set
    End Property

    Public Property AllowChangeInCriteria() As Boolean
        Get
            Return TryGetFromViewState("ACIC", mbAllowChangeInCriteria)
        End Get
        Set(ByVal value As Boolean)
            mbAllowChangeInCriteria = value
            If EnableViewState Then
                ViewState("ACIC") = value
            End If
        End Set
    End Property

    Public ReadOnly Property ShowFileContextMenus() As Boolean
        Get

            Return (Layout Is Nothing OrElse Layout.ItemContextMenus) AndAlso Mode <> DocroomListHelpers.GridMode.Print AndAlso UserProfile.EnableContextMenus

        End Get
    End Property

    Public Property ShowQueryList() As Boolean
        Get
            Return TryGetFromViewState("SHQ", _showQueryList)
        End Get
        Set(ByVal value As Boolean)
            _showQueryList = value
            If EnableViewState Then
                ViewState("SHQ") = value
            End If
        End Set
    End Property

    Public Property ShowSearchedForTree() As Boolean
        Get
            Return TryGetFromViewState("SFT", _showSearchedTree)
        End Get
        Set(ByVal value As Boolean)
            _showSearchedTree = value
            If EnableViewState Then
                ViewState("SFT") = value
            End If
        End Set
    End Property

    Protected ReadOnly Property ResultType() As DM_OBJECTSearch.eResultType
        Get
            If GridVisible Then
                If Not Results Is Nothing Then
                    Return Results.ResultType
                Else
                    Return _resultListCrit.ResultType
                End If

            Else
                Return DM_OBJECTSearch.eResultType.Objects
            End If
        End Get
    End Property

    Private Function ColumnIsHidden(ByVal vsCol As String) As Boolean
        Return _hiddenColumns.Contains(vsCol)
    End Function


    Public Property WhatsNewIncludeModifDate() As Boolean
        Get
            Return mbWhatsNewIncludeModifDate
        End Get
        Set(ByVal value As Boolean)
            mbWhatsNewIncludeModifDate = value
        End Set
    End Property

    Public ReadOnly Property ShowFolderText() As Boolean
        Get
            If Not Layout Is Nothing Then
                Return Layout.ShowFolderText
            Else
                Return False
            End If
        End Get
    End Property

    Public ReadOnly Property KeepSearchScreenVisible() As Boolean
        Get
            If Not Layout Is Nothing Then
                Return Layout.KeepSearchScreenVisible
            Else
                Return False
            End If
        End Get
    End Property

    Public Property Selection() As String
        Get
            Return Sel.Value
        End Get
        Set(ByVal value As String)
            Sel.Value = value
        End Set
    End Property

    Protected ReadOnly Property PreviousVersions() As Boolean
        Get
            Return (_resultListCrit.ResultType = DM_OBJECTSearch.eResultType.ArchivedObjectVersions OrElse _resultListCrit.ResultType = DM_OBJECTSearch.eResultType.ObjectsAndArchivedObjectVersions)
        End Get
    End Property

    Public Property ResultScreenID() As Int32
        Get
            Dim s As String = ResultScreen.Value
            If Not String.IsNullOrEmpty(s) Then
                Dim i As Int32
                If Int32.TryParse(s, i) Then
                    Return i
                Else
                    Return 0
                End If
            Else
                Return 0
            End If
            'Dim o As Object
            'o = ViewState("ResultScreenID")
            'If o Is Nothing Then
            '    Return 0
            'Else
            '    Dim i As Int32 = CLng(o)
            '    If i >= 0 Then
            '        Return CInt(o)
            '    Else
            '        Return 0
            '    End If
            'End If
        End Get
        Set(ByVal value As Int32)
            ResultScreen.Value = value.ToString()
            'ViewState("ResultScreenID") = value
        End Set
    End Property

    Public Property PrintOptions() As String
        Get
            Return msPrintOptions
        End Get
        Set(ByVal value As String)
            msPrintOptions = value
        End Set
    End Property

    Private _showPager As Boolean = True

    Public Property ShowPager() As Boolean
        Get
            Return _showPager
        End Get
        Set(value As Boolean)
            _showPager = value
        End Set
    End Property

    Public Property ShowScroller() As Boolean
        Get
            Return _showScroller AndAlso Layout.ShowScroller
        End Get
        Set(ByVal value As Boolean)
            _showScroller = value
        End Set
    End Property

    Public Property ShowBackToSearchLink() As Boolean
        Get
            Return TryGetFromViewState("BTS", mbShowBackToSearchLink)
        End Get
        Set(ByVal value As Boolean)
            mbShowBackToSearchLink = value
            If EnableViewState Then
                ViewState("BTS") = value
            End If
        End Set
    End Property

    Public Sub SetGridFile(ByVal vsGridFile As String)
        SetGridFiles(vsGridFile, "")
    End Sub

    Public Sub SetObjectGridFile(ByVal vsGridFile As String)
        SetGridFiles(vsGridFile, "DefaultResultgrid.xml")
    End Sub

    Public Sub SetCaseGridFile(ByVal vsGridFile As String)
        SetGridFiles(vsGridFile, "DefaultCasegrid.xml")
    End Sub

    Public Sub SetMailGridFile(ByVal vsGridFile As String)
        SetGridFiles(vsGridFile, "DefaultMailGrid.xml")
    End Sub

    Private Sub SetGridFiles(ByVal vsGridFile As String, ByVal vsDefaultGridFile As String)
        DefaultGridXmlFile = vsDefaultGridFile
        GridXMLFile = vsGridFile
    End Sub

    Public Property DefaultGridXmlFile As String = "DefaultResultgrid.xml"

    Public Property GridXMLFile() As String
        Set(ByVal value As String)
            msGridXMLFileProp = value
            Dim reInit As Boolean = False
            If EnableViewState Then
                reInit = ViewState("GXF") <> msGridXMLFileProp
                ViewState("GXF") = msGridXMLFileProp
            End If
            If Not String.IsNullOrEmpty(msGridXMLFileProp) Then
                _groupinginfo = Nothing
                _filterInfo = Nothing
                _toolbar = Nothing

                LoadGrid(value, reInit)
            End If
        End Set
        Get
            Return TryGetFromViewState("GXF", msGridXMLFileProp)
        End Get
    End Property

    Public Function IsGroupedOn(ByVal grouperid As Int32) As Boolean
        Return (GroupBy.Value.IndexOf(grouperid & ";;", StringComparison.Ordinal) >= 0)
    End Function

    Private _filterInfo As GridFilters

    Public ReadOnly Property GridFilters() As GridFilters
        Get
            If _filterInfo Is Nothing Then
                _filterInfo = GridFilters.LoadFilters(Layout.FilterFile)
            End If
            Return _filterInfo
        End Get
    End Property

    Private _groupinginfo As GridGroupingInfo

    Public ReadOnly Property GridGrouping() As GridGroupingInfo
        Get
            If _groupinginfo Is Nothing Then

                '  If moGridLayout.GroupingFile <> "AUTO" Then
                If Layout IsNot Nothing Then
                    _groupinginfo = GridGroupingInfo.LoadGroupingInfo(Layout.GroupingFile)
                End If
                ' moGroupingInfo.AddGroupingFromGrid(moGridLayout, ContentParams)
                'Else
                '    moGroupingInfo = GridGroupingInfo.LoadGroupingFromGrid(moGridLayout, ContentParams)
                'End If

            End If
            Return _groupinginfo
        End Get
    End Property

    Private _toolbar As GridToolbar

    Private ReadOnly Property Toolbar() As GridToolbar
        Get
            If _toolbar Is Nothing Then
                _toolbar = GridToolbar.LoadGridToolbar(Layout.ToolbarFile)
            End If
            Return _toolbar
        End Get
    End Property

    Private _customToolbarButtons As New List(Of GridToolbar.ToolbarItem)

    Public Property CustomToolbarButtons As List(Of GridToolbar.ToolbarItem)
        Set(ByVal value As List(Of GridToolbar.ToolbarItem))
            _customToolbarButtons = value
            If EnableViewState Then
                ViewState("CBT") = _customToolbarButtons
            End If
        End Set
        Get
            Return TryGetFromViewState("CBT", _customToolbarButtons)
        End Get
    End Property

    Public ReadOnly Property GroupingEnabled() As Boolean
        Get
            If Not GridGrouping Is Nothing Then
                Return GridGrouping.GroupingEnabled
            Else
                Return False
            End If
        End Get
    End Property

    Public Property ShowCalendar() As Boolean
        Get
            Return _showCalendar OrElse ShowWhatsNew.Value = "Y"
        End Get
        Set(ByVal value As Boolean)
            _showCalendar = value
        End Set
    End Property

    Public Property RootID() As Int32
        Get
            Return mlRootID
        End Get
        Set(ByVal value As Int32)
            mlRootID = value
        End Set
    End Property

    Public Property CurrentFolder As DM_OBJECT
        Get
            If _currentFolder Is Nothing Then
                _currentFolder = ObjectRepository.GetObject(_parentID)
            End If
            Return _currentFolder
        End Get
        Set(value As DM_OBJECT)
            _currentFolder = value
        End Set
    End Property

    'Public Property ParentID() As Int32
    '    Get
    '        Return _parentID
    '    End Get
    '    Set(ByVal value As Int32)
    '        mlParentID = value
    '    End Set
    'End Property

    Public Property ShowSearchedForCategory As Boolean = True

    Public Property ShowFolders() As Boolean
        Get
            Return TryGetFromViewState("SFLD", mbShowFolders)
        End Get
        Set(ByVal value As Boolean)
            mbShowFolders = value
            If EnableViewState Then
                ViewState("SFLD") = value
            End If
        End Set
    End Property

    Public Property InSelectionMode As Boolean
        Get
            Return TryGetFromViewState("SELM", mbInSelectionMode)
        End Get
        Set(ByVal value As Boolean)
            mbInSelectionMode = value
            If EnableViewState Then
                ViewState("SELM") = value
            End If
        End Set
    End Property

    Private _showQuery As Boolean

    Public Property ShowQuery() As Boolean
        Get
            Return TryGetFromViewState("SQR", _showQuery)
        End Get
        Set(ByVal value As Boolean)
            _showQuery = value
            If EnableViewState Then
                ViewState("SQR") = value
            End If
        End Set
    End Property

    Public Property CurrentPage() As Int32
        Get
            Dim p As String = currpage.Value
            Dim result As Integer
            If Not String.IsNullOrEmpty(p) AndAlso Integer.TryParse(p, result) Then
                Return result
            End If
            currpage.Value = "1"
            Return 1
        End Get
        Set(ByVal value As Int32)
            currpage.Value = value.ToString()
        End Set
    End Property

    Public Property LastPage() As Int32
        Get
            Dim o As Object
            o = ViewState("_LP")
            If o Is Nothing Then
                Return 0
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal value As Int32)
            ViewState("_LP") = value
        End Set
    End Property

    Public ReadOnly Property RecordsOnThisPage() As Int32
        Get
            If CurrentPage < LastPage Then
                Return RecordsPerPage
            Else
                Return NumberOfResults - ((LastPage - 1) * RecordsPerPage)
            End If
        End Get
    End Property

    Public Property RecordsPerPage() As Int32
        Get
            Dim o As Object
            o = ViewState("_RP")
            If o Is Nothing Then
                Return _defaultresultsPerPage
            Else
                If CInt(o) = 0 Then
                    Return ArcoInfoSettings.DefaultRecordsPerPage
                Else
                    Return CInt(o)
                End If

            End If
        End Get
        Set(ByVal value As Int32)
            ViewState("_RP") = value
        End Set
    End Property

    Public Property NumberOfResults() As Int32
        Get
            Dim o As Object
            o = ViewState("_NRES")
            If o Is Nothing Then
                Return _defaultMaxResults
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal value As Int32)
            ViewState("_NRES") = value
        End Set
    End Property

    Private _contentParams As GridParams

    Public ReadOnly Property ContentParams() As GridParams
        Get
            If _contentParams Is Nothing Then
                _contentParams = New GridParams
                _contentParams.Settings = Settings
                _contentParams.ResultType = ResultType
                _contentParams.GridRootID = RootID
                _contentParams.UserProfile = UserProfile
                _contentParams.GridClientID = JavascriptVariableName
                _contentParams.Results = Results
                _contentParams.ShowFileContextMenus = ShowFileContextMenus

                _contentParams.ExpandFileRows = ExpandFileRows

                _contentParams.GridSelection = Selection

            End If

            Return _contentParams
        End Get
    End Property

#End Region

    Protected GridVisible As Boolean

    Protected Sub imageList_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles imageList.ItemDataBound
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim obj As DM_OBJECTSearch.OBJECTInfo = CType(e.Item.DataItem, DM_OBJECTSearch.OBJECTInfo)
            Dim refobj As DM_OBJECTSearch.OBJECTInfo = Nothing
            If Not IsPlaceHolderObject(obj) Then 'don't show the placeholder
                ProcessDefaultImageListItem(e.Item, obj, refobj)
                AddToObjectLists(obj)
            Else
                'no results found
                Dim objPlaceHolder As PlaceHolder = CType(e.Item.FindControl("plhMainImageListCell"), PlaceHolder)
                objPlaceHolder.Controls.Add(New LiteralControl("<center>" & GetLabel("noresultsfound") & "</center>"))
            End If
        ElseIf e.Item.ItemType = ListItemType.Footer Then
        Else
            ProcessDefaultImageListHeader(e.Item)
        End If
    End Sub

    Private Function IsPlaceHolderObject(ByVal o As DM_OBJECTSearch.OBJECTInfo) As Boolean
        Return (o.ID = 0 AndAlso o.CaseID = 0 AndAlso o.Mail_Id = 0)
    End Function

    Private Sub AddToObjectLists(ByVal obj As DM_OBJECTSearch.OBJECTInfo)
        Const maxlistlen = 1000

        If ResultType = DM_OBJECTSearch.eResultType.CaseList Then
            If obj.TechID > 0 Then
                If msObjectList.Length < maxlistlen Then
                    If msObjectList.Length > 0 Then
                        msObjectList.Append(";")
                    End If
                    msObjectList.Append(obj.TechID)
                End If
                If miFirstHit = 0 Then
                    miFirstHit = obj.TechID
                End If
            End If
        ElseIf ResultType = DM_OBJECTSearch.eResultType.ArchivedCasesList OrElse ResultType = DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList Then
            If obj.CaseID > 0 Then
                If msObjectList.Length < maxlistlen Then
                    If msObjectList.Length > 0 Then
                        msObjectList.Append(";")
                    End If
                    msObjectList.Append(obj.CaseID)
                End If
                If miFirstHit = 0 Then
                    miFirstHit = obj.CaseID
                End If

            End If
        ElseIf ResultType = DM_OBJECTSearch.eResultType.Mails Then
            If obj.Mail_Id > 0 Then
                If msObjectList.Length < maxlistlen Then
                    If msObjectList.Length > 0 Then
                        msObjectList.Append(";")
                    End If
                    msObjectList.Append(obj.Mail_Id)
                End If
                If miFirstHit = 0 Then
                    miFirstHit = obj.Mail_Id
                End If

            End If
        Else
            If msObjectList.Length < maxlistlen Then
                If msObjectList.Length > 0 Then
                    msObjectList.Append(";")
                End If
                msObjectList.Append(obj.ID)
            End If
            If miFirstHit = 0 Then
                miFirstHit = obj.ID
            End If
        End If
    End Sub

    Private Function ShowFilesOnSecondLine(ByVal obj As DM_OBJECTSearch.OBJECTInfo) As Boolean
        If _showAttachmentsOnSecondRow Then
            'show when there are attachments
            If obj.ID = ShowPreviousFileVersionsFor OrElse obj.OBJECT_TYPE = "Mail" OrElse ResultType = DM_OBJECTSearch.eResultType.Mails Then
                Return True
            Else
                If obj.FileCount > 1 Then
                    Return True
                Else
                    If obj.FileCount = 1 Then
                        If obj.GetMainFile().ReferenceID > 0 Then
                            Return True
                        Else
                            Return False
                        End If
                    Else
                        Return False
                    End If
                End If
            End If
        Else
            Return (ExpandFileRows OrElse _fileExpandShown OrElse PreviousVersions)
        End If
    End Function

    Protected Sub doclist_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles docList.ItemDataBound
        Dim bScreenFound As Boolean = False
        Dim objPlaceHolder As PlaceHolder = CType(e.Item.FindControl("plhMainRow"), PlaceHolder)
        If e.Item.ItemType = ListItemType.Item OrElse e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim obj As DM_OBJECTSearch.OBJECTInfo = CType(e.Item.DataItem, DM_OBJECTSearch.OBJECTInfo)
            Dim refobj As DM_OBJECTSearch.OBJECTInfo = Nothing
            If Not IsPlaceHolderObject(obj) Then
                If _customResultScreen IsNot Nothing Then
                    If _customResultScreen.Type = Screen.ScreenSourceType.TemplateFile AndAlso Not String.IsNullOrEmpty(_customResultScreen.Source) Then
                        bScreenFound = True
                        Dim source = Arco.Settings.FrameWorkSettings.ReplaceGlobalVars(_customResultScreen.Source)
                        Dim objtemplate As XMLTemplate = CType(Arco.Serialization.ObjectXMLSerializer.LoadObjectFromXMLFile(source, GetType(XMLTemplate)), XMLTemplate)
                        For Each objItem As XMLTemplate.TemplateItem In objtemplate.Templates
                            ProcessTemplateItem(obj, objItem, objPlaceHolder, e.Item.ItemType)
                        Next
                    End If
                End If
                If Not bScreenFound Then
                    ProcessDefaultItem(e.Item, obj, refobj)
                End If

                AddToObjectLists(obj)

                Dim actualobj As DM_OBJECTSearch.OBJECTInfo = obj
                If Not refobj Is Nothing Then actualobj = refobj

                Dim bCanViewFiles As Boolean
                If Not actualobj.FromArchive Then
                    bCanViewFiles = actualobj.HasAccess(ACL.ACL_Access.Access_Level.ACL_ViewFiles)
                Else
                    Dim oActiveversion As Document = Document.GetLatestVersion(actualobj.DIN)
                    bCanViewFiles = oActiveversion.CanViewFiles
                End If

                If bCanViewFiles AndAlso Not Results.ResultType = DM_OBJECTSearch.eResultType.Files AndAlso ShowFilesOnSecondLine(actualobj) Then
                    Dim c As Control = e.Item.FindControl("filelistrow")
                    If Not c Is Nothing Then
                        Dim fForm As DMFileForm = DirectCast(e.Item.FindControl("fileList"), DMFileForm)
                        fForm.AttachmentsOnly = False
                        fForm.ShowContextMenu = ShowFileContextMenus
                        fForm.PackID = -1
                        fForm.MailFileListCache = mcolMailFiles

                        If Results.ResultType <> DM_OBJECTSearch.eResultType.Mails Then
                            fForm.FromMail = False
                            fForm.DataBind(Of DM_OBJECTSearch.OBJECTInfo.Object_File_Info)(actualobj.Files)
                        Else
                            Dim lcolMailAtts As List(Of Mail.MailFileList.FileInfo) = mcolMailFiles.Cast(Of Mail.MailFileList.FileInfo).Where(Function(x) x.MAIL_ID = actualobj.Mail_Id).ToList
                            fForm.FromMail = True
                            fForm.DataBind(Of Arco.Doma.Library.Mail.MailFileList.FileInfo)(lcolMailAtts)
                        End If



                        If fForm.Count > 0 Then
                            c.Visible = True
                            'this row is to even out the css alternator
                            e.Item.FindControl("filelistrow2").Visible = True

                            Dim c2 As Control = e.Item.FindControl("filelistspacer")
                            Dim liSpacerColCount As Int32 = 0
                            If Layout.FileExpanderOffSet > 0 Then
                                c2.Visible = True

                                liSpacerColCount = Layout.FileExpanderOffSet
                                If Not c2 Is Nothing Then
                                    Dim c3 As HtmlTableCell = DirectCast(c2, HtmlTableCell)
                                    c3.ColSpan = liSpacerColCount
                                End If
                            Else
                                c2.Visible = False
                            End If
                            c2 = e.Item.FindControl("filelistcol")
                            If Not c2 Is Nothing Then
                                Dim c3 As HtmlTableCell = DirectCast(c2, HtmlTableCell)
                                c3.ColSpan = Columns.Count - liSpacerColCount
                            End If

                            DirectCast(e.Item.FindControl("filelistspacer2"), HtmlTableCell).ColSpan = Columns.Count

                            If _fileExpandShown AndAlso Not ExpandFileRows AndAlso Not PreviousVersions AndAlso obj.ID.ToString <> showattachmentsfor.Value AndAlso obj.ID <> ShowPreviousFileVersionsFor Then
                                'If Not (obj.ID.ToString = showattachmentsfor.Value OrElse obj.ID = ShowPreviousFileVersionsFor OrElse ExpandFileRows OrElse PreviousVersions OrElse Not mbFileExpandShown) Then

                                DirectCast(c, HtmlTableRow).Attributes.Add("style", "display:none")
                            End If
                        Else
                            c.Visible = False
                            e.Item.FindControl("filelistrow2").Visible = False
                        End If

                    End If
                End If
            Else
                objPlaceHolder.Controls.Add(New LiteralControl("<tr class='ListFooter footable-disabled'><td data-ignore='true' colspan='" & Columns.Count & "' align='center'>" & GetLabel("noresultsfound") & "</td></tr>"))
            End If

        ElseIf e.Item.ItemType = ListItemType.Header Then
            bScreenFound = False
            Dim plhHeader As PlaceHolder = DirectCast(e.Item.FindControl("plhHeader"), PlaceHolder)
            If _customResultScreen IsNot Nothing Then
                If _customResultScreen.Type = Screen.ScreenSourceType.TemplateFile AndAlso Not String.IsNullOrEmpty(_customResultScreen.Source) Then
                    bScreenFound = True
                    Dim source = Arco.Settings.FrameWorkSettings.ReplaceGlobalVars(_customResultScreen.Source)
                    Dim objtemplate As XMLTemplate = CType(Arco.Serialization.ObjectXMLSerializer.LoadObjectFromXMLFile(source, GetType(XMLTemplate)), XMLTemplate)
                    For Each objItem As XMLTemplate.TemplateItem In objtemplate.Templates
                        ProcessTemplateHeader(objItem, plhHeader)
                    Next
                End If

            End If

            If Not bScreenFound Then
                'add default grid here
                ProcessDefaultHeader(e.Item)
            End If
        End If

    End Sub

    Public Sub LoadGroupersToGrid(ByVal crit As DM_OBJECTSearch.Criteria)
        If Not crit.Groupers Is Nothing Then
            Dim groupingFound As Boolean = False
            GroupByFilter.Value = crit.GroupResultsFor

            For Each grouper As DM_OBJECTSearch.Criteria.Grouper In crit.Groupers
                If grouper Is Nothing Then Continue For

                If Not grouper Is Nothing Then
                    If Not groupingFound Then 'only override the screen grouping if the criteria contains it's own grouping!!
                        GroupBy.Value = ""
                        groupingFound = True
                    End If
                    If Not grouper.OrderBy.HasValue Then
                        GroupBy.Value = GroupBy.Value & grouper.GrouperID & ";;" & grouper.Field & "##"
                    Else
                        GroupBy.Value = GroupBy.Value & grouper.GrouperID & ";;" & grouper.Field & ";;" & grouper.OrderBy.Value & "##"
                    End If

                End If
            Next
        End If
    End Sub

    Private Class SearchedForTable
        Private _tree As obout_ASPTreeView_2_NET.Tree

        Private _html As StringBuilder
        Private ReadOnly _ctrl As BaseUserControl

        Public Property ShowCategory As Boolean
        Public Property Width As String = ""
        Public Property ItemsFound() As Boolean

        Public Property ShowBackToSearchLink() As Boolean

        Public Property AsTree() As Boolean

        Public Property QueryName() As String

        Public Sub New(ByRef loCtrl As BaseUserControl, ByVal showcat As Boolean)
            _ctrl = loCtrl
            ShowCategory = showcat
        End Sub

        Private Function GetBoldLabel(ByVal vsLabel As String) As String
            Return "<b>" & GetLabel(vsLabel) & "</b>"
        End Function

        Private Function GetLabel(ByVal vsLabel As String) As String
            Return _ctrl.GetLabel(vsLabel)
        End Function

        Private _count As Int32

        Private Sub AddItem(ByVal labelKey As String, ByVal value As Int32, ByVal asRoot As Boolean)
            If value <> 0 Then
                Try
                    AddItem(GetBoldLabel(labelKey) & " : " & value, asRoot)
                Catch ex As Exception
                    AddItem(labelKey & " : " & value, asRoot)
                End Try
            End If
        End Sub

        Private Sub AddItem(ByVal labelKey As String, ByVal value As String, ByVal asRoot As Boolean, ByVal page As BasePage)
            AddItem(labelKey, value, asRoot, True, page)
        End Sub

        Private Sub AddItem(ByVal labelKey As String, ByVal value As String, ByVal asRoot As Boolean, ByVal htmlEncode As Boolean, ByVal page As BasePage)
            If Not String.IsNullOrEmpty(value) Then
                If htmlEncode Then
                    value = page.Server.HtmlEncode(value)
                End If
                AddItem(GetBoldLabel(labelKey) & " : " & value, asRoot)
            End If

        End Sub

        Private Sub AddItem(ByVal vsItem As String, ByVal vbAsRoot As Boolean)
            If Not vbAsRoot Then
                ItemsFound = True
                If AsTree Then
                    _tree.Add("src0", "src" & _count, vsItem, False, "root.svg")
                    _count += 1
                Else
                    _html.Append(vsItem)
                    _html.Append("<br/>")
                End If
            Else
                If AsTree Then
                    _tree.Add("root", "src0", vsItem, False, "root.svg")
                    _count = 1
                Else
                    _html.Append(vsItem)
                End If
            End If
        End Sub

        Private Sub AddRootNode()
            Dim lsLabel As String = GetLabel("searchedfor")
            If Not String.IsNullOrEmpty(QueryName) Then
                lsLabel = QueryName
            End If
            If ShowBackToSearchLink Then
                lsLabel = "<a href='javascript:BackToSearch();' class='ButtonLink'>" & _ctrl.Page.Server.HtmlEncode(lsLabel) & "</a>"
            End If
            If Not AsTree Then
                _html.Append("<div class='GroupingPanel' style='width:" & Width & "'><div class='Header'>" & lsLabel & "</div><div class='Content'>")
            Else
                AddItem("<b>" & lsLabel & "</b>", True)
            End If
        End Sub

        Private Sub Init()
            ItemsFound = False

            If AsTree Then
                _tree = New obout_ASPTreeView_2_NET.Tree
                If System.IO.Directory.Exists(_ctrl.Page.Server.MapPath(String.Concat("~/App_Themes/", _ctrl.Page.Theme, "/Tree/Icons"))) Then
                    _tree.FolderIcons = String.Concat("./App_Themes/", _ctrl.Page.Theme, "/Tree/Icons")
                Else
                    _tree.FolderIcons = "./TreeIcons/Icons"
                End If
                _tree.FolderStyle = "./TreeIcons/Styles/Win2003"
                _tree.FolderScript = "./TreeIcons/Script"
            Else
                _html = New StringBuilder(128)

            End If
        End Sub

        Private Sub CloseRootNode()
            If Not AsTree Then
                _html.Append("</div></div>")
            End If
        End Sub

        Public Function GetTree() As Control
            If ItemsFound Then
                If AsTree Then
                    Return New LiteralControl(_tree.HTML)
                Else
                    Return New LiteralControl(_html.ToString)
                End If
            Else
                Return New LiteralControl("")
            End If
        End Function

        Public Sub BuildTree(ByVal crit As DM_OBJECTSearch.Criteria)
            BuildTree(crit, False)
        End Sub

        Private Sub BuildTree(ByVal crit As DM_OBJECTSearch.Criteria, ByVal vbIsParentSearch As Boolean)
            If Not vbIsParentSearch Then
                Init()
                AddRootNode()
            Else
                AddItem("Within Parent Folders :", False)
            End If

            Dim page As BasePage = DirectCast(_ctrl.Page, BasePage)
            Dim language As String = BusinessIdentity.CurrentIdentity.Language
            If ShowCategory AndAlso crit.DM_OBJECT_CAT_ID > 0 Then
                Dim loCat As OBJECT_CATEGORY = OBJECT_CATEGORY.GetOBJECT_CATEGORY(crit.DM_OBJECT_CAT_ID)
                AddItem("category", loCat.TranslatedName(language, Globalisation.LABELList.GetCategoriesLabelList(page.EnableIISCaching)), False, page)
            End If
            If Not String.IsNullOrEmpty(crit.CategoryList) Then
                Dim laCats() As String = crit.CategoryList.Split(","c)
                For Each lsCat As String In laCats
                    Dim loCat As OBJECT_CATEGORY = OBJECT_CATEGORY.GetOBJECT_CATEGORY(Convert.ToInt32(lsCat))
                    AddItem("category", loCat.TranslatedName(language, Globalisation.LABELList.GetCategoriesLabelList(page.EnableIISCaching)), False, page)
                Next
            End If
            If crit.CaseSearch.ProcID > 0 Then
                Dim loProc As Routing.Procedure = Routing.Procedure.GetProcedure(crit.CaseSearch.ProcID)
                AddItem("procedure", Globalisation.LABELList.GetProceduresLabelList(page.EnableIISCaching).GetObjectLabel(loProc.ID, "Procedure", language, loProc.Name), False, page)
            End If
            If Not String.IsNullOrEmpty(crit.CaseSearch.ProcedureList) Then
                Dim laProcs() As String = crit.CaseSearch.ProcedureList.Split(","c)
                For Each lsProc As String In laProcs
                    Dim loProc As Routing.Procedure = Routing.Procedure.GetProcedure(Convert.ToInt32(lsProc))
                    AddItem("procedure", Globalisation.LABELList.GetProceduresLabelList(page.EnableIISCaching).GetObjectLabel(loProc.ID, "Procedure", language, loProc.Name), False, page)
                Next
            End If
            If crit.CaseSearch.StepID > 0 Then
                Dim lostep As Routing.cStep = Routing.cStep.GetStep(crit.CaseSearch.StepID)
                AddItem("step", Globalisation.LABELList.GetProcedureItemsLabelList(lostep.Proc_ID, page.EnableIISCaching).GetObjectLabel(lostep.ID, "Step", language, lostep.Name), False, page)
            End If
            AddItem("step", crit.CaseSearch.StepName, False, page)
            AddItem("globalsearch", crit.GlobalSearchTerm, False, page)
            AddItem("doctitle", crit.DM_OBJECT_NAME, False, page)

            AddItem("fulltext", crit.FileSearch.FullText, False, page)
            AddItem("extension", crit.FileSearch.Extension, False, page)
            AddItem("language", crit.FileSearch.Language, False, page)
            AddItem("comments", crit.FileSearch.Comments, False, page)
            AddItem("messages", crit.FileSearch.Messages, False, page)
            AddItem("createdby", crit.CreatedByFilter, False, page)
            AddItem("modifby", crit.ModifiedByFilter, False, page)
            AddItem("modifby", ArcoFormatting.FormatUserName(crit.ModifiedBy), False, False, page)

            AddItem("modifdate", crit.ModifiedDate, False, page)
            AddItem("stepdate", crit.CaseSearch.StepStartDate, False, page)
            AddItem("assignedto", crit.CaseSearch.AssignedTo, False, page)

            If Not String.IsNullOrEmpty(crit.CreatedBy) Then
                If crit.ResultType = DM_OBJECTSearch.eResultType.CaseList AndAlso crit.CreatedBy = Security.BusinessIdentity.CurrentIdentity.Name Then
                    'my dossiers, don't show
                Else
                    AddItem("createdby", ArcoFormatting.FormatUserName(crit.CreatedBy), False, page)
                End If
            End If

            AddItem("creationdate", crit.CreationDate, False, page)
            AddItem("File ID ", crit.FileSearch.ID, False)
            AddItem("FIN", crit.FileSearch.FIN, False)
            AddItem("whatsnew", crit.WhatsNewDate, False, page)

            If crit.Status <> DM_OBJECT.Object_Status.Undefined AndAlso crit.Status <> DM_OBJECT.Object_Status.Deleted Then
                AddItem("Status", Arco.EnumTranslator.GetEnumLabel(crit.Status), False, False, page)
            End If
            If crit.CaseSearch.Locked Then AddItem("Status", GetLabel("locked"), False, False, page)

            AddItem("listinfo1", crit.Listinfo1, False, page)
            AddItem("listinfo2", crit.Listinfo2, False, page)
            AddItem("listinfo1", crit.Listinfo3, False, page)

            AddItem("mail_subject", crit.MailSearch.MailSubject, False, page)
            AddItem("mail_to", crit.MailSearch.MailTo, False, page)
            AddItem("mail_from", crit.MailSearch.MailFrom, False, page)

            '  AddItem("Time : " & crit.EllapsedTime, False)
            Dim liFieldCounter As Int32

            For liFieldCounter = crit.DM_FIELD_SEARCHES.GetLowerBound(0) To crit.DM_FIELD_SEARCHES.GetUpperBound(0)
                If Not crit.DM_FIELD_SEARCHES(liFieldCounter) Is Nothing Then
                    If crit.DM_FIELD_SEARCHES(liFieldCounter).PROP_ID > 0 AndAlso Not String.IsNullOrEmpty(crit.DM_FIELD_SEARCHES(liFieldCounter).PROP_VALUE) Then
                        Dim loProp As DM_PROPERTY = DM_PROPERTY.GetPROPERTY(crit.DM_FIELD_SEARCHES(liFieldCounter).PROP_ID)

                        Dim lsLabel As String = loProp.TranslatedName
                        Dim lsValue As String = crit.DM_FIELD_SEARCHES(liFieldCounter).PROP_VALUE
                        If crit.DM_FIELD_SEARCHES(liFieldCounter).ForceExact Then
                            If loProp.IsList Then

                                Dim laSplit() As String = lsValue.Split(","c)
                                Dim lsTransCombined As String = ""
                                For Each lsSplit In laSplit.Where(Function(x) Not String.IsNullOrEmpty(x))

                                    Dim lsTrans As String
                                    Select Case loProp.Type
                                        Case PropertyTypes.TypeCodes.MNGDLIST
                                            lsTrans = Lists.ListItem.GetListItem(Convert.ToInt32(lsSplit)).Caption
                                        Case Else
                                            lsTrans = lsSplit

                                            If loProp.PropTypeHandler.isList AndAlso loProp.PropTypeHandler.CanMatchValueWithoutObjectContext(loProp.Definition) Then
                                                Dim filters As MultiPartFilter = loProp.GetListItemFilters()

                                                Try
                                                    If filters.TrySetValueFilter(lsSplit) Then
                                                        Dim li As List(Of PropertyListItem) = loProp.GetListItems(ListRangeRequest.AllItems, filters, False, 7)
                                                        For Each i As PropertyListItem In li
                                                            If i.Value.Equals(lsSplit, StringComparison.OrdinalIgnoreCase) Then
                                                                lsTrans = Arco.Web.ResourceManager.ReplaceLabeltags(i.Caption.ToString)
                                                                Exit For
                                                            End If
                                                        Next
                                                    End If
                                                Catch ex As Exception
                                                End Try
                                            End If
                                    End Select


                                    If String.IsNullOrEmpty(lsTransCombined) Then
                                        lsTransCombined = lsTrans
                                    Else
                                        If crit.DM_FIELD_SEARCHES(liFieldCounter).UseAnd Then
                                            lsTransCombined &= " " & GetLabel("and") & " " & lsTrans
                                        Else
                                            lsTransCombined &= " " & GetLabel("or") & " " & lsTrans
                                        End If
                                    End If

                                Next
                                lsValue = lsTransCombined

                            End If
                        Else
                            Select Case loProp.Type
                                Case PropertyTypes.TypeCodes.MNGDLIST
                                    If lsValue.StartsWith("=" & Chr(34)) AndAlso lsValue.EndsWith(Chr(34)) Then 'trim ="value"
                                        lsValue = lsValue.Substring(2, lsValue.Length - 3)
                                    End If
                            End Select
                        End If

                        AddItem("<b>" & lsLabel & " :</b> " & page.Server.HtmlEncode(lsValue), False)
                    End If
                End If
            Next


            Dim liPackCounter As Int32

            For liPackCounter = crit.DM_PACKAGE_SEARCHES.GetLowerBound(0) To crit.DM_PACKAGE_SEARCHES.GetUpperBound(0)
                If Not crit.DM_PACKAGE_SEARCHES(liPackCounter) Is Nothing Then
                    If crit.DM_PACKAGE_SEARCHES(liPackCounter).PackID > 0 AndAlso Not String.IsNullOrEmpty(crit.DM_PACKAGE_SEARCHES(liPackCounter).Value) Then
                        Dim loPack As Package = Package.GetPackage(crit.DM_PACKAGE_SEARCHES(liPackCounter).PackID)
                        Dim lsLabel As String = loPack.TranslatedName
                        Dim lsValue As String = crit.DM_PACKAGE_SEARCHES(liPackCounter).Value
                        If crit.DM_PACKAGE_SEARCHES(liPackCounter).ForceExact Then
                            Dim laSplit() As String = lsValue.Split(","c)
                            Dim lsTransCombined As String = ""
                            For Each lsSplit In laSplit
                                If Not String.IsNullOrEmpty(lsSplit) Then
                                    Dim lsTrans As String = ""
                                    Select Case loPack.PackType
                                        Case Package.ePackType.CasePackage
                                            Dim loCase As Routing.cCase = Routing.cCase.GetCaseByCaseID(Convert.ToInt32(lsSplit))
                                            If Not loCase Is Nothing Then
                                                lsTrans = loCase.Name
                                            Else
                                                lsTrans = "Unable to find case with case_id " & lsSplit & " for package " & loPack.ID & " - " & loPack.Name
                                            End If

                                        Case Package.ePackType.Docroom
                                            lsTrans = ObjectRepository.GetObjectByDIN(Convert.ToInt32(lsSplit)).Name
                                    End Select


                                    If String.IsNullOrEmpty(lsTransCombined) Then
                                        lsTransCombined = lsTrans
                                    Else
                                        If crit.DM_PACKAGE_SEARCHES(liPackCounter).UseAnd Then
                                            lsTransCombined &= " " & GetLabel("and") & " " & lsTrans
                                        Else
                                            lsTransCombined &= " " & GetLabel("or") & " " & lsTrans
                                        End If
                                    End If
                                End If
                            Next
                            lsValue = lsTransCombined
                        End If

                        AddItem("<b>" & lsLabel & " :</b> " & lsValue, False)
                    End If
                End If
            Next



            If ItemsFound Then
                If crit.LinkedToPackID = 0 Then
                    Select Case crit.ResultType
                        Case DM_OBJECTSearch.eResultType.CaseList
                            If crit.CaseSearch.FilterOnWork Then
                                AddItem(GetBoldLabel("queryresultscope") & " : " & GetLabel("mywork"), False)
                            Else
                                If crit.CreatedBy = Security.BusinessIdentity.CurrentIdentity.Name Then
                                    AddItem(GetBoldLabel("queryresultscope") & " : " & GetLabel("mycases"), False)
                                Else
                                    AddItem(GetBoldLabel("queryresultscope") & " : " & GetLabel("opencases"), False)
                                End If
                            End If
                        Case DM_OBJECTSearch.eResultType.ArchivedCasesList
                            AddItem(GetBoldLabel("queryresultscope") & " : " & GetLabel("archive"), False)
                        Case DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                            AddItem(GetBoldLabel("queryresultscope") & " : " & GetLabel("openandarchivedcases"), False)
                            'Case DM_OBJECTSearch.eResultType.Objects, DM_OBJECTSearch.eResultType.Files
                            '    If crit.DM_INCLUDESUBFOLDERS Then
                            '        AddItem(GetBoldLabel("includesubfolders") & " : " & GetLabel("yes"), False)
                            '    Else
                            '        AddItem(GetBoldLabel("includesubfolders") & " : " & GetLabel("no"), False)
                            '    End If
                    End Select
                End If
            End If

            If Not vbIsParentSearch Then
                If crit.PerformParentsearch Then
                    BuildTree(crit.ParentSearch, True)
                End If
                CloseRootNode()
            End If


        End Sub
    End Class

    Private Class GroupingTree
        Private _tree As obout_ASPTreeView_2_NET.Tree
        Private Class GrouperItem
            Public Field As String
            Public Value As String
            Public Html As String
            Public ImageUrl As String
            Public ImageClass As String
            Public NodeId As String
            Public ParentId As String
            Public Count As Int32
            Public IsBottomItem As Boolean
            Public Property Filter As String
        End Class

        Public Enum ePosition
            Left = 0
            Top = 1
            Bottom = 2
        End Enum

        Public Enum eDisplayMode
            List = 0
            Table = 1
            Pie = 5
            Bar = 6
            Area = 7
            Line = 8
        End Enum

        Private _items As List(Of GrouperItem)

        Public Property ID As Int32

        Private _ctrl As DM_Listcontrol

        Public Sub New(ByVal grouperid As Int32, ByVal c As DM_Listcontrol)

            ID = grouperid
            _ctrl = c
        End Sub

        Public Sub SetPosition(ByVal pos As String)
            Select Case pos
                Case "top"
                    Position = ePosition.Top
                Case "bottom"
                    Position = ePosition.Bottom
                Case Else
                    Position = ePosition.Left
            End Select
        End Sub

        Private _init As Boolean

        Private _folderIcons As String

        Public Sub SetMode(ByVal mode As String)
            Select Case mode.ToLower
                Case "pie"
                    DisplayMode = eDisplayMode.Pie
                Case "table"
                    DisplayMode = eDisplayMode.Table
                Case "area"
                    DisplayMode = eDisplayMode.Area
                Case "bar"
                    DisplayMode = eDisplayMode.Bar
                Case "line"
                    DisplayMode = eDisplayMode.Line
                Case Else
                    DisplayMode = eDisplayMode.List
            End Select

            If Not _init Then

                If System.IO.Directory.Exists(_ctrl.Page.Server.MapPath(String.Concat("~/App_Themes/", _ctrl.Page.Theme, "/Tree/Icons"))) Then
                    _folderIcons = String.Concat("./App_Themes/", _ctrl.Page.Theme, "/Tree/Icons/")
                Else
                    _folderIcons = "./TreeIcons/Icons/"
                End If
                Select Case DisplayMode
                    Case eDisplayMode.List
                        _tree = New obout_ASPTreeView_2_NET.Tree

                        _tree.FolderIcons = _folderIcons
                        _tree.FolderStyle = "./TreeIcons/Styles/Win2003"
                        _tree.FolderScript = "./TreeIcons/Script"
                        _tree.id = "grptree" & ID
                        _tree.ShowRootIcon = False
                        _tree.ShowRootPlusMinus = False
                        'add this so the hidden image is an actual image
                        _tree.AddRootNode("", Nothing, GetItemImage(Nothing, Nothing))
                End Select
                _items = New List(Of GrouperItem)
                _init = True
            End If

        End Sub

        Public Property Position As ePosition

        Public Property DisplayMode As eDisplayMode

        Private _multiLevel As Boolean
        Private Shared Function GetItemImage(ByVal imgUrl As String, ByVal imgClass As String) As String
            If String.IsNullOrEmpty(imgUrl) Then
                Return "item.svg"
            End If
            Return imgUrl

        End Function

        Public Sub Add(ByVal parentnode As String, ByVal nodeid As String, ByVal content As String, ByVal field As String, ByVal value As String, ByVal vsFilter As String, ByVal expand As Boolean, ByVal imgUrl As String, ByVal imgClass As String, ByVal vbIsBottomItem As Boolean)
            If parentnode <> "root" Then
                _multiLevel = True
            End If
            Select Case DisplayMode
                Case eDisplayMode.List
                    If Position = ePosition.Left Then
                        _tree.Add(parentnode, nodeid, content, expand, GetItemImage(imgUrl, imgClass))
                    End If
            End Select

            _items.Add(New GrouperItem() With {.Html = content, .Field = field, .Value = value, .Filter = vsFilter, .ImageUrl = imgUrl, .ImageClass = imgClass, .NodeId = nodeid, .ParentId = parentnode, .IsBottomItem = vbIsBottomItem})
        End Sub

        Public Sub UpdateNodeCount(ByVal nodeid As String, ByVal count As Int32)
            For Each item As GrouperItem In _items
                If item.NodeId = nodeid Then
                    item.Count += count
                    If item.ParentId <> "root" Then
                        UpdateNodeCount(item.ParentId, count)
                    End If
                End If
            Next
        End Sub

        Public Property ShowIcons() As Boolean = True

        Public Property ItemsPerRow As Int32

        Public Property RemoveLink As String

        Public Property Caption As String

        Public Property Height As String

        Public Property Width As String

        Public ReadOnly Property RowHeight() As Integer
            Get
                Return 30
            End Get
        End Property

        Public ReadOnly Property HasItems() As Boolean
            Get
                Return _items IsNot Nothing AndAlso _items.Any
            End Get
        End Property

        Private Function ReplaceCounts(ByVal vsHtml As String) As String
            Return ReplaceCounts(New StringBuilder(vsHtml, vsHtml.Length))
        End Function

        Private Function ReplaceCounts(ByVal sb As StringBuilder) As String
            For Each item As GrouperItem In _items
                If Not item.IsBottomItem Then
                    'vsHtml = vsHtml.Replace("(COUNT_" & loNode.NodeID & ")", "(" & loNode.Count & ")")
                    sb.Replace("(COUNT_" & item.NodeId & ")", "(" & item.Count & ")")
                End If
            Next
            Return sb.ToString
        End Function

        Public ReadOnly Property Html() As String
            Get
                Select Case DisplayMode
                    Case eDisplayMode.List

                        Select Case Position
                            Case ePosition.Left
                                If Not _multiLevel Then
                                    Dim sb As New StringBuilder()

                                    For Each itm As GrouperItem In _items
                                        sb.Append("<div class='Item'>")
                                        If ShowIcons Then
                                            'possibly show icons here?
                                            'sb.Append("<img src='./TreeIcons/Icons/")
                                            sb.Append("<img src='")
                                            sb.Append(_folderIcons)
                                            sb.Append(GetItemImage(itm.ImageUrl, itm.ImageClass))
                                            sb.Append("'/>&nbsp;")
                                        End If
                                        sb.Append(itm.Html)
                                        sb.Append("</div>")

                                    Next


                                    Return sb.ToString
                                Else
                                    _tree.ShowIcons = ShowIcons
                                    Return ReplaceCounts(_tree.HTML)
                                End If
                            Case ePosition.Top, ePosition.Bottom

                                Dim sb As New StringBuilder()
                                sb.Append("<table>")
                                sb.Append(GetHorizontalListRowHtml("root", 0).ToString)
                                sb.Append("</table>")

                                Return ReplaceCounts(sb)
                            Case Else
                                Return ""
                        End Select
                    Case eDisplayMode.Table
                        Select Case Position
                            Case ePosition.Left
                                Return "Table groupers are only supported at top or bottom"
                            Case Else

                                Return GetHorizontalTableHtml().ToString
                        End Select
                    Case Else
                        'not used!!
                        Return ""
                End Select
            End Get
        End Property

        Private Function GetHorizontalTableHtml() As StringBuilder
            Dim lsRet As New StringBuilder()

            'Axelle hier

            'If _multiLevel AndAlso Not String.IsNullOrEmpty(Caption) Then
            '    lsRet.Append("<div>")
            '    lsRet.Append(Caption)
            '    lsRet.Append("</div>")
            'End If

            lsRet.Append("<table id='groupertable' class='SubList GroupingPanel")
            Dim scrollbar As Boolean = False
            If HasItems Then

                If Not _multiLevel Then
                    lsRet.Append("'>")
                    lsRet.Append("<tr><th colspan='2'>")
                    lsRet.Append(RemoveLink)
                    lsRet.Append(Caption)
                    lsRet.Append("</th></tr>")
                    For Each itm As GrouperItem In _items
                        lsRet.Append("<tr><th>")
                        lsRet.Append(itm.Value)
                        lsRet.Append("</th><th style='border:1px solid #d3d3d3'><a href='javascript:")
                        lsRet.Append(_ctrl.ClientID)
                        lsRet.Append(".ApplyGrouper(")
                        lsRet.Append(EncodingUtils.EncodeJsString(itm.Filter))
                        lsRet.Append(");' class='ButtonLink'>")
                        lsRet.Append(itm.Count)
                        lsRet.Append("</a></th></tr>")
                    Next
                Else

                    Dim rootItems As List(Of GrouperItem) = _items.Where(Function(x) x.ParentId.Equals("root")).ToList

                    If Height.EndsWith("px") Then
                        Dim heightNumber As Integer = Integer.Parse(Height.Remove(Height.Length - 2))
                        scrollbar = heightNumber / 30 < rootItems.Count
                    End If

                    If Not String.IsNullOrEmpty(Height) AndAlso scrollbar Then
                        lsRet.Append(" with-scrollbar")
                    End If
                    lsRet.Append("'>")



                    Dim distinctSubItems As List(Of Tuple(Of String, String)) = _items.Where(Function(x) Not x.ParentId.Equals("root")).GroupBy(Function(g) New With {Key g.Field, Key g.Value}).Select(Function(x) New Tuple(Of String, String)(x.Key.Field, x.Key.Value)).ToList

                    lsRet.Append("<thead><tr><td class='Spacer grouper-column' />")
                    For Each distinctField As String In distinctSubItems.GroupBy(Function(x) x.Item1).Select(Function(x) x.Key)
                        Dim colspan As Integer = 0
                        For Each distinctItem As Tuple(Of String, String) In distinctSubItems
                            If distinctItem.Item1.Equals(distinctField) Then
                                colspan += 1
                            End If
                        Next
                        lsRet.Append("<th colspan='" & colspan & "'><div class='property-cell'><div class='property-name'>")
                        lsRet.Append(distinctField)
                        lsRet.Append("</div></div></th>")
                    Next
                    lsRet.Append("<th>")
                    lsRet.Append(_ctrl.GetDecodedLabel("total"))
                    lsRet.Append("</th></tr>")


                    lsRet.Append("<tr><th>")
                    lsRet.Append(RemoveLink)
                    lsRet.Append(rootItems(0).Field)
                    lsRet.Append(" : </th>")
                    For Each distinctField As String In distinctSubItems.GroupBy(Function(x) x.Item1).Select(Function(x) x.Key)
                        For Each distinctItem As Tuple(Of String, String) In distinctSubItems
                            If distinctItem.Item1.Equals(distinctField) Then
                                lsRet.Append("<th class='grouper-column'>")
                                lsRet.Append(distinctItem.Item2)
                                lsRet.Append("</th>")
                            End If
                        Next
                    Next


                    'header under total
                    lsRet.Append("<th  class='grouper-column'></th></tr></thead>")


                    'start items rows
                    lsRet.Append("<tbody")
                    If Not String.IsNullOrEmpty(Height) Then
                        lsRet.Append(" style='max-height:" + Height + ";'")
                    End If
                    lsRet.Append(">")

                    Dim columnTotals As New Dictionary(Of String, Int32)
                    Dim totalTotalCount As Integer = 0
                    For Each rootItem As GrouperItem In rootItems
                        Dim rootItemID As String = rootItem.NodeId
                        Dim rootItemValue As String = rootItem.Value

                        lsRet.Append("<tr>")
                        lsRet.Append("<th class='grouper-column'><a href='javascript:")
                        lsRet.Append(_ctrl.ClientID)
                        lsRet.Append(".ApplyGrouper(")
                        lsRet.Append(EncodingUtils.EncodeJsString(rootItem.Filter))
                        lsRet.Append(");' class='ButtonLink'>")
                        lsRet.Append(rootItem.Value)
                        lsRet.Append("</a></th>")

                        Dim totalCount As Integer = 0
                        For Each distinctField As String In distinctSubItems.GroupBy(Function(x) x.Item1).Select(Function(x) x.Key)
                            totalCount = 0 'restart the count in case of multiple items
                            For Each distinctItem As Tuple(Of String, String) In distinctSubItems
                                If distinctItem.Item1.Equals(distinctField) Then
                                    Dim itemField As String = distinctItem.Item1
                                    Dim itemValue As String = distinctItem.Item2
                                    ' Dim ActualItem As GrouperItem = _items.FirstOrDefault(Function(x) x.ParentId = lsRootItemID AndAlso x.Value = itemValue)
                                    'see if there's an item
                                    Dim actualItemCount As Int32 = GetItemCount(rootItemID, itemField, itemValue)
                                    'Dim ActualItem As GrouperItem = FindNode(lsRootItemID, itemField, itemValue)

                                    If actualItemCount <> 0 Then
                                        lsRet.Append("<td class='grouper-column'><a href='javascript:")
                                        lsRet.Append(_ctrl.ClientID)
                                        lsRet.Append(".ApplyGrouper(")
                                        'Dim ActualItem As GrouperItem = FindNode(lsRootItemID, itemField, itemValue)
                                        Dim actualItemFilter = GetItemFilter(rootItemID, rootItemValue, itemField, itemValue)
                                        lsRet.Append(EncodingUtils.EncodeJsString(actualItemFilter))
                                        lsRet.Append(");' class='ButtonLink'>")
                                        lsRet.Append(actualItemCount)
                                        lsRet.Append("</a></td>")

                                        totalCount += actualItemCount
                                    Else
                                        lsRet.Append("<td  class='grouper-column'/>")
                                    End If
                                End If
                            Next

                        Next

                        totalTotalCount += totalCount

                        lsRet.Append("<td class='grouper-column'><b>")
                        lsRet.Append(totalCount)
                        lsRet.Append("</b></td></tr>")
                    Next


                    'footer table
                    lsRet.Append("</tbody><tfoot><tr><td class='Spacer'/>")

                    'loop in the other direction
                    For Each distinctField As String In distinctSubItems.GroupBy(Function(x) x.Item1).Select(Function(x) x.Key)

                        For Each distinctItem As Tuple(Of String, String) In distinctSubItems
                            If distinctItem.Item1.Equals(distinctField) Then
                                Dim colTotal As Int32 = 0
                                Dim itemField As String = distinctItem.Item1
                                Dim itemValue As String = distinctItem.Item2
                                For Each rootItem As GrouperItem In rootItems
                                    Dim lsRootItemID As String = rootItem.NodeId
                                    colTotal += GetItemCount(lsRootItemID, itemField, itemValue)
                                Next

                                lsRet.Append("<td><b>")
                                lsRet.Append(colTotal)
                                lsRet.Append("</b></td>")

                            End If
                        Next
                    Next


                    'total total
                    lsRet.Append("<td><b>")
                    lsRet.Append(totalTotalCount)
                    lsRet.Append("</b></td></tr></tfoot>")

                End If
            Else
                'no items

                lsRet.Append("<tr><th>")
                lsRet.Append(Caption)
                lsRet.Append("</th></tr>")
                lsRet.Append("<tr><td class='ListFooter'>")
                lsRet.Append(_ctrl.GetLabel("noresultsfound"))
                lsRet.Append("</td></tr>")
            End If
            lsRet.Append("</table>")
            If scrollbar Then
                lsRet.Append("<script type='text/javascript'>const table = document.getElementById('groupertable'); table.tHead.style.width = table.scrollWidth - 18 + 'px'; table.tFoot.style.width = table.scrollWidth - 18 + 'px';</script>")
            End If
            Return lsRet
        End Function

        Private Function GetItemFilter(parentNode As String, parentValue As String, field As String, value As String) As String


            'get the filter of any item          
            Dim anyItem As GrouperItem = FindNodes(parentNode, field, value).FirstOrDefault()
            If anyItem Is Nothing Then Return "" 'this shouldn't happen

            Dim itemFilter As String = anyItem.Filter

            'see how deep the item is
            Dim level As Integer = GetItemDept(parentNode, field, 0)

            'split the filter and remove any part that belongs to a different depth
            Dim parts() As String = itemFilter.Split("||")
            Dim actualFilter As New StringBuilder
            Dim i As Integer = 0
            For Each part As String In parts
                If String.IsNullOrEmpty(part) Then Continue For

                'add root and the exact level
                If i = 0 OrElse i = level Then
                    actualFilter.Append(part)
                Else
                    'clear any other part
                    actualFilter.Append("<O>")
                End If
                actualFilter.Append("||")

                i += 1
            Next

            Return actualFilter.ToString()

        End Function

        Private Function GetItemCount(parentNode As String, field As String, value As String) As Integer

            Return FindNodes(parentNode, field, value).Sum(Function(x) x.Count)

        End Function

        Private Function GetItemDept(parentNode As String, field As String, ByVal prevDept As Integer) As Integer

            prevDept += 1
            'Dim count As Int32 = 0
            For Each node As GrouperItem In _items.Where(Function(x) x.ParentId = parentNode)

                If node.Field.Equals(field) Then
                    Return prevDept
                Else
                    Return GetItemDept(node.NodeId, field, prevDept)
                End If
            Next

            Return 0
            'Return count
        End Function

        Private Function FindNodes(parentNode As String, field As String, value As String) As IEnumerable(Of GrouperItem)
            Dim nodes As New List(Of GrouperItem)
            For Each node As GrouperItem In _items.Where(Function(x) x.ParentId = parentNode)

                If node.Field.Equals(field) AndAlso node.Value.Equals(value) Then
                    nodes.Add(node)
                Else
                    If Not node.IsBottomItem Then
                        nodes.AddRange(FindNodes(node.NodeId, field, value))
                    End If
                End If
            Next
            Return nodes
        End Function

        Private Function GetHorizontalListRowHtml(ByVal vsRoot As String, ByRef iColCount As Int32) As StringBuilder
            Dim lsRet As New StringBuilder()
            Dim lbInit As Boolean = False
            Dim iItemCount As Int32 = 0
            Dim isRootItem As Boolean = (vsRoot = "root")
            Dim sbFullSub As New StringBuilder
            For Each itm As GrouperItem In _items
                If itm.ParentId = vsRoot Then
                    iItemCount += 1
                    Dim liSubColCount As Int32 = 0
                    Dim sbSub As StringBuilder = GetHorizontalListRowHtml(itm.NodeId, liSubColCount)

                    Dim lsItemHtml As String = itm.Html
                    If ShowIcons Then

                        lsItemHtml = "<img src='" & _folderIcons & itm.ImageUrl & "'/>&nbsp;" & lsItemHtml
                    End If

                    If sbSub.Length = 0 Then
                        liSubColCount = 1
                    End If

                    iColCount += liSubColCount

                    If ItemsPerRow > 0 AndAlso isRootItem AndAlso lbInit Then
                        If ItemsPerRow = 1 OrElse iItemCount Mod ItemsPerRow = 1 Then
                            lsRet.Append("</tr>")
                            lbInit = False
                        End If
                    End If
                    If Not lbInit Then
                        If isRootItem Then
                            lsRet.Append("<tr>")
                        End If
                        lbInit = True
                    End If

                    If isRootItem Then
                        lsRet.Append("<td><table width='100%'><tr>")

                        lsRet.Append("<td colspan='")
                        lsRet.Append(liSubColCount)
                        lsRet.Append("'><b>")
                        lsRet.Append(lsItemHtml)
                        lsRet.Append("</b></td>")
                        lsRet.Append("</tr>")
                    Else
                        lsRet.Append("<td colspan='")
                        lsRet.Append(liSubColCount)
                        lsRet.Append("'>")
                        lsRet.Append(lsItemHtml)
                        lsRet.Append("</td>")
                    End If
                    If sbSub.Length > 0 Then
                        If isRootItem Then
                            lsRet.Append("<tr>")
                            lsRet.Append(sbSub)
                            lsRet.Append("</tr>")
                        Else
                            sbFullSub.Append(sbSub)
                        End If
                    End If
                    If isRootItem Then
                        lsRet.Append("</table></td>")
                    Else
                        lsRet.Append("</td>")
                    End If

                End If
            Next
            If sbFullSub.Length > 0 Then
                lsRet.Append("</tr><tr>")
                lsRet.Append(sbFullSub)
            End If

            If lbInit AndAlso isRootItem Then

                lsRet.Append("</tr>")
            End If
            Return lsRet
        End Function

        Public Function GetControl() As Control

            Dim lsGrouperHeader As New StringBuilder

            Select Case DisplayMode
                Case eDisplayMode.List
                    If HasItems Then
                        Select Case Position
                            Case ePosition.Left


                                lsGrouperHeader.Append("<div class='GroupingPanel' style='width:" & Width & "'><div class='Header'>")
                                lsGrouperHeader.Append(RemoveLink)
                                lsGrouperHeader.Append(Caption)
                                lsGrouperHeader.Append("</div><div class='Content' style='max-height:")
                                lsGrouperHeader.Append(Height)
                                lsGrouperHeader.Append(";'>")

                            Case ePosition.Top, ePosition.Bottom
                                lsGrouperHeader.Append("<div style='width:100%;max-height:")
                                lsGrouperHeader.Append(Height)
                                lsGrouperHeader.Append(";overflow:auto'>")
                        End Select
                        Dim sb As New StringBuilder()
                        sb.Append(lsGrouperHeader)
                        sb.Append(Html)
                        Select Case Position
                            Case ePosition.Left
                                sb.Append("</div></div>")
                            Case ePosition.Top, ePosition.Bottom
                                sb.Append("</div>")
                        End Select
                        Return New LiteralControl(sb.ToString())
                    Else
                        Return Nothing
                    End If

                Case eDisplayMode.Table
                    Return New LiteralControl(Html)
                Case eDisplayMode.Pie
                    If HasItems Then
                        If Not _multiLevel Then
                            Dim chPie As RadHtmlChart = NewHtmlChart()
                            Dim loPieSeries As PieSeries = NewHtmlChartSerie(False)

                            '  If Not String.IsNullOrEmpty(vsYAxis) Then serie.AxisName = vsYAxis
                            loPieSeries.Items.AddRange(_items.Select(Function(x) New SeriesItem(x.Count) With {.Name = x.Value}))

                            chPie.Legend.Appearance.Visible = False
                            chPie.PlotArea.Series.Add(loPieSeries)

                            RegisterChartClick(chPie, False)
                            Return chPie
                        Else
                            Return New LiteralControl("Pie charts are not supported for multiple levels")
                        End If
                    Else
                        Return Nothing
                    End If

                Case eDisplayMode.Bar, eDisplayMode.Line, eDisplayMode.Area
                    If HasItems Then
                        Dim chcols As RadHtmlChart = NewHtmlChart()

                        chcols.PlotArea.XAxis.DataLabelsField = "Value"
                        If _multiLevel Then
                            Dim rootItems As List(Of GrouperItem) = _items.Where(Function(x) x.ParentId.Equals("root")).ToList
                            chcols.PlotArea.XAxis.Items.AddRange(rootItems.Select(Function(x) New AxisItem(x.Value)))

                            Dim distinctSubItems As List(Of String) = _items.Where(Function(x) x.IsBottomItem).GroupBy(Function(g) g.Value).Select(Function(x) x.Key).ToList
                            For Each distinctItem As String In distinctSubItems
                                Dim localValue As String = distinctItem
                                Dim loColumnSeries As SeriesBase = NewHtmlChartSerie(True)
                                loColumnSeries.Name = localValue
                                Dim lcolSubItems As IEnumerable(Of GrouperItem) = _items.Where(Function(x) x.Value.Equals(localValue))
                                Dim lcolSubSerie As IEnumerable(Of SeriesItem) = rootItems.Select(Function(x) GetFilteredSeriesItem(x.NodeId, lcolSubItems))
                                loColumnSeries.Items.AddRange(lcolSubSerie)
                                chcols.PlotArea.Series.Add(loColumnSeries)
                            Next
                        Else
                            chcols.PlotArea.XAxis.Items.AddRange(_items.Select(Function(x) New AxisItem(x.Value)))
                            Dim loColumnSeries As SeriesBase = NewHtmlChartSerie(False)
                            loColumnSeries.Items.AddRange(_items.Select(Function(x) New SeriesItem(x.Count)))
                            chcols.PlotArea.Series.Add(loColumnSeries)
                        End If
                        RegisterChartClick(chcols, _multiLevel)
                        Return chcols
                    Else
                        Return Nothing
                    End If

                Case Else
                    Return Nothing
            End Select

        End Function

        Private Function NewHtmlChartSerie(ByVal isSub As Boolean) As SeriesBase
            Select Case DisplayMode
                Case eDisplayMode.Pie
                    Dim loPieSeries As New PieSeries With {
                        .NameField = "Name"
                    }
                    loPieSeries.TooltipsAppearance.ClientTemplate = "#=category# (#=value#)"
                    loPieSeries.DataFieldY = "yValue"
                    loPieSeries.LabelsAppearance.ClientTemplate = "#=category# (#=value#)"
                    Return loPieSeries
                Case eDisplayMode.Bar
                    Dim loBarSeries As New ColumnSeries
                    If Not isSub Then
                        loBarSeries.TooltipsAppearance.ClientTemplate = "#=category# (#=value#)"
                    Else
                        loBarSeries.TooltipsAppearance.ClientTemplate = "#=category# : #=series.name# (#=value#)"
                    End If
                    loBarSeries.DataFieldY = "yValue"
                    'loBarSeries.LabelsAppearance.ClientTemplate = "#=category# (#=value#)"
                    Return loBarSeries
                Case eDisplayMode.Area
                    Dim loAreaSeries As New AreaSeries
                    loAreaSeries.MissingValues = HtmlChart.MissingValuesBehavior.Gap
                    loAreaSeries.TooltipsAppearance.ClientTemplate = "#=category# (#=value#)"
                    loAreaSeries.DataFieldY = "yValue"
                    'loAreaSeries.LabelsAppearance.ClientTemplate = "#=category# (#=value#)"
                    Return loAreaSeries
                Case eDisplayMode.Line
                    Dim loLineSeries As New LineSeries
                    loLineSeries.MissingValues = HtmlChart.MissingValuesBehavior.Gap
                    loLineSeries.TooltipsAppearance.ClientTemplate = "#=category# (#=value#)"
                    loLineSeries.DataFieldY = "yValue"
                    ' loLineSeries.LabelsAppearance.ClientTemplate = "#=category# (#=value#)"
                    Return loLineSeries
                Case Else
                    Return Nothing
            End Select
        End Function

        Private Sub RegisterChartClick(ByVal chart As RadHtmlChart, ByVal isMulti As Boolean)

            chart.OnClientSeriesClicked = "OnClientSeries" & Me.ID & "Clicked"

            Dim sb As New StringBuilder
            sb.AppendLine("function OnClientSeries" & Me.ID & "Clicked(sender, args) {")
            If Not isMulti Then
                sb.AppendLine("switch(args.get_category()){")
                For Each itm As GrouperItem In _items
                    sb.Append("case ")
                    sb.Append(EncodingUtils.EncodeJsString(itm.Value))
                    sb.Append(":")
                    sb.Append(_ctrl.ClientID)
                    sb.Append(".ApplyGrouper(")
                    sb.Append(EncodingUtils.EncodeJsString(itm.Filter))
                    sb.AppendLine(");break;")
                Next
                sb.AppendLine("}")
            Else
                sb.AppendLine("switch(args.get_category()){")
                For Each itm As GrouperItem In _items.Where(Function(x) Not x.IsBottomItem)
                    Dim lsItemID As String = itm.NodeId
                    sb.Append("case ")
                    sb.Append(EncodingUtils.EncodeJsString(itm.Value))
                    sb.AppendLine(":")
                    For Each subitm As GrouperItem In _items.Where(Function(x) x.ParentId = lsItemID)
                        sb.AppendLine(" switch(args.get_category()){")
                        sb.Append(" case ")
                        sb.Append(EncodingUtils.EncodeJsString(subitm.Value))
                        sb.Append(":")

                        sb.Append(_ctrl.ClientID)
                        sb.Append(".ApplyGrouper(")
                        sb.Append(EncodingUtils.EncodeJsString(subitm.Filter))
                        sb.AppendLine(");break;")
                        sb.AppendLine("}")
                    Next

                Next
                sb.AppendLine("}")
            End If

            'sb.Append("var theDataItem = args.get_dataItem();")
            'sb.Append("theDataItem.IsExploded = !theDataItem.IsExploded;")
            'sb.Append("sender.repaint();")
            sb.Append("}")

            _ctrl.Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "grp" & ID, sb.ToString, True)

        End Sub

        Private Function NewHtmlChart() As RadHtmlChart

            Dim chart As New RadHtmlChart
            If Not String.IsNullOrEmpty(Width) Then chart.Width = New Unit(Width)
            If Not String.IsNullOrEmpty(Height) Then chart.Height = New Unit(Height)
            chart.ChartTitle.Text = Caption
            chart.Style.Add("cursor", "pointer")
            Select Case Position
                Case ePosition.Bottom, ePosition.Top
                    'chart.Style.Add("display", "inline")
                    chart.Style.Add("float", "left")
            End Select
            Return chart


        End Function

        Private Function GetFilteredSeriesItem(ByVal vsParentID As String, lcolFiltered As IEnumerable(Of GrouperItem)) As SeriesItem
            Dim loFiltered As GrouperItem = lcolFiltered.FirstOrDefault(Function(x) x.ParentId = vsParentID)
            If Not loFiltered Is Nothing Then
                Return New SeriesItem(loFiltered.Count) With {.Name = loFiltered.Html}
            Else
                If DisplayMode = eDisplayMode.Bar Then
                    Return New SeriesItem(0)
                Else
                    Return New SeriesItem(Nothing)
                End If
            End If
        End Function
    End Class

    Public Function GetSideBarWidth() As String
        If Not Visible OrElse Not pnlLeftSidePanel.Visible Then
            Return Nothing
        End If

        Dim grouperwidth As String = GridGrouping.DefaultGroupingWidth
        If String.IsNullOrEmpty(grouperwidth) Then Return Nothing

        Return (Unit.Parse(grouperwidth).Value + 7) & "px" '7 = 2px padding-left + 5px padding-right of the listControlSide inner div
    End Function

    Private _hasSideBar As Boolean

    Private _sideBarWidth As Integer

    Private Sub RenderSideBar()

        _hasSideBar = (Layout.ShowSideBar AndAlso ShowSideBar)
        If _hasSideBar AndAlso UserProfile.ShowGridSideBar Then


            If ShowCalendar OrElse Layout.EnableQuickUpload Then 'show the calendar
                pnlLeftSidePanel.Visible = True
                _hasSideBar = True
            End If

            If ShowSearchedForTree AndAlso Layout.ShowSearchedFor AndAlso _showList Then
                'show the searched for tree in the grouping location

                Dim loSearchedforTable As New SearchedForTable(Me, ShowSearchedForCategory)
                loSearchedforTable.AsTree = False
                loSearchedforTable.QueryName = msQueryName
                loSearchedforTable.ShowBackToSearchLink = ShowBackToSearchLink AndAlso Layout.ShowBackToSearch
                loSearchedforTable.Width = GridGrouping.DefaultGroupingWidth

                loSearchedforTable.BuildTree(_resultListCrit)

                If loSearchedforTable.ItemsFound Then
                    pnlLeftSidePanel.Visible = True
                    _hasSideBar = True
                    plhGrouperSide.Controls.Add(loSearchedforTable.GetTree)
                End If

            End If

            If Not String.IsNullOrEmpty(GroupBy.Value) Then

                mbShowGrouper = False
                Dim lbShowClearGroupersLink As Boolean = False

                Dim liLastGrouperId As Int32 = -1
                Dim laGroupers() As String = Split(GroupBy.Value, "##")

                Dim currentFilters As String()
                If Not String.IsNullOrEmpty(_resultListCrit.GroupResultsFor) Then
                    currentFilters = Split(_resultListCrit.GroupResultsFor, "||")
                Else
                    currentFilters = New String() {}
                End If

                For Each grouper As String In laGroupers.Select(Function(x) x.Trim).Where(Function(x) x.Length <> 0)

                    Dim ligrouperid As Int32 = CType(grouper.Substring(0, grouper.IndexOf(";;", StringComparison.Ordinal)), Int32)
                    If ligrouperid <> liLastGrouperId Then

                        liLastGrouperId = ligrouperid
                        Dim sbReverseFilter As New StringBuilder

                        Dim logrpCrit As DM_OBJECTSearch.Criteria = _resultListCrit

                        Dim loGroupertree As New GroupingTree(ligrouperid, Me)

                        'read the grouper info
                        Dim excludeEmptyValues As Boolean = False
                        Dim checkAcl As Boolean = True
                        Dim lbApplyFilterToGrouping As Boolean = True
                        Dim orderByValue As Boolean = False
                        Dim orderByAscending As Boolean = True
                        Dim llLinkedGrouperId As Integer = 0
                        Dim showGrouper As Boolean = True
                        Dim maxNumberOfCharacters As Integer = 0
                        Dim llMaxResults As Int32 = 0
                        Dim formatters As New List(Of String)
                        Dim fieldNames As New List(Of String)

                        'set grouping parameters                    
                        For Each grouperDefinition As GridGroupingInfo.Grouper In GridGrouping.GrouperList
                            If grouperDefinition.ID = ligrouperid Then

                                llLinkedGrouperId = grouperDefinition.LinkedID
                                lbApplyFilterToGrouping = grouperDefinition.ApplyFilter
                                orderByValue = grouperDefinition.OrderByValue
                                orderByAscending = grouperDefinition.OrderByAscending

                                loGroupertree.SetPosition(grouperDefinition.Position)
                                loGroupertree.SetMode(grouperDefinition.Display)

                                loGroupertree.Caption = grouperDefinition.GetLabel
                                If String.IsNullOrEmpty(loGroupertree.Caption) Then
                                    loGroupertree.Caption = GetLabel("grouping") & " " & ligrouperid
                                End If
                                loGroupertree.ShowIcons = Not grouperDefinition.HideIcons
                                excludeEmptyValues = Not grouperDefinition.ShowNull
                                checkAcl = grouperDefinition.CheckAcl
                                llMaxResults = grouperDefinition.MaxResults

                                maxNumberOfCharacters = grouperDefinition.MaxNumberOfCharacters
                                loGroupertree.Height = grouperDefinition.Height
                                loGroupertree.Width = grouperDefinition.Width
                                loGroupertree.ItemsPerRow = grouperDefinition.ItemsPerRow



                                If grouperDefinition.UserGrouper Then
                                    Dim lsRemoveLink As New StringBuilder
                                    lsRemoveLink.Append("<a href='javascript:")
                                    For Each loField In grouperDefinition.Fields

                                        lsRemoveLink.Append(JavascriptVariableName)
                                        lsRemoveLink.Append(".RemoveGrouper(")
                                        lsRemoveLink.Append(grouperDefinition.ID)
                                        lsRemoveLink.Append(",")
                                        lsRemoveLink.Append(EncodingUtils.EncodeJsString(loField.GetFieldName(ContentParams)))
                                        lsRemoveLink.Append(");")
                                    Next

                                    lsRemoveLink.Append(JavascriptVariableName)
                                    lsRemoveLink.Append(".Goto(1);'>")
                                    lsRemoveLink.Append("<span class='icon-delete' title='" & GetLabel("remove") & "' ></span>")
                                    lsRemoveLink.Append("</a>&nbsp;")
                                    loGroupertree.RemoveLink = lsRemoveLink.ToString()
                                Else
                                    loGroupertree.RemoveLink = ""
                                End If



                                For Each field As GridGroupingInfo.Grouper.GrouperField In grouperDefinition.Fields
                                    formatters.Add(field.Formatter.ToLower())

                                    fieldNames.Add(field.GetLabel())

                                Next
                            End If
                        Next

                        If llLinkedGrouperId > 0 Then
                            showGrouper = False
                            'check if a filter has been applied on the linked grouper
                            If currentFilters.Length <> 0 Then
                                For n As Integer = 0 To logrpCrit.Groupers.Length - 1
                                    If logrpCrit.Groupers(n).GrouperID = llLinkedGrouperId AndAlso currentFilters(n) <> "<O>" Then
                                        showGrouper = True
                                    End If
                                Next
                            End If
                        End If

                        If showGrouper Then
                            If currentFilters.Length <> 0 Then ' filter applied
                                lbShowClearGroupersLink = True
                                For n As Integer = 0 To logrpCrit.Groupers.Length - 1
                                    If logrpCrit.Groupers(n).GrouperID = ligrouperid Then
                                        sbReverseFilter.Append("<O>||") 'remove the filters from this grouper
                                    Else
                                        sbReverseFilter.Append(currentFilters(n))
                                        sbReverseFilter.Append("||") 'keep the filters from other groupers
                                    End If
                                Next
                            End If

                            Dim reverseFilter As String = sbReverseFilter.ToString

                            Dim liNodeId As Int32 = 1
                            Dim liParentNodeId As Int32
                            Dim laLastValues() As String
                            Dim laNodeLevels() As Int32


                            Dim liResetLevel As Int32 = 0
                            Dim lsFilter As String = ""

                            logrpCrit.GroupingHeadersGroupID = ligrouperid
                            logrpCrit.ExpandResults = False
                            logrpCrit.GroupingHeadersOnly = True
                            logrpCrit.GroupingExcludeNull = excludeEmptyValues
                            If checkAcl Then
                                logrpCrit.ACLCheck = True
                            Else
                                logrpCrit.ACLCheck = False
                                logrpCrit.TenantId = BusinessIdentity.CurrentIdentity.Tenant.Id
                            End If

                            logrpCrit.MAXRESULTS = llMaxResults
                            If orderByValue Then
                                If orderByAscending Then
                                    logrpCrit.GroupingSortBy = DM_OBJECTSearch.Criteria.eGroupingOrderBy.WordAscending
                                Else
                                    logrpCrit.GroupingSortBy = DM_OBJECTSearch.Criteria.eGroupingOrderBy.WordDescending
                                End If

                            Else
                                If orderByAscending Then
                                    logrpCrit.GroupingSortBy = DM_OBJECTSearch.Criteria.eGroupingOrderBy.CountAscending
                                Else
                                    logrpCrit.GroupingSortBy = DM_OBJECTSearch.Criteria.eGroupingOrderBy.CountDescending
                                End If
                            End If
                            If Not lbApplyFilterToGrouping Then
                                lsFilter = logrpCrit.GroupResultsFor
                                logrpCrit.GroupResultsFor = "" 'always show complete grouping
                            End If

                            Dim lbLoadGrouper As Boolean = (NumberOfResults > 0 OrElse Not lbApplyFilterToGrouping)
                            Dim lcolGrouper As DM_OBJECTSearch = Nothing
                            Dim lbGrouperError As Boolean = False
                            Try
                                If lbLoadGrouper Then
                                    lcolGrouper = DM_OBJECTSearch.GetOBJECTList(logrpCrit)
                                Else
                                    lcolGrouper = DM_OBJECTSearch.GetPlaceHolderList
                                End If

                            Catch ex As Exception
                                lbGrouperError = True
                                Arco.Utils.Logging.LogError("Error executing grouper", ex)

                                AddLoadingError("An error executing grouper" & ligrouperid)

                            Finally
                                If Not lbApplyFilterToGrouping Then
                                    logrpCrit.GroupResultsFor = lsFilter
                                End If
                            End Try

                            If Not lbGrouperError Then
                                Dim firstItemInGrouper As Integer = -1
                                Dim grouperFound As Boolean = False

                                Dim lastItemInGrouper As Integer = 0
                                For iTemp4 As Int32 = 0 To lcolGrouper.Groupers.GroupingFields.Count - 1
                                    If lcolGrouper.Groupers.GroupingFields.Item(iTemp4).GrouperID = ligrouperid Then
                                        If Not grouperFound Then
                                            grouperFound = True
                                            firstItemInGrouper = iTemp4
                                        End If
                                        lastItemInGrouper = iTemp4
                                    Else
                                        If grouperFound Then Exit For
                                    End If

                                Next

                                ReDim laLastValues(lcolGrouper.Groupers.GroupingFields.Count)
                                ReDim laNodeLevels(lcolGrouper.Groupers.GroupingFields.Count)

                                For Each loGrouperContent As DM_OBJECTSearch.GrouperContent In lcolGrouper.Groupers.GrouperContent
                                    Dim itemp As Int32 = 0
                                    Dim liNodeLevel As Int32 = 0
                                    Dim liActualNodeLevel As Int32 = 0
                                    For Each loGrpvalue As DM_OBJECTSearch.GrouperValues In loGrouperContent.GrouperValues
                                        Dim isCurrentGrouper As Boolean = (ligrouperid = loGrpvalue.GrouperID)
                                        Dim showNode As Boolean = (isCurrentGrouper AndAlso laLastValues(itemp) <> loGrpvalue.Value)
                                        laLastValues(itemp) = loGrpvalue.Value

                                        If showNode Then

                                            mbShowGrouper = True

                                            'add fillers                                               
                                            Dim filterToApply As New StringBuilder
                                            For iTemp2 As Int32 = 0 To lcolGrouper.Groupers.GroupingFields.Count - 1

                                                If lcolGrouper.Groupers.GroupingFields.Item(iTemp2).GrouperID = ligrouperid Then
                                                    If iTemp2 <= itemp Then
                                                        filterToApply.Append(laLastValues(iTemp2))
                                                    Else
                                                        filterToApply.Append("<O>")
                                                    End If
                                                Else
                                                    If currentFilters.Length <> 0 Then
                                                        filterToApply.Append(currentFilters(iTemp2))
                                                    Else
                                                        filterToApply.Append("<O>")
                                                    End If
                                                End If
                                                filterToApply.Append("||")
                                            Next
                                            liParentNodeId = laNodeLevels(liNodeLevel)

                                            Dim lbisCurrent As Boolean = (filterToApply.ToString = GroupByFilter.Value)
                                            Dim lbExpand As Boolean = (GroupByFilter.Value.IndexOf(loGrpvalue.Value & "||", StringComparison.Ordinal) >= 0)

                                            Dim currentFormatter As String = ""
                                            Dim currentFieldName As String = ""
                                            Try
                                                currentFormatter = formatters(liActualNodeLevel)
                                                currentFieldName = fieldNames(liActualNodeLevel)
                                            Catch ex As Exception
                                                AddLoadingError("Error binding grouper for file " & GridGrouping.FullFilePath)
                                            End Try

                                            Dim lbIsBottomItem As Boolean = (itemp = lastItemInGrouper)


                                            Dim lsCaption As String
                                            If loGrpvalue.Caption.Length <> 0 Then
                                                lsCaption = Arco.Web.ResourceManager.ReplaceLabeltags(loGrpvalue.Caption)
                                                If Not String.IsNullOrEmpty(currentFormatter) Then
                                                    lsCaption = FormatterFactory.GetFormatter(currentFormatter).Format(lsCaption, True)
                                                Else

                                                    lsCaption = ArcoFormatting.FormatText(lsCaption, maxNumberOfCharacters, Nothing, True)
                                                End If
                                            Else
                                                lsCaption = GetLabel("emptygroupervalue")
                                            End If




                                            Dim lsCount As New StringBuilder
                                            If lbIsBottomItem Then 'last item
                                                lsCount.Append("&nbsp;(")
                                                lsCount.Append(loGrouperContent.ResultCount)
                                            Else
                                                lsCount.Append("&nbsp;(COUNT_grp")
                                                lsCount.Append(liNodeId)
                                            End If
                                            lsCount.Append(")")

                                            liNodeLevel += 1
                                            liActualNodeLevel += 1
                                            laNodeLevels(liNodeLevel) = liNodeId
                                            If liResetLevel > liNodeLevel Then
                                                For iTemp3 As Int32 = liNodeLevel To lcolGrouper.Groupers.GroupingFields.Count - 1
                                                    laLastValues(iTemp3) = ""
                                                Next
                                            End If
                                            liResetLevel = liNodeLevel

                                            Dim lsFilterString As String
                                            If lbisCurrent Then
                                                lsFilterString = reverseFilter
                                            Else
                                                lsFilterString = filterToApply.ToString
                                            End If

                                            Dim treeHtml As String
                                            If Not lbisCurrent Then
                                                treeHtml = String.Format("<a href='javascript:{0}.ApplyGrouper({1});' class='GrouperLink'>{2}{3}</a>", JavascriptVariableName, EncodingUtils.EncodeJsString(lsFilterString), lsCaption, lsCount.ToString)
                                            Else
                                                treeHtml = String.Format("<a href='javascript:{0}.ApplyGrouper({1});' class='GrouperLink'><span class='Label'>{2}{3}</span></a>", JavascriptVariableName, EncodingUtils.EncodeJsString(lsFilterString), lsCaption, lsCount.ToString)
                                            End If

                                            If liParentNodeId <> 0 Then
                                                loGroupertree.Add("grp" & liParentNodeId, "grp" & liNodeId, treeHtml, currentFieldName, lsCaption, lsFilterString, lbExpand, loGrpvalue.ImageUrl, loGrpvalue.ImageClass, lbIsBottomItem)
                                            Else
                                                loGroupertree.Add("root", "grp" & liNodeId, treeHtml, currentFieldName, lsCaption, lsFilterString, lbExpand, loGrpvalue.ImageUrl, loGrpvalue.ImageClass, lbIsBottomItem)
                                            End If
                                            If lbIsBottomItem Then
                                                loGroupertree.UpdateNodeCount("grp" & liNodeId, loGrouperContent.ResultCount)
                                            End If
                                            liNodeId += 1
                                        Else 'if shownode
                                            If isCurrentGrouper Then
                                                liActualNodeLevel += 1
                                            End If
                                            liNodeLevel += 1
                                        End If
                                        itemp += 1
                                    Next 'loGrpvalue
                                    ' iGrpIDFilters = itemp
                                Next 'logroupercontent

                                Dim grouperControl = loGroupertree.GetControl
                                If grouperControl IsNot Nothing Then
                                    _hasSideBar = True
                                    ' loTimer.StartWatch()
                                    Select Case loGroupertree.Position
                                        Case GroupingTree.ePosition.Left
                                            pnlLeftSidePanel.Visible = True
                                            plhGrouperSide.Controls.Add(grouperControl)
                                        Case GroupingTree.ePosition.Top
                                            pnlTopSidePanel.Visible = True
                                            plhGrouperTop.Controls.Add(grouperControl)
                                        Case GroupingTree.ePosition.Bottom
                                            pnlBottomSidePanel.Visible = True
                                            plhGrouperBottom.Controls.Add(grouperControl)
                                    End Select
                                End If

                            Else 'groupererror

                            End If
                        End If
                        ' End If
                    End If

                Next 'j

                If lbShowClearGroupersLink Then
                    Dim sbReverseFilter As New StringBuilder
                    Dim filtersApplied As Boolean = False
                    For n As Int32 = 0 To _resultListCrit.Groupers.Length - 1
                        If Not String.IsNullOrEmpty(currentFilters(n)) AndAlso currentFilters(n) <> "<O>" Then
                            filtersApplied = True
                        End If
                        sbReverseFilter.Append("<O>||") 'remove all filters
                    Next
                    If filtersApplied Then
                        Dim clearlink As String = String.Format("<a href='javascript:{0}.ApplyGrouper({1});' class='ButtonLink'>{2}</a>", JavascriptVariableName, EncodingUtils.EncodeJsString(sbReverseFilter.ToString), GetLabel("showall"))
                        If pnlLeftSidePanel.Visible Then
                            plhGrouperSide.Controls.Add(New LiteralControl("<center>" & clearlink & "</center><br/>"))
                        ElseIf pnlTopSidePanel.Visible Then
                            plhGrouperTop.Controls.Add(New LiteralControl(clearlink & "<br/>"))
                        ElseIf pnlBottomSidePanel.Visible Then
                            plhGrouperBottom.Controls.Add(New LiteralControl(clearlink & "<br/>"))
                        End If
                    End If
                End If
            End If

            If ShowQueryList AndAlso Layout.ShowQueries AndAlso _resultListCrit.LinkedToPackID = 0 Then
                _hasSideBar = True
                Dim loQryCrit As New DMQueryList.Criteria(DMQueryList.Criteria.TypeFilter.Normal Or DMQueryList.Criteria.TypeFilter.System, True)
                loQryCrit.ResultType = ConvertCriteriaResultType(_resultListCrit)
                Dim lcolQueries As DMQueryList = DMQueryList.GetQueryList(loQryCrit)
                If lcolQueries.Any Then
                    pnlLeftSidePanel.Visible = True
                    Dim lsQueries As New StringBuilder("<div class='GroupingPanel' style='width:" & GridGrouping.DefaultGroupingWidth & "'><div class='Header'>")
                    lsQueries.Append(GetLabel("myqueries"))
                    lsQueries.Append("</div><div class='Content' style='height:100px;'>")

                    For Each loQuery As DMQueryList.QueryInfo In lcolQueries
                        Dim queryUrl As String = "DM_DOCUMENTLIST.aspx?QRY_ID=" & loQuery.ID & "&screenmode=advsearch&DM_SEARCH_SCREEN_ID=" & loQuery.SearchScreenID & "&result_type=" & loQuery.ResultType & "&DM_PARENT_ID=" & CurrentFolder.ID & "&LOADQRY=Y"
                        lsQueries.Append("<div style='margin-left:5px;'><a href='javascript:PC().GotoLink(")
                        lsQueries.Append(EncodingUtils.EncodeJsString(queryUrl))
                        lsQueries.Append(");'>")
                        lsQueries.Append(Server.HtmlEncode(loQuery.Name))
                        lsQueries.Append("</a><br/></div>")
                    Next

                    lsQueries.Append("</div></div>")

                    plhGrouperSide.Controls.Add(New LiteralControl(lsQueries.ToString))
                End If
            End If

        End If

    End Sub

    Private Sub InitCalendar()
        If Not ShowCalendar Then
            Exit Sub
        End If

        pnlLeftSidePanel.Visible = True
        pnlCalWhatsNew.Visible = True
        calWhatsNew.ClientEvents.OnDateSelecting = JavascriptVariableName & "_whatsnewDaySelected"
        chkwhatsnewdatenotexact.Attributes.Add("onclick", JavascriptVariableName & ".Goto(1);")
        chkWhatsNewIncludeModifDate.Attributes.Add("onclick", JavascriptVariableName & ".Goto(1);")
        If Not Page.IsPostBack Then
            chkWhatsNewIncludeModifDate.Checked = WhatsNewIncludeModifDate
        End If

    End Sub

    ' Public Property QuickUploadCategory As Int32

    Private Sub InitQuickUpload()
        If Not Layout.EnableQuickUpload Then
            Exit Sub
        End If
        Dim loprofile As UserProfile = UserProfile
        If Not Page.IsPostBack Then
            cmbQuickUploadCat.Category = loprofile.GetParam("QuickUploadCat", 0)
        End If

        pnlQuickUpload.Visible = True
        quickupload.ParentId = CurrentFolder.ID
        '  quickupload.CategoryId = QuickUploadCategory
        quickupload.Width = Unit.Parse(GridGrouping.DefaultGroupingWidth)
        cmbQuickUploadCat.ParentID = CurrentFolder.ID
        cmbQuickUploadCat.Filter = ContentParams.GridLimitedToCategory
        cmbQuickUploadCat.Width = New Unit(quickupload.Width.Value - 10, quickupload.Width.Type)
        quickupload.CategoryId = cmbQuickUploadCat.Category

        If Page.IsPostBack Then
            'If cmbQuickUploadCat.Visible Then
            If cmbQuickUploadCat.Enabled Then
                loprofile.SaveParam("QuickUploadCat", cmbQuickUploadCat.Category)
            End If
            quickupload.PerformUploads()
            'End If
        End If

    End Sub

    Private _showList As Boolean = True

    Private _resultListCrit As DM_OBJECTSearch.Criteria

    Public Overloads Sub DataBind(ByVal loResultListCrit As DM_OBJECTSearch.Criteria)
        DataBind(loResultListCrit, False)
    End Sub

    Public Overloads Sub DataBind(ByVal loResultListCrit As DM_OBJECTSearch.Criteria, ByVal vbForce As Boolean)

        If Not vbForce AndAlso AjaxEnabled AndAlso ScriptManager.GetCurrent(Page).IsInAsyncPostBack Then
            Exit Sub
        End If

        _defaultresultsPerPage = ArcoInfoSettings.DefaultRecordsPerPage

        If String.IsNullOrEmpty(maxresults.Value) Then
            _defaultMaxResults = ArcoInfoSettings.MaxResults
        Else
            _defaultMaxResults = Convert.ToInt32(maxresults.Value)
        End If

        If Not String.IsNullOrEmpty(UserProfile.RecordsPerPage) Then
            _defaultresultsPerPage = Convert.ToInt32(UserProfile.RecordsPerPage)
        End If
        If _defaultresultsPerPage <= 0 Then
            _defaultresultsPerPage = 10
        End If




        Dim llFirstRec As Int32
        Dim llLastRec As Int32
        Dim lsPage As String
        Dim lbError As Boolean = False
        Dim lsErrorMsg As String = ""
        Dim llResultCount As Int32 = 0

        LoadResultScreenInfo(loResultListCrit) 'for template and redirect url
        If _gridlayout Is Nothing AndAlso _customResultScreen Is Nothing Then 'wil we still come here??            
            LoadGrid(msGridXMLFileProp, False)
        End If

        BuildColumnsList(loResultListCrit)


        CheckLoadOptions()

        _showList = Layout.AutoLoad OrElse Page.IsPostBack

        lsPage = CurrentPage.ToString()

        If Layout.RecordsPerPage > 0 Then RecordsPerPage = Layout.RecordsPerPage

        If ShowScroller Then
            llFirstRec = ((CInt(lsPage) - 1) * RecordsPerPage) + 1
            llLastRec = llFirstRec + RecordsPerPage - 1
        Else
            lsPage = "1"
            CurrentPage = 1
            llFirstRec = ((CInt(lsPage) - 1) * RecordsPerPage) + 1
            RecordsPerPage = _defaultMaxResults
            llLastRec = _defaultMaxResults
        End If
        _parentID = loResultListCrit.DM_PARENT_ID
        InitCalendar()
        InitQuickUpload()


        If Not String.IsNullOrEmpty(Layout.CategoryFilter) Then
            loResultListCrit.CategoryList = Layout.CategoryFilter
        End If
        If Not String.IsNullOrEmpty(Layout.ProcedureFilter) Then
            loResultListCrit.CaseSearch.ProcedureList = Layout.ProcedureFilter
        End If

        'do this BEFORE the filter
        If AjaxEnabled Then
            critfield.Value = StringEncryptionService.Encrypt(loResultListCrit.Serialize)

            'ViewState("Crit") = loResultListCrit.Serialize 'serialize before the filter crit are applied
        End If

        'analyze the crit to send to the grid (must be before contentparams is init)
        AnalyzeSearchCriteria(loResultListCrit)

        ContentParams.ResultType = loResultListCrit.ResultType

        loResultListCrit.ExpandResults = True
        loResultListCrit.ExpandFiles = ExpandFileRows
        loResultListCrit.ExpandProperties = False

        'force acl check always in this control
        loResultListCrit.ACLCheck = True

        If AllowChangeInCriteria Then
            AddFiltersToCriteria(loResultListCrit)
            AddOrderByToCriteria(loResultListCrit)
        End If

        AddPagingToCriteria(loResultListCrit, llFirstRec, llLastRec)

        If Not loResultListCrit.ForceJoinOnDMObject Then
            loResultListCrit.ForceJoinOnDMObject = mbForceDMObjectJoin
        End If
        If loResultListCrit.ForceJoinOnPropertyStore = 0 Then
            loResultListCrit.ForceJoinOnPropertyStore = miForceJoinOnProperyStore
        End If

        If loResultListCrit.ResultType = DM_OBJECTSearch.eResultType.ArchivedObjectVersions OrElse loResultListCrit.ResultType = DM_OBJECTSearch.eResultType.ObjectsAndArchivedObjectVersions Then
            AlwaysShowPreviousFileversion = True
            If Not Page.IsPostBack Then
                filelisttogglemode.Value = "Y"
            End If
        End If
        Dim i As Int32 = 0
        If Int32.TryParse(showfileversionsfor.Value, i) Then
            ShowPreviousFileVersionsFor = i
        End If


        Try
            If _showList Then
                Dim lsQuery As String = ""

                If ArcoInfoSettings.PrefetchCount Then

                    llResultCount = DM_OBJECTSearch.GetObjectListCount(loResultListCrit, lsQuery)

                    While llResultCount > 0 AndAlso llFirstRec > 1 AndAlso llResultCount < llFirstRec
                        llFirstRec = llFirstRec - RecordsPerPage
                        llLastRec = llLastRec - RecordsPerPage
                        CurrentPage = CurrentPage - 1
                        AddPagingToCriteria(loResultListCrit, llFirstRec, llLastRec)
                    End While

                    If llResultCount <= loResultListCrit.MAXRESULTS Then
                        loResultListCrit.MAXRESULTS = llResultCount + 1
                    Else
                        loResultListCrit.MAXRESULTS = llLastRec + 1
                    End If

                    If llResultCount > 0 Then
                        loResultListCrit.UseDatabasePaging = True 'we can use database paging since we know the count already
                        Results = DM_OBJECTSearch.GetOBJECTList(loResultListCrit)
                        lsQuery = Results.Statement
                        _resultListCrit = Results.SearchedForCriteria
                        _resultListCrit.UseDatabasePaging = False 'in case we reuse it later
                    Else
                        Results = DM_OBJECTSearch.GetPlaceHolderList

                        _resultListCrit = loResultListCrit
                    End If

                Else

                    'make sure we load a full page
                    'If loResultListCrit.MAXRESULTS > RecordsPerPage Then
                    '    While loResultListCrit.MAXRESULTS Mod RecordsPerPage <> 0
                    '        loResultListCrit.MAXRESULTS += 1 '+ or - ?
                    '    End While
                    'End If

                    Results = DM_OBJECTSearch.GetOBJECTList(loResultListCrit)
                    llResultCount = Results.Count

                    While llResultCount > 0 AndAlso llFirstRec > 1 AndAlso llResultCount < llFirstRec

                        'this is the actual number of results since we attempted to load more
                        doclistscroller.ActualNumberOfResults = llResultCount


                        llFirstRec = llFirstRec - RecordsPerPage
                        llLastRec = llLastRec - RecordsPerPage
                        CurrentPage = CurrentPage - 1
                        AddPagingToCriteria(loResultListCrit, llFirstRec, llLastRec)
                        Results = DM_OBJECTSearch.GetOBJECTList(loResultListCrit)
                        llResultCount = Results.Count
                    End While


                    _resultListCrit = Results.SearchedForCriteria
                    lsQuery = Results.Statement
                    If llResultCount = 0 Then
                        Results = DM_OBJECTSearch.GetPlaceHolderList
                    End If
                End If
                If ShowQuery Then
                    lblQuery.Text = "<b>Query : </b>" & lsQuery & "<br>"
                    lblQuery.Visible = True
                    trQuery.Visible = True
                Else
                    lblQuery.Visible = False
                    lblQuery.Text = ""
                    trQuery.Visible = False
                End If
            Else
                _resultListCrit = loResultListCrit
            End If
        Catch ex As Exception
            If ThrowExceptions Then
                Throw
            End If

            Arco.Utils.Logging.LogError("Error in grid databind", ex)
            lbError = True
            lsErrorMsg = "An unexpected error has occured executing the request"

            Results = DM_OBJECTSearch.GetPlaceHolderList
            _resultListCrit = loResultListCrit

        End Try
        If _showList Then
            If Results Is Nothing AndAlso Not lbError Then
                lbError = True
                lsErrorMsg = "An unexpected error has occured executing the request on the application server"
            End If
            If Not lbError AndAlso Not Results.TermsList Is Nothing AndAlso
            (Not String.IsNullOrEmpty(loResultListCrit.FileSearch.FullText) OrElse Not String.IsNullOrEmpty(loResultListCrit.GlobalSearchTerm)) Then 'only highlight fulltext on those searches
                TermsList = Results.TermsList.ToString("FILE_TXT")
            End If
        End If





        'if this was a package grid
        mlLinkToCaseID = _resultListCrit.LinkedToCaseID
        mlLinkToObjectID = _resultListCrit.LinkedToObjectID
        mlLinkToPackID = _resultListCrit.LinkedToPackID

        GridVisible = True
        If Not String.IsNullOrEmpty(GroupBy.Value) Then
            mbShowGrouper = True
        End If
        If _showList Then

            Dim liTotalCount As Int32 = llResultCount ' Results.Count
            If liTotalCount > _resultListCrit.MAXRESULTS AndAlso _resultListCrit.MAXRESULTS > 0 Then
                liTotalCount = _resultListCrit.MAXRESULTS
                doclistscroller.ActualNumberOfResults = llResultCount.ToString
            End If

            maxresults.Value = _resultListCrit.MAXRESULTS
            ItemCount.Value = liTotalCount.ToString()

            If liTotalCount > 0 Then
                NumberOfResults = liTotalCount
                If liTotalCount Mod RecordsPerPage > 0 Then
                    LastPage = (liTotalCount \ RecordsPerPage) + 1
                Else
                    LastPage = (liTotalCount \ RecordsPerPage)
                End If
            Else
                NumberOfResults = 0
                CurrentPage = 1
                LastPage = 1
            End If

            _contentParams.Results = Results
            If liTotalCount > 0 Then
                _contentParams.ResultType = Results.ResultType
            Else
                _contentParams.ResultType = loResultListCrit.ResultType
            End If

            CacheMailFiles()
            If Layout.GridMode = GridLayout.eGridMode.Grid Then
                docList.DataSource = Results ' loPagedDataSource                    
                docList.DataBind()

            ElseIf Layout.GridMode = GridLayout.eGridMode.ImageList Then
                imageList.DataSource = Results ' loPagedDataSource
                imageList.DataBind()
            End If

            If AllowChangeInCriteria Then
                'renalayze, in case the grid has filtered (for the javascript)
                AnalyzeSearchCriteria(loResultListCrit)

            End If

            BindScroller(loResultListCrit.MAXRESULTS)

            RenderSideBar()

            RenderToolbar()

            If ListRows.Length > 0 Then
                ListRows.Remove(ListRows.Length - 1, 1)
            Else
                ListRows.Append("''")
            End If
        Else
            'list not shown
            RenderSideBar()
        End If

        If lbError Then
            trQuery.Visible = True
            lblQuery.Text = "<div class='ErrorLabel'>" & Server.HtmlEncode(lsErrorMsg) & "</div>"
            lblQuery.Visible = True
        End If

    End Sub

    Public Property ThrowExceptions As Boolean

    Private Sub AnalyzeSearchCriteria(ByVal crit As DM_OBJECTSearch.Criteria)
        Dim criteriaAnalyzer As New CriteriaAnalyzer(crit)
        ContentParams.GridLimitedToCategory.AddRange(criteriaAnalyzer.GetCategories)
        Dim procId As Int32 = criteriaAnalyzer.GetProcedure()
        If procId <> 0 Then
            ContentParams.GridLimitedToProcedure.Add(procId)
        End If

        ContentParams.GridLimitedToStep = criteriaAnalyzer.GetStep

    End Sub

    Private Sub CacheMailFiles()

        If Not Results Is Nothing Then
            Dim laMails As New List(Of Int32)
            For Each lores As DM_OBJECTSearch.OBJECTInfo In Results
                If Not IsPlaceHolderObject(lores) Then
                    If lores.Mail_Id <> 0 Then
                        laMails.Add(lores.Mail_Id)
                    Else
                        If Not lores.Files Is Nothing Then
                            For Each loFile As DM_OBJECTSearch.OBJECTInfo.Object_File_Info In lores.Files
                                If loFile.ReferenceID > 0 AndAlso (loFile.isMail OrElse loFile.Extension = "msg" OrElse loFile.Extension = "eml") Then
                                    If Not laMails.Contains(loFile.ReferenceID) Then laMails.Add(loFile.ReferenceID)
                                End If
                            Next
                        End If

                    End If
                End If
            Next
            If laMails.Count > 0 Then
                mcolMailFiles = Mail.MailFileList.GetAttachments(String.Join(",", laMails.ToArray))
            End If

        End If

    End Sub

    Private Sub SetRadToolBarImage(ByRef radToolbarItem As RadToolBarItem, ByVal toolbarItem As GridToolbar.ToolbarItem)
        If Not String.IsNullOrEmpty(toolbarItem.ImageClass) Then
            radToolbarItem.EnableImageSprite = True
            radToolbarItem.SpriteCssClass = toolbarItem.ImageClass
        Else
            radToolbarItem.ImageUrl = GetToolBarItemImageUrl(toolbarItem)
        End If
    End Sub

    Private Function GetToolBarItemImageUrl(ByVal toolbarItem As GridToolbar.ToolbarItem) As String
        If Not String.IsNullOrEmpty(toolbarItem.ImageUrl) Then
            Dim imgFile As String = toolbarItem.ImageUrl
            If toolbarItem.ImageUrl.StartsWith("~") Then
                'bw compat 
                Return toolbarItem.ImageUrl
            Else
                Return ThemedImage.GetUrl(imgFile, Page)
            End If

        Else
            Return ""
        End If
    End Function

    Private Sub AddToolbarItem(ByVal toolbarItem As GridToolbar.ToolbarItem, ByVal voTo As RadToolBarItemCollection, ByVal vbBigButtons As Boolean)

        If Not toolbarItem.Visible OrElse Not toolbarItem.UserCanView OrElse Not toolbarItem.ShowInSiteVersion(7) Then
            Return
        End If
        If toolbarItem.RequiredFolderACL <> ACL.ACL_Access.Access_Level.ACL_None AndAlso Not CurrentFolder.HasAccess(toolbarItem.RequiredFolderACL) Then
            Return
        End If


        Dim radToolBarBtn As New RadToolBarButton
        If toolbarItem.IsGroup Then
            Dim grpItem As New RadToolBarDropDown With {
                .Enabled = True,
                .ToolTip = toolbarItem.GetLabel()
            }
            '    .ImageUrl = GetToolBarItemImageUrl(toolbarItem)
            SetRadToolBarImage(grpItem, toolbarItem)

            If toolbarItem.ShowText Then
                grpItem.Text = toolbarItem.GetLabel
            End If

            For Each loSubItem As GridToolbar.ToolbarItem In toolbarItem.SubItems
                AddToolbarItem(loSubItem, grpItem.Buttons, False)
            Next
            voTo.Add(grpItem)
        Else
            'radToolBarBtn.Group = "Unity" 'group-prop determines which radtoolbarbuttons collapse in a dropdown when toolbar is too small
            If Not toolbarItem.IsSeparator Then
                If Not toolbarItem.IsCustomActionPlaceHolder Then
                    Dim lbAddItem As Boolean = True

                    radToolBarBtn.NavigateUrl = toolbarItem.NavigateUrl
                    radToolBarBtn.ToolTip = toolbarItem.GetLabel()
                    If toolbarItem.ShowText Then
                        radToolBarBtn.Text = toolbarItem.GetLabel
                        radToolBarBtn.Style.Add("white-space", "nowrap")
                    End If
                    If vbBigButtons Then
                        ' loItem.ImagePosition = ToolBarImagePosition.AboveText
                    End If

                    'check if imageclass or imageurl
                    SetRadToolBarImage(radToolBarBtn, toolbarItem)

                    radToolBarBtn.Enabled = Not toolbarItem.ToggleOnMultiSelect AndAlso Not toolbarItem.ToggleOnCaseMultiSelect AndAlso Not toolbarItem.ToggleOnMailMultiSelect
                    If Not radToolBarBtn.Enabled AndAlso Not _hasMultiSelect Then
                        lbAddItem = False
                    End If

                    If toolbarItem.ToggleOnMultiSelect Then
                        radToolBarBtn.Attributes.Add("MS", "1")
                    End If
                    If toolbarItem.ToggleOnCaseMultiSelect Then
                        radToolBarBtn.Attributes.Add("MSC", "1")
                    End If
                    If toolbarItem.ToggleOnMailMultiSelect Then
                        radToolBarBtn.Attributes.Add("MSM", "1")
                    End If
                    radToolBarBtn.Value = toolbarItem.NavigateUrl.GetHashCode().ToString()

                    Dim navigateUrl As String = radToolBarBtn.NavigateUrl.ToLower
                    If navigateUrl.Contains("togglewhatsnew") Then
                        radToolBarBtn.CheckOnClick = True
                        radToolBarBtn.AllowSelfUnCheck = True
                        radToolBarBtn.Group = "togglewhatsnew"
                        If ShowWhatsNew.Value = "Y" Then
                            radToolBarBtn.Checked = True
                        Else
                            radToolBarBtn.Checked = False
                        End If
                    ElseIf navigateUrl.Contains("toggleshowsuspended") Then
                        radToolBarBtn.CheckOnClick = True
                        radToolBarBtn.AllowSelfUnCheck = True
                        radToolBarBtn.Group = "toggleshowsuspended"
                        If UserProfile.ShowSuspendedDossiers Then
                            radToolBarBtn.Checked = True
                        Else
                            radToolBarBtn.Checked = False
                        End If
                    ElseIf navigateUrl.Contains("toggleshowlocked") Then
                        radToolBarBtn.CheckOnClick = True
                        radToolBarBtn.AllowSelfUnCheck = True
                        radToolBarBtn.Group = "toggleshowlocked"
                        'If txtShowLocked.Value = "Y" Then
                        If UserProfile.ShowLockedDossiers Then
                            radToolBarBtn.Checked = True
                        Else
                            radToolBarBtn.Checked = False
                        End If
                    ElseIf navigateUrl.Contains("toggleshowsidebar") Then
                        If _hasSideBar Then
                            radToolBarBtn.CheckOnClick = True
                            radToolBarBtn.AllowSelfUnCheck = True
                            radToolBarBtn.Group = "toggleshowsidebar"
                            'If txtShowLocked.Value = "Y" Then
                            If UserProfile.ShowGridSideBar Then
                                radToolBarBtn.Checked = True
                            Else
                                radToolBarBtn.Checked = False
                            End If
                        Else
                            lbAddItem = False
                        End If
                    ElseIf navigateUrl.Contains("togglehidedelegated") Then
                        lbAddItem = False
                        If BusinessIdentity.CurrentIdentity.ActiveWorkExceptions.Any Then

                            Dim laAdded As New HashSet(Of String)
                            laAdded.Add(BusinessIdentity.CurrentIdentity.Name)

                            Dim drpItem As New RadToolBarDropDown
                            drpItem.Enabled = True

                            drpItem.ToolTip = toolbarItem.GetLabel()
                            If toolbarItem.ShowText Then
                                drpItem.Text = toolbarItem.GetLabel
                                drpItem.Style.Add("white-space", "nowrap")
                            End If
                            SetRadToolBarImage(drpItem, toolbarItem)
                            'loDrpItem.ImageUrl = GetToolBarItemImageUrl(toolbarItem)

                            drpItem.Buttons.Add(New RadToolBarButton() With {.NavigateUrl = "javascript:#GRID#.SelectDelegate(" & EncodingUtils.EncodeJsString("") & ");", .Text = GetToolbarbuttonLabel(GetLabel("all"), (String.IsNullOrEmpty(FilterOnDelegate.Value)))})
                            drpItem.Buttons.Add(New RadToolBarButton() With {.NavigateUrl = "javascript:#GRID#.SelectDelegate(" & EncodingUtils.EncodeJsString(BusinessIdentity.CurrentIdentity.Name) & ");", .Text = GetToolbarbuttonLabel(GetLabel("mywork"), (FilterOnDelegate.Value = BusinessIdentity.CurrentIdentity.Name))})

                            For Each loEx As Arco.Doma.Library.Routing.WorkExceptionList.WorkExceptionInfo In BusinessIdentity.CurrentIdentity.ActiveWorkExceptions
                                If Not laAdded.Contains(loEx.DelegateFrom_ID) Then
                                    Dim loExSubItem As New RadToolBarButton
                                    loExSubItem.NavigateUrl = "javascript:#GRID#.SelectDelegate(" & EncodingUtils.EncodeJsString(loEx.DelegateFrom_ID) & ");"
                                    'loSubItem.ToolTip = loEx.DelegateFrom_ID
                                    loExSubItem.ImageUrl = "" 'todo
                                    loExSubItem.Text = GetToolbarbuttonLabel(ArcoFormatting.FormatUserName(loEx.DelegateFrom_ID), (FilterOnDelegate.Value = loEx.DelegateFrom_ID))

                                    drpItem.Buttons.Add(loExSubItem)
                                    laAdded.Add(loEx.DelegateFrom_ID)
                                End If

                            Next
                            voTo.Add(drpItem)
                        End If
                    ElseIf navigateUrl.Contains("addtoselection(3)") Then
                        lbAddItem = Settings.GetValue("Favorites", "Enabled", True)
                    ElseIf navigateUrl.Contains("addtoselection(2)") Then
                        lbAddItem = Settings.GetValue("Basket", "Enabled", True)
                    ElseIf navigateUrl.Contains("dodocumentlistaction(7,0)") OrElse navigateUrl.Contains("dodocumentlistaction(8,0)") OrElse navigateUrl.Contains("dodocumentlistaction(9,0)") Then
                        lbAddItem = Settings.GetValue("Versioning", "EnableDocumentVersioning", True)
                    End If

                    If lbAddItem Then
                        voTo.Add(radToolBarBtn)
                    End If
                Else
                    AddCustomActionsToToolbar(toolbarItem, voTo)
                End If
            Else
                radToolBarBtn.IsSeparator = True
                voTo.Add(radToolBarBtn)
            End If
        End If

    End Sub

    Private Sub AddCustomActionsToToolbar(ByVal toolBarItem As GridToolbar.ToolbarItem, ByVal radToolBarCollection As RadToolBarItemCollection)
        Dim lcolCustomActions As DMCustomActionList
        Select Case ResultType
            Case DM_OBJECTSearch.eResultType.CaseList, DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                lcolCustomActions = DMCustomActionList.GetCustomActionList("CaseSelection,Selection")
            Case DM_OBJECTSearch.eResultType.Mails
                lcolCustomActions = DMCustomActionList.GetCustomActionList("MailSelection")
            Case Else
                lcolCustomActions = DMCustomActionList.GetCustomActionList("Selection")
        End Select

        If lcolCustomActions.Any Then
            Dim lbUseDropDown As Boolean = (lcolCustomActions.Count > 1)
            Dim drpDwnItem As New RadToolBarDropDown
            drpDwnItem.Enabled = True

            If toolBarItem.ShowText Then
                drpDwnItem.Text = toolBarItem.GetLabel
            End If
            SetRadToolBarImage(drpDwnItem, toolBarItem)
            '(loDrpItem.ImageUrl = GetToolBarItemImageUrl(loToolbarItem)

            For Each loCustActionInfo As DMCustomActionList.CustomActionInfo In lcolCustomActions
                Dim radToolBarBtn As New RadToolBarButton
                radToolBarBtn.NavigateUrl = "javascript:#GRID#.DoDocumentListAction(1," & loCustActionInfo.ID & ");"
                radToolBarBtn.ToolTip = loCustActionInfo.Label(Nothing, IUserEvent.eCallerLocation.ListScreen)
                radToolBarBtn.ImageUrl = "" 'todo
                radToolBarBtn.Text = loCustActionInfo.Label(Nothing, IUserEvent.eCallerLocation.ListScreen)
                radToolBarBtn.Enabled = False
                Select Case loCustActionInfo.Type
                    Case "MailSelection"
                        radToolBarBtn.Attributes.Add("MSM", "1") 'toggle on mail select
                    Case "CaseSelection"
                        radToolBarBtn.Attributes.Add("MSC", "1") 'toggle on case select
                    Case Else
                        radToolBarBtn.Attributes.Add("MS", "1") 'toggle on select
                End Select

                radToolBarBtn.Value = radToolBarBtn.NavigateUrl.GetHashCode().ToString()
                If lbUseDropDown Then
                    drpDwnItem.Buttons.Add(radToolBarBtn)
                Else
                    SetRadToolBarImage(radToolBarBtn, toolBarItem)
                    'loSubItem.ImageUrl = GetToolBarItemImageUrl(toolBarItem)
                    radToolBarCollection.Add(radToolBarBtn)
                End If
            Next
            If lbUseDropDown Then
                radToolBarCollection.Add(drpDwnItem)
            End If
        End If
    End Sub

    Private Function GetToolbarbuttonLabel(ByVal vsText As String, ByVal vbSelected As Boolean) As String
        If vbSelected Then
            Return "<b>" & vsText & "</b>"
        Else
            Return vsText
        End If
    End Function

    Private Sub RenderToolbar()


        If ShowToolbar AndAlso (Toolbar.ToolbarEnabled OrElse CustomToolbarButtons.Any) Then
            pnlToolbar.Visible = True

            If Toolbar.FullWidth AndAlso String.IsNullOrEmpty(GridHeader) Then
                'If _hasSideBar Then
                '    plhGridHeaderStart.Controls.Add(New LiteralControl("<div class='" & ListCssClass & "Header3RightFillerDiv' ><div class='" & ListCssClass & "Header3LeftSpacerDiv'></div><div class='" & ListCssClass & "MainHeader'>"))
                'Else
                '    plhGridHeaderStart.Controls.Add(New LiteralControl("<div class='" & ListCssClass & "Header3RightFillerDiv' ><div class='" & ListCssClass & "MainHeader'>"))
                'End If
                'plhGridHeaderStartEnd.Controls.Add(New LiteralControl("</div>"))

                tlbGrid.Style.Add("width", "100%")

                '  plhGridHeaderEnd.Controls.Add(New LiteralControl("</div>"))
            Else
                If Not ArcoFormatting.FullWidthPanelLabels Then
                    plhGridHeaderStart.Controls.Add(New LiteralControl("<div class='SubListHeader3RightFillerDiv' ><div style='float:right; order:1;'><div class='SubListHeader3LeftSpacerDiv'></div><div class='" & CssClass & "MainHeader'>"))
                    plhGridHeaderStartEnd.Controls.Add(New LiteralControl("</div></div>"))

                    plhGridHeader.Controls.Add(New LiteralControl("<div class='SubListHeader3LeftSpacerDiv'></div><div class='SubListMainHeader'><div style='padding:5px'>" & GridHeader & "</div></div>"))


                    plhGridHeaderEnd.Controls.Add(New LiteralControl("<div class='SubListHeader3RightSpacerDiv'></div></div>"))
                Else
                    plhGridHeaderStart.Controls.Add(New LiteralControl("<div class='SubListHeader3RightFullFillerDiv' ><div style='float:right; order:1'><div class='SubListMainHeader'>"))
                    plhGridHeaderStartEnd.Controls.Add(New LiteralControl("</div></div>"))
                    plhGridHeader.Controls.Add(New LiteralControl("<div class='SubListMainHeader'><div style='padding:5px'>" & GridHeader & "</div></div>"))

                    plhGridHeaderEnd.Controls.Add(New LiteralControl("</div>"))
                End If



            End If
            Const bigbuttons As Boolean = True
            'If lbBigButtons Then
            '    tlbGrid.Height = Unit.Pixel(54)
            'End If               
            If Not CustomToolbarButtons Is Nothing Then
                For Each tlbrItem As GridToolbar.ToolbarItem In CustomToolbarButtons
                    AddToolbarItem(tlbrItem, tlbGrid.Items, bigbuttons)
                Next
                If CustomToolbarButtons.Any AndAlso Toolbar.ToolbarItems.Any Then
                    AddToolbarItem(New GridToolbar.ToolbarItem With {.IsSeparator = True}, tlbGrid.Items, bigbuttons)
                End If
            End If

            For Each loToolbarItem As GridToolbar.ToolbarItem In Toolbar.ToolbarItems
                AddToolbarItem(loToolbarItem, tlbGrid.Items, bigbuttons)
            Next

        ElseIf Not String.IsNullOrEmpty(GridHeader) Then
            'just show the header
            pnlToolbar.Visible = True
            tlbGrid.Visible = False

            If Not ArcoFormatting.FullWidthPanelLabels Then
                plhGridHeaderStart.Controls.Add(New LiteralControl("<div class='SubListHeader3RightFillerDiv' ><div class='SubListHeader3LeftSpacerDiv'></div>"))
                'plhGridHeaderStartEnd.Controls.Add(New LiteralControl("</div>"))
                plhGridHeader.Controls.Add(New LiteralControl("<div  class='SubListMainHeader'><div style='padding:5px'>" & GridHeader & "</div></div>"))
                plhGridHeaderEnd.Controls.Add(New LiteralControl("<div class='SubListHeader3RightSpacerDiv'></div></div>"))
            Else
                plhGridHeaderStart.Controls.Add(New LiteralControl("<div class='SubListHeader3RightFullFillerDiv' style='height:22px'>"))
                'plhGridHeaderStartEnd.Controls.Add(New LiteralControl("</div>"))
                plhGridHeader.Controls.Add(New LiteralControl("<div  class='SubListMainHeader' style='height:inherit'><div style='padding:5px'>" & GridHeader & "</div></div>"))
                plhGridHeaderEnd.Controls.Add(New LiteralControl("</div>"))
            End If

            'pnlNoToolbar.Visible = False
        Else
            pnlToolbar.Visible = False
            ' pnlNoToolbar.Visible = True
        End If
    End Sub

    Private Sub BindScroller(ByVal maxResults As Integer)
        If ShowScroller AndAlso ShowPager Then
            doclistscroller.GridMode = GridMode
            doclistscroller.ShowBackToSearchLink = ShowBackToSearchLink AndAlso Layout.ShowBackToSearch
            doclistscroller.GridClientID = JavascriptVariableName
            doclistscroller.CurrentPage = CurrentPage
            doclistscroller.LastPage = LastPage
            doclistscroller.NumberOfResults = NumberOfResults
            doclistscroller.MaxResults = Math.Max(maxResults, _defaultMaxResults)
            doclistscroller.Visible = True
            doclistscroller.Align = Layout.ScrollerAlign
            doclistscroller.DataBind()
        Else
            doclistscroller.Visible = False
        End If
    End Sub

    Private Function GetHeaderCell(ByVal item As Control, ByVal voProvider As BaseContentProvider, ByVal currentOrderByField As String, ByVal currentOrderByOrder As String) As String

        InitHeaderContextMenu(item)

        Return voProvider.RenderHeaderCell(Layout.AllowOrderBy AndAlso Mode <> DocroomListHelpers.GridMode.Print, currentOrderByField, currentOrderByOrder)

    End Function

    Private Sub InitHeaderContextMenu(ByVal item As Control)
        If Not mbMenuLoaded AndAlso Layout.HeaderContextMenus AndAlso Mode <> DocroomListHelpers.GridMode.Print Then

            mbMenuLoaded = True
            HeaderContextMenu = New RadContextMenu
            HeaderContextMenu.EnableAutoScroll = True
            HeaderContextMenu.ID = "mnuhrdcell_"

            Dim rootFilterMnuAdded As Boolean = False
            Dim filterMenuItem As New RadMenuItem(GetLabel("filter"))
            If Not Page.IsPostBack OrElse String.IsNullOrEmpty(internalsubmit.Value) Then
                DisabledFilters.Value = ""
            End If
            For Each gridFilter As GridFilters.Filter In GridFilters.Filters
                Dim applies As Boolean = True
                Dim enabled As Boolean = gridFilter.Enabled

                Dim name As String = gridFilter.GetLabel(False)

                If Not String.IsNullOrEmpty(gridFilter.Role) Then
                    applies = BusinessIdentity.CurrentIdentity.IsInRole(gridFilter.Role)
                    If Not applies Then
                        If gridFilter.reversefilterwhennotinrole Then
                            enabled = Not enabled
                            applies = True
                        End If
                    End If
                End If
                If gridFilter.Visible AndAlso applies Then 'add filter control
                    If Not rootFilterMnuAdded Then
                        HeaderContextMenu.Items.Add(filterMenuItem)
                        rootFilterMnuAdded = True
                    End If
                    If Page.IsPostBack AndAlso internalsubmit.Value = "Y" Then
                        enabled = Not DisabledFilters.Value.Contains(";" & gridFilter.Index & ";")
                    Else
                        If Not enabled Then
                            DisabledFilters.Value = DisabledFilters.Value & ";" & gridFilter.Index & ";"
                        End If
                    End If

                    Dim link As String
                    If Not enabled Then
                        link = "javascript:" & JavascriptVariableName & ".EnableFilter('" & gridFilter.Index & "')"
                    Else
                        link = "javascript:" & JavascriptVariableName & ".DisableFilter('" & gridFilter.Index & "')"
                    End If

                    Dim menuItem As New RadMenuItem(name, link)
                    If enabled Then
                        menuItem.ImageUrl = ThemedImage.GetUrl("checkbox_checked.svg", Page)
                    End If
                    filterMenuItem.Items.Add(menuItem)
                End If
            Next

            If GroupingEnabled Then
                Dim lbHasUserGrouper As Boolean = False
                Dim menuItemGroup As New RadMenuItem(GetLabel("groupby"))

                For Each gridGrouper As GridGroupingInfo.Grouper In GridGrouping.GrouperList
                    If gridGrouper.UserGrouper Then
                        lbHasUserGrouper = True
                        Dim url As String = "javascript:"
                        Dim menuItem As New RadMenuItem
                        menuItem.Text = gridGrouper.GetLabel()
                        If String.IsNullOrEmpty(menuItem.Text) Then
                            menuItem.Text = GetLabel("grouping") & " " & gridGrouper.ID
                        End If
                        Try
                            For Each loField As GridGroupingInfo.Grouper.GrouperField In gridGrouper.Fields

                                If GroupBy.Value.IndexOf(gridGrouper.ID & ";;", StringComparison.Ordinal) < 0 Then
                                    url &= JavascriptVariableName & ".AddGrouper(" & gridGrouper.ID & ",'" & loField.GetFieldNameAndOrderBy(ContentParams) & "');"
                                Else
                                    menuItem.ImageUrl = ThemedImage.GetUrl("checkbox_checked.svg", Page)
                                    url &= JavascriptVariableName & ".RemoveGrouper(" & gridGrouper.ID & ",'" & loField.GetFieldNameAndOrderBy(ContentParams) & "');"
                                End If

                            Next
                            url &= JavascriptVariableName & ".Goto(1);"
                            menuItem.NavigateUrl = url
                            menuItemGroup.Items.Add(menuItem)
                        Catch pex As Exceptions.PropertyNotFoundException

                        End Try
                    End If
                Next
                If lbHasUserGrouper Then
                    HeaderContextMenu.Items.Add(menuItemGroup)
                End If
            End If

            Dim visibleColCount As Int32 = 0
            Dim nonFixedColCount As Int32 = 0
            For Each loCol As GridLayout.ColumnDefinition In AllFirstRowColumns.Where(Function(x) Not x.Fixed)
                If Not ColumnIsHidden(loCol.UniqueName) Then visibleColCount += 1
                nonFixedColCount += 1
            Next
            If nonFixedColCount > 0 Then
                Dim colItem As New RadMenuItem(GetLabel("selectcolumns"))

                Dim resetColsItem As New RadMenuItem(GetLabel("resetgridcols"))
                resetColsItem.NavigateUrl = "javascript:" & JavascriptVariableName & ".ResetColumns();"
                colItem.Items.Add(resetColsItem)
                colItem.Items.Add(New RadMenuItem() With {.IsSeparator = True})

                'can 't hide the single column
                Dim canHideColumns As Boolean = visibleColCount > 1

                For Each col As GridLayout.ColumnDefinition In AllFirstRowColumns.Where(Function(x) Not x.Fixed)
                    colItem.Items.Add(GetShowHideColumnMenuItem(col, canHideColumns))
                Next

                HeaderContextMenu.Items.Add(colItem)
            End If

            Dim loResetSortItem As New RadMenuItem(GetLabel("resetgridsort"))
            loResetSortItem.NavigateUrl = "javascript:" & JavascriptVariableName & ".ResetOrderby();"
            HeaderContextMenu.Items.Add(loResetSortItem)


            If BusinessIdentity.CurrentIdentity.isAdmin Then
                HeaderContextMenu.Items.Add(New RadMenuItem() With {.IsSeparator = True})

                Dim editGridItem As New RadMenuItem("Grid info")
                editGridItem.NavigateUrl = "javascript:alert(" & EncodingUtils.EncodeJsString(Layout.FullFilePath) & ");"
                HeaderContextMenu.Items.Add(editGridItem)
            End If

            Dim plhMenu As PlaceHolder = CType(item.FindControl("plhMnu"), PlaceHolder)
            plhMenu.Controls.Add(HeaderContextMenu)

        End If
    End Sub

    Private Function GetShowHideColumnMenuItem(loCol As GridLayout.ColumnDefinition, ByVal canHide As Boolean) As RadMenuItem
        Dim lbShown As Boolean = Not ColumnIsHidden(loCol.UniqueName)
        Dim item As New RadMenuItem(loCol.GetLabel(False))

        If lbShown Then
            item.ImageUrl = ThemedImage.GetUrl("checkbox_checked.svg", Page)
            If canHide Then
                item.NavigateUrl = "javascript:" & JavascriptVariableName & ".HideCollumn('" & loCol.UniqueName & "');"
            End If
        Else
            item.NavigateUrl = "javascript:" & JavascriptVariableName & ".ShowCollumn('" & loCol.UniqueName & "');"
        End If
        Return item
    End Function

    Protected Sub ProcessDefaultImageListHeader(ByVal item As RepeaterItem)

        InitHeaderContextMenu(item)

        Dim objLit As New Literal

        objLit.Text = "<table width='100%'><tr class='" & CssClass & "Header' oncontextmenu='return " & JavascriptVariableName & ".HeaderContextMenu();' ><td></td></tr></table>"

        If Not item.FindControl("plhHeader") Is Nothing Then item.FindControl("plhHeader").Controls.Add(objLit)

    End Sub

    Protected Sub ProcessDefaultHeader(ByVal item As RepeaterItem)

        Dim loColHeader As GridLayout.ColumnDefinition
        Dim header As New Literal
        Dim filters As New Literal
        Dim lbRenderFilters As Boolean = (Not item.FindControl("plhFilter") Is Nothing AndAlso ShowFilters AndAlso UserProfile.ShowFilters AndAlso Layout.ShowFilters AndAlso Mode <> DocroomListHelpers.GridMode.Print)


        Dim currentOrderByField As String = ""
        Dim currentOrderByOrder As String = ""

        If Page.IsPostBack Then
            currentOrderByField = OrderbyField.Value
            currentOrderByOrder = OrderbyOrderField.Value
        Else
            Dim orderBy As String = Layout.GetOrderBy
            If Not String.IsNullOrEmpty(orderBy) AndAlso orderBy.Contains(" ") Then
                Dim parts() As String = orderBy.Split(" "c)
                currentOrderByField = parts(0)
                currentOrderByOrder = parts(1)
            End If
        End If

        For Each loColHeader In Columns

            Dim loContentProvider As BaseContentProvider = loColHeader.GetContentProvider(Page, ContentParams, Mode)

            If loContentProvider Is Nothing Then Continue For

            header.Text += GetHeaderCell(item, loContentProvider, currentOrderByField, currentOrderByOrder)

            If lbRenderFilters Then
                If loColHeader.ShowFilter Then

                    filters.Text += "<td>" & loContentProvider.RenderFilter() & "</td>"
                Else
                    filters.Text += "<td>&nbsp;</td>"
                End If
            End If

        Next
        header.Text = "<tr class='" & CssClass & "Header' oncontextmenu='return " & JavascriptVariableName & ".HeaderContextMenu();' >" & header.Text & "</tr>"


        If Not item.FindControl("plhHeader") Is Nothing Then item.FindControl("plhHeader").Controls.Add(header)

        If Not item.FindControl("plhFilter") Is Nothing Then
            If lbRenderFilters Then
                filters.Text = "<tr class='" & CssClass & "Filter' id='" & ClientID & "_FilterRow' oncontextmenu='return " & JavascriptVariableName & ".HeaderContextMenu();'>" & filters.Text & "</tr>"
                item.FindControl("plhFilter").Controls.Add(filters)
                item.FindControl("plhFilter").Visible = True
            Else
                item.FindControl("plhFilter").Visible = False
            End If
        End If

    End Sub

    Private Function GetObjectTypeAttribute(ByVal obj As DM_OBJECTSearch.OBJECTInfo) As String
        If obj.OBJECT_TYPE = "Folder" AndAlso obj.CaseID > 0 Then
            Return "FolderInCase"
        Else
            Return obj.OBJECT_TYPE
        End If

    End Function

    Protected Sub ProcessTemplateItem(ByRef obj As DM_OBJECTSearch.OBJECTInfo, ByVal template As XMLTemplate.TemplateItem, ByVal objPlaceHolder As PlaceHolder, ByVal itemtype As ListItemType)

        Const delimStr As String = "[]"
        Dim delimiter As Char() = delimStr.ToCharArray()
        Dim tokens As String() = template.Content.Split(delimiter)
        ' Dim lbIsShortCut As Boolean = False
        Dim lbProceed As Boolean
        Dim refobj As DM_OBJECTSearch.OBJECTInfo = Nothing
        lbProceed = ExtractShortCut(obj, refobj)

        If lbProceed = False Then Exit Sub
        Dim rowid As Int32
        Dim lsFromArchive As String = ""
        Select Case Results.ResultType
            Case DM_OBJECTSearch.eResultType.Objects
                rowid = obj.ID
                If obj.FromArchive Then
                    lsFromArchive = "Y"
                End If
            Case DM_OBJECTSearch.eResultType.CaseList
                rowid = obj.TechID
            Case DM_OBJECTSearch.eResultType.Files
                rowid = obj.FILE_ID
            Case DM_OBJECTSearch.eResultType.Mails
                rowid = obj.FILE_ID
            Case DM_OBJECTSearch.eResultType.ArchivedCasesList, DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                rowid = obj.CaseID
            Case Else
                rowid = 0
        End Select

        Dim lsRowStart As String = "<tr class='ListContent' id='" & rowid & "'  object_type='" & GetObjectTypeAttribute(obj) & "' object_reference_type='" & obj.Object_Reference_Type & "' archived='" & lsFromArchive & "' default_action='" & obj.DEFAULT_TREE_ACTION & "'>		"

        Dim objStart As New Literal

        objStart.Text = lsRowStart & "<td colspan=""6"">"
        objPlaceHolder.Controls.Add(objStart)

        For i As Integer = 0 To tokens.Length - 1 Step 2
            Dim objlit As New Literal
            objlit.Text = tokens(i)
            objPlaceHolder.Controls.Add(objlit)
            If i < tokens.Length - 1 Then
                Select Case tokens(i + 1)
                    Case "HEADERONLY"
                        While tokens(i + 1) <> "/HEADERONLY"
                            i += 1
                        End While
                    Case "FILELIST"
                        While tokens(i + 1) <> "/FILELIST"
                            i += 1
                        End While
                    Case "/HEADERONLY", "RESULTONLY", "/RESULTONLY", "/FILELIST"
                        'do nothing
                    Case Else
                        Dim loProvider As BaseContentProvider = ContentProviderfactory.GetTemplateContentProvider(Page, ContentParams, Mode, tokens(i + 1))
                        objlit = New Literal

                        Dim content As CellContent = loProvider.RenderContent(obj, refobj)

                        objlit.Text = content.Value

                        objPlaceHolder.Controls.Add(objlit)

                End Select
            End If
        Next

        Dim objEnd As New Literal
        objEnd.Text = "</td>"
        objPlaceHolder.Controls.Add(objEnd)

    End Sub

    Protected Sub ProcessTemplateHeader(ByVal template As XMLTemplate.TemplateItem, ByVal objPlaceholder As PlaceHolder)

        Const delimStr As String = "[]"
        Dim delimiter As Char() = delimStr.ToCharArray()
        Dim tokens As String() = template.Content.Split(delimiter)

        Dim objStart As New Literal
        objStart.Text = "<tr class='" & CssClass & "Header' oncontextmenu='return " & JavascriptVariableName & ".HeaderContextMenu();' ><td colspan=""6"">"
        objPlaceholder.Controls.Add(objStart)

        For i As Integer = 0 To tokens.Length - 1 Step 2
            Dim objlit As New Literal
            objlit.Text = tokens(i)
            objPlaceholder.Controls.Add(objlit)
            If i < tokens.Length - 1 Then
                Select Case tokens(i + 1)
                    Case "RESULTONLY"
                        While tokens(i + 1) <> "/RESULTONLY"
                            i += 1
                        End While
                    Case "FILELIST"
                        While tokens(i + 1) <> "/FILELIST"
                            i += 1
                        End While
                    Case "/RESULTONLY", "HEADERONLY", "/HEADERONLY", "/FILELIST"
                        'do nothing
                    Case Else 'content
                        Dim loProvider As BaseContentProvider = ContentProviderfactory.GetTemplateContentProvider(Page, ContentParams, Mode, tokens(i + 1))
                        If Not String.IsNullOrEmpty(loProvider.OrderByField) Then
                            objlit = New Literal
                            objlit.Text = "<a href=""javascript:" & JavascriptVariableName & ".OrderBy('" & loProvider.OrderByField & "','" & loProvider.DefaultOrderByDirection & "');"">" & loProvider.DefaultLabel & "</a>"
                            objPlaceholder.Controls.Add(objlit)
                        Else
                            objlit = New Literal
                            objlit.Text = loProvider.DefaultLabel
                            objPlaceholder.Controls.Add(objlit)

                        End If
                End Select

            End If
        Next

        Dim objEnd As New Literal
        objEnd.Text = "</td></tr>"
        objPlaceholder.Controls.Add(objEnd)

        'TO ADD : Filters

    End Sub

    Private Function ExtractShortCut(ByVal obj As DM_OBJECTSearch.OBJECTInfo, ByRef referencedobject As DM_OBJECTSearch.OBJECTInfo) As Boolean

        Dim doprocess As Boolean = True

        If obj.OBJECT_TYPE = "Shortcut" Then
            If obj.Object_Reference > 0 Then
                If obj.ReferencedObject IsNot Nothing Then
                    referencedobject = obj.ReferencedObject
                    If obj.ID > 0 Then
                        ' realid = shortcutid
                    Else
                        'we couldn't load the referenced object, nothing we can do...
                        doprocess = False
                    End If
                Else
                    'we couldn't load the referenced object, no acl or item no longer exists
                    doprocess = False
                End If
            Else
                'shortcut to 0 
                doprocess = False
            End If
        End If

        Return doprocess

    End Function

    Private Function ConvertCriteriaResultType(ByVal voCrit As DM_OBJECTSearch.Criteria) As Screen.ScreenSearchMode
        Select Case voCrit.ResultType
            Case DM_OBJECTSearch.eResultType.Objects
                Return Screen.ScreenSearchMode.Objects
            Case DM_OBJECTSearch.eResultType.Files
                Return Screen.ScreenSearchMode.Files
            Case DM_OBJECTSearch.eResultType.Mails
                If voCrit.MailSearch.MailBoxSubject = BusinessIdentity.CurrentIdentity.Name Then
                    Select Case voCrit.MailSearch.MailBoxType
                        Case Mail.MailBoxItem.MailBox.Inbox
                            If voCrit.MailSearch.UserHasWork Then
                                Return Screen.ScreenSearchMode.MailInboxWork
                            Else
                                Return Screen.ScreenSearchMode.MailInbox
                            End If
                        Case Mail.MailBoxItem.MailBox.Sent
                            If voCrit.MailSearch.JoinMailTracking Then
                                Return Screen.ScreenSearchMode.MailFollowUp
                            Else
                                Return Screen.ScreenSearchMode.MailSentItems
                            End If
                        Case Else ' Mail.MailBoxItem.MailBox.Undefined
                            If voCrit.MailSearch.MailBoxStatus = Mail.MailBoxItem.MailStatus.Open Then
                                Return Screen.ScreenSearchMode.MyMails
                            Else
                                Return Screen.ScreenSearchMode.MailDeletedBox
                            End If
                    End Select
                Else
                    Return Screen.ScreenSearchMode.Mails
                End If

            Case DM_OBJECTSearch.eResultType.ArchivedCasesList
                Return Screen.ScreenSearchMode.ArchivedCases
            Case DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                Return Screen.ScreenSearchMode.OpenAndArchivedCases
            Case DM_OBJECTSearch.eResultType.CaseList
                If voCrit.CaseSearch.FilterOnWork Then
                    Return Screen.ScreenSearchMode.Work
                Else
                    If voCrit.CreatedBy = BusinessIdentity.CurrentIdentity.Name Then
                        Return Screen.ScreenSearchMode.MyCases
                    Else
                        Return Screen.ScreenSearchMode.Cases
                    End If
                End If
            Case Else
                Return Screen.ScreenSearchMode.Objects
        End Select
    End Function

    Private Function ProcessDefaultImageListRow(ByRef obj As DM_OBJECTSearch.OBJECTInfo, ByVal voColumns As List(Of GridLayout.ColumnDefinition), ByRef refobj As DM_OBJECTSearch.OBJECTInfo) As Literal

        Dim objLit As New Literal
        If Not ExtractShortCut(obj, refobj) Then Return objLit

        Dim lbHorizontal As Boolean = Layout.ImageListOrientation = Orientation.Horizontal 'AndAlso False

        objLit.Text = "<article class='action-btn tile imagelist-tile" & If(lbHorizontal, " horizontal", "") & "'>"

        If lbHorizontal Then
            objLit.Text += "<section class='tile-horizontal'>"
        End If
        objLit.Text += "<header class='tile-content tile-header'>"

        Dim loColDef As New GridLayout.ColumnDefinition
        loColDef.Name = "thumbnail"

        Dim loThumbNailProvider As ThumbNailProvider = DirectCast(loColDef.GetContentProvider(Page, ContentParams, Mode), ThumbNailProvider)
        Dim lsRenderedThumbnail As String = loThumbNailProvider.RenderContent(obj, refobj).Value

        If lsRenderedThumbnail.Length > 6 Then
            objLit.Text += "<figure class='tile-thumbnail'>" & lsRenderedThumbnail & "</figure>"
        End If

        Dim rowid As Int32
        Dim lsFromArchive As String = ""
        Select Case Results.ResultType
            Case DM_OBJECTSearch.eResultType.Objects
                rowid = obj.ID
                If obj.FromArchive Then
                    lsFromArchive = "Y"
                End If
            Case DM_OBJECTSearch.eResultType.CaseList
                rowid = obj.TechID
            Case DM_OBJECTSearch.eResultType.Files
                rowid = obj.FILE_ID
            Case DM_OBJECTSearch.eResultType.Mails
                rowid = obj.FILE_ID
            Case DM_OBJECTSearch.eResultType.ArchivedCasesList, DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                rowid = obj.CaseID
            Case Else
                rowid = 0
        End Select

        Dim liWidth As Integer = If(lbHorizontal, 400, 250)

        Dim lsRowStart As String = "<div><table><tbody><tr class='ImageListContent' id='" & rowid & "'  object_type='" & GetObjectTypeAttribute(obj) & "' object_reference_type='" & obj.Object_Reference_Type & "' archived='" & lsFromArchive & "' default_action='" & obj.DEFAULT_TREE_ACTION & "'><td>"
        Dim lsCheckBox As String = ""
        Dim lsPropertyTable As String = "<footer class='tile-content tile-property-table'><table class='List'>"

        For Each loColumn As GridLayout.ColumnDefinition In voColumns
            If Not ColumnIsHidden(loColumn.UniqueName) Then

                Dim loProvider As BaseContentProvider = loColumn.GetContentProvider(Page, ContentParams, Mode)

                Select Case loColumn.UniqueName
                    Case "multiselect"
                        If loProvider Is Nothing Then Exit Select
                        lsCheckBox = "<span class='ImageListCheckBox'>" & loProvider.RenderContent(obj, refobj).Value & "</span>"
                    Case "title"
                        If loProvider Is Nothing Then Exit Select
                        objLit.Text += "<h6 class='tile-title'>" & loProvider.RenderContent(obj, refobj).Value & "</h6>"
                    Case Else

                        lsPropertyTable += "<tr><td class='property-label' nowrap><b>"

                        Dim lsLabel As String = loColumn.GetLabel(False)
                        If Not String.IsNullOrEmpty(lsLabel) Then
                            lsPropertyTable += lsLabel & ":</b></td><td width='100%'>"
                        Else
                            lsPropertyTable += "&nbsp;</td><td width='100%'>"
                        End If

                        If loProvider IsNot Nothing Then
                            Dim content As CellContent = loProvider.RenderContent(obj, refobj)

                            lsPropertyTable += content.Value


                        End If

                        lsPropertyTable += "</td></tr>"

                End Select
            End If
        Next

        objLit.Text += "</header>" & lsCheckBox & lsPropertyTable & "</table></footer>"

        If lbHorizontal Then
            objLit.Text += "</section>"
        End If
        objLit.Text += "</article>"

        objLit.Text = lsRowStart & objLit.Text & "</td></tr></tbody></table></div>"
        Return objLit
    End Function

    Private Function ProcessDefaultRow(ByVal item As RepeaterItem, ByRef obj As DM_OBJECTSearch.OBJECTInfo, ByVal voColumns As List(Of GridLayout.ColumnDefinition), ByVal vbMainRow As Boolean, ByRef lbProceed As Boolean, ByRef refobj As DM_OBJECTSearch.OBJECTInfo) As Literal
        If vbMainRow Then
            lbProceed = ExtractShortCut(obj, refobj)
        Else
            lbProceed = True
        End If

        Dim sb As New StringBuilder

        Dim objLit As New Literal
        If lbProceed = False Then Return objLit

        If vbMainRow Then
            ListRows.AppendFormat("'{0}',", item.ClientID)
        End If

        For Each loColumn As GridLayout.ColumnDefinition In voColumns

            Dim loContentProvider As BaseContentProvider = loColumn.GetContentProvider(Page, ContentParams, Mode)
            If loContentProvider Is Nothing Then Continue For

            loContentProvider.RowClientID = item.ClientID

            Dim lsSize As String = loColumn.Width
            If String.IsNullOrEmpty(lsSize) Then lsSize = loContentProvider.DefaultWidth

            Dim lsExtra As String = loColumn.Extra
            Dim colspan As Int32 = loColumn.Colspan

            sb.Append("<td")

            If colspan = 1 Then
                If Not String.IsNullOrEmpty(lsSize) Then
                    sb.Append(" width=""")
                    sb.Append(lsSize)
                    sb.Append("""")
                End If
            Else
                sb.Append(" colspan=""")
                sb.Append(colspan)
                sb.Append("""")
            End If
            Select Case loContentProvider.Alignment
                Case HorizontalAlign.Center
                    sb.Append(" class='CenterAlign'")
                Case HorizontalAlign.Right
                    sb.Append(" class='RightAlign'")
            End Select
            If Not String.IsNullOrEmpty(lsExtra) Then
                sb.Append(" ")
                sb.Append(lsExtra)
            End If

            sb.Append(">")
            Dim content As CellContent = loContentProvider.RenderContent(obj, refobj)

            sb.Append(content.Value)

            sb.Append("</td>")

        Next

        If vbMainRow Then


            Dim rowid As Int32
            Dim lsFromArchive As String = ""
            Select Case Results.ResultType
                Case DM_OBJECTSearch.eResultType.Objects, DM_OBJECTSearch.eResultType.ObjectsAndArchivedObjectVersions, DM_OBJECTSearch.eResultType.ArchivedObjectVersions
                    rowid = obj.ID
                    If obj.FromArchive Then
                        lsFromArchive = "Y"
                    End If
                Case DM_OBJECTSearch.eResultType.CaseList
                    rowid = obj.TechID
                Case DM_OBJECTSearch.eResultType.Files
                    rowid = obj.FILE_ID
                Case DM_OBJECTSearch.eResultType.ArchivedCasesList, DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                    rowid = obj.CaseID
                Case DM_OBJECTSearch.eResultType.Mails
                    rowid = obj.Mail_Id
                Case Else
                    rowid = 0
            End Select

            Dim sb2 As New StringBuilder
            sb2.Append("<tr class='ListContent' id='")
            sb2.Append(rowid)
            sb2.Append("' object_type='")
            sb2.Append(GetObjectTypeAttribute(obj))
            If obj.MailBox <> Mail.MailBoxItem.MailBox.Undefined Then
                sb2.Append("' mailbox='")
                sb2.Append(obj.MailBox)
            End If
            If obj.OBJECT_TYPE = "Shortcut" Then
                sb2.Append("' object_reference_type='")
                sb2.Append(obj.Object_Reference_Type)
            End If
            If Not String.IsNullOrEmpty(lsFromArchive) Then
                sb2.Append("' archived='")
                sb2.Append(lsFromArchive)
            End If
            If Not String.IsNullOrEmpty(obj.DEFAULT_TREE_ACTION) Then
                sb2.Append("' default_action='")
                sb2.Append(obj.DEFAULT_TREE_ACTION)
            End If
            sb2.Append("'>")
            sb2.Append(sb.ToString)
            sb2.Append("</tr>")

            sb = sb2
        End If


        objLit.Text = sb.ToString()
        Return objLit

    End Function

    Protected Sub ProcessDefaultImageListItem(ByVal item As RepeaterItem, ByRef obj As DM_OBJECTSearch.OBJECTInfo, ByRef refobj As DM_OBJECTSearch.OBJECTInfo)
        If Not item.FindControl("plhMainImageListCell") Is Nothing Then item.FindControl("plhMainImageListCell").Controls.Add(ProcessDefaultImageListRow(obj, Columns, refobj))
    End Sub

    Protected Sub ProcessDefaultItem(ByVal item As RepeaterItem, ByRef obj As DM_OBJECTSearch.OBJECTInfo, ByRef refobj As DM_OBJECTSearch.OBJECTInfo)

        Dim lbProceed As Boolean = True
        If Not item.FindControl("plhMainRow") Is Nothing Then item.FindControl("plhMainRow").Controls.Add(ProcessDefaultRow(item, obj, Columns, True, lbProceed, refobj))
        If lbProceed Then
            For Each loRow As GridLayout.RowDefinition In Layout.RowsList
                Dim loLitheader As New Literal
                loLitheader.Text = "<tr class='ListSubContent'>"
                item.FindControl("plhMainRow").Controls.Add(loLitheader)
                If Not item.FindControl("plhMainRow") Is Nothing Then
                    Dim rowColumns As List(Of GridLayout.ColumnDefinition) = loRow.ColumnsList.Where(Function(x) x.ShowInSiteVersion(7)).ToList()
                    item.FindControl("plhMainRow").Controls.Add(ProcessDefaultRow(item, obj, rowColumns, False, lbProceed, refobj))
                End If
                Dim loLitFooter As New Literal
                loLitFooter.Text = "</tr>"
                item.FindControl("plhMainRow").Controls.Add(loLitFooter)
            Next
        End If

    End Sub

#Region " Criteria Adding "

    Private Sub AddOrderByToCriteria(ByVal roCrit As DM_OBJECTSearch.Criteria)

        If Layout.AllowOrderBy AndAlso IsPostBack AndAlso Not String.IsNullOrEmpty(OrderbyField.Value) Then

            For Each column As GridLayout.ColumnDefinition In Columns
                Dim contentProvider As BaseContentProvider = column.GetContentProvider(Page, ContentParams, Mode)
                If contentProvider Is Nothing OrElse String.IsNullOrEmpty(contentProvider.OrderByField) Then Continue For
                If contentProvider.OrderByField = OrderbyField.Value Then
                    roCrit.ORDERBY = contentProvider.OrderByField & " " & GetOrderByOrder(OrderbyOrderField.Value)
                    Layout.SetUserOrderBy(roCrit.ORDERBY)
                    Exit For
                End If
            Next

        ElseIf String.IsNullOrEmpty(roCrit.ORDERBY) Then
            roCrit.ORDERBY = Layout.GetOrderBy
        End If

        roCrit.FileORDERBY = Layout.GetFileOrderBy

    End Sub

    Private Sub AddPagingToCriteria(ByRef roCrit As DM_OBJECTSearch.Criteria, ByVal firstRec As Integer, ByVal lastRec As Integer)
        roCrit.FIRSTRECORD = firstRec
        roCrit.LASTRECORD = lastRec
        roCrit.MAXRESULTS = _defaultMaxResults
        'Do
        '    roCrit.MAXRESULTS += _defaultMaxResults
        'Loop While roCrit.MAXRESULTS < (lastRec - 2)

    End Sub

    Public Sub AddFiltersToCriteria(ByRef roCrit As DM_OBJECTSearch.Criteria)

        If ShowCalendar Then
            roCrit.WhatsNewDateExact = Not chkwhatsnewdatenotexact.Checked
            roCrit.WhatsNewDateIncludeModifDate = chkWhatsNewIncludeModifDate.Checked

            If String.IsNullOrEmpty(CreationDate.Value) Then
                calWhatsNew.SelectedDate = Arco.SystemClock.Now
                roCrit.WhatsNewDate = Arco.SystemClock.Now.Date.ToString(ArcoFormatting.GetDateFormat())
            Else
                Dim ldDate As DateTime

                DateTime.TryParse(CreationDate.Value, ldDate)

                calWhatsNew.SelectedDate = ldDate
                roCrit.WhatsNewDate = ldDate.ToString(ArcoFormatting.GetDateFormat())
            End If
            calWhatsNew.FocusedDate = calWhatsNew.SelectedDate
        End If
        If String.IsNullOrEmpty(roCrit.GlobalSearchTerm) Then
            roCrit.GlobalSearchTerm = txtGlobalSearch.Value
        End If

        Dim lbApplyScreenFilters As Boolean = (UserProfile.ShowFilters AndAlso Layout.ShowFilters)

        For Each column As GridLayout.ColumnDefinition In Columns
            Dim loContentProvider As BaseContentProvider = column.GetContentProvider(Page, ContentParams, Mode)
            If loContentProvider Is Nothing Then Continue For

            loContentProvider.ApplyContentProviderFilter(roCrit)
            If lbApplyScreenFilters AndAlso column.ShowFilter Then
                loContentProvider.ApplyFilter(roCrit)
            End If
        Next

        For Each loRow As GridLayout.RowDefinition In Layout.RowsList
            For Each losubHeader As GridLayout.ColumnDefinition In loRow.ColumnsList
                Dim loContentProvider As BaseContentProvider = losubHeader.GetContentProvider(Page, ContentParams, Mode)
                If loContentProvider Is Nothing Then Continue For

                loContentProvider.ApplyContentProviderFilter(roCrit)
            Next
        Next


        roCrit.AND_OR_FILTERS = GridFilters.AndOr
        For Each loFilter As GridFilters.Filter In GridFilters.Filters
            Dim lbEnabled As Boolean = loFilter.Enabled
            Dim lbApply As Boolean = True
            Dim lbVisible As Boolean = loFilter.Visible
            If Not String.IsNullOrEmpty(loFilter.Role) Then
                lbApply = BusinessIdentity.CurrentIdentity.IsInRole(loFilter.Role)
                If Not lbApply Then
                    'reverse when not in role 
                    If loFilter.reversefilterwhennotinrole Then
                        lbEnabled = Not lbEnabled
                        lbApply = True
                    End If
                End If
            End If

            If lbApply Then
                If Not Page.IsPostBack OrElse String.IsNullOrEmpty(internalsubmit.Value) Then
                    If lbEnabled Then
                        roCrit.AddFilter(CType(loFilter.Index, Int32), loFilter.NormalFilter, loFilter.Recursive)
                    Else
                        If Not String.IsNullOrEmpty(loFilter.ReversedFilter) Then
                            roCrit.AddFilter(CType(loFilter.Index, Int32), loFilter.ReversedFilter, loFilter.Recursive)
                        End If
                    End If
                Else
                    If lbVisible AndAlso Layout.HeaderContextMenus Then
                        lbEnabled = Not DisabledFilters.Value.Contains(";" & loFilter.Index & ";")
                    End If

                    If lbEnabled Then
                        roCrit.AddFilter(CType(loFilter.Index, Int32), loFilter.NormalFilter, loFilter.Recursive)
                    Else
                        If Not String.IsNullOrEmpty(loFilter.ReversedFilter) Then
                            roCrit.AddFilter(CType(loFilter.Index, Int32), loFilter.ReversedFilter, loFilter.Recursive)
                        End If
                    End If
                End If
            End If

        Next

        'filters
        roCrit.CaseSearch.ShowSuspended = UserProfile.ShowSuspendedDossiers
        roCrit.CaseSearch.ShowLockedDossiers = UserProfile.ShowLockedDossiers ' (txtShowLocked.Value = "Y")

        If (FilterOnDelegate.Value <> BusinessIdentity.CurrentIdentity.Name) Then
            roCrit.CaseSearch.ApplyWorkExceptions = True
            roCrit.CaseSearch.FilterOnDelegate = FilterOnDelegate.Value
        Else
            roCrit.CaseSearch.ApplyWorkExceptions = False
        End If

        'grouping
        roCrit.GroupResultsFor = GroupByFilter.Value

        For Each grouper As String In Split(GroupBy.Value, "##").Select(Function(x) x.Trim).Where(Function(x) x.Length <> 0)
            Dim grouperid As Int32 = CType(grouper.Substring(0, grouper.IndexOf(";;", StringComparison.Ordinal)), Int32)
            Dim grouperField As String = grouper.Substring(grouper.IndexOf(";;", StringComparison.Ordinal) + 2)

            Dim orderby As DM_OBJECTSearch.Criteria.eGroupingOrderBy? = Nothing

            If grouperField.Contains(";;") Then
                'also has orderby, split
                Dim orderbyClause As String = grouperField.Substring(grouperField.IndexOf(";;", StringComparison.Ordinal) + 2)
                Select Case orderbyClause
                    Case "WordAscending"
                        orderby = DM_OBJECTSearch.Criteria.eGroupingOrderBy.WordAscending
                    Case "WordDescending"
                        orderby = DM_OBJECTSearch.Criteria.eGroupingOrderBy.WordDescending
                    Case "CountAscending"
                        orderby = DM_OBJECTSearch.Criteria.eGroupingOrderBy.CountAscending
                    Case "CountDescending"
                        orderby = DM_OBJECTSearch.Criteria.eGroupingOrderBy.CountDescending
                End Select
                grouperField = grouperField.Substring(0, grouperField.IndexOf(";;", StringComparison.Ordinal))
            End If

            'check if grouperfield has been tampered with            
            For Each grouperDef As GridGroupingInfo.Grouper In GridGrouping.GrouperList.Where(Function(x) x.ID = grouperid)
                For Each field As GridGroupingInfo.Grouper.GrouperField In grouperDef.Fields

                    If grouperField = field.GetFieldName(ContentParams) Then
                        roCrit.AddGrouper(grouperid, grouperField, orderby)
                        Exit For
                    End If

                Next
            Next
        Next
        roCrit.GroupingHeadersOnly = False

        If Mode = DocroomListHelpers.GridMode.Normal AndAlso msPrintOptions = "0" Then 'filter current selection
            Dim lsList As String = Sel.Value.Trim
            If String.IsNullOrEmpty(lsList) Then
                lsList = lsList.Substring(0, lsList.Length - 1)
                roCrit.DM_OBJECT_ID_LIST = lsList
            End If
        End If

        'add object type filter        
        If String.IsNullOrEmpty(roCrit.Object_Type) AndAlso (roCrit.ResultType = DM_OBJECTSearch.eResultType.Objects) AndAlso roCrit.GetCategories().Count = 0 Then
            roCrit.Exclude_Object_Type = "Query"
            If roCrit.ContainingLinkToListItem = 0 AndAlso (roCrit.DM_PARENT_ID = 0 AndAlso roCrit.DM_INCLUDESUBFOLDERS = False) Then
                roCrit.Exclude_Object_Type &= ",ListItem"
            End If
            If Not ShowFolders Then
                roCrit.Exclude_Object_Type &= ",Folder"
            End If
        End If
    End Sub

#End Region

#Region " Load and Settings "
    Private _defaultresultsPerPage As Int32 = 10
    Private _defaultMaxResults As Int32

    Private _customResultScreen As ScreenList.ScreenInfo

    Private Sub LoadResultScreenInfo(ByVal loResultListCrit As DM_OBJECTSearch.Criteria)
        If ResultScreenID > 0 Then
            Dim crit As New ScreenList.Criteria
            crit.Mode = Screen.ScreenMode.RESULT
            crit.ID = ResultScreenID

            For Each objInfo As ScreenList.ScreenInfo In ScreenList.GetScreenList_Use(crit)
                _customResultScreen = objInfo
                Select Case objInfo.Type
                    Case Screen.ScreenSourceType.DefaultMode
                        Dim source As String = Arco.Settings.FrameWorkSettings.ReplaceGlobalVars(objInfo.Source, False)
                        If _loadedXMLFile <> source Then
                            GridXMLFile = source
                            'LoadGrid(objInfo.Source)
                        End If
                    Case Screen.ScreenSourceType.TemplateFile
                        _loadedXMLFile = ""
                    Case Screen.ScreenSourceType.Url
                        _loadedXMLFile = ""
                        'supported???
                        RedirectToCustomUrl(objInfo.Source, loResultListCrit)
                End Select
            Next
        End If
    End Sub

    Private Sub RedirectToCustomUrl(ByVal vsUrl As String, ByVal loResultListCrit As DM_OBJECTSearch.Criteria)
        Dim lsScript As String = "<html><body><form name='frmRedirect' method='post' action='" & vsUrl & "'>"
        lsScript &= "<input type=hidden name='QRY_XML' VALUE='" & loResultListCrit.Serialize & "' />"
        lsScript &= "<script language='javascript'>document.frmRedirect.submit();</script>"
        lsScript &= "</form></body></html>"

        Response.Write(lsScript)
        Response.End()
    End Sub


    Public Sub LoadGroupingInfo(Optional ByVal forceReinit As Boolean = False)

        Dim doInit As Boolean = forceReinit OrElse Not Page.IsPostBack

        If doInit Then
            GroupBy.Value = ""
        End If

        If GroupingEnabled AndAlso ShowSideBar Then
            'load fixed groupers into the screen                     

            Dim exceptions As HashSet(Of String) = Nothing

            If doInit Then
                exceptions = GridGrouping.ApplyUserExceptionFile()
            End If
            ContentParams.ResultType = Me.ResultType 'for loFieldNode.GetFieldName later

            For Each grouper As GridGroupingInfo.Grouper In GridGrouping.GrouperList
                Dim grouperid As String = grouper.ID.ToString()
                If doInit Then

                    Dim enabled As Boolean

                    If grouper.UserGrouper Then
                        enabled = grouper.DefaultUserGrouperEnabled
                        If exceptions.Contains(grouperid) Then
                            enabled = Not enabled
                        End If
                    Else
                        enabled = True
                    End If

                    If enabled Then
                        For Each field As GridGroupingInfo.Grouper.GrouperField In grouper.Fields
                            Try
                                GroupBy.Value &= grouperid & ";;" & field.GetFieldNameAndOrderBy(ContentParams) & "##"
                            Catch pex As Exceptions.PropertyNotFoundException
                                AddLoadingError("Unable to group on " & field.Value & ", property not found")
                            End Try
                        Next
                    End If
                Else
                    If grouper.UserGrouper Then
                        For Each field As GridGroupingInfo.Grouper.GrouperField In grouper.Fields
                            Try

                                Dim userHasEnabled As Boolean = GroupBy.Value.Contains(grouperid & ";;" & field.GetFieldNameAndOrderBy(ContentParams) & "##")
                                If userHasEnabled <> grouper.DefaultUserGrouperEnabled Then
                                    GridGrouping.AddUserException("ADD:" & grouperid)
                                Else
                                    GridGrouping.AddUserException("REMOVE:" & grouperid)
                                End If
                            Catch pex As Exceptions.PropertyNotFoundException
                                AddLoadingError("Unable to group on " & field.Value & ", property not found")
                            End Try
                            Exit For
                        Next
                    End If
                End If
            Next
            _contentParams = Nothing
        End If


        ' End If
    End Sub

    Private Sub AddLoadingError(ByVal text As String)
        'shows errorors during loading,but without stopping the page
        trMessages.Visible = True
        If Not String.IsNullOrEmpty(lblErrors.Text) Then lblErrors.Text &= "<br/>"
        lblErrors.Text &= Server.HtmlEncode(text)
    End Sub

    Protected ReadOnly Property GridMode() As Int32
        Get
            If GridVisible Then
                Return CType(Layout.GridMode, Int32)
            Else
                Return 0
            End If
        End Get
    End Property

    Private _gridlayout As GridLayout

    Public ReadOnly Property Layout As GridLayout
        Get
            Return _gridlayout
        End Get
    End Property

    Public Property GlobalSearch As String
        Get
            Return txtGlobalSearch.Value
        End Get
        Set(value As String)
            txtGlobalSearch.Value = value
        End Set
    End Property

    'Public Sub LoadGrid()
    '    LoadGrid(Nothing)

    'End Sub

    Private Sub LoadGrid(ByVal xmlFile As String, ByVal reinit As Boolean)
        Dim lsFileToLoad As String

        If Not String.IsNullOrEmpty(xmlFile) Then
            lsFileToLoad = xmlFile
        Else
            lsFileToLoad = "DefaultResultGrid.xml"
        End If


        _gridlayout = GridLayout.LoadGridLayout(lsFileToLoad, DefaultGridXmlFile)

        'reload related data
        _groupinginfo = Nothing
        _toolbar = Nothing
        _filterInfo = Nothing

        _loadedXMLFile = _gridlayout.FileName

        LoadGroupingInfo(reinit)
    End Sub

    Private ReadOnly _allFirstRowcolumns As New List(Of GridLayout.ColumnDefinition)

    Protected ReadOnly Property AllFirstRowColumns As List(Of GridLayout.ColumnDefinition)
        Get
            Return _allFirstRowcolumns
        End Get
    End Property

    Private ReadOnly _columns As New List(Of GridLayout.ColumnDefinition)
    Protected ReadOnly Property Columns As List(Of GridLayout.ColumnDefinition)
        Get
            Return _columns
        End Get
    End Property

    Private Sub BuildColumnsList(ByVal voCrit As DM_OBJECTSearch.Criteria)
        AllFirstRowColumns.Clear()
        If InSelectionMode Then
            AllFirstRowColumns.Add(New GridLayout.ColumnDefinition() With {.Name = "select"})
        End If

        'add grid columns
        For Each col As GridLayout.ColumnDefinition In Layout.ColumnsList
            If Not col.ShowInSiteVersion(7) Then Continue For
            AllFirstRowColumns.Add(col)
        Next

        If CanEdit Then
            If voCrit.LinkedToPackID > 0 Then
                'add remove from package link at the end if needed
                If (voCrit.LinkedToCaseID > 0 OrElse voCrit.LinkedToObjectID > 0) Then
                    AllFirstRowColumns.Add(New GridLayout.ColumnDefinition() With {.Name = "removefrompackage"})
                End If
            End If
            If voCrit.ContainingLinkToObjectDIN > 0 Then
                AllFirstRowColumns.Add(New GridLayout.ColumnDefinition() With {.Name = "removefromdossier"})
            End If
        End If

        'If String.IsNullOrEmpty(msLoadedXMLFile) Then
        '    Exit Sub
        'End If

        If Not String.IsNullOrEmpty(showhidecol.Value) Then
            Select Case showhidecol.Value
                Case "RESETALL"
                    Layout.ResetUserExceptions()
                    OrderbyField.Value = ""
                    OrderbyOrderField.Value = ""
                Case "RESETORDERBY"
                    Layout.ResetUserOrderBy()
                    OrderbyField.Value = ""
                    OrderbyOrderField.Value = ""
                Case "RESETCOLS"
                    Layout.ResetUserColumns()
                Case Else
                    Layout.AddUserException(showhidecol.Value)
            End Select

            showhidecol.Value = ""
        End If

        _hiddenColumns = Layout.GetHiddenCollumns()

        For Each col As GridLayout.ColumnDefinition In AllFirstRowColumns
            If Not _hiddenColumns.Contains(col.UniqueName) Then
                If ShowToolbar OrElse col.Name <> "multiselect" Then
                    Columns.Add(col)
                End If
            End If
        Next
    End Sub

    Private Sub LoadSettings()

        If Not Page.IsPostBack Then
            If ExpandFileRows Then
                filelisttogglemode.Value = "Y"
            Else
                filelisttogglemode.Value = ""
            End If
            If UserProfile.HideDelegatedWork Then
                FilterOnDelegate.Value = Security.BusinessIdentity.CurrentIdentity.Name
            End If
        End If

    End Sub

    Public ReadOnly Property ExpandFileRows() As Boolean
        Get
            If Not Page.IsPostBack Then
                Return UserProfile.AlwaysShowAttachments AndAlso Not AjaxEnabled
            Else
                Return (filelisttogglemode.Value = "Y")
            End If
        End Get
    End Property

    Public Property ShowDisabledToolbarButtons As Boolean = True

    Public ReadOnly Property OpenDetailWindowsModal As Boolean
        Get
            If Not Layout Is Nothing Then
                Return Layout.OpenDetailWindowsModal
            Else
                Return False
            End If
        End Get
    End Property

    Private Sub SetLabels()
        chkwhatsnewdatenotexact.Text = GetLabel("addedsince")
        chkWhatsNewIncludeModifDate.Text = GetLabel("includemodifdate")
    End Sub

    Private Sub CheckLoadOptions()

        mbForceDMObjectJoin = False
        _hasMultiSelect = False
        miForceJoinOnProperyStore = 0

        If Not String.IsNullOrEmpty(_loadedXMLFile) Then
            _showAttachmentsOnSecondRow = False
            _fileExpandShown = False
            mbForceDMObjectJoin = Layout.ForceJoinOnDMObject
            miForceJoinOnProperyStore = Layout.ForceJoinOnPropertStore
            For Each loColumn As GridLayout.ColumnDefinition In Layout.ColumnsList
                If Not ColumnIsHidden(loColumn.UniqueName) Then
                    Dim lsColName As String = loColumn.Name

                    Select Case lsColName
                        Case "expand"
                            _fileExpandShown = True
                        Case "file"
                            _fileExpandShown = True
                            _showAttachmentsOnSecondRow = True
                        Case "documentlinks" ',"mainfile"
                            _showAttachmentsOnSecondRow = True
                        Case "multiselect"
                            _hasMultiSelect = True
                    End Select
                End If

            Next

        End If
    End Sub

    Private _isCallBack As Boolean

    Private ajxInit As Boolean = False

    Private Sub InitAjax()
        If ajxInit Then
            Exit Sub
        End If
        Dim ajxMan As RadAjaxManager = RadAjaxManager.GetCurrent(Page)
        If AjaxEnabled AndAlso ajxMan Is Nothing Then
            'try again onLoad
            Exit Sub
        End If
        ajxInit = True
        If Not ajxMan Is Nothing Then
            ajxMan.DefaultLoadingPanelID = radAjxLoadingPanel.UniqueID
            If AjaxEnabled Then

                ajxMan.EnableAJAX = True
                ajxMan.AjaxSettings.AddAjaxSetting(ajxButton, radAjx1, radAjxLoadingPanel)

                _isCallBack = ajxMan.IsAjaxRequest

            Else
                ajxMan.EnableAJAX = False
            End If
        Else
            AjaxEnabled = False
        End If

        radAjx1.EnableViewState = True
    End Sub

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init


        InitAjax()
        LoadSettings()
        internalsubmit.Value = ""

        'doclistscroller.Visible = False   -- Can this be removed, Stefan? In CMS docroomlist-panels, the databind is called before page_init, so always overwritten to false
        'if not, databind can be called later, but code might get messy. This is cleaner.
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitAjax()
        SetLabels()
    End Sub

    Private Sub AddInitScript(ByVal vbCallBack As Boolean)

        Dim sb As New StringBuilder
        Dim lbCtxMenus As Boolean = (Layout Is Nothing OrElse Layout.ItemContextMenus) AndAlso UserProfile.EnableContextMenus

        If Not vbCallBack Then
            sb.Append("var ")
        End If

        sb.Append(JavascriptVariableName)
        sb.Append(" = new DocroomListControl('")
        sb.Append(ClientID)
        sb.Append("',")
        sb.Append(_parentID)
        sb.Append(",")
        sb.Append(RootID)
        sb.Append(",")
        If ContentParams.GridLimitedToCategory.Any Then
            sb.Append(ContentParams.GridLimitedToCategory(0)) '
        Else
            sb.Append("0")
        End If

        sb.Append(",")
        sb.Append(LastPage)
        sb.Append(",'")
        If HeaderContextMenu IsNot Nothing Then
            sb.Append(HeaderContextMenu.ClientID)
        End If
        sb.Append("',")
        sb.Append(EncodingUtils.EncodeJsBool(GridVisible))
        sb.Append(",")
        sb.Append(GridMode)
        sb.Append(",new Array(")
        sb.Append(ListRows.ToString)
        sb.Append("),'")
        sb.Append(msObjectList.ToString)
        sb.Append("',")
        sb.Append(ResultType)
        sb.Append(",")
        sb.Append(EncodingUtils.EncodeJsBool(lbCtxMenus AndAlso Mode <> DocroomListHelpers.GridMode.Print))
        sb.Append(",'")
        sb.Append(Server.UrlEncode(TermsList))
        sb.Append("','")
        sb.Append(tlbGrid.ClientID)
        sb.Append("',")
        sb.Append(mlLinkToCaseID)
        sb.Append(",")
        sb.Append(mlLinkToObjectID)
        sb.Append(",")
        sb.Append(mlLinkToPackID)
        sb.Append(",")
        sb.Append(EncodingUtils.EncodeJsBool(Page.IsPostBack))
        sb.Append(",")
        sb.Append(EncodingUtils.EncodeJsBool(vbCallBack))
        sb.Append(",'")
        sb.Append(_loadedXMLFile)
        sb.Append("','")
        sb.Append(DefaultGridXmlFile)
        sb.Append("',")
        sb.Append(EncodingUtils.EncodeJsBool(AjaxEnabled))
        sb.Append(",'")
        sb.Append(ajxButton.UniqueID)
        sb.Append("',")
        sb.Append(ResultScreenID)
        sb.Append(",")
        If ResultType = DM_OBJECTSearch.eResultType.CaseList Then
            sb.Append(Convert.ToInt32(DMSelection.SelectionType.CurrentCases))
        Else
            sb.Append(Convert.ToInt32(DMSelection.SelectionType.CurrentCasesByCaseID))
        End If
        sb.Append(",'")
        If Layout IsNot Nothing Then sb.Append(EncodingUtils.EncodeJsString(Layout.ContextMenuFile, False))
        sb.Append("',")
        sb.Append(EncodingUtils.EncodeJsBool(ShowDisabledToolbarButtons))
        sb.Append(",")
        sb.Append(EncodingUtils.EncodeJsBool(OpenDetailWindowsModal))
        sb.Append(");")

        sb.Append("function ")
        sb.Append(JavascriptVariableName)
        sb.Append("_whatsnewDaySelected(sender, eventArgs){")
        sb.Append(JavascriptVariableName)
        sb.Append(".whatsnewDaySelected(sender, eventArgs);}")


        If Not vbCallBack Then
            sb.Append("Sys.Application.add_load(delegate(")
            sb.Append(JavascriptVariableName)
            sb.Append(", ")
            sb.Append(JavascriptVariableName)
            sb.Append(".onLoad));")

        Else

            sb.Append(JavascriptVariableName)
            sb.Append(".onReload();")
        End If



        If Not vbCallBack AndAlso AutoOpenFirstHit AndAlso miFirstHit > 0 Then
            sb.Append(JavascriptVariableName)
            sb.Append(".DoRowAction(")
            sb.Append(miFirstHit)
            sb.Append(");")
        End If

        If AjaxEnabled Then tblMain.Attributes("onKeyDown") = JavascriptVariableName & ".HandleEnter(event);"


        If ShowToolbar AndAlso GridVisible Then
            For Each itm As RadToolBarItem In tlbGrid.Items
                If TypeOf itm Is RadToolBarButton Then
                    Dim but As RadToolBarButton = DirectCast(itm, RadToolBarButton)
                    If Not but.IsSeparator Then
                        If but.NavigateUrl.Contains("#GRID#") Then
                            but.NavigateUrl = but.NavigateUrl.Replace("#GRID#", JavascriptVariableName)
                        End If

                        If but.Enabled = False Then
                            If but.Attributes("MS") = "1" Then
                                sb.Append(JavascriptVariableName)
                                sb.Append(".AddSelectionButton('")
                                sb.Append(but.Value)
                                sb.Append("');")
                                but.Attributes.Remove("MS")
                            End If
                            If but.Attributes("MSC") = "1" Then
                                sb.Append(JavascriptVariableName)
                                sb.Append(".AddCaseSelectionButton('")
                                sb.Append(but.Value)
                                sb.Append("');")
                                but.Attributes.Remove("MSC")
                            End If
                            If but.Attributes("MSM") = "1" Then
                                sb.Append(JavascriptVariableName)
                                sb.Append(".AddMailSelectionButton('")
                                sb.Append(but.Value)
                                sb.Append("');")
                                but.Attributes.Remove("MSM")
                            End If
                        End If
                    End If
                ElseIf TypeOf itm Is RadToolBarDropDown Then
                    Dim tlb As RadToolBarDropDown = DirectCast(itm, RadToolBarDropDown)
                    For Each but As RadToolBarButton In tlb.Buttons
                        If Not but.IsSeparator Then
                            If but.NavigateUrl.Contains("#GRID#") Then
                                but.NavigateUrl = but.NavigateUrl.Replace("#GRID#", JavascriptVariableName)
                            End If

                            If but.Enabled = False Then
                                If but.Attributes("MS") = "1" Then
                                    sb.Append(JavascriptVariableName)
                                    sb.Append(".AddSelectionButton('")
                                    sb.Append(but.Value)
                                    sb.Append("');")
                                    but.Attributes.Remove("MS")
                                End If
                                If but.Attributes("MSC") = "1" Then
                                    sb.Append(JavascriptVariableName)
                                    sb.Append(".AddCaseSelectionButton('")
                                    sb.Append(but.Value)
                                    sb.Append("');")
                                    but.Attributes.Remove("MSC")
                                End If
                                If but.Attributes("MSM") = "1" Then
                                    sb.Append(JavascriptVariableName)
                                    sb.Append(".AddMailSelectionButton('")
                                    sb.Append(but.Value)
                                    sb.Append("');")
                                    but.Attributes.Remove("MSM")
                                End If
                            End If
                        End If
                    Next
                End If
            Next
        End If


        If Not vbCallBack Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType, "init" & JavascriptVariableName, sb.ToString, True)
        Else
            RadAjaxManager.GetCurrent(Page).ResponseScripts.Add(sb.ToString())
        End If
    End Sub

#End Region

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRender
        If Not IsPostBack OrElse Not _isCallBack Then
            AddInitScript(_isCallBack)

            pnlQuickUpload.Visible = cmbQuickUploadCat.Visible


        End If

        Dim grpWidth As String = GetSideBarWidth()

        If Not String.IsNullOrEmpty(grpWidth) Then
            pnlTopSidePanel.Style.Add("margin-left", grpWidth)
            pnlBottomSidePanel.Style.Add("margin-left", grpWidth)
        Else
            pnlTopSidePanel.Style.Remove("margin-left")
            pnlBottomSidePanel.Style.Remove("margin-left")
        End If
        ajxButtonUniqueId.Value = ajxButton.UniqueID

        If Layout IsNot Nothing AndAlso Layout.StickyHeaders AndAlso Me.Mode = DocroomListHelpers.GridMode.Normal Then
            tblMain.CssClass += " StickyHeaders"
        End If
    End Sub

    Protected Sub ajxButton_Click(sender As Object, e As EventArgs) Handles ajxButton.Click
        GridXMLFile = GridXMLFile 'force the correct grid

        Dim crit As DM_OBJECTSearch.Criteria = GetCriteriaForCallBack()
        If crit IsNot Nothing Then
            'force reload of the grouping          
            DataBind(crit, True)
            AddInitScript(True)
        End If

    End Sub

    Private Function GetCriteriaForCallBack() As DM_OBJECTSearch.Criteria
        If Not String.IsNullOrEmpty(critfield.Value) Then
            Return DM_OBJECTSearch.Criteria.Deserialize(StringEncryptionService.Decrypt(critfield.Value))
        End If
        Return Nothing
    End Function

End Class
