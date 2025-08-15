Imports Arco.Doma.Library

Partial Class UserControls_PromptPackage
    Inherits BasePage

    Protected Sub UserControls_PromptPackage_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.Title = GetLabel("select")
        lblPack.Text = GetLabel("addtopackage")
        btnCancel.Text = GetLabel("cancel")
        btnOk.Text = GetLabel("ok")

        Dim toObject As baseObjects.DM_OBJECT = ObjectRepository.GetObject(Convert.ToInt32(Request.QueryString("TO_OBJECT_ID")))
        Dim toAddObject As baseObjects.DM_OBJECT = ObjectRepository.GetObject(Convert.ToInt32(Request.QueryString("DM_OBJECT_ID")))
        Dim lcolLabels As Globalisation.LABELList = Globalisation.LABELList.GetCategoryItemsLabelList(toObject.Category, Arco.Doma.Library.Helpers.ArcoInfo.GetParameters.GetValue("Interface", "EnableIISCaching", Me.EnableIISCaching))
        Dim iCount As Int32 = 0
        For Each p As baseObjects.PackageList.PackageInfo In toObject.Packages
            If toObject.CanAddToPackage(p, toAddObject) Then
                Dim lsName As String = lcolLabels.GetObjectLabel(p.ID, "Package", Arco.Security.BusinessIdentity.CurrentIdentity.Language, p.Name)
                drpPack.Items.Add(New System.Web.UI.WebControls.ListItem(lsName, p.ID) With {.Selected = (toObject.CategoryObject.InputPackage = p.ID)})
                iCount += 1
            End If
        Next
        Select Case iCount
            Case 1
                Page.ClientScript.RegisterStartupScript(Me.GetType, "select", "SetPrompt();", True)
            Case 0
                rowNotFound.visible = True
                lblNotFOund.Text = GetLabel("noresultsfound")
                rowPackage.Visible = False
                btnOk.Visible = False
        End Select
    End Sub
End Class
