Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Globalisation
Imports Arco.Doma.Library.Website
Imports Arco.Doma.WebControls.StackedIconBuilder

Namespace Doma

    Partial Class DM_ACL
        Inherits BasePage

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

        Private mbEditMode As Boolean
        Private _readOnly As Boolean
        Private msDomainName As String = ""
        Private mlObjectID As Int32
        Private mlQueryID As Int32
        Private mlInheritedObjectID As Int32
        Private msRootCaption As String = ""

        Private Img_Remove As String
        Private Img_Add As String

        Private mRoleLabels As LABELList
        Public ReadOnly Property RoleLabels() As LABELList
            Get
                If mRoleLabels Is Nothing Then
                    mRoleLabels = LABELList.GetRolesLabelList(EnableIISCaching)
                End If
                Return mRoleLabels
            End Get
        End Property

        Private _propLabels As LABELList
        Public ReadOnly Property PropertyLabels() As LABELList
            Get
                If _propLabels Is Nothing Then
                    _propLabels = LABELList.GetPropertiesLabelList(EnableIISCaching)
                End If
                Return _propLabels
            End Get
        End Property
        Private _packLabels As LABELList
        Public ReadOnly Property PackageLabels() As LABELList
            Get
                If _packLabels Is Nothing Then
                    _packLabels = LABELList.GetPackagesLabelList(EnableIISCaching)
                End If
                Return _packLabels
            End Get
        End Property
        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
            Img_Remove = "<span class='icon icon-delete' title='" & GetLabel("remove") & "' ></span>"
            Img_Add = "<span class='icon icon-add-new' title='" & GetLabel("add") & "' ></span>"

            lblWarning.Visible = False

            msRootCaption = Settings.GetValue("Doma", "RootCaption", "")
            Dim defaultShowUsers As Boolean = Settings.GetValue("UserBrowser", "DefaultShowUsers", True)
            Dim defaultShowGroups As Boolean = Settings.GetValue("UserBrowser", "DefaultShowGroups", True)
            Dim defaultShowRoles As Boolean = Settings.GetValue("UserBrowser", "DefaultShowRoles", True)


            ' Object / Screen / Query
            If Not IsPostBack Then
                mlObjectID = QueryStringParser.GetInt("DM_OBJECT_ID")
                txtObjectID.Value = mlObjectID.ToString
                If mlObjectID = 0 Then

                    mlQueryID = QueryStringParser.GetInt("QUERY_ID")
                    txtQueryID.Value = mlQueryID.ToString

                End If
            Else
                mlObjectID = Val(txtObjectID.Value)
                If mlObjectID = 0 Then

                    mlQueryID = Val(txtQueryID.Value)

                End If
            End If

            If mlQueryID > 0 Then
                Dim loAccess As QueryRights = QueryRights.GetQueryRights(mlQueryID)
                If Not loAccess.HasAccess(QueryRights.Query_Access_Level.ACL_ModifyQuery) Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                End If
                _readOnly = False
            ElseIf mlObjectID > 0 Then
                Dim loObj As DM_OBJECT = ObjectRepository.GetObject(mlObjectID)
                If loObj Is Nothing Then
                    loObj = ObjectRepository.GetObject(mlObjectID, True)
                    If Not loObj Is Nothing Then
                        loObj = ObjectRepository.GetObjectByDIN(loObj.DIN)
                        mlObjectID = loObj.ID
                    Else
                        GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                    End If
                End If
                If Not loObj.CanViewACL Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                Else
                    If Not loObj.CanSetACL Then
                        _readOnly = True
                    End If
                End If
            Else
                Dim rootFolder As Folder = Folder.GetRoot()
                If Not (rootFolder.HasAccess(ACL_Access.Access_Level.ACL_ACL_Folder) OrElse rootFolder.HasAccess(ACL_Access.Access_Level.ACL_View_ACL_Folder)) Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                End If
                _readOnly = Not rootFolder.HasAccess(ACL_Access.Access_Level.ACL_ACL_Folder)
            End If

            If Not IsPostBack Then
                cmbDomains.Items.Clear()
                cmbDomains.Items.Add("")
                For Each loDomain As DOMAINList.DOMAINSInfo In ACL.DOMAINList.GetDOMAINSList(True)
                    cmbDomains.Items.Add(loDomain.DOMAIN_NAME)
                Next

                If cmbDomains.Items.Count > 2 Then
                    lblDomain.Visible = True
                    cmbDomains.Visible = True
                    chkGroups.Visible = True
                Else
                    lblDomain.Visible = False
                    cmbDomains.Visible = False
                    chkGroups.Visible = False
                End If

                chkRoles.Checked = defaultShowRoles
                chkGroups.Checked = defaultShowGroups
                chkUsers.Checked = defaultShowUsers
                chkProps.Checked = False 'todo param
                chkPacks.Checked = False
            End If

            msDomainName = cmbDomains.SelectedItem.Text
            SetLabels()
            DoActions()
            DoInit()
            LoadItems()

        End Sub

#Region " Labels "
        Private Sub SetLabels()

            lblHeader1.Text = GetLabel("name")
            lblHeader2.Text = GetLabel("description")
            lblHeader3.Text = GetLabel("name")
            Page.Title = GetLabel("permissions")
            lnkFilter.Text = "<span class='icon icon-search'></span>"
            lblDomain.Text = GetLabel("domain")
            lblFilter.Text = GetLabel("filter")
            chkUsers.Text = GetLabel("users")
            chkRoles.Text = GetLabel("roles")
            chkGroups.Text = GetLabel("groups")
            chkProps.Text = GetLabel("properties")
            chkPacks.Text = "Packages"
            lblInherits.Text = GetLabel("inherits") & ": "
            lblFooter.Text = ""

            If mlQueryID > 0 Then
                lblInherits.Visible = False
            End If

        End Sub

