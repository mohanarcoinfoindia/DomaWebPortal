<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_LID2.aspx.vb" Inherits="DM_Catalog_LID2" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
 <head id="Head1" runat="server">
     <title>Lists Details</title></head>
 <body  style="height:100%;margin:0px;overflow:hidden;" scroll="no"> 
  <form id="frmCatalogLID" runat="server" style="height:100%;margin:0px;">
   <asp:ScriptManager runat="server" ID="sc1"></asp:ScriptManager>
   <radS:RadSplitter id="radSplitterContent" runat="server" Width="100%" Height="100%" Orientation="Horizontal" >
    <radS:RadPane ID="radPaneContentTop" runat="server" Width=100% Scrolling="Both" Collapsed="False" BorderStyle=None />   
   </radS:RadSplitter>  
   <script type="text/javascript">
       function GotoManagedLists() { //Close button in listitems page
           
           window.open('DM_Catalog.aspx', '_self');
       }
   </script>   
  </form>
 </body>
</html>