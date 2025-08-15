<%@ Register TagPrefix="arcoctrls" Namespace="Arco.Doma.WebControls" Assembly="Arco.Doma.WebControls" %>
<%@ Page Language="vb" AutoEventWireup="false" Inherits="DM_TABLE_RECORD" CodeFile="DM_TABLE_RECORD.aspx.vb" ValidateRequest="false" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>        
</head>
<body>
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
           // prevent modal from resizing when datepicker opens (empty handler)
           <%-- <%if Modal then %>
                var oWindow = GetRadWindow();
                
                setTimeout(function() {
                        oWindow.autoSize(true);
                }, 0); 
            <%end if %>--%>
        }
  
            function Close(){
            <%if not Modal then %>
	        self.close();
	        <%else %>
	        GetRadWindow().Close();
	        <%end if %>
        }
        function GetContentWindow()
        {        
            return window;
        }
   
        function DisableWindow()
        {
            var alarmWindow = document.createElement("div");
            alarmWindow.id = 'disableWindow';
            alarmWindow.setAttribute('unselectable','on');
            alarmWindow.style.height = document.documentElement.scrollHeight + 'px';     
            document.body.appendChild(alarmWindow)
        }
        function EnableWindow()
        {
            var olddiv = $get('disableWindow');
            if (olddiv && olddiv.parentNode && olddiv.parentNode.removeChild)       
            {                                                 
                olddiv.parentNode.removeChild(olddiv);                                                                 
            }       
            olddiv = null;  
        }
    </script>

    <form id="ident" name="ident" runat="server">
    
	    <script language="javascript">
	        function ConfirmNoSave() {	               
	            if (<%=CurrentRecord %> == 0 || IsDirty()) {
	                return confirm(UIMessages[15]);
	            }
	            else {
	                return true;
	            }
	        }
        function CloseRecord() {   
	       if (ConfirmNoSave()) {
	           Close();
	       }               
		}
	   function NewRecordInTable() {
           
	       if (ConfirmNoSave()) {
	           var address = 'DM_TABLE_RECORD.aspx?DM_OBJECT_ID=<%=llID %>&CASE_ID=<%=llcaseID %>&TECH_ID=<%=llTechID %>&SCREEN_ID=<%=_screenID %>&Dm_Table_ID=<%=lltableID %>&DM_Row_ID=0&parent_row_id=<%=llParentRowID%>&mode=2<%=QueryStringParser.CreateQueryStringPart("modal", Modal) %>';
	           location.href = address;
	       }               
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
                var DisW = true;
		        switch (args.get_item().get_commandName()) 
		        {
                    case "new":
                    case "close":
                        DisW = false;
                        break;
		           case "copy":
		           case "save":
		           case "saveandprevious":
		           case "saveandnext":
                   case "saveandclose":
		               if (typeof Page_ClientValidate === "function") {
		                   Page_ClientValidate('CheckMandatoryFields');
		                   DoPostback = Page_IsValid;
		               }		
		               break;
                    case "delete":
                        if (!window.confirm(UIMessages[21])) {
                        DoPostback = false;
                        }
                        break;                                                
		        }
		        if (!DoPostback)
	            {
	                args.set_cancel(true);
	            }
                else if (DisW)
                {
                    DisableWindow();
                }
		    }		 
	    </script>
		<Arco:PageController ID="PC" runat="server" PageLocation="Popup" ReloadFunction="SaveObject"  />
		
		    <Telerik:RadToolBar ID="tlbMain" runat="server" OnClientButtonClicking="onTableToolbarClick" Width="100%" >
		        <Items>
		            <Telerik:RadToolBarButton EnableImageSprite="true" ID="lnkNew" PostBack="false" NavigateUrl="javascript:NewRecordInTable();" CommandName="new"></Telerik:RadToolBarButton>
		            <Telerik:RadToolBarButton EnableImageSprite="true" ValidationGroup="CheckMandatoryFields" id="cmdCopy" PostBack="true" CommandName="copy"></Telerik:RadToolBarButton>
		            <Telerik:RadToolBarButton EnableImageSprite="true" IsSeparator="true"></Telerik:RadToolBarButton>
		            <Telerik:RadToolBarButton EnableImageSprite="true" ValidationGroup="CheckMandatoryFields" id="cmdSave" PostBack="true" CommandName="save"></Telerik:RadToolBarButton>
		            <Telerik:RadToolBarButton EnableImageSprite="true" ValidationGroup="CheckMandatoryFields" id="cmdSaveAndClose" PostBack="true" CommandName="saveandclose"></Telerik:RadToolBarButton>

		            <Telerik:RadToolbarButton EnableImageSprite="true" id="cmdDelete" ToolTip="Delete" runat="server" CommandName="delete" Enabled="true" Value="delete" />
		            <Telerik:RadToolBarButton EnableImageSprite="true" IsSeparator="true"></Telerik:RadToolBarButton>
		            <Telerik:RadToolBarButton EnableImageSprite="true" ValidationGroup="CheckMandatoryFields" id="cmdSaveAndPrevious" PostBack="true" CommandName="saveandprevious"></Telerik:RadToolBarButton>
		            <Telerik:RadToolBarButton EnableImageSprite="true" ValidationGroup="CheckMandatoryFields" id="cmdSaveAndNext"  PostBack="true" CommandName="saveandnext"></Telerik:RadToolBarButton>
 		            
		            <Telerik:RadToolBarButton EnableImageSprite="true" IsSeparator="true"></Telerik:RadToolBarButton>
		            <Telerik:RadToolBarButton EnableImageSprite="true" id="lnkClose" PostBack="false" NavigateUrl="javascript:CloseRecord();" CommandName="close"></Telerik:RadToolBarButton>
		        </Items>
		    </Telerik:RadToolBar>
		   
		      <asp:PlaceHolder id="sharedcalenderPlaceHolder" runat="server"></asp:PlaceHolder>
			<arcoctrls:DMObjectForm id="docform" Runat="server" isTableRecord="true" ></arcoctrls:DMObjectForm>
			
			
            <Arco:PageFooter ID="ft" runat="server" />
		
		<asp:ValidationSummary id="valSumm" runat="server" ShowMessageBox="false" ShowSummary="false"></asp:ValidationSummary>

<asp:LinkButton runat="server" ID="btnSave" Text=""></asp:LinkButton>
          <script type="text/javascript">
              Sys.Application.add_load(UpgradeASPNETValidation);
        </script>
</form>
</body>
</html>