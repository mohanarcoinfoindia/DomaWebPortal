Imports System.Xml
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Globalisation
Imports Arco.Doma.Library.Security
Imports Arco.Doma.Library.Routing
Imports Arco.Doma.Library.Website

Public Class UserControls_AdvDocroomListLookup
    Inherits BasePage
    Private mobjXML As XmlDocument
    Private _propId As Integer
    Protected mbRefreshOnChange As Boolean
    Protected mbMultiSelect As Boolean
    Protected Autovalidate As String
    Protected msFocusField As String
    Protected msOrder As String
    Protected liPage As Int32
    Protected mbAutoSelect As Boolean
    Protected EditAdvancedLookups As Boolean = True
    Protected mbShowResults As Boolean

    Protected CaseTechID As Integer
    Protected ObjID As Integer
    Protected RowID As Integer
    Private mbHasToolTip As Boolean
    Private moProp As DM_PROPERTY
    Private _lastValue As String
    Private _lastDesc As String

    Public Property ShowCreateItem As Boolean = True

    Private Sub sLoad()


        Form.DefaultButton = lnkEnter.UniqueID

        ShowCreateItem = QueryStringParser.GetBoolean("ci", True)
        _propId = QueryStringParser.GetInt("PROP_ID")

        msOrder = Request("ORDERBY")
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


        moProp = DM_PROPERTY.GetPROPERTY(_propId)
        Dim lcolLabels As LABELList
        If moProp.ProcID > 0 Then
            lcolLabels = LABELList.GetProcedureItemsLabelList(moProp.ProcID, Me.EnableIISCaching)
        Else
            lcolLabels = LABELList.GetCategoryItemsLabelList(moProp.CatID, Me.EnableIISCaching)
        End If

        Page.Title = lcolLabels.GetObjectLabel(moProp.ID, "Property", Arco.Security.BusinessIdentity.CurrentIdentity.Language, moProp.Name)

        mbRefreshOnChange = moProp.PROP_ROC
        mbMultiSelect = QueryStringParser.GetBoolean("multiselect", (moProp.VALUE_NUMBER = 1))
        Dim filter As MultiPartFilter = moProp.GetListItemFilters()

        For Each f As Arco.Doma.Library.Filter In filter.PropertyFilters
            If QueryStringParser.Exists("linked_prop_id_" + f.Property_Name.ToLower()) Then
                f.Value = QueryStringParser.GetString("linked_prop_id_" + f.Property_Name.ToLower())
            End If
        Next
        mbHasToolTip = moProp.GetListItemHeaders(Connectors.ExternalFieldDisplayMode.TooltipWindow, SiteVersion.Version7).Any
        Dim lsXML As String = ""
        If CaseTechID > 0 Then
            Dim loCase As cCase = cCase.GetCase(CaseTechID)
            lsXML = TagReplacer.ReplaceTags(moProp.Definition, BusinessIdentity.CurrentIdentity().LanguageCode, loCase, filter, False, True, moProp.PARENT_PROP_ID, RowID)
        Else
            Dim loObject As DM_OBJECT = ObjectRepository.GetObject(ObjID)
            lsXML = TagReplacer.ReplaceTags(moProp.Definition, Security.BusinessIdentity.CurrentIdentity().LanguageCode, loObject, filter, False, True, moProp.PARENT_PROP_ID, RowID)
        End If

        mobjXML = New XmlDocument
        mobjXML.LoadXml(lsXML)

        ShowSearchFields()
        If mbShowResults Then
            sShowTable()
        Else
            '  Response.Write("Not showing results")
        End If

        AddCreateNewScript()

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
    Private Sub AddCreateNewScript()
        Dim sb As New StringBuilder
        sb.Append("function CreateItem(){")
        sb.Append("PC().OpenDetailWindow('../DM_NEW_OBJECT.ASPX?DM_CAT_ID=" & moProp.ListID & "', 0);")
        sb.Append("}")
        Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "createnewscript", sb.ToString, True)
    End Sub
    Private Function fsFormatCaption(ByVal vsCaption As String) As String
        Return Arco.Web.ResourceManager.ReplaceLabeltags(vsCaption)
    End Function

    Private Sub ShowSearchFields()
        Dim lobjRoot As XmlElement


        lobjRoot = mobjXML.DocumentElement
        If Not Page.IsPostBack AndAlso Not mbShowResults Then
            If Not lobjRoot.SelectSingleNode("A") Is Nothing Then
                mbShowResults = (lobjRoot.SelectSingleNode("A").InnerText = "1")
            End If
        End If
        Dim moCat As OBJECT_CATEGORY = OBJECT_CATEGORY.GetOBJECT_CATEGORY(moProp.ListID)

        domasearch.CategoryID = moProp.ListID
        If moCat.Default_Search_Screen = 0 Then
            domasearch.datasource = Website.ScreenItemList.GetDefaultCategorySearchScreenItems(moCat)
        Else
            domasearch.datasource = Website.ScreenItemList.GetScreenItems(moCat.Default_Search_Screen)
        End If
        domasearch.Labels = LABELList.GetCategoryItemsLabelList(domasearch.CategoryID, EnableIISCaching)

    End Sub

    Protected Function JSIdentifier() As String
        Return _propId & "_" & RowID
    End Function

    Private Function fStr(ByVal s As String) As String
        If Not String.IsNullOrEmpty(s) Then
            Return s.Trim
        Else
            Return ""
        End If
    End Function


    Private Sub sShowTable()
        Dim lstable As New StringBuilder
        Dim lobjRoot As XmlElement
        Dim liColCount As Int32
        Dim liRecsPerPage As Int32
        Dim liFirstRec, liLastRec As Int32

        lstable.Append("<table class='List'>")

        lobjRoot = mobjXML.DocumentElement
        liRecsPerPage = CInt(lobjRoot.SelectSingleNode("R").InnerText)
        liFirstRec = ((liPage - 1) * liRecsPerPage) + 1
        liLastRec = liFirstRec + liRecsPerPage - 1

        Dim loDocroomListHandler As New PropertyTypes.DocroomListHandler
        Dim lcolFields As List(Of PropertyTypes.DocroomListHandler.ReturnField) = loDocroomListHandler.GetFields(moProp.Definition)


        liColCount = lcolFields.Count

        lstable.Append("<tr>")
        If mbHasToolTip Then
            lstable.Append("<th width='20'>&nbsp;</th>")
        End If
        lstable.Append("<th width='20'>&nbsp;</th>")
        For Each lobjField As PropertyTypes.DocroomListHandler.ReturnField In lcolFields
            If (lobjField.DisplayMode And Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow) = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow Then
                lstable.Append("<th>")
                lstable.Append(fsFormatCaption(lobjField.Caption))
                lstable.Append("</th>")
            End If
        Next 'lobfield

        lstable.Append("</tr>")

        Dim loSearchCrit As Search.DM_OBJECTSearch.Criteria = Search.DM_OBJECTSearch.Criteria.GetNewGridCriteria(Website.Screen.ScreenSearchMode.Objects, Folder.GetRoot, moProp.ListID)
        domasearch.ApplyToSearchCriteria(loSearchCrit, True)

        Dim llResultCount As Int32 = loDocroomListHandler.GetResultCount(loSearchCrit, moProp, mobjXML.InnerXml, "", Connectors.ExternalFieldDisplayMode.PopupWindow, SiteVersion.Version7)

        While llResultCount > 0 AndAlso liFirstRec > 1 AndAlso llResultCount < liFirstRec
            liFirstRec = liFirstRec - liRecsPerPage
            liLastRec = liLastRec - liRecsPerPage
            liPage = liPage - 1
        End While

        loSearchCrit.MAXRESULTS = Math.Min(liLastRec, llResultCount)
        loSearchCrit.FIRSTRECORD = liFirstRec
        loSearchCrit.LASTRECORD = liLastRec

        Dim laResults As List(Of PropertyListItem) = loDocroomListHandler.LoadListItems(loSearchCrit, moProp, mobjXML.InnerXml, "", "", Connectors.ExternalFieldDisplayMode.PopupWindow And Connectors.ExternalFieldDisplayMode.DetailWindow, SiteVersion.Version7)
        '    Dim llResultCount As Int32 = laResults.Count

        Dim liRank As Int32 = 0
        For Each loResult In laResults
            liRank = liRank + 1

            lstable.Append("<tr ID='")
            lstable.Append(liRank)
            lstable.Append("'>")


            If mbHasToolTip Then
                Dim lsTooltipUrl As String = "'LookupitemTooltip.aspx?PROP_ID=" & _propId & "&DM_OBJECT_ID=" & ObjID & "&CASE_TECH_ID=" & CaseTechID & "&PROP_VAL=" & loResult.Value & "'"
                Dim lsToolipLink As String = "<span onmouseover=""ajax_showTooltip(" & lsTooltipUrl & ",this,event);return false"" onmouseout=""ajax_hideTooltip()""><a href=""#"" onclick=""javascript:var w = window.open(" & lsTooltipUrl & ",'ExtraInfo','width=400,height=300,scrollbars=yes,resizable=yes');w.focus();return false;"">"
                lsToolipLink &= "<img src='../Images/info.gif' border='0' alt='Info' >"
                lsToolipLink &= "</span>"

                lstable.Append("<td width='20'>")
                lstable.Append(lsToolipLink)
                lstable.Append("</td>")
            End If

            _lastValue = loResult.Value

            lstable.Append("<td width='20'>")
            Dim detailImage As String = ThemedImage.GetClientUrl("details.png", Me)
            lstable.Append(String.Format("<a href=""javascript: PC().OpenDetailWindow('../dm_detail.aspx?DM_OBJECT_ID={0}&mode=1', {0});""><img src='{2}' title='{1}'/></a>", _lastValue, GetDecodedLabel("details"), detailImage))
            lstable.Append("</td>")

            _lastDesc = ""
            Dim iCaption As Int32 = 0
            For Each lobjField As PropertyTypes.DocroomListHandler.ReturnField In lcolFields

                If (lobjField.DisplayMode And Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.DetailWindow) = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.DetailWindow Then
                    If Not String.IsNullOrEmpty(_lastDesc) Then
                        _lastDesc &= ", "
                    End If
                    _lastDesc &= fStr(loResult.Caption(iCaption).Caption)
                End If
                iCaption += 1
            Next

            iCaption = 0

            For Each lobjField As PropertyTypes.DocroomListHandler.ReturnField In lcolFields
                If (lobjField.DisplayMode And Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow) = Arco.Doma.Library.Connectors.ExternalFieldDisplayMode.PopupWindow Then
                    lstable.Append("<td nowrap><a href='javascript:Select(")
                    lstable.Append(EncodingUtils.EncodeJsString(_lastValue))
                    lstable.Append(",")
                    lstable.Append(EncodingUtils.EncodeJsString(_lastDesc))
                    lstable.Append(");'>")

                    Dim lsCaption As String = fStr(loResult.Caption(iCaption).Caption)
                    Select Case lobjField.EncodeHtml
                        Case FieldFlag.Inherited
                            If moProp.EncodeHtml Then
                                lsCaption = Server.HtmlEncode(lsCaption)
                            End If
                        Case FieldFlag.True
                            lsCaption = Server.HtmlEncode(lsCaption)
                    End Select
                    lstable.Append(lsCaption)


                    lstable.Append("</a></td>")

                End If

                iCaption += 1
            Next

            lstable.Append("</tr>")
        Next


        If llResultCount = 1 Then '1 result found
            mbAutoSelect = (Not Page.IsPostBack OrElse Not mbMultiSelect)
        End If

        lstable.Append("<TR><TD COLSPAN=")
        lstable.Append(liColCount + 1)
        lstable.Append(" ALIGN='CENTER'>")

        If llResultCount > 0 Then
            lstable.Append(GridScroller.GetGridScroller(Me, liPage, liRecsPerPage, llResultCount))
        Else
            lstable.Append("<b>")
            lstable.Append(GetLabel("noresultsfound"))
            lstable.Append("</b>")
        End If
        lstable.Append("</TD></TR></table>")

        plhResults.Text = lstable.ToString

    End Sub



    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sLoad()
    End Sub

End Class
