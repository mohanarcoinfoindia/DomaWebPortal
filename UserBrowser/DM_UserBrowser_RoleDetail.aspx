<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_UserBrowser_RoleDetail.aspx.vb" Debug="true" Inherits="DM_UserBrowser_RoleDetail" ValidateRequest="false" %>

<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>User managment - Add</title>
    <script type="text/javascript">
        function DoClose() {
            location.href = 'DM_UserBrowser_Roles.aspx';
        }

        function winopen() {
            window.opener.location.href = window.opener.location.href;
        }

        function ShowDetails(id, type) {
            if (type == "Role") {
                location.href = 'DM_UserBrowser_RoleDetail.aspx' + '?login=' + id + '&new=false&structured=true';             
            }
            if (type == "User") {
                location.href = "DM_UserBrowser_AddUser.aspx?login=" + id + "&new=false";
            }
        }

        function Refresh() {
            document.forms[0].submit();
        }

        function EditLabels(id,type) {
            var oWnd2 = radopen("EditRolelabel.aspx?modal=Y&role_id=" + id + "&type=" + type, "Popup", 600, 367);
        }
   
    </script>
</head>

<body>
    <form id="Form1" method="post" runat="server" defaultbutton="lnkSave">
        <telerik:RadScriptManager runat="server" ID="scriptManager" />
        
        <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" />
        <telerik:RadWindowManager ID="RadWindowManager2" ShowContentDuringLoad="false" VisibleStatusbar="false" Modal="true" ReloadOnShow="true" runat="server">
            <Windows>
                <telerik:RadWindow runat="server" ID="Popup" ShowContentDuringLoad="false" VisibleStatusbar="false" Behaviors="Close" />
                <telerik:RadWindow runat="server" ID="Roles" ShowContentDuringLoad="false" VisibleStatusbar="false" Overlay="true" Behaviors="Close, Maximize, Resize" />
                <telerik:RadWindow runat="server" ID="Groups" ShowContentDuringLoad="false" VisibleStatusbar="false" Overlay="true" Behaviors="Close, Maximize" Height="810" />
            </Windows>
        </telerik:RadWindowManager>

        <asp:Label ID="lblMsgText" runat="server" Visible="false" />

        <div class="container-fluid detail-form-container" id="tblRolesearch">

            <div class="row detail-form-row">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblRoleName" runat="server" CssClass="Label" />
                </div>
                <div class="col-md-8 FieldCell">
                    <asp:TextBox runat="server" ID="txtRoleName" MaxLength="200"></asp:TextBox>
                    <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ID="reqName" ControlToValidate="txtRoleName" ErrorMessage="*" SetFocusOnError="true"></asp:RequiredFieldValidator>
                    <asp:HyperLink ID="lnkLabels" runat="server" Text="Labels" />
                </div>
            </div>

            <div class="row detail-form-row">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblRoleDescription" runat="server" CssClass="Label" />
                </div>
                <div class="col-md-8 FieldCell">
                    <asp:TextBox runat="server" ID="txtRoleDescription" MaxLength="2000" TextMode="MultiLine" SkinID="MultiLine" />
                    <div>
                        <asp:HyperLink ID="lnkDescLabels" runat="server" Text="Labels" />
                        </div>
                </div>
            </div>

            <div class="row detail-form-row" id="rowTenant" style="display: none;" runat="server">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblTenant" runat="server" CssClass="Label" />
                </div>
                <div class="col-md-8 FieldCell">
                    <asp:DropDownList ID="drpTenant" runat="server">
                    </asp:DropDownList>
                </div>
            </div>

            <div class="row detail-form-row">
                <div class="col-md-4 LabelCell">
                </div>
                <div class="col-md-8 FieldCell">
                    <ArcoControls:CheckBox ID="chkStructure" runat="server" />
                    &nbsp;
                    <ArcoControls:CheckBox ID="chkLocked" runat="server" Enabled="false" />
                </div>
            </div>

            <div class="row detail-form-row" id="rowParentRole" runat="server">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblParentStructureRole" runat="server" CssClass="Label" Text="Parent Structure Role" />
                </div>
                <div class="col-md-8 FieldCell">
                    <asp:Label ID="lblRoleHierarchy" runat="server" Text=""></asp:Label>
                    &nbsp;
                </div>
            </div>

            <div class="row detail-form-row">
                <div class="col-md-8 offset-md-4 FieldCell">
                    <Arco:ButtonPanel runat="server" ID="btnPanel">
                        <Arco:OkButton runat="server" ID="lnkSave" CommandName="Save" OnClick="SaveRole"> </Arco:OkButton>
                        <Arco:CancelButton runat="server" ID="btnClose" OnClientClick="javascript:DoClose();return false;"></Arco:CancelButton>&nbsp;
                                    <Arco:SecondaryButton runat="server" ID="lnkDelete" OnClientClick="return confirm(UIMessages[4]);" Text="Delete" OnClick="DoDelete"></Arco:SecondaryButton>
                        <Arco:SecondaryButton runat="server" ID="lnkTransferWork" OnClientClick="javascript:TransferWork();return false;"></Arco:SecondaryButton>
                    </Arco:ButtonPanel>
                </div>
            </div>
        </div>

        <asp:Label ID="Result" runat="server" />

        <telerik:RadTabStrip runat="server" ID="radTab" SelectedIndex="0" MultiPageID="RadMultiPage1" Skin="Vista">
            <Tabs>
                <telerik:RadTab Text="Role Details" />
                <telerik:RadTab Text="Member Of" />
                <telerik:RadTab Text="Security" />
                <telerik:RadTab Text="Audit" />
            </Tabs>
        </telerik:RadTabStrip>
        <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" ScrollBars="Auto">
            <telerik:RadPageView runat="server" ID="PVRoleDetails" BackColor="White">
                <div id="divMembers" runat="server" visible="false" style="margin: 10px; width: 98%;">
                    <table style="border-collapse: collapse; border-spacing: 0; width: 100%;">
                        <tr>
                            <td>
                                <div style="overflow: auto; max-height: 500px; vertical-align: top;">

                                    <table class="SubList">
                                        <asp:Repeater ID="repMembers" runat="server">
                                            <HeaderTemplate>
                                                <tr>
                                                    <th style="width: 10px;"></th>
                                                    <th style="width: 200px;"><%=GetLabel("name")%></th>                                                    
                                                    <%If MultiTenant Then%>
                                                    <th><%=GetLabel("tenant") %></th>
                                                    <%end if %>
                                                </tr>
                                            </HeaderTemplate>

                                            <ItemTemplate>
                                                <tr>
                                                    <td style="width: 10px;">
                                                        <span class="<%#GetIcon(DataBinder.Eval(Container.DataItem, "TYPE").ToString())%>" title="<%#GetLabel(DataBinder.Eval(Container.DataItem, "TYPE").ToString())%>" />
                                                    </td>
                                                    <td style="width: 200px; cursor: pointer" onclick="javascript:ShowDetails('<%#If(DataBinder.Eval(Container.DataItem, "TYPE").ToString() = "Role", GetRoleID(DataBinder.Eval(Container.DataItem, "NAME").ToString()), DataBinder.Eval(Container.DataItem, "NAME").ToString().Replace("_DATABASE\", "").Replace("\", "\\"))%>','<%# DataBinder.Eval(Container.DataItem, "TYPE").ToString() %>')"><%# DataBinder.Eval(Container.DataItem, "CAPTION").ToString().Replace("_DATABASE\", "")%> </td>                                                  
                                                    <%If MultiTenant Then%>
                                                    <td style="width: 200px; cursor: pointer" onclick="javascript:ShowDetails('<%#If(DataBinder.Eval(Container.DataItem, "TYPE").toString() = "Role",GetRoleID(DataBinder.Eval(Container.DataItem, "NAME").toString()),DataBinder.Eval(Container.DataItem, "NAME").toString().Replace("_DATABASE\", "").Replace("\","\\"))%>','<%# DataBinder.Eval(Container.DataItem, "TYPE").toString() %>')"><%#GetTenantLabel(CInt(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TenantId")))%></td>
                                                    <%End If%>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <tr runat="server" id="trMembers" visible="false" class="ListFooter">
                                            <td colspan="2">
                                                <asp:Label runat="server" ID="lblNoMembersFound" /></td>
                                        </tr>

                                    </table>

                                </div>
                            </td>
                        </tr>
                        <asp:PlaceHolder ID="plhEditMembers" runat="server">
                            <tr style="vertical-align: top;">
                                <td style="vertical-align: top;" colspan="3">
                                    <div class="SubListHeader3RightFillerDivSwitched">
                                        <div style="width: 35px;" class="rounded-headerR">
                                            <div class="rounded-headerL">
                                                <div class="SubListMainHeaderT">
                                                    <span class="icon icon-edit" onclick="javascript:EditMembers();" title="<%=getLabel("edit") %>" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        </asp:PlaceHolder>
                    </table>

                </div>
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="PVMemberOf" BackColor="White">
                <div id="divMemberOf" style="margin: 10px; width: 98%;" runat="server" visible="false">
                    <table style="border-collapse: collapse; border-spacing: 0; width: 100%;">
                        <tr>
                            <td>
                                <div style="overflow: auto; max-height: 350px;">
                                    <div style="max-height: 350px; overflow: auto;">
                                        <table class="SubList">
                                            <asp:Repeater ID="repMemberof" runat="server">
                                                <HeaderTemplate>
                                                    <tr>
                                                        <th style="width: 10px;"></th>
                                                        <th style="width: 200px;"><%=GetLabel("name")%></th>                                                        
                                                        <%If MultiTenant Then%>
                                                        <th><%=GetLabel("tenant") %></th>
                                                        <%end if %>
                                                    </tr>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td>
                                                            <span class="<%#If(DataBinder.Eval(Container.DataItem, "TYPE").ToString() = "Role", "icon icon-user-role icon-color-light", "icon icon-user-profile icon-color-light")%>"
                                                                title="<%#If(DataBinder.Eval(Container.DataItem, "TYPE").ToString() = "Role", GetLabel("role"), GetLabel("user"))%>" />
                                                        </td>
                                                        <td style="width: 200px; cursor: pointer" onclick="javascript:ShowDetails('<%#If(DataBinder.Eval(Container.DataItem, "TYPE").ToString() = "Role", GetRoleID(DataBinder.Eval(Container.DataItem, "NAME").ToString()), DataBinder.Eval(Container.DataItem, "NAME").ToString())%>','<%# DataBinder.Eval(Container.DataItem, "TYPE").ToString() %>')"><%# DataBinder.Eval(Container.DataItem, "CAPTION").ToString()%> </td>                                                        
                                                        <%If MultiTenant Then%>
                                                        <td style="width: 200px; cursor: pointer" onclick="javascript:ShowDetails('<%#If(DataBinder.Eval(Container.DataItem, "TYPE").toString() = "Role",GetRoleID(DataBinder.Eval(Container.DataItem, "NAME").toString()),DataBinder.Eval(Container.DataItem, "NAME").toString().Replace("_DATABASE\", "").Replace("\","\\"))%>','<%# DataBinder.Eval(Container.DataItem, "TYPE").toString() %>')"><%#GetTenantLabel(CInt(DataBinder.Eval(CType(Container, RepeaterItem).DataItem, "TenantId")))%></td>
                                                        <%End If%>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <tr runat="server" id="trMemberof" visible="false" class="ListFooter">
                                                <td colspan="3">
                                                    <asp:Label runat="server" ID="lblNoMemberOfFound" /></td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr style="vertical-align: top;">
                            <td style="vertical-align: top;" colspan="3">
                                <div class="SubListHeader3RightFillerDivSwitched">
                                    <div style="width: 35px;" class="rounded-headerR">
                                        <div class="rounded-headerL">
                                            <div class="SubListMainHeaderT">
                                                <span class="icon icon-edit" onclick="javascript:EditMemberOf();" title="<%=GetLabel("edit") %>" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>

                </div>

            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="Security" BackColor="White">

                 <script type="text/javascript">
       
                        function RemoveAcl(subject, subjecttype) {
                            if (confirm(UIMessages[4])) {
              
                                document.forms[0].elements["<%=aclSubjectToDelete.Name %>"].value = subject;
                                document.forms[0].elements["<%=aclSubjectTypeToDelete.Name %>"].value = subjecttype;

                                document.forms[0].submit();
                            }
                        }
                    </script>

                 <input type="hidden" id="aclSubjectToDelete" runat="server" />
                  <input type="hidden" id="aclSubjectTypeToDelete" runat="server" />

                <div id="divSecurity" style="margin: 10px; width: 98%;" runat="server">
                    <table style="border-collapse: collapse; border-spacing: 0; width: 100%;">
                        <tr>
                            <td>
                                <div style="overflow: auto; max-height: 350px;">
                                    <div style="max-height: 350px; overflow: auto;">
                                        <table class="SubList">
                                            <asp:Repeater ID="repSec" runat="server">
                                                <HeaderTemplate>
                                                    <tr>
                                                        <th style="width: 10px;"></th>
                                                        <th style="width: 200px;"><%=GetLabel("name")%></th>
                                                        <th style="width: 200px;"><%=GetLabel("description")%></th>
                                                        <th></th>
                                                    </tr>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td>
                                                            <span class="<%#If(DataBinder.Eval(Container.DataItem, "TYPE").ToString() = "Role", "icon icon-user-role icon-color-light", "icon icon-user-profile icon-color-light")%>"
                                                                title="<%#If(DataBinder.Eval(Container.DataItem, "TYPE").ToString() = "Role", GetLabel("role"), GetLabel("user"))%>" />
                                                        </td>
                                                        <td style="cursor: pointer" onclick="javascript:ShowDetails('<%#If(DataBinder.Eval(Container.DataItem, "TYPE").toString() = "Role",GetRoleID(DataBinder.Eval(Container.DataItem, "NAME").ToString()),DataBinder.Eval(Container.DataItem, "NAME").ToString().Replace("\","\\"))%>','<%# DataBinder.Eval(Container.DataItem, "TYPE").toString() %>')"><%# DataBinder.Eval(Container.DataItem, "CAPTION").ToString()%> </td>
                                                        <td style="cursor: pointer" onclick="javascript:ShowDetails('<%#If(DataBinder.Eval(Container.DataItem, "TYPE").toString() = "Role",GetRoleID(DataBinder.Eval(Container.DataItem, "NAME").ToString()),DataBinder.Eval(Container.DataItem, "NAME").ToString())%>','<%# DataBinder.Eval(Container.DataItem, "TYPE").toString() %>')"><%#DataBinder.Eval(Container.DataItem, "DESCRIPTION")%></td>
                                                        <td>
                                                            <a class='ButtonLink'
                                                                href='javascript:RemoveAcl(<%#EncodingUtils.EncodeJsString(DataBinder.Eval(Container.DataItem, "NAME").ToString())%>,<%#Arco.Doma.WebControls.EncodingUtils.EncodeJsString(DataBinder.Eval(Container.DataItem, "TYPE").ToString())%>)'><%= GetLabel("delete") %></a>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <tr runat="server" id="trNoAclFound" visible="false" class="ListFooter">
                                                <td colspan="3">
                                                    <asp:Label runat="server" ID="lblNoAclFound" /></td>
                                            </tr>
                                        </table>
                                    </div>
                                </div>
                            </td>
                        </tr>
                        <tr class="ListFooter">
                            <td colspan="8">
                                <div class="SubListHeader3RightFillerDivSwitched">
                                    <div class="rounded-headerR">
                                        <div class="rounded-headerL">
                                            <div class="SubListMainHeaderT">
                                                <span class="icon icon-add-new" style="cursor:pointer;" onclick="javascript:AddRoleAcl();" title="<%=GetLabel("add") %>" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </td>
                        </tr>
                    </table>
                    <iframe id="Hidden_Frame" style="display: none; visibility: hidden;"></iframe>
                </div>
            </telerik:RadPageView>

            <telerik:RadPageView runat="server" ID="audit" BackColor="White">
                <div id="divAudit" style="margin: 10px; width: 98%;" runat="server" visible="false">

                    <div style="overflow: auto; max-height: 350px;">
                        <div style="max-height: 350px; overflow: auto;">
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
                    </div>

                </div>
            </telerik:RadPageView>
        </telerik:RadMultiPage>
    </form>

</body>
</html>
