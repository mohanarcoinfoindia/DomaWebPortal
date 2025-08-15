Imports Arco.Doma.Library.baseObjects

Partial Class Tools_FileDiff
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim llFile1 As Int32 = QueryStringParser.GetInt("FILE_ID1")
        Dim llFile2 As Int32 = QueryStringParser.GetInt("FILE_ID2")

        If llFile1 > 0 AndAlso llFile2 > 0 Then
            Dim loFile1 As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(llFile1)
            If loFile1 Is Nothing OrElse Not loFile1.CanView Then
                GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                Return
            End If
            Dim loFile2 As Arco.Doma.Library.File = Arco.Doma.Library.File.GetFile(llFile2)
            If loFile2 Is Nothing OrElse Not loFile2.CanView Then
                GotoErrorPage(LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                Return
            End If

            If Not String.IsNullOrEmpty(loFile1.Html) AndAlso Not String.IsNullOrEmpty(loFile2.Html) Then

                Dim lsHTML1 As String = loFile1.GetString("htm")
                Dim lsHTML2 As String = loFile2.GetString("htm")

                Dim loDiffEngine As Arco.Utils.HTMLDiffEngine = New Arco.Utils.HTMLDiffEngine(lsHTML1, lsHTML2)
                Dim lsOutHTML As String = loDiffEngine.merge

                Response.Write(lsOutHTML)

            Else
                If loFile1.Html = "" Then
                    Response.Write("file 1 has not html rendition")
                End If
                If loFile2.Html = "" Then
                    Response.Write("file 2 has not html rendition")
                End If

            End If



        Else
            Response.Write("Please provide a FILE_ID1 and FILE_ID2 querystring parameter")
        End If
    End Sub
End Class
