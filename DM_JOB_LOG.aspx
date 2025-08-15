<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_JOB_LOG.aspx.vb" Inherits="DM_JOB_LOG" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Batch Job Log</title> 
</head>
<body>
    <form id="form1" runat="server">
    <div>
           <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="JobLogDataSource" HorizontalAlign="Center" SkinID="SubList"  >
            <Columns>
                <asp:BoundField DataField="Level" HeaderText="Level" SortExpression="ID" />
                <asp:BoundField DataField="LogTime" HeaderText="Time" SortExpression="LogTime" />            
                <asp:BoundField DataField="Text" HeaderText="Text" SortExpression="" />                
              </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="JobLogDataSource" runat="server" SelectMethod="GetSyslogList"
            TypeName="Arco.Doma.Library.Logging.SysLogList">
            <SelectParameters>
                <asp:QueryStringParameter Name="vlBatchID" QueryStringField="BATCH_ID" Type="Int32" />
            </SelectParameters>
            
            </asp:ObjectDataSource>
            
        <Arco:PageFooter id="lblFooter" runat="server"/> 
    </div>
    </form>
</body>
</html>
