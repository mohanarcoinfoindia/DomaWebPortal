<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Checkin.aspx.vb" Inherits="DM_Checkin" MasterPageFile="~/masterpages/Toolwindow.master" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
  
   <center><div style="margin:10px;width:400px">
    
    <table class="DetailTable">
     <asp:PlaceHolder ID="plhHeader" runat="server">
    <tr class="DetailHeader">   
    <td class="DetailHeaderContent" colspan="2"><%=GetLabel("checkin")%></td>
    </tr>
    </asp:PlaceHolder>
    
    <tr ><td colspan="2">
        <asp:RadioButtonList ID="rdlAction" runat="server" AutoPostBack="True">
            <asp:ListItem  Value="0">Cancel Checkout</asp:ListItem>
            <asp:ListItem Value="1">Save New Version</asp:ListItem>
            <asp:ListItem Value="2" Selected="True">Save New SubVersion</asp:ListItem>
            <asp:ListItem Value="3">Overwrite Current Version</asp:ListItem>
        </asp:RadioButtonList>
        <ArcoControls:CheckBox ID="chkCheckOut" runat="server" Text="Keep checked out" /><br />
        </td></tr>
        <tr><td class="LabelCell">
            <asp:Label ID="lblComment" runat="server" AssociatedControlID="txtCheckInComment"></asp:Label></td>
            <td class="FieldCell">        
            <asp:TextBox runat="server" ID="txtCheckInComment" TextMode="MultiLine"></asp:TextBox>
    
        </td></tr>
        <tr ><td align="right" colspan="2">
        <Arco:ButtonPanel id="pnlButtons" runat="server">
            <Arco:OkButton ID="cmdSave" runat="server" ValidationGroup="RenameObject" Text="Save"></Arco:OkButton>
            <Arco:CancelButton ID="cmdCancel" runat="server" OnClientClick="javascript:Close();return false;"></Arco:CancelButton>
        </Arco:ButtonPanel>        
        

        </td></tr>
        </table>
        </div>
</center>     

 
   


</asp:Content>