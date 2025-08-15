
Partial Public Class DetailSub
    Inherits BaseMasterPage

    Public Overrides Property ParentID() As Int32
        Get
            Return PC.ParentID
        End Get
        Set(ByVal value As Int32)
            PC.ParentID = value
        End Set
    End Property
   
    Protected Sub DetailSub_Load(sender As Object, e As EventArgs) Handles Me.Load
        Master.BodyTag.Attributes.Add("class", "PreviewPage")
        PC.ReloadFunction = Me.ReloadFunction
    End Sub
End Class

