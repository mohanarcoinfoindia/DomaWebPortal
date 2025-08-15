<%@ Page Language="VB" MasterPageFile="~/masterpages/ToolWindow.master" AutoEventWireup="false" CodeFile="PromptTaskSet.aspx.vb" Inherits="UserControls_PromptTaskSet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <script type="text/javascript">
        function SetPrompt() {
            var oWindow = GetRadWindow();
            var e = document.getElementById("<%=drpTaskSet.ClientID%>"); // select element           
            var p = e.options[e.selectedIndex].value;
            oWindow.argument = p;
            oWindow.Close();
        }
    </script>

    <div class="container-fluid detail-form-container">
        <div id="rowNotFound" class="row detail-form-row" visible="false" runat="server">
            <div class="col-md-8 offset-md-4 FieldCell">
                <asp:Label ID="lblNotFOund" runat="server" CssClass="ErrorLabel"></asp:Label>
            </div>
        </div>

        <div id="rowSet" class="row detail-form-row" runat="server">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblSet" runat="server" CssClass="Label"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:DropDownList ID="drpTaskSet" runat="server"></asp:DropDownList>
            </div>
        </div>
        
        <div class="row detail-form-row">
            <div class="col-md-8 offset-md-4 FieldCell">
                <Arco:ButtonPanel ID="ButtonPanel1" runat="server">
                    <Arco:OkButton runat="server" ID="btnOk" OnClientClick="javascript:SetPrompt();return false;"></Arco:OkButton>
                    <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" ID="btnCancel"></Arco:CancelButton>
                </Arco:ButtonPanel>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        AdjustWindow();
    </script>
</asp:Content>

