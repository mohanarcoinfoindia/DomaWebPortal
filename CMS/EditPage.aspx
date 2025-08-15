<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EditPage.aspx.vb" Inherits="CMS_EditPage" MasterPageFile="~/masterpages/Toolwindow.master" %>

<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" TagPrefix="Arco" %>


<asp:Content ID="headContent" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <script type="text/javascript">      
        function Refresh() {
            document.forms[0].submit();
        }

    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">

    <asp:Panel runat="server" ID="configForm" CssClass="container-fluid detail-form-container CMSConfigTable">
        <asp:Panel CssClass="row detail-form-row" runat="server">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblName" AssociatedControlID="txtName"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel CssClass="row detail-form-row" runat="server">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblTtitle" AssociatedControlID="txtTitle"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <Arco:MultiLanguageTextBox CssClass="Label" runat="server" ID="txtTitle"></Arco:MultiLanguageTextBox>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel CssClass="row detail-form-row" runat="server">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblDesc" AssociatedControlID="txtDesc"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <asp:TextBox ID="txtDesc" runat="server" Rows="5" TextMode="MultiLine"></asp:TextBox>
            </asp:Panel>
        </asp:Panel>
          <asp:Panel CssClass="row detail-form-row" runat="server">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblFolder" AssociatedControlID="txtFolder"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <Asp:TextBox CssClass="Label" runat="server" ID="txtFolder"></Asp:TextBox>
            </asp:Panel>
        </asp:Panel>
        <asp:Panel CssClass="row detail-form-row mt-1" runat="server">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="lblLayouts" Text="Layout"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <asp:HiddenField runat="server" ID="selectedLayout" />
                <asp:Repeater runat="server" ID="repLayouts">
                    <HeaderTemplate>
                        <div class="cms-thumbnail-container" id="<%= repLayouts.ClientID %>">
                    </HeaderTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
            </asp:Panel>
        </asp:Panel>

        <asp:Panel CssClass="row detail-form-row mt-1" runat="server" ID="pnlAcl">
            <asp:Panel CssClass="col-md-4 LabelCell" runat="server">
                <asp:Label CssClass="Label" runat="server" ID="Label1" Text="Acl"></asp:Label>
            </asp:Panel>
            <asp:Panel CssClass="col-md-8 FieldCell" runat="server">
                <asp:HiddenField runat="server" ID="aclSubjectToDelete" />
                <asp:HiddenField runat="server" ID="aclSubjectTypeToDelete" />

                 <script type="text/javascript">
       
                        function RemoveAcl(subject, subjecttype) {
                            if (confirm(UIMessages[4])) {
              
                                document.forms[0].elements["<%=aclSubjectToDelete.ClientID %>"].value = subject;
                                document.forms[0].elements["<%=aclSubjectTypeToDelete.ClientID %>"].value = subjecttype;

                                document.forms[0].submit();
                            }
                        }
                    </script>

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
                                                    <td><%# DataBinder.Eval(Container.DataItem, "CAPTION").ToString()%> </td>
                                                    <td><%#DataBinder.Eval(Container.DataItem, "DESCRIPTION")%></td>
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
                                            <span class="icon icon-add-new" style="cursor: pointer;" onclick="javascript:AddAcl();" title="<%=GetLabel("add") %>" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>

            </asp:Panel>
        </asp:Panel>


        <asp:Panel CssClass="row detail-form-row mt-3" runat="server">
            <asp:Panel CssClass="col-md-8 offset-md-4 FieldCell" runat="server">
                <Arco:ButtonPanel ID="ButtonPanel1" runat="server">
                    <Arco:OkButton runat="server" ID="btnSave"></Arco:OkButton>
                    <Arco:CancelButton runat="server" ID="btnCancel"></Arco:CancelButton>
                </Arco:ButtonPanel>
            </asp:Panel>
        </asp:Panel>
    </asp:Panel>

    <script type="text/javascript">
        function selectLayout(thumbnail, id) {
            document.getElementById('<%= selectedLayout.ClientId %>').value = id;  // assign id to hiddenfield
            const thumbnails = thumbnail.parentElement.children;
            for (var i = 0; i < thumbnails.length; i++) {
                thumbnails[i].classList.remove('selected');
            }
            thumbnail.classList.add('selected');
        }
        // make sure selected layout is visible to user
        const layoutsContainer = $get('<%= repLayouts.ClientID %>');
        const selectedThumbnail = layoutsContainer.getElementsByClassName('selected')[0];
        if (selectedThumbnail) {
            layoutsContainer.scrollTop = selectedThumbnail.offsetTop;
        }
    </script>
</asp:Content>
