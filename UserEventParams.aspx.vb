Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Routing
Imports Arco.Doma.Library.Website

Partial Class UserEventParams
    Inherits BasePage
    Private ReadOnly thisObjectDetails As List(Of DM_OBJECT) = New List(Of DM_OBJECT)
    Private ReadOnly thisCaseInstances As List(Of cCase) = New List(Of cCase)

    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit

        CType(Master, BaseMasterPage).ReloadFunction = "SaveObject()"

    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load

        Dim loUEV As UserEvent = UserEvent.GetUserEvent(QueryStringParser.GetInt("UEV_ID"))

        Form.DefaultButton = cmdSave.UniqueID
        Page.Title = loUEV.Caption
        lblCancel.Text = GetDecodedLabel("cancel")
        cmdSave.Text = GetDecodedLabel("ok") ' Page.Title ' GetDecodedLabel("execute")

        Dim bView As Boolean = True
        Dim loLoaded As Arco.ApplicationServer.Library.BusinessBase = QueryStringParser.CurrentDMObject
        If Not loLoaded Is Nothing Then
            If TypeOf (loLoaded) Is HistoryCase Then
                Response.Write("not supported")
                Response.End()

            ElseIf TypeOf (loLoaded) Is DM_OBJECT Then
                thisObjectDetails.Add((CType(loLoaded, DM_OBJECT)))
            Else
                thisCaseInstances.Add(CType(loLoaded, cCase))
            End If
        ElseIf Not Request.QueryString("SEL_TYPE") Is Nothing Then
            Dim loSelCrit As DMSelectionList.Criteria = New DMSelectionList.Criteria
            loSelCrit.User = Security.BusinessIdentity.CurrentIdentity.Name
            loSelCrit.Type = CType(Request.QueryString("SEL_TYPE"), DMSelection.SelectionType)
            loSelCrit.isDMObjectSelection = False

            For Each loSelInfo As DMSelectionList.SelectionInfo In DMSelectionList.GetSelection(loSelCrit)
                Select Case loSelCrit.Type
                    Case DMSelection.SelectionType.CurrentCases, DMSelection.SelectionType.CurrentCasesByCaseID

                        Dim loCase As cCase
                        If loSelCrit.Type = DMSelection.SelectionType.CurrentCases Then
                            loCase = cCase.GetCase(loSelInfo.Object_ID)
                        Else
                            loCase = cCase.GetCaseByCaseID(loSelInfo.Object_ID)
                        End If

                        If loUEV.Object_Type = "Procedure" Then
                            If (loUEV.Object_ID = 0 OrElse loUEV.Object_ID = loCase.Proc_ID) Then
                                thisCaseInstances.Add(loCase)
                            End If

                        ElseIf loUEV.Object_Type = "Category" Then 'execute on the target object
                            If (loUEV.Object_ID = 0 OrElse loUEV.Object_ID = loCase.TargetObject.Category) Then
                                thisObjectDetails.Add(loCase.TargetObject)
                            End If
                        End If

                    Case Else
                        Dim o As DM_OBJECT = ObjectRepository.GetObject(loSelInfo.Object_ID)
                        If (loUEV.Object_ID = 0 OrElse loUEV.Object_ID = o.Category) Then
                            thisObjectDetails.Add(o)
                        End If

                End Select
            Next

            If thisObjectDetails.Count = 0 AndAlso thisCaseInstances.Count = 0 Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOPERATIONONOBJECTTYPE)
            End If

        Else
            Response.Write("invalid parameters")
            Response.End()
        End If



        If thisObjectDetails.Any Then

            If bView Then
                If thisObjectDetails(0).CanViewMeta Then

                    bView = BindDetailScreen(thisObjectDetails(0), loUEV)

                    If bView Then
                        docform.DataBind(thisObjectDetails(0))
                    End If
                End If
            End If
        Else
            'case instance

            bView = BindDetailScreen(thisCaseInstances(0), loUEV)

            If bView Then
                docform.DataBind(thisCaseInstances(0))
            End If
        End If
        Dim strScript As New StringBuilder

        If bView AndAlso Not Page.IsPostBack Then
            docform.Focus()
            Dim focusedControl As String = docform.FocusedControl
            If String.IsNullOrEmpty(focusedControl) Then
                focusedControl = cmdSave.ClientID
            End If

            strScript.AppendLine("function InitFocus(){ $get(" & EncodingUtils.EncodeJsString(focusedControl) & ").focus();}")
            strScript.AppendLine("function pageLoad(sender, eventArgs){setTimeout('InitFocus();', 200);}")
        End If

        strScript.AppendLine("function SaveObject() {")
        strScript.AppendLine("Page_ClientValidate('');")
        strScript.AppendLine(" if(Page_IsValid){")
        strScript.AppendLine(GetPostBackString(lnkrefresh) & ";")
        strScript.AppendLine("}")
        strScript.AppendLine("}")

        Page.ClientScript.RegisterStartupScript(Me.GetType, "detailviewfuncs", strScript.ToString, True)
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
        Return Page.ClientScript.GetPostBackEventReference(objPostBackoptions)

    End Function
    Private Function BindDetailScreen(ByVal voObject As IHasDetailScreen, ByVal voUserEvent As UserEvent) As Boolean
        Dim loScreen As Screen = Screen.GetScreen(voUserEvent.ParameterScreen)

        If loScreen.Type = Screen.ScreenSourceType.DefaultMode OrElse loScreen.Type = Screen.ScreenSourceType.TemplateFile Then
            docform.ScreenHandlerAssembly = loScreen.Screen_Assembly
            docform.ScreenHandlerClass = loScreen.Screen_Class
            docform.ScreenID = loScreen.ID
            docform.Labels = voObject.GetDetailScreenLabels(EnableIISCaching)
            docform.DisplayItems = loScreen.ScreenItems()
            docform.Template = ""
            docform.DisplayMode = Screen.DetailScreenDisplayMode.Edit
            docform.IsMainScreen = False

            docform.ValidationScript = loScreen.ValidationScript

            If loScreen.Type = Screen.ScreenSourceType.TemplateFile Then
                If Not String.IsNullOrEmpty(loScreen.Source) Then
                    docform.Template = loScreen.Source
                Else
                    Response.Write("No Template File Specified")
                End If
            End If
            Return True
        Else
            Dim lsUrl As String = loScreen.Source
            If thisCaseInstances.Any Then
                lsUrl = TagReplacer.ReplaceTags(lsUrl, DirectCast(voObject, cCase))
            Else
                lsUrl = TagReplacer.ReplaceTags(lsUrl, DirectCast(voObject, DM_OBJECT))
            End If
            Response.Redirect(QueryStringParser.AppendTo(lsUrl))
            Return False
        End If
    End Function

    Protected Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click

        If Not Page.IsValid Then
            Exit Sub
        End If

        Dim lbOk As Boolean

        If thisCaseInstances.Any Then
            For Each loCase As cCase In thisCaseInstances
                lbOk = docform.Save(loCase, False, True)
                If Not lbOk Then Exit For
            Next
        Else
            For Each loObject As DM_OBJECT In thisObjectDetails
                lbOk = docform.Save(loObject, False)
                If Not lbOk Then Exit For
            Next
        End If

        If lbOk Then
            Page.ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "MainPage().ShowProgress();MainPage().GetActionWindow().ContinueUserEvent();Close();", True)
        End If
    End Sub

    Protected Sub lnkRefresh_Click(sender As Object, e As EventArgs) Handles lnkrefresh.Click

        If Not Page.IsValid Then
            Exit Sub
        End If

        Dim lbOk As Boolean

        If thisCaseInstances.Any Then
            For Each loCase As cCase In thisCaseInstances
                lbOk = docform.Save(loCase, False, True)
                If Not lbOk Then Exit For
            Next
        Else
            For Each loObject As DM_OBJECT In thisObjectDetails
                lbOk = docform.Save(loObject, False)
                If Not lbOk Then Exit For
            Next
        End If

    End Sub

End Class
