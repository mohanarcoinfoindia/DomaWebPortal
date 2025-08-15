<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Page.aspx.vb" Inherits="Page" ValidateRequest="false" MasterPageFile="~/masterpages/Base.master" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<%@ Register Assembly="Arco.Doma.CMS" Namespace="Arco.Doma.CMS" TagPrefix="ArcoCms" %>


<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
     <script src="./JS/CMS.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ContentPlaceHolderID="Plh1" runat="server">

        <Arco:PageController ID="thisPageController" runat="server" PageLocation="ListPage"  />
            
        
        <asp:PlaceHolder id="tlbEditPage" runat="server">
            <telerik:RadToolBar runat="server" id="tlbEdit" Width="100%">
                <Items>
                    <Telerik:RadToolBarButton ID="lnkRefresh" EnableImageSprite="true" SpriteCssClass="icon-refresh" PostBack="false"></Telerik:RadToolBarButton>
                     <Telerik:RadToolBarButton ID="lnkSave" EnableImageSprite="true" SpriteCssClass="icon-save" PostBack="true" CommandName="SAVE"></Telerik:RadToolBarButton>
                      <Telerik:RadToolBarButton ID="lnkEdit" EnableImageSprite="true" SpriteCssClass="icon-edit" PostBack="false"></Telerik:RadToolBarButton>
                       <Telerik:RadToolBarButton ID="lnkAdd" EnableImageSprite="true" SpriteCssClass="icon-add-new" PostBack="false"></Telerik:RadToolBarButton>
                        <Telerik:RadToolBarButton ID="lnkToViewMode" EnableImageSprite="true" SpriteCssClass="icon-close" PostBack="false"></Telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>                        
        </asp:PlaceHolder>
        <asp:HiddenField runat="server" ID="hdnActiveButton" />
        <asp:HiddenField runat="server" ID="hdnActiveTile" />
        <ArcoCms:PageContainer runat="server" ID="pageCont"/>

        <asp:PlaceHolder id="tlbViewPage" runat="server">
            <asp:LinkButton ID="lnkToEditMode" runat="server"></asp:LinkButton>&nbsp;

        </asp:PlaceHolder>

    <script type="text/javascript">
        const activeButton = document.getElementById('<%= hdnActiveButton.ClientID %>');
        const activeTile = document.getElementById('<%= hdnActiveTile.ClientID %>');
        $(document).ready(function () {
            if (window.initActivateActionButtons) {
                initActivateActionButtons();
            }
            InitPage();
        });
    </script>
</asp:Content>
