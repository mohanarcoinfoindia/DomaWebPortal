Imports Arco.Doma.Library
Imports Arco.Doma.Library.Search

Partial Class Linked
    Inherits BasePage

    Protected HeaderText As String = "Links"
    Private mbHideTabs As Boolean


    Private Sub BindGrid(ByVal grid As DM_Listcontrol, ByVal xml As String, ByVal caption As String, ByVal crit As DM_OBJECTSearch.Criteria)
        grid.GridXMLFile = xml
        grid.GridHeader = ArcoFormatting.FormatLabel(caption)
        grid.ShowFolders = UserProfile.ShowFoldersInList
        grid.ShowQuery = UserProfile.ShowQuery
        grid.Mode = DocroomListHelpers.GridMode.Normal
        grid.CanEdit = False
        grid.DataBind(crit)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Dim loLoaded As Arco.ApplicationServer.Library.BusinessBase = QueryStringParser.CurrentDMObject
            If loLoaded Is Nothing Then
                GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INVALIDOBJECT)
                Exit Sub
            End If
            Dim loSourceObj As baseObjects.DM_OBJECT
            If TypeOf loLoaded Is Routing.cCase Then
                loSourceObj = DirectCast(loLoaded, Arco.Doma.Library.Routing.cCase).TargetObject
            ElseIf TypeOf loLoaded Is Routing.HistoryCase Then
                GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INVALIDOBJECT)
                Exit Sub
            Else
                loSourceObj = DirectCast(loLoaded, baseObjects.DM_OBJECT)
            End If
            If loSourceObj.ID > 0 Then
                BindGrid(relationshipsgrid, "LinkedDocsGrid.xml", GetDecodedLabel("relatedto"), DM_OBJECTSearch.Criteria.GetNewLinkedObjectsGridCriteria(loSourceObj.DIN))

                If loSourceObj.Object_Type = "ListItem" Then
                    Dim loCrit As DM_OBJECTSearch.Criteria = DM_OBJECTSearch.Criteria.GetNewGridCriteria()
                    loCrit.ContainingLinkToListItem = loSourceObj.ID
                    BindGrid(packagelinksgrid, "SearchResultsgrid.xml", GetDecodedLabel("usedin"), loCrit)
                Else
                    Dim loCrit As DM_OBJECTSearch.Criteria = DM_OBJECTSearch.Criteria.GetNewGridCriteria()
                    '   loCrit.Object_Type = "Dossier"                    
                    loCrit.ContainingLinkToObjectDIN = loSourceObj.DIN
                    BindGrid(packagelinksgrid, "DossiersGrid.xml", GetDecodedLabel("linkedto"), loCrit)
                End If
                Dim loShortcutsCrit As DM_OBJECTSearch.Criteria = DM_OBJECTSearch.Criteria.GetNewGridCriteria()
                loShortcutsCrit.ReferenceID = loSourceObj.DIN
                loShortcutsCrit.Object_Type = "Shortcut"
                loShortcutsCrit.WorkAccessGivesViewRights = False 'no need for work check on shortcuts

                BindGrid(shortcutsgrid, "SearchResultsgrid.xml", GetDecodedLabel("shortcuts"), loShortcutsCrit)
            End If
        End If
    End Sub
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit

        ParseQueryString()

        If Not mbHideTabs Then
            Me.MasterPageFile = "~/masterpages/Preview.master"
        Else
            Me.MasterPageFile = "~/masterpages/DetailSub.master"
        End If
    End Sub
    Private Sub ParseQueryString()
        mbHideTabs = QueryStringParser.GetBoolean("hidetabs")
    End Sub
End Class
