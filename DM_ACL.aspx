<%@ Page Language="vb" AutoEventWireup="false" Inherits="Doma.DM_ACL" CodeFile="DM_ACL.aspx.vb" %>

<%@ Register TagPrefix="Arco" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>ACL</title>
    <script type="text/javascript">
        function Close() {
            self.close();
        }
        function SetAclAction(value) {
            $get("aclaction").value = value;
        }
        function SetAclID(value) {
            $get("aclid").value = value;
        }
        function SetSubject(subjectid, subjecttype) {
            $get("subjectid").value = subjectid;
            $get("subjecttype").value = subjecttype;
        }
        function AddAcl(subjecttype, subjectid) {
            SetAclAction("A");
            SetSubject(subjectid, subjecttype);
            Submit();
        }
        function RemoveAcl(aclid) {
            SetAclAction("D");
            SetAclID(aclid);
            Submit();
        }
        function UnlinkAcl(aclid) {
            SetAclAction("U");
            SetAclID(aclid);
            Submit();
        }
        function LockAcl(aclid) {
            SetAclAction("L");
            SetAclID(aclid);
            Submit();
        }
        function PropagateAcl(aclid) {
            SetAclAction("P");
            SetAclID(aclid);
            Submit();
        }
        function RemoveAcl2(subjecttype, subjectid) {
            SetAclAction("D");
            SetSubject(subjectid, subjecttype);
            Submit();
        }
        function SetAclLevel(aclid, accesslevel, mode) {
            if (accesslevel != 0) {
                SetAclAction("S");
                SetAclID(aclid);
                $get("accesslevel").value = accesslevel;
                $get("accessmode").value = mode;
                Submit();
            }
        }
        function SetAclLevel2(subjecttype, subjectid, accesslevel, mode) {
            SetAclAction("S");
            SetSubject(subjectid, subjecttype);
            $get("accesslevel").value = accesslevel;
            $get("accessmode").value = mode;
            Submit();
        }
        function Override() {
            SetAclAction("O");
            Submit();
        }
        function OverrideSpecial() {
            SetAclAction("OS");
            Submit();
        }
        function ResetACL() {
            if (confirm(UIMessages[24])) {
                SetAclAction("R");
                Submit();
            }
        }
        function TakeParentACL() {
            SetAclAction("TP");;
            Submit();
        }
        function SetSubscriptions() {
            SetAclAction("QS");
            Submit();
        }
        function Submit() {
            document.forms[0].submit();
        }
        function ShowMembers(subject, subjecttype) {
            var a = 'DM_LISTMEMBERS.aspx?subjectid=' + subject + '&subjecttype=' + subjecttype;
            OpenWindow(a, 'ListMembers');
        }
        function OpenWindow(url, name) {
            var w = window.open(url, name, 'width=600,height=400,resizable=yes,scrollbars=yes,status=yes');
            w.focus();
        }
        function ViewProfile(id, objectid) {
            var a = 'DM_VIEW_ACTION_PROFILE.aspx?profileid=' + id + '&dm_object_id=' + objectid;
            OpenWindow(a, 'ViewActionProfile');
        }     
    </script>
    <style>
        span.icon {
            margin-right: 3px;
        }

        .ListTableContainer {
            overflow: auto;
            height: 75vh;
        }
    </style>
