<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_UserBrowser_Groups.aspx.vb"
    Inherits="DM_UserBrowser_Groups" ValidateRequest="false" %>
    <%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<%@ Register TagPrefix="Arco" TagName="Results" Src="DM_UserBrowser_GroupsResult.ascx" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>User management - Groups</title>       
       <style type="text/css">
  
  .DeleteButton {
            height:12px;
        cursor:pointer;                    
        }    
    </style>
</head>
<body>
    <form id="Form1" method="post" runat="server" defaultbutton="lnkSearch">
       <asp:LinkButton ID="lnkSearch" runat="server"></asp:LinkButton>			
    <Arco:PageController ID="thisPageController" runat="server" PageLocation="Popup" />

    <input type="hidden" id="GROUP_NAME_RES" runat="server" />
    <input type="hidden" id="GROUP_DISPLAY_NAME_RES" runat="server" />

     
        <Arco:Results ID="lblGroups" runat="server" />
       
    </form>   
</body>
</html>
