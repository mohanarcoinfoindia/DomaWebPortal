<%@ Page Language="VB" MasterPageFile="~/masterpages/ToolWindow.master" AutoEventWireup="false" CodeFile="PromptCategory.aspx.vb" Inherits="UserControls_PromptCategory" %>
<%@ Register TagPrefix="Arco" TagName="SelectCategory" Src="SelectCategory.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
    <script type="text/javascript">
        function SetPrompt() {
            var oWindow = GetRadWindow();
            var e = $find("<%=drpCat.ClientID %>_cmbCategory"); // select element           
            var p = e.get_value();        
        oWindow.argument = p;
        oWindow.Close();
    }
</script>
<asp:Table ID="tbl" runat="server" Width="100%">
    <asp:TableRow ID="rowPackage">
        <asp:TableCell CssClass="LabelCell">
            <asp:Label ID="lblCat" runat="server" CssClass="Label"></asp:Label>
        </asp:TableCell>
        <asp:TableCell CssClass="FieldCell">
            <arco:SelectCategory ID="drpCat" runat="server" />
        </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
        <asp:TableCell ColumnSpan="2">
            <Arco:ButtonPanel ID="ButtonPanel1" runat="server" style="margin-top: 20px;">
                <Arco:OkButton runat="server" ID="btnOk" OnClientClick="javascript:SetPrompt();return false;" ></Arco:OkButton>
                <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" id="btnCancel"></Arco:CancelButton>
            </Arco:ButtonPanel>
        </asp:TableCell>
    </asp:TableRow>
</asp:Table>
</asp:Content>

