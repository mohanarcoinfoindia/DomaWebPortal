
Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Imports Arco.Doma.Library.Routing

Partial Class CaseHistory
    Inherits BasePage
    Private mbHideTabs As Boolean

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        Dim loLoaded As Object = QueryStringParser.CurrentDMObject
        Dim hc As HistoryCase = TryCast(loLoaded, HistoryCase)
        If hc IsNot Nothing Then
            lblHistory.CASE_ID = hc.Case_ID
            If Not hc.CanViewHistory Then
                GotoErrorPage(hc.GetLastError.Code)
                Return
            End If
            If hc.DM_Object_ID > 0 Then
                lblHistory.OBJ_DIN = ObjectRepository.GetObject(hc.DM_Object_ID).DIN
            End If
        Else
            Dim o As DM_OBJECT = TryCast(loLoaded, DM_OBJECT)
            If o IsNot Nothing Then
                If o.CanViewHistory Then
                    lblHistory.CASE_ID = o.Case_ID
                    lblHistory.OBJ_DIN = o.DIN
                Else
                    GotoErrorPage(o.GetLastError.Code)
                End If
            Else
                Dim c As cCase = TryCast(loLoaded, cCase)
                If c IsNot Nothing Then
                    If c.CanViewHistory Then
                        lblHistory.CASE_ID = c.Case_ID
                        If c.DM_Object_ID <> 0 AndAlso c.TargetObject.CanViewHistory Then
                            lblHistory.OBJ_DIN = c.TargetObject.DIN
                        End If
                    Else
                        GotoErrorPage(c.GetLastError.Code)
                    End If

                End If
            End If
        End If
    End Sub
    Protected Sub Page_PreInit(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreInit

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
