Imports Arco.Doma.Library

Partial Class DM_EXPLORE
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim parentId As Integer = QueryStringParser.GetInt("DM_PARENT_ID", -1)
        If parentId >= 0 Then

            Dim lsCurrentSite As String = SiteManagement.SitesManager.CurrentSite

            Dim lsSite As String = ""
            Dim lsTitle As String = ""

            If parentId > 0 Then
                Dim loFolder As baseObjects.DM_OBJECT = ObjectRepository.GetObject(parentId)

                While TypeOf loFolder Is Shortcut 'load the object the shortcut points too
                    If loFolder.Object_Reference > 0 Then
                        loFolder = CType(loFolder, Shortcut).GetReferencedObject
                        parentId = loFolder.ID
                    Else
                        'shortcut not pointing anywhere
                        Response.Write("Illegal shortcut")
                        Response.End()
                    End If
                End While
                lsTitle = loFolder.Name
                lsSite = loFolder.CategoryObject.ExploreSite
                If String.IsNullOrEmpty(lsSite) Then
                    Select Case loFolder.Object_Type
                        Case "Folder"
                            lsSite = "ExploreFolder"
                        Case "Dossier"
                            lsSite = "ExploreDossier"
                    End Select
                End If
            End If

            'redirect
            Response.Redirect("Default.aspx?DM_PARENT_ID=" & parentId & "&SITE=" & lsSite & "&title=" & StringEncryptionService.Encrypt(lsTitle) & "&DEFAULTSITE=" & lsCurrentSite)
        End If
    End Sub
End Class
