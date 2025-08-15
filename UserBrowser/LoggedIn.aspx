<%@ Page Language="VB" AutoEventWireup="false" CodeFile="LoggedIn.aspx.vb" Inherits="UserBrowser_LoggedIn" Strict="false" MasterPageFile="~/masterpages/Empty.master" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <script type="text/javascript">
        function Refresh() {
            document.forms[0].submit();
        }

        var orderbyField = '<%=orderby.name %>';
        var orderbyorderField = '<%=orderbyorder.name %>';

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
</asp:Content>
<asp:Content ContentPlaceHolderID="Plh1" runat="server">
    <asp:HiddenField ID="hdnLogin" runat="server" />
    <asp:HiddenField ID="hdnToken" runat="server" />
    <asp:HiddenField ID="hdnApp" runat="server" />
    <input type="hidden" name="orderby" id="orderby" runat="server" />
    <input type="hidden" name="orderbyorder" id="orderbyorder" runat="server" />

    <asp:Panel ID="pnlInfo" CssClass="ErrorLabel" runat="server">
        <asp:Label runat="server" ID="lblInfo"></asp:Label>
    </asp:Panel>

    <table class="List PaddedTable StickyHeaders">
        <tr class="ListHeader" style="cursor: pointer">
            <th onclick="javascript:OrderBy('USER_LOGIN');"><%=GetLabel("ub_loginname")%></th>
            <th onclick="javascript:OrderBy('MODULE');"><%=GetLabel("module")%></th>
            <%If MultiTenant Then%>
            <th onclick="javascript:OrderBy('TENANT_ID');"><%=Getlabel("tenant") %></th>
            <%end if %>
            <th onclick="javascript:OrderBy('START_DATE');"><%=GetLabel("del_begin")%></th>
            <th onclick="javascript:OrderBy('LAST_ACTION');"><%=GetLabel("lastaction")%></th>
            <th onclick="javascript:OrderBy('USER_AGENT');">User Agent</th>
            <th onclick="javascript:OrderBy('REMOTE_ADDRESS');">IP</th>
            <th></th>
        </tr>
        <asp:Repeater ID="lstUsers" runat="server">
            <HeaderTemplate></HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td><%#FormatUserName(CType(Container, RepeaterItem).DataItem)%> </td>
                    <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Application").ToString())%></td>
                    <%If MultiTenant Then%>
                    <td><%#GetTenantName(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TenantId"))%></td>
                    <%end If %>
                    <td>
                        <nobr><%#ArcoFormatting.FormatDateLabel(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "StartTime").ToString, True, True, False)%></nobr>
                    </td>
                    <td>
                        <nobr><%#ArcoFormatting.FormatDateLabel(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "LastAction").ToString, True, True, False)%></nobr>
                    </td>
                    <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "UserAgent").ToString())%></td>
                    <td><%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "RemoteAddress").ToString%></td>
                    <td><%#LogOffButton(CType(Container, RepeaterItem).DataItem)%></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></FooterTemplate>
        </asp:Repeater>
        <tr class="ListFooter" runat="server">
            <td colspan="6">
                <div class="rounded-HeaderR">
                    <div class="rounded-HeaderL">
                        <div class="SubListMainHeaderT" runat="server">
                            <a href="javascript:Refresh();" class="ButtonLink">
                                <span class="icon icon-refresh" title="<%=GetLabel("refresh")%>" />&nbsp;
                            <asp:LinkButton ID="LinkButton1" runat="server" Text="Log-off idle users" OnClick="lnkLogoffIdleUsers_Click" />
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
