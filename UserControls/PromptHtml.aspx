<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radE" %>
<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PromptHtml.aspx.vb" Inherits="UserControls_PromptText" MasterPageFile="~/masterpages/Toolwindow.master"   %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server" MasterPageFile="~/masterpages/Toolwindow.master">
<script type="text/javascript">
    function SetPrompt() {
        var oWindow = GetRadWindow();
        var editor = $find('<%=htmlEditor.ClientID%>');
        oWindow.argument = editor.get_html();
        oWindow.Close();
    }
</script>
<div style="display: flex; justify-content: center;">
    <asp:Table ID="tbl" runat="server">
        <asp:TableRow>
            <asp:TableCell>
            <radE:RadEditor ID="htmlEditor" runat="server" SkinID="EditHtmlField"></radE:RadEditor>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell>
                <Arco:ButtonPanel ID="ButtonPanel1" runat="server" style="margin-top: 10px;">
                    <Arco:OkButton runat="server" ID="btnUploadUrl" OnClientClick="javascript:SetPrompt();return false;" ></Arco:OkButton>
                    <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" id="btnCancelUrl"></Arco:CancelButton>
                </Arco:ButtonPanel>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
</div>


</asp:Content>
