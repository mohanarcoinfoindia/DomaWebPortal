<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_FILEACTIONS.aspx.vb" Inherits="DM_FILEACTIONS" MasterPageFile="~/masterpages/Toolwindow.master" ValidateRequest="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <div style="width: 400px;">
        <asp:MultiView ID="MultiView1" runat="server">
            <asp:View ID="viewCheckin" runat="server">
                <div class="container-fluid detail-form-container">
                    <asp:PlaceHolder ID="plhHeaderCheckIn" runat="server">
                        <div class="row detail-form-row DetailHeader">
                            <div class="DetailHeaderContent">
                                <asp:Label ID="lblCheckInHeader" runat="server"></asp:Label>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plhInlineHeaderCheckIn" runat="server">
                        <div class="row detail-form-row">
                            <div class="col-12 d-flex justify-content-center">
                                <asp:Label ID="lblCheckInFileName" runat="server" CssClass="Label"></asp:Label>
                            </div>
                        </div>
                        <hr />
                    </asp:PlaceHolder>
                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="lblEditOptions" runat="server" CssClass="Label"></asp:Label>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:PlaceHolder ID="plhEdit" runat="server"></asp:PlaceHolder>
                        </div>
                    </div>
                    <hr />
                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="lblCheckinoptions" runat="server" Text="Check-in options:" CssClass="Label"></asp:Label>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:RadioButtonList ID="rdlAction" runat="server">
                                <asp:ListItem Value="0">Cancel CheckOut</asp:ListItem>
                                <asp:ListItem Value="1">Save New Version</asp:ListItem>
                                <asp:ListItem Value="2" Selected="True">Save New SubVersion</asp:ListItem>
                                <asp:ListItem Value="3">Overwrite Current Version</asp:ListItem>
                                <asp:ListItem Value="4">Keep checked out</asp:ListItem>
                            </asp:RadioButtonList>
                        </div>
                    </div>
                    <div class="row detail-form-row">
                        <div class="col-md-8 offset-md-4 FieldCell">
                            <Arco:ButtonPanel ID="pnlButtons" runat="server">
                                <Arco:OkButton ID="cmdSave" runat="server"></Arco:OkButton>
                                <Arco:CancelButton ID="cmdCancel" runat="server" OnClientClick="javascript:Close();return false;"></Arco:CancelButton>
                            </Arco:ButtonPanel>
                        </div>
                    </div>
                </div>
            </asp:View>

            <asp:View ID="viewEditProperties" runat="server">
                <div class="container-fluid detail-form-container">
                    <asp:PlaceHolder ID="plhHeaderEditProps" runat="server">
                        <div class="row detail-form-row DetailHeader">
                            <div class="DetailHeaderContent">
                                <asp:Label ID="lblEditPropsHeader" runat="server"></asp:Label>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="Label2" runat="server" Text="New filename : " CssClass="Label" AssociatedControlID="txtNewName"></asp:Label>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:TextBox ID="txtNewName" runat="server" TextMode="MultiLine"></asp:TextBox>
                            <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" ID="valNewName" runat="server"
                                ControlToValidate="txtNewName" ErrorMessage="*" SetFocusOnError="True" ValidationGroup="EditProps">
                            </asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <asp:PlaceHolder ID="plhPacks" runat="server">
                        <div class="row detail-form-row">
                            <div class="col-md-4 LabelCell">
                                <asp:Label ID="lblPack" runat="server" Text="Package : " CssClass="Label"></asp:Label>
                            </div>
                            <div class="col-md-8 FieldCell">
                                <asp:DropDownList ID="cmbPacks" runat="server"></asp:DropDownList>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="Label5" runat="server" Text="language : " CssClass="Label"></asp:Label>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:DropDownList ID="cmbLangs" runat="server"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row detail-form-row">
                        <div class="col-md-8 offset-md-4 FieldCell">
                            <Arco:ButtonPanel ID="ButtonPanel1" runat="server">
                                <Arco:OkButton runat="server" ID="cmdEditProps" ValidationGroup="EditProps"></Arco:OkButton>
                                <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" ID="HyperLink1"></Arco:CancelButton>
                            </Arco:ButtonPanel>
                        </div>
                    </div>
                </div>
            </asp:View>

            <asp:View ID="viewEditUrl" runat="server">
                <div class="container-fluid detail-form-container">
                    <asp:PlaceHolder ID="plhHeaderUrl" runat="server">
                        <div class="row detail-form-row DetailHeader">
                            <div class="DetailHeaderContent">
                                <asp:Label ID="lblEditUrlHeader" runat="server"></asp:Label>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                    <div class="row detail-form-row">
                        <div class="col-md-4 LabelCell">
                            <asp:Label ID="Label3" runat="server" Text="Url :" CssClass="Label" AssociatedControlID="txtUrl"></asp:Label>
                        </div>
                        <div class="col-md-8 FieldCell">
                            <asp:TextBox ID="txtUrl" runat="server" Width="200"></asp:TextBox>
                            <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtUrl"
                                ErrorMessage="You have to enter an url" SetFocusOnError="True"
                                ValidationGroup="EditUrl">
                            </asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="row detail-form-row">
                        <div class="col-md-8 offset-md-4 FieldCell">
                            <Arco:ButtonPanel ID="pnl2" runat="server">
                                <Arco:OkButton runat="server" ID="cmdEditUrl" ValidationGroup="EditUrl"></Arco:OkButton>
                                <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" ID="Hyperlink2"></Arco:CancelButton>
                            </Arco:ButtonPanel>
                        </div>
                    </div>
                </div>
            </asp:View>

            <asp:View ID="viewCheckout" runat="server">
                <asp:Label ID="lblCheckoutfile" runat="server"></asp:Label>....
            </asp:View>
            <asp:View ID="viewCancelCheckout" runat="server">
                <asp:Label ID="lblCancelCheckoutfile" runat="server"></asp:Label>....
            </asp:View>
            <asp:View ID="viewCollectFile" runat="server">
                <asp:Label ID="lblCollectFile" runat="server"></asp:Label>....      
            </asp:View>
            <asp:View ID="viewDeleteFile" runat="server">
                <asp:Label ID="lblDeleteFile" runat="server"></asp:Label>....
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
