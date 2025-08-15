<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_Listitemdetail.aspx.vb"
    Inherits="DM_Catalog_Listitemdetail" ValidateRequest="false" EnableSessionState="True" %>

<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" TagPrefix="ArcoControls" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title></title>
    <script type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
        }
        function Close() {
            GetRadWindow().Close();
        }

        function switchCategorySelection(itemid, catid, checked) {
            WSManagedLists1.switchCategorySelection(itemid, catid, checked, ServiceCompleteCallback1, ServiceErrorCallback1);
        }

        function ServiceCompleteCallback1(ResponseObject, ResponseAsXml, ResponseAsText) {
            return true;
        }

        function ServiceErrorCallback1(args) {
            return true;
        }

        function GotoManagedLists() {
            parent.GotoManagedLists();
        }
        function RefreshRightPaneLID2(listid, itemid, selectedids) {

            var w = window.open('DM_Catalog_listitemdetail.aspx?id=' + itemid + '&selectedids=' + selectedids + '&rnd_str=' + Math.random() + '', '_self', 'width=600,height=600');
            w.focus();
        }
        function GotoListItems(listid, parent, selectedids) {
            var w = window.open('DM_Catalog_listitems.aspx?id=' + listid + '&parent=' + parent + '&selectedids=' + selectedids + '&rnd_str=' + Math.random() + '', '_self', 'width=600,height=600');
            w.focus();
        }

        function validateNumber(evt, txtudccode) {
            var e = evt || window.event;
            var key = e.keyCode || e.which;

            if (!e.shiftKey && !e.altKey && !e.ctrlKey &&
                // numbers   
                key >= 48 && key <= 57 ||
                // Numeric keypad
                key >= 96 && key <= 105 ||
                // Backspace and Tab and Enter
                key == 8 || key == 9 || key == 13 ||
                // Home and End
                key == 35 || key == 36 ||
                // left and right arrows
                key == 37 || key == 39 ||
                // Del and Ins
                key == 46 || key == 45) {
                // input is VALID
                txtudccode.style.borderColor = "Black";
            }
            else {
                // input is INVALID
                txtudccode.style.borderColor = "Red";
                e.returnValue = false;
                if (e.preventDefault) e.preventDefault();
            }
        }
        function SearchRelatedItem(listid, field) {
            var url = "../UserControls/DM_Catalog_Search.aspx?LIST_ID=" + listid + "&PROP_ID=0&DM_OBJECT_ID=0&multiselect=n&field=" + field;
            var w = window.open(url, 'WordList', 'height=600,width=800,scrollbars=yes,resizable=yes,status=yes');
            w.focus();
        }
    </script>
