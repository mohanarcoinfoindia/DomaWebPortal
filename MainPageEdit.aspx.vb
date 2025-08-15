
Imports Arco.Doma.Library.Website

Partial Class MainPageEdit
    Inherits BaseAdminOnlyPage

    Private Property _pageId As Integer
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load


        lblName.Text = GetLabel("name")
        lblDescription.Text = GetLabel("description")
        lblExtraQueryString.Text = "Extra QueryString"
        chkShowTree.Text = GetLabel("pref_showtree")
        chkShowBreadcrumb.Text = GetLabel("currentfolder")
        chkShowGlobalSearch.Text = GetLabel("pref_showglobalsearch")
        lblScreenMode.Text = "Screen mode"

        lblMenu.Text = "Menu"
        lblDefaultAction.Text = "Default Action"
        btnOk.Text = GetLabel("save")
        btnCancelUrl.Text = GetLabel("cancel")

        _pageId = QueryStringParser.GetInt("ID")

        If Not Page.IsPostBack Then


            If _pageId <> 0 Then
                'Title = GetDecodedLabel("edit")
                ShowEntry(MainPage.GetMainPage(_pageId))
            Else
                Title = GetDecodedLabel("new")
                chkShowTree.Checked = True
                chkShowBreadcrumb.Checked = True
                chkShowGlobalSearch.Checked = True
            End If

        End If
    End Sub


    Private Sub ShowEntry(ByVal entry As MainPage)
        Title = entry.Name
        txtName.Text = entry.Name
        txtDescription.Text = entry.Description
        lblUrl.Text = "~/Main.aspx?page=" & txtName.Text

        Dim qrsParser As New QueryStringParser(entry.QueryString)
        ApplyReversedToForm(qrsParser, "hidetree", chkShowTree)
        ApplyReversedToForm(qrsParser, "hidecurrentfolder", chkShowBreadcrumb)
        ApplyReversedToForm(qrsParser, "hideglobalsearch", chkShowGlobalSearch)
        ApplyToForm(qrsParser, "screenmode", txtScreenMode)
        ApplyToForm(qrsParser, "menu", txtMenu)
        ApplyToForm(qrsParser, "defaultaction", txtDefaultAction)

        txtExtraQueryString.Text = qrsParser.ToString()

        lblId.Text = entry.ID

    End Sub

    Private Sub ApplyReversedToForm(ByVal params As QueryStringParser, ByVal paramName As String, ByVal checkbox As Arco.Doma.WebControls.Controls.CheckBox)
        If params.Exists(paramName) Then
            checkbox.Checked = Not params.GetBoolean(paramName)
            params.Remove(paramName)
        Else
            checkbox.Checked = True
        End If
    End Sub


    Private Sub ApplyToForm(ByVal params As QueryStringParser, ByVal paramName As String, ByVal textbox As TextBox)
        If params.Exists(paramName) Then
            textbox.Text = params.GetString(paramName)
            params.Remove(paramName)
        End If
    End Sub

    Protected Sub btnOk_Click(sender As Object, e As System.EventArgs) Handles btnOk.Click
        If Not Page.IsValid Then
            Return
        End If
        Dim entry As MainPage

        If _pageId <> 0 Then
            entry = MainPage.GetMainPage(_pageId)

        Else
            entry = MainPage.NewMainPage()

        End If
        entry.Name = txtName.Text
        entry.Description = txtDescription.Text

        Dim qrsParser As New QueryStringParser(txtExtraQueryString.Text)

        ApplyReversedToParams(qrsParser, "hidetree", chkShowTree)
        ApplyReversedToParams(qrsParser, "hidecurrentfolder", chkShowBreadcrumb)
        ApplyToParams(qrsParser, "menu", txtMenu)
        ApplyToParams(qrsParser, "defaultaction", txtDefaultAction)
        ApplyToParams(qrsParser, "screenmode", txtScreenMode)
        ApplyReversedToParams(qrsParser, "hideglobalsearch", chkShowGlobalSearch)

        entry.QueryString = qrsParser.ToString()


        entry.Save()

        Page.ClientScript.RegisterStartupScript(Page.ClientScript.[GetType](), "onLoad", "GetRadWindow().close();", True)

    End Sub

    Private Sub ApplyToParams(ByVal params As QueryStringParser, ByVal paramName As String, ByVal textbox As TextBox)
        If Not String.IsNullOrEmpty(textbox.Text) Then
            params.Add(paramName, textbox.Text)
        End If
    End Sub
    Private Sub ApplyReversedToParams(ByVal params As QueryStringParser, ByVal paramName As String, ByVal checkbox As Arco.Doma.WebControls.Controls.CheckBox)
        If Not checkbox.Checked Then
            params.Add(paramName, True)
        End If
    End Sub
    Private Sub SetErrorMessage(ByVal t As String)
        lblMsgText.Visible = True
        lblMsgText.Text = t
        lblMsgText.CssClass = "ErrorLabel"
    End Sub

End Class
