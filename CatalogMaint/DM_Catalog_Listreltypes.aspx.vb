Imports Arco.Doma.Library
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls

Partial Class DM_Catalog_Listreltypes

    Inherits BasePage

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
    End Sub

#End Region

    Public Shared ltDT As New DataTable()

    Private maLanguages As New ArrayList()
    Private moList As Lists.List = Lists.List.NewList()
    Private Const DENIED_PAGE As String = "../DM_ACL_DENIED.aspx"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        ' Access granted.
        lblRel.Text = GetLabel("Relationships")
        If Not IsPostBack Then
            If ViewState("list_id") = Nothing Then
                ViewState("list_id") = QueryStringParser.GetString("id")
            End If
        End If
        If Val(ViewState("list_id")) > 0 Then
            If GetList(Val(ViewState("list_id"))) = True Then
                CreateTemplatedGridView()
                GetLanguages()
            Else
                Response.Redirect(DENIED_PAGE)
            End If
        Else
            Response.Redirect(DENIED_PAGE)
        End If
     

    End Sub

    Protected Overloads Function GetLabel(ByVal vsLabel As String) As String

        Dim lsNewLabel As String = "CM_" & vsLabel.Replace(" ", "_")

        Try
            lsNewLabel = Arco.Web.ResourceManager.GetString(lsNewLabel.ToLower, Arco.Security.BusinessIdentity.CurrentIdentity.Language)
        Catch ex As Exception
            lsNewLabel = ""
        End Try

        If lsNewLabel.Length = 0 Then
            Try
                lsNewLabel = Arco.Web.ResourceManager.GetString(vsLabel.ToLower, Arco.Security.BusinessIdentity.CurrentIdentity.Language)
            Catch ex As Exception
                lsNewLabel = ""
            End Try
        End If

        If lsNewLabel.Length = 0 Then
            lsNewLabel = vsLabel
        End If

        Return lsNewLabel

    End Function

    Private Function GetList(ByVal vlListID As Integer) As Boolean

        Dim lbOK As Boolean = False

        If vlListID > 0 Then
            moList = Lists.List.GetList(vlListID)
            If moList.ID = vlListID Then
                If moList.Lock Then
                    lblList.Text = moList.LIST_TYPE & " - " & moList.Name
                    lbOK = True
                End If
            End If
        End If

        Return lbOK

    End Function

    Private Sub GetLanguages()

        If maLanguages.Count = 0 Then
            Dim loListLanguages As Lists.ListLanguageList
            Dim loListLanguage As Lists.ListLanguageList.ListLanguageInfo
            loListLanguages = Lists.ListLanguageList.GetListLanguageList(ViewState("list_id"))
            For Each loListLanguage In loListLanguages
                maLanguages.Add(loListLanguage.LANG_CODE)
            Next
        End If

    End Sub

#Region " Error handling "

    Protected Sub Msg_button_Click(ByVal sender As Object, ByVal e As EventArgs)
        MsgPanel.Visible = False
    End Sub

    Private Sub ShowError(ByVal vsText As String)
        msg_lbl.Text = vsText
        MsgPanel.Visible = True
    End Sub

#End Region

