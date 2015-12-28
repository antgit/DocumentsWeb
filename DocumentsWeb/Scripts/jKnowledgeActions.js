function Hierarchy_NodeSelect(s, e) {
    change_hie_root = true;
    if (!dxGridViewKnowledge.InCallback()) {
        dxGridViewKnowledge.PerformCallback('refresh=false');
        if (e.node != null) {
            e.node.SetExpanded(true);
        }
    }
}
function createCopy() {
    if (dxGridViewKnowledge.GetFocusedRowIndex() != 0) {
        var url = controllerName + "/CreateCopy?Id=" + dxGridViewKnowledge.GetRowKey(dxGridViewKnowledge.GetFocusedRowIndex());
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewKnowledge.PerformCallback('refresh=true');
        });
    }
}
function changeState(state) {
    if (dxGridViewKnowledge.GetFocusedRowIndex() != 0) {
        var url = controllerName + "/ChangeState?Id=" + dxGridViewKnowledge.GetRowKey(dxGridViewKnowledge.GetFocusedRowIndex()) + "&state=" + state.toString();
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewKnowledge.PerformCallback('refresh=true');
        });
    }
}
function editRow() {
    var id = dxGridViewKnowledge.GetRowKey(dxGridViewKnowledge.GetFocusedRowIndex());
    if (id != null)
        ShowPopupEdit(id, 0);
}
function deleteKnowledge() {
    var id = dxGridViewKnowledge.GetRowKey(dxGridViewKnowledge.GetFocusedRowIndex());
    if (id != null && confirm("Вы собираетесь удалить статью, Вы уверены?")) {
        var url = controllerName + "/Delete?Id=" + dxGridViewKnowledge.GetRowKey(dxGridViewKnowledge.GetFocusedRowIndex());
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewKnowledge.PerformCallback('refresh=true');
        });
    }
}
function deleteRow(visibleId) {
    if (dxGridViewKnowledge.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь удалить статью, Вы уверены?")) {
        var url = controllerName + "/Delete?Id=" + dxGridViewKnowledge.GetRowKey(visibleId);
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewKnowledge.PerformCallback('refresh=true');
        });
    }
}
function showCustomizationWindow() {
    if (dxGridViewKnowledge.IsCustomizationWindowVisible())
        dxGridViewKnowledge.HideCustomizationWindow();
    else {
        dxGridViewKnowledge.ShowCustomizationWindow(document.getElementById('mnuShowCustomizationLocation'));
    }
}
function ControlKnowledge() {
    if (dxGridViewKnowledge.GetFocusedRowIndex() != -1) {
        var id = dxGridViewKnowledge.GetRowKey(dxGridViewKnowledge.GetFocusedRowIndex());
        window.open(controllerName + '/ControlView/' + id, "_blank");
    }
}
function previewRow() {
    var id = dxGridViewKnowledge.GetRowKey(dxGridViewKnowledge.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open(controllerName +'/Preview/' + id, "_blank");
}
function ShowOnlyDeleted() {
    dxGridViewKnowledge.ApplyFilter('StateId = 5');
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
    dxGridViewKnowledge.ApplyFilter('StateId = 1');
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
    dxGridViewKnowledge.ApplyFilter('StateId = 2');
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
    dxGridViewKnowledge.ApplyFilter('StateId = 4');
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