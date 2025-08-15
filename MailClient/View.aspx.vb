Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library
Imports Telerik.Web.UI
Imports Arco.Doma.Library.Mail

Public Class View
    Inherits BasePage

    Private mbPreview As Boolean
    Private meMode As Mode

    Protected TemplateID As Int32


    Private moCurrentMail As Arco.Doma.Library.Mail.DMMail

    Private Enum Mode
        Full
        BodyOnly
        Print
    End Enum
    Protected ReadOnly Property CurrentMail() As Arco.Doma.Library.Mail.DMMail
        Get
            If moCurrentMail Is Nothing Then
                Dim llID As Int32 = QueryStringParser.GetInt("DM_MAIL_ID")
                If llID > 0 Then
                    moCurrentMail = Arco.Doma.Library.Mail.DMMail.GetMail(llID)
                    If moCurrentMail.ID <> llID Then
                        Response.Write("Mail not found")
                        Response.End()
                    End If
                Else
                    Response.Write("No DM_MAIL_ID supplied")
                    Response.End()
                End If
            End If
            Return moCurrentMail
        End Get
    End Property
    Private Function FormatTextAsHtml(ByVal text As String) As String
        ' format text into displayable HTML      
        text = Server.HtmlEncode(text)

        text = text.Replace(vbLf, "<br>")
        Return text
    End Function


    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'Put user code to initialize the page here

        mbPreview = QueryStringParser.GetBoolean("PREVIEW")
        meMode = Mode.Full
        If QueryStringParser.GetBoolean("BODYONLY") Then
            meMode = Mode.BodyOnly
        End If
        If (printme.Value = "Y") Then
            meMode = Mode.Print
        End If
        printme.Value = ""

        Select Case meMode
            Case Mode.BodyOnly
                plhBodyOnly.Visible = True
                plhFullContent.Visible = False
                plhPrint.Visible = False
            Case Mode.Print
                plhBodyOnly.Visible = False
                plhFullContent.Visible = False
                plhPrint.Visible = True
            Case Mode.Full
                plhBodyOnly.Visible = False
                plhFullContent.Visible = True
                plhPrint.Visible = False
        End Select

        SetLabels()

        If meMode = Mode.Full Then
            If QueryStringParser.GetBoolean("hidetoolbar") Then
                paneToolbar.Visible = False
                paneMailHeader.Height = Unit.Pixel(200)
            Else
                paneToolbar.Visible = True
                CreateScroller()

                TranslateToolbar(tlbMail)
                paneMailHeader.Height = Unit.Pixel(180)
            End If
        End If


        DisplayMessageFromArchive()



        If meMode = Mode.Full Then

            Me.tabTracking.Visible = CurrentMail.Track

            If Not tabTracking.Visible AndAlso Not tabBody.Visible Then
                'only the body
                paneTabs.Visible = False
            Else
                paneTabs.Visible = True
                If Not Page.IsPostBack Then
                    If Not tabBody.Visible Then
                        tabTracking.Selected = True
                        Mailtracking.Selected = True

                    End If
                End If
            End If
        End If

        If meMode = Mode.Print Then
            Dim lsStartupScript As String
            lsStartupScript = "<script language='javascript'>"
            lsStartupScript &= "PrintContent();"
            lsStartupScript &= "</script>"
            Page.ClientScript.RegisterStartupScript(Me.GetType, "RESPRINT", lsStartupScript)

        End If


        Dim lsPreview As String = ""
        If QueryStringParser.GetBoolean("PREVIEW") Then
            lsPreview = "Y"
        End If

        Dim lsRefresh As String = "function OpenMail(id){"
        lsRefresh &= " PC().GotoLink('View.aspx?DM_MAIL_ID=' + id + '&maillist=" & QueryStringParser.GetString("maillist") & "&preview=" & lsPreview & "');"
        lsRefresh &= "}"

        Page.ClientScript.RegisterStartupScript(Me.GetType, "REFRESH", lsRefresh, True)
    End Sub
    Private Sub SetLabels()
        Title = GetLabel("mail")
        tabMain.Tabs(0).Text = GetLabel("body")
        tabMain.Tabs(1).Text = GetLabel("tracking")
    End Sub
    Protected Sub tlbMail_ButtonClick(ByVal sender As Object, ByVal e As RadToolBarEventArgs) Handles tlbMail.ButtonClick
        Dim lsCommandName As String = CType(e.Item, RadToolBarButton).CommandName
        Select Case lsCommandName
            Case "delete"
                CurrentMail.DeleteForCurrentUser()
            Case "closetracking"
                CurrentMail.CloseTrackingItemForCurrentUser()
            Case "stopfollowup"
                CurrentMail.StopTrackingFollowup()
            Case Else
                Throw New NotImplementedException("The command with CommandName " & lsCommandName & " is not implemented")
        End Select
        If Not mbPreview Then
            Page.ClientScript.RegisterStartupScript(GetType(String), "Close", "RefreshParentPage();window.close();", True)
        Else
            Page.ClientScript.RegisterStartupScript(GetType(String), "Close", "parent.GotoCurrent();parent.CloseDetail();", True)
        End If

    End Sub
    Private Function GetDisplayableEmail(ByVal MailAddress As DMMail.MailRecipient) As String
        If Not String.IsNullOrEmpty(MailAddress.Account) Then
            Return ArcoFormatting.FormatUserName(MailAddress.Account)
        Else
            Return MailAddress.Email
        End If

    End Function
    Private Function GetDisplayableEmail(ByVal MailAddresses As DMMail.MailRecipients) As String

        Dim sRet As String = ""
        For Each ma As DMMail.MailRecipient In MailAddresses
            If Not String.IsNullOrEmpty(sRet) Then
                sRet &= ";"
            End If
            sRet &= GetDisplayableEmail(ma)
        Next
        Return sRet
    End Function
    Private Sub ExpandHeaderHeight(ByVal withSize As Int32)
        paneMailHeader.Height = Unit.Pixel(paneMailHeader.Height.Value + withSize)
    End Sub

    Private Sub ShowInfoLabel(ByVal vsText As String)
        If vsText.Length > 0 Then
            ExpandHeaderHeight(32)
            lblInfoLabel.Visible = True
            lblInfoLabel.Text = vsText
        Else
            lblInfoLabel.Visible = False
        End If
    End Sub
    Private Function TranslateToolBarItem(ByVal t As String) As String
        Dim ret As String = ""
        Try
            ret = GetDecodedLabel(t)
        Catch ex As Exception
            ret = t
        End Try
        If String.IsNullOrEmpty(ret) Then
            ret = t
        End If
        Return ret
    End Function
    Private Sub TranslateToolbar(ByVal tlb As RadToolBar)
        For Each btn As RadToolBarItem In tlb.Items
            If TypeOf btn Is RadToolBarButton Then
                Dim b As RadToolBarButton = CType(btn, RadToolBarButton)
                If Not b.IsSeparator Then
                    If Not String.IsNullOrEmpty(b.Text) Then
                        b.Text = TranslateToolBarItem(b.Text)
                    End If
                    If Not String.IsNullOrEmpty(b.ToolTip) Then
                        b.ToolTip = TranslateToolBarItem(b.ToolTip)
                    End If
                    If String.IsNullOrEmpty(b.SpriteCssClass) Then
                        If Not String.IsNullOrEmpty(b.ImageUrl) AndAlso Not b.ImageUrl.StartsWith("~") Then
                            b.ImageUrl = ThemedImage.GetUrl(b.ImageUrl, Page)
                        End If
                    End If
                End If
            End If
            If TypeOf btn Is RadToolBarDropDown Then
                Dim b As RadToolBarDropDown = CType(btn, RadToolBarDropDown)

                If Not String.IsNullOrEmpty(b.Text) Then
                    b.Text = TranslateToolBarItem(b.Text)
                End If
                If Not String.IsNullOrEmpty(b.ToolTip) Then
                    b.ToolTip = TranslateToolBarItem(b.ToolTip)
                End If
                If String.IsNullOrEmpty(b.SpriteCssClass) Then
                    If Not String.IsNullOrEmpty(b.ImageUrl) AndAlso Not b.ImageUrl.StartsWith("~") Then
                        b.ImageUrl = ThemedImage.GetUrl(b.ImageUrl, Page)
                    End If
                End If
            End If



        Next
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
                        Dim lsName As String = lsReplace.Substring(lsReplace.IndexOf("cid:") + 4)
                        If Not String.IsNullOrEmpty(lsName) Then
                            Dim iAt As Int32 = lsName.IndexOf("@")
                            If iAt > 0 Then
                                lsName = lsName.Substring(0, iAt)
                            Else
                                lsName = lsName.Substring(0, lsName.Length - 1) 'remove last "
                            End If
                            For Each loAt As MailFileList.FileInfo In voAtts
                                If loAt.Name = lsName Then
                                    moInlineAtts.Add(loAt.ID)
                                    Dim lsReplaced As String = "src=""../DM_VIEW_FILE.aspx?FILE_ID=" & loAt.ID & "&frommail=Y"""
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
    Private Sub ShowLinkedObjects()
        Dim fl As FileList = FileList.GetFilesReferencingID(CurrentMail.ID)
        drpLinked.Visible = False

        If fl.Count > 1 Then
            drpLinked.Text = GetLabel("linkedto")
            drpLinked.Buttons.Clear()
        End If

        For Each f As FileList.FileInfo In fl
            Dim obj As DM_OBJECT = ObjectRepository.GetObject(f.OBJECT_ID)
            If Not obj.CanViewMeta Then
                Continue For
            End If

            Const wraplength = 20
            Dim lsText As String = obj.Name
            Dim lsTooltip As String = lsText
            If lsText.Length > wraplength Then
                lsText = lsText.Substring(0, wraplength) & "..."
            End If

            If f.Type = File.File_Type.Message Then
                lsText &= " (" & GetLabel("message") & ")"
                lsTooltip &= " (" & GetLabel("message") & ")"
            Else
                lsText &= " (" & GetLabel("file") & ")"
                lsTooltip &= " (" & GetLabel("file") & ")"
            End If
            Dim newButton As New RadToolBarButton() With {.NavigateUrl = "Javascript:OpenObject(" & obj.ID & ");", .Text = lsText, .ToolTip = lsTooltip}
            If fl.Count = 1 Then
                If Not Page.IsPostBack Then
                    tlbMail.Items.Insert(tlbMail.Items.IndexOf(drpLinked), newButton)
                End If
            Else
                drpLinked.Visible = True
                drpLinked.Buttons.Add(newButton)
            End If

        Next
    End Sub
    Private Sub DisplayMessageFromArchive()
        ' Get the requested message.
        If Not CurrentMail.CanView Then
            GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
        End If

        If Not Page.IsPostBack Then
            CurrentMail.MarkReadForCurrentUser()
        End If

        TemplateID = CurrentMail.Template_ID

        If meMode = Mode.Full Then
            ctrlTracking.CurrentMail = CurrentMail
            ctrlTracking.DataBind()
        End If

        If CurrentMail.ReplyRequired Then
            Select Case CurrentMail.CurrentUserReceivedTrackItem.RecipientStatus
                Case TrackItem.RecipientState.Closed, TrackItem.RecipientState.Replied
                    ShowInfoLabel(GetLabel("repliedon") & " " & ArcoFormatting.FormatDateLabel(CurrentMail.CurrentUserReceivedTrackItem.RepliedDate, True, False, True))
                Case Else
                    ShowInfoLabel(GetLabel("mailrequiresresponse"))
            End Select
        End If

        ' ShowInfoLabel("test")

        If meMode = Mode.Full Then
            ShowLinkedObjects()
        End If

        ' Have the message now. Display stuff.
        Dim dt As New DataTable()
        dt.Columns.Add("label")
        dt.Columns.Add("value")

        AddHeaderRow(dt, GetLabel("mail_from"), GetDisplayableEmail(CurrentMail.From), False)

        Dim lsTo As String = GetDisplayableEmail(CurrentMail.MailTo)
        If String.IsNullOrEmpty(lsTo) Then
            lsTo = "No recipients found."
        End If
        AddHeaderRow(dt, GetLabel("mail_to"), lsTo, False)

        Dim lsCC = GetDisplayableEmail(CurrentMail.CC)

        If CurrentMail.Track Then
            AddHeaderRow(dt, GetLabel("mail_external"), lsCC, False)
        Else
            AddHeaderRow(dt, GetLabel("mail_cc"), lsCC, False)
        End If

        AddHeaderRow(dt, GetLabel("mail_bcc"), GetDisplayableEmail(CurrentMail.BCC), False)

        If Not String.IsNullOrEmpty(CurrentMail.Subject) Then
            AddHeaderRow(dt, GetLabel("mail_subject"), CurrentMail.Subject)
        Else
            AddHeaderRow(dt, GetLabel("mail_subject"), "[no subject]")
        End If

        If CurrentMail.Mail_Date <> DateTime.MinValue Then
            AddHeaderRow(dt, GetLabel("date"), CurrentMail.Mail_Date.ToString)
        Else
            AddHeaderRow(dt, GetLabel("date"), "[no date]")
        End If


        Dim loAttList As MailFileList = MailFileList.GetAttachments(CurrentMail.ID)
        Dim enc As Encoding = Response.ContentEncoding
        Dim lsBody As String
        Try
            lsBody = CurrentMail.GetBody(enc)
            If Not CurrentMail.HtmlBody Then
                lsBody = FormatTextAsHtml(lsBody)
            Else
                Response.ContentEncoding = enc

                lsBody = RenderAttachmentsInline(lsBody, loAttList)
                lsBody = HtmlSanitizer.Sanitize(lsBody)

            End If

        Catch ex As Exception
            lsBody = "<font color='red'>There was an error reading the body: " & ex.Message & "</font>"
        End Try


        '' Get attachments
        Dim sbAtts As New StringBuilder

        For Each loAtt As MailFileList.FileInfo In loAttList
            If Not moInlineAtts.Contains(loAtt.ID) Then

                If meMode = Mode.Full Then
                    If sbAtts.Length <> 0 Then
                        sbAtts.Append(", ")
                    End If
                    sbAtts.Append("<a href='javascript:ViewDomaAtt(")
                    sbAtts.Append(loAtt.ID)
                    sbAtts.Append(");' class='ButtonLink'>")
                    sbAtts.Append(Arco.IO.FileName.CreateFileName(loAtt.Name, loAtt.FILE_EXT))
                    sbAtts.Append("</a> (")
                    sbAtts.Append(Arco.IO.File.FormatSize(loAtt.FILE_SIZE))
                    sbAtts.Append(")")
                Else
                    sbAtts.Append(Arco.IO.FileName.CreateFileName(loAtt.Name, loAtt.FILE_EXT))
                End If
            End If
        Next
        '' Only add a row for attachments if there are attachments present.
        If sbAtts.Length > 0 Then
            AddHeaderRow(dt, GetLabel("mail_attachments"), "<pre style='width: 400px;white-space: pre-line;word-wrap:break-word;font-size:8pt;'>" & sbAtts.ToString & "</pre>", False)
            'ExpandHeaderHeight(32)
        End If

        Select Case meMode
            Case Mode.Full
                Dim dv As New DataView(dt)
                dgHeaders.AutoGenerateColumns = False
                dgHeaders.DataSource = dv
                dgHeaders.DataBind()

                Dim mailClientEnabled As Boolean = Settings.GetValue("MailClient", "Enabled", True)
                'update toolbar
                If Not Page.IsPostBack Then
                    tlbMail.FindItemByValue("closetracking").Visible = CurrentMail.CurrentUserCanCloseTrackingItem()
                    tlbMail.FindItemByValue("stopfollowup").Visible = CurrentMail.CurrentUserCanStopTrackingFollowup
                    tlbMail.FindItemByValue("reply").Visible = mailClientEnabled AndAlso CurrentMail.CanReply
                    tlbMail.FindItemByValue("replyall").Visible = mailClientEnabled AndAlso CurrentMail.CanReply
                    tlbMail.FindItemByValue("forward").Visible = mailClientEnabled AndAlso CurrentMail.CanForward
                    tlbMail.FindItemByValue("delete").Visible = CurrentMail.CurrentUserCanDelete
                End If

                lblMessage.Text = lsBody


                If lblMessage.Text.Length > 0 Then
                    tabBody.Visible = True
                Else
                    tabBody.Visible = False
                End If
                AddFixedMessageToForm(lblTemplate)

            Case Mode.Print

                Dim dv As New DataView(dt)
                dgPrintHeader.AutoGenerateColumns = False
                dgPrintHeader.DataSource = dv
                dgPrintHeader.DataBind()

                lblPrintMessage.Text = lsBody
                AddFixedMessageToForm(lblPrintMessage)
            Case Mode.BodyOnly
                lblMessageBodyOnly.Text = lsBody
                AddFixedMessageToForm(lblMessageBodyOnly)

                If String.IsNullOrEmpty(lblMessageBodyOnly.Text) Then
                    lblMessageBodyOnly.Text = "No text to display."
                End If
                tabBody.Visible = True
        End Select
    End Sub

    Private Sub AddFixedMessageToForm(ByVal toLabel As Label)
        If Not String.IsNullOrEmpty(CurrentMail.FixedMessage) Then
            toLabel.Text &= "<br/><br/>"
            If CurrentMail.HtmlBody Then
                toLabel.Text &= CurrentMail.FixedMessage
            Else
                toLabel.Text &= ArcoFormatting.FormatText(CurrentMail.FixedMessage)
            End If
        End If
    End Sub
    Private Sub AddHeaderRow(ByVal dt As DataTable, ByVal vsLabel As String, ByVal vsValue As String)
        AddHeaderRow(dt, vsLabel, vsValue, True)
    End Sub
    Private Sub AddHeaderRow(ByVal dt As DataTable, ByVal vsLabel As String, ByVal vsValue As String, ByVal htmlEncode As Boolean)
        If String.IsNullOrEmpty(vsValue) Then
            Exit Sub
        End If

        Dim dr As DataRow = dt.NewRow()
        dr(0) = "<span class='Label'>" & vsLabel & ":</span>"
        If htmlEncode Then
            dr(1) = Server.HtmlEncode(vsValue)
        Else
            dr(1) = vsValue
        End If

        dt.Rows.Add(dr)
    End Sub

