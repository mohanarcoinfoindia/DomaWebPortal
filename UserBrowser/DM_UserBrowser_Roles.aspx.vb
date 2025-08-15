Imports Arco.Doma.Library

Partial Class DM_UserBrowser_Roles
    Inherits UserBrowserBasePage
#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here


        If Request.QueryString("delete") = "1" And Not Me.IsPostBack Then
            Dim s As String = ACL.Role.DeleteRoleWithCheck(Request.QueryString("roleid"))

            If Not String.IsNullOrEmpty(s) Then
                Dim sb As New StringBuilder
                sb.AppendLine("alert('" & EncodingUtils.EncodeJsString(s) & "');")
                Page.ClientScript.RegisterClientScriptBlock(GetType(String), "Alert", sb.ToString, True)
            Else
                ' RefreshLeftPane()
                Server.Transfer("DM_UserBrowser_Roles.aspx?delete=0&txtRoleName=" & Request.QueryString("txtRoleName") & "&txtRoleDesc=" & Request.QueryString("txtRoleDesc") & "&roletype=" & Request.QueryString("roletype"), False)
            End If
        End If
        If Not IsPostBack Then

            txtRoleType.Value = Request.QueryString("roletype")
            ROLE_ID_RES.Value = Request.QueryString("ROLE_ID")
            ROLE_NAME_RES.Value = Request.QueryString("ROLE_NAME")
        End If
    End Sub

End Class
