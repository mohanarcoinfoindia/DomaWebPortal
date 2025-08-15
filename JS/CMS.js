function AddPanel(pageid) {
    PC().OpenModalWindow("./CMS/AddPanel.aspx?pageid=" + pageid, true, 0, 0, true, false);
}

function ConfigPanel(panelid, pageid, maximizeModal) {
  
    //const oWnd = GetRadWindowManager().open("./CMS/ConfigPanel.aspx?panelid=" + panelid + "&pageid=" + pageid + "&modal=Y", "wndModalAutosizeResizable");
    //oWnd.add_close(RefreshAfterModal);
    let width = maximizeModal ? Math.min(window.innerWidth - 100, 1400) : 0;
    let height = maximizeModal ? window.innerHeight - 40 : 0;
    PC().OpenModalWindow("./CMS/ConfigPanel.aspx?panelid=" + panelid + "&pageid=" + pageid, true, width, height, true, false);
}

function DeletePanel(panelid, pageid) {
    PC().OpenModalWindow("./CMS/DeletePanel.aspx?panelid=" + panelid + "&pageid=" + pageid, true, 0, 0, true, false);
}

function EditPage(pageid) {
   
    PC().GotoLink("./CMS/EditPage.aspx?pageid=" + pageid);
    //PC().OpenModalWindow("./CMS/EditPage.aspx?pageid=" + pageid, true, 0, 0, true, false);
}

function ShowPage(parentid, pageid, mode) {
    PC().GotoLink('Page.aspx?Pageid=' + pageid + '&DM_PARENT_ID=' + parentid + '&mode=' + mode);
}

function ShowPages(paging) {
    PC().GotoLink('Pages.aspx?paging=' + paging);
}

function DocroomListPanelController(listControlId, panelId, parentId) {

    let ctrl = eval(listControlId);

    this.HasGlobalSearch = true;

    this.LoadFolder = function (folderXml, folderId, screenId) {

        ctrl.SetCriteriaXml(folderXml);
        ctrl.SetResultScreen(screenId); //this will also reload the grid

    };

    this.LoadQuery = function (queryXml, queryId, screenId) {
        ctrl.SetCriteriaXml(queryXml);
        ctrl.SetResultScreen(screenId); //this will also reload the grid
    };

    this.LoadUrl = function (url) {
        (new PanelController(panelId, 0, parentId)).LoadUrl(url);
        CMS().RemovePanelController(panelId);
    };

    this.GlobalSearch = function (value, rootCrit) {
        if (rootCrit) {
            ctrl.SetCriteriaXml(rootCrit);
        }
        ctrl.GlobalSearch(value);
    };
}

function PanelController(panelId, pageId, parentId) {

    this.HasGlobalSearch = false;

    this.LoadFolder = function (folderXml, folderId, screenId) {
        PC().GotoLink("Page.aspx?pageid=" + pageId + "&DM_PARENT_ID=" + parentId + "&PACK_ID=" + PC().GetPackageID() + "&Action=folder&ActionID=" + folderId + "&DM_RESULT_SCREEN=" + screenId + "&TargetPanelID=" + panelId + "&activeButton=" + activeButton.value + "&activeTile=" + activeTile.value);
    };

    this.LoadQuery = function (queryXml, queryId, screenId) {
        PC().GotoLink("Page.aspx?pageid=" + pageId + "&DM_PARENT_ID=" + parentId + "&PACK_ID=" + PC().GetPackageID() + "&Action=query&ActionID=" + queryId + "&DM_RESULT_SCREEN=" + screenId + "&TargetPanelID=" + panelId + "&activeButton=" + activeButton.value + "&activeTile=" + activeTile.value);
    };

    this.LoadUrl = function (url) {
        const panelContent = $("#ctl00_Plh1_pnl_" + panelId + "_C_pnl_" + panelId + "_p");
        panelContent.html('<iframe style="padding:0;margin:0;border:none;width:100%;height:100%;" src="' + url + '" onload="cmsIframeLoaded(this)"></iframe>');
    };
}

function CMSController(pageId, parentId) {

    let panelControllers = new Array();

    this.AddPanelController = function (panelid, panelcontroller) {
        panelControllers.push({
            id: panelid,
            controller: panelcontroller
        });
    };


    this.FindControllerByID = function (id) {
        for (var i = 0; i < panelControllers.length; i++) {
            if (panelControllers[i].id === id) {
                return panelControllers[i];
            }
        }
        return null;
    };

    this.GetPanel = function (id) {
        //let drlController = FindControllerByID(panelControllers, id);
        let drlController = this.FindControllerByID(id);

        return drlController ? drlController.controller : new PanelController(id, pageId, parentId);
    };

    this.RemovePanelController = function (panelId) {
        panelControllers = panelControllers.filter(function (c) { return c.id !== panelId });
    };

    this.GetPageId = function () {
        return pageId;
    };

    this.GetParentId = function () {
        return parentId;
    };

    this.GetJSVariable = function (variableName) {
        var iframes = document.getElementsByTagName("iframe");
        for (var i = 0; i < iframes.length; i++) {
            if (canAccessIFrame(iframes[i])) {
                if (iframes[i].contentWindow[variableName]) {
                    return iframes[i].contentWindow[variableName];
                }
                if (iframes[i].contentWindow.CMS) {
                    const value = iframes[i].contentWindow.CMS().GetJSVariable(variableName);
                    if (value) {
                        return value;
                    }
                }
            }
        }
        return null
    };

}


function FindElement(id, context) {
    var el = $("#" + id, context);
    if (el.length < 1)
        el = $("[id$=_" + id + "]", context);
    return el[0];
}

function cmsIframeLoaded(iframe) {
    const $iframe = $(iframe);
    let contentHeight;
    if (iframe.contentWindow.GetTotalPageScrollHeight) { //if content is doma-mainpage
        contentHeight = iframe.contentWindow.GetTotalPageScrollHeight();
    } else {
        contentHeight = iframe.contentWindow.document.body.scrollHeight;
    }
  
    if (contentHeight > 0) {
        try {
            $iframe.css('height', (contentHeight + 10) + 'px');
            const $panel = $iframe.parent().parent();
            var minPanelHeight = contentHeight + 20;
            if (parseInt($panel.css('height')) < minPanelHeight) { // if panel-container is smaller than iframe: make it bigger to prevent scrolling
                $panel.css('height', minPanelHeight + 'px');
                const radDock = $panel.closest('.RadDock')[0];      // and if height is set on panel, make it scrollable
                if (radDock.style.height && radDock.style.height < minPanelHeight) {
                    radDock.style.overflowY = 'auto';
                } else {
                    radDock.style.overflowY = 'unset';
                }
            }
        }
        catch (e) { }
    }
}

function InitPage() {
    if (PC().InitDone()) {
        if (parent) {
            if (parent.SetFolder) {
                parent.SetFolder(PC().GetParentID(), PC().GetPackageID());
            }
        }
    }
    else {
        setTimeout('InitPage()', 100);
    }
}

function OnPanelDragStart() {
    document.getElementsByTagName('html')[0].style.overflowX = 'hidden';
}

function OnPanelDragEnd() {
    document.getElementsByTagName('html')[0].style.removeProperty('overflow-x');
}

function canAccessIFrame(iframe) {
    var html = null;
    try {
        var doc = iframe.contentDocument || iframe.contentWindow.document;
        html = doc.body.innerHTML;
    } catch (err) {
    }

    return (html !== null);
}
