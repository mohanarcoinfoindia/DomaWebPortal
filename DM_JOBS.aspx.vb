Imports Arco.Doma.Library.Jobs
Imports Arco.Doma.ImportExport.Drum

Partial Class DM_JOBS
    Inherits BaseOperatorPage

    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)


        JobDetailsViewWindow.VisibleOnPageLoad = False
        Title = "Batch jobs"


        hdnOrderBy.Value = GetActualOrderBy()

        If Not IsPostBack OrElse Not RowCommandClicked() Then
            '    Response.Write("init databind " & Request.Form("__EVENTTARGET") & " " & GridView1.UniqueID & "<br>")
            GridView1.DataBind()
        End If

        Translate()

        SetPauzeResumeLinks()
    End Sub

    Private Function RowCommandClicked() As Boolean
        Try
            Return Request.Form("__EVENTTARGET").StartsWith(GridView1.UniqueID)
        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Sub Translate()
        lnkNewDrumJob.Attributes.Add("title", String.Concat(GetLabel("add"), " (Drum)"))

        DetailsView1.Fields(0).HeaderText = GetLabel("name")
        DetailsView1.Fields(1).HeaderText = GetLabel("description")

        DetailsView1.Fields(4).HeaderText = GetLabel("lastsuccess")
        DetailsView1.Fields(5).HeaderText = GetLabel("last")
        DetailsView1.Fields(6).HeaderText = GetLabel("next")

        drpAssemblies.Items(0).Text = GetLabel("all")
        drpAssemblies.Items(1).Text = GetDecodedLabel("system")
        drpAssemblies.Items(3).Text = GetDecodedLabel("custom")

        drpExecStatus.Items(0).Text = GetLabel("all")
        drpExecStatus.Items(1).Text = GetDecodedLabel("idle")
        drpExecStatus.Items(2).Text = GetDecodedLabel("pending")
        drpExecStatus.Items(3).Text = GetDecodedLabel("inerror")

        drpJobStatus.Items(0).Text = GetLabel("all")
        drpJobStatus.Items(1).Text = GetDecodedLabel("enabled")
        drpJobStatus.Items(2).Text = GetDecodedLabel("disabled")
        drpJobStatus.Items(3).Text = GetDecodedLabel("blocked")

        lnkResumeJP.Text = GetDecodedLabel("here")
        lnkPauzeJP.Text = GetDecodedLabel("pausejobs")

        chkExpandDrumJobs.Text = GetDecodedLabel("expanddrumjobs")
    End Sub

    Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles GridView1.RowCommand
        Dim arg As Integer = Convert.ToInt32(e.CommandArgument)

        Select Case e.CommandName
            Case "cmdExecuteJob"
                Job.ExecuteJob(arg)
            Case "cmdBlockJob"
                Job.BlockJob(arg)
            Case "cmdEnableJob"
                Job.EnableJob(arg)
            Case "cmdDisableJob"
                Job.DisableJob(arg)
            Case "cmdStopJob"
                RunningJobs.StopJob(arg)
            Case "cmdExecuteDrumJob"
                Dim drumJob As InputStreamBatchJob = InputStreamBatchJob.GetISBatchJob(arg)
                Job.ExecuteJob(drumJob.JOB_ID, arg.ToString)
            Case "cmdEnableDrumJob"
                Dim drumJob As InputStreamBatchJob = InputStreamBatchJob.GetISBatchJob(arg)
                drumJob.Enable()
            Case "cmdDisableDrumJob"
                Dim drumJob As InputStreamBatchJob = InputStreamBatchJob.GetISBatchJob(arg)
                drumJob.Disable()
            Case Else
                JobDetailsViewWindow.VisibleOnPageLoad = True 'Row was clicked, show detailsview modal (needs to be VisibleOnPageLoad because details view is called after a postback)
        End Select

        GridView1.DataBind()
    End Sub

    Enum JobCells
        ExecutionStatus = 1
        Name = 2
        Server = 3
        LastSuccessDate = 4
        LastDate = 5
        NextDate = 6

        AverageSeconds = 7
        ExecuteButton = 8
        ToggleButton = 9
        AddButton = 10
        Logviewer = 11
    End Enum

    Protected Sub GridView1_RowCreated(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowCreated
        Dim r As GridViewRow = e.Row
        If r.RowType = DataControlRowType.Header Then
            r.Cells(JobCells.Name).Text = GetLabel("name")
            r.Cells(JobCells.LastSuccessDate).Text = GetLabel("lastsuccess")
            r.Cells(JobCells.LastDate).Text = GetLabel("last")
            r.Cells(JobCells.NextDate).Text = GetLabel("next")
        ElseIf r.RowType = DataControlRowType.DataRow Then
            Dim i As Int32
            For Each cell As TableCell In e.Row.Cells
                Select Case i
                    Case JobCells.ExecuteButton, JobCells.ToggleButton, JobCells.Logviewer, JobCells.AddButton

                    Case Else
                        cell.Attributes.Add("onclick", ClientScript.GetPostBackEventReference(GridView1, "Select$" + e.Row.RowIndex.ToString()))
                        'cell.Attributes.Add("onclick", ClientScript.GetCallbackEventReference(GridView1, "Select$" + e.Row.RowIndex.ToString(), "javascript:showDetailsDrumJob()", Me)
                        cell.Style.Add(HtmlTextWriterStyle.Cursor, "pointer")
                        cell.Attributes.Add("title", "Select")
                End Select

                i += 1
            Next
        End If
    End Sub
    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        Dim r As GridViewRow = e.Row
        If r.RowType = DataControlRowType.DataRow Then
            Dim jobinfo As JobList.JobInfo = DirectCast(r.DataItem, JobList.JobInfo)

            r.Cells(JobCells.LastDate).Text = ArcoFormatting.FormatDateLabel(r.Cells(JobCells.LastDate).Text, True, True, False)
            r.Cells(JobCells.NextDate).Text = ArcoFormatting.FormatDateLabel(r.Cells(JobCells.NextDate).Text, True, True, False)

            Dim lsLogViewer As String = r.Cells(JobCells.Logviewer).Text
            If lsLogViewer <> "&nbsp;" Then
                lsLogViewer = "<a href='javascript:OpenLog(" & EncodingUtils.EncodeJsString(lsLogViewer & jobinfo.ID) & ");' class='ButtonLink'>Log</a>"
                r.Cells(JobCells.Logviewer).Text = lsLogViewer
            End If


            Dim b2 As LinkButton = CType(r.Cells(JobCells.ExecuteButton).Controls(0), LinkButton)
            b2.CommandArgument = jobinfo.ID.ToString
            If jobinfo.ExecutionStatus <> Job.JobExecutionStatus.Pending OrElse jobinfo.RunTimeStatus = JobRunTimeStatus.Done Then
                b2.Text = GetLabel("execute")
                b2.ToolTip = String.Format(GetDecodedLabel("executejob"), jobinfo.Name)
                b2.CommandName = "cmdExecuteJob"
            Else
                b2.Text = "Request stop"
                b2.CommandName = "cmdStopJob"
            End If


            Dim b1 As LinkButton = CType(r.Cells(JobCells.ToggleButton).Controls(1), LinkButton)
            Dim btnBlock As LinkButton = CType(r.Cells(JobCells.ToggleButton).Controls(3), LinkButton)

            Dim lsCMD As String
            Dim lsTXT As String
            Dim lsTlt As String
            If Not jobinfo.Status = Job.JobStatus.Enabled Then
                lsCMD = "cmdEnableJob"
                lsTXT = GetLabel("enable")
                lsTlt = String.Format(GetDecodedLabel("enablejob"), jobinfo.Name)
                btnBlock.Visible = False
            Else
                btnBlock.CommandName = "cmdBlockJob"
                btnBlock.CommandArgument = jobinfo.ID.ToString
                btnBlock.Text = GetLabel("block")
                btnBlock.ToolTip = GetDecodedLabel("block") & " " & jobinfo.Name

                lsCMD = "cmdDisableJob"
                lsTXT = GetLabel("disable")
                lsTlt = String.Format(GetDecodedLabel("disablejob"), jobinfo.Name)
            End If

            b1.CommandName = lsCMD
            b1.Text = lsTXT
            b1.ToolTip = lsTlt
            b1.CommandArgument = jobinfo.ID.ToString

            Dim btnAdd As HyperLink = CType(r.Cells(JobCells.AddButton).Controls(1), HyperLink)
            btnAdd.Visible = False

            If jobinfo.Job_Class = "Arco.Doma.ImportExport.StartDrum" Then

                Dim isjobs As InputStreamBatchJobList = InputStreamBatchJobList.GetISBatchJobsByBatchJob(jobinfo.ID)
                If Not chkExpandDrumJobs.Checked AndAlso isjobs.Count = 1 Then
                    If isjobs(0).Attempts > 0 Then
                        r.Cells(JobCells.Name).Text &= " (Retry " & isjobs(0).Attempts & ")"
                    End If
                    btnAdd.Visible = True
                    btnAdd.Text = GetLabel("edit")
                    btnAdd.NavigateUrl = "javascript:EditDrumJob(" & isjobs(0).ID & ");"

                Else
                    r.Cells(JobCells.Logviewer).Text = ""

                    btnAdd.Visible = True
                    btnAdd.Text = GetLabel("add")
                    btnAdd.NavigateUrl = "javascript:NewDrumJob(" & jobinfo.ID & ");"
                End If

                If jobinfo.ExecutionStatus <> Job.JobExecutionStatus.Error AndAlso
                    isjobs.Cast(Of InputStreamBatchJobList.ISBatchJobInfo).Any(Function(x) x.ExecutionStatus = Job.JobExecutionStatus.Error) Then
                    jobinfo.ExecutionStatus = Job.JobExecutionStatus.SubInError
                End If


                If chkExpandDrumJobs.Checked Then

                    Dim subRow As New GridViewRow(e.Row.RowIndex + 1, e.Row.RowIndex + 1, DataControlRowType.DataRow, e.Row.RowState)
                    DirectCast(e.Row.Parent, System.Web.UI.WebControls.Table).Rows.Add(subRow)


                    Dim subTable As New System.Web.UI.WebControls.Table With {.CssClass = "SubList"}

                    subRow.Cells.Add(New TableCell)
                    Dim subCell As New TableCell With {.ColumnSpan = e.Row.Cells.Count - 1}
                    subCell.Controls.Add(subTable)

                    subRow.Cells.Add(subCell)


                    For Each isjob As InputStreamBatchJobList.ISBatchJobInfo In isjobs
                        Dim jobRow As New TableRow With {.CssClass = "SubListRow"}
                        subTable.Rows.Add(jobRow)

                        jobRow.Cells.Add(New TableCell With {.Text = GetStatusCellContent(isjob), .Width = Unit.Pixel(50)})

                        Dim nameCellContent As String = isjob.Name

                        If isjob.Attempts > 0 Then
                            nameCellContent &= " (Retry " & isjob.Attempts & ")"
                        End If
                        jobRow.Cells.Add(New TableCell With {.Text = nameCellContent})

                        If Settings.GetValue("General", "MultiTenant", False) Then
                            jobRow.Cells.Add(New TableCell With {.Text = GetTenantName(isjob.TenantId)})
                        End If
                        jobRow.Cells.Add(New TableCell With {.Text = isjob.LastError, .Width = Unit.Percentage(100)})

                        Dim execCell As New TableCell
                        jobRow.Cells.Add(execCell)

                        If jobinfo.ExecutionStatus <> Job.JobExecutionStatus.Pending Then
                            Dim subExecButton As New LinkButton
                            subExecButton.Text = GetLabel("execute")
                            subExecButton.ToolTip = String.Format(GetDecodedLabel("executejob"), isjob.Name)
                            subExecButton.CommandName = "cmdExecuteDrumJob"
                            subExecButton.CssClass = "ButtonLink"
                            subExecButton.CommandArgument = isjob.ID.ToString
                            FixDynamicLinkButton(subExecButton)

                            execCell.Controls.Add(subExecButton)



                        End If

                        Dim toggleCell As New TableCell
                        jobRow.Cells.Add(toggleCell)

                        Dim subToggleButton As New LinkButton

                        If Not isjob.Status = Job.JobStatus.Enabled Then
                            subToggleButton.CommandName = "cmdEnableDrumJob"
                            subToggleButton.Text = GetLabel("enable")
                            subToggleButton.ToolTip = String.Format(GetDecodedLabel("enablejob"), isjob.Name)
                        Else
                            subToggleButton.CommandName = "cmdDisableDrumJob"
                            subToggleButton.Text = GetLabel("disable")
                            subToggleButton.ToolTip = String.Format(GetDecodedLabel("disablejob"), isjob.Name)
                        End If

                        subToggleButton.CssClass = "ButtonLink"
                        subToggleButton.CommandArgument = isjob.ID.ToString
                        FixDynamicLinkButton(subToggleButton)

                        toggleCell.Controls.Add(subToggleButton)


                        Dim subEditButton As New HyperLink
                        subEditButton.Text = GetLabel("edit")
                        subEditButton.ToolTip = String.Format(GetDecodedLabel("edititem"), isjob.Name)
                        subEditButton.NavigateUrl = "javascript:EditDrumJob(" & isjob.ID & ");"

                        Dim editCell As New TableCell
                        editCell.Controls.Add(subEditButton)
                        jobRow.Cells.Add(editCell)

                        Dim logButton As New HyperLink
                        logButton.Text = "Log"
                        logButton.ToolTip = String.Format("{0} log", isjob.Name)
                        logButton.NavigateUrl = "javascript:OpenLog(" & EncodingUtils.EncodeJsString("drumloggingV2.aspx?drum_batch_id=" & isjob.ID) & ");"

                        Dim logCell As New TableCell
                        logCell.Controls.Add(logButton)
                        jobRow.Cells.Add(logCell)

                    Next
                End If
            Else
                'not  drum
                If jobinfo.Attempts > 0 Then
                    r.Cells(JobCells.Name).Text &= " (Retry " & jobinfo.Attempts & ")"
                End If
            End If

            r.Cells(JobCells.ExecutionStatus).Text = GetStatusCellContent(jobinfo)

        ElseIf r.RowType = DataControlRowType.Header Then
            SetOrderBy(r, JobCells.Name, "JOB_NAME ASC")
            SetOrderBy(r, JobCells.LastDate, "LAST_DATE DESC")
            SetOrderBy(r, JobCells.NextDate, "NEXT_DATE DESC")
            SetOrderBy(r, JobCells.ExecutionStatus, "JOB_STATUS,JOB_EXECUTION_STATUS ASC")
        End If
    End Sub

    Private Sub FixDynamicLinkButton(ByVal button As LinkButton)

        'hack to call rowcommand, otherwise the args are not rendered
        button.OnClientClick = ClientScript.GetPostBackClientHyperlink(GridView1, button.CommandName & "$" & button.CommandArgument) & ";return false;"
    End Sub

    Private Function GetStatusCellContent(ByVal isjob As InputStreamBatchJobList.ISBatchJobInfo) As String
        Return GetStatusCellContent(isjob.Status, isjob.ExecutionStatus, isjob.Attempts, JobRunTimeStatus.Running) 'runtimestatus todo
    End Function

    Private Function GetStatusCellContent(ByVal jobinfo As JobList.JobInfo) As String
        Return GetStatusCellContent(jobinfo.Status, jobinfo.ExecutionStatus, jobinfo.Attempts, jobinfo.RunTimeStatus)
    End Function

    Private Function GetStatusCellContent(ByVal status As Job.JobStatus, ByVal executionstatus As Job.JobExecutionStatus, ByVal attemtps As Int32, ByVal runtimestatus As JobRunTimeStatus) As String
        Dim tooltip As String = ""
        Dim extra As String = ""
        Dim iconClass As String = ""
        Select Case executionstatus
            Case Job.JobExecutionStatus.Idle
                tooltip = GetDecodedLabel("idle")
                If attemtps = 0 Then
                    iconClass = "icon icon-status-ok"
                Else
                    iconClass = "icon icon-status-ok"
                End If
            Case Job.JobExecutionStatus.Pending
                tooltip = GetDecodedLabel("pending") & " - " & runtimestatus.ToString
                iconClass = "icon icon-status-pending"
            Case Job.JobExecutionStatus.Error
                tooltip = GetDecodedLabel("inError")
                iconClass = "icon icon-status-error"
            Case Job.JobExecutionStatus.SubInError
                tooltip = GetDecodedLabel("inError") + " - Sub"
                iconClass = "icon icon-status-warning"
        End Select
        Select Case status
            Case Job.JobStatus.Disabled
                If executionstatus = Job.JobExecutionStatus.Idle Then
                    iconClass = "icon icon-status-disabled"
                    tooltip = GetDecodedLabel("disabled")
                Else
                    tooltip &= " - " & GetDecodedLabel("disabled")
                    extra = " <small>(" & GetLabel("disabled") & ")</small>"
                End If

            Case Job.JobStatus.Blocked
                If executionstatus = Job.JobExecutionStatus.Idle Then
                    iconClass = "icon icon-status-blocked"
                    tooltip = GetDecodedLabel("blocked")
                Else
                    tooltip &= " - " & GetDecodedLabel("blocked")
                    extra = " <small>(" & GetLabel("blocked") & ")</small>"
                End If
        End Select
        Return "<span class='" & iconClass & "' title='" & tooltip & "'/>" & extra
    End Function
    Private Function GetActualOrderBy() As String
        Select Case hdnOrderBy.Value
            Case "JOB_NAME ASC", "JOB_NAME DESC", "LAST_DATE DESC", "LAST_DATE ASC", "NEXT_DATE DESC", "NEXT_DATE ASC", "JOB_STATUS,JOB_EXECUTION_STATUS ASC", "JOB_STATUS,JOB_EXECUTION_STATUS DESC"
                Return hdnOrderBy.Value
        End Select

        Return "JOB_NAME ASC"
    End Function
    Private Sub SetOrderBy(ByVal r As GridViewRow, ByVal cell As JobCells, ByVal orderby As String)
        If hdnOrderBy.Value = orderby Then
            If orderby.Contains(" ASC") Then
                orderby = orderby.Replace(" ASC", " DESC")
            Else
                orderby = orderby.Replace(" DESC", " ASC")
            End If
        End If
        r.Cells(cell).Attributes.Add("onclick", String.Format("javascript:OrderBy('{0}');", orderby))
        r.Cells(cell).Attributes.Add("style", "cursor:pointer")
    End Sub

    Protected Sub lnkResumeJP_Click(sender As Object, e As EventArgs)
        RunningJobs.ResumeJobProcessor()
        SetPauzeResumeLinks()


    End Sub

    Private Sub SetPauzeResumeLinks()
        If RunningJobs.Paused Then
            pnlPaused.Visible = True
            lnkPauzeJP.Visible = False
        Else
            pnlPaused.Visible = False
            lnkPauzeJP.Visible = True
        End If
    End Sub

    Protected Sub lnkPauzeJP_Click(sender As Object, e As EventArgs)
        RunningJobs.PauseJobProcessor()
        SetPauzeResumeLinks()

    End Sub
End Class
