Imports System.Activities.Expressions
Imports Arco.Doma.Library.Routing

Partial Class UserControls_Notes
    Inherits BaseUserControl


    Public Property CanView As Boolean = True
    Public Property CanDeleteAdmin As Boolean = True
    Public Property Height As Int32

    Protected ReadOnly Property ExtraStyle As String
        Get
            If Height > 0 Then
                Return "width:100%;height:" & Height & "px;overflow:auto;zoom:1;clear:both"
            End If

            Return "width:100%;clear:both"
        End Get
    End Property
    Public Property ShowQuickAdd As Boolean = True
    Public Property CanDelete As Boolean = True
    Public Property CanAdd As Boolean = True
    Public Property CanEdit As Boolean = True
    Public Property Count As Integer

    Public Property Case_ID() As Integer
        Get
            Return Convert.ToInt32(ViewState("CASE_ID"))
        End Get
        Set(ByVal value As Integer)
            ViewState("CASE_ID") = value
        End Set
    End Property
    Public Property Object_Type() As String

    'Protected Function NoteInfo(ByVal voFileInfo As Arco.Doma.Library.FileList.FileInfo) As String
    '    Dim sb As New StringBuilder

    '    If voFileInfo.Scope = Arco.Doma.Library.Scope.Private Then
    '        sb.Append("<span class='icon icon-user-profile icon-color-light' style='font-size:15px' title='" & GetLabel("private") & "'></span>")
    '    End If
    '    sb.Append(ArcoFormatting.FormatUserName(voFileInfo.FILE_CREATIONUSER))
    '    sb.Append(" - ")
    '    sb.Append(ArcoFormatting.FormatDateLabel(voFileInfo.FILE_INDEXDATE, True, False))
    '    If voFileInfo.FILE_MODIFDATE.Subtract(voFileInfo.FILE_INDEXDATE).Minutes > 0 Then
    '        If voFileInfo.FILE_CREATIONUSER <> voFileInfo.FILE_MODIFUSER Then
    '            sb.Append("<br/>")
    '            sb.Append(GetLabel("modifby"))
    '            sb.Append(" ")
    '            sb.Append(ArcoFormatting.FormatDateLabel(voFileInfo.FILE_MODIFUSER, True, False))
    '        End If
    '        sb.Append("<br/>")
    '        sb.Append(GetLabel("modifdate"))
    '        sb.Append(" ")
    '        sb.Append(ArcoFormatting.FormatDateLabel(voFileInfo.FILE_MODIFDATE, True, False))
    '    End If

    '    Return sb.ToString
    'End Function


    Protected Sub notes_ItemDataBound(ByVal Sender As Object, ByVal e As RepeaterItemEventArgs)
        If (e.Item.ItemType = ListItemType.Item) OrElse
                    (e.Item.ItemType = ListItemType.AlternatingItem) Then

            Dim lsText As String = ""
            Dim item As NoteList.NoteInfo = CType(e.Item.DataItem, NoteList.NoteInfo)

            Try
                Dim lbCanEdit As Boolean = CanEdit
                Dim lbCanDelete As Boolean = CanDelete


                '  If loF.FILE_CREATIONUSER = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
                'his own comments
                If lbCanEdit Or lbCanDelete Then
                    CType(e.Item.FindControl("pnlEditGrid"), PlaceHolder).Visible = True
                    CType(e.Item.FindControl("plhEditButton"), PlaceHolder).Visible = False 'lbCanEdit 'not atm
                    CType(e.Item.FindControl("plhDeleteButton"), PlaceHolder).Visible = lbCanDelete
                Else
                    CType(e.Item.FindControl("pnlEditGrid"), PlaceHolder).Visible = False
                End If
                'Else
                ''someone elses comments or mailnotes
                'If CanAdminDelete Then
                '    CType(e.Item.FindControl("pnlEditGrid"), PlaceHolder).Visible = True
                '    CType(e.Item.FindControl("plhEditButton"), PlaceHolder).Visible = False
                '    CType(e.Item.FindControl("plhDeleteButton"), PlaceHolder).Visible = True
                'Else
                '    CType(e.Item.FindControl("pnlEditGrid"), PlaceHolder).Visible = False
                'End If
                'End If
            Catch ex As Exception
                'Do nothing...just prevent error from being displayed when comment file is not processed by the indexer yet
            End Try
            If item.IsHtml Then
                CType(e.Item.FindControl("lblContentGrid"), Label).Text = HtmlSanitizer.Sanitize(item.NoteText)
            Else
                CType(e.Item.FindControl("lblContentGrid"), Label).Text = Server.HtmlEncode(item.NoteText).Replace(Environment.NewLine, "<br/>")
            End If
        End If


    End Sub

    Private Sub AddScript()
        Dim sb As New StringBuilder

        sb.Append("function NewNote() {")

        sb.Append("  var win = PC().OpenModalWindow('" & ResolveClientUrl("~/UserControls/PromptText.aspx?modal=Y&Header=" & GetLabel("add")) & "&x=' + Math.random(), true, 700, 300);")
        sb.Append("win.add_close(NewNoteCallBack);}")

        sb.Append("function NewNoteCallBack(win)")
        sb.AppendLine("{if (win.argument){PC().AddCaseNote(" & Case_ID & ",win.argument);}")
        sb.Append("win.remove_close(NewNoteCallBack);")
        sb.AppendLine("}")


        sb.AppendLine("function ReloadParent()")
        sb.AppendLine("{")
        sb.AppendLine("PC().Reload();")
        sb.AppendLine("}")
        ScriptManager.RegisterStartupScript(Me.Page, Me.GetType, "initcomments", sb.ToString, True)
    End Sub
    Public Overrides Sub DataBind()
        Dim c As cCase = cCase.GetCaseByCaseID(Case_ID)
        If c IsNot Nothing Then
            DataBind(c)
        Else
            DataBind(HistoryCase.GetHistoryCase(Case_ID))
        End If

    End Sub
    Public Overloads Sub DataBind(ByVal voCase As cCase)
        Case_ID = voCase.Case_ID

        If Not voCase.IsVirtual Then


            Dim lbEdit As Boolean = voCase.CaseData.CanModifyMeta
            If CanEdit Then
                CanEdit = lbEdit
            End If
            If CanAdd Then
                CanAdd = lbEdit
            End If
            If CanDelete Then
                CanDelete = lbEdit
            End If
            If Not lbEdit Then
                CanView = voCase.CurrentUserHasWork OrElse voCase.CanView
            Else
                CanView = True
            End If
        Else

            CanView = False
            CanAdd = False
        End If

        BindForm()
    End Sub

    Public Overloads Sub DataBind(ByVal voCase As HistoryCase)
        Case_ID = voCase.Case_ID
        CanEdit = False
        CanAdd = False
        CanDelete = False
        CanView = voCase.CanViewMeta

        BindForm()
    End Sub

    Private Sub BindForm()
        If CanView Then
            AddScript()
            NotesList.DataSourceID = "notesListDataSource"
            notesListDataSource.SelectParameters.Item("caseId").DefaultValue = Case_ID
            NotesList.DataBind()
            plhNoResults.Visible = (NotesList.Items.Count = 0)
            Count = NotesList.Items.Count
        Else
            Visible = False
        End If

        If CanAdd Then
            If ShowQuickAdd Then
                lnkQuickAdd.Text = GetLabel("add")
                pnlQuickAdd.Visible = True
            Else
                plhFullAdd.Visible = False 'True
                pnlQuickAdd.Visible = False
                litFullAdd.Text = GetLabel("add")
            End If
        Else
            pnlQuickAdd.Visible = False
        End If
    End Sub

    Protected Sub lnkQuickAdd_Click(ByVal sender As Object, ByVal e As EventArgs) Handles lnkQuickAdd.Click
        If CanAdd AndAlso Not String.IsNullOrEmpty(txtQuickAdd.Text) Then

            Dim loNote As Arco.Doma.Library.Routing.Note = Arco.Doma.Library.Routing.Note.NewCaseNote(Case_ID)
            loNote.NoteText = txtQuickAdd.Content
            loNote.IsHtml = True
            loNote = loNote.Save
            txtQuickAdd.Content = ""
            NotesList.DataBind()
        End If
    End Sub

End Class
