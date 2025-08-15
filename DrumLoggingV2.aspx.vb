Partial Class DrumLoggingV2
    Inherits BaseOperatorPage

    Private _batchID As Int32
    Private _ISID As Int32
    Private meFilterStatus As Arco.Doma.ImportExport.Drum.V2.DrumLogging.eValidationStatus = Arco.Doma.ImportExport.Drum.V2.DrumLogging.eValidationStatus.NA

    Public Const MaxResults = 1000
    Protected ReadOnly Property HasMoreResults As Boolean
        Get
            Return NumberOfResults >= MaxResults AndAlso MaxResults > 0
        End Get
    End Property
    Protected ReadOnly Property NumberOfResultsLabel() As String
        Get

            Dim l As Integer = NumberOfResults
            If HasMoreResults Then
                Return " > " & MaxResults.ToString
            Else
                Return NumberOfResults.ToString
            End If

        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim batches As New List(Of Tuple(Of Int32, String))

            Translate()
            _batchID = QueryStringParser.GetInt("DRUM_BATCH_ID")
            _ISID = QueryStringParser.GetInt("IS_ID")

            Dim filterBatchID As Integer = QueryStringParser.GetInt("BATCH_ID")


            cmbBatch.Items.Add(New ListItem("", "0"))

            If _ISID = 0 Then
                'get from the batch
                If _batchID <> 0 Then
                    _ISID = Arco.Doma.ImportExport.Drum.InputStreamBatchJob.GetISBatchJob(_batchID).IS_ID
                ElseIf filterBatchID <> 0 Then
                    Try
                        _ISID = Arco.Doma.ImportExport.Drum.InputStreamBatchJobList.GetISBatchJobsByBatchJob(filterBatchID)(0).IS_ID
                    Catch ex As NullReferenceException

                    End Try
                End If
            End If
            For Each loIS As Arco.Doma.ImportExport.Drum.InputStreamList.ISInfo In Arco.Doma.ImportExport.Drum.InputStreamList.GetV2ISs()
                If _ISID = 0 Then
                    _ISID = loIS.ID

                End If
                Dim item As New ListItem(loIS.Name, loIS.ID.ToString())
                If _ISID = loIS.ID Then
                    For Each loBatch As Arco.Doma.ImportExport.Drum.InputStreamBatchJobList.ISBatchJobInfo In DrumJobs
                        batches.Add(New Tuple(Of Int32, String)(loBatch.ID, loBatch.Name))
                    Next
                    item.Selected = True
                End If


                cmbIS.Items.Add(item)

            Next

            batches = batches.OrderBy(Function(x) x.Item2.ToUpper).ToList
            For Each batch As Tuple(Of Int32, String) In batches

                Dim item As New ListItem(batch.Item2, batch.Item1)

                If batch.Item1 = _batchID Then
                    item.Selected = True
                End If
                cmbBatch.Items.Add(item)
            Next

            If filterBatchID <> 0 OrElse _batchID <> 0 Then
                cmbStatus.SelectedIndex = 0 ' Default :All in case a specific job is chose
            Else
                cmbStatus.SelectedIndex = 2 ' Default : To be validated.
            End If

        End If

        If Not String.IsNullOrEmpty(cmbIS.SelectedValue) Then
            _ISID = Convert.ToInt32(cmbIS.SelectedValue)
        End If

        If Not String.IsNullOrEmpty(cmbBatch.SelectedValue) Then
            Int32.TryParse(cmbBatch.SelectedValue, _batchID)
        End If

        If cmbStatus.SelectedIndex > 0 Then
            meFilterStatus = Convert.ToInt32(cmbStatus.SelectedValue)
        End If

        ShowLogging()

    End Sub

    Private _jobs As Arco.Doma.ImportExport.Drum.InputStreamBatchJobList
    Private ReadOnly Property DrumJobs As Arco.Doma.ImportExport.Drum.InputStreamBatchJobList
        Get
            If _jobs Is Nothing Then
                _jobs = Arco.Doma.ImportExport.Drum.InputStreamBatchJobList.GetISBatchJobsByIS(_ISID)
            End If
            Return _jobs
        End Get
    End Property

    Private Sub Translate()

        ' filter panel ----------------------     
        lblBatch.Text = GetLabel("dl_batchjob")
        lblStatus.Text = GetLabel("dl_validationstatus")
        btnSearch.Text = GetLabel("search")

        ' detail ---------------------------
        lblID.Text = GetLabel("dl_id")

        lblTime.Text = GetLabel("dl_logtime")
        lblTimestamp.Text = GetLabel("dl_logtime")
        lblMachine.Text = GetLabel("dl_logmachine")

        lblUser.Text = GetLabel("dl_loguser")
        lblBatchID.Text = GetLabel("dl_batchid")

        lblIdentifier.Text = GetLabel("dl_identifier")

        lblStatus2.Text = GetLabel("dl_validationstatus")
        lblErrCode2.Text = GetLabel("dl_errcode")
        lblErrMessage.Text = GetLabel("dl_errmessage")
        lblValidatedOn.Text = GetLabel("dl_validatedon")
        lblValidatedBy.Text = GetLabel("dl_validatedby")
        lblComment.Text = GetLabel("dl_validationcomment")
        cmdList.Text = GetDecodedLabel("dl_list")
        cmdValidate.Text = GetLabel("dl_validate")

        ' Actions panel ----------------------
        cmdExport.Text = GetLabel("dl_archiveall")
        cmdValidateAll.Text = GetLabel("dl_validateall")

        grdResultList.Columns(1).HeaderText = GetLabel("dl_validationstatus")
        grdResultList.Columns(2).HeaderText = GetLabel("dl_logtime")
        grdResultList.Columns(3).HeaderText = GetDecodedLabel("dl_identifier")
        grdResultList.Columns(4).HeaderText = GetDecodedLabel("dl_batchjob")
        grdResultList.Columns(5).HeaderText = GetDecodedLabel("dl_errmessage")
        grdResultList.Columns(6).HeaderText = GetLabel("dl_validationstatus")

        cmbStatus.Items(0).Text = GetDecodedLabel("all")
        cmbStatus.Items(1).Text = GetDecodedLabel("novalidationneeded")
        cmbStatus.Items(2).Text = GetDecodedLabel("tobevalidated")
        cmbStatus.Items(3).Text = GetDecodedLabel("validated")

    End Sub


    Public Property RecordsPerPage() As Integer


    Public Property LastPage() As Integer
    Public Property NumberOfResults() As Integer

    Private Sub ShowLogging()

        Dim lsPage As String = CurrentPage.Value
        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If

        '  RecordsPerPage = Math.Max(ArcoInfoSettings.DefaultRecordsPerPage, 30)

        RecordsPerPage = 10


        Dim loCrit As New Arco.Doma.ImportExport.Drum.V2.DrumLoggingList.Criteria() With {
            .BatchID = _batchID,
            .ISID = _ISID,
            .ValidationStatus = meFilterStatus,
            .ItemIdentifier = txtSourceFilter.Text,
            .TimeStamp = txtTimeStampFilter.Text,
            .OrderBy = GetActualOrderBy()
            }
        loCrit.Range.MaxResults = MaxResults
        loCrit.Range.FirstItem = ((CInt(lsPage) - 1) * RecordsPerPage) + 1
        loCrit.Range.LastItem = loCrit.Range.FirstItem + RecordsPerPage - 1
        Dim loDL As Arco.Doma.ImportExport.Drum.V2.DrumLoggingList = Arco.Doma.ImportExport.Drum.V2.DrumLoggingList.GetDrumLoggings(loCrit)

        If loDL.Count < loCrit.Range.FirstItem AndAlso loCrit.Range.FirstItem > 1 Then
            lsPage = "1"
            CurrentPage.Value = lsPage
            loCrit.Range.FirstItem = 1
            loCrit.Range.LastItem = loCrit.Range.FirstItem + RecordsPerPage - 1
            loDL = Arco.Doma.ImportExport.Drum.V2.DrumLoggingList.GetDrumLoggings(loCrit)
        End If

        Dim loPagedDataSource As New PagedDataSource With {
            .AllowPaging = True,
            .PageSize = RecordsPerPage,
            .DataSource = loDL,
            .CurrentPageIndex = CInt(lsPage) - 1
        }


        Me.NumberOfResults = loPagedDataSource.DataSourceCount
        Me.CurrentPage.Value = lsPage
        Me.LastPage = loPagedDataSource.PageCount

        grdResultList.DataSource = loPagedDataSource
        grdResultList.DataBind()
        grdResultList.Visible = True
        pnlActions.Visible = (grdResultList.Rows.Count <> 0)
        cmdValidateAll.Visible = (meFilterStatus = Arco.Doma.ImportExport.Drum.DrumLogging.eValidationStatus.ToBeValidated)


        lnkPrev.Text = GetLabel("previous")
        lnkPrev.NavigateUrl = "javascript:Goto(" & CInt(lsPage) - 1 & ");"


        lnkNext.Text = GetLabel("next")
        lnkNext.NavigateUrl = "javascript:Goto(" & CInt(lsPage) + 1 & ");"

        lnkPrev.Enabled = Not loPagedDataSource.IsFirstPage
        lnkNext.Enabled = Not loPagedDataSource.IsLastPage

        litScroller.Text = PageScroller.GetPageScroller(CInt(lsPage), loPagedDataSource.PageCount).Render("Goto")

        pnlData.Visible = False
        pnlFilter.Visible = True
        grdResultList.Visible = (grdResultList.Rows.Count <> 0)
        pnlActions.Visible = (grdResultList.Rows.Count <> 0)
        pnlScroll.Visible = True
    End Sub

    Protected Sub grdResultList_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles grdResultList.RowCommand

        Dim lsGUID As String = e.CommandArgument.ToString
        Dim lbCommentEnable As Boolean = False
        Dim lbCommentVisible As Boolean = False

        If e.CommandName.ToLower = "validate" Then

            LogID.Value = lsGUID
            Dim loDL As Arco.Doma.ImportExport.Drum.V2.DrumLogging = Arco.Doma.ImportExport.Drum.V2.DrumLogging.GetDrumLogging(_ISID, _batchID, lsGUID)
            lblIDValue.Text = loDL.GUID
            lblMachineValue.Text = loDL.LogMachine

            lblUserValue.Text = ArcoFormatting.FormatUserName(loDL.LogUser)

            lblTimeValue.Text = ArcoFormatting.FormatDateLabel(loDL.LogTime, True, True, False)

            lblBatchIDValue.Text = loDL.BatchID.ToString

            lblIdentifierValue.Text = loDL.Item_Identifier

            lblErrCodeValue.Text = loDL.Status.ToString
            lblErrMessageValue.Text = loDL.Message
            If loDL.ValidationStatus = Arco.Doma.ImportExport.Drum.DrumLogging.eValidationStatus.NoValidationNeeded Then
                lblStatusValue.Text = "No validation needed"
            ElseIf loDL.ValidationStatus = Arco.Doma.ImportExport.Drum.DrumLogging.eValidationStatus.ToBeValidated Then
                lblStatusValue.Text = "To be validated"
                lbCommentEnable = True
                lbCommentVisible = True
            ElseIf loDL.ValidationStatus = Arco.Doma.ImportExport.Drum.DrumLogging.eValidationStatus.Validated Then
                lblStatusValue.Text = "Validated"
                lbCommentEnable = False
                lbCommentVisible = True
            End If
            lblValidatedOnValue.Text = ArcoFormatting.FormatDateLabel(loDL.ValidatedOn, True, True, False)
            lblValidatedByValue.Text = ArcoFormatting.FormatUserName(loDL.ValidatedBy)
            txtComment.Text = loDL.ValidationComment
            lblComment.Visible = lbCommentVisible
            txtComment.Visible = lbCommentVisible
            cmdValidate.Visible = lbCommentVisible
            lblComment.Enabled = lbCommentEnable
            txtComment.Enabled = lbCommentEnable
            cmdValidate.Enabled = lbCommentEnable
            pnlData.Visible = True
            pnlFilter.Visible = False
            grdResultList.Visible = False
            pnlActions.Visible = False
            pnlScroll.Visible = False
        End If

    End Sub

    Protected Sub cmdList_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdList.Click

        ShowLogging()

    End Sub

    Protected Sub cmdValidate_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdValidate.Click

        Dim loDL As Arco.Doma.ImportExport.Drum.V2.DrumLogging = Arco.Doma.ImportExport.Drum.V2.DrumLogging.GetDrumLogging(_ISID, _batchID, LogID.Value)
        If txtComment.Text.Length = 0 Then
            txtComment.Text = "No Validation comment added"
        End If

        loDL.ValidationStatus = Arco.Doma.ImportExport.Drum.V2.DrumLogging.eValidationStatus.Validated
        loDL.ValidatedOn = Now
        loDL.ValidatedBy = Arco.Security.BusinessIdentity.CurrentIdentity.Name
        loDL.ValidationComment = txtComment.Text
        loDL.Save()

        ShowLogging()

    End Sub

    Protected Sub cmdValidateAll_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdValidateAll.Click
        Dim loCrit As New Arco.Doma.ImportExport.Drum.V2.DrumLoggingList.Criteria With {
            .BatchID = _batchID,
            .ValidationStatus = Arco.Doma.ImportExport.Drum.V2.DrumLogging.eValidationStatus.ToBeValidated
        }

        Dim loDL As Arco.Doma.ImportExport.Drum.V2.DrumLoggingList = Arco.Doma.ImportExport.Drum.V2.DrumLoggingList.GetDrumLoggings(loCrit)

        For i = 0 To loDL.Count - 1
            Dim loDL_item As Arco.Doma.ImportExport.Drum.V2.DrumLogging = Arco.Doma.ImportExport.Drum.V2.DrumLogging.GetDrumLogging(_ISID, _batchID, loDL(i).GUID)
            loDL_item.ValidationStatus = Arco.Doma.ImportExport.Drum.V2.DrumLogging.eValidationStatus.Validated
            loDL_item.ValidatedOn = Now
            loDL_item.ValidatedBy = Arco.Security.BusinessIdentity.CurrentIdentity.Name
            loDL_item.ValidationComment = "Validate All"
            loDL_item.Save()
        Next

        ShowLogging()
    End Sub

    Protected Sub cmdExport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdExport.Click
        'todo
        'export all events in a xml file and save them to disk in the drum ArchiveLog folder. 
        'All archived events are deleted from the process log table. 
        'This is an event that should be sent to the doma service where it actually happens because otherwhise errors may occur because IIS stoppes long running sessions.
    End Sub
    Protected Sub gdDetails_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim loLine As Arco.Doma.ImportExport.Drum.V2.DrumLoggingDetail = CType(e.Row.DataItem, Arco.Doma.ImportExport.Drum.V2.DrumLoggingDetail)
            Dim leStatus As Arco.Doma.ImportExport.Drum.V2.DrumResult.eStatus
            [Enum].TryParse(e.Row.Cells(0).Text, leStatus)
            Select Case leStatus
                Case Arco.Doma.ImportExport.Drum.V2.DrumResult.eStatus.Success
                    Dim lsAction As String
                    If loLine.Code = "UPDATE" Then
                        lsAction = "Updated"
                    Else
                        lsAction = "Created"
                    End If
                    e.Row.Cells(1).Text = String.Format("{0} {1} {2}", lsAction, loLine.ItemType, loLine.ItemID)
                Case Else
                    e.Row.Cells(1).Text = loLine.Message
            End Select

        End If
    End Sub
    Protected Sub grdResultList_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles grdResultList.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then

            Dim item As Arco.Doma.ImportExport.Drum.V2.DrumLoggingList.DrumLoggingInfo = DirectCast(e.Row.DataItem, Arco.Doma.ImportExport.Drum.V2.DrumLoggingList.DrumLoggingInfo)

            If item.ValidationStatus = Arco.Doma.ImportExport.Drum.DrumLogging.eValidationStatus.ToBeValidated Then
                e.Row.Cells(7).FindControl("LinkButton2").Visible = True
            Else
                e.Row.Cells(7).FindControl("LinkButton2").Visible = False
            End If
            Select Case item.BatchID
                Case 0
                    e.Row.Cells(4).Text = ""
                Case Else
                    Try
                        e.Row.Cells(4).Text = DrumJobs.Cast(Of Arco.Doma.ImportExport.Drum.InputStreamBatchJobList.ISBatchJobInfo).FirstOrDefault(Function(x) x.ID = item.BatchID).Name
                    Catch ex As NullReferenceException
                        e.Row.Cells(4).Text = "Not found " & item.BatchID
                    End Try
            End Select

            e.Row.Cells(2).Text = ArcoFormatting.FormatDateLabel(item.LogTime, True, True, False)


            ' Dim lsGUID As String = grdResultList.DataKeys(e.Row.RowIndex).Value.ToString
            Dim ldDetails As GridView = TryCast(e.Row.FindControl("gdDetails"), GridView)
            AddHandler ldDetails.RowDataBound, AddressOf gdDetails_RowDataBound
            ldDetails.DataSource = item.Details 'Arco.Doma.ImportExport.Drum.V2.DrumLoggingDetailList.GetDrumLoggingDetails(lsGUID)
            ldDetails.DataBind()
        Else
            SetOrderBy(e.Row, 1, "STATUS")
            SetOrderBy(e.Row, 2, "LOG_TIME")
            SetOrderBy(e.Row, 3, "ITEM_IDENTIFIER")
            SetOrderBy(e.Row, 4, "BATCH_ID")
            SetOrderBy(e.Row, 5, "MESSAGE")
            SetOrderBy(e.Row, 6, "VALIDATION_STATUS")
        End If
    End Sub

    Private Sub SetOrderBy(ByVal r As GridViewRow, ByVal cell As Int32, ByVal value As String)

        r.Cells(cell).Attributes.Add("onclick", String.Format("javascript:OrderBy('{0}');", value))
        r.Cells(cell).Attributes.Add("style", "cursor:pointer")
    End Sub
    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "STATUS", "LOG_TIME", "ITEM_IDENTIFIER", "BATCH_ID", "MESSAGE", "VALIDATION_STATUS"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function
    Private Sub cmbIS_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbIS.SelectedIndexChanged
        CurrentPage.Value = "1"

        cmbBatch.Items.Clear()
        cmbBatch.Items.Add(New ListItem("", "0"))
        For Each loBatch As Arco.Doma.ImportExport.Drum.InputStreamBatchJobList.ISBatchJobInfo In Arco.Doma.ImportExport.Drum.InputStreamBatchJobList.GetISBatchJobsByIS(_ISID)
            If _ISID = loBatch.IS_ID Then
                cmbBatch.Items.Add(New ListItem(loBatch.Name, loBatch.ID.ToString()))
            End If
        Next
        Int32.TryParse(cmbBatch.SelectedValue, _batchID)


        ShowLogging()
    End Sub
End Class
