
Partial Class DM_FILEACTIONS
    Inherits BasePage

#Region " Private members"

    Protected _DM_File_ID As Integer
    Protected _Action As FILEACTIONS

    Protected Enum FILEACTIONS
        CHECKIN = 0
        EDITPROPS = 1
        EDITURL = 2
        'ENDECRYPTFILE = 3

        'Ajax Simulation
        CHECKOUT = 3
        CANCELCHECKOUT = 4
        COLLECTFILE = 5
        DELETE = 6
    End Enum

#End Region


#Region " Page events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ParseQueryString()

        MultiView1.ActiveViewIndex = CType(_Action, Integer)

    End Sub

#End Region

#Region " Checkin/check out events"

    Protected Sub viewDeleteFile_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles viewDeleteFile.Load

        If MultiView1.ActiveViewIndex = FILEACTIONS.DELETE Then
            lblDeleteFile.Text = GetLabel("ctx_delete")
            Response.Flush()

            Dim oFile As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(_DM_File_ID)
            If oFile.CanDelete Then
                oFile.Delete()
            End If          
            If Not oFile.HasError Then
                AddCloseParentScript("DeleteFile")
            Else
                AddMessageScript(oFile.GetLastError.Description)
            End If
        End If
    End Sub

    Protected Sub viewCheckout_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles viewCheckout.Load

        If MultiView1.ActiveViewIndex = FILEACTIONS.CHECKOUT Then
            lblCheckoutfile.Text = GetLabel("ctx_checkoutfile")
            Response.Flush()

            Dim oFile As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(_DM_File_ID)

            oFile = oFile.CheckOut()

            If Not oFile.HasError Then
                AddReloadScript("CheckoutFile", oFile.ID)
            Else
                AddMessageScript(oFile.GetLastError.Description)
            End If
        End If
    End Sub
    Protected Sub viewCancelCheckout_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles viewCancelCheckout.Load

        If MultiView1.ActiveViewIndex = FILEACTIONS.CANCELCHECKOUT Then
            lblCancelCheckoutfile.Text = GetLabel("ctx_cancelcheckoutfile")
            Response.Flush()

            Dim oFile As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(_DM_File_ID)
            If oFile.CanCheckIn Then
                oFile = oFile.CancelCheckout()
                AddReloadScript("CancelCheckoutFile", oFile.ID)
            Else
                AddMessageScript(oFile.GetLastError.Description)
            End If

        End If
    End Sub
    Protected Sub viewCheckin_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles viewCheckin.Load

        If MultiView1.ActiveViewIndex = FILEACTIONS.CHECKIN Then

            If Not Page.IsPostBack Then
                plhHeaderCheckIn.Visible = Not CType(Me.Master, ToolWindow).Modal
                plhInlineHeaderCheckIn.Visible = True 'Not plhHeaderCheckIn.Visible
                cmdSave.Text = GetLabel("save")
                cmdCancel.Text = GetLabel("close")
                lblCheckInHeader.Text = GetLabel("checkin")
                Page.Title = GetLabel("checkin")
                rdlAction.Items.Clear()
                rdlAction.Items.Add(New ListItem(GetLabel("cancelcheckout"), "0", True))
                rdlAction.Items.Add(New ListItem(GetLabel("savenewversion"), "1", True))
                rdlAction.Items.Add(New ListItem(GetLabel("savenewsubversion"), "2", True) With {.Selected = True})
                rdlAction.Items.Add(New ListItem(GetLabel("overwriteversion"), "3", True))
                rdlAction.Items.Add(New ListItem(GetLabel("keepcheckedout"), "4", True))
            End If

            Dim oFile As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(_DM_File_ID)
            If Not oFile.CanCheckIn Then
                AddMessageScript(oFile.GetLastError.Description)
                Exit Sub
            End If

            Dim lsFileTitle As String
            If Not oFile.Name.Contains("." & oFile.FILE_EXT) Then
                lsFileTitle = oFile.Name & "." & oFile.FILE_EXT
            Else
                lsFileTitle = oFile.Name
            End If
            lblCheckInFileName.Text = lsFileTitle
            lblCheckinoptions.Text = GetLabel("checkin") & ":"
            If Not oFile.IsRemoteUrl Then

                If Not String.IsNullOrEmpty(oFile.FILE_EXT) Then
                    Dim loFT As Arco.Doma.Library.baseObjects.DM_FileTypeList.FileTypeInfo = oFile.LinkedToObject.GetFileType(oFile.FILE_EXT)

                    If loFT.FILE_WEBDAVENABLED Then
                        Dim loLink As New HyperLink
                        loLink.NavigateUrl = "javascript:MainPage().top.PC().EditFile(" & oFile.ID & ");"
                        loLink.Text = "<img src='" & "./Images/FileTypes/" & Icons.GetFileIcon(oFile.FILE_EXT) & "' border='0' title='" & GetLabel("editfile") & " " & GetLabel("with") & " " & Arco.IO.MimeTypes.GetWebDavApplicationName(loFT.Extension) & "'/>&nbsp;"

                        plhEdit.Controls.Add(loLink)

                    End If
                    If loFT.FILE_INLINEEDITING Then
                        Dim loLink As New HyperLink
                        loLink.NavigateUrl = "javascript:MainPage().top.PC().EditFileDirect(" & oFile.ID & ");"

                        loLink.Text = "<img src='" & ThemedImage.GetClientUrl("edit.png", Page) & "' border='0' title='" & GetLabel("editfile") & "'/>"
                        plhEdit.Controls.Add(loLink)

                    End If

                    lblEditOptions.Text = GetLabel("editfile") & ":"
                End If
            End If

        End If

    End Sub

    Protected Sub cmdSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdSave.Click

        Dim oFile As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(_DM_File_ID)
        Dim lsComment As String = ""
        Dim lbOk As Boolean = False

        Select Case Integer.Parse(rdlAction.SelectedValue)
            Case 0
                If oFile.CanCheckIn Then
                    lbOk = True
                    oFile = oFile.CancelCheckout()

                End If
            Case 1
                If oFile.CanCheckIn Then
                    lbOk = True
                    oFile = oFile.CheckIn(True, False, lsComment)
                End If
            Case 2
                If oFile.CanCheckIn Then
                    lbOk = True
                    oFile = oFile.CheckIn(False, True, lsComment)
                End If
            Case 3
                If oFile.CanCheckIn Then
                    lbOk = True
                    oFile = oFile.CheckIn(False, False, lsComment)
                End If
            Case 4
                'do nothing
                lbOk = True
        End Select

        If lbOk Then
            AddReloadScript("CheckInFile", oFile.ID)
        Else
            AddMessageScript(oFile.GetLastError.Description)
        End If

    End Sub


