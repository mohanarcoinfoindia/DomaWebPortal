<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AdvUserLookup.aspx.vb" Inherits="UserControls_AdvUserLookup" MasterPageFile="~/masterpages/Toolwindow.master" %>

<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">

    <script type="text/javascript">
        function RefreshOpener() {
            GetOpenerWindow().RefreshOnChange();
        }

        function SaveClose() {
            RefreshOpener();
            Close();
        }

        function GetOpenerWindow() {
            if (MainPage().GetContentWindow) {
                if (MainPage().GetContentWindow().AddValue<%=JSIdentifier %>) {
                    return MainPage().GetContentWindow();
                }
                else {
                    if (MainPage().GetPreviewWindow) {
                        return MainPage().GetPreviewWindow();
                    }
                    else {
                        return MainPage();
                    }
                }
            }
            else {

                return MainPage();
            }
        }

        function AutoSelect(what, caption) {

            GetOpenerWindow().AddValue<%=JSIdentifier %>(what, caption);

            <%if RefreshOnChange then%>
            SaveClose();
            <%else%>
            Close();
            <%end if%>
        }

        function Select(what, caption) {

            GetOpenerWindow().AddValue<%=JSIdentifier %>(what, caption);

            UpdateYourSelection();
	        <%if not MultiSelect then%>

                <%if RefreshOnChange then%>
            SaveClose();
                <%else%>
            Close();
                <%end if%>
            <%end if %>
        }

        function Clear() {
            GetOpenerWindow().ClearValue<%=JSIdentifier %>();
            UpdateYourSelection();
            <%if not MultiSelect then%>

                <%if RefreshOnChange then%>
            SaveClose();
                <%else%>
            Close();
                <%end if%>
            <%end if%>

        }

        function UpdateYourSelection() {
            $get('lblSelection').innerHTML = "";
            var yoursel = GetOpenerWindow().oProp_<%=JSIdentifier %>.GetCaption().replace(/\<br\/\>/g, "\r\n");
            $get('lblSelection').appendChild(document.createTextNode(yoursel));

        }
    </script>

    <style>
        .toolWindowPopup {
            overflow-x: hidden;
        }

        .checkboxDoma {
            margin: 6px 2px 0px 2px;
        }

    </style>

    <asp:Table ID="Table1" runat="server" CssClass="DetailTable PaddedTable">
        <asp:TableRow>
            <asp:TableCell>
                <asp:Table ID="tblFilter" runat="server">
                    <asp:TableRow>
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
                            <asp:TextBox ID="txtFilter" runat="server"></asp:TextBox>&nbsp;
								<asp:LinkButton ID="lnkFilter" runat="server" />
                        </asp:TableCell>
                        <asp:TableCell ColumnSpan="3">&nbsp;
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2"><hr/></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell ColumnSpan="2">
                <table Class='Panel'>
                    <colgroup>
                        <col style="width:100px;"/><col/>
                    </colgroup>
                    <tr>
                        <td style="vertical-align:top;white-space:nowrap;">
                            <b><%=GetLabel("yourselection") %> : </b>
                        </td>
                        <td style="width:100%;vertical-align:top;">
                            <span id="lblSelection" title="lblSelection"></span>
                        </td>
                        <td style="vertical-align:top;">
                            <%If MultiSelect Then%>
                            <a href='javascript:Clear();' class='ButtonLink'><%=GetLabel("clear")%></a>
                            <%end if %>
                        </td>
                    </tr>
                </table>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
            <asp:TableCell VerticalAlign="Top">
                    <asp:Table ID="SubjectList" runat="server" Width="100%" CellPadding="0" CellSpacing="0" EnableViewState="False">
                        <asp:TableRow CssClass="ListHeader">
                            <asp:TableCell></asp:TableCell>
                            <asp:TableCell>
                                <asp:Label ID="lblHeader1" runat="server"></asp:Label>
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:Label ID="lblHeader2" runat="server"></asp:Label>
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
            </asp:TableCell>

        </asp:TableRow>


    </asp:Table>

</asp:Content>
