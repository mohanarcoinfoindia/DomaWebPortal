<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DM_UserBrowser_Results.ascx.vb" Inherits="DM_UserBrowser_Results" Strict="false" %>
<script type="text/javascript">
    var LastPage = <%=LastPage%>;
    var orderbyField = '<%=orderby.name %>';
    var orderbyorderField = '<%=orderbyorder.name %>';

    function Goto(page) {
        if (((parseInt(page) <= LastPage) && (parseInt(page) > 0)) || (page == 1)) {
            $get("<%=hdnPage.ClientID%>").value = page;
            Reload();
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
        Reload();
    }

    function Reload() {
        document.forms[0].submit();
    }
    function ViewUser(login) {
        location.href = 'DM_UserBrowser_AddUser.aspx?login=' + login + '&new=false&rnd_str=' + Math.random();;
        //parent.ShowFile(url, false, login);          
        //PC().OpenDetailWindow('DM_UserBrowser_usersettings.aspx?new=false&login=' + login +'', false, 700, 600, true, true);
    }
    function AddUser() {
        location.href = 'DM_UserBrowser_AddUser.aspx?new=true&rnd_str=' + Math.random();;
    }
    function passResult(login, display) {
        if (!display) {
            display = login;
        }

        window.opener.document.getElementById(document.getElementById('USER_DISPLAY_NAME_RES').value).value = display;
        window.opener.document.getElementById(document.getElementById('USER_LOGIN_RES').value).value = login;
        self.close();
    }

    function Delete(user, e) {
        if (confirm(UIMessages[29].replace("#ITEM#", user))) {
            document.getElementById('action').value = 'D';
            document.getElementById('USER_LOGIN_RES').value = user;
            Reload();
        }

        e = e || window.event;
        e.stopPropagation ? e.stopPropagation() : (e.cancelBubble = true);


    }
</script>

<input type="hidden" id="hdnPage" name="hdnPage" runat="server" />
<input type="hidden" name="orderby" id="orderby" runat="server" />
<input type="hidden" name="orderbyorder" id="orderbyorder" runat="server" />

<table class="List HoverList PaddedTable StickyHeaders">
    <tr class="ListHeader" style="cursor: pointer">
        <th onclick="javascript:OrderBy('USER_DISPLAY_NAME');"><%=GetLabel("ub_displayname")%></th>
        <th onclick="javascript:OrderBy('USER_LOGIN');"><%=Getlabel("ub_loginname") %></th>
        <%If HasMultipleDomains Then%>
        <th onclick="javascript:OrderBy('USER_LOGIN');"><%=Getlabel("domain") %></th>
        <%end if %>
        <th onclick="javascript:OrderBy('USER_MAIL');"><%=Getlabel("mail") %></th>
        <th onclick="javascript:OrderBy('c');">Status</th>
        <%If MultiTenant Then%>
        <th><%=Getlabel("tenant") %></th>
        <%end if %>
        <th onclick="javascript:OrderBy('LAST_LOGIN');"><%=GetLabel("lastlogin") %></th>
        <%If HasSyncedUsers Then%>
        <th onclick="javascript:OrderBy('LAST_SYNCED');"><%=GetLabel("lastsynced")%></th>
        <%end If %>
        <th>&nbsp;</th>
    </tr>
    <tr class="ListFilter">
        <td>
            <asp:TextBox runat="server" ID="txtDisplayName" /></td>
        <td>
            <asp:TextBox runat="server" ID="txtLoginName" /></td>
        <td>
            <asp:DropDownList runat="server" ID="drpDomain" AutoPostBack="true">
                <asp:ListItem Text=""></asp:ListItem>
            </asp:DropDownList>
        </td>
        <td>
            <asp:TextBox runat="server" ID="txtEmail" /></td>
        <%If HasMultipleDomains Then%>
        <td>
            <asp:DropDownList runat="server" ID="drpdwnStatus" AutoPostBack="true">
                <asp:ListItem Text=""></asp:ListItem>
                <asp:ListItem Value="Valid"></asp:ListItem>
                <asp:ListItem Value="Blocked"></asp:ListItem>
            </asp:DropDownList>
        </td>
        <%end if %>
        <%If MultiTenant Then%>
        <td>
            <asp:DropDownList runat="server" ID="drpTenant" AutoPostBack="true">
                <asp:ListItem Text=""></asp:ListItem>
            </asp:DropDownList>
        </td>
        <%end if %>
        <td>&nbsp;</td>

        <%If HasSyncedUsers Then%>
        <td>&nbsp;</td>
        <%end if %>
        <td>&nbsp;</td>
    </tr>

    <asp:Repeater ID="lstHistory" runat="server">

        <ItemTemplate>
            <tr style="cursor: pointer" onclick='<%#If(InSelectionMode,"javascript:passResult(" & EncodingUtils.EncodeJsString(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "USER_LOGIN")) & "," & EncodingUtils.EncodeJsString(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "USER_DISPLAY_NAME")) & ")","javascript:ViewUser(" & EncodingUtils.EncodeJsString(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "USER_LOGIN")) & ")")%>'>
                <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "USER_DISPLAY_NAME"))%> </td>
                <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "USER_LOGIN"))%></td>
                <%If HasMultipleDomains Then%>
                <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Domain"))%></td>
                <%end if %>
                <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "USER_MAIL"))%></td>
                <td><%#getStatus(Convert.ToInt32(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "USER_STATUS")))%></td>
                <%If MultiTenant Then%>
                <td><%#GetTenantLabel(Container.DataItem)%></td>
                <%end if %>
                <td><%#ArcoFormatting.FormatDateLabel(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "LastLogin"), True, True, False)%></td>
                <%If HasSyncedUsers Then%>
                <td><%#ArcoFormatting.FormatDateLabel(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "LastSynced"), True, True, False)%>
                </td>
                <%end if %>
                <td>
                    <%#If(Not CanDelete(Container.DataItem), "", "<span class='icon icon-delete' onclick='Delete(" & EncodingUtils.EncodeJsString(Eval("USER_LOGIN").ToString) & ")' title='" & GetLabel("delete") & "' />")%> 
                </td>
            </tr>
        </ItemTemplate>

        <FooterTemplate>
            <%if NumberOfResults > 0 then%>
            <tr class="ListFooter">
                <td colspan="<%=ColSpan %>">
                    <%= ShowPaging()%>                        		             
                </td>
            </tr>
            <%else %>
            <%If InSelectionMode Then%>
            <tr class="ListFooter">
                <td colspan="<%=ColSpan %>"></td>
            </tr>
            <%end if%>
            <%end if%>
            <%If Not InSelectionMode Then%>
            <tr class="ListFooter">
                <td colspan="<%=ColSpan %>">
                    <span class="icon icon-add-new" style="cursor:pointer; " title="<%=GetLabel("add")%>" onclick="javascript:AddUser();" ></span>
                </td>
            </tr>
            <%end If %>
        </FooterTemplate>
    </asp:Repeater>
</table>


