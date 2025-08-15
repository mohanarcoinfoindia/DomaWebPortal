<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ForgotUserName.aspx.vb" Inherits="Auth_ForgotUserName" ValidateRequest="false" MasterPageFile="~/masterpages/Base.master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>


<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Plh1">
<Telerik:RadScriptManager runat="server" ID="sc1"/>
    <div class="PanelPage ForgotUsernamePage">

        <Arco:Logo runat="server" ID="lg" />

        <div class="Panel Centered">       
  
            <div class="Content">
                
<asp:Panel ID="pnlMsg" runat="server" Visible="false">    
        <asp:Label ID="lblMsg" runat="server"></asp:Label>
    </asp:Panel>

                <asp:Panel ID="pnlReg" runat="server" Visible="true" DefaultButton="lnkSave">
            <fieldset>
                <legend><asp:Label ID="lblRequestHeader" runat="server" ></asp:Label></legend>
                <ol>
                    <li>
                        <asp:Label ID="lblEmail" runat="server" CssClass="Label" AssociatedControlID="txtEmail"></asp:Label>
                        	<asp:TextBox ID="txtEmail" runat="server" AutoCompleteType="Email" TextMode="Email" TabIndex="1"/>
                      
                            
				        <asp:RequiredFieldValidator ID="req1" runat="server" ControlToValidate="txtEmail" SetFocusOnError="true" Display="Dynamic" ErrorMessage="" />

                          <Arco:EmailValidator ID="emailValidator" runat="server" Display="Dynamic" SetFocusOnError="true" ErrorMessage="" ControlToValidate="txtEmail" />

                    </li>
                    <li>
                        <asp:Label ID="lblCode" runat="server" CssClass="Label" />
                             <Telerik:RadCaptcha runat="server" ID="capt" />
                    </li>
                    <li class="buttons">                       
                        	<Arco:OkButton ID="lnkSave" runat="server" Text="Request password" TabIndex="3"></Arco:OkButton>
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