Imports Arco.Doma.Library

Partial Class DM_UserBrowser_Groups
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

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here

        If Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsGlobal Then
            GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        End If

        If QueryStringParser.GetString("delete") = "1" And Not IsPostBack Then
            Dim s As String = ACL.Group.DeleteGroupWithCheck(QueryStringParser.GetString("groupid"))
            If s <> "" Then
                Dim sb As New StringBuilder
                sb.AppendLine("alert('" & s & "');")
                Page.ClientScript.RegisterClientScriptBlock(GetType(String), "Alert", sb.ToString, True)
            Else
                Server.Transfer("DM_UserBrowser_Groups.aspx?delete=0&txtGName=" & QueryStringParser.GetString("txtGName") & "&txtGDesc=" & QueryStringParser.GetString("txtGDesc"), False)
            End If
        End If
        If Not Me.IsPostBack Then

            GROUP_NAME_RES.Value = QueryStringParser.GetString("GROUP_NAME")
            GROUP_DISPLAY_NAME_RES.Value = QueryStringParser.GetString("GROUP_DISPLAY_NAME")
        End If
    End Sub


  
    Protected Sub lnkSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkSearch.Click

    End Sub
End Class




