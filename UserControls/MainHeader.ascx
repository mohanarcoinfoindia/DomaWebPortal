<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MainHeader.ascx.vb" Inherits="UserControls_MainHeader" Strict="false" %>

<asp:SiteMapDataSource runat="server" ID="smds1" ShowStartingNode="false" StartingNodeOffset="0" />

<script type="text/javascript">
    function LogOff() {
        location.href = '<%=ResolveClientUrl("~/Auth/Logoff.aspx")%>';
    }
    function Login() {
          location.href = '<%=ResolveClientUrl("~/Auth/Login.aspx")%>';
    }

    function MyPrefs(url, isExternal) {

        if (url.indexOf("&modal=") < 0) {
            url = url + '&modal=Y';
        }
        if (url.indexOf("&rnd_str=") < 0) {
            url = url + '&rnd_str=' + Math.random();
        }

        var oWnd = GetRadWindowManager().open(url, "wndModalManualSizeNoResize");
        const height = document.body.clientHeight;
        const width = document.body.clientWidth;

        oWnd.setSize(width * 75 / 100, height - 50); 
        oWnd.set_modal(true);

        if (isExternal) {
            oWnd.add_close(SyncExternalPrefs);
        }
    }    
</script>

<div class="MainMenu_header navbar navbar-expand-xl">
    <Arco:Logo ID="mainLogo" runat="server" Mode="MainLogo" />
    <div class="MainMenu_RightNav d-flex flex-row order-2 order-xl-3">
        <button class="MainMenu_Toggler navbar-toggler" type="button" data-toggle="collapse" data-target="#<%= pnlNav.ClientID %>">
            <span class="MainMenu_TogglerIcon navbar-toggler-icon"></span>
        </button>
        <div class="navbar-nav flex-row">
            <asp:Panel ID="imgWorkWrapper" CssClass="mainmenu_img_wrapper nav-item" runat="server">
                <a href="<%= ResolveClientUrl("~/Main.aspx?page=Routing") %>">
                    <asp:Image ID="imgWork" runat="server" CssClass="mainmenu_img" />
                </a>
            </asp:Panel>
            <asp:Panel ID="imgHelpWrapper" CssClass="mainmenu_img_wrapper nav-item" runat="server">
                <asp:Image ID="imgHelp" runat="server" CssClass="mainmenu_img" ToolTip="Help" />
            </asp:Panel>

            <asp:Panel ID="pnlProfile" runat="server" CssClass="nav-item mainmenu_profile">
                <div>
                    <div class="mainmenu_username">
                        <asp:Label ID="lblUserNameMain" runat="server"></asp:Label>
                    </div>
                    <asp:Panel ID="pnlTenant" runat="server" CssClass="mainmenu_tenant">
                        <asp:Label ID="lblTenant" runat="server"></asp:Label>
                    </asp:Panel>
                </div>

                <div class="mainmenu_img_wrapper">
                    <asp:Image ID="imgProfile" runat="server" CssClass="mainmenu_img" />
                </div>

            </asp:Panel>
            <asp:Panel ID="pnlGuest" runat="server" CssClass="nav-item mainmenu_loginbutton_wrapper" Visible="false">
              
                       <Arco:ArcoButton runat="server" OnClientClick="Login();return false;" Text="Login" ID="btnLogin" />                                     
            </asp:Panel>
        </div>
    </div>

    <asp:Panel ID="pnlNav" runat="server" CssClass="MainMenu_MainNav collapse navbar-collapse order-3 order-xl-2">
        <asp:Repeater ID="repLvl1" runat="server" DataSourceID="smds1" Visible="True">
            <HeaderTemplate>
                <div class="MainMenu_ItemsWrapper navbar-nav mr-auto">
            </HeaderTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
    </asp:Panel>


</div>


<telerik:RadToolTip ID="tltProfile" runat="server" TargetControlID="pnlProfile" Width="360px" AutoCloseDelay="5000" ShowEvent="OnClick" HideEvent="LeaveToolTip" Position="BottomLeft" Style="z-index: 9000;">

    <div class="tltProfileUserActions">
        <div class="profileActionLinks">
            <asp:Panel ID="pnlImpersonator" runat="server" CssClass="profileActionLink">
                <asp:LinkButton ID="lnkImpersonator" runat="server" CssClass="ArcoButton" OnClick="lnkImpersonator_Click"></asp:LinkButton>
            </asp:Panel>

            <asp:Panel ID="pnlSelectTenant" runat="server" CssClass="profileActionLink">
                <asp:HyperLink ID="lnkSelectTenant" runat="server" SkinID="ArcoButton" />
            </asp:Panel>

            <div class="profileActionLink">
                <asp:HyperLink ID="lnkPrefs" runat="server" SkinID="ArcoButton" NavigateUrl="javascript:MyPrefs();" Text="Settings" />
            </div>
            <div class="profileActionLink">
                <asp:HyperLink ID="lnkLogoff" runat="server" SkinID="ArcoButton" NavigateUrl="javascript:LogOff();" Text="Logoff" />
            </div>
        </div>
    </div>

</telerik:RadToolTip>

<telerik:RadToolTip ID="tltHelp" runat="server" TargetControlID="imgHelpWrapper" Width="360px" AutoCloseDelay="5000" ShowEvent="OnClick" HideEvent="LeaveToolTip" Position="BottomLeft" Style="z-index: 9000;">
    <div class="tltProfileUserActions">
        <div class="profileActionLinks">

            <asp:Panel ID="pnlHelpLink" runat="server" CssClass="profileActionLink">
                <asp:HyperLink ID="lnkHelp" runat="server" SkinID="ArcoButton" />

            </asp:Panel>

            <asp:Panel ID="pnlLegalLink" runat="server" CssClass="profileActionLink">
                <asp:HyperLink ID="lnkLegal" runat="server" SkinID="ArcoButton" />

            </asp:Panel>
        </div>
    </div>
</telerik:RadToolTip>

<script type="text/javascript">
    $(function () {
        const navbarCollapseBreakpoint = 768;
        $('.dropdown').hover(function () {
            if ($(window).width() >= navbarCollapseBreakpoint) {
                $(this).trigger('dm.dropdown');
                $(this).addClass('show');
                $('.dropdown-menu', this).addClass('show');
            }
        }, function () {
            if ($(window).width() >= navbarCollapseBreakpoint) {
                $(this).removeClass('show');
                $('.dropdown-menu', this).removeClass('show');
            }
        });
        $('.dropdown').on('dm.dropdown', function (event) {
            const viewportWidth = $(window).width();
            if (viewportWidth >= navbarCollapseBreakpoint) {
                const dropdownMenu = $('.dropdown-menu', this);
                const totalWidth = $(this).position().left + $('.dropdown-menu', this).width();
                const difference = viewportWidth - totalWidth;
                if (difference < 10) {
                    const move = Math.round(difference) - 10;
                    dropdownMenu.css("transform", "translateX(" + move + "px)");
                    $('.MainMenu_Triangle', this).css("transform", "translateX(" + (move * -1) + "px)");
                }
            }
        });
    });
</script>
