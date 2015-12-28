function makeReadOnly() {
    document.getElementById('IsReadOnly').value = true;
    document.getElementById('editForm').submit();
}
function makeNotReadOnly() {
    document.getElementById('IsReadOnly').value = false;
    document.getElementById('editForm').submit();
}