<%@ Page Language="VB" AutoEventWireup="False" CodeFile="DM_JOBS.aspx.vb" Inherits="DM_JOBS" EnableEventValidation="false" MasterPageFile="~/masterpages/Empty.master" %>

<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <script type="text/javascript">
        function OpenLog(url) {
            if (url.indexOf("&rnd_str=") < 0) {
                if (url.indexOf('?') > 0) {
                    url = url + '&rnd_str=' + Math.random();
                }
                else {
                    url = url + '?rnd_str=' + Math.random();
                }
            }
            PC().OpenModalWindowRelativeSize(url, true);
            //w.focus();
        }

        function NewDrumJob(jobid) {
            const width = window.innerWidth * 0.7;
            const height = window.innerHeight * 0.9;
            PC().OpenModalWindow("DrumJob.aspx?JOB_ID=" + jobid, true, width, height, true, false);
        }

        function EditDrumJob(id) {
            const width = window.innerWidth * 0.7;
            const height = window.innerHeight * 0.9;
            PC().OpenModalWindow("DrumJob.aspx?ID=" + id, true, width, height, true, false);
        }

        function OrderBy(val) {
            $get("<%=hdnOrderBy.ClientId %>").value = val;
            Refresh();
        }

        function Refresh() {
            currentLoadingPanel = $find("<%= radAjxLoadingPanel.ClientID%>");
            currentLoadingPanel.show("<%=GridView1.ClientId%>");

            document.forms[0].submit();
        }

        function SetRadWindowDimensions(sender, args) {
            sender.set_height(window.innerHeight * 0.8);
            sender.set_width(window.innerWidth * 0.6);
            sender.center();
        }
    </script>

    <asp:HiddenField ID="hdnOrderBy" runat="server" />
    <asp:Panel ID="pnlPaused" CssClass="WarningLabel" runat="server">
            <%=GetLabel("jobspausedbegin") %>
            <asp:LinkButton ID="lnkResumeJP" OnClick="lnkResumeJP_Click" Text="here" runat="server" />
            <%=GetLabel("jobspausedend") %>
    </asp:Panel>
    <div class="CustomStickyListHeader" style="padding: 3px 5px">
        <span class="icon icon-refresh" onclick="Refresh()" title="<%=GetLabel("refresh")%>"></span>
        <asp:TextBox ID="txtName" runat="server" AutoPostBack="true" />
        <asp:DropDownList ID="drpAssemblies" runat="server" AutoPostBack="true">
            <asp:ListItem Value="" Selected="True">All</asp:ListItem>
            <asp:ListItem Value="Arco.Doma.Library">System</asp:ListItem>
            <asp:ListItem Value="Arco.Doma.ImportExport">Drum</asp:ListItem>
            <asp:ListItem Value="custom">Custom</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="drpExecStatus" runat="server" AutoPostBack="true">
            <asp:ListItem Value="0" Selected="True">All</asp:ListItem>
            <asp:ListItem Value="1">Idle</asp:ListItem>
            <asp:ListItem Value="2">In Progress</asp:ListItem>
            <asp:ListItem Value="3">In Error</asp:ListItem>
        </asp:DropDownList>
        <asp:DropDownList ID="drpJobStatus" runat="server" AutoPostBack="true">
            <asp:ListItem Value="0">All</asp:ListItem>
            <asp:ListItem Value="1" Selected="True">Enabled</asp:ListItem>
            <asp:ListItem Value="2">Disabled</asp:ListItem>
            <asp:ListItem Value="3">Blocked</asp:ListItem>
        </asp:DropDownList>
        <ArcoControls:CheckBox ID="chkExpandDrumJobs" runat="server" Text="Expand Drum Jobs" AutoPostBack="true" />
        <asp:Label ID="chkExpandDrumJobsLabel" runat="server"></asp:Label>
    </div>

    <telerik:RadAjaxLoadingPanel ID="radAjxLoadingPanel" runat="server" EnableEmbeddedSkins="true"></telerik:RadAjaxLoadingPanel>

    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="JobListDataSource" HorizontalAlign="Center"
        DataKeyNames="ID" EnableViewState="true" SkinID="StickyHeadersGrid">
        <HeaderStyle CssClass="ListHeaderLevel2" />
        <Columns>
            <asp:BoundField DataField="ID" HeaderText="ID" Visible="false" />
            <asp:BoundField DataField="ExecutionStatus" />
            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" ItemStyle-Font-Bold="true" />
            <asp:BoundField DataField="Server" HeaderText="Server" />
             <asp:BoundField DataField="Last_SuccessDate" HeaderText="Last Success" />
            <asp:BoundField DataField="Last_Date" HeaderText="Last" />
            <asp:BoundField DataField="Next_Date" HeaderText="Next" />
            <asp:BoundField DataField="LastSeconds" HeaderText="Last run Seconds" />
            <asp:ButtonField Text="Execute" CommandName="cmdExecuteJob" ControlStyle-CssClass="ButtonLink" ItemStyle-Width="25px" />
            <asp:TemplateField ItemStyle-Width="25px">
                <ItemTemplate>
                    <asp:LinkButton ID="cmdToggle" runat="server" CssClass="ButtonLink"></asp:LinkButton>
                    <asp:LinkButton ID="cmdBlock" runat="server" CssClass="ButtonLink" Text="Block"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="25px">
                <ItemTemplate>
                    <asp:HyperLink ID="cmdAdd" runat="server" CssClass="ButtonLink" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="LogViewer" HeaderText="" ItemStyle-Width="25px" />
        </Columns>
    </asp:GridView>

    <asp:ObjectDataSource ID="JobListDataSource" runat="server" SelectMethod="GetAllJobs"
        TypeName="Arco.Doma.Library.Jobs.JobList">
        <SelectParameters>
            <asp:ControlParameter Name="name" ControlID="txtName" DbType="String" />
            <asp:ControlParameter Name="executionStatus" ControlID="drpExecStatus" DbType="Int32" />
            <asp:ControlParameter Name="jobStatus" ControlID="drpJobStatus" DbType="Int32" />
            <asp:ControlParameter Name="vsAssembly" ControlID="drpAssemblies" DbType="String" />
            <asp:ControlParameter Name="vsOrderBy" ControlID="hdnOrderBy" DbType="String" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <div style="padding: 3px">
        <table style="width: 100%;">
            <tr>
                <td style="width: 33%; text-align: center;">
                    <asp:LinkButton ID="lnkPauzeJP" runat="server" Text="Pauze Processor" OnClick="lnkPauzeJP_Click"></asp:LinkButton></td>
                <td style="width: 33%; text-align: center;">
                    <span class="icon icon-refresh" onclick="Refresh()" title="<%=GetLabel("refresh")%>" />
                </td>
                <td style="width: 33%; text-align: center;">
                    <a href="javascript:NewDrumJob(0);" class="ButtonLink">
                        <span class="icon icon-add-new" runat="server" id="lnkNewDrumJob"></span>
                    </a>
                </td>
            </tr>
        </table>
    </div>

    <telerik:RadWindow ID="JobDetailsViewWindow" runat="server" Modal="true" Behaviors="Close, Move, Resize, Maximize" OnClientBeforeShow="SetRadWindowDimensions" CssClass="JobDetailsWindow">
        <ContentTemplate>
            <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="false" DataSourceID="JobDetailDataSource" RowStyle-CssClass="detail-form-row">
                <Fields>
                    <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="Description" HeaderText="Description" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="Job_Assembly" HeaderText="Assembly" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="Job_Class" HeaderText="Class" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />

                     <asp:BoundField DataField="Last_SuccessDate" HeaderText="Last Success" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                     <asp:BoundField DataField="Last_Date" HeaderText="Last" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                     <asp:BoundField DataField="Next_Date" HeaderText="Next" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />

                    <asp:BoundField DataField="IntervalLabel" HeaderText="Interval" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="TimeSlots" HeaderText="TimeSlots" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="Arguments" HeaderText="Args." ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="NextJobLabel" HeaderText="Next Job" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="RunCount" HeaderText="#Runs" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="AverageSeconds" HeaderText="Avg. Secs" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="MaxSeconds" HeaderText="Max Secs" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="LastSeconds" HeaderText="Last run Secs" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="Attempts" HeaderText="Attempts" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="MaxRetries" HeaderText="Max Retries" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <asp:BoundField DataField="LastError" HeaderText="Last Error" ItemStyle-ForeColor="Red" ItemStyle-CssClass="ReadOnlyFieldCell" HeaderStyle-CssClass="LabelCell" />
                    <%--<asp:ButtonField Text="Execute" CommandName="cmdExecuteJob" ControlStyle-CssClass="ButtonLink" ItemStyle-Width="25px" />--%>
                </Fields>
            </asp:DetailsView>
            <asp:ObjectDataSource ID="JobDetailDataSource" runat="server" SelectMethod="GetJob" TypeName="Arco.Doma.Library.Jobs.JobList">
                <SelectParameters>
                    <asp:ControlParameter Name="id" ControlID="GridView1" Type="Int32" PropertyName="SelectedValue" DefaultValue="-1" />
                </SelectParameters>
            </asp:ObjectDataSource>

        </ContentTemplate>
    </telerik:RadWindow>
</asp:Content>

