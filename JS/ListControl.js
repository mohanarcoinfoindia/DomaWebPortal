function DocroomListControl(clientID, parentid, rootid, catid, lastpage, headermenu, visible, GridMode, ListRows, objectlist, resulttype, enablecontextmenu, termslist, tlbclientID, linkedtocaseid, linkedtoobjectid, packid, postback, callback, xmlfile, defaultxmlfile, ajaxenabled, ajaxcontrol, resultscreenid, caseseltype, contextmenufile, showDisabledButtons, opendetailwindowsmodal) {
    this.Reload = function () {
        this.SaveSelection();
        $get(this.clientID + "_internalsubmit").value = "Y";

        if (this.Ajax) {
            var mgr = $find(PC().AjaxManager);

            mgr.ajaxRequestWithTarget(this.AjaxButton);
        }
        else {
            PC().Reload();
        }
    };

    this.HandleEnter = function (e) {
        var key = e.which ? e.which : e.keyCode;
        if (key == 13) {
            this.Filter(1);
            return false;
        }
    };

    this.GetControl = function (id) {
        return $get(this.clientID + "_" + id);
    }

    this.ShowCollumn = function (s) {
        this.GetControl("showhidecol").value = "SHOW:" + s;
        this.Reload();
    };
    this.HideCollumn = function (s) {
        this.GetControl("showhidecol").value = "HIDE:" + s;
        this.Reload();
    };
    this.ResetToDefault = function () {
        this.GetControl("showhidecol").value = "RESETALL";
        this.Reload();
    };


    this.ResetOrderby = function () {
        this.GetControl("showhidecol").value = "RESETORDERBY";
        this.Rebind();
    };
    this.ResetColumns = function () {
        this.GetControl("showhidecol").value = "RESETCOLS";
        this.Reload();
    };
    this.SelectDelegate = function (del) {
        this.GetControl("FilterOnDelegate").value = del;
        this.Filter();
    };
    this.Filter = function () {

        this.ResetSelection();
        this.Rebind();
    };

    this.Rebind = function () {
        this.GetControl("maxresults").value = "";
        this.SetCurrentPage("1");
        this.Reload();
    };

    this.Goto = function (page) {

        // if ((page == 1) || ((parseInt(page) <= this.LastPage) && (parseInt(page) > 0))) {

        this.SetCurrentPage(page);
        this.Reload();
        //  }
    };
    this.OrderBy = function (field, defaultdirection) {
        if (defaultdirection != "ASC" && defaultdirection != "DESC") {
            defaultdirection = "ASC";
        }

        var orderbyField = this.GetControl("OrderbyField");
        var orderbyOrderField = this.GetControl("OrderbyOrderField");
        if (field == orderbyField.value) {
            orderbyOrderField.value == "DESC" ? orderbyOrderField.value = "ASC" : orderbyOrderField.value = "DESC";
        }
        else {
            orderbyField.value = field;
            orderbyOrderField.value = defaultdirection;
        }
        this.Goto(1);
    };
    this.ClearOrderBy = function () {
        this.GetControl("OrderbyField").value = "";
        this.GetControl("OrderbyOrderField").value = "";
    };
    this.GlobalSearch = function (value) {
        this.SetGlobalSearch(value);
        this.Filter();
    };
    this.SetGlobalSearch = function (value) {
        $get(this.clientID + "_txtGlobalSearch").value = value;
    };
    this.ApplyGrouper = function (s) {
        this.GetControl("GroupByFilter").value = s;
        this.Filter();
    };
    this.AddGrouper = function (id, g) {
        var groupByField = this.GetControl("GroupBy");
        groupByField.value = groupByField.value + id + ';;' + g + '##';
        this.GetControl("GroupByFilter").value = "";
    };

    this.RemoveGrouper = function (id, g) {
        var groupByField = this.GetControl("GroupBy");
        var v = new String(groupByField.value);
        v = v.replace(id + ';;' + g + '##', '');
        groupByField.value = v;
        this.GetControl("GroupByFilter").value = "";
    };
    this.SetResultScreen = function (id) {
        var resultScreenField = this.GetControl("ResultScreen");
        if (resultScreenField.value != id) {
            resultScreenField.value = id;
            this.ClearOrderBy();
        }

        this.Filter();

    };
    this.ToggleGrouper = function (id, g) {
        var groupByField = this.GetControl("GroupBy");
        var v = new String(groupByField.value);
        if (v.indexOf(id + ';;' + g + ',') >= 0) {
            v = v.replace(id + ';;' + g + ',', '');
            groupByField.value = v;
            this.GetControl("GroupByFilter").value = "";
        }
        else {
            groupByField.value = groupByField.value + id + ';;' + g + ',';
            this.GetControl("GroupByFilter").value = "";
        }
    };
    this.DisableFilter = function (idx) {
        var field = $get(this.clientID + "_DisabledFilters");
        field.value = field.value + ';' + idx + ';';
        this.Filter();
    };
    this.EnableFilter = function (idx) {
        var field = $get(this.clientID + "_DisabledFilters");
        var v = new String(field.value);
        v = v.replace(';' + idx + ';', '');
        field.value = v;
        this.Filter();
    };

    this.whatsnewDaySelected = function (sender, eventArgs) {
        eventArgs.set_cancel(true);
        var day = eventArgs.get_renderDay().get_date();

        var creationDateField = this.GetControl("CreationDate");
        var brefresh = false;
        if (creationDateField.value != '' && creationDateField.value != day[0] + '-' + day[1] + '-' + day[2])
            brefresh = true;
        creationDateField.value = day[0] + '-' + day[1] + '-' + day[2];
        if (brefresh) {
            this.Filter();
        }
    };
    this.ToggleWhatsNew = function () {
        var field = this.GetControl("ShowWhatsNew");
        field.value == 'Y' ? field.value = '' : field.value = 'Y';
        this.Filter();
    };

    this.ToggleShowSuspended = function () {
        DocroomListFunctions.ToggleUserProfileParameter("SHOWSUSPENDEDDOSSIERS", delegate(this, this.Goto, 1));

    };
    this.ToggleShowLocked = function () {
        DocroomListFunctions.ToggleUserProfileParameter("SHOWLOCKEDDOSSIERS", delegate(this, this.Goto, 1));


    };
    this.ToggleShowSideBar = function () {
        DocroomListFunctions.ToggleUserProfileParameter("SHOWGRIDSIDEBAR", delegate(this, this.Reload));
    };
    this.ToggleHideDelegated = function () {
        DocroomListFunctions.ToggleUserProfileParameter("HIDEDELEGATEDWORK", delegate(this, this.Goto, 1));
    };

    this.ShowAttachmentsFor = function (objid) {
        this.GetControl("showattachmentsfor").value = objid;
        this.Reload();
    };
    this.ShowPreviousFileVersions = function (fileid) {
        this.GetControl("showfileversionsfor").value = fileid;
        this.Reload();

    };

    this.ShowFile = function (file_id, fromarchive, rend, frommail, forceinpreviewpane, mailid) {
        PC().ShowFile(file_id, fromarchive, rend, frommail, this.TermsList, forceinpreviewpane, mailid, false);
    };

    this.SaveSelection = function () {
        this.GetControl("Sel").value = this.GetSelectionFromCheckbox();
    };

    this.GetMailSelection = function () {
        return this.GetSelectionFromCheckbox("mid");
    };

    this.GetCaseSelection = function () {
        if (this.CaseSelectionType == 8) {
            return this.GetSelectionFromCheckbox("tid");
        }
        else {
            return this.GetSelectionFromCheckbox("cid");
        }
    };

    this.GetSelection = function () {
        return this.GetSelectionFromCheckbox("oid");
    };

    this.ArrayContains = function (arr, what) {
        for (var i = 0; i < arr.length; i++) {
            if (arr[i] == what) { return true; }
        }
        return false;
    };
    this.ArrayRemove = function (arr, what) {
        for (var i = arr.length - 1; i >= 0; i--) {
            if (arr[i] == what) {
                arr.splice(i, 1);
            }
        }
    };

    this.GetSelectionFromCheckbox = function (attr) {
        var selectionField = this.GetControl("Sel");
        if (!selectionField) return "";

        var curr = new String(selectionField.value);
        var currItems;
        if (curr.length != 0) {
            currItems = curr.split(";");
            this.ArrayRemove(currItems, "");
        }
        else {
            currItems = new Array();
        }
        var id = "";
        var selectionCheckBoxes = document.getElementsByName(this.clientID + "_chkSelection");
        if (selectionCheckBoxes) {
            if (selectionCheckBoxes.length) {
                for (var intLoop = 0; intLoop < selectionCheckBoxes.length; intLoop++) {
                    id = selectionCheckBoxes[intLoop].value;
                    if (selectionCheckBoxes[intLoop].checked) {
                        if (!this.ArrayContains(currItems, id)) {                          
                            currItems.push(id);
                        }
                    }
                    else {
                        if (this.ArrayContains(currItems, id)) {
                            this.ArrayRemove(currItems, id);
                        }
                    }
                }
            }
            else {
                id = selectionCheckBoxes.value;

                if (selectionCheckBoxes.checked) {
                    if (!this.ArrayContains(currItems, id)) {
                        currItems.push(id);
                    }
                }
                else {
                    if (this.ArrayContains(currItems, id)) {
                        this.ArrayRemove(currItems, id);
                    }
                }
            }
        }
        var sel = "";
        for (var i = 0; i < currItems.length; i++) {         
            if (attr) {
                var idsInItem = currItems[i].split(",");
                for (var j = 0; j < idsInItem.length; j++) {
                    if (idsInItem[j].indexOf(attr) == 0) {
                        var val = idsInItem[j].substr(idsInItem[j].indexOf("=") + 1);
                        sel += val + ";";
                    }
                }
            }
            else {
                sel += currItems[i] + ";";
            }
        }
     
        return sel;
    };

    this.CurrentPage = function () { return this.GetControl("currpage").value; };
    this.SetCurrentPage = function (p) { this.GetControl("currpage").value = p; };
    this.GetResultCount = function () { return parseInt(this.GetControl("ItemCount").value); };

    this.SelectFilter = function (fieldname, initial) {
        if ($get(this.clientID + "_" + fieldname).value != initial) {
            if (fieldname == 'fltproc') {
                var filt = $get(this.clientID + "_fltstep");
                if (filt) {
                    filt.value = '';
                }
            }

            this.Filter();
        }
    };
    this.ResetSelection = function () {
        var button = this.GetControl("togglebutton");

        if (button) {
            button.checked = false;
            this.ToggleMultiSelect();
        }
        this.GetControl("Sel").value = "";
    };

    this.ToggleMultiSelect = function () {
        var button = this.GetControl("togglebutton");
      
        var selectionCheckBoxes = document.getElementsByName(this.clientID + "_chkSelection");
        if (selectionCheckBoxes)
            if (selectionCheckBoxes.length) {
                for (var intLoop = 0; intLoop < selectionCheckBoxes.length; intLoop++)
                    selectionCheckBoxes[intLoop].checked = button.checked;
            }
            else {
                selectionCheckBoxes.checked = button.checked;
            }

        if (button.checked) {
            button.title = UIMessages[14];
        } else {
            button.title = UIMessages[13];
        }
        this.ToggleToolbarSelectionButtons();
    };

    this.ToggleFileListRow = function (r, togglemode) {
        var row = $get(r + '_filelistrow');
        if (row) {
            var img = $get(r + '_togglebuton');
            var bDoToggle = true;
            if (togglemode != null) {
                if (row.style.display == '') {
                    bDoToggle = togglemode;
                }
                else {
                    bDoToggle = !togglemode;
                }
            }
            if (bDoToggle) {
                if (row.style.display == '') {
                    row.style.display = 'none';
                    img.src = './Images/Expand.svg';
                    img.alt = UIMessages[9];
                }
                else {
                    row.style.display = '';
                    img.src = './Images/Collapse.svg';
                    img.alt = UIMessages[10];
                }
            }
            img = null;
        }
        row = null;
    };

    this.ToggleFiles = function () {
        var modeField = this.GetControl("filelisttogglemode");
        var mode = (modeField.value == "Y" ? true : false);
        var toggleButton = this.GetControl("togglefilesbutton");
        for (var i = 0; i < this.ListRows.length; i++)
            this.ToggleFileListRow(this.ListRows[i], mode);

        if (!mode) {
            toggleButton.src = './Images/Collapse.svg';
            toggleButton.title = UIMessages[12];
        } else {
            toggleButton.src = './Images/Expand.svg';
            toggleButton.title = UIMessages[11];
        }
        if (mode) {
            modeField.value = "";
        }
        else {
            modeField.value = "Y";
        }
    };

    this.DoFolderAction = function (id, CommandName) {

        switch (CommandName) {
            case "Browse":
                if (PC().PageLocation == 2) {
                    PC().GotoLink('DM_DocumentList.aspx?DM_PARENT_ID=' + id + '&screenmode=browse&RefreshTree=Y');
                }
                else {
                    PC().OpenPreviewLink('DM_DocumentList.aspx?DM_PARENT_ID=' + id + '&screenmode=browse&RefreshTree=Y', false, false, false);
                }
                break;
            case "BrowseWithSubFolders":
                if (PC().PageLocation == 2) {
                    PC().GotoLink('DM_DocumentList.aspx?DM_PARENT_ID=' + id + '&screenmode=browsewithsubfolders&RefreshTree=Y');
                }
                else {
                    PC().OpenPreviewLink('DM_DocumentList.aspx?DM_PARENT_ID=' + id + '&screenmode=browsewithsubfolders&RefreshTree=Y', false, false, false);
                }
                break;
            case "Open":
                this.OpenDetailWindow('dm_detail.aspx?DM_OBJECT_ID=' + id + '&folderid=' + this.ParentID + '&objectlist=' + this.ObjectList + '&mode=1', id);
                break;
            case "Search":
                if (PC().PageLocation == 2) {
                    PC().GotoLink('DM_DocumentList.aspx?DM_PARENT_ID=' + id + '&screenmode=query&RefreshTree=Y');
                }
                else {
                    PC().OpenPreviewLink('DM_DocumentList.aspx?DM_PARENT_ID=' + id + '&screenmode=query&RefreshTree=Y', false, false, false);
                }
                break;
            case "Explore":
                PC().OpenWindow('DM_EXPLORE.aspx?DM_PARENT_ID=' + id + '&DM_ROOT_ID=' + this.RootID, 'MainWindow' + id, 'height=800,width=1200,scrollbars=yes,resizable=yes');
                break;
            case "Add":
                PC().OpenWindow('DM_SELECT_OBJ_CAT.aspx?CAT_TYPE=&DM_PARENT_ID=' + id, 'EditObject', 'height=600,width=800,scrollbars=yes,resizable=yes');
                break;
            case "Edit":
                this.OpenDetailWindow('dm_detail.aspx?DM_OBJECT_ID=' + id + '&folderid=' + this.ParentID + '&objectlist=' + this.ObjectList + '&mode=2', id);
                break;
            case "FolderText":
                if (PC().PageLocation == 2) {
                    PC().GotoLink('DM_DocumentList.aspx?DM_PARENT_ID=' + id + '&screenmode=foldertext&RefreshTree=Y');
                }
                else {
                    PC().OpenPreviewLink('DM_DocumentList.aspx?DM_PARENT_ID=' + id + '&screenmode=foldertext&RefreshTree=Y', false, false, false);
                }
                break;
            case "Page":
                if (PC().PageLocation == 2) {
                    PC().GotoLink('Page.aspx?DM_PARENT_ID=' + id);
                }
                else {
                    PC().OpenPreviewLink('Page.aspx?DM_PARENT_ID=' + id, false, false, false);
                }
                break;
            default:
                alert('Unhandled command ' + CommandName);
                break;
        }
    };
    this.ToggleSelectionDone = function (objid) {

    };
    this.ToggleSelection = function (imgel, objid, seltype, addicon, removeicon, addlabel, removelabel) {

        let iconSpan = imgel.getElementsByTagName("span")[0];

        if (iconSpan) {
           
            if (iconSpan.className.match(addicon) != null) {
                DocroomListFunctions.AddToSelection(objid, seltype);
                iconSpan.className = removeicon;
                iconSpan.title = removelabel;
            }
            else {
                DocroomListFunctions.RemoveFromSelection(objid, seltype);
                iconSpan.className = addicon;
                iconSpan.title = addlabel;
            }
        }
        else {
           
            let iconImg = imgel.getElementsByTagName("img")[0];        
            if (iconImg.src.match(addicon) != null) {
                DocroomListFunctions.AddToSelection(objid, seltype);
                iconImg.src = iconImg.src.replace(addicon, removeicon); //'./Images/' + removeicon;
                iconImg.title = removelabel;
            }
            else {
                DocroomListFunctions.RemoveFromSelection(objid, seltype);
                iconImg.src = iconImg.src.replace(removeicon, addicon);
                iconImg.title = addlabel;
            }

        }
    };

    this.LinkSelectionToRoutingCase = function (CaseID, PackID) {
        var sel = this.GetSelection();
        if (sel) {
            PC().OpenUrlInActionPane('DM_LINK_DOCS_TO_ROUTING.aspx?CASE_ID=' + CaseID + '&PACK_ID=' + PackID + '&SELECTION=' + sel);
        }
    };
    this.EditObjectSelection = function (screenid) {
        var sel = this.GetSelection();
        if (sel) {
            if (!screenid) screenid = 0;
            PC().OpenModalWindow('MultiView.aspx?mode=2&SCREEN_ID=' + screenid + '&OBJECTLIST=' + sel, true, 800, 500, false, true);
        }
    };
    this.LinkSelectionToObject = function (objid) {
        this.AddToSelectionWithCallback(1, delegate(PC(), PC().LinkObjectsToOIP, objid, 0));
    };

    this.ExecuteDocumentListAction = function (actiontype, actionid, seltype) {
        PC().OpenUrlInActionPane('DM_DocumentListAction.aspx?ACTIONTYPE=' + actiontype + '&SELTYPE=' + seltype + '&ACTIONID=' + actionid);
    };
    this.ExecuteCaseListAction = function (actiontype, actionid, seltype) {
        PC().OpenUrlInActionPane('CaseListAction.aspx?ACTIONTYPE=' + actiontype + '&SELTYPE=' + seltype + '&ACTIONID=' + actionid);
    };
    this.ExecuteMailListAction = function (actiontype, actionid, seltype) {
        PC().OpenUrlInActionPane('./MailClient/MailListAction.aspx?ACTIONTYPE=' + actiontype + '&SELTYPE=' + seltype + '&ACTIONID=' + actionid);
    };
    this.ExecuteMailBoxAction = function (actiontype, actionid, seltype, boxid) {
        if (actiontype != 4) {
            //this keeps the selection checked
            DocroomListFunctions.DoMailboxAction(actiontype, seltype, actionid, boxid, delegate(this, this.Reload));
        }
        else {
            this.GetControl("Sel").value = "";
            PC().OpenUrlInActionPane('./MailClient/MailBoxAction.aspx?ACTIONTYPE=' + actiontype + '&SELTYPE=' + seltype + '&ACTIONID=' + actionid + "&BOXID=" + boxid);
            this.Reload();
        }
    };
    this.DoCaseListAction = function (actiontype, actionid) {
        if (actiontype == 7) {
            if (!confirm(UIMessages[8])) {
                return;
            }
        }
        if (actiontype == 6) {
            if (!confirm(UIMessages[26])) {
                return;
            }
        }
        PC().ShowProgress();
        this.AddCasesToSelectionWithCallback(this.CaseSelectionType, delegate(this, this.ExecuteCaseListAction, actiontype, actionid, this.CaseSelectionType));
    };
    this.DoMailListAction = function (actiontype, actionid) {
        PC().ShowProgress();
        this.AddMailsToSelectionWithCallback(9, delegate(this, this.ExecuteMailListAction, actiontype, actionid, 9));
    };
    this.DoMailBoxAction = function (actiontype, actionid, boxid) {
        if (actiontype == 4) {
            if (!confirm(UIMessages[1])) {
                return;
            }
        }
        PC().ShowProgress();
        this.AddMailsToSelectionWithCallback(9, delegate(this, this.ExecuteMailBoxAction, actiontype, actionid, 9, boxid));
    };
    this.DoDocumentListAction = function (actiontype, actionid) {
        if (actiontype == 4) {
            if (!confirm(UIMessages[1])) {
                return;
            }
        }
        PC().ShowProgress();
        this.AddToSelectionWithCallback(1, delegate(this, this.ExecuteDocumentListAction, actiontype, actionid, 1));
    };

    this.OpenWindow = function (url, name, args) {
        PC().OpenWindow(url, name, args);
    };

    this.CopyMoveSelection = function (parentid) {
        if (!parentid) {
            parentid = "0";
        }
        var url = 'DM_CopyMoveToFolder.ASPX?goto=' + parentid;
        this.AddToSelectionWithCallback(1, delegate(PC(), PC().OpenModalWindow, url, false, 600, 400, true));
    };
    this.MailSelection = function (templateid) {
        if (!templateid) {
            templateid = "0";
        }
        var url = './MailClient/Compose2.aspx?mailsel=1&template_id=' + templateid;

        this.AddToSelectionWithCallback(1, delegate(this, this.OpenDetailWindow, url, templateid));
    };

    this.SubscribeToCurrentFolder = function () {
        this.SubscribeTo(this.ParentID, 'Folder');
    };
    this.SubscribeTo = function (objId,objType) {
        parent.PC().SubscribeTo(objId, objType);
    };

    this.AddNew = function () {
        PC().AddObject('', this.ParentID, this.CategoryID, this.PackID, this.OpenDetailWindowsModal, clientID);
    };

    this.CreateObject = function (catid, inparentid) {
        if (!catid) {
            catid = "";
        }
        if (!inparentid) {
            inparentid = parentid;
        }
        this.OpenDetailWindow('./DM_NEW_OBJECT.aspx?DM_CAT_ID=' + catid + "&DM_PARENT_ID=" + inparentid + "&GRID_ID=" + clientID, 0);
        
    };
    this.CreateObjectFromSelection = function (catid, inparentid) {
        if (!catid) {
            catid = "";
        }
        if (!inparentid) {
            inparentid = parentid;
        }
        var url = './DM_NEW_OBJECT.aspx?DM_CAT_ID=' + catid + "&INPUTSELECTION=1&DM_PARENT_ID=" + inparentid + "&GRID_ID=" + clientID;
        this.AddToSelectionWithCallback(1, delegate(this, this.OpenDetailWindow, url, 0));
    };
    this.CreateObjectFromCaseSelection = function (catid, inparentid) {
        if (!catid) {
            catid = "";
        }
        if (!inparentid) {
            inparentid = parentid;
        }
        var url = './DM_NEW_OBJECT.aspx?DM_CAT_ID=' + catid + "&INPUTSELECTION=" + this.CaseSelectionType + "&DM_PARENT_ID=" + inparentid + "&GRID_ID=" + clientID;
        this.AddCasesToSelectionWithCallback(this.CaseSelectionType, delegate(this, this.OpenDetailWindow, url, 0));
    };
    this.StartProcedureFromSelection = function (procid, inparentid) {
        if (!procid) {
            procid = "";
        }
        if (!inparentid) {
            inparentid = parentid;
        }
        var url = './DM_NEW_CASE.aspx?PROC_ID=' + procid + "&INPUTSELECTION=1&DM_PARENT_ID=" + inparentid + "&GRID_ID=" + clientID;
        this.AddToSelectionWithCallback(1, delegate(this, this.OpenDetailWindow, url, 0));
    };
    this.StartProcedureFromCaseSelection = function (procid, inparentid) {
        if (!procid) {
            procid = "";
        }
        if (!inparentid) {
            inparentid = parentid;
        }
        var url = './DM_NEW_CASE.aspx?PROC_ID=' + procid + "&INPUTSELECTION=" + this.CaseSelectionType + "&DM_PARENT_ID=" + inparentid + "&GRID_ID=" + clientID;
        this.AddCasesToSelectionWithCallback(this.CaseSelectionType, delegate(this, this.OpenDetailWindow, url, 0));
    };
    this.ExecutePrint = function (printoption, printseltype, printxml) {
        var url = "DM_DocumentList.aspx?screenmode=print&printoptions=" + printoption + "&printpage=" + this.CurrentPage() + "&printseltype=" + printseltype;
        if (printxml) {
            url += '&printxml=' + printxml;
        }
        url = PC().AddSiteToUrl(url);

        if (printoption == 0) {
            switch (printseltype) {
                case this.CaseSelectionType:
                    this.AddCasesToSelectionWithCallback(this.CaseSelectionType, delegate(PC(), PC().OpenUrlInActionPane, url));
                    break;
                case 9:
                    this.AddMailsToSelectionWithCallback(9, delegate(PC(), PC().OpenUrlInActionPane, url));
                    break;
                default:
                    this.AddToSelectionWithCallback(1, delegate(PC(), PC().OpenUrlInActionPane, url));
            }
        }
        else {
            PC().OpenUrlInActionPane(url);
        }
    };    
    this.ExecutePrintToExcel = function (printoption, printseltype, printxml) {
    
       // PC().ShowProgress();
        var url = "PrintToExcel.aspx?printoptions=" + printoption + "&printpage=" + this.CurrentPage() + "&printseltype=" + printseltype;
        if (printxml) {
            url += '&printxml=' + printxml;
        }
        else {
            url += '&printxml=' + this.xmlfile;
        }
        url = PC().AddSiteToUrl(url);
        if (printoption == 0) //selection
        {
            switch (printseltype) {
                case this.CaseSelectionType:
                    this.AddCasesToSelectionWithCallback(this.CaseSelectionType, delegate(PC(), PC().OpenModalWindow, url, false, 400, 150, true));
                    break;
                case 9:
                    this.AddMailsToSelectionWithCallback(9, delegate(PC(), PC().OpenModalWindow, url, false, 400, 150, true));
                    break;
                default:
                    this.AddToSelectionWithCallback(1, delegate(PC(), PC().OpenModalWindow, url,false,400,150,true));
            }

        }
        else {
            PC().OpenModalWindow(url, false, 400, 150, true);            
        }
    };
    

    this.RemoveLink = function (sourceid, targetid, linktype) {
        DocroomListFunctions.RemoveLink(sourceid, targetid, linktype, delegate(this, this.Reload));
    };

    this.EmptyRecyclebin = function () {
        if (confirm(UIMessages[7])) {
            DocroomListFunctions.EmptyRecycleBin();
            PC().ShowSuccess(UIMessages[22]);
            this.Reload();
        }
    };
    this.EmptyRecyclebinSelection = function () {
        var docs = this.GetSelection();
        if (docs != "") {
            if (confirm(UIMessages[8])) {
                DocroomListFunctions.EmptyRecycleBinSelection(docs);
                PC().ShowSuccess(UIMessages[23]);
                this.Reload();

            }
        }
    };
    this.RestoreObject = function (id) {
        if (id != "") {
            DocroomListFunctions.RestoreRecycleBinSelection(id, delegate(this, this.Reload));
        }
    };
    this.RestoreRecycleBinSelection = function () {
        var docs = this.GetSelection();
        if (docs !== "") {
            DocroomListFunctions.RestoreRecycleBinSelection(docs, delegate(this, this.Reload));
        }
    };
    this.SelectPrintOptions = function (selection, printxml, printseltype, mode) {

        const hasSelection = selection === "" ? "" : "Y";
        var link = "DM_SELECT_PRINT.aspx?HASSEL=" + hasSelection + '&printseltype=' + printseltype + '&mode=' + mode;
        if (printxml) {
            link += '&printxml=' + printxml;
        }
        PC().OpenModalWindow(link, false, 400, 150, true);
    };
    this.PrintGridToExcel = function (printxml) {        
        this.SelectPrintOptions(this.GetSelection(), printxml, 1, 'xls');
    };
    this.PrintCaseGridToExcel = function (printxml) {        
        this.SelectPrintOptions(this.GetCaseSelection(), printxml, this.CaseSelectionType, 'xls');
    };
    this.PrintMailGridToExcel = function (printxml) {
        
        this.SelectPrintOptions(this.GetMailSelection(), printxml, 9, 'xls');
    };
    this.PrintGrid = function (printxml) {        
        this.SelectPrintOptions(this.GetSelection(), printxml, 1, '');
    };
    this.PrintCaseGrid = function (printxml) {        
        this.SelectPrintOptions(this.GetCaseSelection(), printxml, this.CaseSelectionType, '');
    };
    this.PrintMailGrid = function (printxml) {        
        this.SelectPrintOptions(this.GetMailSelection(), printxml, 9, '');
    };

    this.RemoveFromSelection = function (seltype, reload) {
        var sel = this.GetSelection();
        this.ResetSelection();
        if (reload) {
            DocroomListFunctions.RemoveListFromSelection(sel, seltype, delegate(this, this.Reload));
        }
        else {
            DocroomListFunctions.RemoveListFromSelection(sel, seltype);
        }
    };
    this.AddToSelection = function (seltype) {
        var sel = this.GetSelection();
        if (sel !== "") {
            DocroomListFunctions.AddListToSelection(sel, seltype);
            PC().ShowSuccess(UIMessages[19]);
        }
    };
    this.AddCasesToSelection = function (seltype) {
        var sel = this.GetCaseSelection();
        if (sel !== "") {
            DocroomListFunctions.AddListToSelection(sel, seltype);
            PC().ShowSuccess(UIMessages[19]);
        }
    };
    this.AddMailsToSelection = function (seltype) {
        var sel = this.GetMailSelection();
        if (sel !== "") {
            DocroomListFunctions.AddListToSelection(sel, seltype);
            PC().ShowSuccess(UIMessages[19]);
        }
    };
    this.AddToSelectionWithCallback = function (seltype, deleg) {
        var sel = this.GetSelection();

        DocroomListFunctions.AddListToSelection(sel, seltype, deleg);
    };
    this.AddCasesToSelectionWithCallback = function (seltype, deleg) {
        var sel = this.GetCaseSelection();

        DocroomListFunctions.AddListToSelection(sel, seltype, deleg);
    };
    this.AddMailsToSelectionWithCallback = function (seltype, deleg) {
        var sel = this.GetMailSelection();

        DocroomListFunctions.AddListToSelection(sel, seltype, deleg);
    };
    this.ClearSelection = function (seltype, reload) {

        if (reload) {
            DocroomListFunctions.ClearSelection(seltype, delegate(this, this.Reload));
        }
        else {
            DocroomListFunctions.ClearSelection(seltype);
        }

    };
    this.AddCaseSelectionButton = function (val) {
        this.ToolbarCaseSelectionButtons.push(val);
    };
    this.AddMailSelectionButton = function (val) {
        this.ToolbarMailSelectionButtons.push(val);
    };
    this.AddSelectionButton = function (val) {
        this.ToolbarSelectionButtons.push(val);
    };

    this.AddObjectToCase = function (parentid) {
        this.AddObjectToPackage(parentid);
    };
    this.AddObjectToDossiers = function (objectid, dossiertype, parentid, site) {
        if (!isDefined(parentid)) {
            parentid = 0;
        }
        if (!isDefined(dossiertype)) {
            dossiertype = 0;
        }
        if (!isDefined(site)) {
            site = "AddToDossiers";
        }
        else {
            if (site == "") site = "AddToDossiers";
        }
        var address;

        address = 'Default.aspx?TARGET_OBJ_ID=' + objectid + '&TARGET_DOSSIER_TYPE=' + dossiertype + '&SITE=' + site + '&DEFAULTSITE=' + PC().CustomSite + '&DM_PARENT_ID=' + parentid + '&DM_ROOT_ID=' + parentid;

        parent.PC().OpenModalWindow(address, true, 800, 600, false);
    };
    this.AddObjectToPackage = function (parentid, site) {
        if (this.FromCaseID > 0) {
            parent.PC().AddToCaseObjectPackage(this.FromCaseID, this.PackID, parentid, site, true);
        }
        else {
            parent.PC().AddToObjectObjectPackage(this.FromObjectID, this.PackID, parentid, site, true);
        }
    };
    this.AddObjectToCasePackage = function (parentid, site) {
        if (this.FromCaseID > 0) {
            parent.PC().AddToCaseCasePackage(this.FromCaseID, this.PackID, parentid, site, true);
        }
        else {
            parent.PC().AddToObjectCasePackage(this.FromObjectID, this.PackID, parentid, site, true);
        }
    };
    this.AddSelectionToCase = function (seltype, reload) {
        this.AddSelectionToPackage(seltype, reload);
    };
    this.AddSelectionToPackage = function (seltype, reload) {
        if (this.FromCaseID > 0) {
            DocroomListFunctions.AddSelectionToCasePackage(this.FromCaseID, this.PackID, seltype);
        }
        else {
            DocroomListFunctions.AddSelectionToObjectPackage(this.FromObjectID, this.PackID, seltype);
        }
        if (reload) {
            PC().Reload();
        }
    };
    this.RemoveObjectFromCase = function (item) {
        this.RemoveObjectFromPackage(item);
    };
    this.RemoveObjectFromPackage = function (item) {
        var sMsg = UIMessages[27];
        if (item == 0) sMsg = UIMessages[28];
        if (confirm(sMsg)) {
            if (this.FromCaseID > 0) {
                DocroomListFunctions.RemoveObjectFromCasePackage(this.FromCaseID, this.PackID, item);
            }
            else {
                DocroomListFunctions.RemoveObjectFromObjectPackage(this.FromObjectID, this.PackID, item);
            }
            PC().Reload();
        }
    };
    this.RemoveObjectFromDossier = function (objectdin, dossierid) {
        DocroomListFunctions.RemoveObjectFromDossier(dossierid, objectdin);
        PC().Reload();
    };
    this.ToggleToolbarSelectionButtons = function () {       
        var mode;
        var i = 0;
        var but;
        var tlb = $find(this.ToolbarClientID);
        if (tlb) {
            mode = (this.GetSelection() != "");
            for (i = 0; i < this.ToolbarSelectionButtons.length; i++) {
                but = tlb.findItemByValue(this.ToolbarSelectionButtons[i]);
                this.ToggleToolbarButton(but, mode);
            }

            mode = (this.GetCaseSelection() != "");
            for (i = 0; i < this.ToolbarCaseSelectionButtons.length; i++) {
                but = tlb.findItemByValue(this.ToolbarCaseSelectionButtons[i]);
                this.ToggleToolbarButton(but, mode);
            }

            mode = (this.GetMailSelection() != "");            
            for (i = 0; i < this.ToolbarMailSelectionButtons.length; i++) {
                but = tlb.findItemByValue(this.ToolbarMailSelectionButtons[i]);
                this.ToggleToolbarButton(but, mode);
            }

            if (!showDisabledButtons) {
                var toolbarItems = tlb.get_allItems();
                var lastwassep = false;
                for (i = 0; i < toolbarItems.length; i++) {
                    if (toolbarItems[i].get_isSeparator && toolbarItems[i].get_isSeparator()) {
                        if (lastwassep) {
                            toolbarItems[i].hide();
                        } else {
                            lastwassep = true;
                            toolbarItems[i].show();
                        }
                    } else {
                        if (toolbarItems[i].get_buttons) {
                            var subButtons = toolbarItems[i].get_buttons();
                            var foundVisible = false;
                            for (j = 0; j < subButtons.length; j++) {
                                if (subButtons[j].get_visible()) {
                                    foundVisible = true;
                                    break;
                                }
                            }
                            if (foundVisible) {
                                toolbarItems[i].show();
                            } else {
                                toolbarItems[i].hide();
                            }
                        }
                    }
                }
            }
        }
    };

    this.ToggleToolbarButton = function (but, mode) {
        if (but) {
            if (mode) {
                if (!showDisabledButtons) {
                    but.show();
                }
                but.enable();
            } else {
                but.disable();
                if (!showDisabledButtons) {
                    but.hide();
                }
            }
        }
    };


    this.HiliteRow = function (newRow) {

        if (this.hlColor != null) {


            if (newRow != -1 && newRow != this.SelectedRow) {
                //this.oldRowColor = newRow.style.backgroundColor;
                //newRow.style.backgroundColor = this.hlColor;
                //newRow.style.border = this.hlBorder;
                //newRow.style.cursor = "pointer";
                if (!newRow.className.match(/(?:^|\s)listContentHighlighted(?!\S)/)) {
                    newRow.className += " listContentHighlighted";
                }
            }
            if (this.currHlRow != -1 && this.currHlRow != this.SelectedRow) {
                //this.currHlRow.style.backgroundColor = this.oldRowColor;
                //this.currHlRow.style.border = '0px';
                if (this.currHlRow.className.match(/(?:^|\s)listContentHighlighted(?!\S)/)) {
                    this.currHlRow.className =
                        this.currHlRow.className.replace
                            (/(?:^|\s)listContentHighlighted(?!\S)/g, '');
                }
            }
        }
        this.currHlRow = newRow;
    };

    this.GetCurrentCell = function (e) {

        var srcElem = (!PC().IsIE ? e.target : e.srcElement);
        var bCont = true;
        var i = 1;
        while (bCont) {
            bCont = false;
            while (srcElem.tagName != "TD" && srcElem.tagName != "BODY" && srcElem.tagName != "INPUT") {
                srcElem = PC().IsIE ? srcElem.parentElement : srcElem.parentNode;
            }
        }

        if (srcElem.tagName != "TD") return null;

        return srcElem;
    };
    this.GetMainRowElement = function (e, stoponlink) {

        var srcElem = (!PC().IsIE ? e.target : e.srcElement);
        var bCont = true;
        var i = 1;
        while (bCont) {
            bCont = false;               

            while (srcElem.tagName !== "TR" && srcElem.tagName !== "BODY" && (!stoponlink || !(srcElem.tagName === "INPUT" || srcElem.tagName === "LABEL" || srcElem.tagName === "A" || ((srcElem.tagName === "DIV" || srcElem.tagName === "SPAN") && srcElem.hasAttribute("onclick"))) )) {                         
                srcElem = PC().IsIE ? srcElem.parentElement : srcElem.parentNode;
            }
            if (srcElem.className === "" && srcElem.tagName === "TR") {
                srcElem = PC().IsIE ? srcElem.parentElement : srcElem.parentNode;
                bCont = true;
            }
        }

        if (srcElem.tagName !== "TR") { return null; }

        if (!srcElem.className.match(/(?:^|\s)ListContent(?!\S)/) && !srcElem.className.match(/(?:^|\s)ImageListContent(?!\S)/)) { return null; }
        

        return srcElem;
    };

    this.DoPreviewAction = function (id, forceinpreviewpane) {


        var url;
        if (id) {
            switch (this.ResultType) {
                case 0: //objects
                case 6:
                case 7:
                    var r = $get(id);
                    if (r) {
                        var t = r.getAttribute("object_type");
                        if (t == "Shortcut") {
                            t = r.getAttribute("object_reference_type");
                        }

                        if (t == "Folder") { // && PC().PageLocation == 2) { // browse to folder
                            var cmd = r.getAttribute("default_action");
                            if (!cmd) {
                                cmd = 'Browse';
                            }
                            this.DoFolderAction(id, cmd);
                        }
                        else {
                            if (PC().ShowPreview || forceinpreviewpane) {
                                let arch = r.getAttribute("archived");
                                if (!arch) arch = "";
                                url = 'dm_detailview.aspx?DM_OBJECT_ID=' + id + '&godefault=Y&terms=' + this.TermsList + '&objectlist=' + this.ObjectList + '&fromarchive=' + arch; //TermsList
                                PC().OpenPreviewLink(url, true, forceinpreviewpane, false);
                            }
                            else {
                                this.ListDoubleClick(id);
                            }
                        }
                        t = null;
                    }
                    r = null;
                    break;
                case 1: //files
                    if (PC().ShowPreview || forceinpreviewpane) {
                        let arch = $get(id).getAttribute("archived");
                        if (!arch) arch = "";
                        this.ShowFile(id, arch, 'preview', false, forceinpreviewpane, 0, false);
                    }
                    else {
                        this.ListDoubleClick(id);
                    }
                    break;
                case 2:
                    if (PC().ShowPreview || forceinpreviewpane) {
                        url = 'dm_detailview.aspx?RTCASE_TECH_ID=' + id + '&godefault=Y&terms=' + this.TermsList + '&caselist=' + this.ObjectList + '&fromarchive=N'; //TermsList
                        PC().OpenPreviewLink(url, true, forceinpreviewpane, false);
                    }
                    else {
                        this.ListDoubleClick(id);
                    }
                    break;
                case 5:
                case 3:
                    if (PC().ShowPreview || forceinpreviewpane) {
                        url = 'dm_detailview.aspx?RTCASE_CASE_ID=' + id + '&fromarchive=Y&godefault=Y&terms=' + this.TermsList + '&archivedcaselist=' + this.ObjectList; //TermsList
                        PC().OpenPreviewLink(url, true, forceinpreviewpane, false);
                    }
                    else {
                        this.ListDoubleClick(id);
                    }
                    break;
                case 4: //mails
                    if (PC().ShowPreview || forceinpreviewpane) {
                        url = './MailClient/View.aspx?DM_MAIL_ID=' + id + '&maillist=' + this.ObjectList;
                        PC().OpenPreviewLink(url, true, forceinpreviewpane, false);
                    }
                    else {
                        this.ListDoubleClick(id);
                    }
                    break;
            }
        }
    };

    this.DoRealAction = function (row) {
        if (row != '0') {
            switch (this.ResultType) {
                case 0: //objects
                case 6:
                case 7:
                    this.OpenObjectDetails(row, 1);
                    break;
                case 1: //files                 
                    var arch = $get(row).getAttribute("archived");
                    if (!arch) arch = "";
                    this.ShowFile(row, arch, 'preview', false, false, 0, false);
                    break;
                case 2: //cases                 
                    this.OpenCaseDetails(row, 2);
                    break;
                case 3: //archived cases
                    this.OpenArchiveCaseDetails(row, 1);
                    break;
                case 5: //open and archived cases
                    this.OpenArchiveCaseDetails(row, 2);
                    break;
                case 4: // mails
                    this.OpenMailDetails(row);
                    break;
            }
        }
    };
    this.DoAdminAction = function (row) {
        if (row != '0') {
            switch (this.ResultType) {
                case 0: //objects
                case 6:
                case 7:
                    this.OpenObjectDetails(row, 4);
                    break;
                case 1: //files
                    //can't admin a file directly
                    break;
                case 2: //cases                 
                    this.OpenCaseDetails(row, 4);
                    break;
                case 3: //archived cases
                    this.OpenArchiveCaseDetails(row, 4);
                    break;
                case 5: //open and archived cases
                    this.OpenArchiveCaseDetails(row, 4);
                    break;
                case 4:
                    this.OpenMailDetails(row);
                    break;
            }
        }
    };
    this.ListDoubleClick = function (row) {
        if (this.clicktimeout) {
            window.clearTimeout(this.clicktimeout);
            this.clicktimeout = null;
            this.savPar = null;
        }
        if (!bINT(row))
            row = '0';
        this.DoRealAction(row);
    };
    this.OpenArchiveCaseDetails = function (row, mode) {
        this.OpenDetailWindow('dm_detail.aspx?RTCASE_CASE_ID=' + row + '&folderid=' + this.ParentID + '&archivedcaselist=' + this.ObjectList + '&fromarchive=Y&mode=' + mode, row);
    };
    this.SelectionToString = function (sel, row) {
        if (sel == "") {
            sel = this.ObjectList;
        }
        else {
            if (sel.indexOf(row + ";") < 0) {
                sel = row + ";" + sel;
            }
        }
        return sel;
    };

    this.OpenDetailWindow = function (url, row, openmodal) {
        if (openmodal == undefined) {
            openmodal = this.OpenDetailWindowsModal;
        }
        PC().OpenDetailWindow(url, row, openmodal, clientID);
    };
    this.OpenCaseDetails = function (row, mode,openmodal) {
        var sel = this.SelectionToString(this.GetCaseSelection(), row);

        this.OpenDetailWindow('dm_detail.aspx?RTCASE_TECH_ID=' + row + '&folderid=' + this.ParentID + '&caselist=' + sel + '&fromarchive=N&mode=' + mode, row, openmodal);
    };
    this.OpenMailDetails = function (row, openmodal) {
        this.OpenDetailWindow('./MailClient/View.aspx?DM_MAIL_ID=' + row + '&maillist=' + this.ObjectList, row, openmodal);
    };

    this.OpenObjectDetails = function (row, mode, openmodal) {
        if (row) {
            var r = $get(row);
            var sel = this.SelectionToString(this.GetSelection(), row);
            if (r) {
                var t = r.getAttribute("object_type");
                if (t) {
                    if (t == "Shortcut") {
                        t = r.getAttribute("object_reference_type");
                    }
                }
                else {
                    t = "";
                }
                r = null;
                if (t == "Folder" || t == "Dossier") {
                    this.DoFolderAction(row, 'Explore'); // double click = explore         
                }
                else {
                    var arch = $get(row).getAttribute("archived");
                    if (!arch) arch = "";
                    this.OpenDetailWindow('dm_detail.aspx?DM_OBJECT_ID=' + row + '&folderid=' + this.ParentID + '&objectlist=' + sel + '&fromarchive=' + arch + '&mode=' + mode, row, openmodal);
                }
            }
            else {
                this.OpenDetailWindow('dm_detail.aspx?DM_OBJECT_ID=' + row + '&folderid=' + this.ParentID + '&objectlist=' + sel + '&mode=' + mode, row, openmodal);
            }
        }
    };
    this.DoListSingleClick = function () {
        this.DoPreviewAction(this.savPar, false);
        this.savPar = null;
        this.clicktimeout = null;
    };
    this.DoRowAction = function (id) {
        if (id != "" && id != "0") {
            if (this.clicktimeout && this.savPar == id) {

                window.clearTimeout(this.clicktimeout);
                this.clicktimeout = null;
                this.savPar = null;
            }
            else {
                this.savPar = id;
                this.clicktimeout = setTimeout(delegate(this, this.DoListSingleClick), 250);
            }
        }
    };


    this.RowMouseClick = function (e) {

        srcElem = this.GetMainRowElement(e, true);
        if (srcElem === null || srcElem.id === '' || srcElem.rowIndex < 0) { return; }  

        if (this.SelectedRow !== -1 && this.SelectedRow !== srcElem) {
            $("#" + this.SelectedRow.id).removeClass('listContentHighlighted');
            this.SelectedRow = srcElem;
        }

        srcElem.className += ' listContentHighlighted';
        this.SelectedRow = srcElem;        
        this.DoRowAction(srcElem.id);

    };
    this.GetMainRowElementID = function (e) {
        srcElem = this.GetMainRowElement(e, false);

        if (srcElem == null) return "0";
        if (srcElem.id == '') return "0";
        if (srcElem.rowIndex < 0) return "0";
        return srcElem.id;
    };
    this.GetMailBoxID = function (e) {
        var el = !PC().IsIE ? e.target : e.srcElement;
        while (el.tagName != "TR" && el.tagName != "BODY") {
            el = PC().IsIE ? el.parentElement : el.parentNode;
            if (el.tagName == "TR") {
                if ((!el.id) || (el.id == ""))
                    el = PC().IsIE ? el.parentElement : el.parentNode;
            }
        }
        if (el.tagName == "BODY") {
            return "0";
        }
        else {
            var attr = el.getAttribute("mailbox");
            if (attr) {
                return attr;
            }
            else {
                return "0";
            }
        }
    };
    this.GetRowID = function (e) {
        var el = (!PC().IsIE ? e.target : e.srcElement);
        while (el.tagName != "TR" && el.tagName != "BODY") {
            el = PC().IsIE ? el.parentElement : el.parentNode;
            if (el.tagName == "TR") {
                if ((!el.id) || (el.id == ""))
                    el = PC().IsIE ? el.parentElement : el.parentNode;
            }
        }
        if (el.tagName == "BODY") {
            return "0";
        }
        else {
            return el.id;
        }
    };

    this.RowMouseOut = function (e) {
        // if (this.currentrow == null) this.HiliteRow(-1);

        if (this.hoveredrow == null) {
            var currentCellOverLay = $get('selectedCellOverLay');
            if (currentCellOverLay != null) {
                var cell = currentCellOverLay.parentNode;
                cell.removeChild(currentCellOverLay);
            }
        }
    };

    this.RowMouseOver = function (e) {
        this.SetAsCurrentGrid();
        srcElem = this.GetMainRowElement(e, false);

        this.hoveredrow = srcElem;

        //if (srcElem == null) return;
        //if (srcElem.rowIndex >= 0)
        //    this.HiliteRow(srcElem);
        //else
        //    this.HiliteRow(-1);

        if (this.EnableContextMenu) {

            var currcell = this.GetCurrentCell(e);
            if (currcell != null) {
                if (currcell.className == 'ctxcell') {
                    var currentCellOverLay = $get('selectedCellOverLay');
                    if (currentCellOverLay != null) {
                        if (this.hoveredcell != currcell) {
                            var cell = currentCellOverLay.parentNode;
                            cell.removeChild(currentCellOverLay);
                            currentCellOverLay = null;
                        }
                    }
                    this.hoveredcell = currcell;
                    if (currentCellOverLay == null) {
                        var func = "";
                        switch (this.ResultType) {
                            case 0: //objects
                            case 6:
                            case 7:
                                func = "PC().GetCurrentResultGrid().RowContextMenu(event)";
                                break;
                            case 1: //files
                                func = "PC().GetCurrentResultGrid().RowFileContextMenu(event)";
                                break;
                            case 2: //case
                                func = "PC().GetCurrentResultGrid().RowCaseContextMenu(event)"; // "RowCaseContextMenu";
                                break;
                            case 4: //mails
                                func = "PC().GetCurrentResultGrid().RowMailContextMenu(event)";
                                break;
                            case 3: //archive
                                func = "PC().GetCurrentResultGrid().RowArchiveCaseContextMenu(event)";
                                break;
                            case 5: //open and archive
                                func = "PC().GetCurrentResultGrid().RowArchiveOrOpenCaseContextMenu(event)";
                                break;
                        }

                        var link = document.createElement('div');
                        var x = PC().getX(currcell) + currcell.offsetWidth - 22;
                        var y = PC().getY(currcell);
                        link.innerHTML = '<div onclick="' + func + ';" id="selectedCellOverLay" style="padding:0px; margin:0px; border: 0px solid; text-align:right;line-height:' + currcell.offsetHeight + 'px; height:' + currcell.offsetHeight + 'px; width: 22px; text-align:center; display:block; position: absolute; top: ' + y + 'px; left: ' + x + 'px;  "  >' +
                            '<span class="icon-uncollapse"></span></div>';
                        currcell.appendChild(link);
                    }
                }
            }
        }

    };

    this.currentrow = null;
    this.hoveredrow = null;
    this.hoveredcell = null;

    this.SetAsCurrentGrid = function () {
        PC().SetCurrentResultGrid(this.clientID);
    };
    this.ShowContextMenu = function (e, row) {


        if (!ValidID(row)) return;

        var oldrow = this.currentrow;
        this.currentrow = null;
        this.RowMouseOut();

        this.SetAsCurrentGrid();

        PC().ShowContextMenu(e, row, this.ContextMenuFile, delegate(this, PC().LoadContextItems));

        this.currentrow = row;
        this.RowMouseOver(e);

    };
    this.HeaderContextMenu = function () {
        var e = window.event || arguments.callee.caller.arguments[0];
        var menu = $find(this.HeaderMenu);
        menu.show(e);
        e.cancelBubble = true;
        e.returnValue = false;

        if (e.stopPropagation) {
            e.stopPropagation();
            e.preventDefault();
        }
    };
    this.ShowFileContextMenu = function (e, row) {
        if (!ValidID(row)) return;
        PC().ShowContextMenu(e, row, this.ContextMenuFile, delegate(this, PC().LoadFileContextItems));
    };
    this.ShowObjectFileContextMenu = function (e, row) {
        if (!ValidID(row)) return;
        PC().ShowContextMenu(e, row, this.ContextMenuFile, delegate(this, PC().LoadObjectFileContextItems));
    };
    this.ShowCaseContextMenu = function (e, row) {
        if (!ValidID(row)) return;
        PC().ShowContextMenu(e, row, this.ContextMenuFile, delegate(this, PC().LoadCaseContextItems));
    };
    this.ShowArchiveCaseContextMenu = function (e, row) {
        if (!ValidID(row)) return;
        PC().ShowContextMenu(e, row, this.ContextMenuFile, delegate(this, PC().LoadArchiveCaseContextItems));
    };
    this.ShowArchiveOrOpenCaseContextMenu = function (e, row) {
        if (!ValidID(row)) return;
        PC().ShowContextMenu(e, row, this.ContextMenuFile, delegate(this, PC().LoadArchiveOrOpenCaseContextItems));
    };
    this.ShowMailContextMenu = function (e, row, box) {
        if (!ValidID(row)) return;
        PC().ShowContextMenu(e, row, this.ContextMenuFile, delegate(this, PC().LoadMailContextItems), box);
    };
    this.ShowMailFileContextMenu = function (e, row) {
        if (!ValidID(row)) return;
        PC().ShowContextMenu(e, row, this.ContextMenuFile, delegate(this, PC().LoadMailFileContextItems));
    };
    this.RowMouseDblClick = function (e) {
        this.ListDoubleClick(this.GetMainRowElementID(e, true));
    };
    this.RowContextMenu = function (e) {
        this.ShowContextMenu(e, this.GetRowID(e));
    };
    this.RowFileContextMenu = function (e) {
        this.ShowObjectFileContextMenu(e, this.GetRowID(e));
    };
    this.RowCaseContextMenu = function (e) {
        this.ShowCaseContextMenu(e, this.GetRowID(e));
    };
    this.RowArchiveCaseContextMenu = function (e) {

        this.ShowArchiveCaseContextMenu(e, this.GetRowID(e));
    };
    this.RowArchiveOrOpenCaseContextMenu = function (e) {
        this.ShowArchiveOrOpenCaseContextMenu(e, this.GetRowID(e));
    };
    this.RowMailContextMenu = function (e) {
        this.ShowMailContextMenu(e, this.GetRowID(e), this.GetMailBoxID(e));
    };
    this.RowDragStart = function (e) {
        window.event.dataTransfer.setData("text", "");
        srcElem = this.GetMainRowElement(e, false);

        if (srcElem == null) return;
        if (srcElem.id == '') return;
        if (srcElem.rowIndex < 0) return;

        switch (this.ResultType) {
            case 0: //objects            
                window.event.dataTransfer.setData("text", "f" + srcElem.id);
                break;
        }
    };
    this.StartDragging = function (itm) {
        itm.dragDrop();
    };

    this.RowMouseMove = function (e) {

        srcElem = this.GetMainRowElement(e, false);

        if (srcElem == null) return;
        if (srcElem.id == '') return;
        if (srcElem.rowIndex < 0) return;

        if (this.ResultType == 0) {
            if (window.event) {
                if (window.event.button == 1) {
                    setTimeout(delegate(this, this.StartDragging, srcElem), 500);
                }
            }
        }
    };

    this.DetachTableEvents = function (oTable) {

        var newNode = oTable.cloneNode(true);
        oTable.parentNode.insertBefore(newNode, oTable);
        oTable.parentNode.removeChild(oTable);

    };
    this.AttachTableEvents = function (oTable) {
        oTable.addEventListener("dblclick", delegate(this, this.RowMouseDblClick), false);
        oTable.addEventListener("click", delegate(this, this.RowMouseClick), false);
        oTable.addEventListener("mouseover", delegate(this, this.RowMouseOver), false);
        oTable.addEventListener("mouseout", delegate(this, this.RowMouseOut), false);
        if (this.EnableContextMenu) {
            switch (this.ResultType) {
                case 0: //objects
                case 6:
                case 7:
                    oTable.addEventListener("contextmenu", delegate(this, this.RowContextMenu), false);
                    break;
                case 1: //files
                    oTable.addEventListener("contextmenu", delegate(this, this.RowFileContextMenu), false);
                    break;
                case 2: //case
                    oTable.addEventListener("contextmenu", delegate(this, this.RowCaseContextMenu), false);
                    break;
                case 3: //archive
                    oTable.addEventListener("contextmenu", delegate(this, this.RowArchiveCaseContextMenu), false);
                    break;
                case 5: //open and archive
                    oTable.addEventListener("contextmenu", delegate(this, this.RowArchiveOrOpenCaseContextMenu), false);
                    break;
                case 4: //mail
                    oTable.addEventListener("contextmenu", delegate(this, this.RowMailContextMenu), false);
                    break;
            }
        }
    };
    this.CloseHandlers = function () {
        var oList;
        if (this.GridMode == 0) {
            //document list
            oList = this.GetControl("documentlist");
            if (oList) {
                this.DetachTableEvents(oList);
            }
        }
        else {
            //image list
            oList = this.GetControl("imageList");
            var oItems = oList.getElementsByTagName('table');
            for (j = 0; j < oItems.length; j++) {

                this.DetachTableEvents(oItems);
            }

        }
    };
    this.SetupHandlers = function () {

        var oList;
        if (this.GridMode == 0) {
            //document list
            oList = this.GetControl("documentlist");
            if (oList) {
                this.AttachTableEvents(oList);
                this.attachFilterEvents();
            }
        }
        else {
            //image list
            oList = this.GetControl("imageList");
            var oItems = oList.getElementsByClassName('List');
            for (j = 0; j < oItems.length; j++) {

                this.AttachTableEvents(oItems[j]);
            }
        }
    };

    this.clearAllColumnFilters = function () {
        const filterCells = this.GetControl("FilterRow").children;
        for (var i = 0; i < filterCells.length; i++) {
            const filterControl = filterCells[i].firstChild;
            if (filterControl) {
                let filterId = filterControl.id.slice(0, -10);
                window[filterId]().clearFilter(false);
            }
        }
        this.Reload();
    };

    this.SetCriteriaXml = function (xml) {
        this.GetControl("critfield").value = xml;
    }

    this.FocusFirstField = function () {
        if (!postback && theForm) {
            if (theForm.elements[0] != null) {
                var i;
                var max = theForm.length;
                for (i = 0; i < max; i++) {
                    if (!theForm.elements[i].disabled &&
                        !theForm.elements[i].readOnly &&
                        (theForm.elements[i].type == "text")) {
                        try {
                            theForm.elements[i].focus();
                            break;
                        }
                        catch (er) {

                        }
                    }
                }
            }
        }
    };
    this.onLoad = function (sender, args) {

        this.SetupHandlers();

        this.ToggleToolbarSelectionButtons();
        this.FocusFirstField();

        PC().AddResultGrid(this);

    };
    this.onReload = function () {
        this.CloseHandlers();
        this.onLoad();
    }

    this.attachFilterEvents = function () {
        const filterRow = this.GetControl('FilterRow');
        if (filterRow) {
            const filterCells = this.GetControl('FilterRow').children;
            for (var i = 0; i < filterCells.length; i++) {
                const filterControl = filterCells[i].firstChild;
                if (filterControl && filterControl.dataset) {
                    window['bind_' + filterControl.dataset.id]();
                }
            }
        }
    }


    this.ToolbarSelectionButtons = new Array();
    this.ToolbarCaseSelectionButtons = new Array();
    this.ToolbarMailSelectionButtons = new Array();
    this.clientID = clientID;
    this.ToolbarClientID = tlbclientID;
    this.Visible = visible;
    this.LastPage = lastpage;
    this.GridMode = GridMode;
    this.ListRows = ListRows;
    this.ObjectList = objectlist;
    this.FromCaseID = linkedtocaseid;
    this.FromObjectID = linkedtoobjectid;
    this.PackID = packid;
    this.ParentID = parentid;
    this.CategoryID = catid;
    this.RootID = rootid;
    this.ResultType = resulttype;
    this.EnableContextMenu = enablecontextmenu;
    this.OpenDetailWindowsModal = opendetailwindowsmodal;
    this.hlColor = "#f0fff0";  //'#EFEFEF';
    this.hlBorder = "1px solid #1390b4";
    this.currHlRow = -1;
    this.SelectedRow = -1;
    this.oldRowColor = '';
    this.savPar = '';
    this.clicktimeout = null;
    this.TermsList = termslist;
    this.HeaderMenu = headermenu;
    this.XmlFile = xmlfile;
    this.DefaultXmlFile = defaultxmlfile;
    this.ResultScreenID = resultscreenid;
    this.Ajax = ajaxenabled;
    this.AjaxButton = ajaxcontrol;
    this.CaseSelectionType = caseseltype;
    this.ContextMenuFile = contextmenufile;

}

