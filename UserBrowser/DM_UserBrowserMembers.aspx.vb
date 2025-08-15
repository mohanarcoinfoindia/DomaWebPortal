Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Telerik.Web.UI


''' <summary>
''' 
''' </summary>
Partial Public MustInherit Class DM_UserBrowserMembers
    Inherits UserBrowserBasePage
    Private pMode As pageMode
    Private mDefaultMaxResults As Int32 = 100
    Public Property ShowRoleTenant As Boolean

    Private _restrictToTenantId As Integer = 0

    Private Enum pageMode
        group = 0
        groupsofuser = 1
        rolesofuser = 2
        rolemembers = 3
        rolesofrole = 4
    End Enum

    Private Sub SetLabels()
        ADD.ToolTip = GetLabel("add")
        REMOVE.ToolTip = GetLabel("remove")
    End Sub

    Private Sub SetIcons()
        ADD.Text = "<span class='icon icon-chevron-right icon-color-light' />"
        REMOVE.Text = "<span class='icon icon-chevron-left icon-color-light' />"
        '  search.Text = "<span class='icon icon-search' />"
        btnFilter.Text = GetLabel("filter")
    End Sub

    Private Sub CheckIfAllCheckBoxesAreFalse()
        If Not chckUsers.Checked AndAlso Not chckRoles.Checked AndAlso Not chckGroups.Checked Then
            chckUsers.Checked = True
            chckRoles.Checked = True
            chckGroups.Checked = True
        End If
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim hasAccess As Boolean = IsAdmin()

        SetLabels()
        SetIcons()

        If Not IsPostBack Then
            subjectid.Value = Request.QueryString("roleid")
        End If
        Select Case Request.QueryString("url")
            Case "group"
                trSearchCrit.Visible = False
                tcRoles.Visible = False
                chckRoles.Checked = False
                tcDomains.Visible = False
                'to implement: groups in groups
                tcGroups.Visible = False
                chckGroups.Checked = False
                pMode = pageMode.group

            Case "groupsofuser"
                trSearchCrit.Visible = False
                tcRoles.Visible = False
                chckRoles.Checked = False
                tcDomains.Visible = False
                tcGroups.Visible = False
                tcUsers.Visible = False
                chckUsers.Checked = False
                chckGroups.Checked = True
                pMode = pageMode.groupsofuser

            Case "rolesofuser"
                trSearchCrit.Visible = False
                tcRoles.Visible = False
                chckRoles.Checked = True
                tcDomains.Visible = False
                tcGroups.Visible = False
                tcUsers.Visible = False
                chckUsers.Checked = False
                chckGroups.Checked = False
                pMode = pageMode.rolesofuser
                Page.Title = ArcoFormatting.FormatUserName(subjectid.Value) & " : " & GetLabel("ub_editroles")
                ShowRoleTenant = Settings.GetValue("General", "MultiTenant", False) AndAlso Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsGlobal

            Case "rolesofrole"
                trSearchCrit.Visible = False
                tcRoles.Visible = False
                chckRoles.Checked = True
                tcDomains.Visible = False
                tcGroups.Visible = False
                tcUsers.Visible = False
                chckUsers.Checked = False
                chckGroups.Checked = False
                pMode = pageMode.rolesofrole
                ShowRoleTenant = Settings.GetValue("General", "MultiTenant", False) AndAlso Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.IsGlobal

                Dim loRole As Role
                If Not Page.IsPostBack Then
                    loRole = Role.GetRole(CInt(subjectid.Value))
                    Page.Title = loRole.Name & " : " & GetLabel("ub_editroles")

                    subjectid.Value = loRole.Name
                Else
                    loRole = Role.GetRole(subjectid.Value)
                End If
                _restrictToTenantId = loRole.TenantId
            Case Else 'rolemembers
                If Not IsPostBack Then
                    LoadDomains()

                    ApplyUserBrowserDefaults()

                End If
                If Not Settings.GetValue("Security", "EnableRoleInRole", True) Then
                    tcRoles.Visible = False
                    chckRoles.Checked = False
                End If


                pMode = pageMode.rolemembers

                Dim role As Role = Role.GetRole(CInt(subjectid.Value))

                _restrictToTenantId = role.TenantId

                Page.Title = role.Name & " : " & GetLabel("ub_addmembers")
                hasAccess = role.CanEditMembers
        End Select
        If Not hasAccess Then
            Response.Redirect("../DM_ACL_DENIED.aspx")
        End If

        If ShowRoleTenant Then
            grdSelection.Columns(4).Display = True
            grdSelection.Columns(4).HeaderText = GetLabel("tenant")
        Else
            grdSelection.Columns(4).Display = False
        End If

        closeButtonRow.Visible = Not QueryStringParser.GetBoolean("modal")
        CheckIfAllCheckBoxesAreFalse()
    End Sub

    Private Sub ApplyUserBrowserDefaults()
        chckUsers.Checked = Settings.GetValue("UserBrowser", "DefaultShowUsers", True)
        chckGroups.Checked = Settings.GetValue("UserBrowser", "DefaultShowGroups", True)
        chckRoles.Checked = Settings.GetValue("UserBrowser", "DefaultShowRoles", True)
    End Sub

    Protected Class Member

        Public Sub New(ByVal Id As String, ByVal Name As String, ByVal Description As String, ByVal ICONCLASS As String, ByVal MEMBERTOOLTIP As String, ByVal TYPE As String, ByVal tenantId As Integer)
            Me.ID = Id
            Me.Name = Name
            Me.Description = Description
            Me.ICONCLASS = ICONCLASS
            Me.MEMBERTOOLTIP = MEMBERTOOLTIP
            Me.TYPE = TYPE
            Me.TenantId = tenantId
        End Sub

        Public Property TenantId As String

        Public Property ID() As String

        Public Property Name() As String

        Public Property Description() As String

        Public Property ICONCLASS() As String

        Public Property MEMBERTOOLTIP() As String

        Public Property TYPE() As String

    End Class

    Protected ReadOnly Property SelectionList() As IList(Of Member)
        Get
            Dim nameFilter As String = ""
            '   Dim descriptionFilter As String = ""
            For Each column As GridColumn In grdSelection.MasterTableView.RenderColumns
                If TypeOf column Is GridBoundColumn Then
                    Dim boundColumn As GridBoundColumn = TryCast(column, GridBoundColumn)
                    Select Case boundColumn.UniqueName
                        Case "code"
                            nameFilter = boundColumn.CurrentFilterValue
                            'Case "description"
                            '    descriptionFilter = boundColumn.CurrentFilterValue
                    End Select
                End If
            Next
            Return GetNotMembers(nameFilter, Nothing)
        End Get
    End Property

    Protected Sub grdSelection_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs)

        If Not String.IsNullOrEmpty(grdSelection.MasterTableView.FilterExpression()) Then
            doFilter(True)
            grdSelection.MasterTableView.FilterExpression() = ""
        Else
            grdSelection.DataSource = SelectionList
        End If
    End Sub

    Protected Sub grdMembers_NeedDataSource(ByVal source As Object, ByVal e As GridNeedDataSourceEventArgs)
        grdMembers.DataSource = GetMembersFromMember()
    End Sub

    ''' <summary>
    ''' Gets the user, groups, roles that are not in the Role/group
    ''' </summary>
    ''' <param name="nameFilter">The filter on the name.</param>
    ''' <param name="descriptionFilter">The filter on the description.</param><returns></returns>

    Protected Function GetNotMembers(ByVal nameFilter As String, ByVal descriptionFilter As String) As IList(Of Member)
        Dim memOfUser As IList(Of Member) = GetMembersFromMember()

        Dim results As IList(Of Member) = New List(Of Member)()

        Dim llFirstRec As Int32 = 0
        Dim llLastRec As Int32 = 0
        Dim liTellerTotal As Integer = 0
        Dim domainname As String = ""

        If Not String.IsNullOrEmpty(drpdwnDomains.SelectedValue) Then
            domainname = drpdwnDomains.SelectedValue
        End If

        If chckUsers.Checked Then
            Dim userCrit As New USERList.Criteria(domainname, nameFilter) With {
                .Range = ListRangeRequest.Range(llFirstRec, llLastRec, mDefaultMaxResults)
            }

            For Each user As USERList.USERSInfo In USERList.GetUSERSList(userCrit)
                Dim skip As Boolean = memOfUser.Any(Function(x) x.ID = user.USER_ACCOUNT)

                If Not skip Then
                    Dim iconClass = "icon icon-user-profile icon-color-light"
                    results.Add(New Member(user.USER_ACCOUNT, ArcoFormatting.FormatUserName(user.USER_LOGIN) & "&nbsp;<small>(" & user.USER_LOGIN & ")</small>", Server.HtmlEncode(user.USER_DESC), iconClass, GetLabel("user"), "User", 0))
                End If
            Next
            liTellerTotal = results.Count()
        End If

        If chckRoles.Checked Then
            If liTellerTotal < mDefaultMaxResults Then
                Dim roleCrit As New ROLEList.Criteria With {
                .FILTER = nameFilter,
                .ROLE_DESCRIPTION = descriptionFilter,
                .SHOWEVERYONE = False
            }
                roleCrit.Range.MaxResults = mDefaultMaxResults - liTellerTotal

                For Each role As ROLEList.ROLEInfo In ROLEList.GetROLEList(roleCrit)

                    'don't mix roles of tenants
                    If _restrictToTenantId <> 0 AndAlso role.TenantId <> 0 AndAlso role.TenantId <> _restrictToTenantId Then Continue For

                    'skip self
                    If pMode = pageMode.rolemembers AndAlso role.ID = Convert.ToInt32(subjectid.Value) Then Continue For

                    'skip self
                    If pMode = pageMode.rolesofrole AndAlso role.Name = subjectid.Value Then Continue For

                    'skip duplicates
                    If memOfUser.Any(Function(x) x.ID = role.Name) Then Continue For
                    Dim roleIconClass = "icon icon-user-role icon-color-light"
                    results.Add(New Member(role.Name, Server.HtmlEncode(role.Name), Server.HtmlEncode(role.Description), roleIconClass, GetLabel("role"), "Role", role.TenantId))
                Next
            End If
            liTellerTotal = results.Count()
        End If

        If chckGroups.Checked AndAlso chckGroups.Visible Then
            If liTellerTotal < mDefaultMaxResults Then
                Dim groupCrit As New GROUPList.Criteria With {
                    .FILTER = nameFilter,
                    .GROUP_DESC = descriptionFilter,
                    .DOMAIN = domainname
                }
                groupCrit.Range.MaxResults = mDefaultMaxResults - liTellerTotal
                For Each group As GROUPList.GROUPSInfo In GROUPList.GetGROUPSList(groupCrit)
                    Dim skip As Boolean = False
                    If pMode = pageMode.groupsofuser Then
                        skip = memOfUser.Any(Function(x) x.ID = group.GROUP_NAME)
                    End If

                    If Not skip Then
                        Dim groupIconClass = "icon icon-user-group icon-color-light"
                        results.Add(New Member(group.GROUP_NAME, Server.HtmlEncode(group.GROUP_NAME), Server.HtmlEncode(group.GROUP_DESC), groupIconClass, GetLabel("group"), "Group", 0))
                    End If
                Next
            End If
            liTellerTotal = results.Count()
        End If

        If liTellerTotal >= mDefaultMaxResults Then
            lblWarning.Visible = True
        Else
            lblWarning.Visible = False
        End If

        Return results
    End Function

    Protected Function GetMembersFromMember() As IList(Of Member)
        Dim results As New List(Of Member)()

        Select Case pMode
            Case pageMode.rolemembers
                Dim crit As New RoleMemberList.Criteria
                crit.ROLE_ID = CInt(subjectid.Value)
                crit.TenantId = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.Id
                Dim RoleMemberList As RoleMemberList = RoleMemberList.GetRoleMemberList(crit)
                For Each roleMember As RoleMemberList.RoleMemberInfo In RoleMemberList
                    If roleMember.ROLE_ID <> 0 Then
                        Dim name As String = ""
                        Dim desc As String = ""
                        Dim iconClass As String = ""
                        Dim imageTooltip As String = ""
                        Select Case roleMember.MEMBERTYPE
                            Case "User"
                                iconClass = "icon icon-user-profile icon-color-light"
                                imageTooltip = GetLabel("user")
                                name = ArcoFormatting.FormatUserName(roleMember.MEMBER)
                            Case "Role"
                                iconClass = "icon icon-user-role icon-color-light"
                                imageTooltip = GetLabel("role")
                                name = Server.HtmlEncode(roleMember.MEMBER)
                            Case "Group"
                                iconClass = "icon icon-user-group icon-color-light"
                                imageTooltip = GetLabel("group")
                                name = Server.HtmlEncode(roleMember.MEMBER)
                        End Select
                        'Description
                        results.Add(New Member(roleMember.MEMBER, name, desc, iconClass, imageTooltip, roleMember.MEMBERTYPE, roleMember.TenantId))
                    End If
                Next
            Case pageMode.rolesofuser
                Dim crit As New ROLEList.Criteria()
                crit.USER_ID = subjectid.Value
                crit.TenantId = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.Id
                For Each role As ROLEList.ROLEInfo In ROLEList.GetROLEList(crit)
                    If role.ID <> 0 Then
                        results.Add(New Member(role.Name, Server.HtmlEncode(role.Name), Server.HtmlEncode(role.Description), "icon icon-user-role icon-color-light", GetLabel("Role"), "Role", role.TenantId))
                    End If
                Next
            Case pageMode.rolesofrole

                Dim crit As New ROLEList.Criteria()
                crit.USER_ID = subjectid.Value
                crit.TenantId = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Tenant.Id
                For Each role As ROLEList.ROLEInfo In ROLEList.GetROLEList(crit)
                    If role.ID <> 0 Then
                        results.Add(New Member(role.Name, Server.HtmlEncode(role.Name), Server.HtmlEncode(role.Description), "icon icon-user-role icon-color-light", GetLabel("Role"), "Role", role.TenantId))
                    End If
                Next

        End Select

        Return results
    End Function

    Private Shared Function GetMember(ByVal membersToSearchIn As IEnumerable(Of Member), ByVal ID As String) As Member
        Return membersToSearchIn.FirstOrDefault(Function(x) x.ID = ID)
    End Function

    Protected Sub grdSelection_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles grdSelection.ItemDataBound
        Select Case e.Item.ItemType
            Case GridItemType.Header
                Dim header As GridHeaderItem = DirectCast(e.Item, GridHeaderItem)
                CType(header("code").Controls(0), LinkButton).Text = GetLabel("name")
                CType(header("description").Controls(0), LinkButton).Text = GetLabel("description")
            Case GridItemType.FilteringItem
                Dim filter As GridFilteringItem = DirectCast(e.Item, GridFilteringItem)
                CType(filter("code").Controls(0), TextBox).Width = Unit.Pixel(200)
        End Select

    End Sub

    Protected Sub grdMembers_ItemDataBound(ByVal sender As Object, ByVal e As GridItemEventArgs) Handles grdMembers.ItemDataBound
        If e.Item.ItemType = GridItemType.Header Then

            Dim header As GridHeaderItem = DirectCast(e.Item, GridHeaderItem)
            CType(header("code").Controls(0), LinkButton).Text = GetLabel("name")
            ' CType(header("description").Controls(0), LinkButton).Text = GetLabel("description")

        End If
    End Sub


    Private Sub LoadDomains()

        Dim curDomain As String = Arco.Security.BusinessIdentity.CurrentIdentity.Domain
        drpdwnDomains.Items.Add(New RadComboBoxItem("", ""))
        For Each domain As DOMAINList.DOMAINSInfo In DOMAINList.GetDOMAINSList(True)
            drpdwnDomains.Items.Add(New RadComboBoxItem(domain.DOMAIN_NAME, domain.DOMAIN_NAME) With {.Selected = domain.DOMAIN_NAME.Equals(curDomain)})
        Next
        'if we only have the _database domain, don't show the filter
        If drpdwnDomains.Items.Count > 2 Then
            tcDomains.Visible = True
            tcGroups.Visible = True
        Else
            tcDomains.Visible = False
            tcGroups.Visible = False
        End If

    End Sub

