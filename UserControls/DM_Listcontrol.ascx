<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radC" %>
<%@ Register TagPrefix="Arco" TagName="GridScroller" Src="~/UserControls/DM_ListControlGridScroller.ascx" %>
<%@ Register TagPrefix="Arco" TagName="SelectCategory" Src="~/UserControls/SelectCategory.ascx" %>
<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DM_Listcontrol.ascx.vb" Inherits="DM_Listcontrol" Strict="false" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>


<radC:RadAjaxLoadingPanel ID="radAjxLoadingPanel" runat="server"></radC:RadAjaxLoadingPanel>
<asp:Panel runat="server" ID="radAjx1">
    <asp:LinkButton runat="server" ID="ajxButton" />
    <asp:HiddenField ID="ajxButtonUniqueId" runat="server" />

    <input id="txtSelectedID" type="hidden" name="txtSelectedID" />
    <asp:HiddenField ID="currpage" runat="server" />
    <asp:HiddenField ID="maxresults" runat="server" />
    <asp:HiddenField ID="internalsubmit" runat="server" />
    <asp:HiddenField ID="OrderbyField" runat="server" />
    <asp:HiddenField ID="OrderbyOrderField" runat="server" />
    <asp:HiddenField ID="showhidecol" runat="server" />
    <asp:HiddenField ID="DisabledFilters" runat="server" />
    <asp:HiddenField ID="GroupBy" runat="server" />
    <asp:HiddenField ID="GroupByFilter" runat="server" />
    <asp:HiddenField ID="Sel" runat="server" />
    <asp:HiddenField ID="CreationDate" runat="server" />
    <asp:HiddenField ID="ShowWhatsNew" runat="server" />
    <asp:HiddenField ID="showattachmentsfor" runat="server" />
    <asp:HiddenField ID="showfileversionsfor" runat="server" />
    <asp:HiddenField ID="filelisttogglemode" runat="server" />
    <asp:HiddenField ID="ItemCount" runat="server" />
    <asp:HiddenField ID="FilterOnDelegate" runat="server" />
    <asp:HiddenField ID="ResultScreen" runat="server" />
    <asp:HiddenField ID="txtGlobalSearch" runat="server"></asp:HiddenField>
    <asp:HiddenField ID="critfield" runat="server" />

    <asp:Panel CssClass="LayoutTable listControl" ID="tblMain" runat="server">
        <div id="trMessages" class="trMessages" runat="server" visible="false" enableviewstate="false">
            <asp:Label ID="lblErrors" runat="server" CssClass="ErrorLabel" />
        </div>

        <div id="trQuery" class="trQuery" runat="server">
            <asp:Label ID="lblQuery" runat="server" EnableViewState="false" />
        </div>

        <div id="pnlTopSidePanel" class="pnlTopSidePanel" runat="server" visible="false" enableviewstate="false">
            <asp:Panel ID="plhGrouperTop" runat="server" EnableViewState="false" />
        </div>

        <div class="listControlSideAndBody">
            <div class="listControlSide">
                <asp:PlaceHolder ID="pnlLeftSidePanel" runat="server" Visible="false" EnableViewState="false">
                    <div style="vertical-align: top; white-space: nowrap; padding-right: 5px; padding-left: 2px">
                        <asp:PlaceHolder ID="pnlCalWhatsNew" runat="server" Visible="false" EnableViewState="true">
                            <radC:RadCalendar ID="calWhatsNew" runat="server" EnableViewState="true" EnableMultiSelect="false" EnableRowSelectors="true" UseWeekNumbersAsSelectors="true" ViewSelectorText="false" EnableOutsideScripts="true" Visible="true" AutoPostBackOnDayClick="false" AutoPostBackOnNavigation="false" />
                            <ArcoControls:CheckBox ID="chkwhatsnewdatenotexact" Text="Added since" runat="server" Checked="true" />
                            <br />
                            <ArcoControls:CheckBox ID="chkWhatsNewIncludeModifDate" Text="Include modification Date" Checked="true" runat="server" />
                            <br />
                            <br />
                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="pnlQuickUpload" runat="server" Visible="false" EnableViewState="true">
                            <script type="text/javascript">

                                function OnQuickUploadFilesSelected(sender, args) {
                                    <%=ClientID %>.Reload();
                                }

                                function OnQuickUploadClicked() {
                                    $telerik.$(".ruFileInput").click();
                                }
                            </script>
                            <style type="text/css">
                                .RadUpload .ruBrowse {
                                    display: none !important;
                                }

                                .RadUpload .ruStyled .ruFileInput {
                                    position: relative !important;
                                }

                                .ruDropZone {
                                    display: normal;
                                }

                                #quDropZone {
                                    height: 100px;
                                    line-height: 50px;
                                    overflow: hidden;
                                    text-align: center;
                                    cursor: move;
                                }

                                    #quDropZone:hover {
                                        background-color: #f2f8fa;
                                    }
                            </style>
                            <div class='GroupingPanel' style='width: <%=GridGrouping.DefaultGroupingWidth%>'>
                                <div class='Header'>
                                    <Arco:SelectCategory ID="cmbQuickUploadCat" runat="server" CategoryType="Document" ForInsert="true" />
                                </div>
                                <div class='Content' style='height: 50px; overflow: hidden'>
                                    <div id="quDropZone" onclick="OnQuickUploadClicked();">
                                        Drag files here
                                    </div>
                                    <Arco:QuickUpload runat="server" ID="quickupload" EnableViewState="true" OnClientFilesUploaded="OnQuickUploadFilesSelected" DropZone="#quDropZone" />
                                </div>

                            </div>

                        </asp:PlaceHolder>
                        <asp:PlaceHolder ID="plhGrouperSide" runat="server" EnableViewState="false" />
                    </div>
                </asp:PlaceHolder>

            </div>

            <div width="100%" runat="server" id="tdGrid" class="listControlBody" visible="true">
                <div id="listControlBodyHead"></div>

                <asp:PlaceHolder ID="pnlToolbar" runat="server" Visible="false" EnableViewState="false">
                    <asp:PlaceHolder ID="plhGridHeaderStart" runat="server" />
                    <radC:RadToolBar runat="server" ID="tlbGrid" EnableViewState="false" SkinID="GridToolbar" />
                    <asp:PlaceHolder ID="plhGridHeaderStartEnd" runat="server" />
                    <asp:PlaceHolder ID="plhGridHeader" runat="server" />
                    <asp:PlaceHolder ID="plhGridHeaderEnd" runat="server" />
                </asp:PlaceHolder>

                <%-- Caution! inserting something here will break the StickyHeaders css. --%>

                <div class="SubListDiv">

                    <asp:Repeater ID="imageList" runat="server" EnableViewState="false">
                        <%--ShowHeader="true"--%>
                        <HeaderTemplate>
                            <table id="<%=ClientID %>_imageList" style="width: 100%;">
                                <tr>
                                    <td>
                                        <asp:PlaceHolder ID="plhHeader" runat="server" />
                                        <asp:PlaceHolder ID="plhMnu" runat="server" />
                                        <asp:PlaceHolder ID="plhContentStart" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="tiles">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:PlaceHolder ID="plhMainImageListCell" runat="server" EnableViewState="false" />
                        </ItemTemplate>
                        <FooterTemplate>
                            </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:PlaceHolder ID="plhContentEnd" runat="server" />
                                </td>
                            </tr>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                    <asp:Repeater ID="docList" runat="server" EnableViewState="false">
                        <HeaderTemplate>

                            <div class="docList">
                                <table class="<%=CssClass %>" id="<%=Me.ClientID %>_documentlist" data-show-toggle="false">
                                    <thead>
                                        <asp:PlaceHolder ID="plhHeader" runat="server" />
                                    </thead>
                                    <tbody>

                                        <asp:PlaceHolder ID="plhFilter" runat="server" />
                                        <asp:PlaceHolder ID="plhMnu" runat="server" />

                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:PlaceHolder ID="plhMainRow" runat="server" EnableViewState="false" />
                            <tr runat="server" id="filelistrow" visible="false" class="ListSubContent">
                                <td id="filelistspacer" runat="server">&nbsp;</td>
                                <td>
                                    <img src="./Images/hr_l.gif" alt="" /></td>
                                <td id="filelistcol" runat="server">
                                    <Arco:DMFileForm ID="fileList" runat="server" Visible="true" FileTypes="<%#ContentParams.FileTypes %>" ExpandPreviousVersionsFor="<%#ShowPreviousFileVersionsFor%>" AlwaysExpandPreviousVersions="<%#(AlwaysShowPreviousFileversion) %>" ShowFileVersion="<%#(PreviousVersions) %>" ObjectStatus='<%#DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "Status") %>' InListScreen="true" EnableViewState="false" />
                                </td>
                            </tr>
                            <tr runat="server" id="filelistrow2" visible="false" class="ListSubContent listSubContentFillerRow">
                                <td id="filelistspacer2" runat="server"></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>                       
		                </table>
                        </div>
                    
                        </FooterTemplate>
                    </asp:Repeater>
                    <Arco:GridScroller ID="doclistscroller" runat="server" Visible="false" EnableViewState="false" />
                </div>
            </div>
        </div>


        <div id="pnlBottomSidePanel" class="pnlBottomSidePanel" runat="server" visible="false" enableviewstate="false">
            <asp:PlaceHolder ID="plhGrouperBottom" runat="server" EnableViewState="false" />
        </div>
    </asp:Panel>
    <%If EnableFooTable Then%>
    <script src='<%=ResolveClientUrl("~/JS/footable.min.js")%>' type="text/javascript"></script>

    <script type="text/javascript">
                                $(function ($) {
          //  var lp = $find("<%= radAjxLoadingPanel.ClientID%>");
           // lp.show("<%=clientID %>_documentlist");

                                    $("#<%=clientID %>_documentlist").footable({
                                        toggleSelector: " > tbody > tr > td > span.footable-toggle"
                                    }
                                    );

         //   lp.hide("<%=clientID %>_documentlist");

                                    //  $(".LayoutTable").css("visibility", "visible");
                                });

    </script>
    <%end if%>
</asp:Panel>

