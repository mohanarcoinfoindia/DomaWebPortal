<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DM_ListControlGridScroller.ascx.vb" Inherits="UserControls_DM_ListControlGridScroller" %>
<div class="listControlGridScroller">
<table class="List"  >
<tr <%=me.rowcssclass %>>
<%if NumberOfResults > 0 then%>
    <td  align="<%=Align.tostring %>" nowrap width="100%">       
        <asp:HyperLink runat="server" ID="lnkPrev" SkinID="PrevPage"/>
        <asp:Literal ID="litScroller" runat="server"/>         
        <asp:HyperLink runat="server" ID="lnkNext" SkinID="NextPage"/>             
     &nbsp;&nbsp;<%="(" & NumberOfResultsLabel & " " & GetLabel("resultsfound") & ")"%>

    </td>
    

<%else %>

    <td  width="100%">&nbsp;</td>   
<%end if%>

<td align="right" nowrap>
    <asp:HyperLink NavigateUrl="javascript:BackToSearch();" runat="server" ID="lnkBackToSearch" Text="Back To Search" CssClass="ButtonLink"></asp:HyperLink>
    </td>

</tr>
</table>	
</div>