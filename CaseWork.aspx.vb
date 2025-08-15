
Public Class CaseWork
    Inherits BasePage
    Private mbHideTabs As Boolean
    Private mNuMResults As Int32

    Protected thisCase As Arco.Doma.Library.Routing.cCase
    Protected Sub lstWork_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles lstWork.RowCommand
         Select e.CommandName
            Case "cmdAssignCase"
                Dim index As Integer = Convert.ToInt32(e.CommandArgument)
                Dim selectedRow As GridViewRow = lstWork.Rows(index)

                thisCase = thisCase.AdminAssignCase(selectedRow.Cells(3).Text)
            Case "cmdUnlockCase"
                thisCase = thisCase.UnLockCase()
            Case "cmdDeleteItem"
                Dim index As Integer = Convert.ToInt32(e.CommandArgument)
                Dim selectedRow As GridViewRow = lstWork.Rows(index)
                Dim selectedCell As TableCell = selectedRow.Cells(0)

                thisCase = thisCase.RemoveWorkItem(Convert.ToInt32(selectedCell.Text))
        End Select

        sShowWork()
    End Sub
    Private Enum eCell
        ID = 0
        SubjectCaption = 1
        SubjectType = 2
        SubjectUnformatted = 3
        AssignButton = 4
        UnlockButton = 5
        DeleteButton = 6
    End Enum
    Protected Sub lstWork_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles lstWork.RowDataBound
        Dim r As GridViewRow = e.Row

        r.Cells(eCell.ID).Visible = False
        r.Cells(eCell.SubjectUnformatted).Visible = False

        If r.RowType = DataControlRowType.DataRow Then
            Dim subjectType As String = r.Cells(eCell.SubjectType).Text
            Dim subject As String = Server.HtmlDecode(r.Cells(eCell.SubjectUnformatted).Text)
            Dim subjectCaption As String = GetSubjectCaption(subject, subjectType)

            r.Cells(eCell.SubjectType).Text = GetLabel(r.Cells(eCell.SubjectType).Text)

            CType(r.Cells(eCell.DeleteButton).Controls(0), LinkButton).Text = GetLabel("delete")
            CType(r.Cells(eCell.UnlockButton).Controls(0), LinkButton).Text = GetLabel("unlock")
            CType(r.Cells(eCell.AssignButton).Controls(0), LinkButton).Text = GetLabel("assign")
            If subject = thisCase.StepExecutor Then
                r.Cells(eCell.SubjectCaption).Text = "<font color='red'>" & subjectCaption & "</font>"
                CType(r.Cells(eCell.DeleteButton).Controls(0), LinkButton).Visible = False
                CType(r.Cells(eCell.AssignButton).Controls(0), LinkButton).Visible = False
            Else
                r.Cells(eCell.SubjectCaption).Text = subjectCaption
                CType(r.Cells(eCell.UnlockButton).Controls(0), LinkButton).Visible = False
                If subjectType <> "User" Then
                    CType(r.Cells(eCell.AssignButton).Controls(0), LinkButton).Visible = False
                End If
            End If
        ElseIf r.RowType = DataControlRowType.Header Then
            r.Cells(eCell.SubjectCaption).Text = ""
        End If
    End Sub
    Private Function GetSubjectCaption(ByVal vsSubjectID As String, ByVal vsSubjectType As String) As String
        Select Case vsSubjectType
            Case "Role", "Group"
                Return ArcoFormatting.FormatUserName("(" & vsSubjectType & ") " & vsSubjectID)
            Case Else
                Return ArcoFormatting.FormatUserName(vsSubjectID)
        End Select
    End Function
    Private Sub sShowWork()
        lstWork.DataSource = thisCase.WorkItemList
        lstWork.DataBind()
    End Sub
  
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        thisCase = QueryStringParser.CurrentDMObject
        If Not thisCase Is Nothing Then
            If thisCase.CanAdminCaseStatus Then

                Me.Page.Title = thisCase.Name & ": " & GetLabel("Work")
                lblAdd.Text = GetLabel("add")
                lnkRecalculate.Text = GetLabel("recalculatework")

                If Page.IsPostBack Then
                    If newUser.Subject <> "" Then
                        Dim lbFound As Boolean = False
                        For Each w As Arco.Doma.Library.Routing.WorkItemList.WorkInfo In thisCase.WorkItemList
                            If w.Subject = newUser.SubjectID And w.SubjectType = newUser.SubjectType Then
                                lbFound = True
                                Exit For
                            End If
                        Next
                        If Not lbFound Then
                            thisCase = thisCase.AddWorkItem(newUser.SubjectID, newUser.SubjectType)
                        End If
                        newUser.Clear()
                    End If
                End If
                sShowWork()
            Else
                GotoErrorPage()
            End If
        Else
            Response.Write("case not found")
        End If
    End Sub
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit

        If Not QueryStringParser.GetBoolean("inline") Then
            Me.MasterPageFile = "~/masterpages/ToolWindow.master"
        Else
            Me.MasterPageFile = "~/masterpages/DetailSub.master"
        End If
    End Sub
   

    Protected Sub lnkRecalculate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkRecalculate.Click
        If Not thisCase Is Nothing AndAlso thisCase.CanAdminCaseStatus Then
            thisCase = thisCase.RecalculateWork()
            sShowWork()        
        End If
    End Sub
End Class
