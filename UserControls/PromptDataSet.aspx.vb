Imports Arco.Doma.Library

Partial Class UserControls_PromptDataSet
    Inherits BasePage


    Protected Sub UserControls_PromptCategory_Load(sender As Object, e As EventArgs) Handles Me.Load
        Title = "Add to Dataset"
        btnClose.Text = GetLabel("cancel")
        btnSelect.Text = GetLabel("ok")

        Dim count As Integer = 0
        Dim itemId As Integer = QueryStringParser.GetInt("ITEM_ID")
        Dim allDataSets As DMObjectDatasetList = DMObjectDatasetList.GetDatasetList()
        '   Dim currentDataSets As DMObjectDatasetList = DMObjectDatasetList.GetDatasetList(itemId)

        For Each ds As DMObjectDatasetList.DatasetInfo In allDataSets
            If Not ds.CanEdit() Then Continue For
            drpDataSet.Items.Add(New System.Web.UI.WebControls.ListItem(ds.Name, ds.ID))
            count += 1
        Next
        Select Case count
            'Case 1
            '    Page.ClientScript.RegisterStartupScript(Me.GetType, "select", "SetPrompt();", True)
            Case 0
                rowNotFound.visible = True
                lblNotFOund.Text = GetLabel("noresultsfound")
                rowDataSet.Visible = False
                btnselect.Visible = False
        End Select
    End Sub
End Class
