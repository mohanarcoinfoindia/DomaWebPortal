<%@ Page Language="VB" AutoEventWireup="false" CodeFile="SitemapEntry.aspx.vb" Inherits="UserBrowser_SitemapEntry" MasterPageFile="~/masterpages/Toolwindow.master" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
      <asp:Label ID="lblMsgText" runat="server" Visible="false" />

    <div class="container-fluid detail-form-container">

        <% If ShowV8Options %>
          <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="Label1" runat="server" CssClass="Label" AssociatedControlID="drpSiteVersion" Text="Site" />
            </div>
            <div class="col-md-8 FieldCell">
                <asp:DropDownList runat="server" ID="drpSiteVersion">
                    <asp:ListItem Value="0">All</asp:ListItem>
                    <asp:ListItem Value="7" Selected="true">Version 7</asp:ListItem>
                    <asp:ListItem Value="8">Version 8</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <%end if %>
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
            </div>
            <div class="col-md-8 FieldCell">
               <ArcoControls:MultiLanguageTextBox CssClass="Label" runat="server" ID="txtLabel"></ArcoControls:MultiLanguageTextBox>               
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
                <asp:Label ID="lblUrl" runat="server" CssClass="Label" AssociatedControlID="txtUrl" />
            </div>
            <div class="col-md-8 FieldCell">
                 <asp:TextBox MaxLength="200" runat="server" ID="txtUrl" />       
                
            </div>
        </div>
          <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblParameters" runat="server" CssClass="Label" AssociatedControlID="txtUrl" />
            </div>
            <div class="col-md-8 FieldCell">
                 <asp:TextBox MaxLength="4000" runat="server" ID="txtParameters" TextMode="MultiLine" />       
                
            </div>
        </div>

        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
              
            </div>
            <div class="col-md-8 FieldCell">
               
                <ArcoControls:CheckBox runat="server" ID="chkIsDefault" Text="Default" ToolTip="Check to make this entry the default opened link" />     
              
                <ArcoControls:CheckBox runat="server" ID="chkAppendQueryStrng" Text="Append Querystring" ToolTip="Check to append the querystring of the page to the url" />       
            </div>
        </div>
         <% If ShowV8Options %>
         <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblGroup" runat="server" CssClass="Label" AssociatedControlID="txtGroup"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox runat="server" ID="txtGroup" />              
            </div>
        </div>
        <%End if %>
          <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblParentId" runat="server" CssClass="Label" AssociatedControlID="txtParentId"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox runat="server" ID="txtParentId" />              
            </div>
        </div>
          <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblSeq" runat="server" CssClass="Label" AssociatedControlID="txtSeq"></asp:Label>
            </div>
            <div class="col-md-8 FieldCell">
                <asp:TextBox runat="server" ID="txtSeq" />              
            </div>
        </div>

          <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblRoles" runat="server" CssClass="Label" AssociatedControlID="txtRoles"
                    Text="Role(s)" />
            </div>
            <div class="col-md-8 FieldCell">
                 <asp:TextBox MaxLength="200" runat="server" ID="txtRoles" />
            </div>
        </div>
         <% If ShowV8Options %>
          <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label ID="lblIcon" runat="server" CssClass="Label" AssociatedControlID="txtIcon" />
            </div>
            <div class="col-md-8 FieldCell">
                 <asp:TextBox MaxLength="100" runat="server" ID="txtIcon" />       
                
            </div>
        </div>
        <%end if %>
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