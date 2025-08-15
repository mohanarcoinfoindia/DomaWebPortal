<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radE" %>
<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" TagPrefix="ArcoControls" %>


<%@ Control Language="VB" AutoEventWireup="true" CodeFile="Comments.ascx.vb" Inherits="UserControls_Comments" %>
<div style="<%=ExtraStyle%>">
    <div class="comments" style="zoom: 1;">

        <asp:Panel ID="pnlQuickAdd" runat="server" CssClass="addComment">

            <radE:RadEditor ID="txtQuickAddHtml" runat="server" SkinID="EditHtmlField" />
            <asp:TextBox ID="txtQuickAddText" runat="server" TextMode="MultiLine" width="100%" rows="5" EnableTheming="false" />

            <div class="commentPrivateAdd">
                <ArcoControls:CheckBox runat="server" ID="chkPrivate" />
                &nbsp;
             <asp:LinkButton ID="lnkQuickAdd" runat="server" SkinID="OkButton"></asp:LinkButton>
            </div>


        </asp:Panel>
        <div class="commentLines">
            <table style="width: 100%" cellspacing="0" cellpadding="0">

                <asp:Repeater ID="CommentList" runat="server" OnItemDataBound="comments_ItemDataBound" EnableViewState="false">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <table cellspacing="0" cellpadding="0" width="100%" class="commentLine">
                                    <tr>
                                        <td class="ReadOnlyFieldCell" style="white-space: normal">
                                            <asp:Label ID="lblContentGrid" runat="server"></asp:Label>
                                            <br />
                                            <div class="commentInfo">
                                                <%#CommentInfo(CType(CType(Container, RepeaterItem).DataItem, Arco.Doma.Library.FileList.FileInfo))%>
                                            </div>
                                        </td>

                                        <asp:PlaceHolder ID="pnlEditGrid" runat="server" Visible="true">
                                            <td class="SideHeaderCell">
                                                <div class="commentLineActions">
                                                    <asp:PlaceHolder ID="plhEditButton" runat="server" Visible="true">
                                                        <div>
                                                            <a href="javascript:PC().EditComment(<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "FILE_ID")%>);">
                                                                <%= ThemedImage.GetSpanIconTag("icon icon-edit", GetLabel("edit"))%>
                                                            </a>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                    <asp:PlaceHolder ID="plhDeleteButton" runat="server" Visible="true">
                                                        <div>
                                                            <a href="javascript:PC().DeleteComment(<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "FILE_ID")%>);">
                                                                <%= ThemedImage.GetSpanIconTag("icon icon-delete", GetLabel("delete"))%>
                                                            </a>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                </div>
                                            </td>
                                        </asp:PlaceHolder>
                                    </tr>
                                </table>
                            </td>
                        </tr>

                    </ItemTemplate>
                    <FooterTemplate>
                    </FooterTemplate>
                </asp:Repeater>

                <asp:PlaceHolder ID="plhFullAdd" runat="server" Visible="false" EnableViewState="false">
                    <tr>
                        <td align="right">
                            <a href="javascript:NewComment();" class="ButtonLink">
                                <asp:Literal ID="litFullAdd" runat="server"></asp:Literal>
                            </a>
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhNoResults" runat="server" Visible="false" EnableViewState="false">
                    <tr>
                        <td>&nbsp;
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </table>

        </div>
    </div>
</div>

<asp:ObjectDataSource ID="CommentListDataSource" runat="server" SelectMethod="GetComments"
    TypeName="Arco.Doma.Library.FileList">
    <SelectParameters>
        <asp:Parameter Name="vlObjectID" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>
