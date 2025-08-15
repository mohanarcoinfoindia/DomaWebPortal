<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MainToolbar.ascx.vb" Inherits="UserControls_MainToolbar" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radTlb" %>

<radtlb:radtoolbar id="RadToolbarMain" runat="server"  UseFadeEffect="false" Visible="false" OnClientButtonClicking="onToolbarClick">
    <items>
       
    </items>
</radtlb:radtoolbar>

<script language="javascript">
function onToolbarClick(sender, args)
{
	var command='';
	var arguments='';	
	var cn = args.get_item().get_commandName();
	if (cn.indexOf("_") > 0)
	{
	   	var arrComm = cn.split("_");
	   	command = arrComm[0];
	   	arguments = arrComm[1];
	}
	else 
	{
	    command = cn;
	}
	
	switch (command)
	{
	    case 'DOCUMENT':
	    case 'FOLDER':	        
	        NewObject(arguments);
	        break;
	    case 'FIND':
	    case 'SEARCH':
	        Goto('DM_DOCUMENTLIST.aspx?screenmode=query&DM_SCREEN_ID=' + arguments,0,0);
	        break;
	    case 'RUNQUERY':
	        Goto('DM_DOCUMENTLIST.aspx?screenmode=advsearch&LOADQRY=Y&QRY_ID=' + arguments,0,0);
	        break;	    
	    case 'MAIL':
	        //window.open('./MailClient/Compose.aspx','NewMail','width=800,height=600,scrollbars=yes,resizable=yes');
	        window.open('./MailClient/Compose2.aspx','NewMail','width=800,height=600,scrollbars=yes,resizable=yes');
	        break;
	    case 'DOSSIER':
	        GotoWorkFlowPage('WF_Procs.asp',3);
	        break;
	    case 'ADDFAVORITE':
	        AddToFavorites();
	        break;
	    case 'REMOVEFAVORITE':
	        RemoveFromFavorites();
	        break;
	    case 'DOWNLOADZIP':	        
	        DoDocumentListAction(2);
	        break;
	    case 'ADDBASKET':
	        AddToBasket();
	        break;
	    case 'REMOVEBASKET':
	        RemoveFromBasket();
	        break;
	    case 'CLEARFAVORITES':
	        ClearFavorites();
	        break;
	    case 'CLEARBASKET':
	        ClearBasket();
	        break;
	    case 'JAVASCRIPT':
	        eval(arguments);
	        break;
	    case 'OPENLINK':
		    Goto(arguments,0,0);
	        break;	    
        case 'OPENWINDOW':
            window.open(arguments,'NewMail','width=800,height=600,scrollbars=yes,resizable=yes');
	        break;
	}
	args.set_cancel(true);
	
}
</script>