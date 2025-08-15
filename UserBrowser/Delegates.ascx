<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Delegates.ascx.vb" Inherits="DelegatesControl" %>

<script type="text/javascript">          
    function DeleteDelegate(id) {
        if (confirm(UIMessages[4])) {
            $get('<%=DelegateIDToDelete.ClientID%>').value = id;
            Refresh();
        }

    }

    function EditDelegate(id) {
        // var oWnd2 = radopen("EditDelegate.aspx?id=" + id, "Popup", 600, 570);
        //// oWnd2.fullScreen = true;
        // oWnd2.add_close(Refresh);

        PC().OpenModalWindowRelativeSize("EditDelegate.aspx?id=" + id, true);
    }		   
</script>

<asp:HiddenField ID="DelegateIDToDelete" runat="server" />

<div style="margin: 10px">
    <table class="SubList PaddedTable">
        <tr>
            <td>&nbsp;<asp:Label runat="server" ID="lblTileDelFromMe" CssClass="Label" />:
                                            <br style="line-height: 0.5;" />
            </td>
        </tr>
        <tr>
            <td>
                <table class="SubList" id="Table3">
                    <asp:Repeater ID="repFromMe" runat="server">
                        <HeaderTemplate>
                            <tr>
                                <th style="width: 200px;"><%#GetLabel("del_to")%></th>
                                <th style="width: 200px;"><%#GetLabel("procedure")%></th>
                                <th style="width: 100px;"><%#GetLabel("del_mode")%></th>
                                <th style="width: 100px;"><%#GetLabel("del_begin")%></th>
                                <th style="width: 100px;"><%#GetLabel("del_end")%></th>
                                <th style="width: 16px;"></th>
                                <th style="width: 16px;"></th>
                            </tr>
                        </HeaderTemplate>

                        <ItemTemplate>
                            <tr>
                                <td style="width: 200px"><%#DataBinder.Eval(Container.DataItem, "TO")%></td>
                                <td style="width: 200px;"><%#DataBinder.Eval(Container.DataItem, "PROCEDURE")%></td>
                                <td style="width: 100px;"><%#DataBinder.Eval(Container.DataItem, "MODE")%></td>
                                <td style="width: 100px;"><%#DataBinder.Eval(Container.DataItem, "BEGIN")%></td>
                                <td style="width: 100px;"><%#DataBinder.Eval(Container.DataItem, "END")%></td>
                                <td>
                                    <span class="icon icon-edit" onclick="EditDelegate('<%#DataBinder.Eval(Container.DataItem, "ID")%>')" title="<%#GetLabel("edit")%>" />
                                </td>
                                <td>
                                    <span class="icon icon-delete" onclick="DeleteDelegate('<%#DataBinder.Eval(Container.DataItem, "ID")%>')" title="<%#GetLabel("delete")%>" />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr runat="server" id="tr2" visible="false" class="ListFooter">
                        <td colspan="7">
                            <asp:Label runat="server" ID="lblNoDelatesFound2" /></td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr style="vertical-align: top;" class="ListFooter">
            <td valign="top" id="td_del_actions" runat="server">
                <div class="SubListHeader3RightFillerDivSwitched" id="div_del_actions" runat="server">
                    <div class="rounded-headerR">
                        <div class="rounded-headerL">
                            <div class="SubListMainHeaderT">
                                <span runat="server" id="del_add" class="icon icon-add-new" onclick="javascript:AddDelegate();" />
                            </div>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td>&nbsp;<asp:Label runat="server" ID="lblTileDelToMe" Text="New" CssClass="Label" />:
                                            <br style="line-height: 0.5;" />
            </td>
        </tr>
        <tr>
            <td>
                <table class="SubList" id="documentlist">
                    <asp:Repeater ID="repToMe" runat="server">
                        <HeaderTemplate>
                            <tr>
                                <th style="width: 200px;"><%#GetLabel("del_from")%></th>
                                <th style="width: 200px;"><%#GetLabel("procedure")%></th>
                                <th style="width: 100px;"><%#GetLabel("del_mode")%></th>
                                <th style="width: 100px;"><%#GetLabel("del_begin")%></th>
                                <th style="width: 132px;"><%#GetLabel("del_end")%></th>
                            </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style="width: 200px;"><%#DataBinder.Eval(Container.DataItem, "FROM")%></td>
                                <td style="width: 200px;"><%#DataBinder.Eval(Container.DataItem, "PROCEDURE")%></td>
                                <td style="width: 100px;"><%#DataBinder.Eval(Container.DataItem, "MODE")%></td>
                                <td style="width: 100px;"><%#DataBinder.Eval(Container.DataItem, "BEGIN")%></td>
                                <td style="width: 132px;"><%#DataBinder.Eval(Container.DataItem, "END")%></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr runat="server" id="tr3" visible="false" class="ListFooter">
                        <td colspan="7">
                            <asp:Label runat="server" ID="lblNoDelatesFound" /></td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

</div>
