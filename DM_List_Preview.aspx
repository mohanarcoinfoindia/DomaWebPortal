<%@ Page Language="VB" MasterPageFile="~/masterpages/Preview.master" AutoEventWireup="false" EnableSessionState="True" CodeFile="DM_List_Preview.aspx.vb" Inherits="DM_List_Preview" title="Preview" EnableEventValidation="false" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<%@ Register Src="UserControls/DM_Listcontrol.ascx" TagName="DM_Listcontrol" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
<script type="text/javascript" language="javascript" src="./JS/DocumentList.js"></script>	
<asp:HiddenField ID="hdnDIN" runat="server" />
<div style="width:100%">

<uc1:DM_Listcontrol id="resultgrid1" runat="server" PreviewMode="true" ShowBackToSearchLink="false" AjaxEnabled="True" ShowSideBar="false">
</uc1:DM_Listcontrol>

</div>

<script language="javascript">

    
	try
	{
	    parent.SetHeader('<%=HeaderText%>');
	}
	catch(exc)
	{}
	
	</script>


</asp:Content>

