<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Consent.aspx.vb" Inherits="Auth_Consent"  EnableSessionState="True" MasterPageFile="~/masterpages/Base.master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Plh1">
        <Telerik:RadScriptManager runat="server" ID="sc1"/>
    <asp:HiddenField ID="txtReturnTo" runat="server" />


      <div class="PanelPage ConsentPage">

        <Arco:Logo runat="server" ID="lg" />

        <div class="Panel Centered">    
  
            <div class="Content">
            
                 <asp:Panel ID="pnlChange" runat="server" Visible="true" DefaultButton="lnkConsent">
                       
                            <div style="height:200px;overflow:auto">
                                <asp:Literal ID="consentContent" runat="server" />
                            </div>
                                <Arco:ButtonPanel runat="server">                                  
                                      <Arco:OkButton runat="server"  ID="lnkConsent" />
                                      <Arco:CancelButton runat="server" ID="lnkCancel" />
                                </Arco:ButtonPanel>
                         
                    </asp:Panel>
            </div>
         </div>
            <div class="Footer">
            <arco:PageFooter id="lblFooter" runat="server"/>
        </div>
  </div>     
</asp:Content>

