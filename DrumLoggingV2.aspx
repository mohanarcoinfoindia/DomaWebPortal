<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DrumLoggingV2.aspx.vb" Inherits="DrumLoggingV2" ValidateRequest="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>DRUM Logging</title>

    <script type="text/javascript">
        function ToggleRow(r) {
            var row = r.parentNode.children[1];
            if (row) {
                if (row.style.display == '') {
                    row.style.display = 'none';
                    r.src = './Images/Expand.gif';

                }
                else {
                    row.style.display = '';
                    r.src = './Images/Collapse.gif';

                }
            }
            row = null;
        }

        function Goto(page) {
            if (((parseInt(page) <= LastPage) && (parseInt(page) > 0)) || (page == 1)) {
                document.forms[0].CurrentPage.value = page;
                Reload();
            }
        }

        function OrderBy(field) {
            if (field == document.forms[0].elements[orderbyField].value) {
                if (document.forms[0].elements[orderbyorderField].value == "DESC") {
                    document.forms[0].elements[orderbyorderField].value = "ASC";
                }
                else {
                    document.forms[0].elements[orderbyorderField].value = "DESC";
                }
            }
            else {
                document.forms[0].elements[orderbyorderField].value = "ASC";
            }
            document.forms[0].elements[orderbyField].value = field;
            Reload();
        }

        function Reload() {
            document.forms[0].submit();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <script type="text/javascript">
            var LastPage = <%=LastPage%>;
            var orderbyField = '<%=orderby.name %>';
            var orderbyorderField = '<%=orderbyorder.name %>';
        </script>

        <asp:Panel ID="pnlFilter" runat="server" DefaultButton="btnSearch">
            <div class="container-fluid detail-form-container">
                <div class="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="Label1" runat="server" Text="InputStream"></asp:Label>
                    </div>
                    <div class="col-md-8 FieldCell">
                        <asp:DropDownList ID="cmbIS" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblBatch" runat="server" Text="Batch job"></asp:Label>
                    </div>
                    <div class="col-md-8 FieldCell">
                        <asp:DropDownList ID="cmbBatch" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
                    </div>
                    <div class="col-md-8 FieldCell">
                        <asp:DropDownList ID="cmbStatus" runat="server" AutoPostBack="true">
                            <asp:ListItem Value="9">All</asp:ListItem>
                            <asp:ListItem Value="0">No validation needed</asp:ListItem>
                            <asp:ListItem Value="1">To be validated</asp:ListItem>
                            <asp:ListItem Value="2">Validated</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblTimestamp" runat="server" Text="Date"></asp:Label>
                    </div>
                    <div class="col-md-8 FieldCell">
                        <asp:TextBox ID="txtTimeStampFilter" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div class="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblSourceFilter" runat="server" Text="Source"></asp:Label>
                    </div>
                    <div class="col-md-8 FieldCell">
                        <asp:TextBox ID="txtSourceFilter" runat="server"></asp:TextBox>
                    </div>
                </div>
                <div class="row detail-form-row">
                    <div class="col-md-8 offset-4 FieldCell">
                        <Arco:ButtonPanel runat="server">
                            <Arco:OkButton runat="server" ID="btnSearch" Text="Filter" />
                        </Arco:ButtonPanel>
                    </div>
                </div>
            </div>

        </asp:Panel>
        <asp:HiddenField ID="LogID" runat="server" Visible="False" />
        <input type="hidden" id="CurrentPage" name="CurrentPage" runat="server" />
        <input type="hidden" name="orderby" id="orderby" runat="server" />
        <input type="hidden" name="orderbyorder" id="orderbyorder" runat="server" />

        <asp:GridView ID="grdResultList" runat="server" AutoGenerateColumns="False" DataKeyNames="GUID">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <img alt="" style="cursor: pointer" src="Images/Expand.gif" onclick="javascript:ToggleRow(this);" />
                        <asp:Panel ID="pnlLines" runat="server" Style="display: none; margin-left: 50px">
                            <asp:GridView ID="gdDetails" runat="server" AutoGenerateColumns="false" Width="100%">
                                <Columns>
                                    <asp:BoundField ItemStyle-Width="50px" DataField="Status" HeaderText="Status" />
                                    <asp:TemplateField ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                    </ItemTemplate>
                </asp:TemplateField>


                <asp:BoundField DataField="Status" ItemStyle-Wrap="false" />
                <asp:BoundField DataField="LogTime" HtmlEncode="false" HeaderText="Time" ItemStyle-Wrap="false" />


                <asp:BoundField DataField="Item_Identifier" HtmlEncode="false" ItemStyle-Width="100%" />
                <asp:BoundField DataField="BatchID" HtmlEncode="false" />
                <asp:BoundField DataField="Message" ItemStyle-Wrap="false" />
                <asp:BoundField DataField="ValidationStatus" Visible="true" />
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="false" CommandArgument='<%# DataBinder.Eval(Container,"DataItem.GUID") %>'
                            CommandName="Validate" Text="Validate"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <asp:Panel ID="pnlScroll" runat="server">

            <asp:Table CssClass="List" runat="server">
                <asp:TableRow>
                    <asp:TableCell CssClass="ListFooter" HorizontalAlign="Center">
                        <%If NumberOfResults > 0 Then%>

                            <asp:HyperLink runat="server" ID="lnkPrev" SkinID="PrevPage"></asp:HyperLink>
                            <asp:Literal ID="litScroller" runat="server" />
                            <asp:HyperLink runat="server" ID="lnkNext" SkinID="NextPage"></asp:HyperLink>

                            <%If NumberOfResults > RecordsPerPage Then%>
		                     &nbsp;&nbsp;<%="(" & NumberOfResultsLabel & " " & GetLabel("resultsfound") & ")"%>
                            <%end If %>
                        <%else %>

                        <%= GetLabel("noresultsfound") %>

                        <%end if%>
                    </asp:TableCell>
                </asp:TableRow>
            </asp:Table>

        </asp:Panel>

        <asp:Panel ID="pnlData" runat="server" Height="50px" Width="656px" GroupingText="Log Data" Visible="False">
            <table border="0">
                <tr>
                    <td style="width: 344px">
                        <asp:Label ID="lblID" runat="server" Text="ID"></asp:Label></td>
                    <td style="width: 779px">
                        <asp:Label ID="lblIDValue" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 344px">
                        <asp:Label ID="lblTime" runat="server" Text="Time"></asp:Label></td>
                    <td style="width: 779px">
                        <asp:Label ID="lblTimeValue" runat="server"></asp:Label></td>
                </tr>

                <tr>
                    <td style="width: 344px">
                        <asp:Label ID="lblMachine" runat="server" Text="Machine"></asp:Label></td>
                    <td style="width: 779px">
                        <asp:Label ID="lblMachineValue" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 344px">
                        <asp:Label ID="lblUser" runat="server" Text="User"></asp:Label></td>
                    <td style="width: 779px">
                        <asp:Label ID="lblUserValue" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 344px">
                        <asp:Label ID="lblBatchID" runat="server" Text="Batch ID"></asp:Label></td>
                    <td style="width: 779px">
                        <asp:Label ID="lblBatchIDValue" runat="server"></asp:Label></td>
                </tr>

                <tr>
                    <td style="width: 344px">
                        <asp:Label ID="lblIdentifier" runat="server" Text="Identifier"></asp:Label></td>
                    <td style="width: 779px">
                        <asp:Label ID="lblIdentifierValue" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 344px;">
                        <asp:Label ID="lblErrCode2" runat="server" Text="Error code"></asp:Label></td>
                    <td style="width: 779px;">
                        <asp:Label ID="lblErrCodeValue" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 344px; height: 20px;">
                        <asp:Label ID="lblErrMessage" runat="server" Text="Error message"></asp:Label></td>
                    <td style="width: 779px; height: 20px;">
                        <asp:Label ID="lblErrMessageValue" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 344px">
                        <asp:Label ID="lblStatus2" runat="server" Text="Validation Status"></asp:Label></td>
                    <td style="width: 779px">
                        <asp:Label ID="lblStatusValue" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 344px;">
                        <asp:Label ID="lblValidatedOn" runat="server" Text="Validated on"></asp:Label></td>
                    <td style="width: 779px;">
                        <asp:Label ID="lblValidatedOnValue" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 344px">
                        <asp:Label ID="lblValidatedBy" runat="server" Text="Validated by"></asp:Label></td>
                    <td style="width: 779px">
                        <asp:Label ID="lblValidatedByValue" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td style="width: 344px">
                        <asp:Label ID="lblComment" runat="server" Text="Validation Comment"></asp:Label></td>
                    <td style="width: 779px">
                        <asp:TextBox ID="txtComment" runat="server" Width="320px" MaxLength="500"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 344px">
                        <asp:Label ID="lblGUID" runat="server" Text="GUID"></asp:Label>
                    </td>
                    <td style="width: 779px">
                        <asp:Label ID="lblGUIDValue" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <asp:Button ID="cmdList" runat="server" Text="Return to list" />
            <asp:Button ID="cmdValidate" runat="server" Text="Validate" />
        </asp:Panel>
        <asp:Panel ID="pnlActions" runat="server" Height="1px" Width="232px">
            <asp:Button ID="cmdExport" runat="server" Text="Archive all" Visible="False" />
            <asp:Button ID="cmdValidateAll" runat="server" Text="Validate all" />
        </asp:Panel>
    </form>
</body>
</html>
