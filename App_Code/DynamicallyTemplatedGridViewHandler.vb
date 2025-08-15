Public Class DynamicallyTemplatedGridViewHandler
    Implements ITemplate

#Region " Data members "

    Private ItemType As ListItemType
    Private FieldName As String
    Private InfoType As String
    Private Dropdownlist As DropDownList
    Private URL As String
    Private mbAjaxCheckbox As Boolean
    Private maxlen As Int32 = 0
#End Region



#Region " Constructor "
    Public Sub New(ByVal item_type As ListItemType, _
                  ByVal field_name As String, _
                  ByVal info_type As String)
        ItemType = item_type
        FieldName = field_name
        InfoType = info_type
    End Sub
    Public Sub New(ByVal item_type As ListItemType, _
                  ByVal field_name As String, _
                  ByVal info_type As String, ByVal viMaxLen As Int32)
        ItemType = item_type
        FieldName = field_name
        InfoType = info_type
        maxlen = viMaxLen
    End Sub
    Public Sub New(ByVal item_type As ListItemType, _
                   ByVal field_name As String, _
                   ByVal info_type As String, _
                   ByVal vbAjaxEnableCheckbox As Boolean)
        ItemType = item_type
        FieldName = field_name
        InfoType = info_type
        mbAjaxCheckbox = vbAjaxEnableCheckbox
    End Sub

    Public Sub New(ByVal item_type As ListItemType, _
                   ByVal field_name As String, _
                   ByVal info_type As String, _
                   ByVal dropdown_list As DropDownList)
        ItemType = item_type
        FieldName = field_name
        InfoType = info_type
        Dropdownlist = dropdown_list
    End Sub

    Public Sub New(ByVal item_type As ListItemType, _
                   ByVal field_name As String, _
                   ByVal info_type As String, _
                   ByVal hyper_link As String)
        ItemType = item_type
        FieldName = field_name
        InfoType = info_type
        URL = hyper_link
    End Sub

#End Region

