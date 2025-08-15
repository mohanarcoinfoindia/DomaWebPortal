Imports Arco.Doma.Library.Tasks

Partial Class TaskSet
    Inherits BaseAdminOnlyPage

    Protected Sub Tasks_Set_Init(sender As Object, e As EventArgs) Handles Me.Init
        lstSet.InputManager = WebControlFactory.CreateInputManager("im")
    End Sub

    Public Property SetID As Integer
        Get
            Return CInt(ViewState("ID"))
        End Get
        Set(value As Integer)
            ViewState("ID") = value
        End Set
    End Property
    Public Property FromObject As Integer
        Get
            Return CInt(ViewState("FO"))
        End Get
        Set(value As Integer)
            ViewState("FO") = value
        End Set
    End Property
    Public Property CatID As Integer
        Get
            Return CInt(ViewState("CID"))
        End Get
        Set(value As Integer)
            ViewState("CID") = value
        End Set
    End Property
    Protected Sub Tasks_Set_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            SetID = QueryStringParser.GetInt("ID")
            lblName.Text = GetLabel("name")
            lblDesc.Text = GetLabel("description")

            If SetID = 0 Then

                FromObject = QueryStringParser.GetInt("DM_OBJECT_ID")
                CatID = QueryStringParser.GetInt("CAT_ID")

                plhName.Visible = True
                plhItem.Visible = False
                Page.Title = GetLabel("new") & ": " & GetLabel("taskset")
            Else
                plhItem.Visible = True

                lstSet.CurrentSet = Arco.Doma.Library.Tasks.TaskSet.GetTaskSet(SetID)
                txtName.Text = lstSet.CurrentSet.Name
                txtDesc.Text = lstSet.CurrentSet.Description
                Page.Title = lstSet.CurrentSet.Name
            End If
        End If
    End Sub

    Protected Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        Dim taskSet As Arco.Doma.Library.Tasks.TaskSet

        If SetID = 0 Then

            taskSet = Arco.Doma.Library.Tasks.TaskSet.NewTaskSet()
            taskSet.Name = txtName.Text
            taskSet.Description = txtDesc.Text
            If FromObject <> 0 Then
                taskSet.Category = Arco.Doma.Library.ObjectRepository.GetObject(FromObject).Category
            Else
                taskSet.Category = CatID
            End If

            taskSet = taskSet.Save

            If FromObject <> 0 Then
                taskSet.ImportTaskList(TaskList.GetTasks(FromObject))
            End If

            Response.Redirect("TaskSet.aspx?ID=" & taskSet.ID)
        Else
            taskSet = Arco.Doma.Library.Tasks.TaskSet.GetTaskSet(SetID)
            taskSet.Name = txtName.Text
            taskSet.Description = txtDesc.Text
            taskSet = taskSet.Save
        End If
    End Sub
End Class
