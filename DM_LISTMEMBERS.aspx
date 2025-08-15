<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_LISTMEMBERS.aspx.vb" Inherits="DM_LISTMEMBERS" Strict="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Members</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Repeater ID="lstMembers" runat="server">
                <HeaderTemplate>
                    <table class="List PaddedTable">
                        <tr>
                            <th><%=Getlabel("name") %></th>
                            <th>Type</th>
                        </tr>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "MEMBER")%></td>
                        <td><%#Getlabel(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "MEMBERTYPE"))%></td>
                    </tr>

                </ItemTemplate>
                <FooterTemplate>
                    <tr class="ListFooter">
                        <td colspan="2" style="text-align:right;margin:5px;">
                            <a href="javascript:window.close();" class="ButtonLink" title="<%=GetLabel("close") %>"><%=GetLabel("close") %></a>
                        </td>
                    </tr>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </form>
</body>
</html>
