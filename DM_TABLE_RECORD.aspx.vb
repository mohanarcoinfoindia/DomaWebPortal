Imports Arco.Doma.Library
Imports Arco.Doma.Library.Website
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Globalisation
Imports System.Linq
Imports Arco.Doma.Library.Routing


Partial Class DM_TABLE_RECORD
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

    Protected llID As Int32
    Protected llCaseID As Int32
    Protected llTechID As Int32
    Protected llTableID As Int32
    Protected _screenID As Int32
    Protected llParentRowID As Int32
    Protected lbFromArchive As Boolean

    Private _mode As Int32 '1 = readonly, 2 = edit
    Dim obj As DM_OBJECT
    Dim loCase As cCase

    Private msOpts As String
    Private mModal As Boolean
    Private mWidth As Int32 = 600
    Public ReadOnly Property Modal As Boolean
        Get
            Return mModal
        End Get
    End Property
    Public Property Width() As Int32
        Get
            Return (mWidth - 40)
        End Get
        Set(ByVal value As Int32)
            mWidth = value
        End Set
    End Property


    Protected Property PreviousRecord() As Int32
        Get
            Return Convert.ToInt32(ViewState("PreviousRecord"))
        End Get
        Set(ByVal value As Int32)
            ViewState("PreviousRecord") = value
        End Set
    End Property
    Protected Property NextRecord() As Int32
        Get
            Return Convert.ToInt32(ViewState("NextRecord"))
        End Get
        Set(ByVal value As Int32)
            ViewState("NextRecord") = value
        End Set
    End Property

    Protected Property CurrentRecord() As Int32
        Get
            Return Convert.ToInt32(ViewState("CurrentRecord"))
        End Get
        Set(ByVal value As Int32)
            ViewState("CurrentRecord") = value
        End Set
    End Property

    Public ReadOnly Property IsReadOnly As Boolean
        Get
            Return _mode = 1
        End Get
    End Property

    Private Sub BindDetail(ByVal vbRebind As Boolean)

        Dim lcolLabels As LABELList = Nothing
        Dim lbUserHasEditInView As Boolean = False
        If llCaseID > 0 Then
            If Not lbFromArchive Then
                If llTechID > 0 Then
                    loCase = cCase.GetCase(llTechID)
                Else
                    loCase = cCase.GetCaseByCaseID(llCaseID)
                End If
                obj = loCase.GetPropertyLocationObject(llTableID)
                If Not IsReadOnly Then
                    If Not loCase.CurrentUserHasWork AndAlso Not loCase.CanAdminData Then
                        Server.Transfer("DM_ACL_DENIED.aspx")
                    End If
                Else
                    lbUserHasEditInView = loCase.CurrentUserHasWork()
                End If
                lcolLabels = LABELList.GetProcedureItemsLabelList(loCase.Proc_ID, EnableIISCaching)
                If _screenID <> 0 Then
                    docform.DisplayItems = ScreenItemList.GetTableScreenItems(_screenID, llTableID, loCase, Device.Web, SiteVersion.Version7)
                End If
            Else
                obj = HistoryCase.GetHistoryCase(llCaseID)
                lcolLabels = LABELList.GetProcedureItemsLabelList(CType(obj, HistoryCase).Proc_ID, EnableIISCaching)
                If _screenID <> 0 Then
                    docform.DisplayItems = ScreenItemList.GetTableScreenItems(_screenID, llTableID, obj, Device.Web, SiteVersion.Version7)
                End If
            End If

        Else

            obj = ObjectRepository.GetObject(llID)
            If IsReadOnly Then
                If Not obj.CanViewMeta Then
                    Server.Transfer("DM_ACL_DENIED.aspx")
                Else
                    lbUserHasEditInView = obj.CanModifyMeta
                End If
            Else
                If Not obj.CanModifyMeta Then
                    Server.Transfer("DM_ACL_DENIED.aspx")
                End If
            End If

            lcolLabels = LABELList.GetCategoryItemsLabelList(obj.Category, EnableIISCaching)
            If _screenID <> 0 Then
                docform.DisplayItems = ScreenItemList.GetTableScreenItems(_screenID, llTableID, obj, Device.Web, 7)
            End If
        End If
        If Not obj Is Nothing Then
            Dim loTableInfo As DM_PROPERTY = DM_PROPERTY.GetPROPERTY(llTableID)

            If String.IsNullOrEmpty(msOpts) Then msOpts = loTableInfo.Definition
            Dim lbCanEdit As Boolean = True
            If msOpts.Length >= 3 Then
                If (msOpts(0) = "N"c) Then
                    lnkNew.Visible = False
                    cmdCopy.Visible = False
                End If
                If CurrentRecord > 0 Then
                    If (msOpts(1) = "N"c) Then
                        _mode = 1
                        lbCanEdit = False
                    End If
                Else
                    cmdCopy.Visible = False
                    If (msOpts(0) = "N"c) Then
                        _mode = 1
                        lbCanEdit = False
                    End If
                End If

                If (msOpts(2) = "N"c) Then
                    cmdDelete.Visible = False
                End If
            End If

            docform.Labels = lcolLabels
            docform.CategoryID = obj.Category
            docform.ScreenID = _screenID
            docform.IsMainScreen = False

            'add default screenitems if no screen
            If _screenID = 0 Then
                If IsReadOnly Then
                    docform.DisplayItems = ScreenItemList.GetDefaultTableScreenItems(llTableID, ScreenItem.ItemMode.ReadOnly)
                Else
                    docform.DisplayItems = ScreenItemList.GetDefaultTableScreenItems(llTableID, ScreenItem.ItemMode.Writable)
                End If
            End If

            If IsReadOnly Then
                If lbCanEdit AndAlso lbUserHasEditInView AndAlso HasWritableItem(docform.DisplayItems) Then
                    docform.DisplayMode = Screen.DetailScreenDisplayMode.Edit
                    cmdSave.Enabled = True
                    cmdSaveAndClose.Enabled = True
                Else
                    docform.DisplayMode = Screen.DetailScreenDisplayMode.ReadOnly
                    cmdSave.Enabled = False
                    cmdSaveAndClose.Enabled = False
                End If
                cmdDelete.Enabled = False
                cmdCopy.Enabled = False
                lnkNew.Enabled = False
            Else
                docform.DisplayMode = Screen.DetailScreenDisplayMode.Edit
                cmdSave.Enabled = True
                cmdSaveAndClose.Enabled = True
                Dim propInfo As DMObjectPROPERTYList.PROPERTYInfo = obj.GetPropertyInfo(llTableID, llParentRowID)
                If propInfo.CanAddRow Then
                    cmdCopy.Enabled = True
                    lnkNew.Enabled = True
                Else
                    cmdCopy.Enabled = False
                    lnkNew.Enabled = False
                End If
                If Not propInfo.CanRemoveRow Then
                    cmdDelete.Enabled = False
                End If
            End If

            If Not Page.IsPostBack OrElse vbRebind Then
                If CurrentRecord <> 0 Then
                    If Not obj.Properties.Cast(Of DMObjectPROPERTYList.PROPERTYInfo).Any(Function(x) x.PARENT_PROP_ID = llTableID AndAlso x.ROW_ID = CurrentRecord) Then
                        'attempting to edit a row that doesn't exists
                        GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                    End If
                End If
                docform.DataBindTableRow(llTechID, llCaseID, llID, llTableID, CurrentRecord, llParentRowID, obj, vbRebind)
                If Not Page.IsPostBack Then 'Not Me.Modal Then
                    docform.Focus()
                End If
            End If

            PreviousRecord = 0
            NextRecord = 0
            Dim lsCaption As String = lcolLabels.GetObjectLabel(loTableInfo.ID, "Property", loTableInfo.Name)

            If CurrentRecord > 0 Then
                'create scroller
                Dim liPropTemp As Integer = 0
                For Each loP As DMObjectPROPERTYList.PROPERTYInfo In obj.Properties
                    If loP.PARENT_PROP_ID = llTableID AndAlso loP.PARENT_ROW_ID = llParentRowID Then
                        liPropTemp = loP.ID
                        Exit For
                    End If
                Next
                Dim iCurr As Int32 = 0
                Dim lbFound As Boolean = False
                For Each loP As DMObjectPROPERTYList.PROPERTYInfo In obj.Properties
                    If loP.ID = liPropTemp AndAlso loP.PARENT_ROW_ID = llParentRowID Then
                        iCurr = iCurr + 1
                        If loP.ROW_ID = CurrentRecord Then
                            lbFound = True
                        Else
                            If Not lbFound Then
                                PreviousRecord = loP.ROW_ID
                            Else
                                iCurr = iCurr - 1
                                NextRecord = loP.ROW_ID
                                Exit For
                            End If
                        End If
                    End If
                Next
                Dim iRowCount As Integer = obj.Properties.Cast(Of DMObjectPROPERTYList.PROPERTYInfo).Count(Function(x) x.ID = liPropTemp AndAlso x.PARENT_ROW_ID = llParentRowID)

                Page.Title = lsCaption & " (" & iCurr & "/" & iRowCount & ")"
            Else
                cmdDelete.Visible = False
                Page.Title = lsCaption & " (" & GetLabel("new") & ")"
            End If
            If PreviousRecord > 0 Then
                cmdSaveAndPrevious.Enabled = True
            Else
                cmdSaveAndPrevious.Enabled = False
            End If

            If NextRecord > 0 Then
                cmdSaveAndNext.Enabled = True
            Else
                cmdSaveAndNext.Enabled = False
            End If
        End If
    End Sub

    Private Function HasWritableItem(ByVal voList As ScreenItemList) As Boolean
        Dim lbHasWritable As Boolean = False
        For Each itm As Website.ScreenItemList.ScreenItemInfo In voList
            Select Case itm.Mode
                Case ScreenItem.ItemMode.Mandatory, ScreenItem.ItemMode.Writable
                    If itm.PROP_TYPE <> "TABLE" Then
                        lbHasWritable = True
                        Exit For
                    End If
                    ' Case Else
                    'nothing
            End Select
        Next
        Return lbHasWritable
    End Function
    Private Function GetPostBackString(ByVal btn As LinkButton) As String
        Return GetPostBackString(btn, True, "")
    End Function
    Private Function GetPostBackString(ByVal btn As LinkButton, ByVal vbValidate As Boolean, ByVal vsArgument As String) As String
        Dim objPostBackoptions As PostBackOptions
        If vbValidate Then
            objPostBackoptions = New PostBackOptions(btn, vsArgument, "", True, True, True, True, True, btn.ValidationGroup)
        Else
            objPostBackoptions = New PostBackOptions(btn, vsArgument)
        End If
        Return Page.ClientScript.GetPostBackEventReference(objPostBackoptions)

    End Function
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load
        mModal = QueryStringParser.GetBoolean("modal")
        llID = QueryStringParser.GetInt("DM_OBJECT_ID")
        llCaseID = QueryStringParser.GetInt("CASE_ID")
        llTechID = QueryStringParser.GetInt("TECH_ID")
        llTableID = QueryStringParser.GetInt("DM_TABLE_ID")
        _screenID = QueryStringParser.GetScreenId("SCREEN_ID")
        llParentRowID = QueryStringParser.GetInt("PARENT_ROW_ID")
        msOpts = QueryStringParser.GetString("OPTS")

        If Not Page.IsPostBack Then
            CurrentRecord = QueryStringParser.GetInt("DM_ROW_ID")
        End If
        _mode = QueryStringParser.GetInt("mode", 2)

        lbFromArchive = QueryStringParser.GetBoolean("fromarchive")
        Dim strScript As StringBuilder = New StringBuilder
        strScript.AppendLine("function SaveObject() {")
        strScript.AppendLine(GetPostBackString(btnSave) & ";")
        strScript.AppendLine("}")
        Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "detailviewfuncs", strScript.ToString, True)

        docform.ModalWindow = Modal

        If Not Page.IsPostBack Then
            lnkNew.ToolTip = GetDecodedLabel("new")
            cmdSave.ToolTip = GetDecodedLabel("save")
            cmdSaveAndClose.ToolTip = GetDecodedLabel("savecomplete")
            cmdCopy.ToolTip = GetDecodedLabel("copy")
            cmdSaveAndNext.ToolTip = GetDecodedLabel("next")
            cmdSaveAndPrevious.ToolTip = GetDecodedLabel("previous")
            lnkClose.ToolTip = GetDecodedLabel("close")
            cmdDelete.ToolTip = GetDecodedLabel("delete")

            cmdCopy.SpriteCssClass = "icon icon-copy"
            lnkNew.SpriteCssClass = "icon icon-add-new"
            cmdSave.SpriteCssClass = "icon icon-save"
            cmdSaveAndClose.SpriteCssClass = "icon icon-save-exit"
            lnkClose.SpriteCssClass = "icon icon-close"
            cmdDelete.SpriteCssClass = "icon icon-delete"
            cmdSaveAndNext.SpriteCssClass = "icon icon-page-next"
            cmdSaveAndPrevious.SpriteCssClass = "icon icon-page-previous"
        End If

        BindDetail(False)

    End Sub

    Private Sub Copy()
        If docform.DisplayMode = Screen.DetailScreenDisplayMode.Edit And Page.IsValid Then
            Dim lbOk As Boolean
            If Not loCase Is Nothing Then
                lbOk = docform.SaveTableRow(loCase)
            Else
                lbOk = docform.SaveTableRow(obj)
            End If

            If lbOk Then
                Dim t As DM_OBJECT.Table = DirectCast(obj.GetProperty(llTableID, llParentRowID), DM_OBJECT.Table)
                Dim llNewRowID As Int32 = 0
                obj = t.CopyRow(docform.ROW_ID, llNewRowID)
                obj = obj.Save
                ScrollTo(llNewRowID)
            End If
        End If
    End Sub
    Private Sub ScrollTo(ByVal vlRowID As Int32)

        If vlRowID = 0 Then
            CurrentRecord = docform.ROW_ID
            BindDetail(True)
        Else
            CurrentRecord = vlRowID
            QueryStringParser.AddOrReplace("DM_ROW_ID", CurrentRecord.ToString)
            Response.Redirect("DM_TABLE_RECORD.aspx" & QueryStringParser.ToString())
        End If

    End Sub
    Private Sub Save(ByVal GotoRowAfter As Integer)
        If docform.DisplayMode = Screen.DetailScreenDisplayMode.Edit AndAlso Page.IsValid Then
            Dim lbOk As Boolean
            If Not loCase Is Nothing Then
                lbOk = docform.SaveTableRow(loCase)
            Else
                lbOk = docform.SaveTableRow(obj)
            End If


            If lbOk Then
                'rebind the form, things can change
                ScrollTo(GotoRowAfter)
            Else
                'rebind
                ' sBindDetail()
                Page.ClientScript.RegisterStartupScript(Me.GetType, "AlertHandlerMessage", "alert('" & docform.ErrorMessage & "');", True)
            End If
        Else
            'in read mode, just scroll
            If GotoRowAfter > 0 Then
                CurrentRecord = GotoRowAfter
                BindDetail(True)
            End If
        End If
    End Sub
    Private Sub Delete()
        If docform.DisplayMode = Screen.DetailScreenDisplayMode.Edit AndAlso Page.IsValid Then
            If Not loCase Is Nothing Then
                loCase.GetPropertyLocationObject(llTableID).SetRowToBeDeletedInTable(llTableID, CurrentRecord)
                loCase = loCase.SaveWithOnKeep
            Else
                obj = obj.RemoveRowFromTable(llTableID, CurrentRecord)
                obj = obj.Save
            End If
            If PreviousRecord > 0 Then
                ScrollTo(PreviousRecord)
            ElseIf NextRecord > 0 Then
                ScrollTo(NextRecord)
            Else
                Close()
            End If
        End If
    End Sub
    Private Sub Close()
        Page.ClientScript.RegisterStartupScript(Me.GetType, "Close", "GetRadWindow().Close();", True)
    End Sub
    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Save(0)
    End Sub
    Protected Sub tlbMain_ButtonClick(ByVal sender As Object, ByVal e As Telerik.Web.UI.RadToolBarEventArgs) Handles tlbMain.ButtonClick
        Dim btn As Telerik.Web.UI.RadToolBarButton = CType(e.Item, Telerik.Web.UI.RadToolBarButton)
        Select Case btn.CommandName.ToUpper
            Case "SAVE"
                Save(0)
            Case "SAVEANDCLOSE"
                Save(0)
                Close()
            Case "DELETE"
                Delete()
            Case "COPY"
                Copy()
            Case "SAVEANDNEXT"
                Save(NextRecord)
            Case "SAVEANDPREVIOUS"

                Save(PreviousRecord)
        End Select
    End Sub

End Class


