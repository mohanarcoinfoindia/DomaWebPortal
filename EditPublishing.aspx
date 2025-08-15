<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EditPublishing.aspx.vb" Inherits="EditPublishing" MasterPageFile="~/masterpages/Preview.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
 <script type="text/javascript">
        function RefreshOnChange() {
            PC().Reload();
     }
     function CopyUrlToClipboard() {
         var url = $("#<%=lblUrl.clientId%>").text();
        
         var $temp = $("<input>");
         $("body").append($temp);
        $temp.val(url).select();
        document.execCommand("copy");
         $temp.remove();

        }
    </script>
    
     <asp:Table ID="tblMain" runat="server" class="DetailTable EditPublishing">
     <asp:TableRow>
     <asp:TableCell CssClass="LabelCell">
        <asp:Label CssClass="Label" runat="server" ID="lblObjectType" Text="Object"></asp:Label>
     </asp:TableCell>
     <asp:TableCell CssClass="ReadOnlyFieldCell">
        <asp:Label CssClass="ReadOnly" runat="server" ID="lblObjectName"></asp:Label>
     </asp:TableCell>
     </asp:TableRow>
     <asp:TableRow>
     <asp:TableCell CssClass="LabelCell">
        <asp:Label CssClass="Label" runat="server" ID="Label3" Text="Url"></asp:Label>
     </asp:TableCell>
     <asp:TableCell CssClass="ReadOnlyFieldCell">
        <asp:Label CssClass="ReadOnly" runat="server" ID="lblUrl"></asp:Label>
        <asp:HyperLink runat="server" NavigateUrl="javascript:CopyUrlToClipboard();">
            <span class="icon icon-copy-clipboard" />
        </asp:HyperLink>
     </asp:TableCell>
     </asp:TableRow>

       <asp:TableRow>
    <asp:TableCell CssClass="LabelCell">
    <asp:Label CssClass="Label" Text="Expires" ID="lblExpires" runat="server" AssociatedControlID="dpExpNew"></asp:Label>
    </asp:TableCell>
    <asp:TableCell CssClass="FieldCell">
      <telerik:RadDateTimePicker ID="dpExpNew" runat="server"/>
    </asp:TableCell>
    </asp:TableRow>

    <asp:TableRow>
    <asp:TableCell CssClass="LabelCell">
    <asp:Label CssClass="Label" ID="lblAdd" runat="server"></asp:Label>
    </asp:TableCell>
    <asp:TableCell CssClass="FieldCell">
     <Arco:SelectSubject ID="newUser" runat="server" RefreshOnChange="true" UsersOnly="false" HideDeleteIcon="true" />
    </asp:TableCell>
    </asp:TableRow>
    
    <asp:TableRow>
    <asp:TableCell CssClass="FieldCell" ColumnSpan="2">
        <asp:GridView ID="lstRules" runat="server" AutoGenerateColumns="False" HorizontalAlign="Center" DataKeyNames="ID">
            <Columns>
                <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" ReadOnly="true" />
                <asp:BoundField DataField="Subject_ID" HeaderText="Subject" SortExpression="Subject_ID" ItemStyle-Width="100%" ReadOnly="true" />            
                <asp:BoundField DataField="Subject_Type" HeaderText="Type" SortExpression="" ReadOnly="true" />  
                
                <asp:BoundField DataField="Subject_ID" HeaderText="SubjectUnformatted" SortExpression="Subject" ReadOnly="true"/>               
                <asp:TemplateField HeaderText="Expires" >
                 <EditItemTemplate>
                   <telerik:RadDateTimePicker ID="dpExp" runat="server"/>
                 </EditItemTemplate>
                 <ItemTemplate >
                    <asp:Label Runat="server"  ID="lblExpDate"></asp:Label>
                 </ItemTemplate>
                 </asp:TemplateField>

                <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" ControlStyle-CssClass="ButtonLink" />                               
              </Columns> 
        </asp:GridView>
    </asp:TableCell>
    </asp:TableRow>
       
    
    </asp:Table>
</asp:Content>
