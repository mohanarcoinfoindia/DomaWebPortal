Imports Arco.Doma.CMS.Data
Imports Arco.Doma.CMS.Layouts
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Routing

Partial Class CMS_EditPage
    Inherits BaseAdminOnlyPage

    Private PageID As Integer

    Protected Overrides ReadOnly Property ErrorPageUrl As String
        Get
            Return "../DM_ACL_DENIED.aspx"
        End Get
    End Property
    Protected Sub CMS_EditPage_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Me.Title = GetLabel("editpage")

        lblName.Text = GetLabel("name")
        lblTtitle.Text = GetLabel("title")
        lblDesc.Text = GetLabel("description")
        lblFolder.Text = GetLabel("folder")
        PageID = QueryStringParser.GetInt("pageid")

        If Not Page.IsPostBack Then
            If PageID > 0 Then
                Dim loPage As Arco.Doma.CMS.Data.Page = Arco.Doma.CMS.Data.Page.GetPage(PageID)
                If loPage.ID = PageID Then
                    selectedLayout.Value = loPage.LayoutID.ToString
                    txtName.Text = loPage.Name
                    txtDesc.Text = loPage.Description
                    txtFolder.Text = loPage.Folder
                    txtTitle.SetValue(loPage.Title)
                Else
                    Response.Write("Page not found")
                    Response.End()
                End If
            Else
                selectedLayout.Value = "1"
            End If
            Dim layoutList As PageLayoutList = PageLayoutList.GetLayoutList
            repLayouts.DataSource = layoutList
            repLayouts.DataBind()
        End If

        If PageID <> 0 Then
            pnlAcl.Visible = True
            LoadAcl()
        Else
            pnlAcl.Visible = False
        End If


    End Sub


    Protected Sub btnSave_Click(sender As Object, e As System.EventArgs) Handles btnSave.Click


        Dim loPage As Arco.Doma.CMS.Data.Page
        If PageID > 0 Then
            loPage = Arco.Doma.CMS.Data.Page.GetPage(PageID)
        Else
            loPage = Arco.Doma.CMS.Data.Page.NewPage
        End If
        loPage.Name = txtName.Text
        loPage.Description = txtDesc.Text
        loPage.Folder = txtFolder.Text
        loPage.LayoutID = Convert.ToInt32(selectedLayout.Value)
        loPage.Title = txtTitle.GetValue()
        loPage = loPage.Save

        If PageID = 0 Then
            Dim sec As RoutingSecurity = RoutingSecurity.NewRoutingSecurity()
            sec.Object_ID = loPage.ID
            sec.Object_Type = "Page"
            sec.Subject = Arco.Security.Constants.RoleEveryone
            sec.SubjectType = Arco.Security.Constants.Role
            sec.Level = RoutingSecurity.LevelAccess.View
            sec.Save()
        End If

        Response.Redirect("~/Page.aspx?pageid=" & loPage.ID & "&mode=1")
        'Page.ClientScript.RegisterStartupScript(Me.GetType, "Close", "Close();", True)
    End Sub
    Protected Sub btnCancel_Click(sender As Object, e As System.EventArgs) Handles btnCancel.Click



        If PageID > 0 Then
            Response.Redirect("~/Page.aspx?pageid=" & PageID & "&mode=1")
        Else
            Response.Redirect("~/Pages.aspx")
        End If

    End Sub
    Protected Sub repLayouts_ItemDataBound(ByVal sender As Object, ByVal e As RepeaterItemEventArgs) Handles repLayouts.ItemDataBound
        If e.Item.DataItem IsNot Nothing Then
            Dim layoutInfo As PageLayoutList.PageLayoutInfo = CType(e.Item.DataItem, PageLayoutList.PageLayoutInfo)
            Dim layout As IPageLayout = Arco.ApplicationServer.Library.Shared.PluginManager.CreateInstance(Of IPageLayout)(layoutInfo.Assembly, layoutInfo.Class)
            Dim thumbnail = layout.CreateThumbnail()
            thumbnail.Attributes.Add("onclick", "selectLayout(this, " + layoutInfo.ID.ToString + ");")
            thumbnail.Attributes.Add("title", GetDecodedLabel(layoutInfo.Name.Replace(" ", "")))
            If layoutInfo.ID.ToString() = selectedLayout.Value Then
                thumbnail.CssClass += " selected"
            End If
            e.Item.Controls.Add(thumbnail)
        End If
    End Sub

    Private Sub LoadAcl()

        If Page.IsPostBack Then
            If Not String.IsNullOrEmpty(aclSubjectToDelete.Value) Then
                RoutingSecurity.DeleteRoutingSecurity(PageID, "Page", aclSubjectToDelete.Value, aclSubjectTypeToDelete.Value)
                aclSubjectToDelete.Value = ""
                aclSubjectTypeToDelete.Value = ""
            End If
        End If
        Dim dt As New DataTable()
        dt.Clear()
        dt.Columns.Add("TYPE")
        dt.Columns.Add("NAME")
        dt.Columns.Add("CAPTION")
        dt.Columns.Add("DESCRIPTION")
        For Each loAcl As RoutingSecurityList.SecurityInfo In RoutingSecurityList.GetSecurityList(PageID, "Page")
            Dim lsCaption As String = FormatSubject(loAcl.Subject, loAcl.SubjectType)
            dt.Rows.Add(New String() {loAcl.SubjectType, loAcl.Subject, lsCaption, Server.HtmlEncode(loAcl.GetSubjectDescription)})
        Next
        If dt.Rows.Count = 0 Then
            trNoAclFound.Visible = True
            lblNoAclFound.Text = GetLabel("noresultsfound")
        Else
            trNoAclFound.Visible = False
        End If

        repSec.DataSource = dt
        repSec.DataBind()

        Dim sb As New StringBuilder

        sb.AppendLine(" function AddAcl(){ const left = window.top.outerWidth / 2 + window.top.screenX - ( 950 / 2); const top = window.top.outerHeight / 2 + window.top.screenY - ( 600 / 2);") 'line added to center popup.
        sb.AppendLine("var w = window.open(" & EncodingUtils.EncodeJsString("AddPageAcl.aspx?pageid=" & PageID) & ", 'PageAcl','width=950,height=600,resizable=yes,scrollbars=yes,status=yes,top=' + top + ',left=' + left);}")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "AddAcl", sb.ToString, True)

    End Sub
    Protected Function FormatSubject(ByVal vsName As String, ByVal vsType As String) As String
        Select Case vsType
            Case "User"
                Return ArcoFormatting.FormatUserName(vsName)
            Case Else
                Return Server.HtmlEncode(vsName)
        End Select
    End Function
    Protected Function GetRoleID(ByVal name As String) As String
        Try
            Return Role.GetRole(name).ID.ToString
        Catch ex As NullReferenceException
            Return ""
        End Try

    End Function
End Class
