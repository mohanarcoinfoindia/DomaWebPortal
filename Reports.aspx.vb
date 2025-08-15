Imports Arco.Doma.Library
Partial Class Reports
    Inherits BaseAdminOnlyPage

    Public Property CurrentPage() As Int16

    Public Property LastPage() As Int16

    Public Property RecordsPerPage() As Int16

    Public Property NumberOfResults() As Int32

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        RecordsPerPage = ArcoInfoSettings.DefaultRecordsPerPage

        Dim lsPage As String
        Dim llFirstRec As Int32 = 0
        Dim llLastRec As Int32 = 0

        lsPage = Request.Form("Page")
        If lsPage = "" Then
            lsPage = "1"
        End If

        llFirstRec = ((CInt(lsPage) - 1) * Me.RecordsPerPage) + 1
        llLastRec = llFirstRec + Me.RecordsPerPage - 1


        Dim loCrit As New Statistics.ReportList.Criteria()
        loCrit.Range = ListRangeRequest.Range(llFirstRec, llLastRec)
        loCrit.OrderBy = GetActualOrderBy()

        Dim loPagedDataSource As PagedDataSource
        Dim lcolReports As Statistics.ReportList = Statistics.ReportList.GetReports(loCrit)

        If lcolReports.Any Then
            loPagedDataSource = New PagedDataSource
            loPagedDataSource.AllowPaging = True
            loPagedDataSource.PageSize = RecordsPerPage
            loPagedDataSource.DataSource = lcolReports
            loPagedDataSource.CurrentPageIndex = CInt(lsPage) - 1

            NumberOfResults = loPagedDataSource.DataSourceCount
            CurrentPage = CInt(lsPage)
            LastPage = loPagedDataSource.PageCount
            lstQueries.DataSource = loPagedDataSource
            lstQueries.DataBind()


            lnkPrev.Text = GetLabel("previous")
            lnkPrev.NavigateUrl = "javascript:Goto(" & CurrentPage - 1 & ");"
            lnkPrev.Enabled = Not loPagedDataSource.IsFirstPage

            lnkNext.Text = GetLabel("next")
            lnkNext.NavigateUrl = "javascript:Goto(" & CurrentPage + 1 & ");"
            lnkNext.Enabled = Not loPagedDataSource.IsLastPage

            litScroller.Text = PageScroller.GetPageScroller(CurrentPage, LastPage).Render("Goto") ' GetLabel("page") & " " & CurrentPage & " / " & LastPage
        End If
    End Sub

    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "REPORT_NAME"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        'add init script

        Dim sb As New StringBuilder


        sb.AppendLine(" function InitPage(){")
        sb.AppendLine(" if (PC().InitDone()){")
        sb.AppendLine(" if (parent)")
        sb.AppendLine("	    {")


        sb.AppendLine(" if (parent.SetCaption){")
        sb.AppendLine("	    parent.SetCaption(" & EncodingUtils.EncodeJsString("Reports") & ");}")

        sb.AppendLine(" if (parent.SetFolder){")
        sb.AppendLine("	    parent.SetFolder(0,0);}")


        'sb.AppendLine("	    parent.SetHeader('" & HeaderText & "');")
        sb.AppendLine("	    }")
        sb.AppendLine("//	}")
        sb.AppendLine("//	catch(exc)")
        sb.AppendLine("//	{console.log('err');}")


        sb.AppendLine("} else {")
        sb.AppendLine("setTimeout('InitPage()',100) ;}")
        sb.AppendLine("}")


        sb.AppendLine("setTimeout('InitPage()',100) ;")

        ScriptManager.RegisterStartupScript(Page, Me.Page.GetType, "init" & Me.ClientID, sb.ToString, True)

    End Sub
End Class
