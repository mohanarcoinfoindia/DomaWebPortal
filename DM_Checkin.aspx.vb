
Imports Arco.Doma.Library

Partial Class DM_Checkin
    Inherits BasePage

    Private _ObjectID As Int32
    Private _CatID As Int32
    Private _FolderID As Int32

    Private Enum Action
        CANCEL = 0
        NEWMAJOR = 1
        NEWMINOR = 2
        OVERWRITE = 3
    End Enum

    Private Sub ParseQueryString()

        _ObjectID = QueryStringParser.GetInt("DM_Object_ID")
        _CatID = QueryStringParser.GetInt("catid")
        _FolderID = QueryStringParser.GetInt("folderid")

    End Sub


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        ParseQueryString()

        If Not Page.IsPostBack Then
            Page.Title = GetLabel("checkin")
            cmdSave.Text = GetLabel("save")
            cmdCancel.Text = GetLabel("cancel")
            lblComment.Text = GetLabel("addcomment") & ":"

            plhHeader.Visible = Not CType(Master, ToolWindow).Modal

            Dim obj As Document = Document.GetDocument(_ObjectID)
            If Not obj.CanCheckIn() Then
                Dim sb As New System.Text.StringBuilder
                sb.AppendLine("ShowMessage('" & obj.GetLastError.Description & "','error');")
                sb.AppendLine("Close();")

                Page.ClientScript.RegisterStartupScript(GetType(String), "closescript", sb.ToString, True)
            Else

                rdlAction.Items.Clear()
                Dim itm As WebControls.ListItem
                rdlAction.Items.Add(New WebControls.ListItem(GetLabel("cancelcheckout"), "0", True))
                rdlAction.Items.Add(New WebControls.ListItem(GetLabel("savenewversion"), "1", True))
                itm = New WebControls.ListItem(GetLabel("savenewsubversion"), "2", True)
                itm.Selected = True
                rdlAction.Items.Add(itm)
                If obj.CanOverwriteCurrentVersion Then
                    rdlAction.Items.Add(New WebControls.ListItem(GetLabel("overwriteversion"), "3", True))
                End If
                chkCheckOut.Enabled = True
                chkCheckOut.Text = GetLabel("keepcheckedout")

            End If
        End If
    End Sub


    Protected Sub rdlAction_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs) Handles rdlAction.SelectedIndexChanged

        Dim objAction As Action = CType(rdlAction.SelectedValue, Action)

        If Not objAction = Action.CANCEL Then
            chkCheckOut.Enabled = True
        Else
            chkCheckOut.Checked = False
            chkCheckOut.Enabled = False
        End If


    End Sub

    Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSave.Click

        Dim objAction As Action = CType(rdlAction.SelectedValue, Action)
        Dim bKeepCheckedOut As Boolean = chkCheckOut.Checked
        Dim lsComment As String = txtCheckInComment.Text
        Dim doc As Document = Document.GetDocument(_ObjectID)
        Select Case objAction
            Case Action.CANCEL
                doc = doc.CancelCheckout()
                bKeepCheckedOut = False
            Case Action.NEWMAJOR
                doc = doc.CheckIn(True, False, lsComment)
            Case Action.NEWMINOR
                doc = doc.CheckIn(False, True, lsComment)
            Case Action.OVERWRITE
                doc = doc.CheckIn(False, False, lsComment)
        End Select

        Dim sb As New System.Text.StringBuilder

        If Not doc.HasError Then
            If bKeepCheckedOut Then
                doc = doc.CheckOut()
            End If

            sb.AppendLine("if(MainPage().document.location.href.indexOf('dm_detail.aspx') > 0)")
            sb.AppendLine("{")
            sb.AppendLine("MainPage().document.location.href='dm_detail.aspx?DM_OBJECT_ID= " & doc.ID & "&catid=" & _CatID & "&folderid=" & _FolderID & "&parentreload=true';")
            sb.AppendLine("}")
            sb.AppendLine("else")
            sb.AppendLine("{")
            sb.AppendLine("MainPage().ReloadParent();")
            sb.AppendLine("}")

            sb.AppendLine("Close();")
        Else
            sb.AppendLine("ShowMessage('" & doc.GetLastError().Description & "','error');")
        End If

        Page.ClientScript.RegisterStartupScript(GetType(String), "closescript", sb.ToString, True)
    End Sub
End Class
