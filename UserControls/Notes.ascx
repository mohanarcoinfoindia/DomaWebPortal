<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radE" %>
<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Notes.ascx.vb" Inherits="UserControls_Notes" %>
   
<div style="<%=ExtraStyle%>">   
    <div id="myDiv" class="comments" style="zoom: 1;">

        <asp:Panel ID="pnlQuickAdd" runat="server" CssClass="addComment">
           
            <div class="w-100 my-2">
                <radE:RadEditor ID="txtQuickAdd" runat="server" SkinID="EditHtmlField"></radE:RadEditor>
            </div>
            <asp:LinkButton ID="lnkQuickAdd" runat="server" Text="Add" SkinID="OkButton"></asp:LinkButton>          
        </asp:Panel>

        <div class="commentLines">
            <table style="width: 100%" cellspacing="0" cellpadding="0">

                <asp:Repeater ID="NotesList" runat="server" OnItemDataBound="notes_ItemDataBound" EnableViewState="false">
                    <HeaderTemplate>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <table cellspacing="0" cellpadding="0" width="100%" class="commentLine">
                                    <tr>
                                        <td class="ReadOnlyFieldCell" style="white-space:normal">
                                            <asp:Label ID="lblContentGrid" runat="server"></asp:Label>
                                            <br />
                                            <div class="commentInfo">
                                                <%#ArcoFormatting.FormatUserName(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "NoteCreator").ToString)%> - <%#ArcoFormatting.FormatDateLabel(CType(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "NoteModified"), DateTime), True, False, False)%>
                                            </div>
                                        </td>
                    
                                        <asp:PlaceHolder ID="pnlEditGrid" runat="server" Visible="true">
                                            <td class="SideHeaderCell">
                                                <div class="commentLineActions">
                                                    <asp:PlaceHolder ID="plhEditButton" runat="server" Visible="true">
                                                        <div>
                                                            <a href="javascript:EditNote(<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "NoteID")%>);" >
                                                                <%= ThemedImage.GetSpanIconTag("icon icon-edit", GetLabel("edit"))%>
                                                            </a>
                                                        </div>
                                                    </asp:PlaceHolder> 
                                                    <asp:PlaceHolder ID="plhDeleteButton" runat="server" Visible="true">
                                                        <div>
                                                            <a href="javascript:PC().DeleteCaseNote(<%# DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "NoteID")%>);" >
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
                            <a href="javascript:NewNote();" class="ButtonLink" >
                                <asp:Literal ID="litFullAdd" runat="server"></asp:Literal>    
                            </a> 
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plhNoResults" runat="server" Visible="false" EnableViewState="false">
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </table>
        </div>
    </div>
</div>

  

<asp:ObjectDataSource ID="notesListDataSource" runat="server" SelectMethod="GetCaseNoteList"
    TypeName="Arco.Doma.Library.Routing.NoteList">
    <SelectParameters>       
        <asp:Parameter Name="caseID" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>