#Region " Methods "

    Public Sub InstantiateIn(ByVal Container As System.Web.UI.Control) Implements ITemplate.InstantiateIn

        Select ItemType

            Case ListItemType.Header
                Select Case InfoType.ToUpper
                    Case "STRINGHEAD"
                        ' for other 'non-command' i.e. the key and non key fields, bind textboxes with corresponding field values 
                        Dim field_txtboxF As New TextBox()
                        field_txtboxF.ID = FieldName
                        field_txtboxF.Text = [String].Empty
                        field_txtboxF.Width = Unit.Pixel(50)
                        field_txtboxF.BackColor = Color.White
                        If maxlen > 0 Then
                            field_txtboxF.MaxLength = maxlen
                        End If
                        Container.Controls.Add(field_txtboxF)
                        Exit Select
                    Case "ADDLINKHEAD"
                        Dim add_button As New LinkButton()
                        add_button.ID = "add_button"
                        add_button.Text = GetLabel("add")
                        add_button.CssClass = "ButtonLink"
                        add_button.CommandName = "Insert"
                        add_button.ToolTip = GetLabel("add")
                        AddHandler add_button.Click, AddressOf insert_button_Click
                        Container.Controls.Add(add_button)
                        Exit Select
                    Case "DROPDOWNHEAD"
                        Dim field_dropdownList As New DropDownList
                        If field_dropdownList IsNot Nothing Then
                            field_dropdownList = Dropdownlist
                        End If
                        field_dropdownList.ID = FieldName
                        'field_dropdownList.BackColor = Color.PowderBlue
                        Container.Controls.Add(field_dropdownList)
                        Exit Select
                   
                    Case Else

                        Dim header_ltrl As New Literal()
                        header_ltrl.Text = "<b>" + FieldName + "</b>"
                        Container.Controls.Add(header_ltrl)

                        Exit Select
                End Select
                Exit Select


            Case ListItemType.Item
                Select Case InfoType.ToUpper
                    Case "EDITLINK"
                        Dim html As New HtmlGenericControl
                        Dim edit_button As New LinkButton()
                        edit_button.ID = "edit_button"
                        edit_button.Text = GetLabel("edit")
                        edit_button.CssClass = "ButtonLink"
                        edit_button.CommandName = "Edit"

                        AddHandler edit_button.Click, AddressOf edit_button_Click
                        edit_button.ToolTip = GetLabel("edit")
                        html.InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;"
                        Container.Controls.Add(html)
                        Container.Controls.Add(edit_button)
                        html = New HtmlGenericControl
                        html.InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;"

                        Container.Controls.Add(html)

                        Exit Select
                    Case "DETAILSLINK"
                        Dim html As New HtmlGenericControl
                        Dim edit_button As New LinkButton()
                        edit_button.ID = "details_button"
                        edit_button.Text = GetLabel("details")
                        edit_button.CssClass = "ButtonLink"
                        edit_button.CommandName = "Edit"

                        AddHandler edit_button.Click, AddressOf edit_button_Click
                        edit_button.ToolTip = GetLabel("details")
                        html.InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;"
                        Container.Controls.Add(html)
                        Container.Controls.Add(edit_button)
                        html = New HtmlGenericControl
                        html.InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;"

                        Container.Controls.Add(html)

                        Exit Select
                    Case "DELETELINK"
                        Dim html As New HtmlGenericControl
                        html.InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;"
                        Dim delete_button As New LinkButton()
                        delete_button.ID = "delete_button"
                        delete_button.Text = GetLabel("delete")
                        delete_button.CssClass = "ButtonLink"
                        delete_button.CommandName = "Delete"
                        delete_button.ToolTip = GetLabel("delete")
                        delete_button.OnClientClick = "return confirm('" & GetDecodedLabel("confirmdelete") & "')"
                        Container.Controls.Add(html)
                        Container.Controls.Add(delete_button)
                        html = New HtmlGenericControl
                        html.InnerHtml = "&nbsp;&nbsp;&nbsp;&nbsp;"
                        Container.Controls.Add(html)
                        Exit Select
                    Case "COMMAND"
                        Dim edit_button As New ImageButton()
                        edit_button.ID = "edit_button"
                        edit_button.ImageUrl = "~/images/edit.png" ' ThemedImage.GetUrl("edit.png", Container)
                        edit_button.CssClass = "gridLinks"
                        edit_button.CommandName = "Edit"
                        AddHandler edit_button.Click, AddressOf edit_button_Click
                        edit_button.ToolTip = "Edit"
                        Container.Controls.Add(edit_button)

                        Dim delete_button As New ImageButton()
                        delete_button.ID = "delete_button"
                        delete_button.ImageUrl = ThemedImage.GetUrl("delete.svg", Container)
                        delete_button.CssClass = "gridLinks"
                        delete_button.CommandName = "Delete"
                        delete_button.ToolTip = GetLabel("delete")
                        delete_button.OnClientClick = "return confirm('" & GetDecodedLabel("confirmdelete") & "')"
                        Container.Controls.Add(delete_button)
                        Exit Select
                    Case "EDITONLY"
                        Dim edit_button As New ImageButton()
                        edit_button.ID = "edit_button"
                        edit_button.ImageUrl = "~/images/edit.png" ' ThemedImage.GetUrl("edit.png", Container)
                        'edit_button.Text = "Edit"
                        edit_button.CommandName = "Edit"
                        AddHandler edit_button.Click, AddressOf edit_button_Click
                        edit_button.ToolTip = GetLabel("edit")
                        Container.Controls.Add(edit_button)
                        Exit Select
                    Case "DELETEONLY"
                        Dim delete_button As New ImageButton()
                        delete_button.ID = "delete_button"
                        delete_button.ImageUrl = "~/images/delete.svg" ' ThemedImage.GetUrl("delete.png", Container)
                        delete_button.CommandName = "Delete"
                        'delete_button.Text = "Delete"
                        delete_button.ToolTip = GetLabel("delete")
                        delete_button.OnClientClick = "return confirm('" & GetDecodedLabel("confirmdelete") & "')"
                        Container.Controls.Add(delete_button)
                        Exit Select
                    Case "CHECKBOX"
                        Dim field_chckbox As New CheckBox
                        field_chckbox.ID = FieldName
                        If Not mbAjaxCheckbox Then
                            field_chckbox.Enabled = False
                        Else
                            field_chckbox.Checked = False
                            field_chckbox.Enabled = True
                        End If
                        AddHandler field_chckbox.DataBinding, AddressOf OnDataBinding
                        'we will bind it later through 'OnDataBinding' event 
                        Container.Controls.Add(field_chckbox)
                        Exit Select
                        
                    Case "STRING"
                        Dim field_lbl As New Label()
                        field_lbl.ID = FieldName
                        field_lbl.Text = [String].Empty
                        AddHandler field_lbl.DataBinding, AddressOf OnDataBinding
                        'we will bind it later through 'OnDataBinding' event 
                        Container.Controls.Add(field_lbl)
                        Exit Select
                    Case "HYPERLINK"
                        Dim filed_hl As New HyperLink()
                        filed_hl.ID = FieldName
                        filed_hl.Target = "_blank"
                        AddHandler filed_hl.DataBinding, AddressOf OnDataBinding
                        'we will bind it later through 'OnDataBinding' event 
                        Container.Controls.Add(filed_hl)
                        'passing through the url:
                        Dim field_lbl As New Label()
                        field_lbl.ID = URL
                        AddHandler field_lbl.DataBinding, AddressOf OnDataBinding
                        Container.Controls.Add(field_lbl)
                        Exit Select
                End Select
                Exit Select

            Case ListItemType.EditItem
                Select Case InfoType.ToUpper
                    Case "COMMAND"
                        Dim update_button As New ImageButton()
                        update_button.ID = "update_button"
                        update_button.CommandName = "Update"
                        update_button.ImageUrl = ThemedImage.GetUrl("accept.png", Container)
                        If CInt(New Page().Session("InsertFlag")) = 1 Then
                            update_button.ToolTip = "Add"
                        Else
                            update_button.ToolTip = "Update"
                        End If
                        update_button.OnClientClick = "return confirm('" & GetDecodedLabel("confirmupdate") & "')"
                        Container.Controls.Add(update_button)
                        Dim cancel_button As New ImageButton()
                        cancel_button.ImageUrl = "~/images/cancel.png"
                        cancel_button.ID = "cancel_button"
                        cancel_button.CommandName = "Cancel"
                        cancel_button.ToolTip = "Cancel"
                        Container.Controls.Add(cancel_button)
                        Exit Select
                    Case "ARROW"
                        Dim update_button2 As New ImageButton()
                        update_button2.ID = "update_button2"
                        update_button2.CommandName = "Update"
                        update_button2.ImageUrl = "~/images/nextpage.png"
                        update_button2.OnClientClick = "return confirm('" & GetDecodedLabel("confirmupdate") & "')"
                        Container.Controls.Add(update_button2)
                        Exit Select
                    Case "CHECKBOX"
                        Dim field_chckbox As New CheckBox
                        field_chckbox.ID = FieldName
                        'field_chckbox.BackColor = Color.PowderBlue
                        If CInt(New Page().Session("InsertFlag")) = 0 Then
                            AddHandler field_chckbox.DataBinding, AddressOf OnDataBinding
                        End If
                        Container.Controls.Add(field_chckbox)
                        Exit Select
                    Case "STRING"
                        ' for other 'non-command' i.e. the key and non key fields, bind textboxes with corresponding field values 
                        Dim field_txtbox As New TextBox()
                        field_txtbox.ID = FieldName
                        field_txtbox.Text = [String].Empty
                        field_txtbox.Width = Unit.Pixel(50)
                        field_txtbox.BackColor = Color.PaleGoldenrod
                        field_txtbox.BorderColor = Color.DarkBlue
                        If maxlen > 0 Then
                            field_txtbox.MaxLength = maxlen
                        End If
                        ' if Inert is intended no need to bind it with text..keep them empty 
                        If CInt(New Page().Session("InsertFlag")) = 0 Then
                            AddHandler field_txtbox.DataBinding, AddressOf OnDataBinding
                        End If
                        Container.Controls.Add(field_txtbox)
                        Exit Select
                    Case "DROPDOWN"
                        Dim field_dropdownList As New DropDownList
                        If Dropdownlist IsNot Nothing Then
                            field_dropdownList = Dropdownlist
                        End If
                        field_dropdownList.ID = FieldName
                        'field_dropdownList.BackColor = Color.PowderBlue
                        AddHandler field_dropdownList.DataBinding, AddressOf OnDataBinding
                        Container.Controls.Add(field_dropdownList)
                        Exit Select
                End Select

            Case ListItemType.Footer
                Select Case InfoType.ToUpper
                    Case "COMMAND"
                        Dim insert_button As New ImageButton()
                        insert_button.ID = "insert_button"
                        insert_button.ImageUrl = ThemedImage.GetUrl("accept.png", Container)
                        insert_button.CommandName = "Insert"
                        insert_button.ToolTip = "Insert"
                        AddHandler insert_button.Click, AddressOf insert_button_Click
                        Container.Controls.Add(insert_button)
                        Exit Select
                    Case "ARROW"
                        Dim insert_button2 As New ImageButton()
                        insert_button2.ID = "insert_button2"
                        insert_button2.ImageUrl = "~/images/nextpage.png"
                        insert_button2.CommandName = "Insert"
                        AddHandler insert_button2.Click, AddressOf insert_button_Click
                        Container.Controls.Add(insert_button2)
                        Exit Select
                    Case "ADD"
                        Dim add_button As New ImageButton()
                        add_button.ID = "add_button"
                        add_button.ImageUrl = "~/images/new.png"
                        add_button.CommandName = "Insert"
                        AddHandler add_button.Click, AddressOf insert_button_Click
                        Container.Controls.Add(add_button)
                        Exit Select
                    Case "ADDLINK"
                        Dim add_button As New LinkButton()
                        add_button.ID = "add_button"
                        add_button.Text = "Add"
                        add_button.CssClass = "ButtonLink"
                        add_button.CommandName = "Insert"
                        AddHandler add_button.Click, AddressOf insert_button_Click
                        Container.Controls.Add(add_button)
                        Exit Select
                    Case "CHECKBOX"
                        Dim field_chckboxF As New CheckBox
                        field_chckboxF.ID = FieldName
                        field_chckboxF.Checked = True
                        'field_chckboxF.BackColor = Color.PowderBlue
                        Container.Controls.Add(field_chckboxF)
                        Exit Select
                    Case "STRING"
                        ' for other 'non-command' i.e. the key and non key fields, bind textboxes with corresponding field values 
                        Dim field_txtboxF As New TextBox()
                        field_txtboxF.ID = FieldName
                        field_txtboxF.Text = [String].Empty
                        field_txtboxF.Width = Unit.Pixel(50)
                        field_txtboxF.BackColor = Color.PaleGoldenrod
                        field_txtboxF.BorderColor = Color.DarkBlue
                        'If maxlen > 0 Then
                        '    field_txtboxF.MaxLength = maxlen
                        'End If
                        Container.Controls.Add(field_txtboxF)
                        Exit Select
                    Case "STRING2"
                        ' for other 'non-command' i.e. the key and non key fields, bind textboxes with corresponding field values 
                        Dim field_txtboxF As New TextBox()
                        field_txtboxF.ID = FieldName
                        field_txtboxF.Text = [String].Empty
                        'field_txtboxF.Visible = False
                        field_txtboxF.Width = Unit.Pixel(50)
                        field_txtboxF.BackColor = Color.White
                        field_txtboxF.BorderColor = Color.White
                        field_txtboxF.Enabled = False
                        'If maxlen > 0 Then
                        'field_txtboxF.MaxLength = maxlen
                        'End If
                        Container.Controls.Add(field_txtboxF)
                        Exit Select
                    Case "DROPDOWN"
                        Dim field_dropdownList As New DropDownList
                        If field_dropdownList IsNot Nothing Then
                            field_dropdownList = Dropdownlist
                        End If
                        field_dropdownList.ID = FieldName
                        Container.Controls.Add(field_dropdownList)
                        Exit Select
                End Select
                Exit Select

        End Select

    End Sub