</head>
<body>
    <form id="Form1" method="post" runat="server" defaultbutton="imgbtnSaveAndClose">
        <asp:ScriptManager ID="sc1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/ScriptServices/WSManagedLists1.asmx" />
            </Services>
        </asp:ScriptManager>
        <asp:HiddenField ID="selectedids" runat="server" />
        <table style="margin: 1%;">

            <tr class="DetailHeader">
                <td class="DetailHeaderContent">
                    <a id="imgbtnnew" class="icon icon-add-new" href="#" runat="server" onclick="AddNewItem();"></a>
                    <asp:LinkButton ID="imgbtnSave" runat="server" OnClick="DoSave" />
                    <asp:LinkButton ID="imgbtnSaveAndClose" runat="server" OnClick="DoSaveClose" />
                </td>
                <td class="DetailHeaderContent" style="text-align:right;">
                    <a id="imgbtnBack" href="#" runat="server" visible="false" onserverclick="BacktoListItems">
                        <span class="icon icon-cancel"></span>
                    </a>
                </td>
            </tr>

            <tr>
                <td colspan="2">
                    <asp:Table runat="server">
                        <asp:TableRow>
                            <asp:TableCell ColumnSpan="4">
                                <asp:Panel ID="MsgPanel" runat="server" Width="443px" Visible="False" Style="margin-top: 10px; margin-bottom: 10px;">
                                    <asp:Label ID="msg_lbl" runat="server" CssClass="ErrorLabel"></asp:Label>
                                </asp:Panel>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell Width="200px">
                                <asp:Label ID="lblList1" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell ColumnSpan="3" Width="600px">
                                <asp:Label ID="lblList" runat="server"></asp:Label>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowCode">
                            <asp:TableCell>
                                <asp:Label ID="lblCode" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell ColumnSpan="3">
                                <asp:TextBox ID="txtCode" runat="server" MaxLength="30">                                
                                </asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowUdcCode" Visible="false">
                            <asp:TableCell>
                                <asp:Label ID="lblUdcCode" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell ColumnSpan="3" VerticalAlign="Top">
                                <asp:Label ID="lblUdcParentCode" runat="server" />&nbsp;
                            <asp:TextBox ID="txtUdcChildCode" runat="server" onkeydown="validateNumber(event,this);" MaxLength="30">
                            </asp:TextBox>&nbsp;
                            <asp:TextBox ID="txtUdcCodeExtension" runat="server" MaxLength="30">                               
                            </asp:TextBox>&nbsp;
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowPath" Visible="false">
                            <asp:TableCell>
                                <asp:Label ID="lblPath1" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="Left" ColumnSpan="3">
                                <asp:Label ID="lblPath" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowNativeDesc">
                            <asp:TableCell>
                                <asp:Label ID="lblDesc" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="Left" ColumnSpan="3">
                                <asp:TextBox runat="server" ID="txtDesc" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowNativeHomo">
                            <asp:TableCell>
                                <asp:Label ID="lblDescHomo" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="Left" ColumnSpan="3">
                                <asp:TextBox runat="server" ID="txtDescHomo" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowNativeScopeNote">
                            <asp:TableCell>
                                <asp:Label ID="lblScopeNote" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="Left" ColumnSpan="3">
                                <asp:TextBox runat="server" ID="txtScopeNote" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ColumnSpan="4">&nbsp;</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow Visible="false" ID="rowLangTrans">
                            <asp:TableCell>
                                <asp:Label ID="lblLanguages" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:Label ID="lblheaderDescription" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:Label ID="lblHeaderHomonym" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:Label ID="lblHeaderScopeNote" class="Label" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowGerman" Visible="false">
                            <asp:TableCell HorizontalAlign="left">
                                &nbsp;&nbsp;<asp:Label ID="lblGerman" class="LabelNotBold" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox runat="server" ID="txtGerman" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="left">
                                <asp:TextBox runat="server" ID="txtGermanHomo" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="left">
                                <asp:TextBox runat="server" ID="txtGermanScopeNote" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowEnglish" Visible="false">
                            <asp:TableCell HorizontalAlign="left">
                                &nbsp;&nbsp;<asp:Label ID="lblEnglish" class="LabelNotBold" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox runat="server" ID="txtEnglish" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox runat="server" ID="txtEnglishHomo" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox runat="server" ID="txtEnglishScopeNote" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowFrench" Visible="false">
                            <asp:TableCell HorizontalAlign="left">
                                &nbsp;&nbsp;<asp:Label ID="lblFrench" class="LabelNotBold" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox runat="server" ID="txtFrench" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox runat="server" ID="txtFrenchHomo" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox runat="server" ID="txtFrenchScopeNote" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowDutch" Visible="false">
                            <asp:TableCell HorizontalAlign="left">
                                &nbsp;&nbsp;<asp:Label ID="lblDutch" class="LabelNotBold" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox runat="server" ID="txtDutch" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox runat="server" ID="txtDutchHomo" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:TextBox runat="server" ID="txtDutchScopeNote" MaxLength="200"></asp:TextBox>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ColumnSpan="4">&nbsp;</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow Visible="false" ID="rowSynonyms">
                            <asp:TableCell HorizontalAlign="left" VerticalAlign="Top">
                                <asp:Label ID="lblSynonyms" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="left" ColumnSpan="3">
                                <asp:GridView ID="synGridView" runat="server" Width="300px" AutoGenerateColumns="False"
                                    ShowHeader="true" AllowPaging="True" AllowSorting="True">
                                    <PagerStyle ForeColor="Blue" HorizontalAlign="Left" BackColor="White"></PagerStyle>
                                </asp:GridView>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ColumnSpan="4">&nbsp;</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow Visible="false" ID="rowCategoryDependantList">
                            <asp:TableCell HorizontalAlign="left" VerticalAlign="Top">
                                <asp:Label ID="lblCategoryDependant" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="left" ColumnSpan="3">
                                <div style="width: 300px;">
                                    <asp:DropDownList ID="cmbCat" runat="server" Width="200px">
                                    </asp:DropDownList>
                                    <asp:LinkButton ID="lnkbtnAdd" runat="server" OnClick="AddCategory" class="ButtonLink"
                                        Style="float: right; margin-top: 5px;" />
                                </div>
                                <asp:GridView ID="catGridView" runat="server" Width="300px" AutoGenerateColumns="False"
                                    AllowPaging="True" AllowSorting="True">
                                    <PagerStyle ForeColor="Blue" HorizontalAlign="Left" BackColor="White"></PagerStyle>
                                </asp:GridView>
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow Visible="false" ID="rowRelationships">
                            <asp:TableCell HorizontalAlign="left" VerticalAlign="Top">
                                <asp:Label ID="lblRelationships" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="left" VerticalAlign="Top" ColumnSpan="3">
                                <asp:GridView ID="TableGridViewRels" runat="server" OnRowDeleting="TableGridViewRels_RowDeleting" AutoGenerateColumns="False" AllowPaging="False"
                                    Width="520px" AllowSorting="True" ShowHeader="true">
                                    <PagerStyle ForeColor="Blue" HorizontalAlign="Left" BackColor="White"></PagerStyle>
                                </asp:GridView>
                                <br />
                                <asp:Table ID="tblRelBuilder" runat="server" CellPadding="0" CellSpacing="0" Visible="false">
                                    <asp:TableRow>
                                        <asp:TableCell ID="celllblRel1" runat="server" HorizontalAlign="Left" VerticalAlign="Top"
                                            Width="200px">
                                            <asp:Label ID="lblRel1" runat="server"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellcmbRel1" runat="server" HorizontalAlign="center" VerticalAlign="Top"
                                            Width="100px">
                                            <asp:DropDownList ID="cmbRel1" runat="server" Width="90%">
                                            </asp:DropDownList>
                                        </asp:TableCell>
                                        <asp:TableCell ID="celltxtRel2" runat="server" HorizontalAlign="Left" VerticalAlign="Top"
                                            Width="205px">
                                            <asp:TextBox ID="txtRel2" Style="vertical-align: top; width: 83%" runat="server" MaxLength="200"></asp:TextBox>
                                            <b>
                                                <asp:HyperLink ID="cmdSel1" runat="server" Text="Select" Style="vertical-align: top;"></asp:HyperLink></b>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellcmd1" runat="server" HorizontalAlign="Left">
                                            <b>
                                                <asp:LinkButton ID="cmdRel1" runat="server" Text="Add"></asp:LinkButton></b>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                    <asp:TableRow>
                                        <asp:TableCell ID="celltxtRel1" runat="server" HorizontalAlign="Left" VerticalAlign="Top">
                                            <asp:TextBox ID="txtRel1" Style="vertical-align: top; width: 83%" runat="server" MaxLength="200"></asp:TextBox>
                                            <b>
                                                <asp:HyperLink ID="cmdSel2" runat="server" Style="vertical-align: top;" Text="Select"></asp:HyperLink></b>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellcmbRel2" runat="server" HorizontalAlign="center" VerticalAlign="Top">
                                            <asp:DropDownList ID="cmbRel2" runat="server" Width="90%">
                                            </asp:DropDownList>
                                        </asp:TableCell>
                                        <asp:TableCell ID="celllblRel2" runat="server" HorizontalAlign="Left" VerticalAlign="Top">
                                            <asp:Label ID="lblRel2" runat="server"></asp:Label>
                                        </asp:TableCell>
                                        <asp:TableCell ID="cellcmd2" runat="server" HorizontalAlign="Left" VerticalAlign="Top">
                                            <b>
                                                <asp:LinkButton ID="cmdRel2" runat="server" Text="Add"></asp:LinkButton></b>
                                        </asp:TableCell>
                                    </asp:TableRow>
                                </asp:Table>
                                <br />
                                <br />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableCell ColumnSpan="4">&nbsp;</asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow ID="rowStatus">
                            <asp:TableCell>
                                <asp:Label ID="lblStatus" class="Label" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell HorizontalAlign="Left" ColumnSpan="3">
                                <ArcoControls:CheckBox ID="cbItemStatus" runat="server" Checked="true" />
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </td>

            </tr>
        </table>
    </form>
    <asp:Literal ID="plValidationScript" runat="server"></asp:Literal>
</body>
</html>