#Region " Scroller "

    Private Sub SetScrollerLinks(ByVal voToolbar As RadToolBar)
        Dim btnPrevious As RadToolBarButton = CType(voToolbar.Items.FindItemByValue("prev"), RadToolBarButton)
        Dim btnNext As RadToolBarButton = CType(voToolbar.Items.FindItemByValue("next"), RadToolBarButton)
        Dim btnClose As RadToolBarButton = CType(voToolbar.Items.FindItemByValue("close"), RadToolBarButton)

        btnClose.Visible = mbPreview
        btnClose.EnableImageSprite = True
        btnClose.SpriteCssClass = "icon icon-close"

        btnNext.EnableImageSprite = True
        btnNext.SpriteCssClass = "icon icon-page-next"

        btnPrevious.EnableImageSprite = True
        btnPrevious.SpriteCssClass = "icon icon-page-previous"

        If CanScroll Then
            btnPrevious.Enabled = (PrevID > 0)

            btnPrevious.NavigateUrl = "javascript:ScrollPrevious();"
            If btnPrevious.Enabled Then
                btnPrevious.ToolTip = GetDecodedLabel("prevdoc") & " (" & (CurrentIndex - 1) & "/" & ItemCount & ")"
            End If

            btnNext.Enabled = (NextID > 0)

            btnNext.NavigateUrl = "javascript:ScrollNext();"
            If btnNext.Enabled Then
                'remove the ~/ at the start

                btnNext.ToolTip = GetDecodedLabel("nextdoc") & " (" & (CurrentIndex + 1) & "/" & ItemCount & ")"
            End If
        Else
            btnPrevious.Enabled = False
            btnNext.Enabled = False
        End If
    End Sub

    Private isFirst As Boolean
    Private isLast As Boolean
    Private NextID As Int32
    Private PrevID As Int32
    Private CurrentIndex As Int32
    Private ItemCount As Int32
    Private CanScroll As Boolean
    Private Sub InitScoller()
        Dim lsList As String = QueryStringParser.GetString("maillist")
        If Not String.IsNullOrEmpty(lsList) Then

            Dim laItems() As String = lsList.Split(";"c)
            ItemCount = laItems.Length
            For i As Int32 = 0 To laItems.Length - 1
                If Not String.IsNullOrEmpty(laItems(i)) Then
                    CanScroll = True
                    Dim liCurr As Int32 = Convert.ToInt32(laItems(i))
                    Dim lbCurr As Boolean = False
                    If liCurr = CurrentMail.ID Then
                        lbCurr = True
                    End If
                    If (lbCurr) Then
                        CurrentIndex = i + 1
                        If i = 0 Then isFirst = True
                        If i = laItems.Length - 1 Then isLast = True


                        If (Not isFirst) Then PrevID = laItems(i - 1)
                        If Not isLast Then NextID = laItems(i + 1)

                        Exit For

                    End If
                End If
            Next
        End If
    End Sub
    Private Sub CreateScroller()
        InitScoller()

        SetScrollerLinks(toolbarScroll)

        If CanScroll Then
            Page.Title = Page.Title & " (" & CurrentIndex & "/" & ItemCount & ")"

            Dim sbScrollNext As StringBuilder = New StringBuilder()

            sbScrollNext.Append("function ScrollNext(){")
            If NextID > 0 Then
                sbScrollNext.Append("   OpenMail(" & NextID & ");;")
            Else
                sbScrollNext.Append(" CloseOnReload();")
            End If
            sbScrollNext.Append("}")

            sbScrollNext.Append("function ScrollPrevious(){")
            If PrevID > 0 Then
                sbScrollNext.Append("   OpenMail(" & PrevID & ");")
            Else
                sbScrollNext.Append(" CloseOnReload();")
            End If
            sbScrollNext.Append("}")

            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "DetailScrollNext", sbScrollNext.ToString, True)
        End If
    End Sub

    Private Sub View_LoadComplete(sender As Object, e As EventArgs) Handles Me.LoadComplete
        If tabMain.Tabs.Where(Function(tab) tab.Visible).Count <= 1 Then
            tabMain.Visible = False
        End If
    End Sub

#End Region


End Class
