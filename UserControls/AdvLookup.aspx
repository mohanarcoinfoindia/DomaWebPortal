<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AdvLookup.aspx.vb" Inherits="UserControls_AdvLookup" MasterPageFile="~/masterpages/Toolwindow.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">

    <script type="text/javascript">
        function RefreshOpener() {
            MainPage().GetContentWindow().RefreshOnChange();
        }
        function SaveClose() {
            <%if mbRefreshOnChange Then%>
             RefreshOpener();
            <%end if%>
        Close();
        }

        function Goto(page) {
            theForm.PAGE.value = page;
            theForm.submit();
        }
        function OrderBy(order,orderbyorder) {
            theForm.ORDERBY.value = order;
            theForm.ORDERBYORDER.value = orderbyorder;
            theForm.submit();
        }

        function Search() {
            theForm.PAGE.value = '';
            theForm.submit();
        }
        function AutoSelect(val,desc) {
            MainPage().GetContentWindow().AddValue<%=JSIdentifier %>(val, desc);

            SaveClose();

        }
        function Select(val,desc) {

            MainPage().GetContentWindow().AddValue<%=JSIdentifier %>(val, desc);
    UpdateYourSelection();
	<%if not mbMultiSelect OrElse mbAutoSelect Then%>
        SaveClose();
<%          End If %>
        }

        function Clear() {
            MainPage().GetContentWindow().ClearValue<%=JSIdentifier %>();
    UpdateYourSelection();
<%if not mbMultiSelect Then%>
        SaveClose();
<%end if%>

        }

        function Remove(val) {
<%if mbMultiSelect Then%>
    MainPage().GetContentWindow().oProp_<%=JSIdentifier %>.RemoveValue(val);
    UpdateYourSelection();
<%else%>
        Clear();
<%end if%>

        }
      
        function UpdateYourSelection() {
            document.getElementById('lblSelection').innerHTML = "";
            var selArr = MainPage().GetContentWindow().oProp_<%=JSIdentifier %>.GetCaption().split("<br/>");
        var valArr = MainPage().GetContentWindow().oProp_<%=JSIdentifier %>.GetValue().split(", ");
        for (var i = 0; i < selArr.length; i++) {

        <%if mbMultiSelect Then%>
            var removelink = document.createElement("a");
            removelink.appendChild(document.createTextNode(selArr[i]));
            removelink.href = "javascript:Remove('" + valArr[i] + "');";
            removelink.title = "<%=getLabel("remove") %>";
            // removelink.setAttribute("class","ButtonLink");
            document.getElementById('lblSelection').appendChild(removelink);
            document.getElementById('lblSelection').appendChild(document.createElement("br"));
        <%else %>
                document.getElementById('lblSelection').appendChild(document.createTextNode(selArr[i]));
        <%end if %>

        }
        }

    </script>
    <asp:LinkButton ID="lnkEnter" runat="server" OnClientClick="Search();return false;"></asp:LinkButton>

   
    <input type="hidden" name="PAGE" value="<%=liPage%>" />
    <input type="hidden" name="SEARCH" value="Y" />
    <input type="hidden" name="ORDERBY" value="<%=_orderBy%>" />
    <input type="hidden" name="ORDERBYORDER" />

    <asp:Literal ID="plhSearch" runat="server">

    </asp:Literal>
    <asp:Literal ID="plhResults" runat="server">
            
    </asp:Literal>


</asp:Content>
