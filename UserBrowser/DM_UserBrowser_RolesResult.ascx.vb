Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL

Partial Class DM_UserBrowser_RolesResult
    Inherits UserBrowserBaseUserControl


    Private mDefaultMaxResults As Int32
    Private mbInSelection As Boolean

    Protected lsPage As String = ""

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
    Public Property CurrentPage() As Int32
        Get
            Dim o As Object

            o = Me.ViewState("_CurrentPage")
            If o Is Nothing Then
                Return 0
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal Value As Int32)
            Me.ViewState("_CurrentPage") = Value
        End Set
    End Property
    Public Property LastPage() As Int32
        Get
            Dim o As Object

            o = Me.ViewState("_LastPage")
            If o Is Nothing Then
                Return 0
            Else
                Return CInt(o)
            End If
        End Get
        Set(ByVal Value As Int32)
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
    Protected Function GetStructureLabel(ByVal viStruct As Integer) As String
        If viStruct = 1 Then
            Return GetLabel("yes")
        Else
            Return GetLabel("no")
        End If
    End Function

    Protected Function GetTenantLabel(ByVal tnid As Integer) As String
        Return DirectCast(Me.Page, BasePage).GetTenantName(tnid)
    End Function
    Protected ReadOnly Property InSelectionMode As Boolean
        Get
            Return mbInSelection
        End Get
    End Property
    Public Function OnClickAction(ByVal role As ROLEList.ROLEInfo) As String
        If InSelectionMode Then
            Return "javascript:passResult(" & EncodingUtils.EncodeJsString(role.ID.ToString) & "," & EncodingUtils.EncodeJsString(role.Name) & ")"
        Else
            Return "javascript:OpenRole(" & EncodingUtils.EncodeJsString(role.ID.ToString) & "," & EncodingUtils.EncodeJsString(role.Name) & ")"
        End If
    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        mbInSelection = (Request.QueryString("mode") = "getresult")

        RecordsPerPage = Math.Max(ArcoInfoSettings.DefaultRecordsPerPage, 30)
        mDefaultMaxResults = ArcoInfoSettings.MaxResults
        MultiTenant = Settings.GetValue("General", "MultiTenant", False)
        LoadDropdowns()

        lsPage = Request.Form("Page")
        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If

        Dim llFirstRec As Int32 = GridScroller.FirstRecord(CInt(lsPage), Me.RecordsPerPage)
        Dim llLastRec As Int32 = GridScroller.LastRecord(CInt(lsPage), Me.RecordsPerPage)

        If Not Page.IsPostBack Then
            txtRoleName.Text = Request.QueryString("txtRoleName")
            If String.IsNullOrEmpty(txtRoleName.Text) Then
                txtRoleName.Text = Request.QueryString("txtRName") 'bw compat
            End If
            txtRoleDescription.Text = Request.QueryString("txtRoleDesc")
        End If


        Dim lsRoleType As String

        If Request("txtRoleType") = Nothing And Not IsPostBack Then
            lsRoleType = Request.QueryString("roletype")
        Else
            lsRoleType = Request("txtRoleType")
        End If

        If Not Request.QueryString("ORDERBY") Is Nothing And Not IsPostBack Then
            orderby.Value = Request.QueryString("ORDERBY")
        End If

        Dim loCrit As New ROLEList.Criteria With {
                .ROLE_DESCRIPTION = InputSanitizer.Sanitize(txtRoleDescription.Text),
                .FILTER = InputSanitizer.Sanitize(txtRoleName.Text),
                .SHOWEVERYONE = False,
                .UserHasAcl = (Not IsAdmin AndAlso Not mbInSelection)
            }

        If lsRoleType = "1" Then
            loCrit.Structured = True
        End If
        If Not String.IsNullOrEmpty(drpTenant.SelectedValue) Then
            loCrit.TenantId = Convert.ToInt32(drpTenant.SelectedValue)
        End If
        loCrit.Range = ListRangeRequest.Range(llFirstRec, llLastRec)
        loCrit.OrderBy = GetActualOrderBy()

        Dim luRolelist As ROLEList = ROLEList.GetROLEList(loCrit)

        LastPage = 0
        Dim loPagedDataSource As New PagedDataSource
        loPagedDataSource.AllowPaging = True
        loPagedDataSource.PageSize = RecordsPerPage
        loPagedDataSource.DataSource = luRolelist
        loPagedDataSource.CurrentPageIndex = CInt(lsPage) - 1

        NumberOfResults = loPagedDataSource.DataSourceCount
        CurrentPage = CInt(lsPage)
        LastPage = loPagedDataSource.PageCount
        lstHistory.DataSource = loPagedDataSource
        lstHistory.DataBind()

    End Sub

    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "ROLE_NAME", "ROLE_DESCRIPTION", "TENANT_ID", "ROLE_STRUCTURED"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function

    Public Property MultiTenant As Boolean
    Private Sub LoadDropdowns()
        If Not Page.IsPostBack Then

            If MultiTenant Then
                drpTenant.Items.Clear()
                drpTenant.Items.Add("")
                drpTenant.Items.Add(New WebControls.ListItem("Global", "0"))
                For Each tn As TenantList.TenantInfo In DirectCast(Page, BasePage).Tenants
                    If Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal OrElse Security.BusinessIdentity.CurrentIdentity.Tenant.Id = tn.Id Then
                        drpTenant.Items.Add(New WebControls.ListItem(tn.Name, tn.Id.ToString()))
                    End If
                Next
            End If

        End If

    End Sub
    Public ReadOnly Property ColSpan As Int32
        Get
            Dim i As Int32 = 3
            If MultiTenant Then
                i += 1
            End If
            If CanDelete() Then
                i += 1
            End If
            Return i
        End Get
    End Property
    Public Function CanDelete(ByVal role As ACL.ROLEList.ROLEInfo) As Boolean
        Return Not role.IsSystem AndAlso (Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal OrElse role.TenantId = Security.BusinessIdentity.CurrentIdentity.Tenant.Id)
    End Function
    Public Function CanDelete() As Boolean

        Return Not mbInSelection AndAlso isAdmin
    End Function

    Public Function ShowPaging() As String
        If NumberOfResults > 0 Then
            Return GridScroller.GetGridScroller(CType(Me.Page, BasePage), CInt(lsPage), RecordsPerPage, NumberOfResults)
        Else
            Return "&nbsp;"
            'Return "<b>" & GetDecodedLabel("noresultsfound") & "</b>"
        End If
    End Function

    Public Function ShowAddButton() As Boolean
        Return Not InSelectionMode AndAlso isAdmin
    End Function
End Class
