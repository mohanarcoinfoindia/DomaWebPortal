<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ResetPass.aspx.vb" Inherits="Auth_ResetPass" ValidateRequest="false" MasterPageFile="~/masterpages/Base.master" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls"  %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Plh1">
<Telerik:RadScriptManager runat="server" ID="sc1"/>
     <asp:HiddenField ID="txtRedirectUri" runat="server" />

     <div class="PanelPage ResetpassPage">

        <Arco:Logo runat="server" ID="lg" />

        <div class="Panel Centered">    
              
            <div class="Content">
                <asp:Panel ID="pnlMsg" runat="server" Visible="false">
        
                    <asp:Label ID="lblMsg" runat="server" CssClass="ErrorMessage"></asp:Label>
                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtPassword" EnableClientScript="false" ControlToCompare="txtConfirmPassword" ErrorMessage="Passwords do not match." Display="Dynamic" />
       
                </asp:Panel>
           
                 <asp:Panel ID="pnlChange" runat="server" DefaultButton="lnkChange">
                       <fieldset>
                        <legend><asp:Label ID="lblHeader" runat="server"></asp:Label></legend>
                        <ol>
                            
                            <li>
                                <asp:Label ID="lblPassword" runat="server" CssClass="Label" AssociatedControlID="txtPassword"></asp:Label> 
                                 <asp:Label runat="server" ID="lblPasswordTooltipImg" CssClass="TooltipImg">
                                    <svg width="22px" height="22px" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M12 7.00999V7M12 17L12 10M21 12C21 16.9706 16.9706 21 12 21C7.02944 21 3 16.9706 3 12C3 7.02944 7.02944 3 12 3C16.9706 3 21 7.02944 21 12Z" stroke="#000000" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                    </svg>
                                </asp:Label>
                                  <ArcoControls:PasswordTextBox ID="txtPassword" runat="server" CheckPasswordStrength="false">
                        </ArcoControls:PasswordTextBox> 
				        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword"  SetFocusOnError="true" Display="Dynamic" ErrorMessage=""></asp:RequiredFieldValidator>
                                  
                            </li>
                            <li>
                                <asp:Label ID="lblConfirmPassword" runat="server" CssClass="Label" AssociatedControlID="txtConfirmPassword"></asp:Label>
                                <ArcoControls:PasswordTextBox ID="txtConfirmPassword" runat="server"></ArcoControls:PasswordTextBox> 
				        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtConfirmPassword" SetFocusOnError="true" Display="Dynamic" ErrorMessage=""></asp:RequiredFieldValidator>
                                 
                            </li>
                            <li class="buttons">
                                <Arco:OkButton  ID="lnkChange" runat="server"></Arco:OkButton>
                            </li>
                        </ol>
                    </fieldset>
                    </asp:Panel>
            </div>
         </div>
             <div class="Footer">
            <arco:PageFooter id="lblFooter" runat="server"/>
        </div>
  </div>     
      
</asp:Content>
