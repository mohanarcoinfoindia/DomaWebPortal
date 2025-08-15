<%@ Page Language="vb" AutoEventWireup="false" Inherits="DM_DOCUMENTLIST" CodeFile="DM_DOCUMENTLIST.aspx.vb"
    EnableEventValidation="false" ValidateRequest="false" Strict="false" MasterPageFile="~/masterpages/Base.master" %>

<%@ MasterType TypeName="masterpages_Base" %>
<%@ Register TagPrefix="Arco" TagName="SearchButtons" Src="~/UserControls/SearchButtons.ascx" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<%@ Register TagPrefix="Arco" TagName="DocroomListControl" Src="~/UserControls/DM_ListControl.ascx" %>

<asp:Content ID="header1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <style type="text/css">
        #pagefooter {
            width: 100%;
        }

        .DocPage {
            width: 100%;
        }

        .RadPicker {
            display: inline-table;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="server">

    <Arco:PageController ID="PC" runat="server" PageLocation="ListPage" />
    <telerik:RadCodeBlock ID="blk1" runat="server">
        <script type="text/javascript">
            var screenIDField = '<%=txtSearchScreenID.UniqueID %>';
            var queryIDField = '<%=txtQryID.clientid %>';

            var isRecycleBin = <%=EncodingUtils.EncodeJsBool(msMode = "recyclebin") %>; 
            function HandleEnter() {
                PC().HandleEnter();
                return false;
            }
        </script>
    </telerik:RadCodeBlock>
    <input type="hidden" name="QRY_VALUE" id="QRY_VALUE" runat="server" />
    <input type="hidden" name="SITE" id="SITE" runat="server" />
    <asp:HiddenField ID="selectiontype" runat="server" Value="0" />
    <asp:HiddenField ID="selectiondate" runat="server" />
    <asp:HiddenField ID="selectionid" runat="server" Value="0" />
    <asp:HiddenField ID="catid" runat="server" Value="0" />

    <asp:HiddenField ID="txtQryID" runat="server" EnableViewState="true" />
    <asp:HiddenField ID="txtSearchScreenID" runat="server" EnableViewState="true" />
    <asp:LinkButton ID="lnkEnter" runat="server" OnClientClick="javascript:DisablePanelButtons(this);return HandleEnter();"></asp:LinkButton>
    <div class="DocPage">
        <div id="docPageHead" style="z-index: 1;">
            <%-- z-index to make this hide the pagefooter for IE positioning bug --%>
            <telerik:RadToolBar runat="server" ID="tlbFolderText" Visible="false" EnableViewState="false" SkinID="GridToolbar" Width="100%" />

            <asp:Panel runat="server" ID="pnlFolderText">
                <Arco:DMObjectForm ID="foldertext" runat="server" />
            </asp:Panel>

            <asp:PlaceHolder ID="pnlSaveQuery" runat="server">
                <div class="container-fluid detail-form-container" runat="server">
                    <div class="row detail-form-row DetailHeaderContent">
                        <div class="DetailHeaderContent">
                            <asp:Label ID="lblSaveQueryHeader" runat="server" Text="Save Query"></asp:Label>
                        </div>
                    </div>

                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <span class='FormLabel'>Scope</span>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:RadioButtonList ID="rdQryScope" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="1" Text="Public" Selected="false"></asp:ListItem>
                                <asp:ListItem Value="0" Text="Private" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Secured" Selected="false"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>

                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <span class='FormLabel'><%=GetLabel("folder")%></span>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:RadioButtonList ID="optFolderToUse" runat="server" RepeatDirection="Vertical">
                                <asp:ListItem Value="0" Text="Root"></asp:ListItem>
                                <asp:ListItem Value="1" Text="The folder where the query is executed" Selected="true"></asp:ListItem>
                                <asp:ListItem Value="2" Text="This folder"></asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>

                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <span class='FormLabel'><%=GetLabel("name") %></span>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:TextBox ID="txtQryName" runat="server" MaxLength="100" Width="400px" SkinID="CustomWidth" />
                            <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ID="reqQryName" ControlToValidate="txtQryName"
                                ErrorMessage="*" SetFocusOnError="true" ValidationGroup="EditQuery">
                            </asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <span class='FormLabel'><%=GetLabel("description") %></span>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:TextBox ID="txtQryDesc" runat="server" MaxLength="2000" TextMode="MultiLine" Rows="3" Width="400px" SkinID="CustomWidth" />
                        </div>
                    </div>

                    <div id="saveQueryACLRow" class="row detail-form-row" runat="server">
                        <div class="offset-md-4 col-md-8 FieldCell">
                            <asp:LinkButton ID="lnkQueryACL" runat="server"></asp:LinkButton>
                        </div>
                    </div>

                    <div class="row detail-form-row">
                        <div class="offset-md-4 col-md-8 FieldCell">
                            <Arco:ButtonPanel runat="server">
                                <Arco:OkButton ID="lnkSaveQuery" OnClick="SaveQuery" runat="server" Text="Save" ValidationGroup="EditQuery"></Arco:OkButton>
                                <Arco:SecondaryButton runat="server" ID="lnkCancelQry" OnClientClick="BackToSearch();" Text="Cancel"></Arco:SecondaryButton>
                            </Arco:ButtonPanel>
                        </div>
                    </div>
                </div>
            </asp:PlaceHolder>

            <asp:Panel ID="pnlFilter" runat="server" class="pnlFilter">
                <div style="display: none">
                    <telerik:RadCalendar ID="SharedCalendar" runat="server"></telerik:RadCalendar>
                </div>

                <table class="DetailTable">
                    <asp:PlaceHolder ID="pnlFixedHeader" runat="server">
                        <asp:PlaceHolder ID="pnlSearchHeader" runat="server">
                            <tr class="DetailHeader">
                                <td class="DetailHeaderContent">
                                    <asp:Label ID="lblSearchHeader" Text="&nbsp;" runat="server" /></td>
                            </tr>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="pnlTemplateHeader" runat="server">
                        <asp:Label ID="lblTemplateHeader" Text="&nbsp;" runat="server" />
                    </asp:PlaceHolder>

                    <tr id="trMessages" runat="server" visible="false" enableviewstate="false">
                        <td>
                            <asp:Label ID="lblErrors" runat="server" CssClass="ErrorLabel" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <Arco:DMSearchForm ID="domasearch" runat="server" />
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="pnlgrouper" runat="server">
                        <tr>
                            <td>
                                <span class='Label'><%=GetLabel("groupby")%></span> :
                            <asp:PlaceHolder ID="plhGrouperCheckboxes" runat="server" EnableViewState="false"></asp:PlaceHolder>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="pnlFixedButtons" runat="server">
                        <tr>
                            <td align="center" id="fixedfootercell">
                                <div class="buttons">
                                    <Arco:SearchButtons ID="lnkButtons" runat="server" />
                                </div>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="pnlFixedHeaderCloser" runat="server"></asp:PlaceHolder>
                </table>
            </asp:Panel>
        </div>

        <div id="pagebody">
            <Arco:DocroomListControl runat="server" ID="grd" />
        </div>

        <div id="pagefooter" style="z-index: 0;">
            <Arco:PageFooter ID="lblFooter" runat="server" />
        </div>

    </div>
</asp:Content>

