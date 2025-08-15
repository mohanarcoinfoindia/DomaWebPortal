function AddObject(cattype) {
    if (GetRadPane(listPaneID).PC) {
        GetRadPane(listPaneID).PC().AddObject(cattype, PC().GetParentID(), 0, PC().GetPackageID());
    }
    else {
        PC().AddObject(cattype, PC().GetParentID(), 0, PC().GetPackageID());
    }
}
function NewObject(catid, parentid, packid) {
    if (GetRadPane(listPaneID).PC) {
        GetRadPane(listPaneID).PC().AddObject('', parentid, catid, packid);
    } else {
        PC().AddObject('', parentid, catid, packid);
    }
}
function NewCase(procid, parentid, packid) {
    if (GetRadPane(listPaneID).PC) {
        GetRadPane(listPaneID).PC().AddCase(parentid, procid, packid);
    } else {
        PC().AddCase(parentid, procid, packid);
    }
}

//called from the folderlink
function ChangeContentFolder(id,packid) {
    if (listPaneID != null && GetRadPane(listPaneID).PC) {
        GetRadPane(listPaneID).PC().OpenFolder(id,packid);
    }
}

//function OnGlobalKeyPress(sender, eventArgs) {
   
//    var c = eventArgs.get_keyCode();
//    if (c == 13) {
//        GlobalSearch();
//        eventArgs.set_cancel(true);
//    }
//}
function GlobalSearch(sender, args) {
    if (GetGlobalSearchTerm() != '') {        
        setTimeout("ExecGlobalSearch()", 100);
    }
}

function ExecGlobalSearch() {
    if (GetRadPane(listPaneID).DoGlobalSearchFromMenu) {        
        GetRadPane(listPaneID).DoGlobalSearchFromMenu();
    }
    else {    
        ViewList('', GetGlobalSearchTerm());      
    }
}

function trim(v) {
    v = v.replace(/^\s+/, '');
    v = v.replace(/\s+$/, '');
    return v;
}
function ClearGlobalSearch() {
    if (searchBox != undefined && searchBox != '') {
        var b = $find(searchBox);
        if (b) {
            b.clear();
        }
    }  
}
function GetGlobalSearchTerm() {
    if (searchBox != undefined && searchBox != '') {      
        return trim($find(searchBox).get_inputElement().value);
    }
    else {
        return "";
    }
}

function Goto(link) {   
    try
    {   
        if (link.indexOf('DM_PARENT_ID=') <= 0) {
            if (link.indexOf('?') > 0)
                link = link + '&DM_PARENT_ID=' + PC().GetParentID();
            else
                link = link + '?DM_PARENT_ID=' + PC().GetParentID();
        }
        if (link.indexOf('PACK_ID=') <= 0) {
            if (link.indexOf('?') > 0)
                link = link + '&PACK_ID=' + PC().GetPackageID();
            else
                link = link + '?PACK_ID=' + PC().GetPackageID();
        }
    } 
    catch(e)
    {

    }
    OpenUrl(link);
}
function ShowMainContent(link) {
    if (listPaneID != null) {
     
        link = PC().AddSiteToUrl(link);
        var pane = $find(listPaneID);
        if (!pane) return;

        pane.set_contentUrl(link);
        CloseDetail(false);
    }
    
}

function CloseDetail(force) {
    if (listPaneID != null) {
        var pane = $find(listPaneID);
        if (!pane) return;
        if (pane.get_collapsed()) {
            pane.expand();                                          
        }
        if (HasDetailPane()) {
            var pane2 = $find(detailPaneID);
            if (!pane2) return;
            pane2.set_contentUrl('about:blank');
            pane2.collapse();
        }
    }
}
function HideProgress() {
    var p = GetRadPane(listPaneID);
    if (p != null) {
        if (p.HideProgress) {
            p.HideProgress();
        }
        else {
            p.PC().HideProgress();
        }
    }
}
function ShowProgress() {
    var p = GetRadPane(listPaneID);
    if (p != null) {
        if (p.ShowProgress) {
            p.ShowProgress();
        }
        else {
            p.PC().ShowProgress();
        }
    }
}

