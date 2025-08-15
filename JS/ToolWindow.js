function GetRadWindow() {
    var oWindow = null;
    if (window.radWindow) {
        oWindow = window.radWindow;
    }
    else if (window.frameElement && window.frameElement.radWindow) {
        oWindow = window.frameElement.radWindow;
    }

    return oWindow;
}

function GetContentWindow() {
    return window;
}

function RefreshOpener() {
    if (MainPage().PC) {
        MainPage().PC().CascadeReloadContent();
    }
}