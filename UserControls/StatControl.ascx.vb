Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Statistics
Imports Telerik.Web.UI
Imports Telerik.Web.UI.HtmlChart.PlotArea.Series

Partial Class UserControls_StatControl
    Inherits BaseUserControl

    Private Const TextPercentage As String = "#= kendo.format(\'{0:N2}\',percentage * 100) #%"
    Private Const TextLabel As String = "#=category#: #=value#"
    Private Const TextLabelWithPercentage As String = "#=category#: " & TextPercentage
    Private Const TextLabelWithSeries As String = "#=series.name#: #=category#: #=value#"
    Private Const TextLabelWithSeriesWithPercentage As String = "#=series.name#: #=category#: " & TextPercentage
    Private Const TextTooltip As String = TextLabel
    Private Const TextTooltipWithPercentage As String = TextTooltip & " (" & TextPercentage & ")"
    Private Const TextTooltipWithSeries As String = TextLabelWithSeries
    Private Const TextTooltipWithSeriesWithPercentage As String = TextTooltipWithSeries & " (" & TextPercentage & ")"
    Private Const TextLegend As String = "#=text#"
    Private Const TextLegendWithSeries As String = "#=series.name#: #=text#"

    Private Const FilterToday = ">-1"
    Private Const FilterLastWeek = ">-7"
    Private Const FilterLast2Weeks = ">-14"
    Private Const FilterLast3Weeks = ">-21"
    Private Const FilterThisMonth = "cm"
    Private Const FilterLastMonth = ">-30"
    Private Const FilterLast2Months = ">-60"
    Private Const FilterLast3Months = ">-91"
    Private Const FilterLast4Months = ">-121"
    Private Const FilterLast5Months = ">-152"
    Private Const FilterLast6Months = ">-182"
    Private Const FilterThisYear = "cy"
    Private Const FilterLastYear = ">-365"
    Private Const FilterDateRange = "daterange"
    Private Const FilterCustom = "custom"

    ' standard color 1 = #f4912e
    ' standard color 4 = #2faade
    ' standard color 10 = #1c6cc3
    Private ReadOnly ChartColors As Color() = {ColorTranslator.FromHtml("#00AEEF"),
                                               ColorTranslator.FromHtml("#d5dd37"),
                                               ColorTranslator.FromHtml("#8bc547"),
                                               ColorTranslator.FromHtml("#f4912e"),
                                               ColorTranslator.FromHtml("#9d1f63"),
                                               ColorTranslator.FromHtml("#62b04a"),
                                               ColorTranslator.FromHtml("#d3ad2b"),
                                               ColorTranslator.FromHtml("#f57336"),
                                               ColorTranslator.FromHtml("#9d0941"),
                                               ColorTranslator.FromHtml("#003665"),
                                               ColorTranslator.FromHtml("#008bbf"),
                                               ColorTranslator.FromHtml("#aab12c"),
                                               ColorTranslator.FromHtml("#6f9e39"),
                                               ColorTranslator.FromHtml("#c37425"),
                                               ColorTranslator.FromHtml("#7e194f"),
                                               ColorTranslator.FromHtml("#4e8d3b"),
                                               ColorTranslator.FromHtml("#a98a22"),
                                               ColorTranslator.FromHtml("#c45c2b"),
                                               ColorTranslator.FromHtml("#7e0734"),
                                               ColorTranslator.FromHtml("#002b51")}

    Private _colorDictionary As New Dictionary(Of String, Integer)
    Private _colorPos As Integer = 0
    Private _statData As StatDataList

#Region " Properties "

    Public Property QueryParams As NameValueCollection

    Public Property HideConfigurator As Boolean

    Public Property Filter As String
    Public Property GroupBy As String
    Public Property ChartType As Report.eChartType = Report.eChartType.Column
    Public Property ChartWidth As Unit

    Public Property ReportID As Int32
    Public Property Total As String

    Public Property OnClientSeriesClickedJavascript As String

    Public ReadOnly Property ClickUrl As String
        Get
            If ReportID <> 0 Then Return Report.ClickUrl
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property DateFilter As String
        Get
            Dim crit As New StatDataList.Criteria(0)
            SetDateFilterForQuery(crit)
            If Not String.IsNullOrEmpty(crit.StartDate) AndAlso Not String.IsNullOrEmpty(crit.EndDate) Then
                Return crit.StartDate & ";" & crit.EndDate
            ElseIf Not String.IsNullOrEmpty(crit.StartDate) Then
                Return crit.StartDate
            ElseIf Not String.IsNullOrEmpty(crit.EndDate) Then
                Return crit.EndDate
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Private ReadOnly Property AllowChanges As Boolean
        Get
            If (rowType.Visible AndAlso Report.CanChangeType) OrElse
               (rowGroupBy.Visible AndAlso Report.CanChangeGroupBy) OrElse
               (rowFilter.Visible AndAlso Report.CanChangeDateFilter) OrElse
               (rowShowPercentage.Visible AndAlso Report.CanChangeShowPercentage) OrElse
               (rowTotal.Visible AndAlso Report.CanChangeTotals) Then
                Return True
            Else
                Return False
            End If
        End Get
    End Property

    Private ReadOnly Property ChartTypeIsBar As Boolean
        Get
            Select Case ChartType
                Case Report.eChartType.Bar, Report.eChartType.StackedBar, Report.eChartType.StackedBar100
                    Return True
                Case Else
                    Return False
            End Select
        End Get
    End Property

    Private ReadOnly Property ChartTypeIsColumn As Boolean
        Get
            Select Case ChartType
                Case Report.eChartType.Column, Report.eChartType.StackedColumn, Report.eChartType.StackedColumn100
                    Return True
                Case Else
                    Return False
            End Select
        End Get
    End Property

    Private ReadOnly Property ShowPercentageAvailable As Boolean
        Get
            Return ChartType = Report.eChartType.Donut OrElse ChartType = Report.eChartType.Pie OrElse
                   ChartType = Report.eChartType.PivotGrid OrElse ChartType = Report.eChartType.PivotGridHorizontal
        End Get
    End Property

    Private _statDefinition As Statistic
    Private ReadOnly Property StatDefinition As Statistic
        Get
            If Report.StatID > 0 Then
                If _statDefinition Is Nothing Then
                    _statDefinition = Statistic.GetStatistic(Report.StatID)
                ElseIf _statDefinition.ID <> Report.StatID Then
                    _statDefinition = Statistic.GetStatistic(Report.StatID)
                End If
            Else
                _statDefinition = Nothing
            End If
            Return _statDefinition
        End Get
    End Property

#End Region

