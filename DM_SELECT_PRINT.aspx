<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_SELECT_PRINT.aspx.vb" Inherits="DM_SELECT_PRINT"  MasterPageFile="~/masterpages/Toolwindow.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
<script type="text/javascript">
    function PrintList(printoption) {
        const grid = MainPage().PC().GetCurrentResultGrid();
   <%       If Mode = "xls" %>
    grid.ExecutePrintToExcel(printoption,<%=Printseltype %>,<%=PrintXml %>);
   <%else %>
    grid.ExecutePrint(printoption,<%=Printseltype %>,<%=PrintXml %>);
      Close();
  <%end if %>

}
</script>	
        <asp:HiddenField ID="txtSelection" runat="server" />
                    <asp:HiddenField ID="PrintOption" runat="server" />
                 
   <ul>
   <asp:PlaceHolder runat="server" ID="plhSelection">
                        <li><asp:HyperLink ID="hplSelection" runat="server">Print selection</asp:HyperLink></li>
    </asp:PlaceHolder>
                        <li><asp:HyperLink ID="hplCurrentPage" runat="server">Print current page</asp:HyperLink></li>
                        <li><asp:HyperLink ID="hplAll" runat="server">Print all pages</asp:HyperLink></li>
             
    </ul>
   
</asp:Content>
