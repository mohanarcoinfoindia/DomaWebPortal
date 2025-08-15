<%@ Page Language="vb" AutoEventWireup="false" CodeFile="DM_UserBrowserMembers.aspx.vb" Inherits="DM_UserBrowserMembers" EnableSessionState="True" %>

<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
        #trSearchCrit {
            border-style: none;
            /*border-style: solid;
            border-color: #EEF5F8;
            border-width: 0px 0px 1px 0px;*/
            padding-bottom: 10px;
            height: 70px;
        }

        #CloseWindowBtn {
            float: right;
            margin-right: 20px;
        }

        #grdSelection {
            overflow-y: auto;
            height: auto;
            max-height: 85vh; 
            width:auto !important; /*width property gets set on element level after Ajax refresh.*/
        }

        #grdMembers {
            overflow-y: auto;
            height: auto;
            max-height: 85vh;
            width:auto;
        }

        #btnPanel {
            justify-content: flex-end;
        }

        .filterCriteria {
            display: flex;
        }

            .filterCriteria td {
                margin: 5px;
                padding: 5px;
            }

        .ButtonLink {
            vertical-align: middle;
            opacity: 0.4;
            cursor: default;
        }

            .ButtonLink > span {
                padding: 2px;
                border: 1px solid gray;
                border-radius: 33%;
            }
    </style>
</head>
<body>
    <form id="mainForm" method="post" runat="server">
        <telerik:RadWindowManager ID="Singleton" runat="server">
            <Windows>
                <telerik:RadWindow runat="server" ID="RadWindow1" ShowContentDuringLoad="false" VisibleStatusbar="false" Overlay="true" Behaviors="Close" />
            </Windows>
        </telerik:RadWindowManager>

        <input type="hidden" id="subjectid" runat="server" />

        <telerik:RadScriptManager ID="RadScriptManager1" runat="server" />

        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" />

        <telerik:RadAjaxManager runat="server" ID="radAjax" DefaultLoadingPanelID="RadAjaxLoadingPanel1">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="grdSelection">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdSelection" />
                        <telerik:AjaxUpdatedControl ControlID="grdMembers" />
                        <telerik:AjaxUpdatedControl ControlID="lblWarning" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="grdMembers">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdSelection" />
                        <telerik:AjaxUpdatedControl ControlID="grdMembers" />
                        <telerik:AjaxUpdatedControl ControlID="lblWarning" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="ADD">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdSelection" />
                        <telerik:AjaxUpdatedControl ControlID="grdMembers" />
                        <telerik:AjaxUpdatedControl ControlID="buttons" />
                        <telerik:AjaxUpdatedControl ControlID="lblWarning" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="REMOVE">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdSelection" />
                        <telerik:AjaxUpdatedControl ControlID="grdMembers" />
                        <telerik:AjaxUpdatedControl ControlID="buttons" />
                        <telerik:AjaxUpdatedControl ControlID="lblWarning" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="btnFilter">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="grdSelection" />
                        <telerik:AjaxUpdatedControl ControlID="grdMembers" />
                        <telerik:AjaxUpdatedControl ControlID="btnFilter" />
                        <telerik:AjaxUpdatedControl ControlID="lblWarning" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>

        <telerik:RadScriptBlock runat="server" ID="scriptBlock">
            <script type="text/javascript">          
                function checkSelectionLeft() {
                    $get("ADD").style.opacity = "1";
                    $get("ADD").style.cursor = "pointer";
                }
                function resetSelectionLeft() {
                    $get("ADD").style.opacity = "0.4";
                    $get("ADD").style.cursor = "default";
                }
                function checkSelectionRight() {
                    $get("REMOVE").style.opacity = "1";
                    $get("REMOVE").style.cursor = "pointer";
                }

                function onRowDropping(sender, args) {
                    if (sender.get_id() == "<%=grdSelection.ClientID %>") {
                        var node = args.get_destinationHtmlElement();
                        if (!isChildOf('<%=grdMembers.ClientID %>', node) && !isChildOf('<%=grdSelection.ClientID %>', node)) {
                            args.set_cancel(true);
                        }
                    }
                    else {
                        if (sender.get_id() == "<%=grdMembers.ClientID %>") {
                            var node = args.get_destinationHtmlElement();
                            if (!isChildOf('<%=grdMembers.ClientID %>', node) && !isChildOf('<%=grdSelection.ClientID %>', node)) {
                                args.set_cancel(true);
                            }
                        }
                    }
                }
                function isChildOf(parentId, element) {
                    while (element) {
                        if (element.id && element.id.indexOf(parentId) > -1) {
                            return true;
                        }
                        element = element.parentNode;
                    }
                    return false;
                }
                function AdjustRadWidow() {
                    setTimeout(function () { GetRadWindow().autoSize(false) }, 200);
                }
                function GetRadWindow() {
                    var oWindow = null;
                    if (window.radWindow) oWindow = window.radWindow;
                    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                    return oWindow;
                }
                function onRowDoubleClick(sender, args) {
                    if (sender.get_id() == "<%=grdSelection.ClientID %>") {
                        __doPostBack("<%=ADD.ClientID %>", "");
                    }
                    else if (sender.get_id() == "<%=grdMembers.ClientID %>") {
                        __doPostBack("<%=REMOVE.ClientID %>", "");
                    }
                }

             <% If QueryStringParser.Modal Then %>
                function Close() {
                    var wnd = GetRadWindow();
                    if (wnd) {
                        wnd.close();
                        parent.Refresh();
                    }
                }
             <%Else%>
                function Close() {
                    self.close();
                    opener.parent.location.reload();
                }
             <%End If %>
