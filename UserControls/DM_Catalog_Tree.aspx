<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Catalog_Tree.aspx.vb"
    Inherits="DM_Catalog_Tree" Buffer="true" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Managed List - TreeView</title>
    
    <style type="text/css">         
            .LabelCell
        {
            min-width:0px!important;
        }
</style>  
    <script language="javascript">
        function CollapseExpandTreeNode(id) {
            try {
                if (document.getElementById('D' + id).style.display == 'block') {
                    document.getElementById('D' + id).style.display = 'none';
                    document.getElementById('I' + id).src = '../Images/ARRDOWN.gif';
                } else {
                    document.getElementById('D' + id).style.display = 'block';
                    document.getElementById('I' + id).src = '../Images/ARRUP.gif';
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
        function Goto(page) {
            frmTree.PAGE.value = page;
            frmTree.submit();
        }
        function UpdateYourSelection() {
            document.getElementById('lblSelection').innerHTML = parent.GetSelectedCaption();
        }
        function Select(val,desc) {      
          window.parent.Select(val,desc);
         UpdateYourSelection() ;
        }
        function SelectAndClose(val, desc) {
            window.parent.SelectAndClose(val, desc);
        }
       function ShowDetails(itemid) {
          parent.RefreshRightPane(itemid);
        }
        function NewItem(listid) {
          parent.NewRightPane(listid);
        }
        //new functions

        function Clear() {
            parent.ClearValue();
            UpdateYourSelection();
        }
        function Search() {

            theForm.PAGE.value = '';
            theForm.submit();
        }

        function SaveClose() { parent.SaveClose(); }    
              
    </script>
</head>
<body>
    <form id="frmTree" runat="server">
    <span runat="server" id="lbl"></span>
    <asp:LinkButton ID="lnkSearch" runat="server" OnClientClick="Search();return false;"></asp:LinkButton>
    <input id="txtSelectedID" type="hidden" runat="server" name="txtSelectedID" />
    <input type="HIDDEN" name="PAGE" value="1" />
    <%-- <input id="txtSelectedID" type="hidden" runat="server" name="txtSelectedID">--%>
    <table width="99%" align="CENTER" cellspacing="0" cellpadding="3">
        <tr>
            <td width='55%' valign='top' nowrap>
                <table class='Panel'>
                    <tr class='DetailHeader'>
                       
                        <td class='DetailHeaderContent' align='CENTER' colspan="2">
                            <%=GetLabel("search")%>
                        </td>
                       
                    </tr>
                    <tr>
                        <td class='FieldCell' align='CENTER' colspan="2">
                            <asp:TextBox ID="txtFilter" runat="server" style="width:200px;height:20px;"></asp:TextBox>
                            <%--   <input type="TEXT" size="50" name="fld_CAT_NAME" value="">--%>  <span class='buttons'>
                                <a href='JavaScript:Search();' class='regular'>
                                    <asp:Label ID="lblSearch" Text="Search" runat="server" /></a></span>
                        </td>                      
                    </tr>                    
                </table>
            </td>
            <td valign='top' width='45%' height='100%'>
                <table class='Panel'>
                    <tr class='DetailHeader'>
                       
                        <td class='DetailHeaderContent'>
                            <%=GetLabel("yourselection")%>
                        </td>
                      
                    </tr>
                    <tr>
                        
                        <td width='100%' valign='top'>
                            <span id="lblSelection" name="lblSelection"></span>                         
                        </td>
                       
                    </tr>
                    <tr >
                        <td nowrap valign='top' colspan='3' align='right'>
                            <div id="selectspan" runat="server">
                                <div class='buttons'>
                                    <a href='javascript:Clear();' class='regular'><%=GetLabel("clear")%></a></div>
                                <div class='buttons'>
                                    <a href='JavaScript:SaveClose();' class='regular'><%=GetLabel("Select")%></a></div>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <asp:PlaceHolder ID="pnlTree" runat="server"><span>
        <asp:Literal ID="TreeView" runat="server" EnableViewState="False"></asp:Literal>
    </span></asp:PlaceHolder>
    <asp:PlaceHolder ID="pnlList" runat="server">
        <div style="margin-left: 10px; width: 95%;">
            <asp:Literal ID="plhResults" runat="server">    
            </asp:Literal>
        </div>
    </asp:PlaceHolder>
    <asp:Literal ID="HiddenFields" runat="server" EnableViewState="False"></asp:Literal>
    <asp:Label ID="lblAutoSelect" runat="server"></asp:Label>
    <br />
    </form>
</body>
</html>
