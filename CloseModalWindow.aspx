<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CloseModalWindow.aspx.vb" Inherits="CloseModalWindow" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">    
    <title>Closing Window...</title>    
</head>
<body>
    <form id="form1" runat="server">
    
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement && window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }

        function ReloadParent() {
            <% if _GoToTreeId > 0 then %>;
            parent.PC().ReloadTree(<%= _GoToTreeId %>);
            <% end if %>
            const modalParent = GetRadWindow().BrowserWindow;
            if (modalParent.PC) {
                modalParent.PC().CascadeReloadContent();
            } else {
                modalParent.location.reload();
            }
        }

        function onStartup() {
            const oWnd = GetRadWindow();
            ReloadParent();
            oWnd.Close();
        }

        window.onload = onStartup;
    </script>
        
       
    </form>
	
	
</body>
</html>
