Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Mail

Partial Class MailClient_MailBoxAction
    Inherits ActionProcessorPage
    Protected Overrides ReadOnly Property PageUrl As String
        Get
            Return "MailBoxAction.aspx"
        End Get
    End Property
    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Request.UrlReferrer Is Nothing OrElse Request.UrlReferrer.Host <> Request.Url.Host Then
            Throw New UnauthorizedAccessException()
        End If

        Dim selType As Int32 = QueryStringParser.GetInt("SELTYPE")
        Dim actionType As Int32 = QueryStringParser.GetInt("ACTIONTYPE")
        Dim actionID As Int32 = QueryStringParser.GetInt("ACTIONID")
        Dim boxId As Int32 = QueryStringParser.GetInt("BOXID")


        HideProgress()

        If selType <> 5 Then
            'execute action on the selection
            Dim loAction As New MailBoxAction(CType(actionType, MailBoxAction.eActionType), CType(selType, DMSelection.SelectionType), actionID, CType(boxId, MailBoxItem.MailBox))
            loAction = loAction.Execute()
            ProcessResult(loAction.Result)

        Else
            'execute on the item
            Dim llOIP As Int32 = QueryStringParser.GetInt("OIP")
            Dim loAction As New MailBoxItemAction(CType(actionType, MailBoxItemAction.eActionType), llOIP, actionID, CType(boxId, MailBoxItem.MailBox))
            loAction = loAction.Execute()

            ProcessResult(loAction.Result)

        End If

        Page.ClientScript.RegisterStartupScript(Me.GetType, "DoResult", GetScript, True)
    End Sub

End Class
