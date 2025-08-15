<%@ Page Language="vb" AutoEventWireup="false" Inherits="Doma.Tree" CodeFile="Tree.aspx.vb" %>

<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<!DOCTYPE html>
<html>
<head runat="server">
    <title>Tree</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <style type="text/css">
        .RadToolBarDropDown .rtbWrap {
            font-weight: normal;
            white-space: nowrap;
        }

        .RadToolBarDropDown .rtbChecked .rtbWrap {
            font-weight: bold;
            white-space: nowrap;
        }


        .rtbWrap[target] .rtbText {
            border-bottom: 1px solid #000;
            padding-bottom: 1px;
        }

        .webpanelcontent {
            background-color: #FFFFFF;
        }

        #mbox {
            background-color: #eee;
            padding: 2px;
            border: 2px outset #666;
        }

        #mbm {
            font-family: sans-serif;
            font-weight: bold;
            float: right;
            padding-bottom: 2px;
        }

        .dialog {
            display: none
        }

        .inputCell {
            margin-top: 3px !important;
        }

        .rsbButton.rsbButtonSearch {
            display: none;
        }
    </style>
</head>
<body onload="initmb();SetFocus();">

    <form id="Form1" method="post" runat="server" defaultbutton="lnkEnter" defaultfocus="none">
        <Arco:PageController ID="PC" runat="server" PageLocation="TreePage" />
        <asp:LinkButton ID="lnkEnter" runat="server" OnClientClick="javascript:Filter();return false;"></asp:LinkButton>

        <script type="text/javascript">
            function onSearchBoxButtonCommand(sender, args) {
                switch (args.get_commandName()) {
                    case "ToggleAdvanced":
                        ToggleAdvanced()
                        break;
                }
            }
            function ToggleAdvanced() {
                var slidingZone = $find("<%= sldZoneAdv.ClientID %>");
                var paneID = "<%= slnpaneAdvSearch.ClientID %>";

                // finds the Rad sliding pane need to be expanded.       
                var expandedPaneId = slidingZone.get_dockedPaneId();

                if (expandedPaneId == null || expandedPaneId != paneID) {
                    slidingZone.expandPane(paneID);
                    slidingZone.dockPane(paneID);
                }
                else {
                    slidingZone.undockPane(paneID);
                    slidingZone.collapsePane(paneID);
                }

            }
            function AddToFavoritesComplete() {
             //PC().ShowSuccess('<%=GetdecodedLabel("addpackagedone") %>');
            }
            function AddToPackageComplete() {
                PC().ShowSuccess('<%=GetDecodedLabel("addpackagedone")%>');
            }

            function OnClientPaneDocked_QT(sender, eventArgs) {

                var slidingZone = $find("<%= sldZoneAdv.ClientID%>");
                var container = slidingZone.getTabsContainer();
                var dockedPane = slidingZone.getPaneById(slidingZone.get_dockedPaneId());
                if (dockedPane) {
                    dockedPane.hideTab();
                    container.style.display = 'none';
                }
            }

            function OnClientBeforePaneUndocked_QT(sender, eventArgs) {
                var slidingZone = $find("<%= sldZoneAdv.ClientID %>");
                var container = slidingZone.getTabsContainer();
                var undockedPane = slidingZone.getPaneById(slidingZone.get_dockedPaneId());
                if (undockedPane) {
                    undockedPane.showTab();

                    container.style.display = 'block';
                }
            }



        </script>
        <input id="txtSelectedID" type="hidden" runat="server" name="txtSelectedID" />
        <input id="txtCutID" type="hidden" runat="server" name="txtCutID" />
        <input id="subaction" type="hidden" name="subaction" />
        <input id="txtGotoID" type="hidden" name="txtGotoID" runat="server" />

        <telerik:RadSplitter ID="radSplitterMain" runat="server" Height="100%" Width="100%" Orientation="Horizontal" VisibleDuringInit="false" BorderStyle="None">
            <telerik:RadPane ID="radPaneFilter" runat="server" Height="34px" Scrolling="none" BorderStyle="None">
                <telerik:RadSearchBox runat="server" ID="txtFilter" OnClientSearch="Filter" OnClientButtonCommand="onSearchBoxButtonCommand"
                    EnableAutoComplete="false" Width="100%" EmptyMessage="Search Folders">
                </telerik:RadSearchBox>
                <telerik:RadSlidingZone ID="sldZoneAdv" runat="server" ClickToOpen="true">
                    <telerik:RadSlidingPane ID="slnpaneAdvSearch" runat="server" Title="Advanced" Scrolling="Y" BorderStyle="None" OnClientUndocking="OnClientBeforePaneUndocked_QT" OnClientDocked="OnClientPaneDocked_QT">
                        <Arco:DMSearchForm ID="frmAdvanced" runat="server" />
                    </telerik:RadSlidingPane>
                </telerik:RadSlidingZone>
            </telerik:RadPane>

            <telerik:RadPane ID="radPaneContent" runat="server" Scrolling="Both" Height="100%" BorderStyle="None">
                <asp:PlaceHolder ID="plhTree" runat="server"></asp:PlaceHolder>
                <div id="pastebox" class="dialog">
                    <ul>
                        <li>
                            <a href="javascript:hm('pastebox');DMC('PasteCommand');"><%=Getlabel("move") %></a>
                        </li>
                        <li>
                            <a href="javascript:hm('pastebox');DMC('PasteCopyCommand');"><%=Getlabel("copy") %></a>
                        </li>
                        <li>
                            <a href="javascript:hm('pastebox');DMC('PasteShortCutCommand');"><%=Getlabel("createshortcut") %></a>
                        </li>
                        <li>
                            <a href="javascript:hm('pastebox');"><%=GetLabel("cancel") %></a>
                        </li>
                    </ul>
                </div>
            </telerik:RadPane>
        </telerik:RadSplitter>
    </form>

    <script>
            $("#txtFilter").hover(function () {
                $(".rsbButton.rsbButtonSearch").fadeIn("fast");
            }, function () {
                if (!$("#txtFilter_Input").is(":focus")) {
                    $(".rsbButton.rsbButtonSearch").fadeOut("fast");
                }
            });

            $("#txtFilter_Input").focusout(function () {
                $(".rsbButton.rsbButtonSearch").hide();
            });

            $("#txtFilter_Input").focusin(function () {
                $(".rsbButton.rsbButtonSearch").show();
            });
    </script>
</body>
</html>
