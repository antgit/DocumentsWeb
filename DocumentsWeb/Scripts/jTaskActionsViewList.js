function Hierarchy_NodeSelect(s, e) {
    dxGridViewTasks.PerformCallback('refresh=false');
    if (e.node != null) {
        e.node.SetExpanded(true);
    }
}
function createCopy() {
    var url = "/Kb/Task/CreateCopy?Id=" + dxGridViewTasks.GetRowKey(dxGridViewTasks.GetFocusedRowIndex());
    $.get(url, function (data) {
    }).success(function () {
        dxGridViewTasks.PerformCallback('refresh=true');
    });
}
function changeState(state) {
    var url = "/Kb/Task/ChangeState?Id=" + dxGridViewTasks.GetRowKey(dxGridViewTasks.GetFocusedRowIndex()) + "&state=" + state.toString();
    $.get(url, function (data) {
    }).success(function () {
        dxGridViewTasks.PerformCallback('refresh=true');
    });
}
function deleteTask() {
    if (dxGridViewTasks.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь удалить задачу, Вы уверены?")) {
        var url = "/Kb/Task/Delete?Id=" + dxGridViewTasks.GetRowKey(dxGridViewTasks.GetFocusedRowIndex());
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewTasks.PerformCallback('refresh=true');
        });
    }
}
function deleteRow(visibleId) {
    if (confirm("Вы собираетесь удалить задачу, Вы уверены?")) {
        var url = "/Kb/Task/Delete?Id=" + dxGridViewTasks.GetRowKey(visibleId);
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewTasks.PerformCallback('refresh=true');
        });
    }
}
function showCustomizationWindow() {
    if (dxGridViewTasks.IsCustomizationWindowVisible())
        dxGridViewTasks.HideCustomizationWindow();
    else {
        dxGridViewTasks.ShowCustomizationWindow(document.getElementById('mnuShowCustomizationLocation'));
    }
}
function previewRow() {
    var id = dxGridViewTasks.GetRowKey(dxGridViewTasks.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open('/Kb/Task/Preview/' + id, "_blank");
}
function editRow() {
    var id = dxGridViewTasks.GetRowKey(dxGridViewTasks.GetFocusedRowIndex());
    if (id == null) {
        return true;
    }
    window.open('/Kb/Task/Edit/' + id, "_blank");
}
function create() {
    var node = tvHierarchiesFilter.GetSelectedNode();
    if (node == null) {
        window.open('/Kb/Task/Create/', "_blank");
    }
    else {
        var hierarchyId = node.name;
        window.open('/Kb/Task/Create?hierarchyId=' + hierarchyId, "_blank");
    }
}
function ShowOnlyActive() {
    dxGridViewTasks.ApplyFilter('StateId = 1');
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
    dxGridViewTasks.ApplyFilter('StateId = 2');
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
    dxGridViewTasks.ApplyFilter('StateId = 4');
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
function ShowOnlyDeleted() {
    dxGridViewTasks.ApplyFilter('StateId = 5');
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
function ShowByStateFilter(value) {
    dxGridViewTasks.ApplyFilter('StateId = 1 AND TaskStateId = ' + value);
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