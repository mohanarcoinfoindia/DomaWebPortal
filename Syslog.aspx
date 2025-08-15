<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Syslog.aspx.vb" Inherits="Syslog" Strict="false" %>

<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">           
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <script type="text/javascript">
            var LastPage = <%=LastPage%>;
            var orderbyField = '<%=orderby.name %>';
            var orderbyorderField = '<%=orderbyorder.name %>';
        </script>
        <Arco:PageController ID="thisPageController" runat="server" PageLocation="ListPage" />

        <input type="hidden" id="Page" name="Page" />
        <input type="hidden" name="orderby" id="orderby" runat="server" />
        <input type="hidden" name="orderbyorder" id="orderbyorder" runat="server" />

        <table class="List PaddedTable StickyHeaders">
            <tr class="ListHeader">
                <th onclick="javascript:OrderBy('LOG_LEVEL');" style="cursor: pointer">Level
                </th>
                <th onclick="javascript:OrderBy('LOG_TIME');" style="cursor: pointer"><%=Getlabel("date") %></th>
                <th onclick="javascript:OrderBy('BATCH_ID');" style="cursor: pointer">Job</th>
                <th onclick="javascript:OrderBy('LOG_USER');" style="cursor: pointer"><%=GetLabel("user") %></th>
                <th></th>
                <th></th>
            </tr>
            <tr class='ListFilter'>
                <td>
                    <asp:DropDownList ID="drpLevel" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="drpTime" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:DropDownList ID="drpJob" runat="server" AutoPostBack="true">
                    </asp:DropDownList>
                </td>
                <td></td>
                <td></td>
                <td></td>
            </tr>
            <asp:Repeater ID="lstResults" runat="server" EnableViewState="false">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#GetLevel(CType(Container, RepeaterItem).DataItem)%></td>
                        <td><%#ArcoFormatting.FormatDateLabel(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "LogTime").ToString, True, True, False)%></td>
                        <td><%#GetBatchJob(CType(Container, RepeaterItem).DataItem)%></td>
                        <td><%#ArcoFormatting.FormatUserName(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "User"), False, True)%></td>
                        <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Text"))%></td>
                        <td><%#GetObjectLinkUrl(CType(Container, RepeaterItem).DataItem)%></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
            <tr class='ListFooter'>
                <td colspan="6">
                    <%if NumberOfResults = 0 then%>
                    <%=Getlabel("noresultsfound") %>
                    <%else%>

                    <asp:HyperLink runat="server" ID="lnkPrev" SkinID="PrevPage"></asp:HyperLink>
                    <asp:Literal ID="litScroller" runat="server" />
                    <asp:HyperLink runat="server" ID="lnkNext" SkinID="NextPage"></asp:HyperLink>

                    <%If NumberOfResults > RecordsPerPage Then%>
		                &nbsp;&nbsp;<%="(" & NumberOfResultsLabel & " " & GetLabel("resultsfound") & ")"%>
                    <%end if %>

                    <%end if%>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
