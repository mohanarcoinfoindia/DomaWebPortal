<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DM_UserBrowser_GroupsResult.ascx.vb" Inherits="DM_UserBrowser_GroupsResult" Strict="false" %>      
        <script language="javascript">
            var LastPage = <%=LastPage%>;
            var orderbyField = '<%=orderby.name %>';
            var orderbyorderField = '<%=orderbyorder.name %>';
            
            function Goto(page)
            {
                if (((parseInt(page) <= LastPage) && (parseInt(page) > 0)) || (page == 1))
	                {
		                document.forms[0].Page.value = page;
		                Reload();
	                }
            }	
            
            function OrderBy(field)
            {
                if (field == document.forms[0].elements[orderbyField].value)
                {
	                if (document.forms[0].elements[orderbyorderField].value == "DESC")
	                {
	                    document.forms[0].elements[orderbyorderField].value = "ASC";
	                }
	                else
	                {
	                    document.forms[0].elements[orderbyorderField].value = "DESC";
	                }
	            }
                else
                {
	                document.forms[0].elements[orderbyorderField].value = "ASC";
	            }
                document.forms[0].elements[orderbyField].value = field;
                Reload();
            }  
            
            function Reload()
            {
                document.forms[0].submit();
            }             
          function passResult(groupname){                
                window.opener.document.getElementById(document.getElementById('GROUP_NAME_RES').value).value = groupname;  
                 window.opener.document.getElementById(document.getElementById('GROUP_DISPLAY_NAME_RES').value).value = groupname;                        
                self.close();              
           }   
              function OpenGroup(grp)
           {                
                <%If isAdmin Then%>
               location.href ='DM_UserBrowser_GroupDetail.aspx?login=' + encodeURIComponent(grp);       
               <%end if %>         
           }   
            function Delete(group,e){
                var txtGName = document.getElementById('txtGroupName').value;
                var txtGDesc = document.getElementById('txtGroupDescription').value;
                if (confirm(UIMessages[29].replace("#ITEM#",group))){
	                 window.location = 'DM_UserBrowser_Groups.aspx?txtGName=' + encodeURIComponent(txtGName) + '&txtGDesc=' + encodeURIComponent(txtGDesc) + '&groupid=' + encodeURIComponent(group) + '&delete=1';
                }
                e = e || window.event;
                e.stopPropagation ? e.stopPropagation() : (e.cancelBubble = true);
            }          
        </script>      
        <input type="hidden" id="Page" name="Page"/>
        <input type="hidden" name="orderby" id="orderby" runat="server"/>
        <input type="hidden" name="orderbyorder" id="orderbyorder" runat="server"/>
       <asp:Label ID="Result2" Runat="server" />
        <asp:Repeater  id="lstHistory" runat="server">
        <HeaderTemplate>
        <table Class="List HoverList PaddedTable StickyHeaders">
        <tr class="ListHeader" style="cursor:pointer">
            <th onclick="javascript:OrderBy('GROUP_NAME');"><%=GetLabel("ub_groupname")%></th>
            <th onclick="javascript:OrderBy('GROUP_DESC');"><%=GetLabel("ub_groupdesc")%></th>
            <th onclick="javascript:OrderBy('LAST_SYNCED');"><%=GetLabel("lastsynced")%></th>
            <th>&nbsp;</th>
         </tr>
         
         <tr class="ListFilter">
            <td><input type="text" id="txtGroupName" name="txtGroupName" value="<%=Request("txtGroupName") %>" width="200px" /></td>
            <td><input type="text" id="txtGroupDescription" name="txtGroupDescription" value="<%=Request("txtGroupDescription") %>" width="200px" /></td>  
            <td>&nbsp;</td>        
            <td>&nbsp;</td>
        </tr>
        </HeaderTemplate>       
        <ItemTemplate>
            <tr style="cursor:pointer" onclick='<%# OnClickAction(CType(Container, RepeaterItem).DataItem)%>'>
                 <td><%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "GROUP_NAME")%></td>
                <td><%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "GROUP_DESC")%></td>    
                <td><%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "LastSynced")%></td>     
               <td class="iconCell"><%#If(Not CanDelete(), "", "<img class='DeleteButton' src='" & ThemedImage.GetClientUrl("delete.png", Page) & "' onclick='Delete(" & EncodingUtils.EncodeJsString(Eval("GROUP_NAME").ToString) & ")' title='" & GetLabel("delete") & "' />")%> </td> 
            </tr>
        </ItemTemplate>
         <FooterTemplate>
            <tr class='ListFooter'>
	        <td  colspan="4">
		        <%= ShowPaging()%>    
              </td>
            </tr>
        </table>
        </FooterTemplate>
        </asp:Repeater>
      