function HasDetailPane() {
    return (detailPaneID != null);
}
function ShowDetail(link, FullScreen) {
    if (HasDetailPane()) {
        var pane = $find(detailPaneID);
        if (!pane) return;
        pane.set_contentUrl(link);
        if (pane.get_collapsed()) {
            pane.expand();           
        }
        if (FullScreen) {
            var pane2 = $find(listPaneID);
            if (!pane2) return;
            pane2.collapse();
        }
    }
    else 
    {
        window.open(link, 'MySubDetailWindow', 'height=600,width=800,scrollbars=yes,resizable=yes,toolbar=yes,status=yes');
    }
}
function GetActionPane() {
    return $find(actionPaneID);
}
function DoActionUrl(link) {
    if (actionPaneID != null) {
        var pane = GetActionPane();
        if (!pane)
            return;
        pane.set_contentUrl(link);
    }
}

function RefreshTreeContent(gotoid) {
    if (treePaneID != null) {
        var pane = GetRadPane(treePaneID);
        if (!pane) return;
        pane.Refresh(gotoid);
    }
}
function SetHeader(s) {
//used from the preview pane
}
function GetPreviewWindow() {
    return GetRadPane(detailPaneID);
}

function GetActionWindow() {
    return GetRadPane(actionPaneID);
}
function GetRadPane(paneID) {
    var pane = $find(paneID);
    if (!pane) return;
    var iframe = pane.getExtContentElement(); //.getElementById("RAD_SPLITTER_PANE_EXT_CONTENT_" + paneID);
    if (iframe != null) {      
        return iframe.contentWindow;
    }
    else {      
        return null;
    }
}
function GetContentWindow() {
    return GetRadPane(listPaneID);
}

function SelectCaseFromGrid(CaseID, CaseName) {   
    if (TargetPackID != '') {
        if (TargetCaseID != '') { //add to case package           
            GetRadPane(listPaneID).PC().LinkCaseToRoutingCase(TargetCaseID, TargetPackID, CaseID);
        }
        else if (TargetObjID != '') { //add to object package          
            GetRadPane(listPaneID).PC().LinkCaseToObjectPackage(TargetObjID, TargetPackID, CaseID);
        }
        else {
            eval("MainPage().GetContentWindow().AddValue" + TargetPackID + "_0(" + CaseID + ",'" + CaseName + "');");
            Close();
        }
    }

    else {
        PC().ShowError('Not in selection mode');
    }
}
function SelectArchivedCaseFromGrid(CaseID, CaseName) {
    if (TargetPackID != '') {
        if (TargetCaseID != '') { //add to case package           
            GetRadPane(listPaneID).PC().LinkArchivedCaseToRoutingCase(TargetCaseID, TargetPackID, CaseID);
        }
        else if (TargetObjID != '') { //add to object package          
            GetRadPane(listPaneID).PC().LinkArchivedCaseToObjectPackage(TargetObjID, TargetPackID, CaseID);
        }
        else {
            eval("MainPage().GetContentWindow().AddValue" + TargetPackID + "_0(" + CaseID + ",'" + CaseName + "');");
            Close();
        }
    }

    else {
        PC().ShowError('Not in selection mode');
    }
}
function SelectObjectFromGrid(DocID,DocDin,DocName) {
    if (TargetDossierType != '') {
        if (TargetPackID != '') {
            GetRadPane(listPaneID).PC().LinkDocToObjectPackage(DocID, TargetPackID, TargetObjID);
        }
        else {
            GetRadPane(listPaneID).PC().LinkDocToObjectPackage(DocID, 0, TargetObjID);
        }
    }
    else if (TargetPackID != '') {
        if (TargetCaseID != '') { //add to case package
            GetRadPane(listPaneID).PC().LinkDocToRoutingCase(TargetCaseID, TargetPackID, DocID);
        }
        else if (TargetObjID != '') { //add to object package
            GetRadPane(listPaneID).PC().LinkDocToObjectPackage(TargetObjID, TargetPackID, DocID);
        }
        else { //lookup mode
            eval("MainPage().GetContentWindow().AddValue" + TargetPackID + "_0(" + DocDin + ",'" + DocName + "');");
            Close();
        }
    }
    
    else {
        PC().ShowError('Not in selection mode');
    }
}

function GetRadWindow() {
    var oWindow = null;
    if (window.radWindow) oWindow = window.radWindow;
    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
    return oWindow;
}
function Close() {
    GetRadWindow().Close();
}
function MainPage() {
    return GetRadWindow().BrowserWindow;
}


