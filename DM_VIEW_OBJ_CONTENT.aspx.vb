Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Indexing

Partial Class DM_VIEW_OBJ_CONTENT
    Inherits BaseAdminOnlyPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim loLoaded As BusinessBase = QueryStringParser.CurrentDMObject
        If loLoaded Is Nothing Then
            Exit Sub
        End If

        Dim o As DM_OBJECT = TryCast(loLoaded, DM_OBJECT)
        If o IsNot Nothing Then
            o.AddAudit("VIEWINDEX", "Viewed index")
        End If

        Dim fileName As String = "content_" & loLoaded.ID & ".txt"

        Arco.Utils.Web.Streamer.StreamStream(Request, Response, DirectCast(loLoaded, IFullTextIndexed).GetFullText, fileName, fileName, False, DateTime.MinValue, "")
    End Sub
End Class
