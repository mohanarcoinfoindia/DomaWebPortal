<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EditRoleLabel.aspx.vb" Inherits="UserBrowser_EditRoleLabel" MasterPageFile="~/masterpages/Toolwindow.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">

<asp:Table ID="tbl" runat="server" CssClass="DetailTable" style="margin-top: 16px; border-collapse: separate; border-spacing: 10px; border: 10px solid transparent;">
<asp:TableRow>
<asp:TableCell CssClass="FieldCell">
<asp:Label ID="lblE" runat="server" CssClass="Label" AssociatedControlID="txtE"></asp:Label>
</asp:TableCell>
<asp:TableCell CssClass="FieldCell">
<asp:TextBox MaxLength="255" runat="server" ID="txtE" style="width: 100%;"/>
</asp:TableCell>
</asp:TableRow>

<asp:TableRow>
<asp:TableCell CssClass="FieldCell">
<asp:Label ID="lblN" runat="server" CssClass="Label"  AssociatedControlID="txtN"></asp:Label>
</asp:TableCell>
<asp:TableCell CssClass="FieldCell">
<asp:TextBox MaxLength="255" runat="server" ID="txtN" style="width: 100%;" />
</asp:TableCell>
</asp:TableRow>

<asp:TableRow >
<asp:TableCell CssClass="FieldCell">
<asp:Label ID="lblF" runat="server" CssClass="Label" AssociatedControlID="txtF"></asp:Label>
</asp:TableCell>
<asp:TableCell CssClass="FieldCell">
<asp:TextBox MaxLength="255" runat="server" ID="txtF" style="width: 100%;" />
</asp:TableCell>
</asp:TableRow>

<asp:TableRow>
<asp:TableCell CssClass="FieldCell">
<asp:Label ID="lblG" runat="server" CssClass="Label"  AssociatedControlID="txtG"></asp:Label>
</asp:TableCell>
<asp:TableCell CssClass="FieldCell">
<asp:TextBox MaxLength="255" runat="server" ID="txtG" style="width: 100%;" />
</asp:TableCell>
</asp:TableRow>

<asp:TableRow>
<asp:TableCell ColumnSpan="2" style="padding-top: 21px;">
<Arco:ButtonPanel ID="ButtonPanel1" runat="server">
<Arco:OkButton runat="server" ID="btnOk"></Arco:OkButton>
<Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" id="btnCancelUrl"></Arco:CancelButton>
</Arco:ButtonPanel>
</asp:TableCell>
</asp:TableRow>
</asp:Table>

</asp:Content>