#End Region

#Region " Rename file events"

    Protected Sub viewEditProperties_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles viewEditProperties.Load

        If Not Page.IsPostBack Then
            If MultiView1.ActiveViewIndex = FILEACTIONS.EDITPROPS Then
                plhHeaderEditProps.Visible = Not CType(Me.Master, ToolWindow).Modal
                Page.Title = GetLabel("editfileproperties") 'GetLabel("rename")
                ' Label1.Text = ""
                lblPack.Text = GetLabel("package") & ":"
                Label2.Text = GetLabel("name") & ":"
                cmdEditProps.Text = GetLabel("save")
                HyperLink1.Text = GetLabel("cancel")
                lblEditPropsHeader.Text = GetLabel("editfileproperties")
                Label5.Text = GetLabel("language") & " : "
                valNewName.ErrorMessage += GetLabel("name") & " " & GetLabel("req")
                Dim loFile As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(_DM_File_ID)
                If loFile.CanModify() Then                    
                    txtNewName.Text = loFile.Name

                    cmbLangs.Items.Clear()
                    cmbLangs.Items.Add(New ListItem(""))

                    For Each loLang As Arco.Doma.Library.Globalisation.LanguageList.LanguageInfo In Arco.Doma.Library.Globalisation.LanguageList.GetLanguageList
                        cmbLangs.Items.Add(New ListItem(loLang.Description, loLang.Code) With {.Selected = (loFile.FILE_LANGCODE = loLang.Code)})
                    Next

                    If loFile.CanDelete Then 'todo : other check?
                        cmbPacks.Items.Clear()
                        cmbPacks.Items.Add(New ListItem(GetLabel("files"), 0) With {.Selected = (loFile.PackageID = 0)})
                        For Each p As Arco.Doma.Library.baseObjects.PackageList.PackageInfo In loFile.LinkedToObject.Packages
                            If p.IsFilePackage AndAlso loFile.LinkedToObject.CanAddToPackage(p) Then
                                cmbPacks.Items.Add(New ListItem(p.Name, p.ID) With {.Selected = (loFile.PackageID = p.ID)})
                            End If
                        Next
                    End If
                    plhPacks.Visible = (cmbPacks.Items.Count > 1)
                Else
                    AddMessageScript(loFile.GetLastError.Description)
                End If

            End If
        End If
    End Sub

    Protected Sub cmdEditProps_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEditProps.Click

        If Page.IsValid Then
            Dim loFile As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(_DM_File_ID)
            loFile.Name = txtNewName.Text
            loFile.FILE_LANGCODE = cmbLangs.SelectedValue
            loFile.UpdateModifDate = True
            If plhPacks.Visible Then
                loFile.PackageID = CType(cmbPacks.SelectedValue, Int32)
            End If
            loFile.Save()

            viewEditProperties.Controls.Clear()
            AddReloadScript("EditProps", 0)
        End If
    End Sub

