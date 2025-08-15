<%@ Page Language="VB" AutoEventWireup="false" CodeFile="TransferWork.aspx.vb" Inherits="UserBrowser_TransferWork" MasterPageFile="~/masterpages/Toolwindow.master" %>

<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls" TagPrefix="Arco" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <script type="text/javascript">
        function RefreshOnChange() {
            document.forms[0].submit();
        }
    </script>

    <div id="tblCreate" class="container-fluid detail-form-container">
        <div class="row detail-form-row level0">
            <div class="InfoLabel col-12">
                <%= String.Format(GetLabel("transferworkwarning"), lblUserName.Text) %>
            </div>
        </div>
        <div class="row detail-form-row level0">
            <div class="col-md-4 LabelCell">
                <asp:Label runat="server" ID="lblFrom" CssClass="Label" Text="From" />
            </div>
            <div class="col-md-8 FieldCell">
                <asp:Label runat="server" ID="lblUserName" />
            </div>
        </div>
        <div class="row detail-form-row level0">
            <div class="col-md-4 LabelCell">
                <asp:Label runat="server" ID="lblTo" CssClass="Label" />
            </div>
            <div class="col-md-8 FieldCell">
                <Arco:SelectSubject ID="newUser" runat="server" RefreshOnChange="true" UsersOnly="false" HideDeleteIcon="true" />
            </div>
        </div>
        <div class="row detail-form-row level0">
            <div class="col-md-4 LabelCell">
                <asp:Label runat="server" ID="lblProcedure" CssClass="Label" Text="Procedure" />
            </div>
            <div class="col-md-8 FieldCell">
                <telerik:RadComboBox ID="cmbProcID" runat="server" Width="300px" AllowCustomText="False" MarkFirstMatch="True" />
            </div>
        </div>
        <div id="trCommands" class="row mt-2">
            <div class="col">
                <div class="buttons">
                    <Arco:ButtonPanel ID="pnlButtons" runat="server">
                        <Arco:OkButton ID="cmdSave" runat="server"></Arco:OkButton>
                        <Arco:CancelButton ID="cmdCancel" runat="server" OnClientClick="javascript:Close();return false;"></Arco:CancelButton>
                    </Arco:ButtonPanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
