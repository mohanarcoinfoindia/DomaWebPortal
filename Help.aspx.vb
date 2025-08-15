
Partial Class Help
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim HelpUrl As String
        Dim lbOk As Boolean = True
        HelpUrl = "./Help/WebHelp_" & Arco.Security.BusinessIdentity.CurrentIdentity.Language & ".pdf"
        If Not Arco.IO.File.Exists(Server.MapPath(HelpUrl)) Then
            HelpUrl = "./Help/WebHelp_" & Settings.GetValue("Interface", "DefaultLanguage", "E") & ".pdf"
            If Not Arco.IO.File.Exists(Server.MapPath(HelpUrl)) Then
                HelpUrl = "./Help/WebHelp_E.pdf"
                If Not Arco.IO.File.Exists(Server.MapPath(HelpUrl)) Then
                    lbOk = False
                End If
            End If
        End If

        If lbOk Then
            Response.Redirect(HelpUrl)
        Else
            Response.Write(Arco.Web.ErrorHandler.GetErrorForm("Help files are not installed."))
        End If
    End Sub
End Class
