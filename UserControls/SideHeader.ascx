<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SideHeader.ascx.vb" Inherits="UserControls_SideHeader" %>

<div class="SideHeader">
    <div style="padding-top:20px;padding-left:2px;">
  <asp:Repeater ID="repLvl1" runat="server" DataSourceID="smds1" Visible="true" >
            <HeaderTemplate>
                
            </HeaderTemplate>
            <FooterTemplate>
           
            </FooterTemplate>
            </asp:Repeater>
    </div>
</div>
  <asp:SiteMapDataSource runat="server" ID="smds1" ShowStartingNode="false" StartingNodeOffset="0" />