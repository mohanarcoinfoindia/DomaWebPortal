
Partial Class CMS_DeletePanel
    Inherits BaseAdminOnlyPage
    Private PanelID As Integer
    Private PageID As Integer
    Protected Sub btnSave_Click(sender As Object, e As System.EventArgs) Handles btnSave.Click
        If PanelID <= 0 Then
            GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_UNEXPECTED)
        End If
        For Each page As Arco.Doma.CMS.Data.PageList.PageInfo In Arco.Doma.CMS.Data.PageList.GetPageList(PanelID)
            Arco.Doma.CMS.Data.PagePanel.DeletePagePanel(page.ID, PanelID)
        Next
        Arco.Doma.CMS.Data.Panel.DeletePanel(PanelID)

        Page.ClientScript.RegisterStartupScript(Me.GetType, "CloseModalAndReloadPage", "parent.location.href = parent.location;", True)
    End Sub

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        PanelID = QueryStringParser.GetInt("panelid")
        PageID = QueryStringParser.GetInt("pageid")

        lblMessage.Text = GetLabel("confirmdeletepanel")
    End Sub
    Protected Overrides ReadOnly Property ErrorPageUrl As String
        Get
            Return "../DM_ACL_DENIED.aspx"
        End Get
    End Property
End Class
