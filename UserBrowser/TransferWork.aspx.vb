Imports Telerik.Web.UI
Imports Arco.Doma.Library
Imports Arco.Doma.Library.Routing
Imports Arco.Doma.Library.Globalisation
Imports Arco.Doma.Library.ACL

Partial Class UserBrowser_TransferWork
    Inherits UserBrowserBasePage

    Private sourceAssignee As Assignee

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        'todo : check candeditrole on source role

        CheckIsAdmin()

        Dim sourceType As String = Request.QueryString("subject_type")

        sourceAssignee = New Assignee(Request.QueryString("subject_id"), sourceType)

        If Not CanTransferWork(sourceAssignee) Then
            GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
            Return
        End If

        lblUserName.Text = Server.HtmlEncode(sourceAssignee.GetDisplayName(Settings))
        cmdSave.OnClientClick = "return confirm(" & EncodingUtils.EncodeJsString(GetDecodedLabel("confirmtransferwork").Replace("#type#", GetDecodedLabel(sourceType).ToLower)) & ");"
        FillProcedures(cmbProcID)

        Translate()

    End Sub

    Private Function CanTransferWork(ByVal assignee As Assignee) As Boolean
        Select Case assignee.Type
            Case Assignee.AssigneeType.Role
                Return Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal OrElse (Role.GetRole(assignee.Value).TenantId = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.Id)
            Case Assignee.AssigneeType.Group
                Return Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal
            Case Assignee.AssigneeType.User
                Return Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal OrElse ACL.User.GetUser(assignee.Value).MatchesTenant()

        End Select

        Return False
    End Function

    Private Sub FillProcedures(ByRef dropdown As RadComboBox)
        If dropdown.Items.Count <> 0 Then
            Return
        End If

        Dim labels As LABELList = LABELList.GetProceduresLabelList(EnableIISCaching)
        dropdown.Items.Add(New RadComboBoxItem(GetLabel("all"), "0"))
        For Each proc As PROCEDUREList.PROCEDUREInfo In PROCEDUREList.GetPROCEDUREList()
            dropdown.Items.Add(New RadComboBoxItem(labels.GetObjectLabel(proc.ID, "Procedure", Language, proc.Name), proc.ID.ToString))
        Next


    End Sub
    Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click
        If Not String.IsNullOrEmpty(newUser.SubjectID) Then
            WorkTransfer.Transfer(sourceAssignee, New Assignee(newUser.SubjectID, newUser.SubjectType), Convert.ToInt32(cmbProcID.SelectedValue))
            Dim sb As New StringBuilder
            sb.AppendLine("Close();")

            Page.ClientScript.RegisterStartupScript(GetType(String), "closescript", sb.ToString, True)
        End If

    End Sub

    Private Sub Translate()
        Title = GetLabel("Transfer Work")
        lblTo.Text = GetLabel("mail_to")
        lblFrom.Text = GetLabel("mail_from")
        lblProcedure.Text = GetLabel("Procedure")
        cmdCancel.Text = GetLabel("cancel")
        cmdSave.Text = GetLabel("Transfer Work")
    End Sub

End Class
