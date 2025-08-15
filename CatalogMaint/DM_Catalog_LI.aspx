<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_LI.aspx.vb" Inherits="DM_Catalog_LI" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">   
    <title>Lists Items</title>
</head>
<body style="height: 100%; margin: 0px; overflow: hidden;" scroll="no">
    <form id="frmCatalogLI" runat="server" style="height: 100%; margin: 0px;">
    <asp:ScriptManager runat="server" ID="sc1">
    </asp:ScriptManager>
    <radS:RadSplitter ID="radSplitterMain" runat="server" Width="100%" Height="100%"
        Orientation="Horizontal">
        <radS:RadPane ID="radPaneContent" runat="server" Scrolling="None">
            <radS:RadSplitter ID="radSplitterContent" runat="server" Width="100%" Height="100%"  BackColor="White">
                <radS:RadPane ID="radPaneLeftContent" runat="server" Width="23%" Scrolling="Both"  Collapsed="False" />
                <radS:RadSplitBar ID="radSplitbarMain" runat="server" CollapseMode="Both" />
                <radS:RadPane ID="radPaneRightContent" runat="server" Width="77%" Scrolling="Both" Collapsed="False" />
            </radS:RadSplitter>
        </radS:RadPane>
    </radS:RadSplitter>
    <script language="javascript">

        function ShowLeftPane() {
            var oSplitterContent = $find('<%=radSplitterContent.ClientID %>');
            var oContentPaneLeft = oSplitterContent.GetPaneById('<%=radPaneLeftContent.UniqueId %>');
            if (!oContentPaneLeft) return;
            if (oContentPaneLeft.get_collapsed()) {
                oContentPaneLeft.expand();
            }
        }

        function ShowRightPane() {
            var oSplitterContent = $find('<%=radSplitterContent.ClientID %>');
            var oContentPaneRight = oSplitterContent.GetPaneById('<%=radPaneRightContent.UniqueId %>');
            if (!oContentPaneRight) return;
            if (oContentPaneRight.get_collapsed()) {
                oContentPaneRight.expand();
            }
        }
      
        function HideLeftPane() {
            var oSplitterContent = $find('<%=radSplitterContent.UniqueID %>');
            var oContentPaneLeft = oSplitterContent.GetPaneById('<%=radPaneLeftContent.UniqueId %>');
            oContentPaneLeft.collapse();
        }

        function GotoManagedLists() { //Close button in listitems page
            window.open('DM_Catalog.aspx', '_self');
        }
                
    </script>
    </form>
</body>
</html>
