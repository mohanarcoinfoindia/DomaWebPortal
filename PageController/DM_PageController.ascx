<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="radM" %>
<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DM_PageController.ascx.vb" Inherits="UserControls_DM_PageController" %>
<radM:RadScriptManager runat="server" ID="sc1" EnablePartialRendering="true" EnableEmbeddedjQuery="false">
    <CompositeScript>
        <Scripts>
            <asp:ScriptReference Path="~/JS/ajax-dynamic-content.js" />
            <asp:ScriptReference Path="~/js/ajax.js" />
            <asp:ScriptReference Path="~/js/ajax-tooltip.js" />
            <asp:ScriptReference Path="~/JS/jquery-3.6.0.min.js" />
            <asp:ScriptReference Path="~/JS/popper.min.js" />
            <asp:ScriptReference Path="~/JS/bootstrap.min.js" />
            <asp:ScriptReference Path="~/JS/bootstrap-switch.min.js" />
            <asp:ScriptReference Path="~/JS/moment.min.js" />
        </Scripts>
    </CompositeScript>
</radM:RadScriptManager>

<radM:RadAjaxManager ID="ajxMan" runat="server" EnableAJAX="true">
</radM:RadAjaxManager>
<radM:RadWindowManager ID="wndm1" runat="server" Overlay="true" Style="z-index: 7001" VisibleStatusbar="false"
    ReloadOnShow="false" ShowContentDuringLoad="false" Modal="true" KeepInScreenBounds="true">
    <AlertTemplate>&nbsp;</AlertTemplate>
    <ConfirmTemplate>&nbsp;</ConfirmTemplate>
    <PromptTemplate>
        <div class="rwDialog rwPromptDialog">
            <div class="rwDialogContent">
                <div class="rwDialogMessage">{1}</div>
                <div class="rwPromptInputContainer">
                    <script type="text/javascript">
                        function RadWindowprompt_detectenter(id, ev, input) {
                            if (!ev) ev = window.event;
                            if (ev.keyCode == 13) {
                                var but = input.parentNode.parentNode.parentNode.getElementsByTagName("button")[0];
                                if (but) {
                                    if (but.click) {
                                        but.click();
                                    }
                                    else if (but.onclick) {
                                        but.focus();
                                        var click = but.onclick;
                                        but.onclick = null;
                                        if (click) click.call(but);
                                    }
                                }
                                return false;
                            }
                            else return true;
                        }
                    </script>
                    <div class="m-3">
                        <input title="Enter Value" onkeydown="return RadWindowprompt_detectenter('{0}', event, this);" type="text" class="rwPromptInput radPreventDecorate w-100" value="{2}" />
                    </div>
                </div>
            </div>
            <div class="buttons mt-3 pr-3">
                <button type="button" class="button positive" onclick="$find('{0}').close(this.parentNode.parentNode.getElementsByTagName('input')[0].value); return false;"><%=GetDecodedLabel("ok") %></button>
                <button type="button" class="button negative" onclick="$find('{0}').close(null); return false;"><%=GetDecodedLabel("cancel") %></button>
            </div>
        </div>
    </PromptTemplate>
    <Windows>
        <radM:RadWindow ID="wndModalAutosize" runat="server" AutoSize="true" Behaviors="Close,Move,Maximize"></radM:RadWindow>
        <radM:RadWindow ID="wndModalAutosizeResizable" runat="server" AutoSize="true" Behaviors="Close,Move,Resize,Maximize"></radM:RadWindow>
        <radM:RadWindow ID="wndModalManualSizeNoResize" runat="server" AutoSize="false" Behaviors="Close,Move,Maximize"></radM:RadWindow>
        <radM:RadWindow ID="wndModalManualSize" runat="server" AutoSize="false" Behaviors="Close,Move,Resize,Maximize"></radM:RadWindow>
        <radM:RadWindow ID="wndModalDetail" runat="server" AutoSize="false" Behaviors="Close,Move,Resize" OnClientBeforeClose="OnModalDetailWindowClose"></radM:RadWindow>
    </Windows>
</radM:RadWindowManager>

<radM:RadNotification ID="notifWnd" runat="server" ContentIcon="" TitleIcon="" ShowCloseButton="false" ShowTitleMenu="false" VisibleTitlebar="false" EnableRoundedCorners="true" EnableShadow="true">
</radM:RadNotification>


<radM:RadContextMenu ID="mnuPageContext" EnableScreenBoundaryDetection="true" ContextMenuElementID="none" OnClientMouseOver="PreventContextTimeOut" OnClientMouseOut="HideContextMenu" runat="server"></radM:RadContextMenu>

<asp:HiddenField ID="txtOIP" runat="server" />
<asp:HiddenField ID="txtScreenmode" runat="server" />

<input type="hidden" name="DM_PARENT_ID" id="DM_PARENT_ID" runat="server" />
<input type="hidden" name="DM_ROOT_ID" id="DM_ROOT_ID" runat="server" />
<input type="hidden" name="PACK_ID" id="PACK_ID" runat="server" />


