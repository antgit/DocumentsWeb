function createCopy() {
    var url = controllerName + "/CreateCopy?Id=" + gvDocumentConfigs.GetRowKey(gvDocumentConfigs.GetFocusedRowIndex());
    $.post(url, function (data) {
    }).success(function () {
        gvDocumentConfigs.PerformCallback();
    });
}

function changeState(state) {
    var url = controllerName + "/ChangeState?Id=" + gvDocumentConfigs.GetRowKey(gvDocumentConfigs.GetFocusedRowIndex()) + "&state=" + state.toString();
    $.post(url, function (data) {
    }).success(function () {
        gvDocumentConfigs.PerformCallback();
    });
}

function OnMenuItemClick(s, e) {
    switch (e.item.name) {
        case "mnuEdit":
            var id = gvDocumentConfigs.GetRowKey(gvDocumentConfigs.GetFocusedRowIndex());
            if (id == null)
                break;
            window.open(controllerName + '/Edit/' + id, "_blank");
            break;
        case "mnuDelete":
            if (gvDocumentConfigs.GetFocusedRowIndex() != 0 && confirm("Удалить?")) {
                var url = controllerName + "/Delete?Id=" + gvDocumentConfigs.GetRowKey(gvDocumentConfigs.GetFocusedRowIndex());
                $.post(url, function (data) {
                }).success(function () {
                    gvDocumentConfigs.PerformCallback();
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