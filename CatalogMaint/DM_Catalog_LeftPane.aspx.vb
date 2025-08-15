Imports Arco.Doma.Library

Partial Class DM_Catalog_LeftPane
    Inherits BasePage

    Dim RowCount As Integer
    Private ltDT As DataTable

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim rand As New Random
        If Not Me.IsPostBack Then
            FillListTable()
        End If

        If ltDT.Rows.Count <> 0 Then
            gridRes.DataSource = ltDT
            gridRes.DataBind()
        Else
            lblMessage.Text = GetLabel("noresultsfound")
        End If

        Dim sb2 As New StringBuilder
        sb2.AppendLine("function ShowDetail(id,listtype)")
        sb2.AppendLine("{")
        sb2.AppendLine(" if(listtype=='UDCLIST' || listtype=='THESAURUS' ||listtype=='CONCEPTTREE' )")
        sb2.AppendLine(" { ")
        sb2.AppendLine(" location.href = 'DM_Catalog_LI.aspx?listid=' + id +'&rnd_str=" & rand.Next.ToString() & "';")
        sb2.AppendLine(" } ")
        sb2.AppendLine(" else {")
        sb2.AppendLine(" location.href = 'DM_Catalog_Listitems.aspx?list_id=' + id + '&rnd_str=" & rand.Next.ToString() & "';")
        sb2.AppendLine(" }")
        sb2.AppendLine(" }")
        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FilePaging", sb2.ToString, True)
    End Sub

    Private Sub FillListTable()

        Dim loLists As Lists.ListList
        Dim lsStatus As String = ""
        Dim lsLock As String = ""
        Dim lsLanguages As String = ""
        Dim loDataCol As DataColumn

        ltDT = New System.Data.DataTable

        loDataCol = ltDT.Columns.Add("NAME")
        loDataCol.Caption = GetLabel("cm_name")
        loDataCol = ltDT.Columns.Add("TYPE")
        loDataCol.Caption = GetLabel("cm_Type")
        loDataCol = ltDT.Columns.Add("MULTI-LINGUAL")
        loDataCol.Caption = GetLabel("cm_Multi-Lingual")
        loDataCol = ltDT.Columns.Add("LIST_CATDEPENDANT")
        loDataCol.Caption = GetLabel("cm_Category_Dependant")
        loDataCol = ltDT.Columns.Add("LINKED-LIST")
        loDataCol.Caption = GetLabel("cm_linked_List")
        loDataCol = ltDT.Columns.Add("ID")
        loDataCol = ltDT.Columns.Add("LOCKEDBY")

        loLists = Lists.ListList.GetListList("")
        For Each loList As Lists.ListList.ListInfo In loLists
            If loList.LIST_STATUS = Lists.ListList.ListStatus.Production Then
                lsStatus = GetLabel("enabled")
            Else
                lsStatus = GetLabel("disabled")
            End If
            If loList.LIST_LOCKED = False Then
                lsLock = GetLabel("unlocked")
            Else
                lsLock = GetLabel("locked")
            End If

            ltDT.Rows.Add(New String() {Server.HtmlEncode(loList.Name), loList.LIST_TYPE, loList.MULTI_LINGUAL, loList.LIST_CATDEPENDANT, loList.LINKED_LIST, loList.ID, Arco.Doma.WebControls.ArcoFormatting.FormatUserName(loList.LIST_LOCKED_BY)})
        Next
    

    End Sub


    Private moIntLangs As Globalisation.LanguageList
    Private ReadOnly Property InterfaceLanguages As Globalisation.LanguageList
        Get
            If moIntLangs Is Nothing Then moIntLangs = Globalisation.LanguageList.GetInterfaceLanguageList()
            Return moIntLangs
        End Get
    End Property
    Private moListLangs As Lists.ListLanguageList
    Private ReadOnly Property ListLanguages As Lists.ListLanguageList
        Get
            If moListLangs Is Nothing Then moListLangs = Lists.ListLanguageList.GetListLanguageList(0) 'get all
            Return moListLangs
        End Get
    End Property
    Public Function GetLanguageList(ByVal vlListID As Integer, ByVal vbMultiLang As Boolean) As String
        If vbMultiLang Then
            Dim strLanguageList As StringBuilder = New StringBuilder
            For Each loListLanguage As Lists.ListLanguageList.ListLanguageInfo In ListLanguages
                If loListLanguage.LIST_ID = vlListID Then                    
                    For Each loLanguage As Globalisation.LanguageList.LanguageInfo In InterfaceLanguages
                        If loListLanguage.LANG_CODE = loLanguage.InterfaceLanguageCode Then
                            If strLanguageList.Length > 0 Then strLanguageList.Append(", ")
                            strLanguageList.Append(GetLabel("lang" & loLanguage.InterfaceLanguageCode))
                            Exit For
                        End If
                    Next
                End If
            Next            
            If strLanguageList.Length > 0 Then
                Return "(" & strLanguageList.ToString & ")"
            Else
                Return ""
            End If
        Else
            Return ""
        End If

    End Function
    Protected Sub gridRes_RowCreated(sender As Object, e As GridViewRowEventArgs) Handles gridRes.RowDataBound

        Dim lblListId As Label = e.Row.FindControl("lblListID")
        Dim lblListType As Label = e.Row.FindControl("lblListType")

        If e.Row.RowType = DataControlRowType.DataRow Then
            e.Row.Attributes.Add("onclick", "javascript:ShowDetail(" + lblListId.Text + ",'" + lblListType.Text + "');")
            e.Row.Attributes.Add("onmouseover", "JavaScript:this.style.cursor='pointer';")

        End If
        RowCount += 1
    End Sub
    Protected Sub TableGridView_Init(sender As Object, e As EventArgs)
        ' gridRes.Columns(0).ItemStyle.Width = Unit.Percentage(100)
        gridRes.Columns(1).ItemStyle.Width = Unit.Pixel(200)
        gridRes.Columns(2).ItemStyle.Width = Unit.Pixel(300)
        gridRes.Columns(3).ItemStyle.Width = Unit.Pixel(200)

    End Sub
   
End Class
