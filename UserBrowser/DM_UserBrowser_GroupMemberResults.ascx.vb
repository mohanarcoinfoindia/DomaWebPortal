Imports Arco.Doma.Library
Partial Class DM_UserBrowser_GroupMemberResults
    Inherits BaseUserControl


    Private mDefaultMaxResults As Int32

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
    Public Property RecordsPerPage() As Int16

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

        RecordsPerPage = Math.Max(ArcoInfoSettings.DefaultRecordsPerPage, 1000)

        mDefaultMaxResults = ArcoInfoSettings.MaxResults
     
        Dim lsPage As String
        Dim llFirstRec As Int32 = 0
        Dim llLastRec As Int32 = 0
        Dim lsSearchedFor As String = ""

        lsPage = Request.Form("Page")
        If lsPage = "" Then
            lsPage = "1"
        End If

        llFirstRec = ((CInt(lsPage) - 1) * Me.RecordsPerPage) + 1
        llLastRec = llFirstRec + Me.RecordsPerPage - 1
        If Request.QueryString("login") <> "" Then
            Dim luGroupMemberlist As ACL.GroupMemberList
            luGroupMemberlist = ACL.GroupMemberList.GetGroupMemberList(Request.QueryString("login"))
            If luGroupMemberlist.Any Then
                lstGroups.DataSource = luGroupMemberlist
                lstGroups.DataBind()
            End If
        End If
    End Sub   
End Class
