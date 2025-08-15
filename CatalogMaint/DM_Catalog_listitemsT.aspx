<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_listitemsT.aspx.vb"
    Inherits="DM_Catalog_listitemsT" %>
<!DOCTYPE HTML>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>List - TreeView</title>    
    <script language="javascript">

        function RefreshRightPaneLID2(listid, itemid, tab, id) {
            try {
                var strSids = ob_AllIds(id);
                parent.RefreshRightPaneLID2(listid, itemid, tab, strSids);
            } catch (err) {
                // Handle errors here.
            }
        }

        function RefreshRightPane(listid) {
            try {
                parent.RefreshRightPane(listid);
            } catch (err) {
                // Handle errors here.
            }
        }
        function RefreshLeftPane(listid, itemid, seletedids) {
            try {
                parent.RefreshLeftPane(listid, itemid, seletedids);
            } catch (err) {
                // Handle errors here.
            }
        }
        function CollapseExpandTreeNode(id) {
            try {
                if ($get('D' + id).style.display == 'block') {
                    $get('D' + id).style.display = 'none';
                    $get('I' + id).src = '../Images/ARRDOWN.gif';
                } else {
                    $get('D' + id).style.display = 'block';
                    $get('I' + id).src = '../Images/ARRUP.gif';
                }
            } catch (err) {
                // Handle errors here.
            }
        }

        function ExpandAll() {
            try {
                dvs = document.getElementsByTagName('div');
                for (c = 0; c < dvs.length; c++) {
                    if (dvs[c].style.display == 'none') {
                        id = dvs[c].id.substring(1);
                        CollapseExpandTreeNode(id);
                    }
                }
            } catch (err) {
                // Handle errors here.
            }
        }

        function CollapseAll() {
            try {
                dvs = document.getElementsByTagName('div');
                for (c = 0; c < dvs.length; c++) {
                    if (dvs[c].style.display == 'block') {
                        id = dvs[c].id.substring(1);
                        CollapseExpandTreeNode(id);
                    }
                }
            } catch (err) {
                // Handle errors here.
            }
        }

        function ob_AllIds(id) {
            var strSids = "";
            var oNode = $get(id);
            oNode = ob_getParentOfNode(oNode);
            while (oNode != null) {
                if (strSids != "") {
                    strSids = oNode.id + "," + strSids;
                } else {
                    strSids = oNode.id;
                }
                oNode = ob_getParentOfNode(oNode);
            }
            if (strSids != "") {
                strSids += "," + id;
            }
            return strSids;
        }

        var arr_ob_t2_IndexedNodes = null, arr_ob_t2_NodesParents = null, arr_ob_t2_NodesLevels = null;
        function ob_t2_ExpandCollapseLevel(iLevel, bType) {
            // the first time the function is executed
            // the levels of the nodes must be calculated
            if (arr_ob_t2_NodesLevels == null) {
                arr_ob_t2_IndexedNodes = new Array();
                arr_ob_t2_NodesParents = new Array();
                arr_ob_t2_NodesLevels = new Array();

                var oTempNode = ob_getFirstNodeOfTree();
                var oTempParent = null;
                var i = 0;
                while (oTempNode != null) {
                    arr_ob_t2_IndexedNodes[i] = oTempNode.id;

                    oTempParent = ob_getParentOfNode(oTempNode);
                    if (oTempParent) {                       
                        arr_ob_t2_NodesParents[oTempNode.id] = oTempParent.id;
                    }
                    oTempNode = ob_getNodeDown(oTempNode, true);
                    i++;
                }

                for (i = 0; i < arr_ob_t2_IndexedNodes.length; i++) {
                    var iTempLevel = 0;
                    var sTempParent = arr_ob_t2_NodesParents[arr_ob_t2_IndexedNodes[i]];
                    while (sTempParent != null) {
                        iTempLevel++;
                        sTempParent = arr_ob_t2_NodesParents[sTempParent];
                    }
                    arr_ob_t2_NodesLevels[arr_ob_t2_IndexedNodes[i]] = iTempLevel;
                }
            }

            for (var i = 0; i < arr_ob_t2_IndexedNodes.length; i++) {
                if (arr_ob_t2_NodesLevels[arr_ob_t2_IndexedNodes[i]] == iLevel) {
                    oImg = $get(arr_ob_t2_IndexedNodes[i]).parentNode.firstChild.firstChild;
                    var lensrc = (oImg.src.length - 8);
                    var s = oImg.src.substr(lensrc, 8);
                    if (bType == true) {
                        if ((s == "usik.gif") || (s == "ik_l.gif")) {
                            oImg.onclick();                           
                        }
                    }
                    else if ((s == "inus.gif") || (s == "us_l.gif")) {
                        oImg.onclick();                        
                    }
                }
            }

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
            var mnu = $find("CM_Tree");
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
        function DeleteItem(itemid) {
            if (confirm(UIMessages[4])) {
                form1.delItem.value = itemid;
                form1.submit();
            }    
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">

    <telerik:RadScriptManager runat="server" ID="sc1" EnablePartialRendering="true" EnablePageMethods="true"/> 
     <input type="hidden" name="delItem" value="" />
    <asp:HiddenField ID="txtSelectedID" runat="server" />
    <asp:HiddenField ID="txtListID" runat="server" />
   
      <telerik:RadContextMenu runat="server" ID="CM_Tree" OnClientItemClicked="DMC"   >
        <Items>
            <telerik:RadMenuItem  Value="ChildrenCommand" ImageUrl="../TreeIcons/Icons/triangle_blueS.gif"  />           
            <telerik:RadMenuItem  Value="EditCommand" ImageUrl="../Images/edit.png"  />
            <telerik:RadMenuItem  Value="DeleteCommand" ImageUrl="../Images/delete.png"  />
        </Items>
    </telerik:RadContextMenu>


    <table width="97%" align="CENTER" cellspacing="0" cellpadding="5">
        <tr>
            <td width='auto' valign='top'>
                <table class='Panel'>
                    <tr class='DetailHeader'>
                        
                        <td class='DetailHeaderContent' align='CENTER'>
                            <asp:Label ID="lblList" runat="server"></asp:Label>
                        </td>
                       
                    </tr>
                    <tr>
                       
                        <td class='FieldCell' align='CENTER'>
                            <asp:Label ID="lblSearch" runat="server">  <%=GetLabel("search")%></asp:Label>&nbsp;:&nbsp;
                            <asp:TextBox ID="txtFilter" runat="server" Style="width: 180px;"></asp:TextBox>&nbsp;
                            <asp:ImageButton ID="cmdFilter" ImageUrl="../Images/find.png" runat="server" ToolTip="Find" />
                        </td>
                        
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td width='auto' valign='top'>
                <div id="treediv" class="webpanelcontent" style="padding: 0 0 0 5px;">
                    <div id="TreeViewDiv" runat="server" style="padding: 2px 0 0 10px;">
                        <asp:Literal ID="TreeView" runat="server" EnableViewState="False"></asp:Literal>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
<script language="javascript">
    //OPGELET: DIT MOET -->NA<-- DE HTML TAG GEPLAATST WORDEN!!!

    function onSucceed(result) {
        //  alert('success ' + result);      
    }
    function onError(result) {
        alert(result.get_message());
    }

    function ob_OnBeforeNodeDrop(src, dst, copy) {
        PageMethods.OnBeforeNodeDrop(src, dst, copy, onSucceed, onError);
        return true;       
    }

    function ob_OnNodeDrop(src, dst, copy) {
     
        var listid = GetListID();
        var itemid = src.substr(src.indexOf('_') + 1);
        if (tree_selected_id != null) {
            ob_SelectedId(src);
            RefreshRightPaneLID2(listid, itemid, 'N', src);
        }
        else {
            RefreshRightPaneLID2(listid, itemid, 'N', src);
            ob_SelectedId(src);
        }

    }



</script>
