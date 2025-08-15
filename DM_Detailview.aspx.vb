
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Routing
Imports Arco.Doma.Library.Website

Partial Class DM_Detailview
    Inherits BasePage


    Public Sub New()
        AllowGuestAccess = True
    End Sub

    Private _selectedTab As String = ""

    Protected WithEvents btnSave As New LinkButton
    Protected WithEvents btnSaveWithValidation As New LinkButton
    Protected WithEvents btnToggleCloseDossier As New LinkButton
    Protected WithEvents btnSaveComplete As New LinkButton
    Protected WithEvents btnSaveCloseWindow As New LinkButton
    Protected WithEvents btnSaveNext As New LinkButton
    Protected WithEvents btnSavePrevious As New LinkButton
    Protected WithEvents btnSaveNextEdit As New LinkButton
    Protected WithEvents btnCancel As New LinkButton
    Protected WithEvents btnRelease As New LinkButton
    Protected WithEvents btnReleaseTo As New LinkButton
    Protected WithEvents btnMoveTo As New LinkButton

    Protected WithEvents btnTransferTo As New LinkButton
    Protected WithEvents btnReject As New LinkButton
    Protected WithEvents btnSuspend As New LinkButton

    Protected WithEvents btnUnlock As New LinkButton
    Protected WithEvents btnUnlockAdmin As New LinkButton

    Protected bInCreation As Boolean
    Protected ForceNextCaseOnClose As Boolean
    Private thisObjectDetail As DM_OBJECT
    Private thisCaseInstance As cCase

    Public ReadOnly Property Modal() As String
        Get
            Return QueryStringParser.Modal
        End Get
    End Property

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit
        ParseQueryString()

        If QueryStringParser.Preview Then

            MasterPageFile = "~/masterpages/Preview.master"
            CType(Master, BaseMasterPage).ReloadFunction = "GotoCurrent()"
        Else
            MasterPageFile = "~/masterpages/DetailMain.master"
        End If
    End Sub

    Private Sub AddButtonControls()


        btnSave.ID = "btnSave"
        btnSave.Visible = False
        Controls.Add(btnSave)

        btnSaveWithValidation.ID = "btnSaveWithValidation"
        btnSaveWithValidation.Visible = False
        btnSaveWithValidation.ValidationGroup = "CheckMandatoryFields"
        Controls.Add(btnSaveWithValidation)

        btnSaveComplete.ID = "btnSaveComplete"
        btnSaveComplete.Visible = False
        btnSaveComplete.ValidationGroup = "CheckMandatoryFields"
        Controls.Add(btnSaveComplete)

        btnSaveCloseWindow.ID = "btnSaveCloseWindow"
        btnSaveCloseWindow.Visible = False
        btnSaveCloseWindow.ValidationGroup = "CheckMandatoryFields"
        Controls.Add(btnSaveCloseWindow)

        btnRelease.ID = "btnRelease"
        btnRelease.Visible = False
        btnRelease.ValidationGroup = "CheckMandatoryFields"
        Controls.Add(btnRelease)


        btnReleaseTo.ID = "btnReleaseTo"
        btnReleaseTo.Visible = False
        btnReleaseTo.ValidationGroup = ""
        Controls.Add(btnReleaseTo)

        btnMoveTo.ID = "btnMoveTo"
        btnMoveTo.Visible = False
        btnMoveTo.ValidationGroup = ""
        Controls.Add(btnMoveTo)

        btnTransferTo.ID = "btnTransferWork"
        btnTransferTo.Visible = False
        btnTransferTo.ValidationGroup = ""
        Controls.Add(btnTransferTo)

        btnSaveNext.ID = "btnSaveNext"
        btnSaveNext.Visible = False
        btnSaveNext.ValidationGroup = "CheckMandatoryFields"
        Controls.Add(btnSaveNext)

        btnSavePrevious.ID = "btnSavePrevious"
        btnSavePrevious.Visible = False
        btnSavePrevious.ValidationGroup = "CheckMandatoryFields"
        Controls.Add(btnSavePrevious)

        btnSaveNextEdit.ID = "btnSaveNextEdit"
        btnSaveNextEdit.Visible = False
        btnSaveNextEdit.ValidationGroup = "CheckMandatoryFields"
        Controls.Add(btnSaveNextEdit)

        btnCancel.ID = "btnCancel"
        btnCancel.Visible = False
        btnCancel.CausesValidation = False
        Controls.Add(btnCancel)

        btnUnlock.ID = "btnUnlock"
        btnUnlock.Visible = False
        Controls.Add(btnUnlock)

        btnUnlockAdmin.ID = "btnUnlockAdmin"
        btnUnlockAdmin.Visible = False
        Controls.Add(btnUnlockAdmin)

        btnReject.ID = "btnReject"
        btnReject.Visible = False
        Controls.Add(btnReject)

        btnSuspend.ID = "btnSuspend"
        btnSuspend.Visible = False
        Controls.Add(btnSuspend)

        btnToggleCloseDossier.ID = "btnToggleCloseDossier"
        btnToggleCloseDossier.Visible = False
        btnToggleCloseDossier.ValidationGroup = "CheckMandatoryFields"
        Controls.Add(btnToggleCloseDossier)
    End Sub

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
        Return ClientScript.GetPostBackEventReference(objPostBackoptions) & ";setTimeout(function() {if(!Page_IsValid){EnBtn();}}, 0);"

    End Function

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        AddButtonControls()
    End Sub
    Protected Sub form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If BindDetail() Then

            Dim strScript As New StringBuilder

            strScript.Append("function DisBtn(entirewindow) {")
            '  strScript.Append("parent.DisableButtons();")
            strScript.Append("if (entirewindow) {parent.DisableWindow();} else {parent.DisableButtons();}")
            strScript.Append("}")
            strScript.Append("function EnBtn() {")
            strScript.Append("parent.EnableWindow();parent.EnableButtons();")
            strScript.Append("}")

            If InEditMode() Then


                strScript.Append("function SaveObject() {")
                strScript.Append("Page_ClientValidate('');")
                strScript.Append(" if(Page_IsValid){")
                strScript.Append("DisBtn(false);")
                strScript.Append(GetPostBackString(btnSave))
                strScript.Append("} else {")
                strScript.Append("EnBtn();")
                strScript.Append("}")
                strScript.Append("}")

                strScript.Append("function SaveObjectWithValidation() {")
                strScript.Append("Page_ClientValidate('CheckMandatoryFields');")
                strScript.Append(" if(Page_IsValid){")
                strScript.Append("DisBtn(false);")
                strScript.Append(GetPostBackString(btnSaveWithValidation))
                strScript.Append("} else {")
                strScript.Append("EnBtn();")
                strScript.Append("}")
                strScript.Append("}")

                strScript.Append("function RefreshObject() {")
                strScript.Append("Page_ClientValidate('');")
                strScript.Append(" if(Page_IsValid){")
                strScript.Append("DisBtn(true);")
                strScript.Append(GetPostBackString(btnSave))
                strScript.Append("} else {")
                strScript.Append("EnBtn();")
                strScript.Append("}")
                strScript.Append("}")
                strScript.Append("function SaveObjectAndClose() {")
                strScript.Append("DisBtn(false);")
                strScript.Append(GetPostBackString(btnSaveComplete))
                strScript.Append("}")
                strScript.Append("function SaveObjectAndCloseWindow() {")
                strScript.Append("DisBtn(false);")
                strScript.Append(GetPostBackString(btnSaveCloseWindow))
                strScript.Append("}")
                strScript.Append("function SaveObjectAndNext() {")
                strScript.Append("DisBtn(false);")
                strScript.Append(GetPostBackString(btnSaveNext))
                strScript.Append("}")
                strScript.Append("function SaveObjectAndPrevious() {")
                strScript.Append("DisBtn(false);")
                strScript.Append(GetPostBackString(btnSavePrevious))
                strScript.Append("}")
                strScript.Append("function SaveObjectAndNextEdit() {")
                strScript.Append("DisBtn(false);")
                strScript.Append(GetPostBackString(btnSaveNextEdit))
                strScript.Append("}")

                strScript.Append("function Release() {")
                strScript.Append("DisBtn(true);")
                strScript.Append(GetPostBackString(btnRelease))
                strScript.Append("}")
                strScript.Append("function ReleaseTo(tostep) {")
                strScript.Append("$get('" & hdnReleaseTo.ClientID & "').value = tostep;")
                strScript.Append("DisBtn(true);")
                strScript.Append(GetPostBackString(btnReleaseTo))
                strScript.Append("}")

                strScript.Append("function TransferTo(usr) {")
                strScript.Append("DisBtn(true);")
                strScript.Append("$get('" & hdnReleaseTo.ClientID & "').value = usr;")
                strScript.Append(GetPostBackString(btnTransferTo))
                strScript.Append("}")

                strScript.Append("function Reject() {")
                strScript.Append(" radprompt('', RejectCallBack, 330, 100, null,'" & GetLabel("addcomment") & "', '');")
                strScript.Append("}")


                strScript.Append("function RejectCallBack(args)")
                strScript.Append("{")
                strScript.Append("   if (args != null)")
                strScript.Append("    {")
                strScript.Append("   if (args != '')")
                strScript.Append("    {")
                strScript.Append("DisBtn(true);")
                strScript.Append("$get('" & hdnReleaseTo.ClientID & "').value = args;")
                strScript.Append(GetPostBackString(btnReject))
                strScript.Append("      }}")
                strScript.Append("   }")

                strScript.Append("function Suspend() {")
                strScript.Append("  var oWnd = radopen('./UserControls/PromptDate.aspx?modal=Y', 'wndModalAutosize');")
                strScript.Append(" oWnd.add_close(SuspendCallBack);")
                strScript.Append("}")

                strScript.Append("function SuspendCallBack(winw)")
                strScript.Append("{if (winw.argument){DisBtn(true);$get('" & hdnReleaseTo.ClientID & "').value = winw.argument;")
                strScript.Append(GetPostBackString(btnSuspend))
                strScript.Append("}winw.add_close(SuspendCallBack);}")

                strScript.Append("function CancelEdit() {")
                If Not bInCreation Then
                    strScript.Append("Page_ValidationActive = false;")
                    strScript.Append(GetPostBackString(btnCancel, False, ""))
                Else
                    'strScript.Append("if (confirm('Delete document?')){")
                    strScript.Append("Page_ValidationActive = false;")
                    strScript.Append("parent.DeleteObject();")
                    'strScript.Append("}else{")
                    'strScript.Append("Page_ValidationActive = false;")
                    'strScript.Append("return false;")
                    'strScript.Append("}")
                End If
                strScript.Append("}")

                strScript.Append("function Unlock() {")
                strScript.Append("DisBtn(true);")
                strScript.Append(GetPostBackString(btnUnlock, False, ""))
                strScript.Append("}")

            Else
                'Refresh when not in edit mode
                strScript.Append("function SaveObject() {")
                strScript.Append("RefreshObject();")
                strScript.Append("}")
                strScript.Append("function RefreshObject(fileid) {")
                If UserProfile.AutoOpenFileInDetail Then
                    strScript.Append("if (!(typeof (fileid) == 'undefined' || fileid == null)) {ViewFile(fileid,'','preview');}")
                End If

                'todo, use selecteed tab from screen
                ' QueryStringParser.Remove(QueryStringParser.Parameters.SelectedTab)

                strScript.Append("location.href = '" & ResolveUrl("~/DM_DetailView.aspx") & QueryStringParser.ToString(False) & "';")
                strScript.Append("}")
            End If

            strScript.Append("function ToggleCloseDossier() {")
            strScript.Append("Page_ClientValidate('CheckMandatoryFields');")
            strScript.Append(" if(Page_IsValid){")
            strScript.Append("DisBtn(true);")
            strScript.Append(GetPostBackString(btnToggleCloseDossier))
            strScript.Append("}")
            strScript.Append("}")

            strScript.Append("function UnlockAdmin() {")
            strScript.Append("DisBtn(true);")
            strScript.Append(GetPostBackString(btnUnlockAdmin, False, ""))
            strScript.Append("}")

            strScript.Append("function MoveCaseTo(tostep) {")
            strScript.Append("DisBtn(true);")
            strScript.Append("$get('" & hdnReleaseTo.ClientID & "').value = tostep;")
            strScript.Append(GetPostBackString(btnMoveTo))
            strScript.Append("}")

            strScript.Append("function RunUserEvent(id,validate){")
            strScript.Append("var bValid = true;")
            strScript.Append(" if(validate) {")
            strScript.Append("Page_ClientValidate('CheckMandatoryFields');")
            strScript.Append("bValid = Page_IsValid;")
            strScript.Append("}")
            strScript.Append(" if(bValid){")
            strScript.Append(" DisBtn(true);")
            If Not thisCaseInstance Is Nothing Then
                strScript.Append("PC().DoDocumentListActionCase(10,id," & thisCaseInstance.Tech_ID & ");")
            Else
                strScript.Append("PC().DoDocumentListActionDoc(10,id," & thisObjectDetail.ID & ");")
            End If
            strScript.Append("}")
            strScript.Append("}")

            strScript.Append("function ExecuteUserEvent(id,validate){")
            strScript.Append(" if(!validate) {validate = false;}")
            If Not InEditMode() Then
                strScript.Append("RunUserEvent(id,validate);")
            Else

                strScript.Append(" if (IsDirty() || (id.length == 30 || id.length == 34)){")
                strScript.Append("$get('" & hdnUserEventID.ClientID & "').value = id;")
                strScript.Append(" if(validate) {")
                strScript.Append("SaveObjectWithValidation();")
                strScript.Append(" } else {")
                strScript.Append("SaveObject();")
                strScript.Append(" }")
                strScript.Append("} else {")
                strScript.Append("RunUserEvent(id,validate);")
                strScript.Append("}")
            End If
            strScript.Append("}")


            strScript.Append("function RunCustomAction(id){")
            If Not thisCaseInstance Is Nothing Then
                strScript.Append("PC().DoDocumentListActionCase(1,id," & thisCaseInstance.Tech_ID & ",2);")
            Else
                strScript.Append("PC().DoDocumentListActionDoc(1,id," & thisObjectDetail.ID & ",2);")
            End If
            strScript.Append("}")

            strScript.Append("function ExecuteCustomAction(id){")
            If Not InEditMode() Then
                strScript.Append("RunCustomAction(id);")
            Else
                strScript.Append(" if (IsDirty()){")
                strScript.Append("$get('")
                strScript.Append(hdnCustActionID.ClientID)
                strScript.Append("').value = id;")
                strScript.Append("SaveObject();")
                strScript.Append("} else {")
                strScript.Append("RunCustomAction(id);")
                strScript.Append("}")
            End If
            strScript.Append("}")

            strScript.Append("function CloseWindow(){")
            If Not QueryStringParser.Preview Then
                strScript.Append("parent.CloseWindow();")
            Else
                strScript.Append("parent.CloseDetail(true);")
            End If

            strScript.Append("}")

            ClientScript.RegisterClientScriptBlock(Me.GetType, "detailviewfuncs", strScript.ToString, True)

            ReenablePage()

            If Not IsPostBack Then
                If Not isBuggyIE11() Then
                    domadetail.Focus()
                End If
            End If

        Else
            If QueryStringParser.Preview Then
                Response.End()
            End If
        End If

    End Sub

    Private Function isBuggyIE11() As Boolean
        Return Request.UserAgent.StartsWith("Mozilla") AndAlso Request.UserAgent.Contains("Trident/7")
    End Function

    Private Sub ParseQueryString()

        ForceNextCaseOnClose = QueryStringParser.GetBoolean("forcenext")
        _selectedTab = QueryStringParser(QueryStringParser.Parameters.SelectedTab)

        QueryStringParser.Remove("selectedtab") 'don't use it anymore
    End Sub

    Public Function BindDetail() As Boolean
        Dim bView As Boolean

        Dim loLoaded As Arco.ApplicationServer.Library.BusinessBase
        Try
            loLoaded = QueryStringParser.CurrentDMObject
        Catch ex As Exception
            Arco.Utils.Logging.LogError("Error loading CurrentDMObject", ex)
            GotoErrorPage(LibError.ErrorCode.ERR_UNEXPECTED)
            Return False
        End Try

        If loLoaded Is Nothing Then
            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
            Return False
        End If

        If Not IsPostBack Then
            If Not String.IsNullOrEmpty(_selectedTab) Then
                domadetail.SelectedTabID = CType(_selectedTab, Int32)
            End If
        End If

        If TypeOf (loLoaded) Is HistoryCase Then

            Dim loCaseHist As HistoryCase = CType(loLoaded, HistoryCase)
            If Not loCaseHist.CanViewMeta Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                Return False
            End If
            thisObjectDetail = loCaseHist
            Dim screenMode As Screen.DetailScreenDisplayMode = If(QueryStringParser.Preview, Screen.DetailScreenDisplayMode.Preview, Screen.DetailScreenDisplayMode.ReadOnly)
            bView = BindDetailScreen(loCaseHist, screenMode, screenMode, thisObjectDetail.CanViewMeta)
            If bView Then
                domadetail.DataBind(loCaseHist)
            End If


        ElseIf TypeOf (loLoaded) Is DM_OBJECT Then
            thisObjectDetail = CType(loLoaded, DM_OBJECT)
            bInCreation = (thisObjectDetail.Status = DM_OBJECT.Object_Status.InCreation)

            If thisObjectDetail.Object_Type = "Mail" Then
                Response.Redirect("./MailClient/View.aspx?DM_MAIL_ID=" & thisObjectDetail.Object_Reference & "&PREVIEW=Y", False)
                Context.ApplicationInstance.CompleteRequest()
                Return False
            ElseIf thisObjectDetail.Object_Type = "Shortcut" Then
                Response.Redirect("DM_DetailView.aspx?DM_OBJECT_DIN=" & thisObjectDetail.Object_Reference & "&mode=" & QueryStringParser.GetString("mode") & "&preview=" & QueryStringParser.GetString("preview") & "&godefault=" & QueryStringParser.GetString("godefault") & QueryStringParser.CreateQueryStringPart("terms", QueryStringParser.GetString("terms")) & "&objectlist=" & QueryStringParser.GetString("objectlist"), False)
                Context.ApplicationInstance.CompleteRequest()
                Return False
            Else
                If Not thisObjectDetail.CanViewMeta AndAlso Not thisObjectDetail.CanViewFiles Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOBJECT)
                    Return False
                Else
                    bView = True
                End If

                If QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.Edit Then
                    Select Case thisObjectDetail.Status
                        Case DM_OBJECT.Object_Status.Production
                            If thisObjectDetail.CategoryObject IsNot Nothing AndAlso Not thisObjectDetail.CategoryObject.AllowConcurrentEditing Then
                                thisObjectDetail = thisObjectDetail.Lock() 'lock the document -> only the status will be updated
                                If Not thisObjectDetail.IsLockedByCurrentUser Then 'if not rights to edit doc or files, access denied
                                    QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.ReadOnly
                                End If
                            Else
                                If Not thisObjectDetail.CanModifyMeta Then 'if not rights to edit doc or files, access denied
                                    QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.ReadOnly
                                End If
                            End If

                        Case DM_OBJECT.Object_Status.Locked
                            If Not thisObjectDetail.IsLockedByCurrentUser Then 'if the user has it locked already, he has access, if another user has it locked there is no access
                                QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.ReadOnly
                            End If
                        Case Else
                            If Not thisObjectDetail.CanModifyMeta Then 'if not rights to edit doc or files, access denied
                                QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.ReadOnly
                            End If
                    End Select

                ElseIf QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.Admin Then
                    If Not thisObjectDetail.CanAdminData Then
                        QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.ReadOnly
                        QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.ReadOnly
                    End If
                Else
                    If thisObjectDetail.IsLockedByCurrentUser Then
                        If IsPostBack Then
                            thisObjectDetail = thisObjectDetail.UnLock()
                        End If
                    End If
                End If
            End If

            If bView Then
                If thisObjectDetail.CanViewMeta Then
                    If Not IsPostBack Then
                        thisObjectDetail.MarkRead()
                    End If

                    bView = BindDetailScreen(thisObjectDetail, QueryStringParser.DetailDisplayMode, QueryStringParser.DetailDisplayMode, True)

                    If bView Then
                        domadetail.DataBind(thisObjectDetail)
                    End If
                Else
                    bView = BindDetailScreen(thisObjectDetail, QueryStringParser.DetailDisplayMode, QueryStringParser.DetailDisplayMode, False)

                    If bView Then
                        domadetail.DataBind(thisObjectDetail)
                    End If

                End If
            End If

            If Not IsPostBack Then
                If QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.Admin Then
                    thisObjectDetail.AddAudit("VIEWINADMINMODE", "Viewed in admin mode")
                Else
                    thisObjectDetail.AddMetaViewAudit()
                    Selection.AddToRecentDocuments(thisObjectDetail)
                End If

            End If
        Else
            'case instance
            thisCaseInstance = CType(loLoaded, cCase)
            Dim requestedMode As Screen.DetailScreenDisplayMode = QueryStringParser.DetailDisplayMode
            Dim actualMode As Screen.DetailScreenDisplayMode = QueryStringParser.DetailDisplayMode

            Select Case requestedMode
                Case Screen.DetailScreenDisplayMode.ReadOnly, Screen.DetailScreenDisplayMode.Preview
                    'show the edit screen if the user has work (but in readonly actualmode)
                    If thisCaseInstance.CurrentUserHasWork(True) Then
                        actualMode = requestedMode
                        requestedMode = Screen.DetailScreenDisplayMode.Edit
                    End If
                Case Screen.DetailScreenDisplayMode.Edit
                    Dim hasWork As Boolean = thisCaseInstance.CurrentUserHasWork(True)
                    If hasWork AndAlso (Not thisCaseInstance.IsLocked OrElse thisCaseInstance.IsMyCase) Then
                        'we have work and it's not locked by someone else
                    Else
                        actualMode = Screen.DetailScreenDisplayMode.ReadOnly
                        If Not hasWork Then
                            requestedMode = Screen.DetailScreenDisplayMode.ReadOnly
                        End If
                    End If
                Case Screen.DetailScreenDisplayMode.Admin
                    If Not thisCaseInstance.CanAdminData Then
                        actualMode = Screen.DetailScreenDisplayMode.ReadOnly
                        requestedMode = Screen.DetailScreenDisplayMode.ReadOnly
                    End If
            End Select
            If actualMode = Screen.DetailScreenDisplayMode.ReadOnly OrElse actualMode = Screen.DetailScreenDisplayMode.Preview Then
                If Not thisCaseInstance.CanView Then
                    GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                    Return False
                End If
            End If

            bView = BindDetailScreen(thisCaseInstance, requestedMode, actualMode, True)

            If bView Then

                If Not IsPostBack Then
                    thisCaseInstance.MarkRead()
                End If

                domadetail.DataBind(thisCaseInstance)
            End If

            If Not IsPostBack Then
                If QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.Admin Then
                    thisCaseInstance.AddHistoryAction(HistoryAction.WF_HistActionType.WF_ViewInAdminMode, "Viewed in admin mode")
                Else

                    thisCaseInstance.TargetObject.AddMetaViewAudit()
                    Selection.AddToRecentDocuments(thisCaseInstance.TargetObject)
                End If
            End If

        End If
        Return True
    End Function

    Private Function BindDetailScreen(ByVal objectDetailScreen As IHasDetailScreen, ByVal requestedDisplayMode As Screen.DetailScreenDisplayMode, ByVal actualDisplayMode As Screen.DetailScreenDisplayMode, ByVal canViewMeta As Boolean) As Boolean

        Dim screen As Screen = QueryStringParser.GetRequestedScreen(If(thisCaseInstance IsNot Nothing, thisCaseInstance.Proc_ID, 0))

        If screen Is Nothing Then
            screen = objectDetailScreen.GetDetailScreen(requestedDisplayMode, Device.Web, SiteVersion.Version7)
            If screen Is Nothing Then
                Return False

                'Throw New InvalidOperationException("There is no screen defined for this operation " & veRequestedDisplayMode)
            End If
        Else
            'replace include screens
            If thisCaseInstance IsNot Nothing Then
                ScreenSelector.ExpandScreenItems(screen, thisCaseInstance, Device.Web, SiteVersion.Version7)
            Else
                ScreenSelector.ExpandScreenItems(screen, thisObjectDetail, Device.Web, SiteVersion.Version7)
            End If

        End If

        If screen.Type = Screen.ScreenSourceType.DefaultMode OrElse screen.Type = Screen.ScreenSourceType.TemplateFile Then
            domadetail.IsMainScreen = True
            domadetail.ScreenHandlerAssembly = screen.Screen_Assembly
            domadetail.ScreenHandlerClass = screen.Screen_Class
            domadetail.ScreenID = screen.ID
            domadetail.Labels = objectDetailScreen.GetDetailScreenLabels(EnableIISCaching)
            domadetail.FormSize = screen.FormSize
            domadetail.FormFieldsLayout = screen.FormFieldsLayout
            If canViewMeta Then
                domadetail.DisplayItems = screen.ScreenItems.ReplaceAliasProperties(objectDetailScreen)
            Else
                Dim loNew As ScreenItemList = ScreenItemList.GetEmptyScreenItemList

                Dim loFiltered As ScreenItemList = screen.ScreenItems.ReplaceAliasProperties(objectDetailScreen)
                For Each item As ScreenItemList.ScreenItemInfo In loFiltered
                    If item.Type = ScreenItem.ItemType.Fixed AndAlso (item.FixedItemID = ScreenItem.FixedItem.Files OrElse item.FixedItemID = ScreenItem.FixedItem.Comments) Then
                        loNew.Add(item)
                    End If
                Next
                domadetail.DisplayItems = loNew
            End If

            domadetail.Template = ""
            domadetail.DisplayMode = actualDisplayMode
            If Not QueryStringParser.Preview Then
                Dim lbAutoFileOpened As Boolean = False
                If Not String.IsNullOrEmpty(hdnOpenFile.Value) Then
                    Dim llFileId As Int32 = 0
                    If Int32.TryParse(hdnOpenFile.Value, llFileId) Then
                        If UserProfile.AutoOpenFileInDetail Then
                            Dim sb As New StringBuilder

                            sb.AppendLine("function AutoOpenFile(){")
                            sb.AppendLine("ViewFile(" & llFileId & ",'','preview');")
                            sb.AppendLine("}")
                            sb.AppendLine("Sys.Application.add_load(AutoOpenFile);")

                            ClientScript.RegisterStartupScript(Me.GetType, "AutoOpenFile", sb.ToString, True)
                            lbAutoFileOpened = True
                        End If
                    End If
                    hdnOpenFile.Value = ""
                End If
                If Not lbAutoFileOpened Then
                    If screen.DefaultPreview > 1 Then
                        domadetail.AutoOpenPackage = screen.DefaultPreview
                    End If
                End If
            End If

            If domadetail.DisplayMode = Screen.DetailScreenDisplayMode.Edit Then
                domadetail.ValidationScript = screen.ValidationScript
            End If

            If screen.Type = Screen.ScreenSourceType.TemplateFile Then
                If Not String.IsNullOrEmpty(screen.Source) Then
                    domadetail.Template = screen.Source
                Else
                    Response.Write("No Template File Specified")
                End If
            End If
            Return True
        Else
            Dim lsUrl As String = screen.Source
            If thisCaseInstance Is Nothing Then
                lsUrl = TagReplacer.ReplaceTags(lsUrl, thisObjectDetail)
            Else
                lsUrl = TagReplacer.ReplaceTags(lsUrl, thisCaseInstance)
            End If
            Response.Redirect(QueryStringParser.AppendTo(lsUrl), True)
            Response.End()

            Return False
        End If
    End Function

    Protected Sub domadetail_CategoryChanged(ByVal catid As Integer) Handles domadetail.CategoryChanged
        thisObjectDetail = thisObjectDetail.ChangeCategory(catid)

        ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.LoadContent('dm_detail.aspx" & QueryStringParser.ToString() & "');", True)
    End Sub

    Protected Sub btnCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancel.Click
        If thisObjectDetail.Status <> DM_OBJECT.Object_Status.InCreation Then
            thisObjectDetail.UnLock()
        End If

        QueryStringParser.AddOrReplace("mode", "1")
        ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.LoadContent('dm_detail.aspx" & QueryStringParser.ToString() & "');", True)

    End Sub

    Protected Sub btnSaveWithValidation_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveWithValidation.Click
        Save(False, False, False, 0, False)
    End Sub

    Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSave.Click
        Save(False, False, False, 0, False)
    End Sub
    Protected Sub btnSaveComplete_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveComplete.Click
        Save(True, False, False, 0, False)
    End Sub
    Protected Sub btnSaveCloseWindow_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveCloseWindow.Click
        Save(True, False, False, 0, True)
    End Sub
    Protected Function InEditMode() As Boolean
        Return (QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.Edit OrElse QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.Admin)
    End Function
    Protected Sub btnTransferTo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnTransferTo.Click
        If Not thisCaseInstance Is Nothing Then
            If Not IsValid OrElse Not InEditMode() Then
                ReEnableWindow()
                Exit Sub
            End If
            Dim lbOk As Boolean = domadetail.Save(thisCaseInstance, True, True)
            If lbOk Then
                Dim lsUser As String = hdnReleaseTo.Value
                thisCaseInstance = thisCaseInstance.TransferToUser(lsUser)
                CloseCase(0, 0)
            Else
                ReEnableWindow()
            End If
        Else
            ReEnableWindow()
        End If

    End Sub
    Protected Sub btnMoveTo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnMoveTo.Click
        Dim llToStep As Int32 = GetTargetStep()
        If Not IsValid Then
            ReEnableWindow()
            Exit Sub
        End If

        Dim lbOk As Boolean = True
        If InEditMode() Then
            lbOk = domadetail.Save(thisCaseInstance, False, False)
        End If
        If lbOk Then
            Dim techId As Int32 = thisCaseInstance.Tech_ID
            Dim objectId As Int32 = thisCaseInstance.DM_Object_ID
            If thisCaseInstance.CanAdminCaseStatus Then
                thisCaseInstance = thisCaseInstance.MoveToStep(llToStep)
                If Not thisCaseInstance.IsClosed Then 'not ended                    
                    ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.LoadContent('dm_detail.aspx" & QueryStringParser.ToString & "');", True)
                Else
                    CloseCase(objectId, techId)
                End If
            Else
                ReEnableWindow()
            End If
        Else
            ReEnableWindow()
        End If
    End Sub
    Private Function GetTargetStep() As Integer
        Dim stepId As Integer
        If Int32.TryParse(hdnReleaseTo.Value, stepId) AndAlso stepId <> 0 Then
            Return stepId
        End If

        stepId = thisCaseInstance.CurrentProcedure.GetStepID(hdnReleaseTo.Value)
        If stepId <> 0 Then
            Return stepId
        End If

        Throw New InvalidOperationException("Step " & hdnReleaseTo.Value & " was not found")
    End Function
    Protected Sub btnReleaseTo_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnReleaseTo.Click
        Release(GetTargetStep())
    End Sub
    Private Sub Release(ByVal vlToStep As Int32)
        If Not IsValid OrElse Not InEditMode() Then
            Exit Sub
        End If

        Dim lbOk As Boolean = domadetail.Save(thisCaseInstance, False, True)
        If lbOk Then
            Dim parentCaseTechId As Int32 = thisCaseInstance.Parent_Case
            Dim stepId As Int32 = thisCaseInstance.Step_ID
            Dim techId As Int32 = thisCaseInstance.Tech_ID
            Dim objectId As Int32 = thisCaseInstance.DM_Object_ID


            thisCaseInstance = thisCaseInstance.Dispatch(vlToStep)

            Dim openCase As Boolean = False

            If Not thisCaseInstance.IsClosed Then 'not ended
                If Settings.GetValue("Interface", "AutoFlow", True) OrElse stepId = thisCaseInstance.Step_ID Then
                    openCase = thisCaseInstance.CurrentUserHasWork
                    If Not openCase Then
                        If thisCaseInstance.IsWaitingForChildCases Then
                            For Each subCase As cCase In thisCaseInstance.GetChildCases()
                                If subCase.CurrentUserHasWork Then
                                    ReplaceInQueryString(subCase)
                                    openCase = True
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                End If
            Else
                If Settings.GetValue("Interface", "AutoFlow", True) Then
                    If parentCaseTechId > 0 Then 'closed but we had a parent case                      
                        Dim parentCase As cCase = cCase.GetCase(parentCaseTechId)

                        If parentCase IsNot Nothing Then
                            If parentCase.CurrentUserHasWork Then
                                ReplaceInQueryString(parentCase)
                                openCase = True
                            ElseIf parentCase.IsWaitingForChildCases() Then
                                Dim parentCaseSubCases As List(Of cCase) = parentCase.GetChildCases()
                                If parentCaseSubCases.Count > 0 Then
                                    Dim subCaseWithWork As cCase = parentCaseSubCases.FirstOrDefault(Function(x) x.CurrentUserHasWorkRecursive())
                                    If subCaseWithWork IsNot Nothing Then
                                        ReplaceInQueryString(subCaseWithWork)
                                    Else
                                        ReplaceInQueryString(parentCaseSubCases.First())
                                    End If
                                    openCase = True
                                End If

                            End If
                        End If
                    ElseIf thisCaseInstance.CurrentProcedure.DM_ACTION = Procedure.ProcedureObjectAction.UpdateObject _
                        AndAlso thisCaseInstance.CurrentProcedure.Mode = Procedure.ProcedureMode.Virtual Then
                        QueryStringParser.Remove("RTCASE_TECH_ID")
                        QueryStringParser.Remove("RTCASE_CASE_ID")
                        QueryStringParser.Remove("TECH_ID")
                        QueryStringParser.Remove("CASE_ID")
                        QueryStringParser.AddOrReplace("DM_OBJECT_ID", thisCaseInstance.DM_Object_ID.ToString())
                        openCase = True
                    End If
                End If

            End If
            If openCase Then
                ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.LoadContent('dm_detail.aspx" & QueryStringParser.ToString & "');", True)
            Else
                CloseCase(objectId, techId)
            End If

        Else
            ReEnableWindow()
        End If
    End Sub

    Private Sub ReplaceInQueryString(ByVal withCase As cCase)
        QueryStringParser.AddOrReplace("RTCASE_TECH_ID", withCase.Tech_ID.ToString())
        QueryStringParser.AddOrReplace("RTCASE_CASE_ID", withCase.Case_ID.ToString())
        QueryStringParser.AddOrReplace("DM_OBJECT_ID", withCase.DM_Object_ID.ToString())

        QueryStringParser.Remove("TECH_ID")
        QueryStringParser.Remove("CASE_ID")

    End Sub
    Protected Sub btnRelease_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRelease.Click
        Release(0)
    End Sub

    Protected Sub btnSaveNext_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveNext.Click
        Save(True, True, False, 1, False)
    End Sub
    Protected Sub btnSavePrevious_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSavePrevious.Click
        Save(True, False, True, 1, False)
    End Sub
    Protected Sub btnSaveNextEdit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSaveNextEdit.Click
        Save(True, True, False, 2, False)
    End Sub
    Protected Sub btnToggleCloseDossier_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnToggleCloseDossier.Click
        Dim lbOk As Boolean
        If InEditMode() Then
            If Not IsValid OrElse Not InEditMode() Then
                If Not IsValid Then
                    Response.Write("The page isn't valid and couldn't be saved (you should never get this message)")
                End If
                Exit Sub
            End If

            If Not thisCaseInstance Is Nothing Then
                lbOk = domadetail.Save(thisCaseInstance, False, (QueryStringParser.DetailDisplayMode <> Screen.DetailScreenDisplayMode.Admin))
            Else
                lbOk = domadetail.Save(thisObjectDetail, False)
            End If
        Else
            lbOk = True
        End If


        If lbOk Then
            If thisCaseInstance IsNot Nothing Then
                If thisCaseInstance.TargetObject.CanClose Then
                    thisCaseInstance.TargetObject.Close()
                    lbOk = Not thisCaseInstance.TargetObject.HasError
                ElseIf thisCaseInstance.TargetObject.CanUnClose Then
                    thisCaseInstance.TargetObject.UnClose()
                    lbOk = Not thisCaseInstance.TargetObject.HasError
                End If
                If Not lbOk Then
                    domadetail.ErrorMessage = thisObjectDetail.GetLastError.Description
                End If

            Else
                If thisObjectDetail.CanClose Then
                    thisObjectDetail.Close()
                    lbOk = Not thisObjectDetail.HasError
                ElseIf thisObjectDetail.CanUnClose Then
                    thisObjectDetail.UnClose()
                    lbOk = Not thisObjectDetail.HasError
                End If

                If Not lbOk Then
                    domadetail.ErrorMessage = thisObjectDetail.GetLastError.Description
                End If
            End If
        End If

        If lbOk Then
            QueryStringParser.AddOrReplace("mode", "1")
            QueryStringParser.AddOrReplace("selectedtab", _selectedTab)
            ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.LoadContent('dm_detail.aspx" & QueryStringParser.ToString & "');", True)
        End If
    End Sub
    Private Sub Save(ByVal unlockAfterSave As Boolean, ByVal gotonext As Boolean, ByVal gotoprev As Boolean, ByVal gotonextmode As Int32, ByVal closeWindow As Boolean)
        If Not IsValid Then
            Exit Sub
        End If
        If Not InEditMode() Then
            'clicked from within the read only
            ExecuteUserEventAfterPostBack()
            ExecuteCustomActionAfterPostBack()
            Exit Sub
        End If

        Dim lbOk As Boolean


        If Not thisCaseInstance Is Nothing Then
            lbOk = domadetail.Save(thisCaseInstance, unlockAfterSave, (QueryStringParser.DetailDisplayMode <> Screen.DetailScreenDisplayMode.Admin))

            thisObjectDetail = thisCaseInstance.TargetObject
        Else
            If domadetail.DisplayMode = Screen.DetailScreenDisplayMode.Admin AndAlso thisObjectDetail.Status = DM_OBJECT.Object_Status.InCreation Then
                unlockAfterSave = True 'attempt to complete the creation
            End If
            lbOk = domadetail.Save(thisObjectDetail, unlockAfterSave)
        End If


        If lbOk Then
            _selectedTab = domadetail.SelectedTabID.ToString
            Dim lbParentReload As Boolean = unlockAfterSave
            Dim refreshTree As Boolean = domadetail.RefreshTree AndAlso thisObjectDetail IsNot Nothing

            Dim lsFolderIdToShow As Int32
            If domadetail.NewTreeIDToShow > 0 Then
                lsFolderIdToShow = domadetail.NewTreeIDToShow
            Else
                lsFolderIdToShow = 0
            End If
            If gotonext Then
                If thisCaseInstance Is Nothing Then
                    ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.NextDoc(" & gotonextmode & ");", True)
                Else
                    ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.GotoNextCaseAfterClose(0,0);", True)
                End If
            ElseIf gotoprev Then
                If thisCaseInstance Is Nothing Then
                    ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.PreviousDoc(" & gotonextmode & ");", True)
                Else
                    ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.GotoPreviousCaseAfterClose(0,0);", True)
                End If
            Else

                QueryStringParser.AddOrReplace("folderid", lsFolderIdToShow.ToString)
                If lbParentReload OrElse refreshTree Then
                    QueryStringParser.AddOrReplace("parentreload", "true")
                End If
                If refreshTree Then
                    QueryStringParser.AddOrReplace("refreshtree", True)
                Else
                    QueryStringParser.Remove("refreshtree")
                End If

                If Modal AndAlso Not closeWindow Then
                    If refreshTree Then
                        ClientScript.RegisterStartupScript(Me.GetType(), "AddRefreshTreeAfterModal", "parent.PC().SetGoToTreeId(" & thisObjectDetail.Parent_ID & ");parent.GetRadWindow().add_close(parent.MainPage().RefreshTreeAfterModal);", True)
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "AddRefreshAfterModal", "parent.GetRadWindow().add_close(parent.MainPage().RefreshAfterModal);", True)
                End If

                If unlockAfterSave Then
                    If Not closeWindow Then
                        QueryStringParser.AddOrReplace("mode", "1")
                        QueryStringParser.AddOrReplace("selectedtab", _selectedTab)
                        ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.LoadContent('dm_detail.aspx" & QueryStringParser.ToString & "');", True)
                    Else
                        If refreshTree Then
                            ClientScript.RegisterStartupScript(GetType(String), "RefreshTree", "parent.RefreshParentPage(" & thisObjectDetail.Parent_ID & ",true,false);", True)
                        End If
                        CloseMe()
                    End If
                Else
                    'normal save, this can also be to execute events
                    ExecuteUserEventAfterPostBack()
                    ExecuteCustomActionAfterPostBack()

                    If refreshTree Then
                        ClientScript.RegisterStartupScript(GetType(String), "RefreshTree", "parent.RefreshParentPage(" & thisObjectDetail.Parent_ID & ",true,false);", True)
                    End If
                End If
            End If
        End If
    End Sub
    Private Sub ExecuteUserEventAfterPostBack()
        If String.IsNullOrEmpty(hdnUserEventID.Value) Then Return

        Dim userventid As String = hdnUserEventID.Value
        Dim i As Integer
        If Arco.Utils.GUID.IsGUID(userventid) Then
            Dim loUev As UserEvent

            If Not thisCaseInstance Is Nothing Then
                loUev = UserEvent.GetUserEvent(userventid, thisCaseInstance.Proc_ID)
                If loUev.ID = 0 Then
                    loUev = UserEvent.GetUserEvent(userventid)
                End If
            Else
                loUev = UserEvent.GetUserEvent(userventid)
            End If

            If loUev.ID > 0 Then
                userventid = loUev.ID.ToString()
            End If
        ElseIf Integer.TryParse(userventid, i) Then
            userventid = i.ToString()
        Else
            Return
        End If

        Dim sb As New StringBuilder(128)
        sb.Append("function RunMyUserEvent(){RunUserEvent(")
        sb.Append(userventid)
        sb.Append(",false);}Sys.Application.add_load(RunMyUserEvent);")

        ClientScript.RegisterStartupScript(Me.GetType, "ExecuteEvent", sb.ToString, True)
        hdnUserEventID.Value = ""

    End Sub
    Private Sub ExecuteCustomActionAfterPostBack()
        If String.IsNullOrEmpty(hdnCustActionID.Value) Then Return

        Dim sb As New StringBuilder(128)
        sb.Append("function RunMyCustAction(){RunCustomAction(")
        sb.Append(hdnCustActionID.Value)
        sb.AppendLine(");}Sys.Application.add_load(RunMyCustAction);")
        ClientScript.RegisterStartupScript(Me.GetType, "ExecuteCustAction", sb.ToString, True)
        hdnCustActionID.Value = ""

    End Sub

    Private Sub ReenablePage()

        If Not QueryStringParser.Preview Then
            ClientScript.RegisterStartupScript(GetType(String), "ReenablePage", "parent.EnableButtons();parent.EnableWindow();", True)
        End If

    End Sub
    Private Sub CloseMe()
        ClientScript.RegisterStartupScript(GetType(String), "CloseOnReload", "CloseWindow();", True)
    End Sub
    Protected Sub btnUnlockAdmin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUnlockAdmin.Click
        If Not thisCaseInstance Is Nothing Then
            If IsValid AndAlso QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.Admin Then
                If thisCaseInstance.CanAdminData Then
                    domadetail.Save(thisCaseInstance, False, False)
                End If
            End If
            thisCaseInstance = thisCaseInstance.UnLockCase

            If QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.Admin Then
                ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.LoadContent('dm_detail.aspx" & QueryStringParser.ToString & "');", True)
            Else
                CloseCase(thisCaseInstance.DM_Object_ID, thisCaseInstance.Tech_ID)
            End If
        Else
            If IsValid AndAlso QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.Admin Then
                If thisObjectDetail.CanAdminData Then
                    domadetail.Save(thisObjectDetail, False)
                End If
            End If

            thisObjectDetail = thisObjectDetail.UnLock()

            If QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.Admin Then
                ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.LoadContent('dm_detail.aspx" & QueryStringParser.ToString & "');", True)
            Else
                CloseMe()
            End If


        End If
    End Sub

    Protected Sub btnUnlock_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnUnlock.Click
        Dim lbok As Boolean = False
        If Not thisCaseInstance Is Nothing Then
            If Not IsValid OrElse Not InEditMode() Then
                Exit Sub
            End If

            If QueryStringParser.DetailDisplayMode = Screen.DetailScreenDisplayMode.Edit Then
                lbok = domadetail.Save(thisCaseInstance, True, True)
                If lbok Then
                    CloseCase(thisCaseInstance.DM_Object_ID, thisCaseInstance.Tech_ID)
                End If
            End If
        Else
            lbok = True
            thisObjectDetail.UnLock()
            CloseMe()
        End If
        If Not lbok Then
            ReEnableWindow()
        End If
    End Sub
    Private Sub ReEnableWindow()
        ClientScript.RegisterStartupScript(GetType(String), "CloseOnReload", "parent.EnableWindow();", True)
    End Sub
    ''' <summary>
    ''' Implements the close case logic
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub CloseCase(ByVal itemObjectIdRemove As Int32, ByVal itemTechIdRemove As Int32)
        If Not ForceNextCaseOnClose AndAlso Not UserProfile.OpenNextCaseOnClose Then
            CloseMe()
        Else
            ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.GotoNextCaseAfterClose(" & itemObjectIdRemove & "," & itemTechIdRemove & ");", True)
        End If
    End Sub
    Protected Sub btnReject_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnReject.Click
        If Not thisCaseInstance Is Nothing Then
            If Not IsValid OrElse Not InEditMode() Then
                Exit Sub
            End If
            If domadetail.Save(thisCaseInstance, False, True) Then
                If thisCaseInstance.Rejectable Then
                    thisCaseInstance = thisCaseInstance.Reject(hdnReleaseTo.Value)
                    Dim lbOpenSameCase As Boolean = False
                    If Settings.GetValue("Interface", "AutoFlow", True) Then
                        lbOpenSameCase = thisCaseInstance.CurrentUserHasWork
                    End If
                    If lbOpenSameCase Then
                        ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "parent.LoadContent('dm_detail.aspx" & QueryStringParser.ToString & "');", True)
                    Else
                        CloseCase(thisCaseInstance.DM_Object_ID, thisCaseInstance.Tech_ID)
                    End If
                End If
            Else
                ReEnableWindow()
            End If
        End If
    End Sub
    Protected Sub btnSuspend_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSuspend.Click
        If Not thisCaseInstance Is Nothing Then
            If Not IsValid OrElse Not InEditMode() Then
                Exit Sub
            End If
            If domadetail.Save(thisCaseInstance, True, True) Then
                If thisCaseInstance.Step_ID = 0 OrElse TypeOf thisCaseInstance.CurrentStep.Step_Type.Handler Is Arco.Doma.Library.Routing.StepTypes.ISuspendableStepType Then
                    thisCaseInstance = thisCaseInstance.Suspend(hdnReleaseTo.Value)
                    CloseCase(thisCaseInstance.DM_Object_ID, thisCaseInstance.Tech_ID)
                End If
            Else
                ReEnableWindow()
            End If
        End If
    End Sub
End Class
