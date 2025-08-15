<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SearchButtons.ascx.vb" Inherits="UserControls_SearchButtons" %>

<Arco:OkButton runat="server" ID="lnk1" OnClientClick="AdvSearchIt();return false;" Text="Find" ValidationGroup="CheckMandatoryFields"  href="#"/>
<Arco:SecondaryButton runat="server" ID="lnk2" OnClientClick="CountIt();return false;" Text="Count"  ValidationGroup="CheckMandatoryFields"  href="#"/>
<Arco:SecondaryButton runat="server" ID="lnk3" OnClientClick="ClearForm();return false;" Text="Clear" href="#"/>
<Arco:SecondaryButton runat="server" ID="lnk4" OnClientClick="SaveQuery();return true;" Text="Save"  href="#"/>
<Arco:SecondaryButton runat="server" ID="lnk5" OnClientClick="OpenQuery();return false;" Text="Open"  href="#"/>