<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_Listaddcategory.aspx.vb"
    Inherits="DM_Catalog_Listaddcategory" ValidateRequest="false" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">    
    <script language="javascript">
        
        function GotoManagedLists() {
            parent.GotoManagedLists();
        }

        function GotoListItems(listid, parent, selectedids) {
            var w = window.open('DM_Catalog_listitems.aspx?id=' + listid + '&parent=' + parent + '&selectedids=' + selectedids + '&rnd_str=' + Math.random() + '', '_self', 'width=600,height=600');
            w.focus();
        }
       
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server">
    <asp:ScriptManager ID="sc1" runat="server"></asp:ScriptManager>
    <asp:HiddenField ID="selectedids" runat="server" />
    <table cellpadding="0" cellspacing="0" style="margin: 1%;">
        <tr class="DetailHeader">
           
            <td class="DetailHeaderContent">
            </td>
            <td class="DetailHeaderContent" align="right">
                <a id="imgbtnBack" href="#" runat="server" visible="false" onserverclick="BacktoListItems">
                    <span class="icon icon-cancel" style="border: 0;"></span>
                </a>
            </td>
           
        </tr>
        <tr>
           
            <td colspan="2">
                <asp:Table runat="server">
                    <asp:TableRow>
                        <asp:TableCell ColumnSpan="3" >
                            <asp:Panel ID="MsgPanel" runat="server" Width="443px" Visible="False" Style="margin-top: 10px;
                                margin-bottom: 10px;">
                                <asp:Label ID="msg_lbl" runat="server" CssClass="ErrorLabel"></asp:Label>            
                            </asp:Panel>
                        </asp:TableCell>
                        <asp:TableCell></asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell Width="200px">
                            <asp:Label ID="lblCategory" class="Label" runat="server" /></asp:TableCell>
                        <asp:TableCell ColumnSpan="2" Width="600px">
                            <asp:DropDownList ID="cmbCategory" runat="server" AutoPostBack="true" EnableViewState="true" OnSelectedIndexChanged="ShowListItemsByCategory" Width="150px">
                            </asp:DropDownList>
                        </asp:TableCell>
                        <asp:TableCell>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow >
                        <asp:TableCell>
                            <asp:Label ID="lblListItem" class="Label" runat="server" />
                        </asp:TableCell>
                        <asp:TableCell ColumnSpan="2">
                            <asp:DropDownList runat="server" ID="cmbListItems" Width="150px" EnableViewState="true">
                            </asp:DropDownList>&nbsp;&nbsp;
                            
                            <asp:LinkButton ID="lnkbtnAdd" runat="server" OnClick="AddCategory" class="ButtonLink" />
                        </asp:TableCell>
                        <asp:TableCell>

                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow>
                        <asp:TableCell ColumnSpan="5">&nbsp;</asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow ID="rowCategoryDependantList">
                        <asp:TableCell VerticalAlign="Top">
                            <asp:Label ID="lblListItems" class="Label" runat="server" /></asp:TableCell>
                        <asp:TableCell ColumnSpan="3">
                            <asp:GridView ID="listItemsGridView" runat="server" Width="50%" AutoGenerateColumns="False" ShowHeader="true"
                                AllowPaging="True" AllowSorting="True">
                                <PagerStyle ForeColor="Blue" BackColor="White"></PagerStyle>
                            </asp:GridView>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </td>           
        </tr>
    </table>
    </form>
    <asp:Literal ID="plValidationScript" runat="server"></asp:Literal>
</body>
</html>
