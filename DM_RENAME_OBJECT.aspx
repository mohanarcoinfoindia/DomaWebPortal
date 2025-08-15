<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_RENAME_OBJECT.aspx.vb" ValidateRequest="false" Inherits="DM_RENAME_OBJECT" MasterPageFile="~/masterpages/Toolwindow.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">

    <div class="container-fluid detail-form-container">
        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblOldname" runat="server"></asp:Label>
                &nbsp;<asp:HyperLink runat="server" ID="lnkCopy" ToolTip="Copy" NavigateUrl="javascript:CopyName();"></asp:HyperLink>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox ID="txtNewName" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator CssClass="FormField-Message bad" ID="valNewName" runat="server" ControlToValidate="txtNewName"
                    Display="Dynamic" ErrorMessage="You have to enter a new name" SetFocusOnError="True"
                    ValidationGroup="RenameObject"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator CssClass="FormField-Message bad" ID="regMaxLen" Display="Dynamic" ValidationGroup="RenameObject" runat="server" ControlToValidate="txtNewName" ValidationExpression="^[\s\S]{0,500}$" ErrorMessage="Maximum 500 characters are allowed in comments box." SetFocusOnError="true"> </asp:RegularExpressionValidator>
            </div>
        </div>
        <div>
            <div class="row detail-form-row">
                <div class="col-md-8 offset-md-4 FieldCell">
                    <Arco:ButtonPanel ID="pnlButtons" runat="server">
                        <Arco:OkButton ID="cmdChangeObjectName" runat="server" ValidationGroup="RenameObject" Text="Save"></Arco:OkButton>
                        <Arco:CancelButton ID="lblCancel" runat="server" OnClientClick="javascript:Close();return false;"></Arco:CancelButton>
                    </Arco:ButtonPanel>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
