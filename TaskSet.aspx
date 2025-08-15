<%@ Page Language="VB" MasterPageFile="~/masterpages/ToolWindow.master" AutoEventWireup="false" CodeFile="TaskSet.aspx.vb" Inherits="TaskSet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">

    <asp:PlaceHolder ID="plhName" runat="server">

        <div class="container detail-form-container">
            <div class="row detail-form-row">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblName" runat="server" CssClass="Label"></asp:Label>
                </div>
                <div class="col-md-8 FieldCell">
                    <asp:TextBox ID="txtName" runat="server" ValidationGroup="reqName"/>
                    <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" ID="reqName" runat="server" Text="*" ControlToValidate="txtName" ValidationGroup="reqName"></asp:RequiredFieldValidator>
                </div>
            </div>
               <div class="row detail-form-row">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblDesc" runat="server" CssClass="Label"></asp:Label>
                </div>
                <div class="col-md-8 FieldCell">
                    <asp:TextBox ID="txtDesc" runat="server" TextMode="MultiLine" Rows="3"/>                    
                </div>
            </div>
            <div class="row detail-form-row mt-3">
                <div class="col-md-8 offset-md-4 FieldCell">
                    <Arco:ButtonPanel ID="ButtonPanel1" runat="server">
                        <Arco:OkButton runat="server" ID="btnOk" ValidationGroup="reqName"></Arco:OkButton>
                        <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" ID="btnCancel"></Arco:CancelButton>
                    </Arco:ButtonPanel>
                </div>
            </div>
            <asp:PlaceHolder ID="plhItem" runat="server">
                <Arco:TaskSetItemsField runat="server" ID="lstSet" />
            </asp:PlaceHolder>
        </div>
    </asp:PlaceHolder>

</asp:Content>

