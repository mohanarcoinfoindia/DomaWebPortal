Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Routing
Imports Arco.Doma.Library.Search
Imports Arco.Doma.WebControls.DocroomListHelpers
Imports Arco.Doma.WebControls.DocroomListHelpers.ContentProviders

Partial Class PrintToExcel
    Inherits ActionProcessorPage

    Private Shared _jobs As New Dictionary(Of String, ExportExcelJob)

    Private Function GetJob(ByVal id As String) As ExportExcelJob
        Return CType(HttpRuntime.Cache("exportexcel_" & id), ExportExcelJob)
    End Function

    Private Sub RemoveJob(ByVal id As String)
        HttpRuntime.Cache.Remove("exportexcel_" & id)
    End Sub
    Private Sub AddJob(ByVal id As String, ByVal job As ExportExcelJob)
        HttpRuntime.Cache.Add("exportexcel_" & id, job, Nothing, DateTime.Now.AddHours(1), Cache.NoSlidingExpiration, CacheItemPriority.Normal, Nothing)
    End Sub

    Public Property TaskId As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim getStatus As String = QueryStringParser.GetString("getStatus")
        If Not String.IsNullOrEmpty(getStatus) Then
            TaskId = getStatus
            Dim job As ExportExcelJob = GetJob(TaskId)
            If job IsNot Nothing Then
                If Not String.IsNullOrEmpty(job.DownloadUrl) Then

                    RemoveJob(TaskId)

                    Response.Write(job.DownloadUrl)
                ElseIf job.Progress.Cancelled Then
                    Response.Write("The task was cancelled")
                Else
                    Response.Write(job.Progress.Done & "/" & job.Progress.Total)
                End If
            Else
                Response.Write("The task was cancelled")
            End If
            Response.End()

            Return
        End If
        Dim cancelJob As String = QueryStringParser.GetString("cancelJob")
        If Not String.IsNullOrEmpty(cancelJob) Then
            TaskId = cancelJob
            Dim job As ExportExcelJob = GetJob(TaskId)
            If job IsNot Nothing Then
                job.CancelExport()

                RemoveJob(TaskId)
            End If
            Response.Write("The task was cancelled")
            Response.End()

            Return
        End If


        TaskId = Guid.NewGuid().ToString()
        Page.Title = GetLabel("exporting") & "..."
        Dim lsXMl As String = QueryStringParser.GetString("printxml")

        If String.IsNullOrEmpty(lsXMl) Then
            lsXMl = "printgrid.xml"
        End If

        Dim crit As DM_OBJECTSearch.Criteria = GetCriteria()


        Dim exporter As ExportExcelJob

        If lsXMl.ToUpper <> "AUTO" Then
            Dim gridLayout As GridLayout = GridLayout.LoadGridLayout(lsXMl)
            exporter = New ExportExcelJob(Me, gridLayout)
        Else
            exporter = New ExportExcelJob(Me, Nothing)
        End If

        exporter.StartExport(crit)
        AddJob(TaskId, exporter)



        ''Response.ClearHeaders()    
        ''workbook.Save(Page.Response, "Print.xls", Aspose.Cells.ContentDisposition.Attachment, workbook.SaveOptions)

        'Dim lsFile As String = Arco.IO.File.GetFreeFileName(Settings.GetTempPath, "Print", "xls")
        'workbook.Save(lsFile)

        'Arco.ApplicationServer.Library.Server.TempFileController.Add(lsFile)

        'HideProgress()
        'DownloadFile(lsFile, "Print.xls")
        'Page.ClientScript.RegisterStartupScript(Me.GetType, "DoResult", GetScript, True)
    End Sub

    Private Function GetCriteria() As DM_OBJECTSearch.Criteria
        Dim crit As DM_OBJECTSearch.Criteria
        Dim printOptions As String = QueryStringParser.GetString("printoptions")

        Select Case printOptions
            Case "0" 'selecton

                crit = DM_OBJECTSearch.Criteria.GetNewGridCriteria()
                Dim liSelType As DMSelection.SelectionType = CType(QueryStringParser.GetInt("printseltype", 1), DMSelection.SelectionType)
                crit.SelectionType = liSelType
                Select Case liSelType
                    Case DMSelection.SelectionType.CurrentCases
                        crit.ResultType = DM_OBJECTSearch.eResultType.CaseList
                    Case DMSelection.SelectionType.CurrentCasesByCaseID
                        crit.ResultType = DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                    Case DMSelection.SelectionType.CurrentMails
                        crit.ResultType = DM_OBJECTSearch.eResultType.Mails
                End Select
                crit.DM_INCLUDESUBFOLDERS = True

            Case Else
                crit = Security.BusinessIdentity.CurrentIdentity.GetLastQuery() ' DM_OBJECTSearch.Criteria.Deserialize(UserProfile.LastQuery)
                If crit Is Nothing Then
                    Throw New InvalidOperationException("Unable to retrieve CurrentIdentity.GetLastQuery()")
                End If
                crit.ExpandPropertiesList = Nothing
                crit.ExpandPropertiesDisplayValue = Nothing

                If printOptions = "1" Then 'current page
                    Dim liPage As Integer = QueryStringParser.GetInt("printpage")
                    Dim liResultsPerPage = ArcoInfoSettings.DefaultRecordsPerPage

                    If Not String.IsNullOrEmpty(UserProfile.RecordsPerPage) Then
                        liResultsPerPage = CType(UserProfile.RecordsPerPage, Int32)
                    End If

                    crit.FIRSTRECORD = ((liPage - 1) * liResultsPerPage) + 1
                    crit.LASTRECORD = crit.FIRSTRECORD + liResultsPerPage - 1

                Else 'all pages
                    crit.FIRSTRECORD = 0
                    crit.LASTRECORD = 0

                End If
        End Select

        crit.MAXRESULTS = ArcoInfoSettings.MaxResultsForExport
        Return crit
    End Function

    Protected Overrides ReadOnly Property PageUrl As String
        Get
            Return "PrintToExcel.aspx"
        End Get
    End Property