#Region " Init "

    Protected Sub UserControls_StatControl_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not Me.IsPostBack Then

            DateFrom.SelectedDate = Now()
            DateTo.SelectedDate = Now()

            If Not String.IsNullOrEmpty(OnClientSeriesClickedJavascript) Then
                ch1.OnClientSeriesClicked = OnClientSeriesClickedJavascript
            End If

            LoadReport()

            If Not AllowChanges Then
                HideConfigurator = True
            End If

            pnlSelect.Visible = Not HideConfigurator

            rowType.Visible = (Not HideConfigurator AndAlso Report.CanChangeType)
            rowGroupBy.Visible = (Not HideConfigurator AndAlso Report.CanChangeGroupBy)
            rowFilter.Visible = (Not HideConfigurator AndAlso Report.CanChangeGroupBy)
            rowShowPercentage.Visible = ShowPercentageAvailable AndAlso (Not HideConfigurator AndAlso Report.CanChangeShowPercentage)
            rowTotal.Visible = (Not HideConfigurator AndAlso Report.CanChangeTotals)


            LoadChartTypes()
            InitSource()

            btnRefresh.Text = "<span class='icon icon-refresh' style='margin-top:8px;margin-left:8px;'></span>"
            btnExportExcel.ImageUrl = ThemedImage.GetUrl("filetype-xls.svg", Me)

            SetLabels()

            LoadStats()
        Else
            LoadFormData()
        End If
        ScriptManager.GetCurrent(Page).RegisterPostBackControl(btnExportExcel)
    End Sub

    Private Function SanitizeSql(ByVal value As String) As String
        Return InputSanitizer.Sanitize(value, New List(Of String) From {" ", "'", "(", ")", "="})

    End Function

    Private Sub SetLabels()


        lblDate.Text = GetLabel("date")
        lblGroupBy.Text = GetLabel("groupby")

        lblTotals.Text = GetLabel("totals")

        lblShowPercentage.Text = String.Empty ' GetLabel("showpercentage")
        chkShowPercentage.Text = GetLabel("showpercentage")

        btnRefresh.ToolTip = GetDecodedLabel("refresh")
        DateFrom.DateInput.Label = GetLabel("fromdate")
        DateTo.DateInput.Label = GetLabel("todate")

    End Sub

    Private _report As Report
    Public ReadOnly Property Report As Report
        Get
            If _report Is Nothing Then
                _report = Report.GetReport(ReportID)
            End If
            Return _report
        End Get
    End Property

    Private Sub LoadFormData()


        ReportID = Convert.ToInt32(hdnreportid.Value)


        If drpBarType.Items.Count = 0 Then
            LoadChartTypes()
        End If
        ChartType = CType(drpBarType.SelectedValue, Report.eChartType)
        GroupBy = drpGroupBy.SelectedValue
        Filter = GetDateFilter()

    End Sub

#End Region

#Region " Chart Types "

    Private Sub AddChartType(ByVal veChartType As Report.eChartType)
        drpBarType.Items.Add(New ListItem(veChartType.ToString, Convert.ToInt32(veChartType).ToString) With {.Selected = (ChartType = veChartType)})
    End Sub

    Private Sub LoadChartTypes()
        drpBarType.Items.Clear()
        AddChartType(Report.eChartType.Column)
        AddChartType(Report.eChartType.StackedColumn)
        AddChartType(Report.eChartType.StackedColumn100)
        AddChartType(Report.eChartType.Bar)
        AddChartType(Report.eChartType.StackedBar)
        AddChartType(Report.eChartType.StackedBar100)
        AddChartType(Report.eChartType.Line)
        AddChartType(Report.eChartType.Area)
        AddChartType(Report.eChartType.Pie)
        AddChartType(Report.eChartType.Donut)
        AddChartType(Report.eChartType.PivotGrid)
        AddChartType(Report.eChartType.PivotGridHorizontal)
        'AddChartType(Report.eChartType.Table) ' in latere versie verder uitwerken
    End Sub

    Private Function NewSerie(ByVal type As Report.eChartType, ByVal markType As HtmlChart.MarkersType) As SeriesBase
        Select Case type
            Case Report.eChartType.Area
                Dim loAreaSeries As New AreaSeries
                loAreaSeries.MissingValues = HtmlChart.MissingValuesBehavior.Gap
                Return loAreaSeries
            Case Report.eChartType.Column
                Return New ColumnSeries
            Case Report.eChartType.StackedColumn
                Dim cs = New ColumnSeries With {
                    .Stacked = True,
                    .StackType = HtmlChart.Enums.HtmlChartStackType.Normal
                }
                cs.LabelsAppearance.Visible = False
                Return cs
            Case Report.eChartType.StackedColumn100
                Dim cs = New ColumnSeries With {
                    .Stacked = True,
                    .StackType = HtmlChart.Enums.HtmlChartStackType.Stack100
                }
                cs.LabelsAppearance.Visible = False
                Return cs
            Case Report.eChartType.Bar
                Return New BarSeries
            Case Report.eChartType.StackedBar
                Dim bs As New BarSeries With {
                    .Stacked = True,
                    .StackType = HtmlChart.Enums.HtmlChartStackType.Normal
                }
                bs.LabelsAppearance.Visible = False
                Return bs
            Case Report.eChartType.StackedBar100
                Dim bs As New BarSeries With {
                    .Stacked = True,
                    .StackType = HtmlChart.Enums.HtmlChartStackType.Stack100
                }
                bs.LabelsAppearance.Visible = False
                Return bs
            Case Report.eChartType.Line
                Dim loLineSeries As New LineSeries
                loLineSeries.MissingValues = HtmlChart.MissingValuesBehavior.Gap
                loLineSeries.MarkersAppearance.MarkersType = markType
                Return loLineSeries
            Case Else
                Return New ColumnSeries
        End Select
    End Function
    Private Function NewSerie(ByVal veType As Report.eChartType) As SeriesBase
        Return NewSerie(veType, HtmlChart.MarkersType.Circle)
    End Function

#End Region

#Region " Info label "

    Private Sub ShowInfo(ByVal text As String)
        lblInfo.Visible = True
        lblInfo.Text = text
    End Sub

    Private Sub ShowError(text As String)

        ClearChart(False)
        ShowInfo(text)

    End Sub

    Private Sub ClearInfo()
        lblInfo.Visible = False
        lblInfo.Text = ""
    End Sub

#End Region

#Region " Load/Save report "

    Private Sub LoadReport()

        hdnreportid.Value = ReportID.ToString

        If ReportID > 0 Then


            ChartType = Report.ChartType

            GroupBy = Report.GroupBy

            Filter = Report.Filter


            chkShowPercentage.Checked = Report.ShowPercentage

            Total = Report.Totals

        End If

    End Sub


#End Region

