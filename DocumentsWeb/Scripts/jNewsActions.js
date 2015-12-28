function MarkIsSend() {
    var today = new Date();
    SendDate.SetDate(today);
    SendTimeAsDate.SetDate(today);
}
function MarkIsNotSend() {
    SendDate.SetDate(null);
    SendTimeAsDate.SetDate(null);
}
function create() {
    var node = tvHierarchiesFilter.GetSelectedNode();
    if (node == null) {
        window.open('/Kb/News/Create/', "_blank");
    }
    else {
        var hierarchyId = node.name;
        window.open('/Kb/News/Create/?hierarchyId=' + hierarchyId, "_blank");
    }
}
function createCopy() {
    if (dxGridViewNews.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь создать копию, Вы уверенны?")) {
        var url = "/Kb/News/CreateCopy?Id=" + dxGridViewNews.GetRowKey(dxGridViewNews.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewNews.PerformCallback('refresh=true');
        });
    }
}
function changeState(state) {
    if (dxGridViewNews.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь изменить текущее состояние аналитики, Вы уверенны?")) {
        var url = "/Kb/News/ChangeState?Id=" + dxGridViewNews.GetRowKey(dxGridViewNews.GetFocusedRowIndex()) + "&state=" + state.toString();
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewNews.PerformCallback();
        });
    }
}
function deleteNews() {
    if (dxGridViewNews.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь удалить новость, Вы уверенны?")) {
        var url = "/Kb/News/Delete?Id=" + dxGridViewNews.GetRowKey(dxGridViewNews.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewNews.PerformCallback('refresh=true');
        });
    }
}
function deleteRow(visibleId) {
    if (confirm("Удалить?")) {
        var url = "/Kb/News/Delete?Id=" + dxGridViewNews.GetRowKey(visibleId);
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewNews.PerformCallback('refresh=true');
        });
    }
}
function editRow() {
    var id = dxGridViewNews.GetRowKey(dxGridViewNews.GetFocusedRowIndex());
    if (id != null)
        window.open('/Kb/News/ControlView/' + id, "_blank");
}
function previewRow() {
    var id = dxGridViewNews.GetRowKey(dxGridViewNews.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open('/Kb/News/Preview/' + id, "_blank");
}
function showCustomizationWindow() {
    if (dxGridViewNews.IsCustomizationWindowVisible())
        dxGridViewNews.HideCustomizationWindow();
    else {
        dxGridViewNews.ShowCustomizationWindow(document.getElementById('mnuShowCustomizationLocation'));
    }
}
function Hierarchy_NodeSelect(s, e) {
    dxGridViewNews.PerformCallback('refresh=false');
    if (e.node != null) {
        e.node.SetExpanded(true);
    }
} 
function ShowOnlyDeleted() {
    dxGridViewNews.ApplyFilter('StateId = 5');
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
    dxGridViewNews.ApplyFilter('StateId = 1');
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
    dxGridViewNews.ApplyFilter('StateId = 2');
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
    dxGridViewNews.ApplyFilter('StateId = 4');
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