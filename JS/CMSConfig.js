function SelectFolder(element) {
    const folderIdElement = $(element).prev()[0];
    this.PC().OpenModalWindow('../DM_Selectfolder.aspx?valuef=' + folderIdElement.id + '&goto=' + folderIdElement.value + '&reload=Y', false, 600, 400, false);
}

function LinkControls(condition, paramGroups, controls) {
    Object.keys(controls).forEach(function (key) {
        controls[key].removeClass('linked');
    });

    if (paramGroups[condition]) {
        paramGroups[condition].forEach(function (linkedField) {
            controls[linkedField].addClass('linked');
        });
    }
}
function bindActionButtons () {
    $('.multi-items-table').on('click', "a[id$='MoveUp']", function () {
        let row = $(this).closest('tr');
        let height = row.css('height');
        row.addClass('highlighted');
        row.animate({ 'bottom': height });
        row.prev().animate({ 'top': height });
    });

    $('.multi-items-table').on('click', "a[id$='MoveDown']", function () {
        let row = $(this).closest('tr');
        let height = row.css('height');
        row.addClass('highlighted');
        row.animate({ 'top': height });
        row.next().animate({ 'bottom': height });
    });
}

Sys.Application.add_load(bindActionButtons);

$(function () {
    //bugfix: when opening 'edit panel' when browserscreen was small 'ok' and 'cancel' button in modal didn't show and there was no horizontal scroll bar.
    //modal was not resizable.
    //simply adding horizontal scroll bar caused the scrollbarr to also show when it wasn't necessary. 
    if (resizeOnLoad) {
        const oWnd = GetRadWindow();
        setTimeout(function () {
            oWnd.autoSize(false);
            oWnd.SetWidth(oWnd.GetWidth() + 5);
            document.documentElement.setAttribute('style', 'overflow-x:auto;');
        }, 120);
    }
});
