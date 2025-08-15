<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DM_Detailview.aspx.vb" Inherits="DM_Detailview" ValidateRequest="false" EnableEventValidation="false"
    MaintainScrollPositionOnPostback="false" %>

<%@ Register Assembly="Arco.Doma.WebControls" Namespace="Arco.Doma.WebControls" TagPrefix="doma" %>

<asp:content id="Content1" contentplaceholderid="Plh1" runat="server">
    <script src="./js/DetailView.js" type="text/javascript"></script>
    <script type="text/javascript">
        function ShowFiles(fileid, fromarchive, rend, frommail) {
            <%If Not UserProfile.ShowFilesInSeparateWindow Then%>
            PC().ShowFile(fileid, fromarchive, rend, frommail, '', false, 0, false);
            <%Else%>
            PC().ShowFile(fileid, fromarchive, rend, frommail, '', false, 0, true);
            <%end If%>
        }

        function ViewFile(fileid, fromarchive, rend) {
            ShowFiles(fileid, fromarchive, rend, false);
        }
        function ShowMailFiles(fileid, fromarchive, rend) {
            ShowFiles(fileid, fromarchive, rend, true);
        }


        function ReloadParent(fileid) {
            if (!(typeof (fileid) == 'undefined' || fileid == null)) {
                $get('<%=hdnOpenFile.ClientID %>').value = fileid;
            }
            RefreshObject(fileid);
        }

        Sys.Application.add_load(UpgradeASPNETValidation);
    </script>
    
        <div class="detailView">           
            <asp:HiddenField id="hdnReleaseTo" runat="server"></asp:HiddenField>
            <asp:HiddenField id="hdnUserEventID" runat="server"></asp:HiddenField>
            <asp:HiddenField id="hdnCustActionID" runat="server"></asp:HiddenField>       
            <asp:HiddenField id="hdnOpenFile" runat="server"></asp:HiddenField>   

            <asp:PlaceHolder id="sharedcalenderPlaceHolder" runat="server"></asp:PlaceHolder>

            <doma:DMObjectForm ID="domadetail" runat="server" />
        </div>     

    </asp:content>
