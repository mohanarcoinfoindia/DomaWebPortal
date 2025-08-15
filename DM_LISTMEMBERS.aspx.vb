Imports Arco.Doma.Library.ACL

Partial Class DM_LISTMEMBERS
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim lsSubject As String = QueryStringParser.GetString("subjectid") 'role_id or grp_name
        Dim lsSubjectType As String = QueryStringParser.GetString("subjecttype")

        Page.Title = Server.HtmlEncode(lsSubject) & " : " & GetLabel("members")

        Select Case lsSubjectType.ToLower
            Case "role"
                Dim loRoleList As ROLEList = ROLEList.GetROLE(lsSubject)

                For Each loRole As ROLEList.ROLEInfo In loRoleList
                    Dim lcolRoleMembers As RoleMemberList = RoleMemberList.GetRoleMemberList(loRole.ID)
                    lstMembers.DataSource = lcolRoleMembers
                    lstMembers.DataBind()
                    Exit For
                Next
            Case "group"
                Dim lcolGroupMembers As GroupMemberList = GroupMemberList.GetGroupMemberList(lsSubject)
                lstMembers.DataSource = lcolGroupMembers
                lstMembers.DataBind()
        End Select
    End Sub
End Class
