Imports Arco.Doma.Library
Imports Arco.Doma.CMS
Imports Arco.Doma.CMS.Data

Partial Class Pages
    Inherits BaseAdminOnlyPage

    Public Property LastPage() As Int16

    Public Property RecordsPerPage() As Int16

    Public Property NumberOfResults() As Int32

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If delpage.Value <> "" Then
            Dim deleteID As Integer = Convert.ToInt32(delpage.Value)
            If deleteID > 0 Then
                If confirmDelete.Value = "True" OrElse Not ObjectPageList.PageIsLinkedToObject(deleteID) Then
                    Arco.Doma.CMS.Data.Page.DeletePage(deleteID)
                    delpage.Value = ""
                    confirmDelete.Value = ""
                Else
                    Dim message = GetDecodedLabel("pagelinkedobject")
                    ScriptManager.RegisterStartupScript(Page, Me.Page.GetType, "confirmDelete" & Me.ClientID, "ConfirmDelete('" + message + "');", True)
                End If
            End If
        End If
        If copypage.Value <> "" Then
            Dim copyId As Integer = Convert.ToInt32(copypage.Value)
            If copyId <> 0 Then
                Dim sourcePage As Arco.Doma.CMS.Data.Page = Arco.Doma.CMS.Data.Page.GetPage(copyId)
                sourcePage.Copy()
                copypage.Value = ""
            End If
        End If
        RecordsPerPage = ArcoInfoSettings.DefaultRecordsPerPage


        Dim llFirstRec As Int32 = 0
        Dim llLastRec As Int32 = 0

        If Not IsPostBack Then
            CurrentPage.Value = QueryStringParser.GetInt("paging", 1)
        End If

        Dim currentPageInt As Integer

        If Not Integer.TryParse(CurrentPage.Value, currentPageInt) Then
            CurrentPage.Value = "1"
            currentPageInt = 1
        End If

        llFirstRec = ((currentPageInt - 1) * Me.RecordsPerPage) + 1
        llLastRec = llFirstRec + Me.RecordsPerPage - 1


        Dim crit As New PageList.Criteria()
        crit.Range = ListRangeRequest.Range(llFirstRec, llLastRec)
        crit.OrderBy = GetActualOrderBy()

        Dim loPagedDataSource As PagedDataSource
        Dim lcolPages As PageList = PageList.GetPageList(crit)

        If lcolPages.Any Then
            loPagedDataSource = New PagedDataSource
            loPagedDataSource.AllowPaging = True
            loPagedDataSource.PageSize = RecordsPerPage
            loPagedDataSource.DataSource = lcolPages
            loPagedDataSource.CurrentPageIndex = currentPageInt - 1

            NumberOfResults = loPagedDataSource.DataSourceCount
            LastPage = loPagedDataSource.PageCount
            lstQueries.DataSource = loPagedDataSource
            lstQueries.DataBind()


            lnkPrev.Text = GetLabel("previous")
            lnkPrev.NavigateUrl = "javascript:Goto(" & currentPageInt - 1 & ");"
            lnkPrev.Enabled = Not loPagedDataSource.IsFirstPage

            lnkNext.Text = GetLabel("next")
            lnkNext.NavigateUrl = "javascript:Goto(" & currentPageInt + 1 & ");"
            lnkNext.Enabled = Not loPagedDataSource.IsLastPage

            litScroller.Text = PageScroller.GetPageScroller(currentPageInt, LastPage).Render("Goto") ' GetLabel("page") & " " & CurrentPage & " / " & LastPage
        End If

        lblContextId.Text = GetDecodedLabel("enterfolderid")
    End Sub
    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "NAME", "DESCRIPTION", "ID"
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
        sb.AppendLine("	    parent.SetCaption(" & EncodingUtils.EncodeJsString("Pages") & ");}")

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
