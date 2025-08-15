<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Mainpages.aspx.vb" Inherits="Mainpages" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server" defaultbutton="lnkSearch">
        <script type="text/javascript">
            var orderbyField = '<%=orderby.Name %>';
            var orderbyorderField = '<%=orderbyorder.Name %>';
            var LastPage = <%=LastPage%>;

            function DeletePage(id, name, e) {
                if (confirm(UIMessages[29].replace("#ITEM#", name))) {
                    $get('<%=PageIDToDelete.ClientID%>').value = id;
                    Refresh();
                }
                e = e || window.event;
                e.stopPropagation ? e.stopPropagation() : (e.cancelBubble = true);
            }

            function Refresh() {
                document.forms[0].submit();
            }
            function EditPage(id) {
                var oWnd2 = radopen("MainPageEdit.aspx?id=" + id + "&modal=Y", "Popup", 800, 600);
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

        <asp:LinkButton ID="lnkSearch" runat="server"></asp:LinkButton>

        <asp:HiddenField ID="PageIDToDelete" runat="server" />
        <input type="hidden" id="Page" name="Page" />
        <input type="hidden" name="orderby" id="orderby" runat="server" />
        <input type="hidden" name="orderbyorder" id="orderbyorder" runat="server" />

        <telerik:RadScriptManager runat="server" ID="scriptManager" />
        <telerik:RadWindowManager ID="RadWindowManager1" ShowContentDuringLoad="false" VisibleStatusbar="false" Modal="true" ReloadOnShow="true" runat="server">
            <Windows>
                <telerik:RadWindow runat="server" ID="Popup" ShowContentDuringLoad="false" VisibleStatusbar="false" Behaviors="Close" />
            </Windows>
        </telerik:RadWindowManager>

        <table class="List HoverList PaddedTable StickyHeaders" id="Table3">
            <asp:Repeater ID="repPages" runat="server">
                <HeaderTemplate>
                    <tr class="ListHeader">                      
                        <th onclick="javascript:OrderBy('NAME');" style="width: 200px; cursor: pointer;"><%#GetLabel("name")%></th>
                        <th onclick="javascript:OrderBy('DESCRIPTION');" style="cursor: pointer;"><%#GetLabel("description")%></th>
                        <th onclick="javascript:OrderBy('QUERYSTRING');" style="cursor: pointer;">Querystring</th>
                        <th style="width: 16px;"></th>
                    </tr>

                    <tr class="ListFilter">
                        <td>
                            <input type="text" id="txtNameFilter" name="txtNameFilter" value="<%=Request("txtNameFilter") %>" width="200px" /></td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                    </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr style="cursor: pointer" onclick="EditPage('<%#DataBinder.Eval(Container.DataItem, "ID")%>')">                       
                        <td><%#DataBinder.Eval(Container.DataItem, "Name")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "Description")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "QueryString")%></td>

                        <td>
                            <%#"<span class='icon icon-delete' onclick='DeletePage(" & DataBinder.Eval(Container.DataItem, "ID").ToString & "," & EncodingUtils.EncodeJsString(Eval("Name").ToString) & ")' title='" & GetLabel("delete") & "' />"%>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    <tr class="ListFooter">
                        <td colspan="7">
                            <%= ShowPaging()%>    
                        </td>
                    </tr>
                    <tr class="ListFooter" valign="top" id="td_actions" runat="server">
                        <td colspan="7">
                            <div class="SubListHeader3RightFillerDivSwitched" id="div_del_actions" runat="server">
                                <div class="rounded-headerR">
                                    <div class="rounded-headerL">
                                        <div class="SubListMainHeaderT">
                                            <span runat="server" id="del_add" class="icon icon-add-new" onclick="javascript:EditPage(0);"></span>
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