var formDropdownController = function (container, dropdown) {

    this.selected;
    this.$container = $(container);
    this.$dropdown = $(dropdown);
    this.$dropdownitems = this.$dropdown.find('.form-dropdown-item');

    this.zIndexParentCell;

    this.openDropdown = function () {
        if (!this.$dropdown.hasClass('select')) {
            this.$dropdown.addClass('select');
            setTimeout($.proxy(this.bindClose, this), 200);
            this.zIndexParentCell = parseInt(this.$container.parent().css("z-index"));
            this.$container.parent().css("z-index", this.zIndexParentCell + 1);
        }
    };

    this.closeDropdown = function () {
        this.$dropdown.removeClass('select');
        this.$container.parent().css("z-index", this.zIndexParentCell);
        $('body').off('click.formdropdown');
        this.onClose();
    };

    this.onClose = function () {

    };

    this.bindClose = function () {
        $('body').on('click.formdropdown', $.proxy(function (e) {
            const $calendarPopup = $('.RadCalendarPopup').first();
            if (!this.$dropdown.is(e.target) && this.$dropdown.has(e.target).length === 0) {
                if (!$calendarPopup.is(e.target) && $calendarPopup.has(e.target).length === 0) {
                    this.closeDropdown();
                }
            }
        }, this));
    };

    this.makeSelection = function (clickedElement) {
        this.selected = clickedElement;
        this.$dropdownitems.removeClass('selected');
        this.selected.classList.add('selected');

        return this.selected.dataset.value;
    };

    this.openDropdownExpansion = function () {
        const spaceLeft = this.$dropdown.offset().left;
        const spaceRight = $(window).width() - spaceLeft - this.$dropdown.width();
        if (spaceLeft > spaceRight) {
            this.$dropdown.addClass('expand-left');
        } else {
            this.$dropdown.addClass('expand-right');
        }
    };

    this.closeDropdownExpansion = function () {
        this.$dropdown.removeClass('expand-right');
        this.$dropdown.removeClass('expand-left');
    };

    this.selectTab = function (tabGroup) {
        this.$dropdown.find('.tabbable').removeClass('active');
        this.$dropdown.find('.tab').removeClass('active');
        this.$dropdown.find(".tabbable[data-tab-group*='" + tabGroup + "']").addClass('active');
        this.$dropdown.find(".tab[data-tab='" + tabGroup + "']").addClass('active');
    };

    this.moveSelection = function (direction) {
        const $currentSelected = this.$dropdownitems.filter('.selected').first();
        if ($currentSelected.length == 0) {
            this.$dropdownitems.first().addClass('selected');
        }
        let $newSelected;
        if (direction === "down") {
            $newSelected = $currentSelected.nextAll(":not('.hidden'):first");
        }
        if (direction === "up") {
            $newSelected = $currentSelected.prevAll(":not('.hidden'):first");
        }
        if ($newSelected.length != 0) {
            this.closeDropdownExpansion();
            $currentSelected.removeClass('selected');
            $newSelected.addClass('selected');
        }
    };
};

