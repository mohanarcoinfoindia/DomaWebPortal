Imports Arco.Doma.Library.Logging

Partial Class UserControls_ObjectHistory
    Inherits BaseUserControl


    Protected msRoutingLink As String = ""


    Public Property CurrentPage As Integer
    Public Property LastPage As Integer

    Public ReadOnly Property RecordsPerPage As Integer
        Get
            Return Convert.ToInt32(drpRecsPerPage.SelectedValue)
        End Get
    End Property


    Public Property NumberOfResults As Integer
    Public Property DIN As Integer
        Get
            Dim o As Object

            o = ViewState("OBJECT_DIN")
            If o Is Nothing Then
                Return 0
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal value As Integer)
            ViewState("OBJECT_DIN") = value
        End Set
    End Property
    Public Property SUB_OBJ_DIN As Integer
        Get
            Dim o As Object

            o = ViewState("SUB_OBJ_DIN")
            If o Is Nothing Then
                Return 0
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal value As Integer)
            ViewState("SUB_OBJ_DIN") = value
        End Set
    End Property
    Public Property CASE_ID As Integer
        Get
            Dim o As Object

            o = ViewState("CASE_ID")
            If o Is Nothing Then
                Return 0
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal value As Int32)
            ViewState("CASE_ID") = value
        End Set
    End Property
    Public Function ExtraLink(ByVal voItem As AUDITList.AUDITInfo) As String
        Select Case voItem.Action
            Case "CASE"
                Return "<a href='javascript:OpenCaseHistory(" & voItem.Sub_Object_ID & ");' class='ButtonLink'>" & ThemedImage.GetSpanIconTag("icon icon-details", GetLabel("details")) & "</a>"
            Case "FILEADD"
                Return "<a href='javascript:OpenFileHistory(" & voItem.Sub_Object_DIN & ");' class='ButtonLink'>" & ThemedImage.GetSpanIconTag("icon icon-details", GetLabel("details")) & "</a>"
                'Case "ADDTOPACKAGE"
                '    Return "&nbsp;"
            Case Else
                Return "&nbsp;"
        End Select
    End Function
    Protected Function FormatUserName(ByVal entry As AUDITList.AUDITInfo) As String
        If String.IsNullOrEmpty(entry.Impersonator) Then
            Return ArcoFormatting.FormatUserName(entry.User)
        Else
            Return ArcoFormatting.FormatUserName(entry.User) & " (" & ArcoFormatting.FormatUserName(entry.Impersonator) & ")"
        End If
    End Function
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        chkShowDetails.Text = GetLabel("details")
        If DIN <= 0 Then Return


        lstHistory.Visible = True

        msRoutingLink = "CaseHistory.aspx?CASE_ID="

        Dim lsPage As String = Request.Form("Page")
        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If
        CurrentPage = CInt(lsPage)

        Dim llFirstRec As Integer = ((CurrentPage - 1) * RecordsPerPage) + 1
        Dim llLastRec As Int32 = llFirstRec + RecordsPerPage - 1

        Dim loCrit As New AUDITList.Criteria(DIN, CASE_ID) With {
            .SUB_OBJ_DIN = SUB_OBJ_DIN,
            .FullLog = chkShowDetails.Checked
        }
        loCrit.Range.FirstItem = llFirstRec
        loCrit.Range.LastItem = llLastRec
        loCrit.Range.MaxResults = 0 'always full audit for a single object

        loCrit.orderby = GetActualOrderBy()

        Dim loPagedDataSource As New PagedDataSource With {
            .AllowPaging = True,
            .PageSize = RecordsPerPage,
            .DataSource = AUDITList.GetAUDITList(loCrit),
            .CurrentPageIndex = CurrentPage - 1
        }

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

        litScroller.Text = PageScroller.GetPageScroller(CurrentPage, loPagedDataSource.PageCount).Render(ClientID & "_Goto")


    End Sub

    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "ACTION_DATE", "OBJ_VERSION", "USER_ID", "MODULE"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function
End Class
