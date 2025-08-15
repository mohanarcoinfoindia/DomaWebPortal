<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_UserBrowser_Roles.aspx.vb" Inherits="DM_UserBrowser_Roles" ValidateRequest="false" %>

<%@ Register TagPrefix="Arco" TagName="Results" Src="DM_UserBrowser_RolesResult.ascx" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>User managment - Roles</title>
</head>
<body>
    <form id="Form1" method="post" runat="server" defaultbutton="lnkSearch">
        <asp:LinkButton ID="lnkSearch" runat="server"></asp:LinkButton>
        <Arco:PageController ID="thisPageController" runat="server" PageLocation="Popup" />

        <input type="hidden" id="ROLE_ID_RES" runat="server" />
        <input type="hidden" id="ROLE_NAME_RES" runat="server" />
        <input type="hidden" id="txtRoleType" runat="server" />

        <Arco:Results ID="lblRoles" runat="server" />
    </form>
</body>
</html>
