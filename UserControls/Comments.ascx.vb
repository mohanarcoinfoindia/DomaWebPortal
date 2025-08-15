
Partial Class UserControls_Comments
    Inherits BaseUserControl
    Public Enum eDisplayType
        Grid = 0
        List = 1
    End Enum
    Private mDisplayType As eDisplayType = eDisplayType.Grid
    Private _enableHtmlComments As Boolean = Settings.GetValue("Interface", "EnableHtmlComments", False)

    Public Property Height As Integer
    Public Property Count As Integer
    Public Property ShowQuickAdd() As Boolean = True
    Public Property CanAdminDelete() As Boolean = True
    Public Property CanView() As Boolean = True
    Public Property CanDelete() As Boolean = True
    Public Property CanAdd() As Boolean = True
    Public Property CanEdit() As Boolean = True


    Protected ReadOnly Property ExtraStyle As String
        Get
            If Me.Height > 0 Then
                Return "width:100%;height:" & Me.Height & "px;overflow:auto;zoom:1;clear:both"
            Else
                Return "width:100%;clear:both"
            End If
        End Get
    End Property

    <Obsolete()> _
    Public Property DisplayType() As eDisplayType
        Get
            Return mDisplayType
        End Get
        Set(ByVal value As eDisplayType)
            mDisplayType = value
        End Set
    End Property
    Public Property Object_ID() As Int32
        Get
            Return Convert.ToInt32(ViewState("OBJ_ID"))
        End Get
        Set(ByVal value As Int32)
            ViewState("OBJ_ID") = value
        End Set
    End Property
    Public Property FromArchive() As Boolean

    Protected Function CommentInfo(ByVal voFileInfo As Arco.Doma.Library.FileList.FileInfo) As String
        Dim sb As New StringBuilder

        If voFileInfo.Scope = Arco.Doma.Library.Scope.Private Then
            sb.Append("<span class='icon icon-user-profile icon-color-light' style='font-size:15px' title='" & GetLabel("private") & "'></span>")
        End If
        sb.Append(ArcoFormatting.FormatUserName(voFileInfo.FILE_CREATIONUSER))      
        sb.Append(" - ")
        sb.Append(ArcoFormatting.FormatDateLabel(voFileInfo.FILE_INDEXDATE, True, False, False))
        If voFileInfo.FILE_MODIFDATE.Subtract(voFileInfo.FILE_INDEXDATE).Minutes > 0 Then
            If voFileInfo.FILE_CREATIONUSER <> voFileInfo.FILE_MODIFUSER Then
                sb.Append("<br/>")
                sb.Append(GetLabel("modifby"))
                sb.Append(" ")
                sb.Append(ArcoFormatting.FormatUserName(voFileInfo.FILE_MODIFUSER))
            End If
            sb.Append("<br/>")
            sb.Append(GetLabel("modifdate"))
            sb.Append(" ")
            sb.Append(ArcoFormatting.FormatDateLabel(voFileInfo.FILE_MODIFDATE, True, False, False))
        End If

        Return sb.ToString
    End Function

    Protected Sub comments_ItemDataBound(ByVal Sender As Object, ByVal e As RepeaterItemEventArgs)
        If (e.Item.ItemType = ListItemType.Item) Or _
                    (e.Item.ItemType = ListItemType.AlternatingItem) Then


            Dim loF As Arco.Doma.Library.FileList.FileInfo = CType(e.Item.DataItem, Arco.Doma.Library.FileList.FileInfo)
            Dim content As String = ""
            Try
                Dim lbCanEdit As Boolean = CanEdit
                Dim lbCanDelete As Boolean = CanDelete
                Dim loFileServer As Arco.Doma.Library.FileServers.IFileServer = Arco.Doma.Library.FileServers.FileServerFactory.GetFileServer(loF.FILESERVER_ID)

                Dim enc As Encoding = Response.ContentEncoding
                content = loFileServer.GetFileString(loF, enc)
                If Not _enableHtmlComments OrElse loF.FILE_EXT = "txt" Then
                    content = Server.HtmlEncode(content)
                Else
                    content = HtmlSanitizer.Sanitize(content)
                End If

                'do this?
                '  Response.ContentEncoding = enc

                If loF.FILE_CREATIONUSER = Arco.Security.BusinessIdentity.CurrentIdentity.Name Then
                    'his own comments
                    If lbCanEdit Or lbCanDelete Then
                        CType(e.Item.FindControl("pnlEditGrid"), PlaceHolder).Visible = True
                        CType(e.Item.FindControl("plhEditButton"), PlaceHolder).Visible = lbCanEdit
                        CType(e.Item.FindControl("plhDeleteButton"), PlaceHolder).Visible = lbCanDelete
                    Else
                        CType(e.Item.FindControl("pnlEditGrid"), PlaceHolder).Visible = False
                    End If
                Else
                    'someone elses comments or mailnotes
                    If CanAdminDelete Then
                        CType(e.Item.FindControl("pnlEditGrid"), PlaceHolder).Visible = True
                        CType(e.Item.FindControl("plhEditButton"), PlaceHolder).Visible = False
                        CType(e.Item.FindControl("plhDeleteButton"), PlaceHolder).Visible = True
                    Else
                        CType(e.Item.FindControl("pnlEditGrid"), PlaceHolder).Visible = False
                    End If
                End If
            Catch ex As Exception
                'Do nothing...just prevent error from being displayed when comment file is not processed by the indexer yet
            End Try
            CType(e.Item.FindControl("lblContentGrid"), Label).Text = content
        End If


    End Sub

    Private Sub AddScript()
        Dim sb As New StringBuilder

        sb.AppendLine("function ViewComment(id)")
        sb.AppendLine("{")
        sb.AppendLine("var address = 'DM_VIEW_File.aspx?FILE_ID=' + id + '&DM_OBJECT_ID=" & Me.Object_ID & "';")
        sb.AppendLine("PC().OpenWindow(address,'Viewfile','height=600,width=900,scrollbars=yes,resizable=yes,status=yes');")
        sb.AppendLine("}")

        sb.AppendLine("function NewComment() {")
        sb.Append("        PC().NewComment(")
        sb.Append(Me.Object_ID)
        sb.AppendLine(");")
        sb.AppendLine("}")


        sb.AppendLine("function DeleteComment(id)")
        sb.AppendLine("{")
        sb.AppendLine("PC().DeleteComment(id);")
        sb.AppendLine("}")

        ScriptManager.RegisterStartupScript(Me.Page, Me.GetType, "initcomments", sb.ToString, True)
    End Sub


    Public Overloads Sub DataBind(ByVal voObject As Arco.Doma.Library.baseObjects.DM_OBJECT)

        Me.Object_ID = voObject.ID
        If TypeOf (voObject) Is Arco.Doma.Library.baseObjects.DM_VersionControlledObject Then
            FromArchive = CType(voObject, Arco.Doma.Library.baseObjects.DM_VersionControlledObject).FromArchive
        End If
        If FromArchive Then
            CommentListDataSource.SelectMethod = "GetCommentsFromArchive"
            CanEdit = False
            CanAdd = False
            CanDelete = False
            CanView = True
        Else
            CommentListDataSource.SelectMethod = "GetComments"
            SetAccess(voObject)
        End If

        BindForm()
    End Sub
    Private Sub BindForm()
        If CanView Then
            AddScript()
            CommentList.DataSourceID = "CommentListDataSource"
            CommentListDataSource.SelectParameters.Item("vlObjectID").DefaultValue = Object_ID.ToString
            CommentList.DataBind()
            plhNoResults.Visible = (CommentList.Items.Count = 0)
            Count = CommentList.Items.Count
        Else
            Me.Visible = False
        End If

        pnlQuickAdd.Visible = False

        If CanAdd Then
            If ShowQuickAdd Then
                lnkQuickAdd.Text = GetLabel("add")
                chkPrivate.Text = GetLabel("private")
                pnlQuickAdd.Visible = True
                txtQuickAddHtml.Visible = _enableHtmlComments
                txtQuickAddText.Visible = Not _enableHtmlComments

            Else
                plhFullAdd.Visible = False 'True

                litFullAdd.Text = GetLabel("add")

            End If
        End If

    End Sub
    Private Sub SetAccess(ByVal voObject As Arco.Doma.Library.baseObjects.DM_OBJECT)
        If voObject.IsInRecycleBin Then
            CanEdit = False
            CanAdd = False
            CanDelete = False
        Else
            If CanEdit Then
                CanEdit = voObject.CanModifyComments
            End If
            If CanAdd Then
                CanAdd = voObject.CanAddComment
            End If
            If CanDelete Then
                CanDelete = voObject.HasAccess(Arco.Doma.Library.ACL.ACL_Access.Access_Level.ACL_Delete_Comments) OrElse voObject.HasAccess(Arco.Doma.Library.ACL.ACL_Access.Access_Level.ACL_Admin_Comments)
                If CanAdminDelete Then
                    CanAdminDelete = voObject.HasAccess(Arco.Doma.Library.ACL.ACL_Access.Access_Level.ACL_Admin_Comments)
                End If
            Else
                CanAdminDelete = False
            End If

        End If
        CanView = voObject.HasAccess(Arco.Doma.Library.ACL.ACL_Access.Access_Level.ACL_View_Comments)

    End Sub
    Public Overrides Sub DataBind()
        If FromArchive Then
            CommentListDataSource.SelectMethod = "GetCommentsFromArchive"
            CanEdit = False
            CanAdd = False
            CanDelete = False
            CanView = True
        Else
            CommentListDataSource.SelectMethod = "GetComments"
            If Object_ID > 0 Then
                Dim o As Arco.Doma.Library.baseObjects.DM_OBJECT = Arco.Doma.Library.ObjectRepository.GetObject(Object_ID)
                SetAccess(o)

            Else
                CanView = False
            End If
        End If

        BindForm()

    End Sub

    Protected Sub lnkQuickAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkQuickAdd.Click
        Dim content As String
        Dim fileExt As String
        If _enableHtmlComments Then
            fileExt = ".htm"
            content = txtQuickAddHtml.Content
        Else
            fileExt = ".txt"
            content = txtQuickAddText.Text
        End If
        Dim loObj As Arco.Doma.Library.baseObjects.DM_OBJECT = Arco.Doma.Library.ObjectRepository.GetObject(Object_ID)
        If loObj.CanAddComment AndAlso Not String.IsNullOrEmpty(content) Then
            Dim lsTempFileName = System.IO.Path.Combine(Settings.GetUploadPath, "new" & Object_ID & fileExt)
            Arco.IO.File.WriteFile(content, lsTempFileName)
            Dim scope As Arco.Doma.Library.Scope = If(chkPrivate.Checked, Arco.Doma.Library.Scope.Private, Arco.Doma.Library.Scope.Public)

            loObj.AddComment("", lsTempFileName, scope)
            If Settings.GetValue("Interface", "EnableHtmlComments", False) Then
                txtQuickAddHtml.Content = ""
            Else
                txtQuickAddText.Text = ""
            End If
            CommentList.DataBind()
            End If
    End Sub
End Class
