<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MailTracking.ascx.vb" Inherits="MailClient_MailTracking" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<script type="text/javascript">
    function TSTS_<%=MailID %>(trackid, togglemode) {
        var img = document.getElementById('<%=me.clientid %>_expbut_' + trackid);
        var row = document.getElementById('<%=me.clientid %>_exprow_' + trackid);

        var bDoToggle = true;
        if (togglemode != null) {

            if (img.src.match('Collapse.svg')) {
                bDoToggle = togglemode;
            }
            else {
                bDoToggle = !togglemode;
            }
        }
        if (bDoToggle) {
            if (img.src.match('Collapse.svg')) {
                img.src = '<%=Page.ResolveClientUrl("~/Images/Expand.svg") %>';
                        row.style.display = 'none';
                    }
                    else {
                        img.src = '<%=Page.ResolveClientUrl("~/Images/Collapse.svg") %>';
                        row.style.display = '';
            }
        }
    }        
</script>
<asp:Repeater ID="lstTracking" runat="server">
    <HeaderTemplate>
    </HeaderTemplate>
    <ItemTemplate>
    </ItemTemplate>
    <FooterTemplate>
    </FooterTemplate>
</asp:Repeater>
