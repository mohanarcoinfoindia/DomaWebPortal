Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL

Partial Class DM_UserBrowser_Results
    Inherits UserBrowserBaseUserControl

    Private mDefaultMaxResults As Int32

    Private mbInSelection As Boolean
#Region "Table properties"
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

    Public Property MultiTenant As Boolean
#End Region

    Protected ReadOnly Property InSelectionMode As Boolean
        Get
            Return mbInSelection
        End Get
    End Property


    Protected Overrides Sub OnLoad(ByVal e As EventArgs)
        MyBase.OnLoad(e)

        MultiTenant = Settings.GetValue("General", "MultiTenant", False) AndAlso Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal

        LoadDropdowns()

        If Not Page.IsPostBack Then
            txtDisplayName.Text = Request.QueryString("txtdisplay")
            txtLoginName.Text = Request.QueryString("txtlogin")
            SetDropdownValue(drpdwnStatus, Request.QueryString("drpStatus"))
        End If

        Dim namedInApp As String = Request.QueryString("namedin")

        mbInSelection = (Request.QueryString("mode") = "getresult")


        RecordsPerPage = Math.Max(ArcoInfoSettings.DefaultRecordsPerPage, 30)

        mDefaultMaxResults = ArcoInfoSettings.MaxResults

        Dim llFirstRec As Int32
        Dim llLastRec As Int32

        If String.IsNullOrEmpty(hdnPage.Value) Then
            hdnPage.Value = "1"
        End If

        llFirstRec = GridScroller.FirstRecord(CInt(hdnPage.Value), RecordsPerPage)
        llLastRec = GridScroller.LastRecord(CInt(hdnPage.Value), RecordsPerPage)

        If Not Request.QueryString("ORDERBY") Is Nothing And Not IsPostBack Then
            orderby.Value = Request.QueryString("ORDERBY")
        End If
        Dim liStatus As User.UserStatus
        Select Case drpdwnStatus.SelectedValue
            Case "Valid"
                liStatus = User.UserStatus.Valid
            Case "Blocked"
                liStatus = User.UserStatus.Blocked
            Case Else
                liStatus = User.UserStatus.Undefined
        End Select


        Dim loCrit As New USERList.Criteria(txtLoginName.Text, txtDisplayName.Text, liStatus)
        If Not Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal Then
            loCrit.TenantId = Security.BusinessIdentity.CurrentIdentity.Tenant.Id
        ElseIf Not String.IsNullOrEmpty(drpTenant.SelectedValue) Then
            loCrit.TenantId = Convert.ToInt32(drpTenant.SelectedValue)
        End If

        loCrit.EmailFilter = txtEmail.Text
        loCrit.DOMAIN = drpDomain.SelectedValue
        loCrit.Range = ListRangeRequest.Range(llFirstRec, llLastRec)
        loCrit.OrderBy = GetActualOrderBy()
        loCrit.IsNamedInApplication = namedInApp

        Dim results As USERList = USERList.GetUSERSList(loCrit)

        While results.Count > 0 AndAlso llFirstRec > 1 AndAlso results.Count < llFirstRec
            llFirstRec = llFirstRec - RecordsPerPage
            llLastRec = llLastRec - RecordsPerPage
            hdnPage.Value = CInt(hdnPage.Value) - 1
            loCrit.Range = ListRangeRequest.Range(llFirstRec, llLastRec)
            results = USERList.GetUSERSList(loCrit)
        End While

        If results.Count = 1 AndAlso InSelectionMode Then
            Dim user As USERList.USERSInfo = results.Item(0)
            Page.ClientScript.RegisterStartupScript(Page.ClientScript.[GetType](), "onLoad", "passResult(" & EncodingUtils.EncodeJsString(user.USER_LOGIN) & "," & EncodingUtils.EncodeJsString(user.GetDisplayName) & ");", True)
        Else
            Me.LastPage = 0
            Dim loPagedDataSource As PagedDataSource
            loPagedDataSource = New PagedDataSource
            loPagedDataSource.AllowPaging = True
            loPagedDataSource.PageSize = Me.RecordsPerPage
            loPagedDataSource.DataSource = results
            loPagedDataSource.CurrentPageIndex = CInt(hdnPage.Value) - 1

            NumberOfResults = loPagedDataSource.DataSourceCount
            CurrentPage = CInt(hdnPage.Value)
            LastPage = loPagedDataSource.PageCount
            lstHistory.DataSource = loPagedDataSource
            lstHistory.DataBind()

        End If


    End Sub
    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "USER_DISPLAY_NAME", "USER_LOGIN", "USER_MAIL", "USER_MAIL", "LAST_LOGIN", "LAST_SYNCED"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function

    Protected Function GetTenantLabel(ByVal voUser As USERList.USERSInfo) As String
        If voUser.IsTenantUser Then
            Dim sb As New StringBuilder()
            For Each tenant As TenantList.TenantInfo In TenantList.GetTenants(voUser.USER_LOGIN)
                If sb.Length <> 0 Then
                    sb.Append(", ")
                End If
                sb.Append(tenant.Name)
            Next
            If sb.Length = 0 Then
                sb.Append("No tenant set!")
            End If
            Return sb.ToString()
        End If


        Return ""
    End Function
    Private Shared Sub SetDropdownValue(ByVal drp As DropDownList, ByVal value As String)

        If Not String.IsNullOrEmpty(value) Then
            For Each li In drp.Items
                If li.Value = value Then
                    li.Selected = True
                    Exit Sub
                End If
            Next
        End If
    End Sub

    Public Property HasMultipleDomains As Boolean

    Public ReadOnly Property HasSyncedUsers As Boolean
        Get
            Return True ' HasMultipleDomains
        End Get
    End Property
    Private Sub LoadDropdowns()
        drpdwnStatus.Items(1).Text = GetDecodedLabel("enabled")
        drpdwnStatus.Items(2).Text = GetDecodedLabel("disabled")

        If Not Page.IsPostBack Then
            drpDomain.Items.Clear()
            drpDomain.Items.Add("")
            For Each loDomain As DOMAINList.DOMAINSInfo In DOMAINList.GetDOMAINSList(True)
                drpDomain.Items.Add(loDomain.DOMAIN_NAME)
                HasMultipleDomains = True
            Next

            If MultiTenant Then
                drpTenant.Items.Clear()
                drpTenant.Items.Add("")
                drpTenant.Items.Add(New WebControls.ListItem("Global", "0"))
                For Each tn As TenantList.TenantInfo In DirectCast(Page, BasePage).Tenants
                    drpTenant.Items.Add(New WebControls.ListItem(tn.Name, tn.Id.ToString()))
                Next
            End If
        Else
            HasMultipleDomains = (drpDomain.Items.Count > 1)
        End If

    End Sub
    Public Function getStatus(ByVal bindingValue As Int32) As String
        Dim liBindingValue As User.UserStatus = CType(bindingValue, User.UserStatus)
        Dim lsStatus As String = ""

        Select Case liBindingValue
            Case User.UserStatus.Valid
                lsStatus = GetLabel("enabled")
            Case User.UserStatus.Blocked
                lsStatus = GetLabel("disabled")
            Case Else
                lsStatus = GetLabel("ub_unknown") & " : " & bindingValue
        End Select

        Return lsStatus
    End Function

    Public Function CanDelete(ByVal voUser As USERList.USERSInfo) As Boolean
        Return Not InSelectionMode AndAlso IsAdmin
    End Function

    Public Function ShowPaging() As String

        If NumberOfResults > 0 Then
            Return GridScroller.GetGridScroller(CType(Me.Page, BasePage), CInt(hdnPage.Value), RecordsPerPage, NumberOfResults)
        Else
            Return "<b>" & GetDecodedLabel("noresultsfound") & "</b>"
        End If


    End Function

    Public ReadOnly Property ColSpan As Int32
        Get
            Dim i As Int32 = 6
            If HasMultipleDomains Then
                i += 1
            End If
            If MultiTenant Then
                i += 1
            End If
            If HasSyncedUsers Then
                i += 1
            End If
            Return i
        End Get
    End Property
End Class