#Region " Load chart "

    Private Sub LoadStats()


        ClearChart(True)

        rowShowPercentage.Visible = ShowPercentageAvailable AndAlso (Not HideConfigurator AndAlso Report.CanChangeShowPercentage)

        rowButtons.Visible = (Not HideConfigurator AndAlso (rowTotal.Visible OrElse rowDateCustom.Visible OrElse rowDateRange.Visible))

        Dim lsGroupByFields As String = GroupBy
        Dim lbHasMultipleGroupings As Boolean = lsGroupByFields.Contains(",")

        'If lbHasMultipleGroupings Then
        '    If ChartType = Report.eChartType.Pie Then
        '        ShowError("Chart of type Pie doesn't support multiple groups")
        '        Exit Sub
        '    ElseIf ChartType = Report.eChartType.Donut Then
        '        ShowError("Chart of type Donut doesn't yet support multiple groups")
        '        Exit Sub
        '    End If
        'End If

        Dim loCrit As New StatDataList.Criteria(StatDefinition.ID)
        SetDateFilterForQuery(loCrit)
        loCrit.ExtraWhere = ReplaceQueryParams(Report.ExtraWhere, True)
        loCrit.OrderBy = OrderBy(lsGroupByFields)

        _statData = StatDataList.GetStatistics(loCrit)

        Select Case Me.ChartType
            Case Report.eChartType.Area, Report.eChartType.Line,
                 Report.eChartType.Column, Report.eChartType.StackedColumn, Report.eChartType.StackedColumn100,
                 Report.eChartType.Bar, Report.eChartType.StackedBar, Report.eChartType.StackedBar100
                LoadChart()
            Case Report.eChartType.Donut, Report.eChartType.Pie
                LoadDonutOrPieChart()
            Case Report.eChartType.PivotGrid, Report.eChartType.PivotGridHorizontal
                LoadPivotGrid()
            Case Report.eChartType.Table
                LoadTableGrid()
        End Select

    End Sub

    Public Property ObjectContext As DM_OBJECT

    Private Function ReplaceTags(ByVal text As String) As String
        If ObjectContext Is Nothing Then
            Return Arco.Doma.Library.TagReplacer.ReplaceTags(text)
        Else
            Return Arco.Doma.Library.TagReplacer.ReplaceTags(text, ObjectContext)
        End If
    End Function
    Private Function ReplaceQueryParams(text As String, extraWhereStatement As Boolean) As String

        If String.IsNullOrEmpty(text) Then Return text


        text = ReplaceTags(text)


        If QueryParams Is Nothing Then Return text

        Arco.Utils.Logging.Debug("Before " & text)

        Const beginChar As String = "{"
        Const endChar As String = "}"
        Dim posEnd As Integer = 0

        Dim posBegin As Integer = text.IndexOf(beginChar, StringComparison.Ordinal)
        If posBegin >= 0 Then posEnd = text.IndexOf(endChar, posBegin, StringComparison.Ordinal)

        Dim tryReplace As Boolean = posBegin >= 0 AndAlso posEnd > 0 AndAlso (posBegin < posEnd)
        While tryReplace
            Dim actualPosBegin As Integer = posBegin + beginChar.Length
            Dim key As String = text.Substring(actualPosBegin, posEnd - actualPosBegin)
            Dim value As String = HttpUtility.HtmlDecode(QueryParams.Get(key))
            If value Is Nothing Then
                value = String.Empty
            Else
                value = ReplaceTags(value)
                If extraWhereStatement Then
                    value = SanitizeSql(value.Replace("'", "''")) ' avoid sql injection
                End If
            End If
            Dim originalText As String = text
            text = text.Replace(beginChar & key & endChar, value)
            Dim isChanged As Boolean = Not text.Equals(originalText, StringComparison.CurrentCultureIgnoreCase)
            posBegin = text.IndexOf(beginChar, StringComparison.Ordinal)
            If posBegin >= 0 Then posEnd = text.IndexOf(endChar, posBegin, StringComparison.Ordinal)
            tryReplace = isChanged AndAlso posBegin >= 0 AndAlso posEnd > 0 AndAlso (posBegin < posEnd)
        End While

        Arco.Utils.Logging.Debug("After " & text)

        Return text

    End Function

    Private Sub ClearChart(ByVal vbVisible As Boolean)

        ch1.Visible = vbVisible
        ch1.PlotArea.Series.Clear()
        ch1.PlotArea.XAxis.Items.Clear()
        ch1.PlotArea.XAxis.AxisCrossingPoints.Clear()
        ch1.PlotArea.AdditionalYAxes.Clear()

        _colorDictionary = New Dictionary(Of String, Integer)
        _colorPos = 0

        pivot1.Visible = vbVisible
        pivot1.Fields.Clear()
        pivot1.DataSource = Nothing

        grid1.Visible = vbVisible
        grid1.Columns.Clear()
        grid1.MasterTableView.GroupByExpressions.Clear()
        grid1.DataSource = Nothing

        btnExportExcel.Visible = vbVisible

    End Sub

    Private Sub LoadDonutOrPieChart()

        pivot1.Visible = False
        grid1.Visible = False
        btnExportExcel.Visible = False

        Dim totalItems As List(Of ListItem) = GetSelectedTotals()
        Dim groupedData As IEnumerable(Of IGrouping(Of GroupByStatInfo, StatDataList.StatInfo)) = _statData.GroupBy(Function(gb) GroupByValue(gb, GroupBy))

        For Each totalItem As ListItem In totalItems
            Dim data As IEnumerable(Of PieSeriesItem) = groupedData.Select(Function(g) GetPieSeriesItem(g, GroupBy, totalItem.Value))
            Dim series As PieSeriesBase
            If ChartType = Report.eChartType.Donut Then
                series = New DonutSeries()
                If chkShowPercentage.Checked Then
                    CType(series, DonutSeries).LabelsAppearance.ClientTemplate = TextPercentage
                End If
            Else
                series = New PieSeries()
                If chkShowPercentage.Checked Then
                    CType(series, PieSeries).LabelsAppearance.ClientTemplate = If(totalItems.Count > 1, TextLabelWithSeriesWithPercentage, TextLabelWithPercentage)
                Else
                    CType(series, PieSeries).LabelsAppearance.ClientTemplate = If(totalItems.Count > 1, TextLabelWithSeries, TextLabel)
                End If
            End If
            series.Name = totalItem.Text
            series.TooltipsAppearance.ClientTemplate = If(totalItems.Count > 1, TextTooltipWithSeriesWithPercentage, TextTooltipWithPercentage)
            series.SeriesItems.AddRange(data)
            SetPieColor(series)
            ch1.PlotArea.Series.Add(series)
        Next

        SetChartProperties(StatDefinition.Name)

    End Sub

    Private Sub LoadChart()

        pivot1.Visible = False
        grid1.Visible = False
        btnExportExcel.Visible = False

        Dim lbFirst As Boolean = True
        Dim laTotals As List(Of ListItem) = GetSelectedTotals()

        Dim lcolGroupedData As IEnumerable(Of IGrouping(Of GroupByStatInfo, StatDataList.StatInfo)) = _statData.GroupBy(Function(gb) GroupByValue(gb, GroupBy))
        Dim lbHasMultipleTotals As Boolean = (laTotals.Count > 1)
        Dim lbHasMultipleGroupings As Boolean = GroupBy.Contains(",")
        Dim totalIndex As Integer = 0

        For Each itm As ListItem In laTotals
            Dim data As IEnumerable(Of ArcoSeriesItem) = lcolGroupedData.Select(Function(g) GetSeriesItem(g, GroupBy, itm.Value))

            Dim lsYAxis As String = ""

            If Not lbFirst Then
                ch1.PlotArea.XAxis.AxisCrossingPoints.Add(ch1.PlotArea.XAxis.Items.Count)
                Dim loExtraYAxis As New AxisY() With {.Name = itm.Value}
                loExtraYAxis.TitleAppearance.Text = itm.Text
                loExtraYAxis.AxisCrossingValue = 0
                ch1.PlotArea.AdditionalYAxes.Add(loExtraYAxis)
                lsYAxis = itm.Value
            Else
                ch1.PlotArea.XAxis.AxisCrossingPoints.Add(0)
                ch1.PlotArea.YAxis.TitleAppearance.Text = itm.Text
                If lbHasMultipleGroupings Then
                    For Each lsGroupBy As String In GroupBy.Split(",")
                        ch1.PlotArea.XAxis.TitleAppearance.Text = drpGroupBy.Items.FindByValue(lsGroupBy).Text
                        Exit For
                    Next
                Else
                    ch1.PlotArea.XAxis.TitleAppearance.Text = drpGroupBy.Items.FindByValue(GroupBy).Text
                End If
            End If

            Dim lsLabels As IEnumerable(Of String) = Nothing

            If lbFirst AndAlso data.Any Then
                'add unique labels
                lsLabels = data.GroupBy(Function(g) g.Label).Select(Function(x) x.Key)
                ch1.PlotArea.XAxis.DataLabelsField = "Label"
                ch1.PlotArea.XAxis.Items.AddRange(lsLabels.Select(Function(x) GetXAxisitem(x)))
                lbFirst = False
            End If

            If lbHasMultipleGroupings Then
                If lsLabels Is Nothing Then lsLabels = data.GroupBy(Function(g) g.Label).Select(Function(x) x.Key)
                For Each lsValue As String In data.GroupBy(Function(g) g.Name).OrderBy(Function(ob) ob.Key).Select(Function(x) x.Key)
                    Dim lsLocalValue As String = lsValue
                    Dim lcolFiltered As IEnumerable(Of ArcoSeriesItem) = data.Where(Function(x) x.Name = lsLocalValue)

                    Dim lcolSubSerie As IEnumerable(Of ArcoSeriesItem) = lsLabels.Select(Function(x) GetFilteredSeriesItem(x, lcolFiltered))

                    Dim lsSerieName As String = lsValue
                    If lbHasMultipleTotals Then
                        lsSerieName = itm.Text & ": " & lsSerieName
                    End If
                    AddSerie(lsSerieName, lsValue, lcolSubSerie, True, lsYAxis, totalIndex)
                Next
            Else
                AddSerie(itm.Text, itm.Text, data, lbHasMultipleTotals, lsYAxis, totalIndex)
            End If

            totalIndex += 1

        Next

        SetChartProperties(StatDefinition.Name)

    End Sub

