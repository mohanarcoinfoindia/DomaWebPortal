<%@ Page Language="VB" MasterPageFile="~/masterpages/Preview.master" AutoEventWireup="false" CodeFile="DM_Preview.aspx.vb" Inherits="DM_Preview" title="Preview" ValidateRequest="false" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" Runat="Server">
<style type="text/css">
    .PreviewPage
    {
      overflow:hidden;
    }
    #ifm
    {
        width:100%;
        height:100%;        
    }
</style>
<script type="text/javascript">
    function pageY(elem) {
        return elem.offsetParent ? (elem.offsetTop + pageY(elem.offsetParent)) : elem.offsetTop;
    }
    function resizeIframe() {       
        var h = window.innerHeight || document.body.clientHeight || document.documentElement.clientHeight;
        var ifm = $get('ifm')
        h -= pageY(ifm);
        h = (h < 0) ? 0 : h;      
        ifm.style.height = h + 'px';
    } 
    window.onresize = resizeIframe;
    window.onload = setTimeout(resizeIframe,100)
</script>

<iframe id="ifm" src="DM_VIEW_FILE.aspx<%=msQryParams %>"></iframe>

</asp:Content>



