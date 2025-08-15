<%@ Page Language="VB" AutoEventWireup="false" CodeFile="UserCaseHistory.aspx.vb" Inherits="UserCaseHistory" MasterPageFile="~/masterpages/DetailSub.master" %>
<%@ Register TagPrefix="Arco" TagName="History" Src="~/UserControls/CaseHistory.ascx" %>


    <asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
        <Arco:History id="lblHistory" runat="server" />
    </asp:Content>
