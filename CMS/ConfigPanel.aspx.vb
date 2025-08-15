Imports System.Xml
Imports Arco.Doma.CMS.Data
Imports Arco.Doma.CMS.WebPanels
Imports Arco.Doma.CMS.WebPanels.Configuration
Imports Arco.Doma.CMS.WebPanels.Configuration.ParameterTypes
Imports Arco.Utils

Partial Class CMS_ConfigPanel
    Inherits BaseAdminOnlyPage

    Protected Overrides ReadOnly Property ErrorPageUrl As String
        Get
            Return "../DM_ACL_DENIED.aspx"
        End Get
    End Property

    Private PanelID As Integer
    Protected PageID As Integer
    Protected loConfig As WebPanelConfig
    Protected resizeModalOnLoad As Boolean
    Private _panel As Panel

    Protected Property ShowV8Options As Boolean

    Protected Sub CMS_ConfigPanel_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        PanelID = QueryStringParser.GetInt("panelid")
        PageID = QueryStringParser.GetInt("pageid")

        If PanelID <= 0 OrElse PageID <= 0 Then
            GotoErrorPage(Arco.Doma.Library.baseObjects.LibError.ErrorCode.ERR_UNEXPECTED)
        End If

        ShowV8Options = Not String.IsNullOrEmpty(Settings.GetValue("General", "url_v8", ""))
        _panel = Panel.GetPanel(PanelID)

        rowBorder.Visible = ShowV8Options

        If _panel.ID > 0 Then
            Dim handler As IWebPanel = Arco.ApplicationServer.Library.Shared.PluginManager.CreateInstance(Of IWebPanel)(_panel.PanelTypeAssembly, _panel.PanelTypeClass)
            resizeModalOnLoad = Not handler.MaximizeConfigModal
            Page.Title = GetLabelText("configurepanel") + ": " + GetLabel(handler.Name)
            loConfig = handler.GetConfiguration(PanelID, PageID)
            loConfig.LoadXml(_panel.PanelConfig)
            If Not Page.IsPostBack Then
                txtHeader.SetValue(loConfig.Header)
                txtHeaderImage.Text = XmlConvert.DecodeName(loConfig.HeaderImageUrl)
                txtRoles.Text = loConfig.Roles
                txtCondition.Text = loConfig.Condition
                txtHeight.Text = loConfig.Height
                cbShowHeader.Checked = loConfig.ShowHeader
                If ShowV8Options Then
                    drpBorderStyle.SelectedValue = loConfig.BorderStyle
                End If

                lblHeader.Text = GetLabel("header")
                    lblHeaderImage.Text = GetLabel("headerimgurl")
                    lblShowHeader.Text = GetLabel("showheader")
                    lblRoles.Text = GetLabel("roles")
                    lblCondition.Text = GetLabel("condition")
                    lblHeight.Text = GetLabel("height")
                    txtCondition.ToolTip = GetLabel("conditionfilterhelp")
                    lblBorder.Text = "Border"
                End If

                For Each loPanelParameter As WebPanelParameter In loConfig.Parameters
                Dim c As Control = loPanelParameter.Type.CreateControl()
                c.ID = loPanelParameter.Name

                If Not Page.IsPostBack AndAlso loPanelParameter.Value IsNot Nothing Then
                    loPanelParameter.Type.SetValue(c, loPanelParameter.Value)
                End If

                Dim paramType = TryCast(loPanelParameter.Type, ComplexParameterType)
                If paramType IsNot Nothing Then
                    If IsTableParameterType(paramType.GetType()) Then
                        configForm.CssClass += " has-multi-table"

                        ' we need to add some id's to ensure unique id's for all controls
                        Dim insertRow As TableRow = c.Controls(c.Controls.Count - 1)
                        insertRow.ID = loPanelParameter.Name + "InsertRow"
                        DirectCast(TableRowParameterType.GetActionButtons(insertRow).Last(), LinkButton).ID = loPanelParameter.Name + "InsertButton"

                        If Not Page.IsPostBack Then
                            SetActionButtonsOnTableControl(c, loPanelParameter.Name) 'bind the buttons (delete, moveup, movedown) to serverside methods
                        End If
                    End If
                    If paramType.RenderOnNewLine Then
                        AddParamToFormStacked(loPanelParameter.Name, c, loPanelParameter.Description)
                    Else
                        AddParamToFormInline(loPanelParameter.Name, c, loPanelParameter.Description)
                    End If
                Else
                    AddParamToFormInline(loPanelParameter.Name, c, loPanelParameter.Description)
                End If

            Next
        End If
    End Sub

    ' For ComplexParameterTypes, controls (e.g. tablerows) are added during IWebPanelParameterType.SetValue()
    ' For the framework to load the viewstate (coming from the client) into the controls, all controls need to be added to the page already
    ' because for example rows can be added or removed, we need to save the viewstate after the button-click events, 
    ' so the values can be loaded (and controls added/removed) before the framework maps the values from the viewstate to the controls.
    ' For this, we override the SaveViewState and LoadViewState methods and save the value to the ViewBag.
    Protected Overrides Sub LoadViewState(savedState As Object)
        Dim savedStateArray = CType(savedState, Object())
        MyBase.LoadViewState(savedStateArray(1))
        Dim savedViewStates As Dictionary(Of String, String) = savedStateArray(0)
        For Each item In savedViewStates
            Dim parameter = loConfig.Parameters.Find(Function(x) x.Name = item.Key)
            Dim control = updatePanel1.FindControl(item.Key)
            parameter.Type.SetValue(control, item.Value)
            If IsTableParameterType(parameter.Type.GetType()) Then
                SetActionButtonsOnTableControl(control, item.Key)
            End If
        Next
    End Sub

    Protected Overrides Function SaveViewState() As Object
        Dim flexibleParamsStates As Dictionary(Of String, String) = New Dictionary(Of String, String)
        For Each parameter As WebPanelParameter In loConfig.Parameters
            Dim paramType = TryCast(parameter.Type, ComplexParameterType)
            If paramType IsNot Nothing Then
                Dim control As Control = updatePanel1.FindControl(parameter.Name)
                Dim value As String = paramType.GetValue(control)
                paramType.SetValue(control, value) 'necessary for conditionally linked parameter types to ensure unique id's
                paramType.BeforeRenderControl(control)
                flexibleParamsStates.Add(parameter.Name, value)
            End If
        Next
        Dim saveState = New List(Of Object)(New Object() {flexibleParamsStates, MyBase.SaveViewState})
        Return saveState.ToArray()
    End Function

    Protected Sub BtnInfo_Click(sender As Object, e As EventArgs) Handles btnInfo.Click
        Page.ClientScript.RegisterStartupScript(Me.GetType, "OpenWindow", "window.open('../Help/CMS/FilterInfoPage.aspx','mywindow','menubar=1,resizable=1,width=900,height=600');", True)
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        loConfig.Header = txtHeader.GetValue()
        loConfig.HeaderImageUrl = XmlConvert.EncodeName(txtHeaderImage.Text.Trim())
        loConfig.ShowHeader = cbShowHeader.Checked
        loConfig.Roles = txtRoles.Text
        If ShowV8Options Then
            loConfig.BorderStyle = drpBorderStyle.SelectedValue
        End If

        Dim condition = txtCondition.Text
        Dim condPassed = False
        Try
            Dim condParser = New ConditionParser()
            Dim tst = condParser.Evaluate(condition)
            condPassed = True
        Catch ex As Exception
            Logging.LogError("CMS ConfigPanel: condition error. Exception: " + ex.Message)
        End Try

        If Not condPassed Then
            lblError.Visible = True
            lblError.Text = GetLabel("panelconditionerror")
            If Not txtCondition.CssClass.Contains("form-control is-invalid") Then txtCondition.CssClass += " form-control is-invalid"
            loConfig.Condition = String.Empty
        Else
            lblError.Visible = False
            txtCondition.CssClass.Replace(" form-control is-invalid", "")
            loConfig.Condition = condition
        End If

        Dim height As Integer = 0
        Integer.TryParse(txtHeight.Text, height)
        loConfig.Height = If(height < 0, 0, height)

        For Each loPanelParameter As WebPanelParameter In loConfig.Parameters
            loPanelParameter.Value = loPanelParameter.Type.GetValue(updatePanel1.FindControl(loPanelParameter.Name))
        Next

        _panel.PanelConfig = loConfig.SaveXML()
        _panel = _panel.Save()

        If condPassed Then
            Page.ClientScript.RegisterStartupScript(Me.GetType, "CloseModalAndReloadPage", "setTimeout(function() { parent.location.href = parent.location; }, 200);", True)
        End If
    End Sub

    Private Function IsTableParameterType(ByVal type As Type) As Boolean
        Return type.IsGenericType() AndAlso type.GetGenericTypeDefinition() = GetType(TableParameterType(Of))
    End Function

    Private Sub SetActionButtonsOnTableControl(ByVal control As Control, ByVal parameterName As String)
        For index = 1 To control.Controls.Count - 1
            Dim row As TableRow = control.Controls(index)
            Dim buttons As IEnumerable(Of IButtonControl) = TableRowParameterType.GetActionButtons(row)
            For Each button As IButtonControl In buttons
                AddHandler button.Command, AddressOf Action_Click
                If index = control.Controls.Count - 1 Then
                    button.CommandArgument = parameterName 'for the add-button
                End If
            Next
        Next
    End Sub

    Private Sub AddParamToFormInline(ByVal label As String, ByVal control As Control, ByVal tooltip As String)
        Dim formRow As WebControls.Panel = New WebControls.Panel() With {.ID = control.ID + "row", .CssClass = "row detail-form-row"}
        If label IsNot Nothing Then
            Dim labelControl As New Label() With {.CssClass = "Label", .Text = GetLabelText(label)}
            If Not String.IsNullOrEmpty(tooltip) Then
                labelControl.ToolTip = tooltip
            End If
            Dim labelCol As WebControls.Panel = New WebControls.Panel() With {.CssClass = "col-md-4 LabelCell"}
            labelCol.Controls.Add(labelControl)
            formRow.Controls.Add(labelCol)
        End If

        If control IsNot Nothing Then
            Dim fieldCol As WebControls.Panel = New WebControls.Panel() With {.CssClass = "col-md-8 FieldCell", .ID = control.ID + "cell"}
            fieldCol.Controls.Add(control)
            formRow.Controls.Add(fieldCol)
        End If
        updatePanel1.ContentTemplateContainer.Controls.Add(formRow)
    End Sub

    Private Sub AddParamToFormStacked(ByVal label As String, ByVal control As Control, ByVal tooltip As String)
        If label IsNot Nothing Then

            Dim labelControl As New Label() With {.CssClass = "Label", .Text = GetLabelText(label)}
            If Not String.IsNullOrEmpty(tooltip) Then
                labelControl.ToolTip = tooltip
            End If

            Dim labelRow As WebControls.Panel = New WebControls.Panel With {.CssClass = "row detail-form-row"}
            Dim labelCol As WebControls.Panel = New WebControls.Panel() With {.CssClass = "col-12 LabelCell"}
            labelCol.Controls.Add(labelControl)
            labelRow.Controls.Add(labelCol)
            updatePanel1.ContentTemplateContainer.Controls.Add(labelRow)
        End If

        If control IsNot Nothing Then
            Dim fieldRow As WebControls.Panel = New WebControls.Panel() With {.ID = control.ID + "row", .CssClass = "row detail-form-row"}
            Dim fieldCol As WebControls.Panel = New WebControls.Panel() With {.CssClass = "col-12 FieldCell", .ID = control.ID + "cell"}
            fieldCol.Controls.Add(control)
            fieldRow.Controls.Add(fieldCol)
            updatePanel1.ContentTemplateContainer.Controls.Add(fieldRow)
        End If
    End Sub

    Private Function GetLabelText(ByVal label As String) As String
        Dim returnLabel As String
        Try
            Dim languageLabel As String = GetLabel(label.ToLower())
            returnLabel = If(String.IsNullOrEmpty(languageLabel), label, languageLabel)
        Catch
            returnLabel = label
        End Try
        Return returnLabel
    End Function

    Protected Sub Action_Click(ByVal sender As Object, ByVal e As CommandEventArgs)
        Dim controlClicked As Control = sender
        Dim table As Table = controlClicked.Parent.Parent.Parent
        Dim buttonClicked As IButtonControl = sender
        Select Case buttonClicked.CommandName
            Case "Delete"
                TableParameterType.DeleteRow(buttonClicked.CommandArgument, table)
            Case "Insert"
                Dim parameterType = DirectCast(loConfig.Parameters.Find(Function(x) x.Name = buttonClicked.CommandArgument).Type, TableParameterType)
                parameterType.AddRow(table)
                parameterType.SetValue(table, parameterType.GetValue(table)) 'needed to set unique ID's
            Case "MoveUp"
                TableParameterType.MoveRowUp(buttonClicked.CommandArgument, table)
            Case "MoveDown"
                TableParameterType.MoveRowDown(buttonClicked.CommandArgument, table)
        End Select
    End Sub

End Class
