Imports Arco.Doma.ImportExport.Drum
Imports Arco.Doma.ImportExport.Drum.V2.InputConnectors
Imports Arco.Doma.Library

Partial Class DrumJob
    Inherits BaseOperatorPage
    Public Property DrumJobID As Int32
        Get
            Return CType(ViewState("ID"), Int32)
        End Get
        Set(value As Int32)
            ViewState("ID") = value
        End Set
    End Property

    Private _isjob As InputStreamBatchJob
    Public ReadOnly Property ISJob As InputStreamBatchJob
        Get
            If _isjob Is Nothing AndAlso DrumJobID <> 0 Then
                _isjob = InputStreamBatchJob.GetISBatchJob(DrumJobID)
                If _isjob.ID <> DrumJobID Then
                    Throw New ArgumentException("ISJob not found")
                End If
            End If
            Return _isjob
        End Get
    End Property

    Private _stream As InputStream
    Public ReadOnly Property Stream As InputStream
        Get
            If _stream Is Nothing AndAlso DrumJobID <> 0 Then
                _stream = InputStream.GetIS(ISJob.IS_ID)
            End If
            Return _stream
        End Get
    End Property

    Protected Sub DrumJob_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            DrumJobID = QueryStringParser.GetInt("ID")
        End If

        AddInputStreamsCombo()
        AddName()
        AddStatus()
        AddTenant()
        AddParameters()

        btnOk.Text = GetLabel("save")
        btnCancel.Text = GetLabel("cancel")

        If DrumJobID <> 0 Then
            Title = String.Concat("Drum job", ": ", GetLabel("edit"))
        Else
            Title = String.Concat("Drum job", ": ", GetLabel("add"))
        End If
    End Sub

    Private Sub AddName()
        Dim loRow As Panel = New Panel
        loRow.CssClass = "row detail-form-row"
        loRow.Controls.Add(GetLabelCell(GetLabel("name")))
        Dim txtName As TextBox = New TextBox With {.ID = "JOBNAME"}
        If Not Page.IsPostBack AndAlso DrumJobID > 0 Then
            txtName.Text = ISJob.Name
        End If
        Dim loCell As Panel = New Panel
        loCell.CssClass = "col-md-8 FieldCell"
        loCell.Controls.Add(txtName)
        loRow.Controls.Add(loCell)

        tblDrum.Controls.Add(loRow)

    End Sub
    Private Sub AddTenant()
        If Settings.GetValue("General", "MultiTenant", False) Then
            Dim loRow As Panel = New Panel
            loRow.CssClass = "row detail-form-row"
            loRow.Controls.Add(GetLabelCell(GetLabel("tenant")))
            Dim drpTenant As DropDownList = New DropDownList With {.ID = "DRPTENANT"}
            Dim liSelected As Int32
            If Not Page.IsPostBack AndAlso DrumJobID > 0 Then
                liSelected = ISJob.TenantId
            End If


            drpTenant.Items.Add(New WebControls.ListItem("Global", "0") With {.Selected = (0 = liSelected)})
            drpTenant.Items.Add(New WebControls.ListItem("All Tenants", "-1") With {.Selected = (-1 = liSelected)})

            For Each tn As TenantList.TenantInfo In Tenants
                If Not Page.IsPostBack Then
                    drpTenant.Items.Add(New WebControls.ListItem(tn.Name, tn.Id.ToString()) With {.Selected = (tn.Id = liSelected)})
                Else
                    drpTenant.Items.Add(New WebControls.ListItem(tn.Name, tn.Id.ToString()))
                End If
            Next


            Dim loCell As Panel = New Panel
            loCell.CssClass = "col-md-8 FieldCell"
            loCell.Controls.Add(drpTenant)
            loRow.Controls.Add(loCell)

            tblDrum.Controls.Add(loRow)
        End If

    End Sub


    Private Sub AddStatus()
        Dim loRow As Panel = New Panel
        loRow.CssClass = "row detail-form-row"
        loRow.Controls.Add(GetLabelCell(GetLabel("status")))
        Dim drpStatus As DropDownList = New DropDownList With {.ID = "JOBSTATUS"}
        If Not Page.IsPostBack Then
            Dim liSelected As Arco.Doma.Library.Jobs.Job.JobStatus = Arco.Doma.Library.Jobs.Job.JobStatus.Disabled
            If DrumJobID > 0 Then
                liSelected = ISJob.Status
            End If
            drpStatus.Items.Add(New WebControls.ListItem(GetDecodedLabel("enabled"), "0") With {.Selected = (liSelected = Arco.Doma.Library.Jobs.Job.JobStatus.Enabled)})
            drpStatus.Items.Add(New WebControls.ListItem(GetDecodedLabel("disabled"), "1") With {.Selected = (liSelected = Arco.Doma.Library.Jobs.Job.JobStatus.Disabled)})
        Else
            drpStatus.Items.Add(New WebControls.ListItem(GetDecodedLabel("enabled"), "0"))
            drpStatus.Items.Add(New WebControls.ListItem(GetDecodedLabel("disabled"), "1"))
        End If
        Dim loCell As Panel = New Panel
        loCell.CssClass = "col-md-8 FieldCell"
        loCell.Controls.Add(drpStatus)
        loRow.Controls.Add(loCell)

        tblDrum.Controls.Add(loRow)

    End Sub
    Private Sub AddInputStreamsCombo()
        Dim loRow As Panel = New Panel
        loRow.CssClass = "row detail-form-row"

        loRow.Controls.Add(GetLabelCell("Inputstream"))

        Dim drpIS As DropDownList = New DropDownList With {.ID = "DRPIS"}
        If DrumJobID = 0 Then
            For Each ic As InputConnectorList.ICInfo In InputConnectorList.GetICsForVersion(2)
                Try
                    Dim o As IInputConnector = InputConnectorLoader.LoadFromIC(ic, False)
                    If o.SupportsBatchMode Then
                        Dim inputstreams As InputStreamList = InputStreamList.GetISs(ic.ID)
                        drpIS.Items.Add(New WebControls.ListItem(inputstreams(0).Name, inputstreams(0).ID.ToString))
                    End If
                Catch ex As Exception

                End Try

            Next
        Else
            drpIS.Enabled = False
            drpIS.Items.Add(New WebControls.ListItem(Stream.Name, Stream.ID.ToString) With {.Selected = True})
        End If


        Dim loCell As Panel = New Panel
        loCell.CssClass = "col-md-8 FieldCell"
        loCell.Controls.Add(drpIS)
        loRow.Controls.Add(loCell)

        tblDrum.Controls.Add(loRow)
    End Sub

    Private Sub AddParameters()
        If DrumJobID > 0 Then
            Dim drummer As V2.Drummer = New V2.Drummer(ISJob.Arguments)

            Dim icprops As ICPropertyList = ICPropertyList.GetICProperties(Stream.IC_INPUTCONNECTOR)
            For Each param As ICPropertyList.ICPropInfo In icprops.Cast(Of ICPropertyList.ICPropInfo).Where(Function(x) x.IC_PROP_TYPE = "PARAM")
                If Not drummer.InputArguments.ContainsKey(param.IC_PROP_NAME) Then
                    AddParameter(param.IC_PROP_NAME, param.Source, param.Source)
                Else
                    AddParameter(param.IC_PROP_NAME, drummer.InputArguments(param.IC_PROP_NAME), param.Source)
                End If
            Next
        End If
    End Sub

    Private Function GetLabelCell(ByVal label As String) As Panel
        Dim loCell As Panel = New Panel
        loCell.CssClass = "col-md-4 LabelCell"
        Dim loLabel As Label = New Label
        loLabel.CssClass = "Label"
        loLabel.Text = Server.HtmlEncode(label)
        loCell.Controls.Add(loLabel)
        Return loCell
    End Function
    Private Sub AddParameter(ByVal name As String, ByVal value As String, ByVal defaultvalue As String)
        Dim loRow As Panel = New Panel
        loRow.CssClass = "row detail-form-row"

        loRow.Controls.Add(GetLabelCell(name))

        Dim loCell As Panel = New Panel

        loCell.CssClass = "col-md-8 FieldCell"

        Dim txtBox As TextBox = New TextBox With {.ID = GetValueID(name)}

        If Not Page.IsPostBack Then
            txtBox.Text = value
            If value.Equals(defaultvalue) Then
                txtBox.BackColor = Color.AntiqueWhite
            Else
                txtBox.ToolTip = "Default value : " & defaultvalue
            End If
        End If

        loCell.Controls.Add(txtBox)
        loRow.Controls.Add(loCell)
        tblDrum.Controls.Add(loRow)

    End Sub


    Private Function GetValueID(ByVal name As String) As String
        Return "PRM_" & name
    End Function
    Protected Sub btnOk_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnOk.Click

        If DrumJobID = 0 Then
            Dim drpIS As DropDownList = DirectCast(tblDrum.FindControl("DRPIS"), DropDownList)
            If String.IsNullOrEmpty(drpIS.SelectedValue) Then
                Page.ClientScript.RegisterStartupScript(Me.GetType, "noic", "alert(" & EncodingUtils.EncodeJsString("No Drum Config selected") & ");", True)
                Exit Sub
            End If
            _stream = InputStream.GetIS(Convert.ToInt32(drpIS.SelectedValue))
        End If

        Dim lsName As String = DirectCast(tblDrum.FindControl("JOBNAME"), TextBox).Text
        Dim tenantId As Int32
        If Settings.GetValue("General", "MultiTenant", False) Then
            tenantId = Convert.ToInt32(DirectCast(tblDrum.FindControl("DRPTENANT"), DropDownList).SelectedValue)
        Else
            tenantId = 0
        End If

        If String.IsNullOrEmpty(lsName) Then
            lsName = "BatchJob for " & Stream.Name
            DirectCast(tblDrum.FindControl("JOBNAME"), TextBox).Text = lsName
        End If

        Dim liStatus As Arco.Doma.Library.Jobs.Job.JobStatus = CType(DirectCast(tblDrum.FindControl("JOBSTATUS"), DropDownList).SelectedValue, Arco.Doma.Library.Jobs.Job.JobStatus)
        If DrumJobID = 0 Then
            Dim ij As InputStreamBatchJob = Stream.AddBatchJob(QueryStringParser.GetInt("JOB_ID"), lsName, liStatus)
            DrumJobID = ij.ID

            ij.TenantId = tenantId
            ij = ij.Save



            Response.Redirect("DrumJob.aspx?ID=" & ij.ID & "&Modal=Y")
        Else
            Dim args As List(Of String) = New List(Of String)
            For Each param As ICPropertyList.ICPropInfo In ICPropertyList.GetICProperties(Stream.IC_INPUTCONNECTOR).Cast(Of ICPropertyList.ICPropInfo). _
                Where(Function(x) x.IC_PROP_TYPE = "PARAM")

                Dim txtBox As TextBox = DirectCast(tblDrum.FindControl(GetValueID(param.IC_PROP_NAME)), TextBox)
                If Not txtBox.Text.Equals(param.Source) Then
                    txtBox.BackColor = Color.White
                    txtBox.ToolTip = "Default value : " & param.Source
                    args.Add(String.Concat(param.IC_PROP_NAME, "=", txtBox.Text))
                Else
                    txtBox.ToolTip = ""
                    txtBox.BackColor = Color.AntiqueWhite
                End If
            Next
            ISJob.Name = lsName
            ISJob.Status = liStatus
            ISJob.TenantId = tenantId
            ISJob.Arguments = String.Join(";", args)
            ISJob.Save()

            Page.ClientScript.RegisterStartupScript(Me.GetType, "Close", "Close();", True)
        End If

    End Sub
End Class
