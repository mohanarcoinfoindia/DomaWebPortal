Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL

Partial Class DM_UserBrowser_GroupDetail
    Inherits UserBrowserBasePage
#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Protected groupid As String = ""

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        CheckIsAdmin()
        If Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsGlobal Then
            GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
            Return
        End If

        LoadLabels()

        If Request.QueryString("login") <> "" Then
            groupid = Request.QueryString("login")
        ElseIf ViewState("roleid") <> Nothing Then
            groupid = ViewState("groupid")
        End If

        If Not Page.IsPostBack AndAlso String.IsNullOrEmpty(groupid) Then
            GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INVALIDOPERATION)
            Return
        End If

        If Not Page.IsPostBack Then
            LoadGroup(groupid)
        End If

        lnkTransferWork.Visible = Security.BusinessIdentity.CurrentIdentity.Tenant.IsGlobal


        If Not String.IsNullOrEmpty(groupid) Then
            sInitJS(False)
        Else
            sInitJS(True)
        End If
        ViewState("groupid") = groupid
        CheckFocus()
        checkDomainSettings()
    End Sub

    Private Sub checkDomainSettings()
        Dim lsDomainname As String = Arco.Security.BusinessIdentity.CurrentIdentity.Domain



        txtGroupDescription.BackColor = Color.LightGray
        txtGroupDescription.ReadOnly = True
        SetFocus(txtGroupname)
        lnkSave.Visible = False
    End Sub


    Private Sub CheckFocus()

        SetFocus("txtGroupDescription")
        txtGroupname.ReadOnly = True
        txtGroupname.BackColor = Color.LightGray
        txtDomain.ReadOnly = True
        txtDomain.BackColor = Color.LightGray
        txtDomain.Visible = True
        lblNewDomain.Text = GetLabel("ub_customer")


    End Sub

    Protected Sub doSave(ByVal sender As Object, ByVal e As EventArgs)
        Try

            lblMessage.Visible = True
                lblMsgText.Text = GetLabel("saveok")

            Dim luGroup As Group = Group.GetGroup(txtDomain.Text, txtGroupname.Text)
            luGroup.GROUP_DESC = txtGroupDescription.Text
            luGroup.Save()

        Catch ex As Exception
            lblMsgText.Attributes.Add("style", "color:red")
            lblMessage.Visible = True
            lblMsgText.Text = "Error during the save: " & ex.ToString()
            tblAddMembers.Visible = False
        End Try
        CheckFocus()
        groupid = txtGroupname.Text
        ViewState("groupid") = txtGroupname.Text


        checkDomainSettings()
    End Sub
    Protected Function GetIcon(ByVal Type As String) As String
        If Type.ToLower = "user" Then
            Return "user.bmp"
        ElseIf Type.ToLower = "role" Then
            Return "role.bmp"
        Else
            Return "groups.png"
        End If
    End Function
    Protected Function FormatSubject(ByVal voItem As GroupMemberList.GroupMemberInfo) As String
        Return FormatSubject(voItem.MEMBER, voItem.MEMBERTYPE)
    End Function
    Protected Function FormatSubject(ByVal vsName As String, ByVal vsType As String) As String
        Select Case vsType
            Case "User"
                Return ArcoFormatting.FormatUserName(vsName)
            Case Else
                Return Server.HtmlEncode(vsName)
        End Select
    End Function

    Protected Function GetRoleID(ByVal name As String) As String
        Return Role.GetRole(name).ID.ToString
    End Function
    Private Sub getMembers()
        Dim dt As DataTable = New DataTable()
        dt.Clear()
        dt.Columns.Add("TYPE")
        dt.Columns.Add("NAME")
        dt.Columns.Add("CAPTION")
        dt.Columns.Add("DESCRIPTION")

        Dim luGroupMemberlist As GroupMemberList = GroupMemberList.GetGroupMemberList(txtGroupname.Text)
        For Each member As GroupMemberList.GroupMemberInfo In luGroupMemberlist

            Dim lsCaption As String = FormatSubject(member.MEMBER, member.MEMBERTYPE)
            dt.Rows.Add(New String() {member.MEMBERTYPE, member.MEMBER, lsCaption, Server.HtmlEncode(member.GetDescription)})
        Next
        If luGroupMemberlist.Count = 0 Then
            trMembers.Visible = True
            lblNoMembersFound.Text = GetLabel("noresultsfound")
        Else
            trMembers.Visible = False

        End If

        repMembers.DataSource = dt
        repMembers.DataBind()

        plhEditMembers.Visible = False 'Me.CanEditMembers

    End Sub


    Private Sub LoadLabels()

        lblGroupName.Text = GetLabel("ub_groupname")
        lblGroupDescription.Text = GetLabel("ub_groupdesc")
        lblNewDomain.Text = GetLabel("ub_newdomain")
        lnkTransferWork.Text = GetLabel("Transfer Work")
        btnClose.Text = GetLabel("close")
        lnkSave.Text = GetLabel("save")
        radTab.Tabs(0).Text = GetLabel("members")
    End Sub

    Private Sub LoadGroup(ByVal groupName As String)
        Dim lgGroup As Group = Group.GetGroup(groupName)
        If lgGroup IsNot Nothing Then
            txtGroupDescription.Text = lgGroup.GROUP_DESC
            txtGroupname.Text = lgGroup.GROUP_NAME
            txtDomain.Text = lgGroup.DOMAIN_NAME

            getMembers()
        Else
            Throw New ArgumentException("Group " & groupName & " was not found")
        End If

    End Sub

    Private Sub sInitJS(ByVal withSlachCheck As Boolean)

        Dim sb As New StringBuilder
        sb.AppendLine("function TransferWork() {")
        sb.AppendLine("var w = window.open(" & EncodingUtils.EncodeJsString("TransferWork.aspx?subject_type=Group&subject_id=" & Server.UrlEncode(txtGroupname.Text)) & ", 'TransferWork','width=800,height=600,resizable=yes,scrollbars=yes,status=yes');")
        sb.AppendLine("}")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "Checks", sb.ToString, True)
    End Sub

End Class

