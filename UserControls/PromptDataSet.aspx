<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PromptDataSet.aspx.vb" Inherits="UserControls_PromptDataSet" MasterPageFile="~/masterpages/Toolwindow.master"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
<script type="text/javascript">
    function SetPrompt() {
            var oWindow = GetRadWindow();
             var e = $get("<%=drpDataSet.ClientID %>"); // select element           
        var p = e.options[e.selectedIndex].value;              
        oWindow.argument = p;
        oWindow.Close();
    }
</script>
    <div style="height:345px;width:325px; margin-left: 30px;">
<asp:Table ID="tbl" runat="server">
<asp:TableRow ID="rowDataSet">
<asp:TableCell>
<asp:DropDownList ID="drpDataSet" runat="server"></asp:DropDownList>
</asp:TableCell>
</asp:TableRow>

    <asp:TableRow ID="rowNotFound" Visible="false">
<asp:TableCell CssClass="FieldCell" ColumnSpan="2" >
<asp:Label ID="lblNotFOund" runat="server" CssClass="ErrorLabel"></asp:Label>
</asp:TableCell>
</asp:TableRow>
    <asp:TableRow>
<asp:TableCell CssClass="pt-3">
<Arco:ButtonPanel ID="ButtonPanel1" runat="server">
<Arco:OkButton runat="server" ID="btnSelect" OnClientClick="javascript:SetPrompt();return false;" ></Arco:OkButton>
<Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" id="btnClose"></Arco:CancelButton>
</Arco:ButtonPanel>
</asp:TableCell>
</asp:TableRow>
</asp:Table>
    </div>

</asp:Content>