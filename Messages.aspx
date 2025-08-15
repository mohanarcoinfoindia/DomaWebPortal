<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Messages.aspx.vb" Inherits="Messages"  MasterPageFile="~/masterpages/Preview.master" %>
<%@ Register TagPrefix="Arco" TagName="Messages" Src="~/UserControls/Messages.ascx" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">

        <Arco:Messages id="lblMessages" runat="server" />
    </asp:Content>
