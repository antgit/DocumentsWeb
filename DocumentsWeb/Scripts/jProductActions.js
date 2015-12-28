function ProductCreateCopy(controllerName) {
    if (dxGridViewProducts.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь создать копию объекта учета, Вы уверенны?")) {
        var url = controllerName + "/CreateCopy?Id=" + dxGridViewProducts.GetRowKey(dxGridViewProducts.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewProducts.PerformCallback();
        });
    }
}
function ProductChangeState(state, controllerName) {
    if (dxGridViewProducts.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь изменить текущее состояние объекта учета, Вы уверенны?")) {
        var url = controllerName + "/ChangeState?Id=" + dxGridViewProducts.GetRowKey(dxGridViewProducts.GetFocusedRowIndex()) + "&state=" + state.toString();
        $.post(url, function(data) {
        }).success(function() {
            dxGridViewProducts.PerformCallback();
        });
    }
}
function deleteProduct(controllerName) {
    if (dxGridViewProducts.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь удалить объект учета, Вы уверенны?")) {
        debugger;
        var url = controllerName + "/Delete?Id=" + dxGridViewProducts.GetRowKey(dxGridViewProducts.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewProducts.PerformCallback();
        });
    }
}
function deleteRow(visibleId) {
    if (confirm("Удалить?")) {
        var url = controllerName + "/Delete?Id=" + dxGridViewProducts.GetRowKey(visibleId);
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewProducts.PerformCallback('refresh=true');
        });
    }
}
function editProduct() {
    var id = dxGridViewProducts.GetRowKey(dxGridViewProducts.GetFocusedRowIndex());
    if (id != null)
        ShowPopupEdit(id);
}
function previewRow(controllerName) {
    var id = dxGridViewProducts.GetRowKey(dxGridViewProducts.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open(controllerName + '/Preview/' + id, "_blank");
}
function openProduct(controllerName) {
    var id = dxGridViewProducts.GetRowKey(dxGridViewProducts.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open(controllerName + '/Open/' + id, "_blank");
}
function ShowOnlyDeleted() {
    dxGridViewProducts.ApplyFilter('StateId = 5');
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
    dxGridViewProducts.ApplyFilter('StateId = 1');
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
    dxGridViewProducts.ApplyFilter('StateId = 2');
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
    dxGridViewProducts.ApplyFilter('StateId = 4');
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