var datefilterController = function (container, dropdown, hiddenValue, visibleValue, errorMessage, errorMessages, startDateCtr, endDateCtr, exactDateCtr, predefinedValues, gridID, dateFormat) {

    formDropdownController.call(this, container, dropdown);

    this.open = function () {
        this.openDropdown();
        if (this.$dropdown.children('.selected').data('value') === 'custom') {
            this.openDropdownExpansion();
        }
    };

    this.close = function () {
        this.closeDropdown();
        errorMessage.style.visibility = 'hidden';
    }

    this.select = function (clickedElement) {
        const selection = this.makeSelection(clickedElement);
        if (selection === 'custom') {
            this.openDropdownExpansion();
            return;
        }
        this.closeDropdownExpansion();
        this.setNewValue(selection);
    };

    this.setNewValue = function (selection) {
        const startDate = this.convertToMoment(startDateCtr.value);
        const endDate = this.convertToMoment(endDateCtr.value);
        const exactDate = this.convertToMoment(exactDateCtr.value);

        if (selection === 'range') {
            this.selected = this.$dropdown.children("[data-value='custom']")[0];
            if (!startDateCtr.value && !endDateCtr.value) {
                return;
            }
            if (!startDateCtr.value) {
                if (!endDate.isValid()) {
                    return;
                }
                hiddenValue.value = '<' + endDate.format(dateFormat);
                visibleValue.value = '< ' + endDate.format(dateFormat);
            } else if (!endDateCtr.value) {
                if (!startDate.isValid()) {
                    return;
                }
                hiddenValue.value = '>' + startDate.format(dateFormat);
                visibleValue.value = '> ' + startDate.format(dateFormat);
            } else {
                if (this.checkDateRangeValid()) {
                    hiddenValue.value = startDate.format(dateFormat) + ',' + endDate.format(dateFormat);
                    visibleValue.value = this.selected.innerText;
                } else {
                    return;
                }
            }
        } else if (selection === 'exact') {
            if (!exactDate.isValid()) {
                return;
            }
            hiddenValue.value = exactDate.format(dateFormat);
            visibleValue.value = exactDate.format(dateFormat);
        } else {
            hiddenValue.value = predefinedValues[selection];
            visibleValue.value = this.selected.innerText;
        }
        this.close();
        eval(gridID).Reload();

    };

    this.checkDateRangeValid = function () {
        const startDate = this.convertToMoment(startDateCtr.value);
        const endDate = this.convertToMoment(endDateCtr.value);

        if (!startDate.isValid()) {
            return false;
        }
        if (!endDate.isValid()) {
            return false;
        }

        if (endDate.isBefore(startDate)) {
            this.setErrorMessage(errorMessages.fromtodate);
            return false;
        } else {
            this.setErrorMessage('');
            return true;
        }
    };

    this.checkDateValid = function (dateCtrs) {
        let datesAreValid = true;
        for (var i = 0; i < dateCtrs.length; i++) {
            if (dateCtrs[i].value) {
                const date = this.convertToMoment(dateCtrs[i].value);
                if (!date.isValid()) {
                    dateCtrs[i].classList.add('invalidField');
                    datesAreValid = false;
                    continue;
                }
            }
            dateCtrs[i].classList.remove('invalidField');
        }
        if (datesAreValid) {
            this.setErrorMessage('');
        } else {
            this.setErrorMessage(errorMessages.invaliddate);
        }
    };

    this.convertToMoment = function (value) {
        return moment(value.trim(), dateFormat);
    };

    this.userIsTyping = function () {
        const textboxValue = visibleValue.value.trim().replace('"', '');
        if (!textboxValue) {
            hiddenValue.value = '';
            this.$dropdownitems.removeClass('selected');
            return;
        }
        const prediction = this.$dropdownitems.filter(':contains("' + textboxValue + '")');
        if (prediction.length != 0) {
            const selection = this.makeSelection(prediction[0]);
            if (selection != 'custom') {
                hiddenValue.value = predefinedValues[selection];
            }
            return;
        }
        this.$dropdownitems.removeClass('selected');
        if (textboxValue.length >= 6) {
            let date;
            if (textboxValue.includes(',')) {
                const splittedValues = textboxValue.split(',');
                const date1 = moment(splittedValues[0].trim(), dateFormat);
                if (date1.isValid()) {
                    let date2;
                    if (splittedValues.length > 1) {
                        date2 = moment(splittedValues[1].trim(), dateFormat);
                    }
                    if (!date2 || !date2.isValid()) {
                        hiddenValue.value = date1.format(dateFormat);
                        return;
                    }
                    hiddenValue.value = date1.format(dateFormat) + ',' + date2.format(dateFormat);
                }
                return;
            }
            if (textboxValue.startsWith('<') || textboxValue.startsWith('>')) {
                const symbol = textboxValue.slice(0, 1);
                date = moment(textboxValue.slice(1), dateFormat);
                if (date.isValid()) {
                    hiddenValue.value = symbol + date.format(dateFormat);
                }
                return;
            }
            date = moment(textboxValue, dateFormat);
            if (date.isValid()) {
                hiddenValue.value = date.format(dateFormat);
                return;
            }
        }
        hiddenValue.value = '';
        this.$dropdownitems.removeClass('selected');
    }

    this.setErrorMessage = function (message) {
        errorMessage.innerText = message;
    };

    this.clearFilter = function (reload) {
        hiddenValue.value = '';
        visibleValue.value = '';
        if (reload) {
            eval(gridID).Reload();
        }
    };
}

