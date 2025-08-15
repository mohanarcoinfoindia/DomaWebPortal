<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ChangePass.aspx.vb" Inherits="Auth_ChangePass" ValidateRequest="false" EnableSessionState="True" MasterPageFile="~/masterpages/Base.master" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls"  %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>


    <asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Plh1">
        <telerik:RadScriptManager runat="server" ID="sc1" />

        <asp:HiddenField ID="txtReturnTo" runat="server" />

        <div class="PanelPage ChangePassPage">

            <Arco:Logo runat="server" ID="lg" />

            <div class="Panel Centered">

                <div class="Content">
                    <asp:Panel ID="pnlMsg" runat="server" Visible="false">

                        <asp:Label ID="lblMsg" runat="server" CssClass="ErrorMessage"></asp:Label>
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtPassword" EnableClientScript="false" ControlToCompare="txtConfirmPassword" ErrorMessage="Passwords do not match." Display="Dynamic" />

                    </asp:Panel>

                    <asp:Panel ID="pnlChange" runat="server" Visible="true" DefaultButton="lnkChange">
                        <fieldset>
                            <legend>
                                <asp:Label ID="lblHeader" runat="server"></asp:Label></legend>
                            <ol>
                                <li>
                                    <asp:Label ID="lblOldPassword" runat="server" CssClass="Label" AssociatedControlID="txtOldPassword"></asp:Label>
                                    <ArcoControls:PasswordTextBox ID="txtOldPassword" runat="server" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtOldPassword" SetFocusOnError="true" Display="Dynamic" ErrorMessage=""></asp:RequiredFieldValidator>
                                </li>
                                <li>
                                    <asp:Label ID="lblPassword" runat="server" CssClass="Label" AssociatedControlID="txtPassword"></asp:Label>
                                    <ArcoControls:PasswordTextBox runat="server" ID="txtPassword" CheckPasswordStrength="false"/>
                                    <asp:RequiredFieldValidator  ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword" SetFocusOnError="true" Display="Dynamic" ErrorMessage=""></asp:RequiredFieldValidator>
                                </li>
                                <li>
                                    <asp:Label ID="lblConfirmPassword" runat="server" CssClass="Label" AssociatedControlID="txtConfirmPassword"></asp:Label>
                                    <ArcoControls:PasswordTextBox ID="txtConfirmPassword" runat="server" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtConfirmPassword" SetFocusOnError="true" Display="Dynamic" ErrorMessage=""></asp:RequiredFieldValidator>
                                </li>
                                <li class="mt-3 buttons">
                                    <Arco:OkButton ID="lnkChange" runat="server"></Arco:OkButton>
                                </li>
                            </ol>
                        </fieldset>
                    </asp:Panel>
                </div>
            </div>
            <div class="Footer">
                <Arco:PageFooter ID="lblFooter" runat="server" />
            </div>
        </div>
    </asp:Content>
        
