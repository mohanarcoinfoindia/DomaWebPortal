<%@ Page Language="VB" AutoEventWireup="false" CodeFile="EditDelegate.aspx.vb" MasterPageFile="~/masterpages/Toolwindow.master" Inherits="EditDelegate" ValidateRequest="false" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <%--<title>User Delegates</title>--%>
    <script type="text/javascript">
        var selecttypefirst = '<%=GetDecodedLabel("del_selecttypefirst") %>';

        function checkActivate() {
            if ($find("<%=dateBegin.clientID %>").isEmpty() || $find("<%=dateEnd.clientID %>").isEmpty()) {
                alert("Startdate and Enddate are mandatory!");
                return false;
            }
            else {
                return true;
            }
        }

        function checkLookupDelegate() {

            var idFieldId = "<%=DelegateSUBJECT_ID.ClientId%>";
            //var idField = $get(idFieldId);
            //var val = idField.value;--%>

            var displFieldID = "<%=Me.Delegate.ClientId%>";
            var displayField = $get(displFieldID);

            var type = $find("<%=cmbDelegate.ClientId%>").get_value();

            switch (type) {
                case "User":
                    var address = 'DM_UserBrowser_RightPane.aspx?txtdisplay=' + displayField.value + '&mode=getresult&USER_LOGIN=' + idFieldId + '&USER_DISPLAY_NAME=' + displFieldID;
                    window.open(address, 'Users', 'height=800, left=0, titlebar=1, toolbar=0, status=1, top=10, width=1000');
                    break;
                case "Role":
                    var address = 'DM_UserBrowser_Roles.aspx?txtRoleName=' + displayField.value + '&mode=getresult&ROLE_ID=' + idFieldId + '&ROLE_NAME=' + displFieldID;
                    window.open(address, 'Users', 'height=800, left=0, titlebar=1, toolbar=0, status=1, top=10, width=1000');
                    break;
                default:
                    alert(selecttypefirst);
            }
        }

         function checkLookupDelegateFrom() {

       
           var idFieldId = "<%=DelegateFromSUBJECT_ID.ClientId%>";
             var displFieldID = "<%=Me.DelegateFrom.ClientId%>";
            var displayField = $get(displFieldID);

            var type = $find("<%=cmbDelegateFrom.ClientId%>").get_value();

            switch (type) {
                case "User":
                    var address = 'DM_UserBrowser_RightPane.aspx?txtdisplay=' + displayField.value + '&mode=getresult&USER_LOGIN=' + idFieldId + '&USER_DISPLAY_NAME=' + displFieldID;
                    window.open(address, 'Users', 'height=800, left=0, titlebar=1, toolbar=0, status=1, top=10, width=1000');
                    break;
                case "Role":
                    var address = 'DM_UserBrowser_Roles.aspx?txtRoleName=' + displayField.value + '&mode=getresult&ROLE_ID=' + idFieldId + '&ROLE_NAME=' + displFieldID;
                    window.open(address, 'Users', 'height=800, left=0, titlebar=1, toolbar=0, status=1, top=10, width=1000');
                    break;
                default:
                    alert(selecttypefirst);
            }
        }

        function enterCheckLookup(event, thi) {
            if (!event) event = window.event;
            if ((event.keyCode === 13) && (event.ctrlKey)) {
                checkLookup(thi.id);
            }
        }
        function clearFieldDelegateFrom() {
            var idFieldId = "<%=DelegateFromSUBJECT_ID.ClientId%>";

            if ($get(idFieldId) != null) {
                $get(idFieldId).value = '';
            }
        }
        function clearFieldDelegate() {
            var idFieldId = "<%=DelegateSUBJECT_ID.ClientId%>";

            if ($get(idFieldId) != null) {
                $get(idFieldId).value = '';
            }
        }

        function changeTr() {
            if (GetVal('<%=radioMode.UniqueID %>') === "2") {

                $get('<%=trBegin.ClientID %>').style.display = "";
                //$get("trBegin").style.width = "150px";
                $get('<%=trEnd.ClientID %>').style.display = "";

            }
            else {
                $get('<%=trBegin.ClientID %>').style.display = "none";
                $get('<%=trEnd.ClientID %>').style.display = "none";
            }
            //AdjustRadWidow();
        }

        function GetVal(id) {
            var a = null;
            var f = document.forms[0];
            var e = f.elements[id];

            for (var i = 0; i < e.length; i++) {
                if (e[i].checked) {
                    a = e[i].value;
                    break;
                }
            }
            return a;
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">

    <input type="hidden" id="DelegateFromSUBJECT_ID" runat="server" />
    <input type="hidden" id="DelegateSUBJECT_ID" runat="server" />
    <input type="hidden" id="DelegateIDToDelete" runat="server" />

    <asp:Label runat="server" Visible="false" CssClass="ErrorLabel" ID="lblError"></asp:Label>

    <div class="container-fluid detail-form-container" style="height: 380px">

        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label runat="server" ID="lblFrom" CssClass="Label" />
            </div>
            <div class="col-md-8 FieldCell">
                <asp:Label runat="server" ID="lblUserName" />

                <asp:PlaceHolder runat="server" ID="plhDelegateFrom" Visible="false">
                    <telerik:RadComboBox runat="server" ID="cmbDelegateFrom" Width="150px"
                        AllowCustomText="False" MarkFirstMatch="True" />
                    &nbsp;
                            <asp:TextBox runat="server" ID="DelegateFrom" Width="215px"
                                onkeyup="clearFieldDelegateFrom();enterCheckLookup(event,this);" />
                    <span id="DelegateFrom_Lookup" class="icon icon-search" style="vertical-align: middle;cursor:pointer;"
                        onclick="checkLookupDelegateFrom()" />
                </asp:PlaceHolder>
            </div>
        </div>

        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label runat="server" ID="lblTo" CssClass="Label" />
            </div>
            <div class="col-md-8 FieldCell">
                <telerik:RadComboBox runat="server" ID="cmbDelegate" Width="150px" AllowCustomText="False"
                    MarkFirstMatch="True" />
                &nbsp;
                <asp:TextBox runat="server" ID="Delegate" Width="215px"
                    onkeyup="clearFieldDelegate();enterCheckLookup(event,this);" />
                <span id="Delegate_lookup" class="icon icon-search iconCell" style="vertical-align: middle;cursor:pointer;"
                      onclick="checkLookupDelegate()" />
            </div>
        </div>

        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label runat="server" ID="lblProcedure" CssClass="Label" />
            </div>
            <div class="col-md-8 FieldCell">
                <telerik:RadComboBox ID="cmbProcID" runat="server" Width="400px" AllowCustomText="False"
                    MarkFirstMatch="True" />
            </div>
        </div>

        <div class="row detail-form-row">
            <div class="col-md-4 LabelCell">
                <asp:Label runat="server" ID="lblMode" CssClass="Label" />
            </div>
            <div class="col-md-8 FieldCell" style="padding-top: 15px;">
                <asp:RadioButtonList onclick="changeTr()" RepeatDirection="Horizontal" CellSpacing="10"
                    ID="radioMode" runat="server" CssClass="radio" RepeatLayout="Table" />
            </div>
        </div>

        <asp:Panel ID="trBegin" CssClass="row detail-form-row" runat="server">
            <div class="col-md-4 LabelCell">
                <asp:Label runat="server" ID="lblBegin" CssClass="Label" />
            </div>
            <div class="col-md-8 FieldCell">
                <telerik:RadDatePicker runat="server" ID="dateBegin" />
            </div>
        </asp:Panel>

        <asp:Panel ID="trEnd" CssClass="row detail-form-row" runat="server">
            <div class="col-md-4 LabelCell">
                <asp:Label runat="server" ID="lblEnd" CssClass="Label" />
            </div>
            <div class="col-md-8 FieldCell">
                <telerik:RadDatePicker runat="server" ID="dateEnd" />
            </div>
        </asp:Panel>

        <div id="trCommands" class="row detail-form-row" runat="server">
            <div class="col-md-8 offset-md-4 FieldCell">
                <Arco:ButtonPanel ID="pnlButtons" runat="server">
                    <Arco:OkButton ID="Save" runat="server" Text="Save"></Arco:OkButton>
                </Arco:ButtonPanel>
            </div>
        </div>
    </div>

</asp:Content>
