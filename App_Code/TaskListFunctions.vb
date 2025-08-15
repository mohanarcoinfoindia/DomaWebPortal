Imports System.Web.Services
Imports Arco.Doma.Library
Imports Arco.Doma.Library.Tasks

<System.Web.Script.Services.ScriptService()> _
<WebService(Namespace:="http://tempuri.org/")> _
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Public Class TaskListFunctions
    Inherits WebService

    <WebMethod(EnableSession:=True)> _
    Public Sub CompleteTask(ByVal id As Int32)
        Dim tsk As Task = Task.GetTask(id)
        If tsk.ID = id Then
            tsk = tsk.Complete
        Else
            Throw New ArgumentException("Id is not valid", "id")
        End If
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub StartTask(ByVal id As Int32)
        Dim tsk As Task = Task.GetTask(id)
        If tsk.ID = id Then
            tsk = tsk.Start
        Else
            Throw New ArgumentException("Id is not valid", "id")
        End If
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub RestartTask(ByVal id As Int32)
        Dim tsk As Task = Task.GetTask(id)
        If tsk.ID = id Then
            tsk = tsk.Restart
        Else
            Throw New ArgumentException("Id is not valid", "id")
        End If
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub CancelTask(ByVal id As Int32)
        Dim tsk As Task = Task.GetTask(id)
        If tsk.ID = id Then
            tsk = tsk.Cancel("")
        Else
            Throw New ArgumentException("Id is not valid", "id")
        End If
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub DeleteTask(ByVal id As Int32, ByVal objectid As Int32)
        Dim o As baseObjects.DM_OBJECT = ObjectRepository.GetObject(objectid)
        o.DeleteTask(id)
    End Sub

    <WebMethod(EnableSession:=True)> _
    Public Sub MoveTask(ByVal objectid As Int32, ByVal id As Int32, ByVal otherid As Int32)
        Task.ReorderTasks(objectid, id, otherid)
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub SetTaskName(ByVal id As Int32, ByVal name As String)
        Dim tsk As Task = Task.GetTask(id)
        tsk.Name = name
        tsk.Save()
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub SetTaskDescription(ByVal id As Int32, ByVal desc As String)
        Dim tsk As Task = Task.GetTask(id)
        tsk.Description = desc
        tsk.Save()
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub SetTaskDueDate(ByVal id As Int32, ByVal duedate As String)
        Dim tsk As Task = Task.GetTask(id)
        tsk.DueDate = duedate
        tsk.Save()
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub DeleteTaskSetItem(ByVal id As Int32)
        TaskSetItem.DeleteTaskSetItem(id)
    End Sub

    <WebMethod(EnableSession:=True)> _
    Public Sub MoveTaskSetItem(ByVal setid As Int32, ByVal id As Int32, ByVal otherid As Int32)
        TaskSet.ReorderTaskSetItems(setid, id, otherid)
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub SetTaskSetItemName(ByVal id As Int32, ByVal name As String)
        Dim tsk As TaskSetItem = TaskSetItem.GetTaskSetItem(id)
        tsk.Name = name
        tsk.Save()
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub SetTaskSetItemDescription(ByVal id As Int32, ByVal desc As String)
        Dim tsk As TaskSetItem = TaskSetItem.GetTaskSetItem(id)
        tsk.Description = desc
        tsk.Save()
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub ToggleAutoStartTaskSetItem(ByVal id As Int32)
        Dim tsk As TaskSetItem = TaskSetItem.GetTaskSetItem(id)
        tsk.AutoStart = Not tsk.AutoStart
        tsk.Save()
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub ImportTaskSet(ByVal objectid As Int32, ByVal setid As Int32)
        If setid <> 0 AndAlso objectid <> 0 Then
            Dim o As baseObjects.DM_OBJECT = ObjectRepository.GetObject(objectid)
            o.ImportTaskSet(setid)
        End If
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub ToggleTaskGrouper(ByVal id As Int32)
        Dim tsk As Task = Task.GetTask(id)
        Select Case tsk.IsGrouper
            Case GrouperType.CollapsedGrouper
                tsk.IsGrouper = GrouperType.ExpandedGrouper
            Case GrouperType.ExpandedGrouper
                tsk.IsGrouper = GrouperType.CollapsedGrouper
        End Select
        tsk.Save()
    End Sub
    <WebMethod(EnableSession:=True)> _
    Public Sub ToggleTaskSetGrouper(ByVal id As Int32)
        Dim tsk As TaskSetItem = TaskSetItem.GetTaskSetItem(id)
        Select Case tsk.IsGrouper
            Case GrouperType.CollapsedGrouper
                tsk.IsGrouper = GrouperType.ExpandedGrouper
            Case GrouperType.ExpandedGrouper
                tsk.IsGrouper = GrouperType.CollapsedGrouper
        End Select
        tsk.Save()
    End Sub
End Class
