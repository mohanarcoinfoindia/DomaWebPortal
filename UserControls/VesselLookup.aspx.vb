
Partial Class UserControls_VesselLookup
    Inherits BasePage

    Protected PropID As Integer
    Protected mbRefreshOnChange As Boolean
    Protected mbMultiSelect As Boolean

    Protected msFocusField As String
    Protected msOrder As String
    Protected liPage As Int32
    Protected mbAutoSelect As Boolean = False
    Protected EditAdvancedLookups As Boolean = True
    Protected mbShowResults As Boolean
    Private mbIsIE As Boolean
    Protected CaseTechID As Int32 = 0
    Protected ObjID As Int32 = 0
    Private moProp As Arco.Doma.Library.baseObjects.DM_PROPERTY
    Protected RowID As Integer


    Private _lastValue As String
    Private _lastDesc As String

    Private Sub sLoad()

        mbIsIE = (InStr(UCase(Request.ServerVariables("HTTP_USER_AGENT")), "MSIE") > 0)
        Me.Form.DefaultButton = lnkEnter.UniqueID


        PropID = QueryStringParser.GetInt("PROP_ID")
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

        mbShowResults = True


        moProp = Arco.Doma.Library.baseObjects.DM_PROPERTY.GetPROPERTY(PropID)
        Dim lcolLabels As Arco.Doma.Library.Globalisation.LABELList
        If moProp.ProcID > 0 Then
            lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetProcedureItemsLabelList(moProp.ProcID, Me.EnableIISCaching)
        Else
            lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetCategoryItemsLabelList(moProp.CatID, Me.EnableIISCaching)
        End If

        Page.Title = lcolLabels.GetObjectLabel(moProp.ID, "Property", Arco.Security.BusinessIdentity.CurrentIdentity.Language, moProp.Name)

        mbRefreshOnChange = moProp.PROP_ROC
        mbMultiSelect = QueryStringParser.GetBoolean("multiselect", (moProp.VALUE_NUMBER = 1))

        sShowSearchFields()
        If mbShowResults Then
            sShowTable()
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
            If msFocusField <> "" AndAlso Not CType(Me.Master, ToolWindow).Modal Then
                Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "focus", "theForm." & msFocusField & ".focus();", True)
            End If
            Me.Page.ClientScript.RegisterStartupScript(Me.GetType, "yourselection", "UpdateYourSelection();", True)
        End If

    End Sub
    Protected Function JSIdentifier() As String
        Return PropID & "_" & RowID
    End Function
    Private Sub sUnLoad()
        ' mobjXML = Nothing
    End Sub

    Private Function fsFormatCaption(ByVal vsCaption As String) As String
        Return Arco.Web.ResourceManager.ReplaceLabeltags(vsCaption)
    End Function
    Private Sub sShowSearchFields()
        mbShowResults = True
        Dim lsTable As String = ""

     
        lsTable &= "<TR><TD><TABLE Class='DetailTable'><TR><TD nowrap valign='top'><span class='Label'>" & GetLabel("yourselection") & " : </span></TD><TD width='100%' valign='top'><span id='lblSelection' name='lblSelection'></span></TD><TD valign='top'>"
        If mbMultiSelect Then
            lsTable &= "</TD></TR><TR></TD><TD nowrap valign='top' colspan='3' align='right'><a href='javascript:Clear();' class='ButtonLink'>" & GetLabel("clear") & "</a>&nbsp;<a HREF='JavaScript:Close();' class='ButtonLink'>" & GetLabel("close") & "</a>"
        End If
        lsTable &= "</TD></TR></TABLE></TD></TR>"


        plhSearch.Text = lsTable


        mbShowResults = True


    End Sub



    Private Function fStr(ByVal s As String) As String
        If Not String.IsNullOrEmpty(s) Then
            Return s.Trim
        Else
            Return ""
        End If
    End Function

  

    Private Sub sShowTable()
        Dim lstable As String

        Dim liRecsPerPage As Int32
        Dim liteller3, liFirstRec, liLastRec As Int32

        liRecsPerPage = 20 'todo
        liFirstRec = ((liPage - 1) * liRecsPerPage) + 1
        liLastRec = liFirstRec + liRecsPerPage - 1

        Dim range As Arco.Doma.Library.ListRangeRequest = Arco.Doma.Library.ListRangeRequest.Range(liFirstRec, liLastRec)
        Dim laResults As Collections.Generic.List(Of Arco.Doma.Library.PropertyListItem)
        Dim liColCount As Integer = 0

        If CaseTechID > 0 Then
            Dim loCase As Arco.Doma.Library.Routing.cCase = Arco.Doma.Library.Routing.cCase.GetCase(CaseTechID)
            laResults = moProp.GetListItems(loCase, range, Nothing, 0, True, 7)
        Else
            Dim loObject As Arco.Doma.Library.baseObjects.DM_OBJECT = Arco.Doma.Library.ObjectRepository.GetObject(ObjID)
            laResults = moProp.GetListItems(loObject, range, Nothing, 0, True, 7)
        End If


        lstable = "<table class='List'>"

        lstable = lstable & "<tr>"
        For Each loRes As Arco.Doma.Library.PropertyListItem In laResults
            For Each p As Arco.Doma.Library.CaptionPart In loRes.Caption
                lstable = lstable & "<th>" & fsFormatCaption(p.Header) & "</th>"
                liColCount += 1
            Next
            Exit For

        Next 'lobfield
        lstable = lstable & "</tr>"


        liteller3 = 0

        For Each loResult As Arco.Doma.Library.PropertyListItem In laResults
            liteller3 = liteller3 + 1
           
            lstable = lstable & "<tr ID='" & liteller3 & "'>"
            _lastValue = loResult.Value
            _lastDesc = ""
            For Each p As Arco.Doma.Library.CaptionPart In loResult.Caption

                If _lastDesc <> "" Then
                    _lastDesc = _lastDesc & ", "
                End If
                _lastDesc = _lastDesc & p.Caption
            Next

            For Each p As Arco.Doma.Library.CaptionPart In loResult.Caption
                lstable &= "<td align='left'><a href='javascript:Select(" & EncodingUtils.EncodeJsString(_lastValue) & "," & EncodingUtils.EncodeJsString(_lastDesc) & ");'>"

                Dim lsCaption As String = Str(p.Caption)
                Select Case p.EncodeHtml
                    Case Arco.Doma.Library.FieldFlag.Inherited
                        If moProp.EncodeHtml Then
                            lsCaption = Server.HtmlEncode(lsCaption)
                        End If
                    Case Arco.Doma.Library.FieldFlag.True
                        lsCaption = Server.HtmlEncode(lsCaption)
                End Select

                lstable &= lsCaption

                lstable &= "</a></td>"

            Next

            lstable = lstable & "</tr>"
            ' End If

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
        lstable &= "</TD></TR>"


        lstable = lstable & "</table>"

        plhResults.Text = lstable

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        sLoad()
    End Sub

End Class
