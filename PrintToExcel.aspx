<%@ Page Language="VB" AutoEventWireup="false" CodeFile="PrintToExcel.aspx.vb" Inherits="PrintToExcel" MasterPageFile="~/masterpages/Toolwindow.master" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="Server">
    <style>
        #statuspanel
        {
            display:flex;
        }
        #visiblestatus{
            width:300px
        }
        #cancelButton{
            flex:1
        }
    </style>
    <script type="text/javascript">
        var xmlhttp = null;
        var TimeOutId;

        function AjaxGet(url, helm) {
            if (xmlhttp == null) {
                {
                    if (window.XMLHttpRequest) // code for IE7+, Firefox, Chrome, Opera, Safari
                    {
                        xmlhttp = new XMLHttpRequest();
                    }
                    else  // code for IE6, IE5
                    {
                        xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
                    }
                }
            }

            xmlhttp.onreadystatechange = function () {
                if (xmlhttp.readyState == 4) {
                    if ((xmlhttp.status == 200) || (xmlhttp.status == 304)) { document.getElementById(helm).innerHTML = xmlhttp.responseText; }
                    else { document.getElementById(helm).innerHTML = xmlhttp.status + " " + xmlhttp.statusText; }
                }
            }

            xmlhttp.open("GET", url, true);
            xmlhttp.send();
        }


        function UpdateProgress() {
            var url = "PrintToExcel.aspx?getStatus=" + uniqueId;
            AjaxGet(url, "status");

            var data = document.getElementById("status").innerHTML;        
            if (data.startsWith("./Tools/StreamFile")) {              
                MainPage().PC().OpenUrlInActionPane(data);
                Close();
                //    document.location.href = data;
            }
            else {
                if (data.indexOf("/") > 0) {                    
                    var prg = $find('<%= prg.ClientID %>');
                    var parts = data.split("/");
                    prg.set_maxValue(parseInt(parts[1]));
                    prg.set_value(parseInt(parts[0]));                    
                    document.getElementById("visiblestatus").innerHTML = data;

                    document.getElementById("cancelButton").style = "";

                    TimeOutId = window.setTimeout("UpdateProgress()", 1000);
                }
                else if (data.indexOf("cancelled") > 0)
                {
                    //stop polling and show cancelled
                    document.getElementById("visiblestatus").innerHTML = data;
                }
                else
                {
                    TimeOutId = window.setTimeout("UpdateProgress()", 1000);
                }
            }
        }
          function CancelPrint() {
            var url = "PrintToExcel.aspx?cancelJob=" + uniqueId;
              AjaxGet(url, "status");
              Close();
        }
        TimeOutId = window.setTimeout("UpdateProgress()", 1000);
          var uniqueId = '<%= TaskId %>';
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Plh1" runat="Server">
   
    <telerik:RadProgressBar  ID="prg" runat="server" MinValue="0" ShowLabel="false"></telerik:RadProgressBar>
    <div id="statuspanel">
        <div id="visiblestatus"></div>
        <div id="cancelButton" style="display:none">
            <a href="javascript:CancelPrint()"><%=GetLabel("cancel")%></a>
        </div>
    </div>
    <div id="status" style="display:none"></div>
</asp:Content>

