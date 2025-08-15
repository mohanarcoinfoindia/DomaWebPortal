Imports Arco.Doma.Library
Imports Arco.Doma.Library.Website


Partial Class TaskParams
    Inherits BasePage

    Private thisObjectDetail As baseObjects.DM_OBJECT
    Private taskId As Int32
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit

        CType(Master, BaseMasterPage).ReloadFunction = "SaveObject()"

    End Sub

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As EventArgs) Handles MyBase.Load

        Page.Title = GetDecodedLabel("starttask")
        lblCancel.Text = GetDecodedLabel("cancel")
        cmdSave.Text = GetDecodedLabel("starttask")
        taskId = QueryStringParser.GetInt("TASK_ID")
        If taskId = 0 Then
            Response.Write("invalid parameters")
            Response.End()
        End If

        Dim loTask As Tasks.Task = Tasks.Task.GetTask(taskId)

        thisObjectDetail = ObjectRepository.GetObject(loTask.ObjectId)
        If thisObjectDetail Is Nothing Then
            Response.Write("invalid parameters")
            Response.End()
        End If

        If thisObjectDetail.CanViewMeta Then 'todo canstarttask

            If BindDetailScreen(thisObjectDetail, loTask) Then
                If Not Page.IsPostBack Then
                    docform.DataBind(thisObjectDetail)
                End If
            End If
        End If


        Dim strScript As StringBuilder = New StringBuilder

        If Not Page.IsPostBack Then
            docform.Focus()
            If Not String.IsNullOrEmpty(docform.FocusedControl) Then
                strScript.AppendLine("function InitFocus(){ $get(" & EncodingUtils.EncodeJsString(docform.FocusedControl) & ").focus();}")
                strScript.AppendLine("function pageLoad(sender, eventArgs){setTimeout('InitFocus();', 200);}")
            End If
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
    Private Function BindDetailScreen(ByVal voObject As IHasDetailScreen, ByVal voTask As Tasks.Task) As Boolean
        Dim loType As Tasks.TaskType = Tasks.TaskType.GetTaskType(voTask.TypeId)
        Dim loScreen As Screen = Screen.GetScreen(loType.ParameterScreen)

        If loScreen.Type = Screen.ScreenSourceType.DefaultMode OrElse loScreen.Type = Screen.ScreenSourceType.TemplateFile Then
            docform.ScreenHandlerAssembly = loScreen.Screen_Assembly
            docform.ScreenHandlerClass = loScreen.Screen_Class
            docform.ScreenID = loScreen.ID
            docform.Labels = voObject.GetDetailScreenLabels(Me.EnableIISCaching)
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
            Dim lsUrl As String = TagReplacer.ReplaceTags(loScreen.Source, DirectCast(voObject, Routing.cCase))

            Response.Redirect(QueryStringParser.AppendTo(lsUrl))
            Return False
        End If
    End Function

    Protected Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click

        If Not Page.IsValid Then
            Exit Sub
        End If

        If docform.Save(thisObjectDetail, False) Then
            Page.ClientScript.RegisterStartupScript(Me.GetType, "Redirect", "MainPage().ContinueStartTask(" & taskId & ");Close();", True)
        End If
    End Sub

    Protected Sub lnkRefresh_Click(sender As Object, e As EventArgs) Handles lnkrefresh.Click

        If Not Page.IsValid Then
            Exit Sub
        End If

        docform.Save(thisObjectDetail, False)
    End Sub


End Class
