<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DeletePanel.aspx.vb" Inherits="CMS_DeletePanel" MasterPageFile="~/masterpages/Toolwindow.master" %>


  <asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
    <asp:Table runat="server" ID="configtable" CssClass="DetailTable" EnableViewState="false">
        <asp:TableRow>           
            <asp:TableCell CssClass="FieldCell">
                <asp:Label runat="server" ID="lblMessage"></asp:Label>
            </asp:TableCell>
        </asp:TableRow>

        <asp:TableRow>
            <asp:TableCell CssClass="FieldCell">
                <Arco:ButtonPanel ID="ButtonPanel1" runat="server">
                    <Arco:OkButton runat="server" ID="btnSave"></Arco:OkButton>
                    <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" id="btnCancel"></Arco:CancelButton>
                </Arco:ButtonPanel>
            </asp:TableCell>

        </asp:TableRow>
    </asp:Table>
  </asp:Content>
