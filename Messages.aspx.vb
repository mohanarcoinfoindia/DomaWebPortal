
Partial Class Messages
    Inherits BasePage
    Private mbHideTabs As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not QueryStringParser.Exists("DM_OBJECT_ID") Then
            Dim llID As Int32 = 0
            Dim loLoaded As Arco.ApplicationServer.Library.BusinessBase = QueryStringParser.CurrentDMObject
            If loLoaded Is Nothing Then
                GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_INVALIDOBJECT)
            End If
            If TypeOf (loLoaded) Is Arco.Doma.Library.baseObjects.DM_OBJECT Then
                llID = CType(loLoaded, Arco.Doma.Library.baseObjects.DM_OBJECT).ID
            Else
                llID = CType(loLoaded, Arco.Doma.Library.Routing.cCase).TargetObject.ID
            End If
            lblMessages.Object_ID = llID
        Else
            lblMessages.Object_ID = QueryStringParser.GetInt("DM_OBJECT_ID")
        End If

        lblMessages.FromArchive = QueryStringParser.GetBoolean("fromarchive")


        lblMessages.DataBind()
    End Sub
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit

        ParseQueryString()

        If Not mbHideTabs Then
            Me.MasterPageFile = "~/masterpages/Preview.master"
        Else
            Me.MasterPageFile = "~/masterpages/DetailSub.master"
        End If
    End Sub
    Private Sub ParseQueryString()
        mbHideTabs = QueryStringParser.GetBoolean("hidetabs")
    End Sub
End Class
