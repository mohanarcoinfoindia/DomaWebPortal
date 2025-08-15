function RefreshOnChange() {
    RefreshObject();
}
function UncheckedSave() {
    SaveObject(); // do RefreshObject here?
}
function UnlockButton() { if (ValidateForm('')) { Unlock(); } }
function ReleaseButton() { if (ValidateForm('CheckMandatoryFields')) { Release(); } }
function RejectButton() { if (ValidateForm('')) { Reject(); } }
function SaveKeepOpenButton() { if (ValidateForm('')) { SaveObject(); } }
function SaveAndCloseButton() { if (ValidateForm('CheckMandatoryFields')) { SaveObjectAndClose(); } }
function SaveAndCloseWindowButton() { if (ValidateForm('CheckMandatoryFields')) { SaveObjectAndCloseWindow(); } }

function ForceValidate() {
    try {
        Page_ClientValidate("CheckMandatoryFields");
    }
    catch (e) {
    }
}

function ValidateForm(grp) {
    Page_ClientValidate(grp);
    return Page_IsValid; 
}

function GetSelectedTab() {
    try {
        if (document.forms[0].DM_TABVIEW_SELECTION_0) {
            return document.forms[0].DM_TABVIEW_SELECTION_0.value;
        }
        else {
            return 0;
        }
    }
    catch (e) {
        return 0;
    }
}

function HasPageValidators() {
    try {
        if (Page_Validators.length > 0) {
            return true;
        }
    }
    catch (err) { }
    return false;
}

function ValidationGroupEnable(validationGroupName, isEnable) {
    if (HasPageValidators()) {
        for (i = 0; i < Page_Validators.length; i++) {
            if (Page_Validators[i].validationGroup == validationGroupName) {
                ValidatorEnable(Page_Validators[i], isEnable);
            }
        }
    }
}

var alarmWindow = null;
function DisableWindow() {
    alarmWindow = document.body.appendChild(document.createElement("div"));
    alarmWindow.id = "disableWindow";
    alarmWindow.style.height = document.documentElement.scrollHeight + "px";
    alarmWindow.setAttribute("unselectable", "on");
}
function EnableWindow() {
    if (alarmWindow) {
        document.body.removeChild(alarmWindow);
    }
    alarmWindow = null;
}