Imports Arco.Doma.Library
Partial Class DM_UserBrowser_GroupsResult
    Inherits UserBrowserBaseUserControl


    Private mDefaultMaxResults As Int32

    Protected lsPage As String = ""
    Private mbInSelection As Boolean
#Region "tableProperties"
    Public ReadOnly Property NumberOfResultsLabel() As String
        Get
            Dim l As Int32 = Me.NumberOfResults
            If l >= mDefaultMaxResults Then
                Return " > " & l.ToString
            Else
                Return l.ToString
            End If
        End Get
    End Property
    Public Property CurrentPage() As Int16
        Get
            Dim o As Object

            o = Me.ViewState("_CurrentPage")
            If o Is Nothing Then
                Return 0
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal Value As Int16)
            Me.ViewState("_CurrentPage") = Value
        End Set
    End Property
    Public Property LastPage() As Int16
        Get
            Dim o As Object

            o = Me.ViewState("_LastPage")
            If o Is Nothing Then
                Return 0
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal Value As Int16)
            Me.ViewState("_LastPage") = Value
        End Set
    End Property
    Public Property RecordsPerPage() As Int32

    Public Property NumberOfResults() As Int32
        Get
            Dim o As Object

            o = Me.ViewState("_NumberOfResults")
            If o Is Nothing Then
                Return mDefaultMaxResults
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal Value As Int32)
            Me.ViewState("_NumberOfResults") = Value
        End Set
    End Property
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        mbInSelection = (Request.QueryString("mode") = "getresult")
        RecordsPerPage = Math.Max(ArcoInfoSettings.DefaultRecordsPerPage, 30)
        mDefaultMaxResults = ArcoInfoSettings.MaxResults

        Dim llFirstRec As Int32
        Dim llLastRec As Int32

        lsPage = Request.Form("Page")
        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If

        llFirstRec = GridScroller.FirstRecord(CInt(lsPage), Me.RecordsPerPage)
        llLastRec = GridScroller.LastRecord(CInt(lsPage), Me.RecordsPerPage)

        Dim lsGroupName As String
        Dim lsGroupDescription As String

        If Request("txtGroupName") = Nothing And Not IsPostBack Then
            lsGroupName = Request.QueryString("txtGName")
        Else
            lsGroupName = Request("txtGroupName")
        End If

        If Request("txtGroupDescription") = Nothing And Not IsPostBack Then
            lsGroupDescription = Request.QueryString("txtGDesc")
        Else
            lsGroupDescription = Request("txtGroupDescription")
        End If

        Dim loCrit As New ACL.GROUPList.Criteria()

        loCrit.Range = ListRangeRequest.Range(llFirstRec, llLastRec)

        loCrit.FILTER = lsGroupName
        loCrit.GROUP_DESC = lsGroupDescription

        loCrit.OrderBy = GetActualOrderBy()

        Dim luGrouplist As ACL.GROUPList = ACL.GROUPList.GetGROUPSList(loCrit)

        Dim loPagedDataSource As New PagedDataSource

        loPagedDataSource.AllowPaging = True
        loPagedDataSource.PageSize = Me.RecordsPerPage
        loPagedDataSource.DataSource = luGrouplist
        loPagedDataSource.CurrentPageIndex = CInt(lsPage) - 1

        Me.NumberOfResults = loPagedDataSource.DataSourceCount
        Me.CurrentPage = CInt(lsPage)
        Me.LastPage = loPagedDataSource.PageCount
        lstHistory.DataSource = loPagedDataSource
        lstHistory.DataBind()

    End Sub

    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "GROUP_NAME", "GROUP_DESC", "LAST_SYNCED"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function

    Public Function ShowPaging() As String
        If NumberOfResults > 0 Then         
            Return GridScroller.GetGridScroller(CType(Me.Page, BasePage), CInt(lsPage), RecordsPerPage, NumberOfResults)

        Else
            Return "&nbsp;"
            'Return "<b>" & GetDecodedLabel("noresultsfound") & "</b>"
        End If

    End Function
    
    Protected ReadOnly Property InSelectionMode As Boolean
        Get
            Return mbInSelection
        End Get
    End Property
    Public Function OnClickAction(ByVal group As ACL.GROUPList.GROUPSInfo) As String
        If InSelectionMode Then
            Return "javascript:passResult(" & EncodingUtils.EncodeJsString(group.GROUP_NAME) & ")"
        Else
            Return "javascript:OpenGroup(" & EncodingUtils.EncodeJsString(group.GROUP_NAME) & ")"
        End If
    End Function
    Public Function CanDelete() As Boolean
        Return isAdmin AndAlso Security.BusinessIdentity.CurrentIdentity.IsGlobal AndAlso Not InSelectionMode
    End Function
End Class
