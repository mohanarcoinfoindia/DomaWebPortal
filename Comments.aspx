<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Comments.aspx.vb" Inherits="Doma.Comments" ValidateRequest="false"  MasterPageFile="~/masterpages/Preview.master" %>
<%@ Register TagPrefix="Arco" TagName="Comments" Src="~/UserControls/Comments.ascx" %>

    <asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
        <script type="text/javascript">
            function ReloadParent()
            {
            PC().Reload();
            }
        </script>
        <Arco:Comments id="lblComments" runat="server" />
    </asp:Content>
    

