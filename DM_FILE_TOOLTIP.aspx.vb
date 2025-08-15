Imports Arco
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects

Partial Class DM_FILE_TOOLTIP
    Inherits BaseTooltipPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim bAdmin As Boolean = Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.isAdmin

        Dim fileID As Int32 = QueryStringParser.GetInt("FILE_ID")
        Dim fromArchive As Boolean = QueryStringParser.GetBoolean("fromarchive")
        Dim fromMail As Boolean = QueryStringParser.GetBoolean("frommail")


        If Not fromMail Then
            Dim f As File = Nothing

            If Not fromArchive Then
                If fileID > 0 Then
                    f = File.GetFile(fileID)
                Else
                    Dim llFin As Int32 = QueryStringParser.GetInt("FILE_FIN")
                    If llFin > 0 Then
                        f = File.GetFileByFIN(llFin)
                    End If
                End If
            Else
                f = File.GetFileFromArchive(fileID)
            End If

            If f Is Nothing OrElse f.LinkedToObject Is Nothing OrElse Not f.LinkedToObject.CanViewFiles Then
                Response.Write(GetLabel("filenotfound"))
                Response.End()
                Exit Sub
            End If

            AddHeader()

            Add("Type", GetLabel("file"))

            Add(GetLabel("doctitle"), f.Name, True)

            If Not f.isMail Then
                Add("FIN", f.FIN)
            End If
            Add("ID", f.ID)

            If f.Blocked Then
                Add(GetLabel("blocked"), GetLabel("yes"))
            End If

            If f.PackageID <> 0 Then
                Add(GetLabel("package"), Package.GetPackage(f.PackageID).TranslatedName)
            End If
            If Not f.isMail Then
                Add(GetLabel("version"), f.FILE_CHECKVERSION.toString)
            End If

            Add(GetLabel("creationdate"), ArcoFormatting.FormatDateLabel(f.FILE_INDEXDATE, True, False, False))
            Add(GetLabel("createdby"), ArcoFormatting.FormatUserName(f.FILE_CREATIONUSER), False, False)

            If Not f.isMail Then
                Add(GetLabel("modifdate"), ArcoFormatting.FormatDateLabel(f.FILE_MODIFDATE, True, False, False))
                Add(GetLabel("modifby"), ArcoFormatting.FormatUserName(f.FILE_MODIFUSER), False, False)
            End If
            If Not f.isMail Then
                If Not fromArchive Then
                    Select Case f.LinkedToObject.Status
                        Case DM_OBJECT.Object_Status.CheckedOut
                            Add(GetLabel("status"), EnumTranslator.GetEnumLabel(File.File_Status.CheckedOut))
                            Add(GetLabel("checkedoutby"), ArcoFormatting.FormatUserName(f.LinkedToObject.CheckOut_By), False, False)
                        Case DM_OBJECT.Object_Status.InProgress
                            Add(GetLabel("status"), EnumTranslator.GetEnumLabel(File.File_Status.InProgress))
                        Case DM_OBJECT.Object_Status.Archived, DM_OBJECT.Object_Status.Expired
                            Add(GetLabel("status"), EnumTranslator.GetEnumLabel(DM_OBJECT.Object_Status.Archived))
                        Case Else
                            If f.IndexStatus = File.IDX_Status.IndexOK Then
                                Add(GetLabel("status"), EnumTranslator.GetEnumLabel(f.Status))
                            ElseIf f.IndexStatus = FileBase.IDX_Status.IndexingInProgress Then
                                Add(GetLabel("status"), EnumTranslator.GetEnumLabel(f.Status) & " (Indexing in Progress)")
                            Else
                                Add(GetLabel("status"), EnumTranslator.GetEnumLabel(f.Status) & " (" & f.IndexStatus.ToString & ")")
                            End If
                            If f.Status = File.File_Status.CheckedOut Then
                                Add(GetLabel("checkedoutby"), ArcoFormatting.FormatUserName(f.FILE_CHECKBY), False, False)
                            End If
                            If f.Status = File.File_Status.CheckedOut OrElse f.Status = File.File_Status.InProgress Then
                                If Not String.IsNullOrEmpty(f.FILE_CHECKCOMMENT) Then Add(GetLabel("checkoutcomment"), ArcoFormatting.FormatUserName(f.FILE_CHECKCOMMENT), False, False)
                            End If
                    End Select

                Else
                    Add(GetLabel("status"), EnumTranslator.GetEnumLabel(DM_OBJECT.Object_Status.Archived))
                End If
            End If


            If Not f.isMail Then
                If f.Locked <> File.LockingStatus.NoLock Then
                    Add(GetLabel("locked"), GetLabel("yes"))
                    Add(GetLabel("lockedby"), ArcoFormatting.FormatUserName(f.LockedBy), False, False)
                Else
                    Add(GetLabel("locked"), GetLabel("no"))
                End If
            End If
            If Not String.IsNullOrEmpty(f.FILE_LANGCODE) Then
                Dim lang As Globalisation.LanguageList.LanguageInfo = Globalisation.LanguageList.GetLanguageList.Item(f.FILE_LANGCODE)
                Dim langDescription As String
                If lang IsNot Nothing Then
                    langDescription = lang.Description
                Else
                    langDescription = f.FILE_LANGCODE
                End If
                Add(GetLabel("language"), langDescription)
            End If

            Add(GetLabel("pagecount"), f.FILE_NPAGES)
            Add(GetLabel("size"), Arco.IO.File.FormatSize(f.FILE_SIZE))


            If f.IsReadOnlyOrOnAReadOnlyFileServer Then
                Add(GetLabel("readonly"), GetLabel("yes"))
            Else
                Add(GetLabel("readonly"), GetLabel("no"))
            End If

            If Not String.IsNullOrEmpty(f.FILE_KEYWORDS) Then Add(GetLabel("keywords"), f.FILE_KEYWORDS)

            If bAdmin Then
                Try
                    Add(GetLabel("validsig"), If(f.FileHashIsValid, GetLabel("yes"), GetLabel("no")))
                Catch ex As Exception

                End Try

                Add("In Blob", If(f.FILE_IN_BLOB, GetLabel("yes"), GetLabel("no")))
                Add("FileServer", f.FileServer.Name)
            End If


            AddFooter()
        Else 'mail file
            Dim mf As Mail.MailFile = Mail.MailFile.GetFile(fileID, 0)

            If mf Is Nothing OrElse Not mf.CanView Then
                Response.Write(GetLabel("filenotfound"))
                Response.End()
                Exit Sub
            End If

            AddHeader()
            Add("Type", GetLabel("file"))

            Add(GetLabel("doctitle"), mf.Name)

            Add("ID", mf.ID)


            Add(GetLabel("creationdate"), ArcoFormatting.FormatDateLabel(mf.FILE_INDEXDATE, True, False, False))
            Add(GetLabel("createdby"), ArcoFormatting.FormatUserName(mf.FILE_CREATIONUSER), False, False)

            Add(GetLabel("language"), mf.FILE_LANGCODE)
            Add(GetLabel("pagecount"), mf.FILE_NPAGES)
            Add(GetLabel("size"), Arco.IO.File.FormatSize(mf.FILE_SIZE))

            If Not String.IsNullOrEmpty(mf.FILE_KEYWORDS) Then Add(GetLabel("keywords"), mf.FILE_KEYWORDS)


            AddFooter()
        End If

        litContent.Text = getcontent
    End Sub
End Class
