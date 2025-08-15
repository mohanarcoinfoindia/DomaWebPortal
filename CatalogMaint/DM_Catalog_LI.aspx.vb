
Partial Class DM_Catalog_LI
    Inherits BasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim llListID As Integer = QueryStringParser.GetInt("listid")
        Dim llItemID As Integer = QueryStringParser.GetInt("itemid")
        Dim lsTab As String = QueryStringParser.GetString("tab")
        Dim rand As New Random


        radPaneLeftContent.ContentUrl = "DM_Catalog_listitemsT.aspx?id=" & llListID & "&parent=" & llItemID & "&rnd_str=" & rand.Next
        If llItemID = 0 Then
            radPaneRightContent.ContentUrl = "DM_Catalog_listitems.aspx?list_id=" & llListID & "&rnd_str=" & rand.Next
        Else
            radPaneRightContent.ContentUrl = "DM_Catalog_lID2.aspx?listid=" & llListID & "&itemid=" & llItemID & "&tab=" & lsTab & "&rnd_str=" & rand.Next
        End If


        If llItemID > 0 Then
            ' Left pane (treeview, with parent)
            Dim sb1 As New StringBuilder
            sb1.AppendLine("function RefreshLeftPane(listid,itemid,selectedids)")
            sb1.AppendLine("{")
            sb1.AppendLine(" var splitter = $find('" & radSplitterContent.ClientID & "');")
            sb1.AppendLine(" var pane = splitter.GetPaneById('" & radPaneLeftContent.ClientID & "');")
            sb1.AppendLine(" if(pane!=null)")           
            sb1.AppendLine("  pane.set_contentUrl('DM_Catalog_ListitemsT.aspx?id=' + listid + '&parent=' + itemid + '&selectedids=' + selectedids + '&rnd_str=" & rand.Next & "');}")
            sb1.AppendLine("  ShowLeftPane();")
            sb1.AppendLine(" }")
            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging1", sb1.ToString, True)
        Else
            ' Left pane (treeview, without parent)
            Dim sb1 As New StringBuilder
            sb1.AppendLine("function RefreshLeftPane(listid,itemid,selectedids)")
            sb1.AppendLine("{")
            sb1.AppendLine(" var splitter = $find('" & radSplitterContent.ClientID & "');")
            sb1.AppendLine(" var pane = splitter.GetPaneById('" & radPaneLeftContent.ClientID & "');")
            sb1.AppendLine(" if(pane!=null)")
            sb1.AppendLine(" {")
            sb1.AppendLine("  pane.set_contentUrl('DM_Catalog_ListitemsT.aspx?id=' + listid + '&parent=0&selectedids=' + selectedids + '&rnd_str=" & rand.NextDouble & "');}")
            sb1.AppendLine("  ShowLeftPane();")
            sb1.AppendLine(" }")
            Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging1", sb1.ToString, True)
        End If



        ' Right pane.
        Dim sb2 As New StringBuilder
        sb2.AppendLine("function RefreshRightPane(listid)")
        sb2.AppendLine("{")
        sb2.AppendLine(" var splitter = $find('" & radSplitterContent.ClientID & "');")
        sb2.AppendLine(" var pane = splitter.GetPaneById('" & radPaneRightContent.ClientID & "');")
        sb2.AppendLine(" if(pane!=null)")
        sb2.AppendLine(" {")
        sb2.AppendLine("  pane.set_contentUrl('DM_Catalog_Listitems.aspx?list_id=' + listid + '&rnd_str=" & rand.Next & "');}")
        sb2.AppendLine("  ShowRightPane();")
        sb2.AppendLine(" }")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging2", sb2.ToString, True)

        ' Right pane.
        Dim sb3 As New StringBuilder
        sb3.AppendLine("function RefreshRightPaneLID2(listid,itemid,tab,strSids)")
        sb3.AppendLine("{")
        sb3.AppendLine(" var splitter = $find('" & radSplitterContent.ClientID & "');")
        sb3.AppendLine(" var pane = splitter.GetPaneById('" & radPaneRightContent.UniqueID & "');")
        sb3.AppendLine(" if(pane!=null)")
        sb3.AppendLine(" {")
        sb3.AppendLine("  pane.set_contentUrl('DM_Catalog_LID2.aspx?listid=' + listid + '&itemid=' + itemid + '&tab=' + tab + '&selectedids=' + strSids + '&rnd_str=" & rand.Next & "');}")
        sb3.AppendLine("  ShowRightPane();")
        sb3.AppendLine(" }")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging3", sb3.ToString, True)


        Dim sb4 As New StringBuilder
        sb4.AppendLine("function AddNewItemLID2(listid,itemid,strSids)")
        sb4.AppendLine("{")
        sb4.AppendLine(" var splitter = $find('" & radSplitterContent.ClientID & "');")
        sb4.AppendLine(" var pane = splitter.GetPaneById('" & radPaneRightContent.UniqueID & "');")
        sb4.AppendLine(" if(pane!=null)")
        sb4.AppendLine(" {")
        sb4.AppendLine("  pane.set_contentUrl('DM_Catalog_LID2.aspx?listid=' + listid + '&itemid=' + itemid + '&tab=''&selectedids=' + strSids + '&rnd_str=" & rand.Next & "');}")
        sb4.AppendLine("  ShowRightPane();")
        sb4.AppendLine(" }")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging3", sb4.ToString, True)
    End Sub

End Class
