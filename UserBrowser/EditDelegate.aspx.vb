Imports Telerik.Web.UI

Partial Class EditDelegate
    Inherits UserBrowserBasePage

    Private msFrom As String
    Private msFromType As String
    Private mbCanEdit As Boolean
    Public Function CheckCanEdit() As Boolean
        If Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.CanManageDelegations Then
            GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)            
            Return False
        Else
            Return True
        End If
    End Function
    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        dateBegin.DateInput.DateFormat = ArcoFormatting.GetDateFormat()
        dateEnd.DateInput.DateFormat = ArcoFormatting.GetDateFormat()

        If Not IsPostBack Then
            fillTypes(cmbDelegate)
            fillTypes(cmbDelegateFrom)

            fillRadiobuttons()

        End If

        fillLabels()
        If IsNumeric(Request.QueryString("id")) Then
            Dim del As Arco.Doma.Library.Routing.WorkException = Arco.Doma.Library.Routing.WorkException.GetWorkException(Request.QueryString("id"))
            If Not Page.IsPostBack Then
                FillProcedures(cmbProcID, del.Object_ID)
                Try
                    cmbDelegate.FindItemByValue(del.Object_Type).Selected = True
                Catch ex As Exception

                End Try
                Me.Delegate.Text = ArcoFormatting.FormatUserName(del.Subject.Value)
                Me.DelegateSUBJECT_ID.Value = del.Subject.Value
                Me.cmbDelegate.SelectedValue = del.Subject.TypeString

                Me.radioMode.SelectedValue = del.EXCEPTION_MODE
                If del.EXCEPTION_MODE <> Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Timed Then
                    trBegin.Style.Add("display", "none")
                    trEnd.Style.Add("display", "none")
                End If
                If Not String.IsNullOrEmpty(del.START_DATE) Then
                    dateBegin.SelectedDate = del.START_DATE
                End If
                If Not String.IsNullOrEmpty(del.END_DATE) Then
                    dateEnd.SelectedDate = del.END_DATE
                End If
            End If

            msFrom = del.DelegateFrom.Value
            msFromType = del.DelegateFrom.TypeString
            sSetFromLabel()
        Else
            'new
            If Not Page.IsPostBack Then
                FillProcedures(cmbProcID, 0)
            End If
            msFrom = Request.QueryString("subject_id")
            If Not String.IsNullOrEmpty(msFrom) Then
                msFromType = Request.QueryString("subject_type")

                sSetFromLabel()
            Else
                'new from admin
                lblUserName.Visible = False
                plhDelegateFrom.Visible = True
            End If
        End If

        If msFromType = "User" AndAlso msFrom = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
            mbCanEdit = Settings.GetValue("Interface", "UserCanSetDelegates", False)
            If Not mbCanEdit Then mbCanEdit = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.CanManageDelegations
        Else
            If Not CheckCanEdit() Then Exit Sub
            mbCanEdit = True
        End If

        If msFromType = "User" Then msFrom = Arco.Doma.Library.ACL.User.AddDummyDomain(msFrom)

        checkSecurity()

        If Not IsPostBack AndAlso Not IsNumeric(Request.QueryString("id")) Then
            trBegin.Style.Add("display", "none")
            trEnd.Style.Add("display", "none")
        End If
    End Sub

    Private Sub checkSecurity()
        If mbCanEdit Then
            Save.Visible = True
            trCommands.Visible = True
        Else
            Save.Visible = False
            trCommands.Visible = False
        End If        
    End Sub

    Private Sub fillRadiobuttons()
        radioMode.Items.Clear()
        radioMode.Items.Add(New ListItem(GetLabel("del_manual"), Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Manual))
        radioMode.Items.Add(New ListItem(GetLabel("del_timed"), Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Timed))
        radioMode.SelectedIndex = 0
    End Sub

    Private Sub sSetFromLabel()
        Select Case msFromType
            Case "User"              
                lblUserName.Text = ArcoFormatting.FormatUserName(msFrom)
            Case Else

                lblUserName.Text = msFrom
        End Select
    End Sub
    Private Sub fillLabels()

        
        Me.Title = GetLabel("delegates")
        lblFrom.Text = GetLabel("del_from") & ": "
        lblTo.Text = GetLabel("del_to") & ": "
        lblProcedure.Text = GetLabel("procedure") & ":"
        lblMode.Text = GetLabel("del_mode") & ":"
        lblEnd.Text = GetLabel("del_end") & ":"
        lblBegin.Text = GetLabel("del_begin") & ":"
        Save.Text = GetLabel("save")
        dateBegin.DatePopupButton.ToolTip = GetDecodedLabel("selectdate")
        dateEnd.DatePopupButton.ToolTip = GetDecodedLabel("selectdate")
    End Sub

    Private Sub fillTypes(ByRef dropdown As RadComboBox)
        If dropdown.Items.Count = 0 Then

            dropdown.Items.Add(New RadComboBoxItem(GetDecodedLabel("user"), "User"))
            dropdown.Items.Add(New RadComboBoxItem(GetDecodedLabel("role"), "Role"))
          
        End If
    End Sub

    Private Sub FillProcedures(ByRef dropdown As RadComboBox, ByVal selectedId As Int32)

        Dim procList As Arco.Doma.Library.Routing.PROCEDUREList = Arco.Doma.Library.Routing.PROCEDUREList.GetPROCEDUREList()
        dropdown.Items.Add(New RadComboBoxItem("", "0"))
        Dim lcolLabels As Arco.Doma.Library.Globalisation.LABELList = Arco.Doma.Library.Globalisation.LABELList.GetProceduresLabelList(Me.EnableIISCaching)

        For Each proc As Arco.Doma.Library.Routing.PROCEDUREList.PROCEDUREInfo In procList
            If proc.PROC_ACTIVE OrElse proc.ID = selectedId Then
                dropdown.Items.Add(New RadComboBoxItem(lcolLabels.GetObjectLabel(proc.ID, "Procedure", proc.Name) & " (" & proc.PROC_MAJOR_VERSION & "." & proc.PROC_MINOR_VERSION & ")", proc.ID) With {.Selected = (proc.ID = selectedId)})
            End If

        Next

    End Sub

    Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Save.Click
        If Not Page.IsValid Then
            Exit Sub
        End If
        If radioMode.SelectedValue = Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Timed Then
            trBegin.Style.Remove("display")
            trEnd.Style.Remove("display")
        End If

       
        Dim lsFromid As String
        Dim lsFromType As String
        If String.IsNullOrEmpty(msFrom) Then
            lsFromid = checkSubjectIDExists([DelegateFrom].Text, DelegateFromSUBJECT_ID.Value, cmbDelegateFrom.SelectedValue)
            If String.IsNullOrEmpty(lsFromid) Then
                lblError.Visible = True
                lblError.Text = GetLabel("del_from") & " " & GetLabel("req")
                Exit Sub
            End If
            lsFromType = cmbDelegateFrom.SelectedValue
        Else
            lsFromid = msFrom
            lsFromType = msFromType
        End If

        Dim lsSubjectid As String = checkSubjectIDExists([Delegate].Text, DelegateSUBJECT_ID.Value, cmbDelegate.SelectedValue)
        If String.IsNullOrEmpty(lsSubjectid) Then

            lblError.Visible = True
            lblError.Text = GetLabel("del_to") & " " & GetLabel("req")
            Exit Sub
        End If

        If lsSubjectid.Equals(lsFromid) AndAlso cmbDelegate.SelectedValue.Equals(lsFromType) Then
            lblError.Visible = True
            lblError.Text = GetLabel("del_to") & " " & GetLabel("mustbedifferentfrom") & " " & GetLabel("del_from")
            Exit Sub
        End If

        If radioMode.SelectedValue = Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Timed Then
            If dateBegin.IsEmpty Then
                lblError.Visible = True
                lblError.Text = GetLabel("del_begin") & " " & GetLabel("req")
                Exit Sub
            End If
            If dateEnd.IsEmpty Then
                lblError.Visible = True
                lblError.Text = GetLabel("del_end") & " " & GetLabel("req")
                Exit Sub
            End If
            If dateBegin.SelectedDate.Value.ToString(ArcoFormatting.GetDateFormatForSave()) > dateEnd.SelectedDate.Value.ToString(ArcoFormatting.GetDateFormatForSave()) Then
                lblError.Visible = True
                lblError.Text = GetLabel("del_begin") & " " & GetLabel("mustbesmaller") & " " & GetLabel("del_end")
                Exit Sub
            End If
        End If

        Dim wlitem As Arco.Doma.Library.Routing.WorkException
        If IsNumeric(Request.QueryString("id")) Then
            wlitem = Arco.Doma.Library.Routing.WorkException.GetWorkException(Request.QueryString("id"))
        Else
            wlitem = Arco.Doma.Library.Routing.WorkException.NewWorkException()
            wlitem.DelegateFrom = New Arco.Doma.Library.Assignee(lsFromid, lsFromType)
        End If
        wlitem.Subject = New Arco.Doma.Library.Assignee(lsSubjectid, cmbDelegate.SelectedValue)


        Dim lsMode As String = ""
        Select Case radioMode.SelectedValue
            Case Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Manual
                wlitem.EXCEPTION_MODE = Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Manual
                wlitem.START_DATE = ""
                wlitem.END_DATE = ""
            Case Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Timed
                wlitem.EXCEPTION_MODE = Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Timed
                If Not dateBegin.IsEmpty Then wlitem.START_DATE = dateBegin.SelectedDate.Value.ToString(ArcoFormatting.GetDateFormatForSave())
                If Not dateEnd.IsEmpty Then wlitem.END_DATE = dateEnd.SelectedDate.Value.ToString(ArcoFormatting.GetDateFormatForSave()) & " 23:59:00"
            Case Else
                wlitem.EXCEPTION_MODE = Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_NoMode
                wlitem.START_DATE = ""
                wlitem.END_DATE = ""
        End Select

        wlitem.Type = Arco.Doma.Library.Routing.WF_WorkExceptionType.WF_Delegate
        If IsNumeric(cmbProcID.SelectedValue) Then wlitem.OBJECT_ID = cmbProcID.SelectedValue
        wlitem.OBJECT_TYPE = "Procedure"
        wlitem.SCOPE = Arco.Doma.Library.Routing.WF_WorkExceptionScope.WF_UserScope
        wlitem.Save()
        Page.ClientScript.RegisterStartupScript(Page.ClientScript.[GetType](), "onLoad", "GetRadWindow().close();", True)

    End Sub

    Private Function checkSubjectIDExists(ByVal subjectDisplay As String, ByVal subjectIDFound As String, ByVal subjectType As String) As String
        If Not String.IsNullOrEmpty(subjectIDFound) Then
            If subjectType = "User" Then
                'hack fix : better to change the control               
                Return Arco.Doma.Library.ACL.User.AddDummyDomain(subjectIDFound)
            Else
                Return subjectDisplay
            End If
        End If
        Dim subjectID As String = ""
        If subjectDisplay.IndexOf("#") <> -1 Then
            subjectID = subjectDisplay
        Else
            If Not String.IsNullOrEmpty(subjectDisplay) Then
                If subjectType = "User" Then
                    Dim lUserCrit As New Arco.Doma.Library.ACL.USERList.Criteria()
                    lUserCrit.FILTER = subjectDisplay
                    Dim lUserToList As Arco.Doma.Library.ACL.USERList = Arco.Doma.Library.ACL.USERList.GetUSERSList(lUserCrit)
                    If lUserToList.Count = 1 Then
                        subjectID = lUserToList.Item(0).USER_ACCOUNT
                    End If
                ElseIf subjectType = "Role" Then
                    Dim lRole As Arco.Doma.Library.ACL.Role = Arco.Doma.Library.ACL.Role.GetRole(subjectDisplay)
                    If lRole.ID <> 0 Then
                        subjectID = lRole.Name
                    End If
                End If
            End If
        End If

        Return subjectID

    End Function

End Class
