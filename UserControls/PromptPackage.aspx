<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PromptPackage.aspx.vb" Inherits="UserControls_PromptPackage"   MasterPageFile="~/masterpages/Toolwindow.master"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
<script language="javascript">
    function SetPrompt() {
        var oWindow = GetRadWindow();
        var e = $get("<%=drpPack.ClientID %>"); // select element           
        var p = e.options[e.selectedIndex].value;        
        oWindow.argument = p;
        oWindow.Close();
    }
</script>
<asp:Table ID="tbl" runat="server" Width="100%">
<asp:TableRow ID="rowPackage">
<asp:TableCell CssClass="LabelCell">
<asp:Label ID="lblPack" runat="server" CssClass="Label"></asp:Label>
</asp:TableCell>
<asp:TableCell CssClass="FieldCell">
<asp:DropDownList ID="drpPack" runat="server"></asp:DropDownList>
</asp:TableCell>
</asp:TableRow>
<asp:TableRow ID="rowNotFound" Visible="false">
<asp:TableCell CssClass="FieldCell" ColumnSpan="2" >
<asp:Label ID="lblNotFOund" runat="server" CssClass="ErrorLabel"></asp:Label>
</asp:TableCell>
</asp:TableRow>
<asp:TableRow>
<asp:TableCell ColumnSpan="2">
<Arco:ButtonPanel ID="ButtonPanel1" runat="server">
<Arco:OkButton runat="server" ID="btnOk" OnClientClick="javascript:SetPrompt();return false;" ></Arco:OkButton>
<Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" id="btnCancel"></Arco:CancelButton>
</Arco:ButtonPanel>
</asp:TableCell>
</asp:TableRow>
</asp:Table>

</asp:Content>
