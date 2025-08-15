<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Compose2.aspx.vb" Inherits="MailClient_Compose2" ValidateRequest="false" MasterPageFile="~/masterpages/Toolwindow.master" %>

<%@ Register TagPrefix="ArcoControls" Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls.Controls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">

    <style type="text/css">
        .MailContentPanel {
            padding: 0px 10px 10px 10px;
            margin: 20px
        }

        .toolWindowPopup {
            overflow-y: auto;
        }

        .reToolbarWrapper {
            overflow: hidden;
        }
    </style>
    <script type="text/javascript">

        function onAutoCompleteRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
          
            //Data passed to the service.
           // context["XXX"] = "XXX";
        }

        function OnClientFileUploadFailed(sender, args) {
            ;
            args.set_handled(false);
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


        function OnAutoCompleteClientLoad(sender) {                  
             $(".racInput","#" + sender.get_id()).on("paste", function (e) {
                var $this = $(this);
 
                setTimeout(function () { //break the callstack to let the event finish 
                    var text = $this.val();
 
                    var lastChar = text.charAt(text.length - 1);
                    var myText = text;
                    if (lastChar === ";") {
                        myText = text.substr(0, text.length - 1);
                    }
 
                    var array = myText.split(';') 
                    for (var i = 0; i < array.length; i++) {
                        var currEntryText = array[i];
 
                        var entry = new Telerik.Web.UI.AutoCompleteBoxEntry();
                        entry.set_text(currEntryText);
                        sender.get_entries().add(entry);
                    }
                }, 0);
            })
            }

    <%if not Page.IsPostBack AndAlso txtFileUpload.Visible Then%>
        Telerik.Web.UI.RadAsyncUpload.Modules.Flash.isAvailable = function () { return false; }
    <%end if%>
</script>

    <input type="hidden" id="txtAllowExternal" runat="server" />

    <asp:Table ID="tblToolbar" runat="server" Width="100%" CellPadding="0" CellSpacing="0" border="0" Height="30px" style="margin-left:25px;">
        <asp:TableRow>
            <asp:TableCell VerticalAlign="Top">
                <telerik:RadToolBar ID="tlbMain" runat="server" SingleClick="ToolBar">
                    <Items>
                        <telerik:RadToolBarButton ImageUrl="~/Images/Mail.svg" Text="Send" PostBack="true" CommandName="send"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton IsSeparator="true"></telerik:RadToolBarButton>
                        <telerik:RadToolBarButton ID="btnchkReplyRequired" runat="server">
                            <ItemTemplate>
                                <div style="vertical-align: middle">
                                    <ArcoControls:CheckBox ID="chkReplyRequired" runat="server" title="Require Response" />
                                    <asp:Label ID="lblReplyRequired" runat="server" Text="Require Response" />
                                </div>
                            </ItemTemplate>
                        </telerik:RadToolBarButton>
                    </Items>
                </telerik:RadToolBar>
            </asp:TableCell>

        </asp:TableRow>
    </asp:Table>

    <div>
        <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" Text="Error message" Visible="False"></asp:Label>
    </div>

    <asp:Panel runat="server" ID="hd">
        <div id="mailReceivers" class="container-fluid detail-form-container">
            <div id="trFrom" class="row detail-form-row" runat="server">
                <div class="col-md-4 LabelCell mailReceiversLabel">
                    <asp:Label ID="lblFromHeader" runat="server" Text="From: " CssClass="Label" />
                </div>
                <div class="col-md-8 ReadOnlyFieldCell">
                    <asp:Label ID="lblFrom" runat="server" Text="From: " CssClass="Label" />
                </div>
            </div>

            <div id="trTo" class="row detail-form-row" runat="server">
                <div class="col-md-4 LabelCell mailReceiversLabel">
                    <asp:Label ID="lblTo" runat="server" Text="To (external): " CssClass="Label" />
                </div>
                <div class="col-md-8 FieldCell">
                    <div style="float: left; display: inline">
                        <telerik:RadAutoCompleteBox Visible="true" ID="txtTo" runat="server" Width="500px" SkinID="CustomCssClass"
                            OnClientRequesting="onAutoCompleteRequesting" AllowCustomEntry="true" DropDownWidth="500px" OnClientLoad="OnAutoCompleteClientLoad" ClientIDMode="Static" >
                            <WebServiceSettings Path="~/ScriptServices/MailContactLookup.asmx" Method="GetContacts" />
                            <ClientDropDownItemTemplate>
								<table cellpadding="0" cellspacing="0">
										<tr>
											<td style="width: 75%; padding-left: 10px;">                                                    
												  <b> #:Text #</b><br />
													 #= Value #
										    </td>
										</tr>
									</table>
                            </ClientDropDownItemTemplate>
                        </telerik:RadAutoCompleteBox>
                    </div>
                    <div style="float: left; display: inline; margin-left: 5px;">
                        <ArcoControls:CheckBox ID="chkIncludeDelegates" runat="server" />
                    </div>
                </div>
            </div>

            <div id="trCC" class="row detail-form-row" runat="server">
                <%--style="height: 16px !important;"--%>
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblCC" runat="server" Text="CC: " CssClass="Label" />
                </div>
                <div class="col-md-8 FieldCell">
                    <%--style="height: 16px !important;"--%>
                    <telerik:RadAutoCompleteBox Visible="true" ID="txtCC" runat="server" Width="500px" SkinID="CustomCssClass"
                        OnClientRequesting="onAutoCompleteRequesting" AllowCustomEntry="true" DropDownWidth="500px" OnClientLoad="OnAutoCompleteClientLoad" ClientIDMode="Static" >
                        <WebServiceSettings Path="~/ScriptServices/MailContactLookup.asmx" Method="GetContacts" />
                        
                    </telerik:RadAutoCompleteBox>
                </div>
            </div>

            <div id="trBCC" class="row detail-form row" runat="server">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblBCC" runat="server" Text="BCC: " CssClass="Label" />
                </div>
                <div class="col-md-8 FieldCell">
                    <telerik:RadAutoCompleteBox Visible="true" ID="txtBCC" runat="server" Width="500px" SkinID="CustomCssClass"
                        OnClientRequesting="onAutoCompleteRequesting" AllowCustomEntry="true" DropDownWidth="500px" OnClientLoad="OnAutoCompleteClientLoad" ClientIDMode="Static">
                        <WebServiceSettings Path="~/ScriptServices/MailContactLookup.asmx" Method="GetContacts" />
                    </telerik:RadAutoCompleteBox>
                </div>
            </div>

            <div class="row detail-form-row">
                <div class="col-md-4 LabelCell">
                    <asp:Label ID="lblSubject" runat="server" Text="Subject:" CssClass="Label"></asp:Label>
                </div>
                <div class="col-md-8 FieldCell">
                    <div id="Div1" class="composeSubject bd">
                        <telerik:RadTextBox ID="txtSubject" SkinID="CustomCssClass" runat="server" Columns="60" Width="500px"></telerik:RadTextBox>
                    </div>
                </div>
            </div>

            <div id="trATT" class="row detail-form-row" runat="server">
                <div class="col-md-4 LabelCell mailReceiversLabel">
                    <asp:Label ID="lblAtt" runat="server" Text="Attachments: " CssClass="Label" />
                       <asp:Literal ID="lblFileTypes" runat="server" />
                </div>
                <div class="col-md-8 FieldCell" runat="server">
                    <asp:PlaceHolder runat="server" ID="pnlAttachments">
                        <%--  <input id="filAttachment" runat="server" skinid="CustomCssClass" name="filAttachment" size="50" style="height: 23px;" tabindex="6" type="file" />
                        &nbsp; --%>
                        
                        
                        <telerik:RadAsyncUpload ID="txtFileUpload" runat="server" OnClientValidationFailed="OnClientValidationFailed"
                            OnClientFileUploadFailed="OnClientFileUploadFailed" />
                        &nbsp;
                         <ArcoControls:CheckBox ID="chkZip" runat="server" Text="ZIP attachment(s)" />

                       
                        <telerik:RadProgressArea ID="RadProgressArea1" runat="server" Visible="true" ProgressIndicators="TotalProgress,TransferSpeed,TotalProgressPercent,TotalProgressBar" />
                    </asp:PlaceHolder>
                </div>
            </div>
            <div class="row detail-form-row">
                <div class="col-md-8 offset-md-4">
                    <asp:Button ID="cmdAddAtt" runat="server" CssClass="button positive" />
                </div>
            </div>
            <div class="row detail-form-row">
                <div class="col-md-8 offset-md-4 FieldCell">
                   
                        <ArcoControls:CheckBoxList ID="chkListAtt" runat="server" AutoPostBack="False" RepeatDirection="Vertical" RepeatLayout="Flow"></ArcoControls:CheckBoxList>
                  
                    <asp:Label runat="server" ID="pnlFixedAttachments"></asp:Label>
                </div>
            </div>
            <asp:Panel ID="pnlHyperlinks" runat="server" GroupingText="Hyperlinks" Height="50px" Visible="false">

                <table border="0">
                    <tr>
                        <td style="width: 79px">
                            <asp:Label ID="lblHyperlinkDocroom" runat="server" Text="DocRoom"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbHyperlinksDocroom" runat="server"></asp:DropDownList>
                            <asp:ImageButton ID="imgHyperlinkDocroom" runat="server" ImageUrl="~/Images/OK.gif" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 79px">
                            <asp:Label ID="lblHyperlinkFiles" runat="server" Text="Files"></asp:Label>
                        </td>
                        <td>
                            <asp:DropDownList ID="cmbHyperlinks" runat="server"></asp:DropDownList>
                            <asp:ImageButton ID="imgHyperlink" runat="server" ImageUrl="~/Images/OK.gif" />
                        </td>
                    </tr>
                </table>

            </asp:Panel>
    </asp:Panel>

    <br style="line-height: 5pt;" />
    <div id="messageEditor" class="MailContentPanel">
        <telerik:RadEditor ID="RadEditor1" runat="server" SkinID="EditMailBody" EditModes="Design">
        </telerik:RadEditor>
    </div>
    <asp:Panel runat="server" ID="pnlTemplate" Visible="false" CssClass="MailContentPanel">
        <asp:Label runat="server" ID="lblTemplate" />
    </asp:Panel>

</asp:Content>