datefilterController.prototype = Object.create(formDropdownController.prototype);
datefilterController.prototype.constructor = datefilterController;

var textfilterController = function (container, dropdown, hiddenValue, visibleValue, symbolButton, gridID) {

    formDropdownController.call(this, container, dropdown);

    this.open = function () {
        this.openDropdown();
    };

    this.select = function (clickedElement) {
        const selection = this.makeSelection(clickedElement);
        this.setNewSymbol(selection);
    };

    this.setNewSymbol = function (selection) {
        symbolButton.innerHTML = selection;
        this.closeDropdown();
        this.setValue();
    };

    this.setValue = function () {

        const symbol = symbolButton.innerText;
        const searchString = visibleValue.value;
        if (searchString.length > 0) {
            if (searchString.indexOf('*') < 0 && searchString.indexOf('%') < 0) {
                switch (symbol) {
                    case '=':
                        hiddenValue.value = '="' + searchString + '"';
                        break;
                    case '[':
                        hiddenValue.value = searchString + '*';
                        break;
                    case '%':
                        hiddenValue.value = '*' + searchString + '*';
                        break;
                    default:
                        hiddenValue.value = searchString;
                }
            }
            else {
                hiddenValue.value = searchString;
            }
        }
        else {
            hiddenValue.value = "";
        }
    };

    this.clearFilter = function (reload) {
        hiddenValue.value = '';
        visibleValue.value = '';
        if (reload) {
            eval(gridID).Reload();
        }
    };

};

