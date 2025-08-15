<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_Listreltypes.aspx.vb" Inherits="DM_Catalog_Listreltypes" ValidateRequest="false" EnableSessionState="True" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head2" runat="server">
		<title>Catalog Maintenance - List relationships</title>
		
		<script language="Javascript" src="../dhtmllib.js"></script>
		<script language="Javascript" src="../PopupOpener.js"></script>
		<script language="Javascript" src="../tooltip.js"></script>
		<script language="javascript" src="../FormChek.js"></script>
		<script language="javascript">
		     function CheckForEnter(){
	            if(window.event.keyCode == 13){
	                if (doCheck() == true){
		                window.event.returnValue = false;
		                __doPostBack('lnkSave','');
		            }   
	            }
            }
            function CheckForEnterNN(kc){
	            if(kc.which == 13){
	                 if (doCheck() == true){
		                __doPostBack('lnkSave','');
		                return false;
		             }   
	            }
            }

            if (navigator.appName == 'Netscape') {
	            window.captureEvents(Event.KEYPRESS);
	            window.onkeypress = CheckForEnterNN;
            }   
                                    
		</script>
		</head>
	<body>	
		<form id="Form1" method="post" runat="server" >
			<asp:table CssClass="DetailTable" Runat="server" id="Table1">
                <asp:TableRow VerticalAlign="Middle">
				    <asp:TableHeaderCell ColumnSpan ="4" VerticalAlign="Middle" >
				       <asp:Label ID="lblList" runat="server"></asp:Label>&nbsp;
				       <asp:Label ID="lblSplit1" Text="/" runat="server"></asp:Label>&nbsp;
				       <asp:Label ID="lblRel" Text="Relationships" runat="server"></asp:Label>&nbsp;
				    </asp:TableHeaderCell>
                </asp:TableRow>
				<asp:TableRow>
					<asp:TableCell ColumnSpan="4" HorizontalAlign="left">
						<asp:GridView  ID="TableGridView" runat="server"   OnRowEditing ="TableGridView_RowEditing" OnRowCancelingEdit="TableGridView_RowCancelingEdit" OnRowUpdating="TableGridView_RowUpdating"  OnRowDeleting="TableGridView_RowDeleting" OnPageIndexChanging="TableGridView_PageIndexChanging" AutoGenerateColumns="False"  AllowPaging="True" AllowSorting="True" PageSize="20" >
						    <PagerStyle ForeColor="Blue" HorizontalAlign="Left" BackColor="White" ></PagerStyle>
						</asp:GridView>
						 <i>You are viewing page <%=TableGridView.PageIndex + 1%> of <%=TableGridView.PageCount%> </i>

                    </asp:TableCell>
                </asp:TableRow>                     
             </asp:table>
             <asp:Panel ID="MsgPanel" style="left: 434px; position:relative; top: 9px; z-index: 114;" runat="server" Height="151px" Width="443px" Visible="False">
             <div style=" overflow:auto; width:431px; height:123px; left: 6px; position:relative; top: 3px; z-index: 116; ">
                <asp:Label ID="msg_lbl" runat="server" Text="" style="left: 6px; position: absolute; top: 3px" ></asp:Label>
             </div>
                <asp:LinkButton ID="msg_button" runat="server" OnClick="msg_button_Click" style="z-index: 114; left: 5px; position: absolute; top: 131px">Hide Message</asp:LinkButton>
            </asp:Panel>
		  </form>
		<asp:Literal ID="plValidationScript" Runat="server"></asp:Literal>	
	</body>
</html>
