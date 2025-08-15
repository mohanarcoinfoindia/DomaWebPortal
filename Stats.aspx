<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Stats.aspx.vb" Inherits="Stats" ValidateRequest="false" %>
<%@ Register TagPrefix="Telerik" Assembly="Telerik.Web.Ui" Namespace="Telerik.Web.UI" %>
<%@ Register TagPrefix="Arco" TagName="Stat" Src="~/UserControls/StatControl.ascx" %>
<%@ Register TagPrefix="Arco" TagName="PageController" Src="~/PageController/DM_PageController.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>  
</head>
<body>
     <script type="text/javascript">
         function chart_seriesClick1(sender, e) {
            OpenUrl('<%=stat1.Report.ClickUrl %>', '<%=stat1.DateFilter %>', '<%=stat1.ReportID %>', e);
         }
       
         function OpenUrl(url, datefilter, reportid, e) {
            if (url != null && url != '')
            {
                if (url.includes("{") && url.includes("}")) {
                    url = url.replace(/{category}/gi, e.get_category());
                    url = url.replace(/{series}/gi, e.get_seriesName());
                    url = url.replace(/{y}/gi, e.get_value());
                    url = url.replace(/{reportid}/gi, reportid);
                    url = url.replace(/{datefilter}/gi, datefilter);
                }
                else {
                    if (url.includes("?")) {
                        url = url + '&';
                    }
                    else {
                        url = url + '?';
                    }
                    url = url + 'category=' + encodeURIComponent(e.get_category());
                    url = url + '&series=' + encodeURIComponent(e.get_seriesName());
                    url = url + '&y=' + e.get_value();
                    url = url + '&reportid=' + reportid;
                    url = url + '&datefilter=' + datefilter;
                }
                window.open(url, '_self');
            }
         }
    </script>
    <form id="form1" runat="server">
    <div>
       <Arco:PageController ID="thisPageController" runat="server" PageLocation="ListPage" />
      
         <telerik:RadAjaxLoadingPanel ID="pnlLoading" runat="server"></telerik:RadAjaxLoadingPanel>

        <telerik:RadAjaxPanel ID="RadAjaxPanel2" runat="server" LoadingPanelID="pnlLoading">
        <Arco:Stat runat="server" ID="stat1" OnClientSeriesClickedJavascript="chart_seriesClick1" />
        </telerik:RadAjaxPanel>
    
        <Arco:PageFooter id="lblFooter" runat="server"/>
    </div>
    </form>
</body>
</html>
