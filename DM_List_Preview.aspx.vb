Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Search

Partial Class DM_List_Preview
    Inherits BasePage

    Protected HeaderText As String = ""
    Private mbHideTabs As Boolean


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        CType(Master, BaseMasterPage).ReloadFunction = resultgrid1.ClientID & ".Reload();"

        resultgrid1.ShowFolders = UserProfile.ShowFoldersInList
        resultgrid1.ShowQuery = UserProfile.ShowQuery

        If Not Page.IsPostBack Then
            Dim loCrit As DM_OBJECTSearch.Criteria = Nothing
            Dim lsScreenMode As String = QueryStringParser.GetString("screenmode")

            If lsScreenMode = "links" Then
                HeaderText = Server.HtmlDecode(GetLabel("relatedto"))
                Dim loSourceObj As DM_OBJECT = GetObjectFromParentId()
                resultgrid1.DefaultGridXmlFile = ""
                resultgrid1.GridXMLFile = "LinkedDocsGrid.xml"
                loCrit = DM_OBJECTSearch.Criteria.GetNewLinkedObjectsGridCriteria(loSourceObj.DIN)
            ElseIf lsScreenMode = "prevversions" Then
                HeaderText = Server.HtmlDecode(GetLabel("previousversions"))
                resultgrid1.DefaultGridXmlFile = ""
                resultgrid1.GridXMLFile = "PreviousVersionsGrid.xml"
                resultgrid1.ShowToolbar = False


                If Not Page.IsPostBack Then
                    Dim doc As DM_OBJECT = GetObjectFromParentId()
                    If Not doc.CanViewPreviousVersions Then
                        GotoErrorPage(doc.GetLastError.Code)
                        Exit Sub
                    End If
                    loCrit = DM_OBJECTSearch.Criteria.GetNewAllVersionsGridCriteria(doc.DIN)
                    hdnDIN.Value = doc.DIN
                Else
                    loCrit = DM_OBJECTSearch.Criteria.GetNewAllVersionsGridCriteria(Convert.ToInt32(hdnDIN.Value))
                End If


            ElseIf lsScreenMode = "browse" Then
                HeaderText = Server.HtmlDecode(GetLabel("browse"))
                resultgrid1.GridXMLFile = "BrowseGrid.xml"
                loCrit = DM_OBJECTSearch.Criteria.GetNewBrowseGridCriteria(Website.Screen.ScreenSearchMode.Objects, flParentID)
            ElseIf lsScreenMode = "browsewithsubfolders" Then
                HeaderText = Server.HtmlDecode(GetLabel("browsewithsubfolders"))
                resultgrid1.GridXMLFile = "BrowseGrid.xml"
                loCrit = DM_OBJECTSearch.Criteria.GetNewBrowseGridCriteria(Website.Screen.ScreenSearchMode.Objects, flParentID)
                loCrit.DM_INCLUDESUBFOLDERS = True
            ElseIf lsScreenMode = "browsemywork" Then 'my work within a folder
                HeaderText = Server.HtmlDecode(GetLabel("work"))
                resultgrid1.GridXMLFile = "MyWorkGrid.xml"
                loCrit = DM_OBJECTSearch.Criteria.GetNewMyWorkGridCriteria(GetObjectFromParentId())
                loCrit.CaseSearch.ShowLockedDossiers = UserProfile.ShowLockedDossiers
            ElseIf lsScreenMode = "opencases" Then 'open cases on an object
                HeaderText = Server.HtmlDecode(GetLabel("opendossiers"))
                resultgrid1.GridXMLFile = "CaseGrid.xml"
                loCrit = DM_OBJECTSearch.Criteria.GetNewOpenCasesOnObjectCriteria(flParentID)
            ElseIf lsScreenMode = "mywork" Then 'my work on the object
                HeaderText = Server.HtmlDecode(GetLabel("work"))
                resultgrid1.GridXMLFile = "MyWorkGrid.xml"
                loCrit = DM_OBJECTSearch.Criteria.GetNewMyWorkOnObjectCriteria(flParentID)
            ElseIf lsScreenMode = "linkeddossiers" Then 'linked dossiers for the object
                HeaderText = "Linked Dossiers"
                resultgrid1.DefaultGridXmlFile = ""
                resultgrid1.GridXMLFile = "DossiersGrid.xml"
                loCrit = DM_OBJECTSearch.Criteria.GetNewGridCriteria()
                loCrit.Object_Type = "Dossier"

                If Not Page.IsPostBack Then
                    Dim doc As DM_OBJECT = GetObjectFromParentId()
                    If Not doc.CanViewMeta Then
                        GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                        Exit Sub
                    End If
                    loCrit.ContainingLinkToObjectDIN = doc.DIN
                    hdnDIN.Value = doc.DIN
                Else
                    loCrit.ContainingLinkToObjectDIN = Convert.ToInt32(hdnDIN.Value)
                End If

            End If


            resultgrid1.DataBind(loCrit)
        End If

    End Sub
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit

        ParseQueryString()

        If Not mbHideTabs Then
            Me.MasterPageFile = "~/masterpages/Preview.master"
        Else
            Me.MasterPageFile = "~/masterpages/DetailSub.master"
        End If
    End Sub
    Private Sub ParseQueryString()
        mbHideTabs = QueryStringParser.GetBoolean("hidetabs", False)
    End Sub
    Private Function GetObjectFromParentId() As DM_OBJECT

        Dim loThisVersion As DM_OBJECT = ObjectRepository.GetObject(flParentID)
        If loThisVersion Is Nothing Then
            loThisVersion = ObjectRepository.GetObject(flParentID, True)
        End If
        Return loThisVersion
    End Function

    Private Function flParentID() As Int32
        Return CType(Me.Master, BaseMasterPage).ParentID
    End Function



End Class
