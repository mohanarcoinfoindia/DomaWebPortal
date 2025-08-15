
Partial Class UserControls_PackageSearchLookup
    Inherits BasePage

    Protected Sub UserControls_PackageSearchLookup_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim packID As Integer = QueryStringParser.GetInt("PROP_ID")
        Dim lbModal As Boolean = QueryStringParser.GetInt("modal")

        Dim lbMultiSelect As Boolean = QueryStringParser.GetBoolean("multiselect", True)

        Dim loPack As Arco.Doma.Library.baseObjects.Package = Arco.Doma.Library.baseObjects.Package.GetPackage(packID)
        Dim lsgridFile As String = loPack.DocInfo1
        If String.IsNullOrEmpty(lsgridFile) Then
            lsgridFile = "PackageGrid.xml"
        End If
        Dim lsSite As String = DocroomListHelpers.GridLayout.LoadGridLayout(lsgridFile).SelectionSite
        If String.IsNullOrEmpty(lsSite) Then
            If loPack.PackType = Arco.Doma.Library.baseObjects.Package.ePackType.CasePackage Then
                lsSite = "AddToCasePackage"
            Else
                lsSite = "AddToPackage"
            End If
        End If

        Response.Redirect("~/Default.aspx?screenmode=query&TARGET_PACK_ID=" & packID & "&SITE=" & lsSite)
    End Sub
End Class
