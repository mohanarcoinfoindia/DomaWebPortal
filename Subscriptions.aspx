<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Subscriptions.aspx.vb" Inherits="Subscriptions" Strict="false" %>

<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Subscriptions</title>
</head>
<body>
    <form id="form1" runat="server" defaultbutton="lnkSearch">
        <script type="text/javascript">   
            var LastPage = '<%=LastPage%>';
            var orderbyField = '<%=orderby.uniqueid %>';
            var orderbyorderField = '<%=orderbyorder.uniqueid %>';

            function Delete(id) {
                if (confirm(UIMessages[4])) {
                    var idfield = '<%=delsubs.clientid %>';
                    document.forms[0].elements[idfield].value = id;
                    Refresh()
                }
            }

            function Edit(id) {
                PC().OpenModalWindowRelativeSize('Subscription.aspx?ID=' + id, true);
            }

            function Goto(page) {
                if (((parseInt(page) <= LastPage) && (parseInt(page) > 0)) || (page == 1)) {
                    document.forms[0].Page.value = page;
                    Refresh();
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
                Refresh();
            }

            function Refresh() {
                document.forms[0].submit();
            }

        </script>
        <div>
            <Arco:PageController ID="thisPageController" runat="server" PageLocation="ListPage" />

            <input type="hidden" id="Page" name="Page" />
            <input type="hidden" name="orderby" id="orderby" runat="server" value="" />
            <input type="hidden" name="orderbyorder" id="orderbyorder" runat="server" />
            <input type="hidden" name="delsubs" id="delsubs" runat="server" />

            <div id="tblSearch" class="container-fluid detail-form-container" runat="server">
                <div class="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblFor" runat="server" CssClass="Label" Text="For" AssociatedControlID="txtSubject"></asp:Label>
                    </div>
                    <div class="col-md-8 FieldCell">
                        <Arco:SelectSubject ID="txtSubject" runat="server" RefreshOnChange="false" UsersOnly="false" />
                    </div>
                </div>
                <div class="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblOn" runat="server" CssClass="Label" Text="on" AssociatedControlID="txtOn"></asp:Label>
                    </div>
                    <div class="col-md-8 FieldCell">
                        <asp:TextBox ID="txtOn" runat="server" Width="400px" SkinID="CustomWidth"></asp:TextBox>
                    </div>
                </div>
                <div class="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblWhen" runat="server" CssClass="Label" Text="When" AssociatedControlID="drpJobs"></asp:Label>
                    </div>
                    <div class="col-md-8 FieldCell">
                        <asp:DropDownList ID="drpJobs" runat="server">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row detail-form-row">
                    <div class="col-md-8 offset-md-4 FieldCell">
                        <Arco:ButtonPanel runat="server">
                            <Arco:OkButton ID="lnkSearch" OnClientClick="Goto(0)" runat="server"></Arco:OkButton>
                        </Arco:ButtonPanel>
                    </div>
                </div>
            </div>

            <table class="List PaddedTable HoverList StickyHeaders">
                <asp:Repeater runat="server" ID="lstQueries" EnableViewState="false">
                    <HeaderTemplate>
                        <tr class="ListHeader">
                            <th style="cursor: pointer" onclick="javascript:OrderBy('SUBJECT_ID');"><%=GetLabel("subs_subject")%></th>
                            <th><%=GetLabel("subs_object")%></th>
                            <th><%=GetLabel("subs_schedule")%></th>
                            <th style="cursor: pointer" onclick="javascript:OrderBy('CREATED_BY');"><%=GetLabel("createdby")%></th>
                            <th></th>
                            <th></th>
                        </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%#SubscriptionSubject(CType(Container, RepeaterItem).DataItem)%>
                            </td>
                            <td>
                                <%#SubScriptionDetails(CType(Container, RepeaterItem).DataItem)%>                                             
                            </td>
                            <td>
                                <%#SubScriptionSchedule(CType(Container, RepeaterItem).DataItem)%>                                             
                            </td>
                            <td>
                                <%#ArcoFormatting.FormatUserName(CType(Container, RepeaterItem).DataItem.createdBy)%>
                            </td>
                            <td>
                                <a class="ButtonLink" href='Javascript:Edit(<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>);'>
                                   <span class="icon icon-edit"></span>
                                </a>
                                &nbsp
                                <a class="ButtonLink" href='Javascript:Delete(<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "ID")%>);'>
                                    <span class="icon icon-delete"></span>
                                </a>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </asp:Repeater>

                <tr class='ListFooter'>
                    <td colspan="5">

                        <%if NumberOfResults > 0 then%>

                        <asp:HyperLink runat="server" ID="lnkPrev" SkinID="PrevPage"></asp:HyperLink>

                        <asp:Literal ID="litScroller" runat="server" />

                        <asp:HyperLink runat="server" ID="lnkNext" SkinID="NextPage"></asp:HyperLink>

                        <%If NumberOfResults > RecordsPerPage Then%>
		                 &nbsp;&nbsp;<%="(" & NumberOfResultsLabel & " " & GetLabel("resultsfound") & ")"%>
                        <%end if %>

                        <%else %>
                        <%=GetLabel("noresultsfound") %>
                        <%end if%>
                    </td>
                </tr>

                <tr>
                    <td align="right" colspan="5">
                        <asp:HyperLink runat="server" ID="lnkBack" CssClass="ButtonLink" Text="Back"></asp:HyperLink>
                    </td>
                </tr>
            </table>

        </div>
    </form>
</body>
</html>
