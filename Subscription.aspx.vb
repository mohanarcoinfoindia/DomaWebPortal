Imports Arco.Doma.Library


Partial Class Subscription
    Inherits BasePage

    Private _id As Int32
    Protected Sub Subscription_Load(sender As Object, e As System.EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then

            _id = QueryStringParser.GetInt("ID")
            hdnID.Value = _id.ToString
            If _id > 0 Then               
                Dim subs As Arco.Doma.Library.baseObjects.Subscription = Arco.Doma.Library.baseObjects.Subscription.GetSubscription(_id)
                If Not ((subs.Subject_ID = Security.BusinessIdentity.CurrentIdentity.Name AndAlso subs.Subject_Type = "User") OrElse isAdmin()) Then
                    GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                End If
                FillObjectInfo(subs.Object_ID, subs.Object_Type, subs.Query_Value)
                If subs.Subject_Type <> "User" Then
                    txtSubject.Subject = New Assignee(subs.Subject_ID, subs.Subject_Type).ToString
                Else
                    txtSubject.Subject = ACL.User.AddDummyDomain(subs.Subject_ID)
                    rowTo.Visible = False
                End If
                chkIncSubFolders.Checked = subs.IncludeSubFolders
                drpDate.SelectedValue = Convert.ToInt32(subs.Filter).ToString
                drpStatus.SelectedValue = Convert.ToInt32(subs.Status).ToString
                LoadJobs(subs.JobId)
                LoadTemplates(subs.Template)              
            Else
                Dim llObjID As Int32 = QueryStringParser.GetInt("OBJ_ID")
                Dim lsObjType As String = QueryStringParser.GetString("OBJ_TYPE")
                Dim lsQryValue As String = QueryStringParser.GetString("QRY_VALUE")
                Dim lbExists As Boolean = False
                Dim lsSubject As String = Security.BusinessIdentity.CurrentIdentity.Name
                Dim subs As baseObjects.Subscription = baseObjects.Subscription.GetSubscription(Security.BusinessIdentity.CurrentIdentity.Name, lsObjType, llObjID, lsQryValue)
                If subs IsNot Nothing Then
                    If Not CanCreateForOtherUsers() Then
                        lbExists = True
                    Else
                        lsSubject = ""
                    End If
                End If
                If Not lbExists Then
                    FillObjectInfo(llObjID, lsObjType, lsQryValue)
                    txtSubject.Subject = lsSubject
                    chkIncSubFolders.Checked = True
                    cmdSave.Text = GetLabel("subscribe")
                    LoadJobs(0)
                    LoadTemplates(0)                   
                Else
                    Response.Redirect("Subscription.aspx?ID=" & subs.ID & QueryStringParser.CreateQueryStringPart("modal", QueryStringParser.GetBoolean("Modal")))
                    Exit Sub
                End If
            End If
            rowOptions.Visible = chkIncSubFolders.Visible OrElse drpDate.Visible
        Else
            _id = Convert.ToInt32(hdnID.Value)
        End If
        ApplySecurity()
        SetLabels()
    End Sub

    Protected Sub ApplySecurity()
        rowTo.Visible = CanCreateForOtherUsers()
        rowTemplate.Visible = CanSelectTemplate()
        rowWhen.visible = Not QueryStringParser.GetBoolean("fromup") AndAlso CanSetSchedule()
        lnkDelete.Visible = _id <> 0 AndAlso Not QueryStringParser.GetBoolean("fromup") AndAlso CanCreateForOtherUsers()
    End Sub
    Protected Function CanSetSchedule() As Boolean
        Return Security.BusinessIdentity.CurrentIdentity.isAdmin
    End Function
    Protected Function CanCreateForOtherUsers() As Boolean
        Return Security.BusinessIdentity.CurrentIdentity.isAdmin
    End Function
    Protected Function CanSelectTemplate() As Boolean
        Return Security.BusinessIdentity.CurrentIdentity.isAdmin
    End Function
    Private Sub SetLabels()
        chkIncSubFolders.Text = GetLabel("includesubfolders")
        Title = GetLabel("subscribe")
        lblFor.Text = GetLabel("subs_subject")
        lblO.Text = GetLabel("subs_object")
        lblWhen.Text = GetLabel("subs_schedule")
        cmdSave.Text = GetLabel("save")
        cmdCancel.Text = GetLabel("cancel")
        lnkDelete.Text = GetLabel("delete")

        drpStatus.Items(0).Text = GetDecodedLabel("enabled")
        drpStatus.Items(1).Text = GetDecodedLabel("disabled")

        drpDate.Items(0).Text = GetDecodedLabel("subs_date_0")
        drpDate.Items(1).Text = GetDecodedLabel("subs_date_1")
        drpDate.Items(2).Text = GetLabel("all")
    End Sub

    Private Sub LoadJobs(ByVal selected As Int32)

        Dim lcolJobs As Jobs.JobList = Jobs.JobList.GetJobsWithClass("Arco.Doma.Library.BatchJobs.SendSubscriptions")
        Dim lbShowJobName As Boolean = (lcolJobs.Count > 1)
        drpJob.Items.Clear()
        For Each loJob As Jobs.JobList.JobInfo In lcolJobs
            Dim lsText As String = loJob.Schedule.GetIntervalLabel           
            If lbShowJobName Then
                lsText &= " (" & loJob.Name & ")"
            End If
            Select Case loJob.Status
                Case Jobs.Job.JobStatus.Blocked
                    lsText &= " (" & GetDecodedLabel("blocked") & ")"
                Case Jobs.Job.JobStatus.Disabled
                    lsText &= " (" & GetDecodedLabel("disabled") & ")"
            End Select
            drpJob.Items.Add(New System.Web.UI.WebControls.ListItem(lsText, loJob.ID.ToString) With {.Selected = (loJob.ID = selected)})
        Next
    End Sub
    Private Sub LoadTemplates(ByVal selected As Int32)
        drpTemplate.Items.Clear()
        drpTemplate.Items.Add(New System.Web.UI.WebControls.ListItem(GetLabel("default"), "0"))
        For Each t As Mail.NotificationTemplateList.NotificationTemplateInfo In Mail.NotificationTemplateList.GetNotificationTemplateList(Mail.NotificationUses.Subscription)
            drpTemplate.Items.Add(New System.Web.UI.WebControls.ListItem(t.Name, t.ID.ToString) With {.Selected = (t.ID = selected)})
        Next
    End Sub
    Private Sub FillObjectInfo(ByVal objid As Int32, ByVal objtype As String, ByVal queryvalue As String)
        If Not Arco.Doma.Library.baseObjects.Subscription.CanViewSubscriptionObject(objid, objtype) Then
            GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        End If
        hdnObjectId.Value = objid.ToString
        hdnObjectType.Value = objtype
        hdnQryValue.Value = queryvalue
        lblOn.Text = Server.HtmlEncode(Arco.Doma.Library.baseObjects.Subscription.GetSubscriptionObjectLabel("", objtype, objid, queryvalue))
        imgType.ImageUrl = ThemedImage.GetUrl(Arco.Doma.Library.baseObjects.Subscription.GetSubscriptionIcon(objtype), Me)
        Try
            imgType.ToolTip = GetDecodedLabel(objtype)
        Catch ex As Exception
            imgType.ToolTip = objtype
        End Try
        drpDate.Visible = (objtype = "Folder" OrElse objtype = "Dossier" OrElse objtype = "Query")
        chkIncSubFolders.Visible = (objtype = "Folder" OrElse objtype = "Dossier")
    End Sub

    Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSave.Click
        If String.IsNullOrEmpty(txtSubject.SubjectID) Then
            ShowErrorLabel(GetLabel("subs_subject") & " " & GetLabel("req"))
            Return
        End If
        Dim subs As Arco.Doma.Library.baseObjects.Subscription
        If _id > 0 Then
            subs = Arco.Doma.Library.baseObjects.Subscription.GetSubscription(_id)
        Else
            subs = Arco.Doma.Library.baseObjects.Subscription.NewSubscription
        End If
        subs.Object_ID = Convert.ToInt32(hdnObjectId.Value)
        subs.Object_Type = hdnObjectType.Value
        subs.Query_Value = hdnQryValue.Value
        subs.Subject_ID = txtSubject.SubjectID
        subs.Subject_Type = txtSubject.SubjectType
        If chkIncSubFolders.Visible Then subs.IncludeSubFolders = chkIncSubFolders.Checked
        If drpDate.Visible Then subs.Filter = CType(Convert.ToInt32(drpDate.SelectedValue), Arco.Doma.Library.baseObjects.Subscription.DateFilter)
        If drpStatus.Visible Then subs.Status = CType(Convert.ToInt32(drpStatus.SelectedValue), Arco.Doma.Library.baseObjects.Subscription.SubscriptionStatus)
        subs.JobId = Convert.ToInt32(drpJob.SelectedValue)    
        subs.Template = Convert.ToInt32(drpTemplate.SelectedValue)
        subs.Save()

        Close()
    End Sub

    Private Sub ShowErrorLabel(ByVal text As String)
        rowMessage.Visible = True
        errorLabel.Text = text
    End Sub
    Protected Sub lnkDelete_Click(sender As Object, e As System.EventArgs) Handles lnkDelete.Click
        If _id > 0 Then
            Arco.Doma.Library.baseObjects.Subscription.DeleteSubscription(_id)
            Close()
        End If
    End Sub

    Private Sub Close()
        Page.ClientScript.RegisterStartupScript(Me.GetType, "closescript", "if (typeof(MainPage().Refresh) == 'function') {MainPage().Refresh()};Close();", True)
    End Sub
End Class
