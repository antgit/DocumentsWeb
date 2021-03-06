﻿function OnEditDocumentInit(s, e) {
    if (globalNotReadOnly) {
        if (MainCompanyDepatmentId.GetValue() == null) {
            DocNumber.SetEnabled(false);
            PrcNameId.SetEnabled(false);
            MainCompanyStoreId.SetEnabled(false);
            SupervisorId.SetEnabled(false);
            ManagerId.SetEnabled(false);
            PrcNameId.SetEnabled(false);
            MainCompanyAccountId.SetEnabled(false);
            MainClientDepatmentId.SetEnabled(false);
            gvDocDetails.SetEnabled(false);
            //$("#gridLnkCreate").children().prop('disabled', true);
            $('#lnkAddNewDetailRow').bind('click', false);
        }
        if (MainClientDepatmentId.GetValue() == null) {
            MainClientAccountId.SetEnabled(false);
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

    if (!MainCompanyStoreId.GetEnabled()) {
        MainCompanyStoreId.SetEnabled(true);
    }
    MainCompanyStoreId.PerformCallback();

    if (!PrcNameId.GetEnabled()) {
        PrcNameId.SetEnabled(true);
    }
    PrcNameId.PerformCallback();

    if (!gvDocDetails.GetEnabled()) {
        gvDocDetails.SetEnabled(true);
    }
    //$("#gridLnkCreate").children().prop('disabled',false);
    $('#lnkAddNewDetailRow').unbind('click', false);
    //$("#gridLnkCreate").prop('disabled', false);
}