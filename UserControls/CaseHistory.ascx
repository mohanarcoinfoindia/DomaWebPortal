
<%@ Control Language="VB" AutoEventWireup="false" CodeFile="CaseHistory.ascx.vb" Inherits="UserControls_CaseHistory" Strict="false" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls"  %>

  <script type="text/javascript">
            var <%=Me.ClientID %>_LastPage = <%=LastPage%>;
            var <%=Me.ClientID %>_orderbyField = '<%=orderby.name %>';
            var <%=Me.ClientID %>_orderbyorderField = '<%=orderbyorder.name %>';
            
            function <%=Me.ClientID %>_Goto(page)
            {
                if (((parseInt(page) <= <%=Me.ClientID %>_LastPage) && (parseInt(page) > 0)) || (page == 1))
	                {
		                document.forms[0].Page.value = page;
		                Reload();
	                }
            }	
            
            function <%=Me.ClientID %>_OrderBy(field)
            {
                if (field == document.forms[0].elements[<%=Me.ClientID %>_orderbyField].value)
                {
	                if (document.forms[0].elements[<%=Me.ClientID %>_orderbyorderField].value == "DESC")
	                {
	                    document.forms[0].elements[<%=Me.ClientID %>_orderbyorderField].value = "ASC";
	                }
	                else
	                {
	                    document.forms[0].elements[<%=Me.ClientID %>_orderbyorderField].value = "DESC";
	                }
	            }
                else
                {
	                document.forms[0].elements[<%=Me.ClientID %>_orderbyorderField].value = "ASC";
	            }
                document.forms[0].elements[<%=Me.ClientID %>_orderbyField].value = field;
                Reload();
            }  
            
            function Reload()
            {            
                document.forms[0].submit();
            }   
            function onEnter()
            {
            <%=Me.ClientID %>_Goto(1);
            }
        
            
          
        </script>
        
         <input type="hidden" id="Page" name="Page"/>
        <input type="hidden" name="orderby" id="orderby" runat="server"/>
        <input type="hidden" name="orderbyorder" id="orderbyorder" runat="server"/>
        <asp:LinkButton ID="lnkReload" runat="server" OnClientClick="javascript:onEnter();return false;"></asp:LinkButton>

        <table class="List">
        <tr>
        <%If Not ForUser Then%>
            <th style="cursor:pointer" onclick="javascript:<%=Me.ClientID %>_OrderBy('USER_ID');" ><%=GetLabel("user")%></th>
        <%else %>
        <th></th>
         <%end if %>   
            <th style="cursor:pointer" onclick="javascript:<%=Me.ClientID %>_OrderBy('DATE_OF_ACTION_END');" ><%=Getlabel("date") %></th>
            <%If Not ForUser Then%>
            <th style="cursor:pointer" onclick="javascript:<%=Me.ClientID %>_OrderBy('DATE_OF_ACTION_START');"><%=Getlabel("stepstartdate") %></th>
            <%Else%>
             <th><%=GetLabel("procedure")%></th>
             <%end if %>
            <th><%=GetLabel("step")%></th>
             <th><%=GetLabel("module")%></th>
            <th></th>
          
        </tr>

        <tr class="ListFilter">
            <td></td>
            <td><input type="text" name="txtDateFilter" value="<%=Request.Form("txtDateFilter")%>" /></td>
            <td></td>
            <td></td>
            <td></td>
          <td></td>
        </tr>


        <asp:Repeater  id="lstHistory" runat="server" EnableViewState="false">
        <HeaderTemplate>
       

        </HeaderTemplate>       
        <ItemTemplate>
            <tr>
             <%If Not ForUser Then%>
                <td><%#FormatUserName(CType(Container, RepeaterItem).DataItem)%></td>
                <%else %>
                <td><%# GetIcon(DirectCast(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Case_ID"),Int32))%></td>
                <%end if %>
                <td><nobr><%#ArcoFormatting.FormatDateLabel(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Date_End").ToString, True, True, True)%></nobr></td>
                <%If Not ForUser Then%>
                <td><nobr><%#If(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Action").ToString = "STEPCOMPLETED", ArcoFormatting.FormatDateLabel(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Date_Start").ToString, True, True, True), "")%></nobr></td>                
                <%else%>
                  <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Proc_Name"))%></td>
                  <%end if %>
                <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Step_Name"))%></td>
                <td ><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Module"))%></td>
                <td><%#Server.HtmlEncode(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Description"))%></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
        
        </FooterTemplate>
        </asp:Repeater>
            <tr class="ListFooter">
	            <td  colspan="6">
           <%if NumberOfResults > 0 then%>
            
		               <asp:HyperLink runat="server" ID="lnkPrev" SkinID="PrevPage"></asp:HyperLink>
		                  <asp:Literal ID="litScroller" runat="server"/>
		                 <asp:HyperLink runat="server" ID="lnkNext" SkinID="NextPage"></asp:HyperLink>
                                          
    		              <%If NumberOfResults > RecordsPerPage Then%>
		                 &nbsp;&nbsp;<%="(" & NumberOfResultsLabel & " " & GetLabel("resultsfound") & ")"%>
		                 <%end if %>
		           
            <%else %>
            &nbsp;
            <%end if%>
             </td>
            </tr>
        </table>



  <%=GetLabel("recsperpage") %>
<asp:DropDownList ID="drpRecsPerPage" runat="server" AutoPostBack="true">
    <asp:ListItem Value="30" Selected="true">30</asp:ListItem>
     <asp:ListItem Value="100">100</asp:ListItem>
     <asp:ListItem Value="500">500</asp:ListItem>
</asp:DropDownList>
&nbsp;

       
        <ArcoControls:CheckBox ID="chkShowDetails" runat="server" AutoPostBack="true" Text="Show Details" />
   