
Namespace Doma
    Partial Class Comments
        Inherits BasePage

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

            Dim llID As Integer = QueryStringParser.GetInt("DM_OBJECT_ID")
            If llID = 0 Then

                Dim loLoaded As Arco.ApplicationServer.Library.BusinessBase = QueryStringParser.CurrentDMObject
                If loLoaded Is Nothing Then
                    GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INVALIDOBJECT)
                End If
                If TypeOf (loLoaded) Is Arco.Doma.Library.baseObjects.DM_OBJECT Then
                    lblComments.DataBind(CType(loLoaded, Arco.Doma.Library.baseObjects.DM_OBJECT))
                Else
                    lblComments.DataBind(CType(loLoaded, Arco.Doma.Library.Routing.cCase).TargetObject)
                End If

            Else
                Dim o As Arco.Doma.Library.baseObjects.DM_OBJECT = Arco.Doma.Library.ObjectRepository.GetObject(llID)
                lblComments.DataBind(o)

            End If

        End Sub
        Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit

            If Not QueryStringParser.GetBoolean("hidetabs") Then
                MasterPageFile = "~/masterpages/Preview.master"
            Else
                MasterPageFile = "~/masterpages/DetailSub.master"
            End If
        End Sub

    End Class
End Namespace
