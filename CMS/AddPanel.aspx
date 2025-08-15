<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddPanel.aspx.vb" Inherits="CMS_AddPanel" MasterPageFile="~/masterpages/Toolwindow.master"%>
 <asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">

  <asp:Table runat="server" ID="configtable" CssClass="DetailTable">
        <asp:TableRow>
            <asp:TableCell CssClass="LabelCell">
                <asp:Label CssClass="Label" Text="Type" runat="server" ID="lblHeader" AssociatedControlID="drpDwnTypes"></asp:Label>
            </asp:TableCell>
            <asp:TableCell CssClass="FieldCell">
                <asp:DropDownList ID="drpDwnTypes" runat="server" Width="200px"/>
            </asp:TableCell>
        </asp:TableRow>

         <asp:TableRow>
            <asp:TableCell CssClass="FieldCell" ColumnSpan="2">
                <Arco:ButtonPanel ID="ButtonPanel1" runat="server">
                    <Arco:OkButton runat="server" ID="btnSave"></Arco:OkButton>
                    <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" id="btnCancel"></Arco:CancelButton>
                </Arco:ButtonPanel>
            </asp:TableCell>

        </asp:TableRow>
    </asp:Table>
 </asp:Content>
