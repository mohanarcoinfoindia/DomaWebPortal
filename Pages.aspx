<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Pages.aspx.vb" Inherits="Pages" Strict="false" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>    
</head>
<body>
    <form id="form1" runat="server">
        <script type="text/javascript">
            function EditPage(id) {
                const currentPage = GetCurrentPageField().value;
                const parentId = GetParentId();
                location.href = 'Page.aspx?pageid=' + id + '&DM_PARENT_ID=' + parentId + '&mode=1&returnto=pages&returntoPaging=' + currentPage;
                
            }

            function NewPage() {
                location.href = './CMS/EditPage.aspx';
            }

            function DeletePage(id) {
                if (confirm(UIMessages[4])) {                   
                    document.forms[0].elements['<%=delpage.Clientid %>'].value = id;
                    Reload();
                }
            }
             function CopyPage(id) {
             
                document.forms[0].elements['<%=copypage.Clientid %>'].value = id;
                Reload();
             
            }
            function ViewPage(id) {
                const parentId = GetParentId();
                location.href = 'Page.aspx?pageid=' + id + '&DM_PARENT_ID=' + parentId;

            }

            function GetParentId() {
                let parentId = parseInt($get('txtContextId').value);
                if(isNaN(parentId) || parentId < 0) {
                    parentId = 0;
                }
                return parentId;
            }

            var LastPage = <%=LastPage%>;
            var orderbyField = '<%=orderby.uniqueid %>';
            var orderbyorderField = '<%=orderbyorder.uniqueid %>';

            function Goto(page) {
                if (((parseInt(page) <= LastPage) && (parseInt(page) > 0)) || (page == 1)) {
                    const currentPage = document.getElementById('<%= CurrentPage.ClientID %>');
                    GetCurrentPageField().value = page;
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

            function ConfirmDelete(message) {
                const confirmField = '<%=confirmDelete.Clientid %>';
               if (confirm(message)) {
                   document.forms[0].elements[confirmField].value = "True";
                   Reload();
               } else {
                   const delIdField = '<%=delpage.Clientid %>';
                    document.forms[0].elements[confirmField].value = "";
                    document.forms[0].elements[delIdField].value = "";
                }
            }

            function Reload() {
                document.forms[0].submit();
            }

            function GetCurrentPageField() {
                return document.getElementById('<%= CurrentPage.ClientID %>');
            }
        </script>

        <div style="margin-right:10px">  
            <Arco:PageController ID="thisPageController" runat="server" PageLocation="ListPage" />
            <asp:HiddenField ID="CurrentPage" runat="server" />
            <input type="hidden" name="orderby" id="orderby" runat="server" value=""/>
            <input type="hidden" name="orderbyorder" id="orderbyorder" runat="server"/>
            <input type="hidden" name="delpage" id="delpage" runat="server"/>
            <input type="hidden" name="copypage" id="copypage" runat="server"/>
            <input type="hidden" name="condel" id="confirmDelete" runat="server" />
            <input type="hidden" name="viewpage" id="viewpage" runat="server"/>

            <table class="List PaddedTable StickyHeaders">
              <tr class="ListHeader">
                  <th onclick="javascript:OrderBy('ID');" style="cursor:pointer"><%=GetLabel("id") %></th>
                  <th onclick="javascript:OrderBy('NAME');" style="cursor:pointer"><%=GetLabel("name") %></th>
                <th onclick="javascript:OrderBy('DESCRIPTION');" style="cursor:pointer"><%=GetLabel("description") %></th>
                   <th>&nbsp;</th>
                <th>&nbsp;</th>
                <th>&nbsp;</th>
              </tr>
               <asp:Repeater runat="server" ID="lstQueries" EnableViewState="false">
                <HeaderTemplate></HeaderTemplate>
                <ItemTemplate>
                    <tr>        
                        <td>
                            <%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>
                        </td>
                        <td >
                          <a  class="ButtonLink" href='Javascript:ViewPage(<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>);'>
                              <%#Server.HtmlEncode(CType(Container, RepeaterItem).DataItem.Name)%>
                          </a>                                      
                        </td>   
                         <td style="width:100%">
                          <a  class="ButtonLink" href='Javascript:ViewPage(<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>);'>
                              <%#Server.HtmlEncode(CType(Container, RepeaterItem).DataItem.Description)%>
                          </a>                                      
                        </td>     
                        <td>                     
                            <a class="ButtonLink" href='Javascript:EditPage(<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>);'">
                                <span class="icon icon-edit" ></span>
                            </a>                      
                        </td>
                          <td>                    
                            <a class="ButtonLink" href='Javascript:CopyPage(<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>);'">
                                <span class="icon icon-copy" ></span>
                            </a>                     
                        </td>
                        <td>
                     
                        <a  class="ButtonLink" href='Javascript:DeletePage(<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>);'>
                            <span class="icon icon-delete"></span>
                        </a>
                      
                        </td>                                              
                    </tr>
                </ItemTemplate>
                 <FooterTemplate></FooterTemplate>
            </asp:Repeater>
               <tr class="ListFooter">
                   <td  colspan="6">
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
               <tr class="ListFooter">
                   <td colspan="6">
                       <a class="ButtonLink" href='Javascript:NewPage();'">
                           <span class="icon icon-add-new" />
                       </a>
                    </td>
               </tr>
            </table>
            <div class="text-center">
                <asp:Label runat="server" ID="lblContextId" AssociatedControlID="txtContextId" CssClass="mr-3"></asp:Label>
                <asp:TextBox runat="server" ID="txtContextId" TextMode="Number" min="0" Style="width:100px;"></asp:TextBox>
            </div>
        </div>
    </form>   
</body>
</html>
