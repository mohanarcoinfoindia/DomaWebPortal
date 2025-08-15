<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CaseNotes.aspx.vb" Inherits="CaseNotes" MasterPageFile="~/masterpages/Preview.master" %>
<%@ Register TagPrefix="Arco" TagName="Notes" Src="~/UserControls/Notes.ascx" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">

        <Arco:Notes id="lblNotes" runat="server" />
    </asp:Content>
