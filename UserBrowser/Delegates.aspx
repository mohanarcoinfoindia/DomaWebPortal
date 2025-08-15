<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Delegates.aspx.vb" Inherits="DelegatesPage" %>

<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<!DOCTYPE html
	PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title></title>
</head>

<body>
    <form id="form1" runat="server" defaultbutton="lnkSearch">
        <asp:LinkButton ID="lnkSearch" runat="server"></asp:LinkButton>
        <script language="javascript" type="text/javascript">
            var orderbyField = '<%=orderby.name %>';
            var orderbyorderField = '<%=orderbyorder.name %>';
            var LastPage = <%=LastPage%>;

            function DeleteDelegate(id, e) {
                if (confirm(UIMessages[4])) {
                    $get('<%=DelegateIDToDelete.ClientID%>').value = id;
                    Refresh();
                }
                e = e || window.event;
                e.stopPropagation ? e.stopPropagation() : (e.cancelBubble = true);
            }
            function Refresh() {
                document.forms[0].submit();
            }
            function EditDelegate(id) {
                const width = document.body.scrollWidth * 0.75;
                const height = document.body.scrollHeight * 0.90;

                var oWnd2 = radopen("EditDelegate.aspx?id=" + id, "Popup", width, height);
                // oWnd2.fullScreen = true;
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

        <asp:HiddenField ID="DelegateIDToDelete" runat="server" />
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
            <thead>
                <tr valign="top" style="border-bottom: none;">
                    <td valign="top" id="td_del_activeOnly" runat="server">
                        <div id="div_del_activeOnly" runat="server">
                            <ArcoControls:CheckBox ID="chkActiveOnly" runat="server" AutoPostBack="true" />
                        </div>
                    </td>
                </tr>
                <tr class="ListHeader">
                    <th onclick="javascript:OrderBy('DELEGATE_FROM_ID');" style="cursor: pointer">
                        <%=GetLabel("del_from")%>
                    </th>
                    <th onclick="javascript:OrderBy('SUBJECT_ID');" style="cursor: pointer">
                        <%=GetLabel("del_to")%>
                    </th>
                    <th onclick="javascript:OrderBy('OBJECT_ID');" style="cursor: pointer">
                        <%=GetLabel("procedure")%>
                    </th>
                    <%If MultiTenant Then%>
                    <th onclick="javascript:OrderBy('TENANT_ID');" style="cursor: pointer">
                        <%=GetLabel("tenant")%>
                    </th>
                    <%End If%>
                    <th onclick="javascript:OrderBy('EXCEPTION_MODE');" style="cursor: pointer">
                        <%=GetLabel("del_mode")%>
                    </th>
                    <th onclick="javascript:OrderBy('START_DATE');" style="cursor: pointer">
                        <%=GetLabel("del_begin")%>
                    </th>
                    <th onclick="javascript:OrderBy('END_DATE');" style="cursor: pointer"><%=GetLabel("del_end")%>
                    </th>
                    <th></th>
                </tr>
                <tr class="ListFilter">
                    <td>
                        <input type="text" id="txtFromFilter" name="txtFromFilter" value="<%=Request("txtFromFilter") %>" />
                    </td>
                    <td>
                        <input type="text" id="txtToFilter" name="txtToFilter" value="<%=Request("txtToFilter") %>" />
                    </td>
                    <td>&nbsp;</td>
                    <%If MultiTenant Then%>
                    <td>
                        <asp:DropDownList runat="server" ID="drpTenant" AutoPostBack="true">
                            <asp:ListItem Text=""></asp:ListItem>
                        </asp:DropDownList>

                    </td>
                    <%end if %>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="repDelegates" runat="server">

                    <ItemTemplate>
                        <tr style="cursor: pointer" onclick="EditDelegate('<%#DataBinder.Eval(Container.DataItem, "ID")%>')">
                            <td><%#DataBinder.Eval(Container.DataItem, "FROM")%></td>
                            <td><%#DataBinder.Eval(Container.DataItem, "TO")%></td>
                            <td><%#DataBinder.Eval(Container.DataItem, "PROCEDURE")%></td>
                            <%If MultiTenant Then%>
                            <td><%#GetTenantName(CInt(DataBinder.Eval(Container.DataItem, "TenantId")))%></td>
                            <%End If%>
                            <td><%#DataBinder.Eval(Container.DataItem, "MODE")%></td>
                            <td><%#DataBinder.Eval(Container.DataItem, "BEGIN")%></td>
                            <td><%#DataBinder.Eval(Container.DataItem, "END")%></td>
                            <td class="iconCell">
                                <span class="icon icon-delete"
                                    onclick="DeleteDelegate('<%#DataBinder.Eval(Container.DataItem, "ID")%>')"
                                    title="<%#GetLabel("delete")%>" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        <tr class='ListFooter'>
                            <td colspan="<%=ColSpan() %>">
                                <%= ShowPaging()%>
                            </td>
                        </tr>
                        <tr class="ListFooter">
                            <td colspan="<%=ColSpan %>">
                                <span class="icon icon-add-new" title="<%#GetLabel("cm_add_new")%>" onclick="javascript:AddDelegate();" />
                            </td>
                        </tr>
                    </FooterTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </form>
</body>

</html>
