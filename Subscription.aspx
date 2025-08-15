<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Subscription.aspx.vb" Inherits="Subscription" MasterPageFile="~/masterpages/Toolwindow.master" %>

<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls" TagPrefix="Arco" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>


<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <asp:HiddenField ID="hdnID" runat="server" />
    <asp:HiddenField ID="hdnObjectId" runat="server" />
    <asp:HiddenField ID="hdnObjectType" runat="server" />
    <asp:HiddenField ID="hdnQryValue" runat="server" />

    <div class="container-fluid detail-form-container">
        <div class="row detail-form-row" id="rowMessage" visible="false" runat="server">
            <asp:TableCell ColumnSpan="2">
                <asp:Label ID="errorLabel" runat="server" CssClass="ErrorLabel" />
            </asp:TableCell>
        </div>
        <div class="row detail-form-row" id="rowTo" runat="server">
            <div class="col-md-4 LabelCell">
                <asp:Label CssClass="Label" Text="For :" ID="lblFor" runat="server" AssociatedControlID="txtSubject"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <Arco:SelectSubject ID="txtSubject" runat="server" RefreshOnChange="false" ShowRoleEveryone="true" UsersOnly="false" HideDeleteIcon="true" />
            </div>
        </div>
        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label CssClass="Label" Text="On :" ID="lblO" runat="server"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:Image ID="imgType" runat="server" />
                &nbsp;
                <asp:Label ID="lblOn" runat="server"></asp:Label>
            </div>
        </div>
        <div class="row detail-form-row" id="rowOptions" runat="server">
            <div class="col-md-4 LabelCell">
                <asp:Label CssClass="Label" Text="Options :" ID="Label1" runat="server" AssociatedControlID="drpDate"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:DropDownList ID="drpDate" runat="server">
                    <asp:ListItem Value="0" Selected="True">New or modified since last run</asp:ListItem>
                    <asp:ListItem Value="1">New since last run</asp:ListItem>
                    <asp:ListItem Value="9">All</asp:ListItem>
                </asp:DropDownList>
                <ArcoControls:CheckBox ID="chkIncSubFolders" runat="server" Text="Include subfolders" CssClass="ml-3" />
            </div>
        </div>
        <div class="row detail-form-row" id="rowWhen" runat="server">
            <div class="col-md-4 LabelCell">
                <asp:Label CssClass="Label" Text="When :" ID="lblWhen" runat="server" AssociatedControlID="drpStatus"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <div>
                    <asp:DropDownList ID="drpStatus" runat="server">
                        <asp:ListItem Value="0" Selected="True">Enabled</asp:ListItem>
                        <asp:ListItem Value="3">Disabled</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div>
                    <asp:DropDownList ID="drpJob" runat="server">
                    </asp:DropDownList>
                </div>
            </div>
        </div>
        <div class="row detail-form-row" id="rowTemplate" runat="server">
            <div class="col-md-4 LabelCell">
                <asp:Label CssClass="Label" runat="server" ID="lblTemplate" Text="Template" AssociatedControlID="drpTemplate"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:DropDownList ID="drpTemplate" runat="server">
                </asp:DropDownList>
            </div>
        </div>
        <div class="row detail-form-row">
            <div class="col-md-8 offset-md-4 FieldCell">
                <Arco:ButtonPanel ID="pnlButtons" runat="server">
                    <Arco:OkButton ID="cmdSave" runat="server" Text="Save"></Arco:OkButton>
                    <Arco:CancelButton ID="cmdCancel" runat="server" OnClientClick="javascript:Close();return false;"></Arco:CancelButton>
                    <Arco:SecondaryButton ID="lnkDelete" runat="server" OnClientClick="javascript:return confirm(UIMessages[4]);"></Arco:SecondaryButton>
                </Arco:ButtonPanel>
            </div>
        </div>
    </div>
</asp:Content>
