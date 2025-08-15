<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Queries.aspx.vb" Inherits="Queries" Strict="false" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Queries</title>    
    <script type="text/javascript">

        function ExecQuery(id, searchscreenid, restype) {
            GotoDocumentList(id, "advsearch", searchscreenid, restype);
        }       
        function SubscribeToQuery(id) {
            PC().SubscribeToQuery(id);
        }
        function EditSubscription(id) {
            PC().OpenModalWindow('Subscription.aspx?ID=' + id, false, 800, 600, true, false);
        }
        function ViewSubscriptions(id) {
            PC().GotoLink('Subscriptions.aspx?OBJ_TYPE=Query&OBJ_ID=' + id);
        }
        function DoQueryACL(id) {
            PC().OpenWindow("DM_ACL.ASPX?QUERY_ID=" + id, "ACL", "height=600,width=800,scrollbars=yes,resizable=yes");
        }             
    </script>
    
</head>
<body>
    <form id="form1" runat="server" defaultbutton="lnkSearch">
    <script type="text/javascript">
     function GotoDocumentList(id,screenmode,searchscreenid, restype)
        {         
         PC().GotoLink("DM_DOCUMENTLIST.aspx?QRY_ID=" + id + "&screenmode=" + screenmode + "&DM_SEARCH_SCREEN_ID=" + searchscreenid + "&result_type=" + restype + "&DM_PARENT_ID=" + PC().GetParentID() + "&LOADQRY=Y&PACK_ID=" + PC().GetPackageID() + "&selectionmode=<%=QueryStringParser.GetString("selectionmode")%>");
        }
    function DeleteQuery(id) {
        if (confirm(UIMessages[4])) {            
            $get('<%=delqry.clientid %>').value = id;
            Reload()   
           }         
        }
      var LastPage = <%=LastPage%>;
            var orderbyField = '<%=orderby.uniqueid %>';
            var orderbyorderField = '<%=orderbyorder.uniqueid %>';
         
            function Goto(page)
            {
                if (((parseInt(page) <= LastPage) && (parseInt(page) > 0)) || (page == 1))
	                {
		                $get("Page").value = page;
		                Reload();
	                }
            }	
            
            function OrderBy(field)
            {
                if (field == $get(orderbyField).value)
                {
                    if ($get(orderbyorderField).value == "DESC")
	                {
	                    $get(orderbyorderField).value = "ASC";
	                }
	                else
	                {
	                    $get(orderbyorderField).value = "DESC";
	                }
	            }
                else
                {
	                $get(orderbyorderField).value = "ASC";
	            }
                $get(orderbyField).value = field;
                Reload();
            }  
            
            function Reload()
            {
                document.forms[0].submit();
            }       
            function Refresh() {
                Reload();
            }
    </script>
    <div style="margin-right:10px">
    <Arco:PageController ID="PC" runat="server" PageLocation="ListPage" />

         <input type="hidden" id="Page" name="Page"/>
        <input type="hidden" id="orderby" runat="server" value="QUERY_NAME"/>
        <input type="hidden" id="orderbyorder" runat="server" value="ASC"/>
        <input type="hidden" id="delqry" runat="server"/>

