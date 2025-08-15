<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Clients.aspx.vb" Inherits="UserBrowser_Clients" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
    <title></title>
</head>

<body>
    <form id="form1" runat="server" defaultbutton="lnkSearch">
        <asp:LinkButton ID="lnkSearch" runat="server"></asp:LinkButton>
        <style type="text/css">
            .NewButton {
                cursor: pointer;
            }
        </style>
        <script type="text/javascript">
            var orderbyField = '<%=orderby.Name %>';
            var orderbyorderField = '<%=orderbyorder.Name %>';
            var LastPage = <%=LastPage%>;

            function DeleteClient(id, e) {
                if (confirm(UIMessages[4])) {
                    $get('<%=ClientIDToDelete.ClientID%>').value = id;
                    Refresh();
                }
                e = e || window.event;
                e.stopPropagation ? e.stopPropagation() : (e.cancelBubble = true);
            }
            function Refresh() {
                document.forms[0].submit();
            }
            function EditClient(id) {
                var oWnd2 = radopen("EditClient.aspx?id=" + id + "&modal=Y", "Popup", 800, 600);
                oWnd2.add_close(Refresh);
            }
            function Goto(page) {
                if (((parseInt(page) <= LastPage) && (parseInt(page) > 0)) || (page == 1)) {
                    document.forms[0].Page.value = page;
                    Refresh();
                }
            }
            function OrderBy(field) {
                if (field == document.forms[0].elements[orderbyField].value) {
                    if (document.forms[0].elements[orderbyorderField].value == "DESC") {
                        document.forms[0].elements[orderbyorderField].value = "ASC";
                    }
                    else {
                        document.forms[0].elements[orderbyorderField].value = "DESC";
                    }
                }
                else {
                    document.forms[0].elements[orderbyorderField].value = "ASC";
                }
                document.forms[0].elements[orderbyField].value = field;
                Refresh();
            }
        </script>

        <asp:HiddenField ID="ClientIDToDelete" runat="server" />

        <input type="hidden" id="Page" name="Page" />
        <input type="hidden" name="orderby" id="orderby" runat="server" />
        <input type="hidden" name="orderbyorder" id="orderbyorder" runat="server" />

        <telerik:RadScriptManager runat="server" ID="scriptManager" />
        <telerik:RadWindowManager ID="RadWindowManager1" ShowContentDuringLoad="false" VisibleStatusbar="false"
            Modal="true" ReloadOnShow="true" runat="server">
            <Windows>
                <telerik:RadWindow runat="server" ID="Popup" ShowContentDuringLoad="false" VisibleStatusbar="false"
                    Behaviors="Close" />
            </Windows>
        </telerik:RadWindowManager>

        <table class="List HoverList PaddedTable StickyHeaders" id="Table3">
            <asp:Repeater ID="repClients" runat="server">
                <HeaderTemplate>
                    <tr class="ListHeader">
                        <th style="cursor: pointer" onclick="javascript:OrderBy('CLIENTID');">Id</th>
                        <th style="cursor: pointer" onclick="javascript:OrderBy('NAME');"><%#GetLabel("name")%></th>
                        <th><%#GetLabel("url")%>(s)</th>
                        <th></th>
                    </tr>
                    <tr class="ListFilter">
                        <td>&nbsp;</td>
                        <td>
                            <input type="text" id="txtNameFilter" name="txtNameFilter"
                                value="<%=Request("txtNameFilter") %>" /></td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                </HeaderTemplate>

                <ItemTemplate>
                    <tr style="cursor: pointer"
                        onclick="EditClient('<%#DataBinder.Eval(Container.DataItem, "ClientId")%>')">
                        <td><%#:DataBinder.Eval(Container.DataItem, "ClientId")%></td>
                        <td><%#:DataBinder.Eval(Container.DataItem, "Name")%></td>
                        <td><%#:DataBinder.Eval(Container.DataItem, "AllowedCallbacks")%>
                        </td>
                        <td>
                            <span class="icon icon-delete"
                                onclick="DeleteClient('<%#DataBinder.Eval(Container.DataItem, "ClientId")%>')"
                                title="<%#GetLabel("delete")%>" />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <tr class='ListFooter'>
                        <td colspan="4">
                            <%= ShowPaging()%>
                        </td>
                    </tr>
                    <tr class="ListFooter">
                        <td colspan="4">
                            <div class="SubListHeader3RightFillerDivSwitched" id="div_del_actions" runat="server">
                                <div class="rounded-headerR">
                                    <div class="rounded-headerL">
                                        <div class="SubListMainHeaderT">
                                            <span class="icon icon-add-new" onclick="javascript:AddClient();" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                </FooterTemplate>
            </asp:Repeater>
        </table>
    </form>


</body>

</html>
