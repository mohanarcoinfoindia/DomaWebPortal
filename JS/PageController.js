var Popups = new Array();
function delegate(that, thatMethod) {
    if (arguments.length > 2) {
        var _params = [];
        for (var n = 2; n < arguments.length; ++n) {
            _params.push(arguments[n]);
        }
        return function () { return thatMethod.apply(that, _params); };
    }
    else {
        return function () { return thatMethod.apply(that, arguments); };
    }

}

function getCookieVal(offset) {
    var endstr = document.cookie.indexOf(";", offset);
    if (endstr == -1) { endstr = document.cookie.length; }
    return unescape(document.cookie.substring(offset, endstr));
}

function SetCookie(name, value, expires, path, domain, secure) {

    var today = new Date();
    today.setTime(today.getTime());

    if (expires) {
        expires = expires * 1000 * 60 * 60;
    }
    var expires_date = new Date(today.getTime() + (expires));

    document.cookie = name + "=" + escape(value) +
        (expires ? ";expires=" + expires_date.toGMTString() : "") +
        (path ? ";path=" + path : "") +
        (domain ? ";domain=" + domain : "") +
        (secure ? ";secure" : "");
}

function GetCookie(name) {
    var arg = name + "=";
    var alen = arg.length;
    var clen = document.cookie.length;
    var i = 0;
    while (i < clen) {
        var j = i + alen;
        if (document.cookie.substring(i, j) == arg) {
            return getCookieVal(j);
        }
        i = document.cookie.indexOf(" ", i) + 1;
        if (i == 0) {
            break;
        }
    }
    return null;
}

function isDefined(x) {
    if (typeof (x) == 'undefined' || x == null) {
        return false;
    }
    else {
        return true;
    }
}

function GetDataTransfer(e) {
    if (!e) e = window.event;
    var d = e.dataTransfer.getData("text");
    if (d != null) {
        var secondchar = d.substring(1, 1);
        if (isNaN(secondchar)) { d = null; }
    }
    return d;
}

function cancelEvent(e) {
    if (!e) { e = window.event; }
    if (e.preventDefault) { e.preventDefault(); }
    if (e.returnValue) { e.returnValue = false; }
    if (e.stopPropagation) { e.stopPropagation(); }
    if (e.cancelBubble != null) { e.cancelBubble = true; }
    return false;
}

function ShowMessage(msg, type, blockui) {

    if (!isDefined(blockui)) { blockui = true; }

    setTimeout(function () { PC().ShowMessage(msg, type, blockui); }, 200);
}

