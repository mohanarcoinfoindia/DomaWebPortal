Imports Arco.Doma.Library
Imports Arco.Doma.Library.ACL
Imports Arco.Doma.Library.Globalisation
Imports Arco.Doma.Library.Mail

Partial Class UserControls_Messages
    Inherits BaseUserControl


    Private _packLabels As LABELList
    Public ReadOnly Property PackageLabels() As LABELList
        Get
            If _packLabels Is Nothing Then
                _packLabels = LABELList.GetPackagesLabelList(EnableIISCaching)
            End If
            Return _packLabels
        End Get
    End Property

    Private _gridheader As String = ""
    Public Property GridHeader As String
        Get
            Return TryGetFromViewState("GH", _gridheader)
        End Get
        Set(ByVal value As String)
            _gridheader = value
            TrySetViewState("GH", value)
        End Set
    End Property

    Public Property Height As Int32
    Public Property Count As Int32

    Public ReadOnly Property ExtraStyle As String
        Get
            If Height > 0 Then
                Return ";height:" & Height & "px;overflow:auto;zoom:1"
            End If
            Return ""
        End Get
    End Property

    Private _packageLabelCache As New Dictionary(Of Integer, String)

    Public Property ShowTracking() As Boolean

    Public Property CanView() As Boolean = True

    Public Property CanDelete() As Boolean = True

    Public Property CanAdd() As Boolean = True

    Public Property CanEdit() As Boolean = True

    Public Property CanAdminDelete() As Boolean = True
    Private _object_id As Int32 = 0

    Public Property Object_ID() As Int32
        Get
            Return TryGetFromViewState("OBJ_ID", _object_id)
        End Get
        Set(ByVal value As Int32)
            _object_id = value
            TrySetViewState("OBJ_ID", value)
        End Set

    End Property

    Public Property Package_Id() As Int32 = 0

    Public Property FromArchive() As Boolean

    Private _showAsJournal As Boolean
    Private _showAsJournalSet As Boolean
    Public Property ShowAsJournal As Boolean
        Get
            Return _showAsJournal
        End Get
        Set(value As Boolean)
            _showAsJournal = value
            _showAsJournalSet = True
        End Set
    End Property

    Protected Function TryGetFromViewState(Of T)(ByVal key As String, ByVal defaultValue As T) As T
        If Me.EnableViewState AndAlso Me.IsPostBack Then
            Dim value As Object = ViewState(key)
            Return (If(value IsNot Nothing, CType(value, T), defaultValue))
        End If

        Return defaultValue
    End Function
    Protected Sub TrySetViewState(ByVal key As String, ByVal value As Object)
        If Me.EnableViewState Then
            ViewState(key) = value
        End If
    End Sub


    Protected Sub messages_ItemDataBound(ByVal Sender As Object, ByVal e As RepeaterItemEventArgs)



        If (e.Item.ItemType = ListItemType.Item) OrElse (e.Item.ItemType = ListItemType.AlternatingItem) Then
            Dim mail As MailSearch.MailInfo = CType(e.Item.DataItem, MailSearch.MailInfo)

            CType(e.Item.FindControl("lblContentGrid"), Literal).Text = GetItemRowContent(mail)
            Dim trackingRow As PlaceHolder = CType(e.Item.FindControl("plhTracking"), PlaceHolder)
            If Not ShowAsJournal Then

                If mail.Track Then
                    Dim tracking As MailClient_MailTracking = CType(trackingRow.FindControl("pnlTracking"), MailClient_MailTracking)

                    trackingRow.Visible = True
                    tracking.Visible = True
                    tracking.Inline = True
                    tracking.InitiallyCollapsed = True
                    tracking.MailID = mail.ID
                    tracking.DataBind()
                Else
                    trackingRow.Visible = False
                End If
            Else
                'no tracking in journal mode
                trackingRow.Visible = False
            End If

            'ElseIf e.Item.ItemType = ListItemType.Footer Then
            '    If (Count <= 0) Then
            '        Dim objPlaceHolder As PlaceHolder = CType(e.Item.FindControl("plhMainRow"), PlaceHolder)
            '        objPlaceHolder.Controls.Add(New LiteralControl("<tr class='ListFooter footable-disabled'><td data-ignore='true' colspan='7' align='center'>" & GetLabel("nomessagesfound") & "</td></tr>"))
            '    End If
        End If


    End Sub

    Private Function GetItemRowContent(ByVal mail As MailSearch.MailInfo) As String
        If ShowAsJournal Then
            Return GetItemJournalRowContent(mail)
        Else
            Return GetItemTableRowContent(mail)
        End If
    End Function
    Private Function GetItemTableRowContent(ByVal mail As MailSearch.MailInfo) As String
        Dim text As New StringBuilder

        text.Append("<tr><td >")
        If mail.Track Then
            text.Append("<span class='icon icon-flag-error' style='font-size: 15px;'></span>") 'Alternative for MailFlagRed
        Else
            text.Append(ThemedImage.GetSpanIconTag("icon icon-flag"))
        End If
        text.Append("</td>")


        text.Append("<td align='left' >")
        text.Append(ArcoFormatting.FormatEmails(mail.From.ToString))
        text.Append("</td><td align='left' >")
        If Not String.IsNullOrEmpty(mail.MailTo) Then
            text.Append(ArcoFormatting.FormatEmails(mail.MailTo.ToString))
        ElseIf Not String.IsNullOrEmpty(mail.CC) Then
            text.Append(ArcoFormatting.FormatEmails(mail.CC.ToString))
        End If
        text.Append("</td><td align='left' >")
        If (mail.Subject.ToUpper() <> "UNTITLED") Then
            text.Append(Server.HtmlEncode(mail.Subject))
        End If
        text.Append("</td><td align='left' >")
        text.Append(ArcoFormatting.FormatDateLabel(mail.Mail_Date, True, False, False))


        If Package_Id <= 0 Then
            text.Append("</td><td align='left' >")
            text.Append(GetPackageDisplayLabel(mail))
        End If


        text.Append("</td><td align='left' >")
        text.Append("<a href='javascript:ViewMessage(")
        text.Append("</td><td  align='left' ><span style='color:#a6a6a6;font-style:italic;font-size:7pt'>Click <a href='javascript:ViewMessage(")
        text.Append(mail.ReferencingFileID)
        text.Append(");'>here</a> for full message details</span>")
        If (CanDelete OrElse CanAdminDelete) Then
            text.Append("</td><td  align='left' ><a href='javascript:PC().DeleteComment(")
            text.Append(mail.ReferencingFileID)
            text.Append(");'>")
            text.Append(ThemedImage.GetSpanIconTag("icon icon-delete", GetLabel("delete")))
            text.Append("</a>")
        End If
        text.Append("</td></tr>")

        Return text.ToString()
    End Function



    Private Function GetItemJournalRowContent(ByVal mail As MailSearch.MailInfo) As String
        Dim sb As New StringBuilder

        Dim attachments As MailFileList = MailFileList.GetAttachments(mail.ID)

        Dim senderUser As User = User.GetUser(mail.From)
        Dim currentUser As User = User.GetCurrentUser()

        Dim isFromSelf As Boolean = (currentUser.USER_ACCOUNT = mail.From)

        'start row
        sb.Append("<div class='row message")
        sb.Append(If(isFromSelf, " self", " other"))
        sb.Append("'>")

        'start col
        sb.Append("<div class='col-lg'>")

        'start card
        sb.Append("<div class='card'>")

        'start header
        sb.Append("<div class='card-header'>")

        sb.Append("<div class='from-container'>")
        sb.Append("<span class='from'>")
        'sb.Append("<span class='from'>" & GetLabel("mail_from") & ":")
        If senderUser IsNot Nothing Then
            RenderAvatar(sb, senderUser)
        Else
            RenderAvatar(sb, mail.From)
        End If


        If senderUser IsNot Nothing Then
            sb.Append(ArcoFormatting.FormatUserName(senderUser))
        Else
            sb.Append(mail.From)
        End If
        sb.Append("</span>")

        If Not String.IsNullOrEmpty(mail.MailTo) Then
            sb.Append("<br/><span class='title'>" & GetLabel("mail_to") & ": ")
            sb.Append(mail.MailTo)
            sb.Append("</span>")
        End If

        If Not String.IsNullOrEmpty(mail.CC) Then
            sb.Append("<br/><span class='title'>" & GetLabel("mail_cc") & ": ")
            sb.Append(mail.CC)
            sb.Append("</span>")
        End If

        If mail.Subject IsNot Nothing AndAlso mail.Subject.ToUpper() <> "UNTITLED" Then
            sb.Append("<hr>")
            sb.Append("<div>")
            sb.Append("<span style='font-style:italic;'>")
            sb.Append(Server.HtmlEncode(mail.Subject))
            sb.Append("</span>")
            sb.Append("</div>")
        End If

        sb.Append("</div>")

        sb.Append("<div align='right'>")

        If Package_Id <= 0 Then
            If Package_Id <= 0 Then
                sb.Append("<span class='title'>" & GetPackageDisplayLabel(mail) & "</span>")
            End If
        End If
        sb.Append("<span class='datetime'>")

        sb.Append(ArcoFormatting.FormatDateLabel(mail.Mail_Date, True, False, False))
        sb.Append("</span>")
        sb.Append("<div>")
        'sb.Append("<a href = 'javascript:ViewMessage(" & mail.ReferencingFileID & ");'>")
        'sb.Append(ThemedImage.GetSpanIconTag("icon icon-message-mail", GetLabel("preview")))
        'sb.Append("</a>&nbsp;")
        If CanDelete OrElse CanAdminDelete Then

            sb.Append("<a href = 'javascript:PC().DeleteComment(" & mail.ReferencingFileID & ");'>")
            sb.Append(ThemedImage.GetSpanIconTag("icon icon-delete", GetLabel("delete")))
            sb.Append("</a>")
        End If
        sb.Append("</div>")



        sb.Append("</div>")

        'end header

        sb.Append("</div>")

        'start body
        sb.Append("<div class='card-body'>")
        Dim lsBody As String
        Try
            Dim body As MailFile = MailFile.GetBody(mail.ID)
            If body IsNot Nothing AndAlso body.ID <> 0 Then

                lsBody = body.GetString()
                If body.FILE_EXT = "txt" Then
                    lsBody = Server.HtmlEncode(lsBody)
                Else
                    lsBody = RenderAttachmentsInline(lsBody, attachments)
                    lsBody = HtmlSanitizer.Sanitize(lsBody)
                End If

            Else
                lsBody = "<p>No body found</p>"
            End If

            sb.Append(lsBody)
        Catch ex As Exception
            sb.Append(ex.Message)
        End Try

        'end body
        sb.Append("</div>")


        If attachments.Any Then
            Dim openFooter As Boolean = False

            For Each attachment As MailFileList.FileInfo In attachments

                If Not moInlineAtts.Contains(attachment.ID) Then
                    If Not openFooter Then
                        openFooter = True
                        sb.Append("<div class='card-footer'>")
                    End If
                    sb.Append("<div class='file'><a href='javascript:void(0);' onclick='javascript:ViewDomaAtt( ")
                    sb.Append(attachment.ID)
                    sb.Append(",")
                    sb.Append(mail.ID)
                    sb.Append(")'>")
                    sb.Append(Icons.GetFileIconImage(attachment.FILE_EXT))
                    sb.Append("<span>")
                    sb.Append(GetFileTitle(attachment))
                    sb.Append("</span>")
                    sb.Append("</a></div>")
                End If
            Next

            'end footer
            If openFooter Then
                sb.Append("</div>")
            End If
        End If

        'end card and col 
        sb.Append("</div></div>")

        'end row
        sb.Append("</div>")

        Return sb.ToString
    End Function
    Private Function GetPackageDisplayLabel(mail As MailSearch.MailInfo) As String
        Dim refFile As File = File.GetFile(mail.ReferencingFileID)
        Dim packageLabel As String = GetLabel("messages")

        If refFile.PackageID > 0 Then
            ' Use cached package if available, otherwise retrieve and cache it
            Dim packageId As Integer = refFile.PackageID
            If Not _packageLabelCache.TryGetValue(packageId, packageLabel) Then
                Dim pack As IPackage = baseObjects.Package.GetPackage(packageId)
                packageLabel = PackageLabels.GetObjectLabel(pack.ID, "Package", Language, pack.Name)
                _packageLabelCache.Add(packageId, packageLabel)
            End If
        End If

        Return packageLabel
    End Function

    Private Sub RenderAvatar(ByVal builder As StringBuilder, ByVal senderUser As User)
        builder.Append("<div class='avatar'>")

        If Not String.IsNullOrEmpty(senderUser.USER_FIRSTNAME) Then
            builder.Append(senderUser.USER_FIRSTNAME.Substring(0, 1).ToUpper())
            If Not String.IsNullOrEmpty(senderUser.USER_LASTNAME) Then
                builder.Append(senderUser.USER_LASTNAME.Substring(0, 1).ToUpper())
            End If
        ElseIf Not String.IsNullOrEmpty(senderUser.USER_LASTNAME) Then
            builder.Append(senderUser.USER_LASTNAME.Substring(0, 1).ToUpper())
        ElseIf Not String.IsNullOrEmpty(senderUser.USER_DISPLAY_NAME) Then
            builder.Append(senderUser.USER_DISPLAY_NAME.Substring(0, 1).ToUpper())
        ElseIf Not String.IsNullOrEmpty(senderUser.USER_LOGIN) Then
            builder.Append(senderUser.USER_LOGIN.Substring(0, 1).ToUpper())
        End If

        builder.Append("</div>")
    End Sub
    Private Sub RenderAvatar(ByVal builder As StringBuilder, ByVal senderUser As String)
        builder.Append("<div class='avatar'>")
        If Not String.IsNullOrEmpty(senderUser) Then
            builder.Append(senderUser.Substring(0, 1).ToUpper())
        End If
        builder.Append("</div>")
    End Sub
    Private Function GetFileTitle(attachment As MailFileList.FileInfo) As String
        If Not attachment.Name.Contains("." & attachment.FILE_EXT) Then
            Return String.Concat(attachment.Name, ".", attachment.FILE_EXT)
        Else
            Return attachment.Name
        End If
    End Function


    Private Sub AddScript()
        Dim sb As New StringBuilder

        sb.Append("function ViewMessage(id){")
        sb.Append("var address = 'DM_VIEW_File.aspx?FILE_ID=' + id + '&DM_OBJECT_ID=")
        sb.Append(Object_ID & "';")
        sb.Append("PC().OpenWindow(address,'Viewfile','height=600,width=900,scrollbars=yes,resizable=yes,status=yes');}")

        sb.Append("function NewMessage() {PC().NewMessage(")
        sb.Append(Object_ID & ");}")

        sb.Append("function DeleteMessage(id){PC().DeleteMessage(id);}")
        sb.Append("function ReloadParent(){PC().Reload();}")
        ScriptManager.RegisterStartupScript(Page, Me.GetType(), "initmessages", sb.ToString, True)
    End Sub

    Public Overloads Sub DataBind(ByVal voObject As Arco.Doma.Library.baseObjects.DM_OBJECT)
        Object_ID = voObject.ID
        If TypeOf (voObject) Is Arco.Doma.Library.baseObjects.DM_VersionControlledObject Then
            FromArchive = CType(voObject, Arco.Doma.Library.baseObjects.DM_VersionControlledObject).FromArchive
        End If
        If FromArchive Then
            MessageListDataSource.SelectMethod = "GetArchivedObjectMessages"
            CanEdit = False
            CanAdd = False
            CanDelete = False
            CanView = True
        Else
            MessageListDataSource.SelectMethod = "GetObjectMessages"
            SetAccess(voObject)
        End If

        RenderHeader()
        BindForm()
    End Sub
    Private Sub BindForm()
        If CanView Then
            If Not _showAsJournalSet Then
                ShowAsJournal = Settings.GetValue("Interface", "DefaultMessagesControlMode", "").ToLower().Contains("journal")
            End If
            AddScript()
            MessageList.DataSourceID = "MessageListDataSource"
            MessageListDataSource.SelectParameters.Item("vlObjectID").DefaultValue = Object_ID
            MessageListDataSource.SelectParameters.Item("vlPackageID").DefaultValue = Package_Id

            MessageList.DataBind()
            Count = MessageList.Items.Count
            If (Count <= 0) Then
                Dim noMessageLabel As New StringBuilder
                Dim objPlaceHolder As PlaceHolder = CType(FindControl("plhMainRow"), PlaceHolder)
                If Not ShowAsJournal Then
                    noMessageLabel.Append("<table Class='SubList'><tr class='ListFooter footable-disabled'><td data-ignore='true' colspan='7' align='center'>")
                Else
                    noMessageLabel.Append("<div style='font-size: 12px;font-family:""Museo Sans"", sans-serif;color: #666666;font-weight: 300;'>")

                End If
                noMessageLabel.Append(GetLabel("nomessagesfound"))
                If Not ShowAsJournal Then
                    noMessageLabel.Append("</td></tr></table>")
                Else
                    noMessageLabel.Append("</div>")
                End If

                objPlaceHolder.Controls.Add(New LiteralControl(noMessageLabel.ToString()))
            End If


        Else
            Visible = False
        End If
    End Sub

    Private Sub SetAccess(ByVal voObject As Arco.Doma.Library.baseObjects.DM_OBJECT)
        If voObject.IsInRecycleBin Then
            CanEdit = False
            CanAdd = False
            CanDelete = False
        Else
            CanAdd = voObject.CanAddMessage
            CanDelete = voObject.CanDeleteMessages
            CanAdminDelete = voObject.HasAccess(ACL_Access.Access_Level.ACL_Admin_Messages)
        End If
        CanView = voObject.HasAccess(ACL_Access.Access_Level.ACL_View_Messages)

    End Sub

    Public Overrides Sub DataBind()

        If FromArchive Then
            MessageListDataSource.SelectMethod = "GetArchivedObjectMessages"
            CanEdit = False
            CanAdd = False
            CanDelete = False
            CanView = True
        Else
            MessageListDataSource.SelectMethod = "GetObjectMessages"
            If Object_ID > 0 Then
                Dim o As Arco.Doma.Library.baseObjects.DM_OBJECT = Arco.Doma.Library.ObjectRepository.GetObject(Object_ID)
                SetAccess(o)
            Else
                CanView = False
            End If
        End If

        BindForm()
        RenderHeader()

    End Sub

    Private moInlineAtts As List(Of Int32) = New List(Of Int32)
    Private Function RenderAttachmentsInline(ByVal vsBody As String, ByVal voAtts As MailFileList) As String
        Dim iPos As Int32
        Dim iPosEnd As Int32
        Dim dict As New Dictionary(Of String, String)
        iPos = vsBody.IndexOf("src=" & Chr(34) & "cid:")
        If iPos > 0 Then
            Do
                iPosEnd = vsBody.IndexOf(Chr(34), iPos + 7)
                If iPosEnd > 0 Then
                    Dim lsReplace As String = vsBody.Substring(iPos, iPosEnd - iPos + 1)
                    If Not dict.ContainsKey(lsReplace) Then
                        Dim lsName As String = lsReplace.Substring(lsReplace.IndexOf("cid") + 4)
                        If Not String.IsNullOrEmpty(lsName) Then
                            Dim iAt As Int32 = lsName.IndexOf("@")
                            If iAt > 0 Then
                                lsName = lsName.Substring(0, iAt)
                            Else
                                lsName = lsName.Substring(0, lsName.Length - 1) 'remove last "
                            End If
                            For Each loAt As MailFileList.FileInfo In voAtts
                                If loAt.Name = lsName OrElse loAt.Name & "." & loAt.FILE_EXT = lsName Then
                                    moInlineAtts.Add(loAt.ID)
                                    Dim lsReplaced As String = "src=""DM_VIEW_FILE.aspx?FILE_ID=" & loAt.ID & "&frommail=Y"""
                                    dict.Add(lsReplace, lsReplaced)
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                End If

                iPos = vsBody.IndexOf("src=" & Chr(34) & "cid:", iPos + 1)
            Loop While iPos > 0

            For Each d As KeyValuePair(Of String, String) In dict
                vsBody = vsBody.Replace(d.Key, d.Value)
            Next
        End If
        Return vsBody
    End Function

    Private Sub RenderHeader()
        If Not String.IsNullOrEmpty(GridHeader) Then
            'just show the header
            pnlToolbar.Visible = True

            If Not ArcoFormatting.FullWidthPanelLabels Then
                plhGridHeaderStart.Controls.Add(New LiteralControl("<div class='SubListHeader3RightFillerDiv' ><div class='SubListHeader3LeftSpacerDiv'></div>"))
                'plhGridHeaderStartEnd.Controls.Add(New LiteralControl("</div>"))
                plhGridHeader.Controls.Add(New LiteralControl("<div  class='SubListMainHeader'><div style='padding:5px'>" & GridHeader & "</div></div>"))
                plhGridHeaderEnd.Controls.Add(New LiteralControl("<div class='SubListHeader3RightSpacerDiv'></div></div>"))
            Else
                plhGridHeaderStart.Controls.Add(New LiteralControl("<div class='SubListHeader3RightFullFillerDiv' style='height:22px'>"))
                'plhGridHeaderStartEnd.Controls.Add(New LiteralControl("</div>"))
                plhGridHeader.Controls.Add(New LiteralControl("<div  class='SubListMainHeader' style='height:inherit'><div style='padding:5px'>" & GridHeader & "</div></div>"))
                plhGridHeaderEnd.Controls.Add(New LiteralControl("</div>"))
            End If
        End If

    End Sub

End Class
