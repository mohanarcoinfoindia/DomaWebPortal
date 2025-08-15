<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_OBJECTHISTORY.aspx.vb" Inherits="DM_OBJECTHISTORY" MasterPageFile="~/masterpages/Preview.master" %>
<%@ Register TagPrefix="Arco" TagName="History" Src="~/UserControls/ObjectHistory.ascx" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
        <Arco:History id="lblHistory" runat="server" />
    </asp:Content>
