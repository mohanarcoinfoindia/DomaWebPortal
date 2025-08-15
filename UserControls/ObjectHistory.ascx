<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ObjectHistory.ascx.vb" Inherits="UserControls_ObjectHistory" Strict="false" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<script type="text/javascript">
    var <%=Me.ClientID %>_LastPage = <%=LastPage%>;
    var <%=Me.ClientID %>_orderbyField = '<%=orderby.name %>';
    var <%=Me.ClientID %>_orderbyorderField = '<%=orderbyorder.name %>';

            function <%=Me.ClientID %>_Goto(page) {
                if (((parseInt(page) <= <%=Me.ClientID %>_LastPage) && (parseInt(page) > 0)) || (page == 1)) {
                    document.forms[0].Page.value = page;
                    Reload();
                }
            }

            function <%=Me.ClientID %>_OrderBy(field) {
                if (field == document.forms[0].elements[<%=Me.ClientID %>_orderbyField].value) {
                    if (document.forms[0].elements[<%=Me.ClientID %>_orderbyorderField].value == "DESC") {
                        document.forms[0].elements[<%=Me.ClientID %>_orderbyorderField].value = "ASC";
                    }
                    else {
                        document.forms[0].elements[<%=Me.ClientID %>_orderbyorderField].value = "DESC";
                    }
                }
                else {
                    document.forms[0].elements[<%=Me.ClientID %>_orderbyorderField].value = "ASC";
                }
                document.forms[0].elements[<%=Me.ClientID %>_orderbyField].value = field;
                Reload();
            }

            function Reload() {
                document.forms[0].submit();
            }
            function OpenCaseHistory(id) {
                var addr = '<%=msroutinglink %>' + id;
                var w = window.open(addr, 'History', 'height=600,width=800,scrollbars=yes,resizable=yes,status=yes');
                w.focus();
            }
            function OpenFileHistory(fin) {
                var addr = 'DM_ObjectHistory.aspx?hidetabs=Y&DM_OBJECT_DIN=<%=DIN%>&SUB_OBJ_DIN=' + fin;
                var w = window.open(addr, 'History', 'height=600,width=800,scrollbars=yes,resizable=yes,status=yes');
                w.focus();
            }

</script>
<input type="hidden" id="Page" name="Page" />
<input type="hidden" name="orderby" id="orderby" runat="server" />
<input type="hidden" name="orderbyorder" id="orderbyorder" runat="server" />
<table class="List PaddedTable">
    <tr style="cursor: pointer">
        <th onclick="javascript:<%=Me.ClientID %>_OrderBy('USER_ID');"><%=GetLabel("user")%></th>
        <th onclick="javascript:<%=Me.ClientID %>_OrderBy('ACTION_DATE');"><%=Getlabel("date") %></th>
        <th onclick="javascript:<%=Me.ClientID %>_OrderBy('OBJ_VERSION');"><%=Getlabel("version") %></th>
        <th onclick="javascript:<%=Me.ClientID %>_OrderBy('MODULE');"><%=GetLabel("module")%></th>
        <th>&nbsp;</th>
        <th>&nbsp;</th>
    </tr>

    <asp:Repeater ID="lstHistory" runat="server">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%#FormatUserName(CType(Container, RepeaterItem).DataItem)%>  </td>
                <td>
                    <nobr><%#ArcoFormatting.FormatDateLabel(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "AuditDate").ToString, True, True, True)%></nobr>
                </td>
                <td><%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Version")%></td>
                <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Module"))%></td>
                <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Description"))%></td>
                <td>
                    <%#ExtraLink(CType(Container, RepeaterItem).DataItem)%>  
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
        </FooterTemplate>
    </asp:Repeater>
    <tr class="ListFooter">
        <td colspan="6">

            <%if NumberOfResults > 0 then%>

            <asp:HyperLink runat="server" ID="lnkPrev" SkinID="PrevPage"></asp:HyperLink>
            <asp:Literal ID="litScroller" runat="server" />
            <asp:HyperLink runat="server" ID="lnkNext" SkinID="NextPage"></asp:HyperLink>

            <%If NumberOfResults > RecordsPerPage Then%>
		                 &nbsp;&nbsp;<%="(" & NumberOfResults & " " & GetLabel("resultsfound") & ")"%>
            <%end if %>

            <%else %>
            &nbsp;
            <%end if%>
        </td>
    </tr>
  
</table>


  <%=GetLabel("recsperpage") %>
<asp:DropDownList ID="drpRecsPerPage" runat="server" AutoPostBack="true">
    <asp:ListItem Value="30" Selected="true">30</asp:ListItem>
     <asp:ListItem Value="100">100</asp:ListItem>
     <asp:ListItem Value="500">500</asp:ListItem>
</asp:DropDownList>
&nbsp;
 <ArcoControls:CheckBox ID="chkShowDetails" runat="server" AutoPostBack="true" Text="Show Details"  />