End Class

Public Class ExportProgress
    Public Property Total As Integer
    Public Property Done As Integer
    Public Property Started As Boolean
    Public Property Cancelled As Boolean

    Public Sub Start(ByVal total As Integer)
        Me.Total = total
        Done = 0
        Started = True
    End Sub
    Public Sub Increment()
        ' System.Threading.Thread.Sleep(500)
        Done += 1
    End Sub

End Class
Public Class ExportExcelJob
    Public Delegate Function RunExportAsync(ByVal crit As DM_OBJECTSearch.Criteria, ByVal progress As ExportProgress) As Aspose.Cells.Workbook


    Public Sub New(ByVal page As BasePage, ByVal gridLayout As GridLayout)
        _page = page
        _tempPath = page.Settings.GetTempPath
        _gridLayout = gridLayout
    End Sub

    Private _tempPath As String
    Private _page As BasePage
    Private _gridLayout As GridLayout

    Private _runner As RunExportAsync
    Public Progress As ExportProgress = New ExportProgress


    Public Function StartExport(ByVal crit As DM_OBJECTSearch.Criteria) As IAsyncResult

        Dim exporter = New ExcelExporter(_page, _gridLayout)

        _runner = New RunExportAsync(AddressOf exporter.Export)
        Return _runner.BeginInvoke(crit, Progress, AddressOf Callback, Nothing)
    End Function

    Public Sub CancelExport()
        Progress.Cancelled = True
    End Sub


    Private Sub Callback(ByVal ar As IAsyncResult)
        Try


            Dim workbook As Aspose.Cells.Workbook = _runner.EndInvoke(ar)
            If workbook IsNot Nothing Then

                Dim lsFile As String = Arco.IO.File.GetFreeFileName(_tempPath, "Print", "xls")
                workbook.Save(lsFile)

                Arco.ApplicationServer.Library.Server.TempFileController.Add(lsFile)

                DownloadUrl = "./Tools/StreamFile.aspx?file=" + StreamFileEncryptionService.Encrypt(lsFile) + "&attach=n&filename=Print.xls"
            Else
                Cancelled = True
            End If

            'HideProgress()
            'DownloadFile(lsFile, "Print.xls")
            'Page.ClientScript.RegisterStartupScript(Me.GetType, "DoResult", GetScript, True)
        Catch ex As Exception
            Arco.Utils.Logging.LogError("Unhandled exception in print to excel", ex)
            Cancelled = True
        End Try
    End Sub

    Public Property Cancelled As Boolean

    Public Property DownloadUrl As String

    Public Property PercentDone As Integer
