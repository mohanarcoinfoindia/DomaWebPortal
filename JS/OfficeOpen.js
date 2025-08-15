OfficeOpen = (function () {
    if (!Array.prototype.indexOf) {
        Array.prototype.indexOf = function (p, o) {
            if (o == null) {
                o = 0
            } else {
                if (o < 0) {
                    o = Math.max(0, this.length + o);
                }
            } for (var n = o, m = this.length;
n < m;
n++) {
                if (this[n] === p) {
                    return n;
                }
            } return -1;
        }
    } var e = false, i = null, h = 0;
    function l(m) {
        if (typeof m == "undefined") {
            return false
        } var n = String(m);
        return n.substring(n.lastIndexOf(".") + 1, n.length).toLowerCase()
    } function a(m) {
        var n = l(m);
        if (OfficeOpen.wordTypes.split(",").indexOf(n) >= 0) {
            return 0
        } if (OfficeOpen.excelTypes.split(",").indexOf(n) >= 0) {
            return 1
        } if (OfficeOpen.outlookTypes.split(",").indexOf(n) >= 0) {
            return 2
        } if (OfficeOpen.powerTypes.split(",").indexOf(n) >= 0) {
            return 3
        } if (OfficeOpen.accessTypes.split(",").indexOf(n) >= 0) {
            return 4
        } if (OfficeOpen.frontPageTypes.split(",").indexOf(n) >= 0) {
            return 5
        } if (OfficeOpen.publisherTypes.split(",").indexOf(n) >= 0) {
            return 6
        } return -1
    } function b(n) {
        n = String(n || "");
        var m = n.length;
        if (m > 0) {
            var q = n.indexOf("/", 8);
            if (q >= 0 && q < m - 1) {
                var p = n.substring(q + 1, m).split("/");
                for (var o = 0;
o < p.length;
o++) {
                    p[o] = window.encodeURIComponent(p[o]);
                } p.unshift(n.substring(0, q));
                return p.join("/");
            }
        } return n
    } var f = new (function () {
        try {
            this.firefox36up = false;
            this.firefox35up = false;
            var o = navigator.userAgent.toLowerCase();
            this.osver = 1;
            if (o) {
                var n = o.substring(o.indexOf("windows ") + 11);
                this.osver = parseFloat(n)
            } this.major = parseInt(navigator.appVersion);
            this.nav = ((o.indexOf("mozilla") != -1) && ((o.indexOf("spoofer") == -1) && (o.indexOf("compatible") == -1)));
            this.nav6 = this.nav && (this.major == 5);
            this.nav6up = this.nav && (this.major >= 5);
            this.nav7up = false;
            if (this.nav6up) {
                var s = o.indexOf("netscape/");
                if (s >= 0) {
                    this.nav7up = parseInt(o.substring(s + 9)) >= 7;
                }
            } this.ie = (o.indexOf("msie") != -1);
            this.aol = this.ie && o.indexOf(" aol ") != -1;
            if (this.ie) {
                var r = o.substring(o.indexOf("msie ") + 5);
                this.iever = parseInt(r);
                this.verIEFull = parseFloat(r)
            } else {
                this.iever = 0
            } this.ie4up = this.ie && (this.major >= 4);
            this.ie5up = this.ie && (this.iever >= 5);
            this.ie55up = this.ie && (this.verIEFull >= 5.5);
            this.ie6up = this.ie && (this.iever >= 6);
            this.ie7down = this.ie && (this.iever <= 7);
            this.ie7up = this.ie && (this.iever >= 7);
            this.ie8standard = this.ie && document.documentMode && (document.documentMode == 8);
            this.ie9up = this.ie && (this.iever >= 9);
            this.ie9standard = this.ie && document.documentMode && (document.documentMode == 9);
            this.ie10up = this.ie && (this.iever >= 10);
            this.ie10standard = this.ie && document.documentMode && (document.documentMode == 10);
            this.winnt = ((o.indexOf("winnt") != -1) || (o.indexOf("windows nt") != -1));
            this.win32 = ((this.major >= 4) && (navigator.platform == "Win32")) || (o.indexOf("win32") != -1) || (o.indexOf("32bit") != -1);
            this.win64bit = (o.indexOf("win64") != -1) || (o.indexOf("wow64") != -1);
            this.win = this.winnt || this.win32 || this.win64bit;
            this.mac = (o.indexOf("mac") != -1);
            this.ipad = (o.indexOf("ipad") != -1);
            this.iphone = (o.indexOf("iphone") != -1);
            this.ubuntu = (o.indexOf("ubuntu") != -1);
            this.w3c = this.nav6up;
            this.safari = (o.indexOf("webkit") != -1);
            this.safari125up = false;
            this.safari3up = false;
            if (this.safari && this.major >= 5) {
                var q = o.indexOf("webkit/");
                if (q >= 0) {
                    this.safari125up = parseInt(o.substring(q + 7)) >= 125
                } var w = o.indexOf("version/");
                if (w >= 0) {
                    this.safari3up = parseInt(o.substring(w + 8)) >= 3
                }
            } this.firefox = this.nav && (o.indexOf("firefox") != -1);
            this.firefox3up = false;
            this.firefox35up = false;
            this.firefox36up = false;
            this.firefox4up = false;
            if (this.firefox && this.major >= 5) {
                var B = o.indexOf("firefox/");
                if (B >= 0) {
                    var A = o.substring(B + 8);
                    var y = parseInt(A);
                    this.firefox3up = y >= 3;
                    this.firefox4up = y >= 4;
                    var u = parseFloat(A);
                    this.firefox35up = u >= 3.5;
                    this.firefox36up = u >= 3.6
                }
            } this.chrome = this.nav && (o.indexOf("chrome") != -1);
            this.chrome2up = false;
            this.chrome3up = false;
            this.chrome6up = false;
            if (this.chrome) {
                var z = o.indexOf("chrome/");
                if (z >= 0) {
                    var t = o.substring(z + 7);
                    var p = parseInt(t);
                    this.chrome2up = p >= 2;
                    this.chrome3up = p >= 3;
                    this.chrome6up = p >= 6
                }
            } this.opera = this.nav && o.indexOf("opera") != -1;
            this.opera11up = false;
            if (this.opera) {
                var m = o.indexOf("version/");
                if (m >= 0) {
                    var v = o.substring(m + 8);
                    this.opera11up = parseFloat(v) >= 11
                }
            } this.yandex = this.nav && (o.indexOf("YaBrowser") != -1)
        } catch (x) { }
    })();
    function d() {
        var s = function () {
            return f.mac && (f.firefox3up || f.safari3up)
        };
        var v = function (x) {
            return navigator.mimeTypes && navigator.mimeTypes[x] && navigator.mimeTypes[x].enabledPlugin;
        };
        var n = function () {
            var x = v("application/x-sharepoint-webkit");
            var y = v("application/x-sharepoint");
            if (f.safari3up && x) {
                return true
            } return y
        };
        var m = function () {
            var y = null;
            if (false && s()) { //custom : don't start sharepoint
                y = document.getElementById("macSharePointPlugin");
                if (y == null && n()) {
                    var x = null;
                    if (f.safari3up && v("application/x-sharepoint-webkit")) {
                        x = "application/x-sharepoint-webkit"
                    } else {
                        x = "application/x-sharepoint"
                    } var z = document.createElement("object");
                    z.id = "macSharePointPlugin";
                    z.type = x;
                    z.width = 0;
                    z.height = 0;
                    z.style.setProperty("visibility", "hidden", "");
                    document.body.appendChild(z);
                    y = document.getElementById("macSharePointPlugin")
                }
            } return y;
        };
        var r = function () {           
            return (f.winnt || f.win32 || f.win64bit) && f.firefox3up
        };
        var q = function () {
            var x = null;
            if (false && r()) { //customization : disable winFireFoxPlugin
                try {
                    x = document.getElementById("winFirefoxPlugin");
                    if (!x && v("application/x-sharepoint")) {
                        var z = document.createElement("object");
                        z.id = "winFirefoxPlugin";
                        z.type = "application/x-sharepoint";
                        z.width = 0;
                        z.height = 0;
                        z.style.setProperty("visibility", "hidden", "");
                        document.body.appendChild(z);
                        x = document.getElementById("winFirefoxPlugin")
                    }
                } catch (y) {
                    x = null;
                }
            } return x;
        };
        var w = null;
        if (window.ActiveXObject) {
            var o = [function () {
                return new ActiveXObject("SharePoint.OpenDocuments.5")
            }, function () {
                return new ActiveXObject("SharePoint.OpenDocuments.4")
            }, function () {
                return new ActiveXObject("SharePoint.OpenDocuments.3")
            }, function () {
                return new ActiveXObject("SharePoint.OpenDocuments.2")
            }, function () {
                return new ActiveXObject("SharePoint.OpenDocuments.1")
            } ], p = 0, t = o.length;
            for (;
p < t;
++p) {
                try {
                    w = (o[p])();
                    break;
                } catch (u) { }
            }
        } else {
            if (f) {
                try {
                    if (s()) {
                        w = m()
                    } else {
                        if (r()) {
                            w = q();
                        }
                    }
                } catch (u) { }
            }
        } return w
    } function c(q) {
        var s = String(q).replace(/%/g, "%25").replace(/&/g, "%26").replace(/#/g, "%23").replace(/\+/g, "%2B");
        var n = d();
        if (!n && window.ActiveXObject) {
            var p = l(q);
            if (!p) {
                return false;
            } p = "," + p + ",";
            var m;
            if (("," + OfficeOpen.wordTypes + ",").indexOf(p) > -1) {
                m = OfficeOpen.activeXMSOfficeApplications.Word
            } else {
                if (("," + OfficeOpen.excelTypes + ",").indexOf(p) > -1) {
                    m = OfficeOpen.activeXMSOfficeApplications.Excel
                } else {
                    if (("," + OfficeOpen.powerTypes + ",").indexOf(p) > -1) {
                        m = OfficeOpen.activeXMSOfficeApplications.PowerPoint
                    }
                }
            } if (typeof m != "undefined") {
                try {
                    var o = new ActiveXObject(m.app);
                    if (typeof o.Visible != "undefined") {
                        o.Visible = true;
                    } else {
                        if (typeof o.ActiveWindow != "undefined") {
                            o.ActiveWindow.Visible = true;
                        }
                    } if (typeof m.obj != "undefined") {
                        o[m.obj].Open(s);
                        return true;
                    }
                } catch (r) {
                    return false;
                }
            } return false;
        } try {
            n.EditDocument(s);
            return true;
        } catch (r) {
            return false;
        }
    } function k(n) {
        if (window.ActiveXObject) {
            try {
                var r = new ActiveXObject("com.sun.star.ServiceManager");
                if (r) {
                    var q = r.createInstance("com.sun.star.frame.Desktop");
                    if (q) {
                        var o = r.Bridge_GetStruct("com.sun.star.beans.PropertyValue");
                        o.Name = "InteractionHandler";
                        o.Value = r.createInstance("com.sun.star.task.InteractionHandler");
                        var m = new Array();
                        m[0] = o;
                        q.LoadComponentFromURL(n, "_blank", 0, m);
                        return true;
                    }
                }
            } catch (p) {
                return false;
            }
        } return false;
    } function g(m, n) {
        switch (n) {
            case 0: if (!c(m)) {
                    return k(m)
                } return true;
            case 1: if (!k(m)) {
                    return c(m)
                } return true;
            case 2: return c(m);
            case 3: return k(m);
            default: return false;
        }
    } function j(n, r) {
        if (!g(n, r)) {
            var p = document.getElementById("OfficeLauncher");
            if (!p) {
                i = n;
                h = r;
                var q = OfficeOpen.appletContents;
                q = q.replace("_data.accessTypes", OfficeOpen.accessTypes).replace("_data.excelTypes", OfficeOpen.excelTypes).replace("_data.outlookTypes", OfficeOpen.outlookTypes).replace("_data.powerTypes", OfficeOpen.powerTypes).replace("_data.wordTypes", OfficeOpen.wordTypes).replace("_data.frontPageTypes", OfficeOpen.frontPageTypes).replace("_data.publisherTypes", OfficeOpen.publisherTypes);
                var m = document.getElementById("appletHolder");
                if (!m) {
                    var o = document.createElement("div");
                    o.setAttribute("id", "appletHolder");
                    document.appendChild(o);
                    m = document.getElementById("appletHolder");
                }
                m.innerHTML = "";
                m.innerHTML = q;
            } else {
                if (e === true) {
                    p.openDocument(b(n), r, a(n));
                } else {
                    window.onOfficePathNotDetected();
                }
            }
        }
    } window.onLauncherInited = function (m) {
        if (typeof OfficeOpen.LauncherInited == "function") {
            OfficeOpen.LauncherInited(m);
        }
        if (window.opener) { window.close(); } //customization
    };
    window.onOfficePathDetected = function (n) {
        e = true;
        var m = true;
        if (typeof OfficeOpen.BeforeOfficeLaunch == "function") {
            m = OfficeOpen.BeforeOfficeLaunch(n) !== false;
        } if (m && i != null) {
            j(i, h);
            i = null;
            if (typeof OfficeOpen.AfterOfficeLaunch == "function") {
                OfficeOpen.AfterOfficeLaunch(n);
            }
        }
    };
    window.onOfficePathNotDetected = function () {
        e = false;
        if (typeof OfficeOpen.OfficePathNotDetected == "function") {
            OfficeOpen.OfficePathNotDetected();
        }
    };
    return { oooSupportedtypes: ";sxd;sxm;sxi;sxc;sxw;odb;odf;odt;ott;oth;and;odm;stw;sxg;doc;dot;xml;docx;docm;dotx;dotm;wpd;wps;rtf;txt;csv;sdw;sgl;vor;uot;uof;jtd;jtt;hwp;602;pdb;psw;ods;ots;stc;xls;xlw;xlt;xlsx;xlsm;xltx;xltm;xlsb;wk1;wks;123;dif;csv;sdc;dbf;slk;uos;pxl;wb2;odp;odg;std;otp;otg;sti;ppt;pps;pot;pptx;pptm;potx;potm;ppsx;sda;sdd;sdp;uop;cgm;bmp;jpeg;jpg;pcx;psd;svg;wmf;dxf;met;pgm;ras;svm;xbm;emf;pbm;plt;tga;xpm;eps;pcd;png;tif;tiff;gif;pct;ppm;sgf;mml;", accessTypes: "accda,accdb,accdc,accde,accdp,accdr,accdt,accdu,ade,adp,maf,mam,maq,mar,mat,mda,mdb,mde,mdt,mdw,laccdb,snp", excelTypes: "csv,dbf,dif,ods,pdf,prn,slk,xla,xlam,xls,xlsb,xlsm,xlsx,xlt,xltm,xltx,xlw,xml,xml,xps", outlookTypes: "obi,oft,ost,prf,pst,msg,oab,iaf", powerTypes: "emf,odp,pdf,pot,potm,potx,ppa,ppam,pps,ppsm,ppsx,ppt,pptm,pptx,pptx,rtf,thmx,tif,wmf,xml,xps", wordTypes: "doc,docm,docx,dot,dotm,dotx,htm,html,mht,mhtml,odt,pdf,rtf,txt,wps,xml,xml,xps", frontPageTypes: "btr,dwt,elm,fwp,htx,mso", publisherTypes: "pub,bdr,bdrpbr,pbrpubhtml,pubhtml,pubmhtml,puz,wiz", activeXMSOfficeApplications: { Word: { app: "Word.Application", obj: "Documents" }, Excel: { app: "Excel.Application", obj: "Workbooks" }, PowerPoint: { app: "PowerPoint.Application", obj: "Presentations"} }, appletContents: '<applet code="com.elementit.OfficeLauncher.OfficeLauncher" archive="java/OfficeLauncher.jar" width="1" height="1" name="OfficeLauncher" id="OfficeLauncher" mayscript="true" alt="OfficeLauncher by www.element-it.com" VIEWASTEXT><param name="MSOffice.Types.Access" value="_data.accessTypes"> <param name="MSOffice.Types.Excel" value="_data.excelTypes"> <param name="MSOffice.Types.Outlook" value="_data.outlookTypes"> <param name="MSOffice.Types.PowerPoint" value="_data.powerTypes"> <param name="MSOffice.Types.Word" value="_data.wordTypes"> <param name="MSOffice.Types.FrontPage" value="_data.frontPageTypes"> <param name="MSOffice.Types.Publisher" value="_data.publisherTypes"> <param name="progressbar" value="true"> <param name="boxmessage" value="Loading OfficeLauncher Applet ..."> <span style="border:1px solid #FF0000;display:block;padding:5px;margin-top:10px;margin-bottom:10px;text-align:left; background: #FDF2F2;color:#000;">You should <b>enable applets</b> running at browser and to have the <b>Java</b> (JRE) version &gt;= 1.5.<br />If applet is not displaying properly, please check <a target="_blank" href="http://java.com/en/download/help/testvm.xml" title="Check Java applets">additional configurations</a></span> </applet>', GetFileExtension: l, GetAssosiatedMSApp: a, GetMSOSupportedTypes: function () {
        return ";" + OfficeOpen.accessTypes.replace(/,/g, ";") + ";" + OfficeOpen.excelTypes.replace(/,/g, ";") + ";" + OfficeOpen.outlookTypes.replace(/,/g, ";") + ";" + OfficeOpen.powerTypes.replace(/,/g, ";") + ";" + OfficeOpen.wordTypes.replace(/,/g, ";") + ";" + OfficeOpen.frontPageTypes.replace(/,/g, ";") + ";" + OfficeOpen.publisherTypes.replace(/,/g, ";") + ";"
    }, OpenFileWith: function (m, n) {
        h = n;
        j(m, n);
    }
    }
})();