</head>
<body>
    <form id="Form1" method="post" runat="server" defaultbutton="lnkFilter" defaultfocus="txtFilter">
        <Arco:PageController ID="PC" runat="server" PageLocation="Popup" />

        <input type="hidden" id="aclaction" name="aclaction" value="">
        <input type="hidden" id="aclid" name="aclid" value="">
        <input type="hidden" id="accesslevel" name="accesslevel" value="">
        <input type="hidden" id="accessmode" name="accessmode" value="">
        <input type="hidden" id="subjectid" name="subjectid" value="">
        <input type="hidden" id="subjecttype" name="subjecttype" value="">

        <asp:Label ID="lblWarning" runat="server" Visible="False" ForeColor="red"></asp:Label>

        <div class="container-fluid">
            <%-- header --%>
            <div class="row pt-2">
                <asp:Table runat="server" CssClass="List PaddedTable">
                    <%-- header table --%>
                    <asp:TableRow VerticalAlign="Top" CssClass="ListHeader">
                        <asp:TableCell Width="60%">
                            <asp:Table ID="tblFilter" runat="server" Width="95%">
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="lblDomain" runat="server"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <asp:DropDownList ID="cmbDomains" runat="server" AutoPostBack="True" EnableViewState="True"></asp:DropDownList>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <ArcoControls:CheckBox ID="chkUsers" runat="server" Text="Users" AutoPostBack="True"></ArcoControls:CheckBox>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <ArcoControls:CheckBox ID="chkGroups" runat="server" Text="Groups" AutoPostBack="True"></ArcoControls:CheckBox>
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <ArcoControls:CheckBox ID="chkRoles" runat="server" Text="Roles" AutoPostBack="True"></ArcoControls:CheckBox>
                                    </asp:TableCell>
                                </asp:TableRow>

                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label ID="lblFilter" runat="server"></asp:Label>
                                    </asp:TableCell>
                                    <asp:TableCell Wrap="True">
                                        <asp:TextBox ID="txtFilter" runat="server"></asp:TextBox>&nbsp;
								<asp:LinkButton ID="lnkFilter" runat="server" />
                                    </asp:TableCell>
                                    <asp:TableCell>
                                        <ArcoControls:CheckBox ID="chkProps" runat="server" Text="Properties" AutoPostBack="True"></ArcoControls:CheckBox>
                                    </asp:TableCell>
                                    <asp:TableCell ColumnSpan="2">
                                        <ArcoControls:CheckBox ID="chkPacks" runat="server" Text="Packages" AutoPostBack="True"></ArcoControls:CheckBox>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:TableCell>
                        <asp:TableCell Width="40%">
                            <asp:Table runat="server" ID="tblObjectInfo" Width="95%">
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label runat="server" ID="lblObjectType"></asp:Label>: <b>
                                            <asp:Label runat="server" ID="txtObjectname"></asp:Label></b>&nbsp;
                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Right">
                                        <asp:Label ID="phInheritance" runat="server"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                                <asp:TableRow>
                                    <asp:TableCell>
                                        <asp:Label runat="server" ID="lblInherits" Style="float: left; display: inline" />
                                        <Arco:FolderLink runat="server" ID="lblInheritsFrom" JavaScriptOpenFunction="" />

                                    </asp:TableCell>
                                    <asp:TableCell HorizontalAlign="Right">
                                        <asp:Label ID="phReset" runat="server"></asp:Label>
                                    </asp:TableCell>
                                </asp:TableRow>
                            </asp:Table>
                        </asp:TableCell>
                    </asp:TableRow>

                    <asp:TableRow>
                        <asp:TableCell ColumnSpan="2"><hr/></asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>
            <%-- content --%>
            <div class="row">
                <div class="col-md-7 ListTableContainer">
                    <%-- left table --%>
                    <asp:Table ID="DomainList" runat="server" CssClass="List" EnableViewState="False">
                        <asp:TableHeaderRow CssClass="ListHeader">
                            <asp:TableHeaderCell />
                            <asp:TableHeaderCell>
                                <asp:Label ID="lblHeader1" runat="server"></asp:Label>
                            </asp:TableHeaderCell>
                            <asp:TableHeaderCell>
                                <asp:Label ID="lblHeader2" runat="server"></asp:Label>
                            </asp:TableHeaderCell>

                        </asp:TableHeaderRow>
                    </asp:Table>
                </div>
                <div class="col-md-5 ListTableContainer">
                    <%-- right table --%>
                    <asp:Table runat="server">
                        <asp:TableRow>
                            <asp:TableCell VerticalAlign="Top">
                                <asp:Table ID="ObjectPermissions" CssClass="SubList" runat="server" EnableViewState="False">
                                    <asp:TableHeaderRow CssClass="SubListHeader">
                                        <asp:TableHeaderCell>
                                            <asp:Label ID="lblHeader3" runat="server"></asp:Label>
                                        </asp:TableHeaderCell>
                                        <asp:TableHeaderCell>&nbsp;</asp:TableHeaderCell>
                                    </asp:TableHeaderRow>
                                </asp:Table>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </div>
            </div>
        </div>

        <asp:Label ID="lblFooter" runat="server"></asp:Label>
        <input type="hidden" id="txtObjectID" runat="server" name="txtObjectID">
        <input type="hidden" id="txtQueryID" runat="server" name="txtQueryID">&nbsp;
    </form>
</body>
</html>
