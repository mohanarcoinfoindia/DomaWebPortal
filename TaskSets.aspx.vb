Imports Arco.Doma.Library
Imports Arco.Doma.Library.Tasks

Partial Class TaskSets
    Inherits BaseAdminOnlyPage


    Public Property CurrentPage() As Int32

    Public Property LastPage() As Int32

    Public Property RecordsPerPage() As Int32

    Public Property NumberOfResults() As Int32

    Public Function GetCategory(ByVal taskSet As TaskSetList.TaskSetInfo) As String
        Return GetCategoryLabel(taskSet.Category)
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not String.IsNullOrEmpty(deltaskset.Value) Then
            Dim llID As Int32 = Convert.ToInt32(deltaskset.Value)
            Dim taskSet As Arco.Doma.Library.Tasks.TaskSet = Arco.Doma.Library.Tasks.TaskSet.GetTaskSet(llID)
            If taskSet IsNot Nothing Then

                Arco.Doma.Library.Tasks.TaskSet.DeleteTaskSet(llID)

            End If
            deltaskset.Value = ""
        End If

        RecordsPerPage = ArcoInfoSettings.DefaultRecordsPerPage

        Dim lsPage As String = Request.Form("Page")

        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If

        Dim llFirstRec As Int32 = ((CInt(lsPage) - 1) * RecordsPerPage) + 1
        Dim llLastRec As Int32 = llFirstRec + RecordsPerPage - 1

        Dim crit As New TaskSetList.Criteria(0) With {
            .Name = txtFilterName.Text,
               .Range = ListRangeRequest.Range(llFirstRec, llLastRec),
                 .OrderBy = GetActualOrderBy()
        }


        Dim tasksets As TaskSetList = TaskSetList.GetTaskSets(crit)

        If tasksets.Any Then
            Dim loPagedDataSource As New PagedDataSource With {
                .AllowPaging = True,
                .PageSize = RecordsPerPage,
                .DataSource = tasksets,
                .CurrentPageIndex = CInt(lsPage) - 1
            }

            NumberOfResults = loPagedDataSource.DataSourceCount
            CurrentPage = CInt(lsPage)
            LastPage = loPagedDataSource.PageCount
            lstTaskSets.DataSource = loPagedDataSource
            lstTaskSets.DataBind()


            lnkPrev.Text = GetLabel("previous")
            lnkPrev.NavigateUrl = "javascript:Goto(" & CurrentPage - 1 & ");"
            lnkPrev.Enabled = Not loPagedDataSource.IsFirstPage

            lnkNext.Text = GetLabel("next")
            lnkNext.NavigateUrl = "javascript:Goto(" & CurrentPage + 1 & ");"
            lnkNext.Enabled = Not loPagedDataSource.IsLastPage

            litScroller.Text = PageScroller.GetPageScroller(CurrentPage, LastPage).Render("Goto")
        End If
    End Sub
    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "SET_NAME"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreRender
        'add init script

        Dim sb As New StringBuilder


        sb.Append(" function InitPage(){ if (PC().InitDone()){if (parent){if (parent.SetCaption){parent.SetCaption(")
        sb.Append(EncodingUtils.EncodeJsString("tasksets"))
        sb.Append(");} if (parent.SetFolder){ parent.SetFolder(")
        sb.Append(PC.ParentID)
        sb.Append(",0);}}} else {setTimeout('InitPage()',100) ;}}setTimeout('InitPage()',100) ;")

        ScriptManager.RegisterStartupScript(Page, Page.GetType, "init" & ClientID, sb.ToString, True)

    End Sub
End Class
