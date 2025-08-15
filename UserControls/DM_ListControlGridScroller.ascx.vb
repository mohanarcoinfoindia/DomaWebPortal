
Partial Class UserControls_DM_ListControlGridScroller
    Inherits BaseUserControl


    Public Enum eResultType
        Objects = 0
        Files = 1
        Work = 2
        CaseList = 3
        ArchivedCasesList = 4
    End Enum

    Public Property Align As HorizontalAlign = HorizontalAlign.Center

    Protected ReadOnly Property FirstEnabled() As Boolean
        Get
            Return (CurrentPage > 1)
        End Get
    End Property
    Protected ReadOnly Property LastEnabled() As Boolean
        Get
            Return (CurrentPage < LastPage)
        End Get
    End Property
    Protected ReadOnly Property NextEnabled() As Boolean
        Get
            If Not String.IsNullOrEmpty(ActualNumberOfResults) Then
                Return (CurrentPage < LastPage) OrElse Convert.ToInt32(ActualNumberOfResults) > NumberOfResults
            Else
                Return (CurrentPage < LastPage)
            End If
        End Get
    End Property

    Protected ReadOnly Property PreviousEnabled() As Boolean
        Get
            Return (CurrentPage > 1)
        End Get
    End Property
    Protected ReadOnly Property RowCSSClass() As String
        Get

            Return "class='ListFooter'"

        End Get
    End Property
    Public Property GridMode() As Int32

    Public Property ShowBackToSearchLink() As Boolean
    Public Property MaxResults() As Int32
    Public Property GridClientID() As String = ""
    Public Property CurrentPage() As Int32 = 1

    Public Property LastPage() As Int32 = 1

    Public Property NumberOfResults As Int32


    Public Property ActualNumberOfResults As String

    Protected ReadOnly Property NumberOfResultsLabel() As String
        Get
            If String.IsNullOrEmpty(ActualNumberOfResults) Then
                Dim l As Int32 = NumberOfResults
                If NumberOfResults >= MaxResults AndAlso MaxResults > 0 Then
                    Return " > " & MaxResults.ToString
                Else
                    Return NumberOfResults.ToString
                End If
            Else
                Return ActualNumberOfResults
            End If

        End Get
    End Property


    Public Overrides Sub DataBind()
        MyBase.DataBind()

        lnkPrev.Text = GetLabel("previous")
        lnkPrev.NavigateUrl = "javascript:" & GridClientID & ".Goto(" & CurrentPage - 1 & ");"
        lnkPrev.Enabled = PreviousEnabled

        lnkNext.Text = GetLabel("next")
        lnkNext.NavigateUrl = "javascript:" & GridClientID & ".Goto(" & CurrentPage + 1 & ");"
        lnkNext.Enabled = NextEnabled



        If ShowBackToSearchLink Then
            lnkBackToSearch.Visible = True
            lnkBackToSearch.Text = GetLabel("backtosearch")
        Else
            lnkBackToSearch.Visible = False
        End If

        litScroller.Text = PageScroller.GetPageScroller(CurrentPage, LastPage).Render(GridClientID & ".Goto") ' GetLabel("page") & " " & CurrentPage & " / " & LastPage
    End Sub

End Class
