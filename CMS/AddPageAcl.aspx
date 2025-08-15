<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddPageAcl.aspx.vb" Inherits="CMS_AddPageAcl" MasterPageFile="~/masterpages/Toolwindow.master" %>

<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls" TagPrefix="Arco" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <script type="text/javascript">
        function RefreshOnChange() {
            document.forms[0].submit();
        }
    </script>

    <div ID="tbl" runat="server" class="container-fluid detail-form-container mt-2">
        <div class="row detail-form-row">
            <div class="col FieldCell">
                <Arco:SelectSubject ID="newUser" runat="server" RefreshOnChange="true" UsersOnly="false" HideDeleteIcon="true" ShowRoleEveryone="true" />
            </div>
        </div>

        <div class="row detail-form-row">
            <div class="col buttons">
                <Arco:ButtonPanel ID="pnlButtons" runat="server">
                    <Arco:OkButton ID="cmdSave" runat="server" Text="Save"></Arco:OkButton>
                    <Arco:CancelButton ID="cmdCancel" runat="server" OnClientClick="javascript:Close();return false;"></Arco:CancelButton>
                </Arco:ButtonPanel>
            </div>
        </div>
    </div>

</asp:Content>
