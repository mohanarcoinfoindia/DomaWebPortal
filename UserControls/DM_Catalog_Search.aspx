<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_Search.aspx.vb" Inherits="DM_Catalog_Search" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radS" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" style="height:100%;margin:0px;">
<head id="Head1" runat="server">
    <title>Lists Items</title>
</head>
<body  style="height:100%;margin:0px;overflow:hidden;" scroll="no"> 
    <form id="frmCatalogLI" runat="server" style="height:100%;margin:0px;">
    <radS:RadScriptManager ID="sc1" runat="server"></radS:RadScriptManager>
    
        <radS:RadSplitter ID="radSplitterContent" runat="server" Width="100%" Height="100%" BackColor="white">
            <radS:RadPane ID="radPaneLeftContent" runat="server"  Width="50%" Scrolling="Both" Collapsed="False"/>              
            <radS:RadSplitBar ID="radSplitbarMain" runat="server" CollapseMode="Backward"  />
            <radS:RadPane ID="radPaneRightContent" runat="server" Width="50%" Scrolling="Both" Collapsed="true"/>
        </radS:RadSplitter>

<script language="javascript">
 

function GetRadWindow() {
  var oWindow = null;
  if (window.radWindow) oWindow = window.radWindow;
  else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
  return oWindow;
}

function MainPage()
{
    <%if not Modal then %>
    return window.opener;
    <%else %>
    return GetRadWindow().BrowserWindow;
    <%end if %>
}
function GetOpener()
{
    if (MainPage().GetContentWindow)
    {
     return MainPage().GetContentWindow();
    }
    else
    {
        return MainPage();
    }
}
function RefreshOpener()
{
    GetOpener().RefreshOnChange();
}  

  function Close(){
    <%if not Modal then %>
	self.close();
	<%else %>
	GetRadWindow().Close();
	<%end if %>
}
 
 function SaveClose() {
   //RefreshOpener();
   
   Close();
 }
      
         
// function GetRadPane(paneName) {                   
//   var oSplitterContent = <%=radSplitterContent.UniqueID %>;
//   var pane = oSplitterContent.GetPaneById(paneName);
//   if (!pane) return;
//   var iframe = document.getElementById("RAD_SPLITTER_PANE_EXT_CONTENT_" + paneName);
//   var contentWindow = iframe.contentWindow;  
//   return contentWindow;
// }        

 function ShowLeftPane(url) {
   var oSplitterContent = $find('<%=radSplitterContent.ClientID %>');
   var oContentPaneLeft = oSplitterContent.GetPaneById('<%=radPaneLeftContent.ClientID %>');
   if (!oContentPaneLeft) return;  
    oContentPaneLeft.set_contentUrl(url);                   
   if (oContentPaneLeft.get_collapsed()) {
     oContentPaneLeft.expand();  
   }         
 }
        
 function ShowRightPane(url) {
   var oSplitterContent = $find('<%=radSplitterContent.ClientID %>');
   var oContentPaneRight = oSplitterContent.GetPaneById('<%=radPaneRightContent.ClientID %>');
   if (!oContentPaneRight) return;  
   oContentPaneRight.set_contentUrl(url);              
   if (oContentPaneRight.get_collapsed()) {
     oContentPaneRight.expand();  
   }         
 }

// function ResizePaneBySplit(id,nrOfPanes) {
//   var oSplitterContent = <%=radSplitterContent.UniqueID %>;
//   var pane = oSplitterContent.GetPaneById(id);
//   if (!pane) return;                
//   if (!pane.IsCollapsed()) {
//      pane.Expand();      
//      var delta = 0;   
//      var newSize = oSplitterContent.GetWidth()/nrOfPanes;   
//      delta = parseInt(newSize - pane.GetWidth());   
//      pane.Resize(delta);   
//      // these lines below are a temp workaround -    
//      // without them the height of the inner page becomes bigger   
//      pane.SetHeight(oSplitterContent.GetHeight() - 1);   
//      pane.SetHeight(oSplitterContent.GetHeight() + 1);    
//    }              
//  }

// function ResizePane(id,width) {
//   var oSplitterContent = <%=radSplitterContent.UniqueID %>;
//   var pane = oSplitterContent.GetPaneById(id);
//   if (!pane) return;                
//   if (!pane.IsCollapsed()) {
//     var delta = 0;   
//     var newSize = width;   
//     delta = parseInt(newSize - pane.GetWidth());   
//     pane.Resize(delta);   
//     // these lines below are a temp workaround -    
//     // without them the height of the inner page becomes bigger   
//     pane.SetHeight(oSplitterContent.GetHeight() - 1);   
//     pane.SetHeight(oSplitterContent.GetHeight() + 1);    
//   }              
// }
        
// function ReloadContent() {
//   //debugger;
//   var oSplitterContent = <%=radSplitterContent.UniqueID %>;
//   var oContentPaneLeft = oSplitterContent.GetPaneById('<%=radPaneLeftContent.UniqueId %>');
//   oContentPaneLeft.Reload();
// }
       

// function CloseOnReload() {
//   window.close();
// }

// function RefreshParentPage() {
//   //window.opener.document.reload();
//   try {
//     window.opener.Goto(window.opener.CurrentPage());
//   } catch(e) {
//   }
// }
// 
</script>
    </form>
</body>
</html>
