Namespace Doma

Partial Class DM_ACL_DENIED
        Inherits BaseAnonymousPage

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

        Public Overrides Sub GotoErrorPage(code As Arco.Doma.Library.baseObjects.LibError.ErrorCode, singlepagelogin As Boolean)
            'we are the error page, don't do anything
        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            'Put user code to initialize the page here         
            Dim lsMsg As String = GetLabel("accessdenied")

            Dim lsCode As String = QueryStringParser.GetString("code")
            If Not String.IsNullOrEmpty(lsCode) Then
                Dim leCode As Arco.Doma.Library.baseObjects.LibError.ErrorCode = CType(lsCode, Arco.Doma.Library.baseObjects.LibError.ErrorCode)
                If leCode <> Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_LICENCE Then
                    lsMsg = lsMsg & " : " & Arco.Doma.Library.baseObjects.LibError.GetErrorDescription(leCode)
                Else
                    lsMsg = "Licence message" ' Me.LicenceMessage
                    'Dim lic As Arco.Doma.Library.Licensing.License = Arco.Doma.Library.Licensing.License.GetLicence("Docroom")
                    'lic.ValidateLicence()
                    'lsMsg = lic.Message
                End If
            End If
            lblMessage.Text = lsMsg
        End Sub

End Class

End Namespace
