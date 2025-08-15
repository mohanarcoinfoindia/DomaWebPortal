
Partial Class UserControls_SelectFolderOLD
    Inherits BaseUserControl

    Private msWindowURL As String = "DM_Selectfolder.aspx"
    Private mbShowTextBox As Boolean = True

    Public Property WindowURL() As String
        Get
            Return msWindowURL & "?valuef=" & txtFolderID.UniqueID & "&textf=" & txtFolderName.UniqueID
        End Get
        Set(ByVal value As String)
            msWindowURL = value
        End Set
    End Property
    Public Property ShowTextBox() As Boolean
        Get
            Return mbShowTextBox
        End Get
        Set(ByVal value As Boolean)
            mbShowTextBox = value
        End Set
    End Property
    Public Property FolderID() As Int32
        Get
            Try
                Return CLng(txtFolderID.Value)
            Catch ex As Exception
                Return 0
            End Try
        End Get
        Set(ByVal value As Int32)
            txtFolderID.Value = value
            If Not mbShowTextBox Then
                pnlFolderLink.FolderID = value
            End If
        End Set
    End Property
    Public Property FolderName() As String
        Get
            Return txtFolderName.Text
        End Get
        Set(ByVal value As String)
            txtFolderName.Text = value
        End Set
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If mbShowTextBox Then
            txtFolderName.BackColor = System.Drawing.ColorTranslator.FromHtml("#F0F0F0")
            'txtFolderName.Enabled = false
            'UNDONE : setting textbox property enabled=false excludes it from postback, so data can't be accessed server-side, add client-site attribute
            txtFolderName.Attributes.Add("readonly", True)
            pnlFolderLink.Visible = False
            txtFolderName.Visible = True
        Else
            txtFolderName.Visible = False
            pnlFolderLink.Visible = True
            If Page.IsPostBack Then
                pnlFolderLink.FolderID = FolderID
            End If
        End If
        lnkSelect.NavigateUrl = "javascript:" & Me.ClientID & "_OpenSelection();"
        lnkSelect.Text = GetLabel("select")
    End Sub
End Class
