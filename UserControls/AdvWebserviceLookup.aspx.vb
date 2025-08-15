Imports System.Xml

Partial Class UserControls_AdvWebserviceLookup
    Inherits BasePage
    Private mobjXML As System.Xml.XmlDocument
    Protected PropID As Integer
    Protected mbRefreshOnChange As Boolean
    Protected mbMultiSelect As Boolean
    Protected Autovalidate As String
    Protected msFocusField As String
    Protected msOrder As String
    Protected liPage As Int32
    Protected mbAutoSelect As Boolean = False
    Protected EditAdvancedLookups As Boolean = True
    Protected mbShowResults As Boolean
    Private mbIsIE As Boolean
    Protected CaseTechID As Int32
    Protected ObjID As Int32
    Protected Modal As Boolean = False
    Protected RowID As Integer
    Private mbHasToolTip As Boolean = False

    Private _lastValue As String
    Private _lastDesc As String

    Protected Function JSIdentifier() As String
        Return PropID & "_" & RowID
    End Function
    Private Sub sLoad()

        mbIsIE = (InStr(UCase(Request.ServerVariables("HTTP_USER_AGENT")), "MSIE") > 0)
        Me.Form.DefaultButton = lnkEnter.UniqueID

        PropID = QueryStringParser.GetInt("PROP_ID")
        msOrder = Request("ORDERBY")
        Modal = QueryStringParser.GetBoolean("modal")
        RowID = QueryStringParser.GetInt("ROW_ID")

        liPage = 1
        If Not Request("PAGE") Is Nothing Then
            If Request("PAGE") <> "" Then
                liPage = CType(Request("PAGE"), Int32)
            End If
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


        Dim loProp As Arco.Doma.Library.baseObjects.DM_PROPERTY = Arco.Doma.Library.baseObjects.DM_PROPERTY.GetPROPERTY(PropID)
        Dim lcolLabels As Arco.Doma.Library.Globalisation.LABELList
        If loProp.ProcID > 0 Then
            lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetProcedureItemsLabelList(loProp.ProcID, Me.EnableIISCaching)
        Else
            lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetCategoryItemsLabelList(loProp.CatID, Me.EnableIISCaching)
        End If

        Page.Title = lcolLabels.GetObjectLabel(loProp.ID, "Property", Arco.Security.BusinessIdentity.CurrentIdentity.Language, loProp.Name)

        mbRefreshOnChange = loProp.PROP_ROC
        mbMultiSelect = QueryStringParser.GetBoolean("multiselect", (loProp.VALUE_NUMBER = 1))

        Dim filters As Arco.Doma.Library.MultiPartFilter = loProp.GetListItemFilters()

        For Each f As Arco.Doma.Library.Filter In filters.PropertyFilters
            If QueryStringParser.Exists("linked_prop_id_" + f.Property_Name.ToLower()) Then
                f.Value = QueryStringParser.GetString("linked_prop_id_" + f.Property_Name.ToLower())
            End If
        Next
        mbHasToolTip = loProp.GetListItemHeaders(Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.TooltipWindow, 7).Any
        Dim lsXML As String = ""
        If CaseTechID > 0 Then
            Dim loCase As Arco.Doma.Library.Routing.cCase = Arco.Doma.Library.Routing.cCase.GetCase(CaseTechID)
            lsXML = Arco.Doma.Library.TagReplacer.ReplaceTags(loProp.Definition, Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity().LanguageCode, loCase, filters, False, True, loProp.PARENT_PROP_ID, RowID)
        Else
            Dim loObject As Arco.Doma.Library.baseObjects.DM_OBJECT = Arco.Doma.Library.ObjectRepository.GetObject(ObjID)
            lsXML = Arco.Doma.Library.TagReplacer.ReplaceTags(loProp.Definition, Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity().LanguageCode, loObject, filters, False, True, loProp.PARENT_PROP_ID, RowID)
        End If

        mobjXML = New XmlDocument
        mobjXML.LoadXml(lsXML)

        ' sShowSearchFields()
        '  If mbShowResults Then
        sShowTable(loProp)
        ' End If


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

    Private Sub sUnLoad()
        mobjXML = Nothing
    End Sub

    Private Function fsFormatCaption(ByVal vsCaption As String) As String
        Return Arco.Web.ResourceManager.ReplaceLabeltags(vsCaption)
    End Function



    Private Function fStr(ByVal s As String) As String
        If Not String.IsNullOrEmpty(s) Then
            Return s.Trim
        Else
            Return ""
        End If
    End Function


    Private Function ReplaceTags(ByVal vsPropDef As String) As String
        If vsPropDef.Contains("#AUTOCOMPLETE#") Then
            vsPropDef = vsPropDef.Replace("#AUTOCOMPLETE#", "")
        End If
        If vsPropDef.Contains("#VALUEFILTER#") Then
            vsPropDef = vsPropDef.Replace("#VALUEFILTER#", "")
        End If
        If vsPropDef.Contains("#FIRSTRECORD#") Then
            vsPropDef = vsPropDef.Replace("#FIRSTRECORD#", "0")
        End If
        If vsPropDef.Contains("#LASTRECORD#") Then
            vsPropDef = vsPropDef.Replace("#LASTRECORD#", "0")            
        End If
        Return vsPropDef
    End Function
    Private Sub sShowTable(ByVal loProp As Arco.Doma.Library.baseObjects.DM_PROPERTY)
        Dim lstable As String
        Dim lobjRoot As XmlElement
        Dim lcolFieldsNode As XmlNode
        Dim lcolFields As XmlNodeList
        Dim lobjField As XmlNode
        Dim liColCount As Int32
        Dim liRecsPerPage As Int32
        Dim liteller3, liFirstRec, liLastRec As Int32

        lstable = "<table class='List'>"

        lobjRoot = mobjXML.DocumentElement
        liRecsPerPage = CInt(lobjRoot.SelectSingleNode("R").InnerText)
        liFirstRec = ((liPage - 1) * liRecsPerPage) + 1
        liLastRec = liFirstRec + liRecsPerPage - 1


        lcolFieldsNode = lobjRoot.SelectSingleNode("FS")
        lcolFields = lcolFieldsNode.SelectNodes("F")
        liColCount = lcolFields.Count


        lstable = lstable & "<tr>"
        If mbHasToolTip Then
            lstable &= "<th width='20'>&nbsp;</th>"
        End If
        For Each lobjField In lcolFields
            Dim liDisplayMode As Arco.Doma.Library.Connectors.ExternalFieldDisplayMode = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow

            If Not lobjField.SelectSingleNode("DI") Is Nothing Then
                liDisplayMode = CType(Convert.ToInt32(lobjField.SelectSingleNode("DI").InnerText), Arco.Doma.Library.Connectors.ExternalFieldDisplayMode)
            End If
            If (liDisplayMode And Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow) = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow Then
                lstable = lstable & "<th>" & fsFormatCaption(lobjField.SelectSingleNode("C").InnerText) & "</th>"
            End If
        Next 'lobfield


        lstable = lstable & "</tr>"

        '  fsFillInSearchValues()


        Dim loWS As New Arco.Doma.Library.Connectors.WebServiceLookup
        Dim laResults As List(Of Arco.Doma.Library.Connectors.ResultListItem) = loWS.GetResults(ReplaceTags(mobjXML.InnerXml))

        liteller3 = 0

        For Each loResult As Arco.Doma.Library.Connectors.ResultListItem In laResults
            liteller3 = liteller3 + 1
            If liteller3 >= liFirstRec AndAlso liteller3 <= liLastRec Then

                lstable = lstable & "<tr ID='" & liteller3 & "'>"

                Dim loSubItem As Arco.Doma.Library.Connectors.ResultListSubItem

                If mbHasToolTip Then
                    Dim value As String = ""
                    For Each loSubItem In loResult.SubItems
                        If loSubItem.SelectionField Then
                            value = loSubItem.Value
                            Exit For
                        End If
                    Next
                    Dim lsTooltipUrl As String = "'LookupitemTooltip.aspx?PROP_ID=" & PropID & "&DM_OBJECT_ID=" & ObjID & "&CASE_TECH_ID=" & CaseTechID & "&PROP_VAL=" & Server.UrlEncode(value) & "'"
                    Dim lsToolipLink As String = "<span onmouseover=""ajax_showTooltip(" & lsTooltipUrl & ",this,event);return false"" onmouseout=""ajax_hideTooltip()""><a href=""#"" onclick=""javascript:var w = window.open(" & lsTooltipUrl & ",'ExtraInfo','width=400,height=300,scrollbars=yes,resizable=yes');w.focus();return false;"">"
                    lsToolipLink &= "<img src='../Images/info.gif' border='0' alt='Info' >"
                    lsToolipLink &= "</span>"

                    lstable &= "<td width='20'>" & lsToolipLink & "</td>"
                End If

                _lastValue = ""
                _lastDesc = ""
                For Each loSubItem In loResult.SubItems

                    If loSubItem.SelectionField Then
                        _lastValue = loSubItem.Value
                    End If
                    If (CType(loSubItem.DisplayMode, Arco.Doma.Library.Connectors.ExternalFieldDisplayMode) And Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.DetailWindow) = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.DetailWindow Then
                        If Not String.IsNullOrEmpty(_lastDesc) Then
                            _lastDesc &= ", "
                        End If
                        _lastDesc &= loSubItem.Value
                    End If


                Next

                For Each loSubItem In loResult.SubItems

                    If (CType(loSubItem.DisplayMode, Arco.Doma.Library.Connectors.ExternalFieldDisplayMode) And Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow) = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow Then
                        lstable &= "<td align='left'><a href='javascript:Select(" & EncodingUtils.EncodeJsString(_lastValue) & "," & EncodingUtils.EncodeJsString(_lastDesc) & ");'>"

                        Dim lsCaption As String = fStr(loSubItem.Value)
                        Select Case loSubItem.EncodeHtml
                            Case Arco.Doma.Library.FieldFlag.Inherited
                                If loProp.EncodeHtml Then
                                    lsCaption = Server.HtmlEncode(lsCaption)
                                End If
                            Case Arco.Doma.Library.FieldFlag.True
                                lsCaption = Server.HtmlEncode(lsCaption)
                        End Select

                        lstable &= lsCaption

                        lstable &= "</a></td>"
                    End If


                Next

                lstable = lstable & "</tr>"
            End If

        Next


        If liteller3 = 1 Then '1 result found
            mbAutoSelect = (Not Page.IsPostBack OrElse Not mbMultiSelect)
        End If


        lstable &= ("<TR><TD COLSPAN=")
        lstable &= (liColCount + 1)
        lstable &= (" ALIGN='CENTER'>")

        If liteller3 > 0 Then
            lstable &= (GridScroller.GetGridScroller(Me, liPage, liRecsPerPage, liteller3))

        Else
            lstable &= ("<b>")
            lstable &= (GetLabel("noresultsfound"))
            lstable &= ("</b>")
        End If
        lstable &= "</table>"

        plhResults.Text = lstable

    End Sub



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sLoad()
    End Sub
End Class
