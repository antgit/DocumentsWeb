function UnitreateCopy(controllerName) {
    if (dxGridViewUnits.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь создать копию аналитики, Вы уверенны?")) {
        var url = controllerName + "/CreateCopy?Id=" + dxGridViewUnits.GetRowKey(dxGridViewUnits.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewUnits.PerformCallback('refresh=true');
        });
    }
}
function UnitChangeState(state) {
    if (dxGridViewUnits.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь изменить текущее состояние аналитики, Вы уверенны?")) {
        var url = controllerName + "/ChangeState?Id=" + dxGridViewUnits.GetRowKey(dxGridViewUnits.GetFocusedRowIndex()) + "&state=" + state.toString();
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewUnits.PerformCallback();
        });
    }
}
function deleteUnit(controllerName) {
    if (dxGridViewUnits.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь удалить объект учета, Вы уверенны?")) {
        var url = controllerName + "/Delete?Id=" + dxGridViewUnits.GetRowKey(dxGridViewUnits.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewUnits.PerformCallback();
        });
    }
}
function deleteRow(visibleId) {
    if (confirm("Удалить?")) {
        var url = controllerName + "/Delete?Id=" + dxGridViewUnits.GetRowKey(visibleId);
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewUnits.PerformCallback('refresh=true');
        });
    }
}
function editUnit() {
    var id = dxGridViewUnits.GetRowKey(dxGridViewUnits.GetFocusedRowIndex());
    if (id != null)
        ShowPopupEdit(id);
}
function previewRow() {
    var id = dxGridViewUnits.GetRowKey(dxGridViewUnits.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open('Unit/Preview/' + id, "_blank");
}
function ShowOnlyDeleted() {
    dxGridViewUnits.ApplyFilter('StateId = 5');
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
    dxGridViewUnits.ApplyFilter('StateId = 1');
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
    dxGridViewUnits.ApplyFilter('StateId = 2');
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
    dxGridViewUnits.ApplyFilter('StateId = 4');
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
    if (dxGridViewUnits.IsCustomizationWindowVisible())
        dxGridViewUnits.HideCustomizationWindow();
    else {
        dxGridViewUnits.ShowCustomizationWindow(document.getElementById('mnuShowCustomizationLocation'));
    }
}