End Class

Public Class ExcelExporter

    Public Sub New(ByVal page As BasePage, ByVal gridLayout As GridLayout)
        _page = page
        _gridLayout = gridLayout
    End Sub


    Private _page As BasePage
    Private _gridLayout As GridLayout
    Public Function Export(ByVal crit As DM_OBJECTSearch.Criteria, ByVal progress As ExportProgress) As Aspose.Cells.Workbook
        If _gridLayout IsNot Nothing Then
            Return ExportWithGridDefintion(crit, progress)
        Else
            Return ExportAuto(crit, progress)
        End If
    End Function

    Public Function ExportWithGridDefintion(ByVal crit As DM_OBJECTSearch.Criteria, ByVal progress As ExportProgress) As Aspose.Cells.Workbook
        Try


            Dim workbook As Aspose.Cells.Workbook = Arco.AsposeWrappers.Excel.GetWorkbook
            Dim WorkSheet As Aspose.Cells.Worksheet = workbook.Worksheets(0)
            'Dim gridLayout As GridLayout = gridLayout.LoadGridLayout(lsXMl)
            crit.ForceJoinOnDMObject = _gridLayout.ForceJoinOnDMObject

            crit.ExpandProperties = False
            Dim i As Integer = 0


            For Each column As GridLayout.ColumnDefinition In _gridLayout.ColumnsList
                Dim contentProvider As BaseContentProvider = column.GetContentProvider(_page, GetGridParams(Nothing), GridMode.Excel)
                If contentProvider IsNot Nothing AndAlso contentProvider.Printable Then
                    contentProvider.ApplyContentProviderFilter(crit)
                    WorkSheet.Cells(0, i).Value = EncodingUtils.HtmlDecode(contentProvider.HeaderLabel)
                    i += 1
                    If column.Name = "property" Then
                        crit.ExpandProperties = True
                    End If
                End If
            Next

            Dim results As DM_OBJECTSearch = DM_OBJECTSearch.GetOBJECTList(crit)
            progress.Start(results.LoadedItemsCount)
            Dim gridParams As GridParams = GetGridParams(results)

            Dim r As Integer = 1
            For Each result As DM_OBJECTSearch.OBJECTInfo In results
                If progress.Cancelled Then Return Nothing
                i = 0
                Dim refobj As DM_OBJECTSearch.OBJECTInfo = Nothing
                ExtractShortCut(result, refobj)
                For Each column In _gridLayout.ColumnsList
                    column.Init()
                    Dim contentProvider As BaseContentProvider = column.GetContentProvider(_page, gridParams, GridMode.Excel)
                    If contentProvider IsNot Nothing AndAlso contentProvider.Printable Then
                        If TypeOf (contentProvider) Is PropertyProvider Then
                            Dim loPropProvider As PropertyProvider = DirectCast(contentProvider, PropertyProvider)
                            FillPropertyCell(WorkSheet.Cells(r, i), result, loPropProvider.Params.PropID, Nothing, True)
                        Else

                            Dim content As CellContent = contentProvider.RenderContent(result, refobj)

                            WorkSheet.Cells(r, i).Value = SanitizeData(content.Value)
                        End If

                        i += 1
                    End If
                Next
                r += 1
                progress.Increment()
            Next
            WorkSheet.AutoFitColumns()

            Return workbook
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Function ExportAuto(ByVal crit As DM_OBJECTSearch.Criteria, ByVal progress As ExportProgress) As Aspose.Cells.Workbook

        Try

            crit.ExpandProperties = True
            crit.CaseSearch.ExpandAssignedToList = True

            Dim workbook As Aspose.Cells.Workbook = Arco.AsposeWrappers.Excel.GetWorkbook


            Dim loAutoTabs As New AutoTabs
            Dim loLabels As Globalisation.LABELList
            Dim lsCatProcType As String
            Dim cats As OBJECT_CATEGORYList = Nothing
            Dim procs As PROCEDUREList = Nothing
            Dim isCases As Boolean = IsCaseSearch(crit)

            If isCases Then
                loLabels = Globalisation.LABELList.GetProceduresLabelList(_page.EnableIISCaching)
                lsCatProcType = "Procedure"
                procs = PROCEDUREList.GetPROCEDUREList()

                Dim critProcId As Integer = New CriteriaAnalyzer(crit).GetProcedure()

                If critProcId <> 0 Then
                    crit.ExpandPropertiesList = PROPERTYList.GetAllPropertiesForProcedure(critProcId, False, False, "", True).Cast(Of PROPERTYList.PROPERTYInfo).Select(Function(x) x.ID).ToList
                End If

            Else
                loLabels = Globalisation.LABELList.GetCategoriesLabelList(_page.EnableIISCaching)
                lsCatProcType = "Category"
                cats = OBJECT_CATEGORYList.GetOBJECT_CATEGORYList(False, _page.EnableIISCaching)



                Dim critCatId As Integer = New CriteriaAnalyzer(crit).GetCategories().FirstOrDefault
                If critCatId <> 0 Then
                    crit.ExpandPropertiesList = PROPERTYList.GetAllPropertiesForCategory(critCatId, False, True).Cast(Of PROPERTYList.PROPERTYInfo).Select(Function(x) x.ID).ToList
                End If
            End If

            Dim results As DM_OBJECTSearch = DM_OBJECTSearch.GetOBJECTList(crit)
            progress.Start(results.LoadedItemsCount)
            For Each result As DM_OBJECTSearch.OBJECTInfo In results
                If progress.Cancelled Then Return Nothing

                Dim refobj As DM_OBJECTSearch.OBJECTInfo = Nothing
                ExtractShortCut(result, refobj)
                If Not refobj Is Nothing Then result = refobj

                Dim catProcID As Integer
                Dim lbIsCat As Boolean = True
                If isCases Then
                    catProcID = result.ProcedureID
                    lbIsCat = False
                Else
                    catProcID = result.Category.ID
                End If
                Dim tab As AutoTabs.AutoTab
                If Not loAutoTabs.ExistsTab(catProcID) Then

                    Dim lsName As String = ""
                    Dim props As PROPERTYList
                    If isCases Then
                        For Each proc As PROCEDUREList.PROCEDUREInfo In procs
                            If proc.ID = catProcID Then
                                lsName = loLabels.GetObjectLabel(proc.ID, lsCatProcType, proc.Name)
                                Exit For
                            End If
                        Next
                        props = PROPERTYList.GetAllPropertiesForProcedure(catProcID, False, False, "", True)
                    Else

                        For Each cat As OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In cats
                            If cat.ID = catProcID Then
                                lsName = loLabels.GetObjectLabel(cat.ID, lsCatProcType, cat.Name)
                                Exit For
                            End If
                        Next
                        props = PROPERTYList.GetAllPropertiesForCategory(catProcID, False, True)
                    End If

                    tab = loAutoTabs.CreateTab(_page, workbook, catProcID, lsName, props, lbIsCat)

                Else
                    tab = loAutoTabs.GetTab(catProcID)
                End If

                tab.CurrentRow += 1
                If lbIsCat Then
                    tab.worksheet.Cells(tab.CurrentRow, 0).Value = result.ID.ToString()
                Else
                    tab.worksheet.Cells(tab.CurrentRow, 0).Value = result.CaseID.ToString()
                End If

                tab.worksheet.Cells(tab.CurrentRow, 1).Value = SanitizeData(result.Name)
                Dim i As Integer
                If lbIsCat Then
                    tab.worksheet.Cells(tab.CurrentRow, 2).Value = ArcoFormatting.FormatDateLabel(result.Creation_Date, True, False, False)
                    tab.worksheet.Cells(tab.CurrentRow, 3).Value = ArcoFormatting.FormatUserName(result.Created_By, False, False)
                    tab.worksheet.Cells(tab.CurrentRow, 4).Value = ArcoFormatting.FormatDateLabel(result.Modif_Date, True, False, False)
                    tab.worksheet.Cells(tab.CurrentRow, 5).Value = ArcoFormatting.FormatUserName(result.Modif_By, False, False)
                    i = 6
                Else
                    tab.worksheet.Cells(tab.CurrentRow, 2).Value = ArcoFormatting.FormatDateLabel(result.Creation_Date, True, False, False)
                    tab.worksheet.Cells(tab.CurrentRow, 3).Value = ArcoFormatting.FormatUserName(result.Created_By, False, False)

                    tab.worksheet.Cells(tab.CurrentRow, 4).Value = ArcoFormatting.FormatDateLabel(result.Case_Due, True, False, False)
                    tab.worksheet.Cells(tab.CurrentRow, 5).Value = result.StepName
                    tab.worksheet.Cells(tab.CurrentRow, 6).Value = ArcoFormatting.FormatDateLabel(result.Step_Start, True, False, False)
                    tab.worksheet.Cells(tab.CurrentRow, 7).Value = ArcoFormatting.FormatDateLabel(result.Step_Due, True, False, False)

                    Dim assignedTo As String = ""
                    For Each w As DM_OBJECTSearch.OBJECTInfo.Object_WorkItem_Info In result.WorkAssignedTo
                        If assignedTo.Length > 0 Then assignedTo &= ", "
                        assignedTo &= w.DisplayName
                    Next
                    tab.worksheet.Cells(tab.CurrentRow, 8).Value = assignedTo
                    tab.worksheet.Cells(tab.CurrentRow, 9).Value = ArcoFormatting.FormatUserName(result.StepExecutor, False, False)
                    i = 10
                End If

                For Each prop As PROPERTYList.PROPERTYInfo In tab.Properties
                    If isPrintable(prop) Then


                        FillPropertyCell(tab.worksheet.Cells(tab.CurrentRow, i), result, prop.ID, prop, False)
                        i += 1
                    End If
                Next
                progress.Increment()
            Next
            loAutoTabs.AutoFit()
            Return workbook

        Catch ex As Exception
            Return Nothing
        End Try

    End Function


    Private Function SanitizeData(ByVal value As String) As String
        If String.IsNullOrEmpty(value) Then Return value

        While Not String.IsNullOrEmpty(value) AndAlso value.StartsWith("=")
            value = value.Substring(1)
        End While

        Return value
    End Function
    Private Function IsCaseSearch(ByVal crit As DM_OBJECTSearch.Criteria) As Boolean
        Select Case crit.ResultType
            Case DM_OBJECTSearch.eResultType.ArchivedCasesList, DM_OBJECTSearch.eResultType.CaseList, DM_OBJECTSearch.eResultType.OpenAndArchivedCasesList
                Return True
        End Select
        Return False
    End Function

    Private Sub FillPropertyCell(ByVal cell As Aspose.Cells.Cell, ByVal item As DM_OBJECTSearch.OBJECTInfo, ByVal propId As Integer, ByVal prop As PROPERTYList.PROPERTYInfo, ByVal wasExtracted As Boolean)
        Dim lbPropFound As Boolean = False

        Dim propInfo As DM_OBJECTSearch.OBJECTInfo.Object_Property_Info = item.Properties.Cast(Of DM_OBJECTSearch.OBJECTInfo.Object_Property_Info).FirstOrDefault(Function(x) x.ID = propId)
        If propInfo IsNot Nothing Then
            Dim propInfoValue As Object = propInfo.Value
            Select Case propInfo.Type
                Case PropertyTypes.TypeCodes.DATETIME
                    If propInfoValue IsNot Nothing Then
                        cell.Value = ArcoFormatting.FormatDateLabel(propInfoValue.ToString(), True, True, False)
                    Else
                        cell.Value = ""
                    End If
                Case PropertyTypes.TypeCodes.DATE
                    If propInfoValue IsNot Nothing Then
                        cell.Value = ArcoFormatting.FormatDateLabel(propInfoValue.ToString(), False, False, False)
                    Else
                        cell.Value = ""
                    End If
                Case PropertyTypes.TypeCodes.NUMBER, PropertyTypes.TypeCodes.AUTONUMBER
                    Dim iVal As Integer = Convert.ToInt32(propInfoValue)
                    If iVal = DM_PROPERTY.EmptyNumValue Then
                        cell.Value = ""
                    Else
                        Dim style As Aspose.Cells.Style = cell.GetStyle
                        style.Number = 1 '0
                        cell.SetStyle(style)
                        cell.Value = iVal
                    End If
                Case PropertyTypes.TypeCodes.DECIMAL
                    Dim dVal As Decimal = Convert.ToDecimal(propInfoValue)
                    If dVal = DM_PROPERTY.EmptyFloatValue Then
                        cell.Value = ""
                    Else
                        Dim style As Aspose.Cells.Style = cell.GetStyle
                        style.Number = 2 '0.00
                        cell.SetStyle(style)
                        cell.Value = dVal
                    End If
                Case PropertyTypes.TypeCodes.CURRENCY
                    cell.Value = GetPropertyDisplayValue(item, propId)
                Case PropertyTypes.TypeCodes.ASSIGNEE
                    If propInfoValue IsNot Nothing Then
                        cell.Value = ArcoFormatting.FormatUserName(propInfoValue.ToString(), True, False)
                    End If
                    'Case PropertyTypes.TypeCodes.BOOLEAN

                    '    If Convert.ToBoolean(propInfo.Value) Then
                    '        cell.Value = _page.GetLabel("yes")
                    '    Else
                    '        cell.Value = _page.GetLabel("no")
                    '    End If
                Case Else
                    If propInfoValue IsNot Nothing AndAlso Not String.IsNullOrEmpty(propInfoValue) Then
                        Dim lsValue As String
                        If Not wasExtracted Then
                            lsValue = GetPropertyDisplayValue(item, propId)
                        Else
                            lsValue = propInfo.DisplayValue.ToString()
                        End If
                        cell.Value = SanitizeData(Arco.Web.ResourceManager.ReplaceLabeltags(lsValue))
                    Else
                        cell.Value = ""
                    End If
            End Select
        ElseIf prop IsNot Nothing Then
            Dim propDefaultValue As Object = prop.DefaultValue
            Select Case prop.Type
                Case PropertyTypes.TypeCodes.DATETIME
                    If propDefaultValue IsNot Nothing Then
                        cell.Value = ArcoFormatting.FormatDateLabel(propDefaultValue.ToString(), True, True, False)
                    Else
                        cell.Value = ""
                    End If
                Case PropertyTypes.TypeCodes.DATE
                    If propDefaultValue IsNot Nothing Then
                        cell.Value = ArcoFormatting.FormatDateLabel(propDefaultValue.ToString(), False, False, False)
                    Else
                        cell.Value = ""
                    End If
                Case PropertyTypes.TypeCodes.NUMBER, PropertyTypes.TypeCodes.AUTONUMBER
                    Dim iVal As Integer = Convert.ToInt32(prop.DefaultValue)
                    If iVal = DM_PROPERTY.EmptyNumValue Then
                        cell.Value = ""
                    Else
                        Dim style As Aspose.Cells.Style = cell.GetStyle
                        style.Number = 1 '0
                        cell.SetStyle(style)
                        cell.Value = iVal
                    End If
                Case PropertyTypes.TypeCodes.DECIMAL
                    Dim dVal As Decimal = Convert.ToDecimal(prop.DefaultValue)
                    If dVal = DM_PROPERTY.EmptyFloatValue Then
                        cell.Value = ""
                    Else
                        Dim style As Aspose.Cells.Style = cell.GetStyle
                        style.Number = 2 '0.00
                        cell.SetStyle(style)
                        cell.Value = dVal
                    End If
                Case PropertyTypes.TypeCodes.CURRENCY
                    cell.Value = GetPropertyDisplayValue(item, prop.ID)
                Case PropertyTypes.TypeCodes.ASSIGNEE
                    If propDefaultValue IsNot Nothing Then
                        cell.Value = ArcoFormatting.FormatUserName(propDefaultValue.ToString(), True, False)
                    End If

                    'Case PropertyTypes.TypeCodes.BOOLEAN

                    '    If Convert.ToBoolean(propInfo.Value) Then
                    '        cell.Value = _page.GetLabel("yes")
                    '    Else
                    '        cell.Value = _page.GetLabel("no")
                    '    End If
                Case Else
                    If propDefaultValue IsNot Nothing Then
                        Dim lsValue As String = GetPropertyDisplayValue(item, prop.ID)

                        cell.Value = SanitizeData(Arco.Web.ResourceManager.ReplaceLabeltags(lsValue))
                    Else
                        cell.Value = ""
                    End If
            End Select
        Else
            cell.Value = ""
        End If
    End Sub

    Private Function GetPropertyDisplayValue(ByVal item As DM_OBJECTSearch.OBJECTInfo, ByVal propID As Int32) As String
        If item.TechID > 0 Then
            Return item.CurrentCase().GetPropertyDisplayValue(propID).ToString
        ElseIf item.CaseID > 0 Then
            Try
                If item.Case_Active Then
                    Return cCase.GetCaseByCaseID(item.CaseID).GetPropertyDisplayValue(propID).ToString
                Else
                    Return HistoryCase.GetHistoryCase(item.CaseID).GetPropertyDisplayValue(propID).ToString
                End If
            Catch ex As Exceptions.PropertyNotFoundException
                Return "Property not found (2)"
            End Try
        ElseIf item.ID > 0 Then
            Try
                Return item.BusinessObject().GetPropertyDisplayValue(propID).ToString
            Catch ex As Exception
                Return ex.Message
            End Try

        Else
            Return String.Empty
        End If
    End Function

    Private Shared Function isPrintable(ByVal loProp As PROPERTYList.PROPERTYInfo) As Boolean
        Return loProp.PROP_PRINTABLE AndAlso loProp.Type <> "TABLE" AndAlso loProp.Mode = baseObjects.DM_PROPERTY.ePropMode.Normal
    End Function

    Private Sub ExtractShortCut(ByVal obj As DM_OBJECTSearch.OBJECTInfo, ByRef refobj As DM_OBJECTSearch.OBJECTInfo)
        If obj.OBJECT_TYPE = "Shortcut" Then
            If obj.Object_Reference > 0 Then
                If obj.ReferencedObject IsNot Nothing Then
                    refobj = obj.ReferencedObject
                End If
            End If
        End If
    End Sub

    Public Function GetGridParams(ByVal voRes As DM_OBJECTSearch) As GridParams
        Dim params As New GridParams
        params.Settings = _page.Settings
        If Not voRes Is Nothing Then
            params.ResultType = voRes.ResultType
        End If

        params.GridRootID = 0
        params.UserProfile = _page.UserProfile
        params.GridClientID = _page.ClientID
        params.Results = voRes
        params.ShowFileContextMenus = False

        params.GridLimitedToStep = 0
        params.GridSelection = ""
        params.ExpandFileRows = False
        Return params
    End Function