function isInSelectionMode() {
    if (TargetPackID != '' || TargetDossierType != '')
        return true
    else
        return false;
}
function OpenUrl(url) {
    ClearGlobalSearch();
    if (isInSelectionMode()) {
        if (url.indexOf("selectionmode=") < 0) {
            if (url.indexOf('?') > 0) {
                url = url + '&selectionmode=Y';
            }
        }
        if (TargetPackID != '') {
            if (url.indexOf("selectionpack=") < 0) {
                if (url.indexOf('?') > 0) {
                    url = url + '&selectionpack=' + TargetPackID;
                }
            }
        } 
    }
    ShowMainContent(url);
}


       

function ToggleWhatsNew() { GetRadPane(listPaneID).PC().GetCurrentResultGrid().ToggleWhatsNew(); }
             
function ClearBasket(){ClearSelection(2);}
function ClearFavorites(){ClearSelection(3);}      
function ClearCDRom(){ClearSelection(6);}
function AddToSelection(type) {
    GetRadPane(listPaneID).PC().GetCurrentResultGrid().AddToSelection(type); 
}
function RemoveFromSelection(type) { GetRadPane(listPaneID).PC().GetCurrentResultGrid().RemoveFromSelection(type); }
            
function ClearSelection(type) { GetRadPane(listPaneID).PC().ClearSelection(type); }
function ResetSelection() { GetRadPane(listPaneID).PC().GetCurrentResultGrid().ResetSelection() };

function ReloadContent() {
    var p = GetRadPane(listPaneID);
    if (p != null) {
        if (p.ReloadParent) {
            p.ReloadParent();
        }
        else {
            p.ReloadContent();
        }
    }
    else {
        PC().Reload();
    }
}
function GotoCurrent() {
    ReloadContent();
}
function DoCustomDocumentListAction(id){GetRadPane(listPaneID).DoCustomDocumentListAction(1,id);}
function DoDocumentListAction(id){GetRadPane(listPaneID).PC().DoDocumentListAction(1,id,0);} 
function LinkObjects(id) {GetRadPane(listPaneID).DoDocumentListActionOnOIP(1,3,id);} 
                 
            
function CopyMoveSelection()
{
    AddToSelection(1);
    var w = window.open('DM_CopyMoveToFolder.ASPX','CopyMoveToFolder','width=600,height=200,resizable=yes,scrollbars=yes,status=yes');
    w.focus();
    return;
}
function GetOIP()
{
    return GetRadPane(listPaneID).GetOIP();
}

            
function SelectAll(){GetRadPane(listPaneID).ToggleMultiSelect();}
                     

function PrintListPage()
{
    try
    {
    GetRadPane(listPaneID).PrintPage();
    }
    catch(e)
    {
    UIMessages[6];
    }
}
            

function ShowFolder(id)
{
    SetTreeContent(PC().AddSiteToUrl("Tree.aspx?OID=" + id));
}

           
function SetTreeContent(link) {
    if (treePaneID != null) {
        var pane = $find(treePaneID);
        if (!pane) return;
        pane.set_contentUrl(link);
    }
}           	             	  	     
function GetContentScreenMode()
{
    var s;
try
{
	s = GetRadPane(listPaneID).PC().GetScreenMode();
}
catch(e)
{ 
	s = "";
}
return s;
}

function ShowRecyclebin() {
    OpenUrl('DM_DocumentList.aspx?screenmode=recyclebin');
}
function AppServer() {
    OpenUrl('DM_Appserver.aspx');
}
function SysLog() {
    OpenUrl('Syslog.aspx');
}
function DrumLog() {
    OpenUrl('DrumLoggingV2.aspx');
}
function LoggedInUsers() {
    OpenUrl('./UserBrowser/LoggedIn.aspx');
}
function BatchJobs() {
    OpenUrl('./DM_Jobs.aspx');
}
function FileServers() {
    OpenUrl('DM_FileServers.aspx');
}
function FileTypes() {
    OpenUrl('DM_FileTypes.aspx');
}

function ManageLists() {
    OpenUrl('./CatalogMaint/DM_Catalog.aspx');
}

