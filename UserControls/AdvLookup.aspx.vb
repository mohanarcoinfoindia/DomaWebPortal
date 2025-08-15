Imports System.Xml
Imports Arco.Doma.Library.TextFormatters

Public Class UserControls_AdvLookup
    Inherits BasePage

    Private mobjXML As System.Xml.XmlDocument
    Protected PropID As Integer
    Protected mbRefreshOnChange As Boolean
    Protected mbMultiSelect As Boolean
    Protected Autovalidate As String
    Protected msFocusField As String
    Protected _orderBy As String
    Protected _orderByOrder As String
    Protected liPage As Int32

    Protected mbAutoSelect As Boolean
    Private _forSelection As Boolean
    Protected EditAdvancedLookups As Boolean = True
    Protected mbShowResults As Boolean
    Protected ObjID As Int32
    Protected CaseTechID As Int32
    Protected RowID As Integer
    Private AdvLookupSearchPrefix As String = "%"
    Private AdvLookupSearchSufix As String = "%"
    Private mbHasToolTip As Boolean

    Private Sub sLoad()

        Me.Form.DefaultButton = lnkEnter.UniqueID

        PropID = QueryStringParser.GetInt("PROP_ID")
        RowID = QueryStringParser.GetInt("ROW_ID")

        _orderBy = Request("ORDERBY")
        _orderByOrder = Request("ORDERBYORDER")

        liPage = 1

        If Not String.IsNullOrEmpty(Request("PAGE")) Then
            liPage = CType(Request("PAGE"), Int32)
        End If


        If Not Page.IsPostBack Then
            Autovalidate = QueryStringParser.GetString("autovalidate")
        End If

        If Request("SEARCH") = "Y" OrElse Not String.IsNullOrEmpty(Autovalidate) Then
            mbShowResults = True
        Else
            mbShowResults = False
        End If


        ObjID = QueryStringParser.GetInt("DM_OBJECT_ID")

        CaseTechID = QueryStringParser.GetInt("CASE_TECH_ID")


        Dim loProp As Arco.Doma.Library.baseObjects.DM_PROPERTY = Arco.Doma.Library.baseObjects.DM_PROPERTY.GetPROPERTY(PropID)
        Dim lcolLabels As Arco.Doma.Library.Globalisation.LABELList
        If loProp.ProcID > 0 Then
            lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetProcedureItemsLabelList(loProp.ProcID, Me.EnableIISCaching)
        Else
            lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetCategoryItemsLabelList(loProp.CatID, Me.EnableIISCaching)
        End If
        If loProp.Tag3 = 1 Then
            AdvLookupSearchPrefix = ""
        End If

        Page.Title = lcolLabels.GetObjectLabel(loProp.ID, "Property", Arco.Security.BusinessIdentity.CurrentIdentity.Language, loProp.Name)

        mbRefreshOnChange = loProp.PROP_ROC
        mbMultiSelect = QueryStringParser.GetBoolean("multiselect", (loProp.VALUE_NUMBER = 1))
        mbHasToolTip = loProp.GetListItemHeaders(Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.TooltipWindow, 7).Any

        _forSelection = (Request("forselection") = "Y")

        Dim filters As Arco.Doma.Library.MultiPartFilter = loProp.GetListItemFilters()

        For Each f As Arco.Doma.Library.Filter In filters.PropertyFilters
            If QueryStringParser.Exists("linked_prop_id_" + f.Property_Name.ToLower()) Then
                f.Value = QueryStringParser.GetString("linked_prop_id_" + f.Property_Name.ToLower())
            End If

        Next





        Dim lsXML As String = ""
        If CaseTechID > 0 Then
            Dim loCase As Arco.Doma.Library.Routing.cCase = Arco.Doma.Library.Routing.cCase.GetCase(CaseTechID)
            If loCase IsNot Nothing Then
                lsXML = Arco.Doma.Library.TagReplacer.ReplaceTags(loProp.Definition, Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.LanguageCode, loCase, filters, GetPropertyDefinitionReplaceFormatter(), True, loProp.PARENT_PROP_ID, RowID)
            Else
                Response.Write("Case " & CaseTechID & " not found")
            End If
        Else
            Dim loObject As Arco.Doma.Library.baseObjects.DM_OBJECT = Arco.Doma.Library.ObjectRepository.GetObject(ObjID)
            If loObject IsNot Nothing Then
                lsXML = Arco.Doma.Library.TagReplacer.ReplaceTags(loProp.Definition, Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.LanguageCode, loObject, filters, GetPropertyDefinitionReplaceFormatter(), True, loProp.PARENT_PROP_ID, RowID)
            Else
                Response.Write("object " & ObjID & " not found")
            End If
        End If


        mobjXML = New XmlDocument
        mobjXML.LoadXml(lsXML)

        ShowSearchFields()
        If mbShowResults Then
            ShowTable(loProp)
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
            If Not String.IsNullOrEmpty(msFocusField) Then
                Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "focus", "theForm." & msFocusField & ".focus();", True)
            End If

            Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "yourselection", "UpdateYourSelection();", True)

        End If
    End Sub

    Private Function GetPropertyDefinitionReplaceFormatter() As ITextFormatter

        Return New MultiFormatter(New List(Of ITextFormatter) From {New EscapeSingleQuotesFormatter, New XmlEncodeFormatter})

    End Function
    Private Sub sUnLoad()
        mobjXML = Nothing
    End Sub

    Protected Function JSIdentifier() As String
        Return PropID & "_" & RowID
    End Function

    Private Function fsFormatCaption(ByVal vsCaption As String) As String
        Return Arco.Web.ResourceManager.ReplaceLabeltags(vsCaption)
    End Function
    Private Sub ShowSearchFields()
        Dim lobjRoot As XmlElement
        Dim loFieldsNode As XmlNode
        Dim lcolFields As XmlNodeList
        Dim lobjField As XmlNode
        Dim lsTable As New StringBuilder
        Dim numberOfSearchableFields As Int32
        Dim lsVal As String

        lsTable.Append("<table Class='DetailTable'>")
        lsTable.Append("<TR Class='DetailHeader'><TD class='DetailHeaderContent' colspan=2>")
        lsTable.Append(GetLabel("search"))
        lsTable.Append("</TD></TR>")
        lobjRoot = mobjXML.DocumentElement
        If Not Page.IsPostBack AndAlso Not mbShowResults Then
            If Not lobjRoot.SelectSingleNode("A") Is Nothing Then
                mbShowResults = (lobjRoot.SelectSingleNode("A").InnerText = "1")
            End If
        End If
        loFieldsNode = lobjRoot.SelectSingleNode("FS")
        lcolFields = loFieldsNode.SelectNodes("F")
        Dim sbClear As StringBuilder = New StringBuilder
        sbClear.AppendLine("function ClearForm(){")

        For Each lobjField In lcolFields
            If lobjField.SelectSingleNode("@SR").Value = "1" Then
                numberOfSearchableFields += 1
            End If
        Next

        Dim i As Integer = 1
        Dim lsSearchTable As New StringBuilder
        For Each lobjField In lcolFields
            If lobjField.SelectSingleNode("@SR").Value = "1" Then
                Dim fieldName As String = "FLD_" & i
                lsVal = ""
                If Not Page.IsPostBack Then
                    If numberOfSearchableFields <= 1 Then
                        lsVal = Replace(Autovalidate, Chr(34), "")
                    End If
                Else
                    lsVal = Replace(Request.Form(fieldName), Chr(34), "")
                End If
                Dim lsType As String = GetFieldType(lobjField)

                lsSearchTable.Append("<TR><TD class='LabelCell'>")
                lsSearchTable.Append(ArcoFormatting.FormatLabel(fsFormatCaption(lobjField.SelectSingleNode("C").InnerText)))
                lsSearchTable.Append("</TD>")
                lsSearchTable.Append("<TD class='FieldCell'>")
                Select Case lsType
                    Case "BOOLEAN"
                        lsSearchTable.Append("<SELECT NAME=")
                        lsSearchTable.Append(Chr(34))
                        lsSearchTable.Append(fieldName)
                        lsSearchTable.Append(Chr(34))
                        lsSearchTable.Append(" VALUE=")
                        lsSearchTable.Append(Chr(34))
                        lsSearchTable.Append(lsVal)
                        lsSearchTable.Append(Chr(34))
                        lsSearchTable.Append(">")
                        lsSearchTable.Append("<OPTION VALUE='' ")
                        If String.IsNullOrEmpty(lsVal) Then
                            lsSearchTable.Append(" SELECTED")
                        End If
                        lsSearchTable.Append(">")
                        lsSearchTable.Append("</OPTION>")

                        lsSearchTable.Append("<OPTION VALUE='0' ")
                        If lsVal = "0" Then
                            lsSearchTable.Append(" SELECTED")
                        End If
                        lsSearchTable.Append(">")
                        lsSearchTable.Append(GetLabel("no"))
                        lsSearchTable.Append("</OPTION>")

                        lsSearchTable.Append("<OPTION VALUE='1' ")
                        If lsVal = "1" Then
                            lsSearchTable.Append(" SELECTED")
                        End If
                        lsSearchTable.Append(">")
                        lsSearchTable.Append(GetLabel("yes"))
                        lsSearchTable.Append("</OPTION>")

                        lsSearchTable.Append("</SELECT>")
                    Case Else
                        lsSearchTable.Append("<INPUT TYPE=TEXT SIZE=50 NAME=")
                        lsSearchTable.Append(Chr(34))
                        lsSearchTable.Append(fieldName)
                        lsSearchTable.Append(Chr(34))
                        lsSearchTable.Append(" VALUE=")
                        lsSearchTable.Append(Chr(34))
                        lsSearchTable.Append(lsVal)
                        lsSearchTable.Append(Chr(34))
                        lsSearchTable.Append(">")
                End Select

                lsSearchTable.Append("</TD></TR>")
                If String.IsNullOrEmpty(msFocusField) Then
                    msFocusField = fieldName
                End If

                sbClear.AppendLine("theForm." & fieldName & ".value='';")
            End If

            i += 1
        Next 'lobjField

        If numberOfSearchableFields > 0 Then
            Dim lsAndOR As String = ""
            Dim lsClearForm As String = ""

            If numberOfSearchableFields > 1 Then

                If Not Page.IsPostBack Then

                    lsVal = Replace(Autovalidate, Chr(34), "")

                Else
                    lsVal = Replace(Request.Form("LKPGLOBAL"), Chr(34), "")

                End If
                sbClear.AppendLine("theForm.LKPGLOBAL.value='';")
                lsTable.Append("<TR><TD class='LabelCell' >")
                lsTable.Append(ArcoFormatting.FormatLabel(GetLabel("all")))
                lsTable.Append("</TD>")
                lsTable.Append("<TD class='FieldCell'><INPUT TYPE=TEXT SIZE=50 NAME=")
                lsTable.Append(Chr(34))
                lsTable.Append("LKPGLOBAL")
                lsTable.Append(Chr(34))
                lsTable.Append(" VALUE=")
                lsTable.Append(Chr(34))
                lsTable.Append(lsVal)
                lsTable.Append(Chr(34))
                lsTable.Append("></TD></TR>")
                msFocusField = "LKPGLOBAL"

                lsTable.Append(lsSearchTable.ToString)

                lsClearForm = "<a href='javascript:ClearForm();' class='ButtonLink button secondary'>" & GetLabel("clear") & "</a>"
                sbClear.AppendLine("}")
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "clearform", sbClear.ToString, True)
            Else
                lsTable.Append(lsSearchTable.ToString)
            End If

            lsTable.Append("<TR><td>")
            lsTable.Append(lsClearForm)
            lsTable.Append("</td><td><div class='buttons'><a HREF='JavaScript:Search();' class='regular button positive'>")
            lsTable.Append(GetLabel("search"))
            lsTable.Append("</a></div></td></TR>")
            lsTable.Append("</table>")

        Else
            lsTable.Clear()
        End If

        lsTable.Append("<table Class='DetailTable'><TR><TD class='LabelCell'><span class='Label'>")
        lsTable.Append(GetLabel("yourselection"))
        lsTable.Append(" : </span></TD><TD width='100%' class='ReadOnlyFieldCell'><span id='lblSelection' name='lblSelection'></span></TD></tr>")
     
        If mbMultiSelect Then

            lsTable.Append("<tr><td colspan='2' align='right'><a HREF='JavaScript:Close();' class='ButtonLink button secondary'>" & GetLabel("close") & "</a>&nbsp;<a href='javascript:Clear();' class='ButtonLink button secondary'>")
            lsTable.Append(GetLabel("clear"))
            lsTable.Append("</a></td></tr>")
        End If
        lsTable.Append("</table>")

        '  lsTable.Append("</TD></TR></TABLE>")

        plhSearch.Text = lsTable.ToString

        If numberOfSearchableFields = 0 Then
            mbShowResults = True
        End If

    End Sub

    Private Function GetFieldType(ByVal voField As XmlNode) As String
        Dim lsType As String = ""
        If Not voField.SelectSingleNode("T") Is Nothing Then
            lsType = voField.SelectSingleNode("T").InnerText
        End If
        If lsType = "STRING" Then
            lsType = ""
        End If
        Return lsType
    End Function
    Private Function BuildSQL(ByVal vbCaseSensitive As Boolean, ByVal voDP As Arco.Server.DataProvider, ByVal orderBy As String) As String
        Dim lsSQL As StringBuilder = New StringBuilder
        Dim lobjRoot As XmlElement
        Dim loFieldsnode As XmlNode
        Dim lcolFields As XmlNodeList
        Dim lobjField As XmlNode
        Dim lsTables As String
        Dim lsWhereData As String
        Dim lsWhereSelection As String

        Dim lsLastPart As New StringBuilder
        Dim lsFirstPart As String

        Dim lbCriteriaFound As Boolean
        Dim lsConcat As String

        Dim AdvLookupAllowFullResults As Boolean = True

        lobjRoot = mobjXML.DocumentElement
        lsTables = lobjRoot.SelectSingleNode("T").InnerText
        If _forSelection AndAlso Not lobjRoot.SelectSingleNode("WS") Is Nothing Then
            lsWhereSelection = lobjRoot.SelectSingleNode("WS").InnerText
        Else
            lsWhereSelection = ""
        End If

        lsWhereData = lobjRoot.SelectSingleNode("W").InnerText

        lsFirstPart = ""

        lbCriteriaFound = False

        loFieldsnode = lobjRoot.SelectSingleNode("FS")
        lcolFields = loFieldsnode.SelectNodes("F")

        Dim loQueryParser As New Arco.QueryParser.LikeTextParser(voDP)

        Dim lsGlobal As String
        If Page.IsPostBack Then

            lsGlobal = Request.Form("LKPGLOBAL")
        Else
            lsGlobal = Autovalidate
        End If

        Dim i As Integer = 1
        For Each lobjField In lcolFields
            If String.IsNullOrEmpty(lsFirstPart) Then
                lsFirstPart = "SELECT " & lobjField.SelectSingleNode("V").InnerText
            Else
                lsFirstPart = lsFirstPart & " , " & lobjField.SelectSingleNode("V").InnerText
            End If

            If lobjField.SelectSingleNode("@SR").Value = "1" Then
                Dim lsSearchValue As String = ""
                Dim lsType As String = GetFieldType(lobjField)
                Dim lsField As String = lobjField.SelectSingleNode("V").InnerText
                If Page.IsPostBack Then

                    lsSearchValue = Request.Form("FLD_" & i)
                    If String.IsNullOrEmpty(lsSearchValue) Then
                        If String.IsNullOrEmpty(lsType) Then lsSearchValue = lsGlobal
                        lsConcat = "OR"
                    Else
                        lsConcat = "AND"
                    End If
                Else
                    If String.IsNullOrEmpty(lsType) Then lsSearchValue = lsGlobal
                    lsConcat = "OR"
                End If

                If Not String.IsNullOrEmpty(lsSearchValue) Then
                    lbCriteriaFound = True

                    If lsLastPart.Length > 0 Then
                        lsLastPart.Append(" ")
                        lsLastPart.Append(lsConcat)
                        lsLastPart.Append(" ")
                    End If

                    Select Case lsType
                        Case "NUMBER"
                            Dim loNumberParser As New Arco.QueryParser.NumberParser(voDP)
                            lsLastPart.Append(loNumberParser.Parse(lsField, lsSearchValue))
                        Case "BOOLEAN"
                            lsLastPart.Append(lsField & " = " & lsSearchValue)
                        Case "DATE", "DATETIME"
                            Dim loDateParser As New Arco.QueryParser.DateParser(voDP, ArcoFormatting.DateInputFormat)
                            lsLastPart.Append(loDateParser.Parse(lsField, lsSearchValue))
                        Case Else
                            If Not loQueryParser.ContainsSpecialChars(lsSearchValue) Then
                                If vbCaseSensitive Then
                                    lsLastPart.Append("UPPER(" & lsField & ") Like '" & AdvLookupSearchPrefix & lsSearchValue.ToUpper.Replace("'", "''") & AdvLookupSearchSufix & "'")
                                Else
                                    lsLastPart.Append(lsField & " Like '" & AdvLookupSearchPrefix & lsSearchValue.Replace("'", "''") & AdvLookupSearchSufix & "'")
                                End If
                            Else
                                If vbCaseSensitive Then
                                    lsLastPart.Append(loQueryParser.Parse("UPPER(" & lsField & ")", lsSearchValue.ToUpper))
                                Else
                                    lsLastPart.Append(loQueryParser.Parse(lsField, lsSearchValue))
                                End If
                            End If
                    End Select


                End If
            End If

            i += 1
        Next 'lobjField
        lobjField = Nothing
        lcolFields = Nothing
        lobjRoot = Nothing

        lsSQL.Append(lsFirstPart)
        lsSQL.Append(" FROM ")
        lsSQL.Append(lsTables)

        If Not String.IsNullOrEmpty(lsWhereSelection) OrElse Not String.IsNullOrEmpty(lsWhereData) OrElse lsLastPart.Length > 0 Then

            If Not lbCriteriaFound AndAlso Not AdvLookupAllowFullResults Then
                lsLastPart.Clear()
                lsLastPart.Append("0=1") 'so the query never returns results
            End If

            lsSQL.Append(" WHERE ")
            Dim lbHasClause As Boolean = False
            If Not String.IsNullOrEmpty(lsWhereData) Then
                lsSQL.Append(lsWhereData)
                lbHasClause = True
            End If
            If Not String.IsNullOrEmpty(lsWhereSelection) Then
                If lbHasClause Then lsSQL.Append(" AND ")
                lsSQL.Append(lsWhereSelection)
                lbHasClause = True
            End If
            If lsLastPart.Length > 0 Then
                If lbHasClause Then lsSQL.Append(" AND ")
                lsSQL.Append("(")
                lsSQL.Append(lsLastPart.ToString)
                lsSQL.Append(")")
            End If
        End If

        If Not String.IsNullOrEmpty(orderBy) Then
            lsSQL.Append(" ORDER BY ")
            lsSQL.Append(orderBy)
        End If

        Return lsSQL.ToString
    End Function

    Private Function fStr(ByVal s As String) As String
        If Not String.IsNullOrEmpty(s) Then
            Return s.Trim
        Else
            Return String.Empty
        End If
    End Function


    Private Sub ShowTable(ByVal loProp As Arco.Doma.Library.baseObjects.DM_PROPERTY)
        Dim lstable As New StringBuilder
        Dim lobjRoot As XmlElement
        Dim lcolFieldsNode As XmlNode
        Dim lcolFields As XmlNodeList
        Dim lobjField As XmlNode
        Dim liColCount As Int32
        Dim liRecsPerPage As Int32
        Dim lsDSN As String
        Dim liteller3, liFirstRec, liLastRec As Int32
        Dim lbCaseSensitive As Boolean

        Dim leDefProvider As Arco.Server.ProviderType = Arco.Server.ProviderType.ODBC

        lstable.Append("<table class='List'>")

        lobjRoot = mobjXML.DocumentElement
        liRecsPerPage = CInt(lobjRoot.SelectSingleNode("R").InnerText)

        liFirstRec = GridScroller.FirstRecord(liPage, liRecsPerPage)
        liLastRec = GridScroller.LastRecord(liPage, liRecsPerPage)


        lsDSN = lobjRoot.SelectSingleNode("D").InnerText
        If Not lobjRoot.SelectSingleNode("DT") Is Nothing Then
            leDefProvider = Arco.Settings.FrameWorkSettings.StringToDatabaseType(lobjRoot.SelectSingleNode("DT").InnerText)
        End If
        lcolFieldsNode = lobjRoot.SelectSingleNode("FS")
        lcolFields = lcolFieldsNode.SelectNodes("F")
        liColCount = lcolFields.Count

        Dim actualOrderBy As String
        If String.IsNullOrEmpty(_orderBy) Then
            actualOrderBy = lobjRoot.SelectSingleNode("O").InnerText
        Else
            actualOrderBy = ""
        End If


        lstable.Append("<tr>")
        If mbHasToolTip Then
            lstable.Append("<th width='20'></th>")
        End If
        For Each lobjField In lcolFields
            Dim liDisplayMode As Arco.Doma.Library.Connectors.ExternalFieldDisplayMode = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow

            If Not lobjField.SelectSingleNode("DI") Is Nothing Then
                liDisplayMode = CType(Convert.ToInt32(lobjField.SelectSingleNode("DI").InnerText), Arco.Doma.Library.Connectors.ExternalFieldDisplayMode)
            End If

            If (liDisplayMode And Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow) = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow Then
                Dim orderbyOrder As String
                Dim fieldValue As String = lobjField.SelectSingleNode("V").InnerText


                If _orderBy = fieldValue Then
                    If _orderByOrder = "ASC" Then
                        actualOrderBy = fieldValue & " ASC"
                        orderbyOrder = "DESC"
                    Else
                        actualOrderBy = fieldValue & " DESC"
                        orderbyOrder = "ASC"
                    End If
                Else
                    If actualOrderBy = fieldValue & " ASC" Then
                        orderbyOrder = "DESC"
                    Else
                        orderbyOrder = "ASC"
                    End If
                End If

                lstable.Append("<th><a href=")
                lstable.Append(Chr(34))
                lstable.Append("javascript:OrderBy('")
                lstable.Append(fieldValue)
                lstable.Append("','")
                lstable.Append(orderbyOrder)
                lstable.Append("');")
                lstable.Append(Chr(34))
                lstable.Append(">")
                lstable.Append(fsFormatCaption(lobjField.SelectSingleNode("C").InnerText))
                lstable.Append("</a></th>")
            End If

        Next 'lobfield


        lstable.Append("</tr>")

        Using loQRY As Arco.Server.DataQuery = Arco.Doma.Library.Connectors.SQLDSNParser.GetDataQuery(lsDSN, leDefProvider)
            lbCaseSensitive = loQRY.DataProvider.CaseSensitive
            liteller3 = 0
            loQRY.Query = BuildSQL(lbCaseSensitive, loQRY.DataProvider, actualOrderBy)
            Using lobjRS As Arco.Server.SafeDataReader = loQRY.ExecuteReader()
                While lobjRS.Read
                    liteller3 += 1
                    If liteller3 >= liFirstRec AndAlso liteller3 <= liLastRec Then
                        lstable.Append("<tr ID='")
                        lstable.Append(liteller3)
                        lstable.Append("'>")

                        _lastValue = ""
                        _lastDesc = ""
                        Dim fieldCounter As Integer = 0

                        If mbHasToolTip Then
                            Dim value As String = ""
                            For Each lobjField In lcolFields
                                If lobjField.SelectSingleNode("@SL").Value = "1" Then
                                    value = fStr(lobjRS(fieldCounter))
                                    If value.IndexOf("||") > 0 Then
                                        value = value.Substring(0, value.IndexOf("||"))
                                    End If
                                    Exit For
                                End If
                                fieldCounter += 1
                            Next
                            fieldCounter = 0
                            Dim lsTooltipUrl As String = "'LookupitemTooltip.aspx?PROP_ID=" & PropID & "&DM_OBJECT_ID=" & ObjID & "&CASE_TECH_ID=" & CaseTechID & "&PROP_VAL=" & Server.UrlEncode(value) & "'"
                            Dim lsToolipLink As String = "<span onmouseover=""ajax_showTooltip(" & lsTooltipUrl & ",this,event);return false"" onmouseout=""ajax_hideTooltip()""><a href=""#"" onclick=""javascript:var w = window.open(" & lsTooltipUrl & ",'ExtraInfo','width=400,height=300,scrollbars=yes,resizable=yes');w.focus();return false;"">"
                            lsToolipLink &= "<img src='../Images/info.gif' border='0' alt='Info' >"
                            lsToolipLink &= "</span>"

                            lstable.Append("<td width='20'>")
                            lstable.Append(lsToolipLink)
                            lstable.Append("</td>")
                        End If


                        Dim sbDesc As New StringBuilder()

                        For Each lobjField In lcolFields
                            Dim liDisplayMode As Arco.Doma.Library.Connectors.ExternalFieldDisplayMode = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow

                            If Not lobjField.SelectSingleNode("DI") Is Nothing Then
                                liDisplayMode = CType(Convert.ToInt32(lobjField.SelectSingleNode("DI").InnerText), Arco.Doma.Library.Connectors.ExternalFieldDisplayMode)
                            End If
                            Dim lsValue As String = fStr(lobjRS(fieldCounter))
                            Dim lsCaption As String = lsValue
                            If lsValue.IndexOf("||") > 0 Then
                                lsCaption = lsValue.Substring(lsValue.IndexOf("||") + 2)
                                lsValue = lsValue.Substring(0, lsValue.IndexOf("||"))
                            End If

                            If lobjField.SelectSingleNode("@SL").Value = "1" Then
                                _lastValue = lsValue
                            End If

                            If (liDisplayMode And Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.DetailWindow) = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.DetailWindow Then
                                If sbDesc.Length <> 0 Then
                                    sbDesc.Append(", ")
                                End If
                                sbDesc.Append(lsCaption)
                            End If

                            fieldCounter += 1

                        Next 'lobjfield

                        _lastDesc = sbDesc.ToString()

                        fieldCounter = 0

                        For Each lobjField In lcolFields
                            Dim liDisplayMode As Arco.Doma.Library.Connectors.ExternalFieldDisplayMode = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow

                            If Not lobjField.SelectSingleNode("DI") Is Nothing Then
                                liDisplayMode = CType(Convert.ToInt32(lobjField.SelectSingleNode("DI").InnerText), Arco.Doma.Library.Connectors.ExternalFieldDisplayMode)
                            End If
                            Dim lsValue As String = fStr(lobjRS(fieldCounter))
                            Dim lsCaption As String = lsValue
                            If lsValue.IndexOf("||") > 0 Then
                                lsCaption = lsValue.Substring(lsValue.IndexOf("||") + 2)
                                lsValue = lsValue.Substring(0, lsValue.IndexOf("||"))
                            End If


                            If (liDisplayMode And Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow) = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow Then
                                lstable.Append("<TD nowrap><a href='javascript:Select(")
                                lstable.Append(EncodingUtils.EncodeJsString(_lastValue))
                                lstable.Append(",")
                                lstable.Append(EncodingUtils.EncodeJsString(_lastDesc))
                                lstable.Append(");'>")


                                Dim encodingMode As Arco.Doma.Library.FieldFlag = Arco.Doma.Library.FieldFlag.Inherited
                                If Not lobjField.SelectSingleNode("ENC") Is Nothing Then
                                    encodingMode = CType(Convert.ToInt32(lobjField.SelectSingleNode("ENC").InnerText), Arco.Doma.Library.FieldFlag)
                                End If
                                Select Case encodingMode
                                    Case Arco.Doma.Library.FieldFlag.Inherited
                                        If loProp.EncodeHtml Then
                                            lsCaption = Server.HtmlEncode(lsCaption)
                                        End If
                                    Case Arco.Doma.Library.FieldFlag.True
                                        lsCaption = Server.HtmlEncode(lsCaption)
                                End Select

                                lstable.Append(lsCaption)

                                lstable.Append("</a></TD>")
                            End If

                            fieldCounter += 1

                        Next 'lobjfield

                        lstable.Append("</tr>")
                    End If

                End While


                If liteller3 = 1 Then '1 result found
                    mbAutoSelect = (Not Page.IsPostBack OrElse Not mbMultiSelect)

                End If

            End Using

        End Using
        ' Response.Write(liteller3)
        lstable.Append("<TR ><TD COLSPAN=")
        lstable.Append(liColCount + 1)
        lstable.Append(" ALIGN='CENTER'>")

        If liteller3 > 0 Then
            lstable.Append(GridScroller.GetGridScroller(Me, liPage, liRecsPerPage, liteller3))
        Else
            lstable.Append("<b>")
            lstable.Append(GetLabel("noresultsfound"))
            lstable.Append("</b>")

        End If
        lstable.Append("</TD></TR></table>")

        plhResults.Text = lstable.ToString

    End Sub

    Private _lastValue As String
    Private _lastDesc As String


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sLoad()
    End Sub
End Class
