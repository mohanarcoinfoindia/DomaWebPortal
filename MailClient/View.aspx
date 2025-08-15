<%@ Page Language="VB" AutoEventWireup="false" CodeFile="View.aspx.vb" Inherits="View" %>

<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<%@ Register TagPrefix="Arco" TagName="MailTracking" Src="MailTracking.ascx" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<!DOCTYPE HTML>
<html>
<head runat="server">
    <title>Mail Client: View Message</title>
    <style type="text/css">
        #MailBody {
            padding: 20px;
            padding-right: 0px;
        }

        #lblMessage {
            display: inline-block;
            width: 100%;
            height: auto;
        }

        .AutoHeight {
            height: auto !important;
        }
    </style>
</head>
<script type="text/javascript">

    function RefreshParentPage() {
        try {
            window.opener.PC().Reload();
        }
        catch (e) {
        }
    }

    function Reply() {
        Compose('reply');
    }
    function ReplyAll() {
        Compose('replyall');
    }
    function Forward() {
        Compose('forward');
    }
    function Compose(act) {
        var url = 'Compose2.aspx?action=' + act + '&DM_MAIL_ID=<%=CurrentMail.ID %>&TEMPLATE_ID=<%=templateid %>';
        PC().OpenWindow(url, 'MailFile', 'height=600,width=800,scrollbars=yes,resizable=yes,toolbar=no');
    }
    function ViewDomaAtt(id) {
        var url = '../DM_VIEW_FILE.aspx?frommail=Y&file_id=' + id + '&DM_MAIL_ID=<%=CurrentMail.ID %>';
        PC().OpenWindow(url, 'MailFile', 'height=600,width=800,scrollbars=yes,resizable=yes,toolbar=yes');

    }

    function PrintMe() {
        $get("printme").value = "Y";
        document.forms[0].target = "_PrintWindow";
        document.forms[0].submit();
        document.forms[0].target = "";
        $get("printme").value = "";
    }
    function PrintContent() {
        window.print();
    }

    function OpenObject(id) {
        PC().OpenDetailWindow('../dm_detail.aspx?dm_object_id=' + id, id);
    }
    function onToolbarClick(sender, args) {
        var DoPostback = true;
        switch (args.get_item().get_commandName()) {
            case "delete":
                if (!window.confirm(UIMessages[4])) {
                    DoPostback = false;
                }
                break;
        }
        if (!DoPostback) {
            args.set_cancel(true);
        }
    }
