<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Sitemap.aspx.vb" Inherits="UserBrowser_Sitemap" %>

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

            function DeleteEntry(id, name, e) {
                if (confirm(UIMessages[29].replace("#ITEM#", name))) {
                    $get('<%=EntryIDToDelete.ClientID%>').value = id;
                    Refresh();
                }
                e = e || window.event;
                e.stopPropagation ? e.stopPropagation() : (e.cancelBubble = true);
            }

            function Refresh() {
                document.forms[0].submit();
            }
            function EditEntry(id) {
                var oWnd2 = radopen("SitemapEntry.aspx?id=" + id + "&modal=Y", "Popup", 800, 600);
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

        <asp:HiddenField ID="EntryIDToDelete" runat="server" />
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
              <tr class="ListHeader">
                   <%if ShowSiteVersion %>
                        <th onclick="javascript:OrderBy('SITE_VERSION');" style="cursor: pointer;">Site Version</th>
                  <%End if %>
                        <th onclick="javascript:OrderBy('SEQ');" style="cursor: pointer;">Seq.</th>
                        <th onclick="javascript:OrderBy('ID');" style="cursor: pointer;">ID</th>
                        <th onclick="javascript:OrderBy('PARENT_ID');" style="cursor: pointer;">Parent ID</th>
                            <th onclick="javascript:OrderBy('APP_GROUP');" style="width: 200px; cursor: pointer;"><%=GetLabel("group")%></th>
                        <th onclick="javascript:OrderBy('NAME');" style="width: 200px; cursor: pointer;"><%=GetLabel("name")%></th>
                        <th onclick="javascript:OrderBy('DESCRIPTION');" style="cursor: pointer;"><%=GetLabel("description")%></th>
                        <th onclick="javascript:OrderBy('URL');" style="cursor: pointer;"><%=GetLabel("url")%></th>
                        <th style="width: 16px;"></th>
                    </tr>

                    <tr class="ListFilter">
                        <%if ShowSiteVersion %>
                        <td>
                            <asp:DropDownList runat="server" ID="drpSiteVersion" AutoPostBack="true">
                                <asp:ListItem Text=""></asp:ListItem>
                                <asp:ListItem Value="7">Version 7</asp:ListItem>
                                <asp:ListItem Value="8">Version 8</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <%end If%>
                        <td>&nbsp;</td>
                        <td>&nbsp;</td>
                        <td> 
                            <asp:DropDownList runat="server" ID="drpParentId" AutoPostBack="true">
                              
                            </asp:DropDownList>

                        </td>
                         <td>
                             <asp:TextBox runat="server" ID="txtGroupFilter" />
                        </td>
                        <td>
                             <asp:TextBox runat="server" ID="txtNameFilter" />
                        </td>
                        <td>
                             <asp:TextBox runat="server" ID="txtDescFilter" />
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtUrlFilter" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>

            <asp:Repeater ID="repEntries" runat="server">
               
                <ItemTemplate>
                    <tr style="cursor: pointer" onclick="EditEntry('<%#DataBinder.Eval(Container.DataItem, "ID")%>')">
                         <%if ShowSiteVersion %>
                         <td><%#DataBinder.Eval(Container.DataItem, "SiteVersion")%></td>
                        <%End if %>
                        <td><%#DataBinder.Eval(Container.DataItem, "Sequence")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "ID")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "ParentId")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "Group")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "Title")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "Description")%></td>
                        <td><%#DataBinder.Eval(Container.DataItem, "Url")%></td>

                        <td>
                            <%#"<span class='icon icon-delete' onclick='DeleteEntry(" & DataBinder.Eval(Container.DataItem, "ID").ToString & "," & EncodingUtils.EncodeJsString(Eval("Title").ToString) & ")' title='" & GetLabel("delete") & "' />"%>
                        </td>
                    </tr>
                </ItemTemplate>
              
            </asp:Repeater>

              <tr class="ListFooter">
                        <td colspan="8">
                            <%= ShowPaging()%>    
                        </td>
                    </tr>
                    <tr class="ListFooter" valign="top" id="td_actions" runat="server">
                        <td colspan="8">
                            <div class="SubListHeader3RightFillerDivSwitched" id="div_del_actions" runat="server">
                                <div class="rounded-headerR">
                                    <div class="rounded-headerL">
                                        <div class="SubListMainHeaderT">
                                            <span runat="server" id="del_add" class="icon icon-add-new" onclick="javascript:EditEntry(0);"></span>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
        </table>
    </form>
</body>
</html>
