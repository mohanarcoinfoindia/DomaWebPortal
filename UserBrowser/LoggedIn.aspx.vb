Imports Arco.Doma.Library
Imports Arco.Doma.Library.Security

Public Class UserBrowser_LoggedIn
    Inherits BaseAdminOnlyPage

    Public Property MultiTenant As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        MultiTenant = Settings.GetValue("General", "MultiTenant", False) AndAlso BusinessIdentity.CurrentIdentity.Tenant.IsGlobal

        pnlInfo.Visible = False
        LogoffUser()
        BindGrid()
        RegisterLogOffScript()
    End Sub

    Private Sub LogoffUser()
        If String.IsNullOrEmpty(hdnLogin.Value) Then
            Exit Sub
        End If

        BusinessIdentity.LogoffUser(hdnLogin.Value, hdnApp.Value, hdnToken.Value)
        hdnLogin.Value = ""
        hdnToken.Value = ""
    End Sub
    Private Sub BindGrid()
        Dim loUsers As SessionList = SessionList.GetSessions(Nothing, GetActualOrderBy())
        lstUsers.DataSource = loUsers
        lstUsers.DataBind()

    End Sub


    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "USER_LOGIN", "MODULE", "TENANT_ID", "START_DATE", "LAST_ACTION", "USER_AGENT", "REMOTE_ADDRESS"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function


    Protected Function FormatUserName(ByVal entry As SessionList.SessionInfo) As String
        If String.IsNullOrEmpty(entry.Impersonator) Then
            Return ArcoFormatting.FormatUserName(entry.Login)
        End If
        Return ArcoFormatting.FormatUserName(entry.Login) & " (" & ArcoFormatting.FormatUserName(entry.Impersonator) & ")"

    End Function

    Private Sub RegisterLogOffScript()
        Dim sb As New StringBuilder
        sb.Append("function LogOff(user,app,token,userdesc){")
        sb.Append("if (window.confirm('")
        sb.AppendFormat(GetDecodedLabel("confirmlogoffuser"), "' + userdesc + '")
        sb.Append("')){")
        sb.Append("document.getElementById('")
        sb.Append(hdnLogin.ClientID)
        sb.Append("').value = user;")

        sb.Append("document.getElementById('")
        sb.Append(hdnApp.ClientID)
        sb.Append("').value = app;")

        sb.Append("document.getElementById('")
        sb.Append(hdnToken.ClientID)
        sb.Append("').value = token;")
        sb.Append("document.forms[0].submit();}}")
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "logoff", sb.ToString, True)
    End Sub
    Protected Function LogOffButton(ByVal user As SessionList.SessionInfo) As String
        Return String.Format("<a href='javascript:LogOff({0},{1},{2},{3});' class='ButtonLink'>{4}</a>", EncodingUtils.EncodeJsString(ACL.User.AddDummyDomain(user.Login)), EncodingUtils.EncodeJsString(user.Application), EncodingUtils.EncodeJsString(user.Token), EncodingUtils.EncodeJsString(ArcoFormatting.FormatUserName(user.Login)), GetLabel("logoff"))
    End Function
    Protected Sub lnkLogoffIdleUsers_Click(sender As Object, e As EventArgs)
        Jobs.Job.ExecuteJob(15)

        ShowInfo("The log-off idle users job has been started")

    End Sub

    Private Sub ShowInfo(text As String)
        lblInfo.Text = text
        pnlInfo.Visible = True
    End Sub
End Class
