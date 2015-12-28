function UserCreateCopy(controllerName) {
    if (confirm("Вы собираетесь создать копию пользователя, Вы уверенны?")) {
        var url = controllerName + "/CreateCopy?Id=" + dxGridViewUsers.GetRowKey(dxGridViewUsers.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewUsers.PerformCallback();
        });
    }
}
function UserChangeState(state, controllerName) {
    if (confirm("Вы собираетесь изменить текущее состояние пользователя, Вы уверенны?")) {
        var url = controllerName + "/ChangeState?Id=" + dxGridViewUsers.GetRowKey(dxGridViewUsers.GetFocusedRowIndex()) + "&state=" + state.toString();
        $.post(url, function(data) {
        }).success(function() {
            dxGridViewUsers.PerformCallback();
        });
    }
}
function deleteUser(controllerName) {
    if (dxGridViewUsers.GetFocusedRowIndex() != -1 && confirm("Вы собираетесь удалить пользователя, Вы уверенны?")) {
        debugger;
        var url = controllerName + "/Delete?Id=" + dxGridViewUsers.GetRowKey(dxGridViewUsers.GetFocusedRowIndex());
        $.post(url, function (data) {
        }).success(function () {
            dxGridViewUsers.PerformCallback();
        });
    }
}
function editUser() {
    var id = dxGridViewUsers.GetRowKey(dxGridViewUsers.GetFocusedRowIndex());
    if (id != null)
        ShowPopupEdit(id);
}
function previewRow() {
    var id = dxGridViewUsers.GetRowKey(dxGridViewUsers.GetFocusedRowIndex());
    if (id == null) {
        return;
    }
    window.open('User/Preview/' + id, "_blank");
}