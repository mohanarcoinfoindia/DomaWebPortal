<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MFA.aspx.vb" Inherits="Auth_MFA" ValidateRequest="false" EnableSessionState="True" MasterPageFile="~/masterpages/Base.master" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls"  %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>


    <asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Plh1">
        <telerik:RadScriptManager runat="server" ID="sc1" />

        <asp:HiddenField ID="txtReturnTo" runat="server" />
        <asp:HiddenField ID="txtSelectedProvider" runat="server" />
    
        <div class="PanelPage MFAPage">

            <Arco:Logo runat="server" ID="lg" />

            <div class="Panel Centered">

                <div class="Content">
                    <asp:Panel ID="pnlMsg" runat="server" Visible="false">

                        <asp:Label ID="lblMsg" runat="server" CssClass="ErrorMessage"></asp:Label>                     
                    </asp:Panel>

                    <asp:Panel ID="pnlMfa" runat="server" Visible="true">
                        <h5>
                            <small>
                               
                            <%=ArcoFormatting.FormatUserName(Arco.Security.BusinessIdentity.CurrentIdentity.Name) %>
                            </small>
                        </h5>                                              
                    
                        <fieldset>                                 
                            <ol>
                               
                                <asp:PlaceHolder ID="plhSend" runat="server" Visible="false">
                                <li>
                                    <Arco:ArcoLinkButton ID="lnkSendCodeUsingSms" runat="server" ValidationGroup="SendCode" Visible="false" SkinID="ArcoButton" Width="400px" />                                 
                                </li>      
                                  <li>
                                    <Arco:ArcoLinkButton ID="lnkSendCodeUsingEmail" runat="server" ValidationGroup="SendCode" Visible="false" SkinID="ArcoButton" Width="400px" />                                 
                                </li>    
                                       <li>
                                    <Arco:ArcoLinkButton ID="lnkSendCodeUsingLog" runat="server" ValidationGroup="SendCode" Visible="false" SkinID="ArcoButton" Width="400px" />                                 
                                </li>   
                                </asp:PlaceHolder>

                                 <asp:PlaceHolder ID="plhRegisterOtp" runat="server" Visible="false">
                                <li>
                                     <div>
                                    <%=GetLabel("registerotpmfa") %>
                                         </div>
                                     <div>
                                    <asp:Image ID="imgOtpQRCode" runat="server" Width="200" Height="200" />
                                         </div>
                                </li>
                                </asp:PlaceHolder>

                                   <asp:PlaceHolder ID="pnlCaptcha" runat="server" Visible="false">
                                  <li>
                                      <telerik:RadCaptcha ValidationGroup="VerifyCode" runat="server" ID="capt" />
                                  </li>
                              </asp:PlaceHolder>

                                  <asp:PlaceHolder ID="plhVerifyOtp" runat="server" Visible="false">
                                      <%=GetLabel("verifyotpmfa") %>
                                 </asp:PlaceHolder>

                                <asp:PlaceHolder ID="plhVerify" runat="server">
                                <li>
                                    <asp:Label ID="lblCode" runat="server" CssClass="Label" AssociatedControlID="txtCode"></asp:Label>
                                    <asp:TextBox runat="server" ID="txtCode"/>
                                    <asp:RequiredFieldValidator  ID="RequiredFieldValidator2" ValidationGroup="VerifyCode" runat="server" ControlToValidate="txtCode" SetFocusOnError="true" Display="Dynamic" ErrorMessage=""></asp:RequiredFieldValidator>
                                </li>

                             
                              
                                <li class="mt-3 buttons">
                                    
                                    <Arco:OkButton ID="lnkVerifyCode" runat="server" ValidationGroup="VerifyCode"></Arco:OkButton>

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