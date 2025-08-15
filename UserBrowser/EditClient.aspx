<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EditClient.aspx.vb"  MasterPageFile="~/masterpages/Toolwindow.master" Inherits="UserBrowser_EditClient" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">

    <div class="container-fluid detail-form-container">

        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblName" runat="server" CssClass="Label" AssociatedControlID="txtName"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox MaxLength="200" runat="server" ID="txtName" />
                <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ID="reqName"
                    ControlToValidate="txtName" ErrorMessage="*" SetFocusOnError="true" />
            </div>
        </div>

        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblCallBacks" runat="server" CssClass="Label" AssociatedControlID="txtAllowedCallBacks"
                    Text="Url(s)" />
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox MaxLength="2000" runat="server" SkinID="MultiLine" ID="txtAllowedCallBacks"
                    TextMode="MultiLine" Rows="5" />
                  <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ID="reqAllowedCallback"
                    ControlToValidate="txtAllowedCallBacks" ErrorMessage="*" SetFocusOnError="true" />
            </div>
        </div>

         <div class="row detail-form-row" runat="server" ID="rowClientID" Visible="false">
            <div class="col-md-4 LabelCell">
                 <asp:Label ID="Label1" runat="server" CssClass="Label" AssociatedControlID="lblClientID" Text="Client Id"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                 <asp:Label ID="lblClientID" runat="server"></asp:Label>
            </div>
        </div>
        <div class="row detail-form-row" runat="server" ID="rowClientSecret" Visible="false">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="Label2" runat="server" CssClass="Label" AssociatedControlID="lblClientID" Text="Client Secret"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:Label ID="lblClientSecret" runat="server"></asp:Label>
            </div>
        </div>       

        <div class="row detail-form-row" runat="server">
            <div class="col buttons">
                <Arco:ButtonPanel ID="ButtonPanel1" runat="server">
                    <Arco:OkButton runat="server" ID="btnOk"></Arco:OkButton>
                    <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" ID="btnCancelUrl">
                    </Arco:CancelButton>
                </Arco:ButtonPanel>
            </div>
        </div>
    </div>

</asp:Content>
