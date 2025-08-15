<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PromptDate.aspx.vb" Inherits="UserControls_PromptDate"  MasterPageFile="~/masterpages/Toolwindow.master"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
<script type="text/javascript">
    function SetPrompt() {
        var oWindow = GetRadWindow();       
        var d = $find("<%=d1.ClientID %>").get_selectedDate();
        if (d) {
            d = d.format("<%=ArcoFormatting.GetDateTimeFormatForSave(False)%>")
        }
        else {
            d = '';
        }      
        oWindow.argument = d;
        oWindow.Close(); 
    }
</script>
    <div style="height:345px;width:325px; margin-left: 30px;">
<asp:Table ID="tbl" runat="server">
<asp:TableRow>
<asp:TableCell>
<telerik:RadDateTimePicker ID="d1" runat="server" CssClass="mt-4">
</telerik:RadDateTimePicker>
</asp:TableCell>
</asp:TableRow>
<asp:TableRow>
<asp:TableCell CssClass="pt-3">
<Arco:ButtonPanel ID="ButtonPanel1" runat="server">
<Arco:OkButton runat="server" ID="btnUploadUrl" OnClientClick="javascript:SetPrompt();return false;" ></Arco:OkButton>
<Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" id="btnCancelUrl"></Arco:CancelButton>
</Arco:ButtonPanel>
</asp:TableCell>
</asp:TableRow>
</asp:Table>
    </div>

    <script type="text/javascript">
        AdjustWindow();
    </script>
</asp:Content>