#Region " Insert/Update/Delete/Fetch "

    Private Sub InsertRow()

        Dim loRow As GridViewRow = TableGridView.FooterRow
        Dim laParameters As New ArrayList()

        For Each col As DataColumn In ltDT.Columns
            Dim loType As Type = loRow.FindControl(col.ColumnName).GetType
            Select Case loType.FullName
                Case "System.Web.UI.WebControls.TextBox"
                    Dim lsValue As String = DirectCast(loRow.FindControl(col.ColumnName), TextBox).Text
                    laParameters.Add(lsValue)
                Case "System.Web.UI.WebControls.CheckBox"
                    Dim lbValue As Boolean = DirectCast(loRow.FindControl(col.ColumnName), CheckBox).Checked
                    laParameters.Add(lbValue)
                Case "System.Web.UI.WebControls.DropDownList"
                    Dim lsValue As String = DirectCast(loRow.FindControl(col.ColumnName), DropDownList).SelectedValue
                    laParameters.Add(lsValue)
            End Select
        Next

        Dim loListRelType As Lists.ListRelType
        Try
            loListRelType = Lists.ListRelType.NewListRelType()
            loListRelType.LIST_ID = moList.ID
            loListRelType.REL_DESC = laParameters(1)
            loListRelType.Save()
            For j As Integer = 0 To maLanguages.Count - 1
                Dim loListRelTypeLang As Lists.ListRelTypeLang = Lists.ListRelTypeLang.NewListRelTypeLang(loListRelType.REL_ID, maLanguages(j))
                If laParameters(2 + j) <> "" Then
                    loListRelTypeLang.REL_ID = loListRelType.REL_ID
                    loListRelTypeLang.LANG_CODE = maLanguages(j)
                    loListRelTypeLang.REL_DESC = laParameters(2 + j)
                    loListRelTypeLang.Save()
                End If
            Next           
        Catch ex As Exception
            ShowError(ex.ToString)       
        End Try

        CreateTemplatedGridView()
        TableGridView.FooterRow.Visible = True

    End Sub

    Private Sub UpdateRow(ByVal vlRowIndex As Integer)

        Try

            Dim loRow As GridViewRow = TableGridView.Rows(vlRowIndex)
            Dim loListRel As Lists.ListRelType
            Dim laParameters As New ArrayList()

            For Each col As DataColumn In ltDT.Columns
                Dim loType As Type = loRow.FindControl(col.ColumnName).GetType
                Select Case loType.FullName
                    Case "System.Web.UI.WebControls.TextBox"
                        Dim lsValue As String = DirectCast(loRow.FindControl(col.ColumnName), TextBox).Text
                        laParameters.Add(lsValue)
                    Case "System.Web.UI.WebControls.CheckBox"
                        Dim lbValue As Boolean = DirectCast(loRow.FindControl(col.ColumnName), CheckBox).Checked
                        laParameters.Add(lbValue)
                    Case "System.Web.UI.WebControls.DropDownList"
                        Dim lsValue As String = DirectCast(loRow.FindControl(col.ColumnName), DropDownList).SelectedValue
                        laParameters.Add(lsValue)
                End Select
            Next

            loListRel = Lists.ListRelType.GetListRelType(laParameters(0))
            loListRel.REL_ID = laParameters(0)
            loListRel.LIST_ID = moList.ID
            loListRel.REL_DESC = laParameters(1)
            loListRel.Save()

            ' Update/insert all languageitems:
            For i As Integer = 0 To maLanguages.Count - 1
                Dim loListRelLang As Lists.ListRelTypeLang
                Try
                    loListRelLang = Lists.ListRelTypeLang.GetListRelTypeLang(loListRel.REL_ID, maLanguages(i))
                    If loListRelLang.REL_DESC <> laParameters(2 + i) Then
                        If loListRelLang.REL_DESC = "" Then                           
                            loListRelLang = Lists.ListRelTypeLang.NewListRelTypeLang(loListRel.REL_ID, maLanguages(i))
                            loListRelLang.REL_ID = loListRel.REL_ID
                            loListRelLang.LANG_CODE = maLanguages(i)
                        End If
                        If laParameters(2 + i) <> "" Then
                            loListRelLang.REL_DESC = laParameters(2 + i)
                            loListRelLang.Save()
                        Else
                            Lists.ListItemLanguage.DeleteListItemLanguage(loListRel.REL_ID, maLanguages(i))
                        End If
                    End If

                Catch ex As Exception
                    ShowError(ex.ToString)               
                End Try
            Next

            TableGridView.EditIndex = -1
            CreateTemplatedGridView()
            TableGridView.FooterRow.Visible = True

            Session("InsertFlag") = If(CInt(Session("InsertFlag")) = 1, 0, 1)

        Catch ex As Exception
            ShowError(ex.ToString)
        End Try

    End Sub

    Private Sub DeleteRow(ByVal vlRowIndex As Integer)

        Dim loRow As GridViewRow = TableGridView.Rows(vlRowIndex)
        Dim llRelID As Integer = DirectCast(loRow.FindControl(ltDT.Columns(0).ColumnName), Label).Text

        Lists.ListRelType.DeleteListRelType(llRelID)
        CreateTemplatedGridView()
        TableGridView.FooterRow.Visible = True

    End Sub

    Private Sub FetchRows()

        Dim loListRels As Lists.ListRelTypeList
        Dim loListRel As Lists.ListRelTypeList.ListRelTypeInfo
        Dim loCrit As Lists.ListRelTypeList.Criteria = New Lists.ListRelTypeList.Criteria()
        Dim loCol As DataColumn
        Dim loListLanguages As Lists.ListLanguageList
        Dim loListLanguage As Lists.ListLanguageList.ListLanguageInfo
        Dim i As Integer = 0
        Dim j As Integer = 0

        TableGridView.Columns.Clear()

        ltDT = New DataTable()
        loCol = ltDT.Columns.Add("ID")
        loCol = ltDT.Columns.Add("NAME")
        loCol.Caption = GetLabel("Relationship")

        loCrit.LIST_ID = ViewState("list_id")
        loListRels = Lists.ListRelTypeList.GetListRelTypeList(loCrit)

        loListLanguages = Lists.ListLanguageList.GetListLanguageList(ViewState("list_id"))
        Dim laLanguages(loListLanguages.Count) As String
        For Each loListLanguage In loListLanguages
            laLanguages(i) = loListLanguage.LANG_CODE
            ltDT.Columns.Add(loListLanguage.LANG_CODE)
            i = i + 1
        Next

        For Each loListRel In loListRels
            Dim laRowItems(1 + i) As String
            laRowItems(0) = loListRel.REL_ID
            laRowItems(1) = loListRel.REL_DESC
            For j = 1 To i
                
                Try
                    Dim loListRelTypeLang As Lists.ListRelTypeLang = Lists.ListRelTypeLang.GetListRelTypeLang(loListRel.REL_ID, laLanguages(j - 1))
                    laRowItems(1 + j) = loListRelTypeLang.REL_DESC
                Catch ex As Exception

                End Try
               
            Next
            ltDT.Rows.Add(laRowItems)
        Next

        If loListRels.Count = 0 Then
            ' Show dummy
            Dim laRowItems1(1 + i) As String
            laRowItems1(0) = 0
            laRowItems1(1) = GetLabel("Please enter relationship")
            ltDT.Rows.Add(laRowItems1)
        End If

    End Sub

