function createCopy() {
    var url = controllerName + "/CreateCopy?Id=" + gvPriceDocuments.GetRowKey(gvPriceDocuments.GetFocusedRowIndex());
    $.post(url, function (data) {
    }).success(function () {
        gvPriceDocuments.PerformCallback();
    });
}
function changeState(state) {
    var url = controllerName + "/ChangeState?Id=" + gvPriceDocuments.GetRowKey(gvPriceDocuments.GetFocusedRowIndex()) + "&state=" + state.toString();
    $.post(url, function (data) {
    }).success(function () {
        gvPriceDocuments.PerformCallback();
    });
}
function ShowBrowseTemplate() {
    //var a = ASPxClientControl.GetControlCollection().GetByName('SelectDocumentTemplate');
    if (typeof SelectDocumentTemplate === "undefined") {
            $.ajax({
                type: "POST",
                url: viewListControllerName + "/SelectDocumentTemplate",
                success: function (response) {
                    $("#popupDiv").html(response);
                    SelectDocumentTemplate.ShowAtElementByID('popupDiv');
                }
            });
        }
        else {
            SelectDocumentTemplate.ShowAtElementByID('popupDiv');
        }
}
function OnMenuItemClick(s, e) {
    switch (e.item.name) {
        case "mnuEdit":
            var id = gvPriceDocuments.GetRowKey(gvPriceDocuments.GetFocusedRowIndex());
            if (id == null)
                break;
            window.open(controllerName + '/Edit/' + id, "_blank");
            break;
        case "mnuDelete":
            if (gvPriceDocuments.GetFocusedRowIndex() != 0 && confirm("Удалить?")) {
                var url = controllerName + "/Delete?Id=" + gvPriceDocuments.GetRowKey(gvPriceDocuments.GetFocusedRowIndex());
                $.post(url, function (data) {
                }).success(function () {
                    gvPriceDocuments.PerformCallback();
                });
            }
            break;
        case "mnuStateActive":
            changeState(1);
            break;
        case "mnuStateNeedCorrect":
            changeState(2);
            break;
        case "mnuStateBan":
            changeState(4);
            break;
    }
}