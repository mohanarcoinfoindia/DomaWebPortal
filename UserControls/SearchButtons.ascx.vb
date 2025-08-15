
Partial Class UserControls_SearchButtons
    Inherits BaseUserControl

    Public Property RenderSingleButton() As Boolean
#Region " Show/Hide buttons "
    Private mbShowSaveButton As Boolean = True
    Private mbShowClearButton As Boolean
    Private mbShowOpenButton As Boolean = True
    Private mbShowFindButton As Boolean = True
    Private mbShowCountButton As Boolean = True

    Public Property ShowClearButton() As Boolean
        Get
            Return mbShowClearButton
        End Get
        Set(ByVal value As Boolean)
            mbShowClearButton = value
            lnk3.Visible = value

        End Set
    End Property
    Public Property ShowCountButton() As Boolean
        Get
            Return mbShowCountButton
        End Get
        Set(ByVal value As Boolean)
            mbShowCountButton = value
            lnk2.Visible = value
        End Set
    End Property
    Public Property ShowSaveButton() As Boolean
        Get
            Return mbShowSaveButton
        End Get
        Set(ByVal value As Boolean)
            mbShowSaveButton = value
            lnk4.Visible = value
        End Set
    End Property
    Public Property ShowOpenButton() As Boolean
        Get
            Return mbShowOpenButton
        End Get
        Set(ByVal value As Boolean)
            mbShowOpenButton = value
            lnk5.Visible = value
        End Set
    End Property
    Public Property ShowFindButton() As Boolean
        Get
            Return mbShowFindButton
        End Get
        Set(ByVal value As Boolean)
            mbShowFindButton = value
            lnk1.Visible = value
        End Set
    End Property
#End Region

#Region " Labels "
    Private _findButtonLabel As String
    Public Property FindButtonLabel As String
        Get
            If Not String.IsNullOrEmpty(_findButtonLabel) Then
                Return _findButtonLabel
            End If
            Return GetLabel("find")
        End Get
        Set(value As String)
            _findButtonLabel = value
        End Set
    End Property
    Private _countButtonLabel As String
    Public Property CountButtonLabel As String
        Get
            If Not String.IsNullOrEmpty(_countButtonLabel) Then
                Return _countButtonLabel
            End If
            Return GetLabel("countresults")
        End Get
        Set(value As String)
            _countButtonLabel = value
        End Set
    End Property

    Private _saveButtonLabel As String
    Public Property SaveButtonLabel As String
        Get
            If Not String.IsNullOrEmpty(_saveButtonLabel) Then
                Return _saveButtonLabel
            End If
            Return GetLabel("save")
        End Get
        Set(value As String)
            _saveButtonLabel = value
        End Set
    End Property

    Private _openButtonLabel As String
    Public Property OpenButtonLabel As String
        Get
            If Not String.IsNullOrEmpty(_openButtonLabel) Then
                Return _openButtonLabel
            End If
            Return GetLabel("open")
        End Get
        Set(value As String)
            _openButtonLabel = value
        End Set
    End Property
    Private _clearButtonLabel As String
    Public Property ClearButtonLabel As String
        Get
            If Not String.IsNullOrEmpty(_clearButtonLabel) Then
                Return _clearButtonLabel
            Else
                Return GetLabel("clear")
            End If
        End Get
        Set(value As String)
            _clearButtonLabel = value
        End Set
    End Property
#End Region

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        lnk1.Visible = ShowFindButton
        lnk2.Visible = ShowCountButton
        lnk3.Visible = ShowClearButton
        lnk4.Visible = ShowSaveButton
        lnk5.Visible = ShowOpenButton

        lnk1.Text = FindButtonLabel
        lnk2.Text = CountButtonLabel
        lnk3.Text = ClearButtonLabel
        lnk4.Text = SaveButtonLabel
        lnk5.Text = OpenButtonLabel

    End Sub

End Class
