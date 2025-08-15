Imports Arco.Batch

Public Class UserControls_WebDavControl
    Inherits BaseUserControl
   
    Public Sub EditFile(ByVal vlFileID As Int32)

        Try
            Dim loCopy As Arco.Doma.Library.BatchJobs.CopyFileForEditing = Arco.Doma.Library.BatchJobs.CopyFileForEditing.RequestCopy(vlFileID)

            Dim sbContent As New StringBuilder
            If Arco.IO.File.Exists(loCopy.FullPath) Then

                sbContent.Append("<script type='text/javascript'>")
                Dim useOfficeUriSchemes As Boolean = Settings.GetValue("WebDav", "UseOfficeUriSchemes", False)
                Dim closeSelf As Boolean = True

                If loCopy.IsWebDav Then
                    If useOfficeUriSchemes AndAlso loCopy.IsOffice Then
                        'office uri scheme
                        sbContent.Append("location.href = '" & loCopy.GetUrl(True) & "';")
                        closeSelf = False
                    ElseIf loCopy.IsOffice OrElse loCopy.IsOpenOffice Then
                        'add the js file
                        AddScript("<script type='text/javascript' src='./JS/OfficeOpen.js'></script>")

                        'office : use officeopen js
                        'mode = 0|1|2|3; 0 - MS,OO; 1 - OO,MS; 2 - MS; 3 - OO                         
                        sbContent.Append("OfficeOpen.OpenFileWith('" & loCopy.GetUrl(False) & "', 0);")
                    Else
                        'non office
                        sbContent.Append("window.open('" & loCopy.GetUrl(False) & "','Editfile" & vlFileID.ToString & "');")
                    End If
                Else
                    'unc
                    sbContent.Append("window.open('" & loCopy.GetUrl(False) & "','Editfile" & vlFileID.ToString & "');")
                End If
                If closeSelf Then
                    sbContent.Append("if (window.opener) {setTimeout(function(){window.close();}, 500);}")
                End If

                sbContent.Append("</script>")

                    AddScript(sbContent.ToString)
                Else
                    AddScript("<script language='javascript'>alert('File not found');</script>")
            End If
        Catch ex As Exception
            Arco.Utils.Logging.LogError("Unexpected error in webdavcontrol", ex)

            AddScript("<script language='javascript'>alert('Something went wrong');</script>")
        End Try
    End Sub

    Private Sub AddScript(ByVal script As String)
        plScript.Controls.Add(New LiteralControl(script))
    End Sub
End Class
