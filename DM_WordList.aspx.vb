Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Vocabulary
Imports Arco.Doma.Library.Website

Partial Class DM_WordList
    Inherits BasePage
    Private _CatID As Integer
    Private _procID As Integer
    Private _ParentID As Integer
    ' Private _IncludeSubFolders As Boolean
    Private _PropID As Integer = -1
    Private _ParentTextBox As String = ""
    Private _SearchValue As String = ""
    Private _PageIndex As Integer
    Dim objWords As New WordList()
    Private msCurrentPage As String = ""
    Private _IdxField As String = ""
    Private mbAddQuotes As Boolean
    Private mbMultiSelect As Boolean
    Private _numericPaging As Boolean
    Private meResType As Screen.ScreenSearchMode = Screen.ScreenSearchMode.Objects

    Private Const MaxResults As Int32 = 1000
    Private Const ResultsPerPage As Int32 = 100
    Protected Modal As Boolean

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        Page.Title = GetLabel("vocabulary")
        Me.Form.DefaultButton = butSearch.UniqueID
        Me.Form.DefaultFocus = txtSearch.UniqueID

        CType(Me.Master, ToolWindow).Width = 800

        Modal = QueryStringParser.GetBoolean("Modal")

        If Not Page.IsPostBack Then
            txtOrderBy.Value = WordList.eOrderBy.WordAscending
        End If

        ParseQuerystring()

        If _IdxField = "WORK" Then
            pnlSearch.Visible = False 'hack for now
        End If

        RegisterClientScript()

        AddPager()

        If Not Page.IsPostBack Then
            lnkClear.Text = GetLabel("clear")
            lblMaxResFound.Text = GetLabel("maxvocabresults")
            LoadResults("")
        End If

    End Sub


    Private Sub LoadResults(ByVal Filter As String)

        objWords.ResultType = meResType
        objWords.MaxResults = MaxResults
        objWords.OrderBy = CType(txtOrderBy.Value, WordList.eOrderBy)

        If (meResType = Screen.ScreenSearchMode.Objects OrElse meResType = Screen.ScreenSearchMode.Files) Then
            objWords.AddCategoryFilter(_CatID)
        End If

        If (meResType = Screen.ScreenSearchMode.Cases OrElse meResType = Screen.ScreenSearchMode.Work OrElse meResType = Screen.ScreenSearchMode.ArchivedCases OrElse meResType = Screen.ScreenSearchMode.MyCases OrElse meResType = Screen.ScreenSearchMode.OpenAndArchivedCases) Then
            objWords.AddProcedureFilter(_procID)
        End If

        If _PropID > 0 Then
            Dim loProp As DM_PROPERTY = DM_PROPERTY.GetPROPERTY(_PropID)
            If Not String.IsNullOrEmpty(Filter) Then
                objWords.AddFilter(New WordList.Filter(loProp, Filter))
            End If
            Dim lcolLabels As Globalisation.LABELList
            If loProp.ProcID > 0 Then
                lcolLabels = Globalisation.LABELList.GetProcedureItemsLabelList(loProp.ProcID, Me.EnableIISCaching)
            Else
                lcolLabels = Globalisation.LABELList.GetCategoryItemsLabelList(loProp.CatID, Me.EnableIISCaching)
            End If

            Page.Title = GetLabel("vocabulary") & " : " & lcolLabels.GetObjectLabel(loProp.ID, "Property", Arco.Security.BusinessIdentity.CurrentIdentity.Language, loProp.Name)

            objWords.LoadList(_ParentID, loProp)

        Else
            If Not String.IsNullOrEmpty(Filter) Then
                objWords.AddFilter(New WordList.Filter(_IdxField, Filter))
            End If
            objWords.LoadList(_ParentID, _IdxField)
            If _IdxField = "OBJ_UNAME" Then
                Page.Title = GetLabel("Vocabulary") & " : " & GetLabel("docTitle")
            End If
        End If

        lnkOrderTerm.Text = GetLabel("term")
        GridView1.EmptyDataText = "<center>" & GetLabel("noresultsfound") & "</center>"

        ExtractCaptions()

        GridView1.DataSource = ExtractCaptions()
        GridView1.DataBind()

        If objWords.Count >= MaxResults Then
            lblMaxResFound.Visible = True
        Else
            lblMaxResFound.Visible = False
        End If
    End Sub


    Private Class Item
        Public Property Key As String
        Public Property Caption As String
        Public Property Count As String
    End Class
    Private Function ExtractCaptions() As List(Of Item)
        Dim l As New List(Of Item)

        For Each key As String In objWords.Keys
            Dim lsCaption As String = objWords.Item(key)
            Dim lsCount As String = lsCaption.Substring(lsCaption.LastIndexOf("||") + 2)
            lsCaption = lsCaption.Substring(0, lsCaption.LastIndexOf("||"))
            Select Case objWords.Formatter
                Case WordList.eFormatter.BooleanFormatter
                    Select Case key
                        Case "1"
                            lsCaption = GetLabel("yes")
                        Case "0"
                            lsCaption = GetLabel("no")
                        Case Else
                            lsCaption = GetLabel("all") 'undefined
                    End Select
                Case WordList.eFormatter.DateFormatter
                    lsCaption = ArcoFormatting.FormatDate(lsCaption)
                Case WordList.eFormatter.UserFormatter
                    lsCaption = ArcoFormatting.FormatUserName(lsCaption, True, False) 'htmlencode is done later
            End Select
            l.Add(New Item() With {.Key = key, .Caption = lsCaption, .Count = lsCount})
        Next
        If objWords.Formatter <> WordList.eFormatter.NoFormatter Then
            Return l.OrderBy(Function(x) x.Caption).ToList
        Else
            Return l
        End If

    End Function

    Private Sub ParseQuerystring()

        _PropID = QueryStringParser.GetInt("propid")

        If WordList.IsFixedPropId(_PropID) Then
            _IdxField = WordList.GetFixedIndexField(_PropID)
            _PropID = 0
        End If

        _ParentTextBox = QueryStringParser.GetString("field")
        _SearchValue = QueryStringParser.GetString("search")

        _ParentID = QueryStringParser.GetInt("DM_PARENT_ID")
        _CatID = QueryStringParser.GetInt("CAT_ID")
        _procID = QueryStringParser.GetInt("PROC_ID")
        mbAddQuotes = QueryStringParser.GetBoolean("addquotes")
        mbMultiSelect = QueryStringParser.GetBoolean("multiselect")
        meResType = CType(QueryStringParser.GetInt("restype"), Screen.ScreenSearchMode)

        If _PropID > 0 Then
            Dim lsUrl As String = Arco.Settings.FrameWorkSettings.GetSetting("vocab_" & _PropID.ToString & "_url")
            If Not String.IsNullOrEmpty(lsUrl) Then
                If lsUrl.IndexOf("?") < 0 Then
                    lsUrl &= "?"
                Else
                    lsUrl &= "&"
                End If
                lsUrl &= "&field=" & _ParentTextBox & "&DM_PARENT_ID=" & _ParentID & "&INCLUDESUBFOLDERS=Y&CAT_ID=" & _CatID
                'Server.Transfer(lsUrl)
                Response.Redirect(lsUrl)
            End If
        End If

    End Sub

    Private Sub RegisterClientScript()

        If Not Page.ClientScript.IsClientScriptBlockRegistered("DM_WordList") Then

            Dim sb As New StringBuilder()

            sb.AppendLine("function ReturnSelection(value, e)")
            sb.AppendLine("{")
            sb.AppendLine(" var nav4 = window.Event ? true : false;")
            sb.AppendLine(" e = e||event;")
            sb.AppendLine(" var bModifPressed;")
            sb.AppendLine(" if (nav4)")
            sb.AppendLine("     bModifPressed = ((e.modifiers & Event.SHIFT_MASK) || (e.modifiers & Event.CONTROL_MASK));")
            sb.AppendLine(" else ")
            sb.AppendLine("     bModifPressed = (e.shiftKey || e.ctrlKey);")

            sb.AppendLine("     bModifPressed = true;")

            sb.AppendLine("     var orgVal = GetValue();")
            sb.AppendLine("     if(orgVal!='')")
            sb.AppendLine("     {")
            If Not mbMultiSelect Then
                If mbAddQuotes Then
                    sb.AppendLine("         orgVal= orgVal = orgVal + ' OR ' ;")
                Else
                    sb.AppendLine("         orgVal= orgVal = orgVal + ' ' ;")
                End If
            Else
                sb.AppendLine("         orgVal= orgVal = orgVal + '\n' ;")
            End If
            sb.AppendLine("     }")

            sb.AppendLine(" if (!bModifPressed){")
            If mbAddQuotes Then
                sb.AppendLine("     SetValue('=""'  + value + '""');")
            Else
                sb.AppendLine("     SetValue(value);")
            End If
            sb.AppendLine("     CloseMe();")
            sb.AppendLine(" } else {")
            If mbAddQuotes Then
                sb.AppendLine("     SetValue(orgVal + '=""'  + value + '""');")
            Else
                sb.AppendLine("     SetValue(orgVal + value);")
            End If
            sb.AppendLine("  }")

            sb.AppendLine(" if (bModifPressed){")
            sb.AppendLine("e.cancelBubble = true;")
            sb.AppendLine("e.returnValue = false;  ")

            sb.AppendLine("}")
            sb.AppendLine("}")

            sb.AppendLine(" function GetValue(){")
            sb.AppendLine(" return MainPage().GetContentWindow().document.getElementById(" & EncodingUtils.EncodeJsString(_ParentTextBox) & ").value;")
            sb.AppendLine("}")
            sb.AppendLine(" function CloseMe(){")
            sb.AppendLine("     Close();")
            sb.AppendLine("}")

            sb.AppendLine("function SetSelectionLabel(v){")
            sb.AppendLine(" var lblSelection = $get('lblSelection');")
            sb.AppendLine(" if (lblSelection) {")
            sb.AppendLine("lblSelection.innerHTML = '';")
            sb.AppendLine("lblSelection.appendChild(document.createTextNode(v));")
            sb.AppendLine("}}")

            sb.AppendLine(" function SetValue(v){")
            sb.AppendLine(" MainPage().GetContentWindow().document.getElementById(" & EncodingUtils.EncodeJsString(_ParentTextBox) & ").value = v;")
            sb.AppendLine(" SetSelectionLabel(v);")
            sb.AppendLine("}")

            sb.AppendLine(" function InitValue(){")
            sb.AppendLine(" SetSelectionLabel(GetValue());")
            sb.AppendLine("}")


            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "DM_WordList", sb.ToString, True)

        End If

    End Sub


    Dim lbFirstShown As Boolean
    Dim iItemsShown As Integer
    Dim lsLastLetter As Char

    Protected Sub GridView1_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.DataBinding

    End Sub

    Private Function GetPagerButton(ByVal id As String) As LinkButton
        Dim c As Control = plhPager.FindControl(id)

        If Not c Is Nothing Then
            Return DirectCast(c, LinkButton)
        Else
            Return Nothing
        End If
    End Function
    Protected Sub GridView1_DataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridView1.DataBound
        If _numericPaging Then
            Dim liRes As Int32 = GridView1.Rows.Count
            If String.IsNullOrEmpty(msCurrentPage) Then
                msCurrentPage = "1"
            End If
            Dim liCurrentPage As Int32 = CType(msCurrentPage, Int32)
            Dim liNextPage As Int32
            Dim liPrevPage As Int32
            Dim liLastPage As Int32

            liLastPage = ((liRes - 1) \ ResultsPerPage) + 1
            liPrevPage = liCurrentPage - 1
            liNextPage = liCurrentPage + 1

            If Not plhPager.FindControl("lblPage") Is Nothing Then
                CType(plhPager.FindControl("lblPage"), Label).Text = GetLabel("page") & " " & liCurrentPage & " / " & liLastPage
            End If
            If Not plhPagerTop.FindControl("lblPageTop") Is Nothing Then
                CType(plhPagerTop.FindControl("lblPageTop"), Label).Text = GetLabel("page") & " " & liCurrentPage & " / " & liLastPage
            End If

            Dim pgLast As LinkButton = GetPagerButton("pglast")
            If Not pgLast Is Nothing Then
                pgLast.CommandArgument = liLastPage
                If liLastPage > liCurrentPage Then
                    pgLast.Enabled = True
                Else
                    pgLast.Enabled = False
                End If
            End If
            Dim pgFirst As LinkButton = GetPagerButton("pgfirst")
            If Not pgFirst Is Nothing Then
                pgFirst.CommandArgument = "1"
                If liCurrentPage > 1 Then
                    pgFirst.Enabled = True
                Else
                    pgFirst.Enabled = False
                End If
            End If
            Dim pgPrev As LinkButton = GetPagerButton("pgprev")
            If Not pgPrev Is Nothing Then
                pgPrev.CommandArgument = liPrevPage.ToString
                If liCurrentPage > 1 Then
                    pgPrev.Enabled = True
                Else
                    pgPrev.Enabled = False
                End If
            End If
            Dim pgNext As LinkButton = GetPagerButton("pgnext")
            If Not pgNext Is Nothing Then
                pgNext.CommandArgument = liNextPage.ToString
                If liCurrentPage < liLastPage Then
                    pgNext.Enabled = True
                Else
                    pgNext.Enabled = False
                End If
            End If

            Dim pgLastTop As LinkButton = GetPagerButton("pglasttop")
            If Not pgLastTop Is Nothing Then
                pgLastTop.CommandArgument = liLastPage
                If liLastPage > liCurrentPage Then
                    pgLastTop.Enabled = True
                Else
                    pgLastTop.Enabled = False
                End If
            End If
            Dim pgFirstTop As LinkButton = GetPagerButton("pgfirsttop")
            If Not plhPagerTop.FindControl("pgfirsttop") Is Nothing Then
                pgFirstTop.CommandArgument = "1"
                If liCurrentPage > 1 Then
                    pgFirstTop.Enabled = True
                Else
                    pgFirstTop.Enabled = False
                End If
            End If
            Dim pgPrevTop As LinkButton = GetPagerButton("pgprevtop")
            If Not pgPrevTop Is Nothing Then
                pgPrevTop.CommandArgument = liPrevPage.ToString
                If liCurrentPage > 1 Then
                    pgPrevTop.Enabled = True
                Else
                    pgPrevTop.Enabled = False
                End If
            End If
            Dim pgNextTop As LinkButton = GetPagerButton("pgnexttop")
            If Not pgNextTop Is Nothing Then
                pgNextTop.CommandArgument = liNextPage.ToString
                If liCurrentPage < liLastPage Then
                    pgNextTop.Enabled = True
                Else
                    pgNextTop.Enabled = False
                End If
            End If
        End If
    End Sub
    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles GridView1.RowDataBound

        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim objLink As HyperLink = e.Row.Cells(0).FindControl("Hyperlink1")

            If Not objLink Is Nothing Then
                Dim lsPager As Char


                Dim loItem As Item = DirectCast(e.Row.DataItem, Item)
                If loItem.Caption.Length > 0 Then

                    lsPager = loItem.Caption(0)

                    If String.IsNullOrEmpty(msCurrentPage) Then
                        If Not _numericPaging Then
                            msCurrentPage = lsPager
                        Else
                            msCurrentPage = "1"
                        End If
                    End If

                    If Not _numericPaging Then
                        Dim pgControl As Control
                        If Not Char.IsLetter(lsPager) Then
                            'pgControl = plhPager.FindControl("pg*")
                            'If pgControl IsNot Nothing Then
                            '    pgControl.Visible = True
                            'End If
                            pgControl = plhPagerTop.FindControl("pgtop*")
                            If pgControl IsNot Nothing Then
                                pgControl.Visible = True
                            End If
                        Else
                            'pgControl = plhPager.FindControl("pg" & lsPager)
                            'If pgControl IsNot Nothing Then
                            '    pgControl.Visible = True
                            'End If
                            pgControl = plhPagerTop.FindControl("pgtop" & lsPager)
                            If pgControl IsNot Nothing Then
                                pgControl.Visible = True
                            End If

                        End If
                    End If

                    If Not _numericPaging Then
                        If (msCurrentPage = lsPager AndAlso Char.IsLetter(lsPager)) OrElse (Not Char.IsLetter(lsPager) AndAlso Not Char.IsLetter(msCurrentPage(0))) OrElse (lbFirstShown AndAlso iItemsShown < ResultsPerPage) Then

                            If lsLastLetter <> lsPager Then
                                Dim objHeader As Label = e.Row.Cells(0).FindControl("lblHeader")
                                objHeader.Text = "<b>" & lsPager & " ></b>"
                                lsLastLetter = lsPager
                            End If
                            e.Row.Visible = True
                            lbFirstShown = True
                            iItemsShown = iItemsShown + 1

                            objLink.NavigateUrl = "#"

                            objLink.Text = Server.HtmlEncode(loItem.Caption)

                            objLink.Attributes.Add("onclick", "javascript:ReturnSelection(" & EncodingUtils.EncodeJsString(loItem.Key) & ",event);")

                            Dim objCount As Label = e.Row.Cells(0).FindControl("lblCount")
                            objCount.Text = loItem.Count

                        Else
                            e.Row.Visible = False
                        End If
                    Else
                        iItemsShown = iItemsShown + 1

                        Dim llFirstRec As Int32 = ((CInt(msCurrentPage) - 1) * ResultsPerPage) + 1
                        Dim llLastRec As Int32 = llFirstRec + ResultsPerPage - 1

                        If (iItemsShown >= llFirstRec AndAlso iItemsShown <= llLastRec) Then
                            e.Row.Visible = True

                            objLink.NavigateUrl = "#"
                            objLink.Text = Server.HtmlEncode(loItem.Caption)
                            objLink.Attributes.Add("onclick", "javascript:ReturnSelection(" & EncodingUtils.EncodeJsString(loItem.Key) & ",event);")

                            Dim objCount As Label = e.Row.Cells(0).FindControl("lblCount")
                            objCount.Text = loItem.Count

                        Else

                            e.Row.Visible = False

                        End If
                    End If
                End If

            End If
        End If
    End Sub


    Private Sub AddPagerLink(ByVal vsPager As String)
        Dim lnk As LinkButton = New LinkButton
        lnk.ID = "pg" & vsPager
        lnk.Text = vsPager
        lnk.Visible = False
        lnk.CommandArgument = vsPager
        AddHandler lnk.Click, AddressOf onPagerClick 'have them all use the same event handler
        plhPager.Controls.Add(lnk)
        plhPager.Controls.Add(New LiteralControl(" ")) 'space them out

        Dim lnk2 As New LinkButton
        lnk2.ID = "pgtop" & vsPager
        lnk2.Text = vsPager
        lnk2.Visible = False
        lnk2.CommandArgument = vsPager
        AddHandler lnk2.Click, AddressOf onPagerClick 'have them all use the same event handler

        plhPagerTop.Controls.Add(lnk2)
        plhPagerTop.Controls.Add(New LiteralControl(" ")) 'space them out

    End Sub

    Private Sub AddNumericPagers()

        plhPager.Controls.Add(CreateNumericPager("pgfirst", GetLabel("first")))
        plhPager.Controls.Add(New LiteralControl(" ")) 'space them out

        plhPager.Controls.Add(CreateNumericPager("pgprev", GetLabel("previous")))
        plhPager.Controls.Add(New LiteralControl(" ")) 'space them out

        Dim lblPageCounter As Label = New Label
        lblPageCounter.ID = "lblPage"
        plhPager.Controls.Add(lblPageCounter)
        plhPager.Controls.Add(New LiteralControl(" ")) 'space them out
        plhPager.Controls.Add(CreateNumericPager("pgnext", GetLabel("next")))
        plhPager.Controls.Add(New LiteralControl(" ")) 'space them out

        plhPager.Controls.Add(CreateNumericPager("pglast", GetLabel("last")))
        plhPager.Controls.Add(New LiteralControl(" ")) 'space them out

        plhPagerTop.Controls.Add(CreateNumericPager("pgfirsttop", GetLabel("first")))
        plhPagerTop.Controls.Add(New LiteralControl(" ")) 'space them out

        plhPagerTop.Controls.Add(CreateNumericPager("pgprevtop", GetLabel("previous")))
        plhPagerTop.Controls.Add(New LiteralControl(" ")) 'space them out

        Dim lblPageCounterTop As Label = New Label
        lblPageCounterTop.ID = "lblPageTop"
        plhPagerTop.Controls.Add(lblPageCounterTop)
        plhPagerTop.Controls.Add(New LiteralControl(" ")) 'space them out

        plhPagerTop.Controls.Add(CreateNumericPager("pgnexttop", GetLabel("next")))
        plhPagerTop.Controls.Add(New LiteralControl(" ")) 'space them out

        plhPagerTop.Controls.Add(CreateNumericPager("pglasttop", GetLabel("last")))
        plhPagerTop.Controls.Add(New LiteralControl(" ")) 'space them out
    End Sub
    Private Function CreateNumericPager(ByVal id As String, ByVal label As String) As LinkButton
        Dim lnk As LinkButton = New LinkButton
        lnk.ID = id
        lnk.Text = label
        lnk.Visible = True
        lnk.CommandArgument = "1"
        AddHandler lnk.Click, AddressOf onNumericPagerClick 'have them all use the same event handler
        Return lnk
    End Function
    Protected Sub onNumericPagerClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim lnk As LinkButton = DirectCast(sender, LinkButton)
        msCurrentPage = lnk.CommandArgument
        'lsLastPager = ""
        LoadResults(txtSearch.Text)

    End Sub

    Protected Sub onPagerClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim lnk As LinkButton = DirectCast(sender, LinkButton)
        msCurrentPage = lnk.CommandArgument

        LoadResults(txtSearch.Text)
    End Sub
    Protected Sub onOrderTerm(ByVal sender As Object, ByVal e As EventArgs)
        msCurrentPage = ""
        If CType(txtOrderBy.Value, Arco.Doma.Library.Vocabulary.WordList.eOrderBy) = WordList.eOrderBy.WordAscending Then
            txtOrderBy.Value = WordList.eOrderBy.WordDescending
        Else
            txtOrderBy.Value = WordList.eOrderBy.WordAscending
        End If
        AddPager()

        LoadResults(txtSearch.Text)

    End Sub
    Protected Sub onOrdercount(ByVal sender As Object, ByVal e As EventArgs)
        msCurrentPage = "1"

        If CType(txtOrderBy.Value, Arco.Doma.Library.Vocabulary.WordList.eOrderBy) = WordList.eOrderBy.CountDescending Then
            txtOrderBy.Value = WordList.eOrderBy.CountAscending
        Else
            txtOrderBy.Value = WordList.eOrderBy.CountDescending
        End If
        AddPager()
        LoadResults(txtSearch.Text)

    End Sub
    Private Sub AddPager()
        If CType(txtOrderBy.Value, Arco.Doma.Library.Vocabulary.WordList.eOrderBy) = WordList.eOrderBy.WordAscending OrElse CType(txtOrderBy.Value, Arco.Doma.Library.Vocabulary.WordList.eOrderBy) = WordList.eOrderBy.WordDescending Then
            _numericPaging = False
        Else
            _numericPaging = True
        End If

        If Not _numericPaging Then
            plhPager.Visible = False
            plhPagerTop.Visible = True
            plhPager.Controls.Clear()
            plhPagerTop.Controls.Clear()

            AddPagerLink("*")
            For keycode As Integer = 65 To 90
                AddPagerLink(Chr(keycode))
            Next
        Else
            plhPager.Visible = True
            plhPagerTop.Visible = False

            plhPager.Controls.Clear()
            plhPagerTop.Controls.Clear()

            AddNumericPagers()
        End If
    End Sub


    Protected Sub butSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles butSearch.Click

        msCurrentPage = ""
        AddPager()
        LoadResults(txtSearch.Text)
    End Sub

End Class
