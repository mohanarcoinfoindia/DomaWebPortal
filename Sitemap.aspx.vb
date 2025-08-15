
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Website
Imports Arco.Doma.Library.Security

Partial Class UserBrowser_Sitemap
    Inherits BaseAdminOnlyPage


    Protected lsPage As String = ""
#Region "tableProperties"
    Public Property ShowSiteVersion As Boolean

    Public Property CurrentPage() As Integer
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
    Public Property LastPage() As Integer
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
    Public Property RecordsPerPage As Integer

    Public Property NumberOfResults As Integer
#End Region
    Private Sub UserBrowser_Sitemap_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not BusinessIdentity.CurrentIdentity.IsGlobal Then
            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        End If

        ShowSiteVersion = Not String.IsNullOrEmpty(Settings.GetValue("General", "url_v8", ""))

        scriptManager.Scripts.Add(New ScriptReference("~/Resources/" & Language & "/Messages.js"))

        If IsNumeric(EntryIDToDelete.Value) Then
            Try
                SiteApplication.DeleteApplication(Convert.ToInt32(EntryIDToDelete.Value))
            Catch appEx As Arco.ApplicationServer.Library.Exceptions.ApplicationServerException
                ShowException(Arco.Utils.ExceptionHelper.GetInnerExceptionMessage(Of Arco.Doma.Library.Exceptions.DomaException)(appEx))
            Catch ex As Exception
                ShowException(ex.Message)
            End Try

            EntryIDToDelete.Value = ""
        End If

        If Not Page.IsPostBack Then
            'load parentId filter
            drpParentId.Items.Add(New WebControls.ListItem("", "-1"))

            For Each parentId As Integer In GetAvailableParentIds()
                drpParentId.Items.Add(New WebControls.ListItem(parentId, parentId))
            Next

        End If

        LoadEntries()

    End Sub

    Private Function GetAvailableParentIds() As IEnumerable(Of Integer)
        Dim c As New SiteApplicationList.Criteria()
        Return SiteApplicationList.GetApplications(c).Select(Function(x) x.ParentId).Distinct()

    End Function


    Private Sub LoadEntries()

        RecordsPerPage = Math.Max(ArcoInfoSettings.DefaultRecordsPerPage, 30)

        lsPage = Request.Form("Page")
        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If

        Dim firstRecord As Integer = ((CInt(lsPage) - 1) * RecordsPerPage) + 1
        Dim lastRecord As Integer = firstRecord + RecordsPerPage - 1


        Dim c As New SiteApplicationList.Criteria()

        c.Range = ListRangeRequest.Range(firstRecord, lastRecord)
        If ShowSiteVersion Then
            c.SiteVersion = drpSiteVersion.SelectedValue
        End If
        c.Name = txtNameFilter.Text
            c.Group = txtGroupFilter.Text
        c.Description = txtDescFilter.Text
        c.Url = txtUrlFilter.Text
        c.ParentId = CInt(drpParentId.SelectedValue)
        c.OrderBy = GetActualOrderBy()

        Dim loPagedDataSource As New PagedDataSource
        loPagedDataSource.AllowPaging = True
        loPagedDataSource.PageSize = RecordsPerPage
        loPagedDataSource.DataSource = SiteApplicationList.GetApplications(c)
        loPagedDataSource.CurrentPageIndex = CInt(lsPage) - 1

        Me.NumberOfResults = loPagedDataSource.DataSourceCount
        Me.CurrentPage = CInt(lsPage)
        Me.LastPage = loPagedDataSource.PageCount


        repEntries.DataSource = loPagedDataSource
        repEntries.DataBind()
    End Sub

    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "NAME", "DESCRIPTION", "URL", "ID", "SEQ", "SITE_VERSION", "PARENT_ID", "APP_GROUP"
                Return orderby.Value & " " & GetOrderByOrder(orderbyorder.Value)
        End Select
        Return ""
    End Function

    Public Function ShowPaging() As String

        If NumberOfResults > 0 Then
            Return GridScroller.GetGridScroller(CType(Me.Page, BasePage), CInt(lsPage), RecordsPerPage, NumberOfResults)
        End If

        Return "<b>" & GetDecodedLabel("noresultsfound") & "</b>"

    End Function

    Private Sub ShowException(ByVal msg As String)
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "Alert", "alert(" & EncodingUtils.EncodeJsString(msg) & ");", True)
    End Sub


End Class
