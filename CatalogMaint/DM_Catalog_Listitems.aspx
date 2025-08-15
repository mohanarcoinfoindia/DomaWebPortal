<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_Listitems.aspx.vb"
    Inherits="DM_Catalog_Listitems" ValidateRequest="false" EnableEventValidation="false" EnableViewState="true" %>

<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head2" runat="server">
    <title>Catalog Maintenance - Listitems</title>
    <style type="text/css">
        td > a.icon {
            margin: 0 2px;
        }
    </style>
    <script type="text/javascript">

        function SelectFilter(fieldname, initial) {
            if ($get(fieldname).value != initial) {
                this.Goto(1);
            }
        }

        function RefreshRightPaneLID2(listid, itemid, id) {
            parent.RefreshRightPaneLID2(listid, itemid, 'N', id);
        }

        function EditItem(itemid) {
            const width = document.body.scrollWidth * 0.60;
            const height = document.body.scrollHeight * 0.65;
            PC().OpenModalWindow('DM_Catalog_listitemdetail.aspx?id=' + itemid + '&rnd_str=' + Math.random(), true, width, height, true, false);
        }  

        function DeleteItem(itemid) {
            if (confirm(UIMessages[4])) {
                theForm.delItem.value = itemid;
                theForm.submit();
            }
        }

        function AddNewItem(listid) {
            const width = document.body.scrollWidth * 0.60;
            const height = document.body.scrollHeight * 0.65;
            PC().OpenModalWindow('DM_Catalog_listitemdetail.aspx?listid=' + listid + '&rnd_str=' + Math.random() + '', true, width, height, true, false);
        }

        function AddNewItemLID2(listid, itemid, id) {
            parent.RefreshRightPaneLID2(listid, itemid, '', id);
        }

        function AddListItemCategoryLID2(listid, itemid, id) {
            parent.RefreshRightPaneLID2(listid, itemid, 'C', id);
        }

        function AddListItemCategory(listid) {
            PC().OpenModalWindow('DM_Catalog_listaddcategory.aspx?listid=' + listid + '&rnd_str=' + Math.random() + '', true, 700, 600, true, true);
        }

        function Goto(page) {
            theForm.PAGE.value = page;
            theForm.submit();
        }

        function OrderBy(field) {
            if (field == theForm.elements[orderbyField].value) {
                if (theForm.elements[orderbyorderField].value == "DESC") {
                    theForm.elements[orderbyorderField].value = "ASC";
                }
                else {
                    theForm.elements[orderbyorderField].value = "DESC";
                }
            }
            else {
                theForm.elements[orderbyorderField].value = "ASC";
            }
            theForm.elements[orderbyField].value = field;
            theForm.submit();
        }

    </script>
</head>
<body>
    <script type="text/javascript">
        var orderbyField = '<%=orderby.name %>';
        var orderbyorderField = '<%=orderbyorder.name %>';
    </script>
    <form id="Form1" method="post" runat="server" defaultbutton="lnkEnter">
        <asp:LinkButton ID="lnkEnter" runat="server" OnClientClick="Goto(1);return false;"></asp:LinkButton>
        <input type="hidden" name="PAGE" value="<%=liPage%>" />
        <input type="hidden" name="delItem" value="" />
        <input type="hidden" name="orderby" id="orderby" runat="server" />
        <input type="hidden" name="orderbyorder" id="orderbyorder" runat="server" />

        <Arco:PageController ID="thisPageController" runat="server" PageLocation="ListPage" />
        <div>
            <asp:Literal ID="plhResults" runat="server"></asp:Literal>
        </div>
    </form>
</body>
</html>