#End Region

#Region " Init "

        Private Sub DoInit()

            If mlQueryID > 0 Then
                InitQuery()
            Else
                InitObject()
            End If

        End Sub

        Private Sub InitQuery()

            Dim lsPHReset As String = ""
            Dim loQuery As DMQuery = DMQuery.GetQuery(mlQueryID)

            mbEditMode = True

            lblObjectType.Text = "Query"
            txtObjectname.Text = Server.HtmlEncode(loQuery.Name)

            lblInheritsFrom.Visible = False
            lblInherits.Visible = False
            lsPHReset = "&nbsp;<a href='javascript:SetSubscriptions();' class='ButtonLink'><b>" & GetLabel("setsubscriptions") & "</b></a>"

            phInheritance.Text = ""
            phReset.Text = lsPHReset

        End Sub


        Private Sub InitObject()

            Dim lsPHIneritance As String = ""
            Dim lsPHReset As String = ""

            If mlObjectID > 0 Then
                'force reload
                DM_OBJECT.ReloadInApplicationServer(mlObjectID)
                Dim loObject As DM_OBJECT = ObjectRepository.GetObject(mlObjectID)

                lblInheritsFrom.FolderID = loObject.ACL_ID
                If loObject.ID = loObject.ACL_ID Then
                    mbEditMode = True
                    lblObjectType.Text = GetLabel(loObject.Object_Type)
                    txtObjectname.Text = Server.HtmlEncode(loObject.Name)
                    If Not _readOnly Then
                        If loObject.HasChildren Then
                            lsPHReset = "&nbsp;<a href='javascript:ResetACL();' class='ButtonLink'><b>" & GetLabel("ResetChildNodes") & "</b></a>"
                        Else
                            lsPHReset = ""
                        End If
                        lsPHReset = lsPHReset & "<br><br><a href='javascript:TakeParentACL();' class='ButtonLink'><b>" & GetLabel("TakeParentACL") & "</b></a>"
                    End If
                Else
                    mbEditMode = False
                    mlInheritedObjectID = loObject.ACL_ID
                    lblObjectType.Text = GetLabel(loObject.Object_Type)
                    txtObjectname.Text = Server.HtmlEncode(loObject.Name)
                    If Not _readOnly Then
                        lsPHIneritance = "&nbsp;<a href='javascript:Override();' class='ButtonLink'><b>" & GetLabel("override") & "</b></a>"
                    End If
                End If

            Else

                lblInheritsFrom.FolderID = 0
                mbEditMode = True
                lblObjectType.Text = "Root"

                txtObjectname.Text = msRootCaption

                lsPHIneritance = ""
                If Not _readOnly Then
                    lsPHReset = ""
                End If

            End If

            phInheritance.Text = lsPHIneritance
            phReset.Text = lsPHReset

        End Sub

#End Region

