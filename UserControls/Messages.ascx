<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radE" %>
<%@ Register TagPrefix="Arco" TagName="MailTracking" Src="~/MailClient/Mailtracking.ascx" %>

<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Messages.ascx.vb" Inherits="UserControls_Messages" %>
<script type="text/javascript">

    function OpenMail(mailid) {
        PC().OpenDetailWindow('./MailClient/View.aspx?DM_MAIL_ID=' + mailid, mailid);
    }
    function ViewDomaAtt(id, currentmailid) {
        var url = 'DM_VIEW_FILE.aspx?frommail=Y&file_id=' + id + '&DM_MAIL_ID=' + currentmailid;
        PC().OpenWindow(url, 'MailFile', 'height=600,width=800,scrollbars=yes,resizable=yes,toolbar=yes');

    }
</script>
<div style="clear: both; width: 100%<%=ExtraStyle%>">
                <%if (Not ShowAsJournal OrElse Count <= 0) Then %>
     <asp:PlaceHolder ID="pnlToolbar" runat="server" Visible="false" EnableViewState="false">
        <asp:PlaceHolder ID="plhGridHeaderStart" runat="server" />
         <asp:PlaceHolder ID="plhGridHeader" runat="server" />
         <asp:PlaceHolder ID="plhGridHeaderEnd" runat="server" />
     </asp:PlaceHolder>
                    <%End If %>


    <div id="myDiv" class="SubListDiv container-fluid messages" style="zoom: 1;">
        <asp:Repeater ID="MessageList" runat="server" OnItemDataBound="Messages_ItemDataBound" EnableViewState="false" >
            <HeaderTemplate>
                <%if (Not ShowAsJournal) Then %>

                <table class="SubList">
                    <tr>
                        <th>&nbsp;</th>
                        <th><%=GetLabel("mail_from")%></th>
                        <th><%=GetLabel("mail_to")%></th>
                        <th><%=GetLabel("mail_subject")%></th>
                        <th><%=GetLabel("date")%></th>  
                        <th><%=GetLabel("package")%></th>
                        <th>&nbsp;</th>
                    </tr>

                    <%End If %>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Literal ID="lblContentGrid" runat="server"></asp:Literal>

                <asp:PlaceHolder ID="plhTracking" runat="server">
                    <tr>
                        <td>&nbsp;</td>
                        <td colspan='7' valign='top'>
                            <Arco:MailTracking ID="pnlTracking" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="7">&nbsp;</td>
                    </tr>
                </asp:PlaceHolder>
            </ItemTemplate>
            <FooterTemplate>
                
                </table>

            </FooterTemplate>
           
        </asp:Repeater>
        <asp:PlaceHolder ID="plhMainRow" runat="server" EnableViewState="false" />
    </div>
</div>
<asp:ObjectDataSource ID="MessageListDataSource" runat="server" SelectMethod="GetObjectMessages" TypeName="Arco.Doma.Library.Mail.MailSearch" >
    
    <SelectParameters>
        <asp:Parameter Name="vlObjectID" Type="Int32" />
    </SelectParameters>
    <SelectParameters>
        <asp:Parameter Name="vlPackageID" Type="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>
