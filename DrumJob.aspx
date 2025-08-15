<%@ Page Title="" Language="VB" MasterPageFile="~/masterpages/ToolWindow.master" AutoEventWireup="false" CodeFile="DrumJob.aspx.vb" Inherits="DrumJob" %>

<asp:Content ID="Header1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <style>
        #aspnetForm {
            overflow-x: hidden;
        }
    </style>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
    <div id="tblDrum" class="container-fluid detail-form-container" runat="server">
    </div>
    <div class="row detail-form-row">
        <div class="col-md-8 offset-md-4 FieldCell">
            <Arco:ButtonPanel runat="server" ID="pnlButtons">
                <Arco:OkButton ID="btnOk" runat="server"></Arco:OkButton>
                <Arco:CancelButton ID="btnCancel" runat="server" OnClientClick="javascript:Close();return false;"></Arco:CancelButton>
            </Arco:ButtonPanel>
        </div>
    </div>
</asp:Content>