textfilterController.prototype = Object.create(formDropdownController.prototype);
textfilterController.prototype.constructor = textfilterController;

var numberfilterController = function (container, dropdown, hiddenValue, visibleValue, symbolButton, gridID, minRange, maxRange, decimalSeparator) {

    formDropdownController.call(this, container, dropdown);

    this.open = function () {
        this.openDropdown();
        if (this.$dropdown.children('.selected').data('value') === '><') {
            this.openDropdownExpansion();
        }
    };

    this.select = function (clickedElement) {
        const selection = this.makeSelection(clickedElement)
        this.setNewSymbol(selection);
    };

    this.setNewSymbol = function (selection) {
        if (selection === '><') {
            this.openDropdownExpansion();
            return;
        }
        symbolButton.innerHTML = selection;
        this.closeDropdown();
        this.setValue(false);
    };

    this.setValue = function () {
        let symbol = symbolButton.innerText;
        let textboxValue = visibleValue.value;
        if (textboxValue.length > 0) {
            if (textboxValue.includes(' , ')) {
                symbol = '><';
                textboxValue = textboxValue.replace(' , ', ' | ');
            }
            const validNumber = this.getValidNumberAsString(textboxValue);

            switch (symbol) {
                case '<':
                    hiddenValue.value = '<' + validNumber;
                    break;
                case '>':
                    hiddenValue.value = '>' + validNumber;
                    break;
                case '><':
                    const numbers = textboxValue.split(' | ');
                    if (numbers.length <= 1) {
                        hiddenValue.value = validNumber;
                        symbolButton.innerHTML = '=';
                    } else {
                        const minValue = this.getValidNumberAsString(numbers[0]);
                        const maxValue = this.getValidNumberAsString(numbers[1]);
                        if (!minValue && !maxValue) {
                            hiddenValue.value = '';
                        } else if (!minValue) {
                            hiddenValue.value = '<' + maxValue;
                        } else if (!maxValue) {
                            hiddenValue.value = '>' + minValue;
                        } else {
                            hiddenValue.value = minValue + ' , ' + maxValue;
                        }
                    }
                    break;
                default:
                    hiddenValue.value = validNumber;
            }
        }
        else {
            hiddenValue.value = "";
        }
    };

    this.setRange = function () {
        let minValue = this.getValidNumberAsString(minRange.value);
        let maxValue = this.getValidNumberAsString(maxRange.value);
        if (!minValue && !maxValue) {
            return;
        } else if (!minValue) {
            symbolButton.innerHTML = '<';
            visibleValue.value = maxValue;
            hiddenValue.value = '<' + maxValue;
        } else if (!maxValue) {
            symbolButton.innerHTML = '>';
            visibleValue.value = minValue;
            hiddenValue.value = '>' + minValue;
        } else {
            symbolButton.innerHTML = '><';
            visibleValue.value = minValue + " | " + maxValue;
            hiddenValue.value = minValue + ' , ' + maxValue;
        }
        this.closeDropdown();
        eval(gridID).Reload();
    }

    this.getValidNumberAsString = function (value) {
        let parsedValue = parseFloat(value.replace(',', '.'));
        if (isNaN(parsedValue)) {
            parsedValue = '';
        }
        return parsedValue.toString().replace('.', decimalSeparator);
    };

    this.clearFilter = function (reload) {
        visibleValue.value = '';
        symbolButton.innerHTML = '=';
        hiddenValue.value = '';
        if (reload) {
            eval(gridID).Reload();
        }
    };

};

