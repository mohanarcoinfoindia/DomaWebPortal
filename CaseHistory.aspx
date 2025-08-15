<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CaseHistory.aspx.vb" Inherits="CaseHistory" MasterPageFile="~/masterpages/Preview.master" %>
<%@ Register TagPrefix="Arco" TagName="History" Src="~/UserControls/CaseHistory.ascx" %>


    <asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
        <Arco:History id="lblHistory" runat="server" />
    </asp:Content>
