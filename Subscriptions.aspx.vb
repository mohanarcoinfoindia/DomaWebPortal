Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects

Partial Class Subscriptions
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

    Public Property NumberOfResults() As Int32

    Private Sub LoadJobsCombo()
        drpJobs.Items.Clear()
        drpJobs.Items.Add(New System.Web.UI.WebControls.ListItem("", "0"))
        For Each loJob As Jobs.JobList.JobInfo In _Jobs
            Dim lsText As String = loJob.Schedule.GetIntervalLabel
            Select Case loJob.Status
                Case Jobs.Job.JobStatus.Blocked
                    lsText &= " (" & GetDecodedLabel("blocked") & ")"
                Case Jobs.Job.JobStatus.Disabled
                    lsText &= " (" & GetDecodedLabel("disabled") & ")"
            End Select
            drpJobs.Items.Add(New System.Web.UI.WebControls.ListItem(lsText, loJob.ID.ToString))
        Next
    End Sub
    Public Function SubScriptionDetails(ByVal subscription As SubscriptionList.SubscriptionInfo) As String

        Dim icon As String = Icons.GetObjectIconClassByType(subscription.Object_Type)
        Dim text As String = subscription.Object_Label

        If String.IsNullOrEmpty(icon) Then
            'get svg icon
            icon = baseObjects.Subscription.GetSubscriptionIcon(subscription.Object_Type)
            If Not String.IsNullOrEmpty(icon) Then
                Return ThemedImage.GetImageTag(icon, Page) & Server.HtmlEncode(text)
            Else
                Return Server.HtmlEncode(text)
            End If
        Else
            Return ThemedImage.GetSpanIconTag("icon " + icon, text) & "<span style='margin-left:5px'>" & Server.HtmlEncode(text) & "</span>"
        End If

    End Function
    Public Function SubscriptionSubject(ByVal voSub As SubscriptionList.SubscriptionInfo) As String
        Return ArcoFormatting.FormatUserName(New Assignee(voSub.Subject_ID, voSub.Subject_Type).ToString)
    End Function
    Public Function SubScriptionSchedule(ByVal voSub As SubscriptionList.SubscriptionInfo) As String
        If voSub.Status = baseObjects.Subscription.SubscriptionStatus.Enabled Then
            Dim loJob As Jobs.JobList.JobInfo = _Jobs.Cast(Of Jobs.JobList.JobInfo).FirstOrDefault(Function(x) x.ID = voSub.JobID)
            If voSub.JobID <> 0 Then
                If loJob.ID = voSub.JobID Then
                    If loJob.Status = Jobs.Job.JobStatus.Enabled Then
                        Return loJob.Schedule.GetIntervalLabel
                    Else
                        Return loJob.Schedule.GetIntervalLabel & " (" & GetLabel("disabled") & ")"
                    End If
                Else
                    Return "Job not found"
                End If
            Else
                Return "Never"
            End If
        Else
            Return GetLabel("disabled")
        End If
    End Function
    Private _Jobs As Jobs.JobList
    
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
     
        If Not String.IsNullOrEmpty(delsubs.Value) Then
            baseObjects.Subscription.DeleteSubscription(Convert.ToInt32(delsubs.Value))
            delsubs.Value = ""
        End If

        _Jobs = Jobs.JobList.GetJobsWithClass("Arco.Doma.Library.BatchJobs.SendSubscriptions")
        If Not Page.IsPostBack Then
            LoadJobsCombo()
            lblFor.Text = GetLabel("subs_subject")
            lblWhen.Text = GetLabel("subs_schedule")
            lnkSearch.Text = GetLabel("search")
            lnkSearch.CssClass += " icon icon-search"
            lblOn.Text = GetLabel("subs_object")

        End If

        RecordsPerPage = ArcoInfoSettings.DefaultRecordsPerPage
        mDefaultMaxResults = Settings.GetValue("Interface", "Maxresults", 0)

        Dim lsPage As String = Request.Form("Page")
       
        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If

        Dim llFirstRec As Int32 = ((CInt(lsPage) - 1) * RecordsPerPage) + 1
        Dim llLastRec As Int32 = llFirstRec + RecordsPerPage - 1


        Dim loCrit As New SubscriptionList.Criteria With {
            .ObjectId = QueryStringParser.GetInt("OBJ_ID", 0),
            .ObjectType = QueryStringParser.GetString("OBJ_TYPE"),
            .NameFilter = txtOn.Text,
            .JobId = Convert.ToInt32(drpJobs.SelectedValue),
            .Range = ListRangeRequest.Range(llFirstRec, llLastRec, mDefaultMaxResults),
            .OrderBy = GetActualOrderBy()
        }


        If txtSubject.SubjectType = "User" Then
            loCrit.User = txtSubject.SubjectID
        Else
            loCrit.Subject = txtSubject.SubjectID
            loCrit.SubjectType = txtSubject.SubjectType
        End If

        Dim lcolSubs As SubscriptionList = SubscriptionList.GetSubscriptionList(loCrit)

        Dim loPagedDataSource As New PagedDataSource With {
            .AllowPaging = True,
            .PageSize = RecordsPerPage,
            .DataSource = lcolSubs,
            .CurrentPageIndex = CInt(lsPage) - 1
        }

        NumberOfResults = loPagedDataSource.DataSourceCount
        CurrentPage = CInt(lsPage)
        LastPage = loPagedDataSource.PageCount

        lstQueries.DataSource = loPagedDataSource
        lstQueries.DataBind()

        If lcolSubs.Any Then

            lnkPrev.Text = GetLabel("previous")
            lnkPrev.NavigateUrl = "javascript:Goto(" & CurrentPage - 1 & ");"
            lnkPrev.Enabled = Not loPagedDataSource.IsFirstPage

            lnkNext.Text = GetLabel("next")
            lnkNext.NavigateUrl = "javascript:Goto(" & CurrentPage + 1 & ");"
            lnkNext.Enabled = Not loPagedDataSource.IsLastPage

            litScroller.Text = PageScroller.GetPageScroller(CurrentPage, LastPage).Render("Goto") ' GetLabel("page") & " " & CurrentPage & " / " & LastPage            
        End If

        If loCrit.ObjectType = "Query" AndAlso loCrit.ObjectId > 0 Then
            lnkBack.Visible = True
            lnkBack.NavigateUrl = "javascript:PC().GotoLink('Queries.aspx');"
            lnkBack.Text = GetLabel("ub_back")
        Else
            lnkBack.Visible = False
        End If
    End Sub

    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "SUBJECT_ID", "CREATED_BY"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function

End Class
