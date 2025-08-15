

//modal dialog	    
function pageWidth() { return window.innerWidth != null ? window.innerWidth : document.documentElement && document.documentElement.clientWidth ? document.documentElement.clientWidth : document.body != null ? document.body.clientWidth : null; }
function pageHeight() { return window.innerHeight != null ? window.innerHeight : document.documentElement && document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body != null ? document.body.clientHeight : null; }
function posLeft() { return typeof window.pageXOffset != 'undefined' ? window.pageXOffset : document.documentElement && document.documentElement.scrollLeft ? document.documentElement.scrollLeft : document.body.scrollLeft ? document.body.scrollLeft : 0; }
function posTop() { return typeof window.pageYOffset != 'undefined' ? window.pageYOffset : document.documentElement && document.documentElement.scrollTop ? document.documentElement.scrollTop : document.body.scrollTop ? document.body.scrollTop : 0; }
function $(x) { return $get(x); }
function scrollFix() { var obol = $('obol'); if (obol && obol.style) { obol.style.top = posTop() + 'px'; obol.style.left = posLeft() + 'px'; } }
function sizeFix() { var obol = $('obol'); if (obol && obol.style) { obol.style.height = pageHeight() + 'px'; obol.style.width = pageWidth() + 'px'; }}
function kp(e) { ky = e ? e.which : event.keyCode; if (ky == 88 || ky == 120) hm(); return false }
function inf(h) { tag = document.getElementsByTagName('select'); for (i = tag.length - 1; i >= 0; i--) tag[i].style.visibility = h; tag = document.getElementsByTagName('iframe'); for (i = tag.length - 1; i >= 0; i--) tag[i].style.visibility = h; tag = document.getElementsByTagName('object'); for (i = tag.length - 1; i >= 0; i--) tag[i].style.visibility = h; }
function sm(obl, wd, ht) { var h = 'hidden'; var b = 'block'; var p = 'px'; var obol = $('obol'); var obbxd = $('mbd'); obbxd.innerHTML = $(obl).innerHTML; obol.style.height = pageHeight() + p; obol.style.width = pageWidth() + p; obol.style.top = posTop() + p; obol.style.left = posLeft() + p; obol.style.display = b; var tp = posTop() + ((pageHeight() - ht)) - 12; var lt = posLeft() + ((pageWidth() - wd) / 2) - 12; var obbx = $('mbox'); obbx.style.top = (tp < 0 ? 0 : tp) + p; obbx.style.left = (lt < 0 ? 0 : lt) + p; obbx.style.width = wd + p; obbx.style.height = ht + p; inf(h); obbx.style.display = b; return false; }
function hm() { var v = 'visible'; var n = 'none'; $('obol').style.display = n; $('mbox').style.display = n; inf(v); document.onkeypress = '' }
function initmb() { var ab = 'absolute'; var n = 'none'; var obody = document.getElementsByTagName('body')[0]; var frag = document.createDocumentFragment(); var obol = document.createElement('div'); obol.setAttribute('id', 'obol'); obol.style.display = n; obol.style.position = ab; obol.style.top = 0; obol.style.left = 0; obol.style.zIndex = 998; obol.style.width = '100%'; frag.appendChild(obol); var obbx = document.createElement('div'); obbx.setAttribute('id', 'mbox'); obbx.style.display = n; obbx.style.position = ab; obbx.style.zIndex = 999; var obl = document.createElement('span'); obbx.appendChild(obl); var obbxd = document.createElement('div'); obbxd.setAttribute('id', 'mbd'); obl.appendChild(obbxd); frag.insertBefore(obbx, obol.nextSibling); obody.insertBefore(frag, obody.firstChild); window.onscroll = scrollFix; window.onresize = sizeFix; }
//end modal dialog  

//required for client side menu validation

function OpenUrl(url) {
    parent.OpenUrl(url);
}
function OpenUrlInRoot(url) {
    //   parent.SetFolder(0,'');
    parent.ClearGlobalSearch();
    OpenUrl(url);
}
function OpenUrlInFolder(id, url) {
    //  parent.SetFolder(id,name);
    parent.ClearGlobalSearch();
    OpenUrl(url);
}
function ShowMyInCreationDocs() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=myincreation');
}
function ShowBasket() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=browse&seltype=2');
}
function ShowCDRom() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=browse&seltype=6');
}
function ShowFavorites() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=browse&seltype=3');
}
function ShowWhatsNew() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=whatsnew');
}
function ShowWhatsModified() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=whatsmodified');
}
function ShowMyCheckedOutDocs() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=mycheckedoutdocs');
}
function ShowRecyclebin() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=recyclebin');
}

