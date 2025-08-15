Imports Arco.Doma.Library

Partial Class dm_action_profiles
    Inherits BaseAdminOnlyPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        pnlError.Visible = False
        If QueryStringParser.Exists("deleteid") Then
            Try
                Dim llDelID As Int32 = QueryStringParser.GetInt("deleteid")
                Dim loProf As ACL.ACL_PROFILE = ACL.ACL_PROFILE.GetProfile(llDelID)
                If Not loProf.isSystem Then
                    Dim lcolUsedIn As ACL.ACL_RIGHTSList = ACL.ACL_RIGHTSList.GetACLRightsForProfile(llDelID)
                    If lcolUsedIn.Count = 0 Then
                        ACL.ACL_PROFILE.DeleteACL_PROFILE(llDelID)
                    Else
                        pnlError.Visible = True
                        errormsg.Text = "This profile is still used at " & lcolUsedIn.Count & " places"
                    End If
                End If
            Catch ex As Exception

            End Try
        End If

        'load the contents        
        Dim lsContent As New StringBuilder("<table class='List HoverList PaddedTable StickyHeaders'><tr class='ListHeader'><th>" & GetLabel("name") & "</th><th>&nbsp;</th></tr>")
        For Each loProfile As ACL.ACL_PROFILE_LIST.PROFILEInfo In ACL.ACL_PROFILE_LIST.GetPROFILEList()
            If Not loProfile.isSystem Then
                lsContent.Append("<tr>")
                lsContent.Append("<td><a href='javascript:EditProfile(")
                lsContent.Append(loProfile.ID)
                lsContent.Append(");' class='ButtonLink'>")
                lsContent.Append(loProfile.Name)
                lsContent.Append("</a></b></td>")
                lsContent.AppendLine("<td><a href ='javascript:DeleteProfile(")
                lsContent.Append(loProfile.ID)
                lsContent.Append(",")
                lsContent.Append(EncodingUtils.EncodeJsString(loProfile.Name))
                lsContent.Append(");'>")
                lsContent.Append("<span class='icon icon-delete' title='" & GetLabel("delete") & "' />")
                lsContent.Append("</a></td>")
                lsContent.AppendLine("</tr>")
            Else
                lsContent.Append("<tr><td><a href='javascript:ViewProfile(")
                lsContent.Append(loProfile.ID)
                lsContent.Append(");'  class='ButtonLink'>")
                lsContent.Append(loProfile.Name)
                lsContent.Append("</a></td><td>&nbsp;</td></tr>")
            End If
        Next
        lsContent.Append("<tr class='ListFooter'><td colspan='2'><div class='SubListMainHeaderT'>")

        lsContent.Append("<span class='icon icon-add-new' title='" & GetLabel("add") & "' onclick='javascript:NewProfile();'")

        lsContent.Append("</div></td></tr>")
        lsContent.Append("</table>")

        Dim loContent As Label = New Label
        loContent.Text = lsContent.ToString

        pnlProfiles.Controls.Add(loContent)
    End Sub
End Class
