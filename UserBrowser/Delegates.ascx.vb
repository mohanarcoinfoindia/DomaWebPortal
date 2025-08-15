
Public Class DelegatesControl
    Inherits UserBrowserBaseUserControl

    Public Property SubjectID() As String
     
    Public Property SubjectType() As String

    Private _count As Integer = 0
    Public ReadOnly Property Count As Integer
        Get
            Return _count
        End Get
    End Property
    Private Sub checkDelegateTab()

        If IsNumeric(DelegateIDToDelete.Value) Then
            Arco.Doma.Library.Routing.WorkException.DeleteWorkException(Convert.ToInt32(DelegateIDToDelete.Value))
            DelegateIDToDelete.Value = ""
        End If

        lblTileDelFromMe.Text = GetLabel("del_from") & " " & ArcoFormatting.FormatUserName(SubjectID)
        lblTileDelToMe.Text = GetLabel("del_to") & " " & ArcoFormatting.FormatUserName(SubjectID)

        del_add.Attributes.Add("title", GetLabel("add"))
        'del_add.Attributes.Add("src", ThemedImage.GetClientUrl("new.svg", Page))


        del_add.Visible = True
        If Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.CanManageDelegations Then
            If SubjectType = "User" AndAlso SubjectID = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
                If Not Settings.GetValue("Interface", "UserCanSetDelegates", False) Then
                    del_add.Visible = False
                End If


                If Not del_add.Visible Then
                    div_del_actions.Visible = False

                    td_del_actions.Controls.Add(New Label With {
                        .Text = "<br />"
                    })
                End If
            Else
                del_add.Visible = False
            End If
        End If

        If SubjectType = "User" Then
            getDelegates(True)
        End If
        getDelegates(False)

        Dim sb As String = ""
        sb &= "function AddDelegate() {"
        sb &= "var oWnd2 = radopen(" & EncodingUtils.EncodeJsString("EditDelegate.aspx?subject_type=" & SubjectType & "&subject_id=" & SubjectID) & ", 'Popup', 600, 570);"
        sb &= "oWnd2.fullScreen = true;"
        sb &= "oWnd2.add_close(Refresh);"
        sb &= "}"

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "addelegate", sb, True)

    End Sub
    Private Sub getDelegates(ByVal ToMe As Boolean)


        Dim lcolDelegates As Arco.Doma.Library.Routing.WorkExceptionList

        If ToMe Then
            lcolDelegates = Arco.Doma.Library.Routing.WorkExceptionList.GetDelegatedToUser(SubjectID, False)
        Else
            lcolDelegates = Arco.Doma.Library.Routing.WorkExceptionList.GetDelegatedFrom(SubjectID, SubjectType, False)
        End If

        Dim dt As New DataTable()
        dt.Columns.Add("ID")
        dt.Columns.Add("FROM")
        dt.Columns.Add("TO")
        dt.Columns.Add("PROCEDURE")
        dt.Columns.Add("MODE")
        dt.Columns.Add("BEGIN")
        dt.Columns.Add("END")

        If lcolDelegates.Any Then
            Dim lcolProcLabels As Arco.Doma.Library.Globalisation.LABELList = Nothing
            For Each loDel As Arco.Doma.Library.Routing.WorkExceptionList.WorkExceptionInfo In lcolDelegates
                Dim lsMode As String = ""
                Select Case loDel.Mode
                    Case Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Timed
                        lsMode = GetLabel("del_timed")

                    Case Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_NoMode
                        lsMode = "NO MODE"

                    Case Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Manual
                        lsMode = GetLabel("del_manual")
                End Select
                Dim lsProc As String = ""
                If loDel.Object_ID <> 0 Then
                    Try
                        If lcolProcLabels Is Nothing Then lcolProcLabels = Arco.Doma.Library.Globalisation.LABELList.GetProceduresLabelList(Me.EnableIISCaching)
                        Dim loProc As Arco.Doma.Library.Routing.Procedure = Arco.Doma.Library.Routing.Procedure.GetProcedure(loDel.Object_ID)
                        lsProc = lcolProcLabels.GetObjectLabel(loProc.ID, "Procedure", loProc.Name) & " (" & loProc.PROC_MAJOR_VERSION & "." & loProc.PROC_MINOR_VERSION & ")"
                    Catch ex As Exception

                    End Try

                End If
                Dim lsFrom As String
                Select Case loDel.DelegateFrom_Type
                    Case "User"
                        lsFrom = ArcoFormatting.FormatUserName(loDel.DelegateFrom_ID)

                        If lsFrom <> loDel.DelegateFrom_ID Then
                            lsFrom &= "&nbsp;<small>(" & loDel.DelegateFrom_ID & ")</small>"
                        End If

                    Case Else
                        lsFrom = loDel.DelegateFrom_ID
                End Select
                Dim lsTo As String
                Select Case loDel.Subject_Type
                    Case "User"
                        lsTo = ArcoFormatting.FormatUserName(loDel.Subject_ID) & "&nbsp;<small>(" & loDel.Subject_ID & ")</small>"
                    Case Else
                        lsTo = loDel.Subject_ID
                End Select
                dt.Rows.Add(New String() {loDel.ID, lsFrom, lsTo, lsProc, lsMode, ArcoFormatting.FormatDateLabel(loDel.Start_Date, False, False, False), ArcoFormatting.FormatDateLabel(loDel.End_Date, False, False, False)})
            Next
            tr3.Visible = False
            tr2.Visible = False
        Else
            If ToMe Then
                tr3.Visible = True
                lblNoDelatesFound.Text = GetLabel("del_nodelatesfound")
            Else
                tr2.Visible = True
                lblNoDelatesFound2.Text = GetLabel("del_nodelatesfound")
            End If
        End If

        _count = dt.Rows.Count

        If ToMe Then
            repToMe.DataSource = dt
            repToMe.DataBind()

        Else
            repFromMe.DataSource = dt
            repFromMe.DataBind()
        End If
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Reload()
    End Sub
    Public Sub Reload()
        If Not String.IsNullOrEmpty(SubjectID) Then
            checkDelegateTab()
        End If
    End Sub
End Class