numberfilterController.prototype = Object.create(formDropdownController.prototype);
numberfilterController.prototype.constructor = numberfilterController;

jQuery.expr[':'].contains = function (a, i, m) {
    return jQuery(a).text().toUpperCase().indexOf(m[3].toUpperCase()) >= 0;
};

var selectionfilterController = function (container, dropdown, hiddenValue, visibleValue, listFilter, gridID, emptyValue, itemsLabel) {

    formDropdownController.call(this, container, dropdown);

    this.open = function () {
        this.openDropdown();
        if (visibleValue.value) {
            visibleValue.select();
        }
        if (listFilter) {
            listFilter.focus();
        }
    };

    this.onClose = function () {
        if (hiddenValue.value.includes(',')) {
            visibleValue.innerText = hiddenValue.value.split(',').length + ' ' + itemsLabel;
        } else {
            visibleValue.innerText = this.$dropdownitems.filter('.selected').first().text().trim();
        }
    };

    this.select = function (clickedElement) {
        if (clickedElement) {
            const selection = this.makeSelection(clickedElement);
            hiddenValue.value = selection;
            visibleValue.innerHtml = this.selected.innerHtml;
            eval(gridID).Reload();
        }
    };

    this.multiSelect = function (clickedElement) {
        if (clickedElement) {
            const selection = clickedElement.dataset.value;
            const checkbox = clickedElement.getElementsByTagName('input').item(0);
            if (clickedElement.classList.contains('selected')) {
                clickedElement.classList.remove('selected');
                hiddenValue.value = this.removeValue(hiddenValue.value, selection);
                checkbox.checked = false;
            } else {
                clickedElement.classList.add('selected');
                hiddenValue.value = this.addValue(hiddenValue.value, selection);
                checkbox.checked = true;
            }
        }
    };

    this.addValue = function (currentValue, value) {
        if (currentValue) {
            return currentValue + ',' + value;
        }
        return value;
    };

    this.removeValue = function (currentValue, value) {
        separator = ',';
        var values = currentValue.split(separator);
        for (var i = 0; i < values.length; i++) {
            if (values[i] == value) {
                values.splice(i, 1);
                return values.join(separator);
            }
        }
        return currentValue;
    };

    this.search = function () {
        const searchString = listFilter.value;
        this.$dropdownitems.removeClass('first-visible last-visible');
        const $searchResults = this.$dropdownitems.filter(':contains("' + searchString + '")');
        $searchResults.removeClass('hidden');
        $searchResults.first().addClass('first-visible');
        $searchResults.last().addClass('last-visible');
        this.$dropdownitems.not(':contains("' + searchString + '")').addClass('hidden');
        this.$dropdownitems.filter('.hidden').each($.proxy(function (i, element) {
            element.classList.remove('selected');
            const checkbox = element.getElementsByTagName('input').item(0);
            if (checkbox) {
                checkbox.checked = false;
            }
            hiddenValue.value = this.removeValue(hiddenValue.value, element.dataset.value);
        }, this));
    };

    this.confirm = function () {
        eval(gridID).Reload();
    };

    this.selectAllOrNothing = function (element) {
        const masterCheckbox = element.getElementsByTagName('input').item(0);
        masterCheckbox.checked = !masterCheckbox.checked;
        if (masterCheckbox.checked) {
            hiddenValue.value = '';
            this.$dropdownitems.not('.hidden').each($.proxy(function (i, element) {
                element.classList.add('selected');
                const checkbox = element.getElementsByTagName('input').item(0);
                checkbox.checked = true;
                hiddenValue.value = this.addValue(hiddenValue.value, element.dataset.value);
            }, this));
        } else {
            this.$dropdownitems.each($.proxy(function (i, element) {
                element.classList.remove('selected');
                const checkbox = element.getElementsByTagName('input').item(0);
                checkbox.checked = false;
            }, this));
            hiddenValue.value = '';
        }
    };

    this.clearFilter = function (reload) {
        visibleValue.value = '';
        hiddenValue.value = emptyValue;
        if (reload) {
            eval(gridID).Reload();
        }
    };

};

