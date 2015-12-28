//function Hierarchy_NodeSelect(s, e) {
//    dxGridViewUserMessage.PerformCallback('refresh=false');
//    if (e.node != null) {
//        e.node.SetExpanded(true);
//    }
//}
function createCopy() {
    var url = "/UserPersonal/UserMessage/CreateCopy?Id=" + dxGridViewUserMessage.GetRowKey(dxGridViewUserMessage.GetFocusedRowIndex());
    $.get(url, function (data) {
    }).success(function () {
        dxGridViewUserMessage.PerformCallback('refresh=true');
    });
}
function changeState(state) {
    var url = "UserMessage/ChangeState?Id=" + dxGridViewUserMessage.GetRowKey(dxGridViewUserMessage.GetFocusedRowIndex()) + "&state=" + state.toString();
    $.get(url, function(data) {
    }).success(function() {
        dxGridViewUserMessage.PerformCallback('refresh=true');
    });
       
}
function previewRow() {
    var id = dxGridViewUserMessage.GetRowKey(dxGridViewUserMessage.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open('UserMessage/Preview/' + id, "_blank");
}

function deleteMessage() {
    if (dxGridViewUserMessage.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь удалить сообщение, Вы уверены?")) {
        var url = "UserMessage/ToTrash?Id=" + dxGridViewUserMessage.GetRowKey(dxGridViewUserMessage.GetFocusedRowIndex());
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewUserMessage.PerformCallback('refresh=true');
        });
    }
}
function deleteRow(visibleId) {
    if (confirm("Вы собираетесь удалить сообщение, Вы уверены?")) {
        var url = "UserMessage/ToTrash?Id=" + dxGridViewUserMessage.GetRowKey(visibleId);
        $.get(url, function (data) {
        }).success(function () {
            dxGridViewUserMessage.PerformCallback('refresh=true');
        });
    }
}
function showCustomizationWindow() {
    if (dxGridViewUserMessage.IsCustomizationWindowVisible())
        dxGridViewUserMessage.HideCustomizationWindow();
    else {
        dxGridViewUserMessage.ShowCustomizationWindow(document.getElementById('mnuShowCustomizationLocation'));
    }
}
function editRow() {
    var id = dxGridViewUserMessage.GetRowKey(dxGridViewUserMessage.GetFocusedRowIndex());
    if (id == null) {
        return true;
    }
    window.open('UserMessage/Edit/' + id, "_blank");
}
//function create() {
//    var node = tvHierarchiesFilter.GetSelectedNode();
//    if (node == null) {
//        window.open('/UserPersonal/UserMessage/Create/', "_blank");
//    }
//    else {
//        var hierarchyId = node.name;
//        window.open('/UserPersonal/UserMessage/Create?hierarchyId=' + hierarchyId, "_blank");
//    }
//}