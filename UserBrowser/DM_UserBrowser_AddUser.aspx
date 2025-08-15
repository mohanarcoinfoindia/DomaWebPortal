<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_UserBrowser_AddUser.aspx.vb" Inherits="DM_UserBrowser_AddUser" ValidateRequest="false" Strict="false" MasterPageFile="~/MasterPages/base.master" %>

<%@ Register TagPrefix="Arco" TagName="Delegates" Src="Delegates.ascx" %>
<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" TagPrefix="ArcoControls" %>

<asp:Content ID="Header1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <style>
        .RadWindow.RadWindow_Default {
            border-style: none !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <script type="text/javascript">
        function checkAll(togglechk, list) {
            var checkboxCollection = $get(list).getElementsByTagName('input');

            for (var i = 0; i < checkboxCollection.length; i++) {
                if (checkboxCollection[i].type.toString().toLowerCase() == "checkbox") {
                    checkboxCollection[i].checked = togglechk.checked;
                }
            }
        }

        function EditRoles() {
            const width = window.innerWidth - 40;
            const height = window.innerHeight - 40;
            var oWnd = radopen("DM_UserBrowserMembers.aspx?modal=y&url=rolesofuser&roleid=<%=Arco.Doma.WebControls.EncodingUtils.EncodeJsString(Account,False) %>", "Roles", width, height);
            oWnd.center();
            oWnd.add_close(Refresh);
        }

        function EditSubscription(id) {
            var oWnd2 = PC().OpenModalWindowRelativeSize("../Subscription.aspx?modal=Y&ID=" + id + "&fromup=Y", false);
        }

        function DeleteSubscription(id) {
            if (ConfirmDelete()) {
                $get("<%=hndDelSubs.ClientID %>").value = id;
                Refresh();
            }
        }

        function ConfirmDeleteUser() {
            return confirm(UIMessages[29].replace("#ITEM#", <%=Arco.Doma.WebControls.EncodingUtils.EncodeJsString(txtLoginName.Text)%>));
        }

        function ConfirmDelete() {
            return confirm(UIMessages[4]);
        }

        function ConfirmDisconnect() {
            return confirm(<%=Arco.Doma.WebControls.EncodingUtils.EncodeJsString(GetDecodedLabel("confirmdisconnectoauthaccount"))%>);
        }
      

    </script>

    <telerik:RadWindowManager ID="RadWindowManager1" ShowContentDuringLoad="false" VisibleStatusbar="false" Modal="true" ReloadOnShow="true" runat="server">
        <Windows>
            <telerik:RadWindow runat="server" ID="Roles" ShowContentDuringLoad="false" VisibleStatusbar="false" Overlay="true" Behaviors="Close" />
            <telerik:RadWindow runat="server" ID="Groups" ShowContentDuringLoad="false" VisibleStatusbar="false" Overlay="true" Behaviors="Close" Width="700" Height="810" />
            <telerik:RadWindow runat="server" ID="Popup" ShowContentDuringLoad="false" VisibleStatusbar="false" Behaviors="Close" />
            <telerik:RadWindow runat="server" ID="Subs" ShowContentDuringLoad="false" VisibleStatusbar="false" Width="600" Height="600" Behaviors="Close" />
        </Windows>
    </telerik:RadWindowManager>

    <asp:HiddenField ID="hdnIsRefresh" runat="server" />

    <div class="detailView">
        <div class="container-fluid detail-form-container">
            <div class="row detail-form-row level0">
                <div class="col-12">
                    <asp:PlaceHolder ID="lblMessage" Visible="False" runat="server">
                        <asp:Label ID="lblMsgText" runat="server" CssClass="InfoLabel" />
                    </asp:PlaceHolder>
                </div>
            </div>

            <div class="row detail-form-row level0">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblLoginName" runat="server" CssClass="Label" AssociatedControlID="txtLoginName" />
                </div>
                <div class="col-md-8 FieldCell">
                    <asp:TextBox runat="server" ID="txtLoginName"></asp:TextBox>
                    <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ID="reqName" ControlToValidate="txtLoginName" ErrorMessage="<font color='red'> (*)</font>" SetFocusOnError="true" ></asp:RequiredFieldValidator>
                </div>
            </div>

            <div class="row detail-form-row level0">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblDisplayName" runat="server" CssClass="Label" AssociatedControlID="txtDisplayName" />
                </div>
                <div class="col-md-8 FieldCell">
                    <asp:TextBox runat="server" ID="txtDisplayName" MaxLength="100" />
                </div>
            </div>

            <div class="row detail-form-row level0">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblDescription" runat="server" CssClass="Label" AssociatedControlID="txtDescription" />
                </div>
                <div class="col-md-8 FieldCell">
                    <asp:TextBox TextMode="MultiLine" Rows="4" runat="server" ID="txtDescription" MaxLength="2000" />
                </div>
            </div>

            <asp:Panel runat="server" CssClass="row detail-form-row level0" ID="rAuthMethod">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblAuthentication" runat="server" CssClass="Label" />
                </div>
                <div class="col-md-8 FieldCell">
                    <telerik:RadComboBox runat="server" ID="drpAuthMethod" Width="205" />
                </div>
            </asp:Panel>

            <asp:Panel runat="server" CssClass="row detail-form-row level0" ID="rOldPassWord">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblOldPassword" runat="server" CssClass="Label" AssociatedControlID="txtOldPassword" />
                </div>
                <div class="col-md-8 FieldCell">
                    <ArcoControls:PasswordTextBox runat="server" ID="txtOldPassword" EnableViewState="true" />
                </div>
            </asp:Panel>

            <asp:Panel runat="server" CssClass="row detail-form-row level0" ID="rNewPassWord">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblPassword" runat="server" AssociatedControlID="txtNewPassWord" CssClass="Label" />
                    &nbsp;
                      <asp:Label runat="server" ID="lblPasswordTooltipImg" CssClass="TooltipImg">
                                    <svg width="22px" height="22px" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                                        <path d="M12 7.00999V7M12 17L12 10M21 12C21 16.9706 16.9706 21 12 21C7.02944 21 3 16.9706 3 12C3 7.02944 7.02944 3 12 3C16.9706 3 21 7.02944 21 12Z" stroke="#000000" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                                    </svg>
                                </asp:Label>
                </div>
                <div class="col-md-8 FieldCell">
                    <ArcoControls:PasswordTextBox runat="server" ID="txtNewPassWord" EnableViewState="true" CheckPasswordStrength="false" />
                    <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ID="RequiredFieldValidator1" ControlToValidate="txtNewPassWord" ErrorMessage="<font color='red'> (*)</font>" SetFocusOnError="true"></asp:RequiredFieldValidator>
                </div>
            </asp:Panel>

            <asp:Panel runat="server" CssClass="row detail-form-row level0" ID="rConfirmNewPassWord">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblConfirmPassword" runat="server" CssClass="Label" AssociatedControlID="txtConfirmNewPassword" />
                </div>
                <div class="col-md-8 FieldCell">
                    <ArcoControls:PasswordTextBox runat="server" ID="txtConfirmNewPassword" EnableViewState="true" />

                    <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ID="RequiredFieldValidator2" ControlToValidate="txtConfirmNewPassword" ErrorMessage="<font color='red'> (*)</font>" SetFocusOnError="true"></asp:RequiredFieldValidator>
                </div>
            </asp:Panel>

            <asp:Panel runat="server" CssClass="row detail-form-row level0" ID="trLockPass">
                <div class="col-md-4 LabelCell"></div>
                <div class="col-md-8 FieldCell d-block">

                    <ArcoControls:CheckBox ID="chkLockPass" runat="server" Text="Lock Password" /><br />
                    <ArcoControls:CheckBox ID="chkExpPass" runat="server" Text="Password expired" /><br />
                    <ArcoControls:CheckBox ID="chkNeverExpPass" runat="server" Text="Password never expires" />
                </div>
            </asp:Panel>

            <div class="row detail-form-row level0">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblLanguage" runat="server" CssClass="Label" AssociatedControlID="drpDwnLanguage" />
                </div>
                <div class="col-md-8 FieldCell">
                    <telerik:RadComboBox runat="server" ID="drpdwnLanguage" SkinID="NoWidth" />
                </div>
            </div>

            <div class="row detail-form-row level0">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblEmail" runat="server" CssClass="Label" AssociatedControlID="txtEmail" />
                </div>
                <div class="col-md-8 FieldCell">
                    <asp:TextBox runat="server" ID="txtEmail" AutoCompleteType="Email" />
                    <Arco:EmailValidator CssClass="FormField-Message Inline bad" ID="emailValidator" runat="server" SetFocusOnError="true" ErrorMessage="*" ControlToValidate="txtEmail" />
                </div>
            </div>

            <asp:Panel runat="server" CssClass="row detail-form-row level0" ID="rowTenant" Visible="false">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblTenant" runat="server" CssClass="Label" />:
                </div>
                <div class="col-md-8 FieldCell">
                    <asp:CheckBox ID="chkIsTenantUser" runat="server" />
                </div>
            </asp:Panel>

            <asp:Panel runat="server" CssClass="row detail-form-row level0" Visible="false" ID="trDomain">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblDomains" runat="server" CssClass="Label" AssociatedControlID="drpdwnDomains" />
                </div>
                <div class="col-md-8 FieldCell">
                    <telerik:RadComboBox runat="server" ID="drpdwnDomains" Width="205" />
                    <asp:Label runat="server" ID="lblDomainName" />
                </div>
            </asp:Panel>

            <asp:Panel runat="server" CssClass="row detail-form-row level0" ID="trStatus">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblStatus" runat="server" CssClass="Label" AssociatedControlID="drpdwnStatus" />
                </div>
                <div class="col-md-8 FieldCell">
                    <telerik:RadComboBox Width="200" runat="server" ID="drpdwnStatus" />
                </div>
            </asp:Panel>

            <div class="row detail-form-row mt-3">
                <telerik:RadTabStrip runat="server" ID="radTab" SelectedIndex="0" MultiPageID="RadMultiPage1">
                    <Tabs>
                        <telerik:RadTab Text="Details" />
                        <telerik:RadTab Text="Tenant(s)" />
                        <telerik:RadTab Text="Member Of" />
                        <telerik:RadTab Text="Preferences" />
                        <telerik:RadTab Text="Delegates" />
                        <telerik:RadTab Text="Notifications" />
                        <telerik:RadTab Text="Subscriptions" />
                        <telerik:RadTab Text="MFA" />
                        <telerik:RadTab Text="Open Auth Accounts" />
                        <telerik:RadTab Text="Licencing" />
                        <telerik:RadTab Text="Audit" />
                    </Tabs>
                </telerik:RadTabStrip>
                <telerik:RadMultiPage CssClass="groupingRow" ID="RadMultiPage1" runat="server" SelectedIndex="0" ScrollBars="Auto">
                    <telerik:RadPageView CssClass="RowCell GroupingDiv" runat="server" ID="PVDetails" BackColor="White">
                        <div class="detailView">
                            <div class="container-fluid detail-form-container">
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowFirstName">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblFirstName" runat="server" CssClass="Label" AssociatedControlID="txtFirstName" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtFirstName" MaxLength="50" AutoCompleteType="FirstName"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowLastName">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblLastName" runat="server" CssClass="Label" AssociatedControlID="txtLastName" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtLastName" MaxLength="100" AutoCompleteType="LastName"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowPhone">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblPhone" runat="server" CssClass="Label" AssociatedControlID="txtPhone" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtPhone" MaxLength="20" AutoCompleteType="BusinessPhone"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowFax">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblFax" runat="server" CssClass="Label" AssociatedControlID="txtFax" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtFax" MaxLength="20" AutoCompleteType="BusinessFax"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowCompany">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblCompany" runat="server" CssClass="Label" AssociatedControlID="txtCompany" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtCompany" MaxLength="100" AutoCompleteType="Company"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowAddress1">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblAdress1" runat="server" CssClass="Label" AssociatedControlID="txtAddress1" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtAddress1" MaxLength="200" AutoCompleteType="BusinessStreetAddress"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowAddress2">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblAdress2" runat="server" CssClass="Label" AssociatedControlID="txtAddress2" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtAddress2" MaxLength="200"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowCity">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblCity" runat="server" CssClass="Label" AssociatedControlID="txtCity" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtCity" MaxLength="50" AutoCompleteType="BusinessCity"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowZip">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblZip" runat="server" CssClass="Label" AssociatedControlID="txtZip" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtZip" MaxLength="20" AutoCompleteType="BusinessZipCode"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowState">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblState" runat="server" CssClass="Label" AssociatedControlID="txtState" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtState" MaxLength="50" AutoCompleteType="BusinessState"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowCountry">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblCountry" runat="server" CssClass="Label" AssociatedControlID="txtCountry" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtCountry" MaxLength="50" AutoCompleteType="BusinessCountryRegion"></asp:TextBox>
                                    </div>
                                </asp:Panel>
                                <asp:Panel runat="server" CssClass="row detail-form-row level1" ID="rowSpecialCode">
                                    <div class="col-md-4 LabelCell">
                                        <asp:Label ID="lblSpecialCode" runat="server" CssClass="Label" AssociatedControlID="txtSpecialCode" />
                                    </div>
                                    <div class="col-md-8 FieldCell">
                                        <asp:TextBox runat="server" ID="txtSpecialCode" MaxLength="100"></asp:TextBox>
                                    </div>
                                </asp:Panel>

                            </div>
                        </div>
                    </telerik:RadPageView>

                    <telerik:RadPageView ID="pvTenants" runat="server">
                        <table class="SubList">
                            <asp:Repeater ID="repTenants" runat="server">
                                <HeaderTemplate>
                                    <tr>
                                        <th><%#GetLabel("name")%></th>
                                    </tr>
                                </HeaderTemplate>

                                <ItemTemplate>
                                    <tr>

                                        <td><%#DataBinder.Eval(Container.DataItem, "Name")%></td>

                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr runat="server" id="trTenantFooter" visible="false" class="ListFooter">
                                <td align="center" runat="server" id="td1">
                                    <asp:Label runat="server" ID="lblNoTenantsFound" Text="No Tenants found" /></td>
                            </tr>
                        </table>
                    </telerik:RadPageView>

                    <telerik:RadPageView ID="pVMemberOf" runat="server">
                        <div style="margin: 10px">
                            <table class="SubList">
                                <asp:Repeater ID="repRoles" runat="server">
                                    <HeaderTemplate>
                                        <tr>
                                            <th style="width: 10px;"></th>
                                            <th><%#GetLabel("ub_rolename")%></th>
                                            <%If ShowRoleTenant Then%>
                                            <th><%=Getlabel("tenant") %></th>
                                            <%end if %>
                                        </tr>
                                    </HeaderTemplate>

                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <span class="<%#If(DataBinder.Eval(Container.DataItem, "TYPE").ToString() = "Role", "icon icon-user-role icon-color-light", "icon icon-user-group icon-color-light")%>"
                                                    title="<%#If(DataBinder.Eval(Container.DataItem, "TYPE").toString() = "Role",GetLabel("role"),GetLabel("group"))%>" />
                                            </td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "NAME")%></td>
                                            <%If ShowRoleTenant Then%>
                                            <td style="width: 50px;"><%#GetTenantLabel(CInt(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TenantId")))%></td>
                                            <%End If%>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <tr runat="server" id="trRoleFooter" visible="false" class="ListFooter">
                                    <td align="center" runat="server" id="tdNoRolesFound">
                                        <asp:Label runat="server" ID="lblNoRolesFound" /></td>
                                </tr>
                            </table>

                            <%If isAdmin() AndAlso Not FromUserProfile Then%>
                            <div class="SubListHeader3RightFillerDivSwitched">
                                <div style="width: 35px;" class="rounded-headerR">
                                    <div class="rounded-headerL">
                                        <div class="SubListMainHeaderT" id="SubListMainHeaderT_EditButton" runat="server">
                                            <span class="icon icon-edit" onclick="javascript:EditRoles();" title="<%=GetLabel("edit") %>" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <%end if %>


                            <asp:Repeater ID="repGroups" runat="server">
                                <HeaderTemplate>
                                    <tr>
                                        <table class="SubList">
                                            <th style="width: 10px;"></th>
                                            <th><%#GetLabel("ub_groupname") %></th>
                                    </tr>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <tr>
                                        <td>
                                            <span class="<%#If(DataBinder.Eval(Container.DataItem, "TYPE").ToString() = "Role", "icon icon-user-role icon-color-light", "icon icon-user-group icon-color-light")%>"
                                                title="<%#If(DataBinder.Eval(Container.DataItem, "TYPE").toString() = "Role",GetLabel("role"),GetLabel("group"))%>" />
                                        </td>
                                        <td><%#DataBinder.Eval(Container.DataItem, "NAME")%></td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </table>
                                </FooterTemplate>
                            </asp:Repeater>

                        </div>
                    </telerik:RadPageView>

                    <telerik:RadPageView ID="pvPreferences" runat="server" CssClass="overflow-hidden">
                        <div style="margin: 10px">
                            <table class="SubList">
                                <tr>
                                    <th colspan="2"><%=GetLabel("general")%></th>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div class="row detail-form-row level1">
                                            <div class="col-md-6 col-lg-4 my-2">
                                                <asp:Label ID="lblTheme" runat="server" Text="Theme"></asp:Label>&nbsp;
                                                        <asp:DropDownList runat="server" ID="drpTheme" EnableViewState="true"></asp:DropDownList>
                                            </div>
                                            <div class="col-md-6 col-lg-8 my-2">
                                                <asp:Label ID="lblDensity" runat="server" Text="Density"></asp:Label>&nbsp;
                                                        <asp:DropDownList runat="server" ID="drpDensity" EnableViewState="true"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row detail-form-row level1">
                                            <div class="col-md-6 col-lg-4">
                                                <ArcoControls:CheckBox runat="server" ID="chkShowTree" EnableViewState="true" />
                                            </div>
                                            <div class="col-md-6 col-lg-8">
                                                <ArcoControls:CheckBox runat="server" ID="chkSHOWFOLDERLINKICONS" Text="Show folder link icons" EnableViewState="true" />
                                            </div>
                                        </div>
                                        <div class="row detail-form-row level1">
                                            <div class="col-md-6 col-lg-4">
                                                <ArcoControls:CheckBox runat="server" ID="chkSHOWGLOBALSEARCH" Text="Global search" EnableViewState="true" />
                                            </div>
                                            <div class="col-md-6 col-lg-8">
                                                <ArcoControls:CheckBox runat="server" ID="chkSHOWSUCCESSMESSAGES" Text="Show success messages" EnableViewState="true" />
                                            </div>
                                        </div>
                                        <div class="row detail-form-row level1">
                                            <div class="col-md-6 col-lg-4">
                                             <ArcoControls:CheckBox runat="server" ID="chkEnableContextMenus" Text="Show Context menus"  />
                                                </div>
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <th colspan="2"><%=GetLabel("defaultresultscreen") %></th>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div class="row detail-form-row level1">
                                            <div class="col my-2">
                                                <asp:Label ID="lblRecsPerpage" runat="server"></asp:Label>&nbsp;
                                                        <asp:TextBox SkinID="CustomWidth" runat="server" ID="txtRecsperPage" Width="50" EnableViewState="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row detail-form-row level1">
                                            <div class="col-md-6 col-lg-4">
                                                <ArcoControls:CheckBox runat="server" ID="chkShowPreview" EnableViewState="true" />
                                            </div>
                                            <div class="col-md-6 col-lg-8">
                                                <ArcoControls:CheckBox runat="server" ID="chkSHOWGRIDSIDEBAR" EnableViewState="true" />
                                            </div>
                                        </div>
                                        <div class="row detail-form-row level1">
                                            <div class="col-md-6 col-lg-4">
                                                <ArcoControls:CheckBox runat="server" ID="chkShowFilters" EnableViewState="true" />
                                            </div>
                                            <div class="col-md-6 col-lg-8">
                                                <ArcoControls:CheckBox runat="server" ID="chkShowFoldersInList" EnableViewState="true" />
                                            </div>
                                        </div>
                                        <div class="row detail-form-row level1">
                                            <div class="col-md-6 col-lg-4">
                                                <ArcoControls:CheckBox runat="server" ID="chkShowCurrentFolder" EnableViewState="true" />
                                            </div>
                                            <div class="col-md-6 col-lg-8">
                                                <ArcoControls:CheckBox runat="server" ID="chkShowQuery" EnableViewState="true" />
                                            </div>
                                        </div>
                                        <div class="row detail-form-row level1">
                                            <div class="col-md-6 col-lg-4">
                                                <ArcoControls:CheckBox runat="server" ID="chkShowSearchInList" EnableViewState="true" />
                                            </div>
                                            <div class="col-md-6 col-lg-8">
                                                <ArcoControls:CheckBox runat="server" ID="chkALWAYSSHOWATTACHMENTS" EnableViewState="true" />
                                            </div>
                                        </div>
                                    </td>
                                </tr>

                                <tr>
                                    <th colspan="2"><%=GetLabel("details") %></th>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div class="row detail-form-row level1 mt-2">
                                            <div class="col-md-6 col-lg-4">
                                                <ArcoControls:CheckBox runat="server" ID="chkOPENDETAILWINDOWMAXIMIZED" EnableViewState="true" />
                                            </div>
                                            <div class="col-md-6 col-lg-8">
                                                <ArcoControls:CheckBox runat="server" ID="chkAUTOOPENFILEINDETAIL" EnableViewState="true" />
                                            </div>
                                        </div>
                                        <div class="row detail-form-row level1">
                                            <div class="col-md-6 col-lg-4">
                                                <ArcoControls:CheckBox runat="server" ID="chkSHOWFILESINSEPARATEWINDOW" EnableViewState="true" />
                                            </div>
                                            <div class="col-md-6 col-lg-8">
                                                <ArcoControls:CheckBox runat="server" ID="chkShowPreviewInDetailWindows" EnableViewState="true" />
                                            </div>
                                        </div>
                                         <div class="row detail-form-row level1">
                                            <div class="col-md-6 col-lg-4">
                                                  <ArcoControls:CheckBox runat="server" ID="chkShowFileToolbar" Text="Show file toolbar"  />
                                                </div>
                                        </div>
                                        <div class="row detail-form-row level1">
                                            <div class="col mb-2">
                                                <asp:Label ID="lblOnCloseWindow" runat="server"></asp:Label>&nbsp;
                                                        <asp:DropDownList runat="server" ID="drpOnCloseWindow" EnableViewState="true"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </td>
                                </tr>

                                <tr>
                                    <th colspan="2"><%=GetLabel("case")%></th>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <div class="row detail-form-row level1 mt-2">
                                            <div class="col-md-6 col-lg-4">
                                                <ArcoControls:CheckBox runat="server" ID="chkSHOWSUSPENDEDDOSSIERS" EnableViewState="true" />
                                            </div>
                                            <div class="col-md-6 col-lg-8">
                                                <ArcoControls:CheckBox runat="server" ID="chkSHOWLOCKEDDOSSIERS" EnableViewState="true" />
                                            </div>
                                        </div>
                                        <div class="row detail-form-row level1">
                                            <div class="col-md-6 col-lg-4">
                                                <ArcoControls:CheckBox runat="server" ID="chkSHOWDELEGATEDDOSSIERS" EnableViewState="true" />
                                            </div>
                                            <div class="col-md-6 col-lg-8">
                                                <ArcoControls:CheckBox runat="server" ID="chkOPENNEXTCASEONCLOSE" EnableViewState="true" />
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </telerik:RadPageView>

                    <telerik:RadPageView ID="pvDelegates" runat="server">
                        <Arco:Delegates ID="ctlDelegates" runat="server" />
                    </telerik:RadPageView>

                    <telerik:RadPageView ID="pvNotifications" runat="server">
                        <ArcoControls:CheckBox runat="server" ID="chkNotifications" Text="Enable Notifications" EnableViewState="true" Checked="true" />
                        <ArcoControls:CheckBox runat="server" ID="chkSelectAllNotifs" Text="Select All" /><br />
                        <ArcoControls:CheckBoxList runat="server" ID="chkFilteredNotifs" RepeatColumns="2" RepeatDirection="Horizontal"></ArcoControls:CheckBoxList>
                    </telerik:RadPageView>

                    <telerik:RadPageView ID="pvSubscriptions" runat="server">
                        <asp:HiddenField ID="hndDelSubs" runat="server" />
                        <div style="margin: 10px">
                            <table class="SubList">
                                <asp:Repeater ID="repSubscriptins" runat="server">
                                    <HeaderTemplate>
                                        <tr>
                                            <th></th>
                                            <th><%=GetLabel("subs_object")%></th>
                                            <th><%=GetLabel("subs_schedule")%></th>
                                            <th><%=GetLabel("subs_subject") %></th>
                                            <th></th>
                                            <th></th>
                                        </tr>
                                    </HeaderTemplate>

                                    <ItemTemplate>
                                        <tr>
                                            <td class="iconCell">
                                                <asp:HiddenField ID="hdnId" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
                                                <ArcoControls:CheckBox ID="chkSelected" runat="server" Checked='<%#DataBinder.Eval(Container.DataItem, "CHECKED")%>' />
                                                <asp:PlaceHolder ID='plEditSubs' runat='server' Visible='<%# DataBinder.Eval(Container.DataItem, "CANEDIT") %>'></asp:PlaceHolder>
                                            </td>
                                            <td>
                                                <img class="iconImage" src="<%#DataBinder.Eval(Container.DataItem, "ICON")%>" />
                                                <span><%#: DataBinder.Eval(Container.DataItem, "TEXT")%></span>
                                            </td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "WHEN")%></td>
                                            <td><%#DataBinder.Eval(Container.DataItem, "TO")%></td>
                                            <td class="iconCell">
                                                <span class="icon icon-edit" onclick="EditSubscription('<%#DataBinder.Eval(Container.DataItem, "ID")%>')" title="<%=GetLabel("edit")%>" />
                                            </td>
                                            <td class="iconCell">
                                                <span class="icon icon-delete icon-clickable" onclick="DeleteSubscription('<%#DataBinder.Eval(Container.DataItem, "ID")%>')" title="<%=GetLabel("delete")%>" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
                        </div>
                    </telerik:RadPageView>

                    <telerik:RadPageView ID="pvMFA" runat="server">
                        <div style="margin: 10px">
                            <table class="SubList">
                                <asp:Repeater ID="repMFA" runat="server">                                   
                                    <ItemTemplate>
                                        <tr>
                                            <td>
                                                <%#DataBinder.Eval(Container.DataItem, "Name")%>
                                            </td>
                                            <td>
                                                  <%# DataBinder.Eval(Container.DataItem, "Target")%>	
                                            </td>
                                             <td>
                                                <asp:PlaceHolder ID='plhEnabled' runat='server' Visible='<%# (Not FromUserProfile AndAlso DataBinder.Eval(Container.DataItem, "Enabled"))%>'>
                                                     <asp:LinkButton CssClass="ButtonLink" ID="lnkDisableMfa" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Name")%>' runat="server" OnClick="lnkDisableMfa_Click" Text='<%#GetDecodedLabel("disable")%>' />                                                  													     
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID='plhDisabled' runat='server' Visible='<%# (Not FromUserProfile AndAlso Not DataBinder.Eval(Container.DataItem, "Enabled"))%>'>       
                                                    <asp:LinkButton CssClass="ButtonLink" ID="lnkEnableMfa" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Name")%>' runat="server" OnClick="lnkEnableMfa_Click" Text='<%#GetDecodedLabel("enable")%>' />                                                  													     
                                                </asp:PlaceHolder>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
                        </div>
                    </telerik:RadPageView>

                    <telerik:RadPageView ID="pvOAuth" runat="server">
                        <div style="margin: 10px">
                            <table class="SubList">
                                <asp:Repeater ID="repOAuth" runat="server">
                                    <HeaderTemplate>
                                        <tr>
                                            <th style="width: 32px;"></th>
                                            <th style="width: 200px;"></th>
                                        </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="width: 32px;">
                                                <img src="<%#DataBinder.Eval(Container.DataItem, "Image")%>" title="<%#DataBinder.Eval(Container.DataItem, "Description")%>" />
                                            </td>
                                            <td style="width: 200px;">
                                                <asp:PlaceHolder ID='plConnected' runat='server' Visible='<%# DataBinder.Eval(Container.DataItem, "Connected")%>'>
                                                    <%# DataBinder.Eval(Container.DataItem, "ProviderUserName")%>
														        &nbsp;
														        <asp:LinkButton CssClass="ButtonLink" Visible='<%# FromUserProfile%>' ID="lnkDisConnectOAuth" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Name")%>' runat="server" OnClick="lnkDisConnectOAuth_Click" Text='<%#GetDecodedLabel("disconnect")%>' OnClientClick="return ConfirmDisconnect();" />
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID='plNotConnected' runat='server' Visible='<%# (Not DataBinder.Eval(Container.DataItem, "Connected"))%>'>
                                                    <asp:LinkButton CssClass="ButtonLink" Visible='<%# FromUserProfile%>' ID="lnkConnectOAuth" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Name")%>' runat="server" OnClick="lnkConnectOAuth_Click" Text='<%#GetDecodedLabel("connect")%>' />
                                                </asp:PlaceHolder>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </table>
                        </div>
                    </telerik:RadPageView>

                    <telerik:RadPageView ID="pvLicencing" runat="server" CssClass="overflow-hidden">
                        <div style="margin: 10px">
                            <div class="row detail-form-row level1 mt-3">
                                <div class="col-md-6 col-lg-4">
                                    <ArcoControls:CheckBox ID="chkNamedWeb" runat="server" Text="Named in Web" />
                                </div>
                                <div class="col-md-6 col-lg-8">
                                    <ArcoControls:CheckBox ID="chkNamedWebAPI" runat="server" Text="Named in Web API (Doma Explorer, Mobile Apps)" />
                                </div>
                            </div>
                        </div>
                    </telerik:RadPageView>

                    <telerik:RadPageView runat="server" ID="pvAudit" BackColor="White">

                        <div style="overflow: auto; max-height: 350px; margin: 10px; width: 98%;">

                            <table class="SubList">
                                <asp:Repeater ID="repAudit" runat="server">
                                    <HeaderTemplate>
                                        <tr>
                                            <th style="width: 200px;"><%=GetLabel("user")%></th>
                                            <th style="width: 100px;"><%=GetLabel("date")%></th>
                                            <th style="width: 200px;"><%=GetLabel("description")%></th>

                                        </tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td style="width: 200px;"><%#DataBinder.Eval(Container.DataItem, "USER")%></td>
                                            <td style="width: 100px;"><%#DataBinder.Eval(Container.DataItem, "DATE")%></td>
                                            <td style="width: 200px;"><%#DataBinder.Eval(Container.DataItem, "DESCRIPTION")%></td>


                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <tr runat="server" id="trNoAuditFound" visible="false" class="ListFooter">
                                    <td colspan="3">
                                        <asp:Label runat="server" ID="lblNoAuditFound" /></td>
                                </tr>
                            </table>
                        </div>
                    </telerik:RadPageView>
                </telerik:RadMultiPage>
            </div>
        </div>

        <div class="container-fluid">
            <div class="row mt-2">
                <div class="col">
                    <Arco:ButtonPanel runat="server" ID="btnPanel">
                        <Arco:OkButton runat="server" ID="lnkSave" CommandName="Save" OnClick="doSave" Text="Save"> </Arco:OkButton>
                        <Arco:CancelButton runat="server" ID="lnkCancel" OnClientClick="javascript:DoCancel();return false;"></Arco:CancelButton>
                        <Arco:SecondaryButton runat="server" ID="TransferWorkButton" Text="Transfer Work" OnClientClick="javascript:TransferWork();return false;"></Arco:SecondaryButton>
                        <Arco:SecondaryButton runat="server" ID="lnkUnlockWorkflows" OnClientClick="return confirm(UIMessages[30]);" Text="Unlock workflows" OnClick="doUnlock"></Arco:SecondaryButton>
                        <Arco:SecondaryButton runat="server" ID="lnkDelete" OnClientClick="return ConfirmDeleteUser();" Text="Delete" OnClick="doDelete"></Arco:SecondaryButton>
                        <Arco:SecondaryButton runat="server" ID="lnkImpersonate" OnClick="lnkImpersonate_Click"></Arco:SecondaryButton>
                    </Arco:ButtonPanel>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
