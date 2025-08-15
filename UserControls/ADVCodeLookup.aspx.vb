Imports System.Xml
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Connectors

Public Class UserControls_ADVCodeLookup
    Inherits BasePage
    Private mobjXML As System.Xml.XmlDocument
    Protected PropID As String
    Protected mbRefreshOnChange As Boolean
    Protected mbMultiSelect As Boolean
    Protected Autovalidate As String
    Protected msFocusField As String
    Protected msOrder As String
    Protected liPage As Int32
    Protected mbAutoSelect As Boolean
    Protected EditAdvancedLookups As Boolean = True
    Protected mbShowResults As Boolean
    '  Private mbIsIE As Boolean
    Protected CaseTechID As Int32
    Protected ObjID As Int32
    Protected RowID As String
    Private mbHasToolTip As Boolean


    Private _lastValue As String
    Private _lastDesc As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        '  mbIsIE = (InStr(UCase(Request.ServerVariables("HTTP_USER_AGENT")), "MSIE") > 0)
        Me.Form.DefaultButton = lnkEnter.UniqueID


        PropID = QueryStringParser.GetInt("PROP_ID")
        msOrder = Request("ORDERBY")
        RowID = QueryStringParser.GetInt("ROW_ID")

        liPage = 1
        If Not String.IsNullOrEmpty(Request("PAGE")) Then
            liPage = CType(Request("PAGE"), Int32)
        End If


        CaseTechID = QueryStringParser.GetInt("CASE_TECH_ID")


        ObjID = QueryStringParser.GetInt("DM_OBJECT_ID")

        If Not Page.IsPostBack Then
            Autovalidate = QueryStringParser.GetString("autovalidate")
        End If

        If Request("SEARCH") = "Y" OrElse Not String.IsNullOrEmpty(Autovalidate) Then
            mbShowResults = True
        Else
            mbShowResults = False
        End If


        Dim loProp As DM_PROPERTY = DM_PROPERTY.GetPROPERTY(PropID)
        Dim lcolLabels As Globalisation.LABELList
        If loProp.ProcID > 0 Then
            lcolLabels = Globalisation.LABELList.GetProcedureItemsLabelList(loProp.ProcID, Me.EnableIISCaching)
        Else
            lcolLabels = Globalisation.LABELList.GetCategoryItemsLabelList(loProp.CatID, Me.EnableIISCaching)
        End If

        Page.Title = lcolLabels.GetObjectLabel(loProp.ID, "Property", Arco.Security.BusinessIdentity.CurrentIdentity.Language, loProp.Name)

        mbRefreshOnChange = loProp.PROP_ROC
        mbMultiSelect = QueryStringParser.GetBoolean("multiselect", (loProp.VALUE_NUMBER = 1))
        Dim filter As MultiPartFilter = loProp.GetListItemFilters()

        For Each f As Arco.Doma.Library.Filter In filter.PropertyFilters
            If QueryStringParser.Exists("linked_prop_id_" + f.Property_Name.ToLower()) Then
                f.Value = QueryStringParser.GetString("linked_prop_id_" + f.Property_Name.ToLower())
            End If
        Next
        mbHasToolTip = loProp.GetListItemHeaders(Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.TooltipWindow, 7).Any
        Dim lsXML As String = ""
        If CaseTechID > 0 Then
            Dim loCase As Arco.Doma.Library.Routing.cCase = Arco.Doma.Library.Routing.cCase.GetCase(CaseTechID)
            lsXML = Arco.Doma.Library.TagReplacer.ReplaceTags(loProp.Definition, Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity().LanguageCode, loCase, filter, False, True, loProp.PARENT_PROP_ID, RowID)
        Else
            Dim loObject As Arco.Doma.Library.baseObjects.DM_OBJECT = Arco.Doma.Library.ObjectRepository.GetObject(ObjID)
            lsXML = Arco.Doma.Library.TagReplacer.ReplaceTags(loProp.Definition, Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity().LanguageCode, loObject, filter, False, True, loProp.PARENT_PROP_ID, RowID)
        End If

        mobjXML = New XmlDocument
        mobjXML.LoadXml(lsXML)

        ShowSearchFields()
        If mbShowResults Then
            ShowTable(loProp)
        Else
            '  Response.Write("Not showing results")
        End If


        If mbAutoSelect Then
            Dim sb As New StringBuilder()
            sb.Append("AutoSelect(")
            sb.Append(EncodingUtils.EncodeJsString(_lastValue))
            sb.Append(",")
            sb.Append(EncodingUtils.EncodeJsString(_lastDesc))
            sb.Append(");")

            Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "autoselect", sb.ToString(), True)
        Else
            If msFocusField <> "" Then
                Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "focus", "theForm." & msFocusField & ".focus();", True)
            End If
            Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "yourselection", "UpdateYourSelection();", True)
        End If

    End Sub

    Private Function FormatCaption(ByVal vsCaption As String) As String
        Return Arco.Web.ResourceManager.ReplaceLabeltags(vsCaption)
    End Function

    Private Shared Function RemoveQuotes(value As String) As String
        Return Replace(value, Chr(34), "")
    End Function
    Private Sub ShowSearchFields()
        Dim lobjRoot As XmlElement
        Dim loParamsNode As XmlNode
        Dim lcolParams As XmlNodeList
        Dim lobjParam As XmlNode
        Dim lsTable As StringBuilder = New StringBuilder(256)
        Dim liSR As Int32
        Dim lsVal As String
        Dim lsName As String

        lsTable.Append("<TABLE Class='DetailTable'><TR Class='DetailHeader'><TD class='DetailHeaderContent' colspan=2>")
        lsTable.Append(GetLabel("search"))
        lsTable.Append("</TD></TR>")
        lobjRoot = mobjXML.DocumentElement
        If Not Page.IsPostBack AndAlso Not mbShowResults Then
            If Not lobjRoot.SelectSingleNode("A") Is Nothing Then
                mbShowResults = (lobjRoot.SelectSingleNode("A").InnerText = "1")
            End If
        End If

        loParamsNode = lobjRoot.SelectSingleNode("PS")
        lcolParams = loParamsNode.SelectNodes("P")

        liSR = 0
        For Each lobjParam In lcolParams
            If lobjParam.SelectSingleNode("@SR").Value = "1" Then
                liSR = liSR + 1
            End If
        Next


        Dim i As Integer = 1
        For Each lobjParam In lcolParams
            If lobjParam.SelectSingleNode("@SR").Value = "1" Then
                lsName = "FLD_" & i
                If Not Page.IsPostBack Then
                    If liSR = 1 Then
                        lsVal = RemoveQuotes(Autovalidate)
                    Else
                        lsVal = ""
                    End If
                Else
                    lsVal = RemoveQuotes(Request.Form(lsName))
                End If
                lsTable.Append("<tr><TD class='LabelCell'>")
                lsTable.Append(ArcoFormatting.FormatLabel(FormatCaption(lobjParam.SelectSingleNode("N").InnerText)))
                lsTable.Append("</TD>")
                lsTable.Append("<TD class='FieldCell'><INPUT TYPE=TEXT SIZE=50 NAME=" & Chr(34) & lsName & Chr(34) & " VALUE=" & Chr(34) & lsVal & Chr(34) & "></TD></tr>")
                If String.IsNullOrEmpty(msFocusField) Then
                    msFocusField = lsName
                End If
            End If
            i += 1
        Next 'lobjField

        If liSR > 0 Then
            lsTable.Append("<tr><TD colspan='2'><div class='buttons'><a HREF='JavaScript:Search();' class='regular'>")
            lsTable.Append(GetLabel("search"))
            lsTable.Append("</a></div></TD><TD>&nbsp;</TD></tr>")
            lsTable.Append("</TABLE>")
        Else
            lsTable.Clear()
        End If

        lsTable.Append("<table Class='DetailTable'><TR><TD class='LabelCell'><span class='Label'>")
        lsTable.Append(GetLabel("yourselection"))
        lsTable.Append(" : </span></TD><TD width='100%' class='ReadOnlyFieldCell'><span id='lblSelection' name='lblSelection'></span></TD></tr>")

        If mbMultiSelect Then

            lsTable.Append("<tr><td colspan='2' align='right'><a HREF='JavaScript:Close();' class='ButtonLink'>" & GetLabel("close") & "</a>&nbsp;<a href='javascript:Clear();' class='ButtonLink'>")
            lsTable.Append(GetLabel("clear"))
            lsTable.Append("</a></td></tr>")
        End If
        lsTable.Append("</table>")

        plhSearch.Text = lsTable.ToString

        If liSR = 0 Then
            mbShowResults = True
        End If

    End Sub

    Protected Function JSIdentifier() As String
        Return PropID & "_" & RowID
    End Function

    Private Function Str(ByVal s As String) As String
        If Not String.IsNullOrEmpty(s) Then
            Return s.Trim
        End If
        Return ""
    End Function

    Private Sub FillInSearchValues()
        Dim lobjRoot As XmlElement = mobjXML.DocumentElement
        Dim loParamsNode As XmlNode = lobjRoot.SelectSingleNode("PS")
        Dim lcolParams As XmlNodeList = loParamsNode.SelectNodes("P")

        Dim liSR As Integer = 0
        For Each lobjParam In lcolParams
            If lobjParam.SelectSingleNode("@SR").Value = "1" Then
                liSR = liSR + 1
            End If
        Next

        Dim i As Integer = 1
        For Each lobjParam As XmlNode In lcolParams


            If lobjParam.SelectSingleNode("@SR").Value = "1" Then
                Dim lsName As String = "FLD_" & i

                Dim lsTemp As String = Nothing
                If Page.IsPostBack Then
                    lsTemp = Request.Form(lsName)
                ElseIf liSR = 1 Then
                    lsTemp = Autovalidate
                End If

                If Not String.IsNullOrEmpty(lsTemp) Then
                    lobjParam.SelectSingleNode("V").InnerText = lsTemp
                End If
            End If

            i += 1
        Next 'lobjField     
    End Sub

    Private Function ReplaceTags(ByVal vsPropDef As String, ByVal range As ListRangeRequest) As String
        If vsPropDef.Contains("#AUTOCOMPLETE#") Then
            vsPropDef = vsPropDef.Replace("#AUTOCOMPLETE#", "")
        End If
        If vsPropDef.Contains("#VALUEFILTER#") Then
            vsPropDef = vsPropDef.Replace("#VALUEFILTER#", "")
        End If
        If vsPropDef.Contains("#FIRSTRECORD#") Then
            vsPropDef = vsPropDef.Replace("#FIRSTRECORD#", range.FirstItem)
        End If
        If vsPropDef.Contains("#LASTRECORD#") Then
            vsPropDef = vsPropDef.Replace("#LASTRECORD#", range.LastItem)
        End If
        If vsPropDef.Contains("#MAXRESULTS#") Then
            vsPropDef = vsPropDef.Replace("#MAXRESULTS#", range.MaxResults)
        End If
        Return vsPropDef
    End Function
    Private Sub ShowTable(loProp As DM_PROPERTY)
        Dim sb As New StringBuilder
        Dim lobjRoot As XmlElement
        Dim lcolFieldsNode As XmlNode

        Dim colCount As Integer


        Dim laResults As List(Of ResultListItem)
        Dim loResult As ResultListItem
        Dim range As ListRangeRequest = ListRangeRequest.AllItems

        sb.Append("<table class='List'>")

        lobjRoot = mobjXML.DocumentElement
        Dim recsPerPage As Integer = CInt(lobjRoot.SelectSingleNode("R").InnerText)
        range.FirstItem = ((liPage - 1) * recsPerPage) + 1
        range.LastItem = range.FirstItem + recsPerPage - 1


        lcolFieldsNode = lobjRoot.SelectSingleNode("FS")

        sb.Append("<tr>")
        If mbHasToolTip Then
            sb.Append("<th width='20'>&nbsp;</th>")
            colCount += 1
        End If
        For Each lobjField As XmlNode In lcolFieldsNode.SelectNodes("F")
            Dim liDisplayMode As ExternalFieldDisplayMode = ExternalFieldDisplayMode.PopupWindow

            If Not lobjField.SelectSingleNode("DI") Is Nothing Then
                liDisplayMode = CType(Convert.ToInt32(lobjField.SelectSingleNode("DI").InnerText), ExternalFieldDisplayMode)
            End If
            If (liDisplayMode And ExternalFieldDisplayMode.PopupWindow) = ExternalFieldDisplayMode.PopupWindow Then
                sb.Append("<th>")
                sb.Append(FormatCaption(lobjField.SelectSingleNode("C").InnerText))
                sb.Append("</th>")

                colCount += 1
            End If
        Next 'lobfield

        sb.Append("</tr>")


        FillInSearchValues()


        Dim loCL As New CodeLookup
        laResults = loCL.GetResults(ReplaceTags(mobjXML.InnerXml, range))

        Dim index As Integer = 0

        For Each loResult In laResults
            index += 1
            If range.IsInRange(index) Then
                sb.Append("<tr ID='")
                sb.Append(index)
                sb.Append("'>")


                If mbHasToolTip Then
                    Dim value As String = ""
                    For Each loSubItem As ResultListSubItem In loResult.SubItems
                        If loSubItem.SelectionField Then
                            value = loSubItem.Value
                            Exit For
                        End If
                    Next
                    Dim lsTooltipUrl As String = "'LookupitemTooltip.aspx?PROP_ID=" & PropID & "&DM_OBJECT_ID=" & ObjID & "&CASE_TECH_ID=" & CaseTechID & "&PROP_VAL=" & Server.UrlEncode(value) & "'"
                    Dim lsToolipLink As String = "<span onmouseover=""ajax_showTooltip(" & lsTooltipUrl & ",this,event);return false"" onmouseout=""ajax_hideTooltip()""><a href=""#"" onclick=""javascript:var w = window.open(" & lsTooltipUrl & ",'ExtraInfo','width=400,height=300,scrollbars=yes,resizable=yes');w.focus();return false;"">"
                    lsToolipLink &= "<img src='../Images/info.gif' border='0' alt='Info' >"
                    lsToolipLink &= "</span>"

                    sb.Append("<td width='20'>")
                    sb.Append(lsToolipLink)
                    sb.Append("</td>")
                End If

                _lastValue = ""
                _lastDesc = ""

                Dim sbDesc As New StringBuilder()

                For Each loSubItem As ResultListSubItem In loResult.SubItems


                    If loSubItem.SelectionField Then
                        _lastValue = loSubItem.Value
                    End If
                    If (loSubItem.DisplayMode And ExternalFieldDisplayMode.DetailWindow) = ExternalFieldDisplayMode.DetailWindow Then
                        If sbDesc.Length <> 0 Then
                            sbDesc.Append(", ")
                        End If
                        sbDesc.Append(loSubItem.Value)
                    End If
                Next

                _lastDesc = sbDesc.ToString()

                For Each loSubItem As ResultListSubItem In loResult.SubItems

                    If (loSubItem.DisplayMode And ExternalFieldDisplayMode.PopupWindow) = ExternalFieldDisplayMode.PopupWindow Then
                        sb.Append("<td nowrap><a href='javascript:Select(")
                        sb.Append(EncodingUtils.EncodeJsString(_lastValue))
                        sb.Append(",")
                        sb.Append(EncodingUtils.EncodeJsString(_lastDesc))
                        sb.Append(");'>")
                        Dim lsCaption As String = Str(loSubItem.Value)
                        Select Case loSubItem.EncodeHtml
                            Case FieldFlag.Inherited
                                If loProp.EncodeHtml Then
                                    lsCaption = Server.HtmlEncode(lsCaption)
                                End If
                            Case FieldFlag.True
                                lsCaption = Server.HtmlEncode(lsCaption)
                        End Select
                        sb.Append(lsCaption)

                        sb.Append("</a></td>")
                    End If

                Next

                sb.Append("</tr>")
            End If

        Next
        Dim numberOfResults As Integer = Index

        If numberOfResults = 1 Then '1 result found
            mbAutoSelect = (Not Page.IsPostBack OrElse Not mbMultiSelect)
        End If

        sb.Append("<TR><TD COLSPAN=")
        sb.Append(colCount)
        sb.Append(" ALIGN='CENTER'>")

        If numberOfResults > 0 Then


            sb.Append(GridScroller.GetGridScroller(Me, liPage, recsPerPage, numberOfResults))

        Else
            sb.Append("<b>")
            sb.Append(GetLabel("noresultsfound"))
            sb.Append("</b>")
        End If
        sb.Append("</TD></TR></table>")

        plhResults.Text = sb.ToString

    End Sub


End Class
