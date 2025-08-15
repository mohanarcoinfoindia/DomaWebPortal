<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_EDIT_QUERY.aspx.vb" Inherits="Doma.DM_EDIT_QUERY" ValidateRequest="false" MasterPageFile="~/masterpages/Toolwindow.master" %>

<asp:Content ID="headcontent" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <style>
        .multipleRowsComboBox .rcbItem, .multipleRowsComboBox .rcbHovered {
            width: 49%;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
    <asp:Label Visible="false" ID="txtParentID" Runat="server"></asp:Label>
    <asp:Label Visible="False" ID="txtObjectID" Runat="server"></asp:Label>
    <asp:Label Visible="False" ID="txtOwner" Runat="server"></asp:Label>
    <div style="padding:5px;">
        <telerik:RadTabStrip runat="server" ID="radTab" SelectedIndex="0" MultiPageID="RadMultiPage1" Skin="Vista">
		    <Tabs>
			    <telerik:RadTab Text="Folder" PageViewID="pgFolder" />               						
			    <telerik:RadTab Text="Query" PageViewID="pgQuery" />							
		    </Tabs>
	    </telerik:RadTabStrip>


        <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" ScrollBars="Auto">
			<telerik:RadPageView ID="pgFolder" runat="server" Style="margin-top: 10px; margin-bottom: 20px;">

                <asp:Panel CssClass="container-fluid detail-form-container" runat="server" ID="container1">
                    <div class="row detail-form-row">
                        <div class="col-md-8 offset-md-4 FieldCell">
                            <asp:RadioButtonList ID="rdQryScope" runat="server" RepeatDirection="Horizontal" >
                                <asp:ListItem Value="1" Text="Public" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="0" Text="Private" Selected="false"></asp:ListItem>                        
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="lblFolder" runat="server" Text="Folder" CssClass="Label"></asp:Label>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:Label ID="lblParentName" runat="server"></asp:Label>
                        </div>
                    </div>
                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="lblName" Runat="server" CssClass="Label" AssociatedControlID="txtObjectName"/>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:TextBox Runat="server" ID="txtObjectName" Enabled="false" MaxLength="100" />
						    <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" ID="reqName" runat="server" ControlToValidate="txtobjectName" ValidationGroup="savefields" ErrorMessage=" *"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="Label2" Runat="server" Text="Result Screen" CssClass="Label" AssociatedControlID="cmbResultScreen"></asp:Label>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <Arco:ResultScreenSelectionField ID="cmbResultScreen" runat="server" />	
                        </div>
                    </div>
                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="Label3" Runat="server" Text="Icon" CssClass="Label" AssociatedControlID="cmbTreeIcon"></asp:Label>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <Arco:TreeIconSelectionField ID="cmbTreeIcon" runat="server" />
                        </div>
                    </div>
                </asp:Panel>
            </telerik:RadPageView>

            <telerik:RadPageView ID="pgQuery" runat="server" Style="margin-top: 10px; margin-bottom: 20px;">
                <asp:Panel runat="server" CssClass="container detail-form-container" ID="container2">
                    <div class="row detail-form-row">
                        <div class="col-md-8 offset-md-4 FieldCell">
                            <asp:RadioButtonList ID="optFolderToUse" runat="server" RepeatDirection="Horizontal" >
                                <asp:ListItem Value="0" Text="Root"></asp:ListItem>
                                <asp:ListItem Value="1" Text="The folder where the query is executed" Selected="true"></asp:ListItem>                      
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <asp:Panel CssClass="row detail-form-row" runat="server" ID="rowQueryName">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="Label5" Runat="server" CssClass="Label" AssociatedControlID="lblQueryName"/>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:Label ID="lblQueryName" runat="server" />
                        </div>
                    </asp:Panel>
                    <asp:Panel CssClass="row detail-form-row" runat="server" ID="rowQueryDescription">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="Label6" Runat="server" CssClass="Label" AssociatedControlID="lblQueryDesc"/>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:Label ID="lblQueryDesc" runat="server" />
                        </div>
                    </asp:Panel>
                    <asp:Panel runat="server" CssClass="row detail-form-row" ID="rowProp">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="Label1" Runat="server" Text="Property" CssClass="Label" AssociatedControlID="cmbPropID"></asp:Label>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:DropDownList Runat="server" ID="cmbPropID" Enabled="false"></asp:DropDownList>
						    <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" ID="RequiredFieldValidator1" runat="server"  ValidationGroup="savefields" ControlToValidate="cmbPropID" ErrorMessage="*"></asp:RequiredFieldValidator>
                        </div>
                    </asp:Panel>
                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="Label4" Runat="server" Text="Result type" CssClass="Label" AssociatedControlID="drpResType"></asp:Label>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:Label runat="server" ID="lblResType" />
						    <asp:DropDownList Runat="server" ID="drpResType" />
                        </div>
                    </div>
                </asp:Panel>

            </telerik:RadPageView>
        </telerik:RadMultiPage>

        <Arco:ButtonPanel ID="ButtonPanel1" runat="server" Style="margin-top: 20px;">
		    <Arco:OkButton ID="lnkSave" CommandName="SaveQuery" Runat="server" OnClick="DoSave" Visible="false"  ValidationGroup="savefields" ></Arco:OkButton>
		    <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" id="lnkCancel"></Arco:CancelButton>
	    </Arco:ButtonPanel>
			
        <asp:Label ID="lblMsgText" Visible="false" Runat="server" Text="Save ok." />
    </div>
    <script type="text/javascript">	
	
    <%if lsAction <> "" then%>
		
        if (window.opener.parent.RefreshTreeContent)
        {
		    window.opener.parent.RefreshTreeContent('<%=lsAction%>');
	    }
	    else
	    {
		    window.opener.RefreshTreeContent('<%=lsAction%>');
	    }
	    try{
        
        window.opener.Goto(window.opener.CurrentPage());        
        }
        catch(e)
        {
        }	
        Close();	
		
    <%end if%>
    </script>		
</asp:Content>