#Region "Actions"

    Protected Sub btnApplyFilter(ByVal sender As Object, ByVal e As EventArgs)
        doFilter()
    End Sub

    Protected Sub filter(ByVal sender As Object, ByVal e As EventArgs)
        doFilter()
    End Sub

    Protected Sub doFilter(Optional ByVal fromNeedDataSource As Boolean = False)
        grdSelection.DataSource = SelectionList ' GetNotMembers(lsCode, lsDescription)
        If Not fromNeedDataSource Then
            grdSelection.Rebind()
            grdMembers.Rebind()
        End If
    End Sub

    Protected Sub btnAdd(ByVal sender As Object, ByVal e As EventArgs)
        addMembers(True)
    End Sub

    Protected Sub btnRemove(ByVal sender As Object, ByVal e As EventArgs)
        removeMembers(True)
    End Sub

    Private Sub removeMembers(ByVal onlySelected As Boolean)
        Dim loMemberList As IList(Of Member) = GetMembersFromMember()
        Dim loSelectionList As IList(Of Member) = SelectionList

        For Each radTblView As GridDataItem In grdMembers.MasterTableView.Items
            If radTblView.Selected OrElse Not onlySelected Then
                Dim id As String = ""
                For Each gridOrderline As GridTableCell In radTblView.Controls
                    For Each Control In gridOrderline.Controls
                        Select Case Control.ID
                            Case "ID"
                                id = Control.Text
                                Exit For
                        End Select
                    Next
                    If Not String.IsNullOrEmpty(id) Then
                        Dim tmpOrder As Member = GetMember(loMemberList, id)
                        If tmpOrder IsNot Nothing Then
                            delete(loMemberList, loSelectionList, tmpOrder)
                        End If
                        Exit For
                    End If
                Next
            End If
        Next
        doFilter()
    End Sub

    Private Sub addMembers(ByVal onlySelected As Boolean)
        Dim loMemberList As IList(Of Member) = GetMembersFromMember()
        Dim loSelectionList As IList(Of Member) = SelectionList

        For Each radTblView As GridDataItem In grdSelection.MasterTableView.Items
            If radTblView.Selected OrElse Not onlySelected Then
                Dim id As String = ""
                For Each gridOrderline As GridTableCell In radTblView.Controls
                    For Each Control In gridOrderline.Controls
                        Select Case Control.ID
                            Case "ID"
                                id = Control.Text
                                Exit For
                        End Select
                    Next
                    If Not String.IsNullOrEmpty(id) Then
                        Dim tmpOrder As Member = GetMember(loSelectionList, id)
                        If tmpOrder IsNot Nothing Then
                            update(loMemberList, loSelectionList, tmpOrder)
                        End If

                        Exit For
                    End If
                Next
            End If
        Next
        doFilter()
    End Sub

    Private Sub delete(ByVal membersFrom As IList(Of Member), ByVal members As IList(Of Member), ByVal tmpMember As Member)

        Dim role As Role
        If pMode = pageMode.rolemembers Then
            role = Role.GetRole(Convert.ToInt32(subjectid.Value))

        Else
            role = Role.GetRole(tmpMember.ID)
        End If
        If role IsNot Nothing AndAlso role.CanEditMembers() Then

            Select Case pMode
                Case pageMode.rolesofuser
                    role.RemoveMember(subjectid.Value, "User")

                Case pageMode.rolesofrole
                    role.RemoveMember(subjectid.Value, "Role")

                Case pageMode.rolemembers
                    role.RemoveMember(tmpMember.ID, tmpMember.TYPE)

            End Select

            membersFrom.Remove(tmpMember)
            members.Add(tmpMember)
        End If
    End Sub

    Private Sub update(ByVal membersFrom As IList(Of Member), ByVal members As IList(Of Member), ByVal tmpMember As Member)


        Dim role As Role
        If pMode = pageMode.rolemembers Then
            role = Role.GetRole(Convert.ToInt32(subjectid.Value))

        Else
            role = Role.GetRole(tmpMember.ID)
        End If
        If role IsNot Nothing AndAlso role.CanEditMembers() Then


            Select Case pMode
                Case pageMode.rolesofuser
                    role.AddMember(subjectid.Value, "User")

                Case pageMode.rolesofrole
                    role.AddMember(subjectid.Value, "Role")

                Case pageMode.rolemembers
                    role.AddMember(tmpMember.ID, tmpMember.TYPE)

            End Select

            members.Remove(tmpMember)
            membersFrom.Add(tmpMember)

        End If
    End Sub
