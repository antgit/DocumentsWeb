function AnaliticCreateCopy(controllerName) {
    if (dxGridViewAnalitic.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь создать копию аналитики, Вы уверенны?")) {
        var url = controllerName + "/CreateCopy?Id=" + dxGridViewAnalitic.GetRowKey(dxGridViewAnalitic.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewAnalitic.PerformCallback('refresh=true');
        });
    }
}
function AnaliticChangeState(state, controllerName) {
    if (dxGridViewAnalitic.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь изменить текущее состояние аналитики, Вы уверенны?")) {
        var url = controllerName + "/ChangeState?Id=" + dxGridViewAnalitic.GetRowKey(dxGridViewAnalitic.GetFocusedRowIndex()) + "&state=" + state.toString();
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewAnalitic.PerformCallback();
        });
    }
}
function deleteAnalitic(controllerName) {
    if (dxGridViewAnalitic.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь удалить объект учета, Вы уверенны?")) {
        var url = controllerName + "/Delete?Id=" + dxGridViewAnalitic.GetRowKey(dxGridViewAnalitic.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewAnalitic.PerformCallback();
        });
    }
}
function deleteRow(visibleId) {
    if (confirm("Удалить?")) {
        var url = controllerName + "/Delete?Id=" + dxGridViewAnalitic.GetRowKey(visibleId);
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewAnalitic.PerformCallback('refresh=true');
        });
    }
}
function editAnalitic() {
    var id = dxGridViewAnalitic.GetRowKey(dxGridViewAnalitic.GetFocusedRowIndex());
    if (id != null)
        ShowPopupEdit(id);
}
function previewRow(controllerName) {
    var id = dxGridViewAnalitic.GetRowKey(dxGridViewAnalitic.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open(controllerName + '/Preview/' + id, "_blank");
}
function openAnalitic(controllerName) {
    var id = dxGridViewAnalitic.GetRowKey(dxGridViewAnalitic.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open(controllerName + '/Open/' + id, "_blank");
}
function ShowOnlyDeleted() {
    dxGridViewAnalitic.ApplyFilter('StateId = 5');
    var item = nbFeatures.GetItemByName('mnuDelete');
    if (item != null) {
        item.SetVisible(false);
    }
    item = nbFeatures.GetItemByName('mnuStateActive');
    if (item != null) {
        item.SetVisible(true);
    }
    item = nbFeatures.GetItemByName('mnuStateNeedCorrect');
    if (item != null) {
        item.SetVisible(true);
    }
    item = nbFeatures.GetItemByName('mnuStateBan');
    if (item != null) {
        item.SetVisible(false);
    }
}
function ShowOnlyActive() {
    dxGridViewAnalitic.ApplyFilter('StateId = 1');
    var item = nbFeatures.GetItemByName('mnuDelete');
    if (item != null) {
        item.SetVisible(true);
    }
    item = nbFeatures.GetItemByName('mnuStateActive');
    if (item != null) {
        item.SetVisible(false);
    }
    item = nbFeatures.GetItemByName('mnuStateNeedCorrect');
    if (item != null) {
        item.SetVisible(true);
    }
    item = nbFeatures.GetItemByName('mnuStateBan');
    if (item != null) {
        item.SetVisible(true);
    }
}
function ShowOnlyNotDone() {
    dxGridViewAnalitic.ApplyFilter('StateId = 2');
    var item = nbFeatures.GetItemByName('mnuDelete');
    if (item != null) {
        item.SetVisible(true);
    }
    item = nbFeatures.GetItemByName('mnuStateActive');
    if (item != null) {
        item.SetVisible(true);
    }
    item = nbFeatures.GetItemByName('mnuStateNeedCorrect');
    if (item != null) {
        item.SetVisible(false);
    }
    item = nbFeatures.GetItemByName('mnuStateBan');
    if (item != null) {
        item.SetVisible(true);
    }
}
function ShowOnlyDenyed() {
    dxGridViewAnalitic.ApplyFilter('StateId = 4');
    var item = nbFeatures.GetItemByName('mnuDelete');
    if (item != null) {
        item.SetVisible(true);
    }
    item = nbFeatures.GetItemByName('mnuStateActive');
    if (item != null) {
        item.SetVisible(true);
    }
    item = nbFeatures.GetItemByName('mnuStateNeedCorrect');
    if (item != null) {
        item.SetVisible(true);
    }
    item = nbFeatures.GetItemByName('mnuStateBan');
    if (item != null) {
        item.SetVisible(false);
    }
}
function showCustomizationWindow() {
    if (dxGridViewAnalitic.IsCustomizationWindowVisible())
        dxGridViewAnalitic.HideCustomizationWindow();
    else {
        dxGridViewAnalitic.ShowCustomizationWindow(document.getElementById('mnuShowCustomizationLocation'));
    }
}