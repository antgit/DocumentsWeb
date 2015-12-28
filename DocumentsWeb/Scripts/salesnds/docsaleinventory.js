function OnEditDocumentInit(s, e) {
    if (globalNotReadOnly) {
        if (MainCompanyDepatmentId.GetValue() == null) {
            DocNumber.SetEnabled(false);
            PrcNameId.SetEnabled(false);
            SupervisorId.SetEnabled(false);
            ManagerId.SetEnabled(false);
            PrcNameId.SetEnabled(false);
            MainClientDepatmentId.SetEnabled(false);
            gvDocDetails.SetEnabled(false);
            $('#lnkAddNewDetailRow').bind('click', false);
        }
    }
}
function AgentToId_Changed(s, e) {
    if (!MainClientDepatmentId.GetEnabled()) {
        MainClientDepatmentId.SetEnabled(true);
    }
    MainClientDepatmentId.PerformCallback();

    if (!DocNumber.GetEnabled()) {
        DocNumber.SetEnabled(true);
    }

    if (!PrcNameId.GetEnabled()) {
        PrcNameId.SetEnabled(true);
    }
    PrcNameId.PerformCallback();
    

    if (!SupervisorId.GetEnabled()) {
        SupervisorId.SetEnabled(true);
    }
    SupervisorId.PerformCallback();

    if (!ManagerId.GetEnabled()) {
        ManagerId.SetEnabled(true);
    }
    ManagerId.PerformCallback();


    if (!gvDocDetails.GetEnabled()) {
        gvDocDetails.SetEnabled(true);
    }
    $('#lnkAddNewDetailRow').unbind('click', false);
}