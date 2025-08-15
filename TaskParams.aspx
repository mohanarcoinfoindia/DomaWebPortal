<%@ Page Title="" Language="VB" MasterPageFile="~/masterpages/ToolWindow.master" AutoEventWireup="false" CodeFile="TaskParams.aspx.vb" Inherits="TaskParams" %>
<%@ Register TagPrefix="arcoctrls" Namespace="Arco.Doma.WebControls" Assembly="Arco.Doma.WebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
    
		<div style="min-height:150px">
		    <table class="DetailTable">		   
		    <tr><td>
		      <asp:PlaceHolder id="sharedcalenderPlaceHolder" runat="server"></asp:PlaceHolder>
			<arcoctrls:DMObjectForm id="docform" Runat="server" ModalWindow="true"></arcoctrls:DMObjectForm>
			</td></tr>

            <tr><td>          
            <Arco:ButtonPanel id="pnlButtons" runat="server">
                        <Arco:OkButton ID="cmdSave" runat="server" ValidationGroup="CheckMandatoryFields" Text="Ok" OnClientClick="return FormOk();"></Arco:OkButton>
                        <Arco:CancelButton ID="lblCancel" runat="server" OnClientClick="javascript:Close();return false;" Text="Cancel"></Arco:CancelButton>
                    </Arco:ButtonPanel>         

            </td></tr>

			</table>
			
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

             Sys.Application.add_load(UpgradeASPNETValidation);
        </script>
        </asp:Content>

