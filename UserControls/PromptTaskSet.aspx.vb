Imports Arco.Doma.Library.Tasks
Partial Class UserControls_PromptTaskSet
    Inherits BasePage


    Protected Sub UserControls_PUserControls_PromptTaskSet_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.Title = GetLabel("selecttaskset")
        lblSet.Text = GetLabel("taskset")
        btnCancel.Text = GetLabel("cancel")
        btnOk.Text = GetLabel("Ok")

        Dim iCount As Int32 = 0

        For Each taskSet As TaskSetList.TaskSetInfo In TaskSetList.GetTaskSets(QueryStringParser.GetInt("CAT_ID"))            
            drpTaskSet.Items.Add(New System.Web.UI.WebControls.ListItem(taskSet.Name, taskSet.ID))
            iCount += 1
        Next

        Select Case iCount
            Case 1
                Page.ClientScript.RegisterStartupScript(Me.GetType, "select", "SetPrompt();", True)
            Case 0
                rowNotFound.Visible = True
                lblNotFOund.Text = GetLabel("noresultsfound")
                rowSet.Visible = False
                btnOk.Visible = False
        End Select
    End Sub

    Private Sub UserControls_PromptTaskSet_Init(sender As Object, e As EventArgs) Handles Me.Init
        'Me.Title = GetLabel("select")
    End Sub
End Class
