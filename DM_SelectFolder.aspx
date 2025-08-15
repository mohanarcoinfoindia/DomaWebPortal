<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_SelectFolder.aspx.vb" Inherits="DM_SelectFolder" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Soluz.io - Select Folder</title>
  
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
   <script language="javascript">
         function Filter()
        {
        document.forms[0].submit();
        }

   </script>
</head>
<body style="height:100%;margin:0px;overflow:hidden;" scroll="no">
    <form id="Form1" runat="server" defaultbutton="lnkEnter">
        <asp:LinkButton ID="lnkEnter" runat="server" OnClientClick="Filter"></asp:LinkButton>

      <script language="javascript" src="./Resources/<%=Arco.Security.BusinessIdentity.CurrentIdentity.Language  %>/Messages.js"></script>
     <script language="javascript">
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
    	
    	var bFromDetail = false;
    	var bConfirmed = true;
		var id = new String($get("txtSelectedID").value);
		id = id.substring(1,id.length);
		    <%if not IndexMode Then %>
			<% if ConfirmMove Then %>
			    bConfirmed = window.confirm(UIMessages[16]);
			<% else %>
			    bConfirmed = true;
			<%end if %>
			if (bConfirmed)
			{
			    try
			    {
			    MainPage().document.forms[0].elements['<%=GetReturnTextField() %>'].value = id;
			    }
			    catch (e)
			    {
    			
			    }		
			    MainPage().document.forms[0].elements['<%=GetReturnValueField() %>'].value = id;	
			    try
			    {
			        MainPage().SaveObject();
			    }
			    catch (e)
			    {
                }
                
                Close();

                <%If ReloadMainPage Then%>
                MainPage().__doPostBack();
                <%End If%>
			}
			<%else %>
			parent.ChangeFolder(id);
			<%end if %>
        }

         function Close(){
    <%if not Modal then %>
	self.close();
	<%else %>
	GetRadWindow().Close();
	<%end if %>
}
    </script>
    	<input id="txtSelectedID" type="hidden" runat="server">
<Telerik:RadScriptManager runat="server" id="sc1"></Telerik:RadScriptManager>
    <Telerik:RadSplitter id="radSplitterMain" runat="server"  Height="100%" Width="100%" Orientation="Horizontal" VisibleDuringInit="false" >
<Telerik:RadPane ID="radPaneNavigation" runat="server" Height="30px" Scrolling="none">
  
     <Telerik:RadToolBar ID="tlbSearch" runat="server" SkinID="GridToolbar" width="100%" >
    <Items>
    <Telerik:RadToolBarDropDown ExpandDirection="Down">
        <Buttons>
            
        </Buttons>
    </Telerik:RadToolBarDropDown>
    <Telerik:RadToolBarButton>
    <ItemTemplate>
        <asp:TextBox id="txtFilter" runat="server" Width="100" placeholder="Search Folders"></asp:TextBox>
    </ItemTemplate>
    </Telerik:RadToolBarButton>
    <Telerik:RadToolBarButton NavigateUrl="javascript:Filter();" ImageUrl="./Images/Search.png" ></Telerik:RadToolBarButton>
    
    </Items>
    </Telerik:RadToolBar>

</Telerik:RadPane>
   <Telerik:RadPane  ID="radPaneContent" runat="server" Scrolling="Both" Height="100%" >
		
		<span oncontextmenu="return ShowContextMenu(arguments[0]);">
			<asp:Literal ID="TreeView" Runat="server" EnableViewState="False"></asp:Literal>
		</span>
	
	</Telerik:RadPane>
</Telerik:RadSplitter>	
		    		
    </form>
</body>
</html>
