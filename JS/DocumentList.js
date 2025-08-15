

function DoCustomDocumentListAction(seltype, actionid) { PC().DoDocumentListAction(seltype, 1, actionid); }

function Subscribe() {
    SubscribeToFolder(PC().GetParentID());
}
function SubscribeToFolder(id) {
    PC().SubscribeToFolder(id);
}


function DoDocumentListActionOnOIP(seltype, actiontype, actionid) { parent.DoActionUrl('DM_DocumentListAction.aspx?OIP=' + PC().GetOIP() + '&ACTIONTYPE=' + actiontype + '&SELTYPE=' + seltype + '&ACTIONID=' + actionid + '&SELECTION=' + PC().GetDefaultResultGrid().GetSelection()); }


function Links(id) {
    PC().GotoLink('DM_DocumentList.aspx?screenmode=links&DM_PARENT_ID=' + id);
}
function PreviousVersions(id) {
    PC().GotoLink('DM_DocumentList.aspx?screenmode=prevversions&DM_PARENT_ID=' + id);
}



function DoQueryACL(id) {
    parent.PC().OpenModalWindowRelativeSize("DM_ACL.ASPX?QUERY_ID=" + id, false);
    //PC().OpenWindow("DM_ACL.ASPX?QUERY_ID=" + id, "ACL", "height=700,width=1200,scrollbars=yes,resizable=yes");
}


//add this function so popup from file context will work
function ReloadParent() {    
    PC().ReloadAndClosePreview();
}
function ReloadContent() {
    PC().Reload();
}
function RefreshOnChange() {
    PC().Reload();
}

function BackToSearch() {
    PC().GetCurrentResultGrid().SetCurrentPage(1);
    PC().GetCurrentResultGrid().ClearOrderBy();
    return PC().ChangeScreenMode('query');
}

function CountIt() {
    PC().ShowProgress();
    PC().ChangeScreenMode('querycount');
}
function AdvSearchIt() {
    PC().ShowProgress();
    PC().GetCurrentResultGrid().SetCurrentPage(1);
    PC().ChangeScreenMode('advsearch');
}
function GlobalSearchIt() {
    PC().ShowProgress();
    PC().GetCurrentResultGrid().SetCurrentPage(1);
    if (PC().GetScreenMode() != "package") {
        PC().ChangeScreenMode('search');
    }
    else {
        PC().ChangeScreenMode('package');
    }
}
function SaveQuery() {
    PC().ChangeScreenMode('savequery');
}
function OpenQuery() {
    PC().GotoLink("Queries.aspx?DM_PARENT_ID=" + PC().GetParentID());
}
function EditQuery(id, screenid, restype) {
    PC().ChangeScreenMode('advsearch');
    PC().GotoLink("DM_DOCUMENTLIST.aspx?QRY_ID=" + id + "&screenmode=query&DM_SCREEN_ID=" + screenid + "&result_type=" + restype + "&DM_PARENT_ID=" + PC().GetParentID() + '&LOADQRY=Y');

}

function ViewFile(fileid, fromarchive, rend) {
    ShowFiles(fileid, fromarchive, rend, false,0);
}
function ShowFiles(file_id, fromarchive, rend, frommail,mailid) {
    PC().GetCurrentResultGrid().ShowFile(file_id, fromarchive, rend, frommail, mailid);
}
function ShowMailFiles(fileid, fromarchive, rend) {
    ShowFiles(fileid, fromarchive, rend, true,0);
}


function DoGlobalSearchFromMenu() {
    var t = parent.GetGlobalSearchTerm();
    if (t != '') {
        PC().GetCurrentResultGrid().SetGlobalSearch(t);   
        GlobalSearchIt();
    }
}

