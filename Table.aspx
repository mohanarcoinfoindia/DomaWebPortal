<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Table.aspx.vb" Inherits="Table" ValidateRequest="false"%>
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
    function AdjustWindow() {
 <%if Modal then %>
    var oWindow = GetRadWindow();
    setTimeout(function() {
            oWindow.autoSize(true);
    }, 0);
 <%end if %>
}

 function Close(){
    <%if not Modal then %>
	self.close();
	<%else %>
	GetRadWindow().Close();
	<%end if %>
}
function RefreshOnChange()
{
	SaveRows();
}
 var alarmWindow = null;
   function DisableWindow()
   {
      alarmWindow = document.body.appendChild(document.createElement("div"))
      alarmWindow.id = 'disableWindow';
      alarmWindow.style.height = document.documentElement.scrollHeight + 'px';
      alarmWindow.setAttribute('unselectable','on');    
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
                   case "new":
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
                else
                {
                DisableWindow();
                }
		    }

    function GetContentWindow()
    {        
        return window;
    }
   
</script>

      
       <asp:HiddenField runat="server" ID="hdnRow" />

    <Arco:PageController ID="PC" runat="server" PageLocation="Popup" ReloadFunction="SaveObject"  />
		<div>
            <asp:ValidationSummary runat="server" CssClass="ErrorLabel" ID="valSum" ShowSummary="true" EnableClientScript="true" ValidationGroup="CheckMandatoryFields" DisplayMode="List" />

		    <Telerik:RadToolBar ID="tlbMain" runat="server" OnClientButtonClicking="onTableToolbarClick" width="100%">
		        <Items>
		          <Telerik:RadToolBarButton ID="lnkNew" ValidationGroup="CheckMandatoryFields" PostBack="true" CommandName="new"></Telerik:RadToolBarButton>
		         <Telerik:RadToolBarButton runat="server"> 
                     <ItemTemplate> 
                          <telerik:RadNumericTextBox ID="txtNumRows" runat="server" Value="1" Width="30" MinValue="0" MaxValue="20">
                           <NumberFormat GroupSeparator="" DecimalDigits="0" /> 
                          </telerik:RadNumericTextBox> 
                     </ItemTemplate> 
                     </Telerik:RadToolBarButton> 
                    <Telerik:RadToolBarButton IsSeparator="true"></Telerik:RadToolBarButton>
		            <Telerik:RadToolBarButton ValidationGroup="CheckMandatoryFields" id="cmdSave" PostBack="true" CommandName="save"></Telerik:RadToolBarButton>
		            <Telerik:RadToolBarButton ValidationGroup="CheckMandatoryFields" id="cmdSaveAndClose" PostBack="true" CommandName="saveandclose"></Telerik:RadToolBarButton>
		         
		            <Telerik:RadToolBarButton IsSeparator="true"></Telerik:RadToolBarButton>
		        
		            <Telerik:RadToolBarButton id="lnkClose" PostBack="false" NavigateUrl="javascript:Close();"></Telerik:RadToolBarButton>
		        </Items>
		    </Telerik:RadToolBar>
		  
              <asp:PlaceHolder id="sharedcalenderPlaceHolder" runat="server"></asp:PlaceHolder>
			<asp:PlaceHolder ID="rows" runat="server"></asp:PlaceHolder>

               <Arco:PageFooter ID="ft" runat="server" />

			<asp:LinkButton runat="server" ID="btnSave" Text=""></asp:LinkButton>
            <asp:LinkButton runat="server" ID="btnDelete" Text=""></asp:LinkButton>
            <asp:LinkButton runat="server" ID="btnCopy" Text=""></asp:LinkButton>
		</div>      
    </form>
</body>
</html>
