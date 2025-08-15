
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Routing

Partial Class DM_OBJECTHISTORY
    Inherits BasePage
    Private mbHideTabs As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim loLoaded As Object = QueryStringParser.CurrentDMObject
        Dim o As DM_OBJECT = TryCast(loLoaded, DM_OBJECT)
        If o IsNot Nothing Then
            lblHistory.DIN = o.DIN
            lblHistory.CASE_ID = o.Case_ID
            If Not o.CanViewHistory Then
                GotoErrorPage(o.GetLastError.Code)
                Exit Sub
            End If
        Else
            Dim c As cCase = TryCast(loLoaded, cCase)
            If c IsNot Nothing Then
                lblHistory.DIN = c.TargetObject.DIN
                lblHistory.CASE_ID = c.Case_ID
                If Not c.CanViewHistory Then
                    GotoErrorPage(c.GetLastError.Code)
                End If
            Else
                Throw New InvalidOperationException("No object supplied")
            End If
            End If
        lblHistory.SUB_OBJ_DIN = QueryStringParser.GetInt("SUB_OBJ_DIN")

    End Sub
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As EventArgs) Handles Me.PreInit

        ParseQueryString()

        If Not mbHideTabs Then
            MasterPageFile = "~/masterpages/Preview.master"
        Else
            MasterPageFile = "~/masterpages/DetailSub.master"
        End If
    End Sub
    Private Sub ParseQueryString()
        If QueryStringParser.GetBoolean("hidetabs") Then
            mbHideTabs = True
        End If
    End Sub
End Class
