Imports Arco.Doma.WebControls.SiteManagement

Namespace Doma


Partial Class TreeLevel
        Inherits BasePage
        Dim loTreeView As Arco.Doma.Treeview.Treeview


        Public Sub New()
            AllowGuestAccess = True
        End Sub

#Region " Web Form Designer Generated Code "

        'This call is required by the Web Form Designer.
        <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub


    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
            'CODEGEN: This method call is required by the Web Form Designer
            'Do not modify it using the code editor.
            InitializeComponent()
        End Sub

#End Region

        Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
            loTreeView = New Arco.Doma.Treeview.Treeview(Me)

            loTreeView.OrderBy = TreeDefinition.OrderBy

            loTreeView.FilterAndOr = TreeDefinition.TreeFilter.AndOr
            For Each loFilter As TreeDefinition.TreeFilters.Filter In TreeDefinition.TreeFilter.Filters
                If loTreeView.FilterIsEnabled(loFilter.Index) Then
                    loTreeView.AddFilter(loFilter.Index, loFilter.NormalFilter, loFilter.Recursive)
                Else
                    If Not String.IsNullOrEmpty(loFilter.ReversedFilter) Then
                        loTreeView.AddDisabledFilter(loFilter.Index, loFilter.ReversedFilter, loFilter.Recursive)
                    End If
                End If
            Next
            loTreeView.SubTree = True

            Response.Write(loTreeView.BuildTree())

        End Sub

        Private moTreeDef As TreeDefinition
        Protected ReadOnly Property TreeDefinition As TreeDefinition
            Get
                If moTreeDef Is Nothing Then
                    Dim lsTreeFile As String = QueryStringParser.GetString("treefile")
                    If String.IsNullOrEmpty(lsTreeFile) Then
                        lsTreeFile = "Tree.xml"
                    End If
                    If Not lsTreeFile.Contains(".xml") Then
                        lsTreeFile = lsTreeFile & ".xml"
                    End If
                    moTreeDef = TreeDefinition.GetTreeLayout(lsTreeFile)
                End If
                Return moTreeDef
            End Get
        End Property

    End Class

End Namespace
