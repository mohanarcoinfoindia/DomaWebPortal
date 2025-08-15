<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AdvDocroomListLookup.aspx.vb" Inherits="UserControls_AdvDocroomListLookup" MasterPageFile="~/masterpages/Toolwindow.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">

   <script language="javascript">
function RefreshOpener()
{
 MainPage().GetContentWindow().RefreshOnChange();
}   
function SaveClose(){
<%if mbRefreshOnChange then%>
	RefreshOpener();
    <%end if %>
	Close();
}

function Goto(page){
theForm.PAGE.value = page;
theForm.submit();
}


function Search(){
theForm.PAGE.value = '';
theForm.submit();
}
function AutoSelect(val,desc)
{
    MainPage().GetContentWindow().AddValue<%=JSIdentifier %>(val,desc);
   
	 SaveClose();
   
}

function Select(val,desc){

	MainPage().GetContentWindow().AddValue<%=JSIdentifier %>(val,desc);
    UpdateYourSelection();
	<%if not mbMultiSelect OrElse mbAutoSelect Then%>
	SaveClose();
<%end if %>}

function Clear(){
MainPage().GetContentWindow().ClearValue<%=JSIdentifier %>();
UpdateYourSelection();
<%if not mbMultiSelect then%>
	SaveClose();
<%end if%>

}
function Remove(val){
<%if mbMultiSelect then%>
MainPage().GetContentWindow().oProp_<%=JSIdentifier %>.RemoveValue(val);
UpdateYourSelection();
<%else%>
	Clear();
<%end if%>

}


 function UpdateYourSelection()
  {
        document.getElementById('lblSelection').innerHTML = ""; 
        var selArr = MainPage().GetContentWindow().oProp_<%=JSIdentifier %>.GetCaption().split("<br/>");
        var valArr = MainPage().GetContentWindow().oProp_<%=JSIdentifier %>.GetValue().split(", ");
        for (var i = 0; i < selArr.length; i++) {
            
        <%if mbMultiSelect then%>
            var removelink = document.createElement("a");
            removelink.appendChild(document.createTextNode(selArr[i]));
            removelink.href = "javascript:Remove('" + valArr[i] + "');";
            removelink.title = "<%=getLabel("remove") %>";
            // removelink.setAttribute("class","ButtonLink");
                document.getElementById('lblSelection').appendChild(removelink);
                document.getElementById('lblSelection').appendChild(document.createElement("br")); 
        <%else %>
            document.getElementById('lblSelection').appendChild(document.createTextNode(selArr[i]));   
        <%end if %>
           
        }
  } 
    </script>
    
        <asp:LinkButton ID="lnkEnter" runat="server" OnClientClick="Search();return false;"></asp:LinkButton>  
		
		<INPUT TYPE=HIDDEN  NAME=PAGE VALUE=<%=liPage%> />
		<INPUT TYPE=HIDDEN NAME=SEARCH VALUE="Y" />
		
		
	
            <table class='DetailTable'>
                <tr class='DetailHeader'><td class='DetailHeaderContent' colspan="2">
                <%=GetLabel("search")%>
                </td></tr>
                <tr><td colspan="2">
                <Arco:DMSearchForm id="domasearch" runat="server" ShowSearchButtons="false" />  
                </td></tr>
                <tr>
                    <td>
                        <%if ShowCreateItem Then %>
                         <a href="javascript:CreateItem();" class="ButtonLink"><%=GetLabel("add")%></a>
                        <%end If %>
                        </td><td>
                            <div class="buttons">
                        <a HREF='JavaScript:Search();' class='button'><%=GetLabel("search") %></a> 
                                </div>                       
                    </td></tr>
                <tr>
                    <td  valign="top">
                        <span class='Label'><%=GetLabel("yourselection") %> : </span>
                    </td>
                    <td class="ReadOnlyFieldCell" valign="top" width="100%">
                         <span id="lblSelection"></span>
                    </td>
                </tr>
                 <%If mbMultiSelect Then%>
               
                <tr>
                   <td colspan="2" align="right">
                       <a href='javascript:Clear();' class='ButtonLink'><%=GetLabel("clear") %></a>&nbsp;<a HREF='JavaScript:Close();' class='ButtonLink'><%=GetLabel("close")%></a>
                   </td>
                </tr>
                <%End If%>                   
             </table>
           
            <asp:Literal ID="plhResults" runat="server" />
            

	<script language="javascript">
	<%if Not String.IsNullOrEmpty(msFocusField) then%>
	theForm.<%=msFocusField%>.focus();
	<%end if%>
	
	</script>   
	
</asp:Content>