function DocroomPageController(clientID, showpreview, customsite, ctxmenu, reloadfunction, pageloc, ismodalwindow, tabsindetailpreview, ajxmanager, showsuccmsg, detailwindowmaximized, theme, notifWindow) {

    this.ShowMessage = function (msg, type, blockui) {
        //for now always block
        if (true || blockui) {
            alert(msg);
        }
        else {

            var notifWnd = $find(notifWindow);
            switch (type) {
                case "error":
                    msg = "<div style='color:red;'>" + msg + "</div>";
                    break;
            }
            if (notifWnd) {
                notifWnd.set_text(msg);
                notifWnd.show();
            }
            else {
                setTimeout(function () { this.ShowMessage(msg, type, false); }, 200);
            }
        }

    };
    this.ShowSuccess = function (msg, blockui) {
        if (this.ShowSuccessMessages) {
            this.ShowMessage(msg, "info", blockui);
        }
    };
    this.ShowWarning = function (msg, blockui) {
        this.ShowMessage(msg, "warning", blockui);
    };

    this.ShowError = function (msg, blockui) {
        this.ShowMessage(msg, "error", blockui);
    };
    this.preloadImages = function (path) {
        d = document;
        if (d.images) {
            if (!d.p) {
                d.p = new Array();
            }
            j = d.p.length;
            a = preloadImages.arguments;
            for (i = 1; i < a.length; i++) {
                d.p[j] = new Image;
                d.p[j++].src = path + a[i];
            }
        }
    };

    this.GetRedirectUrl = function (url) {


        url = this.AddSiteToUrl(url);
        if (this.IsModalWindow) {
            if (url.indexOf("&modal=") < 0) {
                url = url + '&modal=Y';
            }
        }
        return url;
    };

    this.AddSiteToUrl = function (url) {
        if (url.indexOf("SITE=") < 0) {
            if (url.indexOf('?') > 0) {
                url = url + '&SITE=' + this.CustomSite;
            }
            else {
                url = url + '?SITE=' + this.CustomSite;
            }
        }
        if (url.indexOf("&rnd_str=") < 0) {
            url = url + '&rnd_str=' + Math.random();
        }

        return url;
    };

    this.BrowseFolder = function (id) {
        this.OpenFolder(id, null);
    };
    this.OpenFolder = function (id, packid) {
        if (!isDefined(packid)) {
            packid = "";
        }
        if (this.GetParentID() != id || this.GetPackageID() != packid) {
            parent.SetFolder(id, 0);
            this.ResetGrids();
            this.SetPackageID(packid);
            this.SetParentID(id);
        }
        this.Reload();
    };
    this.ResetGrids = function () {
        for (i = 0; i < this.ResultGrids.length; i++) {
            this.ResultGrids[i].SetCurrentPage(1);
        }
    };
    this.AddToObjectObjectPackage = function (objectid, packid, parentid, site, refreshonclose) {
        if (!isDefined(parentid)) {
            parentid = 0;
        }
        if (!isDefined(site) || site === "") {
            site = "AddToPackage";
        }
     
        var address = 'Default.aspx?TARGET_OBJ_ID=' + objectid + '&TARGET_PACK_ID=' + packid + '&SITE=' + site + '&DEFAULTSITE=' + customsite + '&DM_PARENT_ID=' + parentid + '&DM_ROOT_ID=' + parentid;

        this.OpenModalWindow(address, refreshonclose, window.innerWidth - 40, window.innerHeight - 20, false);
    };
    this.AddToCaseObjectPackage = function (caseid, packid, parentid, site, refreshonclose) {
        if (!isDefined(parentid)) {
            parentid = 0;
        }
        if (!isDefined(site) || site === "") {
            site = "AddToPackage";
        }
   
        var address = 'Default.aspx?TARGET_CASE_ID=' + caseid + '&TARGET_PACK_ID=' + packid + '&SITE=' + site + '&DEFAULTSITE=' + customsite + '&DM_PARENT_ID=' + parentid + '&DM_ROOT_ID=' + parentid;

        this.OpenModalWindow(address, refreshonclose, window.innerWidth - 40, window.innerHeight - 20, false);
    };
    this.AddToObjectCasePackage = function (objectid, packid, parentid, site, refreshonclose) {
        if (!isDefined(parentid)) {
            parentid = 0;
        }
        if (!isDefined(site) || site === "") {
            site = "AddToCasePackage";
        }     
        var a = 'Default.aspx?TARGET_OBJ_ID=' + objectid + '&TARGET_PACK_ID=' + packid + '&SITE=' + site + '&DEFAULTSITE=' + customsite + '&DM_PARENT_ID=' + parentid + '&DM_ROOT_ID=' + parentid;

        this.OpenModalWindow(a, refreshonclose, window.innerWidth - 40, window.innerHeight - 20, false);
    };
    this.AddToCaseCasePackage = function (caseid, packid, parentid, site, refreshonclose) {
        if (!isDefined(parentid)) {
            parentid = 0;
        }
        if (!isDefined(site) || site === "") {
            site = "AddToCasePackage";
        }
     
        var a = 'Default.aspx?TARGET_CASE_ID=' + caseid + '&TARGET_PACK_ID=' + packid + '&SITE=' + site + '&DEFAULTSITE=' + customsite + '&DM_PARENT_ID=' + parentid + '&DM_ROOT_ID=' + parentid;

        this.OpenModalWindow(a, refreshonclose, window.innerWidth - 40, window.innerHeight - 20, false);
    };
    this.SetParentID = function (id) {
        this.ParentIDField.value = id;
    };
    this.GetParentID = function () {
        return this.ParentIDField.value;
    };
    this.SetPackageID = function (id) {
        this.PackageIDField.value = id;
    };
    this.GetPackageID = function () {
        return this.PackageIDField.value;
    };
    this.Reload = function (msg) {

        if (!this.isReloading) {

            this.isReloading = true;
            this.ShowProgress(msg);
            if (this.ReloadFunction == "") {
                document.forms[0].submit();
            }
            else {
                eval(this.ReloadFunction);
                this.isReloading = false;
            }
        }
    };
    this.ReloadAndClosePreview = function () {
        try {
            parent.CloseDetail();
        }
        catch (e) {
        }
        this.Reload();
    };
    this.ReloadAfterModal = function (sender, args) {

        sender.remove_close(RefreshAfterModal);
        this.CascadeReloadContent();
    };

    this.AddResultGrid = function (g) {
        var bfound = false;
        for (var i = 0; i < this.ResultGrids.length; i++) {
            if (this.ResultGrids[i].clientID == g.clientID) {
                bfound = true;
                this.ResultGrids[i] = g;
                break;
            }
        }
        if (!bfound) {
            this.ResultGrids.push(g);
        }
    };
    this.GetDefaultResultGrid = function () {
        return this.ResultGrids[0];
    };
    this.currgrid = 0;
    this.SetCurrentResultGrid = function (cid) {
        for (var i = 0; i < this.ResultGrids.length; i++) {
            if (this.ResultGrids[i] != null && this.ResultGrids[i].clientID == cid) {
                this.currgrid = i;
                break;
            }
        }
    };
    this.GetCurrentResultGrid = function () {

        return this.ResultGrids[this.currgrid];
    };

    this.HandleEnter = function () {
        if (this.ScreenModeField) {
            if ((this.ScreenModeField.value == 'query')) {
                return PC().ChangeScreenMode('advsearch');
            }
            else {
                if (this.GetCurrentResultGrid().Visible) {
                    this.GetCurrentResultGrid().Goto(1);
                }
                else {
                    this.Reload();
                }
            }

        }
    };
    this.ShowFile = function (file_id, fromarchive, rend, frommail, terms, forceinpreviewpane, mailid, forceinpopup) {
        //if (!rend) {
        //    rend = '';
        //}
        var u;
        if (!(this.IsSafari && this.IsMobile)) {
            u = "DM_PREVIEW.aspx?FILE_ID=" + file_id;
        }
        else {
            u = "DM_VIEW_FILE.aspx?hidetoolbar=Y&FILE_ID=" + file_id;
            forceinpopup = true;
        }

        if (rend) {
            u = u + "&rend=" + rend;
        }
        if (terms) {
            u = u + "&terms=" + terms;
        }
        if (fromarchive) {
            u = u + "&fromarchive=" + fromarchive;
        }
        switch (this.PageLocation) {
            case 4:
                if (this.TabsInDetailPreview)
                    u = u + '&tabnode=Sub';
                else
                    u = u + '&hidetabs=Y';
                break;
        }
        if (frommail) {
            u = u + "&frommail=Y";
            u = u + "&DM_MAIL_ID=" + mailid;
        }
        console.log("Viewfile " + u);
        this.OpenPreviewLink(u, null, forceinpreviewpane, forceinpopup, "file" + file_id);
    };


    this.PersistDetailWindowSize = function () {

        if (!window.opener) {
            return;
        }

        var CookieContent;
        var winW = window.innerWidth;
        var winH = window.innerHeight;
        var winL = window.screenLeft - 4;
        var winT = window.screenTop - 23;
        var WindowName = window.name;
        var oneYear = 7 * 24 * 60 * 60 * 52000;
        var expDate = new Date();
        expDate.setTime(expDate.getTime() + oneYear);

        if (window.innerWidth) {
            //FF
            winW = window.innerWidth;
            winH = window.innerHeight;
            winL = window.screenX;
            winT = window.screenY;
        }
        else {
            //IE
            winW = document.body.offsetWidth - 4;
            winH = document.body.offsetHeight - 4;
            winL = window.screenLeft - 4;
            winT = window.screenTop - 23;
        }

        var CookieName = 'DMDETSIZE';
        CookieContent = '_l:' + escape(winL) + '|_t:' + escape(winT);
        CookieContent += '|_h:' + escape(winH) + '|_w:' + escape(winW) + '; expires=' + expDate.toGMTString();

        document.cookie = CookieName + '=' + CookieContent;
    };

    this.OpenDetailWindow = function (url, objectid, modal, gridId) {
        this.SetOpeningGridId(gridId);
        if (!modal) {
            var sFeatures = '';
            if (objectid == 0) {
                objectid = Math.floor((Math.random() * 10000) + 1);
            }

            if (!this.DetailWindowsMaximized) {
                var lLeft = '';
                var lTop = '';
                var lHeight = '';
                var lWidth = '';
                var myCookie = ' ' + document.cookie + ';';
                //left
                var CookieName = 'DMDETSIZE';
                var startOfCookie = myCookie.indexOf(CookieName);
                var endOfCookie;

                if (startOfCookie != -1) {
                    startOfCookie += CookieName.length + 4;
                    endOfCookie = myCookie.indexOf('|', startOfCookie);
                    lLeft = unescape(myCookie.substring(startOfCookie, endOfCookie));

                    startOfCookie = endOfCookie + 4;
                    endOfCookie = myCookie.indexOf('|', startOfCookie);
                    lTop = unescape(myCookie.substring(startOfCookie, endOfCookie));

                    startOfCookie = endOfCookie + 4;
                    endOfCookie = myCookie.indexOf('|', startOfCookie);
                    lHeight = unescape(myCookie.substring(startOfCookie, endOfCookie));

                    startOfCookie = endOfCookie + 4;
                    endOfCookie = myCookie.indexOf(';', startOfCookie);
                    lWidth = unescape(myCookie.substring(startOfCookie, endOfCookie));
                }


                if (lLeft != '') {
                    sFeatures += 'left=' + lLeft + ',';
                }
                if (lTop != '') {
                    sFeatures += 'top=' + lTop + ',';
                }

                if (lHeight === '') { lHeight = this.DetailWindowsHeight; }
                if (lWidth === '') { lWidth = this.DetailWindowsWidth; }


                sFeatures += 'height=' + lHeight + ',';

                sFeatures += 'width=' + lWidth + ',';


                sFeatures += 'resizable=yes,scrollbars=yes,status=yes';

                this.OpenWindow(url, 'MyDetailWindow' + objectid, sFeatures);
            } else {

                sFeatures = 'height=' +
                    screen.availHeight +
                    ',width=' +
                    screen.availWidth +
                    ',left=0,top=0,resizable=yes,scrollbars=yes,status=yes';
                var i = Popups.length;
                Popups[i] = window.open(url, 'MyDetailWindow' + objectid, sFeatures);
                Popups[i].moveTo(0, 0);
                Popups[i].focus();
            }
        } else {


            switch (this.PageLocation) {
                case 2:
                case 7:
                    parent.PC().OpenDetailWindow(url, objectid, true, gridId);

                    break;
                default:
                    url = this.AddSiteToUrl(url);
                    if (url.indexOf("&modal=") < 0) {
                        url = url + '&modal=Y';
                    }

                    let offSet = 20;

                    var oWnd = GetRadWindowManager().getWindowByName("wndModalDetail");
                    oWnd.set_title("");
                    oWnd.setUrl(url);


                    // var oWnd = GetRadWindowManager().open(url, "wndModalDetail");
                    oWnd.moveTo(offSet, offSet);
                    oWnd.setSize((window.innerWidth - (2 * offSet)), (window.innerHeight - (2 * offSet)));
                    oWnd.show();

                    break;
            }


        }
    };

    var popupBlockingErrorShown;

    this.OpenWindow = function (url, name, args) {

        if (!args) {
            args = 'width=' + this.DetailWindowsWidth + ',height=' + this.DetailWindowsHeight + ',resizable=yes,scrollbars=yes,status=yes';
        }

        name = '60' + name;
        url = this.AddSiteToUrl(url);

        var i = Popups.length;
        Popups[i] = window.open(url, name, args);

        try {

            Popups[i].focus();

        } catch (e) {
            if (!popupBlockingErrorShown) {
                if (Popups[i - 1]) { //there is a previous popup and it's been focused on.
                    Popups[i - 1].alert(UIMessages[33]); //show alert on opened popup.
                    popupBlockingErrorShown = true;
                }

                alert(UIMessages[33]);
                popupBlockingErrorShown = true;
            }
        }
    };

    this.OpenModalWindowRelativeSize = function (url, autorefresh) {

        const width = window.innerWidth * 0.7;
        const height = window.innerHeight * 0.8;

       return this.OpenModalWindow(url, autorefresh, width, height, true);
    };

    this.OpenModalWindow = function (url, autorefresh, width, height, center, maximize, offset) {  // use 'offset' as { left: int, top: int } in pixels   
        if (this.PageLocation != 3) {
            url = this.AddSiteToUrl(url);
            if (url.indexOf("&modal=") < 0) {
                url = url + '&modal=Y';
            }
            let oWnd;

            if (!(width > 0 && height > 0)) {

                if (url.indexOf("&autosize=") < 0) {
                    url = url + '&autosize=Y';
                }
                oWnd = GetRadWindowManager().getWindowByName("wndModalAutosize");
                oWnd.set_title("");
                oWnd.setUrl(url);
            }
            else {
                oWnd = GetRadWindowManager().getWindowByName("wndModalManualSize");
                oWnd.set_title("");
                oWnd.setUrl(url);
                oWnd.setSize(width, height);
                oWnd.set_modal(true);    // needed to remove unnecessary scrollbars on parent
            }
            if (autorefresh) //autorefresh
            {
                oWnd.add_close(RefreshAfterModal);
            }

            if (!center) {
                oWnd.set_centerIfModal(false);
                if (offset) {
                    oWnd.moveTo(offset.left || 0, offset.top || 0);
                } 
                else {
                    var ie = document.all ? true : false;
                    var t = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop;
                    oWnd.moveTo(0, t);
                }
            } 
            else {
                oWnd.center();
                if (offset) {
                    oWnd.moveTo(oWnd.get_left() + offset.left, oWnd.get_top() + offset.top);
                }
            }
            oWnd.show();
            if (isDefined(maximize) && maximize) {
                oWnd.maximize();
            }

          
            return oWnd;
        }
        else {
            return parent.PC().OpenModalWindow(url, autorefresh, width, height, center, maximize);
        }
    };
    this.AddCase = function (folderid, procid, packid, modal) {
        if (!isDefined(packid)) {
            packid = this.GetPackageID();
        }
        if (!isDefined(procid)) {
            procid = 0;
        }
        if (!isDefined(folderid)) {
            folderid = this.GetParentID();
        }
        var url;
        if (procid == 0) {
            url = 'DM_SELECT_OBJ_CAT.aspx?CAT_TYPE=Procedure&DM_PARENT_ID=' + folderid + '&PACK_ID=' + packid;
        }
        else {
            url = 'DM_NEW_CASE.ASPX?PROC_ID=' + procid + '&DM_PARENT_ID=' + folderid + '&PACK_ID=' + packid;
        }
        this.OpenDetailWindow(url, packid, modal);
    };
    this.AddObject = function (cattype, folderid, catid, packid, modal, gridId) {
        if (!isDefined(packid)) {
            packid = this.GetPackageID();
        }
        if (!isDefined(catid)) {
            catid = 0;
        }
        if (!isDefined(folderid)) {
            folderid = this.GetParentID();
        }
        var url;
        if (catid == 0) {
            url = 'DM_SELECT_OBJ_CAT.aspx?CAT_TYPE=' + cattype + '&DM_PARENT_ID=' + folderid + '&PACK_ID=' + packid;
        }
        else {
            url = 'DM_NEW_OBJECT.ASPX?DM_CAT_ID=' + catid + '&DM_PARENT_ID=' + folderid + '&PACK_ID=' + packid + '&CAT_TYPE=' + cattype;
        }
        this.SetOpeningGridId(gridId);
        // always open in modal
        modal = true;
        this.OpenDetailWindow(url, packid, modal);
    };
    this.AddObjectToCase = function (cattype, folderid, catid, caseid, packid, modal) {
        if (!isDefined(packid)) {
            packid = this.GetPackageID();
        }
        if (!isDefined(folderid)) {
            folderid = this.GetParentID();
        }
        var url;
        if (catid == 0) {
            url = 'DM_SELECT_OBJ_CAT.aspx?CAT_TYPE=' + cattype + '&DM_PARENT_ID=' + folderid + '&PACK_ID=' + packid + "&CASE_ID=" + caseid;
        }
        else {
            url = 'DM_NEW_OBJECT.ASPX?DM_CAT_ID=' + catid + '&DM_PARENT_ID=' + folderid + '&PACK_ID=' + packid + "&CASE_ID=" + caseid;
        }
        this.OpenDetailWindow(url, packid, modal);
    };
    this.OpenPreviewLink = function (url, CanBeFullScreen, forceinpreviewpane, forceinpopup, windowName) {
        url = this.AddSiteToUrl(url);
        if (CanBeFullScreen == null) {
            CanBeFullScreen = false;
        }

        if (!forceinpopup && (this.ShowPreview || forceinpreviewpane) && (typeof parent.ShowDetail == 'function') && parent.HasDetailPane()) {
            url += "&preview=Y";
            parent.ShowDetail(url, (this.FullScreen && CanBeFullScreen));
        }
        else {
            if (!isDefined(windowName)) {
                windowName = "MySubDetailWindow";
            }
            this.OpenWindow(url, windowName);
        }
    };
    this.GotoLink = function (url) {
        location.href = this.GetRedirectUrl(url);
    };

    this.AddFileToClipboard = function (fileid, fileaction) {
        SetCookie("FileClipBoard", fileid, 1, null, null, null);
        SetCookie("FileClipBoardAction", fileaction, 1, null, null, null);
    };

    this.AddMailFileToClipboard = function (fileid, fileaction) {
        SetCookie("FileClipBoard", "M" + fileid, 1, null, null, null);
        SetCookie("FileClipBoardAction", fileaction, 1, null, null, null);
    };
    this.ToggleRQManualStatus = function (rqid, itm) {
        if (itm.checked) {
            DocroomListFunctions.ToggleRQManualStatus(rqid, true);
        }
        else {
            DocroomListFunctions.ToggleRQManualStatus(rqid, false);
        }
    };
    this.callbackid = 0;
    this.CheckOutFileCallBack = function (ResponseObject) {
        if (ResponseObject > 0) {
            this.CheckInFile(ResponseObject);
        }
    };
    this.CheckOutFile = function (fileid) {
        DocroomListFunctions.CheckoutFile(fileid, "", delegate(this, this.CheckOutFileCallBack), delegate(this, this.OnAjaxError));
    };
    this.CancelCheckOutFile = function (fileid) {
        DocroomListFunctions.CancelCheckoutFile(fileid, delegate(this, this.Reload));
    };
    this.ReIndexFile = function (fileid) {
        DocroomListFunctions.ReIndexFile(fileid, delegate(this, this.Reload));
    };

    this.CheckOutCallBack = function (args) {
        if (args != null) {
            DocroomListFunctions.CheckoutDocument(this.callbackid, args, delegate(this, this.Reload), delegate(this, this.OnAjaxError));
        }
    };
    this.OnAjaxError = function (error) {
        this.ShowError(error.get_message());
    };

    this.EditObjectCallback = function (ResponseObject) {
        if (ResponseObject > 0) {
            this.OpenDetailWindow('dm_detail.aspx?DM_OBJECT_ID=' + ResponseObject + '&mode=2', ResponseObject); //edit the doc
        }
    };

    var tempFileID = 0;
    var tempFolderID = 0;
    var selectCatForMailCalled = false;
    var selectCatForMailFileCalled = false;
    var tempToObjID = 0;
    var tempObjID = 0;
    var tempItemId;
    var tempItemType;
    var bSelectPackCalled = false;
    var SelectDataSetCalled = false;

    this.PromoteMailFileToDocCallBack = function (winw) {
        if (winw.argument) {
            this.PromoteMailFileToDoc(tempFileID, tempFolderID, winw.argument);
        }
        winw.argument = null;
        winw.setUrl('about:blank');
    };

    this.PromoteMailFileToDoc = function (fileid, folderid, catid) {

        if (catid == 0) {
            var oWnd = radopen('./UserControls/PromptCategory.aspx?modal=Y&DM_PARENT_ID=' + folderid + '&DM_OBJECT_TYPE=Document', 'wndModalManualSize');
            oWnd.setSize(500, 100);
            tempFileID = fileid;
            tempFolderID = folderid;
            if (!selectCatForMailFileCalled) {
                selectCatForMailFileCalled = true;
                oWnd.add_close(delegate(this, this.PromoteMailFileToDocCallBack));
            }
        }
        else {
            DocroomListFunctions.PromoteMailFileToDoc(fileid, folderid, catid, delegate(this, this.EditObjectCallback), delegate(this, this.OnAjaxError));
        }
    };
    this.PromoteMailToDocCallBack = function (winw) {
        if (winw.argument) {
            this.PromoteMailToDoc(tempObjID, tempFolderID, winw.argument);
        }
        winw.argument = null;
        winw.setUrl('about:blank');
    };
    this.PromoteMailToDoc = function (mailid, folderid, catid) {
        if (catid == 0) {
            var oWnd = radopen('./UserControls/PromptCategory.aspx?modal=Y&DM_PARENT_ID=' + folderid + '&DM_OBJECT_TYPE=Document', 'wndModalManualSize');
            oWnd.setSize(500, 100);
            tempObjID = mailid;
            tempFolderID = folderid;
            if (!selectCatForMailCalled) {
                selectCatForMailCalled = true;
                oWnd.add_close(delegate(this, this.PromoteMailToDocCallBack));
            }
        }
        else {
            DocroomListFunctions.PromoteMailToDoc(mailid, folderid, catid, delegate(this, this.EditObjectCallback), delegate(this, this.OnAjaxError));
        }
    };
    this.CheckOut = function (docid) {
        this.callbackid = docid;
        radprompt('', delegate(this, this.CheckOutCallBack), 330, 100, null, UIMessages[25], '');
    };
    this.CancelCheckOut = function (docid) {
        DocroomListFunctions.CancelCheckout(docid, delegate(this, this.Reload));
    };

    this.Unlock = function (docid) {
        DocroomListFunctions.UnlockDocument(docid, delegate(this, this.Reload));
    };
    this.RemoveLink = function (sourceid, targetid, linktype) {
        DocroomListFunctions.RemoveLink(sourceid, targetid, linktype, delegate(this, this.Reload));
    };

    this.SetMainFile = function (fileid) {
        DocroomListFunctions.SetMainFile(fileid, delegate(this, this.ReloadAndClosePreview));
    };
    this.ToggleFileReadOnly = function (fileid) {
        DocroomListFunctions.ToggleFileReadOnly(fileid, delegate(this, this.ReloadAndClosePreview));
    };
    this.EditPublishing = function (itemid, itemtype) {
        this.OpenWindow('EditPublishing.aspx?ITEM_ID=' + itemid + '&ITEM_TYPE=' + itemtype, 'EditPublishing', 'height=600,width=900,scrollbars=yes,resizable=yes');
    };
    this.SubscribeTo = function (id, type, qryval) {
        if (!isDefined(qryval)) { qryval = ""; }

        this.OpenModalWindow('Subscription.aspx?OBJ_ID=' + id + '&OBJ_TYPE=' + type + '&QRY_VALUE=' + qryval, false, 800, 600, true, false);
    };
    this.SubscribeToFolder = function (id) {
        this.SubscribeTo(id, "Folder", "");
    };
    this.SubscribeToQuery = function (id) {
        this.SubscribeTo(id, "Query", "");
    };

    this.DoDocumentListActionDoc = function (actiontype, actionid, docid, from) {
        this.ShowProgress();
        if (!from) from = "1";
        this.OpenUrlInActionPane('DM_DocumentListAction.aspx?ACTIONTYPE=' + actiontype + '&SELTYPE=5&ACTIONID=' + actionid + '&OIP=' + docid + "&FROM=" + from);
    };
    this.DoDocumentListActionCase = function (actiontype, actionid, techid, from) {
        this.ShowProgress();
        if (!from) from = "1";
        this.OpenUrlInActionPane('CaseListAction.aspx?ACTIONTYPE=' + actiontype + '&SELTYPE=5&ACTIONID=' + actionid + '&OIP=' + techid + "&FROM=" + from);
    };

    this.ShowProgress = function (msg) {
        if (pageloc == 2) {
            ProgressIndicator.display(msg);
        }
    };
    this.HideProgress = function () {
        ProgressIndicator.hide();
    };
    this.OpenUrlInActionPane = function (url) {
        if (typeof parent.DoActionUrl == 'function') {
            parent.DoActionUrl(url);
        }
        else {
            this.OpenWindow(url, 'ActionWindow', 'height=200,width=400,scrollbars=yes,resizable=yes,toolbar=yes,status=yes');
        }
    };
    ///links the current selection to objid
    this.LinkObjectsToOIP = function (objid, reltype) {
        if (!reltype) {
            reltype = 0;
        }
        if (reltype > 0) {

            //todo
            //            document.forms[0].SELECTION.value = objid;
            //            document.forms[0].SELTYPE.value = '1';
            //            document.forms[0].ACTIONTYPE.value = '3';
            //            document.forms[0].ACTIONID.value = reltype;
            //            document.forms[0].target = this.ActionPane;
            //            document.forms[0].action = 'DM_DocumentListAction.aspx'
            //            document.forms[0].submit();
            //            document.forms[0].SELECTION.value = '';
            //            document.forms[0].SELTYPE.value = '';
            //            document.forms[0].ACTIONTYPE.value = '';
            //            document.forms[0].ACTIONID.value = '';
            //            document.forms[0].target = '';
            //            document.forms[0].action = '';
            this.ShowError('todo');
        }
        else {
            this.OpenModalWindow('DM_LINk_DOCS.aspx?OIP=' + objid, true, 500, 300, true);
        }
    };


    this.RenameObject = function (id) {
        var url = "DM_RENAME_OBJECT.aspx?DM_OBJECT_ID=" + id;
        if (pageloc == 3) {
            this.OpenWindow(url, "RenameObject", "height=300,width=500,scrollbars=yes,resizable=yes");
        }
        else {
            this.OpenModalWindow(url, true, 0, 0, true, false);
        }

    };

    this.SetObjectLabel = function (id, seltype, selid) {
        var url = "DM_RENAME_OBJECT.aspx?DM_OBJECT_ID=" + id + "&SEL_TYPE=" + seltype + "&SEL_ID=" + selid;

        if (pageloc == 3) {
            this.OpenWindow(url, "RenameObject", "height=300,width=500,scrollbars=yes,resizable=yes");
        }
        else {
            this.OpenModalWindow(url, true, 0, 0, true, false);
        }

    };

    this.DeleteObject = function (id) {
        if (confirm(UIMessages[4])) {
            DocroomListFunctions.DeleteObject(id, delegate(this, this.ReloadAndClosePreview), delegate(this, this.OnAjaxError));
        }
    }
    this.DeleteMailFromBox = function (mailid, boxid) {
        if (confirm(UIMessages[4])) {
            DocroomListFunctions.DeleteMailFromBox(mailid, boxid, delegate(this, this.ReloadAndClosePreview));
        }
    };
    this.CloseTrackingOnMail = function (mailid) {
        DocroomListFunctions.CloseTrackingOnMail(mailid, delegate(this, this.Reload));
    };
    this.StopFollowupOnMail = function (mailid) {
        DocroomListFunctions.StopFollowupOnMail(mailid, delegate(this, this.Reload));
    };
    this.ToggleReadOnCase = function (techid) {
        DocroomListFunctions.ToggleReadOnCase(techid, delegate(this, this.Reload));
    };
    this.ToggleReadOnMail = function (mailid) {
        DocroomListFunctions.ToggleReadOnMail(mailid, delegate(this, this.Reload));
    };
    this.RestoreMailBoxItem = function (mailid, boxid) {
        DocroomListFunctions.RestoreMailBoxItem(mailid, boxid, delegate(this, this.Reload));
    };

    this.AddToDataSet = function (itemId, itemType, datasetId) {
        if (datasetId == 0) {
            var oWnd = radopen('./UserControls/PromptDataSet.aspx?modal=Y&ITEM_ID=' + itemId, 'wndModalManualSize');
            oWnd.setSize(400, 100);
            tempItemId = itemId;
            tempItemType = itemType;
            if (!SelectDataSetCalled) {
                SelectDataSetCalled = true;
                oWnd.add_close(delegate(this, this.AddToDataSetSelectCallBack));
            }
        }
        else {           
            DocroomListFunctions.AddToDataSet(datasetId, itemId, itemType, delegate(this, this.AddToDataSetDone), delegate(this, this.OnAjaxError));
        }
    };
    this.AddToDataSetSelectCallBack = function (winw) {       
        if (winw.argument) {           
            this.AddToDataSet(tempItemId, tempItemType, winw.argument);
        }
        winw.argument = null;
        winw.setUrl('about:blank');
    };
    this.AddToDataSetDone = function () {
        this.ShowSuccess("The object was added to the dataset");
    };

    this.LinkToPackageDone = function () {
        this.ShowSuccess(UIMessages[18]);
    };
    this.LinkDocToRoutingCase = function (ToCaseID, PackID, ObjID) {
        DocroomListFunctions.LinkObjectToRoutingCase(ObjID, ToCaseID, PackID, delegate(this, this.LinkToPackageDone), delegate(this, this.OnAjaxError));
    };
   
    this.LinkDocToObjectPackageSelectCallBack = function (winw) {
        if (winw.argument) {
            this.LinkDocToObjectPackage(tempToObjID, winw.argument, tempObjID);
        }
        winw.argument = null;
        winw.setUrl('about:blank');
    };
    this.LinkDocToObjectPackage = function (ToObjID, PackID, ObjID) {
        if (PackID == 0) {
            var oWnd = radopen('./UserControls/PromptPackage.aspx?modal=Y&TO_OBJECT_ID=' + ToObjID + '&DM_OBJECT_ID=' + ObjID, 'wndModalManualSize');
            oWnd.setSize(400, 100);
            tempToObjID = ToObjID;
            tempObjID = ObjID;
            if (!bSelectPackCalled) {
                bSelectPackCalled = true;
                oWnd.add_close(delegate(this, this.LinkDocToObjectPackageSelectCallBack));
            }
        }
        else {
            DocroomListFunctions.LinkObjectToObjectPackage(ObjID, ToObjID, PackID, delegate(this, this.LinkToPackageDone), delegate(this, this.OnAjaxError));
        }
    };
    this.LinkCaseToRoutingCase = function (ToCaseID, PackID, CaseID) {
        DocroomListFunctions.LinkCaseToRoutingCase(CaseID, ToCaseID, PackID, delegate(this, this.LinkToPackageDone), delegate(this, this.OnAjaxError));
    };
    this.LinkCaseToObjectPackage = function (ToObjID, PackID, CaseID) {
        DocroomListFunctions.LinkCaseToObjectPackage(CaseID, ToObjID, PackID, delegate(this, this.LinkToPackageDone), delegate(this, this.OnAjaxError));
    };
    this.LinkArchivedCaseToRoutingCase = function (ToCaseID, PackID, CaseID) {
        DocroomListFunctions.LinkArchivedCaseToRoutingCase(CaseID, ToCaseID, PackID, delegate(this, this.LinkToPackageDone), delegate(this, this.OnAjaxError));
    };
    this.LinkArchivedCaseToObjectPackage = function (ToObjID, PackID, CaseID) {
        DocroomListFunctions.LinkArchivedCaseToObjectPackage(CaseID, ToObjID, PackID, delegate(this, this.LinkToPackageDone), delegate(this, this.OnAjaxError));
    };
    this.Comments = function (id, fromarchive) {
        this.OpenPreviewLink('Comments.aspx?DM_OBJECT_ID=' + id + '&IGNORESESSIONTAB=Y&fromarchive=' + fromarchive, null, false, false);
    };


    this.CheckIn = function (docid) {
        this.OpenModalWindow('DM_Checkin.aspx?DM_OBJECT_ID=' + docid, true, 0, 0, true);
    };
    this.EditWork = function (techid) {
        this.OpenModalWindow('CaseWork.aspx?TECH_ID=' + techid, false, 800, 600, true, false);
    };
    this.ToggleSuspendCase = function (techid) {
        DocroomListFunctions.ToggleSuspendCase(techid, delegate(this, this.Reload));
    };
    this.UnlockCase = function (techid) {
        DocroomListFunctions.UnlockCase(techid, delegate(this, this.Reload));
    };

    this.AddFile = function (objid, packid) {
        if (!isDefined(packid)) {
            packid = '';
        }
        this.OpenWindow('DM_FILE_ADD.aspx?DM_OBJECT_ID=' + objid + '&PACK_ID=' + packid, 'AddFile', 'height=400,width=800,scrollbars=yes,resizable=yes,status=yes');
    };
    this.AddFileAsMessage = function (objid) {
        this.OpenWindow('DM_FILE_ADD.aspx?DM_OBJECT_ID=' + objid + '&addas=Message', 'AddFile', 'height=400,width=800,scrollbars=yes,resizable=yes,status=yes');
    };
    this.ScanFile = function (objid) {
        this.OpenWindow('./WebScan/Scan.aspx?DM_OBJECT_ID=' + objid, 'AddFile', 'height=550,width=800,scrollbars=no,resizable=yes,status=yes');
    };
    this.AddMail = function (objid) {
        this.ComposeMail(0, '', objid, 0, 2);
    };
    this.AddMessage = function (objid) {
        this.ComposeMail(0, '', objid, 0, 4);
    };
    this.ComposeMail = function (sourcemailid, action, objid, template, addasfiletype) {
        this.OpenWindow('./MailClient/Compose2.aspx?DM_MAIL_ID=' + sourcemailid + '&ACTION=' + action + '&TEMPLATE_ID=' + template + '&DM_ADD_TO_OBJECT_ID=' + objid + '&DM_ADD_TO_OBJECT_FILE_TYPE=' + addasfiletype, 'AddMail', 'height=600,width=800,scrollbars=no,resizable=yes,status=yes');
    };
    this.ShowObjectACL = function (objid) {
        parent.PC().OpenModalWindowRelativeSize("DM_ACL.ASPX?DM_OBJECT_ID=" + objid, true);        
    };
    this.ShowObjectIndex = function (objid) {
        this.OpenWindow("DM_VIEW_OBJ_CONTENT.aspx?DM_OBJECT_ID=" + objid, 'IDX', 'height=600,width=800,scrollbars=yes,resizable=yes');
    };
    this.ShowCaseIndex = function (techid) {
        this.OpenWindow("DM_VIEW_OBJ_CONTENT.aspx?TECH_ID=" + techid, 'IDX', 'height=600,width=800,scrollbars=yes,resizable=yes');
    };
    this.ReplaceFile = function (fileid) {
        this.OpenWindow('DM_FILE_ADD.aspx?FILE_ID=' + fileid, 'AddFile', 'height=600,width=800,scrollbars=yes,resizable=yes');
    };
    this.PasteFile = function (id) {
        this.OpenWindow('DM_FILE_ADD.aspx?DM_OBJECT_ID=' + id + '&tab=5', 'AddFile', 'height=400,width=800,scrollbars=no,resizable=yes');
    };
    this.EditFile = function (id) {
        if (!this.IsFirefox) {
            this.OpenUrlInActionPane('DM_EDIT_FILE.aspx?FILE_ID=' + id);
        }
        else {
            this.OpenWindow("DM_EDIT_FILE.aspx?FILE_ID=" + id, "Editfile", "height=50,width=50,scrollbars=yes,resizable=yes");
        }
    };
    this.EditFileDirect = function (id) {
        this.OpenWindow("DM_EDIT_FILE_DIRECT.aspx?FILE_ID=" + id, "Editfile", "height=600,width=1100,scrollbars=yes,resizable=yes");
    };
    this.EditComment = function (id) {
        this.OpenWindow("DM_EDIT_FILE_DIRECT.aspx?DM_FILE_TYPE=3&FILE_ID=" + id, "Editfile", "height=600,width=1100,scrollbars=yes,resizable=yes");
    };
    this.NewComment = function (objid) {
        this.OpenWindow("DM_EDIT_FILE_DIRECT.aspx?DM_FILE_TYPE=3&DM_OBJECT_ID=" + objid + "&FILE_ID=0", "Editfile", "height=600,width=1100,scrollbars=yes,resizable=yes");
    };
    this.EditMessage = function (id) {
        this.OpenWindow("DM_EDIT_FILE_DIRECT.aspx?DM_FILE_TYPE=4&FILE_ID=" + id, "Editfile", "height=600,width=1100,scrollbars=yes,resizable=yes");
    };
    this.NewMessage = function (objid) {
        this.OpenWindow("DM_EDIT_FILE_DIRECT.aspx?DM_FILE_TYPE=4&DM_OBJECT_ID=" + objid + "&FILE_ID=0", "Editfile", "height=600,width=1100,scrollbars=yes,resizable=yes");
    };
    this.DeleteFile = function (fileid) {
        if (!confirm(UIMessages[2])) {
            return;
        }
        DocroomListFunctions.DeleteFile(fileid, delegate(this, this.ReloadAndClosePreview));
    };
    this.DeleteComment = function (fileid) {
        if (!confirm(UIMessages[4])) {
            return;
        }
        DocroomListFunctions.DeleteFile(fileid, delegate(this, this.Reload));
    };
    this.DeleteMessage = function (messageid) {
        if (!confirm(UIMessages[4])) {
            return;
        }
        DocroomListFunctions.DeleteMessage(messageid, delegate(this, this.Reload));
    };
    this.DeleteCaseNote = function (noteid) {
        if (!confirm(UIMessages[4])) {
            return;
        }
        DocroomListFunctions.DeleteCaseNote(noteid, delegate(this, this.Reload));
    };
    this.AddCaseNote = function (caseid, text) {

        DocroomListFunctions.AddCaseNote(caseid, text, delegate(this, this.Reload));
    };
    this.EditFileProps = function (fileid) {
        this.ShowFileAction(fileid, 1);
    };
    this.CheckInFile = function (fileid) {
        this.ShowFileAction(fileid, 0);
    };
    this.CollectFile = function (fileid, force) {
        DocroomListFunctions.CollectFile(fileid, force, delegate(this, this.ReloadAndClosePreview), delegate(this, this.OnAjaxError));
    };

    this.ShowFileAction = function (fileid, action) {
        this.OpenModalWindow('DM_FILEACTIONS.aspx?DM_FILE_ID=' + fileid + '&action=' + action, true, 0, 0, true);
    };

    this.DownLoadFile = function (fileid, fromarchive) {
        this.OpenUrlInActionPane('DM_VIEW_FILE.aspx?FILE_ID=' + fileid + '&attach=Y&fromarchive=' + fromarchive + '&x=' + Math.random());
    };
    this.DownLoadMailFile = function (fileid, fromarchive) {
        this.OpenUrlInActionPane('DM_VIEW_FILE.aspx?FILE_ID=' + fileid + '&attach=Y&frommail=Y&x=' + Math.random());
    };

    this.CopyMoveObject = function (objectid) {
        this.OpenModalWindow('DM_CopyMoveToFolder.ASPX?DM_OBJECT_ID=' + objectid, true, 600, 400, true);
    };

    this.GetScreenMode = function () {
        return this.ScreenModeField.value;
    };
    this.SetScreenMode = function (mode) {
        this.ScreenModeField.value = mode;
    };
    this.ChangeScreenMode = function (mode) {
        var bCont = true;
        if (this.ClientValidate) {
            if (window.Page_ClientValidate) {
                window.Page_ClientValidate(this.ValidationGroup);
                bCont = window.Page_IsValid;
            }
        }
        if (bCont) {
            this.SetScreenMode(mode);

            this.Reload();
            return true;
        }
        else {
            return false;
        }
    };


    this.SetContextTimeout = function (t) {
        this.ContextTimeout = t;
    };
    this.PreventContextTimeout = function () {
        if (this.ContextTimeout) {
            window.clearTimeout(this.ContextTimeout);
        }
    };
    this.GetContextMenu = function () {
        return $find(this.PageContextMenu);
    };
    this.ContextErrorCallback = function (args) {
        this.ShowError("Error occurred:" + args.get_message());
    };
    this.GetScrollPosition = function (e) {
        var x = 0;
        var y = 0;

        if (typeof (window.pageYOffset) == 'number') {
            x = window.pageXOffset;
            y = window.pageYOffset;
        } else if (document.documentElement && (document.documentElement.scrollLeft || document.documentElement.scrollTop)) {
            x = document.documentElement.scrollLeft;
            y = document.documentElement.scrollTop;
        } else if (document.body && (document.body.scrollLeft || document.body.scrollTop)) {
            x = document.body.scrollLeft;
            y = document.body.scrollTop;
        }

        var position = {
            'x': x,
            'y': y
        }

        return position;

    };


    this.GetMousePosition = function (e) {
        e = e ? e : window.event;
        var position = {
            'x': e.clientX,
            'y': e.clientY
        };
        return position;
    };

    this.LoadContextItems = function (item, file) {
        ContextMenus.GetObjectContextMenuItems(item, customsite, file, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };
    this.LoadTreeContextItems = function (item, file, extra) {
        ContextMenus.GetTreeContextItems(item, customsite, file, extra, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };
    this.LoadPropertyExpansionContextItems = function (item, file, value) {
        ContextMenus.GetPropertyExpansionContextItems(item, customsite, value, file, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };
    this.LoadListContextItems = function (item, file) {
        ContextMenus.GetListContextItems(item, customsite, file, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };
    this.LoadPackageContextItems = function (packid, file, parent) {
        ContextMenus.GetPackageContextItems(packid, parent, customsite, file, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };
    this.LoadCaseContextItems = function (item, file) {
        ContextMenus.GetCaseContextMenuItems(item, customsite, file, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };
    this.LoadArchiveCaseContextItems = function (item, file) {
        ContextMenus.GetHistoryCaseContextMenuItems(item, customsite, file, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };
    this.LoadArchiveOrOpenCaseContextItems = function (item, file) {
        ContextMenus.GetOpenOrArchivedCaseContextMenuItems(item, customsite, file, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };
    this.LoadMailContextItems = function (item, file, box) {
        ContextMenus.GetMailContextMenuItems(item, box, customsite, file, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };
    this.LoadFileContextItems = function (item, file) {
        ContextMenus.GetFileContextMenuItems(item, customsite, file, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };
    this.LoadObjectFileContextItems = function (item, file) {
        ContextMenus.GetObjectFileContextMenuItems(item, customsite, file, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };
    this.LoadMailFileContextItems = function (item, file) {
        ContextMenus.GetMailFileContextMenuItems(item, customsite, file, delegate(PC(), PC().ContextCompleteCallback), delegate(PC(), PC().ContextErrorCallback));
    };

    var mX = 0;
    var mY = 0;
    var bInContext = false;

    this.ShowContextMenu = function (e, row, file, LoadDelegate, subparam) {
        if (bInContext) return;

        bInContext = true;

        this.PreventContextTimeout();
        this.HideContextMenu();
        var menu = this.GetContextMenu();
        menu.hide();

        if (menu.get_items().get_count() > 0) {
            menu.get_items().clear();
        }

        if (!isDefined(subparam)) {
            LoadDelegate(row, file);
        }
        else {
            LoadDelegate(row, file, subparam);
        }

        mX = this.GetMousePosition(e).x + this.GetScrollPosition(e).x;
        mY = this.GetMousePosition(e).y + this.GetScrollPosition(e).y;

        $telerik.cancelRawEvent(e);

    };

    this.ContextCompleteCallback = function (ResponseObject) {

        var menu = this.GetContextMenu();
        var i;

        for (i = 0; i < ResponseObject.length; i++) {
            var childItem = new Telerik.Web.UI.RadMenuItem();
            childItem.set_text(ResponseObject[i].Text);
            childItem.set_value(ResponseObject[i].Value);

            if (ResponseObject[i].isSeparator == true) {
                childItem.set_isSeparator(ResponseObject[i].isSeparator);
            }

            if (ResponseObject[i].Image) {
                if (ResponseObject[i].Image.indexOf("/") == -1) {
                    childItem.set_imageUrl("./App_Themes/" + this.Theme + "/Img/" + ResponseObject[i].Image);
                }
                else {
                    childItem.set_imageUrl(ResponseObject[i].Image);
                }
            }
            if (ResponseObject[i].Parent) {
                var parentItem = menu.findItemByValue(ResponseObject[i].Parent);
                if (parentItem) {
                    parentItem.get_items().add(childItem);
                }
                else {
                    menu.get_items().add(childItem);
                }
            }
            else {
                menu.get_items().add(childItem);
            }
            if (ResponseObject[i].NavigateUrl) {
                childItem.set_navigateUrl(ResponseObject[i].NavigateUrl); // need to set this AFTER the add child, bug in telerik
            }

            if (!childItem.get_visible()) {
                childItem.show();
            }
        }

        menu.showAt(mX, mY);
        menu.repaint();
        bInContext = false;

    };


    this.HideContextMenu = function () {
        this.ContextTimeout = window.setTimeout(this.HideContextMenuFunction, 4000);
    };

    this.getY = function (oElement) {
        var iReturnValue = 0;
        while (oElement != null) {
            iReturnValue += oElement.offsetTop;
            oElement = oElement.offsetParent;
        }
        return iReturnValue;
    };
    this.getX = function (oElement) {
        var iReturnValue = 0;
        while (oElement != null) {
            iReturnValue += oElement.offsetLeft;
            oElement = oElement.offsetParent;
        }
        return iReturnValue;
    };

    this.initImagePreviews = function () {

        xOffset = 10;
        yOffset = 30;
        $("a.imagepreview").hover(function (e) {
            this.t = this.title;
            this.title = "";
            var c = (this.t !== "") ? "<br/>" + this.t : "";
            $("body").append("<p id='imagepreview'><img src='" + this.href + "' alt='Image preview' />" + c + "</p>");
            $("#imagepreview")
                .css("top", (e.pageY - xOffset) + "px")
                .css("left", (e.pageX + yOffset) + "px")
                .fadeIn("fast");
        },
            function () {
                this.title = this.t;
                $("#imagepreview").remove();
            });
        $("a.imagepreview").mousemove(function (e) {
            $("#imagepreview")
                .css("top", (e.pageY - xOffset) + "px")
                .css("left", (e.pageX + yOffset) + "px");
        }
        );
    };
    this.InitDone = function () {
        return this._InitDone;
    };
    this.onUnLoad = function () {
        ProgressIndicator.hide();
        this.ScreenModeField = null;
        this.ParentIDField = null;
        this.RootIDField = null;
        this.PackageIDField = null;
        this.ResultGrids = null;
        Popups = null;
    };

    this.onLoad = function () {
        if (this.clientID != null) {
            this.ScreenModeField = $get(this.clientID + "_txtScreenmode");
            this.ParentIDField = $get(this.clientID + "_DM_PARENT_ID");
            this.PackageIDField = $get(this.clientID + "_PACK_ID");
            this.RootIDField = $get(this.clientID + "_DM_ROOT_ID");

            this.initImagePreviews();

        }
        this._InitDone = true;
    };

    this.GetOpeningGridObject = function () {
        const gridId = PC().GetOpeningGridId();
        if (gridId) {
            if (window[gridId] || this.IsModalWindow) {               // check current window for grid
                return window[gridId];
            } else {                                                  // check main content pane for grid
                const contentWindow = this.GetMainPageWindow().GetContentWindow();
                if (contentWindow) {
                    if (contentWindow[gridId]) {
                        return contentWindow[gridId];
                    }
                    if (contentWindow.CMS) {
                        return contentWindow.CMS().GetJSVariable(gridId);
                    }
                }
            }
        }
        return null;
    };

    this.GetOpeningGridId = function (executeOnThisWindow) {
        if (executeOnThisWindow) {
            return openingGridId;
        } else {
            return this.GetMainPageWindow().PC().GetOpeningGridId(true);
        }
    };

    this.SetOpeningGridId = function (id, executeOnThisWindow) {
        if (executeOnThisWindow) {
            if (id) {
                openingGridId = id;
            }
        } else {
            this.GetMainPageWindow().PC().SetOpeningGridId(id, true);
        }
    };

    let openingGridId = "";

    this.TryReloadGrid = function () {
        const grid = this.GetOpeningGridObject();
        if (grid) {
            grid.Reload();
            return true;
        } else {
            return false;
        }
    };

    this.TryReloadMainPane = function () {
        const mainPane = this.GetContentWindow();
        if (mainPane && mainPane.PC) {
            mainPane.PC().Reload();
            return true;
        }
        return false;
    };

    this.GetContentWindow = function () {
        if (window.GetContentWindow) {
            return window.GetContentWindow();
        }
        if (window.parent.GetContentWindow) {
            return window.parent.GetContentWindow();
        }
        return window.top.GetContentWindow();
    };

    this.GetMainPageWindow = function () {
        let windowToCheck = window;
        for (var i = 0; i < 10; i++) {
            if (windowToCheck.isMainPageWindow) {
                return windowToCheck;
            }
            windowToCheck = windowToCheck.parent;
        }
        return window.top;
    };

    this.CascadeReloadContent = function () {
        const gridReloaded = this.TryReloadGrid();
        if (!gridReloaded) {
            let mainPaneReloaded;
            if (!this.IsModalWindow) {
                mainPaneReloaded = this.TryReloadMainPane();
            }
            if (!mainPaneReloaded) {
                this.Reload();
            }
        }
    };

    let goToTreeId = '';

    this.SetGoToTreeId = function (id, executeOnThisWindow) {
        if (executeOnThisWindow) {
            goToTreeId = id;
        } else {
            window.top.PC().SetGoToTreeId(id, true);
        }
    };

    this.GetGoToTreeId = function (executeOnThisWindow) {
        if (executeOnThisWindow) {
            return goToTreeId;
        } else {
            window.top.PC().GetGoToTreeId(true);
        }
    };

    this.ReloadTreeAfterModal = function (sender, args) {
        sender.remove_close(RefreshTreeAfterModal);
        this.ReloadTree(this.GetGoToTreeId());
        this.SetGoToTreeId('');
    };

    this.ReloadTree = function (id) {
        window.top.RefreshTreeContent(id);
    };


    this.ShowSuccessMessages = showsuccmsg;
    this.clientID = clientID;
    this.ResultGrids = new Array();
    this.ShowPreview = showpreview;
    this.ActionPane = 'paneActions';
    this.CustomSite = customsite;
    this.PageContextMenu = ctxmenu;
    this.currentrow = null;
    this.ContextTimeout = null;
    this.FullScreen = false;
    this._InitDone = false;
    this.IsModalWindow = ismodalwindow;
    this.ReloadFunction = reloadfunction;
    this.PageLocation = pageloc;
    this.ClientValidate = false;
    this.ValidationGroup = 'CheckMandatoryFields';
    this.TabsInDetailPreview = tabsindetailpreview;
    this.isReloading = false;
    this.AjaxManager = ajxmanager;
    this.DetailWindowsMaximized = detailwindowmaximized;

    this.DetailWindowsWidth = Math.min(1100, screen.width - 100);
    this.DetailWindowsHeight = Math.min(800, screen.height - 100);

    this.Theme = theme;
    switch (this.PageLocation) {
        case 2:
            this.ClientValidate = true;
            break;
        case 3:
        case 5:
        case 6:
        case 7:
            this.ShowPreview = false;
            break;
        case 4:
            break;
    }
    if (this.PageLocation == 0) {
        this.ShowError("Define a pagelocation in the PageController Component");
    }
    this.IsSafari = false;
    this.IsFirefox = false;

    this.IsIE = (navigator.appName.indexOf("Microsoft") > -1) && !(navigator.userAgent.indexOf("Opera") > -1);
    if (!this.IsIE) {
        this.IsFirefox = (navigator.userAgent.toLowerCase().indexOf('firefox') > -1);
        if (!this.IsFirefox) {
            this.IsSafari = (navigator.userAgent.indexOf('Safari') > -1);
        }
    }
    this.IsMobile = (navigator.userAgent.indexOf("Mobile") > -1)

    this.HideContextMenuFunction = function () {
        PC().GetContextMenu().hide();
    };
    if (this.clientID != null) {

        Sys.Application.add_load(delegate(this, this.onLoad));
        Sys.Application.add_unload(delegate(this, this.onUnLoad));

    }

}

//hacks for telerik
function RefreshAfterModal(sender, args) {
    PC().ReloadAfterModal(sender, args);
}

function RefreshTreeAfterModal(sender, args) {
    PC().ReloadTreeAfterModal(sender, args);
}

function PreventContextTimeOut() {
    PC().PreventContextTimeout();
}

function HideContextMenu() {
    PC().HideContextMenu();
}

function OnModalDetailWindowClose(sender, args) {
    try {
        var wnd = sender.get_contentFrame().contentWindow;
        if (wnd != null && wnd.OnModalWindowClose) {
            wnd.OnModalWindowClose(sender, args);
        }
    } catch (e) {
        // if window contains Cross-Origin url, reading property from wnd can throw error
        // catching it to enable closing the window
    }
}

var LoadingImage = 'Images/busy.gif';// 'Images/loading_large.gif';
var ProgressIndicator = new function () {
    var _anchorId;
    var _inProgress = false;

    // ----------------------------------------------------------------	
    function get_anchorId() { return _anchorId; }
    function set_anchorId(value) { _anchorId = value; }


    // ----------------------------------------------------------------
    this.display = function (msg) {

        if (_inProgress)
            return;

        var progress = $get('progress-area');
        var x = 0, y = 0;
        var h = 0, w = 0;


        if (!progress) {
            progress = document.createElement('div');
            progress.setAttribute('id', 'progress-area');

            progress.style.position = 'absolute';
            progress.style.border = '0px solid #000000';
            progress.style.zIndex = 9999;

            document.body.appendChild(progress);
        }


        //w = 130 + 80;
        //h = 60 + 80;
        w = 120;
        h = 60;

        var scrollX = window.pageXOffset || document.documentElement.scrollLeft || document.body.scrollLeft;
        var scrollY = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop;

        var windowHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
        var windowWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;

        x = Math.round(windowWidth / 2 - w / 2) + scrollX;
        y = Math.round(windowHeight / 2 - h / 2) + scrollY;

        progress.style.background = '#fff url(' + LoadingImage + ') no-repeat';
        progress.style.width = w + 1 + 'px';
        progress.style.height = h + 1 + 'px';
        progress.style.left = x + 'px';
        progress.style.top = y + 'px';
        progress.style.display = '';

        _inProgress = true;
    };

    this.hide = function () {
        if (!_inProgress)
            return;

        var progress = $get('progress-area');
        if (progress) progress.style.display = 'none';

        var anchorId = get_anchorId();
        if (anchorId) {
            var m = $get(anchorId);
            if (m) m.style.visibility = '';
        }

        set_anchorId(null);
        _inProgress = false;
    };
};

var tooltip = function () {
    var id = 'tt';
    var top = 3;
    var left = 3;
    var maxw = 400;
    var speed = 100; //10
    var timer = 20;
    var endalpha = 100;
    var alpha = 0;
    var tt, t, c, b, h;
    var ie = document.all ? true : false;
    return {
        show: function (v, w) {
            if (tt == null) {
                tt = document.createElement('div');
                tt.setAttribute('id', id);
                t = document.createElement('div');
                t.setAttribute('id', id + 'top');
                c = document.createElement('div');
                c.setAttribute('id', id + 'cont');
                b = document.createElement('div');
                b.setAttribute('id', id + 'bot');
                tt.appendChild(t);
                tt.appendChild(c);
                tt.appendChild(b);
                document.body.appendChild(tt);
                tt.style.opacity = 0;
                tt.style.filter = 'alpha(opacity=0)';
                document.onmousemove = this.pos;
            }
            tt.style.display = 'block';
            c.textContent = v;
            tt.style.width = w ? w + 'px' : 'auto';
            if (!w && ie) {
                t.style.display = 'none';
                b.style.display = 'none';
                tt.style.width = tt.offsetWidth;
                t.style.display = 'block';
                b.style.display = 'block';
            }
            if (tt.offsetWidth > maxw) { tt.style.width = maxw + 'px' }
            h = parseInt(tt.offsetHeight) + top;
            clearInterval(tt.timer);
            tt.timer = setInterval(function () { tooltip.fade(1) }, timer);
        },
        pos: function (e) {
            var l = PC().GetMousePosition(e).x + PC().GetScrollPosition(e).x;
            var u = PC().GetMousePosition(e).y + PC().GetScrollPosition(e).y;

            if ((u - h) > 0) {
                tt.style.top = (u - h) + 'px';
            }
            else {
                tt.style.top = '0px';
            }

            var windowWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;
            windowWidth = windowWidth + PC().GetScrollPosition(e).x;

            var diff = windowWidth - (l + tt.offsetWidth);
            if (diff < 0) { l = l - 20 + diff; }

            tt.style.left = (l + left) + 'px';
        },
        fade: function (d) {
            var a = alpha;
            if ((a != endalpha && d == 1) || (a != 0 && d == -1)) {
                var i = speed;
                if (endalpha - a < speed && d == 1) {
                    i = endalpha - a;
                } else if (alpha < speed && d == -1) {
                    i = a;
                }
                alpha = a + (i * d);
                tt.style.opacity = alpha * .01;
                tt.style.filter = 'alpha(opacity=' + alpha + ')';
            } else {
                clearInterval(tt.timer);
                if (d == -1) { tt.style.display = 'none' }
            }
        },
        hide: function () {
            clearInterval(tt.timer);
            tt.timer = setInterval(function () { tooltip.fade(-1) }, timer);
        }
    };
}();