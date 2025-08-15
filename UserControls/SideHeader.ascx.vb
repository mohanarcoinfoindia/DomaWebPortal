
Imports Arco.Doma.Library.Security

Partial Class UserControls_SideHeader
    Inherits BaseUserControl


    Protected Sub repLvl1_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles repLvl1.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim loNode As SiteMapNode = CType(e.Item.DataItem, SiteMapNode)

            If AddSitemapNode(loNode) Then
                Dim ctrls As List(Of Control) = GetHyperLink(loNode)
                For Each c As Control In ctrls
                    e.Item.Controls.Add(c)
                Next
            End If


        End If
    End Sub

    Protected Function GetHyperLink(ByVal loNode As SiteMapNode) As List(Of Control)

        Dim ctrls As List(Of Control) = New List(Of Control)

        Dim ctrl As HyperLink = New HyperLink
        ctrl.SkinID = "SideNavigationHyperLink"
        ctrl.NavigateUrl = loNode.Url
        Dim sb As New StringBuilder(128)

        If isTabActive(loNode) Then
            sb.Append("<div class='SideMenu Selected'>")
        Else
            sb.Append("<div class='SideMenu'>")
        End If
        sb.Append("<img src='./Images/Basket.png'/><div>")
        sb.Append(loNode.Title)
        sb.Append("</div></div>")

        ctrl.Text = sb.ToString
        ctrls.Add(ctrl)
        Return ctrls
    End Function

    Private Function AddSitemapNode(ByVal node As SiteMapNode) As Boolean
        Dim lbAdd As Boolean = True
        If node.Roles.Count <> 0 Then
            lbAdd = BusinessIdentity.CurrentIdentity.IsInOneOfRoles(node.Roles.Cast(Of String))
        End If
        If Not String.IsNullOrEmpty(node("licence")) Then
            'todo
            '    lbAdd = Arco.Doma.Library.Licensing.License.GetLicencedApplications(False).Contains(node("licence"))
        End If
        If Not String.IsNullOrEmpty(node("AdminOnly")) Then
            lbAdd = BusinessIdentity.CurrentIdentity.isAdmin
        End If
        Return lbAdd
    End Function

    Protected Function isTabActive(ByVal tab As SiteMapNode) As Boolean
        Return SiteMap.CurrentNode IsNot Nothing AndAlso (SiteMap.CurrentNode.Equals(tab) OrElse SiteMap.CurrentNode.IsDescendantOf(tab))
    End Function
End Class
