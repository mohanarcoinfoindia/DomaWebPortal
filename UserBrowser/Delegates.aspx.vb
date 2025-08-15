
Imports Arco.Doma.Library

Public Class DelegatesPage
    Inherits UserBrowserBasePage


    Private mDefaultMaxResults As Int32

    Public Property MultiTenant As Boolean

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
    Public Function CheckCanManage() As Boolean
        If Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.CanManageDelegations Then
            Response.Redirect("../DM_ACL_DENIED.aspx")
            Return False
        Else
            Return True
        End If
    End Function
    Protected Sub UserBrowser_Delegates_Load(sender As Object, e As EventArgs) Handles Me.Load
        If CheckCanManage() Then

            MultiTenant = Settings.GetValue("General", "MultiTenant", False)

            scriptManager.Scripts.Add(New ScriptReference("~/Resources/" & Language & "/Messages.js"))

            If IsNumeric(DelegateIDToDelete.Value) Then
                Arco.Doma.Library.Routing.WorkException.DeleteWorkException(Convert.ToInt32(DelegateIDToDelete.Value))
                DelegateIDToDelete.Value = ""
            End If

            If Not Page.IsPostBack AndAlso MultiTenant Then
                drpTenant.Items.Clear()
                drpTenant.Items.Add("")
                drpTenant.Items.Add(New WebControls.ListItem("Global", "0"))
                For Each tn As TenantList.TenantInfo In Tenants
                    If Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal OrElse Security.BusinessIdentity.CurrentIdentity.Tenant.Id = tn.Id Then
                        drpTenant.Items.Add(New WebControls.ListItem(tn.Name, tn.Id.ToString()))
                    End If
                Next
            End If

            LoadDelegates()
            RegisterScripts()
        End If
    End Sub
    Private Sub RegisterScripts()
        Dim sb As String = ""
        sb &= "function AddDelegate() {"
        sb &= "const width = document.body.scrollWidth * 0.75; const height = document.body.scrollHeight * 0.90;"
        sb &= "var oWnd2 = radopen(" & EncodingUtils.EncodeJsString("EditDelegate.aspx") & ", 'Popup', width, height);"
        sb &= "oWnd2.fullScreen = true;"
        sb &= "oWnd2.add_close(Refresh);"
        sb &= "}"

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "addelegate", sb, True)
    End Sub

    Private Sub LoadDelegates()
        chkActiveOnly.Text = GetLabel("activeonly")

        RecordsPerPage = Math.Max(ArcoInfoSettings.DefaultRecordsPerPage, 30)
        mDefaultMaxResults = ArcoInfoSettings.MaxResults

        Dim llFirstRec As Int32
        Dim llLastRec As Int32

        lsPage = Request.Form("Page")
        If lsPage = "" Then
            lsPage = "1"
        End If

        llFirstRec = ((CInt(lsPage) - 1) * Me.RecordsPerPage) + 1
        llLastRec = llFirstRec + Me.RecordsPerPage - 1

        Dim c As New Arco.Doma.Library.Routing.WorkExceptionList.Criteria
        c.ForCurrentUser = False
        c.ActiveOnly = chkActiveOnly.Checked
        c.Type = Arco.Doma.Library.Routing.WorkExceptionList.WF_WorkExceptionType.WF_Delegate
        c.Range = ListRangeRequest.Range(llFirstRec, llLastRec)

        If Not Request.Form("txtFromFilter") = Nothing Then
            c.DelegateFromFilter = Request.Form("txtFromFilter")
        End If

        If Not Request.Form("txtToFilter") = Nothing Then
            c.SubjectFilter = Request.Form("txtToFilter")
        End If
        If Not String.IsNullOrEmpty(drpTenant.SelectedValue) Then
            c.TenantId = Convert.ToInt32(drpTenant.SelectedValue)
        End If
        c.OrderBy = GetActualOrderBy()

        Dim lcolDelegates As Arco.Doma.Library.Routing.WorkExceptionList = Arco.Doma.Library.Routing.WorkExceptionList.GetWorkExceptions(c)

        Me.LastPage = 0
        Dim loPagedDataSource As PagedDataSource = New PagedDataSource
        loPagedDataSource.AllowPaging = True
        loPagedDataSource.PageSize = Me.RecordsPerPage
        loPagedDataSource.DataSource = lcolDelegates
        loPagedDataSource.CurrentPageIndex = CInt(lsPage) - 1

        Me.NumberOfResults = loPagedDataSource.DataSourceCount
        Me.CurrentPage = CInt(lsPage)
        Me.LastPage = loPagedDataSource.PageCount

        Dim dt As New DataTable()
        dt.Columns.Add("ID")
        dt.Columns.Add("FROM")
        dt.Columns.Add("TO")
        dt.Columns.Add("PROCEDURE")
        dt.Columns.Add("MODE")
        dt.Columns.Add("BEGIN")
        dt.Columns.Add("END")
        dt.Columns.Add("TenantId")

        Dim lcolProcLabels As Arco.Doma.Library.Globalisation.LABELList = Nothing
        For Each loDel As Arco.Doma.Library.Routing.WorkExceptionList.WorkExceptionInfo In loPagedDataSource
            Dim lsMode As String = ""
            Select Case loDel.Mode
                Case Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Timed
                    lsMode = GetLabel("del_timed")

                Case Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_NoMode
                    lsMode = "NO MODE"

                Case Arco.Doma.Library.Routing.WF_WorkExceptionMode.WF_Manual
                    lsMode = GetLabel("del_manual")
            End Select
            Dim lsProc As String = ""
            If loDel.Object_ID <> 0 Then
                Try
                    If lcolProcLabels Is Nothing Then lcolProcLabels = Arco.Doma.Library.Globalisation.LABELList.GetProceduresLabelList(Me.EnableIISCaching)
                    Dim loProc As Arco.Doma.Library.Routing.Procedure = Arco.Doma.Library.Routing.Procedure.GetProcedure(loDel.Object_ID)
                    lsProc = lcolProcLabels.GetObjectLabel(loProc.ID, "Procedure", Language, loProc.Name) & " (" & loProc.PROC_MAJOR_VERSION & "." & loProc.PROC_MINOR_VERSION & ")"
                Catch ex As Exception

                End Try

            End If
            Dim lsFrom As String
            Select Case loDel.DelegateFrom_Type
                Case "User"
                    lsFrom = ArcoFormatting.FormatUserName(loDel.DelegateFrom_ID) & "&nbsp;<small>(" & loDel.DelegateFrom_ID & ")</small>"
                Case Else
                    lsFrom = loDel.DelegateFrom_ID
            End Select
            Dim lsTo As String
            Select Case loDel.Subject_Type
                Case "User"
                    lsTo = ArcoFormatting.FormatUserName(loDel.Subject_ID) & "&nbsp;<small>(" & loDel.Subject_ID & ")</small>"
                Case Else
                    lsTo = loDel.Subject_ID
            End Select
            dt.Rows.Add(New String() {loDel.ID, lsFrom, lsTo, lsProc, lsMode, ArcoFormatting.FormatDateLabel(loDel.Start_Date, False, False, False), ArcoFormatting.FormatDateLabel(loDel.End_Date, False, False, False), loDel.TenantId})
        Next

        repDelegates.DataSource = dt
        repDelegates.DataBind()
    End Sub
    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "SUBJECT_ID", "DELEGATE_FROM_ID", "OBJECT_ID", "TENANT_ID", "EXCEPTION_MODE", "START_DATE", "END_DATE"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function

    Public Function ShowPaging() As String

        If NumberOfResults = 0 Then
            Return GetLabel("del_nodelatesfound")
        End If

        Dim lstable As String = ""
        Dim liFirstRec, liLastRec As Int32

        liFirstRec = ((CInt(lsPage)) * RecordsPerPage) + 1
        liLastRec = liFirstRec + RecordsPerPage - 1
        If NumberOfResults >= liFirstRec Or NumberOfResults <= liLastRec Then
            Dim lbPrev, lbNext As Boolean
            Dim lsPrev, lsNext As String
            Dim liLastPage As Integer

            liLastPage = (NumberOfResults \ RecordsPerPage)

            If NumberOfResults Mod RecordsPerPage > 0 Then
                liLastPage += 1
            End If

            If CInt(lsPage) > 1 Then
                lbPrev = True
            Else
                lbPrev = False
            End If

            If CInt(lsPage) < liLastPage Then
                lbNext = True
            Else
                lbNext = False
            End If

            If lbPrev Then
                lsPrev = "<a href='JavaScript:Goto(" & CInt(lsPage) - 1 & ");' class='ButtonLink'>" & GetDecodedLabel("previous") & "</a>"
            Else
                lsPrev = "<a class='ButtonLink' disabled='disabled'>" & GetDecodedLabel("previous") & "</a>"
            End If

            If lbNext Then
                lsNext = "<a href='JavaScript:Goto(" & CInt(lsPage) + 1 & ");' class='ButtonLink'>" & GetDecodedLabel("next") & "</a>"
            Else
                lsNext = "<a class='ButtonLink' disabled='disabled'>" & GetDecodedLabel("next") & "</a>"
            End If
            lstable = lstable & lsPrev & "&nbsp;" & PageScroller.GetPageScroller(CInt(lsPage), liLastPage).Render("Goto") & "&nbsp;" & lsNext & " &nbsp;&nbsp;<b>(" & NumberOfResults & " " & GetDecodedLabel("resultsfound") & ")</b>"
        Else
            lstable = lstable & "<b>" & GetDecodedLabel("noresultsfound") & "</b>"
        End If
        Return lstable

    End Function

    Public ReadOnly Property ColSpan As String
        Get
            If MultiTenant Then
                Return "8"
            End If
            Return "7"
        End Get
    End Property

End Class
