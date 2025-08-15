Imports Arco.Doma.Library

Partial Class DM_Catalog_NewItem
    Inherits System.Web.UI.Page

    Private moList As Lists.List = Lists.List.NewList()

    Private Const DENIED_PAGE As String = "../DM_ACL_DENIED.aspx"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not IsPostBack Then
            If ViewState("list_id") = Nothing Then ViewState("list_id") = Request.QueryString("id")
        End If

        If Val(ViewState("list_id")) > 0 Then
            If GetList(Val(ViewState("list_id"))) = True Then
                If Not IsPostBack Then
                    ' Insert mode.
                    txtTitle.Text = "INSERT ITEM"
                    txtID.Value = 0
                    txtCode.Text = ""
                    txtDesc.Text = ""
                    txtDescHomo.Text = ""
                    txtScopeNote.Text = ""
                End If
                If moList.LIST_TYPE = "THESAURUS" Then
                    lblCode.Visible = False
                    txtCode.Visible = False
                End If
            Else
                Response.Redirect(DENIED_PAGE)
            End If
        Else
            Response.Redirect(DENIED_PAGE)
        End If

    End Sub

    Private Function GetList(ByVal vlListID As Integer) As Boolean

        Dim lbOK As Boolean = False

        If vlListID > 0 Then
            moList = Lists.List.GetList(vlListID)
            If moList.ID = vlListID Then
                lblList.Text = moList.LIST_TYPE & " - " & moList.Name
                If moList.LIST_ALLOWNEW <> Lists.List.ListAllowNew.NotAllowed Then
                    lbOK = True
                End If
            End If
        End If

        Return lbOK

    End Function

    Private Sub RefreshLeftPane()

        Dim sb As New StringBuilder

        sb.AppendLine("parent.RefreshLeftPane()")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "refresh", sb.ToString, True)

    End Sub

    Protected Function GetLabel(ByVal vsLabel As String) As String
        Return Arco.Web.ResourceManager.GetString(vsLabel.ToLower, Arco.Security.BusinessIdentity.CurrentIdentity.Language)
    End Function

    Protected Sub DoSave(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim lbProceed As Boolean = True
        Dim lbNotUnique, lbHomonymIsMissing, lbLeadTermAlreadyExists As Boolean
        Dim loItem As Lists.ListItem

        If Val(txtID.Value) = 0 Then
            loItem = Lists.ListItem.NewListItem(moList.ID)
        Else
            loItem = Lists.ListItem.GetListItem(Convert.ToInt32(txtID.Value))
        End If
        If txtDesc.Text.Trim.Length = 0 Then
            ShowError("Enter description.")
        ElseIf (moList.LIST_TYPE = "CONCEPTTREE" Or _
                moList.LIST_TYPE = "CODELIST" Or _
                moList.LIST_TYPE = "UDCLIST") And txtCode.Text.Trim.Length = 0 Then
            ShowError("Enter code.")
        Else
            If moList.LIST_TYPE = "CONCEPTTREE" Or _
               moList.LIST_TYPE = "CODELIST" Or _
               moList.LIST_TYPE = "UDCLIST" Then
                lbProceed = Lists.ListItem.IsCodeUnique(loItem.ID, loItem.LIST_ID, txtCode.Text)
                If lbProceed = False Then
                    ShowError("Code is not unique.")
                End If
            End If
            If lbProceed Then
                lbProceed = Lists.ListItem.IsDescriptionValid(loItem.ID, loItem.LIST_ID, 0, True, txtDesc.Text, txtDescHomo.Text, lbNotUnique, lbHomonymIsMissing, lbLeadTermAlreadyExists)
                If lbProceed Then
                    Try
                        loItem.ITEM_CODE = txtCode.Text
                        loItem.Description = txtDesc.Text
                        loItem.ITEM_DESC_HOMO = txtDescHomo.Text
                        loItem.ITEM_SCOPE_NOTE = txtScopeNote.Text
                        If moList.LIST_TYPE = "UDCLIST" Then
                            loItem.ITEM_ROOT_NODE = (txtCode.Text.IndexOf(".") = -1)
                        Else
                            loItem.ITEM_ROOT_NODE = True
                        End If
                        loItem.ITEM_TYPE = If(txtDescHomo.Text.Length > 0, "HOMO", "")
                        loItem.ITEM_STATUS = Lists.ListItem.ITEMSTATUS.Production
                        loItem = loItem.Save() ' Triggers AfterSaveHandling in ListItem.
                        txtID.Value = loItem.ID
                        RefreshLeftPane()
                        Table1.Visible = False

                    Catch ex As Exception
                        ShowError(ex.ToString)
                    End Try
                Else
                    If lbNotUnique Then
                        ShowError("Description is not unique.")
                    ElseIf lbHomonymIsMissing Then
                        ShowError("Homonym is missing.")
                    ElseIf lbLeadTermAlreadyExists Then
                        ShowError("Description already exists without hymonym.")
                    End If
                End If
            End If
        End If

    End Sub

#Region " Error handling "

    Private Sub ShowError(ByVal vsText As String)
        msg_lbl.Text = vsText
        MsgPanel.Visible = True
    End Sub

    Protected Sub Msg_button_Click(ByVal sender As Object, ByVal e As EventArgs)
        MsgPanel.Visible = False
    End Sub

#End Region

End Class
