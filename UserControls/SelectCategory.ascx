<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="Telerik" %>

<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SelectCategory.ascx.vb" Inherits="UserControls_SelectCategory" %>

<div style="vertical-align:top; width:100%; max-width:400px;">
    <div class="comboBoxDiv">
        <Telerik:RadComboBox ID="cmbCategory" runat="server" MarkFirstMatch="true" CausesValidation="false" EnableViewState="true" Width="400px"></Telerik:RadComboBox>
    </div>
</div>
