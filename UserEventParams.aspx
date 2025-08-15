<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UserEventParams.aspx.vb" Inherits="UserEventParams" ValidateRequest="false" MasterPageFile="~/masterpages/Toolwindow.master" %>

<%@ Register TagPrefix="arcoctrls" Namespace="Arco.Doma.WebControls" Assembly="Arco.Doma.WebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <asp:PlaceHolder ID="sharedcalenderPlaceHolder" runat="server"></asp:PlaceHolder>
    <arcoctrls:DMObjectForm ID="docform" runat="server" ModalWindow="true"></arcoctrls:DMObjectForm>
    <div class="container-fluid detail-form-container">
        <div class="row detail-form-row">
            <div class="col-md-8 offset-md-4 FieldCell">
                <Arco:ButtonPanel ID="pnlButtons" Style="margin-top: 20px;" runat="server">
                    <Arco:OkButton ID="cmdSave" runat="server" ValidationGroup="CheckMandatoryFields" Text="Ok" OnClientClick="return FormOk();"></Arco:OkButton>
                    <Arco:CancelButton ID="lblCancel" runat="server" OnClientClick="javascript:Close();return false;" Text="Cancel"></Arco:CancelButton>
                </Arco:ButtonPanel>
            </div>
        </div>
    </div>
    
    <asp:LinkButton ID="lnkrefresh" runat="server"></asp:LinkButton>
    <script type="text/javascript">
        function FormOk() {
            if (typeof Page_ClientValidate === "function") {
                Page_ClientValidate('CheckMandatoryFields');
                return Page_IsValid;
            }
            else {
                return true;
            }
        }

        function RefreshOnChange() {
            SaveObject();
        }

        document.onkeydown = function (evt) {
            evt = evt || window.event;
            if (evt.keyCode == 27) {   // 27=esc 
                Close();
            }
        };


        Sys.Application.add_load(UpgradeASPNETValidation);
    </script>
</asp:Content>
