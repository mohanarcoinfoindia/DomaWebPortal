Imports Arco.Doma.Library

Partial Class DM_EDIT_ACTION_PROFILE
    Inherits BaseAdminOnlyPage

    Protected msID As Int32
    Private loProfile As ACL.ACL_PROFILE


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



    Private Sub Save()
        Dim lbNameOk As Boolean = True
        For Each loProf As ACL.ACL_PROFILE_LIST.PROFILEInfo In ACL.ACL_PROFILE_LIST.GetPROFILEList
            If loProf.ID <> msID AndAlso loProf.Name.Equals(txtProfileName.Text.Trim, StringComparison.InvariantCultureIgnoreCase) Then
                lbNameOk = False
                Exit For
            End If
        Next
        If lbNameOk Then
            If msID = 0 Then 'insert
                loProfile = ACL.ACL_PROFILE.NewACL_PROFILE
                loProfile.Name = txtProfileName.Text               
                loProfile = loProfile.Save()
                msID = loProfile.ID
            Else 'update
                loProfile.Name = txtProfileName.Text
                loProfile = loProfile.Save()
            End If

            Page.ClientScript.RegisterStartupScript(Me.GetType, "refreshopener", "window.opener.Refresh();", True)
        Else
            Page.ClientScript.RegisterStartupScript(Me.GetType, "profexists", "ShowMessage('" & GetDecodedLabel("profileexists") & "','error');", True)
        End If
    End Sub
    Private Sub DoActions()
        If Not Page.IsPostBack Then Return

        Dim act As String = Request.Form("actiontype")
        Select Case act
            Case "save"
                Save()
            Case "add"
                Dim llID As Integer = Convert.ToInt32(Request.Form("actionid"))
                Dim llSubID As Integer = Convert.ToInt32(Request.Form("subactionid"))
                loProfile.AddAction(llID, llSubID)

            Case "remove"
                Dim llID As Integer = Convert.ToInt32(Request.Form("actionid"))
                Dim llSubID As Integer = Convert.ToInt32(Request.Form("subactionid"))

                loProfile.RemoveAction(llID, llSubID)
        End Select
    End Sub
    Private mcolProfileActions As ACL.ACL_ACTION_LIST
    Private Sub LoadScreen()

        txtProfileName.Text = loProfile.Name
        Page.Title = loProfile.Name

        mcolProfileActions = ACL.ACL_ACTION_LIST.GetProfileACTIONList(msID)
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


        Dim t As New System.Web.UI.WebControls.Table With {.CssClass = "SubList"}
        Dim currHeaderRow As TableRow
        Dim currDataRow As TableRow
        If _selectedactions.Any Then
            currHeaderRow = New TableRow
            t.Rows.Add(currHeaderRow)
            currDataRow = New TableRow
            t.Rows.Add(currDataRow)

            currHeaderRow.Cells.Add(New TableHeaderCell() With {.Text = "Selected (" & _selectedactions.Count & ")", .VerticalAlign = VerticalAlign.Top, .Wrap = False})
            Dim sb As New StringBuilder

            For Each s As String In _selectedactions
                sb.Append(s)
            Next
            Dim c As New TableCell() With {.Text = sb.ToString(), .VerticalAlign = VerticalAlign.Top, .Wrap = False}
            currDataRow.Cells.Add(c)
        End If

        For Each kv As KeyValuePair(Of String, List(Of String)) In _actions
            currHeaderRow = New TableRow
            t.Rows.Add(currHeaderRow)
            currDataRow = New TableRow
            t.Rows.Add(currDataRow)

            currHeaderRow.Cells.Add(New TableHeaderCell() With {.Text = GetProfileCategoryLabel(kv.Key), .VerticalAlign = VerticalAlign.Top, .Wrap = False})
            Dim sb As New StringBuilder

            For Each s As String In kv.Value
                sb.Append(s)
            Next
            Dim c As New TableCell() With {.Text = sb.ToString(), .VerticalAlign = VerticalAlign.Top, .Wrap = False}
            currDataRow.Cells.Add(c)

        Next
        pnlProfileActions.Controls.Add(t)
    End Sub
    Private Function GetProfileCategoryLabel(ByVal cat As String) As String
        Try
            Dim s As String = GetDecodedLabel(cat)
            If String.IsNullOrEmpty(s) Then s = cat
            Return s
        Catch ex As Exception
            Return cat
        End Try
    End Function
    Private _actions As New SortedDictionary(Of String, List(Of String))
    Private _selectedactions As New List(Of String)
    Private Sub AddAction(ByVal vlID As ACL.ACL_Access.Access_Level, ByVal vsCategory As String, ByVal vsName As String, ByVal vlSubObjectID As Integer)
        Dim lsImg As String = ""
        Dim lsHref As String

        If ActionFound(vlID, vlSubObjectID) Then
            lsHref = "<a href='javascript:RemoveAction(" & vlID & "," & vlSubObjectID & ");'>"
            lsImg = ThemedImage.GetImageTag("ok.svg", Me, GetLabel("ok"), "class='iconImage'")
            _selectedactions.Add(lsHref & vsName & "</a><br>")
        Else
            lsHref = "<a href='javascript:AddAction(" & vlID & "," & vlSubObjectID & ");'>"
            lsImg = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        End If       
        If Not _actions.ContainsKey(vsCategory) Then _actions(vsCategory) = New List(Of String)
        _actions(vsCategory).Add(lsHref & lsImg & "&nbsp;" & vsName & "</a><br>")
    End Sub

    Private Function ActionFound(ByVal viActionID As ACL.ACL_Access.Access_Level, ByVal vlSubObjectID As Integer) As Boolean
        Return mcolProfileActions.Cast(Of ACL.ACL_ACTION_LIST.ACTIONInfo).Any(Function(x) x.ID = viActionID AndAlso x.Sub_Object_ID = vlSubObjectID)
    End Function

    Protected Overrides Sub OnLoad(ByVal e As System.EventArgs)
        MyBase.OnLoad(e)
        If Request.Form.Count <> 0 Then
            msID = Convert.ToInt32(Request.Form("profileid"))
        Else
            msID = QueryStringParser.GetInt("profileid")
        End If

        'lnkSave.Text = GetLabel("save")
        lblName.Text = GetLabel("name")
        lblHeader4.Text = GetLabel("edit")
        'lnkClose.Text = GetLabel("close")
        Page.Title = GetLabel("add")

        If msID > 0 Then
            loProfile = ACL.ACL_PROFILE.GetProfile(msID)
            If loProfile.isSystem Then
                GotoErrorPage()
            End If
        End If

        DoActions()

        If msID > 0 Then
            LoadScreen()
        End If
    End Sub

End Class