</script>
<body>
    <form id="Form10" method="post" runat="server">
        <Arco:PageController ID="thisPageController" runat="server" PageLocation="Popup" />
        <asp:HiddenField runat="server" ID="printme" Value="" />
        <asp:PlaceHolder ID="plhFullContent" runat="server">
            <asp:Table CssClass="detailTblToolbar" runat="server">
                <asp:TableRow ID="paneToolbar" runat="server">
                    <asp:TableCell CssClass="cellToolbars" Width="100%">
                        <telerik:RadToolBar ID="tlbMail" runat="server" Style="width: 100%" OnClientButtonClicking="onToolbarClick">
                            <Items>
                                <telerik:RadToolBarButton EnableImageSprite="true" CommandName="closetracking" SpriteCssClass="icon-status-complete" ToolTip="markmailcompleted" Value="closetracking" PostBack="True"></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton EnableImageSprite="true" CommandName="stopfollowup" SpriteCssClass="icon-action-stop" ToolTip="stopmailfollowup" Value="stopfollowup" PostBack="True"></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton EnableImageSprite="true" CommandName="reply" SpriteCssClass="icon-arrow-reply" ToolTip="Reply" Value="reply" PostBack="false" NavigateUrl="Javascript:Reply();"></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton EnableImageSprite="true" CommandName="replyall" SpriteCssClass="icon-arrow-reply-all" ToolTip="ReplyAll" Value="replyall" PostBack="false" NavigateUrl="Javascript:ReplyAll();"></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton EnableImageSprite="true" CommandName="forward" SpriteCssClass="icon-message-mail-forward" ToolTip="Forward" Value="forward" PostBack="false" NavigateUrl="Javascript:Forward();"></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton IsSeparator="true"></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton EnableImageSprite="true" NavigateUrl="javascript:PrintMe();" ToolTip="Print" PostBack="false" SpriteCssClass="icon-printer"></telerik:RadToolBarButton>
                                <telerik:RadToolBarDropDown ID="drpLinked" Visible="false">
                                </telerik:RadToolBarDropDown>
                                <telerik:RadToolBarButton Visible="true" IsSeparator="true"></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton CommandName="delete" EnableImageSprite="true" SpriteCssClass="icon-delete" ToolTip="Delete" Value="delete" PostBack="True"></telerik:RadToolBarButton>

                            </Items>
                        </telerik:RadToolBar>
                    </asp:TableCell>
                    <asp:TableCell HorizontalAlign="right">
                        <telerik:RadToolBar ID="toolbarScroll" runat="server" AutoPostBack="false" Style="width: 100%;" EnableViewState="false">
                            <Items>
                                <telerik:RadToolBarButton IsSeparator="true"></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton EnableImageSprite="true" SpriteCssClass="icon-page-previous icon-color-light" runat="server" Value="prev" CommandName="Previous" ToolTip=""></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton EnableImageSprite="true" SpriteCssClass="icon-page-next icon-color-light" runat="server" Value="next" CommandName="Next" ToolTip=""></telerik:RadToolBarButton>
                                <telerik:RadToolBarButton Visible="false" Value="close" NavigateUrl="Javascript:parent.CloseDetail(true);" ToolTip="Close" PostBack="false" SpriteCssClass="icon-close" EnableImageSprite="true"></telerik:RadToolBarButton>
                            </Items>
                        </telerik:RadToolBar>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>
            <telerik:RadSplitter ID="radSplitterContent" runat="server" Width="100%" Height="100%" Orientation="Horizontal" BorderStyle="none">
                <telerik:RadPane runat="server" ID="paneMailHeader" Scrolling="None" CssClass="AutoHeight">
                    <table class="DetailTable">
                        <tr>
                            <td>
                                <asp:Label ID="lblInfoLabel" runat="server" CssClass="InfoLabel" Visible="false"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DataGrid ID="dgHeaders" runat="server" CellPadding="2" GridLines="None" ShowHeader="false">
                                    <Columns>
                                        <asp:BoundColumn DataField="label" ItemStyle-CssClass="LabelCell"></asp:BoundColumn>
                                        <asp:BoundColumn DataField="value" ItemStyle-CssClass="FieldCell"></asp:BoundColumn>
                                    </Columns>
                                </asp:DataGrid>
                            </td>
                        </tr>
                    </table>
                </telerik:RadPane>
                <telerik:RadPane ID="paneTabs" runat="server" Scrolling="None" CssClass="AutoHeight" >
                    <telerik:RadTabStrip ID="tabMain" runat="server" MultiPageID="RadMultiPageMail" SelectedIndex="0">
                        <Tabs>
                            <telerik:RadTab runat="server" Text="Body" id="tabBody" />
                            <telerik:RadTab runat="server" Text="Tracking" id="tabTracking" />
                        </Tabs>
                    </telerik:RadTabStrip>
                    <telerik:RadMultiPage ID="RadMultiPageMail" runat="server" SelectedIndex="0" Width="100%">
                        <telerik:RadPageView runat="server" ID="MailBody" Height="100%">
                            <!-- Page content begins here -->
                            <asp:Label ID="lblMessage" runat="server"></asp:Label>
                            <asp:Label ID="lblTemplate" runat="server" />
                            <!-- And ends here -->
                        </telerik:RadPageView>
                        <telerik:RadPageView runat="server" ID="Mailtracking">
                            <Arco:MailTracking runat="server" ID="ctrlTracking"></Arco:MailTracking>
                        </telerik:RadPageView>
                    </telerik:RadMultiPage>
                </telerik:RadPane>
            </telerik:RadSplitter>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhPrint" runat="server">
            <table border="0">
                <tr>
                    <td style="vertical-align: top;">
                        <asp:DataGrid ID="dgPrintHeader" runat="server" CellPadding="2" GridLines="None" ShowHeader="false">
                            <Columns>
                                <asp:BoundColumn DataField="label" ItemStyle-CssClass="LabelCell"></asp:BoundColumn>
                                <asp:BoundColumn DataField="value" ItemStyle-CssClass="FieldCell"></asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </td>
                </tr>
                <tr>
                    <td>
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top;">
                        <!-- Page content begins here -->
                        <asp:Label ID="lblPrintMessage" runat="server">Label</asp:Label>
                        <!-- And ends here -->
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="plhBodyOnly" runat="server">
            <table style="width: 100%; height: 100%;" border="0">
                <tr>
                    <td style="vertical-align: top; text-align: center;">
                        <!-- Page content begins here -->
                        <asp:Label ID="lblMessageBodyOnly" runat="server">Label</asp:Label>
                        <!-- And ends here -->
                    </td>
                </tr>
            </table>
        </asp:PlaceHolder>
    </form>
</body>
</html>
