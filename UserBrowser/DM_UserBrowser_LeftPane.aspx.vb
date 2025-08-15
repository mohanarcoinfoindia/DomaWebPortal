Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Security

Partial Class DM_UserBrowser_LeftPane
    Inherits UserBrowserBasePage

    Protected moTreeView As New obout_ASPTreeView_2_NET.Tree
    Private ReadOnly moDisableDrag As List(Of String) = New List(Of String)
    Private ReadOnly moDisableDrop As List(Of String) = New List(Of String)
 
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        sc1.Scripts.Add(New ScriptReference("~/Resources/" & Language & "/Messages.js"))

        CM_TreeRoles.Visible = isAdmin()

        If Not IsPostBack Then

            If System.IO.Directory.Exists(Page.Server.MapPath(String.Concat("~/App_Themes/", Page.Theme, "/Tree/Icons"))) Then
                moTreeView.FolderIcons = String.Concat("../App_Themes/", Page.Theme, "/Tree/Icons")
            Else
                moTreeView.FolderIcons = "../TreeIcons/Icons"
            End If
            moTreeView.FolderStyle = "../TreeIcons/Styles/Win2003"
            moTreeView.FolderScript = "../TreeIcons/Script"
            moTreeView.SubTree = False
            moTreeView.ShowRootIcon = True
            moTreeView.ShowRootPlusMinus = True
            moTreeView.DragAndDropEnable = isAdmin()
            moTreeView.SelectedId = QueryStringParser.GetString("selected")

            If QueryStringParser.GetString("delete") = "1" Then
                Dim roleIdToDelete As Int32 = QueryStringParser.GetInt("roleid")
                Dim S As String = Role.DeleteRoleWithCheck(roleIdToDelete)

                If Not String.IsNullOrEmpty(S) Then
                    moTreeView.SelectedId = GetRoleStructureString(roleIdToDelete)
                    Dim sb As New StringBuilder
                    sb.AppendLine("alert(" & EncodingUtils.EncodeJsString(S) & ");")
                    Page.ClientScript.RegisterClientScriptBlock(GetType(String), "Alert", sb.ToString, True)
                End If
            End If

        End If
        GetNextLevel(moTreeView) ' Root Node


        moTreeView.DragDisableId = String.Join(",", String.Join(",", moDisableDrag))
        moTreeView.DropDisableId = String.Join(",", String.Join(",", moDisableDrop))

        txtSelectedID.Value = moTreeView.SelectedId
        TreeView.Text = moTreeView.HTML

        CM_TreeRoles.Items(0).Text = GetLabel("add")
        CM_TreeRoles.Items(0).ImageUrl = ThemedImage.GetUrl("new.png", Me)
        CM_TreeRoles.Items(2).Text = GetLabel("edit")
        CM_TreeRoles.Items(2).ImageUrl = ThemedImage.GetUrl("edit.png", Me)
        CM_TreeRoles.Items(3).Text = GetLabel("delete")
        CM_TreeRoles.Items(3).ImageUrl = ThemedImage.GetUrl("delete.png", Me)

        CM_Users.Items(0).Text = GetLabel("add")
        CM_Users.Items(0).ImageUrl = ThemedImage.GetUrl("new.png", Me)
        CM_Roles.Items(0).Text = GetLabel("add")
        CM_Roles.Items(0).ImageUrl = ThemedImage.GetUrl("new.png", Me)
        CM_HRoles.Items(0).Text = GetLabel("add")
        CM_HRoles.Items(0).ImageUrl = ThemedImage.GetUrl("new.png", Me)
        CM_HRoles.Items(1).Text = GetLabel("refresh")
        CM_HRoles.Items(1).ImageUrl = ThemedImage.GetUrl("refresh.png", Me)
    End Sub

    Private Function GetRoleStructureString(ByVal childroleid As Int32) As String
        Dim idList As New List(Of Int32)
        Dim roleid As Int32 = childroleid
        While roleid <> 0
            Dim currentRoleId As Int32 = roleid
            idList.Add(currentRoleId)
            roleid = 0
            For Each link As RoleLinkList.ROLELinkInfo In RoleLinkList.GetRoleLinkList(0, currentRoleId)
                roleid = link.PARENT_ROLE_ID
                Exit For
            Next
        End While

        idList.Reverse()
        Return String.Join(",", idList)
    End Function
    Private Sub GetNextLevel(ByVal tree As obout_ASPTreeView_2_NET.Tree)
        Dim rootNode As String = "root"
        If isAdmin() Then
            tree.Add(rootNode, "t_users", "<a href=javascript:OpenUrl('DM_UserBrowser_RightPane.aspx?drpStatus=Valid');>" & GetLabel("users") & "</a>", False, "User.svg")
            If Not Settings.GetValue("General", "MultiTenant", False) Then
                tree.Add("t_users", "t_namedweb", "<a href=javascript:OpenUrl('DM_UserBrowser_RightPane.aspx?namedin=Web');>Named in Web</a>", False, "User.svg")
                tree.Add("t_users", "t_namedwebapi", "<a href=javascript:OpenUrl('DM_UserBrowser_RightPane.aspx?namedin=Web+API');>Named in Web API</a>", False, "User.svg")
            End If

            tree.Add(rootNode, "t_roles", "<a href=javascript:OpenUrl('DM_UserBrowser_Roles.aspx?roletype=0');> " & GetLabel("roles") & " </a>", False, "Role.svg")

            If BusinessIdentity.CurrentIdentity.Tenant.IsGlobal Then
                tree.Add(rootNode, "t_hroles", "<a href=javascript:OpenUrl('DM_UserBrowser_Roles.aspx?roletype=1');>" & GetLabel("rolestructure") & " </a>", True, "Role.svg")

                tree.Add(rootNode, "t_groups", "<a href=javascript:OpenUrl('DM_UserBrowser_Groups.aspx');> " & GetLabel("groups") & " </a>", True, "Group.svg")
            End If


            If BusinessIdentity.CurrentIdentity.CanManageDelegations Then
                tree.Add(rootNode, "t_delegates", "<a href=javascript:OpenUrl('Delegates.aspx');> " & GetLabel("delegates") & " </a>", False, "Delegate.svg")
            End If


            moDisableDrag.Add("t_users")
            moDisableDrag.Add("t_roles")
            moDisableDrag.Add("t_hroles")
            moDisableDrag.Add("t_groups")
            moDisableDrag.Add("t_delegates")


            moDisableDrop.Add("t_users")
            moDisableDrop.Add("t_roles")
            moDisableDrop.Add("t_groups")
            moDisableDrop.Add("t_delegates")


            Dim loCrit As New ROLEList.Criteria()
            loCrit.Structured = True
            loCrit.SHOWEVERYONE = False
            For Each loItem As ROLEList.ROLEInfo In ROLEList.GetROLEList(loCrit)
                Dim lcolParentStructuredRoles As ROLEList = ROLEList.GetParentStructureRoles(loItem.ID.ToString)
                If lcolParentStructuredRoles.Count = 0 Then 'is parent
                    Dim lcolChildStructuredRoles As ROLEList = ROLEList.GetChildStructureRoles(loItem.ID.ToString)
                    If lcolChildStructuredRoles.Any Then
                        ShowNode(tree, loItem, True)
                    Else
                        ShowNode(tree, loItem, False)
                    End If
                End If
            Next
        Else
            tree.Add("root", "t_roles", "<a href=javascript:OpenUrl('DM_UserBrowser_Roles.aspx?roletype=0');>" & GetLabel("roles") & "</a>", False, "folder.svg")
            moDisableDrag.Add("t_roles")
            CM_Roles.Visible = False

            If BusinessIdentity.CurrentIdentity.CanManageDelegations Then
                tree.Add("root", "t_delegates", "<a href=javascript:OpenUrl('Delegates.aspx');>" & GetLabel("delegates") & "</a>", False, "folder.svg")
                moDisableDrag.Add("t_delegates")
                moDisableDrop.Add("t_delegates")
            End If
        End If


        If BusinessIdentity.CurrentIdentity.isAdmin AndAlso
              BusinessIdentity.CurrentIdentity.IsGlobal Then

            tree.Add(rootNode, "t_defaultprefs", "<a href=javascript:OpenUrl('DefaultUserPreferences.aspx');>" & GetLabel("prefs") & "</a>", False, "Settings.svg")

            If Settings.GetValue("General", "MultiTenant", False) Then
                tree.Add(rootNode, "t_tenants", "<a href=javascript:OpenUrl('Tenants.aspx');>Tenants</a>", False, "Tenant.svg")
                moDisableDrag.Add("t_tenants")
                moDisableDrop.Add("t_tenants")
            End If
            '  If Settings.GetValue("Security", "EnableIdentityProvider", False) Then
            tree.Add(rootNode, "t_clients", "<a href=javascript:OpenUrl('Clients.aspx');> " & GetLabel("clientapps") & " </a>", False, "Website.svg")
            moDisableDrag.Add("t_clients")
                moDisableDrop.Add("t_clients")
            '  End If

        End If

    End Sub
    Private Sub ShowNode(ByVal voTree As obout_ASPTreeView_2_NET.Tree, ByVal voItem As ROLEList.ROLEInfo, ByVal ChildRoles As Boolean)

        Dim lsURL As String
        Dim lsCaption As String
        Dim lsDesc As String = voItem.Name

        lsCaption = "<NOBR><span id='t_" & voItem.ID & "'>"
            lsCaption &= "<a href=javascript:ShowUrl('DM_UserBrowser_RoleDetail.aspx'," & voItem.ID & ",false,'',true) >"
        lsCaption &= lsDesc
        lsCaption &= "</a>"
        lsCaption &= "</span></NOBR>"

        If ChildRoles Then
            lsURL = "DM_RoleSubTree.aspx?parent=" & voItem.ID & ""
        Else
            lsURL = ""
        End If
        voTree.Add("t_hroles", voItem.ID, lsCaption, True, "Role.svg", lsURL)

    End Sub


    <System.Web.Services.WebMethod()> _
    Public Shared Function OnBeforeNodeDrop(ByVal src As String, _
                                   ByVal dst As String, _
                                   ByVal copy As Boolean) As String

        If src = "t_users" Or src = "t_roles" Or src = "t_groups" Or src = "t_hroles" And (dst = "t_hroles") Then
            Return "dragdropnotallowed"
        End If
        If src = "0" Or dst = "0" Then
            Return "dragdropnotallowed"
        End If
        If copy Then
            ''todo
            'Dim liSrcID As Integer
            'Dim lsCopyString As String
            'If src.Contains("_") Then
            '    Dim laSplit() As String = src.Split("_"c)
            '    liSrcID = Convert.ToInt32(laSplit(0))
            '    lsCopyString = laSplit(1)
            'Else
            '    liSrcID = Convert.ToInt32(src)
            '    lsCopyString = "Copy"
            'End If
            'Dim loSrcRole As ACL.Role = ACL.Role.GetRole(liSrcID)
            'Dim loNewRole As ACL.Role = ACL.Role.NewRole
            'loNewRole.ROLE_STRUCTURED = True
            'loNewRole.ROLE_NAME = loSrcRole.ROLE_NAME & " " & lsCopyString
            'loNewRole.ROLE_DESCRIPTION = loSrcRole.ROLE_DESCRIPTION
            'loNewRole = loNewRole.Save
            'If dst = "t_hroles" Then
            '    ACL.RoleLink.CreateLink(0, loNewRole.ROLE_ID)
            'Else
            '    ACL.RoleLink.CreateLink(dst, loNewRole.ROLE_ID)
            'End If
            Throw New InvalidOperationException("Copy is not supported")
        Else
            If IsNumeric(src) And IsNumeric(dst) Then
                RoleLink.DeleteLink(0, src)
                RoleLink.CreateLink(dst, src)
                Return "0"
            ElseIf IsNumeric(src) And dst = "t_hroles" Then
                RoleLink.DeleteLink(0, src)
                RoleLink.CreateLink(0, src)
                Return "0"
            End If
        End If
       

        Return "dragdropnotallowed"
    End Function
  

End Class
