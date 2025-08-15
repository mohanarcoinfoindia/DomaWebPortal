<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Reports.aspx.vb" Inherits="Reports" Strict="false" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>    
</head>
<body>
    <form id="form1" runat="server">
        <script type="text/javascript">    
           
            function ViewReport(id) {
                PC().OpenModalWindow("Stats.aspx?Report=" + id, false, 800, 600, true, false);
            }

            var LastPage = <%=LastPage%>;
            var orderbyField = '<%=orderby.uniqueid %>';
            var orderbyorderField = '<%=orderbyorder.uniqueid %>';

            function Goto(page) {
                if (((parseInt(page) <= LastPage) && (parseInt(page) > 0)) || (page == 1)) {
                    document.forms[0].Page.value = page;
                    Reload();
                }
            }

            function OrderBy(field) {
                if (field == document.forms[0].elements[orderbyField].value) {
                    if (document.forms[0].elements[orderbyorderField].value == "DESC") {
                        document.forms[0].elements[orderbyorderField].value = "ASC";
                    }
                    else {
                        document.forms[0].elements[orderbyorderField].value = "DESC";
                    }
                }
                else {
                    document.forms[0].elements[orderbyorderField].value = "ASC";
                }
                document.forms[0].elements[orderbyField].value = field;
                Reload();
            }

            function Reload() {
                document.forms[0].submit();
            }
        </script>

        <div style="margin-right:10px">  
            <Arco:PageController ID="thisPageController" runat="server" PageLocation="ListPage" />
            <input type="hidden" id="Page" name="Page"/>
            <input type="hidden" name="orderby" id="orderby" runat="server" value=""/>
            <input type="hidden" name="orderbyorder" id="orderbyorder" runat="server"/>
           
            <table class="List PaddedTable StickyHeaders">
              <tr class="ListHeader">
                <th onclick="javascript:OrderBy('REPORT_NAME');" style="cursor:pointer"><%=GetLabel("name") %></th>
             
                
              </tr>
               <asp:Repeater runat="server" ID="lstQueries" EnableViewState="false">
                <HeaderTemplate></HeaderTemplate>
                <ItemTemplate>
                    <tr>                    
                        <td style="width:100%">
                          <a  class="ButtonLink" href='Javascript:ViewReport(<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>);'>
                              <%#Server.HtmlEncode(CType(Container, RepeaterItem).DataItem.Name)%>
                          </a>                                      
                        </td>                       
                        
                                                                  
                    </tr>
                </ItemTemplate>
                 <FooterTemplate></FooterTemplate>
            </asp:Repeater>
               <tr class="ListFooter">
                   <td  colspan="1">
                      <%if NumberOfResults > 0 Then%>          
                      <asp:HyperLink runat="server" ID="lnkPrev" SkinID="PrevPage"></asp:HyperLink>
                      <asp:Literal ID="litScroller" runat="server"/>
                      <asp:HyperLink runat="server" ID="lnkNext" SkinID="NextPage"></asp:HyperLink>
                      <%If NumberOfResults > RecordsPerPage Then%>
                      &nbsp;&nbsp;<%="(" & NumberOfResults & " " & GetLabel("resultsfound") & ")"%>
        	          <%end If %>		            
                      <%else %>
                      <%=GetLabel("noresultsfound")%>;
                      <%end If%>
                   </td>
               </tr>               
            </table>
        </div>
    </form>   
</body>
</html>
