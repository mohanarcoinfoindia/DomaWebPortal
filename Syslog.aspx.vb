


Partial Class Syslog
    Inherits BaseAdminOnlyPage


    Private mDefaultMaxResults As Int32

    Public ReadOnly Property NumberOfResultsLabel() As String
        Get
            Dim l As Int32 = Me.NumberOfResults
            If l >= mDefaultMaxResults Then
                Return " > " & l.ToString
            Else
                Return l.ToString
            End If
        End Get
    End Property
    Public Property CurrentPage() As Int32

    Public Property LastPage() As Int32

    Public Property RecordsPerPage() As Int32

    Private mResCount As Int32 = 0

    Public Property NumberOfResults() As Int32
        Get
            Return mResCount

        End Get
        Set(ByVal Value As Int32)
            mResCount = Value
        End Set
    End Property

    Protected Function GetObjectLinkUrl(ByVal voItem As Arco.Doma.Library.Logging.SysLogList.SysLogInfo) As String
        If voItem.ObjectID > 0 Then
            Return "<a href=""javascript:PC().OpenDetailWindow('dm_detail.aspx?DM_OBJECT_ID=" & voItem.ObjectID & "'," & voItem.ObjectID & ");"" class='ButtonLink'>View Object</a>"
        ElseIf voItem.ObjectDIN > 0 Then
            Return "<a href=""javascript:PC().OpenDetailWindow('dm_detail.aspx?DM_OBJECT_DIN=" & voItem.ObjectDIN & "'," & voItem.ObjectDIN & ");"" class='ButtonLink'>View Object</a>"
        Else
            Return "&nbsp;"
        End If

    End Function
    Private mcolJobs As Arco.Doma.Library.Jobs.JobList = Nothing
    Protected Function GetLevel(ByVal voItem As Arco.Doma.Library.Logging.SysLogList.SysLogInfo) As String
        Dim iconClass = ""
        Dim tooltip As String = voItem.Level.ToString
        Select Case voItem.Level
            Case Arco.Doma.Library.Logging.SysLog.LogLevel.Error
                iconClass = "icon icon-status-error"
            Case Arco.Doma.Library.Logging.SysLog.LogLevel.Warning
                iconClass = "icon icon-status-warning"
            Case Arco.Doma.Library.Logging.SysLog.LogLevel.Info
                iconClass = "icon icon-status-ok"

            Case Arco.Doma.Library.Logging.SysLog.LogLevel.Undefined
                Return ""
        End Select

        Return "<span class='" & iconClass & "' title='" & tooltip & "'/>"
    End Function
    Protected Function GetBatchJob(ByVal voItem As Arco.Doma.Library.Logging.SysLogList.SysLogInfo) As String
        If voItem.BatchID > 0 Then
            If mcolJobs Is Nothing Then
                mcolJobs = Arco.Doma.Library.Jobs.JobList.GetAllJobs
            End If
            For Each j As Arco.Doma.Library.Jobs.JobList.JobInfo In mcolJobs
                If j.ID = voItem.BatchID Then
                    Return j.Name
                End If
            Next
            Return "Not found"
            'ElseIf voItem.FileID > 0 Then
            '    Return GetLabel("file")
            'ElseIf voItem.ObjectID > 0 Then
            '    Return GetLabel("Object")
        ElseIf voItem.FileID > 0 Then
            Return "No Job (File Indexing)"
        Else
            Return "No Job"
        End If

    End Function
    Private Sub sLoadFilters()
        If Not Page.IsPostBack Then
            drpTime.Items.Clear()
            drpTime.Items.Add(New ListItem(GetDecodedLabel("today"), Arco.SystemClock.Now.ToString("yyyy-MM-dd")) With {.Selected = True})
            drpTime.Items.Add(New ListItem(GetDecodedLabel("lastweek"), Arco.SystemClock.Now.AddDays(-7).ToString("yyyy-MM-dd")))
            drpTime.Items.Add(New ListItem(GetDecodedLabel("lastmonth"), Arco.SystemClock.Now.AddDays(-30).ToString("yyyy-MM-dd")))
            drpTime.Items.Add(New ListItem("", ""))

            drpJob.Items.Clear()
            drpJob.Items.Add(New ListItem("", "0") With {.Selected = True})
            If mcolJobs Is Nothing Then
                mcolJobs = Arco.Doma.Library.Jobs.JobList.GetAllJobs
            End If
            For Each j As Arco.Doma.Library.Jobs.JobList.JobInfo In mcolJobs
                drpJob.Items.Add(New ListItem(j.Name, j.ID.ToString))
            Next

            drpLevel.Items.Clear()
            drpLevel.Items.Add(New ListItem("", "0") With {.Selected = True})
            '    drpLevel.Items.Add(New ListItem(GetLabel("okandwarnings"), "1"))
            drpLevel.Items.Add(New ListItem(GetDecodedLabel("warningsanderrors"), "2"))
            drpLevel.Items.Add(New ListItem(GetDecodedLabel("errors"), "3"))
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sLoadFilters()

        RecordsPerPage = Math.Max(ArcoInfoSettings.DefaultRecordsPerPage, 30)
        mDefaultMaxResults = ArcoInfoSettings.MaxResults


        Dim lsPage As String
        Dim llFirstRec As Int32
        Dim llLastRec As Int32

        lsPage = Request.Form("Page")
        If lsPage = "" Then
            lsPage = "1"
        End If

        llFirstRec = ((CInt(lsPage) - 1) * RecordsPerPage) + 1
        llLastRec = llFirstRec + RecordsPerPage - 1

        Dim loCrit As New Arco.Doma.Library.Logging.SysLogList.Criteria()
        loCrit.Range = Arco.Doma.Library.ListRangeRequest.Range(llFirstRec, llLastRec, mDefaultMaxResults)

        loCrit.MinLevel = Convert.ToInt32(drpLevel.SelectedValue)

        If Not String.IsNullOrEmpty(drpTime.SelectedValue) Then
            loCrit.FROMDATE = drpTime.SelectedValue
        End If
        loCrit.BatchID = Convert.ToInt32(drpJob.SelectedValue)
        loCrit.OrderBy = GetActualOrderBy()

        Dim lcolResults As Arco.Doma.Library.Logging.SysLogList = Arco.Doma.Library.Logging.SysLogList.GetSyslogList(loCrit)

        Dim loPagedDataSource As PagedDataSource


        If lcolResults.Any Then
            loPagedDataSource = New PagedDataSource
            loPagedDataSource.AllowPaging = True
            loPagedDataSource.PageSize = Me.RecordsPerPage
            loPagedDataSource.DataSource = lcolResults
            loPagedDataSource.CurrentPageIndex = CInt(lsPage) - 1

            Me.NumberOfResults = loPagedDataSource.DataSourceCount
            Me.CurrentPage = CInt(lsPage)
            Me.LastPage = loPagedDataSource.PageCount
            lstResults.DataSource = loPagedDataSource
            lstResults.DataBind()

            lnkPrev.Text = GetLabel("previous")
            lnkPrev.NavigateUrl = "javascript:Goto(" & CInt(lsPage) - 1 & ");"


            lnkNext.Text = GetLabel("next")
            lnkNext.NavigateUrl = "javascript:Goto(" & CInt(lsPage) + 1 & ");"

            lnkPrev.Enabled = Not loPagedDataSource.IsFirstPage
            lnkNext.Enabled = Not loPagedDataSource.IsLastPage

            litScroller.Text = PageScroller.GetPageScroller(CInt(lsPage), loPagedDataSource.PageCount).Render("Goto")
        End If

    End Sub

    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "LOG_LEVEL", "LOG_TIME", "BATCH_ID", "LOG_USER"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function
End Class
