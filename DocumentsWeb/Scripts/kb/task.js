function toggleAdditionalFields() {
    lblName.SetVisible(!lblName.GetVisible());
    Name.SetVisible(!Name.GetVisible());

    lblCode.SetVisible(!lblCode.GetVisible());
    Code.SetVisible(!Code.GetVisible());

    lblNameFull.SetVisible(!lblNameFull.GetVisible());
    NameFull.SetVisible(!NameFull.GetVisible());

    if (lblName.GetVisible())
        tsEditDocument.SetHeight(tsEditDocument.GetHeight() + 100);
    else
        tsEditDocument.SetHeight(tsEditDocument.GetHeight() - 100);
}
function markTaskAsCompleted() {
    var today = new Date();
    TaskStateId.SetValue(doneTaskId);
    DateEnd.SetDate(today);
    DateEndTimeAsDate.SetDate(today);
    DonePersent.SetValue(100);
}
function markTaskUnCompleted() {
    TaskStateId.SetValue(notDoneTaskId);
    DateEnd.SetDate(null);
    DateEndTimeAsDate.SetDate(null);
    DonePersent.SetValue(0);
}
function markTaskInProgress() {
    TaskStateId.SetValue(inprogressTaskId);
    DateEnd.SetDate(null);
    DateEndTimeAsDate.SetDate(null);
    DonePersent.SetValue(0);
}
function makeReadOnly() {
    document.getElementById('IsReadOnly').value = true;
    document.getElementById('documentEditForm').submit();
}
function makeNotReadOnly() {
    document.getElementById('IsReadOnly').value = false;
    document.getElementById('documentEditForm').submit();
}
function actionSaveClose() {
    document.getElementById('documentEditForm').submit();
    setTimeout(function() {
        $.ajax({
            type: 'GET',
            url: '@Url.Action("IsValidated")',
            async: false,
            success: function(data) {
                window.open('', '_self', '');
                window.close();
            }
        });
    }, 1000);
}