#End Region

#Region "Drag & drop events"
    Protected Sub dragAndDrop_addMember(ByVal sender As Object, ByVal e As GridDragDropEventArgs)
        If String.IsNullOrEmpty(e.HtmlElement) Then
            If e.DraggedItems(0).OwnerGridID = grdSelection.ClientID Then
                ' items are drag from pending to shipped grid 
                Dim loMemberList As IList(Of Member) = GetMembersFromMember()
                If (e.DestDataItem Is Nothing AndAlso loMemberList.Count = 0) OrElse (e.DestDataItem IsNot Nothing AndAlso e.DestDataItem.OwnerGridID = grdMembers.ClientID) Then

                    Dim loSelectionList As IList(Of Member)

                    loSelectionList = SelectionList

                    For Each draggedItem As GridDataItem In e.DraggedItems
                        Dim id As String = ""
                        For Each tc As TableCell In draggedItem.Cells
                            For Each Control In tc.Controls
                                Select Case Control.ID
                                    Case "ID"
                                        id = Control.Text
                                        Exit For
                                End Select
                            Next
                        Next

                        Dim tmpOrder As Member = GetMember(loSelectionList, id)

                        If tmpOrder IsNot Nothing Then
                            Dim skip As Boolean = False
                            Dim tempPOGLS As IList(Of Member) = GetMembersFromMember()
                            For Each a As Member In tempPOGLS
                                If a.ID = tmpOrder.ID Then
                                    skip = True
                                    Exit For
                                End If
                            Next
                            If Not skip Then
                                update(loMemberList, loSelectionList, tmpOrder)
                            End If

                        End If
                    Next

                    doFilter()
                End If
            End If
        End If
    End Sub

    Protected Sub dragAndDrop_removeMember(ByVal sender As Object, ByVal e As GridDragDropEventArgs)
        If String.IsNullOrEmpty(e.HtmlElement) Then
            If e.DraggedItems(0).OwnerGridID = grdMembers.ClientID Then
                ' items are drag from pending to shipped grid 
                Dim memberList As IList(Of Member) = GetMembersFromMember()
                If (e.DestDataItem Is Nothing AndAlso memberList.Count = 0) OrElse (e.DestDataItem IsNot Nothing AndAlso e.DestDataItem.OwnerGridID = grdSelection.ClientID) Then
                    Dim loSelectionList As IList(Of Member)

                    loSelectionList = SelectionList

                    For Each draggedItem As GridDataItem In e.DraggedItems
                        Dim id As String = ""
                        For Each tc As TableCell In draggedItem.Cells
                            For Each Control In tc.Controls
                                Select Case Control.ID
                                    Case "ID"
                                        id = Control.Text
                                        Exit For
                                End Select
                            Next
                        Next

                        Dim tmpOrder As Member = GetMember(memberList, id)

                        If tmpOrder IsNot Nothing Then
                            delete(memberList, loSelectionList, tmpOrder)
                        End If
                    Next

                    doFilter()
                End If
            End If
        End If
    End Sub

#End Region

End Class

