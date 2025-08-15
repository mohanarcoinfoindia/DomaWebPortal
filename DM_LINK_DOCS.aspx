<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_LINK_DOCS.aspx.vb" Inherits="DM_LINK_DOCS" ValidateRequest="false" EnableEventValidation="false" EnableViewStateMac="false" MasterPageFile="~/masterpages/Toolwindow.master" %>
 <asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
<script type="text/javascript">
    function LinkAs(type)
    {
        theForm.elements['<%=txtToBeLinkedAs.ClientID %>'].value = type;
        theForm.submit();
    }

</script>
   
    <asp:HiddenField ID="txtToBeLinkedAs" runat="server" />
 
          <asp:Panel ID="pnlSelType" runat="server" Visible="true">

       <asp:Table ID="Table1" CssClass="DetailTable" runat="server" HorizontalAlign="Center" >
    <asp:Tablerow CssClass="DetailHeader" ID="plhHeader">   
    <asp:TableCell HorizontalAlign="Center" ColumnSpan="2" CssClass="DetailHeaderContent"><asp:Label ID="lblHeader" runat="server" Text="Document Linking"></asp:Label></asp:TableCell>    
    </asp:Tablerow>
    
    
    <asp:TableRow>
        <asp:TableCell Width="10%" VerticalAlign="top"><asp:Label ID="lblLink" runat="server"></asp:Label></asp:TableCell>
        <asp:TableCell  Width="90%" VerticalAlign="top" HorizontalAlign="left">
            <asp:Label id="lblSourceDocs" runat="server" ></asp:Label>
        </asp:TableCell>       
    </asp:TableRow>
    <asp:TableRow>
    <asp:TableCell CssClass="LabelCell"><asp:Label ID="lblAs" runat="server" CssClass="Label"></asp:Label></asp:TableCell>
    <asp:TableCell ID="tableContent" CssClass="FieldCell">
    <span>
    <asp:Label ID="lblLinks" runat="server">
    
    </asp:Label>
   
</span>
    
    </asp:TableCell></asp:TableRow>
    
        <asp:TableRow>
        <asp:TableCell CssClass="LabelCell"><asp:Label ID="lblTo" runat="server"></asp:Label></asp:TableCell>
        <asp:TableCell  CssClass="FieldCell">
        <asp:Label id="lblTargetDoc" runat="server"></asp:Label>
        </asp:TableCell>
        
    </asp:TableRow>
</asp:Table>
    </asp:Panel>
    
    <asp:Panel ID="pnlSelMsg" runat="server" Visible="false">
        <asp:Table ID="Table2" CssClass="Panel" runat="server" HorizontalAlign="Center" >
        <asp:TableRow CssClass="DetailHeader"><asp:TableCell CssClass="DetailHeaderContent">&nbsp;</asp:TableCell></asp:TableRow>
        <asp:TableRow> <asp:TableCell HorizontalAlign="Center" ColumnSpan="2">
            <asp:Label ID="lblMsg" runat="server" Text="Linking ok"></asp:Label>
            </asp:TableCell>          
           </asp:TableRow>
           </asp:Table>
    </asp:Panel>
        
</asp:Content>