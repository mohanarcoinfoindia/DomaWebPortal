<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_UserBrowser_GroupDetail.aspx.vb" Inherits="DM_UserBrowser_GroupDetail" ValidateRequest="false" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head2" runat="server">
		<title>User managment - Add</title>		
		<script type="text/javascript">
		    function DoClose() {
		        location.href = 'DM_UserBrowser_Groups.aspx';
		    }
		    function ShowDetails(id, type) {
		        if (type == "Role") {
		            location.href = 'DM_UserBrowser_RoleDetail.aspx' + '?login=' + id + '&new=false&structured=true&popup=null';		          
		        }
		        if (type == "User") {
		            location.href = "DM_UserBrowser_AddUser.aspx?login=" + id + "&new=false";
		        }
		    }        
		</script>
	</head>
	<body>	
		<form id="Form1" method="post" runat="server" defaultbutton="lnkSave" >
		
          <telerik:RadScriptManager runat="server" ID="scriptManager" />


			<asp:table CssClass="DetailTable" Runat="server" id="Table1">				
				<asp:TableRow>
					<asp:TableCell CssClass="LabelCell"><b><asp:Label ID="lblGroupName" Runat="server" CssClass="Label" AssociatedControlID="txtGroupName" /></b></asp:TableCell>
					<asp:TableCell CssClass="FieldCell">
						<asp:TextBox runat="server" ID="txtGroupname" />
					</asp:TableCell>					
				</asp:TableRow>		
				<asp:TableRow>
					<asp:TableCell CssClass="LabelCell"><asp:Label ID="lblGroupDescription" Runat="server" CssClass="Label" AssociatedControlID="txtGroupdescription" /></asp:TableCell>
					<asp:TableCell CssClass="FieldCell">
						<asp:TextBox runat="server" ID="txtGroupDescription" MaxLength="2000" TextMode="MultiLine" />
					</asp:TableCell>
					<asp:TableCell></asp:TableCell>
				</asp:TableRow>		
				<asp:TableRow Visible="false" ID="trDomain">				
					<asp:TableCell CssClass="LabelCell"><asp:Label ID="lblNewDomain" Runat="server" CssClass="Label" AssociatedControlID="txtDomain" /> :</asp:TableCell>
					<asp:TableCell CssClass="FieldCell">
						<asp:TextBox runat="server" ID="txtDomain" />
					</asp:TableCell>					
				</asp:TableRow>	
				
				<asp:TableRow>
					<asp:TableCell ColumnSpan="2" HorizontalAlign="right">
                     <Arco:ButtonPanel runat="server" ID="btnPanel">                                   
                                    <Arco:OkButton runat="server" ID="lnkSave" CommandName="Save" OnClick="doSave"> </Arco:OkButton>
                                    <Arco:CancelButton runat="server" ID="btnClose" OnClientClick="javascript:DoClose();return false;"></Arco:CancelButton>&nbsp;
                                    <Arco:SecondaryButton runat="server" ID="lnkTransferWork"  OnClientClick="javascript:TransferWork();return false;"></Arco:SecondaryButton>
                                </Arco:ButtonPanel>
                
					</asp:TableCell>					
				</asp:TableRow>
			</asp:table>		
			
			  <asp:Label ID="Label1" Runat="server" />
			<asp:PlaceHolder ID="lblMessage" Visible="False" Runat="server">
                 <div style="width:600px;margin: 10px auto 10px auto">		
                     <asp:Label ID="lblMsgText" Runat="server" Text="Save ok." CssClass="InfoLabel" />	                   
	            </div>							
			</asp:PlaceHolder>							
			  <asp:Label ID="Result" Runat="server" />			
			<asp:table runat="server" ID="tblAddMembers" Visible="false">
			    <asp:TableRow><asp:TableCell>
			        <a href="DM_UserBrowser_AddToRoleMain.aspx?roleid='<%= replace(groupid,"\","\")%>'&url=group"  target="_blank">
			        <b><%= GetLabel("ub_addmembers")%>&nbsp;&nbsp;<asp:Image ID="Image1" ImageUrl="~/Images/User.bmp" runat="server" /></b></a>
			     </asp:TableCell> 

			     </asp:TableRow>			    
			</asp:table>
            <telerik:RadTabStrip runat="server" ID="radTab" SelectedIndex="0" MultiPageID="RadMultiPage1" Skin="Vista">
        <Tabs>
            <telerik:RadTab Text="Members" />                          
        </Tabs>
        </telerik:RadTabStrip>
        <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" ScrollBars="Auto" BorderColor="DarkGray" BorderStyle="Solid" BorderWidth="1px">
            <telerik:RadPageView runat="server" ID="PVGroupDetails" BackColor="White">
            <div id="divMembers" runat="server" style="margin:10px;width:98%;">
                  <table cellspacing="0" cellpadding="0" width="100%" >
                                <tr>
                                    <td>
                                    <div style="overflow:auto; max-height:500px;vertical-align:top;">
                                    
                                           <table class="SubList" width="100%" cellpadding="3">   
                                            <asp:Repeater ID="repMembers" runat="server">
                                                <HeaderTemplate> 
                                                    <tr>
                                                        <th style="width:10px;"></th> 
                                                        <th style="width:200px;"><%=GetLabel("name")%></th>                                                        
                                                        <th style="width:200px;"><%=GetLabel("description")%></th>
                                                    </tr>                
                                                </HeaderTemplate>                                               
                                                <ItemTemplate>		
                                                    <tr>		
                                                        <td style="width:10px;" ><img height="12px" src="../Images/<%#GetIcon(DataBinder.Eval(Container.DataItem, "TYPE").toString())%>" title="<%#GetLabel(DataBinder.Eval(Container.DataItem, "TYPE").ToString())%>" /></td>
                                                        <td style="width:200px;cursor:pointer" onclick="javascript:ShowDetails('<%#If(DataBinder.Eval(Container.DataItem, "TYPE").toString() = "Role",GetRoleID(DataBinder.Eval(Container.DataItem, "NAME").toString()),DataBinder.Eval(Container.DataItem, "NAME").toString().Replace("_DATABASE\", "").Replace("\","\\"))%>','<%# DataBinder.Eval(Container.DataItem, "TYPE").toString() %>')"> <%# DataBinder.Eval(Container.DataItem, "CAPTION").ToString().Replace("_DATABASE\", "")%> </td>                                                      
                                                        <td style="width:200px;cursor:pointer" onclick="javascript:ShowDetails('<%#If(DataBinder.Eval(Container.DataItem, "TYPE").toString() = "Role",GetRoleID(DataBinder.Eval(Container.DataItem, "NAME").toString()),DataBinder.Eval(Container.DataItem, "NAME").toString().Replace("_DATABASE\", "").Replace("\","\\"))%>','<%# DataBinder.Eval(Container.DataItem, "TYPE").toString() %>')"> <%#DataBinder.Eval(Container.DataItem, "DESCRIPTION")%></td>                                                                
                                                    </tr>
                                                </ItemTemplate>	                                                    
                                            </asp:Repeater>	
                                            <tr runat="server" id="trMembers" visible="false" class="ListFooter">
                                                <td colspan="3" align="center"><asp:Label runat="server" ID="lblNoMembersFound" /></td>
                                            </tr> 
                                            	                                
                                        </table>
                                     
                                      </div>
                                    </td>
                                </tr>
                                <asp:PlaceHolder ID="plhEditMembers" runat="server">
                                <tr valign="top">
                                    <td valign="top" colspan="3">
                                        <div class="SubListHeader3RightFillerDivSwitched">
                                            <div style="width:35px;" class="rounded-headerR">                                                     
                                                <div class="rounded-headerL">			      		
                                                    <div class="SubListMainHeaderT" >				                                                                               
                                                        <img src="../images/edit.png" onclick="javascript:EditMembers();" title="<%=getLabel("edit") %>" style="cursor:pointer;" />                             
                                                    </div>	
                                                </div>
                                            </div>                                                                        
                                        </div>
                                    </td>
                                </tr>      
                                </asp:PlaceHolder>                         
                            </table>                  
              
                </div>
             </telerik:RadPageView>
        </telerik:RadMultiPage>
		   
			 </form>
		<asp:Literal ID="plValidationScript" Runat="server"></asp:Literal>	
	</body>
</html>