#End Region

#Region " TableGridView "

    Protected Sub TableGridView_RowEditing(ByVal sender As Object, ByVal e As GridViewEditEventArgs)
        TableGridView.EditIndex = e.NewEditIndex
        TableGridView.DataBind()
        Session("SelecetdRowIndex") = e.NewEditIndex
        TableGridView.FooterRow.Visible = False
    End Sub

    Protected Sub TableGridView_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)
        DeleteRow(e.RowIndex)
    End Sub

    Protected Sub TableGridView_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
        UpdateRow(e.RowIndex)
    End Sub

    Protected Sub TableGridView_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)

        CreateTemplatedGridView()
        TableGridView.PageIndex = e.NewPageIndex
        TableGridView.DataBind()
        TableGridView.FooterRow.Visible = True

    End Sub

    Protected Sub TableGridView_RowCancelingEdit(ByVal sender As Object, ByVal e As GridViewCancelEditEventArgs)

        TableGridView.EditIndex = -1
        TableGridView.DataBind()
        Session("SelecetdRowIndex") = -1
        TableGridView.FooterRow.Visible = True

    End Sub

    Protected Sub TableGridView_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles TableGridView.RowCommand
        Select Case e.CommandName
            Case "Insert"
                InsertRow()
        End Select
    End Sub

    Protected Sub TableGridView_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles TableGridView.RowCreated

        If e.Row.RowType = DataControlRowType.DataRow Or _
           e.Row.RowType = DataControlRowType.Header Or _
           e.Row.RowType = DataControlRowType.Footer Then
            e.Row.Cells(1).Style.Add("display", "none")
        End If

    End Sub

    Private Sub CreateTemplatedGridView()

        ' Fill the table which is to bound to the GridView.
        FetchRows()

        ' Add templated fields to the GridView.
        Dim BtnTmpField As New TemplateField()
        BtnTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, "", "Command")
        BtnTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, "", "Command")
        BtnTmpField.EditItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.EditItem, "", "Command")
        BtnTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, "", "Command")
        TableGridView.Columns.Add(BtnTmpField)
        For Each col As DataColumn In ltDT.Columns
            Dim ItemTmpField As New TemplateField()
            ItemTmpField.HeaderTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Header, col.Caption, col.DataType.Name)
            ItemTmpField.ItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Item, col.ColumnName, col.DataType.Name)
            ItemTmpField.EditItemTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.EditItem, col.ColumnName, col.DataType.Name)
            ItemTmpField.FooterTemplate = New DynamicallyTemplatedGridViewHandler(ListItemType.Footer, col.ColumnName, col.DataType.Name)
            TableGridView.Columns.Add(ItemTmpField)
        Next

        ' Bind and display the data.
        If ltDT.Rows.Count() > 0 Then
                TableGridView.DataSource = ltDT
                TableGridView.DataBind()
                TableGridView.FooterRow.Visible = True
            End If

    End Sub

#End Region

End Class