<div class="queries">
<table class="List">
   <tr ><th>&nbsp;</th>
                    <th onclick="javascript:OrderBy('QUERY_NAME');" style="cursor:pointer"><%=GetLabel("name") %></th>   
        <th onclick="javascript:OrderBy('QUERY_DESCRIPTION');" style="cursor:pointer"><%=GetLabel("description") %></th> 
                    <th width="150" onclick="javascript:OrderBy('QUERY_OWNER');" style="cursor:pointer"><%=GetLabel("createdby")%></th>
                    <th width="150" onclick="javascript:OrderBy('QUERY_SCOPE');" style="cursor:pointer"><%=GetLabel("queryviewscope")%></th>
                    <th width="150" onclick="javascript:OrderBy('RES_TYPE');" style="cursor:pointer"><%=GetLabel("queryresultscope")%></th>
       <th>&nbsp;</th><th>&nbsp;</th><th>&nbsp;</th><th>&nbsp;</th>
                    </tr>
     <tr class="ListFilter">
            <td>&nbsp;</td>
         <td><asp:TextBox runat="server" ID="txtFilterName"/></td>
         <td><asp:TextBox runat="server" ID="txtFilterDesc"/></td>
         <td><asp:TextBox runat="server" ID="txtFilterOwner"/></td>
         <td> <asp:DropDownList runat="server" ID="drpScopeFilter" AutoPostBack="true"/>     </td>
         <td>
             <asp:DropDownList runat="server" ID="drpResultTypeFilter" AutoPostBack="true"/>                                
         <td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td>
         </tr>
      <asp:Repeater runat="server" ID="lstQueries" EnableViewState="false">
                <HeaderTemplate>
                   
                  
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                        <asp:PlaceHolder ID='plDD' runat='server' Visible="<%# DragAndDropEnabled %>">
                        <span title="<%=GetLabel("queriesdraghelp")%>"  class="icon icon-query" draggable="true" ondragstart="handleDragStart('q<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>')" ondragover='cancelEvent()' ondragleave='handleDragLeave()' ></span>
                        &nbsp;
                        </asp:PlaceHolder>
                        </td>
                                                                     
                        <td width="100%">
                        <asp:PlaceHolder ID='plExecQry' runat='server' Visible='<%# CanRunQuery(CType(Container, RepeaterItem).DataItem) %>'>
                        <a draggable="false" href='Javascript:ExecQuery(<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>,<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ScreenID")%>,<%#Convert.ToInt32(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ResultType"))%>);' class="ButtonLink"> <%#Server.HtmlEncode(CType(Container, RepeaterItem).DataItem.Name)%></a>
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID='plNoExecQry' runat='server' Visible='<%# (not CanRunQuery(CType(Container, RepeaterItem).DataItem)) %>'>
                        <%#Server.HtmlEncode(CType(Container, RepeaterItem).DataItem.Name)%>
                        </asp:PlaceHolder>                        
                        </td>

                         <td>
                        <%# Server.HtmlEncode(CType(Container, RepeaterItem).DataItem.Description)%>
                        </td>

                        <td>
                        <%# QueryOwner(CType(Container, RepeaterItem).DataItem)%>   
                        </td>
                        <td>
                         <%#Arco.EnumTranslator.GetEnumLabel(CType(Container, RepeaterItem).DataItem.Scope)%>
                        </td>
                        <td>
                         <%#Arco.EnumTranslator.GetEnumLabel(CType(Container, RepeaterItem).DataItem.ResultType)%>
                        </td>
                       
                        
                        
                        <td class="iconCell">
                        <%# EditQueryButton(CType(Container, RepeaterItem).DataItem)%>                      
                        </td>
                        <td class="iconCell">
                        <%# SubscriptionButtons(CType(Container, RepeaterItem).DataItem)%>            
                        
                            
                        </td>
                        <td class="iconCell">
                              <%# ShortCutsField(CType(Container, RepeaterItem).DataItem)%>    

                         <%# DeleteQueryButton(CType(Container, RepeaterItem).DataItem)%>     
                          
                        </td>                        
                        <td class="iconCell">
                        <asp:PlaceHolder ID='plQryACL' runat="server" Visible='<%# CanSetAcl(CType(Container, RepeaterItem).DataItem) %>'>
                            <a class="ButtonLink" href='javascript:DoQueryACL(<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>);'><span class="icon icon-acl" title='<%=GetLabel("ctx_aclsetacl")%>'></span></a>
                        </asp:PlaceHolder>
                        </td> 
                        
                    </tr>
                </ItemTemplate>
                
                 <FooterTemplate>
                  
        </FooterTemplate>
        
            </asp:Repeater>
     <tr class="ListFooter">
                <td  colspan="10">
              <%if NumberOfResults > 0 then%>       
		                <asp:HyperLink runat="server" ID="lnkPrev" SkinID="PrevPage"></asp:HyperLink>

        <asp:Literal ID="litScroller" runat="server"/>
         
             <asp:HyperLink runat="server" ID="lnkNext" SkinID="NextPage"></asp:HyperLink>   
		                 
    		              <%If NumberOfResults > RecordsPerPage Then%>
		                 &nbsp;&nbsp;<%="(" & NumberOfResults & " " & GetLabel("resultsfound") & ")"%>
		                 <%end if %>		      
            <%else %>
            <%=GetLabel("noresultsfound")%>;
            <%end if%>
                    </td>
            </tr>
        </table>
</div>
          <arco:PageFooter id="lblFooter" runat="server"/>
    </div>

          <asp:LinkButton ID="lnkSearch" runat="server" />
    </form>
</body>
</html>
