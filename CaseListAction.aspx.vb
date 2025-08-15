Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Website

Public Class CaseListAction
    Inherits ActionProcessorPage

    Protected Overrides ReadOnly Property PageUrl As String
        Get
            Return "CaseListAction.aspx"
        End Get
    End Property
    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Dim selType As Integer = QueryStringParser.GetInt("SELTYPE")
        Dim actionTypeInt As Integer = QueryStringParser.GetInt("ACTIONTYPE")
        Dim actionID As String = QueryStringParser.GetString("ACTIONID")
        Dim lbAskParams As Boolean = False

        HideProgress()

        If selType <> 5 Then
            'execute action on the selection
            Dim actionType As Actions.CaseListAction.eActionType = CType(actionTypeInt, Actions.CaseListAction.eActionType)
            Select Case actionType
                Case Actions.CaseListAction.eActionType.UserEvent
                    If Arco.Utils.GUID.IsGUID(actionID) Then
                        Dim loUEV As UserEvent = UserEvent.GetUserEvent(actionID)
                        If loUEV IsNot Nothing Then
                            actionID = loUEV.ID.ToString
                        End If
                    End If

                    If Not QueryStringParser.GetBoolean("UEVPARAMSOK") Then
                        Dim loUEV As UserEvent = UserEvent.GetUserEvent(Convert.ToInt32(actionID))
                        If loUEV.ParameterScreen > 0 Then
                            AskUEVParamsForSelection(loUEV, CType(selType, DMSelection.SelectionType))
                            lbAskParams = True
                        End If
                    End If
                Case Actions.CaseListAction.eActionType.Custom
                    actionID = ConvertCustomActionActionId(actionID)
            End Select


            If Not lbAskParams Then
                Try
                    Dim loAction As New Actions.CaseListAction(actionType, CType(selType, DMSelection.SelectionType), Convert.ToInt32(actionID), 7)
                    loAction = loAction.Execute()

                    ProcessResult(loAction.Result, False)

                Catch ex As Exception
                    Message(ex)
                End Try
            End If

        Else

            Dim llOIP As Int32 = QueryStringParser.GetInt("OIP")
            Dim actionType As Actions.CaseAction.eActionType = CType(actionTypeInt, Actions.CaseAction.eActionType)
            Select Case actionType
                Case Actions.CaseAction.eActionType.UserEvent
                    If Arco.Utils.GUID.IsGUID(actionID) Then
                        Dim loCase As Routing.cCase = Routing.cCase.GetCase(llOIP)
                        Dim loUEV As UserEvent = UserEvent.GetUserEvent(actionID, loCase.Proc_ID)
                        If loUEV Is Nothing Then
                            loUEV = UserEvent.GetUserEvent(actionID, loCase.CurrentProcedure.DM_CAT_ID)
                        End If
                        If loUEV IsNot Nothing Then
                            actionID = loUEV.ID.ToString
                        End If
                    End If

                    If Not QueryStringParser.GetBoolean("UEVPARAMSOK") Then
                        Dim loUEV As UserEvent = UserEvent.GetUserEvent(Convert.ToInt32(actionID))
                        If loUEV.ParameterScreen > 0 Then
                            AskUEVParams(loUEV, "TECH_ID", llOIP)
                            lbAskParams = True
                        End If
                    End If
                Case Actions.CaseAction.eActionType.Custom
                    actionID = ConvertCustomActionActionId(actionID)
            End Select

            If Not lbAskParams Then
                'execute on the case               
                Dim leFrom As IUserEvent.eCallerLocation = CType(QueryStringParser.GetInt("FROM", 1), IUserEvent.eCallerLocation)
                Try
                    Dim action As New Actions.CaseAction(actionType, llOIP, Convert.ToInt32(actionID), leFrom)
                    action = action.Execute()

                    ProcessResult(action.Result, True)
                Catch ex As Exception
                    Message(ex)
                End Try

            End If

        End If

        Page.ClientScript.RegisterStartupScript(Me.GetType, "DoResult", GetScript, True)
    End Sub
    Private Function ConvertCustomActionActionId(ByVal actionId As String) As String

        If Arco.Utils.GUID.IsGUID(actionId) Then
            Dim custact As DMCustomAction = DMCustomAction.GetCustomAction(actionId)
            If custact IsNot Nothing Then
                Return custact.ID.ToString
            End If
        End If

        Return actionId
    End Function
End Class

