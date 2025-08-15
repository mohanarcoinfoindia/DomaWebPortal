<%@ Page Language="vb" AutoEventWireup="true" Inherits="DM_FILE_ADD" CodeFile="DM_FILE_ADD.aspx.vb" Buffer="false" MasterPageFile="~/masterpages/Toolwindow.master" %>

<%@ Register TagPrefix="Telerik" Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" %>
<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <script type="text/javascript">

        function GotoTab(t) {
            $get("<%=SelectedTab.ClientID %>").value = t;
            theForm.submit();
        }

        function onFileTypeChanged(sender, eventArgs) {
            var item = eventArgs.get_item();
            if (item.get_value() == "0") {
                toggleFileUpload(true);
            }
            else {
                toggleFileUpload(false);
            }
        }

        function OnClientValidationFailed(sender, args) {
            if (args.get_fileName().lastIndexOf('.') != -1) {//this checks if the extension is correct
                var fileExtention = args.get_fileName().substring(args.get_fileName().lastIndexOf('.') + 1, args.get_fileName().length);
                if (sender.get_allowedFileExtensions().indexOf(fileExtention) == -1) {
                    alert("<%=GetLabel("invalidfileextension") %> (." + fileExtention + ")");
            }
            else {
                alert("Invalid file size");
            }
        }
        else {
            PC().ShowError("<%=GetLabel("invalidfileextension") %>");
            }
        }

        function OnClientFileUploadFailed(sender, args) {
            ;
            args.set_handled(false);
        }

        function toggleFileUpload(mode) {
            var upload = getRadUpload("<%= txtFileUpload.ClientID  %>");
            upload.set_enabled(mode);

            var count = upload.getFileInputs().length;
            for (var i = 0; i < count; i++) {
                upload.clearFileInputAt(i);
            }
            var inputEls = $get('<%=txtFileUpload.ClientID %>ListContainer').getElementsByTagName('input');
            for (var br = 0; br < inputEls.length; br++) {
                if (inputEls[br].className == 'RadUploadSelectButton') {
                    inputEls[br].disabled = !mode;
                }
            }
        }

    <%if not Page.IsPostBack then%>
        Telerik.Web.UI.RadAsyncUpload.Modules.Flash.isAvailable = function () { return false; }
    <%end if%>
