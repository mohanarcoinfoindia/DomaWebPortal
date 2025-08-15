<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DM_UserBrowser_RolesResult.ascx.vb" Inherits="DM_UserBrowser_RolesResult" Strict="false" %>
<script type="text/javascript">
    var LastPage = <%=LastPage%>;
    var orderbyField = '<%=orderby.name %>';
    var orderbyorderField = '<%=orderbyorder.name %>';

    function Goto(page) {
        if (((parseInt(page) <= LastPage) && (parseInt(page) > 0)) || (page == 1)) {
            document.forms[0].Page.value = page;
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

<%If InSelectionMode Then%>
    function passResult(roleid, rolename) {
        window.opener.document.getElementById(document.getElementById('ROLE_ID_RES').value).value = roleid;
        window.opener.document.getElementById(document.getElementById('ROLE_NAME_RES').value).value = rolename;
        self.close();
    }
<%else%>

    function Delete(user, rolename, e) {        
        var txtRName = document.getElementById('<%=txtRoleName.ClientID%>').value;
        var txtRDesc = document.getElementById('<%=txtRoleDescription.ClientID%>').value;
        var txtRoleType = document.getElementById('txtRoleType').value;
        if (confirm(UIMessages[29].replace("#ITEM#", rolename))) {
            location.href = 'DM_UserBrowser_Roles.aspx?txtRoleName=' + encodeURIComponent(txtRName) + '&txtRoleDesc=' + encodeURIComponent(txtRDesc) + '&roletype=' + txtRoleType + '&roleid=' + encodeURIComponent(user) + '&delete=1';
        }
        e = e || window.event;
        e.stopPropagation ? e.stopPropagation() : (e.cancelBubble = true);
    }

    function OpenRole(roleid) {
        location.href = 'DM_UserBrowser_RoleDetail.aspx?new=false&login=' + encodeURIComponent(roleid) + '&rnd_str=' + Math.random();
    }

    function AddRole() {
        location.href = 'DM_UserBrowser_RoleDetail.aspx?new=true&rnd_str=' + Math.random();
    }

            <%End If%>
</script>
<input type="hidden" id="Page" name="Page" />
<input type="hidden" name="orderby" id="orderby" runat="server" />
<input type="hidden" name="orderbyorder" id="orderbyorder" runat="server" />
<asp:Label ID="Result2" runat="server" />

<table class="List HoverList PaddedTable StickyHeaders">
    <tr class="ListHeader" style="cursor: pointer">
        <th onclick="javascript:OrderBy('ROLE_NAME');"><%=GetLabel("ub_rolename")%></th>
        <th onclick="javascript:OrderBy('ROLE_DESCRIPTION');"><%=GetLabel("ub_roledesc")%></th>
        <%If MultiTenant Then%>
        <th onclick="javascript:OrderBy('TENANT_ID');"><%=GetLabel("tenant")%></th>
        <%End If%>
        <th onclick="javascript:OrderBy('ROLE_STRUCTURED');"><%=GetLabel("structuredrole")%></th>
        <% If CanDelete then %>
        <th>&nbsp;</th>
        <%end if %>
    </tr>

    <tr class="ListFilter">
        <td>
            <asp:TextBox ID="txtRoleName" runat="server" />           
        <td>
             <asp:TextBox ID="txtRoleDescription" runat="server" />                  
        </td>
        <%If MultiTenant Then%>
        <td>
            <asp:DropDownList runat="server" ID="drpTenant" AutoPostBack="true">
                <asp:ListItem Text=""></asp:ListItem>
            </asp:DropDownList>

        </td>
        <%  End If %>
        <td>&nbsp;</td>
        <% If CanDelete then %>
        <td>&nbsp;</td>
        <%end if %>
    </tr>

    <asp:Repeater ID="lstHistory" runat="server">

        <ItemTemplate>
            <tr style="cursor: pointer" onclick='<%# OnClickAction(CType(Container, RepeaterItem).DataItem)%>'>
                <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ROLE_NAME"))%></td>
                <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ROLE_DESCRIPTION"))%></td>
                <%If MultiTenant Then%>
                <td><%#GetTenantLabel(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TenantId"))%></td>
                <%End If%>
                <td><%#GetStructureLabel(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ROLE_STRUCTURED"))%></td>
                <% If CanDelete then %>
                <td>
                    <asp:PlaceHolder ID='plDelete' runat="server" Visible='<%# CanDelete(CType(Container, RepeaterItem).DataItem) %>'>
                        <%# "<span class='icon icon-delete' onclick='Delete(" & Eval("ROLE_ID") & "," & EncodingUtils.EncodeJsString(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ROLE_NAME")) & ")' title='" & GetLabel("delete") & "'/>"%> 
                    </asp:PlaceHolder>
                </td>
                <%end if %>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            <tr class="ListFooter">
                <td colspan="<%=ColSpan%>">
                    <%= ShowPaging()%>    
                </td>
            </tr>
            <%If ShowAddButton() Then%>
            <tr class="ListFooter">
                <td colspan="<%=ColSpan%>">
                    <div class="SubListHeader3RightFillerDivSwitched">
                        <div class="rounded-headerR">
                            <div class="rounded-headerL">
                                <div class="SubListMainHeaderT">
                                    <span class="icon icon-add-new" style="cursor: pointer;" title="<%=GetLabel("add")%>" onclick="javascript:AddRole();" />
                                </div>
                            </div>
                        </div>
                    </div>
                </td>
            </tr>
            <%end If %>
        </FooterTemplate>
    </asp:Repeater>
</table>


