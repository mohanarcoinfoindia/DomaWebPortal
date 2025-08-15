<%@ Page Language="VB" AutoEventWireup="false" CodeFile="MainPageEdit.aspx.vb" Inherits="MainPageEdit"  MasterPageFile="~/masterpages/Toolwindow.master" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
      <asp:Label ID="lblMsgText" runat="server" Visible="false" />

    <div class="container-fluid detail-form-container">
          <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label Text="ID" runat="server" CssClass="Label"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:Label ID="lblId" runat="server" CssClass="Label"></asp:Label>
            </div>
        </div>
        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblName" runat="server" CssClass="Label" AssociatedControlID="txtName"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox MaxLength="200" runat="server" ID="txtName" />
                <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ID="reqName"
                    ControlToValidate="txtName" ErrorMessage="*" SetFocusOnError="true" />
            </div>
        </div>
         <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label Text="Url" runat="server" CssClass="Label"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:Label ID="lblUrl" runat="server" CssClass="Label"></asp:Label>
            </div>
        </div>
        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblDescription" runat="server" CssClass="Label" AssociatedControlID="txtDescription" />
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox MaxLength="2000" runat="server" SkinID="MultiLine" ID="txtDescription"
                    TextMode="MultiLine" Rows="5" />
            </div>
        </div>

           <div class="row detail-form-row">      
               <div class="col-md-4 LabelCell">
                <asp:Label ID="lblOptions" runat="server" CssClass="Label" Text="Options"/>
            </div>
            <div class="col-md-8 FieldCell">
               
                <ArcoControls:CheckBox runat="server" ID="chkShowTree" />            
                 &nbsp;
                <ArcoControls:CheckBox runat="server" ID="chkShowBreadcrumb" />       
                    &nbsp;
                <ArcoControls:CheckBox runat="server" ID="chkShowGlobalSearch" />     
            </div>
        </div>
        
         <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblMenu" runat="server" CssClass="Label" AssociatedControlID="txtMenu"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox runat="server" ID="txtMenu" />              
            </div>
        </div>
         <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblDefaultAction" runat="server" CssClass="Label" AssociatedControlID="txtDefaultAction"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox runat="server" ID="txtDefaultAction" />              
            </div>
        </div>
            <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblScreenMode" runat="server" CssClass="Label" AssociatedControlID="txtScreenmode"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox runat="server" ID="txtScreenMode" />              
            </div>
        </div>
        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblExtraQueryString" runat="server" CssClass="Label" AssociatedControlID="txtExtraQueryString" />
            </div>
            <div class="col-md-8 FieldCell">
                 <asp:TextBox MaxLength="200" runat="server" ID="txtExtraQueryString" />                   
            </div>
        </div>

        <div class="row detail-form-row" runat="server">
            <div class="col-md-12 LabelCell">
                <Arco:ButtonPanel ID="ButtonPanel1" runat="server">
                    <Arco:OkButton runat="server" ID="btnOk"></Arco:OkButton>
                    <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" ID="btnCancelUrl">
                    </Arco:CancelButton>
                </Arco:ButtonPanel>
            </div>
        </div>
    </div>

</asp:Content>
