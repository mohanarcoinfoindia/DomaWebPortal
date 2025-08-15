<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ConfigPanel.aspx.vb" Inherits="CMS_ConfigPanel" MasterPageFile="~/masterpages/Toolwindow.master" ValidateRequest="false" %>

<%@ Import Namespace="Arco.Doma.CMS.WebPanels.Configuration" %>
<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" TagPrefix="Arco" %>

<asp:Content ID="headContent" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <style type="text/css">
        html {
            overflow-y: auto;
            overflow-x: hidden;
        }

        .alert-danger {
            width: 100%;
            display: inline-block;
        }

        .btnInfo {
            background-color: unset;
            border: unset;
            align-self: center;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <script src="../JS/CMSConfig.js" type="text/javascript"></script>
    <asp:Panel runat="server" ID="configForm" CssClass="container-fluid detail-form-container CMSConfigTable">
        <asp:Label runat="server" ID="lblError" CssClass="alert alert-danger" Visible="false" />
        <asp:Panel CssClass="row detail-form-row" runat="server">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblHeader" AssociatedControlID="txtHeader"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <Arco:MultiLanguageTextBox CssClass="Label" runat="server" ID="txtHeader"></Arco:MultiLanguageTextBox>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel CssClass="row detail-form-row" runat="server">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblHeaderImage" AssociatedControlID="txtHeaderImage"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <asp:TextBox CssClass="Label" runat="server" ID="txtHeaderImage"></asp:TextBox>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel CssClass="row detail-form-row" runat="server">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblShowHeader" AssociatedControlID="cbShowHeader"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <Arco:CheckBox runat="server" ID="cbShowHeader"></Arco:CheckBox>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel CssClass="row detail-form-row" runat="server">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblRoles" AssociatedControlID="txtRoles"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <asp:TextBox CssClass="Label" runat="server" ID="txtRoles"></asp:TextBox>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel CssClass="row detail-form-row" runat="server">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblCondition" AssociatedControlID="txtCondition"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <asp:TextBox CssClass="Label" runat="server" ID="txtCondition" ToolTip=""></asp:TextBox>
                <asp:Button runat="server" ID="btnInfo" CssClass="tooltip-icon btnInfo" />
            </asp:Panel>
        </asp:Panel>
        <asp:Panel CssClass="row detail-form-row" runat="server">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblHeight" AssociatedControlID="txtHeight"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <asp:TextBox CssClass="Label" runat="server" ID="txtHeight" TextMode="Number" min="0"></asp:TextBox>&nbsp;px
            </asp:Panel>
        </asp:Panel>
          <asp:Panel CssClass="row detail-form-row" runat="server" ID="rowBorder">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblBorder" AssociatedControlID="drpBorderStyle"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <asp:DropdownList runat="server" ID="drpBorderStyle">
                    <asp:ListItem Value="" Text="None" />
                    <asp:ListItem Value="Normal" Text="Normal" />
                    <asp:ListItem Value="Elevation" Text="Elevation" />
                </asp:DropdownList>
            </asp:Panel>
        </asp:Panel>
        <asp:UpdatePanel runat="server" ID="updatePanel1">
            <ContentTemplate></ContentTemplate>
        </asp:UpdatePanel>
        <asp:Panel CssClass="row detail-form-row" runat="server">
            <asp:Panel CssClass="col-md-8 offset-md-4 FieldCell" runat="server">
                <Arco:ButtonPanel ID="ButtonPanel1" CssClass="mt-3" runat="server">
                    <Arco:OkButton runat="server" ID="btnSave"></Arco:OkButton>
                    <Arco:CancelButton OnClientClick="javascript:Close(true);return false;" runat="server" ID="btnCancel"></Arco:CancelButton>
                </Arco:ButtonPanel>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>
    <script type="text/javascript">
        var resizeOnLoad = <%= If(resizeModalOnLoad, "true", "false") %>;
    </script>
    <asp:UpdateProgress ID="updProgress" AssociatedUpdatePanelID="updatePanel1" runat="server">
        <ProgressTemplate>
            <div style="position: fixed; width: 100%; height: 100%; top: 0; left: 0;" class="d-flex justify-content-center align-items-center">
                <div class="spinner-border"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>
</asp:Content>
