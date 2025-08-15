
''' <summary>
''' Streams the file send in the request querystring file
''' the parameter must be encrypted!!
''' </summary>
''' <remarks></remarks>
Partial Class Tools_StreamFile
    Inherits BasePage

    Protected Overrides Sub OnInit(e As EventArgs)
        'override to stop default theming
    End Sub
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim lsFile As String = QueryStringParser.GetString("file")
        Dim lbAttach As Boolean = QueryStringParser.GetBoolean("attach")

        lsFile = StreamFileEncryptionService.Decrypt(lsFile)
        Dim fileName As String = QueryStringParser.GetString("fileName")

        Arco.Utils.Web.Streamer.StreamFile(Request, Response, lsFile, fileName, lbAttach)
    End Sub
End Class
