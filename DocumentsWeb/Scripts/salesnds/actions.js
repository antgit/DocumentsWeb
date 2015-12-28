function createCopy() {
    var url = controllerName + "/CreateCopy?Id=" + gvSalesDocuments.GetRowKey(gvSalesDocuments.GetFocusedRowIndex());
    $.post(url, function (data) {
    }).success(function () {
        gvSalesDocuments.PerformCallback();
    });
}

function changeState(state) {
    var url = controllerName + "/ChangeState?Id=" + gvSalesDocuments.GetRowKey(gvSalesDocuments.GetFocusedRowIndex()) + "&state=" + state.toString();
    $.post(url, function (data) {
    }).success(function () {
        gvSalesDocuments.PerformCallback();
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
            var id = gvSalesDocuments.GetRowKey(gvSalesDocuments.GetFocusedRowIndex());
            if (id == null)
                break;
            window.open(controllerName + '/Edit/' + id, "_blank");
            break;
        case "mnuDelete":
            if (gvSalesDocuments.GetFocusedRowIndex() != 0 && confirm("Удалить?")) {
                var url = controllerName + "/Delete?Id=" + gvSalesDocuments.GetRowKey(gvSalesDocuments.GetFocusedRowIndex());
                $.post(url, function (data) {
                }).success(function () {
                    gvSalesDocuments.PerformCallback();
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