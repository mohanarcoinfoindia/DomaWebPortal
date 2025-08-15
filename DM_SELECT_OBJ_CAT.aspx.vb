Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Routing

Namespace Doma
    Partial Class DM_SELECT_OBJ_CAT
        Inherits BasePage

        Enum eDisplayMode
            List = 1
            Thumbnail = 2
        End Enum

        Public Property HasResults As Boolean
        Private meDisplayMode As eDisplayMode = eDisplayMode.List
        Private mbAutoSelect As Boolean = True
        Private _autoLink As String = ""
        Private msMode As String = "0"        
        Private msInputSelection As String = ""
        Protected PackID As Int32
        Protected LinkToObjectID As Int32
        Protected LinkToCaseID As Int32
        Private moCatItemList As List(Of CatItem) = New List(Of CatItem)
        Private mbInit As Boolean
        Private GridID As String

        Private Sub SetLabels()
            Page.Title = GetDecodedLabel("selectcategory")
            chkMode.Text = GetLabel("multidocupload")
        End Sub

        Protected Sub Page_Load1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            Dim llObjectID As Int32
            Dim llCaseID As Int32
            Dim loObject As DM_OBJECT = Nothing
            Dim loLinkToObject As DM_OBJECT = Nothing

            Dim lsCatType As String

            Form.DefaultButton = lnkEnter.UniqueID

            SetLabels()

            If Not Page.IsPostBack Then
                msMode = QueryStringParser.GetInt("mode")

                chkMode.Checked = (msMode = "1")
            Else
                If chkMode.Checked Then msMode = "1"
            End If

            lsCatType = QueryStringParser.GetString("CAT_TYPE")
            msInputSelection = QueryStringParser.GetString("INPUTSELECTION")
            llObjectID = QueryStringParser.GetInt("DM_PARENT_ID")
            llCaseID = QueryStringParser.GetInt("CASE_ID")
            PackID = QueryStringParser.GetInt("PACK_ID")

            If llCaseID > 0 AndAlso PackID > 0 Then
                Dim loCase As cCase = cCase.GetCaseByCaseID(llCaseID)
                loLinkToObject = loCase.GetPackageLocationObject(PackID)
                If loLinkToObject.CanAddToPackage(PackID) Then
                    LinkToCaseID = llCaseID

                    Dim lsDefFolder As String = TagReplacer.ReplaceTags(loLinkToObject.GetPackageInfo(PackID).DefaultFolder)

                    If loLinkToObject.GetPackageInfo(PackID).Type = Package.ePackType.CasePackage Then lsCatType = "Procedure"

                    If Not String.IsNullOrEmpty(lsDefFolder) Then
                        Dim iFolderID As Int32 = Folder.CreateFolderStructure(lsDefFolder, 0)
                        If iFolderID > 0 Then
                            loObject = Folder.GetFolder(iFolderID)
                        Else
                            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                        End If
                    ElseIf llObjectID > 0 Then
                        loObject = ObjectRepository.GetObject(llObjectID)
                    Else
                        'add to root?
                        loObject = Folder.GetRoot
                    End If
                Else
                    GotoErrorPage(loLinkToObject.GetLastError.Code)
                End If
            ElseIf llObjectID > 0 Then

                If PackID > 0 Then
                    'add to package
                    loLinkToObject = ObjectRepository.GetObject(llObjectID)
                    If loLinkToObject.CanAddToPackage(PackID) Then
                        LinkToObjectID = loLinkToObject.ID

                        Dim lsDefFolder As String = TagReplacer.ReplaceTags(loLinkToObject.GetPackageInfo(PackID).DefaultFolder)

                        If loLinkToObject.GetPackageInfo(PackID).Type = Package.ePackType.CasePackage Then lsCatType = "Procedure"

                        If Not String.IsNullOrEmpty(lsDefFolder) Then
                            Dim iFolderID As Int32 = Folder.CreateFolderStructure(lsDefFolder, 0)
                            If iFolderID > 0 Then
                                loObject = Folder.GetFolder(iFolderID)
                            Else
                                GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                            End If
                        Else
                            'add to root?
                            loObject = Folder.GetRoot
                        End If
                    Else
                        GotoErrorPage(loLinkToObject.GetLastError.Code)
                    End If

                Else
                    'add in folder
                    loObject = ObjectRepository.GetObject(llObjectID)
                End If

                While loObject.Object_Type = "Shortcut" 'load the object the shortcut points too
                    If loObject.Object_Reference > 0 Then
                        loObject = CType(loObject, Shortcut).GetReferencedObject
                    Else
                        'shortcut not pointing anywhere
                        Response.Write("Illegal shortcut")
                        Response.End()
                    End If
                End While
            Else
                loObject = Folder.GetRoot
            End If


            If Not Page.IsPostBack Then
                If Not String.IsNullOrEmpty(lsCatType) Then
                    drpDwnType.Items.Add(New WebControls.ListItem(GetLabel(lsCatType), lsCatType))
                    drpDwnType.Visible = False
                Else
                    'default lib
                    drpDwnType.Items.Add(New WebControls.ListItem(GetLabel("all"), ""))
                    drpDwnType.Items.Add(New WebControls.ListItem(GetDecodedLabel("document"), "Document"))

                    drpDwnType.Items.Add(New WebControls.ListItem(GetDecodedLabel("folder"), "Folder"))
                    drpDwnType.Items.Add(New WebControls.ListItem("Dossier", "Dossier"))

                    If loObject.ID = 0 Then
                        drpDwnType.Items.Add(New WebControls.ListItem(GetDecodedLabel("listitem"), "ListItem"))
                    End If
                    drpDwnType.Items.Add(New WebControls.ListItem(GetDecodedLabel("case"), "Case"))
                End If

                If UserProfile.SelectCategoryMode = "2" Then
                    drpDownMode.SelectedIndex = 1
                End If
            Else
                lsCatType = drpDwnType.SelectedValue
            End If
            meDisplayMode = CType(drpDownMode.SelectedValue, eDisplayMode)

            If lsCatType <> "Procedure" Then
                Page.Title = GetDecodedLabel("selectcategory")


                For Each loCat As OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In OBJECT_CATEGORYList.GetAllowedCategories(lsCatType, loObject, loLinkToObject, PackID, True, (msMode = 1), Nothing)
                    AddCategoryInfo(loCat, loObject)
                Next
                CloseContent()
            Else
                Page.Title = GetDecodedLabel("selectproc")
                If msMode <> "1" Then
                    For Each loProc As PROCEDUREList.PROCEDUREInfo In PROCEDUREList.GetAllowedProcedures(loObject, loLinkToObject, PackID, True, Nothing)
                        If loProc.DM_ACTION = Procedure.ProcedureObjectAction.None OrElse (loProc.DM_ACTION = Procedure.ProcedureObjectAction.InsertObject AndAlso loProc.DM_CAT_ID > 0) Then 'action procedures
                            AddProcInfo(loProc, loObject)
                        End If
                    Next
                End If

            End If

            mbAutoSelect = Not Page.IsPostBack
            RenderItems()

            If Not String.IsNullOrEmpty(_autoLink) Then
                Response.Redirect(GetRedirectUrl(_autoLink), False)
                Context.ApplicationInstance.CompleteRequest()
            End If

        End Sub

        Private mcolAllPacks As PackageList
        Private ReadOnly Property AllPackages As PackageList
            Get
                If mcolAllPacks Is Nothing Then
                    mcolAllPacks = PackageList.GetAllPackages
                End If
                Return mcolAllPacks
            End Get
        End Property
        Private Function IsInputPackageMatch(ByVal vlPackID As Int32, ByVal vePackType As Package.ePackType) As Boolean
            For Each lopack As PackageList.PackageInfo In AllPackages
                If lopack.ID = vlPackID Then
                    If lopack.Type = vePackType Then
                        Return True
                    Else
                        Return False
                    End If
                End If
            Next
            Return False
        End Function
        Private Sub AddProcInfo(ByVal loProc As PROCEDUREList.PROCEDUREInfo, ByVal voParent As DM_OBJECT)

            Dim bCanCreate As Boolean = True

            If Not String.IsNullOrEmpty(msInputSelection) Then
                bCanCreate = False
                If loProc.InputPackage > 0 Then
                    Select Case CType(msInputSelection, DMSelection.SelectionType)
                        Case DMSelection.SelectionType.CurrentCases, DMSelection.SelectionType.CurrentCasesByCaseID
                            bCanCreate = IsInputPackageMatch(loProc.InputPackage, Package.ePackType.CasePackage)
                        Case DMSelection.SelectionType.CurrentMails
                            bCanCreate = False
                        Case Else
                            bCanCreate = IsInputPackageMatch(loProc.InputPackage, Package.ePackType.Docroom)
                    End Select
                End If
            End If

            If bCanCreate Then
                Dim lsIcon As String = "case.gif"
                If loProc.DM_CAT_ID > 0 Then
                    lsIcon = OBJECT_CATEGORY.GetOBJECT_CATEGORY(loProc.DM_CAT_ID).Icon
                    If String.IsNullOrEmpty(lsIcon) Then
                        lsIcon = "case.png"
                    End If
                End If
                AddItem(loProc.ID, voParent.ID, GetProcedureLabel(loProc), "Procedure", lsIcon, GetProcedureTooltip(loProc), "")
            Else
                ' Response.Write("Can't create " & loProc.PROC_NAME & "<br>")
            End If

        End Sub
        Private Sub AddCategoryInfo(ByVal loCat As OBJECT_CATEGORYList.OBJECT_CATEGORYInfo, ByVal voParent As DM_OBJECT)

            Dim bCanCreate As Boolean = True

            If Not String.IsNullOrEmpty(msInputSelection) Then
                bCanCreate = False
                If loCat.InputPackage > 0 Then
                    Select Case CType(msInputSelection, DMSelection.SelectionType)
                        Case DMSelection.SelectionType.CurrentCases, DMSelection.SelectionType.CurrentCasesByCaseID
                            bCanCreate = IsInputPackageMatch(loCat.InputPackage, Package.ePackType.CasePackage)
                        Case DMSelection.SelectionType.CurrentMails
                            bCanCreate = False
                        Case Else
                            bCanCreate = IsInputPackageMatch(loCat.InputPackage, Package.ePackType.Docroom)
                    End Select
                End If
            End If

            If bCanCreate Then
                AddItem(loCat.ID, voParent.ID, GetCategoryLabel(loCat), loCat.Type, loCat.Icon, GetCategoryTooltip(loCat), "")
            End If

        End Sub

        Private Sub InitContent()
            If Not mbInit Then
                HasResults = False
                mbInit = True
            End If
        End Sub

        Private Sub CloseContent()
            If mbInit Then
                mbInit = False
            End If
        End Sub

        Private Class CatItem
            Public Property ID As Integer
            Public Property Name As String
            Public Property ParentID As Integer
            Public Property Type As String
            Public Property Image As String
            Public Property ToolTip As String
            Public Property ErrorMsg As String
        End Class

        Private Class CatItemComparer
            Implements IComparer(Of CatItem)


            Public Function Compare(ByVal x As CatItem, ByVal y As CatItem) As Integer Implements IComparer(Of CatItem).Compare
                Return x.Name.ToUpper.CompareTo(y.Name.ToUpper)
            End Function
        End Class


        Private Sub RenderItems()
            moCatItemList.Sort(New CatItemComparer)
            Dim liCurrItem As Integer = 1
            InitContent()

            Dim loRow As TableRow = Nothing
            Dim lbNewRow As Boolean = True
            Dim lsSize As String = ""
            Dim liColCount As Integer = 0
            Select Case meDisplayMode
                Case eDisplayMode.List
                    lsSize = ""
                    liColCount = 1
                Case eDisplayMode.Thumbnail
                    lsSize = "height='32px' width='32px'"
                    liColCount = 3
            End Select

            Dim folderIconPath As String
            If System.IO.Directory.Exists(Page.Server.MapPath(String.Concat("~/App_Themes/", Page.Theme, "/Tree/Icons"))) Then
                folderIconPath = String.Concat("./App_Themes/", Page.Theme, "/Tree/Icons/")
            Else
                folderIconPath = "./TreeIcons/Icons/"
            End If

            Dim lsFilter As String = txtFilter.Text.ToUpper
            For Each loCatItem As CatItem In moCatItemList
                Dim lbAdd As Boolean = True

                If Not String.IsNullOrEmpty(lsFilter) Then
                    If Not loCatItem.Name.ToUpper.Contains(lsFilter) Then
                        lbAdd = False
                    End If
                End If

                If lbAdd Then
                    If lbNewRow Then
                        loRow = New TableRow
                        lbNewRow = False
                    End If

                    If liCurrItem Mod liColCount = 0 Then
                        lbNewRow = True
                    End If

                    Dim lsName As String = Server.HtmlEncode(loCatItem.Name)
                    If String.IsNullOrEmpty(loCatItem.ErrorMsg) Then
                        Dim loLink As HyperLink = New HyperLink
                        Dim loCell As TableCell = New TableCell
                        loCell.CssClass = "FieldCell"
                        If loCatItem.Type = "Procedure" Then
                            loLink.NavigateUrl = String.Format("javascript:NewCase({0},{1},'{2}',{3},{4},{5});", loCatItem.ID, loCatItem.ParentID, msInputSelection, LinkToObjectID, PackID, LinkToCaseID)
                        Else
                            loLink.NavigateUrl = String.Format("javascript:NewObject({0},'{7}',{1},'{2}',{3},{4},{5},{6});", loCatItem.ID, loCatItem.ParentID, msInputSelection, LinkToObjectID, PackID, LinkToCaseID, msMode, loCatItem.Type)
                        End If

                        If mbAutoSelect Then
                            If String.IsNullOrEmpty(_autoLink) Then
                                If loCatItem.Type = "Procedure" Then
                                    _autoLink = String.Format("DM_NEW_CASE.ASPX?PROC_ID={0}&DM_PARENT_ID={1}&INPUTSELECTION={2}&DM_OBJECT_ID={3}&PACK_ID={4}&CASE_ID={5}", loCatItem.ID, loCatItem.ParentID, msInputSelection, LinkToObjectID, PackID, LinkToCaseID)
                                Else
                                    _autoLink = String.Format("DM_NEW_OBJECT.ASPX?DM_CAT_ID={0}&DM_PARENT_ID={1}&INPUTSELECTION={2}&DM_OBJECT_ID={3}&PACK_ID={4}&CASE_ID={5}&mode={6}&CAT_TYPE={7}", loCatItem.ID, loCatItem.ParentID, msInputSelection, LinkToObjectID, PackID, LinkToCaseID, msMode, loCatItem.Type)
                                End If
                            Else
                                _autoLink = ""
                                mbAutoSelect = False
                            End If
                        End If

                        If Not String.IsNullOrEmpty(loCatItem.Image) Then
                            loLink.Text = "<img src='" & folderIconPath & loCatItem.Image & "' border=0 " & lsSize & " >&nbsp;" & lsName
                        Else
                            loLink.Text = "<img src='" & folderIconPath & loCatItem.Type & ".png' border=0 " & lsSize & ">&nbsp;" & lsName
                        End If

                        loLink.ToolTip = loCatItem.ToolTip
                        loCell.Controls.Add(loLink)
                        loRow.Cells.Add(loCell)
                    Else
                        Dim loCell As New TableCell
                        Dim loLit As New LiteralControl With {
                            .Text = "<font color='red'>Unable to create " & lsName & " : " & loCatItem.ErrorMsg & "</font>"
                        }
                        loCell.Controls.Add(loLit)
                        loRow.Cells.Add(loCell)
                    End If


                    If meDisplayMode = eDisplayMode.List Then
                        Dim loCell As TableCell = New TableCell
                        loCell.CssClass = "LabelCell"
                        Dim lsType As String = loCatItem.Type
                        If lsType = "Procedure" Then
                            lsType = "Case"
                        End If
                        loCell.Text = GetLabel(lsType)
                        loRow.Cells.Add(loCell)
                    End If

                    liCurrItem += 1
                    tblTabs.Rows.Add(loRow)
                End If
            Next
        End Sub

        Private Sub AddItem(ByVal vlID As Int32, ByVal vlParentID As Int32, ByVal vsName As String, ByVal vsCatType As String, ByVal vsImg As String, ByVal vsTooltip As String, ByVal vsError As String)

            Dim loCatItem As CatItem = New CatItem
            loCatItem.ID = vlID
            loCatItem.Image = vsImg
            loCatItem.Name = vsName
            loCatItem.ParentID = vlParentID
            loCatItem.ToolTip = vsTooltip
            loCatItem.Type = vsCatType
            loCatItem.ErrorMsg = vsError
            moCatItemList.Add(loCatItem)

        End Sub

        Protected Sub drpDownMode_SelectedIndexChanged(sender As Object, e As EventArgs) Handles drpDownMode.SelectedIndexChanged

            UserProfile.SelectCategoryMode = drpDownMode.SelectedValue

        End Sub
    End Class
End Namespace