function ShowRecent() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=browse&seltype=4');
}
function ShowMyInbox() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=mymailinbox');
    //OpenUrlInRoot('MailClient/Maillist.aspx?screenmode=mymail&mail_type=1');
}
function ShowMyDeletedItems() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=mymaildeletedbox');
    //OpenUrlInRoot('MailClient/Maillist.aspx?screenmode=mymail&mail_type=0&mail_status=9');
}
function ShowMyMails() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=mymail');
    //OpenUrlInRoot('MailClient/Maillist.aspx?screenmode=mymail&mail_type=0');
}
function ShowMySubScriptions() {
    OpenUrlInRoot('MySubscriptions.aspx');
}
function ShowMySentItems() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=mymailoutbox');
    //OpenUrlInRoot('MailClient/Maillist.aspx?screenmode=mymail&mail_type=2');
}
function ShowSavedQueries() {
    OpenUrlInRoot('Queries.aspx');
}
function MyWork() {
    OpenUrlInRoot('DM_DocumentList.aspx?screenmode=mywork');
}

function DML(CommandName) {
    var id = GetSelectedId();
    id = id.substring(1, id.length);
    var sParentQRYString = "DM_CAT_ID=" + id;
    switch (CommandName) {       
        case "Browse":           
           OpenUrlInFolder(id, 'DM_DocumentList.aspx?' + sParentQRYString + '&screenmode=browse');
           break;       
       case "Search":
           OpenUrlInFolder(id, 'DM_DocumentList.aspx?' + sParentQRYString + '&screenmode=query');
           break;
       case "Add":       
           if (parent.NewObject) {
               parent.NewObject(id, 0, 0);
           }
           else {
               PC().AddObject('ListItem', 0, id, 0);
           }
           break;     
    }
}
function DMC(CommandName,itemName) {
    var nodeid = GetSelectedId();   
    var id = nodeid.substring(1, nodeid.length);
    var aSplit = id.split("_");
    id = aSplit[0];
    var packid = 0;
    if (aSplit.length > 1) {
        packid = aSplit[1];
    }

    var sParentQRYString = "DM_PARENT_ID=" + id + "&PACK_ID=" + packid;
    switch (CommandName) {
        case "NoAction":
            break;        
        case "Open":
            PC().OpenDetailWindow('dm_detail.aspx?DM_OBJECT_ID=' + id + '&mode=1', id);            
            break;
        case "OpenInline":

            OpenUrlInFolder(id, 'dm_detail.aspx?DM_OBJECT_ID=' + id + '&mode=1&inline=true');
            break;
        case "EditInline":
            OpenUrlInFolder(id, 'dm_detail.aspx?DM_OBJECT_ID=' + id + '&mode=2&inline=true');
            break;        
        case "Info":
            PC().OpenWindow('DM_OBJECT_TOOLTIP.aspx?DM_OBJECT_ID=' + id, 'ExtraInfo', 'width=400,height=300,scrollbars=yes,resizable=yes');
            break;
        case "Page":
            OpenUrlInFolder(id, 'Page.aspx?' + sParentQRYString);
            break;        
        case "Browse":
            switch (parent.ViewMode) {
                case "mywork":
                case "opencases":
                case "mycases":
                case "archive":
                    OpenUrlInFolder(id, 'DM_DocumentList.aspx?' + sParentQRYString + '&screenmode=' + parent.ViewMode + '&result_type=' + parent.restype);
                    break;
                default:
                    OpenUrlInFolder(id, 'DM_DocumentList.aspx?' + sParentQRYString + '&screenmode=browse&object_type=' + parent.ViewMode);
                    break;
            }

            break;
        case "BrowseWithSubFolders":
            switch (parent.ViewMode) {
                case "mywork":
                case "opencases":
                case "mycases":
                case "archive":
                    OpenUrlInFolder(id, 'DM_DocumentList.aspx?' + sParentQRYString + '&screenmode=' + parent.ViewMode + '&result_type=' + parent.restype);
                    break;
                default:
                    OpenUrlInFolder(id, 'DM_DocumentList.aspx?' + sParentQRYString + '&screenmode=browsewithsubfolders&object_type=' + parent.ViewMode);
                    break;
            }
            break;
        case "FolderText":
            OpenUrlInFolder(id, 'DM_DocumentList.aspx?' + sParentQRYString + '&screenmode=foldertext');
            break;        
        case "Search":
            OpenUrlInFolder(id, 'DM_DocumentList.aspx?' + sParentQRYString + '&screenmode=query');
            break;
        case "EditQuery":
            PC().OpenWindow('DM_EDIT_QUERY.aspx?DM_OBJECT_ID=' + id, 'EditQuery', 'width=800,height=450,resizable=yes,scrollbars=yes,status=yes');
            break;
        case "AddQuery":
            PC().OpenWindow('DM_EDIT_QUERY.aspx?DM_PARENT_ID=' + id, 'EditQuery', 'width=800,height=450,resizable=yes,scrollbars=yes,status=yes');
            break;
        case "Explore":        
            PC().OpenWindow('DM_EXPLORE.aspx?DM_PARENT_ID=' + id, 'MainWindow' + id, 'height=800,width=1200,scrollbars=yes,resizable=yes,status=yes');
            break;
        case "Add":        
            if (parent.NewObject) {
                parent.NewObject(0, id, packid);
            }
            else {
                PC().AddObject('', id, 0, packid);
            }
            break;
        case "Edit":        
            PC().OpenDetailWindow('dm_detail.aspx?DM_OBJECT_ID=' + id + '&mode=2', id);
            break;
        case "Acl":        
            PC().ShowObjectACL(id);
            break;
        case "Cut":        
            SetSubAction("");          
            $get("txtCutID").value = nodeid;
            break;
        case "Copy":        
            SetSubAction("FROMCOPY");
            $get("txtCutID").value = nodeid;
            break;
        case "Delete":        
            var confirmMsg = UIMessages[4];
            if (isDefined(itemName)) {
                confirmMsg = confirmMsg + " (" + itemName + ")";
            }
            if (window.confirm(confirmMsg)) {
                SetSubActionAndSubmit("D");                
            }
            break;
        case "Paste":        
            if ($get("subaction").value == 'FROMCOPY') {
                SetSubActionAndSubmit("PC");                
            }
            else {
                if (confirm(UIMessages[16])) {
                    SetSubActionAndSubmit("P");                   
                }
            }            
            break;
        case "PasteQuery":
            SetSubActionAndSubmit("PQ");            
            break;
        case "PasteIntoPackage":
            SetSubActionAndSubmit("PIP");            
            break;
        case "PasteBasket":
            SetSubActionAndSubmit("PBSK");            
            break;
        case "PasteFavorites":
            SetSubActionAndSubmit("PFAV");            
            break;
        case "PasteCopy":
            SetSubActionAndSubmit("PC");            
            break;
        case "PasteShortCut":
            SetSubActionAndSubmit("PS");            
            break;
        case "ExpandAll":       
            SetSubActionAndSubmit("XP");            
            break;
        case "Refresh":
            Refresh();
            break;
        case "WorkView":
            SetTreeContent("Tree.aspx?TM=1");
            break;

        case "CaseView":
            SetTreeContent("Tree.aspx?TM=0");
            break;
        case "OpenCasesView":
            SetTreeContent("Tree.aspx?TM=2");
            break;
        case "MyCasesView":
            SetTreeContent("Tree.aspx?TM=3");
            break;
        case "ArchivedCasesView":
            SetTreeContent("Tree.aspx?TM=4");
            break;
        case "Rename":        
            PC().RenameObject(id);
            break;
        case "SetBskLbl":
            PC().SetObjectLabel(id,2,0);
            break;
        case "SetFavLbl":
            PC().SetObjectLabel(id, 3, 0);
            break;
        default:
            eval(CommandName);
            break;
    }
}
function SetSubActionAndSubmit(value) {
    $get("subaction").value = value;
    Submit();
}
function SetSubAction(value) {
    $get("subaction").value = value;
}
function Submit() {
    document.forms[0].submit();
}
function SetTreeContent(url) {
    PC().GotoLink(url);
}
function OQX(id, parent_id, value) {
    OpenUrlInFolder(parent_id, "DM_DOCUMENTLIST.ASPX?&screenmode=advsearch&QRY_ID=" + id + "&DM_PARENT_ID=" + parent_id + "&QRY_VALUE=" + escape(value) + '&LOADQRY=Y');
}
function OQ(id, parent_id, resscreen) {
    OpenUrlInFolder(parent_id, "DM_DOCUMENTLIST.ASPX?&screenmode=advsearch&QRY_ID=" + id + "&DM_PARENT_ID=" + parent_id + '&LOADQRY=Y&DM_RESULT_SCREEN_ID=' + resscreen);
}
function OW(id) {
    PC().OpenWindow("DM_Detail.aspx?RTCASE_CASE_ID=" + id);
}