#End Region

#Region " Event Handlers "

    ' Set InsertFlag ON (for an insert).
    Protected Sub insert_button_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim page As Page = New Page()
        page.Session("InsertFlag") = 1
    End Sub

    ' Set InsertFlag OFF (for an update).
    Protected Sub edit_button_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim page As Page = New Page()
        page.Session("InsertFlag") = 0
    End Sub

    Private Sub OnDataBinding(ByVal sender As Object, ByVal e As EventArgs)

        Dim bound_value_obj As Object = Nothing
        Dim ctrl As Control = DirectCast(sender, Control)

        Dim data_item_container As IDataItemContainer = DirectCast(ctrl.NamingContainer, IDataItemContainer)
        bound_value_obj = DataBinder.Eval(data_item_container.DataItem, FieldName)

        Select Case ItemType
            Case ListItemType.Item

                Select Case ctrl.GetType().Name.ToUpper

                    Case "CHECKBOX"
                        Dim field_chckbox As CheckBox = DirectCast(sender, CheckBox)
                        field_chckbox.Checked = Convert.ToBoolean(bound_value_obj)
                        If mbAjaxCheckbox Then

                            field_chckbox.ID = "CHK_" & DataBinder.Eval(data_item_container.DataItem, "CATID") & "_" & DataBinder.Eval(data_item_container.DataItem, "ID")
                            field_chckbox.Attributes.Add("onclick", "switchCategorySelection(" & DataBinder.Eval(data_item_container.DataItem, "ID") & "," & _
                                                                                                 DataBinder.Eval(data_item_container.DataItem, "CATID") & "," & _
                                                                                                 "this.checked)")

                        End If
                        Exit Select

                    Case "LABEL"
                        Dim field_ltrl As Label = DirectCast(sender, Label)
                        field_ltrl.Text = bound_value_obj.ToString()
                        Exit Select

                    Case "HYPERLINK"
                        Dim field_ltrl As HyperLink = DirectCast(sender, HyperLink)
                        Dim lsURL As String = DataBinder.Eval(data_item_container.DataItem, "URL")
                        If lsURL.Length > 0 Then
                            field_ltrl.Text = "" ' DataBinder.Eval(data_item_container.DataItem, "NAME")
                            field_ltrl.ImageUrl = "../images/links.gif"
                            field_ltrl.NavigateUrl = lsURL
                        Else
                            field_ltrl.Text = ""
                            field_ltrl.NavigateUrl = ""
                        End If
                        Exit Select
                    Case "COMMANDLINK"
                        Dim edit_button As LinkButton = DirectCast(sender, LinkButton)
                        Dim ItemID As String = DataBinder.Eval(data_item_container.DataItem, "ID")
                        If ItemID > 0 Then
                            edit_button.OnClientClick = "RefreshRightPaneLID3('" & ItemID & "','t0_" & ItemID & "');"
                        End If
                End Select

            Case ListItemType.EditItem
                Select Case ctrl.GetType().Name.ToUpper
                    Case "CHECKBOX"
                        Dim field_chckbox As CheckBox = DirectCast(sender, CheckBox)
                        field_chckbox.Checked = Convert.ToBoolean(bound_value_obj)
                        Exit Select

                    Case "TEXTBOX"
                        Dim field_txtbox As TextBox = DirectCast(sender, TextBox)
                        field_txtbox.Text = bound_value_obj.ToString()
                        Exit Select

                    Case "DROPDOWNLIST"
                        Dim field_dropdown As DropDownList = DirectCast(sender, DropDownList)
                        'Try
                        field_dropdown.Items.FindByText(bound_value_obj.ToString()).Selected = True
                        'Catch ex As Exception
                        'End Try
                        Exit Select

                End Select

            Case ListItemType.Footer
                Select Case ctrl.GetType().Name.ToUpper
                    Case "CHECKBOX"
                        Dim field_chckbox As CheckBox = DirectCast(sender, CheckBox)
                        field_chckbox.Checked = Convert.ToBoolean(bound_value_obj)
                        Exit Select

                    Case "TEXTBOX"
                        'Dim field_txtbox As TextBox = DirectCast(sender, TextBox)
                        'field_txtbox.Text = bound_value_obj.ToString()
                        Dim field_txtbox As TextBox = New TextBox()
                        If maxlen > 0 Then
                            field_txtbox.MaxLength = maxlen
                        End If
                        Exit Select

                    Case "DROPDOWNLIST"
                        Dim field_dropdown As DropDownList = DirectCast(sender, DropDownList)
                        'field_dropdown.Items.FindByText(bound_value_obj.ToString()).Selected = True
                        Exit Select

                End Select
        End Select

    End Sub

