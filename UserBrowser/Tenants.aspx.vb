
Imports Arco.Doma.Library

Partial Class UserBrowser_Tenants
    Inherits BaseAdminOnlyPage

    Protected lsPage As String = ""
#Region "tableProperties"

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

#End Region

    Protected Sub UserBrowser_Tenants_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsGlobal Then
            GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        End If

        scriptManager.Scripts.Add(New System.Web.UI.ScriptReference("~/Resources/" & Language & "/Messages.js"))

        If IsNumeric(TenantIDToDelete.Value) Then
            Try
                Arco.Doma.Library.Tenant.DeleteTenant(Convert.ToInt32(TenantIDToDelete.Value))
            Catch appEx As Arco.ApplicationServer.Library.Exceptions.ApplicationServerException
                ShowException(Arco.Utils.ExceptionHelper.GetInnerExceptionMessage(Of Arco.Doma.Library.Exceptions.DomaException)(appEx))
            Catch ex As Exception
                ShowException(ex.Message)
            End Try

            TenantIDToDelete.Value = ""
        End If

        LoadTenants()
        RegisterScripts()

    End Sub

    Private Sub ShowException(ByVal msg As String)
        Dim sb As New StringBuilder
        sb.AppendLine("alert(" & EncodingUtils.EncodeJsString(msg) & ");")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "Alert", sb.ToString, True)
    End Sub

    Public Function GetStatusLabel(ByVal tenant As Arco.Doma.Library.ITenant) As String
        Return If(tenant.Enabled, GetLabel("enabled"), GetLabel("disabled"))
    End Function
    Private Sub RegisterScripts()
        Dim sb As StringBuilder = New StringBuilder

        sb.Append("function AddTenant() {")
        sb.AppendLine("location.href = ")
        sb.Append(EncodingUtils.EncodeJsString("EditTenant.aspx") & ";")
        'sb &= "var oWnd2 = radopen(" & EncodingUtils.EncodeJsString("EditTenant.aspx") & ", 'Popup');"
        'sb &= "oWnd2.fullScreen = true;"
        'sb &= "oWnd2.add_close(Refresh);"
        sb.AppendLine("}")

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "adtenant", sb.ToString, True)
    End Sub

    Private Sub LoadTenants()

        RecordsPerPage = Math.Max(ArcoInfoSettings.DefaultRecordsPerPage, 30)


        lsPage = Request.Form("Page")
        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If

        Dim llFirstRec As Int32 = GridScroller.FirstRecord(CInt(lsPage), Me.RecordsPerPage)
        Dim llLastRec As Int32 = GridScroller.LastRecord(CInt(lsPage), Me.RecordsPerPage)

        Dim c As New TenantList.Criteria

        c.Range = ListRangeRequest.Range(llFirstRec, llLastRec)


        c.Name = Request.Form("txtNameFilter")

        c.OrderBy = GetActualOrderBy()

        Me.LastPage = 0
        Dim loPagedDataSource As New PagedDataSource
        loPagedDataSource.AllowPaging = True
        loPagedDataSource.PageSize = Me.RecordsPerPage
        loPagedDataSource.DataSource = TenantList.GetTenants(c)
        loPagedDataSource.CurrentPageIndex = CInt(lsPage) - 1

        Me.NumberOfResults = loPagedDataSource.DataSourceCount
        Me.CurrentPage = CInt(lsPage)
        Me.LastPage = loPagedDataSource.PageCount


        repTenants.DataSource = loPagedDataSource
        repTenants.DataBind()
    End Sub
    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "TENANT_NAME", "TENANT_DESCRIPTION", "TENANT_ID", "URL", "ENABLED"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
            Case Else
                Return ""
        End Select

    End Function

    Public Function ShowPaging() As String

        If NumberOfResults = 0 Then
            Return GetLabel("noresultsfound")
        End If

        Dim lstable As String = ""
        Dim liFirstRec, liLastRec As Int32

        liFirstRec = ((CInt(lsPage)) * RecordsPerPage) + 1
        liLastRec = liFirstRec + RecordsPerPage - 1
        If NumberOfResults >= liFirstRec OrElse NumberOfResults <= liLastRec Then
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

End Class
