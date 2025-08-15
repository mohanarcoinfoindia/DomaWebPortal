Imports Arco.Doma.Library
Imports Arco.Doma.Library.baseObjects
Partial Class DM_RENAME_OBJECT
    Inherits BasePage

    Private _selectionType As DMSelection.SelectionType
    Private _selectionId As Int32
    Private _objectId As Int32
    Private _target As DM_OBJECT
    Private _selTarget As DMSelection

    Private Sub ParseQueryString()

        _objectId = QueryStringParser.GetInt("DM_OBJECT_ID")
        _selectionType = CType(QueryStringParser.GetInt("SEL_TYPE"), DMSelection.SelectionType)
        If _selectionType <> DMSelection.SelectionType.Undefined Then
            _selectionId = QueryStringParser.GetInt("SEL_ID")
        End If
    End Sub
    Private Sub SetLabels()
        If _selectionType = DMSelection.SelectionType.Undefined Then
            Page.Title = GetLabel("rename")
        Else
            Page.Title = GetLabel("setlabel")
        End If
        cmdChangeObjectName.Text = GetLabel("save")
        lblCancel.Text = GetLabel("cancel")

        lnkCopy.ToolTip = GetDecodedLabel("copy")
        lnkCopy.Text = ThemedImage.GetSpanIconTag("icon icon-copy")

        valNewName.ErrorMessage = GetLabel("name") & " " & GetLabel("req")
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        ParseQueryString()

        If _objectId = 0 Then
            Exit Sub
        End If

        Dim initialValue As String
        If _selectionType = 0 Then
            'normal rename
            _target = ObjectRepository.GetObject(_objectId)
            If Not _target.CanModifyMeta Then
                GotoErrorPage(_target.GetLastError.Code)
            End If
            initialValue = _target.Name
            valNewName.Enabled = True
        Else
            'rename of the object in a selection
            _selTarget = DMSelection.GetSelection(_objectId, _selectionType, _selectionId)
            If _selTarget.Object_ID = 0 Then
                'not found in the seleiton, what to do here??
            End If
            initialValue = _selTarget.Label
            If String.IsNullOrEmpty(initialValue) Then
                initialValue = ObjectRepository.GetObject(_objectId).Name
            End If
            valNewName.Enabled = False
        End If

        If Not Page.IsPostBack Then
            SetLabels()
            txtNewName.Width = ArcoFormatting.DefaultControlWidth


            Const wraplength = 75
            Dim lsDisplayName As String = initialValue
            If lsDisplayName.Length > wraplength Then
                lsDisplayName = ArcoFormatting.AddTooltip(lsDisplayName.Substring(0, wraplength) & "...", lsDisplayName)
            End If
            lsDisplayName = lsDisplayName.Replace(Environment.NewLine, "<br>")
            lblOldname.Text = lsDisplayName

            Dim itemLabel As String = If(_selectionType = DMSelection.SelectionType.Undefined, GetLabel("name"), "Label")
            Dim iMaxLen As Int32 = Settings.GetValue("Storage", "MaxNamesLength", 500)
            regMaxLen.ValidationExpression = "^[\s\S]{0," & iMaxLen & "}$"
            regMaxLen.ErrorMessage = itemLabel & " " & GetLabel("maxlen").Replace("#FIELDLEN#", iMaxLen.ToString)

            AddCopyScript(initialValue)
        End If

    End Sub

    Protected Sub cmdChangeObjectName_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdChangeObjectName.Click

        If Page.IsValid Then
            If _selectionType = 0 Then
                _target.Name = txtNewName.Text
                _target = _target.Save()

                If Not _target.HasError Then
                    AddReloadScript("RenameObject")
                Else
                    AddRenameCancelledScript(_target.GetLastError.Description)
                End If
            Else
                _selTarget.Label = txtNewName.Text
                _selTarget.Save()
                AddReloadScript("RenameObject")
            End If
        End If
    End Sub

    Private Sub AddCopyScript(ByVal vsName As String)
        Dim sb As New StringBuilder

        Dim sCopy As String = EncodingUtils.EncodeJsString(vsName) ' vsName.Replace(Chr(34), "\" & Chr(34)).Replace(Environment.NewLine, "\r")

        sb.AppendLine("function CopyName()")
        sb.AppendLine("{")
        sb.AppendLine("theForm." & txtNewName.ClientID & ".value = " & sCopy & ";")
        sb.AppendLine("}")

        Page.ClientScript.RegisterClientScriptBlock(GetType(String), "FCopy", sb.ToString, True)
    End Sub
    Private Sub AddRenameCancelledScript(ByVal vsMsg As String)

        Page.ClientScript.RegisterStartupScript(GetType(String), "FCancelled", "alert(" & EncodingUtils.EncodeJsString(vsMsg) & ")", True)

    End Sub
    Private Sub AddReloadScript(ByVal ScriptName As String)
        Dim sb As New StringBuilder
        If Not CType(Me.Master, ToolWindow).Modal Then
            sb.AppendLine("MainPage().ReloadParent();")
        End If
        sb.AppendLine("Close();")

        Page.ClientScript.RegisterStartupScript(GetType(String), ScriptName, sb.ToString, True)
    End Sub

End Class