#End Region

    Public Function GetLabel(ByVal vsLabel As String) As String

        Dim lsNewLabel As String = "CM_" & vsLabel.Replace(" ", "_")

        Try
            lsNewLabel = Arco.Web.ResourceManager.GetString(lsNewLabel.ToLower, Arco.Security.BusinessIdentity.CurrentIdentity.Language, False)
        Catch ex As Exception
            lsNewLabel = ""
        End Try

        If lsNewLabel.Length = 0 Then
            Try
                lsNewLabel = Arco.Web.ResourceManager.GetString(vsLabel.ToLower, Arco.Security.BusinessIdentity.CurrentIdentity.Language, False)
            Catch ex As Exception
                lsNewLabel = ""
            End Try
        End If

        If lsNewLabel.Length = 0 Then
            lsNewLabel = vsLabel
        End If

        Return lsNewLabel

    End Function

    Protected Function GetDecodedLabel(ByVal vsLabel As String) As String

        Dim lsNewLabel As String = vsLabel

        Try
            lsNewLabel = Arco.Web.ResourceManager.GetString(lsNewLabel.ToLower, Arco.Security.BusinessIdentity.CurrentIdentity.Language, False)
        Catch ex As Exception
            lsNewLabel = ""
        End Try

        If lsNewLabel.Length = 0 Then
            Try
                lsNewLabel = Arco.Web.ResourceManager.GetString(lsNewLabel.ToLower, Arco.Security.BusinessIdentity.CurrentIdentity.Language, False)
            Catch ex As Exception
                lsNewLabel = ""
            End Try
        End If

        If lsNewLabel.Length = 0 Then
            lsNewLabel = vsLabel
        End If

        Return Arco.Doma.WebControls.EncodingUtils.EncodeJsString(lsNewLabel, False)

    End Function
End Class
