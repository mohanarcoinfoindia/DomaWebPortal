<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Main.aspx.vb" Inherits="Main" MasterPageFile="~/masterpages/Main.master" ValidateRequest="false" %>

<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<%@ Register TagPrefix="Arco" TagName="MainHeader" Src="~/UserControls/MainHeader.ascx" %>
<%@ Register TagPrefix="Arco" TagName="SideHeader" Src="~/UserControls/SideHeader.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radS" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1">
    <Arco:PageController ID="PC" runat="server" PageLocation="MainPage" />
    <input type="hidden" id="hdnCaption" runat="server" />
    
    <script type="text/javascript">
      
        var listPaneID = '<%=paneList.ClientID %>';
        var detailPaneID = '<%=paneDetail.ClientID %>';
        var actionPaneID = '<%=paneActions.ClientID %>';
        var treePaneID = '<%=paneTree.ClientID %>';
        var menuPaneID = '<%=paneMenu.ClientID%>';
        var MenuID = '<%=mnuMain.ClientID%>';
        var ViewMode = '';
        var searchBox = '<%=SearchBox%>';
        var TargetCaseID = '<%=QueryStringParser.GetString("TARGET_CASE_ID") %>';
        var TargetObjID = '<%=QueryStringParser.GetString("TARGET_OBJ_ID") %>';
        var TargetPackID = '<%=QueryStringParser.GetString("TARGET_PACK_ID") %>';
        var TargetDossierType = '<%=QueryStringParser.GetString("TARGET_DOSSIER_TYPE") %>';
        var bCaptionChanged = false;

        function SwitchTenant(id) {
           $get('<%=hdnTenant.ClientID%>').value = id;
           document.forms[0].submit();
        }

        function SetFolder(id, packid) {
            if (packid == undefined || packid == null) {
                packid = 0;
            }
            var sComp = new String(id);
            var sCompPack = new String(packid);
            if (bCaptionChanged || sComp != PC().GetParentID() || sCompPack != PC().GetPackageID()) {
                PC().SetParentID(id);
                PC().SetPackageID(packid);
                <%if lblFolderLink.Visible then %>
                $find('<%=radAjx1.ClientID %>').ajaxRequestWithTarget('<%=radAjx1.UniqueID %>');
                <%End If%>
            }
        }

        function SetMenu(menu) {
            var currMenu = $get('<%=hdnMenu.clientid %>');
            
            if (currMenu != null && currMenu.value != menu) {
                currMenu.value = menu;
                $find('<%=ajxMenu.ClientID%>').ajaxRequestWithTarget('<%=ajxMenu.UniqueID%>');
            }
        }

        function SetCaption(stext) {
            var c = $get('<%=hdnCaption.clientid %>');
            if (c.value != stext) {                
                c.value = stext;
                bCaptionChanged = true;
            }
        }

        function DecodeHtmlEntity(str) {
            return str.replace(/&#(\d+);/g, function (match, dec) {
                return String.fromCharCode(dec);
            });
        }

        function GetNewButton() {
            var tlb = $find("<%=mnuMain.clientId %>");
            if (tlb) {
                return tlb.findItemByValue("New");
            }
            return null;
        }

        function ToggleNewButton() {
            var but = GetNewButton();
            if (but) {
                var on = ($get('<%=hdnCanAdd.clientid %>').value == '1' ? true : false);
                if (on) {
                    but.enable();
                }
                else {
                    but.disable();
                }
            }
        }
       
        function onMenuItemClicking(sender, args) {
            var url = args.get_item().get_navigateUrl();
            if ((url.indexOf("AddObject(") > -1) || (url.indexOf("NewCase(") > -1)) {
                eval(url);
                args.set_cancel(true);
            }
        }

        
