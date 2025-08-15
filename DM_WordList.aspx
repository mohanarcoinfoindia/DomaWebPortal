<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_WordList.aspx.vb" Inherits="DM_WordList" MasterPageFile="~/masterpages/Toolwindow.master" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <asp:HiddenField ID="txtOrderBy" runat="server" />
    <asp:Panel ID="pnlSearch" runat="server" Width="100%">
        <div style="text-align: center">
            <fieldset>
                <legend>Search</legend>
                <ol>
                    <li>
                        <asp:TextBox ID="txtSearch" runat="server" Width="300"></asp:TextBox>
                        &nbsp;
                        <asp:LinkButton ID="butSearch" runat="server"><%=GetLabel("search") %></asp:LinkButton>
                    </li>
                    <li></li>
                </ol>
            </fieldset>
        </div>
       
    </asp:Panel>

     <b><%=GetLabel("yourselection") %> : </b><span id="lblSelection" name="lblSelection"></span>

    <div style="text-align: center">
        <asp:PlaceHolder ID="plhPagerTop" runat="server"></asp:PlaceHolder>
    </div>

    <table class="List PaddedTable">
        <tr>
            <th>&nbsp;</th>
            <th>
                <asp:LinkButton ID="lnkOrderTerm" runat="server" OnClick="onOrderTerm" Text="Word"></asp:LinkButton>
            </th>
            <th>
                <asp:LinkButton ID="lnkOrderCount" runat="server" OnClick="onOrderCount" Text="#"></asp:LinkButton>
            </th>
        </tr>
    </table>

    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" ShowHeader="false" GridLines="None" EmptyDataText="<center>No results found</center>" AllowPaging="false">
        <Columns>
            <asp:TemplateField ItemStyle-Width="20px">
                <ItemTemplate>
                    <asp:Label ID="lblHeader" runat="server"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField>
                <ItemTemplate>
                    <asp:HyperLink ID="HyperLink1" runat="server"></asp:HyperLink>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-Width="20px">
                <ItemTemplate>
                    <asp:Label ID="lblCount" runat="server"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>


    </asp:GridView>

    <div style="text-align: center">
        <asp:PlaceHolder ID="plhPager" runat="server"></asp:PlaceHolder>
    </div>

    <asp:Table Width="100%" runat="server" ID="tblFooter">
        <asp:TableRow>
            <asp:TableCell HorizontalAlign="left" Width="50%">
                <asp:Label ID="lblMaxResFound" runat="server" Visible="false" Text="Max Results reached, please refine your search" Font-Size="XX-Small"></asp:Label>
            </asp:TableCell>
            <asp:TableCell HorizontalAlign="right" Width="50%">
                <asp:LinkButton ID="lnkClear" runat="server" OnClientClick="javascript:SetValue('')" Text="clear"></asp:LinkButton>         
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>

    <script type="text/javascript">
        InitValue();
    </script>
</asp:Content>
