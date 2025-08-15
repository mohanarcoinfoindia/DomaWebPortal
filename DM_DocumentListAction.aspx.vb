Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Website

Public Class DM_DocumentListAction
    Inherits ActionProcessorPage

    Public Sub New()
        AllowGuestAccess = True
    End Sub
    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Dim selType As Integer = QueryStringParser.GetInt("SELTYPE")
        Dim actionTypeInt As Integer = QueryStringParser.GetInt("ACTIONTYPE")
        Dim actionID As String = QueryStringParser.GetString("ACTIONID")

        Dim askParams As Boolean = False

        HideProgress()

        If selType <> 5 Then
            'execute action on the selection
            Dim actionType As Actions.ObjectListAction.eActionType = CType(actionTypeInt, Actions.ObjectListAction.eActionType)
            Select Case actionType
                Case Actions.ObjectListAction.eActionType.UserEvent
                    actionID = ConvertUserEventActionId(actionID)

                    If Not QueryStringParser.GetBoolean("UEVPARAMSOK") Then
                        Dim loUEV As UserEvent = UserEvent.GetUserEvent(Convert.ToInt32(actionID))
                        If loUEV.ParameterScreen > 0 Then
                            AskUEVParamsForSelection(loUEV, CType(selType, DMSelection.SelectionType))
                            askParams = True
                        End If
                    End If
                Case Actions.ObjectListAction.eActionType.Custom
                    actionID = ConvertCustomActionActionId(actionID)
            End Select

            If Not askParams Then
                Try
                    Dim loAction As New Actions.ObjectListAction(actionType, CType(selType, DMSelection.SelectionType), Convert.ToInt32(actionID))
                    loAction = loAction.Execute()
                    ProcessResult(loAction.Result, False)

                Catch ex As Exception
                    Message(ex)
                End Try

            End If

        Else
            'execute on the object
            Dim llOIP As Integer = QueryStringParser.GetInt("OIP")

            Dim actionType As Actions.ObjectAction.eActionType = CType(actionTypeInt, Actions.ObjectAction.eActionType)
            Select Case actionType
                Case Actions.ObjectAction.eActionType.UserEvent
                    actionID = ConvertUserEventActionId(actionID)

                    If Not QueryStringParser.GetBoolean("UEVPARAMSOK") Then
                        Dim loUEV As UserEvent = UserEvent.GetUserEvent(Convert.ToInt32(actionID))
                        If loUEV.ParameterScreen > 0 Then
                            AskUEVParams(loUEV, "DM_OBJECT_ID", llOIP)
                            askParams = True
                        End If
                    End If
                Case Actions.ObjectAction.eActionType.Custom
                    actionID = ConvertCustomActionActionId(actionID)
            End Select


            If Not askParams Then
                Try
                    Dim leFrom As IUserEvent.eCallerLocation = CType(QueryStringParser.GetInt("FROM", 1), IUserEvent.eCallerLocation)
                    Dim loAction As New Actions.ObjectAction(actionType, llOIP, Convert.ToInt32(actionID), leFrom)
                    loAction = loAction.Execute()
                    ProcessResult(loAction.Result, True)
                Catch ex As Exception
                    Message(ex)
                End Try
            End If
        End If


        Page.ClientScript.RegisterStartupScript(Me.GetType, "DoResult", GetScript, True)
    End Sub

    Private Function ConvertUserEventActionId(ByVal actionId As String) As String

        If Arco.Utils.GUID.IsGUID(actionId) Then
            Dim loUEV As UserEvent = UserEvent.GetUserEvent(actionId)
            If loUEV IsNot Nothing Then
                Return loUEV.ID.ToString
            End If
        End If

        Return actionId
    End Function
    Private Function ConvertCustomActionActionId(ByVal actionId As String) As String

        If Arco.Utils.GUID.IsGUID(actionId) Then
            Dim custact As DMCustomAction = DMCustomAction.GetCustomAction(actionId)
            If custact IsNot Nothing Then
                Return custact.ID.ToString
            End If
        End If

        Return actionId
    End Function
    Protected Overrides ReadOnly Property PageUrl As String
        Get
            Return "DM_DocumentListAction.aspx"
        End Get
    End Property
End Class