selectionfilterController.prototype = Object.create(formDropdownController.prototype);
selectionfilterController.prototype.constructor = selectionfilterController;


//// BEGIN OLD CODE ///////


function handleDragStart(id, e) {    
    if (!e) e = window.event;
    //var dragSrcEl = (window.event) ? window.event.srcElement /* for IE */ : event.target;    
    e.dataTransfer.setData("text", id);
}
function handleDragEnter(e) {
    cancelEvent(e);
}
function handleDragLeave(e) {
    cancelEvent(e);
}

function drop() {
    var src, trgID, srcID;
    src = window.event.srcElement;
    while (src.tagName != "TR") { src = src.parentNode; }
    trgID = src.id;
    srcID = window.event.dataTransfer.getData("text");
    if ((trgID && srcID) && (trgID != srcID)) {
        PC().SetOIP(trgID);
        srcID = srcID.substring(1, srcID.length);
        PC().LinkObjectsToOIP(srcID, 1); // todo : get relation type	  
    }
}



function IsNumeric(sText) {
    var ValidChars = "0123456789.";
    var IsNumber = true;
    var Char;


    for (i = 0; i < sText.length && IsNumber == true; i++) {
        Char = sText.charAt(i);
        if (ValidChars.indexOf(Char) == -1) {
            IsNumber = false;
        }
    }
    return IsNumber;

}


function bINT(sText) {
    if (isNaN(parseInt(sText)))
        return false;
    return true;
}

function ValidID(i) {
    if (i == '0')
        return false;

    if (!bINT(i))
        return false;
    return true;
}





