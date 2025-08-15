<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_UserBrowser_LeftPane.aspx.vb" Inherits="DM_UserBrowser_LeftPane" %>

<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<%@ Register TagPrefix="oajax" Namespace="OboutInc" Assembly="obout_AJAXPage" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">  
    <script type="text/javascript">
        function OpenUrl(url) {
            if (url.indexOf("&rnd_str=") < 0) {
                if (url.indexOf('?') > 0) {
                    url = url + '&rnd_str=' + Math.random();
                }
                else {
                    url = url + '?rnd_str=' + Math.random();
                }
               
            }
            parent.ShowRightPane(url);
        }

        function ShowUrl(url, login, isNew, id, isStructured) {
            url = url + '?login=' + login + '&new=' + isNew + '&parent=' + id + '&structured=' + isStructured + '&popup=false&radtab='
            OpenUrl(url);
        }
     
        function Delete(roleid) {           
            if (confirm(UIMessages[4])) {
                window.location = 'DM_UserBrowser_LeftPane.aspx?roleid=' + roleid + '&delete=1';
            }
        }
        function RefreshLeftPane() {
            location.href = "DM_UserBrowser_LeftPane.aspx";
            //window.open('DM_UserBrowser_LeftPane.aspx', '_self');
        }
       
        function ShowContextMenu() {                    
            var src;
            var e = window.event || arguments.callee.caller.arguments[0];
            if (e.srcElement) {
                src = e.srcElement;
            } else {
                src = e.target;
            }
           
            while (src.tagName != "TD" && src.tagName != "TABLE" && src.tagName != "BODY")
            { src = src.parentNode; }

            if (src.tagName != "TD") {
                e.cancelBubble = true;
                return false;
            }

            try {
                ob_t22(src, e);
            } //on contextmenu on the treeview -> highlight and select the node
            catch (ex)
	        { }

	        var id = $get("txtSelectedID").value;
	      
	        var mnuId = "";
	        switch (id) {
	            case "":
	            case "root":
	            case "t_groups":
	            case "t_delegates":
	            case "t_tenants":
                case "t_clients":
                case "t_defaultprefs": 
                case "t_namedweb":
                case "t_namedwebapi":
	                mnuId = "";
	                break;
	            case "t_users":	              
	                mnuId = "CM_Users";
	                break;
	            case "t_roles":
	                mnuId = "CM_Roles";
	                break;
	            case "t_hroles":
	                mnuId = "CM_HRoles";
	                break;
	            default:	                
	                mnuId = "CM_TreeRoles";
	                break;
	        }	       
	        if (mnuId != "") {
	            var mnu = $find(mnuId);
	            if (mnu) {
	                mnu.show(e);
	                e.cancelBubble = true;
	                return false;
	            }
	            else {
	                e.cancelBubble = false;
	                return true;
	            }
	        }
        }
        

    </script>
</head>
<body >
    <form id="Form1" runat="server">
    <telerik:RadScriptManager runat="server" ID="sc1" EnablePartialRendering="true" EnablePageMethods="true"/>   
   
    <asp:HiddenField ID="txtSelectedID" runat="server" />        

     <telerik:RadContextMenu runat="server" ID="CM_TreeRoles" OnClientItemClicked="DMC"   >
        <Items>
            <telerik:RadMenuItem  Value="hrCreateCommand" ImageUrl="../TreeIcons/Icons/triangle_blueS.gif"  />
            <telerik:RadMenuItem IsSeparator="true"></telerik:RadMenuItem>
            <telerik:RadMenuItem  Value="hrDetailsCommand"  />
            <telerik:RadMenuItem  Value="hrDeleteCommand"  />
        </Items>
    </telerik:RadContextMenu>

     <telerik:RadContextMenu runat="server" ID="CM_Users">
        <Items>
            <telerik:RadMenuItem ImageUrl="../TreeIcons/Icons/triangle_blueS.gif" NavigateUrl="javascript:OpenUrl('DM_UserBrowser_AddUser.aspx?new=true');"  />          
        </Items>
    </telerik:RadContextMenu>
     <telerik:RadContextMenu runat="server" ID="CM_Roles">
        <Items>
            <telerik:RadMenuItem ImageUrl="../TreeIcons/Icons/triangle_blueS.gif" NavigateUrl="javascript:OpenUrl('DM_UserBrowser_RoleDetail.aspx');"  />          
        </Items>
    </telerik:RadContextMenu>
     <telerik:RadContextMenu runat="server" ID="CM_HRoles">
        <Items>
            <telerik:RadMenuItem ImageUrl="../TreeIcons/Icons/triangle_blueS.gif" NavigateUrl="javascript:ShowUrl('DM_UserBrowser_RoleDetail.aspx','',true,0,true);"  />          
            <telerik:RadMenuItem NavigateUrl="javascript:RefreshLeftPane();"  />          
        </Items>
    </telerik:RadContextMenu>
    <telerik:RadScriptBlock ID="RadScriptBlock1" runat="server">
        <script type="text/javascript">
        
           
            function DMC(sender, args) {
                var CommandName = args.get_item().get_value();
                var id = new String($get("txtSelectedID").value);
                 switch (CommandName) {                                       
                    case "hrCreateCommand":
                        ShowUrl('DM_UserBrowser_RoleDetail.aspx', '', true, id, true);
                        break;
                    case "hrDetailsCommand":
                        ShowUrl('DM_UserBrowser_RoleDetail.aspx', id, false, '', true);
                        break;
                     case "hrDeleteCommand":                         
                         setTimeout(Delete, 500, id);                        
                        break;                                                                  
                    default:
                        break;
                }
             
            }
                            
        </script>
    </telerik:RadScriptBlock>
     
    <asp:Table runat="server">
        <asp:TableRow ID="trRoleHierarchy" runat="server">
            <asp:TableCell ColumnSpan="2">
                <asp:PlaceHolder ID="pnlTree" runat="server">
                <span oncontextmenu="return ShowContextMenu();">
                    <asp:Literal ID="TreeView" runat="server" EnableViewState="False"></asp:Literal>
                    </span>
                </asp:PlaceHolder>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    </form>
</body>
</html>
<script language="javascript" type="text/javascript">
    //OPGELET: DIT MOET -->NA<-- DE HTML TAG GEPLAATST WORDEN!!!
    function onSucceed(result)
    {
     
    }
    function onError(result) {     
        alert(result.get_message());        
    }
    function ob_OnBeforeNodeDrop(src, dst, copy) {      
        if (!copy) {
            PageMethods.OnBeforeNodeDrop(src, dst, copy, onSucceed, onError);
            return true;
        }
        else {        
            return false;
        }

    }

    function ob_OnNodeDrop(src, dst, copy) {

      //  performSorting(src);
        
        if (tree_selected_id != null) {
            ob_SelectedId(src);
            ShowUrl('DM_UserBrowser_RoleDetail.aspx', src, false, '', true)
        }
        else {
            ShowUrl('DM_UserBrowser_RoleDetail.aspx', src, false, '', true)
            ob_SelectedId(src);
        }
    }
  

</script>
