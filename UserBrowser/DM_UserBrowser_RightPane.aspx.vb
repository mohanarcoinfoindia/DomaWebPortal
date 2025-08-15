Imports Arco.Doma.Library

Partial Class DM_UserBrowser_RightPane
    Inherits BasePage
#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load   
        If action.Value = "D" Then
            If Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsUserAdmin Then
                Dim s As String = ACL.User.DeleteUserWithCheck(USER_LOGIN_RES.Value)
                If Not String.IsNullOrEmpty(s) Then
                    Dim sb As New StringBuilder
                    sb.AppendLine("alert('" & s & "');")
                    Page.ClientScript.RegisterClientScriptBlock(GetType(String), "Alert", sb.ToString, True)
                End If
            Else
                GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
            End If
            action.Value = ""
            USER_LOGIN_RES.Value = ""
        End If
        If Not IsPostBack Then


            USER_DISPLAY_NAME_RES.Value = QueryStringParser.GetString("USER_DISPLAY_NAME")
            USER_LOGIN_RES.Value = QueryStringParser.GetString("USER_LOGIN")

        End If


    End Sub

    Protected Sub DoSearch(ByVal sender As Object, ByVal e As EventArgs)

    End Sub
  
End Class



