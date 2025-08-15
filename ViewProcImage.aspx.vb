
Partial Class ViewProcImage
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim llProcID As Int32 = QueryStringParser.GetInt("PROC_ID")
        If llProcID > 0 Then
            Dim loProc As Arco.Doma.Library.Routing.Procedure = Arco.Doma.Library.Routing.Procedure.GetProcedure(llProcID)
            If loProc IsNot Nothing Then
                Dim bCanView As Boolean = True 'todo : check view rigths?
                If bCanView Then
                    Dim llStepID As String = QueryStringParser.GetInt("STEP_ID")

                    Dim path As String = Settings.GetDirectory("Locations", "ProcImgPath")


                    path &= "Proc" & llProcID.ToString
                    If llStepID > 0 Then
                        path &= "Step" & llStepID.ToString
                    End If

                    path &= ".pdf"

                    Arco.Utils.Web.Streamer.StreamFile(Request, Response, path, Nothing, False)

                Else
                    GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                End If
            Else
                Response.Write("Procedure not found")
            End If
        End If
    End Sub
End Class
