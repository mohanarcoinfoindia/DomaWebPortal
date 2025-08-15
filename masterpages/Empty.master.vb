
Partial Class Empty
    Inherits BaseMasterPage

    Public Overrides Property ParentID() As Int32
        Get
            Return PC.ParentID
        End Get
        Set(ByVal value As Int32)
            PC.ParentID = value
        End Set
    End Property
End Class



