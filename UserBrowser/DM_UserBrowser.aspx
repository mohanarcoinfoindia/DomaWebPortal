<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_UserBrowser.aspx.vb" Inherits="DM_UserBrowser" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radS" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radW" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>User Managment</title>
</head>
<body style="overflow: hidden;" scroll="no">
    <form id="form1" runat="server" style="height: 100%; margin: 0px;">
        <radS:RadScriptManager ID="sc1" runat="server"></radS:RadScriptManager>
        <radS:RadSplitter ID="radSplitterMain" runat="server" Width="100%" Height="100%" Orientation="Vertical">
            <radS:RadPane ID="radPaneLeftContent" runat="server" Width="15%" Scrolling="Both" ContentUrl="DM_UserBrowser_LeftPane.aspx" />
            <radS:RadSplitBar ID="radSplitbarMain" runat="server" CollapseMode="ForWard" />
            <radS:RadPane ID="radPaneRightContent" runat="server" InitiallyCollapsed="False" ContentUrl="DM_UserBrowser_RightPane.aspx" Style="overflow: visible !important;" />
        </radS:RadSplitter>

        <script language="javascript" type="text/javascript">

            function ShowRightPane(url) {
                var splitter = $find('<%=radSplitterMain.ClientID%>');
            var pane = splitter.GetPaneById('<%=radPaneRightContent.UniqueID %>');
            if (pane != null) {
                pane.set_contentUrl(url);
            }
            if (pane.get_collapsed()) {
                pane.expand();
            }
            }
            function ShowLeftPane(url) {
                var splitter = $find('<%=radSplitterMain.ClientID%>');
            var pane = splitter.GetPaneById('<%=radPaneLeftContent.UniqueID %>');
            if (pane != null) {
                pane.set_contentUrl(url);
            }
            if (pane.get_collapsed()) {
                pane.expand();
            }
            }
        </script>
    </form>
</body>
</html>
