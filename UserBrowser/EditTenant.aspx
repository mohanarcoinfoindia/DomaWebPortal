<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EditTenant.aspx.vb" Inherits="UserBrowser_EditTenant" MasterPageFile="~/masterpages/Base.master" ValidateRequest="false" %>

<asp:Content ID="Header1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <script type="text/javascript">
        function DoClose() {
            location.href = 'Tenants.aspx';
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <telerik:RadScriptManager runat="server" ID="scriptManager" />

    <div class="container-fluid detail-form-container">
        <asp:Label ID="lblMsgText" runat="server" Visible="false" />

        <div class="row detail-form-row" id="rowError" visible="false" runat="server">
            <div class="col ReadOnlyFieldCell">
                <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" Text="test"></asp:Label>
            </div>
        </div>

        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="Label1" runat="server" CssClass="Label" Text="ID"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:Label ID="lblId" runat="server" CssClass="Label"></asp:Label>
            </div>
        </div>

        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblName" runat="server" CssClass="Label" AssociatedControlID="txtName"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox MaxLength="200" runat="server" ID="txtName" />
                <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ID="reqName" ControlToValidate="txtName" ErrorMessage="*" SetFocusOnError="true" />
            </div>
        </div>

        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblDesc" runat="server" CssClass="Label" AssociatedControlID="txtDesc" />
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox MaxLength="2000" runat="server" SkinID="MultiLine" ID="txtDesc" TextMode="MultiLine" />
            </div>
        </div>

        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblStatus" runat="server" CssClass="Label" AssociatedControlID="rdStatus" />
            </div>
            <div class="col-md-8 FieldCell">
                <asp:RadioButtonList ID="rdStatus" runat="server" RepeatColumns="2">
                    <asp:ListItem Value="1" Text="Enabled" />
                    <asp:ListItem Value="0" Text="Disabled" />
                </asp:RadioButtonList>
            </div>
        </div>

        <div class="row detail-form-row">
            <div class="col-md-8 offset-md-4 FieldCell">
                <Arco:ButtonPanel ID="ButtonPanel1" runat="server">
                    <Arco:OkButton runat="server" ID="btnOk"></Arco:OkButton>
                    <Arco:CancelButton OnClientClick="javascript:DoClose();return false;" runat="server" ID="btnCancelUrl"></Arco:CancelButton>
                </Arco:ButtonPanel>
            </div>
        </div>

        <%-- Tabs --%>
        <div class="row detail-form-row mt-3">
            <telerik:RadTabStrip runat="server" ID="radTab" SelectedIndex="0" MultiPageID="RadMultiPage1">
                <Tabs>
                    <telerik:RadTab Text="Details" />
                    <telerik:RadTab Text="Audit" />
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage CssClass="groupingRow" ID="RadMultiPage1" runat="server" SelectedIndex="0" ScrollBars="Auto">
                <telerik:RadPageView CssClass="RowCell GroupingDiv" runat="server" ID="PVDetails" BackColor="White">
                    <div class="container-fluid detail-form-container" id="tblDetails" runat="server">
                        <div class="row detail-form-row">
                            <div class="col-md-4 LabelCell">
                                <asp:Label ID="lblSeeGlobal" runat="server" CssClass="Label" AssociatedControlID="rdSeeGlobal" />
                            </div>
                            <div class="col-md-8 FieldCell">
                                <asp:RadioButtonList ID="rdSeeGlobal" runat="server" RepeatColumns="2">
                                    <asp:ListItem Value="1" Text="True" />
                                    <asp:ListItem Value="0" Text="False" />
                                </asp:RadioButtonList>
                            </div>
                        </div>

                        <div class="row detail-form-row">
                            <div class="col-md-4 LabelCell">
                                <asp:Label ID="lblUrl" runat="server" CssClass="Label" AssociatedControlID="txtUrl" />
                            </div>
                            <div class="col-md-8 FieldCell">
                                <asp:TextBox MaxLength="200" runat="server" ID="txtUrl" />
                            </div>
                        </div>

                        <div class="row detail-form-row">
                            <div class="col-md-4 LabelCell">
                                <asp:Label ID="lblFolder" runat="server" CssClass="Label" Text="Root Folder" />
                            </div>
                            <div class="col-md-8 FieldCell">
                                <asp:TextBox ID="txtFolder" runat="server" />
                            </div>
                        </div>

                        <div class="row detail-form-row">
                            <div class="col-md-4 LabelCell">
                                <asp:Label ID="lblLang" runat="server" CssClass="Label" AssociatedControlID="drpLang" Text="Theme" />
                            </div>
                            <div class="col-md-8 FieldCell">
                                <asp:DropDownList runat="server" ID="drpLang" EnableViewState="true"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="row detail-form-row">
                            <div class="col-md-4 LabelCell">
                                <asp:Label ID="lblTheme" runat="server" CssClass="Label" AssociatedControlID="drpTheme" Text="Theme"></asp:Label>
                            </div>
                            <div class="col-md-8 FieldCell">
                                <asp:DropDownList runat="server" ID="drpTheme" EnableViewState="true"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="row detail-form-row">
                            <div class="col-md-4 LabelCell">
                                <asp:Label ID="lblEmail" runat="server" CssClass="Label" AssociatedControlID="txtEmailSender" Text="Email"></asp:Label>
                            </div>
                            <div class="col-md-8 FieldCell">
                                <asp:TextBox ID="txtEmailSender" runat="server" />
                            </div>
                        </div>

                        <div class="row detail-form-row">
                            <div class="col-md-4 LabelCell">
                                <asp:Label ID="lblEmailDisplay" runat="server" CssClass="Label" AssociatedControlID="txtSenderDisplay" Text="Email Display"></asp:Label>
                            </div>
                            <div class="col-md-8 FieldCell">
                                <asp:TextBox ID="txtSenderDisplay" runat="server" />
                            </div>
                        </div>
                    </div>
                </telerik:RadPageView>

                <telerik:RadPageView runat="server" ID="pvAudit" BackColor="White">
                    <table class="SubList">
                        <asp:Repeater ID="repAudit" runat="server">
                            <HeaderTemplate>
                                <tr>
                                    <th><%=GetLabel("user")%></th>
                                    <th><%=GetLabel("date")%></th>
                                    <th><%=GetLabel("description")%></th>
                                </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td><%#DataBinder.Eval(Container.DataItem, "USER")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "DATE")%></td>
                                    <td><%#DataBinder.Eval(Container.DataItem, "DESCRIPTION")%></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr runat="server" id="trNoAuditFound" visible="false" class="ListFooter">
                            <td colspan="3">
                                <asp:Label runat="server" ID="lblNoAuditFound" />
                            </td>
                        </tr>
                    </table>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </div>
    </div>
</asp:Content>

