function GetW(p) {
    return $get("RAD_SPLITTER_PANE_EXT_CONTENT_" + p).contentWindow;
}
function GetContentWindow() {
    return GetW(winMetaData);
}
function GetContentPane() {
    return $find(splitterContent).GetPaneById(paneMetaData);
}

function GetPreviewWindow() {
    return GetW(winPreviewContent);
}
function GetPreviewPane() {
    return $find(splitterContent).GetPaneById(panePreviewContent);
}
function GetActionWindow() {
    return GetW(winActions);
}
function GetActionPane() {
    return $find(splitterMain).GetPaneById(paneActions);
}

function LoadContent(url) {
    ShowUnLoadMessage = false;
    location.href = url;
}
function ReloadContent() {
    GetContentWindow().RefreshObject();
}

function Refresh() {
    var contentWindow = GetContentWindow();
    if (_Mode == 2) {
        contentWindow.Page_ClientValidate();
    }
    contentWindow.SaveObject();
}

function ShowFile(fileid,fromarchive,rend){ViewFile(fileid,fromarchive,rend,false);}

function ShowMailFile(fileid,fromarchive,rend){ViewFile(fileid,fromarchive,rend,true);}

function ViewFile(fileid,fromarchive,rend,frommail){
    var url = "DM_VIEW_FILE.aspx?FILE_ID=" + fileid + "&rend=" + rend + "&fromarchive=" + fromarchive;  
    if (frommail){
     url = url + "frommail=Y";}
 GetPreviewPane().set_contentUrl(url);
 ShowRightPane();
}

function CloseFile(){
 GetPreviewPane().set_contentUrl("javascript:false");
}

function CloseIFrames() {
    var pane = GetContentPane();
    if (pane != null) {
        pane.set_contentUrl("javascript:false");
    }
    CloseFile();
}

function ValidateForm(grp) {
    return GetContentWindow().ValidateForm(grp);  
}



var hddwn = false;
var mouseOverDropdown = false;
function tlbOnMouseOut(toolbar, args) {

    var item = args.get_item();
    if (item._isDropDownItem()) {
        hddwn = true;
        setTimeout(function () { HideDropDown(item) }, 200);
    }
}
function tlbOnMouseOver(toolbar, args) {

    var item = args.get_item();
    if (item._isDropDownItem()) {
        hddwn = false;
        item.showDropDown();
    } else if (item._isDropDownChild()) {
        if (mouseOverDropdown === false) {
            const dropdown = item.get_element().parentElement.parentElement;
            dropdown.addEventListener('mouseleave', function (e) {
                hddwn = true;
                setTimeout(function () {
                    HideDropDown(item.get_parent());
                }, 200)
            });
            mouseOverDropdown = true;
        }
        hddwn = false;
    }
}

function HideDropDown(dropDown) {
    if (hddwn == true) {
        dropDown.hideDropDown();
        hddwn = false;
    }
}

var alarmWindow = null;
function DisableWindow() {
    if (!alarmWindow) {

        alarmWindow = document.body.appendChild(document.createElement("div"));
        alarmWindow.id = "disableWindow";
        alarmWindow.style.height = document.documentElement.scrollHeight + "px";
        alarmWindow.setAttribute("unselectable", "on");
    }
}
function EnableWindow() {
    if (alarmWindow) {
        document.body.removeChild(alarmWindow);
    }
    alarmWindow = null;
}

function ShowProgress() {    
    DisableWindow();
    DisableButtons();
    //GetContentWindow().PC().HideProgress();
}
function HideProgress() {
    EnableWindow();
    EnableButtons();
    GetContentWindow().PC().HideProgress();
}

function CalculateMenu() {
    //dummy placeholder
}

function SetFolder(id, name) {
    //dummy
}
function ToggleCloseCase(itm) {
    DocroomListFunctions.SetUserProfileParameter("OPENNEXTCASEONCLOSE", itm.checked ? "1" : "0");
}

function GetToolbarButton(tlbClientID, tlbbuttonValue) {
    return $find(tlbClientID).findItemByValue(tlbbuttonValue);
}
function Togglebuttons(tlbid, on) {
    var toolBar = $find(tlbid);
    var toolbarItems = toolBar.get_allItems();
    var i = 0;
    while (i < toolbarItems.length) {
        toolbarItems[i].set_enabled(on);
        i++;
    }
} 

function RefreshParentPage(gototreeid, refreshTree, reloadParent) {
    try {
        if (refreshTree) {
            MainPage().PC().ReloadTree(gototreeid);
        }
        if (reloadParent) {
            MainPage().PC().CascadeReloadContent(); // todo: pass gototreeid ?
        }
    }
    catch (e) {
        //ignore errors
    }
}


function ResizePaneBySplit(id, nrOfPanes) {
    var pane = $find(splitterContent).GetPaneById(id);
    if (!pane) return;
    if (!pane.get_collapsed()) {

        var delta = 0;
        var newSize = ($find(splitterContent).get_width() / nrOfPanes) + 30;
        delta = parseInt(newSize - pane.get_width());
        pane.resize(delta, 1);
    }
    pane = null;
}

function ResizePane(id, width) {
    var pane = $find(splitterContent).GetPaneById(id);
    if (!pane) return;
    if (!pane.get_collapsed()) {
        var delta = 0;
        var newSize = width;
        delta = parseInt(newSize - pane.get_width());
        pane.resize(delta);
    }
    pane = null;
}


function HTP() {
    //for old code
}
function HideTreePane() {
    //for old code
}

function ChangeFolder(id) {
    var contentWindow = GetContentWindow();
    contentWindow.SetFolderSelection(id);
    contentWindow.SaveObject();
}

function ShowList(id, dummy) {
    ShowDetail("DM_List_Preview.aspx?screenmode=browse&DM_PARENT_ID=" + id + "&tabnode=Sub");
    SetHeader("Browse");
}

function HasDetailPane() {
    return true;
}

function ShowRightPane() {
    var oContentPaneRight = GetPreviewPane();
    if (!oContentPaneRight) return;
    if (oContentPaneRight.get_collapsed()) {
        oContentPaneRight.expand();      
    }
}

function PopoutHelp() {  
    let w = window.open('', 'Help', 'width=500,height=400');
    w.document.open();
    w.document.write($('#helptext').html());
    w.document.title = 'Help';
    w.focus();
}

function GetTotalPageScrollHeight() {
    let navigationPaneHeight = getNavigationPaneHeight();
    let detailviewHeight = GetContentWindow().document.body.scrollHeight;
    return navigationPaneHeight + detailviewHeight;
}

