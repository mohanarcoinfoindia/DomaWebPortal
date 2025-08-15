<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SelectFolder.ascx.vb" Inherits="UserControls_SelectFolderOLD" %>
<%@ Register TagPrefix="Arco" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls" %>

<script language="javascript">
function <%=me.clientid %>_OpenSelection()
{
var sFeatures = "";

sFeatures += "height=400,width=600,";
sFeatures += "resizable=yes,scrollbars=yes";	

var w = window.open("<%=WindowUrl%>","selectfolder",sFeatures);
w.focus();
}
function SetFolderSelection(id,name)
{
document.forms[0].elements['<%=txtFolderID.clientID %>'].value = id;
try
{
    document.forms[0].elements['<%=txtFolderName.clientID %>'].value = name;
}
catch(e)
{
}
}

function GetFolderSelection()
{
    return document.forms[0].elements['<%=txtFolderID.UniqueID %>'].value;
}
</script>
<asp:HiddenField ID="txtFolderID" runat="server" EnableViewState="true"/>
<asp:Textbox ID="txtFolderName" runat="server" EnableViewState="true" width="300px" ></asp:Textbox>
<Arco:FolderLink runat="server" ID="pnlFolderLink" JavaScriptOpenFunction="" ShowFolderEasyBrowser="false"/>
&nbsp;<asp:HyperLink runat="server" ID="lnkSelect" NavigateUrl="javascript:OpenSelection();" Text="Select" CssClass="ButtonLink"></asp:HyperLink>
