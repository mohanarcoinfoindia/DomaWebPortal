<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_NEW_CASE.aspx.vb" Inherits="DM_NEW_CASE" MasterPageFile="~/masterpages/ToolWindow.master" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">

		<asp:Label Visible="False" ID="txtParentID" Runat="server"></asp:Label>
			<asp:Label Visible="False" ID="txtProcID" Runat="server"></asp:Label>
			<asp:Label Visible="False" ID="txtCaseID" Runat="server"></asp:Label>
            <asp:Label Visible="False" ID="txtObjectID" Runat="server"></asp:Label>			
			<asp:Label Visible="False" ID="txtPackID" Runat="server"></asp:Label>

			<center><div style="margin:10px;max-width:600px">
			
			<asp:Table CssClass="Panel dmNewObject" HorizontalAlign="Center" Runat="server" id="Table1">
				<asp:TableRow CssClass="DetailHeader">				   
					<asp:TableCell ColumnSpan="2" CssClass="DetailHeaderContent">
							<asp:Label ID="txtTitle" Runat="server"></asp:Label>
					</asp:TableCell>					
				</asp:TableRow>
				
				<asp:TableRow>
                    <asp:TableCell  ColumnSpan="2">
                        <div class="container-fluid detail-form-container">
                            <div class="row detail-form-row">
                                <div class="col-md-4 LabelCell">
                                    <asp:Label ID="lblName" Runat="server" CssClass="Label" AssociatedControlID="txtCaseName"></asp:Label>
                                </div>
                                <div class="col-md-8 FieldCell">
                                    <asp:TextBox Runat="server" ID="txtCaseName"  />
						            <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ValidationGroup="CheckName" ID="reqName" ControlToValidate="txtCaseName" ErrorMessage="*" SetFocusOnError="true"></asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="row detail-form-row">
                                <div class="col-md-4 LabelCell">
                                    <asp:Label ID="lblDesc" Runat="server" CssClass="Label" AssociatedControlID="txtCaseDesc"></asp:Label>
                                </div>
                                <div class="col-md-8 FieldCell">
                                    <asp:TextBox Runat="server" ID="txtCaseDesc" TextMode="MultiLine" Rows="5" MaxLength="2000"  />
                                </div>
                            </div>
                            <div class="row detail-form-row mt-4">
                                <div class="col-md-4"></div>
                                <div class="col-md-8 FieldCell">
                                    <Arco:ButtonPanel id="pnlButtons" runat="server">
                                        <Arco:OkButton ID="lnkSave" OnClientClick="ProgressIndicator.display();" DoubleClickProtection="true" Runat="server" OnClick="DoSave" ValidationGroup="CheckName"></Arco:OkButton>
                                        <Arco:CancelButton ID="lnkCancel" runat="server" OnClientClick="javascript:Close();"></Arco:CancelButton>
                                    </Arco:ButtonPanel> 
                                </div>
                            </div>
                        </div>
                    </asp:TableCell>				
				</asp:TableRow>
			</asp:Table>			
			</div></center>
				
			<asp:Literal ID="plValidationScript" Runat="server"></asp:Literal>
	
    </asp:Content>
