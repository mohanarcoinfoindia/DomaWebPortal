Partial Class UserBrowser_Clients
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

        'If Not Settings.GetValue("Security", "EnableIdentityProvider", False) Then
        '    GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        '    Exit Sub
        'End If

        scriptManager.Scripts.Add(New System.Web.UI.ScriptReference("~/Resources/" & Language & "/Messages.js"))

        If Not String.IsNullOrEmpty(ClientIDToDelete.Value) Then
            Arco.Doma.Library.Security.OpenAuthApp.DeleteApplication(ClientIDToDelete.Value)
            ClientIDToDelete.Value = ""
        End If

        LoadClients()
        RegisterScripts()

    End Sub
    Private Sub RegisterScripts()
        Dim sb As String = ""
        sb &= "function AddClient() {"
        sb &= "var oWnd2 = radopen(" & EncodingUtils.EncodeJsString("EditClient.aspx") & ", 'Popup', 800, 600);"
        sb &= "oWnd2.fullScreen = true;"
        sb &= "oWnd2.add_close(Refresh);"
        sb &= "}"

        Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "addclient", sb, True)
    End Sub

    Private Sub LoadClients()

        RecordsPerPage = Math.Max(ArcoInfoSettings.DefaultRecordsPerPage, 30)


        lsPage = Request.Form("Page")
        If String.IsNullOrEmpty(lsPage) Then
            lsPage = "1"
        End If

        Dim llFirstRec As Int32 = GridScroller.FirstRecord(CInt(lsPage), Me.RecordsPerPage)
        Dim llLastRec As Int32 = GridScroller.LastRecord(CInt(lsPage), Me.RecordsPerPage)



        Dim c As New Arco.Doma.Library.Security.OpenAuthAppList.Criteria

        c.FIRSTRECORD = llFirstRec
        c.LASTRECORD = llLastRec
        If Not Request.Form("txtNameFilter") = Nothing Then
            c.Name = Request.Form("txtNameFilter")
        End If

        c.OrderBy = GetActualOrderBy()

        Dim clients As Arco.Doma.Library.Security.OpenAuthAppList = Arco.Doma.Library.Security.OpenAuthAppList.GetApplications(c)

        Me.LastPage = 0
        Dim loPagedDataSource As New PagedDataSource
        loPagedDataSource.AllowPaging = True
        loPagedDataSource.PageSize = Me.RecordsPerPage
        loPagedDataSource.DataSource = clients
        loPagedDataSource.CurrentPageIndex = CInt(lsPage) - 1

        Me.NumberOfResults = loPagedDataSource.DataSourceCount
        Me.CurrentPage = CInt(lsPage)
        Me.LastPage = loPagedDataSource.PageCount


        repClients.DataSource = loPagedDataSource
        repClients.DataBind()
    End Sub

    Private Function GetActualOrderBy() As String

        Select Case orderby.Value
            Case "CLIENTID", "NAME"
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
            If liLastRec > NumberOfResults Then
                liLastRec = NumberOfResults
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

