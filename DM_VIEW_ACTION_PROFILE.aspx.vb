Imports Arco.Doma.Library

Public Class DM_VIEW_ACTION_PROFILE
    Inherits BasePage
    Private mcolProfileActions As ACL.ACL_ACTION_LIST
    Private mcolCats As baseObjects.OBJECT_CATEGORYList

    Private ReadOnly Property Categories As baseObjects.OBJECT_CATEGORYList
        Get
            If mcolCats Is Nothing Then
                mcolCats = baseObjects.OBJECT_CATEGORYList.GetOBJECT_CATEGORYList(False)
            End If
            Return mcolCats
        End Get
    End Property

    Private mcolProcs As Routing.PROCEDUREList


    Private ReadOnly Property Procedures As Routing.PROCEDUREList
        Get
            If mcolProcs Is Nothing Then
                mcolProcs = Routing.PROCEDUREList.GetPROCEDUREList()
            End If
            Return mcolProcs
        End Get
    End Property

    Private Function GetUserEventName(ByVal voUserEvent As Website.UserEventList.UserEventInfo) As String
        Dim lsName As String = "Global"
        If voUserEvent.Object_ID > 0 Then
            Select Case voUserEvent.Object_Type
                Case "Category"
                    lsName = "Global Category"
                    For Each loCat As baseObjects.OBJECT_CATEGORYList.OBJECT_CATEGORYInfo In Categories
                        If loCat.ID = voUserEvent.Object_ID Then
                            lsName = GetCategoryLabel(loCat)
                            Exit For
                        End If
                    Next
                Case "Procedure"
                    lsName = "Global Procedure"
                    For Each loProc As Routing.PROCEDUREList.PROCEDUREInfo In Procedures
                        If loProc.ID = voUserEvent.Object_ID Then
                            lsName = GetProcedureLabel(loProc)
                            Exit For
                        End If
                    Next
            End Select
        End If

        Return lsName & ":" & voUserEvent.Caption
    End Function


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim llID As Int32 = QueryStringParser.GetInt("profileid")
       
        If Not Arco.Doma.Library.Security.BusinessIdentity.CurrentIdentity.isAdmin Then
            If Not ObjectRepository.GetObject(QueryStringParser.GetInt("dm_object_id")).CanViewACL Then
                GotoErrorPage(baseObjects.LibError.ErrorCode.ERR_INSUFFICIENTRIGHTS)
                Exit Sub
            End If
        End If

        Page.Title = ACL.ACL_PROFILE.GetProfile(llID).Name

        mcolProfileActions = ACL.ACL_ACTION_LIST.GetProfileACTIONList(llID)

        Dim i As Int32 = 1
        For Each loAction As ACL.ACL_ACTION_LIST.ACTIONInfo In ACL.ACL_ACTION_LIST.GetACTIONList()
            Select Case loAction.ID
                Case ACL.ACL_Access.Access_Level.ACL_Execute_User_Event
                    Dim lcolUserEvents As Website.UserEventList = Arco.Doma.Library.Website.UserEventList.GetUserEventList()
                    AddAction(loAction.ID, loAction.Category, "Execute All User Events", 0)
                    For Each loUserEvent As Website.UserEventList.UserEventInfo In lcolUserEvents

                        AddAction(loAction.ID, loAction.Category, GetUserEventName(loUserEvent), loUserEvent.ID)
                    Next
                Case ACL.ACL_Access.Access_Level.ACL_Execute_Custom_Action
                    Dim lcolCustActions As Website.DMCustomActionList = Arco.Doma.Library.Website.DMCustomActionList.GetCustomActionList("")
                    AddAction(loAction.ID, loAction.Category, "Execute All Custom Actions", 0)
                    For Each loCustAction As Website.DMCustomActionList.CustomActionInfo In lcolCustActions
                        AddAction(loAction.ID, loAction.Category, loCustAction.Name, loCustAction.ID)
                    Next
                Case Else

                    AddAction(loAction.ID, loAction.Category, loAction.Name, 0)

            End Select

            i = i + 1
        Next

        Dim t As New System.Web.UI.WebControls.Table With {.CssClass = "SubList PaddedTable"}
        Dim currHeaderRow As TableRow
        Dim currDataRow As TableRow
        For Each kv As KeyValuePair(Of String, List(Of String)) In _actions
            currHeaderRow = New TableRow
            t.Rows.Add(currHeaderRow)
            currDataRow = New TableRow
            t.Rows.Add(currDataRow)

            currHeaderRow.Cells.Add(New TableHeaderCell() With {.Text = GetProfileCategoryLabel(kv.Key), .VerticalAlign = VerticalAlign.Top, .Wrap = False})
            Dim sb As StringBuilder = New StringBuilder

            sb.Append("<div style=""max-height:100px;overflow:auto;"">")
            For Each s As String In kv.Value
                sb.Append(s)
                sb.Append("<br/>")
            Next
            sb.Append("</div>")
            Dim c As TableCell = New TableCell() With {.Text = sb.ToString(), .VerticalAlign = VerticalAlign.Top, .Wrap = False}
            currDataRow.Cells.Add(c)

        Next
        pnlProfileActions.Controls.Add(t)
    End Sub

    Private Function GetProfileCategoryLabel(ByVal cat As String) As String
        Try
            Return GetDecodedLabel(cat)
        Catch ex As Exception
            Return cat
        End Try
    End Function

    Private Function ActionFound(ByVal viActionID As ACL.ACL_Access.Access_Level, ByVal vlSubObjectID As Integer) As Boolean
        Return mcolProfileActions.Cast(Of ACL.ACL_ACTION_LIST.ACTIONInfo).Any(Function(x) x.ID = viActionID AndAlso x.Sub_Object_ID = vlSubObjectID)       
    End Function

    Private _actions As SortedDictionary(Of String, List(Of String)) = New SortedDictionary(Of String, List(Of String))
    Private Sub AddAction(ByVal vlID As ACL.ACL_Access.Access_Level, ByVal vsCategory As String, ByVal vsName As String, ByVal vlSubObjectID As Integer)

        If ActionFound(vlID, vlSubObjectID) Then

            If Not _actions.ContainsKey(vsCategory) Then _actions(vsCategory) = New List(Of String)

            _actions(vsCategory).Add(vsName)
        End If

    End Sub
End Class
