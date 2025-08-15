<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Linked.aspx.vb" Inherits="Linked" MasterPageFile="~/masterpages/Preview.master"%>
<%@ Register Src="UserControls/DM_Listcontrol.ascx" TagName="DM_Listcontrol" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
<script type="text/javascript" language="javascript" src="./JS/DocumentList.js"></script>	
<asp:HiddenField ID="hdnDIN" runat="server" />
<div style="width:100%;margin:5px">

<uc1:DM_Listcontrol id="packagelinksgrid" runat="server" EnableViewState="true" ShowToolbar="false" AjaxEnabled="true" ShowSideBar="false" ShowBackToSearchLink="false" ListCssClass="SubList" >
</uc1:DM_Listcontrol>

</div>

<div style="width:100%;margin:5px">

<uc1:DM_Listcontrol id="relationshipsgrid" runat="server" EnableViewState="true" ShowToolbar="false" AjaxEnabled="true" ShowSideBar="false" ShowBackToSearchLink="false" ListCssClass="SubList" />
</uc1:DM_Listcontrol>

</div>

<div style="width:100%;margin:5px">

<uc1:DM_Listcontrol id="shortcutsgrid" runat="server" EnableViewState="true" ShowToolbar="false" AjaxEnabled="true" ShowSideBar="false" ShowBackToSearchLink="false" ListCssClass="SubList" />
</uc1:DM_Listcontrol>

</div>
<script language="javascript">


    try {
        parent.SetHeader('<%=HeaderText%>');
    }
    catch (exc)
	{ }
	
	</script>


</asp:Content>