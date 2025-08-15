<%@ Page Language="VB" AutoEventWireup="false" CodeFile="FileCompare.aspx.vb" Inherits="Tools_FileCompare" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radS" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>File Scroller</title>
    <script type="text/javascript">
    var s1= "";
    function SetRightSubTitle(s)
    {
        s2 = s;
        SetTitle();
    }
    var s2 = "";
    function SetLeftSubTitle(s)
    {
        s1 = s;
        SetTitle();
    }
    function SetTitle()
    {
        document.title = s1 + ' - ' + s2;
    }
    
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="sc1" runat="server"></asp:ScriptManager>

           <radS:RadSplitter id="radSplitterMain" runat="server" Width="100%" Height="100%" Orientation="Horizontal" >
       <radS:RadPane ID="radPaneNavigation" runat="server" Height="30px" Scrolling="none">
            <table width="100%" cellpadding="0" cellspacing="0" border="0" style="height:30px;" class="MenuTable">
            <tr>
            <td width="50%" align="right">
                 <asp:HyperLink ID="hplPreviousLeftFile" runat="server" NavigateUrl="#" Enabled="false" ImageUrl="~/Images/previouspage.png"></asp:HyperLink>&nbsp;
                 <asp:HyperLink ID="hplNextLeftFile" runat="server" Enabled="False" NavigateUrl="#"  ImageUrl="~/Images/nextpage.png"></asp:HyperLink>&nbsp;
            </td>
                        <td width="50%" align="right">
                 <asp:HyperLink ID="hplPreviousRightFile" runat="server" NavigateUrl="#" Enabled="false" ImageUrl="~/Images/previouspage.png"></asp:HyperLink>&nbsp;
                 <asp:HyperLink ID="hplNextRightFile" runat="server" Enabled="False" NavigateUrl="#"  ImageUrl="~/Images/nextpage.png"></asp:HyperLink>&nbsp;
            </td>
            </tr>
            </table>
        </radS:RadPane>
         <radS:RadPane ID="radPaneContent" runat="server" Scrolling="None" >
             <radS:RadSplitter ID="radSplitterContent" runat="server" Width="100%" Height="100%" BackColor="White">
             <radS:RadPane ID="radPaneLeftFile" runat="server" Scrolling="Y" Width="50%" InitiallyCollapsed="false"/>
            <radS:RadSplitBar ID="radSplitbarTree" runat="server" CollapseMode="Both"  />
            <radS:RadPane ID="radPaneRightFile" runat="server" Scrolling="Y" Width="50%" InitiallyCollapsed="false"/>
            </radS:RadSplitter>
         </radS:RadPane>
    </radS:RadSplitter>
    </form>
</body>
</html>
