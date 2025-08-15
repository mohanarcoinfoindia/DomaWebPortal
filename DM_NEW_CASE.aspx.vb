Imports System.IO
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Routing

Partial Class DM_NEW_CASE
    Inherits BasePage

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Dim creationInfo As CaseCreationInfo = Nothing

        'Put user code to initialize the page here
        If Not Page.IsPostBack Then
            txtCaseName.MaxLength = Settings.GetValue("Storage", "MaxNamesLength", 500)

            Dim creationInfoQueryString As String = QueryStringParser.GetString("creationinfo")
            If String.IsNullOrEmpty(creationInfoQueryString) Then
                txtObjectID.Text = QueryStringParser.GetInt("DM_OBJECT_ID")
                txtPackID.Text = QueryStringParser.GetInt("PACK_ID")
                txtParentID.Text = QueryStringParser.GetInt("DM_PARENT_ID")
                txtCaseID.Text = QueryStringParser.GetInt("CASE_ID")

                txtProcID.Text = QueryStringParser.GetInt("PROC_ID")
                If txtProcID.Text = "0" Then
                    Server.Transfer(GetRedirectUrl("DM_SELECT_OBJ_CAT.aspx?DM_PARENT_ID=" & txtParentID.Text & "&CAT_TYPE=Procedure&INPUTSELECTION=" & QueryStringParser.GetString("INPUTSELECTION")))
                    Return
                End If
            Else
                creationInfo = GetCaseCreationInfoFromJson(creationInfoQueryString)
            End If


            txtCaseName.Focus()

        Else
            If Request.ContentType = "application/json" Then
                creationInfo = GetCaseCreationInfoFromJson(GetRequestBody())

            End If
        End If
        Dim loProc As Procedure

        If creationInfo IsNot Nothing Then
            loProc = creationInfo.GetProcedure()
            Table1.Visible = False
            If loProc Is Nothing Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDPROCEDURE)
            ElseIf loProc.DisableManualCreation Then
                GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOPERATIONONOBJECTTYPE)
            Else
                DoSave(creationInfo, True)
            End If
            Return
        End If

        loProc = GetProcedure()
        If loProc Is Nothing Then
            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDPROCEDURE)
            Return
        ElseIf loProc.DisableManualCreation Then

            GotoErrorPage(LibError.ErrorCode.ERR_INVALIDOPERATIONONOBJECTTYPE)
            Return
        End If


        Dim loFolder As DM_OBJECT = GetFolder()
        If loFolder Is Nothing Then
            Return
        End If

        If Not loProc.CanStart(loFolder) Then
            GotoErrorPage(loProc.GetLastError.Code)
            Return
        End If

        SetLabels(loProc)

        If loProc.PROC_AUTONAME = AutoNamingMode.EmptyName OrElse loProc.PROC_AUTONAME = AutoNamingMode.AutoName Then 'for doc, don't show the name box
            Table1.Visible = False
            DoSave(loProc, True)
        Else
            If loProc.PROC_AUTONAME <> AutoNamingMode.RequiredName Then
                reqName.Enabled = False
            End If

        End If

    End Sub
    Protected Sub DoSave(ByVal sender As Object, ByVal e As EventArgs)
        Dim proc As Procedure = Procedure.GetProcedure(CInt(txtProcID.Text))
        DoSave(proc, False)
    End Sub
    Protected Sub DoSave(ByVal proc As Procedure, ByVal autoSave As Boolean)
        DoSave(GetCaseCreationInfo(proc), autoSave)
    End Sub
    Protected Sub DoSave(creationInfo As CaseCreationInfo, ByVal autoSave As Boolean)

        If String.IsNullOrEmpty(creationInfo.Name) AndAlso creationInfo.GetProcedure().NamingMode = AutoNamingMode.RequiredName Then
            Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", "ShowMessage('" & lblName.Text & " " & GetLabel("req") & "','error',true);", True)
            Return
        End If


        Dim creationFactory As ICaseFactory = New CaseFactory()

        Dim loCase As cCase = creationFactory.CreateCase(creationInfo, Nothing)

        If loCase.Tech_ID <> 0 AndAlso Not loCase.ActionCancelled Then

            Dim redirectUrl As String = GetRedirectUrlForCase(loCase)
            If Not String.IsNullOrEmpty(redirectUrl) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", "document.location.href='" & redirectUrl & "';", True)
            Else
                Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", "Close();", True)
            End If

        Else
            If Not autoSave Then
                Page.ClientScript.RegisterStartupScript(Me.GetType, "NewObject", "ShowMessage('" & loCase.GetLastError().Description & "','error',true);", True)
            Else
                GotoErrorPage(loCase.GetLastError().Code)
            End If
        End If


    End Sub

    Private Function GetRedirectUrlForCase(ByVal newCase As cCase) As String
        Dim lbRedirect As Boolean = False
        If newCase.CurrentUserHasWork Then
            lbRedirect = True
        ElseIf newCase.CurrentStep.Step_Type_Code = "SUBPROC" Then
            If Settings.GetValue("Interface", "AutoFlow", True) Then
                For Each subCase As cCase In newCase.GetChildCases()
                    If subCase.CurrentUserHasWork Then
                        lbRedirect = True
                        Exit For
                    End If
                Next
            End If
        End If

        If lbRedirect Then
            Return GetRedirectUrl("dm_detail.aspx?RTCASE_TECH_ID=" & newCase.Tech_ID & "&folderid=" & txtParentID.Text & "&catid=" & newCase.TargetObject.Category & "&mode=2&isnew=Y&refreshtree=" & If(newCase.TargetObject.ShowInTree, "Y", ""))
        End If

        Return Nothing
    End Function

    Private Sub SetLabels(ByVal proc As Procedure)
        txtTitle.Text = GetLabel("insert") & " : " & Server.HtmlEncode(proc.Name)
        lblName.Text = GetLabel("name")
        lnkSave.Text = GetLabel("insert")
        lnkCancel.Text = GetLabel("cancel")
        lblDesc.Text = GetLabel("description")
    End Sub

    Private Function GetFolder() As Folder

        Dim llParentID As Integer = Convert.ToInt32(txtParentID.Text)

        If llParentID > 0 Then
            Dim loFolder As DM_OBJECT = ObjectRepository.GetObject(llParentID)

            While loFolder.Object_Type = "Shortcut" 'load the object the shortcut points too
                If loFolder.Object_Reference > 0 Then
                    loFolder = CType(loFolder, Shortcut).GetReferencedObject
                    txtParentID.Text = loFolder.ID
                Else
                    'shortcut not pointing anywhere
                    Response.Write("Illegal shortcut")
                    Response.End()
                    Return Nothing
                End If
            End While

            Return loFolder
        Else
            Return Folder.GetRoot
        End If
    End Function
    Private Function GetProcedure() As Procedure
        Dim loProc As Procedure = Nothing

        'check if we need to convert from name
        If Not Page.IsPostBack Then
            Dim liProcID As Int32
            If Not Int32.TryParse(txtProcID.Text, liProcID) Then
                loProc = Procedure.GetProcedure(txtProcID.Text)
                txtProcID.Text = loProc.ID.ToString
            End If
        End If


        If loProc Is Nothing Then
            loProc = Procedure.GetProcedure(CInt(txtProcID.Text))
        End If

        Return loProc


    End Function

    Private Function GetCaseCreationInfo(ByVal proc As Procedure) As CaseCreationInfo
        Dim creationInfo As New CaseCreationInfo
        creationInfo.SetProcedure(proc)
        If proc.DM_ACTION = Procedure.ProcedureObjectAction.UpdateObject AndAlso FilledIn(txtObjectID) Then
            creationInfo.TargetObject = ObjectRepository.GetObject(Convert.ToInt32(txtObjectID.Text))
        End If
        creationInfo.Name = txtCaseName.Text
        creationInfo.Description = txtCaseDesc.Text
        creationInfo.FolderId = CLng(txtParentID.Text)
        If FilledIn(txtPackID) Then
            If FilledIn(txtCaseID) Then
                creationInfo.AddToCases = New List(Of CreationInfo.TargetCaseInfo) From {New CreationInfo.TargetCaseInfo() With {.CaseId = Convert.ToInt32(txtCaseID.Text), .PackageId = Convert.ToInt32(txtPackID.Text)}}
            End If
            If FilledIn(txtObjectID) Then
                creationInfo.AddToObjects = New List(Of CreationInfo.TargetObjectInfo) From {New CreationInfo.TargetObjectInfo() With {.ObjectId = Convert.ToInt32(txtObjectID.Text), .PackageId = Convert.ToInt32(txtPackID.Text)}}
            End If
        End If
        Dim inputSelection As String = QueryStringParser.GetString("INPUTSELECTION")
        If Not String.IsNullOrEmpty(inputSelection) Then
            creationInfo.SelectionToAddToInputPackage = CType(inputSelection, DMSelection.SelectionType)
        End If
        Return creationInfo
    End Function
    Private Function GetCaseCreationInfoFromJson(ByVal json As String) As CaseCreationInfo
        If String.IsNullOrEmpty(json) Then Return Nothing

        Arco.Utils.Logging.Debug("CaseCreationInfo json: " + json)

        Return Newtonsoft.Json.JsonConvert.DeserializeObject(Of CaseCreationInfo)(json)

    End Function
    Private Function FilledIn(ByVal txtField As Label) As Boolean
        Return Not String.IsNullOrEmpty(txtField.Text) AndAlso txtField.Text <> "0"
    End Function

    Private Function GetRequestBody() As String
        Using ms As New MemoryStream()
            Request.InputStream.CopyTo(ms)
            Request.InputStream.Position = 0
            Using reader As New StreamReader(ms)
                Return reader.ReadToEnd()
            End Using
        End Using

    End Function
End Class
