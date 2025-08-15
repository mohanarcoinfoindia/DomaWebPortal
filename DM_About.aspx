<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_About.aspx.vb" Inherits="DM_About" MasterPageFile="~/masterpages/Empty.master" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <style type="text/css">
        .TabContent {
            padding: 5px;
        }
    </style>

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">

    <telerik:RadTabStrip ID="tabAbout" runat="server" MultiPageID="rpm" AutoPostBack="true" ClickSelectedTab="true">
        <Tabs>
            <telerik:RadTab Text="General" />
            <telerik:RadTab Text="Application Server" />
            <telerik:RadTab Text="Web Server" />
            <telerik:RadTab Text="Log Files" />
            <telerik:RadTab Text="Version" />
            <telerik:RadTab Text="Assemblies" />
            <telerik:RadTab Text="License Information" />
            <telerik:RadTab Text="Installation History" />
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage ID="rpm" runat="server" SelectedIndex="0">
        <telerik:RadPageView ID="pgGeneral" runat="server">
            <div class="TabContent PaddedTable">
                <asp:Label ID="lblGeneral" runat="server"></asp:Label>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="pgAppServer" runat="server">
            <div class="TabContent PaddedTable">
                <div style="text-align: center">
                    <asp:LinkButton ID="btnClearAllCaches" runat="server" Text="Clear all caches"></asp:LinkButton>
                    &nbsp;
            <asp:LinkButton ID="btnClearCache" runat="server" Text="Clear Application Server Cache"></asp:LinkButton>
                    &nbsp;
            <asp:LinkButton ID="btnClearDataCache" runat="server" Text="Clear Application Server Data Cache"></asp:LinkButton>
                </div>

                <asp:LinkButton ID="btnPauseService" runat="server" />
                <asp:LinkButton ID="btnResumeService" runat="server" />

                <asp:Literal ID="litAppServer" runat="server"></asp:Literal>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="pgWebServer" runat="server">
            <div class="TabContent PaddedTable">
                <div style="text-align: center">
                    <asp:LinkButton ID="btnClearWebCache" runat="server" Text="Clear Webserver Cache"></asp:LinkButton>
                </div>
                <asp:Literal ID="litWebServer" runat="server"></asp:Literal>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="pgLogFiles" runat="server">
            <div class="TabContent PaddedTable">
                <asp:Label ID="lblLogFiles" runat="server"></asp:Label>
                <asp:LinkButton ID="bntRemoveLogFile" runat="server" />
                <asp:LinkButton ID="btnBackupLogFile" runat="server" />
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="pgVersion" runat="server">
            <div class="TabContent PaddedTable">
                <asp:Label ID="lblVersion" runat="server"></asp:Label>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server" ID="pgAss">
            <div class="TabContent PaddedTable">
                <asp:Label ID="lblAssemblies" runat="server"></asp:Label>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="pgLic" runat="server">
            <div class="TabContent PaddedTable">
                <asp:Label ID="lblLicences" runat="server"></asp:Label>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="pgInst" runat="server">
            <div class="TabContent PaddedTable">
                <asp:GridView ID="grdInst" runat="server" AutoGenerateColumns="False" HorizontalAlign="Center">
                    <Columns>
                        <asp:BoundField DataField="Product" HeaderText="Product" SortExpression="Product" />
                        <asp:BoundField DataField="Version" HeaderText="Version" SortExpression="Version" />
                        <asp:BoundField DataField="TimeStamp" HeaderText="Installed on" SortExpression="Timestamp" />
                    </Columns>
                </asp:GridView>
            </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>

    <asp:ObjectDataSource ID="VersionListDataSource" runat="server" SelectMethod="GetInstalledversions"
        TypeName="Arco.Doma.Library.InstalledVersions"></asp:ObjectDataSource>

</asp:Content>
