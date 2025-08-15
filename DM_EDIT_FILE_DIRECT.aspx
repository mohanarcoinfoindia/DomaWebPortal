<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_EDIT_FILE_DIRECT.aspx.vb" Inherits="DM_EDIT_FILE_DIRECT"  ValidateRequest="false" MasterPageFile="~/masterpages/Toolwindow.master" %>
<%@ register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radE"  %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server"> 
     <style type="text/css">
        .ContentPanel {
            padding: 0px 10px 10px 10px;
            margin: 20px
        }

    </style>
    <script type="text/javascript">
        function RefreshOpener() {
            if (MainPage()) {

                try {                    
                    MainPage().RefreshParentPage();
                    MainPage().Refresh();
                }
                catch (e) {
                    try {
                        
                        MainPage().ReloadParent(<%=OpenFileAfterSave %>);
                    }
                    catch (e) {
                        try {
                            MainPage().UncheckedSave();
                        }
                        catch (e) {

                        }
                    }

                }
            }
        }
  
    </script>
    <asp:Panel ID="pnlEdit" runat="server" CssClass="pnlEdit">
        <asp:HiddenField ID="txtFileID" runat="server" />
        <asp:HiddenField ID="txtObjectID" runat="server" />
        <asp:HiddenField ID="txtFileType" runat="server" />
         <asp:HiddenField ID="txtHash" runat="server" />

        <asp:table ID="tblMain" runat="server" CssClass="Panel" HorizontalAlign="center">
         <asp:TableRow CssClass="PanelRow" ID="rowEditConflict">
         <asp:TableCell ColumnSpan="2">
            <asp:Label runat="server" ID="lblConflict" CssClass="ErrorLabel" Text="Conflict detected, save anyways?"></asp:Label>
         </asp:TableCell>

         </asp:TableRow>
        <asp:TableRow CssClass="PanelRow">
        <asp:TableCell HorizontalAlign="left" Width="100%" ColumnSpan="2">
             <div id="textEditor" class="ContentPanel">
        <asp:TextBox ID="txtFileContent" runat="server" TextMode="MultiLine" width="100%" rows="20" EnableTheming="false"></asp:TextBox>
        <radE:RadEditor ID="txtHTMLContent"  runat="server" SkinID="EditHtmlFile" ></radE:RadEditor>
        <radE:RadEditor ID="txtHTMLField"  runat="server" SkinID="EditHtmlField" ></radE:RadEditor>
                 </div>
        </asp:TableCell>
        
        </asp:TableRow>        
        <asp:TableRow CssClass="PanelRow">
            <asp:TableCell>
                 <ArcoControls:CheckBox runat="server" ID="chkPrivate" />
            </asp:TableCell>
            <asp:TableCell  HorizontalAlign="Right">
        
        <Arco:ButtonPanel runat="server" ID="btnPanel" >
						    <Arco:OKButton runat="server" ID="cmdSave" DoubleClickProtection="true" Text="Save" Enabled="true"> </Arco:OKButton>
						    <Arco:SecondaryButton ID="cmdSaveAndClose" DoubleClickProtection="true" runat="server" Text="Save and Close"></Arco:SecondaryButton> 
                            <Arco:CancelButton runat="server" Text="Cancel" ID="cmdCancel"></Arco:CancelButton>

						    <Arco:OKButton runat="server" ID="cmdSaveForce" DoubleClickProtection="true" Text="Save" visible="false" > </Arco:OKButton>
                            <Arco:SecondaryButton ID="cmdSaveCloseForce" DoubleClickProtection="true" runat="server" Text="Save and Close" visible="false" ></Arco:SecondaryButton> 

						</Arco:ButtonPanel>
               
        </asp:TableCell></asp:TableRow>
        
        </asp:table>
        
        <Arco:PageFooter id="lblFooter" runat="server"/>
        
       
    </asp:Panel>
  
</asp:Content>
