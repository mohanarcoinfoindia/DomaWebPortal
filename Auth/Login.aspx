<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Auth_Login" ValidateRequest="false" EnableSessionState="True" MasterPageFile="~/masterpages/Base.master" %>

<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" TagPrefix="ArcoControls" %>

<asp:Content runat="server" ID="HeaderContent" ContentPlaceHolderID="HeaderPlaceHolder1">
    <script type="text/javascript">
        function MakeTopMost() {
            if (window != window.top) {
                window.top.location.href = "Login.aspx";
            }
        }

    </script>
</asp:Content>


<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="Plh1">
    <telerik:RadScriptManager runat="server" ID="sc1" EnableEmbeddedjQuery="false">
        <Scripts>
            <asp:ScriptReference Path="~/JS/jquery-3.6.0.min.js" />
            <asp:ScriptReference Assembly="Telerik.Web.UI" Name="Telerik.Web.UI.Common.Core.js" />
        </Scripts>

    </telerik:RadScriptManager>

    <asp:HiddenField ID="txtRedirectUri" runat="server" />
    <asp:HiddenField ID="cookievalidator" runat="server" />

    <asp:Panel ID="pnlLogin" runat="server" DefaultButton="lnkLogin" CssClass="LoginPage PanelPage">
            <script type="text/javascript">

                function ClearMessage() {
                    var lbl = $get("<%=lblMsg.ClientID %>");
                    if (lbl) {
                        lbl.innerHTML = "";
                    }
                }
            </script>

            <Arco:Logo runat="server" ID="lg" />

            <div class="Panel Centered">
                <div class="Content">

                    <asp:Panel ID="pnlMsg" runat="server" Visible="false">
                        <div>
                            <asp:Label ID="lblMsg" runat="server" ForeColor="Red" />
                        </div>
                    </asp:Panel>

                    <fieldset>
                        <legend>
                            <asp:Label ID="lblLoginHeader" CssClass="Title" runat="server" /></legend>
                        <ol>
                            <li>
                                <asp:Label runat="server" ID="lblUserName" CssClass="Label" AssociatedControlID="txtUserName" />
                                <asp:TextBox ID="txtUsername" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ValidationGroup="ManualLogin" ID="req1" runat="server" ControlToValidate="txtUserName" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                            </li>
                            <li>
                                <asp:Label runat="server" ID="lblPassword" CssClass="Label" AssociatedControlID="txtPassword" />
                                <ArcoControls:PasswordTextBox ID="txtPassword" runat="server" />

                                <asp:RequiredFieldValidator ValidationGroup="ManualLogin" ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtPassword" SetFocusOnError="true" Display="Dynamic"></asp:RequiredFieldValidator>
                            </li>
                            <asp:PlaceHolder ID="pnlDomain" runat="server">
                                <li>
                                    <asp:Label runat="server" ID="lblDomain" CssClass="Label" AssociatedControlID="cmbDomain" />
                                    <asp:DropDownList ID="cmbDomain" runat="server" Width="100%" />
                                </li>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="pnlCaptcha" runat="server" Visible="false">
                                <li>
                                    <telerik:RadCaptcha ValidationGroup="ManualLogin" runat="server" ID="capt" />
                                </li>
                            </asp:PlaceHolder>
                            <li>
                                <asp:Panel ID="pnlExtra" runat="server">
                                    <div style="float: left;">
                                        <ArcoControls:CheckBox ID="chkPersist" runat="server" Checked="false" Text="remember me" />
                                    </div>
                                    <div class="forgotWrapper" style="float: right;">
                                        <asp:HyperLink ID="lnkRegister" runat="server" SkinID="SideLink" NavigateUrl="Register.aspx" Text="Register" />
                                        <span class="hideOnMobile">&nbsp;</span>
                                       
                                        <asp:HyperLink ID="lnkForgotUserName" runat="server" SkinID="SideLink" CssClass="ForgotUserName" NavigateUrl="ForgotUsername.aspx" Text="Forgot username" />
                                    
                                        <asp:HyperLink ID="lnkForgotPass" runat="server" SkinID="SideLink" NavigateUrl="ForgotPass.aspx" Text="Forgot password" />
                                    </div>
                                    <div style="clear: both;"></div>
                                </asp:Panel>
                            </li>
                            <li class="buttons">
                                <Arco:OkButton runat="server" OnClientClick="javascript:ClearMessage();" ID="lnkLogin" ValidationGroup="ManualLogin" />
                            </li>

                            <asp:PlaceHolder ID="pnlOAuth" runat="server" Visible="true">
                                <li class="LoginExternal">
                                    <asp:Panel ID="pnlOAuthButtons" runat="server" Width="400px">
                                    </asp:Panel>
                                </li>
                            </asp:PlaceHolder>
                            <li>
                                <ul class="langList">
                                    <li id="langListE" runat="server">
                                        <span class="flag">
                                            <asp:ImageButton ID="btnLangE" runat="server" ImageUrl="~/Images/LangIcons/E.gif" />
                                        </span>
                                        <span class="code">
                                            <asp:LinkButton ID="btnLangEc" runat="server" Text="EN"></asp:LinkButton>
                                        </span>
                                    </li>
                                    <li id="langListF" runat="server">
                                        <span class="flag">
                                            <asp:ImageButton ID="btnLangF" runat="server" ImageUrl="~/Images/LangIcons/F.gif" />
                                        </span>
                                        <span class="code">
                                            <asp:LinkButton ID="btnLangFc" runat="server" Text="FR"></asp:LinkButton>
                                        </span>
                                    </li>
                                    <li id="langListN" runat="server">
                                        <span class="flag">
                                            <asp:ImageButton ID="btnLangN" runat="server" ImageUrl="~/Images/LangIcons/N.gif" />
                                        </span>
                                        <span class="code">
                                            <asp:LinkButton ID="btnLangNc" runat="server" Text="NL"></asp:LinkButton>
                                        </span>
                                    </li>
                                </ul>
                            </li>
                        </ol>
                    </fieldset>
                </div>
            </div>
        <div class="Footer">
            <Arco:PageFooter ID="lblFooter" runat="server" />
        </div>
    </asp:Panel>
</asp:Content>