function Filter() {
    PC().Reload();    
}

//DRAG DROP
function cE(e) {   
    if (!e) e = window.event;
    if (e.preventDefault) {
        e.preventDefault();
    }
    if (e.returnValue) {
        e.returnValue = false;
    }
    return false;
}

function DRBasket() {

    var src, trgID, srcID;

    src = window.event.srcElement;
    while (src.tagName != "TD")
    { src = src.parentNode; }
    trgID = src.id;
    srcID = GetDataTransfer();
    if ((trgID && srcID) && (trgID != srcID)) {
        var id = new String(srcID);
        SetSelectedId(srcID);
        DMC("Cut");
        SetSelectedId(trgID);
        DMC("PasteBasket");
    }

}

function DRFavorites() {

    var src, trgID, srcID;

    src = window.event.srcElement;
    while (src.tagName != "TD")
    { src = src.parentNode; }
    trgID = src.id;
    srcID = GetDataTransfer();
    if ((trgID && srcID) && (trgID != srcID)) {
        var id = new String(srcID);
        SetSelectedId(srcID);
        DMC("Cut");
        SetSelectedId(trgID);
        DMC("PasteFavorites");
    }

}


function DR(e) {
 
    var trgID, srcID;
    if (!e) e = window.event;

    var src = (window.event) ? window.event.srcElement /* for IE */ : event.target;
    //src = window.event.srcElement;
    while (src.tagName != "TD")
    { src = src.parentNode; }
    trgID = src.id;
    srcID = GetDataTransfer(e);
   
    if ((trgID && srcID) && (trgID != srcID)) {
        var id = new String(srcID);
        var type = id.substring(0, 1);       

        SetSelectedId(srcID);
        DMC("Cut");
        SetSelectedId(trgID);

        if (type == 'q') //query, create a link in the tree
        {
          
            DMC("PasteQuery");
        }
        else {           
          
            var targettype = new String(trgID).substring(0, 1);
            switch (targettype) {
                case "p":
                    DMC("PasteIntoPackage");
                    break;
                default:
                    if (window.event.ctrlKey || window.event.shiftKey) {
                        sm('pastebox', 200, 75);
                    }
                    else {
                        DMC("Paste");
                    }
                    break;
            }

        }
    }
    cE(e);
}

