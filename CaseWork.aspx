<%@ Page Language="VB" AutoEventWireup="false" CodeFile="CaseWork.aspx.vb" Inherits="CaseWork"  MasterPageFile="~/masterpages/Preview.master" MaintainScrollPositionOnPostback="false" %>
<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls" TagPrefix="Arco" %>

  <asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
    <script type="text/javascript">
        function RefreshOnChange() {
            PC().Reload();
        }
    </script>
    <asp:Table ID="tblMain" runat="server" class="DetailTable">
    <asp:TableRow>
    <asp:TableCell CssClass="LabelCell" >
    <asp:Label CssClass="Label" Text="Add :" ID="lblAdd" runat="server"></asp:Label>
    </asp:TableCell>
    <asp:TableCell CssClass="FieldCell">
     <Arco:SelectSubject ID="newUser" runat="server" RefreshOnChange="true" UsersOnly="false" HideDeleteIcon="true" />
    </asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
    <asp:TableCell ColumnSpan="2" HorizontalAlign="Right" >
    <asp:LinkButton ID="lnkRecalculate" runat="server" Text="Recalculate work" OnClientClick="return true;"></asp:LinkButton></asp:TableCell>
    </asp:TableRow>
    <asp:TableRow>
    <asp:TableCell ColumnSpan="2">
        <asp:GridView ID="lstWork" runat="server" AutoGenerateColumns="False" HorizontalAlign="Center" DataKeyNames="ID">
            <Columns>
                <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
                <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" ItemStyle-Width="100%" />            
                <asp:BoundField DataField="SubjectType" HeaderText="Type" SortExpression="" />  
                
                <asp:BoundField DataField="Subject" HeaderText="SubjectUnformatted" SortExpression="Subject"/> 
                
                <asp:ButtonField Text="Assign" CommandName="cmdAssignCase" ControlStyle-CssClass="ButtonLink" />   
                <asp:ButtonField Text="Unlock" CommandName="cmdUnlockCase" ControlStyle-CssClass="ButtonLink" />           
                <asp:ButtonField Text="Delete" CommandName="cmdDeleteItem" ControlStyle-CssClass="ButtonLink" />              

              </Columns> 
        </asp:GridView>
    </asp:TableCell>
    </asp:TableRow>
    </asp:Table>
    
   
        
       </asp:Content>
       
  
 
