<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_NewItem.aspx.vb" Inherits="DM_Catalog_NewItem" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head2" runat="server">
		<title>Catalog Maintenance - Item detail</title>
		<script language="Javascript" src="../dhtmllib.js"></script>
		<script language="Javascript" src="../PopupOpener.js"></script>
		<script language="Javascript" src="../tooltip.js"></script>
		<script language="javascript" src="../FormChek.js"></script>

<script language="javascript">
 function CheckForEnter(){
  if (window.event.keyCode == 13) {
    window.event.returnValue = false;
    __doPostBack('lnkSave','');
  }
 }
 function CheckForEnterNN(kc) {
  if (kc.which == 13) {
    __doPostBack('lnkSave','');
    return false;
  }
 }

 if (navigator.appName == 'Netscape') {
    window.captureEvents(Event.KEYPRESS);
    window.onkeypress = CheckForEnterNN;
 }   

 function Switch(langcode, listid,active) {
   window.location = 'DM_Catalog_Listdetail.aspx?id=' + listid + '&langcode=' + langcode + '&active=' + active;
 }                              
</script>

	</head>
	<body>	
		<form id="Form1" method="post" runat="server" >
			
			<asp:table CssClass="DetailTable" Runat="server" id="Table1">
				<asp:TableRow CssClass="DetailHeader" >
					<asp:TableCell ColumnSpan="4" >
						<b><asp:Label ID="txtTitle" Runat="server" /></b>
					</asp:TableCell>
				</asp:TableRow>
				<asp:TableRow>
					<asp:TableCell Width="30%"><b><asp:Label ID="lblList1" Text="List" Runat="server" /></b></asp:TableCell>
					<asp:TableCell Width="70%"><asp:Label ID="lblList" runat="server"></asp:Label></asp:TableCell>								   		
				</asp:TableRow>	
				<asp:TableRow >
					<asp:TableCell Width="30%"><b><asp:Label ID="lblCode" Text="Code" Runat="server" /></b></asp:TableCell>
					<asp:TableCell Width="70%"><asp:TextBox Runat="server" ID="txtCode"></asp:TextBox></asp:TableCell>								   		
				</asp:TableRow>	
				<asp:TableRow >
					<asp:TableCell Width="30%"><b><asp:Label ID="lblDesc" Text="Description" Runat="server" /></b></asp:TableCell>
					<asp:TableCell Width="70%">
						<asp:TextBox Runat="server" ID="txtDesc"></asp:TextBox>
						<asp:HiddenField runat=server ID="txtID" />
					</asp:TableCell>								   		
				</asp:TableRow>	
				<asp:TableRow >
				  <asp:TableCell Width="30%"><asp:Label ID="lblDescHomo" Text="Homonym" runat=server /></asp:TableCell>
					<asp:TableCell Width="70%"><asp:TextBox Runat="server" ID="txtDescHomo"></asp:TextBox></asp:TableCell>								   		
				</asp:TableRow>
				<asp:TableRow >
				  <asp:TableCell Width="30%"><asp:Label ID="lblScopeNote" Text="Scope note" runat=server /></asp:TableCell>
				  <asp:TableCell Width="70%"><asp:TextBox Runat="server" ID="txtScopeNote"></asp:TextBox></asp:TableCell>
				</asp:TableRow>
				<asp:TableRow>
					<asp:TableCell ColumnSpan="4" HorizontalAlign="right">
						<asp:LinkButton ID="lnkSave"   CommandName="Save"   Runat="server" OnClick="doSave"   ><%=GetLabel("save")%></asp:LinkButton>
					</asp:TableCell>
				</asp:TableRow>
		</asp:table> 
           
             <asp:Panel ID="MsgPanel" runat="server" Visible="False">
               <div>
                <asp:Label ID="msg_lbl" ForeColor=red runat="server" Text=""></asp:Label>
               </div>
               <asp:LinkButton ID="msg_button" runat="server" OnClick="msg_button_Click">Hide Message</asp:LinkButton>
            </asp:Panel>
           
		  </form>
		<asp:Literal ID="plValidationScript" Runat="server"></asp:Literal>	
	</body>
</html>
