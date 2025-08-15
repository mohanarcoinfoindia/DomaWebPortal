
Partial Class UserControls_LookupItemTooltip
    Inherits BasePage
    Private Function InfoLine(ByVal vsField As String, ByVal vsValue As String, ByVal htmlEncode As Boolean) As String
        If htmlEncode Then
            Return String.Format("<tr><td ><b>{0}:</b></td><td >{1}</td></tr>", vsField, Server.HtmlEncode(vsValue))
        End If
        Return String.Format("<tr><td ><b>{0}:</b></td><td >{1}</td></tr>", vsField, vsValue)
    End Function
    Protected Sub UserControls_LookupItemTooltip_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim lsContent As StringBuilder = New StringBuilder
        Dim propid As Int32 = QueryStringParser.GetInt("PROP_ID")
        Dim lsValue As String = Request.QueryString("PROP_VAL")
        Dim mlCaseTechID As Int32 = QueryStringParser.GetInt("CASE_TECH_ID")
        Dim mlObjID As Int32 = QueryStringParser.GetInt("DM_OBJECT_ID")
        Dim llRowID As Int32 = QueryStringParser.GetInt("ROW_ID")

     
        Dim loProp As Arco.Doma.Library.baseObjects.DM_PROPERTY = Arco.Doma.Library.baseObjects.DM_PROPERTY.GetPROPERTY(propid)
        Dim lcolLabels As Arco.Doma.Library.Globalisation.LABELList
        If loProp.ProcID > 0 Then
            lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetProcedureItemsLabelList(loProp.ProcID, Me.EnableIISCaching)
        Else
            lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetCategoryItemsLabelList(loProp.CatID, Me.EnableIISCaching)
        End If

        Page.Title = lcolLabels.GetObjectLabel(loProp.ID, "Property", Language, loProp.Name)


        Dim filters As Arco.Doma.Library.MultiPartFilter = loProp.GetListItemFilters()

        For Each f As Arco.Doma.Library.Filter In filters.Filters
            If (f.isPropertyFilter) Then
                If Not Request.QueryString("linked_prop_id_" + f.Property_Name.ToLower()) Is Nothing Then
                    f.Value = Request.QueryString("linked_prop_id_" + f.Property_Name.ToLower())
                End If
            Else
                If f.Property_Name = "VALUEFILTER" Then
                    f.Value = lsValue
                End If
            End If
        Next

        Dim loList As List(Of Arco.Doma.Library.PropertyListItem)
        If mlCaseTechID > 0 Then
            Dim loCase As Arco.Doma.Library.Routing.cCase = Arco.Doma.Library.Routing.cCase.GetCase(mlCaseTechID)
            loList = loProp.GetListItems(loCase, Arco.Doma.Library.ListRangeRequest.AllItems, filters, llRowID, Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.TooltipWindow, False)
        Else
            Dim loObject As Arco.Doma.Library.baseObjects.DM_OBJECT = Arco.Doma.Library.ObjectRepository.GetObject(mlObjID)
            loList = loProp.GetListItems(loObject, Arco.Doma.Library.ListRangeRequest.AllItems, filters, llRowID, Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.TooltipWindow, False)
        End If
        Page.Title = lcolLabels.GetObjectLabel(loProp.Id, "Property", Arco.Security.BusinessIdentity.CurrentIdentity.Language, loProp.Name)

        If loList.Any Then
            lsContent.Append("<table width='100%' cellspacing='0' cellpadding='1' >")
            For Each itm As Arco.Doma.Library.PropertyListItem In loList
                For Each c As Arco.Doma.Library.CaptionPart In itm.Caption
                    Dim encodeHtml As Boolean
                    Select Case c.EncodeHtml
                        Case Arco.Doma.Library.FieldFlag.True
                            encodeHtml = True
                        Case Arco.Doma.Library.FieldFlag.False
                            encodeHtml = False
                        Case Else
                            encodeHtml = loProp.EncodeHtml
                    End Select
                    lsContent.Append(InfoLine(Arco.Web.ResourceManager.ReplaceLabeltags(c.Header), Arco.Web.ResourceManager.ReplaceLabeltags(c.Caption), encodeHtml))
                Next
                Exit For
            Next
            lsContent.Append("</table>")
        End If

        litContent.Text = lsContent.ToString
    End Sub
End Class
