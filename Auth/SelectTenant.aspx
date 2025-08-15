<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SelectTenant.aspx.vb" Inherits="Auth_SelectTenant" EnableSessionState="True" MasterPageFile="~/masterpages/Base.master" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>


    <asp:Content ID="Content1" runat="server" ContentPlaceHolderID="Plh1">

        <Telerik:RadScriptManager runat="server" ID="sc1"/>

        <asp:HiddenField ID="txtReturnTo" runat="server" />


        <div class="PanelPage SelectTenantPage">

            <Arco:Logo runat="server" ID="Logo1" />

            <div class="Panel Centered">

                <div class="Content">
                    <asp:Panel ID="pnlMsg" runat="server" Visible="false">
                        <div>
                            <asp:Label ID="lblMsg" runat="server" CssClass="ErrorMessage" /></div>
                    </asp:Panel>
                    <asp:Panel ID="pnlSelect" runat="server" Visible="true" DefaultButton="lnkSelect">
                        <fieldset>
                            <legend>
                                <asp:Label ID="lblRequestHeader" runat="server"></asp:Label></legend>
                            <ol>
                                <li>
                                    <asp:Label runat="server" ID="lblTenant" CssClass="Label" AssociatedControlID="cmbTenant" />
                                    <telerik:RadComboBox ID="cmbTenant" runat="server" />
                                </li>

                                <li>

                                    <Arco:ButtonPanel runat="server">
                                        <Arco:OkButton runat="server" ID="lnkSelect" />
                                    </Arco:ButtonPanel>

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