</script>

    <telerik:RadProgressManager ID="RadProgressManager1" runat="server" SuppressMissingHttpModuleError="false"></telerik:RadProgressManager>

    <input type="hidden" name="OBJECT_ID" runat="server" id="OBJECT_ID" />
    <input type="hidden" name="CAT_ID" runat="server" id="CAT_ID" />
    <input type="hidden" name="FILE_ID" runat="server" id="FILE_ID" />
    <input type="hidden" name="DM_PARENT_ID" runat="server" id="DM_PARENT_ID" />
    <input type="hidden" name="CASE_ID" runat="server" id="CASE_ID" />
    <input type="hidden" name="PACK_ID" runat="server" id="PACK_ID" />
    <input type="hidden" name="LINK_TO_DIN" runat="server" id="LINK_TO_DIN" />
    <input type="hidden" name="LINK_AS" runat="server" id="LINK_AS" />
    <input type="hidden" name="ADD_AS" runat="server" id="ADD_AS" />
    <input type="hidden" name="SelectedTab" runat="server" id="SelectedTab" />
    <input id="txtSelectedID" type="hidden" runat="server" name="txtSelectedID" />
    <div style="margin-left: 10px; margin-right: 10px;">
        <asp:Table ID="tblTabs" CssClass="Panel" HorizontalAlign="Center" CellSpacing="0" runat="server">
            <asp:TableRow BackColor="#FFFFFF">
                <asp:TableCell>
                    <telerik:RadTabStrip ID="tabMain" runat="server" EnableViewState="false">
                    </telerik:RadTabStrip>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="rowOrigFileName" Visible="false">
                <asp:TableCell HorizontalAlign="center">
                    <asp:Label ID="lblOrigFileName" runat="server"></asp:Label>
                </asp:TableCell>
            </asp:TableRow>

            <asp:TableRow>
                <asp:TableCell HorizontalAlign="left">
                    &nbsp;
		            <asp:Panel ID="pnlReplaceOptions" runat="server" Visible="false">
                        <br />
                        <asp:RadioButtonList ID="rdlReplaceAction" runat="server" AutoPostBack="false">
                            <asp:ListItem Value="0" Selected="true">Overwrite Current Version</asp:ListItem>
                            <asp:ListItem Value="1">Save New Version</asp:ListItem>
                            <asp:ListItem Value="2">Save New SubVersion</asp:ListItem>
                        </asp:RadioButtonList>
                    </asp:Panel>
                </asp:TableCell>
            </asp:TableRow>


            <asp:TableRow>
                <asp:TableCell>
                    <asp:PlaceHolder ID="pnlUploadMsg" runat="server" Visible="False">
                        <asp:Label ID="lblMessage" runat="server" CssClass="ErrorLabel"></asp:Label>
                        <script type="text/javascript">

                            function doFeedback(fileid) {

                                if (MainPage()) {
                                    try {

                                        MainPage().RefreshParentPage();
                                        MainPage().Refresh();
                                    }
                                    catch (e) {
                                        try {
                                            MainPage().ReloadParent(fileid);
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
                                if (window.parent) {
                                    try {
                                        window.parent.ReloadContent();
                                    }
                                    catch (e) {
                                    }
                                }
                                Close();
                            }
                        </script>


                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="pnlUploadDesktop" runat="server" Visible="False">
                        <div class="container-fluid detail-form-container">
                            <asp:Panel runat="server" CssClass="row detail-form-row" ID="rowName">
                                <div class="col-md-4 LabelCell">
                                    <asp:Label ID="lblName" runat="server" CssClass="Label"></asp:Label>
                                </div>
                                <div class="col-md-8 FieldCell">
                                    <asp:TextBox ID="txtFileName" runat="server" MaxLength="200"></asp:TextBox>
                                </div>
                            </asp:Panel>

                              <asp:Panel runat="server" CssClass="row detail-form-row" ID="rowFileType">
                                  <div class="col-md-4 LabelCell">
                                    <asp:Label ID="lblType" runat="server" CssClass="Label" Text="Type"></asp:Label>
                                </div>
                                <div class="col-md-8 FieldCell">
                                    <telerik:RadComboBox NoWrap="false" ID="cmbFileType" runat="server" OnClientSelectedIndexChanged="onFileTypeChanged" />
                                </div>
                                </asp:Panel>

                            <div class="row detail-form-row">
                                <div class="col-md-4 LabelCell">
                                    <asp:Label ID="lblFile" runat="server" CssClass="Label"></asp:Label><asp:Literal ID="lblFileTypes" runat="server"></asp:Literal>
                                </div>
                                <div class="col-md-8 FieldCell">                                                                                                            
                                    <asp:CustomValidator CssClass="FormField-Message bad" runat="server" ID="CustomValidator1" Display="Dynamic">
                                        <asp:Label ID="lblInvalidFileType" runat="server" Text="Unknown File Extension"></asp:Label>
                                    </asp:CustomValidator>
                                    <telerik:RadAsyncUpload ID="txtFileUpload" runat="server" OnClientValidationFailed="OnClientValidationFailed" OnClientFileUploadFailed="OnClientFileUploadFailed"></telerik:RadAsyncUpload>
                                    <telerik:RadProgressArea ID="RadProgressArea1" runat="server" Visible="true" ProgressIndicators="TotalProgress,TransferSpeed,TotalProgressPercent,TotalProgressBar" />
                                </div>
                            </div>

                            <asp:Panel runat="server" CssClass="row detail-form-row" ID="rowPack">
                                <div class="col-md-4 LabelCell">
                                    <asp:Label ID="lblPack" runat="server" Text="Package : " CssClass="Label"></asp:Label>
                                </div>
                                <div class="col-md-8 FieldCell">
                                    <telerik:RadComboBox ID="cmbFilePack" runat="server"></telerik:RadComboBox>
                                </div>
                            </asp:Panel>

                            <asp:Panel runat="server" CssClass="row detail-form-row" ID="rowLang">
                                <div class="col-md-4 LabelCell">
                                    <asp:Label ID="lblLang" runat="server" Text="language : " CssClass="Label"></asp:Label>
                                </div>
                                <div class="col-md-8 FieldCell">
                                    <telerik:RadComboBox ID="cmbLangs" runat="server"></telerik:RadComboBox>
                                </div>
                            </asp:Panel>

                            <asp:Panel runat="server" CssClass="row detail-form-row" ID="rowOcr">
                                <div class="col-md-4 LabelCell">
                                    &nbsp;
                                </div>
                                <div class="col-md-8 FieldCell">
                                    <ArcoControls:CheckBox runat="server" Text="Ocr when available" ID="chkAlwaysOCR" Checked="true" />
                                </div>
                            </asp:Panel>

                            <div class="row detail-form-row">
                                <div class="col-md-4 LabelCell"></div>
                                <div class="col-md-8 FieldCell">
                                    <Arco:ButtonPanel ID="pnl1" runat="server">
                                        <Arco:OkButton runat="server" ID="btnUploadDesktop" DoubleClickProtection="true"></Arco:OkButton>
                                        <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" ID="btnCancelDesktop"></Arco:CancelButton>
                                    </Arco:ButtonPanel>
                                </div>
                            </div>



                            <asp:Panel runat="server" CssClass="row detail-form-row" ID="Panel1">
                                <div class="col-md-4 LabelCell">
                                </div>
                                <div class="col-md-8 FieldCell">
                                </div>
                            </asp:Panel>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="pnlUploadUrl" runat="server" Visible="False">
                        <div class="container-fluid detail-form-container">
                            <div class="row detail-form-row">
                                <div class="col-md-4 LabelCell">
                                    <asp:Label ID="Label1" runat="server" Text="Url:" CssClass="Label"></asp:Label>
                                </div>
                                <div class="col-md-8 FieldCell">
                                    <asp:TextBox ID="txtUrl" runat="server" MaxLength="200"></asp:TextBox>
                                </div>
                            </div>
                            <div class="row detail-form-row">
                                <div class="col-md-4 LabelCell"></div>
                                <div class="col-md-8 FieldCell">
                                    <Arco:ButtonPanel ID="pnl2" runat="server">
                                        <Arco:OkButton runat="server" ID="btnUploadUrl" DoubleClickProtection="true"></Arco:OkButton>
                                        <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" ID="btnCancelUrl"></Arco:CancelButton>
                                    </Arco:ButtonPanel>
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="pnlUploadServer" runat="server" Visible="False">
                        <asp:Literal ID="TreeView" runat="server"></asp:Literal>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="pnlUploadMail" runat="server" Visible="False">
                        <asp:Literal ID="TreeViewMail" runat="server"></asp:Literal>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="pnlUploadClipboard" runat="server" Visible="False">
                        <asp:Table HorizontalAlign="Center" runat="server" BorderWidth="0" ID="Table1" Width="100%">
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="Center">
                                    <input type="text" id="txtPasteFileName" runat="server" name="url" size="50" maxlength="200">
                                </asp:TableCell>
                            </asp:TableRow>
                            <asp:TableRow>
                                <asp:TableCell HorizontalAlign="Center">
                                    <Arco:ButtonPanel ID="pnl3" runat="server">
                                        <Arco:OkButton runat="server" ID="btnUploadCLipboard" DoubleClickProtection="true"></Arco:OkButton>
                                        <Arco:CancelButton OnClientClick="javascript:Close();return false;" runat="server" ID="btnCancelClipboard"></Arco:CancelButton>
                                    </Arco:ButtonPanel>

                                </asp:TableCell>
                            </asp:TableRow>
                        </asp:Table>
                    </asp:PlaceHolder>

                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </div>

</asp:Content>
