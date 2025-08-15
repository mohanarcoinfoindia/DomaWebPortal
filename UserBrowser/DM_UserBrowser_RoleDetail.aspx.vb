Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL

Partial Class DM_UserBrowser_RoleDetail
    Inherits UserBrowserBasePage
#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Public Property UpdateModus As Boolean
        Get
            Dim o As Object = ViewState("updatemodus")
            If o Is Nothing Then
                Return False
            End If
            Return CType(o, Boolean)
        End Get
        Set(value As Boolean)
            ViewState("updatemodus") = value
        End Set
    End Property
    Private _roleLocked As Boolean
    Private _roleTenantId As Integer
    Protected roleid As Integer
    Protected parentroleid As Integer  
    Protected structured As Boolean

    Public Property IsSystemRole As Boolean
    Public Property CanEditRole As Boolean     
    Public Property CanEditMembers As Boolean

    Public Property MultiTenant As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        CanEditRole = isAdmin()
        MultiTenant = Settings.GetValue("General", "MultiTenant", False)

        scriptManager.Scripts.Add(New ScriptReference("~/Resources/" & Language & "/Messages.js"))

        loadLabels()


        If IsNumeric(Request.QueryString("login")) Then
            roleid = Request.QueryString("login")
        ElseIf ViewState("roleid") <> Nothing Then
            roleid = ViewState("roleid")
        End If

        If Request.QueryString("structured") <> Nothing Then
            structured = Request.QueryString("structured")
        End If

        If IsNumeric(Request.QueryString("parent")) Then
            parentroleid = Request.QueryString("parent")
        End If

        Dim liTabID As Integer = 0
        If IsNumeric(Request.QueryString("radtab")) Then
            liTabID = Request.QueryString("radtab")
        End If

        If Not Page.IsPostBack Then
            If liTabID <> 0 Then
                RadMultiPage1.SelectedIndex = liTabID
                radTab.SelectedIndex = liTabID
            Else
                RadMultiPage1.SelectedIndex = 0
            End If
            LoadTenants()
            If roleid > 0 Then
                LoadRole()
                If roleid > 0 Then UpdateModus = True
            End If

            If roleid = 0 Then
                If structured Then
                    chkStructure.Checked = True
                    If parentroleid > 0 Then
                        RenderHierarchy(parentroleid, True)
                    Else
                        lblRoleHierarchy.Text = "\"
                    End If
                Else
                    chkStructure.Checked = False
                    rowParentRole.Visible = False
                End If
                chkStructure.Enabled = False
            End If

            checkFocus()
        Else
            'postback
            If roleid > 0 Then
                LoadRoleFlags()
            End If
        End If

        ViewState("roleid") = roleid

        InitJS()

        If CanEditRole Then
            CanEditMembers = Not _roleLocked
            'onyl allow to edit the meta of your own role
            If roleid <> 0 Then
                CanEditRole = (Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal OrElse _roleTenantId = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.Id)
            End If

        Else
            If roleid <> 0 Then
                CanEditMembers = Not _roleLocked AndAlso (Routing.RoutingSecurityList.GetRoleSecurityList(roleid).FilterForCurrentUser.Any)
            End If
        End If

        SetLabelsLink()
        LoadTabs()

        SetDetailFormMode()

        If UpdateModus Then

            divMembers.Visible = True
            divMemberOf.Visible = CanEditRole
            divSecurity.Visible = CanEditRole AndAlso Not IsSystemRole
            divAudit.Visible = CanEditRole


            lnkTransferWork.Visible = CanEditRole
            lnkDelete.Visible = CanEditRole AndAlso Not IsSystemRole
        Else
            lnkTransferWork.Visible = False
            lnkDelete.Visible = False
        End If

    End Sub


    Protected Function GetTenantLabel(ByVal id As Integer) As String
        If id = 0 Then Return GetLabel("all")
        Return GetTenantName(id)
      
    End Function

    Private Sub LoadTenants()
        If Not Page.IsPostBack AndAlso MultiTenant Then
            If Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal Then
                drpTenant.Items.Add(New System.Web.UI.WebControls.ListItem("Global", "0"))
                For Each tn As TenantList.TenantInfo In Tenants
                    drpTenant.Items.Add(New System.Web.UI.WebControls.ListItem(tn.Name, tn.Id.ToString()))
                Next
            Else
                drpTenant.Items.Add(New System.Web.UI.WebControls.ListItem(Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.Name, Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.Id.ToString))
            End If

            If drpTenant.Items.Count > 1 Then
                rowTenant.Visible = True
            End If
        End If
    End Sub
    Private Sub LoadTabs()
        If CanEditRole Then
            If roleid > 0 Then
                radTab.Tabs(TabPages.ACL).Visible = Not IsSystemRole
                LoadMembers()
                LoadAcl()
                LoadAudit()
            Else
                radTab.Tabs(TabPages.Members).Visible = False
                radTab.Tabs(TabPages.MemberOf).Visible = False
                radTab.Tabs(TabPages.ACL).Visible = False
                radTab.Tabs(TabPages.Audit).Visible = False
                radTab.Style.Add("border-style", "none !important")
            End If
        Else
            LoadMembers() 'show members
            radTab.Tabs(TabPages.MemberOf).Visible = False
            radTab.Tabs(TabPages.ACL).Visible = False
            radTab.Tabs(TabPages.Audit).Visible = False
        End If
    End Sub

    Public Enum TabPages
        Members = 0
        MemberOf = 1
        ACL = 2
        Audit = 3
    End Enum

    Private Sub SetDetailFormMode()

        Dim mode As Boolean = CanEditRole AndAlso Not IsSystemRole      
        txtRoleName.Enabled = mode
        txtRoleDescription.Enabled = mode
        chkStructure.Enabled = mode
        lnkSave.Visible = mode
    End Sub
    Private Sub SetInfoMessage(ByVal t As String)
        lblMsgText.Visible = True
        lblMsgText.Text = t
        lblMsgText.CssClass = "InfoLabel"
    End Sub
    Private Sub SetErrorMessage(ByVal t As String)
        lblMsgText.Visible = True
        lblMsgText.Text = t
        lblMsgText.CssClass = "ErrorLabel"
    End Sub
    Protected Sub SaveRole(ByVal sender As Object, ByVal e As EventArgs)

        Try
            If Not Page.IsValid Then
                Exit Sub
            End If
            Dim luRole As Role
            Dim lbIsNew As Boolean = False
            If roleid = 0 Then
                If MultiTenant Then
                    luRole = Role.NewRole(Convert.ToInt32(drpTenant.SelectedValue))
                Else
                    luRole = Role.NewRole()
                End If
                lbIsNew = True
            Else
                luRole = Role.GetRole(roleid)
                If luRole Is Nothing Then
                    SetErrorMessage("Role not found")
                    Exit Sub
                End If
            End If

            If Rules.isReservedName(txtRoleName.Text) Then
                SetErrorMessage(txtRoleName.Text & " " & GetLabel("isreservedword"))
                Exit Sub
            End If
            Dim invalidChar As String = Nothing
            If Rules.ContainsInvalidCharacters(txtRoleName.Text, invalidChar) Then
                SetErrorMessage(Server.HtmlEncode(txtRoleName.Text) & " " & GetLabel("containsinvalidchars") & ": " & Server.HtmlEncode(invalidChar))
                Exit Sub
            End If

            luRole.Name = txtRoleName.Text.Trim
            txtRoleName.Text = luRole.Name
            luRole.Description = txtRoleDescription.Text
            luRole.Structured = chkStructure.Checked
            Dim structuredChanged As Boolean = luRole.isChanged("ROLE_STRUCTURED")
            luRole = luRole.Save()

            SetFlagsFromRole(luRole)

            UpdateModus = True
            checkFocus()
            If lbIsNew Then
                If luRole.Structured Then
                    If parentroleid > 0 Then
                        RoleLink.CreateLink(Convert.ToInt32(parentroleid), luRole.ID)
                    End If
                End If
            Else
                If structuredChanged Then
                    If luRole.Structured Then
                        'structured turned on
                    Else
                        'structured turned off
                        RoleLink.DeleteLink(0, luRole.ID)
                        RoleLink.DeleteLink(luRole.ID, 0)
                        lblRoleHierarchy.Text = ""
                    End If
                End If
            End If

            roleid = luRole.ID


            RefreshPanes(lbIsNew, luRole)

            If Not lbIsNew Then

                ViewState("roleid") = roleid

                divMembers.Visible = True
                divMemberOf.Visible = True
                divSecurity.Visible = Not IsSystemRole
                divAudit.Visible = True

                LoadTabs()
                SetInfoMessage(GetLabel("saveok"))

                lnkTransferWork.Visible = isAdmin()
            End If

        Catch rolexists As Role.RoleAlreadyExistsException
            SetErrorMessage(GetLabel("roleexists"))
            UpdateModus = False
            lnkTransferWork.Visible = False
        Catch ex As Exception
            SetErrorMessage("Error during the save: " & ex.ToString)
            UpdateModus = False
            lnkTransferWork.Visible = False
        End Try
        checkFocus()
        SetLabelsLink()
    End Sub
    Private Function GetRoleStructureString(ByVal childroleid As Int32) As String
        Dim idList As List(Of Int32) = New List(Of Int32)
        Dim i As Int32 = childroleid
        While i <> 0
            Dim currentRoleId As Int32 = i
            idList.Add(currentRoleId)
            i = 0
            For Each link As RoleLinkList.ROLELinkInfo In RoleLinkList.GetRoleLinkList(0, currentRoleId)
                i = link.PARENT_ROLE_ID
                Exit For
            Next
        End While

        idList.Reverse()
        Return String.Join(",", idList)
    End Function
    Private Sub RefreshPanes(ByVal vbIsNew As Boolean, ByVal role As Role)
        Dim sb As New StringBuilder
        If chkStructure.Checked Then
            sb.AppendLine("parent.ShowLeftPane('DM_UserBrowser_LeftPane.aspx?selected=" & GetRoleStructureString(role.ID) & "');")
        End If
        If vbIsNew Then sb.AppendLine("location.href = 'DM_Userbrowser_RoleDetail.aspx?new=false&login=" & roleid & "';")
        If sb.Length > 0 Then Page.ClientScript.RegisterClientScriptBlock(GetType(String), "refresh", sb.ToString, True)
    End Sub
    Private Sub LoadLabels()
        lblRoleName.Text = GetLabel("ub_rolename")
        lblRoleDescription.Text = GetLabel("ub_roledesc")
        lblParentStructureRole.Text = GetLabel("rolestructure")
        chkStructure.Text = GetLabel("structuredrole")
        chkLocked.Text = GetLabel("locked")
        lnkTransferWork.Text = GetLabel("Transfer Work")
        btnClose.Text = GetLabel("close")
        lnkSave.Text = GetLabel("save")
        lnkDelete.Text = GetLabel("delete")
        radTab.Tabs(TabPages.Members).Text = GetDecodedLabel("members")
        radTab.Tabs(TabPages.MemberOf).Text = GetDecodedLabel("memberof")
        radTab.Tabs(TabPages.ACL).Text = GetDecodedLabel("ctx_aclsetacl")
        radTab.Tabs(TabPages.Audit).Text = "Audit"
        lblTenant.Text = "Tenant"
    End Sub
    Private Sub SetLabelsLink()
        If roleid > 0 AndAlso CanEditRole AndAlso Not IsSystemRole Then
            lnkLabels.Visible = True
            lnkLabels.NavigateUrl = "javascript:EditLabels(" & roleid & ",'Role');"

            lnkDescLabels.Visible = True
            lnkDescLabels.NavigateUrl = "javascript:EditLabels(" & roleid & ",'RoleDesc');"
        Else
            lnkLabels.Visible = False
            lnkDescLabels.Visible = False
        End If
    End Sub
    Private Sub InitJS()

        Dim sb As New StringBuilder

        sb.AppendLine("function TransferWork() {")
        sb.AppendLine("var w = window.open(" & EncodingUtils.EncodeJsString("TransferWork.aspx?subject_type=Role&subject_id=" & Server.UrlEncode(txtRoleName.Text)) & ", 'TransferWork','width=800,height=600,resizable=yes,scrollbars=yes,status=yes');")
        sb.AppendLine("}")

        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "Checks", sb.ToString, True)

        RegisterTabScript()
    End Sub
    Private Sub checkFocus()
        If UpdateModus Then
            If CanEditRole AndAlso Settings.GetValue("Security", "AllowRenamingRoles", True) Then
                SetFocus("txtRoleName")

                txtRoleName.ReadOnly = False
                txtRoleName.BackColor = Color.White
            Else
                SetFocus("txtRoleDescription")
                txtRoleName.ReadOnly = True
                txtRoleName.BackColor = Color.LightGray
            End If
        Else
            SetFocus("txtRoleName")
            txtRoleName.ReadOnly = False
            txtRoleName.BackColor = Color.White
        End If
    End Sub
    Private Sub LoadRole()
        Dim lrRole As Role = Role.GetRole(roleid)
        If lrRole IsNot Nothing Then
            txtRoleDescription.Text = lrRole.Description
            txtRoleName.Text = lrRole.Name

            drpTenant.SelectedValue = lrRole.TenantId.ToString

            SetFlagsFromRole(lrRole)

            'allow change
            drpTenant.Enabled = False
            chkStructure.Checked = lrRole.Structured
            chkLocked.Checked = lrRole.Locked

            If chkStructure.Checked Then
                RenderHierarchy(lrRole.ID, False)
                rowParentRole.Visible = True
            Else
                rowParentRole.Visible = False
            End If
        Else
            roleid = 0
        End If
    End Sub
    Private Sub LoadRoleFlags()
        Dim lrRole As Role = Role.GetRole(roleid)
        If lrRole IsNot Nothing Then

            SetFlagsFromRole(lrRole)
        End If
    End Sub
    Private Sub SetFlagsFromRole(ByVal role As Role)
        IsSystemRole = role.IsSystem
        _roleTenantId = role.TenantId
        _roleLocked = role.Locked

    End Sub
    Private Shared Function GetHierarchy(ByVal vlRoleID As Integer) As String
        If vlRoleID > 0 Then
            Dim lcolSubParentStructuredRoles As ROLEList = ROLEList.GetParentStructureRoles(vlRoleID.ToString())
            If lcolSubParentStructuredRoles.Any Then
                Return GetHierarchy(lcolSubParentStructuredRoles.Item(0).ID) & lcolSubParentStructuredRoles.Item(0).Name & "\"
            End If
        End If

        Return String.Empty
    End Function
    Private Sub RenderHierarchy(ByVal vlRoleID As Integer, ByVal vbIncludeRole As Boolean)
        lblRoleHierarchy.Text = GetHierarchy(vlRoleID)

        If vbIncludeRole AndAlso vlRoleID > 0 Then
            Dim lRole As Role = Role.GetRole(vlRoleID)
            lblRoleHierarchy.Text = lblRoleHierarchy.Text & lRole.Name & "\"
        End If
    End Sub

    Private Sub LoadAcl()

        If Page.IsPostBack Then
            If Not String.IsNullOrEmpty(aclSubjectToDelete.Value) Then
                Arco.Doma.Library.Routing.RoutingSecurity.DeleteRoutingSecurity(roleid, "Role", aclSubjectToDelete.Value, aclSubjectTypeToDelete.Value)
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
        For Each loAcl As Routing.RoutingSecurityList.SecurityInfo In Routing.RoutingSecurityList.GetRoleSecurityList(roleid)
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

    End Sub

    Private Sub LoadAudit()
        Dim dt As New DataTable()
        dt.Clear()
        dt.Columns.Add("DATE")
        dt.Columns.Add("USER")
        dt.Columns.Add("DESCRIPTION")

        For Each loAudit As Logging.AppAuditList.AppAuditInfo In Logging.AppAuditList.GetAppAuditList(roleid, "ROLE")
            Dim lsUSer As String = ArcoFormatting.FormatUserName(loAudit.User)
            Dim lsDesc As String = Server.HtmlEncode(loAudit.Description)
            Dim lsDate As String = ArcoFormatting.FormatDateLabel(loAudit.AuditDate, True, True, False)

            dt.Rows.Add(New String() {lsDate, lsUSer, lsDesc})
        Next
        If dt.Rows.Count = 0 Then
            trNoAuditFound.Visible = True
            lblNoAuditFound.Text = GetLabel("noresultsfound")
        Else
            trNoAuditFound.Visible = False
        End If

        repAudit.DataSource = dt
        repAudit.DataBind()

    End Sub

    Private Sub LoadMembers()
        Dim dt As New DataTable()
        dt.Clear()
        dt.Columns.Add("TYPE")
        dt.Columns.Add("NAME")
        dt.Columns.Add("CAPTION")
        dt.Columns.Add("TenantId", GetType(Int32))
        Dim crit As New RoleMemberList.Criteria
        crit.ROLE_ID = roleid
        '   If Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal Then
        crit.TenantId = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.Id
        '    End If
        Dim luRoleMemberlist As RoleMemberList = RoleMemberList.GetRoleMemberList(crit)
        For Each member As RoleMemberList.RoleMemberInfo In luRoleMemberlist

            Dim lsCaption As String = FormatSubject(member.MEMBER, member.MEMBERTYPE)
            dt.Rows.Add(New Object() {member.MEMBERTYPE, member.MEMBER, lsCaption, member.TenantId})
        Next
        If luRoleMemberlist.Count = 0 Then
            trMembers.Visible = True
            lblNoMembersFound.Text = GetLabel("noresultsfound")
        Else
            trMembers.Visible = False

        End If

        repMembers.DataSource = dt
        repMembers.DataBind()

        plhEditMembers.Visible = CanEditMembers


        If Settings.GetValue("Security", "EnableRoleInRole", True) Then
            dt = New DataTable()
            dt.Clear()
            dt.Columns.Add("TYPE")
            dt.Columns.Add("NAME")
            dt.Columns.Add("CAPTION")
            dt.Columns.Add("TenantId", GetType(Int32))

            Dim luRoleMemberOflist As RoleMemberList = RoleMemberList.GetRoleMemberList(txtRoleName.Text, "Role")
            For Each member As RoleMemberList.RoleMemberInfo In luRoleMemberOflist
                dt.Rows.Add(New Object() {"Role", member.ROLENAME, FormatSubject(member.ROLENAME, "Role"), member.TenantId})
            Next
            If luRoleMemberOflist.Count = 0 Then
                trMemberof.Visible = True
                lblNoMemberOfFound.Text = GetLabel("noresultsfound")
            Else
                trMemberof.Visible = False
            End If

            repMemberof.DataSource = dt
            repMemberof.DataBind()
        Else
            radTab.Tabs(TabPages.MemberOf).Visible = False
        End If

    End Sub

    Protected Function FormatSubject(ByVal voItem As RoleMemberList.RoleMemberInfo) As String
        Return FormatSubject(voItem.MEMBER, voItem.MEMBERTYPE)
    End Function
    Protected Function FormatSubject(ByVal voItem As Routing.RoutingSecurityList.SecurityInfo) As String
        Return FormatSubject(voItem.Subject, voItem.SubjectType)
    End Function
    Protected Function FormatSubject(ByVal vsName As String, ByVal vsType As String) As String
        Select Case vsType
            Case "User"
                Return ArcoFormatting.FormatUserName(vsName)
            Case Else
                Return Server.HtmlEncode(vsName)
        End Select
    End Function

    Private Sub RegisterTabScript()
        If roleid > 0 Then

            Dim sb As New StringBuilder
            Dim url As String

            url = "DM_UserBrowserMembers.aspx?modal=y&roleid=" & roleid
            sb.AppendLine("function EditMembers() {")
            sb.AppendLine(RadOpenMembersWindowJS(url))
            sb.AppendLine("} ")

            url = "DM_UserBrowserMembers.aspx?modal=y&url=rolesofrole&roleid=" & roleid
            sb.AppendLine("function EditMemberOf() {")
            sb.AppendLine(RadOpenMembersWindowJS(url))
            sb.AppendLine("} ")
            sb.AppendLine(" function AddRoleAcl(){ const left = window.top.outerWidth / 2 + window.top.screenX - ( 950 / 2); const top = window.top.outerHeight / 2 + window.top.screenY - ( 600 / 2);") 'line added to center popup.
            sb.AppendLine("var w = window.open(" & EncodingUtils.EncodeJsString("EditRoleAcl.aspx?role_id=" & roleid) & ", 'RoleAcl','width=950,height=600,resizable=yes,scrollbars=yes,status=yes,top=' + top + ',left=' + left);}")
            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "EditEmbers", sb.ToString, True)
        End If

    End Sub

    Private Function RadOpenMembersWindowJS(ByVal url As String) As String
        Dim sb As New StringBuilder
        sb.Append("const width = window.innerWidth - 40;")
        sb.Append("const height = window.innerHeight - 40;")
        sb.Append("const oWnd = radopen('" & url & "', 'Roles', width , height);")
        sb.Append("oWnd.center();")
        sb.Append("oWnd.add_close(Refresh);")

        Return sb.ToString()
    End Function

    Sub GetNoMembers()
        Dim dt As New DataTable()
        dt.Clear()
        repMembers.DataSource = dt
        repMembers.DataBind()
        trMembers.Visible = True
        lblNoMembersFound.Text = GetLabel("noresultsfound")
        repMemberof.DataSource = dt
        repMemberof.DataBind()
        trMemberof.Visible = True
        lblNoMemberOfFound.Text = GetLabel("noresultsfound")
    End Sub
    Protected Function GetRoleID(ByVal name As String) As String
        Try
            Return Role.GetRole(name).ID.ToString
        Catch ex As NullReferenceException
            Return ""
        End Try

    End Function
    Protected Function GetIcon(ByVal Type As String) As String
        If Type.ToLower = "user" Then
            Return "icon icon-user-profile icon-color-light"
        ElseIf Type.ToLower = "role" Then
            Return "icon icon-user-role icon-color-light"
        Else
            Return "icon icon-user-group icon-color-light"
        End If
    End Function

    Protected Sub DoDelete(ByVal sender As Object, ByVal e As EventArgs)
        If roleid <> 0 Then           
            Dim s As String = Role.DeleteRoleWithCheck(roleid)
            If Not String.IsNullOrEmpty(s) Then
                SetErrorMessage(s)
            Else
                Response.Redirect("DM_UserBrowser_Roles.aspx")
            End If
        End If

    End Sub

  
End Class