function AboutDocroom() {
    OpenUrl('DM_ABOUT.aspx');
}
function ActionProfiles() {
    OpenUrl('dm_action_profiles.aspx');
}
function ManageUsers() {
    OpenUrl('./UserBrowser/DM_Userbrowser.aspx');
}
function IFilters() {
    OpenUrl('DM_IFilters.aspx');
}
function ShowBasket() {
    OpenUrl('DM_DocumentList.aspx?screenmode=browse&seltype=2');
}
function ShowCDRom() {
    OpenUrl('DM_DocumentList.aspx?screenmode=browse&seltype=6');
}
function ShowFavorites() {
    OpenUrl('DM_DocumentList.aspx?screenmode=browse&seltype=3');
}
function ShowWhatsNew() {
    OpenUrl('DM_DocumentList.aspx?screenmode=whatsmodified');
}
function ShowMyCheckedOutDocs() {
    OpenUrl('DM_DocumentList.aspx?screenmode=mycheckedoutdocs');
}
function ShowRecent() {
    OpenUrl('DM_DocumentList.aspx?screenmode=browse&seltype=4');
}
function ShowMails() {
    OpenUrl('DM_DocumentList.aspx?screenmode=browse&result_type=5');
}
function ShowMyInbox() {
    OpenUrl('DM_DocumentList.aspx?screenmode=mymail&mail_type=1');
}
function ShowMyMails() {
    OpenUrl('DM_DocumentList.aspx?screenmode=mymail&mail_type=0');
}
function ShowMySentItems() {
    OpenUrl('DM_DocumentList.aspx?screenmode=mymail&mail_type=2');
}
function ShowMyWork() {
    MyWork();
}
function ShowGlobalSearch() {
    OpenUrl('DM_DocumentList.aspx?screenmode=query');
}
function SearchInCurrent() {
    OpenUrl('DM_DocumentList.aspx?screenmode=query&result_type=' + restype);
}
function SearchIn(type) {
    OpenUrl('DM_DocumentList.aspx?screenmode=query&result_type=' + type);
}
function MyMailWork() {
    restype = 10;
    OpenUrl('DM_DocumentList.aspx?screenmode=mymailwork&result_type=' + restype);
}
function MyWork() {
    restype = 2;
    ViewMode = "mywork";
    Goto('DM_DocumentList.aspx?screenmode=mywork&result_type=' + restype);
}
function OpenCases() {
    restype = 3;
    ViewMode = "opencases";
    Goto('DM_DocumentList.aspx?screenmode=opencases&result_type=' + restype);
}
function MyCases() {
    restype = 6;
    ViewMode = "mycases";
    Goto('DM_DocumentList.aspx?screenmode=mycases&result_type=' + restype);
}
function ArchivedCases() {
    restype = 4;
    ViewMode = "archivedcases";
    Goto('DM_DocumentList.aspx?screenmode=archivedcases&result_type=' + restype);
}
function FollowUp() {
    restype = 12;
    OpenUrl('DM_DocumentList.aspx?screenmode=mymailfollowup&result_type=' + restype);
}
function Inbox() {
    restype = 8;
    OpenUrl('DM_DocumentList.aspx?screenmode=mymailinbox&result_type=' + restype);
}
function Outbox() {
    restype = 9;
    OpenUrl('DM_DocumentList.aspx?screenmode=mymailoutbox&result_type=' + restype);
}
function MyMails() {
    restype = 7;
    OpenUrl('DM_DocumentList.aspx?screenmode=mymail&result_type=' + restype);
}
function MyDeletedBox() {
    restype = 11;
    OpenUrl('DM_DocumentList.aspx?screenmode=mymaildeletedbox');
}
function Stats() {
    OpenUrl('Stats.aspx');
}

function OpenMyDocsCallBack(ResponseObject, ResponseAsXml, ResponseAsText) {    
    OpenUrl('DM_DocumentList.aspx?screenmode=browse&DM_PARENT_ID=' + ResponseObject);
}
function ShowPage(pageid) {
    OpenUrl('Page.aspx?pageid=' + pageid);
}
function OpenInline() {
    try{
        var p = PC().GetParentID();      
        var url = 'dm_detail.aspx?DM_OBJECT_ID=' + PC().GetParentID() + '&mode=1&inline=true';   
        ShowMainContent(url);   
    }
    catch(e)
    {
    setTimeout("OpenInline()", 100);
    }
}
function EditInline() {
    try {
        var p = PC().GetParentID();
        var url = 'dm_detail.aspx?DM_OBJECT_ID=' + PC().GetParentID() + '&mode=2&inline=true';
        ShowMainContent(url);
    }
    catch (e) {
        setTimeout("EditInline()", 100);
    }
}

