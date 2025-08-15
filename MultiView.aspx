<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MultiView.aspx.vb" Inherits="MultiView" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<%@ Register TagPrefix="arcoctrls" Namespace="Arco.Doma.WebControls" Assembly="Arco.Doma.WebControls" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>      
</head>
<body>
    <form id="form1" runat="server">

             <script type="text/javascript">
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
  
 function Close(){
     MainPage().PC().Reload();
    <%if not Modal then %>
	self.close();
	<%else %>
	GetRadWindow().Close();
	<%end if %>
}
function RefreshOnChange()
{
	SaveObject();
}
		function RefreshOpener() {
		    if (MainPage())
		        if (MainPage().GetContentWindow().RefreshOnChange)
		            MainPage().GetContentWindow().RefreshOnChange();
		}
        
	  function onTableToolbarClick(sender, args) {
		        var DoPostback = true;
		        switch (args.get_item().get_commandName()) 
		        {		                  
                   case "saveandclose":     
		           case "save":
                    if (typeof Page_ClientValidate === "function") {
		                Page_ClientValidate('CheckMandatoryFields');
                        DoPostback = Page_IsValid;	
                    }		            
                    break;                    
                    default:
                    break;
		        }
		        if (!DoPostback)
	            {
	                args.set_cancel(true);
	            }
		    }

    function GetContentWindow()
    {        
        return window;
    }
</script>


    <Arco:PageController ID="thisPageController" runat="server" PageLocation="Popup" ReloadFunction="SaveObject"  />
		<div>
            <asp:ValidationSummary runat="server" CssClass="ErrorLabel" ID="valSum" ShowSummary="true" EnableClientScript="true" ValidationGroup="CheckMandatoryFields" DisplayMode="List" />

		    <Telerik:RadToolBar ID="tlbMain" runat="server" OnClientButtonClicking="onTableToolbarClick" width="100%">
		        <Items>		         
		            <Telerik:RadToolBarButton ValidationGroup="CheckMandatoryFields" ImageUrl="~/Images/Savekeepopen.png" id="cmdSave" PostBack="true" CommandName="save"></Telerik:RadToolBarButton>
		            <Telerik:RadToolBarButton ValidationGroup="CheckMandatoryFields" ImageUrl="~/Images/Release.png" id="cmdSaveAndClose" PostBack="true" CommandName="saveandclose"></Telerik:RadToolBarButton>
		         
		            <Telerik:RadToolBarButton IsSeparator="true"></Telerik:RadToolBarButton>
		        
		            <Telerik:RadToolBarButton id="lnkClose" ImageUrl="~/Images/Cancel.png" CommandName="close"></Telerik:RadToolBarButton>
		        </Items>
		    </Telerik:RadToolBar>
		   		   
			<asp:PlaceHolder ID="rows" runat="server"></asp:PlaceHolder>

			<asp:LinkButton runat="server" ID="btnSave" Text=""></asp:LinkButton>           
		</div>
    </form>
</body>
</html>
