Imports System.Xml

Partial Class DM_Catalog_Search

    Inherits BasePage
    Protected Modal As Boolean = False


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim llListID As Integer = 0
        Dim llPropID As Integer = 0
        Dim llRowID As Integer = 0
        Dim llObjID As Integer = 0
        Dim lbRefreshOnChange As Boolean = False
        Dim lbMultiSelect As Boolean = False
        Dim loProp As Arco.Doma.Library.baseObjects.DM_PROPERTY
        Dim _ParentTextBox As String = ""
     
        If Not Request.QueryString("field") Is Nothing Then
            _ParentTextBox = Request.QueryString("field")
        End If
        llPropID = QueryStringParser.GetInt("PROP_ID")
        llObjID = QueryStringParser.GetInt("DM_OBJECT_ID")
        llRowID = QueryStringParser.GetInt("ROW_ID")

        Modal = QueryStringParser.GetBoolean("modal")

        If llPropID > 0 Then
            Try
                loProp = Arco.Doma.Library.baseObjects.DM_PROPERTY.GetPROPERTY(llPropID)
                If loProp.ID = llPropID Then

                    Dim lcolLabels As Arco.Doma.Library.Globalisation.LABELList
                    If loProp.ProcID > 0 Then
                        lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetProcedureItemsLabelList(loProp.ProcID, Me.EnableIISCaching)
                    Else
                        lcolLabels = Arco.Doma.Library.Globalisation.LABELList.GetCategoryItemsLabelList(loProp.CatID, Me.EnableIISCaching)
                    End If
                    Page.Title = lcolLabels.GetObjectLabel(loProp.ID, "Property", Arco.Security.BusinessIdentity.CurrentIdentity.Language, loProp.Name)

                    llListID = loProp.ListID
                    lbRefreshOnChange = loProp.PROP_ROC
                    lbMultiSelect = (loProp.VALUE_NUMBER = 1)
                End If

            Catch ex As Exception
           
            End Try
        Else
            llListID = Val(Request("LIST_ID"))
        End If

        ' Overwrite multiselect (for example, when this form is used for entering relationships)
        If Not Request("multiselect") Is Nothing Then                  
            If Request("multiselect").ToLower = "n" Then
                lbMultiSelect = False
            ElseIf Request("multiselect").ToLower = "y" Then
                lbMultiSelect = True
            End If
        End If


        If Not Page.IsPostBack Then
            ' Treeview.
            If _ParentTextBox = "" Then
                radPaneLeftContent.ContentUrl = "DM_Catalog_Tree.aspx?id=" & llListID & "&PROP_ID=" & llPropID & "&DM_OBJECT_ID=" & llObjID & "&codeindesc=Y&autovalidate=" & Request.QueryString("autovalidate")
            Else
                radPaneLeftContent.ContentUrl = "DM_Catalog_Tree.aspx?id=" & llListID & "&PROP_ID=" & llPropID & "&DM_OBJECT_ID=" & llObjID & "&codeindesc=N&autovalidate=" & Request.QueryString("autovalidate")
            End If
            ' Add item / details.
            radPaneRightContent.ContentUrl = "about:blank"
        End If

        ' Left pane (treeview)
        Dim sb1 As New StringBuilder
        sb1.AppendLine("function RefreshLeftPane() {")
        sb1.AppendLine("  ShowLeftPane('DM_Catalog_Tree.aspx?id=" & llListID & "&PROP_ID=" & llPropID & "&DM_OBJECT_ID=" & llObjID & "');")
        sb1.AppendLine("}")

        ' Right pane (details).
        sb1.AppendLine("function RefreshRightPane(itemid) {")
        sb1.AppendLine("  ShowRightPane('DM_Catalog_Detail.aspx?id=' + itemid);")
        sb1.AppendLine("}")

        ' Right pane (new).
        sb1.AppendLine("function NewRightPane(listid) {")
        sb1.AppendLine("  ShowRightPane('DM_Catalog_NewItem.aspx?id=' + listid);")
        sb1.AppendLine("}")

        sb1.AppendLine("function GetSelectedCaption(){")
        If _ParentTextBox = "" Then
            sb1.AppendLine("return GetOpener().oProp_" & llPropID & "_" & llRowID & ".GetCaption();")
        Else
            sb1.AppendLine("return GetOpener().document.getElementById('" & _ParentTextBox & "').value;")
        End If
        sb1.AppendLine("}")

        'clear value 
        sb1.AppendLine("function ClearValue(){")
        'sb1.AppendLine("  alert('hi');")
        sb1.AppendLine("  GetOpener().ClearValue" & llPropID & "_" & llRowID & "();")
        'sb1.AppendLine("  GetOpener().document.getElementById('" & _ParentTextBox & "').value = '';")

        sb1.AppendLine("}")


        ' Select.
        sb1.AppendLine("function Select(val,desc) {")
        If _ParentTextBox = "" Then
            ' From detail window.
            sb1.AppendLine("  GetOpener().AddValue" & llPropID & "_" & llRowID & "(val,desc);")            
        Else
            If Not lbMultiSelect Then
                ' Always singleselect for relationship selection.
                sb1.AppendLine("     GetOpener().document.getElementById('" & _ParentTextBox & "').value = '=""'  + desc + '""';")              
            Else
                ' Always multiselect for search selection.
                sb1.AppendLine("     var orgVal = GetOpener().document.getElementById('" & _ParentTextBox & "').value;")
                sb1.AppendLine("     if(orgVal!='')")
                sb1.AppendLine("     {")
                sb1.AppendLine("         orgVal= orgVal = orgVal + ' OR ' ;")
                sb1.AppendLine("     }")
                sb1.AppendLine("     GetOpener().document.getElementById('" & _ParentTextBox & "').value = orgVal + '=""'  + desc + '""';")
            End If
        End If
        If Not lbMultiSelect Then
            If lbRefreshOnChange = True Then
                sb1.AppendLine(" SaveClose();")
            Else
                sb1.AppendLine("  Close();")
            End If
        End If

        sb1.AppendLine("}")


        ' Select.
        'sb1.AppendLine("function SelectAndClose(val,desc) {")      
        'If _ParentTextBox = "" Then
        '    ' From detail window.
        '    sb1.AppendLine("  GetOpener().AddValue" & llPropID & "_" & llRowID & "(val,desc);")
        '    If lbRefreshOnChange = True Then
        '        sb1.AppendLine(" SaveClose();")
        '    Else
        '        sb1.AppendLine("  Close();")
        '    End If
        'Else
        '    If Not lbMultiSelect Then
        '        ' Always singleselect for relationship selection.
        '        sb1.AppendLine("     GetOpener().document.getElementById('" & _ParentTextBox & "').value = val + ':' + desc;")
        '        sb1.AppendLine("     window.close();")
        '    Else
        '        ' Always multiselect for search selection.
        '        sb1.AppendLine("     var orgVal = MainPage().GetContentWindow().document.getElementById('" & _ParentTextBox & "').value;")
        '        sb1.AppendLine("     if(orgVal!='')")
        '        sb1.AppendLine("     {")
        '        sb1.AppendLine("         orgVal= orgVal = orgVal + ' OR ' ;")
        '        sb1.AppendLine("     }")
        '        sb1.AppendLine("     GetOpener().document.getElementById('" & _ParentTextBox & "').value = val + ':' + desc + '""';")
        '        sb1.AppendLine("     window.close();")
        '    End If
        'End If
        ' sb1.AppendLine("}")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "LP1", sb1.ToString, True)


    End Sub

End Class
