<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TaskSets.aspx.vb" Inherits="TaskSets" Strict="false" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Task Sets</title>     
</head>
<body>
    <form id="form1" runat="server" defaultbutton="lnkSearch">
        <script type="text/javascript">
           function EditTaskSet(id) {
              PC().OpenModalWindowRelativeSize('Taskset.aspx?ID=' + id, true);
          }
             function DeleteEntry(id, name) {
                if (confirm(UIMessages[29].replace("#ITEM#", name))) {
                    $get('<%=deltaskset.Clientid %>').value = id;
                    Reload()
                }              
            }

            var LastPage = <%=LastPage%>;
            var orderbyField = '<%=orderby.uniqueid %>';
            var orderbyorderField = '<%=orderbyorder.uniqueid %>';

            function Goto(page) {
                if (((parseInt(page) <= LastPage) && (parseInt(page) > 0)) || (page == 1)) {
                    $get("Page").value = page;
                    Reload();
                }
            }

            function OrderBy(field) {
                if (field == $get(orderbyField).value) {
                    if ($get(orderbyorderField).value == "DESC") {
                        $get(orderbyorderField).value = "ASC";
                    }
                    else {
                        $get(orderbyorderField).value = "DESC";
                    }
                }
                else {
                    $get(orderbyorderField).value = "ASC";
                }
                $get(orderbyField).value = field;
                Reload();
            }

            function Reload() {
                document.forms[0].submit();
            }

            function Refresh() {
                Reload();
            }
        </script>

        <Arco:PageController ID="PC" runat="server" PageLocation="ListPage" />

        <input type="hidden" id="Page" name="Page"/>
        <input type="hidden" id="orderby" runat="server" value="SET_NAME"/>
        <input type="hidden" id="orderbyorder" runat="server" value="ASC"/>
        <input type="hidden" id="deltaskset" runat="server"/>
    
        <div class="tasksets">
            <table class="List HoverList PaddedTable StickyHeaders">
                <thead>
                    <tr class="ListHeader">
                        <th><%=GetLabel("category") %></th>   
                        <th onclick="javascript:OrderBy('SET_NAME');" style="cursor:pointer"><%=GetLabel("name") %></th>
                        <th></th>
                        <th></th>
                    </tr>
                <tr class="ListFilter">
                    <td></td>
                    <td>
                        <asp:TextBox runat="server" ID="txtFilterName"/>
                    </td>
                    <td></td>
                    <td></td>
                </tr>
                </thead>
                <tbody>
                    <asp:Repeater runat="server" ID="lstTaskSets" EnableViewState="false">
                        <HeaderTemplate></HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <%# GetCategory(CType(Container, RepeaterItem).DataItem)%>                
                                </td>
                                <td>
                                    <%#Server.HtmlEncode(CType(Container, RepeaterItem).DataItem.Name)%>   
                                </td>
                                <td class="iconCell">
                                    <asp:PlaceHolder ID='plEdit' runat='server'>
                                        <a class="ButtonLink" href='Javascript:EditTaskSet(<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>);'">
                                            <span class="icon icon-edit"></span>
                                        </a>
                                    </asp:PlaceHolder>
                                </td>
                                 <td class="iconCell">
                            <%#"<span class='icon icon-delete' onclick='DeleteEntry(" & DataBinder.Eval(Container.DataItem, "ID").ToString & "," & EncodingUtils.EncodeJsString(Eval("Name").ToString) & ")' title='" & GetLabel("delete") & "' />"%>
                        </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate></FooterTemplate>
                        
                    </asp:Repeater>
                </tbody>
                <tr class="ListFooter">
                    <td  colspan="4">
                        <%if NumberOfResults > 0 Then%>
                        <asp:HyperLink runat="server" ID="lnkPrev" SkinID="PrevPage"></asp:HyperLink>
                        <asp:Literal ID="litScroller" runat="server"/>
                        <asp:HyperLink runat="server" ID="lnkNext" SkinID="NextPage"></asp:HyperLink>   
            		    <%If NumberOfResults > RecordsPerPage Then%>
            		    &nbsp;&nbsp;<%="(" & NumberOfResults & " " & GetLabel("resultsfound") & ")"%>
            		    <%end If %>		      
                        <%else %>
                        <%=GetLabel("noresultsfound")%>;
                        <%end if%>
                    </td>
                </tr>
            </table>
            <arco:PageFooter id="lblFooter" runat="server"/>
            <asp:LinkButton ID="lnkSearch" runat="server" />
        </div>
    </form>
</body>
</html>
