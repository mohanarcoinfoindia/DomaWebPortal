Imports System.Xml
Imports System.Web.Services
Imports Arco.Doma.Library
Imports Arco.Doma.Library.Helpers
Imports Arco.Doma.Library.Tree
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Website
Imports Arco.Doma.Library.Routing
Imports Arco.Doma.Library.Settings

<WebService(Namespace:="http://tempuri.org/")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<CompilerServices.DesignerGenerated()>
<System.Web.Script.Services.ScriptService()>
Public Class ContextMenus
    Inherits WebService

#Region " Generic "

    Private ctxItemsList As New List(Of ContextMenuItem)
    Private oItemsAdded As New HashSet(Of String)
    Private mbItemShown As Boolean
    Private _disabledItems As HashSet(Of String)
    Private _settings As ISettings

    Protected Function Result() As ContextMenuItem()
        If ctxItemsList.Count = 1 Then 'hack, arrays with a single item in can't be read so add an empty 2nd item
            Dim oMItem As New ContextMenuItem
            oMItem.Text = ""
            oMItem.NavigateUrl = ""
            ctxItemsList.Add(oMItem)
        End If

        Return ctxItemsList.ToArray
    End Function

    Protected Function GetLabel(ByVal vsLabel As String) As String
        Dim lsLang As String = Arco.Security.BusinessIdentity.CurrentIdentity.Language
        Return Arco.Web.ResourceManager.GetString(vsLabel.ToLower, lsLang)
    End Function

    Protected Sub AddItem(ByVal vsKey As String, ByVal vsLabel As String, ByVal vsUrl As String)
        AddItem(vsKey, vsLabel, vsUrl, "")
    End Sub

    Public ReadOnly Property Settings As ISettings
        Get
            If _settings Is Nothing Then
                _settings = Configuration.GetSettings
            End If
            Return _settings
        End Get
    End Property

    Private Function ShowItem(ByVal vsKey As String) As Boolean
        Return _disabledItems Is Nothing OrElse Not _disabledItems.Contains(vsKey.ToLower)

    End Function

    Protected Sub AddItem(ByVal vsKey As String, ByVal vsLabel As String, ByVal vsUrl As String, ByVal vsImg As String)
        AddItem(vsKey, vsLabel, vsUrl, vsImg, Nothing)
    End Sub

    Protected Sub AddItem(ByVal vsKey As String, ByVal vsLabel As String, ByVal vsUrl As String, ByVal vsImg As String, ByVal parent As String)

        If Not oItemsAdded.Contains(vsKey) AndAlso ShowItem(vsKey) Then
            Dim cntxItem As ContextMenuItem = New ContextMenuItem
            cntxItem.Value = vsKey
            cntxItem.Parent = parent
            cntxItem.Text = vsLabel

            cntxItem.NavigateUrl = "javascript:" & vsUrl


            cntxItem.isSeparator = False
            If Not String.IsNullOrEmpty(vsImg) Then
                cntxItem.Image = vsImg
            End If
            ctxItemsList.Add(cntxItem)
            oItemsAdded.Add(vsKey)
            mbItemShown = True
        End If
    End Sub

    Protected Sub AddSeparator()
        AddSeparator(Nothing)
    End Sub

    Protected Sub AddSeparator(ByVal parent As String)
        If mbItemShown OrElse Not String.IsNullOrEmpty(parent) Then
            Dim oMItem As ContextMenuItem = New ContextMenuItem
            oMItem.isSeparator = True
            oMItem.Parent = parent
            ctxItemsList.Add(oMItem)
            mbItemShown = False
        End If

    End Sub

    Private Sub LoadCtxDefinition(ByVal vsContextMenuTag As String, ByVal vlCatID As Int32, ByVal siteName As String, ByVal file As String)
        mbItemShown = False
        If String.IsNullOrEmpty(vsContextMenuTag) Then Return
        If String.IsNullOrEmpty(file) Then file = "ContextMenus.xml"

        Dim cacheKey As String
        Dim lsXMLFile As String
        If Not file.Contains("\") Then
            If String.IsNullOrEmpty(siteName) Then siteName = SiteManagement.SitesManager.CurrentSite

            cacheKey = siteName & ":" & file
            lsXMLFile = SiteManagement.SitesManager.GetFullFilePath(file, Nothing, siteName, "Default", True, True)
        Else
            cacheKey = file
            lsXMLFile = file
        End If

        Dim d As XmlDocument
        Dim loParentNode As XmlNode

        If HttpRuntime.Cache.Item(cacheKey) Is Nothing Then
            d = New XmlDocument
            d.Load(lsXMLFile)
            HttpRuntime.Cache.Insert(cacheKey, d, New CacheDependency(lsXMLFile), DateTime.MaxValue, TimeSpan.Zero)
        Else
            d = CType(HttpRuntime.Cache.Item(cacheKey), XmlDocument)
        End If
        loParentNode = d.SelectSingleNode("contextmenus")

        If Not loParentNode.SelectSingleNode(vsContextMenuTag & "_" & vlCatID) Is Nothing Then
            loParentNode = loParentNode.SelectSingleNode(vsContextMenuTag & "_" & vlCatID)
        Else
            loParentNode = loParentNode.SelectSingleNode(vsContextMenuTag)
        End If
        If loParentNode IsNot Nothing Then
            Dim disabledItemsNode As XmlNode = loParentNode.SelectSingleNode("disableditems")
            If disabledItemsNode IsNot Nothing Then
                _disabledItems = New HashSet(Of String)
                For Each n As XmlNode In disabledItemsNode.SelectNodes("item")
                    If String.IsNullOrEmpty(n.InnerText) Then
                        Continue For
                    End If
                    Dim forRolesNode As XmlNode = n.Attributes.GetNamedItem("roles")
                    If forRolesNode IsNot Nothing AndAlso Not String.IsNullOrEmpty(forRolesNode.Value) Then
                        If Not Security.BusinessIdentity.CurrentIdentity.IsInOneOfRoles(forRolesNode.Value) Then
                            Continue For
                        End If
                    End If

                    _disabledItems.Add(n.InnerText.ToLower)
                Next
            End If
        End If


    End Sub

#End Region

    <WebMethod(EnableSession:=True)>
    Public Function GetPackageContextItems(ByVal vlPackID As Int32, ByVal vlObjectID As Int32, ByVal siteName As String, ByVal file As String) As ContextMenuItem()
        Dim obj As DM_OBJECT = ObjectRepository.GetObject(vlObjectID)
        If obj IsNot Nothing Then
            LoadCtxDefinition("packagecontextmenu", vlPackID, siteName, file)

            If obj.CanAddToPackage(vlPackID) AndAlso obj.GetPackageInfo(vlPackID).AllowedContent.Any Then

                Dim lsGridXMlFile As String
                If (String.IsNullOrEmpty(obj.GetPackageInfo(vlPackID).DocInfo1)) Then
                    lsGridXMlFile = "PackageGrid.xml"
                Else
                    lsGridXMlFile = obj.GetPackageInfo(vlPackID).DocInfo1
                End If
                Select Case obj.GetPackageInfo(vlPackID).Type
                    Case Package.ePackType.Docroom
                        AddItem("new", GetLabel("new"), "DMC('Add');", "newdocument.svg")
                        AddItem("add", GetLabel("add"), "parent.PC().AddToObjectObjectPackage(" & obj.ID & "," & vlPackID & ",0,'" & DocroomListHelpers.GridLayout.LoadGridLayout(lsGridXMlFile).SelectionSite & "',false);", "AddToDossier.svg")

                        AddItem("addclipboard", GetLabel("addclipboard"), "DocroomListFunctions.AddSelectionToObjectPackage(" & obj.ID & ", " & vlPackID & ", 7,AddToPackageComplete);", "copyfromclipboard.svg")
                        If Settings.GetValue("Basket", "Enabled", True) Then
                            AddItem("addbasket", GetLabel("addbasket"), "DocroomListFunctions.AddSelectionToObjectPackage(" & obj.ID & ", " & vlPackID & ", 2,AddToPackageComplete);", "AddToBasket.svg")
                        End If
                    Case Package.ePackType.CasePackage
                        AddItem("add", GetLabel("new"), "DMC('Add');", "newdocument.svg")
                        AddItem("add", GetLabel("add"), "parent.PC().AddToObjectCasePackage(" & obj.ID & "," & vlPackID & ",0,'" & DocroomListHelpers.GridLayout.LoadGridLayout(lsGridXMlFile).SelectionSite & "',false);", "linkedAvailable.svg")
                End Select
                AddSeparator()
            End If

            AddItem("browse", GetLabel("browse"), "DMC('Browse');", "browse.svg")
            AddItem("search", GetLabel("search"), "DMC('Search');", "find.svg")
        Else
            AddItemNotFound(False)
        End If
        Return Result()

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetListContextItems(ByVal vlListID As Integer, ByVal siteName As String, ByVal file As String) As ContextMenuItem()
        Dim loList As OBJECT_CATEGORY = OBJECT_CATEGORY.GetOBJECT_CATEGORY(vlListID)
        If loList.ID = vlListID Then
            LoadCtxDefinition("treecontextmenu", loList.ID, siteName, file)

            AddItem("new", GetLabel("new"), "DML('Add');", "newdocument.svg")
            AddItem("browse", GetLabel("browse"), "DML('Browse');", "browse.svg")
            AddItem("search", GetLabel("search"), "DML('Search');", "find.svg")
        Else
            AddItemNotFound(False)
        End If
        Return Result()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetPropertyExpansionContextItems(ByVal DM_OBJECT_ID As Integer, ByVal siteName As String, ByVal vsValue As String, ByVal file As String) As ContextMenuItem()
        LoadCtxDefinition("treecontextmenu", 0, siteName, file)
        Dim obj As DM_OBJECT = ObjectRepository.GetObject(DM_OBJECT_ID)
        If obj IsNot Nothing Then
            AddQueryMenuItems(obj, vsValue, "")
        Else
            AddItemNotFound(False)
        End If
        Return Result()

    End Function

    Private Sub AddQueryMenuItems(ByVal obj As DM_OBJECT, ByVal vsQueryValue As String, ByVal extra As String)
        AddItem("edit", GetLabel("edit"), "DMC('EditQuery');", "Edit.svg")
        AddItem("refresh", GetLabel("refresh"), "ob_t2_Reload('f" & obj.ID & "');", "Refresh.svg")
        AddSeparator()

        Dim q As QueryShortcut = DirectCast(obj, QueryShortcut)

        If q.Query.Type = DMQuery.QueryType.PropertyExpansion Then
            If Not q.Query.isNonCombinablePropertyExpansion Then
                If obj.HasAccess(ACL.ACL_Access.Access_Level.ACL_Create_QueryShortcut) Then
                    AddItem("addpropexp", GetLabel("addpropexp"), "DMC('AddQuery');")
                    AddSeparator()
                End If

            End If
            If Not String.IsNullOrEmpty(vsQueryValue) Then
                'actual valiue, we can subscribe to it
                AddItem("subscribe", GetLabel("subscribe"), "parent.PC().SubscribeTo(" & q.ID & ",""Query""," & EncodingUtils.EncodeJsString(vsQueryValue) & ");")
            End If
        Else
            AddCopyPasteItems(obj)

            AddSeparator()


            AddExtraTreeItems(obj, extra)
            AddSeparator()

            If obj.HasAccess(ACL.ACL_Access.Access_Level.ACL_Create_QueryShortcut) Then
                AddItem("addpropexp", GetLabel("addpropexp"), "DMC('AddQuery');")
                AddSeparator()
            End If

            AddItem("subscribe", GetLabel("subscribe"), "parent.PC().SubscribeTo(" & q.ID & ",'Query','');", "newSubscription.svg")
        End If
        If String.IsNullOrEmpty(extra) Then
            If ShowItem("delete") AndAlso obj.CanDelete(False) Then
                AddItem("delete", GetLabel("ctx_delete"), "DMC('Delete');", "delete.svg")
            End If
        End If
    End Sub

    Private Sub AddCopyPasteItems(ByVal obj As DM_OBJECT)
        If obj.CanMove Then
            AddItem("cut", GetLabel("cut"), "DMC('Cut');", "cut.svg")
        End If
        If obj.CanCopy Then
            AddItem("copy", GetLabel("copy"), "DMC('Copy');", "copy.svg")
        End If

        AddItem("paste", GetLabel("paste"), "DMC('Paste');", "paste.svg")
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Function GetTreeContextItems(ByVal DM_OBJECT_ID As Integer, ByVal siteName As String, ByVal file As String, ByVal extra As String) As ContextMenuItem()
        Dim obj As DM_OBJECT = ObjectRepository.GetObject(DM_OBJECT_ID)
        If obj IsNot Nothing Then
            LoadCtxDefinition("treecontextmenu", obj.Category, siteName, file)
            Select Case obj.Object_Type
                Case "Folder", "Shortcut", "Dossier"

                    AddItem("new", GetLabel("new"), "DMC('Add');", "newdocument.svg")

                    Dim actualObject As DM_OBJECT = obj
                    If obj.Object_Type = "Shortcut" Then
                        actualObject = DirectCast(obj, Shortcut).GetReferencedObject
                    End If


                    Select Case actualObject.Object_Type
                        Case "Dossier"
                            If DirectCast(actualObject, Dossier).HasSubDossiers Then
                                AddItem("browse", GetLabel("browsesubdossiers"), "DMC('Browse');", "browse.svg")
                            End If
                            AddItem("search", GetLabel("search"), "DMC('Search');", "find.svg")
                            AddItem("browsewithsubfolders", GetLabel("browsedossiercontent"), "DMC('BrowseWithSubFolders');", "browse.svg")
                            If actualObject.CanViewMeta Then
                                AddItem("open", GetLabel("open"), "DMC('Explore');")
                            End If

                        Case Else
                            AddItem("browse", GetLabel("browse"), "DMC('Browse');", "browse.svg")
                            AddItem("search", GetLabel("search"), "DMC('Search');", "find.svg")
                            AddItem("explore", GetLabel("explore"), "DMC('Explore');", "folder.svg")
                            If obj.ID > 0 AndAlso actualObject.CanViewMeta AndAlso obj.Scope = Scope.Public Then
                                AddItem("open", GetLabel("open"), "DMC('Open');")
                            End If
                    End Select

                    ' AddItem("homepage", "HomePage", "DMC('Page');")

                    If obj.HasSubFolders Then
                        AddItem("refresh", GetLabel("refresh"), "ob_t2_Reload('f" & obj.ID & "');", "Refresh.svg")
                        If obj.Scope = Scope.Public Then
                            AddItem("expandall", GetLabel("expandall"), "DMC('ExpandAll');")
                        End If
                    End If

                    AddSeparator()
                    If obj.Object_Type <> "Shortcut" AndAlso obj.HasAccess(ACL.ACL_Access.Access_Level.ACL_Create_QueryShortcut) Then
                        AddItem("addpropexp", GetLabel("addpropexp"), "DMC('AddQuery');")
                    End If

                    AddSeparator()
                    Dim lbIsRoot As Boolean = False
                    If TypeOf (obj) Is Folder Then
                        lbIsRoot = DirectCast(obj, Folder).isRoot
                    End If
                    If Not lbIsRoot Then
                        If obj.Scope = Scope.Public Then

                            AddCopyPasteItems(obj)

                            If obj.CanModifyMeta Then
                                If String.IsNullOrEmpty(extra) Then
                                    AddItem("rename", GetLabel("rename"), "DMC('Rename');", "Rename.svg")
                                End If
                                Select Case obj.Object_Type
                                    Case "Dossier"
                                        AddItem("edit", GetLabel("edit"), "DMC('EditInline');", "Edit.svg")
                                    Case Else
                                        AddItem("edit", GetLabel("edit"), "DMC('Edit');", "Edit.svg")
                                End Select

                            End If
                            AddExtraTreeItems(obj, extra)

                            If String.IsNullOrEmpty(extra) Then
                                If ShowItem("delete") AndAlso obj.CanDelete(False) Then
                                    AddItem("delete", GetLabel("ctx_delete"), "DMC(""Delete""," & EncodingUtils.EncodeJsString(obj.Name) & ");", "delete.svg")
                                End If
                            End If

                            AddSeparator()
                            AddDataSetObjectItems(actualObject)
                            AddSeparator()
                            AddExtraObjectItems(obj, actualObject, False, True, False)

                        Else
                            'private folder -> my docs
                            AddItem("paste", GetLabel("paste"), "DMC('Paste');", "paste.svg")
                        End If
                    Else
                        AddItem("paste", GetLabel("paste"), "DMC('Paste');", "paste.svg")
                        AddSeparator()
                        If obj.HasAccess(ACL.ACL_Access.Access_Level.ACL_Browse) Then
                            AddItem("subscribe", GetLabel("subscribe"), "parent.PC().SubscribeToFolder(0);")
                        End If
                    End If
                Case "Query"
                    AddQueryMenuItems(obj, "", extra)

                Case "Case"

                    AddItem("open", GetLabel("open"), "DMC('Open')")
                    AddSeparator()
                    If obj.CanMove Then
                        AddItem("cut", GetLabel("cut"), "DMC('Cut');", "cut.svg")
                    End If

                Case "Document" 'shouldn't happen
                    If obj.CanViewMeta Then
                        AddItem("open", GetLabel("open"), "DMC('Open');")
                        AddSeparator()
                    End If
                    If obj.CanMove Then
                        AddItem("cut", GetLabel("cut"), "DMC('Cut');", "cut.svg")
                    End If
                    AddItem("copy", GetLabel("copy"), "DMC('Copy');", "copy.svg")
                    If obj.CanModifyMeta Then
                        AddItem("rename", GetLabel("rename"), "DMC('Rename');", "Rename.svg")
                        AddItem("edit", GetLabel("edit"), "DMC('Edit');", "Edit.svg")
                    End If
                    If ShowItem("delete") AndAlso obj.CanDelete(False) Then
                        AddItem("delete", GetLabel("ctx_delete"), "DMC('Delete');", "delete.svg")
                    End If
                    AddSeparator()
                    AddDataSetObjectItems(obj)
                    AddSeparator()
                    AddExtraObjectItems(obj, obj, False, True, False)

            End Select
            If obj.Scope = Scope.Public AndAlso obj.CanViewACL Then
                AddSeparator()
                AddItem("acl", GetLabel("ctx_aclsetacl"), "DMC('Acl');", "acl.svg")
            End If
        Else
            AddItemNotFound(False)
        End If
        Return Result()
    End Function

    Private Sub AddExtraTreeItems(ByVal obj As DM_OBJECT, ByVal extra As String)
        Select Case extra
            Case "fav"
                AddItem("setlabel", GetLabel("setlabel"), "DMC('SetFavLbl');")
                AddItem("removefromfavorites", GetLabel("removefromfavorites"), "DocroomListFunctions.RemoveFromSelection(" & obj.ID & ",3,Filter);", "RemoveFromFavorites.svg")
            Case "bsk"
                AddItem("setlabel", GetLabel("setlabel"), "DMC('SetBskLbl');")
                AddItem("removefrombasket", GetLabel("removefrombasket"), "DocroomListFunctions.RemoveFromSelection(" & obj.ID & ",2,Filter);", "RemoveFromBasket.svg")
        End Select

        If String.IsNullOrEmpty(extra) AndAlso Settings.GetValue("Favorites", "Enabled", True) Then
            AddItem("addtofavorites", GetLabel("addtofavorites"), "DocroomListFunctions.AddToSelection(" & obj.ID & ",3,AddToFavoritesComplete);", "AddToFavorites.svg")
        End If
    End Sub

    Private Sub AddExtraObjectItems(ByVal obj As DM_OBJECT, ByVal actualobj As DM_OBJECT, ByVal addsubgroup As Boolean, ByVal index As Boolean, ByVal fromGrid As Boolean)
        If Not Security.BusinessIdentity.CurrentIdentity.IsAuthenticated Then
            Return
        End If

        Dim groupKey As String = ""
        If addsubgroup Then
            groupKey = "more"
            AddItem(groupKey, GetLabel("ctx_more"), "")
        End If

        If Not actualobj.IsInRecycleBin AndAlso actualobj.HasAccess(ACL.ACL_Access.Access_Level.ACL_Browse) Then
            AddItem("subscribe", GetLabel("subscribe"), "parent.PC().SubscribeTo(" & actualobj.ID & ",'" & actualobj.Object_Type & "');", "newSubscription.svg", groupKey)
        End If
        If addsubgroup AndAlso obj.Object_Type <> "ListItem" Then
            'don't show this in the tree
            AddItem("copymove", GetLabel("ctx_copymovetofolder"), "PC().CopyMoveObject(" & obj.ID & ");", "", groupKey)
        End If


        If fromGrid AndAlso actualobj.Status <> DM_OBJECT.Object_Status.Archived AndAlso actualobj.CanAdminData Then
            AddItem("admin", GetLabel("administer"), "PC().GetCurrentResultGrid().OpenObjectDetails(" & actualobj.ID & ",4);")
        End If

        'only from tree for now
        If Not addsubgroup AndAlso obj.Object_Type <> "Shortcut" AndAlso actualobj.CanViewMeta Then
            AddItem("info", "Info", "DMC('Info');", "", groupKey)
        End If



        If index AndAlso actualobj.Status <> DM_OBJECT.Object_Status.Archived AndAlso Security.BusinessIdentity.CurrentIdentity.isAdmin AndAlso actualobj.CanViewMeta AndAlso actualobj.CanViewFiles Then

            AddItem("idx", "Index", "PC().ShowObjectIndex(" & actualobj.ID & ");")
        End If


    End Sub

    Private Sub AddDataSetObjectItems(ByVal actualobj As DM_OBJECT)
        Dim groupKey As String = "datasets"

        AddItem(groupKey, "Datasets", "")

        AddItem("addobjecttodataset", "Add current object to dataset", "PC().AddToDataSet(" & actualobj.ID & ",1,0);", "", groupKey)

        If actualobj.Object_Type = "Folder" Then
            AddItem("addfoldercontenttodataset", "Add folder content to dataset", "PC().AddToDataSet(" & actualobj.ID & ",2,0);", "", groupKey)
            AddItem("addfoldercontentwithsubstodataset", "Add folder content to dataset (with subfolders)", "PC().AddToDataSet(" & actualobj.ID & ",3,0);", "", groupKey)
            AddItem("addfoldercontenttodataset", "Add folder with content to dataset", "PC().AddToDataSet(" & actualobj.ID & ",4,0);", "", groupKey)
            AddItem("addfoldercontentwithsubstodataset", "Add folder with content to dataset  (with subfolders)", "PC().AddToDataSet(" & actualobj.ID & ",5,0);", "", groupKey)
        End If
    End Sub

    Private Sub AddItemNotFound()
        AddItemNotFound(True)
    End Sub

    Private Sub AddItemNotFound(ByVal vbFromGrid As Boolean)

        If vbFromGrid Then
            AddItem("notfound", GetLabel("ctxitemnotfound"), "PC().GetCurrentResultGrid().Reload();")
        Else
            AddItem("notfound", GetLabel("ctxitemnotfound"), "PC().Reload();")
        End If
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Function GetMailFileContextMenuItems(ByVal DM_FILE_ID As Integer, ByVal siteName As String, ByVal file As String) As ContextMenuItem()

        Dim loFile As Mail.MailFile = Mail.MailFile.GetFile(DM_FILE_ID, 0)

        If loFile IsNot Nothing Then
            LoadCtxDefinition("mailfilecontextmenu", 0, siteName, file)
            AddItem("title", DisplayName(loFile.Name, True), "ShowFiles(" & loFile.ID & ",'','preview',true,0);") 'todo : pass mailid
            AddItem("download", GetLabel("download"), "PC().DownLoadMailFile('" + loFile.ID.ToString() + "','');", "download.svg")
            AddSeparator()

            If Settings.GetValue("MyDocuments", "Enabled", True) Then
                Dim llFolderID As Integer = Folder.GetMyDocumentsFolderID
                If loFile.CanCreateDocument(llFolderID, 0) Then
                    AddItem("promotetodoc", GetLabel("ctx_promotetodoc"), "PC().PromoteMailFileToDoc(" + loFile.ID.ToString() + "," & llFolderID & ",0);", "newDocument.svg")
                End If
            End If
            AddItem("copy", GetLabel("copy"), "PC().AddMailFileToClipboard('" + loFile.ID.ToString() + "','Copy');", "Copy.svg")
        Else
            AddItemNotFound()
        End If
        Return Result()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetFolderEasyBrowserItems(ByVal vlParentID As Int32, ByVal vsJSFunction As String, ByVal Maxitems As Int32) As ContextMenuItem()
        Dim i As Int32 = 0
        Dim loCrit As TreeLevel.Criteria = New TreeLevel.Criteria(vlParentID)
        loCrit.TypeFilter = "Folder,Dossier"

        For Each loTreeItem As TreeLevel.OBJECTInfo In TreeLevel.GetTreeLevel(loCrit)
            AddItem("feb" & loTreeItem.OBJECT_ID, DisplayName(loTreeItem.NAME, False), vsJSFunction & "(" & loTreeItem.OBJECT_ID & ");")

            i = i + 1
            If i = Maxitems AndAlso Maxitems > 0 Then
                Exit For
            End If
        Next

        If i < Maxitems AndAlso Maxitems > 0 Then
            Dim p As DM_OBJECT = ObjectRepository.GetObject(vlParentID)
            If p.Object_Type = "Dossier" Then
                Dim lcolLabels As Globalisation.LABELList = Nothing
                For Each loPack As PackageList.PackageInfo In p.Packages
                    Select Case loPack.Type
                        Case Package.ePackType.CasePackage, Package.ePackType.Docroom, Package.ePackType.Query, Package.ePackType.CaseQuery

                            If p.CanViewPackage(loPack) Then

                                If lcolLabels Is Nothing Then
                                    lcolLabels = Globalisation.LABELList.GetCategoryItemsLabelList(p.Category, Settings.GetValue("Interface", "EnableIISCaching", True))
                                End If
                                Dim lsName As String = lcolLabels.GetObjectLabel(loPack.ID, "Package", loPack.Name)
                                AddItem("pack" & loPack.ID, DisplayName(lsName, False), vsJSFunction & "(" & p.ID & "," & loPack.ID & ");")
                                i = i + 1
                                If i = Maxitems AndAlso Maxitems > 0 Then
                                    Exit For
                                End If
                            End If
                    End Select
                Next
            End If
        End If

        Return Result()

    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetFileContextMenuItems(ByVal DM_FILE_ID As Integer, ByVal siteName As String, ByVal file As String) As ContextMenuItem()
        Dim loFile As File = Arco.Doma.Library.File.GetFile(DM_FILE_ID)
        If loFile.ID = 0 Then
            loFile = Arco.Doma.Library.File.GetFileFromArchive(DM_FILE_ID)
        End If

        If loFile.ID = DM_FILE_ID Then
            LoadCtxDefinition("filecontextmenu", loFile.LinkedToObject.Category, siteName, file)

            AddFileContextMenuItems(loFile, True)
        Else
            AddItemNotFound()
        End If

        Return Result()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetObjectFileContextMenuItems(ByVal DM_FILE_ID As Integer, ByVal siteName As String, ByVal file As String) As ContextMenuItem()
        Dim loFile As File = Arco.Doma.Library.File.GetFile(DM_FILE_ID)
        If loFile.ID = 0 Then
            loFile = Arco.Doma.Library.File.GetFileFromArchive(DM_FILE_ID)
        End If

        If loFile.ID = DM_FILE_ID Then

            AddObjectContextMenuItems(loFile.LinkedToObject, loFile, siteName, file)

            Return Result()
        Else
            AddItemNotFound()
        End If

        Return Result()
    End Function

    Private Sub AddFileContextMenuItems(ByVal loFile As File, ByVal vbFull As Boolean)
        Dim fileType As DM_FileTypeList.FileTypeInfo
        If Not String.IsNullOrEmpty(loFile.FILE_EXT) Then
            fileType = loFile.LinkedToObject.GetFileType(loFile.FILE_EXT)
        Else
            fileType = Nothing
        End If
        Dim lsFromArchive As String = ""
        If loFile.FromArchive Then
            lsFromArchive = "Y"
        End If

        If Not loFile.CanView Then
            Return
        End If

        Dim fileId As String = loFile.ID.ToString

        AddItem("filename", GetLabel("preview"), "ShowFiles(" + fileId + ",'" & lsFromArchive & "','preview',false,0);")

        If loFile.CanBeDownloaded(fileType) Then
            AddItem("download", GetLabel("download"), "PC().DownLoadFile('" + fileId + "','" & lsFromArchive & "');", "download.svg")
        End If

        Dim canModify As Boolean = loFile.CanModify()

        If Not loFile.FromArchive Then
            If loFile.Type = File.File_Type.File Then
                If canModify Then
                    If loFile.IsRemoteUrl Then
                        AddItem("editfile", GetLabel("editurl"), "PC().ShowFileAction('" + fileId + "','2');", "Edit.svg")
                    Else
                        If fileType.FILE_WEBDAVENABLED AndAlso (loFile.Locked = Arco.Doma.Library.File.LockingStatus.NoLock OrElse loFile.Locked = Arco.Doma.Library.File.LockingStatus.WebDavLock) Then
                            AddItem("editfilewebdav", GetLabel("editfile") & " " & GetLabel("with") & " " & Arco.IO.MimeTypes.GetWebDavApplicationName(fileType.Extension), "PC().EditFile(" + fileId + ");", "Images/FileTypes/" & Icons.GetFileIcon(loFile.FILE_EXT))
                        End If
                        If fileType.FILE_INLINEEDITING AndAlso (loFile.Locked = Arco.Doma.Library.File.LockingStatus.NoLock OrElse loFile.Locked = Arco.Doma.Library.File.LockingStatus.RemoteLock2) Then
                            AddItem("editfileinline", GetLabel("editfile"), "PC().EditFileDirect(" + fileId + ");", "Edit.svg")
                        End If

                    End If
                    If loFile.Locked = File.LockingStatus.NoLock Then
                        AddItem("replacefile", GetLabel("Replacefile"), "PC().ReplaceFile(" + fileId + ");", "replacefile.svg")
                    End If
                Else
                    If Settings.GetValue("Versioning", "EnableFileVersioning", True) AndAlso loFile.CanCheckOut Then
                        AddItem("replacefile", GetLabel("Replacefile"), "PC().ReplaceFile(" + fileId + ");", "replacefile.svg")
                    End If
                End If
            End If
            If Settings.GetValue("Versioning", "EnableFileVersioning", True) Then
                If loFile.CanCheckOut Then
                    AddItem("checkoutfile", GetLabel("ctx_Checkoutfile"), "PC().CheckOutFile(" + fileId + ");", "documentCheckOut.svg")
                End If
                If loFile.CanCheckIn() Then
                    If loFile.FILE_CHECKBY = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
                        AddItem("Checkinfile", GetLabel("ctx_checkinfile"), "PC().CheckInFile(" + fileId + ");", "documentCheckIn.svg")
                        AddItem("cancelcheckoutfile", GetLabel("ctx_Cancelcheckoutfile"), "PC().CancelCheckOutFile(" + fileId + ");", "cancelDocumentCheckOut.svg")
                    Else
                        AddItem("Checkinfile", GetLabel("admincheckin"), "PC().CheckInFile(" + fileId + ");", "documentCheckIn.svg")
                        AddItem("cancelcheckoutfile", GetLabel("adminCancelcheckout"), "PC().CancelCheckOutFile(" + fileId + ");", "cancelDocumentCheckOut.svg")
                    End If
                End If
            End If

            If loFile.Locked = File.LockingStatus.WebDavLock Then
                    AddItem("collectfile", GetLabel("collectfile"), "PC().CollectFile(" & fileId & ",false);")
                End If


                If vbFull Then
                    If ShowItem("delete") AndAlso loFile.CanDelete Then
                        AddItem("delete", GetLabel("ctx_Delete"), "PC().DeleteFile(" & fileId & ");", "delete.svg")
                    End If
                End If

                AddItem("copy", If(vbFull, GetLabel("copy"), GetLabel("copyfile")), "PC().AddFileToClipboard('" & fileId & "','Copy');", "Copy.svg")

                If loFile.CanDelete Then
                    AddItem("cut", If(vbFull, GetLabel("cut"), GetLabel("cutfile")), "PC().AddFileToClipboard('" & fileId & "','Cut');", "Cut.svg")
                End If
                If vbFull AndAlso canModify AndAlso loFile.Locked = File.LockingStatus.NoLock Then
                    AddItem("editfileprops", GetLabel("editfileproperties"), "PC().EditFileProps(" & fileId & ");", "rename.svg")
                End If
                If Settings.GetValue("Publishing", "Enabled", True) AndAlso Not loFile.LinkedToObject.IsInRecycleBin Then
                    AddItem("publishing", GetLabel("publishing"), String.Format("PC().EditPublishing({0},'File');", fileId), "publish.svg")
                End If

                If vbFull Then
                    If canModify AndAlso loFile.isAttachment AndAlso (loFile.LinkedToObject.CategoryObject.FileCardinalityConstraint = ConstraintEnum.AtLeastOne OrElse loFile.LinkedToObject.CategoryObject.FileCardinalityConstraint = ConstraintEnum.NoConstraint) Then
                        AddItem("setmainfile", GetLabel("setmainfile"), String.Format("PC().SetMainFile({0});", fileId))
                    End If
                    If loFile.CanModify(True) Then
                        If loFile.IsReadOnly Then
                            AddItem("togglereadonly", GetLabel("setreadonlyoff"), String.Format("PC().ToggleFileReadOnly({0});", fileId))
                        Else
                            AddItem("togglereadonly", GetLabel("setreadonlyon"), String.Format("PC().ToggleFileReadOnly({0});", fileId))
                        End If
                    End If
                    If Security.BusinessIdentity.CurrentIdentity.isAdmin OrElse canModify Then
                        AddItem("reindex", GetLabel("reindexfiletype"), String.Format("PC().ReIndexFile({0});", fileId))
                    End If
                End If



                For Each loCustAction As DMCustomActionList.CustomActionInfo In DMCustomActionList.GetCustomActionList("File", loFile.LinkedToObject)
                    If loCustAction.Enabled(loFile, IUserEvent.eCallerLocation.ContextMenu) Then
                        InitCustomActions()
                        AddItem("cust_" & loCustAction.ID, loCustAction.Label(loFile, IUserEvent.eCallerLocation.ContextMenu), "PC().DoDocumentListActionDoc(1," & loCustAction.ID & "," + fileId + ",1);", "", "custom") 'javascript to be implemented)
                    End If
                Next
            End If

    End Sub

    Private _profile As UserProfile

    Private ReadOnly Property UserProfile() As UserProfile
        Get
            If _profile Is Nothing Then
                _profile = Security.BusinessIdentity.CurrentIdentity().GetUserProfile()
            End If
            Return _profile
        End Get
    End Property

    <WebMethod(EnableSession:=True)>
    Public Function GetMailContextMenuItems(ByVal vlMailID As Integer, ByVal vlBoxId As Mail.MailBoxItem.MailBox, ByVal siteName As String, ByVal file As String) As ContextMenuItem()
        Dim loMail As Mail.DMMail = Mail.DMMail.GetMail(vlMailID)
        If loMail IsNot Nothing Then
            Dim mailClientEnabled As Boolean = Settings.GetValue("MailClient", "Enabled", True)

            LoadCtxDefinition("mailcontextmenu", 0, siteName, file)
            AddItem("name", DisplayName(loMail.Subject, True), "PC().GetCurrentResultGrid().DoRealAction(" & vlMailID & ");")
            If Settings.GetValue("MyDocuments", "Enabled", True) Then
                AddSeparator()
                Dim llFolderID As Integer = Folder.GetMyDocumentsFolderID
                If loMail.CanCreateDocument(llFolderID, 0) Then
                    AddItem("promotetodoc", GetLabel("ctx_promotetodoc"), "PC().PromoteMailToDoc(" + loMail.ID.ToString() + "," & llFolderID & ",0);", "newdocument.svg")
                End If
            End If
            AddSeparator()
            If mailClientEnabled AndAlso loMail.AllowReply Then
                AddItem("reply", GetLabel("reply"), "PC().ComposeMail(" & vlMailID & ",'reply',0,0,0);", "reply.svg")
                AddItem("replyall", GetLabel("replyall"), "PC().ComposeMail(" & vlMailID & ",'replyall',0,0,0);", "reply-all.svg")
            End If
            If mailClientEnabled AndAlso loMail.AllowForward Then
                AddItem("forward", GetLabel("forward"), "PC().ComposeMail(" & vlMailID & ",'forward',0,0,0);", "forward.svg")
            End If
            If mailClientEnabled AndAlso (loMail.AllowReply OrElse loMail.AllowForward) Then
                AddSeparator()
            End If
            If loMail.CurrentUserCanCloseTrackingItem Then
                AddItem("closetrackingonmail", GetLabel("markmailcompleted"), "PC().CloseTrackingOnMail(" & vlMailID & ");", "ok.svg")
            End If
            If loMail.CurrentUserCanStopTrackingFollowup Then
                AddItem("StopFollowupOnMail", GetLabel("stopmailfollowup"), "PC().StopFollowupOnMail(" & vlMailID & ");", "stop.svg")
            End If

            If loMail.isInCurrentUserMailBox(Mail.MailBoxItem.MailBox.Inbox) Then
                If loMail.CurrentUserInboxLink.Item_Status = Arco.Doma.Library.Mail.MailBoxItem.ItemStatus.Unread Then
                    AddItem("markread", GetLabel("markread"), "PC().ToggleReadOnMail(" & vlMailID & ");", "read.svg")
                Else
                    AddItem("markread", GetLabel("markunread"), "PC().ToggleReadOnMail(" & vlMailID & ");", "unread.svg")
                End If
            End If
            If vlBoxId = Mail.MailBoxItem.MailBox.Undefined Then
                If loMail.isInCurrentUserMailBox(Mail.MailBoxItem.MailBox.Inbox) Then
                    vlBoxId = Mail.MailBoxItem.MailBox.Inbox
                End If
            End If
            If vlBoxId = Mail.MailBoxItem.MailBox.Undefined Then
                If loMail.isInCurrentUserMailBox(Mail.MailBoxItem.MailBox.Sent) Then
                    vlBoxId = Mail.MailBoxItem.MailBox.Sent
                End If
            End If

            If vlBoxId <> Mail.MailBoxItem.MailBox.Undefined Then

                If loMail.GetCurrentUserMailBoxLink(vlBoxId).Status = Mail.MailBoxItem.MailStatus.Open Then
                    If loMail.CurrentUserCanDelete(vlBoxId) Then
                        AddItem("delete", GetLabel("ctx_delete"), "PC().DeleteMailFromBox(" & vlMailID & "," & Convert.ToInt32(vlBoxId) & ");", "delete.svg")
                    End If
                Else
                    AddItem("restore", GetLabel("restore"), "PC().RestoreMailBoxItem(" & vlMailID & "," & Convert.ToInt32(vlBoxId) & ");")
                    AddItem("delete", GetLabel("ctx_delete"), "PC().DeleteMailFromBox(" & vlMailID & "," & Convert.ToInt32(vlBoxId) & ");", "delete.svg")
                End If
            End If
        Else
            AddItemNotFound()
        End If

        Return Result()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetCaseContextMenuItems(ByVal vlTechID As Integer, ByVal siteName As String, ByVal file As String) As ContextMenuItem()
        Dim loCase As cCase = cCase.GetCase(vlTechID)
        Return GetCaseContextMenuItems(loCase, siteName, file)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetHistoryCaseContextMenuItems(ByVal vlCaseID As Integer, ByVal siteName As String, ByVal file As String) As ContextMenuItem()
        Dim loCase As HistoryCase = HistoryCase.GetHistoryCase(vlCaseID)
        Return GetHistoryCaseContextMenuItems(loCase)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function GetOpenOrArchivedCaseContextMenuItems(ByVal vlCaseID As Integer, ByVal siteName As String, ByVal file As String) As ContextMenuItem()
        Dim loCase As HistoryCase = HistoryCase.GetHistoryCase(vlCaseID)
        If loCase Is Nothing OrElse loCase.Case_ID = 0 Then
            Return GetCaseContextMenuItems(cCase.GetCaseByCaseID(vlCaseID), siteName, file)
        Else
            Return GetHistoryCaseContextMenuItems(loCase)
        End If

    End Function

    Private Function GetHistoryCaseContextMenuItems(ByVal loCase As HistoryCase) As ContextMenuItem()
        If loCase.Case_ID > 0 Then
            AddItem("name", DisplayName(loCase.Name, True), "PC().GetCurrentResultGrid().OpenArchiveCaseDetails(" & loCase.Case_ID & ",1);")
        End If
        Return Result()
    End Function

    Private Function GetCaseContextMenuItems(ByVal loCase As cCase, ByVal siteName As String, ByVal file As String) As ContextMenuItem()
        If loCase.Case_ID > 0 Then
            LoadCtxDefinition("casecontextmenu", loCase.Proc_ID, siteName, file)
            AddItem("name", DisplayName(loCase.Name, True), "PC().GetCurrentResultGrid().OpenCaseDetails(" & loCase.Tech_ID & ",2);")

            AddItem("openinseparatewindow", GetLabel("openinseparatewindow"), "PC().GetCurrentResultGrid().OpenCaseDetails(" & loCase.Tech_ID & ",2,false);")

            If Not UserProfile.ShowPreview Then
                AddItem("preview", GetLabel("Preview"), "PC().GetCurrentResultGrid().DoPreviewAction(" & loCase.Tech_ID & ",true);")
            End If


            Dim userHasWork As Boolean = loCase.CurrentUserHasWork(True)
            Dim userHasWorkAndIsUsersCase As Boolean = userHasWork AndAlso (Not loCase.IsLocked OrElse loCase.IsMyCase)

            Dim lbUserHasAdminStatus As Boolean = loCase.CanAdminCaseStatus
            If loCase.CanUnlock Then
                AddItem("unlock", GetLabel("ctx_unlock"), "PC().UnlockCase(" & loCase.Tech_ID & ");", "unlock.svg")
            End If

            If userHasWork Then
                If loCase.Read = DM_OBJECT.ReadStatus.UnRead Then
                    AddItem("markread", GetLabel("markread"), "PC().ToggleReadOnCase(" & loCase.Tech_ID & ");")
                Else
                    AddItem("markread", GetLabel("markunread"), "PC().ToggleReadOnCase(" & loCase.Tech_ID & ");")
                End If
            End If

            If lbUserHasAdminStatus OrElse userHasWorkAndIsUsersCase Then
                If Not loCase.Suspended Then
                    AddItem("suspend", GetLabel("ctx_suspend"), "PC().ToggleSuspendCase(" & loCase.Tech_ID & ");", "suspend.svg")
                Else
                    AddItem("suspend", GetLabel("unsuspend"), "PC().ToggleSuspendCase(" & loCase.Tech_ID & ");", "unsuspend.svg")
                End If
            End If

            If lbUserHasAdminStatus OrElse loCase.CanAdminData Then
                AddSeparator()
                AddItem("admin", GetLabel("administer"), "PC().GetCurrentResultGrid().OpenCaseDetails(" & loCase.Tech_ID & ", 4);")
                If loCase.CanAdminCaseStatus AndAlso (loCase.CurrentStep Is Nothing OrElse TypeOf loCase.CurrentStep.Step_Type.Handler Is StepTypes.IHasWorkStepType) Then
                    AddItem("editwork", GetLabel("ctx_editwork"), "PC().EditWork(" & loCase.Tech_ID & ");", "EditWork.svg")
                End If
            End If
            If Security.BusinessIdentity.CurrentIdentity.isAdmin Then
                AddItem("idx", "Index", "PC().ShowCaseIndex(" & loCase.Tech_ID & ");")
            End If

            If loCase.DM_Object_ID > 0 Then
                AddDataSetObjectItems(loCase.TargetObject)
                AddSeparator()
                AddExtraObjectItems(loCase.TargetObject, loCase.TargetObject, True, False, True)

                If loCase.TargetObject.CanViewACL Then
                    AddSeparator()
                    AddItem("acl", GetLabel("ctx_aclsetacl"), "PC().ShowObjectACL(" & loCase.TargetObject.ID & ");", "acl.svg")
                End If

            End If


            If userHasWorkAndIsUsersCase Then
                For Each loUserEvent As UserEventList.UserEventInfo In UserEventList.GetUserEventList(loCase.Proc_ID, "Procedure", UserEvent.eLocation.ContextMenu, True)
                    If loUserEvent.IsEnabledFor(loCase) Then
                        InitCustomActions()
                        AddItem("usrevent_" & loUserEvent.ID, ReplaceTags(loUserEvent.Caption, loCase), "PC().DoDocumentListActionCase(10," & loUserEvent.ID & "," + loCase.Tech_ID.ToString() + ",1);", loUserEvent.Icon, "custom") 'javascript to be implemented                
                    End If
                Next

                For Each loCustAction As DMCustomActionList.CustomActionInfo In DMCustomActionList.GetCustomActionList("Work", loCase.TargetObject)
                    If loCustAction.Enabled(loCase, IUserEvent.eCallerLocation.ContextMenu) Then
                        InitCustomActions()
                        AddItem("cust_" & loCustAction.ID, loCustAction.Label(loCase, IUserEvent.eCallerLocation.ContextMenu), "PC().DoDocumentListActionCase(1," & loCustAction.ID & "," + loCase.Tech_ID.ToString() + ",1);", "", "custom") 'javascript to be implemented
                    End If
                Next
            End If

            If loCase.TargetObject.HasCategory Then
                'user events                
                For Each loUserEvent As UserEventList.UserEventInfo In UserEventList.GetUserEventList(loCase.TargetObject.Category, "Category", UserEvent.eLocation.ContextMenu, True)
                    If loUserEvent.IsEnabledFor(loCase.TargetObject) AndAlso loCase.TargetObject.CanExecuteUserEvent(loUserEvent) Then
                        InitCustomActions()
                        AddItem("usrevent_" & loUserEvent.ID, ReplaceTags(loUserEvent.Caption, loCase.TargetObject), "PC().DoDocumentListActionDoc(10," & loUserEvent.ID & "," + loCase.TargetObject.ID.ToString() + ");", loUserEvent.Icon, "custom") 'javascript to be implemented                
                    End If
                Next

                'custom actions                
                For Each loCustAction As DMCustomActionList.CustomActionInfo In DMCustomActionList.GetCustomActionList(loCase.TargetObject.Object_Type, loCase.TargetObject)
                    If loCustAction.Enabled(loCase.TargetObject, IUserEvent.eCallerLocation.ContextMenu) Then
                        InitCustomActions()
                        AddItem("cust_" & loCustAction.ID, loCustAction.Label(loCase.TargetObject, IUserEvent.eCallerLocation.ContextMenu), "PC().DoDocumentListActionDoc(1," & loCustAction.ID & "," + loCase.TargetObject.ID.ToString() + ");", "", "custom") 'javascript to be implemented
                    End If
                Next
            End If
        Else
            AddItemNotFound()
        End If

        Return Result()
    End Function

    Private Function ReplaceTags(ByVal text As String, ByVal source As IPropertyContainer) As String
        Return TagReplacer.ReplaceTags(text, Security.BusinessIdentity.CurrentIdentity.LanguageCode, source, Nothing, New Arco.Doma.Library.TextFormatters.NullTextFormatter(), True, True, 0, 0, False)
    End Function

    Private Const Maxlength = 50

    Private _hascustaction As Boolean = False

    Private Sub InitCustomActions()
        If Not _hascustaction Then
            AddSeparator()
            AddItem("custom", GetLabel("Custom"), "", "", Nothing)
            _hascustaction = True
        End If
    End Sub

    Private Shared Function DisplayName(ByVal vsName As String, ByVal bold As Boolean) As String
        If Not String.IsNullOrEmpty(vsName) Then
            If vsName.Length > Maxlength Then
                vsName = vsName.Substring(0, Maxlength) & "..."
            End If
        End If
        If Not bold Then
            Return HttpUtility.HtmlEncode(vsName)
        End If

        Return "<b>" & HttpUtility.HtmlEncode(vsName) & "</b>"

    End Function

    Private Sub AddObjectContextMenuItems(ByVal obj As DM_OBJECT, ByVal file As File, ByVal siteName As String, ByVal fileStr As String)
        If obj.Case_ID > 0 Then
            If obj.Case_Active Then
                Dim loCase As cCase = cCase.GetCaseByCaseID(obj.Case_ID)
                GetCaseContextMenuItems(loCase, siteName, fileStr)
                If Not file Is Nothing Then
                    If loCase.TargetObject.CanViewFiles Then
                        AddSeparator()
                        AddFileContextMenuItems(file, False)
                        AddSeparator()
                    End If
                End If
            Else
                Dim loCase As HistoryCase = HistoryCase.GetHistoryCase(obj.Case_ID)
                GetHistoryCaseContextMenuItems(loCase)
            End If
            Return
        End If

        If obj.HasAccess(ACL.ACL_Access.Access_Level.ACL_Browse) Then
            Dim name As String
            If obj.Status <> DM_OBJECT.Object_Status.Archived Then
                name = DisplayName(obj.Name, True)
            Else
                name = "<i>" & DisplayName(obj.Name, False) & " (" & obj.Version.toString & ")</i>"
            End If
            AddItem("name", name, "PC().GetCurrentResultGrid().OpenObjectDetails(" & obj.ID & ",1);")

            If Not TypeOf obj Is Folder Then
                If Not UserProfile.ShowPreview Then
                    AddItem("preview", GetLabel("Preview"), "PC().GetCurrentResultGrid().DoPreviewAction(" & obj.ID & ",true);")
                End If
            End If

            AddItem("openinseparatewindow", GetLabel("openinseparatewindow"), "PC().GetCurrentResultGrid().OpenObjectDetails(" & obj.ID & ",1,false);")
        End If

        If Not (TypeOf obj Is Shortcut OrElse TypeOf obj Is MailShortcut OrElse TypeOf obj Is CaseObject) Then
            LoadCtxDefinition("objectcontextmenu", obj.Category, siteName, fileStr)
        End If

        Dim actualObject As DM_OBJECT = obj

        If TypeOf obj Is Document Then
            If obj.CanModifyMeta Then
                AddItem("rename", GetLabel("rename"), "PC().RenameObject(" & obj.ID & ");", "rename.svg")
                AddItem("edit", GetLabel("edit"), "PC().GetCurrentResultGrid().OpenObjectDetails(" & obj.ID & ",2);", "Edit.svg")
            End If
            If obj.CanRestore Then
                AddItem("restore", GetLabel("restore"), "PC().GetCurrentResultGrid().RestoreObject(" & obj.ID & ");")
            End If
            If ShowItem("delete") AndAlso obj.CanDelete(False) Then
                AddItem("delete", GetLabel("ctx_delete"), "PC().DeleteObject(" & obj.ID & ");", "delete.svg")
            End If
            If Settings.GetValue("Versioning", "EnableDocumentVersioning", True) Then
                If DirectCast(obj, Document).CanCheckOut Then
                    AddItem("checkout", GetLabel("ctx_checkout"), "PC().CheckOut(" & obj.ID & ");", "documentcheckout.svg")
                End If
                If DirectCast(obj, Document).CanCheckIn() Then
                    If obj.CheckOut_By = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
                        AddItem("checkin", GetLabel("ctx_checkin"), "PC().CheckIn(" & obj.ID & ");", "documentcheckin.svg")
                        AddItem("cancelcheckout", GetLabel("ctx_cancelcheckout"), "PC().CancelCheckOut(" & obj.ID & ");", "cancelDocumentCheckOut.svg")
                    Else
                        AddItem("adminCheckin", GetLabel("adminCheckin"), "PC().CheckIn(" & obj.ID & ");", "documentcheckin.svg")
                        AddItem("cancelcheckout", GetLabel("adminCancelcheckout"), "PC().CancelCheckOut(" & obj.ID & ");", "cancelDocumentCheckOut.svg")
                    End If
                End If
            End If

            If obj.CanUnLock Then
                    AddItem("unlock", GetLabel("ctx_unlock"), "PC().Unlock(" & obj.ID & ");", "unlock.svg")
                End If
                If obj.CanAddFile() Then
                    AddItem("addfile", GetLabel("ctx_addfile"), "PC().AddFile(" & obj.ID & ");", "file.svg")
                    AddItem("addmessage", GetLabel("addmessage"), "PC().AddFileAsMessage(" & obj.ID & ");", "mail.svg")

                    'If Settings.GetValue("WebScan", "Enabled", False) AndAlso obj.CanAddFileType("tif") Then
                    '    AddItem("scanfile", GetLabel("ctx_scanfile"), "PC().ScanFile(" & obj.ID & ");", "Scan.svg")
                    'End If
                    Dim lsPasteAction As String = ""
                    If Not HttpContext.Current.Request.Cookies.Get("FileClipBoardAction") Is Nothing Then
                        lsPasteAction = HttpContext.Current.Request.Cookies.Get("FileClipBoardAction").Value
                    End If
                    If lsPasteAction = "Copy" OrElse lsPasteAction = "Cut" Then
                        AddItem("pastefile", GetLabel("Pastefile"), "PC().PasteFile(" & obj.ID & ");", "Paste.svg")
                    End If
                End If

                If file Is Nothing Then
                    If obj.FileCount = 1 Then
                        Dim f As File = obj.GetFile(obj.Files(0).ID)
                        If f IsNot Nothing Then
                            If f.ReferenceID = 0 AndAlso obj.CanViewFiles Then
                                AddSeparator()
                                AddFileContextMenuItems(f, False)
                                AddSeparator()
                            End If
                        End If
                    End If
                Else
                    If obj.CanViewFiles Then
                        AddSeparator()
                        AddFileContextMenuItems(file, False)
                        AddSeparator()
                    End If
                End If

                If obj.CanViewFiles Then
                    AddItem("sendmail", GetLabel("sendmail"), "PC().OpenWindow('./MailClient/Compose2.aspx?DM_OBJECT_ID=" & obj.ID & "', 'MyDetailWindow', 'width=800,height=600,resizable=yes,scrollbars=yes');", "email_attach.svg")
                End If
                If obj.Object_Reference_Type = "Mail" Then
                    AddItem("viewMail", GetLabel("viewMail"), "PC().OpenPreviewLink('./MailClient/View.aspx?DM_MAIL_ID=" & obj.Object_Reference & "');", "email_attach.svg")
                End If
                If (obj.CategoryObject.FileCardinalityConstraint = ConstraintEnum.NoConstraint OrElse obj.CategoryObject.FileCardinalityConstraint = ConstraintEnum.AtLeastOne) AndAlso obj.CanViewPreviousFileVersions Then
                    AddItem("ViewFileVersions", GetLabel("ViewFileVersions"), "PC().GetCurrentResultGrid().ShowPreviousFileVersions('" + obj.ID.ToString() + "');", "VersionHistory.svg")
                End If

            ElseIf TypeOf obj Is ListItem Then
                If obj.CanModifyMeta Then
                    AddItem("rename", GetLabel("rename"), "PC().RenameObject(" & obj.ID & ");", "rename.svg")
                    AddItem("edit", GetLabel("edit"), "PC().GetCurrentResultGrid().OpenObjectDetails(" & obj.ID & ",2);", "Edit.svg")
                End If
                If obj.CanRestore Then
                    AddItem("restore", GetLabel("restore"), "PC().GetCurrentResultGrid().RestoreObject(" & obj.ID & ");")
                End If
                If ShowItem("delete") AndAlso obj.CanDelete(False) Then
                    AddItem("delete", GetLabel("ctx_delete"), "PC().DeleteObject(" & obj.ID & ");", "delete.svg")
                End If
                If obj.CanUnLock Then
                    AddItem("unlock", GetLabel("ctx_unlock"), "PC().Unlock(" & obj.ID & ");", "unlock.svg")
                End If
                If obj.CanAddFile() Then
                    AddItem("addfile", GetLabel("ctx_addfile"), "PC().AddFile(" & obj.ID & ");", "file.svg")
                    'If Settings.GetValue("WebScan", "Enabled", True) AndAlso obj.CanAddFileType("tif") Then
                    '    AddItem("scanfile", GetLabel("ctx_scanfile"), "PC().ScanFile(" & obj.ID & ");", "Scan.svg")
                    'End If
                    Dim lsPasteAction As String = ""
                    If Not HttpContext.Current.Request.Cookies.Get("FileClipBoardAction") Is Nothing Then
                        lsPasteAction = HttpContext.Current.Request.Cookies.Get("FileClipBoardAction").Value
                    End If
                    If lsPasteAction = "Copy" OrElse lsPasteAction = "Cut" Then
                        AddItem("pastefile", GetLabel("Pastefile"), "PC().PasteFile(" & obj.ID & ");", "Paste.svg")
                    End If
                End If

                If file Is Nothing Then
                    If obj.FileCount = 1 Then
                        Dim f As File = obj.GetMainFile
                        If Not f Is Nothing Then
                            If f.ID > 0 AndAlso f.ReferenceID = 0 AndAlso obj.CanViewFiles Then
                                AddSeparator()
                                AddFileContextMenuItems(f, False)
                                AddSeparator()
                            End If
                        End If
                    End If
                Else
                    If obj.CanViewFiles Then
                        AddSeparator()
                        AddFileContextMenuItems(file, False)
                        AddSeparator()
                    End If
                End If
            ElseIf TypeOf obj Is Dossier Then
                If DirectCast(obj, Dossier).HasSubDossiers Then
                    AddItem("browse", GetLabel("browsesubdossiers"), "PC().GetCurrentResultGrid().DoFolderAction(" & obj.ID & ",'Browse');", "browse.svg")
                End If

                AddItem("search", GetLabel("search"), "PC().GetCurrentResultGrid().DoFolderAction(" & obj.ID & ",'Search');", "find.svg")

                If obj.CanModifyMeta Then
                    AddItem("rename", GetLabel("rename"), "PC().RenameObject(" & obj.ID & ");", "rename.svg")
                End If
                If obj.CanRestore Then
                    AddItem("restore", GetLabel("restore"), "PC().GetCurrentResultGrid().RestoreObject(" & obj.ID & ");")
                End If
                If ShowItem("delete") AndAlso obj.CanDelete(False) Then
                    AddItem("delete", GetLabel("ctx_delete"), "PC().DeleteObject(" & obj.ID & ");", "delete.svg")
                End If

                If obj.CanUnLock Then
                    AddItem("unlock", GetLabel("ctx_unlock"), "PC().Unlock(" & obj.ID & ");", "unlock.svg")
                End If

            ElseIf TypeOf obj Is Shortcut Then
                actualObject = DirectCast(obj, Shortcut).GetReferencedObject

                If obj.Object_Reference_Type = "Folder" Then
                    If actualObject.HasAccess(ACL.ACL_Access.Access_Level.ACL_CreateDocument) OrElse actualObject.HasAccess(ACL.ACL_Access.Access_Level.ACL_CreateSubFolder) Then
                        AddItem("new", GetLabel("new"), "PC().GetCurrentResultGrid().DoFolderAction(" & actualObject.ID & ",'Add');", "newdocument.svg")
                    End If
                    AddItem("browse", GetLabel("browse"), "PC().GetCurrentResultGrid().DoFolderAction(" & actualObject.ID & ",'Browse');", "browse.svg")
                    AddItem("search", GetLabel("search"), "PC().GetCurrentResultGrid().DoFolderAction(" & actualObject.ID & ",'Search');", "find.svg")
                    AddItem("Explore", GetLabel("explore"), "PC().GetCurrentResultGrid().DoFolderAction(" & actualObject.ID & ",'Explore');", "folder.svg")
                End If
                If actualObject.CanViewMeta Then
                    AddItem("open", GetLabel("open"), "PC().GetCurrentResultGrid().OpenObjectDetails(" & obj.ID & ",1);")
                End If
                If obj.HasAccess(ACL.ACL_Access.Access_Level.ACL_Create_ShortCut) AndAlso obj.ShowInTree Then 'rename shortcut rights
                    AddItem("rename", GetLabel("rename"), "PC().RenameObject(" & obj.ID & ");", "rename.svg")
                End If
                If ShowItem("delete") AndAlso obj.CanDelete(False) Then 'delete shortcut rights
                    AddItem("delete", GetLabel("ctx_delete"), "PC().DeleteObject(" & obj.ID & ");", "delete.svg")
                End If
                If obj.Object_Reference_Type = "Mail" Then
                    AddSeparator()
                    AddItem("viewMail", GetLabel("viewmail"), "PC().OpenPreviewLink('./MailClient/View.aspx?DM_MAIL_ID=" & obj.Object_Reference & "');", "email_attach.svg")

                End If
            ElseIf TypeOf obj Is CaseObject Then

            ElseIf TypeOf obj Is Folder Then
                If Not obj.Status = DM_OBJECT.Object_Status.Deleted Then
                    If obj.HasAccess(ACL.ACL_Access.Access_Level.ACL_CreateDocument) OrElse obj.HasAccess(ACL.ACL_Access.Access_Level.ACL_CreateSubFolder) Then
                        AddItem("new", GetLabel("new"), "PC().GetCurrentResultGrid().DoFolderAction(" & obj.ID & ",'Add');", "newdocument.svg")
                    End If
                    AddItem("browse", GetLabel("browse"), "PC().GetCurrentResultGrid().DoFolderAction(" & obj.ID & ",'Browse');", "browse.svg")
                    AddItem("search", GetLabel("search"), "PC().GetCurrentResultGrid().DoFolderAction(" & obj.ID & ",'Search');", "find.svg")
                    AddItem("Explore", GetLabel("explore"), "PC().GetCurrentResultGrid().DoFolderAction(" & obj.ID & ",'Explore');", "folder.svg")
                    If obj.CanViewMeta Then
                        AddItem("open", GetLabel("open"), "PC().GetCurrentResultGrid().DoFolderAction(" & obj.ID & ",'Open');")
                    End If
                    AddSeparator()
                    If obj.CanUnLock Then
                        AddItem("unlock", GetLabel("ctx_unlock"), "PC().Unlock(" & obj.ID & ");", "unlock.svg")
                    End If
                    If obj.CanModifyMeta Then
                        AddItem("rename", GetLabel("rename"), "PC().RenameObject(" & obj.ID & ");", "rename.svg")
                        AddItem("edit", GetLabel("edit"), "PC().GetCurrentResultGrid().DoFolderAction(" & obj.ID & ",'Edit');", "Edit.svg")
                    End If

                Else
                    If obj.CanViewMeta Then
                        AddItem("open", GetLabel("open"), "PC().GetCurrentResultGrid().DoFolderAction(" & obj.ID & ",'Open');")
                    End If
                    If obj.CanRestore Then
                        AddItem("restore", GetLabel("restore"), "PC().GetCurrentResultGrid().RestoreObject(" & obj.ID & ");")
                    End If

                End If
            ElseIf TypeOf obj Is MailShortcut Then
                If Not obj.Status = DM_OBJECT.Object_Status.Deleted Then
                'hack : actually show the same items as in the file menu, this object has only 1 file           
                For Each loFileInfo As FileList.FileInfo In obj.Files
                    Dim loFile As File = Arco.Doma.Library.File.GetFile(loFileInfo.ID)
                    AddItem("download", GetLabel("download"), "PC().DownLoadFile('" + loFile.ID.ToString() + "','');", "download.svg")
                    If loFile.CanDelete Then
                        AddItem("delete", GetLabel("ctx_delete"), "PC().DeleteObject(" & obj.ID & ");", "delete.svg")
                    End If
                    If loFile.CanModify Then
                        AddItem("copy", GetLabel("copy"), "PC().AddFileToClipboard('" + loFile.ID.ToString() + "','Copy');", "Copy.svg")
                    End If
                    'Cut file
                    If loFile.CanModify Then
                        AddItem("cut", GetLabel("cut"), "PC().AddFileToClipboard('" + loFile.ID.ToString() + "','Cut');", "Cut.svg")
                    End If
                Next
                If obj.CanViewFiles Then
                    'AddItem("sendmail", GetLabel("sendmail"), "MailDoc(" & obj.ID & ");", "email_attach.svg")
                    AddItem("sendmail", GetLabel("sendmail"), "PC().OpenWindow('./MailClient/Compose2.aspx?DM_OBJECT_ID=" & obj.ID & "', 'MyDetailWindow', 'width=800,height=600,resizable=yes,scrollbars=yes');", "email_attach.svg")
                End If
            Else
                If obj.CanRestore Then
                    AddItem("restore", GetLabel("restore"), "PC().GetCurrentResultGrid().RestoreObject(" & obj.ID & ");")
                End If
            End If
        End If


        'all types
        If ShowItem("delete") AndAlso obj.CanDelete(False) Then
            AddItem("delete", GetLabel("ctx_delete"), "PC().DeleteObject(" & obj.ID & ");", "delete.svg")
        End If

        AddDataSetObjectItems(actualObject)

        AddExtraObjectItems(obj, actualObject, True, True, True)

        If Not obj.IsInRecycleBin AndAlso obj.Scope = Scope.Public AndAlso obj.CanViewACL Then
            AddSeparator()
            AddItem("acl", GetLabel("ctx_aclsetacl"), "PC().ShowObjectACL(" & obj.ID & ");", "acl.svg")
        End If


        'user events
        If Not actualObject.IsInRecycleBin Then
            For Each loUserEvent As UserEventList.UserEventInfo In UserEventList.GetUserEventList(actualObject.Category, "Category", UserEvent.eLocation.ContextMenu, True)
                If loUserEvent.IsEnabledFor(actualObject) AndAlso actualObject.CanExecuteUserEvent(loUserEvent) Then
                    InitCustomActions()

                    AddItem("usrevent_" & loUserEvent.ID, ReplaceTags(loUserEvent.Caption, actualObject), "PC().DoDocumentListActionDoc(10," & loUserEvent.ID & "," + actualObject.ID.ToString() + ");", loUserEvent.Icon, "custom") 'javascript to be implemented                
                End If
            Next

            'custom actions            
            For Each loCustAction As DMCustomActionList.CustomActionInfo In DMCustomActionList.GetCustomActionList(actualObject.Object_Type, actualObject)
                If loCustAction.Enabled(actualObject, IUserEvent.eCallerLocation.ContextMenu) Then
                    InitCustomActions()
                    AddItem("cust_" & loCustAction.ID, loCustAction.Label(actualObject, IUserEvent.eCallerLocation.ContextMenu), "PC().DoDocumentListActionDoc(1," & loCustAction.ID & "," + actualObject.ID.ToString() + ");", "", "custom") 'javascript to be implemented
                End If
            Next
        End If

    End Sub

    <WebMethod(EnableSession:=True)>
    Public Function GetObjectContextMenuItems(ByVal DM_OBJECT_ID As Integer, ByVal siteName As String, ByVal file As String) As ContextMenuItem()
        Dim obj As DM_OBJECT = ObjectRepository.SearchObject(DM_OBJECT_ID)



        If Not obj Is Nothing Then
            AddObjectContextMenuItems(obj, Nothing, siteName, file)
        Else
            AddItemNotFound()
        End If

        Return Result()
    End Function
End Class
<Serializable()>
Public Structure ContextMenuItem
    Public Text As String
    Public NavigateUrl As String
    Public Value As String
    Public isSeparator As Boolean
    Public Image As String
    Public Parent As String
End Structure