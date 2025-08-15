
Imports System.IO


Partial Class DM_EDIT_FILE_DIRECT
    Inherits BasePage


    Private Enum eMode
        TextMode = 0
        HtmlFileMode = 1
        HtmlFieldMode = 2
        NotSupported = 3
    End Enum
    Private Mode As eMode

    Protected ReadOnly Property FileID As String
        Get
            Return txtFileID.Value
        End Get
    End Property
    Protected ReadOnly Property OpenFileAfterSave As String
        Get
            If txtFileType.Value = "3" Then 'comments
                Return ""
            Else
                Return Me.FileID
            End If
        End Get
    End Property


    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim llID As Int32

        CType(Master, ToolWindow).Width = 900

        SetForceMode(False, Nothing)

        If Not Page.IsPostBack Then
            cmdSave.Text = GetLabel("save")
            cmdSaveForce.Text = GetLabel("save")
            cmdCancel.Text = GetLabel("cancel")
            cmdSaveAndClose.Text = GetLabel("savecomplete")
            cmdSaveCloseForce.Text = GetLabel("savecomplete")

            txtFileID.Value = QueryStringParser.GetInt("FILE_ID")
            txtFileType.Value = QueryStringParser.GetInt("DM_FILE_TYPE", 1)
        End If


        chkPrivate.Visible = False
        chkPrivate.Text = GetDecodedLabel("private")


        llID = Convert.ToInt32(txtFileID.Value)

        If llID > 0 Then
            loFile = Arco.Doma.Library.File.GetFile(llID)
            If loFile Is Nothing OrElse loFile.Status = Arco.Doma.Library.File.File_Status.Deleted Then
                GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INVALIDOBJECT)
            End If
            If Not loFile.CanModify Then
                GotoErrorPage(loFile.GetLastError.Code)
            End If
            txtObjectID.Value = loFile.OBJECT_ID.ToString
            chkPrivate.Visible = (loFile.isComment AndAlso loFile.FILE_CREATIONUSER = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.Name)

            pnlEdit.Visible = True


            If loFile.Type = Arco.Doma.Library.File.File_Type.Comment Then
                If Settings.GetValue("Interface", "EnableHtmlComments", False) Then
                    Mode = eMode.HtmlFieldMode
                Else
                    Mode = eMode.TextMode
                End If

            Else
                If loFile.FILE_EXT = "htm" Then
                    Mode = eMode.HtmlFileMode
                ElseIf loFile.FILE_EXT = "txt" Then
                    Mode = eMode.TextMode
                Else
                    Mode = eMode.NotSupported
                End If
            End If

            Select Case Mode
                Case eMode.NotSupported
                    pnlEdit.Visible = False


                Case Else 'htm or txt
                    pnlEdit.Visible = True

                    ' loFile.Lock(Arco.Security.BusinessIdentity.CurrentIdentity.Name)
                    ShowCorrectInputBox()



                    If Not Page.IsPostBack Then

                        chkPrivate.Checked = (loFile.Scope = Arco.Doma.Library.Scope.Private)

                        Select Case Mode
                            Case eMode.TextMode
                                txtFileContent.Text = loFile.GetString
                            Case eMode.HtmlFileMode
                                'always sanitize before showing in the editor                                
                                txtHTMLContent.Content = HtmlSanitizer.Sanitize(loFile.GetString)
                            Case eMode.HtmlFieldMode
                                'always sanitize before showing in the editor           
                                txtHTMLField.Content = HtmlSanitizer.Sanitize(loFile.GetString)
                        End Select
                        txtHash.Value = loFile.Hash
                    End If
            End Select


            If txtFileType.Value = "3" Then 'comments
                Page.Title = GetLabel("edit")
            Else
                Page.Title = GetLabel("editfile")
            End If


        Else
            pnlEdit.Visible = True


            If txtFileType.Value = "3" Then 'comments
                chkPrivate.Visible = True
                Page.Title = GetLabel("addcomment")

                If Settings.GetValue("Interface", "EnableHtmlComments", False) Then
                    Mode = eMode.HtmlFieldMode
                Else
                    Mode = eMode.TextMode
                End If

            Else
                Mode = eMode.HtmlFileMode
                Page.Title = GetLabel("editfile") 'change to add, when possible
            End If

            If Not Page.IsPostBack Then
                txtObjectID.Value = QueryStringParser.GetInt("DM_OBJECT_ID")
            End If

            ShowCorrectInputBox()
        End If


        Select Case Mode
            Case eMode.HtmlFileMode
                If Not Page.IsPostBack Then
                    txtHTMLContent.OnClientLoad = "OnClientLoad"
                End If
                ClientScript.RegisterStartupScript(Me.GetType(), "focusScript", "function OnClientLoad(editor) { setTimeout(function(){editor.setFocus();}, 100); }", True)
            Case eMode.HtmlFieldMode
                If Not Page.IsPostBack Then
                    txtHTMLField.OnClientLoad = "OnClientLoad"
                End If
                ClientScript.RegisterStartupScript(Me.GetType(), "focusScript", "function OnClientLoad(editor) { setTimeout(function(){editor.setFocus();}, 100); }", True)
            Case eMode.TextMode
                txtFileContent.Focus()

            Case eMode.NotSupported
                ClientScript.RegisterStartupScript(Me.GetType(), "notsupported", "alert('Editing files directly is not supported for this file type');Close();", True)
        End Select

    End Sub

    Private Sub ShowCorrectInputBox()
        Select Case Mode
            Case eMode.TextMode
                txtFileContent.Visible = True
                txtHTMLContent.Visible = False
                txtHTMLField.Visible = False
            Case eMode.HtmlFileMode
                txtHTMLField.Visible = False
                txtHTMLContent.Visible = True
                txtFileContent.Visible = False
            Case eMode.HtmlFieldMode
                txtHTMLField.Visible = True
                txtHTMLContent.Visible = False
                txtFileContent.Visible = False
        End Select
    End Sub

    Private Sub SetForceMode(ByVal vbOn As Boolean, ByVal voFile As Arco.Doma.Library.File)
        cmdSave.Visible = Not vbOn
        cmdSaveAndClose.Visible = Not vbOn
        cmdSaveForce.Visible = vbOn
        cmdSaveCloseForce.Visible = vbOn
        rowEditConflict.Visible = vbOn
        If vbOn Then
            Dim lsMsg As String = GetLabel("fileconflict")
            lsMsg = lsMsg.Replace("#USERNAME#", ArcoFormatting.FormatUserName(voFile.FILE_MODIFUSER))
            lsMsg = lsMsg.Replace("#MODIFDATE#", ArcoFormatting.FormatDateLabel(voFile.FILE_MODIFDATE, True, False, False))
            lblConflict.Text = lsMsg
        End If
    End Sub

    Private lsCachedFile As String
    Dim loFile As Arco.Doma.Library.File
    Private Sub Save(ByVal closeAfterSave As Boolean, ByVal vbForceOverride As Boolean)
        Dim llID As Int32

        Dim lsFileName As String
        Dim lbOk As Boolean = True


        llID = Convert.ToInt32(txtFileID.Value)

        Select Case Mode

            Case eMode.HtmlFileMode
                txtHTMLContent.Content = HtmlSanitizer.Sanitize(txtHTMLContent.Content)
            Case eMode.HtmlFieldMode
                txtHTMLField.Content = HtmlSanitizer.Sanitize(txtHTMLField.Content)
        End Select

        If llID > 0 Then 'update

            loFile.Scope = Scope
            loFile = loFile.Save

            If vbForceOverride OrElse String.IsNullOrEmpty(txtHash.Value) OrElse txtHash.Value.Equals(loFile.Hash) Then
                lsFileName = loFile.ID & "." & loFile.FILE_EXT
                lsCachedFile = Path.Combine(Settings.GetUploadPath, lsFileName)

                Select Case Mode
                    Case eMode.TextMode
                        Arco.IO.File.WriteFile(txtFileContent.Text, lsCachedFile)
                    Case eMode.HtmlFileMode
                        Arco.IO.File.WriteFile(txtHTMLContent.Content, lsCachedFile)
                    Case eMode.HtmlFieldMode
                        Arco.IO.File.WriteFile(txtHTMLField.Content, lsCachedFile)
                End Select

                If loFile.FileHasChanged(lsCachedFile) Then
                    txtHash.Value = Arco.Utils.MD5.GetHashCodeAsString(lsCachedFile)
                    If loFile.Type <> Arco.Doma.Library.File.File_Type.Comment Then
                        If Arco.Doma.Library.File.TrySetLockingStatus(loFile, Arco.Doma.Library.File.LockingStatus.DirectLock) Then
                            loFile = loFile.ReplaceFileWithDefaultEditBeheaviour(lsCachedFile, True, True)
                            If loFile.HasError Then
                                lbOk = False
                                Response.Write(loFile.GetLastError.Description)
                            End If
                            loFile = loFile.SetLockingStatus(Arco.Doma.Library.File.LockingStatus.NoLock)

                        Else
                            lbOk = False
                            Response.Write(loFile.GetLastError.Description)
                        End If

                    Else
                        loFile = loFile.ReplaceFile(lsCachedFile, True, True)
                    End If
                    txtFileID.Value = loFile.ID.ToString
                Else
                    Arco.IO.File.DeleteFile(lsCachedFile)
                End If

            Else
                'conflict
                SetForceMode(True, loFile)
                lbOk = False
            End If

        Else

            Dim loDocument As Arco.Doma.Library.baseObjects.DM_OBJECT = Arco.Doma.Library.ObjectRepository.GetObject(Convert.ToInt32(txtObjectID.Value))
            If loDocument Is Nothing Then
                GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INVALIDOBJECT)
            End If

            Dim lsExt As String = ""

            Select Case Mode
                Case eMode.HtmlFileMode, eMode.HtmlFieldMode
                    lsExt = "htm"
                Case eMode.TextMode
                    lsExt = "txt"
            End Select

            lsFileName = "new" & txtObjectID.Value & "." & lsExt

            lsCachedFile = Path.Combine(Settings.GetUploadPath, lsFileName)

            Select Case Mode
                Case eMode.TextMode
                    Arco.IO.File.WriteFile(txtFileContent.Text, lsCachedFile)
                Case eMode.HtmlFileMode
                    Arco.IO.File.WriteFile(txtHTMLContent.Content, lsCachedFile)
                Case eMode.HtmlFieldMode
                    Arco.IO.File.WriteFile(txtHTMLField.Content, lsCachedFile)
            End Select
            txtHash.Value = Arco.Utils.MD5.GetHashCodeAsString(lsCachedFile)


            Dim loType As Arco.Doma.Library.File.File_Type = CType(txtFileType.Value, Arco.Doma.Library.File.File_Type)
            If loType = Arco.Doma.Library.File.File_Type.Comment Then

                loFile = loDocument.AddComment("", lsCachedFile, Scope)
            Else
                loFile = loDocument.AddFile("", lsCachedFile, True, "", True, 0, "", True)
            End If
            If Not loFile Is Nothing Then
                txtFileID.Value = loFile.ID.ToString
                If Not loFile.CanModify Then
                    closeAfterSave = True
                End If
            Else
                Response.Write(loDocument.GetLastError.Description)
                closeAfterSave = False
            End If
        End If

        If closeAfterSave AndAlso lbOk Then
            Cancel()
        End If
    End Sub

    Protected Property Scope As Arco.Doma.Library.Scope
        Get
            Return If(chkPrivate.Checked, Arco.Doma.Library.Scope.Private, Arco.Doma.Library.Scope.Public)
        End Get
        Set(value As Arco.Doma.Library.Scope)
            chkPrivate.Checked = (value = Arco.Doma.Library.Scope.Private)
        End Set
    End Property
    Protected Sub cmdSave_Command(ByVal sender As Object, ByVal e As CommandEventArgs) Handles cmdSave.Command
        Save(False, False)
    End Sub
    Protected Sub cmdSaveForce_Command(ByVal sender As Object, ByVal e As CommandEventArgs) Handles cmdSaveForce.Command
        Save(False, True)
    End Sub
    Private Sub Cancel()
        Dim llID As Int32 = Convert.ToInt32(txtFileID.Value)
        If llID > 0 Then 'update
            'Dim loFile As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(llID)
            'loFile.Unlock()
        End If

        Dim sb As New StringBuilder
        sb.AppendLine("RefreshOpener();")
        sb.AppendLine("Close();")


        Page.ClientScript.RegisterStartupScript(GetType(String), "closescript", sb.ToString, True)

    End Sub
    Protected Sub cmdCancel_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdCancel.Click
        Cancel()
    End Sub

    Protected Sub cmdSaveAndClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSaveAndClose.Click
        Save(True, False)
    End Sub
    Protected Sub cmdSaveCloseForce_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdSaveCloseForce.Click
        Save(True, True)
    End Sub
    Protected Sub txtHTMLContent_ExportContent(sender As Object, e As Telerik.Web.UI.EditorExportingArgs) Handles txtHTMLContent.ExportContent

        Dim lsRTF As String = e.ExportOutput
        Arco.IO.File.WriteFile(lsRTF, lsCachedFile)

        loFile.UpdateModifDate = True
        loFile.FILE_CHECKVERSION.NewRevision()
        loFile.ReplaceFile(lsCachedFile, True, True)

        Response.Redirect("DM_EDIT_FILE_DIRECT.aspx?FILE_ID=" & loFile.ID)
    End Sub

End Class
