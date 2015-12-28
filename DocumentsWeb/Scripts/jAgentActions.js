function createCopy(controllerName) {
    if (dxGridViewAgents.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь создать копию корреспондента, Вы уверенны?")) {
        var url = controllerName + "/CreateCopy?Id=" + dxGridViewAgents.GetRowKey(dxGridViewAgents.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewAgents.PerformCallback();
        });
    }
}
function changeState(state, controllerName) {
    if (dxGridViewAgents.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь изменить текущее состояние корреспондента, Вы уверенны?")) {
        var url = controllerName + "/ChangeState?Id=" + dxGridViewAgents.GetRowKey(dxGridViewAgents.GetFocusedRowIndex()) + "&state=" + state.toString();
        $.post(url, function(data) {
        }).success(function() {
            dxGridViewAgents.PerformCallback();
        });
    }
}
function deleteAgent(controllerName) {
    if (dxGridViewAgents.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь удалить корреспондента, Вы уверенны?")) {
        var url = controllerName + "/Delete?Id=" + dxGridViewAgents.GetRowKey(dxGridViewAgents.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewAgents.PerformCallback();
        });
    }
}
function deleteRow(visibleId) {
    if (confirm("Удалить корреспондента?")) {
        var url = controllerName + "/Delete?Id=" + dxGridViewAgents.GetRowKey(visibleId);
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewAgents.PerformCallback('refresh=true');
        });
    }
}
function editAgent() {
    var id = dxGridViewAgents.GetRowKey(dxGridViewAgents.GetFocusedRowIndex());
    if (id != null)
        ShowPopupEdit(id);
}
function previewRow(controllerName) {
    var id = dxGridViewAgents.GetRowKey(dxGridViewAgents.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open(controllerName +'/Preview/' + id, "_blank");
}
function openAgent(controllerName) {
    var id = dxGridViewAgents.GetRowKey(dxGridViewAgents.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open(controllerName + '/Open/' + id, "_blank");
}
function ShowOnlyDeleted() {
    dxGridViewAgents.ApplyFilter('StateId = 5');
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
    dxGridViewAgents.ApplyFilter('StateId = 1');
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
    dxGridViewAgents.ApplyFilter('StateId = 2');
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
    dxGridViewAgents.ApplyFilter('StateId = 4');
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