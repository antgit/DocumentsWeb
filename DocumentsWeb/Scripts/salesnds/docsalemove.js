function OnEditDocumentInit(s, e) {
    if (globalNotReadOnly) {
        if (MainCompanyDepatmentId.GetValue() == null) {
            DocNumber.SetEnabled(false);
            PrcNameId.SetEnabled(false);
            MainCompanyStoreId.SetEnabled(false);
            MainClientStoreId.SetEnabled(false);
            SupervisorId.SetEnabled(false);
            ManagerId.SetEnabled(false);
            PrcNameId.SetEnabled(false);
            gvDocDetails.SetEnabled(false);
            $('#lnkAddNewDetailRow').bind('click', false);
        }
    }
}
function AgentToId_Changed(s, e) {
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

    //MainCompanyAccountId.PerformCallback();
    if (!MainCompanyStoreId.GetEnabled()) {
        MainCompanyStoreId.SetEnabled(true);
    }
    MainCompanyStoreId.PerformCallback();

    if (!MainClientStoreId.GetEnabled()) {
        MainClientStoreId.SetEnabled(true);
    }
    MainClientStoreId.PerformCallback();

    if (!PrcNameId.GetEnabled()) {
        PrcNameId.SetEnabled(true);
    }
    PrcNameId.PerformCallback();

    if (!gvDocDetails.GetEnabled()) {
        gvDocDetails.SetEnabled(true);
    }
    $('#lnkAddNewDetailRow').unbind('click', false);
}