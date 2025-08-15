
Imports Arco.Doma.Library
Imports Arco.Doma.Library.Routing

Partial Class UserControls_CaseHistory
    Inherits BaseUserControl


    Private _maxResults As Integer

    Public ReadOnly Property NumberOfResultsLabel As String
        Get
            Dim l As Int32 = NumberOfResults
            If l >= _maxResults Then
                Return " > " & l.ToString
            Else
                Return l.ToString
            End If
        End Get
    End Property
    Public Property CurrentPage As Integer
    Public Property LastPage As Integer
    Public ReadOnly Property RecordsPerPage As Integer
        Get
            Return Convert.ToInt32(drpRecsPerPage.SelectedValue)
        End Get
    End Property

    Public Property NumberOfResults As Integer
    Public Property USER_ID As String
        Get
            Dim o As Object

            o = ViewState("USER_ID")
            If o Is Nothing Then
                Return ""
            Else
                Return o.ToString
            End If
        End Get
        Set(ByVal value As String)
            ViewState("USER_ID") = value
        End Set
    End Property
    Public Property CASE_ID As Int32
        Get
            Dim o As Object

            o = ViewState("CASE_ID")
            If o Is Nothing Then
                Return 0
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal value As Integer)
            ViewState("CASE_ID") = value
        End Set
    End Property
    Public Property OBJ_DIN As Integer
        Get
            Dim o As Object

            o = ViewState("OBJ_DIN")
            If o Is Nothing Then
                Return 0
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal value As Int32)
            ViewState("OBJ_DIN") = value
        End Set
    End Property
    Public ReadOnly Property ForUser As Boolean
        Get
            Return Not String.IsNullOrEmpty(USER_ID)
        End Get
    End Property

    Protected Function FormatUserName(ByVal entry As HistoryActionList.HistoryActionInfo) As String
        If String.IsNullOrEmpty(entry.Impersonator) Then
            Return ArcoFormatting.FormatUserName(entry.User)
        End If
        Return ArcoFormatting.FormatUserName(entry.User) & " (" & ArcoFormatting.FormatUserName(entry.Impersonator) & ")"
    End Function
    Protected Function GetIcon(ByVal caseID As Integer) As String
        Dim lsTooltipLink As String = "'DM_CASE_TOOLTIP.aspx?CASE_ID=" & caseID & "'"
        Dim lsRes As String = "<span onmouseover=""ajax_showTooltip(" & lsTooltipLink & ",this,event);return false"" onmouseout=""ajax_hideTooltip()""><a href=""#"" onclick=""javascript:var w = window.open(" & lsTooltipLink & ",'ExtraInfo','width=400,height=300,scrollbars=yes,resizable=yes');w.focus();return false;"">"

        lsRes &= "<img src='./Images/Case.png' border='0'>"
        lsRes &= "</span>"

        Return lsRes
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Page.Form.DefaultButton = lnkReload.UniqueID

        chkShowDetails.Text = GetLabel("details")

        If CASE_ID <= 0 AndAlso String.IsNullOrEmpty(USER_ID) Then Return


        lstHistory.Visible = True


        _maxResults = ArcoInfoSettings.MaxResults


        Dim lsPage As String = Request.Form("Page")
        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If
        CurrentPage = CInt(lsPage)

        Dim llFirstRec As Int32 = ((CurrentPage - 1) * RecordsPerPage) + 1
        Dim llLastRec As Int32 = llFirstRec + RecordsPerPage - 1

        Dim loCrit As New HistoryActionList.Criteria(CASE_ID, OBJ_DIN)
        loCrit.FullLog = chkShowDetails.Checked
        loCrit.Range = ListRangeRequest.Range(llFirstRec, llLastRec)

        loCrit.User_ID = USER_ID
        loCrit.DateFormat = ArcoFormatting.DateInputFormat
        If Not String.IsNullOrEmpty(Request.Form("txtDateFilter")) Then
            loCrit.ActionDate = Request.Form("txtDateFilter")
        End If

        If ForUser Then
            loCrit.Range.MaxResults = _maxResults
        End If
        If String.IsNullOrEmpty(orderby.Value) AndAlso Not String.IsNullOrEmpty(USER_ID) Then
            orderby.Value = "DATE_OF_ACTION_END"
            orderbyorder.Value = "DESC"
        End If

        loCrit.OrderBy = GetActualOrderBy()

        Dim loPagedDataSource As New PagedDataSource
        loPagedDataSource.AllowPaging = True
        loPagedDataSource.PageSize = RecordsPerPage
        loPagedDataSource.DataSource = HistoryActionList.GetHistoryActionList(loCrit)
        loPagedDataSource.CurrentPageIndex = CurrentPage - 1

        NumberOfResults = loPagedDataSource.DataSourceCount

        LastPage = loPagedDataSource.PageCount
        lstHistory.DataSource = loPagedDataSource
        lstHistory.DataBind()


        lnkPrev.Text = GetLabel("previous")
        lnkPrev.NavigateUrl = "javascript:" & ClientID & "_Goto(" & CurrentPage - 1 & ");"


        lnkNext.Text = GetLabel("next")
        lnkNext.NavigateUrl = "javascript:" & ClientID & "_Goto(" & CurrentPage + 1 & ");"

        lnkPrev.Enabled = Not loPagedDataSource.IsFirstPage
        lnkNext.Enabled = Not loPagedDataSource.IsLastPage

        litScroller.Text = PageScroller.GetPageScroller(CInt(lsPage), loPagedDataSource.PageCount).Render(ClientID & "_Goto")

    End Sub

    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "DATE_OF_ACTION_END", "DATE_OF_ACTION_START", "USER_ID"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function
End Class
