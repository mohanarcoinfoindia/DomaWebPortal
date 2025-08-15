
Public Class masterpages_Base
    Inherits BaseMasterPage

    Public ReadOnly Property BodyTag As HtmlGenericControl
        Get
            Return MasterPageBodyTag
        End Get
    End Property

    Private Sub masterpages_Base_Load(sender As Object, e As EventArgs) Handles Me.Load

        InitializePageTitle()

        Dim faviconTags As LiteralControl = New LiteralControl()
        faviconTags.Text = GenerateFaviconHeadTags(Me.Page)
        Page.Header.Controls.Add(faviconTags)

        AddDensityCssClass(BodyTag)

    End Sub

End Class

