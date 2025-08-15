Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Mail

Public Class MailClient_MailListAction
    Inherits ActionProcessorPage
    Protected Overrides ReadOnly Property PageUrl As String
        Get
            Return "MailListAction.aspx"
        End Get
    End Property

    Protected Overrides Sub OnLoad(e As EventArgs)
        MyBase.OnLoad(e)

        Dim selType As Int32 = QueryStringParser.GetInt("SELTYPE")
        Dim actionType As Int32 = QueryStringParser.GetInt("ACTIONTYPE")
        Dim actionID As Int32 = QueryStringParser.GetInt("ACTIONID")

        HideProgress()


        If selType <> 5 Then
            'execute action on the selection
            Dim loAction As New MailListAction(CType(actionType, MailListAction.eActionType), CType(selType, DMSelection.SelectionType), actionID)
            loAction = loAction.Execute()

            ProcessResult(loAction.Result)

        Else
            'execute on the mail
            Dim llOIP As Int32 = QueryStringParser.GetInt("OIP")
            Dim loAction As New MailAction(CType(actionType, MailAction.eActionType), llOIP, actionID)
            loAction = loAction.Execute()

            ProcessResult(loAction.Result)

        End If

        Page.ClientScript.RegisterStartupScript(Me.GetType, "DoResult", GetScript, True)

    End Sub

End Class
