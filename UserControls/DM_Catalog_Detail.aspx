<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_Detail.aspx.vb" Inherits="DM_Catalog_Detail" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>List Item Details</title>
</head>
<body>
    <form id="form1" runat="server">
      <div>
		<TABLE WIDTH='90%' ALIGN='CENTER' cellspacing=0 cellpadding=5>
		    <asp:Literal ID="plhDetail" runat="server"></asp:Literal>
            <asp:Literal ID="plhTranslations" runat="server"></asp:Literal>
            <asp:Literal ID="plhSynonyms" runat="server"></asp:Literal>
            <asp:Literal ID="plhRelations" runat="server"></asp:Literal>
        </TABLE>
      </div>
    </form>
</body>
</html>
