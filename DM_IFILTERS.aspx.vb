Imports Arco.Utils

Partial Class DM_IFILTERS
    Inherits BaseAdminOnlyPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'load the contents
        Dim lcolFilters As TextFilterImplementations = New TextFilterImplementations
        Dim loFilter As TextFilterImplementation

        Dim sbContent As StringBuilder = New StringBuilder("<table class='List PaddedTable StickyHeaders'>")
        sbContent.AppendLine("<thead>")
        sbContent.AppendLine("<tr class='ListHeader'>")
        sbContent.AppendLine("<th>Name</th> <th>Path</th>")
        sbContent.AppendLine("</tr></thead>")

        sbContent.AppendLine("<tbody>")
        For Each loFilter In lcolFilters
            sbContent.AppendLine("<tr>")
            sbContent.AppendLine("<td><b>" & loFilter.Description & " </b></td>")
            sbContent.AppendLine("<td>" & loFilter.DllFileName & "</td>")
            sbContent.AppendLine("</tr>")
        Next
        sbContent.AppendLine("</tbody>")
        sbContent.AppendLine("</table>")
        Dim content As Label = New Label
        content.Text = sbContent.ToString()

        FullTextFiltersPanel.Controls.Add(content)
    End Sub
End Class
