<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_SELECT_OBJ_CAT.aspx.vb" Inherits="Doma.DM_SELECT_OBJ_CAT" MasterPageFile="~/masterpages/Toolwindow.master" %>

<%@ Register TagPrefix="Telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

 <asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" Runat="Server">    
    <script type="text/javascript">
        function NewObject(catid, type, parentid, inputselection, objectid, packid, caseid, mode) {
            location.href = PC().GetRedirectUrl("DM_NEW_OBJECT.ASPX?DM_CAT_ID=" + catid + "&CAT_TYPE=" + type + "&DM_PARENT_ID=" + parentid + "&INPUTSELECTION=" + inputselection + "&DM_OBJECT_ID=" + objectid + "&PACK_ID=" + packid + "&CASE_ID=" + caseid + "&mode=" + mode);
        }
        function NewCase(procid, parentid, inputselection, objectid, packid, caseid) {
            location.href = PC().GetRedirectUrl("DM_NEW_CASE.ASPX?PROC_ID=" + procid + "&DM_PARENT_ID=" + parentid + "&INPUTSELECTION=" + inputselection + "&DM_OBJECT_ID=" + objectid + "&PACK_ID=" + packid + "&CASE_ID=" + caseid);
        }
        function NewObjectAndFile(catid, parentid, inputselection) {
            location.href = PC().GetRedirectUrl('DM_FILE_ADD.ASPX?CAT_ID=' + catid + '&DM_PARENT_ID=' + parentid + "&INPUTSELECTION=" + inputselection);
        }
     
        function Filter() {
            document.forms[0].submit();
        }
    </script>
    <style>
        .List td {
            width: auto;
        }

        .toolWindowPopup {
            margin: 0 15px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <asp:LinkButton ID="lnkEnter" runat="server" OnClientClick="Filter"></asp:LinkButton>

    <telerik:RadAjaxLoadingPanel runat="server" ID="pnlLoading"></telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxPanel runat="server" ID="pnlUpdate" LoadingPanelID="pnlLoading">
        <asp:Table ID="tblTabs" CssClass="List PaddedTable" runat="server">
            <asp:TableRow CssClass="DetailHeader">
                <asp:TableCell CssClass="DetailHeaderContent">
                    <div class="selectObjectHeader">
                        <div class="multiDocUpload">
                            <ArcoControls:CheckBox ID="chkMode" runat="server" AutoPostBack="true" Text="Multi-Document upload" />
                        </div>
                    </div>
                </asp:TableCell>
                <asp:TableCell>
                    <div class="selectObjectHeader">
                        <div class="filter">
                            <span><strong>View: &nbsp</strong></span>
                            <asp:DropDownList ID="drpDownMode" runat="server" AutoPostBack="true">
                                <asp:ListItem Value="1" Selected="true" Text="Details"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Icons"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow>
                <asp:TableCell>
                    <asp:TextBox ID="txtFilter" runat="server"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="drpDwnType" runat="server" AutoPostBack="true"></asp:DropDownList>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </telerik:RadAjaxPanel>

    <Arco:PageFooter ID="lblFooter" runat="server" />
</asp:Content>