#End Region

#Region " Private methods"

    Private Sub ParseQueryString()


        _DM_File_ID = QueryStringParser.GetInt("DM_FILE_ID")

        _Action = CType(QueryStringParser.GetInt("action"), FILEACTIONS)

    End Sub
    Private Sub AddMessageScript(ByVal text As String)

        Dim sb As New StringBuilder

        sb.AppendLine("PC().ShowError(" & EncodingUtils.EncodeJsString(text) & ");")
        sb.AppendLine("Close();")
        Page.ClientScript.RegisterStartupScript(GetType(String), "msg", sb.ToString, True)
    End Sub
    Private Sub AddCloseScript(ByVal scriptName As String)
        Dim sb As New StringBuilder

        sb.AppendLine("Close();")

        Page.ClientScript.RegisterStartupScript(GetType(String), scriptName, sb.ToString, True)
    End Sub
    Private Sub AddReloadScript(ByVal ScriptName As String, ByVal vlNewFileID As Int32)
        Dim sb As New StringBuilder

        If Not CType(Me.Master, ToolWindow).Modal Then
            sb.AppendLine("try {")
            sb.AppendLine("MainPage().ReloadParent(" & vlNewFileID & ");")
            sb.AppendLine("} catch (e){")
            sb.AppendLine("MainPage().UncheckedSave();}")
        End If
        sb.AppendLine("Close();")

        Page.ClientScript.RegisterStartupScript(GetType(String), ScriptName, sb.ToString, True)
    End Sub
    Private Sub AddCloseParentScript(ByVal ScriptName As String)
        Dim sb As New StringBuilder

        If Not CType(Me.Master, ToolWindow).Modal Then        
            sb.AppendLine("MainPage().Close();")            
        End If
        sb.AppendLine("Close();")

        Page.ClientScript.RegisterStartupScript(GetType(String), ScriptName, sb.ToString, True)
    End Sub
#End Region


    Protected Sub viewEditUrl_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles viewEditUrl.Load
        If Not Page.IsPostBack Then
            If MultiView1.ActiveViewIndex = FILEACTIONS.EDITURL Then
                plhHeaderUrl.Visible = Not CType(Me.Master, ToolWindow).Modal
                HyperLink2.Text = GetLabel("cancel")
                cmdEditUrl.Text = GetLabel("save")
                lblEditUrlHeader.Text = GetLabel("editurl")
                Page.Title = GetLabel("editurl")
                Dim loFile As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(_DM_File_ID)

                txtUrl.Text = loFile.FILE_PATH
            End If
        End If
    End Sub

    Protected Sub cmdEditUrl_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmdEditUrl.Click
        If Page.IsValid Then
            Dim loFile As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(_DM_File_ID)
            loFile = loFile.ReplaceUrl(txtUrl.Text, True)
            If Not loFile.HasError Then
                viewEditUrl.Controls.Clear()
                AddReloadScript("EditUrl", 0)
            Else
                AddMessageScript(loFile.GetLastError.Description)
            End If

        End If
    End Sub

    Protected Sub viewCollectFile_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles viewCollectFile.Load
        If MultiView1.ActiveViewIndex = FILEACTIONS.COLLECTFILE Then
            lblCollectFile.Text = GetLabel("collectfile")
            Dim loProc As New Arco.Doma.Library.BatchJobs.CollectSingleFileForEditing(_DM_File_ID, False)
            loProc = loProc.Collect()
            If Not String.IsNullOrEmpty(loProc.ErrorMessage) Then
                AddMessageScript(loProc.ErrorMessage)
            End If
            AddReloadScript("reload", _DM_File_ID)
        End If
    End Sub
End Class
