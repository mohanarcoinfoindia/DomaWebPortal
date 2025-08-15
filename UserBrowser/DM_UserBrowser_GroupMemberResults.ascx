<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DM_UserBrowser_GroupMemberResults.ascx.vb" Inherits="DM_UserBrowser_GroupMemberResults" Strict="false" %>
<br />
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
            function OpenUser(login) {
                location.href = "DM_UserBrowser_AddUser.aspx?login=" + login + "&new=false";
                //window.open(url+'?new=false&login='+login,'_blank');
            }
</script>
<input type="hidden" id="Page" name="Page" />
<input type="hidden" name="orderby" id="orderby" runat="server" />
<input type="hidden" name="orderbyorder" id="orderbyorder" runat="server" />
<asp:Label ID="Result2" runat="server" />
<asp:Repeater ID="lstGroups" runat="server">
    <HeaderTemplate>
        <table class="List">
            <tr>
                <th onclick="javascript:OrderBy('MEMBER');" style="cursor: pointer"><%=GetLabel("ub_displayname")%></th>
                <th onclick="javascript:OrderBy('MEMBER');" style="cursor: pointer">Type</th>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td style="cursor: pointer" onclick='javascript:OpenUser(<%#EncodingUtils.EncodeJsString(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "MEMBER"))%>);'><%#ArcoFormatting.FormatUserName(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "MEMBER"))%></td>
            <td><%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "MEMBERTYPE")%></td>
        </tr>
    </ItemTemplate>
</asp:Repeater>