</script>
        </telerik:RadScriptBlock>

        <asp:Table ID="Table1" CssClass="DetailTable" runat="server">
            <asp:TableRow runat="server" ID="trSearchCrit">
                <asp:TableCell ColumnSpan="3">
                    <asp:Table runat="server" ID="tblTop" Width="100%">
                        <asp:TableRow>
                            <asp:TableCell ID="tcDomains" runat="server" VerticalAlign="Bottom">
                                <asp:Label runat="server" ID="Label2" CssClass="LabelNotBold"><%=GetLabel("ub_domains") %>:&nbsp</asp:Label>
                                <telerik:RadComboBox runat="server" ID="drpdwnDomains" />
                            </asp:TableCell>

                            <asp:TableCell>
                                <asp:TableRow CssClass="filterCriteria" runat="server">
                                    <asp:TableCell ID="tcGroups" runat="server" VerticalAlign="Middle">
                                        <asp:Label runat="server" ID="Label3" CssClass="LabelNotBold"><%=GetLabel("ub_showgroups") %>:&nbsp</asp:Label>
                                        <ArcoControls:CheckBox runat="server" Checked="true" ID="chckGroups" />
                                    </asp:TableCell>
                                    <asp:TableCell ID="tcUsers" runat="server" VerticalAlign="Middle">
                                        <asp:Label runat="server" ID="Label4" CssClass="LabelNotBold"><%=GetLabel("ub_showusers")%>:&nbsp</asp:Label>
                                        <ArcoControls:CheckBox runat="server" Checked="true" ID="chckUsers" />
                                    </asp:TableCell>
                                    <asp:TableCell ID="tcRoles" runat="server" VerticalAlign="Middle">
                                        <asp:Label runat="server" ID="Label5" CssClass="LabelNotBold"><%=GetLabel("ub_showroles") %>:&nbsp</asp:Label>
                                        <ArcoControls:CheckBox runat="server" Checked="true" ID="chckRoles"  />
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <Arco:ButtonPanel runat="server" ID="btnPanel">
                                            <Arco:SecondaryButton ID="btnFilter" OnClientClick="resetSelectionLeft()" OnClick="filter" runat="server">
                                            </Arco:SecondaryButton>
                                        </Arco:ButtonPanel>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </asp:TableCell>
            </asp:TableRow>
         
        </asp:Table>

        <div class="container-fluid mt-1">
            <div class="row flex-nowrap">
                <div class="col-7 pr-2">
                    <div class="affix">
                        <telerik:RadGrid runat="server" ID="grdSelection" OnNeedDataSource="grdSelection_NeedDataSource"
                            AllowPaging="True" OnRowDrop="dragAndDrop_addMember" ClientSettings-ClientEvents-OnRowDblClick="onRowDoubleClick" AllowMultiRowSelection="true"
                            PageSize="12" AllowSorting="true" HeaderStyle-CssClass="ListHeader" AllowFilteringByColumn="true">
                            <MasterTableView AutoGenerateColumns="false" Width="100%" TableLayout="Auto" CssClass="List">
                                <Columns>
                                    <telerik:GridTemplateColumn Display="false">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="ID" Text='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="25">
                                        <ItemTemplate>
                                            <span class="<%#DataBinder.Eval(Container.DataItem, "ICONCLASS")%>" title="<%#DataBinder.Eval(Container.DataItem, "MEMBERTOOLTIP")%>" />
                                        </ItemTemplate>
                                        <FilterTemplate>
                                            <asp:LinkButton ID="btnShowAll" runat="server" OnClick="btnApplyFilter">
                                            </asp:LinkButton>
                                        </FilterTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn UniqueName="code" DataField="Name" DataType="System.String" Display="true" HeaderText="Name"
                                        ShowFilterIcon="false" AutoPostBackOnFilter="true" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" />

                                    <telerik:GridBoundColumn AllowFiltering="false" UniqueName="description" DataField="Description" DataType="System.String" Display="true" HeaderText="description" ShowFilterIcon="false" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                       
                                    </telerik:GridBoundColumn>

                                    <telerik:GridTemplateColumn AllowFiltering="false">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="tenant" Text='<%#GetTenantName(CInt(DataBinder.Eval(Container.DataItem, "TenantId")))%>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>

                                </Columns>
                                <NoRecordsTemplate>
                                    <div style="height: 30px; cursor: pointer;">
                                        <%=GetLabel("noresultsfound")%>
                                    </div>
                                </NoRecordsTemplate>
                            </MasterTableView>
                            <ClientSettings AllowRowsDragDrop="True">
                                <Selecting AllowRowSelect="True" EnableDragToSelectRows="true" />
                                <ClientEvents OnRowDropping="onRowDropping" OnRowSelected="checkSelectionLeft" />
                                <Scrolling AllowScroll="False" UseStaticHeaders="true" />
                            </ClientSettings>
                            <PagerStyle Mode="NumericPages" PageButtonCount="4" />
                        </telerik:RadGrid>
                        <asp:Label ID="lblWarning" runat="server"><%=GetLabel("ub_toomanyusers") %> </asp:Label>
                    </div>
                </div>
                <div class="sidebar-outer pr-0 pl-2">
                    <div class="sidebar">
                        <telerik:RadAjaxPanel ID="buttons" runat="server">
                            <asp:LinkButton ID="ADD" runat="server" ToolTip="Add" OnClick="btnAdd" Style="margin-right: 3px;" />
                            <asp:LinkButton ID="REMOVE" runat="server" ToolTip="Remove" OnClick="btnRemove" />
                        </telerik:RadAjaxPanel>
                    </div>
                </div>
                <div class="col-4 px-2">
                    <div class="affix">
                        <telerik:RadGrid runat="server" AllowPaging="True" ID="grdMembers" OnNeedDataSource="grdMembers_NeedDataSource"
                            OnRowDrop="dragAndDrop_removeMember" ClientSettings-ClientEvents-OnRowDblClick="onRowDoubleClick" AllowMultiRowSelection="true" PageSize="12" AllowSorting="true">
                            <MasterTableView DataKeyNames="ID" AutoGenerateColumns="false" Width="100%" HeaderStyle-CssClass="ListHeader">
                                <Columns>
                                    <telerik:GridTemplateColumn Display="false">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="ID" Text='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderStyle-Width="25">
                                        <ItemTemplate>
                                            <span class="<%#DataBinder.Eval(Container.DataItem, "ICONCLASS")%>" title="<%#DataBinder.Eval(Container.DataItem, "MEMBERTOOLTIP")%>" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn UniqueName="code" DataField="Name" DataType="System.String" Display="true" HeaderText="Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" />

                                </Columns>
                                <NoRecordsTemplate>
                                    <div style="height: 30px; cursor: pointer;">
                                        <%=GetLabel("noresultsfound")%>
                                    </div>
                                </NoRecordsTemplate>
                                <PagerStyle Mode="NumericPages" PageButtonCount="4" />
                            </MasterTableView>
                            <ClientSettings AllowRowsDragDrop="True">
                                <Selecting AllowRowSelect="True" EnableDragToSelectRows="false" />
                                <ClientEvents OnRowDropping="onRowDropping" OnRowSelected="checkSelectionRight" />
                            </ClientSettings>
                        </telerik:RadGrid>
                    </div>
                </div>
            </div>
            <div class="exMessage" runat="server" id="msg" visible="false" enableviewstate="false">
                Rol succesvol verwijderd!
            </div>
            <div class="row" runat="server" id="closeButtonRow">
                <div class="col-12 pt-3">
                    <Arco:OkButton ID="CloseWindowBtn" OnClientClick="javascript:Close();" runat="server" Text="Close"></Arco:OkButton>
                </div>
            </div>
        </div>

    </form>
</body>
</html>
