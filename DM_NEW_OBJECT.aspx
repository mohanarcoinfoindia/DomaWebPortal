<%@ Page Language="vb" AutoEventWireup="false" Inherits="Doma.DM_NEW_OBJECT" CodeFile="DM_NEW_OBJECT.aspx.vb" MasterPageFile="~/masterpages/ToolWindow.master" ValidateRequest="false" %>

<%@ Register TagPrefix="arcoctrls" Namespace="Arco.Doma.WebControls" Assembly="Arco.Doma.WebControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <script type="text/javascript">
        //parent.SetHeader('Edit');
        function RefreshTree(parentId) {
            const mainPage = MainPage();
            if (window.top.RefreshTreeContent) {
                window.top.RefreshTreeContent(parentId);
            }
            else {
                mainPage.PC().Reload();
            }
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

        function toggleFileUpload(mode) {
            var upload = getRadUpload("<%=txtFileUpload.ClientID%>");
            upload.set_enabled(mode);

            var count = upload.getFileInputs().length;

            for (var i = 0; i < count; i++) {
                upload.clearFileInputAt(i);
            }

            var inputEls = $get('<%=txtFileUpload.ClientID%>ListContainer').getElementsByTagName('input');
            for (var br = 0; br < inputEls.length; br++) {
                if (inputEls[br].className == 'RadUploadSelectButton') {
                    inputEls[br].disabled = !mode;
                }
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
             alert("<%=GetLabel("invalidfileextension") %>");
            }
        }

        function OnClientFileUploadFailed(sender, args) {
            //  alert(args.get_message());        
            args.set_handled(false);
        }
    </script>

    <telerik:RadProgressManager ID="RadProgressManager1" runat="server" SuppressMissingHttpModuleError="false"></telerik:RadProgressManager>

    <div class="row m-3">
        <a href="javascript:history.go(-1)">
          <span class="icon icon-return icon-color-light"></span>
        </a>
    </div>
    <div class="container-fluid detail-form-container dmNewObject">
        <asp:PlaceHolder ID="Table1" runat="server">
            <div class="DetailHeaderContent mb-3">
                <div class="DetailHeaderLabel">
                    <asp:Label ID="txtTitle" runat="server"></asp:Label>
                </div>
            </div>
            <asp:Panel runat="server" CssClass="container-fluid detail-form-container" ID="Table2">

                <asp:Panel runat="server" ID="rowFolder" CssClass="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblFolder" runat="server" CssClass="Label"></asp:Label>
                    </div>
                    <div class="col-md-8 FieldCell">
                        <Arco:FolderLink ID="fldField" runat="server" JavaScriptOpenFunction="" BuildOnLoad="true" ShowFolderEasyBrowser="false" />
                    </div>
                </asp:Panel>

                <asp:Panel runat="server" ID="rowName" CssClass="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblName" runat="server" CssClass="Label" AssociatedControlID="txtObjectName" />
                    </div>
                    <div class="col-md-8 FieldCell">
                        <asp:TextBox runat="server" ID="txtObjectName" />
                        <asp:RequiredFieldValidator CssClass="FormField-Message Inline bad" runat="server" ValidationGroup="CheckName" ID="reqName" ControlToValidate="txtObjectName" ErrorMessage="*" SetFocusOnError="true"></asp:RequiredFieldValidator>
                    </div>
                </asp:Panel>

                <asp:Panel runat="server" ID="rowFileUpload" CssClass="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblFile" runat="server" Text="File" CssClass="Label" AssociatedControlID="txtFileUpload" /><asp:Literal ID="lblFileTypes" runat="server"></asp:Literal>
                    </div>
                    <div class="col-md-8 FieldCell flex-column">
                        <telerik:RadComboBox NoWrap="false" ID="cmbFileType" runat="server" OnClientSelectedIndexChanged="onFileTypeChanged" />
                        <asp:CustomValidator CssClass="FormField-Message bad" runat="server" ID="CustomValidator1" ValidationGroup="CheckName" Display="Dynamic">
                            <asp:Label ID="lblInvalidFileType" runat="server" Text="Unknown File Extension"></asp:Label>
                        </asp:CustomValidator>

                        <telerik:RadAsyncUpload ID="txtFileUpload" runat="server" OnClientValidationFailed="OnClientValidationFailed" OnClientFileUploadFailed="OnClientFileUploadFailed"></telerik:RadAsyncUpload>

                        <telerik:RadProgressArea ID="RadProgressArea1" runat="server" Visible="true" ProgressIndicators="TotalProgress,TransferSpeed,TotalProgressPercent,TotalProgressBar">
                        </telerik:RadProgressArea>
                    </div>
                </asp:Panel>

                <asp:Panel runat="server" ID="rowFileUploadPack" CssClass="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblPack" runat="server" Text="Package : " CssClass="Label" AssociatedControlID="cmbFilePack"></asp:Label>
                    </div>
                    <div class="col-md-8 FieldCell">
                        <telerik:RadComboBox ID="cmbFilePack" runat="server" SkinID="NoWidth"></telerik:RadComboBox>
                    </div>
                </asp:Panel>

                <asp:Panel runat="server" ID="rowFileUploadLang" CssClass="row detail-form-row">
                    <div class="col-md-4 LabelCell">
                        <asp:Label ID="lblLang" runat="server" Text="language : " class="Label" AssociatedControlID="cmbLangs"></asp:Label>
                    </div>
                    <div class="col-md-8 FieldCell">
                        <telerik:RadComboBox ID="cmbLangs" runat="server" SkinID="NoWidth"></telerik:RadComboBox>
                    </div>
                </asp:Panel>

                <div class="row detail-form-row mt-4">
                    <div class="col-md-4 LabelCell">
                    </div>
                    <div class="col-md-8 FieldCell">
                        <Arco:ButtonPanel ID="pnlButtons" runat="server">
                            <Arco:OkButton ID="lnkSave" OnClientClick="ProgressIndicator.display();" DoubleClickProtection="true" CommandName="InsertFolder" runat="server" OnClick="DoSave" ValidationGroup="CheckName"></Arco:OkButton>
                            <Arco:CancelButton ID="lnkCancel" runat="server" OnClientClick="javascript:Close();"></Arco:CancelButton>
                        </Arco:ButtonPanel>
                    </div>
                </div>
            </asp:Panel>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="lblMessage" Visible="False" runat="server">
            <table class="Panel" style="text-align: center">
                <tr class="DetailHeader">
                    <td class="DetailHeaderContent" style="text-align:center">&nbsp;</td>
                </tr>
                <tr>
                    <td style="text-align:center">
                        <asp:Label ID="lblMsgText" runat="server" Text="Save ok." /></td>
                </tr>
            </table>
        </asp:PlaceHolder>
    </div>

    <asp:Label Visible="False" ID="txtParentID" runat="server"></asp:Label>
    <asp:Label Visible="False" ID="txtObjectID" runat="server"></asp:Label>
    <asp:Label Visible="False" ID="txtCatID" runat="server"></asp:Label>
    <asp:Label Visible="False" ID="txtCaseID" runat="server"></asp:Label>
    <asp:Label Visible="False" ID="txtPackID" runat="server"></asp:Label>
    <asp:Label Visible="False" ID="txtLinkToDIN" runat="server"></asp:Label>
    <asp:Label Visible="False" ID="txtLinkAs" runat="server"></asp:Label>
    <asp:Label Visible="False" ID="txtCatType" runat="server"></asp:Label>

</asp:Content>
