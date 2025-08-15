<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Register.aspx.vb" Inherits="Auth_Register" ValidateRequest="false" MasterPageFile="~/masterpages/Base.master" %>

<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Plh1">
    <telerik:RadScriptManager runat="server" ID="sc1" />
    <asp:HiddenField ID="txtRedirectUri" runat="server" />
    <asp:HiddenField ID="txtSession" runat="server" />

    <div class="PanelPage RegisterPage">

        <Arco:Logo runat="server" ID="Logo1" />

        <div class="Panel Centered">

            <div class="Content">

                <asp:Panel ID="pnlMsg" runat="server" Visible="false">
                    <div>
                        <asp:Label ID="lblMsg" runat="server" />
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtPassword" EnableClientScript="false"
                            ControlToCompare="txtConfirmPassword" ErrorMessage="Passwords do not match." Display="Dynamic" />

                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlReg" runat="server" DefaultButton="lnkRegister">
                    <fieldset>
                        <legend>
                            <asp:Label ID="lblRegisterHeader" CssClass="Title" runat="server" /></legend>
                        <ol>
                            <li>
                                <asp:Label runat="server" ID="lblMail" CssClass="Label" AssociatedControlID="txtMail" Text="Mail" />

                                <asp:TextBox ID="txtMail" runat="server" TextMode="Email" AutoCompleteType="Email" MaxLength="100"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="req1" runat="server" CssClass="FormField-Message Inline bad" ControlToValidate="txtMail" ErrorMessage="*" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                <Arco:EmailValidator ID="emailValidator" runat="server" SetFocusOnError="true" CssClass="FormField-Message Inline bad" ErrorMessage="*" ControlToValidate="txtMail" />
                            </li>

                            <li runat="server" id="pnlFirstName">
                                <asp:Label runat="server" ID="lblFirstName" CssClass="Label" AssociatedControlID="txtFirstName" />
                                <asp:TextBox ID="txtFirstName" runat="server" AutoCompleteType="FirstName" MaxLength="50"></asp:TextBox>
                            </li>


                            <li runat="server" id="pnlLastName">
                                <asp:Label runat="server" ID="lblLastName" CssClass="Label" AssociatedControlID="txtLastName" />
                                <asp:TextBox ID="txtLastName" runat="server" AutoCompleteType="LastName" MaxLength="100"></asp:TextBox>
                            </li>


                            <li runat="server" id="pnlLanguage">
                                <asp:Label runat="server" ID="lblLang" CssClass="Label" AssociatedControlID="drpLang" />
                                <asp:DropDownList runat="server" ID="drpLang" Width="250px">
                                </asp:DropDownList>
                            </li>
                            <li>
                                <asp:Label runat="server" ID="lblPassword" CssClass="Label LabelInline" AssociatedControlID="txtPassword" Text="Password" />
                                <asp:Label runat="server" ID="lblPasswordTooltipImg" CssClass="TooltipImg">
                                    <svg width="22px" height="22px" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M12 7.00999V7M12 17L12 10M21 12C21 16.9706 16.9706 21 12 21C7.02944 21 3 16.9706 3 12C3 7.02944 7.02944 3 12 3C16.9706 3 21 7.02944 21 12Z" stroke="#000000" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                    </svg>
                                </asp:Label>
                                <ArcoControls:PasswordTextBox ID="txtPassword" runat="server" CheckPasswordStrength="false" />

                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" CssClass="FormField-Message Inline bad" ControlToValidate="txtPassword" SetFocusOnError="true"></asp:RequiredFieldValidator>
                            </li>

                            <li>
                                <asp:Label ID="lblConfirmPassword" runat="server" CssClass="Label" AssociatedControlID="txtConfirmPassword"></asp:Label>
                                <ArcoControls:PasswordTextBox ID="txtConfirmPassword" runat="server"></ArcoControls:PasswordTextBox>

                            </li>

                            <li>
                                <asp:Label ID="lblCode" runat="server" CssClass="Label" />
                                <telerik:RadCaptcha runat="server" ID="capt" />
                            </li>

                            <li class="buttons">
                                <Arco:OkButton runat="server" OnClientClick="javascript:ClearMessage()" ID="lnkRegister" />
                            </li>

                        </ol>
                    </fieldset>
                </asp:Panel>


                <asp:Panel ID="pnlActivate" runat="server" Visible="false">
                    <h5>
                        <small>
                            <asp:Label ID="lblUserNameForActivation" runat="server" />
                        </small>
                    </h5>
                    <h2>
                        <asp:Label ID="lblRegOk" runat="server" />
                    </h2>
                    <fieldset>
                        <ol>

                            <asp:PlaceHolder ID="plhVerify" runat="server">
                                <li>
                                    <asp:Label ID="lblActivationCode" runat="server" CssClass="Label" AssociatedControlID="txtActivationCode"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtActivationCode" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ValidationGroup="VerifyCode" runat="server" ControlToValidate="txtActivationCode" SetFocusOnError="true" Display="Dynamic" ErrorMessage=""></asp:RequiredFieldValidator>
                                </li>
                                <li>
                                    <telerik:RadCaptcha runat="server" ID="captAuthCode" visible="false"/>
                                </li>
                                <li class="mt-3 buttons">

                                    <Arco:OkButton ID="lnkActivate" runat="server" ValidationGroup="VerifyCode"></Arco:OkButton>

                                </li>
                            </asp:PlaceHolder>
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