var lastScreenMode = "";
function ViewList(objtype, globalsearch) {
   
    var m = GetContentScreenMode();
  
    if (m && (m == "browse" || m == "browsewithsubfolders")) { lastScreenMode = m; }

    switch (objtype) {
        case "mywork":
            MyWork();
            break;
        case "opencases":
            OpenCases();
            break;
        case "mycases":
            MyCases();
            break;
        case "archivedcases":
            ArchivedCases();
            break;
        default:
            var u;
            restype = 0;
            ViewMode = objtype;
            if (!globalsearch) {
                m = lastScreenMode;
                if (!m) { m = "browsewithsubfolders" };                            
                u = 'DM_DocumentList.aspx?screenmode=' + m;
            }
            else {
                u = 'DM_DocumentList.aspx?screenmode=search&globalsearch=' + globalsearch;
            }
            if (objtype != "") {
                u = u + '&object_type=' + objtype;
            }          
            Goto(u);
            break;
    }
   
}

function SavedQueries() {
    ViewMode = '';
    Goto('Queries.aspx');
}
function SearchList(objtype) {
    var url = 'DM_DocumentList.aspx?screenmode=query';
    if (objtype != "") {
        url = url + '&object_type=' + objtype;
    }
    Goto(url);
}

function SelectFirstMenuItem() {
    var prm = Sys.WebForms.PageRequestManager.getInstance();
    if (prm.get_isInAsyncPostBack()) return;

    var menu = $find(MenuID);
   
    var menuItem = menu.get_selectedItem();
    if (menuItem) {
        var sNavig = new String(menuItem.get_navigateUrl());
      
        if (sNavig.toLowerCase().indexOf("javascript:") == 0) {
            sNavig = sNavig.replace(/javascript:/gi, "");
            eval(sNavig);
        }
        else {
            window.location.href = sNavig;
        }
    }
    else {
        GotoDefault();
    }
}

let modalInitBounds;
let resizeTimeout = false;

function SetModalInitBounds(oWnd, eventArgs) {
    modalInitBounds = oWnd.getWindowBounds();
}

$(function () {
    const radWindows = GetRadWindowManager().get_windows();
    radWindows.forEach(function (wnd) {
        wnd.add_show(SetModalInitBounds);
    });
});


window.addEventListener('resize', function () {

    let radWindow = GetRadWindowManager().getActiveWindow();

    if (radWindow) {

        let offsetLeft, offsetTop;

        clearTimeout(resizeTimeout); // to debounce the resize events (callback executed only when resizing stops for 250ms)

        resizeTimeout = setTimeout(function () {

            const widthDiff = window.innerWidth - modalInitBounds.width;
            const heightDiff = window.innerHeight - modalInitBounds.height;

            if (window.innerWidth > modalInitBounds.width + modalInitBounds.x * 2) {   // window is wider when modal opened
                offsetLeft = modalInitBounds.x;
            } else if (widthDiff > 0) {   // original modal size fits window
                offsetLeft = widthDiff / 2;
            } else if (widthDiff <= 0) {  // window is smaller then original modal size
                offsetLeft = 0;
            }


            // same for height
            if (window.innerHeight > modalInitBounds.height + modalInitBounds.y * 2) {
                offsetTop = modalInitBounds.y;
            } else if (heightDiff > 0) {
                offsetTop = heightDiff / 2;
            } else if (heightDiff <= 0) {
                offsetTop = 0;
            }

            radWindow.moveTo(offsetLeft, offsetTop);
            radWindow.setSize((window.innerWidth - offsetLeft * 2), (window.innerHeight - offsetTop * 2));


        }, 250);
    }
});

function GetTotalPageScrollHeight() {
    let height = GetRadPane(listPaneID).document.body.scrollHeight;
    const menu = document.getElementById(menuPaneID);
    if (menu) {
        height += menu.scrollHeight;
    }
    return height;
}

const isMainPageWindow = true;