#Region " Load Items "


        Protected MaxGroups As Int32 = 20
        Protected MaxUsers As Int32 = 100

        Private ReadOnly Property Writeable() As Boolean
            Get
                Return mbEditMode AndAlso Not _readOnly
            End Get
        End Property

        Private Function GetAddAclCell(type As String, id As String) As TableCell
            Dim td As New TableCell
            If Writeable() Then
                td.Controls.Add(AddAclLink(type, id))
            End If
            Return td
        End Function

        Private Sub LoadItems()

            Dim loTR As TableRow

            Dim lbIsObjectACL As Boolean = (mlQueryID = 0)

            If lbIsObjectACL AndAlso chkUsers.Checked Then
                If Settings.GetValue("Security", "EnableGuestAccess", False) Then
                    loTR = New TableRow
                    loTR.Controls.Add(GetAddAclCell("Predefined", "GUEST"))
                    loTR.Controls.Add(New TableCell With {.Text = GetSubjectTypeImage("User") & "&nbsp;Guest"})
                    loTR.Controls.Add(New TableCell With {.Text = "The guest user"})


                    DomainList.Controls.Add(loTR)
                End If
                loTR = New TableRow
                loTR.Controls.Add(GetAddAclCell("Predefined", "_OWNER"))
                loTR.Controls.Add(New TableCell With {.Text = GetSubjectTypeImage("User") & "&nbsp;_OWNER"})
                loTR.Controls.Add(New TableCell With {.Text = "The owner of the folder/document"})

                DomainList.Controls.Add(loTR)

                loTR = New TableRow
                loTR.Controls.Add(GetAddAclCell("Predefined", "_WORKEXECUTOR"))
                loTR.Controls.Add(New TableCell With {.Text = GetSubjectTypeImage("User") & "&nbsp;_WORKEXECUTOR"})
                loTR.Controls.Add(New TableCell With {.Text = "Anyone having work access on this object"})


                DomainList.Controls.Add(loTR)
            End If

            If lbIsObjectACL AndAlso chkProps.Checked Then

                For Each prop As PROPERTYList.PROPERTYInfo In PROPERTYList.GetSecurityEnabledProperties(txtFilter.Text)
                    loTR = New TableRow
                    loTR.Controls.Add(GetAddAclCell("Assignee", prop.ID.ToString))
                    loTR.Controls.Add(New TableCell With {.Text = GetSubjectTypeImage("Assignee") & "&nbsp;" & PropertyLabels.GetObjectLabel(prop.ID, "Property", prop.Name) & CategoryLabel(prop)})
                    loTR.Controls.Add(New TableCell With {.Text = prop.Description})


                    DomainList.Controls.Add(loTR)
                Next

            End If

            If lbIsObjectACL AndAlso chkPacks.Checked Then

                loTR = New TableRow
                loTR.Controls.Add(GetAddAclCell("PackageView", "0"))
                loTR.Controls.Add(New TableCell With {.Text = GetSubjectTypeImage("PackageView") & "&nbsp;View any package"})
                loTR.Controls.Add(New TableCell With {.Text = ""})


                DomainList.Controls.Add(loTR)

                loTR = New TableRow
                loTR.Controls.Add(GetAddAclCell("PackageEdit", "0"))
                loTR.Controls.Add(New TableCell With {.Text = GetSubjectTypeImage("PackageEdit") & "&nbsp;Edit any package"})
                loTR.Controls.Add(New TableCell With {.Text = ""})


                DomainList.Controls.Add(loTR)

                Dim crit = New PackageList.Criteria With {.Filter = txtFilter.Text, .Securable = True}

                For Each loPack As PackageList.PackageInfo In PackageList.GetPackageList(crit)
                    loTR = New TableRow
                    loTR.Controls.Add(GetAddAclCell("PackageView", loPack.ID.ToString))
                    loTR.Controls.Add(New TableCell With {.Text = GetSubjectTypeImage("PackageView") & "&nbsp;View " & PackageLabels.GetObjectLabel(loPack.ID, "Package", loPack.Name) & CategoryLabel(loPack)})
                    loTR.Controls.Add(New TableCell With {.Text = loPack.Description})

                    DomainList.Controls.Add(loTR)

                    loTR = New TableRow
                    loTR.Controls.Add(GetAddAclCell("PackageEdit", loPack.ID.ToString))
                    loTR.Controls.Add(New TableCell With {.Text = GetSubjectTypeImage("PackageEdit") & "&nbsp;Edit " & PackageLabels.GetObjectLabel(loPack.ID, "Package", loPack.Name) & CategoryLabel(loPack)})
                    loTR.Controls.Add(New TableCell With {.Text = loPack.Description})

                    DomainList.Controls.Add(loTR)
                Next
            End If

            If chkRoles.Checked Then
                For Each loRole As ROLEList.ROLEInfo In ROLEList.GetROLEList(txtFilter.Text, Settings.GetValue("Security", "EnableRoleEveryone", True))

                    loTR = New TableRow
                    loTR.Controls.Add(GetAddAclCell("Role", loRole.Name))

                    Dim loTD As New TableCell
                    loTD.Wrap = True
                    loTD.Text = GetSubjectTypeImage("Role") & "&nbsp;"
                    If loRole.ID = 0 Then 'everyone role
                        loTD.Text &= GetLabel("everyone")
                    Else
                        loTD.Text &= "<a href='javascript:ShowMembers(" & EncodingUtils.EncodeJsString(loRole.Name) & "," & EncodingUtils.EncodeJsString("Role") & ");'>" & Role.GetDisplayValue(loRole.ID, loRole.Name, RoleLabels) & "</a>"
                    End If
                    loTR.Controls.Add(loTD)
                    loTR.Controls.Add(New TableCell With {.Text = ArcoFormatting.FormatText(loRole.Description, 50, Nothing, True, False, Nothing, Nothing)})


                    DomainList.Controls.Add(loTR)
                Next
            End If

            If Not Domain.IsDummy(msDomainName) And chkGroups.Checked Then
                Dim iGrp As Int32 = 0
                For Each loGroup As GROUPList.GROUPSInfo In GROUPList.GetGROUPSList(msDomainName, txtFilter.Text)
                    If iGrp < MaxGroups Then
                        loTR = New TableRow
                        loTR.Controls.Add(GetAddAclCell("Group", loGroup.GROUP_NAME))
                        loTR.Controls.Add(New TableCell With {.Text = GetSubjectTypeImage("Group") & "&nbsp;<a href='javascript:ShowMembers(" & EncodingUtils.EncodeJsString(loGroup.GROUP_NAME) & "," & EncodingUtils.EncodeJsString("Group") & ");'>" & loGroup.GROUP_NAME & "</a>"})
                        loTR.Controls.Add(New TableCell With {.Text = Server.HtmlEncode(loGroup.GROUP_DESC), .Wrap = True})


                        DomainList.Controls.Add(loTR)
                        iGrp += 1
                    Else
                        loTR = New TableRow
                        Dim loTD As New TableCell
                        If Writeable Then
                            loTD.ColumnSpan = 3
                        Else
                            loTD.ColumnSpan = 2
                        End If
                        loTD.Text = "Please filter..."
                        loTR.Controls.Add(loTD)
                        DomainList.Controls.Add(loTR)
                        Exit For
                    End If

                Next
            End If

            If chkUsers.Checked Then
                Dim iUsr As Int32 = 0
                For Each loUser As USERList.USERSInfo In USERList.GetUSERSList(msDomainName, txtFilter.Text)

                    If iUsr < MaxUsers Then

                        loTR = New TableRow
                        loTR.Controls.Add(GetAddAclCell("User", loUser.USER_ACCOUNT))
                        loTR.Controls.Add(New TableCell With {.Text = GetSubjectTypeImage("User") & "&nbsp;" & Server.HtmlEncode(loUser.USER_LOGIN)})
                        loTR.Controls.Add(New TableCell With {.Text = Server.HtmlEncode(loUser.GetDisplayName(loUser.USER_DESC)), .Wrap = True})

                        DomainList.Controls.Add(loTR)
                        iUsr += 1
                    Else
                        loTR = New TableRow
                        Dim loTD As New TableCell With {.Text = "Please filter..."}
                        If Writeable Then
                            loTD.ColumnSpan = 3
                        Else
                            loTD.ColumnSpan = 2
                        End If
                        loTR.Controls.Add(loTD)
                        DomainList.Controls.Add(loTR)
                        Exit For
                    End If

                Next
            End If

            If mlQueryID > 0 Then
                LoadItemsQuery()
            Else
                LoadItemsObject()
            End If

        End Sub
        Private Function AddAclLink(ByVal type As String, ByVal id As String) As HyperLink
            Dim loAdd As HyperLink = New HyperLink
            loAdd.NavigateUrl = String.Format("javascript:AddAcl({0},{1});", EncodingUtils.EncodeJsString(type), EncodingUtils.EncodeJsString(id))
            loAdd.Text = "<span class='icon icon-add-new'></span>"
            loAdd.ToolTip = GetLabel("add")
            loAdd.CssClass = "ButtonLink"
            Return loAdd
        End Function
        Private Sub LoadItemsQuery()
            Dim lsOldType As String = "", lsOldID As String = ""
            Dim lsProfile As String = ""

            Const liProf_Edit As Int32 = 1
            Const liProf_Run As Int32 = 2
            Const liProf_RunSub As Int32 = 3

            Const lsProf_Edit As String = "Edit Query"
            Const lsProf_Run As String = "Run Query"
            Const lsProf_RunSub As String = "Run Query Subscription"

            Dim aclList As QUERY_ACLList = QUERY_ACLList.GetQUERY_ACLList(mlQueryID)
            Dim subjects As List(Of Tuple(Of String, String)) = aclList.Cast(Of QUERY_ACLList.QUERY_ACLInfo).Select(Of Tuple(Of String, String))(Function(x) New Tuple(Of String, String)(x.SUBJECT_ID, x.SUBJECT_TYPE)).Distinct().ToList

            For Each subject As Tuple(Of String, String) In subjects
                Dim lbEdit As Boolean = False
                Dim lbRun As Boolean = False
                Dim lbRunSub As Boolean = False
                Dim lsTD As String = ""
                For Each loACL As QUERY_ACLList.QUERY_ACLInfo In aclList
                    If loACL.SUBJECT_ID = subject.Item1 AndAlso loACL.SUBJECT_TYPE = subject.Item2 Then
                        If loACL.RIGHTS = liProf_Edit Then
                            lbEdit = True
                        ElseIf loACL.RIGHTS = liProf_Run Then
                            lbRun = True
                        ElseIf loACL.RIGHTS = liProf_RunSub Then
                            lbRunSub = True
                        End If
                    End If
                Next

                AddUserGroupRole(subject.Item2, subject.Item1)
                If lbEdit Then
                    lsTD &= HREF_OK(lsProf_Edit, subject.Item2, subject.Item1, liProf_Edit)
                Else
                    lsTD &= HREF_NOK(lsProf_Edit, subject.Item2, subject.Item1, liProf_Edit)
                End If
                If lbRun Then
                    lsTD &= HREF_OK(lsProf_Run, subject.Item2, subject.Item1, liProf_Run)
                Else
                    lsTD &= HREF_NOK(lsProf_Run, subject.Item2, subject.Item1, liProf_Run)
                End If
                If lbRunSub Then
                    lsTD &= HREF_OK(lsProf_RunSub, subject.Item2, subject.Item1, liProf_RunSub)
                Else
                    lsTD &= HREF_NOK(lsProf_RunSub, subject.Item2, subject.Item1, liProf_RunSub)
                End If

                Dim loTR As New TableRow
                Dim loTD As New TableCell With {
                    .ColumnSpan = 2,
                    .Text = lsTD
                }
                loTR.Controls.Add(loTD)
                ObjectPermissions.Controls.Add(loTR)

            Next

        End Sub



        Private mcolCats As OBJECT_CATEGORYList = Nothing
        Private Function CategoryLabel(ByVal voPack As IPackage) As String
            Dim lsCatLabel As String = ""
            If voPack.ProcID > 0 Then

            ElseIf voPack.CatID > 0 Then
                lsCatLabel = CategoryLabel(voPack.CatID)
            Else
                '    lsCatLabel = " [Pool]"
            End If
            Return lsCatLabel
        End Function
        Private Function CategoryLabel(ByVal voProp As IProperty) As String
            Dim lsCatLabel As String = ""
            If voProp.ProcID > 0 Then

            ElseIf voProp.CatID > 0 Then
                lsCatLabel = CategoryLabel(voProp.CatID)
            Else
                '    lsCatLabel = " [Pool]"
            End If
            Return lsCatLabel
        End Function

        Private Function CategoryLabel(ByVal vlCatID As Int32) As String

            If mcolCats Is Nothing Then
                mcolCats = OBJECT_CATEGORYList.GetOBJECT_CATEGORYList(False, Me.EnableIISCaching)
            End If

            For Each loCat As OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In mcolCats
                If loCat.ID = vlCatID Then
                    Return " [" & GetCategoryLabel(loCat) & "]"
                End If
            Next
            Return ""
        End Function

        Private Sub LoadItemsObject()

            Dim lcolACLs As OBJECT_ACLList
            If mbEditMode Then
                lcolACLs = OBJECT_ACLList.GetOBJECT_ACLList(txtObjectID.Value)
            Else
                lcolACLs = OBJECT_ACLList.GetOBJECT_ACLList(mlInheritedObjectID)
            End If
            For Each loACL As OBJECT_ACLList.OBJECT_ACLInfo In lcolACLs

                Dim lbEditable As Boolean = (Writeable AndAlso loACL.CanEdit)
                Dim loTR As New TableRow
                loTR.ID = "ACL" & loACL.ACL_ID
                Dim loTD As New TableCell
                Dim lsFormat As String = "b"
                If loACL.ACL_ID <> loACL.PARENT_ACL_ID Then
                    lsFormat = "i"
                End If
                loTD.Text = GetSubjectTypeImage(loACL.SUBJECT_TYPE) & "&nbsp;<" & lsFormat & ">"
                Select Case loACL.SUBJECT_TYPE
                    Case "User"
                        loTD.Text &= ACL.User.RemoveDummyDomain(loACL.SUBJECT_ID)
                    Case "Group"
                        loTD.Text &= "<a href='javascript:ShowMembers(" & EncodingUtils.EncodeJsString(loACL.SUBJECT_ID) & "," & EncodingUtils.EncodeJsString("Group") & ");'>" & loACL.SUBJECT_ID & "</a>"
                    Case "Role"
                        If loACL.SUBJECT_ID = "Everyone" Then
                            loTD.Text &= GetLabel("everyone")
                        Else
                            loTD.Text &= "<a href='javascript:ShowMembers(" & EncodingUtils.EncodeJsString(loACL.SUBJECT_ID) & "," & EncodingUtils.EncodeJsString("Role") & ");'>" & loACL.SUBJECT_ID & "</a>"
                        End If
                    Case "Predefined"
                        loTD.Text &= loACL.SUBJECT_ID
                    Case "Assignee"
                        Dim loProp As DM_PROPERTY = DM_PROPERTY.GetPROPERTY(Convert.ToInt32(loACL.SUBJECT_ID))
                        loTD.Text &= "<" & lsFormat & ">" & PropertyLabels.GetObjectLabel(loProp.ID, "Property", loProp.Name) & CategoryLabel(loProp) & "</" & lsFormat & ">"
                    Case "PackageView", "PackageEdit"
                        Dim action As String
                        If loACL.SUBJECT_TYPE = "PackageView" Then
                            action = "View "
                        Else
                            action = "Edit "
                        End If
                        If loACL.SUBJECT_ID = 0 Then
                            loTD.Text &= action & "any package"
                        Else
                            Dim loPack As Package = Package.GetPackage(Convert.ToInt32(loACL.SUBJECT_ID))

                            loTD.Text &= action & PackageLabels.GetObjectLabel(loPack.ID, "Package", loPack.Name) & CategoryLabel(loPack)
                        End If

                End Select
                loTD.Text &= "</" & lsFormat & ">"
                loTR.Controls.Add(loTD)
                loTD = New TableCell
                If lbEditable Then
                    Dim loRemove As New HyperLink
                    loRemove.NavigateUrl = "javascript:RemoveAcl(" & loACL.ACL_ID & ");"
                    loRemove.Text = ThemedImage.GetSpanIconTag("icon icon-delete", GetLabel("remove"))
                    loRemove.CssClass = "ButtonLink"
                    loTD.Controls.Add(loRemove)
                    If loACL.ACL_ID <> loACL.PARENT_ACL_ID Then
                        loTD.Controls.Add(New LiteralControl("&nbsp;"))
                        Dim unlink As New HyperLink
                        unlink.NavigateUrl = "javascript:UnlinkAcl(" & loACL.ACL_ID & ");"
                        'loUnlink.ImageUrl = ThemedImage.GetUrl("edit.png", Page)
                        unlink.Text = ThemedImage.GetSpanIconTag("icon icon-edit", GetLabel("override"))
                        unlink.CssClass = "ButtonLink"
                        loTD.Controls.Add(unlink)
                    Else
                        loTD.Controls.Add(New LiteralControl("&nbsp;"))
                        Dim lockLink As New HyperLink
                        lockLink.NavigateUrl = "javascript:LockAcl(" & loACL.ACL_ID & ");"
                        If loACL.Fixed Then
                            lockLink.Text = ThemedImage.GetSpanIconTag("icon icon-status-locked", GetDecodedLabel("unlock"))
                        Else
                            lockLink.Text = ThemedImage.GetSpanIconTag("icon icon-status-unlocked", GetDecodedLabel("lock"))  'show current status icon, so status unlocked if action is locking.
                        End If
                        lockLink.CssClass = "ButtonLink"
                        loTD.Controls.Add(lockLink)

                        loTD.Controls.Add(New LiteralControl("&nbsp;"))
                        Dim loPropagLink As New HyperLink
                        loPropagLink.NavigateUrl = "javascript:PropagateAcl(" & loACL.ACL_ID & ");"
                        loPropagLink.Text = ThemedImage.GetSpanIconTag("icon icon-file-copy", GetDecodedLabel("copyacltochildren"))
                        loPropagLink.CssClass = "ButtonLink"
                        loTD.Controls.Add(loPropagLink)
                    End If

                End If
                loTR.Controls.Add(loTD)
                ObjectPermissions.Controls.Add(loTR)
                Dim lcolRights As ACL_RIGHTSList = ACL_RIGHTSList.GetACL_RIGHTSList(loACL.ACL_ID)
                loTR = New TableRow
                loTD = New TableCell
                loTD.ColumnSpan = 2
                Dim loCMBAdd As New DropDownList
                Dim lbItemsToAdd As Boolean = False
                If lbEditable Then
                    loCMBAdd.Text = GetLabel("add")
                    loCMBAdd.EnableViewState = False
                    loCMBAdd.Items.Add(New System.Web.UI.WebControls.ListItem("", "0"))
                    loCMBAdd.Attributes.Add("onchange", "javascript:SetAclLevel(" & loACL.ACL_ID & ",this.value,1);")
                End If

                For Each loProfile As ACL_PROFILE_LIST.PROFILEInfo In ACL_PROFILE_LIST.GetPROFILEList()
                    If lcolRights.Cast(Of ACL_RIGHTSList.ACL_RIGHTSInfo).Any(Function(x) x.Profile_ID = loProfile.ID) Then
                        AddAclRight(loTD, loProfile, loACL.ACL_ID, lbEditable)
                    Else
                        If lbEditable Then
                            lbItemsToAdd = True
                            loCMBAdd.Items.Add(New System.Web.UI.WebControls.ListItem(loProfile.Name, loProfile.ID))
                        End If
                    End If
                Next

                loTR.Controls.Add(loTD)
                ObjectPermissions.Controls.Add(loTR)

                Dim loLit As Literal
                If lbEditable AndAlso lbItemsToAdd Then
                    loLit = New Literal
                    loLit.Text = GetLabel("add") & " : "

                    loTR = New TableRow
                    loTD = New TableCell
                    loTD.ColumnSpan = 2
                    loTD.Controls.Add(loLit)
                    loTD.Controls.Add(loCMBAdd)
                    loTR.Controls.Add(loTD)
                    ObjectPermissions.Controls.Add(loTR)
                End If
                If Writeable Then
                    'loLit = New Literal
                    'loLit.Text = "<hr>"
                    loTR = New TableRow
                    'loTD = New TableCell
                    'loTD.ColumnSpan = 2
                    'loTD.Controls.Add(loLit)
                    'loTR.Controls.Add(loTD)
                    loTR.Style.Add("border-bottom", "none")
                    loTR.Height = "30"
                    ObjectPermissions.Controls.Add(loTR)
                End If
            Next

        End Sub

        ''' <summary>
        ''' Adds a link to add the acl
        ''' </summary>      
        Private Function HREF_NOK(ByVal name As String, ByVal vsSubjectType As String,
                             ByVal vsSubjectID As String,
                             ByVal viRights As Int32) As String

            Return "<a href='javascript:SetAclLevel2(" &
                                           EncodingUtils.EncodeJsString(vsSubjectType) & "," &
                                           EncodingUtils.EncodeJsString(vsSubjectID) & "," &
                                          viRights & "," &
                                          "1);'>" & Img_Add & "</a>&nbsp;" & name & "<br>"

        End Function
        ''' <summary>
        ''' Adds a link to remove the acl
        ''' </summary>
        Private Function HREF_OK(ByVal name As String, ByVal vsSubjectType As String,
                                   ByVal vsSubjectID As String,
                                   ByVal viRights As Int32) As String

            Return "<a href='javascript:SetAclLevel2(" & EncodingUtils.EncodeJsString(vsSubjectType) & "," &
                                                EncodingUtils.EncodeJsString(vsSubjectID) & "," &
                                                viRights & "," &
                                                "0);'>" & Img_Remove & "</a>&nbsp;" & name & "<br>"

        End Function
        'Private Sub AddCanBrowse(ByRef roTD As TableCell)
        '    roTD.Text &= GetLabel("browse") & "<br>"
        'End Sub
        Private Sub AddAclRight(ByRef roTD As TableCell, ByRef voProfile As ACL_PROFILE_LIST.PROFILEInfo, ByVal viACLID As Int32, ByVal vbEditable As Boolean)
            Dim lsNameLink As String = "<a href='javascript:ViewProfile(" & voProfile.ID & "," & mlObjectID & ");'>" & voProfile.Name & "</a>"

            If vbEditable Then
                roTD.Text &= "<a href='javascript:SetAclLevel(" & viACLID & "," & voProfile.ID & ",0);'>" & Img_Remove & "</a>&nbsp;"
            Else
                roTD.Text &= "&nbsp;"
            End If

            roTD.Text &= lsNameLink & "<br>"

        End Sub

        Private Function GetSubjectTypeImage(ByVal type As String) As String
            Select Case type
                Case "Predefined"
                    Return "<span class='icon icon-user-profile icon-color-light' title='" & type & "'></span>"
                Case "Assignee"
                    Dim stackedIcons = New List(Of StackedIcon)() From {
                       New StackedIcon With {
                           .IconClass = "icon icon-user-group icon-color-light",
                           .Size = StackedIconSize.M
                       },
                       New StackedIcon With {
                           .IconClass = "icon icon-at-sign icon-color-light",
                           .Size = StackedIconSize.S,
                           .HorizontalAlignment = HorizontalAlign.Right,
                           .VerticalAlignment = VerticalAlign.Bottom
                       }
                    }

                    Return StackedIconBuilder.CreateStackedIcon(stackedIcons, attributes:="title='" & type & "'")
                Case "PackageView"
                    Return "<span class='icon icon-color-light icon-package' title='" & type & "'></span>"
                Case "PackageEdit"
                    Dim stackedIcons = New List(Of StackedIcon)() From {
                        New StackedIcon With {
                            .IconClass = "icon icon-color-light icon-package",
                            .Size = StackedIconSize.M
                        },
                        New StackedIcon With {
                            .IconClass = "icon icon-color-light icon-edit",
                            .Size = StackedIconSize.S,
                            .HorizontalAlignment = HorizontalAlign.Right,
                            .VerticalAlignment = VerticalAlign.Bottom,
                            .TagAttributes = "style='cursor: default !important; '"
                        }
                    }

                    Return StackedIconBuilder.CreateStackedIcon(stackedIcons, attributes:="title='" & type & "'")
                Case Else
                    Return ThemedImage.GetSpanIconTag("icon icon-color-light icon-user-" & type.ToLower(), type)
            End Select
        End Function

        Private Sub AddUserGroupRole(ByVal lsSubjectType As String,
                                      ByVal lsSubjectID As String)

            Dim loTR As TableRow = New TableRow
            Dim loTD As TableCell = New TableCell

            loTR.ID = GetTDID(lsSubjectType & "_" & lsSubjectID)
            loTD.Text = GetSubjectTypeImage(lsSubjectType) & "&nbsp;"
            Select Case lsSubjectType
                Case "User"
                    loTD.Text &= "<b>" & ACL.User.RemoveDummyDomain(lsSubjectID) & "</b>"
                Case "Group"
                    loTD.Text &= "<a href='javascript:ShowMembers(" & EncodingUtils.EncodeJsString(lsSubjectID) & "," & EncodingUtils.EncodeJsString("Group") & ");'><b>" & lsSubjectID & "</b></a>"
                Case "Role"
                    If lsSubjectID = "Everyone" Then
                        loTD.Text &= "<b>" & GetLabel("everyone") & "</b>"
                    Else
                        loTD.Text &= "<a href='javascript:ShowMembers(" & EncodingUtils.EncodeJsString(lsSubjectID) & "," & EncodingUtils.EncodeJsString("Role") & ");'><b>" & lsSubjectID & "</b></a>"
                    End If
                Case "Predefined"
                    loTD.Text &= "<b>" & lsSubjectID & "</b>"
                Case "Assignee"
                    loTD.Text &= "<b>" & DM_PROPERTY.GetPROPERTY(Convert.ToInt32(lsSubjectID)).Name & "</b>"
            End Select
            loTR.Controls.Add(loTD)

            loTD = New TableCell
            If Writeable Then
                Dim loRemove As HyperLink = New HyperLink
                loRemove.NavigateUrl = "javascript:RemoveAcl2(" & EncodingUtils.EncodeJsString(lsSubjectType) & "," & EncodingUtils.EncodeJsString(lsSubjectID) & ");"
                loRemove.Text = GetLabel("Remove")
                loRemove.ToolTip = GetLabel("Remove")
                loTD.Controls.Add(loRemove)
            End If
            loTR.Controls.Add(loTD)

            ObjectPermissions.Controls.Add(loTR)

        End Sub

