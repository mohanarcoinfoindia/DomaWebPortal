<%@ Page Language="VB" AutoEventWireup="false" ValidateRequest="false" CodeFile="DM_UserBrowser_RightPane.aspx.vb"
    Inherits="DM_UserBrowser_RightPane" %>

<%@ Register TagPrefix="Arco" TagName="Results" Src="DM_UserBrowser_Results.ascx" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>User managment - Search</title>
</head>
<body>
    <form id="Form1" method="post" runat="server" defaultbutton="lnkSearch">
        <Arco:PageController ID="thisPageController" runat="server" PageLocation="Popup" />

        <input type="hidden" id="action" runat="server" />
        <input type="hidden" id="USER_LOGIN_RES" runat="server" />
        <input type="hidden" id="USER_DISPLAY_NAME_RES" runat="server" />

        <asp:HiddenField ID="parentURL" Value="user" runat="server" />
        <asp:LinkButton ID="lnkSearch" CommandName="Search" runat="server" OnClick="DoSearch" />

        <Arco:Results ID="lblUsers" runat="server" />
    </form>
    <asp:Literal ID="plValidationScript" runat="server"></asp:Literal>
</body>
</html>
