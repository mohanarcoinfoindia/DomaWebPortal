<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CaseDeadlines.aspx.vb" Inherits="CaseDeadlines" MasterPageFile="~/masterpages/Preview.master" %>

 <asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
    <asp:Table ID="tblDeadlines" runat="server">
    
    </asp:Table>
    <asp:Table ID="Table1" runat="server" Width="100%">
    <asp:TableRow><asp:TableCell HorizontalAlign="Right">
    <Arco:ButtonPanel runat="server" ID="pnlButtons">        
        <Arco:OkButton ID="btnOk" runat="server"></Arco:OkButton>
    </Arco:ButtonPanel>
    </asp:TableCell></asp:TableRow>
    </asp:Table>
 </asp:Content>