function hDS(id, e) {   
    if (!e) e = window.event;
       
    e.dataTransfer.setData("text", id);
}
function hDE(e) {   
    cE(e);
}
function hDL(e) {    
    cE(e);
}
//END DRAG DROP


function GetSelectedId() {
    return new String($get("txtSelectedID").value);
}
function SetSelectedId(value) {
    $get("txtSelectedID").value = value;
}

function ShowContextMenu(e) {

    if (e == undefined)
        e = window.event;

    var src = (!PC().IsIE ? e.target : e.srcElement);
    if (src.tagName == "IMG") {
        e.cancelBubble = false;
        return false;
    }
    while (src.tagName != "TD" && src.tagName != "TABLE" && src.tagName != "DIV" && src.tagName != "BODY")
    { src = src.parentNode; }

    if (src.tagName != "TD") {
        return true;
    }
    try { ob_t22(src, e); } //on contextmenu on the treeview -> highlight and select the node
    catch (ex)
	        { }

    var id = GetSelectedId();
    if (id == "") {
        return true;
    }
    var w;



    var aSplit = id.split("_");
    id = aSplit[0];
    var packid = 0;
    if (aSplit.length > 1) {
        packid = aSplit[1];
    }
    id = id.substring(1, id.length);

    var ctxFile = "";
    while (src.tagName != "SPAN")
    { src = src.childNodes[0]; }

    switch (src.getAttribute('dmtype')) {
        case "Package":
            PC().ShowContextMenu(e, packid, ctxFile, delegate(this, PC().LoadPackageContextItems), id);
            e.cancelBubble = true;
            return false;
            break;
        case "Custom":
            e.cancelBubble = true;
            return false;
            break;
        case "QueryPropExp":
            //for a prop exp value, show the parent context menu
            //		            var aSplit = id.split("_");
            //		            id = aSplit[0];           
            PC().ShowContextMenu(e, id, ctxFile, delegate(this, PC().LoadPropertyExpansionContextItems), src.getAttribute('qryvalue'));
            e.cancelBubble = true;
            return false;
            break;
        case "List":
            PC().ShowContextMenu(e, id, ctxFile, delegate(this, PC().LoadListContextItems));
            e.cancelBubble = true;
            return false;
            break;
        default:                        
            var extra = src.getAttribute('extra');
            if (!extra) { extra = ""; }

            PC().ShowContextMenu(e, id, ctxFile, delegate(this, PC().LoadTreeContextItems), extra);
            e.cancelBubble = true;
            return false;
            break;
    }
}



function Refresh(openid) {
    if (!openid) {
        openid = $get("txtGotoID").value;
    }    
    SetSelectedId("f" + openid);
    Submit();
}

function ReloadParent() {
    SetSubActionAndSubmit("");    
}
function ReloadList() {
    parent.ReloadContent();
}


