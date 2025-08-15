<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_detail.aspx.vb" Inherits="DM_detail" EnableEventValidation="false"  %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" TagPrefix="ArcoControls" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">     
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <script src="./js/DetailWindow.js" type="text/javascript"></script>

<style type="text/css">  
    .checkB label{  
        position: relative;  
        top: -2px;  
    }      

    .modal .modal-dialog-aside{ 
	    width: 350px; max-width:80%; height: 100%; margin:0;transform: translate(0); transition: transform .2s;
    }
.modal .modal-dialog-aside .modal-content{  height: inherit; border:0; border-radius: 0;}
.modal .modal-dialog-aside .modal-content .modal-body{ overflow-y: auto }
.modal.modal-right .modal-dialog-aside{ margin-left:auto; transform: translateX(100%); }
.modal.show .modal-dialog-aside{ transform: translateX(0);  }
.modal {z-index:10000!important}
</style>  


</head>
<body id="body" runat="server">
    <form id="form1" runat="server" style="height:100%;margin:0px;">        
        <asp:HiddenField id="hdnComment" runat="server"></asp:HiddenField>
        <Arco:PageController ID="PC" runat="server" PageLocation="DetailWindow" ReloadFunction="ReloadContent()"  />

      
        <Telerik:RadSplitter id="radSplitterMain" runat="server" Height="100%" Width="100%" Orientation="Horizontal" VisibleDuringInit="false" SplitBarsSize="0"
           OnClientLoaded="SetRadSplitterNavigationHeight" OnClientResized="SetRadSplitterNavigationHeight">
        
           <Telerik:RadPane ID="radPaneNavigation" runat="server" CssClass="detailRadPaneNavigation" Scrolling="None" ShowContentDuringLoad="false">
               <%--<asp:Label runat="server" ID="helpButtonEyeCatcher" CssClass="help-eye-catcher"></asp:Label>--%>
               <asp:Table runat="server" ID="tblToolbar" width="100%"  CellPadding="0" CellSpacing="0" border="0" CssClass="detailTblToolbar">
                <asp:TableRow ID="rowTabs">
                    <asp:TableCell  VerticalAlign="Top">
                        <telerik:RadTabStrip ID="tabToolbar" runat="server" MultiPageID="RadMultiPage1" SkinID="DetailWindowTabStrip"  SelectedIndex="0">
				            <Tabs></Tabs>
                        </telerik:RadTabStrip>
			        </asp:TableCell>
			        <asp:TableCell></asp:TableCell>
                </asp:TableRow>
			    <asp:TableRow ID="rowToolbar">
                    <asp:TableCell ID="cellToolbars" CssClass="cellToolbars" VerticalAlign="Top">
                        <telerik:RadMultiPage ID="RadMultiPage1" CssClass="detailToolbarIcons" runat="server"  SelectedIndex="0" Width="100%" Height="100%" >
                    	</telerik:RadMultiPage>
                    </asp:TableCell>                    
                    <asp:TableCell ID="cellScroll" HorizontalAlign="right">
                        <Telerik:RadToolBar ID="toolbarScroll" CssClass="toolbarScroll" runat="server" AutoPostBack="false"  EnableViewState="false">
                            <Items>
                                <Telerik:RadToolBarButton ID="btnChkNextCase" runat="server">
                                <ItemTemplate>
                                    <div class="rtbDomaCheckbox">
                                        <ArcoControls:CheckBox ID="chkOpenNextCaseOnClose" runat="server" onclick="ToggleCloseCase(this)" Text=" " />
                                    </div>
                                </ItemTemplate>
                                </Telerik:RadToolBarButton>
                                <Telerik:RadToolBarButton IsSeparator="true"></Telerik:RadToolBarButton>
                                <Telerik:RadToolBarButton runat="server" Value="prev" CommandName="Previous" ToolTip=""></Telerik:RadToolBarButton>
                                <Telerik:RadToolBarButton runat="server" Value="next" CommandName="Next" ToolTip=""></Telerik:RadToolBarButton>   
                                   <Telerik:RadToolBarButton value="help" runat="server"></Telerik:RadToolBarButton>
                                  <Telerik:RadToolBarButton value="close" runat="server" NavigateUrl="javascript:CloseButton();"></Telerik:RadToolBarButton>
                            </Items>
                        </Telerik:RadToolBar>
                               
                  </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            </Telerik:RadPane>
            <Telerik:RadPane ID="radPaneContent" runat="server" Scrolling="None" Height="100%" CssClass="OverFlowClass"  ShowContentDuringLoad="false" >
                <Telerik:RadSplitter ID="radSplitterContent" runat="server" Width="100%" Height="100%" Orientation="Vertical"  VisibleDuringInit="false" SplitBarsSize="1px">               
                    <Telerik:RadPane ID="radPaneLeftContent" runat="server"  width="60%" Scrolling="Both"  Height="100%"   />                            
                    <Telerik:RadSplitBar ID="radSplitbarMain" runat="server" CollapseMode="Both"   />                               
                    <Telerik:RadPane ID="radPaneRightContent" runat="server" Height="100%" Width="40%" Scrolling="Both" />
                </Telerik:RadSplitter>
            </Telerik:RadPane>
            <Telerik:RadSplitBar ID="splitAction" runat="server" CollapseMode="None" />
      
            <Telerik:RadPane ID="paneActions" Width="100%" Scrolling="Both" runat="server" ContentUrl="" Collapsed="true" Height="100px"></Telerik:RadPane>        
        </Telerik:RadSplitter>   
        
          <asp:Panel ID="helpWindow" CssClass="modal modal-right fade" runat="server" role="dialog" TabIndex="-1" aria-hidden="true">            
            <div class="modal-dialog modal-dialog-aside" role="document">
    
                  <!-- Modal content-->
                  <div class="modal-content" style="background:#fff">
                    <div class="modal-header">
                       <h5 class="modal-title">Help</h5>
                       <div>
                           <a class="icon icon-shortcut" href="javascript:PopoutHelp(); HideHelp();" title="Open in new window"></a>
                           <a class="icon icon-close icon-clickable icon-bold" data-dismiss="modal"></a>
                       </div> 
                    </div>
                    <div id="helptext" class="modal-body">
                      <p><%=HelpText %></p>
                    </div>
                  </div>
      
                </div>
        </asp:Panel>

        <script type="text/javascript">       
            function SetRadSplitterNavigationHeight(splitter, args) {
                var navigationPane = splitter.getPaneById('<%= radPaneNavigation.ClientID %>');
                if (navigationPane) {
                    var menuHeight = navigationPane.getContentElement().scrollHeight;
                    navigationPane.set_height(menuHeight);   
                    var contentPane = splitter.getPaneById('<%= radpaneContent.ClientID %>');
                    contentPane.set_height(splitter.get_height() - menuHeight);
                }
            }

            function DoActionUrl(link) {
                <%if not inline Then %>       
                    GetActionPane().set_contentUrl(link);      
                <%else %>
                    parent.DoActionUrl(link);
                <%end if %>
            }
            function CloseDetail(force) {        
                if (force)
                {
                    <%if not inline Then %>
                        var oContentPaneRight = GetPreviewPane();          
                        if (!oContentPaneRight.get_collapsed()) {
                            oContentPaneRight.collapse();
                        }            
                    <%else %>
                        parent.CloseDetail();
                    <%end if %>
                }
            }   

            function ShowDetail(url)
            {
                <%if not inline Then %>       
                    GetPreviewPane().set_contentUrl(url);                    
                    ShowRightPane();
                <%else %>
                    parent.ShowDetail(url);
                <%end if %>    
            } 
          
            function SaveContentDefaultMode(){SaveContent(1);}
        
            function SaveContent(mode)
            {            
                switch (mode)
                {
                    case 0:
                        GetContentWindow().SaveObject();
                        break;
                    case 1:
                        GetContentWindow().SaveObjectAndClose();
                        break;
                    case 2:
                         <% if not isLast Then %>
                        GetContentWindow().SaveObjectAndNext();
                        <%else %>
                        GetContentWindow().SaveObjectAndCloseWindow();
                        <%end if %>
                        break;
                    case 3:
                        <% if not isLast Then %>
                        GetContentWindow().SaveObjectAndNextEdit();
                        <%else %>
                        GetContentWindow().SaveObjectAndCloseWindow();
                        <%end if %>                
                        break;
                    case 4:
                        GetContentWindow().SaveObjectAndCloseWindow();
                        break;
                }
            }
              
            var bInCheckout = false; 
            
            function onDynamicToolbarClick(sender, args)
            {
                var doPostback = true;
                var clickedbutton = args.get_item();
            
                switch(clickedbutton.get_commandName())
	            {
	                case "Checkout":
	                    if (!bInCheckout)
	                    {
	                        radprompt("", CheckOutCallBack, 330, 100, null,"<%=GetLabel("addcomment") %>", "");
	                        doPostback = false;
	                    }
	                    break;
                    case "ReleaseTo":
                        if(ValidateForm('')){GetContentWindow().ReleaseTo(clickedbutton.get_commandArgument());}                    
                        doPostback  =false;
	                    break;		
                    case "MoveTo":                
                        if(ValidateForm('')){ GetContentWindow().MoveCaseTo(clickedbutton.get_commandArgument()); }                    
                        doPostback  =false;
	                    break;	
                    case "FinishCase":
	                    if(!confirm(UIMessages[20])){doPostback = false;} 
                        break;
	                case "DeleteCase":
	                    if(!confirm(UIMessages[4])){doPostback = false;} 
                        break;	 
                    case "DELETE":
                        if (!isInCreation) {
                            if(!confirm(UIMessages[4])){doPostback = false;}                       
                        }                        
                        break;	               	   	                                
	            }
	            if (!doPostback){args.set_cancel(true);}	       
            }
        
            function SetCheckOutComment(args) { $get('<%=hdnComment.ClientID %>').value = args; }                          
            function SetHeader(s){document.title = docName + " - " + s;}   
            function InitHeader(s) { if (s != "") { SetHeader(s); } } 

            function MainPage() {
                <% If QueryStringParser.Modal Then %>
                return GetRadWindow().BrowserWindow;
                <% Else %>
                return window.opener;
                <%  End If %>
            }

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) {
                    oWindow = window.radWindow;
                }
                else if (window.frameElement && window.frameElement.radWindow) {
                    oWindow = window.frameElement.radWindow;
                }

                return oWindow;
            }

            function getNavigationPaneHeight() {
                return document.getElementById('<%= radPaneNavigation.ClientID %>').scrollHeight;
            }


       </script>
    </form>
</body>


  
</html>
