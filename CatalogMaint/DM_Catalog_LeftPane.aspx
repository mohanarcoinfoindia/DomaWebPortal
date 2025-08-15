<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_LeftPane.aspx.vb"
    Inherits="DM_Catalog_LeftPane" %>

<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" TagPrefix="ArcoControls" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="formLeftPane" runat="server">
        <Arco:PageController ID="thisPageController" runat="server" PageLocation="ListPage" />
        <asp:GridView runat="server" ID="gridRes" AutoGenerateColumns="False" OnLoad="TableGridView_Init" SkinID="StickyHeadersGrid">
            <HeaderStyle CssClass="ListHeader"/>
            <Columns>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# GetLabel("cm_name") %>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblListID" runat="server" Visible="false" Text='<%# Eval("ID") %>' />
                        <asp:Label ID="lblListType" runat="server" Visible="false" Text='<%# Eval("TYPE") %>' />
                        <asp:Label ID="lblLinkedList" runat="server" Visible="false" Text='<%# Eval("LINKED-LIST") %>' />
                        <asp:Label ID="lblName" runat="server" Text='<%# Eval("NAME") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# GetLabel("cm_type")%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblType" runat="server" Text='<%# Eval("TYPE") %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# GetLabel("cm_multi-lingual")%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <ArcoControls:CheckBox runat="server" Checked='<%# Eval("MULTI-LINGUAL") %>' Enabled="false" />
                        <asp:Label ID="lblLanguages" runat="server" Text='<%#GetLanguageList(Convert.ToInt32(Eval("ID")),Convert.ToBoolean(Eval("MULTI-LINGUAL"))) %>' />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <%# GetLabel("cm_category_dependant")%>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <ArcoControls:CheckBox runat="server" Checked='<%# Eval("LIST_CATDEPENDANT") %>' Enabled="false" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <div style="text-align: center">
            <asp:Label runat="server" ID="lblMessage"></asp:Label>
        </div>

    </form>
</body>
</html>
