function OnEditDocumentInit(s, e) {
    if (globalNotReadOnly) {
        if (MainCompanyDepatmentId.GetValue() == null) {
            DocNumber.SetEnabled(false);
            date.SetEnabled(false);
            PrcNameId.SetEnabled(false);
            DateStart.SetEnabled(false);
            DateExpire.SetEnabled(false);
            gvDocDetails.SetEnabled(false);
            $('#lnkAddNewDetailRow').bind('click', false);
        }
    }
}
function AgentToId_Changed(s, e) {
    if (!DocNumber.GetEnabled()) {
        DocNumber.SetEnabled(true);
    }
    if (!date.GetEnabled()) {
        date.SetEnabled(true);
    }
    if (!DateStart.GetEnabled()) {
        DateStart.SetEnabled(true);
    }
    if (!DateExpire.GetEnabled()) {
        DateExpire.SetEnabled(true);
    }
    if (!PrcNameId.GetEnabled()) {
        PrcNameId.SetEnabled(true);
    }
    PrcNameId.PerformCallback();

    if (!gvDocDetails.GetEnabled()) {
        gvDocDetails.SetEnabled(true);
    }
    $('#lnkAddNewDetailRow').unbind('click', false);
}