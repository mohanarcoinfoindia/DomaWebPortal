<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_EDIT_ACTION_PROFILE.aspx.vb" Inherits="DM_EDIT_ACTION_PROFILE" MasterPageFile="~/masterpages/Toolwindow.master" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <script type="text/javascript">
        function SetActionType(type) {
            document.forms[0].actiontype.value = type;
        }
        function SetFormValues(type, id, subid) {
            SetActionType(type);
            document.forms[0].actionid.value = id;
            document.forms[0].subactionid.value = subid;
        }

        function AddAction(id, subid) {
            SetFormValues("add", id, subid);
            Submit();
        }
        function RemoveAction(id, subid) {
            SetFormValues("remove", id, subid);
            Submit();
        }
        function Save() {
            Page_ClientValidate();
            if (Page_IsValid) {
                SetActionType("save");
                Submit();
            }
        }
        function Submit() {
            document.forms[0].submit();
        }
    </script>

    <input type="hidden" name="profileid" value="<%=msID%>" />
    <input type="hidden" name="actionid" value="" />
    <input type="hidden" name="subactionid" value="" />
    <input type="hidden" name="actiontype" value="" />

    <div id="tblEditProfile" class="container-fluid detail-form-container" runat="server">
        <div class="row DetailHeader">
            <div class="col DetailHeaderContent">
                <asp:Label ID="lblHeader4" runat="server" Text="Edit Profile"></asp:Label>
            </div>
        </div>
        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblName" runat="server" Text="Name"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox ID="txtProfileName" runat="server" EnableViewState="true" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ID="reqName" ControlToValidate="txtProfileName" ErrorMessage="*" SetFocusOnError="true"></asp:RequiredFieldValidator>
            </div>
        </div>
        <div class="row detail-form-row">
            <div class="col-md-8 offset-md-4 FieldCell">
                <Arco:ButtonPanel runat="server">
                    <Arco:OkButton ID="lnkSave" runat="server" OnClientClick="javascript:Save();return false;"  ></Arco:OkButton>
                   <Arco:CancelButton ID="lnkClose" runat="server" OnClientClick="javascript:self.close();return false;" ></Arco:CancelButton>
                </Arco:ButtonPanel>
            </div>
        </div>
        <div class="row">
            <div class="container-fluid" id="pnlProfileActions" runat="server" horizontalalign="Left" verticalalign="Top">
            </div>
        </div>
    </div>
</asp:Content>
