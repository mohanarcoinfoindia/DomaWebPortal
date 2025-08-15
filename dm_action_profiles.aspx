<%@ Page Language="VB" AutoEventWireup="false" CodeFile="dm_action_profiles.aspx.vb" Inherits="dm_action_profiles" MasterPageFile="~/masterpages/Empty.master" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <script type="text/javascript">
        function EditProfile(id) {
            var a = 'DM_EDIT_ACTION_PROFILE.aspx?profileid=' + id + '&rnd_str=' + Math.random();;
            var w = window.open(a, 'EditActionProfile', 'height=600,width=800,scrollbars=yes,resizable=yes');
            w.focus();
        }
        function ViewProfile(id) {
            var a = 'DM_View_ACTION_PROFILE.aspx?profileid=' + id + '&rnd_str=' + Math.random();;
            var w = window.open(a, 'ViewActionProfile', 'height=600,width=800,scrollbars=yes,resizable=yes');
            w.focus();
        }
        function NewProfile() {
            EditProfile(0);
        }
        function DeleteProfile(id,name) {
            if (confirm(UIMessages[29].replace("#ITEM#", name))) {
                location.href = 'dm_action_profiles.aspx?deleteid=' + id;
            }
        }
        function Refresh() {
            location.href = 'dm_action_profiles.aspx';
        }
</script>

</asp:Content>

<asp:Content ContentPlaceHolderID="Plh1" runat="server">
       <asp:Panel ID="pnlError" runat="server" CssClass="ErrorLabel" Visible="false">
           <asp:Label ID="errormsg" runat="server"></asp:Label>
       </asp:Panel>
       <asp:Panel ID="pnlProfiles" Runat="server"/>
</asp:Content>

