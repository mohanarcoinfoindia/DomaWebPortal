<%@ Control Language="VB" AutoEventWireup="false" CodeFile="StatControl.ascx.vb" Inherits="UserControls_StatControl" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<asp:HiddenField ID="hdnreportid" runat="server" Value="0" />


<asp:Panel ID="pnlSelect" runat="server">
    <div runat="server" id="tblSelect" class="container-fluid detail-form-container">
        <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" Visible="false"></asp:Label>
       
        <div class="row detail-form-row" id="rowType" runat="server">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblType" runat="server" CssClass="Label"></asp:Label>
            </div>
            <div class="col-md-4 FieldCell">
                <asp:DropDownList ID="drpBarType" runat="server" AutoPostBack="true"></asp:DropDownList>
            </div>            
        </div>
        <div class="row detail-form-row" id="rowGroupBy" runat="server">
            <div class="col-md-4 LabelCell" runat="server">
                <asp:Label ID="lblGroupBy" runat="server" CssClass="Label" Text="Group by"></asp:Label>
            </div>
            <div class="col-md-4 FieldCell" runat="server">
                <asp:DropDownList ID="drpGroupBy" runat="server" AutoPostBack="true"></asp:DropDownList>&nbsp;
            </div>               
        </div>
        <div class="row detail-form-row" id="rowFilter" runat="server">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblDate" runat="server" CssClass="Label" Text="Date"></asp:Label>
            </div>
            <div class="col-md-4 FieldCell">
                <asp:DropDownList ID="drpDate" runat="server" AutoPostBack="true"></asp:DropDownList>
            </div>          
        </div>
        <div class="row detail-form-row" id="rowDateCustom" runat="server" Visible="false">
            <div class="col-md-4 LabelCell"></div>
            <div class="col-md-8 LabelCell">
                <asp:TextBox ID="txtDateCustom" runat="server" Visible="false" />
            </div>
        </div>
        <div class="row detail-form-row" id="rowDateRange" runat="server" Visible="false">
            <div class="col-md-4 LabelCell">
            </div>
            <div class="col-md-8 LabelCell">
                <telerik:RadDatePicker ID="DateFrom" runat="server" RenderMode="Lightweight" DateInput-Label="From"></telerik:RadDatePicker>&nbsp;
                <telerik:RadDatePicker ID="DateTo"   runat="server" RenderMode="Lightweight" DateInput-Label="To"></telerik:RadDatePicker>                
            </div>
        </div>
      
        <div class="row detail-form-row" id="rowShowPercentage" runat="server">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblShowPercentage" Text="Show percentage" runat="server" CssClass="Label"></asp:Label>
            </div>
            <div class="col-md-4 FieldCell">
                <ArcoControls:CheckBox ID="chkShowPercentage" AutoPostBack="true" Text="Show percentage" runat="server" />
            </div>           
        </div>
       
        <div class="row detail-form-row" id="rowTotal" runat="server">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblTotals" runat="server" CssClass="Label" Text="Totals"></asp:Label>
            </div>
            <div class="col-md-4 FieldCell">
                <ArcoControls:CheckBoxList RepeatDirection="Horizontal" ID="chkTotals" runat="server"></ArcoControls:CheckBoxList>
            </div>           
        </div>
        <div class="row detail-form-row" id="rowButtons" runat="server">
            <div class="col-md-8 offset-md-4 FieldCell">
                <Arco:ButtonPanel runat="server">
                    <asp:LinkButton ID="btnRefresh" runat="server" AlternateText="refresh" />                                    
                </Arco:ButtonPanel>
            </div>
        </div>
    </div>
</asp:Panel>

<telerik:RadClientExportManager runat="server" ID="RadClientExportManager1">
</telerik:RadClientExportManager>

<telerik:RadHtmlChart ID="ch1" runat="server">
    <PlotArea>
        <XAxis Type="Category">
            <MinorGridLines Visible="false" />
        </XAxis>
        <YAxis>
            <MinorGridLines Visible="false" />
        </YAxis>
    </PlotArea>
</telerik:RadHtmlChart>

<telerik:RadPivotGrid runat="server" ID="pivot1" AllowPaging="true"
    OnNeedDataSource="RadPivotGrid1_NeedDataSource" 
    AllowFiltering="false" ShowFilterHeaderZone="false"
    AllowSorting="true" Skin="Metro" ShowRowHeaderZone="true" ShowColumnHeaderZone="true" ShowDataHeaderZone="false">
    <Fields></Fields>
    <PagerStyle Mode="NextPrevNumericAndAdvanced" AlwaysVisible="false"></PagerStyle>
    <ClientSettings EnableFieldsDragDrop="false">
        <Scrolling AllowVerticalScroll="true"></Scrolling>
    </ClientSettings>
</telerik:RadPivotGrid>

<telerik:RadGrid runat="server" ID="grid1" 
    OnNeedDataSource="RadGrid1_NeedDataSource"  
    AllowPaging="true" AllowFilteringByColumn="false" AllowSorting="true" 
    ShowGroupPanel="false"
    ShowFooter="true">
    <MasterTableView AutoGenerateColumns="false" Width="100%" TableLayout="Auto" ShowGroupFooter="true" >
        <Columns></Columns>
    </MasterTableView>
    <PagerStyle Mode="NextPrevNumericAndAdvanced" AlwaysVisible="true"></PagerStyle>
    <ClientSettings AllowDragToGroup="false" AllowRowsDragDrop="false" AllowColumnsReorder="false" >
    </ClientSettings>
    <GroupingSettings ShowUnGroupButton="false" />
</telerik:RadGrid>

<%--<telerik:RadButton RenderMode="Lightweight" runat="server" OnClientClicked="exportChartToPDF" Text="Export to PDF" AutoPostBack="false" />--%>
<asp:ImageButton ID="btnExportExcel" runat="server" Text="Excel" />

<script>
    function exportChartToPDF() {
        var $ = $telerik.$;
        $find('<%=RadClientExportManager1.ClientID%>').exportPDF($(".RadHtmlChart"));
    }

    function exportChartToImage() {
        var $ = $telerik.$;
        $find('<%=RadClientExportManager1.ClientID%>').exportImage($(".RadHtmlChart"));
    }
</script>