End Class



Public Class AutoTabs
    Public Class AutoTab
        Public ID As Integer
        Public worksheet As Aspose.Cells.Worksheet
        Public Properties As PROPERTYList
        Public CurrentRow As Integer
    End Class
    Private ReadOnly _tabs As List(Of AutoTab) = New List(Of AutoTab)

    Public Sub AutoFit()
        For Each a As AutoTab In _tabs
            a.worksheet.AutoFitColumns()
        Next
    End Sub
    Public Function ExistsTab(ByVal vlID As Integer) As Boolean
        Dim bFound As Boolean = False
        For Each a As AutoTab In _tabs
            If a.ID = vlID Then
                bFound = True
                Exit For
            End If
        Next
        Return bFound
    End Function
    Private Function TrimSheetName(ByVal vsName As String) As String
        ': \ / ? * [  or ]
        vsName = vsName.Replace(":", "").Replace("\", "").Replace("/", "").Replace("*", "").Replace("[", "").Replace("]", "").Replace("?", "")
        If vsName.Length > 30 Then
            vsName = vsName.Substring(0, 30)
        End If

        Return vsName
    End Function
    Public Function CreateTab(ByVal voPage As BasePage, ByVal wb As Aspose.Cells.Workbook, ByVal vlID As Integer, ByVal vsName As String, ByVal voProps As PROPERTYList, ByVal vbIsCat As Boolean) As AutoTab


        Dim a As New AutoTab
        a.ID = vlID
        a.Properties = voProps
        vsName = TrimSheetName(vsName)

        If _tabs.Any Then


            Try
                a.worksheet = wb.Worksheets.Add(vsName)
            Catch ex As Exception
                a.worksheet = wb.Worksheets.Add(vsName & "(" & vlID & ")")
            End Try


        Else
            a.worksheet = wb.Worksheets(0)
            a.worksheet.Name = vsName
        End If
        Dim i As Integer
        a.worksheet.Cells(0, 0).Value = "Id"
        a.worksheet.Cells(0, 1).Value = voPage.GetLabel("docTitle")
        If vbIsCat Then
            a.worksheet.Cells(0, 2).Value = voPage.GetDecodedLabel("creationdate")
            a.worksheet.Cells(0, 3).Value = voPage.GetDecodedLabel("createdby")
            a.worksheet.Cells(0, 4).Value = voPage.GetDecodedLabel("modifdate")
            a.worksheet.Cells(0, 5).Value = voPage.GetDecodedLabel("modifby")
            i = 6
        Else
            a.worksheet.Cells(0, 2).Value = voPage.GetDecodedLabel("creationdate")
            a.worksheet.Cells(0, 3).Value = voPage.GetDecodedLabel("createdby")

            a.worksheet.Cells(0, 4).Value = voPage.GetDecodedLabel("duedate")
            a.worksheet.Cells(0, 5).Value = voPage.GetDecodedLabel("step")
            a.worksheet.Cells(0, 6).Value = voPage.GetDecodedLabel("stepdate")
            a.worksheet.Cells(0, 7).Value = voPage.GetDecodedLabel("stepduedate")

            a.worksheet.Cells(0, 8).Value = voPage.GetDecodedLabel("assignedto")
            a.worksheet.Cells(0, 9).Value = voPage.GetDecodedLabel("lockedby")
            i = 10
        End If

        a.CurrentRow = 0
        For Each loProp As PROPERTYList.PROPERTYInfo In voProps
            If isPrintable(loProp) Then
                Dim lsLabel As String
                Select Case Arco.Security.BusinessIdentity.CurrentIdentity.Language
                    Case "N"
                        lsLabel = loProp.LabelDutch
                    Case "F"
                        lsLabel = loProp.LabelFrench
                    Case "G"
                        lsLabel = loProp.LabelGerman
                    Case Else
                        lsLabel = loProp.LabelEnglish
                End Select
                If String.IsNullOrEmpty(lsLabel) Then
                    lsLabel = loProp.Name
                End If
                a.worksheet.Cells(0, i).Value = lsLabel
                i += 1
            End If
        Next

        _tabs.Add(a)

        Return a
    End Function
    Public Function GetTab(ByVal tabId As Integer) As AutoTab

        Return _tabs.FirstOrDefault(Function(x) x.ID = tabId)

    End Function
    Private Shared Function isPrintable(ByVal loProp As PROPERTYList.PROPERTYInfo) As Boolean
        Return loProp.PROP_PRINTABLE AndAlso loProp.Type <> "TABLE" AndAlso loProp.Mode = baseObjects.DM_PROPERTY.ePropMode.Normal
    End Function
End Class