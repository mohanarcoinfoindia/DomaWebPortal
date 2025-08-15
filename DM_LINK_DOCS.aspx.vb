Imports Arco.Doma.Library
Imports Arco.Doma.Library.Actions
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Globalisation

Partial Class DM_LINK_DOCS
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim targetObject As DM_OBJECT = ObjectRepository.GetObject(QueryStringParser.GetInt("OIP"))
        If targetObject Is Nothing OrElse Not targetObject.CanViewMeta() Then
            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
            Exit Sub
        End If

        If Not Page.IsPostBack Then
            Page.Title = GetLabel("linkselection")
            lblHeader.Text = GetLabel("linkselection")
            lblLink.Text = GetLabel("link")
            lblAs.Text = GetLabel("as")
            lblTo.Text = GetLabel("to")
            plhHeader.Visible = Not CType(Master, ToolWindow).Modal

            Dim lsLinks As String = ""
            Dim lcolLabels As LABELList = LABELList.GetRelationTypesLabelList(EnableIISCaching)
            For Each loRelType As OBJECT_RELATION_TYPE_LIST.OBJECT_RELATION_TYPEInfo In OBJECT_RELATION_TYPE_LIST.GetOBJECT_RELATION_TYPE_List()
                Dim lsCaption As String = lcolLabels.GetObjectLabel(loRelType.ID, "Relation", Arco.Security.BusinessIdentity.CurrentIdentity.Language, loRelType.Name)
                lsLinks &= "<a href='javascript:LinkAs(" & loRelType.ID & ");' class='ButtonLink'>" & lsCaption & "</a> <br />"
            Next
            lblLinks.Text = lsLinks

            lblTargetDoc.Text = targetObject.Name & "<br/>"

            Dim lbFound As Boolean = False
            Dim lsMsg As String = ""
            For Each sel As DMSelectionList.SelectionInfo In Selection.GetCurrent
                If sel.Object_ID <> targetObject.ID Then
                    lblSourceDocs.Text &= sel.GetBusinessObject.Name & "<br/>"
                    lbFound = True
                Else
                    lsMsg = LibError.GetErrorDescription(LibError.ErrorCode.ERR_INVALIDOPERATION)
                End If
            Next
            If Not lbFound Then
                Dim sb As New StringBuilder
                If Not String.IsNullOrEmpty(lsMsg) Then
                    sb.AppendLine("ShowMessage(" & EncodingUtils.EncodeJsString(lsMsg) & ",""error"");")
                End If
                sb.AppendLine("Close();")
                Page.ClientScript.RegisterStartupScript(GetType(String), "closescript", sb.ToString, True)
            End If
        End If

        If Not String.IsNullOrEmpty(txtToBeLinkedAs.Value) And txtToBeLinkedAs.Value <> "0" Then
            pnlSelType.Visible = False
            pnlSelMsg.Visible = True

            Selection.SetOIP(targetObject)

            Dim loAction As New ObjectListAction(ObjectListAction.eActionType.LinkDocuments, DMSelection.SelectionType.Current, CType(txtToBeLinkedAs.Value, Int32))
            loAction = loAction.Execute()
            If Not loAction.Result.Any(Function(x) x.ResultAction = Website.IUserEvent.eResultAction.Message) Then
                lblMsg.Text = GetLabel("linkingok")
            Else
                lblMsg.Text = GetLabel("linkingnotallok")
            End If

        Else
            pnlSelType.Visible = True
            pnlSelMsg.Visible = False
        End If
    End Sub
End Class
