Imports Arco.Doma.Library.Mail

Partial Class MailTooltip
    Inherits BaseTooltipPage


    Private Function GetMailToolTip(ByVal mail As DMMail) As String
        If mail.ID = 0 Then
            Return "Mail not found"
            Exit Function
        End If
        AddHeader()
        If Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.isAdmin Then
            Add("Mail ID", mail.ID)
        End If
        Add(GetLabel("mail_from"), mail.From.ToString)
        Add(GetLabel("date"), ArcoFormatting.FormatDateLabel(mail.Mail_Date, True, False, False))
        If mail.Track Then
            If mail.CurrentUserReceivedTrackItem IsNot Nothing Then
                Select Case mail.CurrentUserReceivedTrackItem.RecipientStatus
                    Case TrackItem.RecipientState.UnRead
                        If mail.CurrentUserReceivedTrackItem.ReplyRequired Then
                            Add(GetLabel("status"), GetLabel("unread") & " (" & GetLabel("mailrequiresresponse") & ")")
                        Else
                            Add(GetLabel("status"), GetLabel("unread"))
                        End If
                    Case TrackItem.RecipientState.Read
                        If mail.CurrentUserReceivedTrackItem.ReplyRequired Then
                            Add(GetLabel("status"), GetLabel("read") & " (" & GetLabel("mailrequiresresponse") & ")")
                        Else
                            Add(GetLabel("status"), GetLabel("read"))
                        End If
                    Case TrackItem.RecipientState.Replied
                        Add(GetLabel("repliedon"), ArcoFormatting.FormatDateLabel(mail.CurrentUserReceivedTrackItem.RepliedDate, True, False, False))
                    Case TrackItem.RecipientState.Closed
                        Add(GetLabel("status"), GetLabel("closed"))
                        If Not String.IsNullOrEmpty(mail.CurrentUserReceivedTrackItem.RepliedDate) Then
                            Add(GetLabel("repliedon"), ArcoFormatting.FormatDateLabel(mail.CurrentUserReceivedTrackItem.RepliedDate, True, False, False))
                        End If
                End Select

            End If
        End If

        AddFooter()
        Return GetContent()

    End Function

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load


        Dim id As Integer = QueryStringParser.GetInt("MAIL_ID")

        If id = 0 Then
            litContent.Text = "No tooltip for object 0"
            Return
        End If

        Dim loMail As DMMail = DMMail.GetMail(id)
        If loMail.CanView() Then
            litContent.Text = GetMailToolTip(loMail)
        Else
            litContent.Text = "Mail not found"
        End If
    End Sub
End Class
