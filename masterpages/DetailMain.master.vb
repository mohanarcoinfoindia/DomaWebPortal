
Partial Class DetailMain
    Inherits BaseMasterPage
    Public Overrides Property ParentID() As Int32
        Get
            Return PC.ParentID
        End Get
        Set(ByVal value As Int32)
            PC.ParentID = value
        End Set
    End Property
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        PC.ReloadFunction = "SaveObject()"
        PC.ShowPreview = UserProfile.ShowPreviewInDetailWindows ' Not UserProfile.ShowFilesInSeparateWindow
    End Sub
End Class