function SyncExternalPrefs() {
            $.ajax(
                {
                    type: "GET",
                    url: "Main.aspx/SyncExternalUserPreferences",        
                    contentType: "application/json; charset=utf-8",
                    dataType: "json"
                });
        }
    </script>

    <input type="hidden" id="hdnTenant" runat="server" />
    <radS:RadSplitter ID="radSplitterMenu" Width="100%" Height="100%" runat="server" Orientation="Vertical" BorderStyle="None">
        
        <radS:RadPane runat="server" ID="paneSideMenu" Scrolling="None">
            <radS:RadSplitter ID="radSplitter1" Height="100%" runat="server" Orientation="horizontal" SplitBarsSize="0px" BorderSize="0" Width="100%" VisibleDuringInit="false">
                <radS:RadPane ID="paneSideBarLogo" runat="server" Scrolling="None" EnableViewState="False" CssClass="OverFlowClass">
                    <div class="MainMenu_header">
                        <Arco:Logo ID="mainLogo" runat="server" Mode="MainLogo"/>
                    </div>
                </radS:RadPane>
                <radS:RadPane ID="radpane2" runat="server" Scrolling="None" CssClass="OverFlowClass">
                    <Arco:SideHeader ID="sideHeader" runat="server" />
                </radS:RadPane>
            </radS:RadSplitter>
        </radS:RadPane>

        <radS:RadPane runat="server" ID="radPaneMainContent" Scrolling="None">
            <radS:RadSplitter ID="radSplitterContent" Height="100%" runat="server" Orientation="horizontal" SplitBarsSize="0px" BorderSize="0"
                Width="100%" VisibleDuringInit="false" OnClientLoaded="SetRadSplitterContentHeight" OnClientResized="SetRadSplitterContentHeight">
                <radS:RadPane ID="paneMenu" runat="server" Width="100%" Scrolling="None" EnableViewState="False" CssClass="OverFlowClass">
                    <Arco:MainHeader ID="headMain" runat="server" ShowLogo="false" />
                    <asp:Panel runat="server" ID="tblMenu" CssClass="MainMenuTable">
                        <div class="mainMenuTableStart">
                            <radS:RadAjaxPanel EnableAJAX="true" ID="radAjx1" runat="server">
                                <input type="hidden" id="hdnCanAdd" runat="server" />
                                <asp:Label ID="lblFolderLink" runat="server"></asp:Label>
                            </radS:RadAjaxPanel>
                        </div>
                        <div class="mainMenuTableEnd">
                            <radS:RadAjaxPanel EnableAJAX="true" ID="ajxMenu" runat="server">
                                <input type="hidden" id="hdnMenu" runat="server" />
                                <radS:RadMenu ID="mnuMain" runat="server" SkinID="MainNavigationMenu" OnClientItemClicking="onMenuItemClicking" />
                            </radS:RadAjaxPanel>
                        </div>
                    </asp:Panel>
                </radS:RadPane>
                <radS:RadPane ID="radpaneContent" runat="server" Width="100%" Height="100%" Scrolling="none" CssClass="OverFlowClass">
                    <radS:RadSplitter ID="radSplitterMain" runat="server" Height="100%" Orientation="Vertical" Width="100%" BorderSize="0" VisibleDuringInit="false" SplitBarsSize="5px" >
                        <radS:RadPane ID="paneTree" runat="server" Width="200" BorderWidth="0px" BorderStyle="None" Height="100%" MinWidth="200" Scrolling="None"></radS:RadPane>
                        <radS:RadSplitBar ID="split1" runat="server" CollapseMode="None" /> <!-- this is the tree / list splitbar -->
                        <radS:RadPane ID="radPane3" runat="server" Scrolling="none" BorderStyle="None">
                            <radS:RadSplitter ID="radSplitterDetail" runat="server" Orientation="vertical" VisibleDuringInit="false" SplitBarsSize="0">
                                <radS:RadPane ID="paneList" runat="server" Width="50%" Scrolling="Both" CssClass="OverFlowClass" BorderStyle="None" />
                                <radS:RadSplitBar ID="splitdetail" runat="server" CollapseMode="Backward" /> <!-- this is the list / preview splitbar -->
                                <radS:RadPane ID="paneDetail" runat="server" Width="50%" ContentUrl="about:blank" Collapsed="true" Scrolling="Both" BorderStyle="None" />
                            </radS:RadSplitter>
                        </radS:RadPane>
                    </radS:RadSplitter>
                </radS:RadPane>
                <%--<radS:RadSplitBar ID="splitAction" runat="server" CollapseMode="None" />--%>
                <radS:RadPane ID="paneActions" Width="100%" Scrolling="None" runat="server" ContentUrl="about:blank" Collapsed="true" Height="100px" BorderStyle="None"></radS:RadPane>
            </radS:RadSplitter>
        </radS:RadPane>
    
    </radS:RadSplitter>

    <script type="text/javascript">
        function SetRadSplitterContentHeight(splitter, args) {
            var paneMenu = splitter.getPaneById('<%= paneMenu.ClientID %>');
            var menuHeight = 0;
            if (paneMenu) {
                var menuContentEl = paneMenu.getContentElement();
                if (menuContentEl) {
                    menuHeight = menuContentEl.scrollHeight;
                    paneMenu.set_height(menuHeight);
                }
            }
            var contentPane = splitter.getPaneById('<%= radpaneContent.ClientID %>');
            contentPane.set_height(splitter.get_height() - menuHeight);
        }

    </script>

</asp:Content>

