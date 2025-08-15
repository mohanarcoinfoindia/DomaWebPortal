Imports Arco.Doma.Library.Routing
Imports Arco.Doma.WebControls.Controls
Partial Class CaseDeadlines
    Inherits BasePage
    Protected thisCase As cCase
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit

        If Not QueryStringParser.GetBoolean("inline") Then
            MasterPageFile = "~/masterpages/ToolWindow.master"
        Else
            MasterPageFile = "~/masterpages/DetailSub.master"
        End If
    End Sub
    Dim maFoundSteps As List(Of Integer)
    Dim mcolDeadlines As DeadLines
    Dim lcolLabels As Arco.Doma.Library.Globalisation.LABELList
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        btnOk.Text = GetLabel("save")

        thisCase = QueryStringParser.CurrentDMObject
        If Not thisCase Is Nothing Then
            If thisCase.CanAdminCaseStatus OrElse (thisCase.CaseData.Created_By = Arco.Security.BusinessIdentity.CurrentIdentity.Name AndAlso thisCase.CurrentProcedure.CaseCreatorCanSetDeadlines) Then
                maFoundSteps = New List(Of Integer)

                If Not Page.IsPostBack Then
                    mcolDeadlines = DeadLines.GetDeadLines(thisCase.Case_ID)
                End If
                lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetProcedureItemsLabelList(thisCase.Proc_ID, EnableIISCaching) 'todo : enableisscache param
                AddCaseField()
                AddStepField()
            Else
                GotoErrorPage()
            End If
        Else
            Response.Write("case not found")
        End If

    End Sub
    Private Sub SaveCaseDueDate()
        Dim dp As DateTimePicker = DirectCast(tblDeadlines.FindControl("DL_0"), DateTimePicker)
        If Not dp Is Nothing Then
            If (dp.SelectedDate.HasValue) Then
                Dim lsValue As String = dp.SelectedDate.Value.ToString(ArcoFormatting.GetDateTimeFormatForSave(False))
                thisCase.CaseData.DeathDate = lsValue
                thisCase = thisCase.Save
            End If
        End If

    End Sub
    Private Sub SaveStepDueDate()
        If thisCase.CurrentStep Is Nothing Then
            Exit Sub
        End If

        maFoundSteps.Add(thisCase.Step_ID)
        If TypeOf thisCase.CurrentStep.Step_Type.Handler Is StepTypes.IHasDeadlinesStepType Then
            Dim dp As DateTimePicker = DirectCast(tblDeadlines.FindControl("DL_" & thisCase.CurrentStep.ID), DateTimePicker)
            If Not dp Is Nothing Then
                If (dp.SelectedDate.HasValue) Then
                    thisCase.Step_DueDate = dp.SelectedDate.Value.ToString(ArcoFormatting.GetDateTimeFormatForSave(False))
                    thisCase = thisCase.Save

                    Deadline.NewDeadline(thisCase.Step_ID, thisCase.Case_ID, dp.SelectedDate.Value)
                End If
            End If
        End If

        SaveSteps(thisCase.CurrentStep)

    End Sub
    Private Sub SaveSteps(ByVal voStep As cStep)

        For Each loRoute As RouteList.RouteInfo In RouteList.GetOutgoingRoutes(voStep.ID, Route.WF_RouteType.WF_RouteType_Static)
            Dim loNextStep As cStep = cStep.GetStep(loRoute.NEXT_STEP_ID)
            If Not maFoundSteps.Contains(loNextStep.ID) Then
                maFoundSteps.Add(loNextStep.ID)
                If TypeOf loNextStep.Step_Type.Handler Is StepTypes.IHasDeadlinesStepType Then
                    Dim dp As DateTimePicker = DirectCast(tblDeadlines.FindControl("DL_" & loNextStep.ID), DateTimePicker)
                    If (dp.SelectedDate.HasValue) Then
                        Deadline.NewDeadline(loNextStep.ID, thisCase.Case_ID, dp.SelectedDate.Value)
                    End If
                End If
                SaveSteps(loNextStep)
            End If

        Next

    End Sub

    Private Sub AddStepField()
        If thisCase.CurrentStep Is Nothing Then
            Exit Sub
        End If

        If TypeOf thisCase.CurrentStep.Step_Type.Handler Is StepTypes.IHasDeadlinesStepType Then
            AddField(thisCase.CurrentStep, True)
        End If
        maFoundSteps.Add(thisCase.CurrentStep.ID)
        AddNextSteps(thisCase.CurrentStep)
    End Sub
    Private Sub AddNextSteps(ByVal voStep As cStep)

        For Each loRoute As RouteList.RouteInfo In RouteList.GetOutgoingRoutes(voStep.ID, Route.WF_RouteType.WF_RouteType_Static)
            Dim loNextStep As cStep = cStep.GetStep(loRoute.NEXT_STEP_ID)
            If Not maFoundSteps.Contains(loNextStep.ID) Then
                maFoundSteps.Add(loNextStep.ID)
                If TypeOf loNextStep.Step_Type.Handler Is StepTypes.IHasDeadlinesStepType Then
                    AddField(loNextStep, False)
                End If
                AddNextSteps(loNextStep)
            End If
        Next

    End Sub
    Private Sub AddCaseField()
        Dim loRow As New TableRow

        tblDeadlines.Rows.Add(loRow)

        Dim loCell As New TableCell
        loRow.Cells.Add(loCell)

        loCell.CssClass = "LabelCell"
        Dim loLabel As New Label With {
            .CssClass = "Label",
            .Text = GetLabel("case")
        }


        loCell.Controls.Add(loLabel)


        loCell = New TableCell
        loRow.Cells.Add(loCell)
        loCell.CssClass = "FieldCell"

        Dim picker As DateTimePicker = CreateDateTimePicker(0)

        If Not Page.IsPostBack Then

            Dim datValue As DateTime
            If DateTime.TryParse(thisCase.CaseData.DeathDate, datValue) Then
                picker.SelectedDate = datValue
            End If

        End If

        loCell.Controls.Add(picker)

    End Sub
    Private Function CreateDateTimePicker(ByVal vlStepID As Integer) As DateTimePicker
        Dim picker As DateTimePicker = WebControlFactory.NewDateTimePicker("DL_" & vlStepID)
        picker.DatePopupButton.ToolTip = GetDecodedLabel("selectdate")
        picker.TimePopupButton.ToolTip = GetDecodedLabel("selecttime")
        picker.MinDate = Date.MinValue
        picker.MaxDate = Date.MaxValue

        Return picker
    End Function
    Private Sub AddField(ByVal voStep As cStep, ByVal vbIsCurrent As Boolean)
        Dim loRow As New TableRow

        tblDeadlines.Rows.Add(loRow)

        Dim loCell As New TableCell
        loRow.Cells.Add(loCell)

        loCell.CssClass = "LabelCell"
        Dim loLabel As New Label With {
            .CssClass = "Label",
            .Text = lcolLabels.GetObjectLabel(voStep.ID, "Step", voStep.Name)
        }

        loCell.Controls.Add(loLabel)


        loCell = New TableCell
        loRow.Cells.Add(loCell)
        loCell.CssClass = "FieldCell"

        Dim picker As DateTimePicker = CreateDateTimePicker(voStep.ID)

        If Not Page.IsPostBack Then
            If Not vbIsCurrent Then
                For Each loDL As DeadLines.DeadLineInfo In mcolDeadlines
                    If loDL.Step_ID = voStep.ID Then
                        If Not String.IsNullOrEmpty(loDL.Deadline) Then
                            Dim datValue As DateTime
                            If DateTime.TryParse(loDL.Deadline, datValue) Then
                                picker.SelectedDate = datValue
                            End If
                        End If
                        Exit For
                    End If
                Next
            Else
                Dim datValue As DateTime
                If DateTime.TryParse(thisCase.Step_DueDate, datValue) Then
                    picker.SelectedDate = datValue
                End If
            End If
        End If

        loCell.Controls.Add(picker)

    End Sub

    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnOk.Click
        maFoundSteps = New List(Of Integer)
        Deadline.DeleteCaseDeadlines(thisCase.Case_ID)
        SaveCaseDueDate()
        SaveStepDueDate()


    End Sub
End Class