#Region " PivotGrid "

    Private Sub LoadPivotGrid()

        ch1.Visible = False
        grid1.Visible = False

        pivot1.ShowRowHeaderZone = False
        pivot1.ShowColumnHeaderZone = False

        If GroupBy.Contains(",") Then
            Dim group1 As String = GroupBy.Split(",")(0)
            Dim group2 As String = GroupBy.Split(",")(1)
            Dim group2AsRow As Boolean = (Me.ChartType = Report.eChartType.PivotGrid)
            Dim group3 As String = String.Empty
            Dim group3AsRow As Boolean = (Me.ChartType = Report.eChartType.PivotGrid)
            If group1 = "MonthNumber" AndAlso group2 <> "Year" Then
                ' Insert year.
                group3 = group2
                group2 = group1
                group1 = "Year"
                group2AsRow = True
            ElseIf group2 = "MonthNumber" AndAlso group1 <> "Year" Then
                ' Insert year.
                group3 = group2
                group2 = "Year"
            End If
            AddPivotRowField(drpGroupBy.Items.FindByValue(group1).Text, group1)
            If Not String.IsNullOrEmpty(group2) Then
                If group2AsRow Then
                    AddPivotRowField(drpGroupBy.Items.FindByValue(group2).Text, group2)
                Else
                    AddPivotColumnField(drpGroupBy.Items.FindByValue(group2).Text, group2)
                End If
            End If
            If Not String.IsNullOrEmpty(group3) Then
                If group3AsRow Then
                    AddPivotRowField(drpGroupBy.Items.FindByValue(group3).Text, group3)
                Else
                    AddPivotColumnField(drpGroupBy.Items.FindByValue(group3).Text, group3)
                End If
            End If
        Else
            If Me.ChartType = Report.eChartType.PivotGrid Then
                AddPivotRowField(drpGroupBy.Items.FindByValue(GroupBy).Text, GroupBy)
            Else
                AddPivotColumnField(drpGroupBy.Items.FindByValue(GroupBy).Text, GroupBy)
            End If
        End If

        For Each itm As ListItem In GetSelectedTotals()
            AddPivotAggregateField(chkShowPercentage.Checked, itm.Text, itm.Value)
        Next

        'pivot1.PageSize = 20 ' zetten van page size doet web crashen :-(
        pivot1.ExportSettings.FileName = StatDefinition.Name
        pivot1.Rebind()
        pivot1.ToolTip = FormatText(Report.ChartTooltip)

    End Sub

    Private Sub AddPivotRowField(ByVal vsCaption As String, ByVal vsDataField As String)

        If Not String.IsNullOrEmpty(vsDataField) Then
            Dim field As New PivotGridRowField
            field.Caption = vsCaption
            field.DataField = vsDataField
            field.IsHidden = False
            pivot1.Fields.Add(field)
            pivot1.ShowRowHeaderZone = True
        End If

    End Sub

    Private Sub AddPivotColumnField(ByVal vsCaption As String, ByVal vsDataField As String)

        If Not String.IsNullOrEmpty(vsDataField) Then
            Dim field As New PivotGridColumnField
            field.Caption = vsCaption
            field.DataField = vsDataField
            field.IsHidden = False
            pivot1.Fields.Add(field)
            pivot1.ShowColumnHeaderZone = True
        End If

    End Sub

    Private Sub AddPivotAggregateField(ByVal showPercentage As Boolean, ByVal vsCaption As String, ByVal vsDataField As String)

        Dim loRowField As New PivotGridAggregateField
        loRowField.Caption = vsCaption
        loRowField.HeaderCellTemplate = New PivotAggregateHeader(vsCaption)
        loRowField.DataField = vsDataField
        loRowField.Aggregate = PivotGridAggregate.Sum
        If showPercentage Then loRowField.TotalFormat.TotalFunction = PivotGridTotalFunction.PercentOfColumnTotal
        loRowField.IsHidden = False
        pivot1.Fields.Add(loRowField)

    End Sub

    Public Class PivotAggregateHeader
        Implements ITemplate
        Public Sub New(ByVal vsLabel As String)
            msLabel = vsLabel
        End Sub
        Private msLabel As String
        Sub InstantiateIn(ByVal container As Control) _
              Implements ITemplate.InstantiateIn
            Dim lc As New Literal()
            lc.Text = msLabel
            container.Controls.Add(lc)
        End Sub
    End Class

#End Region

#Region " RadGrid "

    Private Sub LoadTableGrid()

        ch1.Visible = False
        pivot1.Visible = False

        Dim crit As New StatDataList.Criteria(0)
        SetDateFilterForQuery(crit)
        crit.StartDate = ConvertToDate(crit.StartDate)
        crit.EndDate = ConvertToDate(crit.EndDate)

        If String.IsNullOrEmpty(crit.EndDate) Then
            crit.EndDate = Now.ToString("yyyy-MM-dd")
        End If

        If drpGroupBy.Items.FindByValue("Year") IsNot Nothing Then
            If String.IsNullOrEmpty(crit.StartDate) OrElse
                CDate(crit.StartDate).Year <> CDate(crit.EndDate).Year Then
                AddGridField(drpGroupBy.Items.FindByValue("Year").Text, "Year")
            End If
        End If
        If drpGroupBy.Items.FindByValue("Month") IsNot Nothing Then
            If String.IsNullOrEmpty(crit.StartDate) OrElse
               CDate(crit.StartDate).Year <> CDate(crit.EndDate).Year OrElse
               CDate(crit.StartDate).Month <> CDate(crit.EndDate).Month Then
                AddGridField(drpGroupBy.Items.FindByValue("Month").Text, "Month")
            End If
        End If
        If drpGroupBy.Items.FindByValue("Day") IsNot Nothing Then
            AddGridField(drpGroupBy.Items.FindByValue("Day").Text, "Day")
        End If

        If Not String.IsNullOrEmpty(StatDefinition.Field1.Value) Then AddGridField(StatDefinition.Field1.Caption, "Field1")
        If Not String.IsNullOrEmpty(StatDefinition.Field2.Value) Then AddGridField(StatDefinition.Field2.Caption, "Field2")
        If Not String.IsNullOrEmpty(StatDefinition.Field3.Value) Then AddGridField(StatDefinition.Field3.Caption, "Field3")
        If Not String.IsNullOrEmpty(StatDefinition.Field4.Value) Then AddGridField(StatDefinition.Field4.Caption, "Field4")
        If Not String.IsNullOrEmpty(StatDefinition.Field5.Value) Then AddGridField(StatDefinition.Field5.Caption, "Field5")

        For Each itm As ListItem In GetSelectedTotals()
            AddGridField(itm.Text, itm.Value, True)
        Next

        grid1.MasterTableView.GroupByExpressions.Clear()

        If GroupBy.Contains(",") Then
            For Each lsGroupBy As String In GroupBy.Split(",")
                AddGroupByExpression(lsGroupBy)
            Next
        Else
            AddGroupByExpression(GroupBy)
        End If

        grid1.ExportSettings.FileName = StatDefinition.Name
        grid1.Rebind()
        grid1.ToolTip = FormatText(Report.ChartTooltip)

    End Sub

    Private Function ConvertToDate(value As String) As String

        If String.IsNullOrEmpty(value) Then Return String.Empty

        If value.Contains("=") Then
            value = value.Replace(">", "")
            value = value.Replace("<", "")
            value = value.Replace("=", "")
        Else
            If value.Contains(">") Then
                value = value.Replace(">", "")
                If IsDate(value) Then
                    value = CDate(value).AddMinutes(1).ToString("yyyy-MM-dd")
                End If
            ElseIf value.Contains("<") Then
                value = value.Replace("<", "")
                If IsDate(value) Then
                    value = CDate(value).AddMinutes(-1).ToString("yyyy-MM-dd")
                End If
            End If
        End If

        If Not IsDate(value) Then Return String.Empty

        Return value

    End Function

    Private Sub AddGridField(ByVal vsCaption As String, ByVal vsDataField As String)
        AddGridField(vsCaption, vsDataField, False)
    End Sub

    Private Sub AddGridField(ByVal vsCaption As String, ByVal vsDataField As String, aggregate As Boolean)

        If Not String.IsNullOrEmpty(vsDataField) Then
            Dim loRowField As New GridBoundColumn()
            loRowField.HeaderText = vsCaption
            loRowField.DataField = vsDataField
            loRowField.SortExpression = vsDataField
            If aggregate Then
                loRowField.Aggregate = GridAggregateFunction.Sum
                loRowField.FooterText = "Total: "
            End If
            grid1.Columns.Add(loRowField)
        End If

    End Sub

    Private Sub AddGroupByExpression(dataField As String)

        Try
            Dim col As GridColumn = grid1.Columns.FindByDataField(dataField)
            If col IsNot Nothing Then
                col.Visible = False
                Dim ggbe As New GridGroupByExpression(col)
                grid1.MasterTableView.GroupByExpressions.Add(ggbe)
            End If
        Catch ex As Telerik.Web.UI.GridException
            ' ignore error (FindByDataField throws GridException when dataField is not found)
        End Try

    End Sub

#End Region

#End Region

    Public Function GetColor(index As Integer) As Color
        Return ChartColors(index Mod ChartColors.Length)
    End Function

    Private Sub LoadFilterData()

        rowFilter.Visible = (Not HideConfigurator AndAlso Report.CanChangeDateFilter) AndAlso HasDateField()

        drpDate.Items.Clear()
        drpDate.Items.Add(New ListItem(GetLabel("today"), FilterToday))
        drpDate.Items.Add(New ListItem(GetDecodedLabel("lastweek"), FilterLastWeek))
        drpDate.Items.Add(New ListItem(GetLastXLabel("lastweeks", 2), FilterLast2Weeks))
        drpDate.Items.Add(New ListItem(GetLastXLabel("lastweeks", 3), FilterLast3Weeks))
        drpDate.Items.Add(New ListItem(GetDecodedLabel("thismonth"), FilterThisMonth))
        drpDate.Items.Add(New ListItem(GetDecodedLabel("lastmonth"), FilterLastMonth))
        drpDate.Items.Add(New ListItem(GetLastXLabel("lastmonths", 2), FilterLast2Months))
        drpDate.Items.Add(New ListItem(GetLastXLabel("lastmonths", 3), FilterLast3Months))
        drpDate.Items.Add(New ListItem(GetLastXLabel("lastmonths", 4), FilterLast4Months))
        drpDate.Items.Add(New ListItem(GetLastXLabel("lastmonths", 5), FilterLast5Months))
        drpDate.Items.Add(New ListItem(GetLastXLabel("lastmonths", 6), FilterLast6Months))
        drpDate.Items.Add(New ListItem(GetDecodedLabel("thisyear"), FilterThisYear))
        drpDate.Items.Add(New ListItem(GetDecodedLabel("lastyear"), FilterLastYear))
        drpDate.Items.Add(New ListItem(GetLabel("all"), ""))
        drpDate.Items.Add(New ListItem(GetDecodedLabel("daterange"), FilterDateRange))

        rowDateCustom.Visible = False
        rowDateRange.Visible = False

        If Filter Is Nothing Then Filter = FilterLastWeek
        If Filter.Contains(";") Then
            drpDate.Items.FindByValue(FilterDateRange).Selected = True
            Dim values As String() = Filter.Split(";")
            If IsDate(values(0)) Then
                DateFrom.SelectedDate = CDate(values(0))
            Else
                DateFrom.SelectedDate = Nothing
            End If
            If IsDate(values(1)) Then
                DateTo.SelectedDate = CDate(values(1))
            Else
                DateTo.SelectedDate = Nothing
            End If
            rowDateRange.Visible = rowFilter.Visible
        Else
            Dim itm As ListItem = drpDate.Items.FindByValue(Filter)
            If Not itm Is Nothing Then
                itm.Selected = True
            Else
                itm = drpDate.Items.FindByText(Filter)
                If Not itm Is Nothing Then
                    itm.Selected = True
                Else
                    drpDate.Items.Add(New ListItem("Custom", FilterCustom))
                    drpDate.Items.FindByValue(FilterCustom).Selected = True
                    rowDateCustom.Visible = rowFilter.Visible
                    txtDateCustom.Text = Filter
                End If
            End If
        End If

    End Sub

    Private Function GetLastXLabel(ByVal label As String, ByVal num As Int32) As String
        Return GetDecodedLabel(label).Replace("#NUM#", num)
    End Function

    Private Sub AddGroupByField(ByVal laGroupBy As List(Of ListItem), ByVal voField As StatField, ByVal vsName As String)
        If Not String.IsNullOrEmpty(voField.Value) Then
            Dim capt As String = voField.Caption
            If String.IsNullOrEmpty(capt) Then capt = vsName
            capt = Arco.Web.ResourceManager.ReplaceLabeltags(capt)
            laGroupBy.Add(New ListItem(capt, vsName))
        End If
    End Sub
    Private Sub AddGroupByFieldWithDay(ByVal laGroupBy As List(Of ListItem), ByVal voField As StatField, ByVal vsName As String)
        If Not String.IsNullOrEmpty(voField.Value) Then
            Dim capt As String = voField.Caption
            If String.IsNullOrEmpty(capt) Then capt = vsName
            capt = Arco.Web.ResourceManager.ReplaceLabeltags(capt)
            laGroupBy.Add(New ListItem(capt, "Day," & vsName))
        End If
    End Sub
    Private Sub AddTotalField(ByVal laTotals As List(Of ListItem), ByVal voField As StatField, ByVal vsName As String)
        If Not String.IsNullOrEmpty(voField.Value) Then
            Dim capt As String = voField.Caption
            If String.IsNullOrEmpty(capt) Then capt = vsName
            capt = Arco.Web.ResourceManager.ReplaceLabeltags(capt)
            laTotals.Add(New ListItem(capt, vsName))
        End If
    End Sub

    Private Sub LoadTotalData()
        chkTotals.Items.Clear()
        If Report.StatID <> 0 Then
            Dim laTotals As New List(Of ListItem)
            laTotals.Add(New ListItem(GetLabel("Total"), "Count"))
            AddTotalField(laTotals, StatDefinition.Total1, "Total1")
            AddTotalField(laTotals, StatDefinition.Total2, "Total2")

            chkTotals.Items.AddRange(laTotals.ToArray)

            For Each loItem As ListItem In chkTotals.Items
                loItem.Selected = String.IsNullOrEmpty(Total) OrElse Total.Contains(loItem.Value)
            Next
        End If
        rowTotal.Visible = chkTotals.Items.Count > 1 AndAlso (Not HideConfigurator AndAlso Report.CanChangeTotals)
    End Sub
    Private Sub LoadGroupByData()
        drpGroupBy.Items.Clear()
        If Report.StatID <> 0 Then
            If StatDefinition.GetSourceHandler().AllowDifferential Then
                rowGroupBy.Visible = (Not HideConfigurator AndAlso Report.CanChangeGroupBy)
                Dim laGroupBy As New List(Of ListItem)

                If HasDateField() Then
                    laGroupBy.Add(New ListItem(GetLabel("day"), "Day"))
                    laGroupBy.Add(New ListItem(GetLabel("month"), "Month"))
                    laGroupBy.Add(New ListItem(GetLabel("month") & " (number)", "MonthNumber"))
                    laGroupBy.Add(New ListItem(GetDecodedLabel("year"), "Year"))
                End If

                AddGroupByField(laGroupBy, StatDefinition.Field1, "Field1")
                AddGroupByField(laGroupBy, StatDefinition.Field2, "Field2")
                AddGroupByField(laGroupBy, StatDefinition.Field3, "Field3")
                AddGroupByField(laGroupBy, StatDefinition.Field4, "Field4")
                AddGroupByField(laGroupBy, StatDefinition.Field5, "Field5")

                For Each laItem As ListItem In laGroupBy
                    drpGroupBy.Items.Add(laItem)
                Next

                'now combine them
                For Each laItem As ListItem In laGroupBy
                    For Each laOtherItem As ListItem In laGroupBy
                        If Not laItem.Value.Equals(laOtherItem.Value) Then
                            Dim laCombinedItem As ListItem = New ListItem
                            laCombinedItem.Value = laItem.Value & "," & laOtherItem.Value
                            laCombinedItem.Text = laItem.Text & "," & laOtherItem.Text
                            drpGroupBy.Items.Add(laCombinedItem)
                        End If

                    Next
                Next
            Else

                Dim laGroupBy As New List(Of ListItem)
                laGroupBy.Add(New ListItem(GetLabel("day"), "Day"))

                AddGroupByFieldWithDay(laGroupBy, StatDefinition.Field1, "Field1")
                AddGroupByFieldWithDay(laGroupBy, StatDefinition.Field2, "Field2")
                AddGroupByFieldWithDay(laGroupBy, StatDefinition.Field3, "Field3")
                AddGroupByFieldWithDay(laGroupBy, StatDefinition.Field4, "Field4")
                AddGroupByFieldWithDay(laGroupBy, StatDefinition.Field5, "Field5")

                For Each laItem As ListItem In laGroupBy
                    drpGroupBy.Items.Add(laItem)
                Next

                If drpGroupBy.Items.Count > 1 Then
                    rowGroupBy.Visible = (Not HideConfigurator AndAlso Report.CanChangeGroupBy)
                Else
                    rowGroupBy.Visible = False
                End If
            End If
        End If

        Dim itm As ListItem = drpGroupBy.Items.FindByValue(GroupBy)
        If Not itm Is Nothing Then
            itm.Selected = True
        Else
            If drpGroupBy.Items.Count > 0 Then
                GroupBy = drpGroupBy.Items(0).Value
            Else
                GroupBy = String.Empty
            End If
        End If

    End Sub

    Private Function HasDateField() As Boolean

        If StatDefinition IsNot Nothing Then
            If StatDefinition.DirectData Then
                Dim handler = StatDefinition.GetSourceHandler()
                handler.InitParameters(StatDefinition)
                Return handler.HasDateField
            Else
                Return True
            End If
        Else
            Return False
        End If

    End Function

    Private Function GetDateFilter() As String

        If drpDate.SelectedValue = FilterCustom Then
            Return txtDateCustom.Text
        ElseIf drpDate.SelectedValue = FilterDateRange Then
            Dim dateRange As String = String.Empty
            If DateFrom.SelectedDate IsNot Nothing Then
                dateRange &= DateFrom.SelectedDate.Value.ToString("yyyy-MM-dd")
            End If
            dateRange &= ";"
            If DateTo.SelectedDate IsNot Nothing Then
                dateRange &= DateTo.SelectedDate.Value.ToString("yyyy-MM-dd")
            End If
            Return dateRange
        Else
            Return drpDate.SelectedValue
        End If

    End Function

    Private Sub SetDateFilterForQuery(crit As StatDataList.Criteria)

        Select Case drpDate.SelectedValue
            Case FilterToday
                crit.StartDate = ">=" & Now.ToString("yyyy-MM-dd")
            Case FilterLastWeek
                crit.StartDate = GetWeekFilterForQuery(1)
            Case FilterLast2Weeks
                crit.StartDate = GetWeekFilterForQuery(2)
            Case FilterLast3Weeks
                crit.StartDate = GetWeekFilterForQuery(3)
            Case FilterThisMonth
                crit.StartDate = ">=" & New DateTime(Now.Year, Now.Month, 1).ToString("yyyy-MM-dd")
            Case FilterLastMonth
                crit.StartDate = GetMonthFilterForQuery(1)
            Case FilterLast2Months
                crit.StartDate = GetMonthFilterForQuery(2)
            Case FilterLast3Months
                crit.StartDate = GetMonthFilterForQuery(3)
            Case FilterLast4Months
                crit.StartDate = GetMonthFilterForQuery(4)
            Case FilterLast5Months
                crit.StartDate = GetMonthFilterForQuery(5)
            Case FilterLast6Months
                crit.StartDate = GetMonthFilterForQuery(6)
            Case FilterThisYear
                crit.StartDate = ">=" & New DateTime(Now.Year, 1, 1).ToString("yyyy-MM-dd")
            Case FilterLastYear
                crit.StartDate = ">" & DateAdd(DateInterval.Year, -1, Now()).ToString("yyyy-MM-dd")
            Case FilterDateRange
                If DateFrom.SelectedDate IsNot Nothing Then
                    crit.StartDate = ">=" & DateFrom.SelectedDate.Value.ToString("yyyy-MM-dd")
                End If
                If DateTo.SelectedDate IsNot Nothing Then
                    crit.EndDate = "<" & DateAdd(DateInterval.Day, 1, DateTo.SelectedDate.Value).ToString("yyyy-MM-dd")
                End If
            Case FilterCustom
                crit.StartDate = txtDateCustom.Text
            Case Else
                crit.StartDate = drpDate.SelectedValue
        End Select

    End Sub

    Private Function GetWeekFilterForQuery(lastWeeks As Integer) As String
        Return ">" & DateAdd(DateInterval.Day, -(lastWeeks * 7), Now()).ToString("yyyy-MM-dd")
    End Function

    Private Function GetMonthFilterForQuery(lastMonths As Integer) As String
        Return ">" & DateAdd(DateInterval.Month, -lastMonths, Now()).ToString("yyyy-MM-dd")
    End Function

    Private Function GetSelectedTotals() As List(Of ListItem)
        Dim laTotals As New List(Of ListItem)
        For Each loTotalItem As ListItem In chkTotals.Items
            If loTotalItem.Selected Then
                laTotals.Add(loTotalItem)
            End If
        Next
        Return laTotals
    End Function

    Private Function GetXAxisitem(ByVal vsKey As String) As AxisItem
        Return New AxisItem(vsKey)
    End Function

    Private Function GetCount(ByVal voStatInfo As StatDataList.StatInfo, ByVal vsWhich As String) As Decimal
        Select Case vsWhich
            Case "Total1"
                Return voStatInfo.Total1
            Case "Total2"
                Return voStatInfo.Total2
            Case Else
                Return voStatInfo.Count
        End Select
    End Function

    Private Function GetPieSeriesItem(g As IGrouping(Of GroupByStatInfo, StatDataList.StatInfo),
                                      groupByFields As String,
                                      totalField As String) As PieSeriesItem

        Dim sum As Decimal = Math.Round(g.Sum(Function(s) GetCount(s, totalField)), 2)

        Return New PieSeriesItem(sum) With
        {
            .Name = GetFieldDisplayValue(g, groupByFields, True)
        }

    End Function

    Private Function GetSeriesItem(g As IGrouping(Of GroupByStatInfo, StatDataList.StatInfo),
                                   groupByFields As String,
                                   totalField As String) As ArcoSeriesItem

        Dim sum As Decimal = Math.Round(g.Sum(Function(s) GetCount(s, totalField)), 2)

        Return New ArcoSeriesItem(sum) With
        {
            .Label = GetFieldDisplayValue(g, groupByFields, False),
            .TooltipValue = GetFieldDisplayValue(g, groupByFields, True),
            .Name = GetFieldDisplayValue(g, groupByFields, True)
        }

    End Function

    Private Function GetFilteredSeriesItem(ByVal vsLabel As String, lcolFiltered As IEnumerable(Of ArcoSeriesItem)) As ArcoSeriesItem

        Dim loFiltered As ArcoSeriesItem = lcolFiltered.FirstOrDefault(Function(x) x.Label = vsLabel)

        If Not loFiltered Is Nothing Then
            Return loFiltered
        Else
            If ChartTypeIsColumn OrElse ChartTypeIsBar OrElse
               Me.ChartType = Report.eChartType.Pie OrElse Me.ChartType = Report.eChartType.Donut Then
                Return New ArcoSeriesItem(0) With {.Label = vsLabel}
            Else
                Return New ArcoSeriesItem(Nothing) With {.Label = vsLabel}
            End If
        End If

    End Function

    Private Function GetFieldDisplayValue(ByVal vsVal As String, ByVal voField As StatField) As String

        If String.IsNullOrEmpty(vsVal) Then
            Return GetLabel("emptygroupervalue")
        End If

        If voField.Value = "USER_ID" OrElse voField.Value = "#USER_ID#" Then
            vsVal = ArcoFormatting.FormatUserName(vsVal)
        End If

        vsVal = vsVal.Replace("\", "\\") ' escaping backslash

        Return vsVal

    End Function

    Private Function GetFieldDisplayValue(ByVal voItem As IGrouping(Of GroupByStatInfo, StatDataList.StatInfo), ByVal vsField As String, ByVal vbReverse As Boolean) As String
        Dim laFields() As String = vsField.Split(","c)
        If vbReverse Then
            Array.Reverse(laFields)
        End If
        For Each lsGroupBy As String In laFields
            Select Case lsGroupBy
                Case "Field1"
                    Return GetFieldDisplayValue(voItem.Key.Field1, StatDefinition.Field1)
                Case "Field2"
                    Return GetFieldDisplayValue(voItem.Key.Field2, StatDefinition.Field2)
                Case "Field3"
                    Return GetFieldDisplayValue(voItem.Key.Field3, StatDefinition.Field3)
                Case "Field4"
                    Return GetFieldDisplayValue(voItem.Key.Field4, StatDefinition.Field4)
                Case "Field5"
                    Return GetFieldDisplayValue(voItem.Key.Field5, StatDefinition.Field5)
                Case "Month"
                    Return voItem.Key.Month
                Case "MonthNumber"
                    Return voItem.Key.MonthNumber
                Case "Year"
                    Return voItem.Key.Year
                Case "Day"
                    Return voItem.Key.Day
                Case Else
                    Return "error"
            End Select
        Next
        Return "no fields"
    End Function

    Private Function GroupByValue(ByVal item As StatDataList.StatInfo, ByVal fields As String) As GroupByStatInfo

        Dim loRet As New GroupByStatInfo

        For Each groupBy As String In fields.Split(","c)
            Select Case groupBy
                Case "Field1"
                    loRet.Field1 = item.Field1
                Case "Field2"
                    loRet.Field2 = item.Field2
                Case "Field3"
                    loRet.Field3 = item.Field3
                Case "Field4"
                    loRet.Field4 = item.Field4
                Case "Field5"
                    loRet.Field5 = item.Field5
                Case "Month"
                    loRet.Month = item.Month
                Case "MonthNumber"
                    loRet.MonthNumber = item.MonthNumber
                Case "Year"
                    loRet.Year = item.Year
                Case "Day"
                    loRet.Day = item.Day
                Case Else
                    Throw New NotImplementedException(groupBy & " is not implemented")
            End Select
        Next
        Return loRet
    End Function

    Private Function OrderBy(ByVal vsField As String) As String
        Dim sbGroupBy As List(Of String) = New List(Of String)
        For Each lsGroupBy As String In vsField.Split(","c)
            Select Case lsGroupBy
                Case "Month", "Day", "Year", "MonthNumber"
                    If Not sbGroupBy.Contains("STAT_DATE") Then sbGroupBy.Add("STAT_DATE")
                Case Else
                    sbGroupBy.Add("STAT_" & lsGroupBy)
            End Select
        Next
        Return String.Join(",", sbGroupBy)
    End Function

    Private Sub AddSerie(ByVal seriesName As String, ByVal categoryName As String, ByVal data As IEnumerable(Of ArcoSeriesItem), ByVal vbIsSub As Boolean, ByVal vsYAxis As String, ByVal totalIndex As Integer)

        Dim serie As SeriesBase

        If totalIndex > 0 AndAlso
           (ChartType = Report.eChartType.StackedColumn OrElse ChartType = Report.eChartType.StackedColumn100 OrElse
            ChartType = Report.eChartType.StackedBar OrElse ChartType = Report.eChartType.StackedBar100 OrElse
            ChartType = Report.eChartType.Line) Then
            serie = NewSerie(Report.eChartType.Line, IIf(totalIndex = 1, HtmlChart.MarkersType.Square, HtmlChart.MarkersType.Triangle)) ' draw a line with second or third total values
            CType(serie, LineSeries).LabelsAppearance.Visible = False
        Else
            serie = NewSerie(CType(drpBarType.SelectedValue, Report.eChartType))
        End If

        serie.Name = seriesName
        serie.TooltipsAppearance.ClientTemplate = If(Not vbIsSub, TextTooltip, TextTooltipWithSeries)
        serie.DataFieldY = "YValue"
        If Not String.IsNullOrEmpty(vsYAxis) Then serie.AxisName = vsYAxis

        serie.Items.AddRange(data)

        SetColor(serie, categoryName)

        ch1.PlotArea.Series.Add(serie)

    End Sub

    Private Sub SetPieColor(serie As PieSeriesBase)
        For Each item As PieSeriesItem In serie.SeriesItems
            item.BackgroundColor = GetColor(GetColorIndex(item.Name))
        Next
    End Sub

    Private Sub SetColor(serie As SeriesBase, categoryName As String)
        serie.Appearance.FillStyle.BackgroundColor = GetColor(GetColorIndex(categoryName))
    End Sub

    Private Function GetColorIndex(categoryName As String) As Integer

        Dim index As Integer
        If _colorDictionary.TryGetValue(categoryName, index) Then
        Else
            index = _colorPos
            _colorDictionary.Add(categoryName, index)
            _colorPos += 1
        End If
        Return index

    End Function

    Private Sub SetChartProperties(ByVal vsName As String)

        If ChartWidth <> Unit.Empty Then
            ch1.Width = ChartWidth
        End If

        Dim labels As Arco.Doma.Library.Globalisation.LABELList = Arco.Doma.Library.Globalisation.LABELList.GetReportsLabelList(EnableIISCaching)
        Dim defaultTitle As String
        If Not String.IsNullOrEmpty(Report.Title) Then
            defaultTitle = Report.Title
        Else
            defaultTitle = Report.Name
        End If
        Dim title As String = labels.GetObjectLabel(Report.ID, "Report", defaultTitle)
        If Not String.IsNullOrEmpty(title) Then
            ch1.ChartTitle.Text = FormatText(title)
        Else
            ch1.ChartTitle.Text = FormatText(vsName)
        End If
        ch1.ToolTip = FormatText(labels.GetObjectLabel(Report.ID, "ReportHelp", Report.ChartTooltip))

        ch1.Legend.Appearance.Visible = Report.ShowLegend ' (ch1.PlotArea.Series.Count > 1 AndAlso ChartType <> Report.eChartType.Pie) OrElse  ChartType = Report.eChartType.Donut
        If ChartType = Report.eChartType.Donut AndAlso ch1.PlotArea.Series.Count > 1 Then
            ch1.Legend.Appearance.ClientTemplate = TextLegendWithSeries
        Else
            ch1.Legend.Appearance.ClientTemplate = TextLegend
        End If

        If ChartTypeIsBar Then
            ch1.PlotArea.XAxis.LabelsAppearance.RotationAngle = 0
        Else
            If ch1.PlotArea.XAxis.Items.Count > 5 Then ch1.PlotArea.XAxis.LabelsAppearance.RotationAngle = 45
            If ch1.PlotArea.XAxis.Items.Count > 10 Then ch1.PlotArea.XAxis.LabelsAppearance.RotationAngle = 90
        End If

    End Sub

    Private Sub InitSource()

        LoadFilterData()
        LoadGroupByData()
        LoadTotalData()

    End Sub

    Private Function FormatText(text As String) As String

        If String.IsNullOrWhiteSpace(text) Then Return text
        Return EncodingUtils.EncodeJsString(ReplaceQueryParams(Arco.Web.ResourceManager.ReplaceLabeltags(text), False), False)

    End Function

#Region " Events "

    Protected Sub drpGroupBy_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drpGroupBy.SelectedIndexChanged
        ClearInfo()
        LoadStats()
    End Sub

    Protected Sub drpDate_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drpDate.SelectedIndexChanged

        If drpDate.SelectedValue = FilterCustom Then
            rowDateCustom.Visible = True
            txtDateCustom.Text = "0"
            rowDateRange.Visible = False
        ElseIf drpDate.SelectedValue = FilterDateRange Then
            rowDateRange.Visible = True
            rowDateCustom.Visible = False
        Else
            rowDateCustom.Visible = False
            rowDateRange.Visible = False
        End If

        ClearInfo()
        LoadStats()

    End Sub

    Protected Sub drpBarType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drpBarType.SelectedIndexChanged
        ClearInfo()
        LoadStats()
    End Sub

    Private Sub chkShowPercentage_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPercentage.CheckedChanged
        ClearInfo()
        LoadStats()
    End Sub

    Protected Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        ClearInfo()
        LoadStats()
    End Sub


    Protected Sub RadPivotGrid1_NeedDataSource(sender As Object, e As PivotGridNeedDataSourceEventArgs)

        If pivot1.Visible Then
            TryCast(sender, RadPivotGrid).DataSource = _statData
        End If

    End Sub

    Protected Sub pivot1_PageIndexChanged(sender As Object, e As PivotGridPageChangedEventArgs) Handles pivot1.PageIndexChanged
        LoadStats()
    End Sub
    Protected Sub pivot1_CellDataBound(sender As Object, e As PivotGridCellDataBoundEventArgs) Handles pivot1.CellDataBound
        e.Cell.Wrap = False
    End Sub
    Protected Sub pivot1_PageSizeChanged(sender As Object, e As PivotGridPageSizeChangedEventArgs) Handles pivot1.PageSizeChanged
        LoadStats()
    End Sub
    Protected Sub pivot1_Sorting(sender As Object, e As PivotGridSortEventArgs) Handles pivot1.Sorting
        LoadStats()
    End Sub

    Protected Sub RadGrid1_NeedDataSource(sender As Object, e As GridNeedDataSourceEventArgs)

        If grid1.Visible Then
            TryCast(sender, RadGrid).DataSource = _statData
        End If

    End Sub

    Protected Sub grid1_PageIndexChanged(sender As Object, e As GridPageChangedEventArgs) Handles grid1.PageIndexChanged
        LoadStats()
    End Sub

    Private Sub grid1_PageSizeChanged(sender As Object, e As GridPageSizeChangedEventArgs) Handles grid1.PageSizeChanged
        LoadStats()
    End Sub

    Private Sub grid1_SortCommand(sender As Object, e As GridSortCommandEventArgs) Handles grid1.SortCommand
        LoadStats()
    End Sub

    'Private Sub grid1_ItemDataBound(sender As Object, e As GridItemEventArgs) Handles grid1.ItemDataBound

    '    If TypeOf e.Item Is GridGroupFooterItem Then
    '        Dim groupFooter As GridGroupFooterItem = CType(e.Item, GridGroupFooterItem)
    '        groupFooter.Text = "Subtotal: " + groupFooter.text
    '    End If

    'End Sub

    Private Sub grid1_GroupsChanging(sender As Object, e As GridGroupsChangingEventArgs) Handles grid1.GroupsChanging
        LoadStats()
    End Sub

    Protected Sub btnExportExcel_Click(sender As Object, e As ImageClickEventArgs) Handles btnExportExcel.Click

        LoadStats()

        If pivot1.Visible Then
            pivot1.ExportSettings.IgnorePaging = True
            pivot1.ExportToExcel()
        ElseIf grid1.Visible Then
            grid1.ExportSettings.IgnorePaging = True
            grid1.ExportToExcel()
        End If

    End Sub

#End Region

    Private Structure GroupByStatInfo

        Public Day As String
        Public Month As String
        Public MonthNumber As String
        Public Year As String

        Public Field1 As String
        Public Field2 As String
        Public Field3 As String
        Public Field4 As String
        Public Field5 As String

    End Structure

    Public Class ArcoSeriesItem
        Inherits Telerik.Web.UI.SeriesItem

        Public Property Label As String

        Public Sub New(ByVal yValue As Decimal?)
            Me.YValue = yValue
        End Sub

    End Class

End Class