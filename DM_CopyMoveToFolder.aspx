<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_CopyMoveToFolder.aspx.vb" Inherits="DM_CopyMoveToFolder" %>
<%@ Register Src="UserControls/SelectFolder.ascx" TagName="SelectFolder" TagPrefix="uc1" %>
<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls" TagPrefix="Arco" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Copy/Move to folder</title>   
    <script type="text/javascript" >
        function SaveObject() {
            __doPostBack('', '');
        }
    </script>
     <style type="text/css">
    .inputCell   
    {   
        margin-top: 3px !important;    
    }   

        .RadToolBarDropDown .rtbWrap { font-weight: normal; white-space:nowrap; }
        .RadToolBarDropDown .rtbChecked .rtbWrap { font-weight:bold;white-space:nowrap; }
        
            
        .rtbWrap[target] .rtbText { border-bottom: 1px solid #000; padding-bottom: 1px; }
  
	.webpanelcontent{
		background-color:#FFFFFF;
		}

   
   </style>       
   <script type="text/javascript">
       function Filter() {
           document.forms[0].submit();
       }

   </script>
</head>
<body style="height:100%;margin:0px;overflow:hidden;" scroll="no">
    <form id="form1" runat="server" defaultbutton="lnkEnter">
            <Arco:PageController ID="PC" runat="server" PageLocation="Popup"  />
        <asp:LinkButton ID="lnkEnter" runat="server" OnClientClick="Filter"></asp:LinkButton>
     
     <script type="text/javascript">
            function ShowBasket()
            {
            //do nothing, just don't generate a js error
            }
            function ShowFavorites()
            {
            //do nothing, just don't generate a js error
            }  
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
    	function DMC(CommandName){    	    
    	
        }
    </script>

    
            <asp:PlaceHolder runat="server" ID="pnlSelection" Visible="true">
            
                	<input id="txtSelectedID" type="hidden" runat="server" NAME="txtSelectedID" >

    <Telerik:RadSplitter id="radSplitterMain" runat="server"  Height="100%" Width="100%" Orientation="Horizontal" VisibleDuringInit="false" >
<Telerik:RadPane ID="radPaneNavigation" runat="server" Height="30px" Scrolling="none">
  
     <Telerik:RadToolBar ID="tlbSearch" runat="server" SkinID="GridToolbar" style="width:100%" >
     
    <Items>   
    <Telerik:RadToolBarButton>
    <ItemTemplate>
        <asp:TextBox id="txtFilter" runat="server" Width="100" placeholder="Search Folders"></asp:TextBox>
    </ItemTemplate>
    </Telerik:RadToolBarButton>
    <Telerik:RadToolBarButton NavigateUrl="javascript:Filter();" SpriteCssClass="icon icon-search" EnableImageSprite="true" ></Telerik:RadToolBarButton>
     <Telerik:RadToolBarDropDown ExpandDirection="Down">
        <Buttons>
            
        </Buttons>
    </Telerik:RadToolBarDropDown>
       <telerik:RadToolBarButton IsSeparator="true"></telerik:RadToolBarButton>
       
        <telerik:RadToolBarButton id="lnkCopy" runat="server" CommandName="COPY" Text="Copy"></telerik:RadToolBarButton>
        <telerik:RadToolBarButton id="lnkMove" runat="server" CommandName="MOVE" Text="Move"></telerik:RadToolBarButton>
         <telerik:RadToolBarButton id="lnkMerge" runat="server" CommandName="MERGE" Text="Merge"></telerik:RadToolBarButton>
        <telerik:RadToolBarButton id="lnkShortcut" runat="server" CommandName="SHORTCUT" Text="Create shortcut"></telerik:RadToolBarButton>
    
    </Items>
    </Telerik:RadToolBar>

</Telerik:RadPane>
   <Telerik:RadPane  ID="radPaneContent" runat="server" Scrolling="Both" Height="100%" >
 
	
			<asp:Literal ID="TreeView" Runat="server" EnableViewState="False"></asp:Literal>		
		
	</Telerik:RadPane>
</Telerik:RadSplitter>	
           
        </asp:PlaceHolder>
        
        
        <asp:PlaceHolder runat="server" ID="pnlMsg" Visible="false">
             <asp:Label ID="lblMessage" runat="server"></asp:Label>

             <asp:PlaceHolder ID="plhReloadPanel" runat="server">
              <script language="javascript">
                  try {
                      MainPage().PC().GetCurrentResultGrid().ResetSelection();
                  }
                  catch (e) {

                  }

                  MainPage().PC().Reload();

             </script>
             </asp:PlaceHolder>

        </asp:PlaceHolder>

  
    </form>
</body>
</html>