#End Region

#Region " Do Actions "

        Private Sub DoActions()
            If _readOnly Then
                Return
            End If


            If mlQueryID > 0 Then
                DoActions_Query()
            Else
                DoActions_Object()
            End If

        End Sub

        Private Sub DoActions_Object()

            Dim lsAction As String
            Dim lsType As String
            Dim lsSubject As String
            Dim aclId As Int32
            Dim liMode As Int32
            Dim liProfile As Int32

            Dim loObject As DM_OBJECT = ObjectRepository.GetObject(mlObjectID)

            'shouldn't happen but we can check
            If Not loObject.CanSetACL Then
                GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                Return
            End If

            Dim aclIdFormValue As String = Request.Form("aclid")
            If Not String.IsNullOrEmpty(aclIdFormValue) Then
                aclId = Convert.ToInt32(aclIdFormValue)
                'If aclId > 0 Then
                '    'check it belongs to this object
                'End If
            End If


            lsAction = Request.Form("aclaction")

            Select Case lsAction

                Case "A" 'add acl
                    lsType = Request.Form("subjecttype")
                    lsSubject = Request.Form("subjectid")

                    loObject.AddACL(lsSubject, lsType, True, False)

                Case "U"

                    Dim loACL As OBJECT_ACL = loObject.GetObjectAclToEdit(aclId)
                    If loACL IsNot Nothing Then
                        loACL.PARENT_ACL_ID = loACL.ACL_ID
                        loACL.Save()
                    End If


                Case "L"
                    Dim loACL As OBJECT_ACL = loObject.GetObjectAclToEdit(aclId)
                    If loACL IsNot Nothing Then
                        loACL.Fixed = Not loACL.Fixed
                        loACL.Save()
                    End If
                Case "P"
                    Dim loACL As OBJECT_ACL = loObject.GetObjectAclToEdit(aclId)
                    If loACL IsNot Nothing Then
                        loACL.PropagateToChildren()
                    End If
                Case "D" 'delete acl                 

                    loObject.RemoveACL(aclId)

                Case "S" 'toggle acl right

                    liProfile = Convert.ToInt32(Request.Form("accesslevel"))
                    liMode = Convert.ToInt32(Request.Form("accessmode"))
                    If liMode = 1 Then 'insert
                        loObject.AddACLRight(aclId, liProfile)
                    Else
                        loObject.RemoveACLRight(aclId, liProfile)
                    End If

                Case "R" 'reset children
                    Dim loReset As New ACL_RESET(mlObjectID)
                    loReset.Reset()

                Case "TP" 'take parent acl
                    loObject.TakeParentAcl()
                    _readOnly = True
                Case "O" 'override
                    If mlObjectID > 0 Then
                        'this can't happen on the root anyways
                        loObject.OverrideACL(True, True)
                    End If
                Case "OS" 'override special
                    If mlObjectID > 0 Then
                        'this can't happen on the root anyways
                        loObject.OverrideACL(True, False)
                    End If
            End Select


            Dim lbDoAutoFix As Boolean = False

            If Not _readOnly Then
                If Not loObject.CanSetACL Then
                    lblWarning.Visible = True
                    lblWarning.Text = GetLabel("aclwarning") & ". " & GetLabel("aclautofixapplied")
                    lbDoAutoFix = True
                End If

            End If

            If lbDoAutoFix Then 'autofix, give the current user full control
                lsType = "User"
                lsSubject = Arco.Security.BusinessIdentity.CurrentIdentity.Name
                Using New Security.RunElevated(True)

                    Dim llACLID As Int32 = 0
                    For Each loACLCheck As OBJECT_ACLList.OBJECT_ACLInfo In OBJECT_ACLList.GetOBJECT_ACLList(txtObjectID.Value)
                        If loACLCheck.SUBJECT_ID = lsSubject And loACLCheck.SUBJECT_TYPE = lsType Then
                            llACLID = loACLCheck.ACL_ID
                            Exit For
                        End If
                    Next

                    If llACLID = 0 Then
                        llACLID = loObject.AddACL(lsSubject, lsType, True, False)
                    End If

                    loObject.AddACLRight(llACLID, 40) 'give set acl profile
                End Using
            End If

        End Sub


        Private Sub DoActions_Query()

            Dim lsType As String
            Dim lsSubject As String
            Dim loACL As QUERY_ACL
            Dim i As Int32 = 0

            Dim lsAction As String = Request.Form("aclaction")

            Select Case lsAction
                Case "A" 'add acl
                    lsType = Request.Form("subjecttype")
                    lsSubject = Request.Form("subjectid")

                    Dim lbFound As Boolean = False
                    For Each loRightsRecord As QUERY_ACLList.QUERY_ACLInfo In QUERY_ACLList.GetQUERY_ACLList(mlQueryID)
                        If loRightsRecord.SUBJECT_ID = lsSubject And
                           loRightsRecord.SUBJECT_TYPE = lsType Then
                            lbFound = True
                            Exit For
                        End If
                    Next
                    If Not lbFound Then
                        ' Edit query
                        loACL = QUERY_ACL.NewQUERY_ACL()
                        loACL.QUERY_ID = mlQueryID
                        loACL.SUBJECT_TYPE = lsType
                        loACL.SUBJECT_ID = lsSubject
                        loACL.RIGHTS = 1
                        loACL.Save()
                        ' Use query
                        loACL = QUERY_ACL.NewQUERY_ACL()
                        loACL.QUERY_ID = mlQueryID
                        loACL.SUBJECT_TYPE = lsType
                        loACL.SUBJECT_ID = lsSubject
                        loACL.RIGHTS = 2
                        loACL.Save()
                        ' Use query subscription
                        loACL = QUERY_ACL.NewQUERY_ACL()
                        loACL.QUERY_ID = mlQueryID
                        loACL.SUBJECT_TYPE = lsType
                        loACL.SUBJECT_ID = lsSubject
                        loACL.RIGHTS = 3
                        loACL.Save()
                    End If

                Case "D" 'delete acl
                    lsType = Request.Form("subjecttype")
                    lsSubject = Request.Form("subjectid")
                    QUERY_ACL.DeleteQuery_ACL(mlQueryID, lsType, lsSubject)

                Case "S" 'toggle acl right
                    Dim liMode As Int32 = Convert.ToInt32(Request.Form("accessmode"))
                    Dim liProfile As Int32 = Convert.ToInt32(Request.Form("accesslevel"))
                    lsType = Request.Form("subjecttype")
                    lsSubject = Request.Form("subjectid")
                    If liMode = 1 Then 'insert
                        loACL = QUERY_ACL.NewQUERY_ACL()
                        loACL.QUERY_ID = mlQueryID
                        loACL.SUBJECT_TYPE = lsType
                        loACL.SUBJECT_ID = lsSubject
                        loACL.RIGHTS = liProfile
                        loACL.Save()
                    Else
                        QUERY_ACL.DeleteQUERY_ACL(mlQueryID, lsType, lsSubject, liProfile)
                    End If

                Case "QS" ' set subscriptions
                    Arco.Doma.Library.baseObjects.Subscription.DeleteQuerySubscriptions(mlQueryID)
                    Dim loACLList As QUERY_ACLList = QUERY_ACLList.GetQUERY_ACLList(mlQueryID, 3)
                    For i = 0 To loACLList.Count - 1
                        Dim loS As Arco.Doma.Library.baseObjects.Subscription = Arco.Doma.Library.baseObjects.Subscription.NewSubscription()
                        loS.Name = "Query - " & DMQuery.GetQuery(mlQueryID).Name
                        loS.Object_Type = "Query"
                        loS.Object_ID = mlQueryID
                        loS.Subject_Type = loACLList(i).SUBJECT_TYPE
                        loS.Subject_ID = loACLList(i).SUBJECT_ID
                        loS.Save()
                    Next
                    lblWarning.Visible = True
                    lblWarning.Text = GetLabel("subscriptioncreated")
            End Select


            Dim lbDoAutoFix As Boolean = False
            If Not _readOnly Then
                Dim loAccess As QUERY_ACLList = QUERY_ACLList.GetQUERY_ACLList(mlQueryID)
                If Not loAccess.HasAccess(1) Then
                    lblWarning.Visible = True
                    lblWarning.Text = GetLabel("aclwarning") & ". " & GetLabel("aclautofixapplied")
                    lbDoAutoFix = True
                End If

            End If

            If lbDoAutoFix Then 'autofix, give the current user edit rights
                Using New Security.RunElevated(True)
                    loACL = QUERY_ACL.NewQUERY_ACL()
                    loACL.QUERY_ID = mlQueryID
                    loACL.SUBJECT_TYPE = "User"
                    loACL.SUBJECT_ID = Arco.Security.BusinessIdentity.CurrentIdentity.Name
                    loACL.RIGHTS = 1
                    loACL.Save()
                End Using
            End If

        End Sub

#End Region

        Private Function GetTDID(ByVal vsText As String) As String

            vsText = vsText.Replace("\", "_")
            vsText = vsText.Replace("'", "_")
            vsText = vsText.Replace(" ", "_")

            Return vsText

        End Function

    End Class

End Namespace
