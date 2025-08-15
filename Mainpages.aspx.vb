
Imports Arco.Doma.Library
Imports Arco.Doma.Library.Website

Partial Class Mainpages
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
    Private Sub UserBrowser_Sitemap_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsGlobal Then
            GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        End If

        scriptManager.Scripts.Add(New ScriptReference("~/Resources/" & Language & "/Messages.js"))

        If IsNumeric(PageIDToDelete.Value) Then
            Try
                MainPage.DeleteMainPage(Convert.ToInt32(PageIDToDelete.Value))
            Catch appEx As Arco.ApplicationServer.Library.Exceptions.ApplicationServerException
                ShowException(Arco.Utils.ExceptionHelper.GetInnerExceptionMessage(Of Arco.Doma.Library.Exceptions.DomaException)(appEx))
            Catch ex As Exception
                ShowException(ex.Message)
            End Try

            PageIDToDelete.Value = ""
        End If

        LoadEntries()

    End Sub


    Private Sub LoadEntries()

        RecordsPerPage = Math.Max(ArcoInfoSettings.DefaultRecordsPerPage, 30)


        Dim llFirstRec As Int32
        Dim llLastRec As Int32

        lsPage = Request.Form("Page")
        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If

        llFirstRec = ((CInt(lsPage) - 1) * Me.RecordsPerPage) + 1
        llLastRec = llFirstRec + Me.RecordsPerPage - 1



        Dim c As New MainPageList.Criteria()

        c.Range = ListRangeRequest.Range(llFirstRec, llLastRec)


        c.Name = Request.Form("txtNameFilter")

        c.OrderBy = GetActualOrderBy()

        Me.LastPage = 0
        Dim loPagedDataSource As New PagedDataSource
        loPagedDataSource.AllowPaging = True
        loPagedDataSource.PageSize = Me.RecordsPerPage
        loPagedDataSource.DataSource = MainPageList.GetMainPages(c)
        loPagedDataSource.CurrentPageIndex = CInt(lsPage) - 1

        Me.NumberOfResults = loPagedDataSource.DataSourceCount
        Me.CurrentPage = CInt(lsPage)
        Me.LastPage = loPagedDataSource.PageCount


        repPages.DataSource = loPagedDataSource
        repPages.DataBind()
    End Sub

    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "NAME", "DESCRIPTION", "QUERYSTRING"
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

    Private Sub ShowException(ByVal msg As String)
        Dim sb As New StringBuilder
        sb.AppendLine("alert(" & EncodingUtils.EncodeJsString(msg) & ");")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "Alert", sb.ToString, True)
    End Sub


End Class
