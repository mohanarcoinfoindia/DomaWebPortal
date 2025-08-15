
Partial Class UserControls_PromptCategory
    Inherits BasePage


    Protected Sub UserControls_PromptCategory_Load(sender As Object, e As EventArgs) Handles Me.Load
        Title = GetLabel("category")
        lblCat.Text = GetLabel("select")
        btnCancel.Text = GetLabel("cancel")
        btnOk.Text = GetLabel("ok")

        drpCat.CategoryType = Request.QueryString("DM_OBJECT_TYPE")
        drpCat.ForInsert = True
    End Sub


End Class
