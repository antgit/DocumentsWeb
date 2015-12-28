function OnEditDocumentInit(s, e) {
    if (globalNotReadOnly) {
        if (MainCompanyDepatmentId.GetValue() == null) {
            DocNumber.SetEnabled(false);
            PrcNameId.SetEnabled(false);
            SupervisorId.SetEnabled(false);
            ManagerId.SetEnabled(false);
            PrcNameId.SetEnabled(false);
            MainClientDepatmentId.SetEnabled(false);
            MainClientAccountId.SetEnabled(false);
            MainCompanyAccountId.SetEnabled(false);
            gvDocDetails.SetEnabled(false);
            $('#lnkAddNewDetailRow').bind('click', false);
        }
    }
}
function OnAgentFromIdSelectedIndexChanged(s, e) {
    if (!MainClientAccountId.GetEnabled()) {
        MainClientAccountId.SetEnabled(true);
    }
    MainClientAccountId.PerformCallback();
}
function AgentToId_Changed(s, e) {
    if (!MainClientDepatmentId.GetEnabled()) {
        MainClientDepatmentId.SetEnabled(true);
    }
    MainClientDepatmentId.PerformCallback();

    if (!DocNumber.GetEnabled()) {
        DocNumber.SetEnabled(true);
    }

    if (!SupervisorId.GetEnabled()) {
        SupervisorId.SetEnabled(true);
    }
    SupervisorId.PerformCallback();

    if (!ManagerId.GetEnabled()) {
        ManagerId.SetEnabled(true);
    }
    ManagerId.PerformCallback();

    if (!MainCompanyAccountId.GetEnabled()) {
        MainCompanyAccountId.SetEnabled(true);
    }
    MainCompanyAccountId.PerformCallback();

    if (!PrcNameId.GetEnabled()) {
        PrcNameId.SetEnabled(true);
    }
    PrcNameId.PerformCallback();

    if (!gvDocDetails.GetEnabled()) {
        gvDocDetails.SetEnabled(true);
    }
    $('#lnkAddNewDetailRow').unbind('click', false);
}