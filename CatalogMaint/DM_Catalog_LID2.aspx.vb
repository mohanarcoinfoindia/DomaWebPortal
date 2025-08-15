
Partial Class DM_Catalog_LID2
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim lsTab As String = ""
        Dim lsSelectedIDs As String = ""
        Dim llListID As Integer = 0
        Dim llItemID As Integer = 0
        Dim rand As New Random
        If Not Page.IsPostBack Then
            lsTab = QueryStringParser.GetString("tab")
            llListID = QueryStringParser.GetInt("listid")
            llItemID = QueryStringParser.GetInt("itemid")
            lsSelectedIDs = QueryStringParser.GetString("selectedids")
            radPaneContentTop.ContentUrl = "DM_Catalog_listitemdetail.aspx?id=" & llItemID & "&selectedids=" & lsSelectedIDs & "&rnd_str=" & rand.Next
            Select Case lsTab.ToUpper
                Case "N"
                    ' Edit/Open
                    radPaneContentTop.ContentUrl = "DM_Catalog_listitemdetail.aspx?id=" & llItemID & "&selectedids=" & lsSelectedIDs & "&rnd_str=" & rand.Next

                Case "C"
                    ' Add Category
                    radPaneContentTop.ContentUrl = "DM_Catalog_listaddcategory.aspx?listid=" & llListID & "&rnd_str=" & rand.Next

                Case Else
                    ' Add Node/ Children
                    ' Narrow terms.
                    radPaneContentTop.ContentUrl = "DM_Catalog_listitemdetail.aspx?listid=" & llListID & "&parent=" & llItemID & "&selectedids=" & lsSelectedIDs & "&rnd_str=" & rand.Next
            End Select
        End If

        ' Left pane. DM_Catalog_LID2 is always opened as right pane in DM_Catalog_LI.
        Dim sb1 As New StringBuilder
        sb1.AppendLine("function RefreshLeftPane(listid,itemid,selectedids)")
        sb1.AppendLine("{")
        sb1.AppendLine(" parent.RefreshLeftPane(listid,itemid,selectedids);")
        sb1.AppendLine(" }")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging1", sb1.ToString, True)

        Dim sb2 As New StringBuilder
        sb2.AppendLine("function RefreshRightPane(listid)")
        sb2.AppendLine("{")
        sb2.AppendLine(" parent.RefreshRightPane(listid);")
        sb2.AppendLine(" }")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging2", sb2.ToString, True)

        Dim sb4 As New StringBuilder
        sb4.AppendLine("function AddNewItemLID2(listid,itemid,strSids)")
        sb4.AppendLine("{")
        sb4.AppendLine(" parent.RefreshRightPaneLID2(listid,itemid,'',strSids);")
        sb4.AppendLine(" }")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging3", sb4.ToString, True)

        Dim sb5 As New StringBuilder
        sb5.AppendLine("function RefreshRightPaneLID2(listid,itemid,tab,strSids)")
        sb5.AppendLine("{")
        sb5.AppendLine(" parent.RefreshRightPaneLID2(listid,itemid,tab,strSids);")
        sb5.AppendLine(" }")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging3", sb5.ToString, True)
    End Sub

End Class
