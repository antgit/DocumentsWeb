function createCopy() {
    var url = controllerName + "/CreateCopy?Id=" + gvFinancesDocuments.GetRowKey(gvFinancesDocuments.GetFocusedRowIndex());
    $.post(url, function (data) {
    }).success(function () {
        gvFinancesDocuments.PerformCallback();
    });
}
function changeState(state) {
    var url = controllerName + "/ChangeState?Id=" + gvFinancesDocuments.GetRowKey(gvFinancesDocuments.GetFocusedRowIndex()) + "&state=" + state.toString();
    $.post(url, function (data) {
    }).success(function () {
        gvFinancesDocuments.PerformCallback();
    });
}
function ShowBrowseTemplate() {
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
            var id = gvFinancesDocuments.GetRowKey(gvFinancesDocuments.GetFocusedRowIndex());
            if (id == null)
                break;
            window.open(controllerName + '/Edit/' + id, "_blank");
            break;
        case "mnuDelete":
            if (gvFinancesDocuments.GetFocusedRowIndex() != 0 && confirm("Удалить?")) {
                var url = controllerName + "/Delete?Id=" + gvFinancesDocuments.GetRowKey(gvFinancesDocuments.GetFocusedRowIndex());
                $.post(url, function (data) {
                }).success(function () {
                    gvFinancesDocuments.PerformCallback();
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