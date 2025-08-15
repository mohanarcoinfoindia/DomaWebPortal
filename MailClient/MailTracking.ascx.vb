Imports Arco.Doma.Library

Partial Class MailClient_MailTracking
    Inherits BaseUserControl

    Private moCurrentMail As Arco.Doma.Library.Mail.DMMail
    Private mbInline As Boolean = False

    Public WriteOnly Property Inline() As Boolean
        Set(ByVal value As Boolean)
            mbInline = value
        End Set
    End Property
    Public Property CurrentMail() As Mail.DMMail
        Get
            Return moCurrentMail
        End Get
        Set(ByVal value As Mail.DMMail)
            moCurrentMail = value
        End Set
    End Property
    Public Property MailID() As Int32
        Set(ByVal value As Int32)
            moCurrentMail = Mail.DMMail.GetMail(value)
        End Set
        Get
            If moCurrentMail Is Nothing Then
                Return 0
            Else
                Return moCurrentMail.ID
            End If
        End Get
    End Property

   
    Public Overrides Sub DataBind()
        If CurrentMail Is Nothing OrElse CurrentMail.ID = 0 Then
            Exit Sub
        End If
        If CurrentMail.Track Then
            maRows.Clear()
            laLoadedMails.Add(CurrentMail)
            If Not CurrentMail.Template Is Nothing Then
                If CurrentMail.Template.SHOW_FULL_TRACKING Then
                    'start from the original mail
                    Dim lcolItemList As Mail.TrackItemList = Mail.TrackItemList.GetTrackItemListFromMailid(CurrentMail.ID)
                    Dim llOrigMailID As Int32 = 0
                    For Each loItm As Mail.TrackItemList.TrackItemInfo In lcolItemList
                        If loItm.OrigMailID > 0 Then
                            llOrigMailID = loItm.OrigMailID
                            Exit For
                        End If
                    Next
                    If llOrigMailID <> CurrentMail.ID AndAlso llOrigMailID > 0 Then
                        lcolItemList = Mail.TrackItemList.GetTrackItemListFromMailid(llOrigMailID)
                    End If
                    lstTracking.DataSource = lcolItemList
                Else
                    'go down only
                    lstTracking.DataSource = Mail.TrackItemList.GetTrackItemListFromMailid(CurrentMail.ID)
                End If
            Else
                lstTracking.DataSource = Mail.TrackItemList.GetTrackItemListFromMailid(CurrentMail.ID)
            End If

            lstTracking.DataBind()



            Dim sb As StringBuilder = New StringBuilder
            If Me.InitiallyCollapsed Then
                sb.AppendLine(" var ftm_" & CurrentMail.ID & " = false;")
            Else
                sb.AppendLine(" var ftm_" & CurrentMail.ID & " = true;")
            End If
            sb.AppendLine("function TAR_" & CurrentMail.ID & "(){")
            sb.AppendLine("var img = document.getElementById('" & Me.ClientID & "_expallbut' );")
            sb.AppendLine("if (img.src.match('Collapse.svg')) {")
            sb.AppendLine(" img.src = '" & Page.ResolveClientUrl("~/Images/Expand.svg") & "';")
            sb.AppendLine("}")
            sb.AppendLine(" else {    ")
            sb.AppendLine("   img.src = '" & Page.ResolveClientUrl("~/Images/Collapse.svg") & "';")
            sb.AppendLine("}")
            For Each r As Int32 In maRows
                sb.AppendLine("TSTS_" & CurrentMail.ID & "(" & r & ",ftm_" & CurrentMail.ID & ");")
            Next
            sb.AppendLine("ftm_" & CurrentMail.ID & " = !ftm_" & CurrentMail.ID & ";")
            sb.AppendLine("}")

            Page.ClientScript.RegisterClientScriptBlock(Me.GetType, "expandall" & CurrentMail.ID, sb.ToString, True)
        End If
    End Sub
    Private maRows As System.Collections.Generic.List(Of Int32) = New System.Collections.Generic.List(Of Int32)

    Dim laLoadedMails As System.Collections.Generic.List(Of Mail.DMMail) = New System.Collections.Generic.List(Of Mail.DMMail)
    Private mbInitCollapsed As Boolean = False

    Public Function GetMail(ByVal vlID As Int32) As Mail.DMMail    
        For Each loMail As Mail.DMMail In laLoadedMails
            If loMail.ID = vlID Then               
                Return loMail
            End If
        Next
        Dim loNewMail As Mail.DMMail = Mail.DMMail.GetMail(vlID)
        laLoadedMails.Add(loNewMail)
        Return loNewMail

    End Function
    Protected Sub lstTracking_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles lstTracking.ItemDataBound
        If e.Item.ItemType = ListItemType.Item Or e.Item.ItemType = ListItemType.AlternatingItem Then
            Dim obj As Mail.TrackItemList.TrackItemInfo = CType(e.Item.DataItem, Mail.TrackItemList.TrackItemInfo)

            e.Item.Controls.Add(New LiteralControl(GetTrackingRow(obj, 1)))
        ElseIf e.Item.ItemType = ListItemType.Header Then
            e.Item.Controls.Add(New LiteralControl(GetHeader()))
        ElseIf e.Item.ItemType = ListItemType.Footer Then
            e.Item.Controls.Add(New LiteralControl(GetFooter()))
        End If

    End Sub
    Private Function GetFooter() As String
        Return " </table></center>"
    End Function
    Private Function GetHeader() As String
        Dim sb As StringBuilder = New StringBuilder
        If Not mbInline Then
            sb.Append("<center><table class='List'><tr>")
        Else
            sb.Append("<center><table class='SubList'><tr>")
        End If


        Dim lsToggleIcon As String
        If Me.InitiallyCollapsed Then
            lsToggleIcon = "<a href='javascript:TAR_" & CurrentMail.ID & "();'><img id='" & Me.ClientID & "_expallbut' class='icon-xs' src='" & Page.ResolveClientUrl("~/Images/Expand.svg") & "' border='0' alt=''/></a>"
        Else
            lsToggleIcon = "<a href='javascript:TAR_" & CurrentMail.ID & "();'><img id='" & Me.ClientID & "_expallbut' class='icon-xs' src='" & Page.ResolveClientUrl("~/Images/Collapse.svg") & "' border='0' alt=''/></a>"
        End If

        sb.Append("     <th align='left' valign='top'>")
        sb.Append(lsToggleIcon)
        sb.Append("</th>")

        sb.Append("     <th align='left'>&nbsp;</th>")
        sb.Append("    <th align='left'>")
        sb.Append(GetLabel("mail_from"))
        sb.Append("</th>")
        sb.Append("    <th align='left'>")
        sb.Append(GetLabel("mail_to"))
        sb.Append("</th>")
        sb.Append("    <th align='left'>")
        sb.Append(GetLabel("date"))
        sb.Append("</th>")
        sb.Append("     <th align='left'>&nbsp;</th>")
        sb.Append("     <th align='left'>&nbsp;</th>")
        sb.Append("      <th align='left'>")
        sb.Append(GetLabel("mail_subject"))
        sb.Append("</th>")

        sb.Append(" </tr>")

        Return sb.ToString
    End Function
    Public Property InitiallyCollapsed() As Boolean
        Get
            Return mbInitCollapsed
        End Get
        Set(ByVal value As Boolean)
            mbInitCollapsed = True
        End Set
    End Property
    Private Function GetTrackingRow(ByVal voItem As Mail.TrackItemList.TrackItemInfo, ByVal viLevel As Int32) As String
        Dim lbIsCurrent As Boolean = (voItem.Mail_ID = CurrentMail.ID)
        If voItem.Mail_ID = 0 Then
            Return ""
        End If



        Dim lsScript As String
        Dim loMail As Mail.DMMail = GetMail(voItem.Mail_ID)

        Dim lcolSubItems As Mail.TrackItemList = Mail.TrackItemList.GetSubTrackItemList(voItem.Track_ID)
        Dim lsAbstract = loMail.Abstract

        lsAbstract = lsAbstract.Replace(Chr(34), "'")
        lsAbstract = lsAbstract.Replace("'", "\'")
        lsAbstract = lsAbstract.Replace(vbCr, "")
        lsAbstract = lsAbstract.Replace(vbLf, "<br/>")

        lsScript = "onclick=""javascript:OpenMail(" & voItem.Mail_ID & ");"" style=""cursor:pointer"""

        Dim sb As New StringBuilder

        sb.Append("<tr ")
        sb.Append(lsScript)
        sb.Append("  >")

        'expand collapse
        Dim lsIcon As String
        If lcolSubItems.Any Then
            If Me.InitiallyCollapsed Then
                lsIcon = "<a href='javascript:TSTS_" & CurrentMail.ID & "(" & voItem.Track_ID & ",null);'><img id='" & Me.ClientID & "_expbut_" & voItem.Track_ID & "' class='icon-xs' src='" & Page.ResolveClientUrl("~/Images/Expand.svg") & "' border='0' alt=''/></a>"
            Else
                lsIcon = "<a href='javascript:TSTS_" & CurrentMail.ID & "(" & voItem.Track_ID & ",null);'><img id='" & Me.ClientID & "_expbut_" & voItem.Track_ID & "' class='icon-xs' src='" & Page.ResolveClientUrl("~/Images/Collapse.svg") & "' border='0' alt=''/></a>"
            End If
        Else
            lsIcon = "<img src='" & Page.ResolveClientUrl("~/Images/hr_l.gif") & "' border='0' alt=''/>"
        End If
        sb.Append("<td width='20px' align='left' valign='top'>")
        sb.Append(lsIcon)
        sb.Append("</td>")


        'type
        Dim icon As String = ""
        Select Case voItem.Type
            Case Mail.TrackItem.MailDirection.NewMail
                icon = ThemedImage.GetSpanIconTag("icon icon-unread icon-color-light")
            Case Mail.TrackItem.MailDirection.Reply
                icon = ThemedImage.GetSpanIconTag("icon icon-arrow-reply")
            Case Mail.TrackItem.MailDirection.Forward
                icon = ThemedImage.GetSpanIconTag("icon icon-message-mail-forward")
        End Select
        'If lsAbstract.Length > 0 Then
        '    sb.Append("<td width='20px' align='left' valign='top'><img  src='" & lsImg & "' onmouseover=" & Chr(34) & "tooltip.show('" & lsAbstract & "...');" & Chr(34) & " onmouseout=" & Chr(34) & "tooltip.hide();" & Chr(34) & "/></td>")
        'Else
        sb.Append("<td width='20px' align='left' valign='top'>")
        sb.Append(icon)
        sb.Append("</td>")
        ' End If

        'from
        sb.Append("<td width='100px' align='left' valign='top'><nobr>")
        sb.Append(ArcoFormatting.FormatUserName(voItem.Sender))
        sb.Append("</nobr></td>")
        'to
        sb.Append("<td width='100px' align='left' valign='top'><nobr>")
        sb.Append(ArcoFormatting.FormatUserName(voItem.Recipient))
        sb.Append("</nobr></td>")
        'send date
        sb.Append("<td width='70px' align='left' valign='top'><nobr>")
        sb.Append(ArcoFormatting.FormatDateLabel(loMail.Mail_Date, True, False, False))
        sb.Append("</nobr></td>")

        'sender status
        sb.Append("<td width='50px' align='left' valign='top'><nobr>")
        If voItem.SenderStatus = Mail.TrackItem.SenderState.Open Then
            sb.Append("Open")
        Else
            sb.Append("Closed")
        End If
        sb.Append("</nobr></td>")
        'recipient status
        sb.Append("<td width='70px' align='left' valign='top'><nobr>")
        Select Case voItem.RecipientStatus
            Case Mail.TrackItem.RecipientState.UnRead
                sb.Append(ThemedImage.GetSpanIconTag("icon icon-disabled icon-unread icon-color-light"))
            Case Mail.TrackItem.RecipientState.Read
                sb.Append(ThemedImage.GetSpanIconTag("icon icon-disabled icon-read"))
                sb.Append("&nbsp;")
                sb.Append(ArcoFormatting.FormatDateLabel(voItem.ReadDate, True, False, False))
            Case Mail.TrackItem.RecipientState.Replied
                sb.Append(ThemedImage.GetSpanIconTag("icon icon-disabled icon-arrow-reply"))
                sb.Append("&nbsp;")
                sb.Append(ArcoFormatting.FormatDateLabel(voItem.RepliedDate, True, False, False))
            Case Mail.TrackItem.RecipientState.Closed
                sb.Append(ThemedImage.GetSpanIconTag("icon icon-disabled icon-delete"))
                sb.Append("&nbsp;")
                sb.Append(ArcoFormatting.FormatDateLabel(voItem.ClosedDate, True, False, False))
        End Select
        sb.Append("</nobr></td>")
        'mail subject
        sb.Append("<td width='300px' align='left' valign='top'><nobr>")
        sb.Append(Server.HtmlEncode(loMail.Subject))
        sb.Append("</nobr></td>")



        sb.Append("</tr>")

        If lcolSubItems.Any Then
            maRows.Add(voItem.Track_ID)
            sb.Append("<tr id='")
            sb.Append(Me.ClientID)
            sb.Append("_exprow_")
            sb.Append(voItem.Track_ID)
            sb.Append("'")
            If Me.InitiallyCollapsed Then
                sb.Append("style='display:none'")
            End If
            sb.Append("><td width='20px'>&nbsp;</td><td colspan='7' width='100%' valign='top'><table class='List'>")
            For Each loSubItem As Mail.TrackItemList.TrackItemInfo In lcolSubItems
                sb.Append(GetTrackingRow(loSubItem, viLevel + 1))
            Next
            sb.Append("</table></td></tr>")
        End If
        Return sb.ToString

    End Function
End Class
