Public Class DM_EDIT_FILE
    Inherits BasePage


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Page.Title = GetLabel("editfile")

        Dim llID As Int32 = QueryStringParser.GetInt("FILE_ID")

        Dim loFile As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(llID)
        If loFile.CanModifyContent Then

            If Not String.IsNullOrEmpty(loFile.FILE_EXT) Then
                Dim loFT As Arco.Doma.Library.baseObjects.DM_FileTypeList.FileTypeInfo = loFile.LinkedToObject.GetFileType(loFile.FILE_EXT)
                If Not loFT.FILE_WEBDAVENABLED Then
                    If loFT.FILE_INLINEEDITING Then
                        Server.Transfer("DM_EDIT_FILE_DIRECT.aspx?FILE_ID=" & llID)
                    Else
                        'editing is not allowed
                        GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INVALIDOPERATION)
                    End If
                Else
                    webdavcontrol.EditFile(llID)
                End If
            End If
        Else
            GotoErrorPage(loFile.GetLastError.Code)
        End If
    End Sub
End Class
