function OnEditDocumentInit(s, e) {
    if (globalNotReadOnly) {
        if (MainCompanyDepatmentId.GetValue() == null) {
            DocNumber.SetEnabled(false);
            //PrcNameId.SetEnabled(false);
            MainClientDepatmentId.SetEnabled(false);
            MainClientAccountId.SetEnabled(false);
            MainCompanyAccountId.SetEnabled(false);
            PaymentId.SetEnabled(false);
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

    if (!MainCompanyAccountId.GetEnabled()) {
        MainCompanyAccountId.SetEnabled(true);
    }
    MainCompanyAccountId.PerformCallback();


    if (!PaymentId.GetEnabled()) {
        PaymentId.SetEnabled(true);
    }

    if (!gvDocDetails.GetEnabled()) {
        gvDocDetails.SetEnabled(true);
    }
    $('#lnkAddNewDetailRow').unbind('click', false);
}

function OnAgentFromIdSelectedIndexChanged(s, e) {
    if (globalNotReadOnly) {
        if (!MainClientAccountId.GetEnabled()) {
            MainClientAccountId.SetEnabled(true);
        }
        MainClientAccountId.PerformCallback();
    }
}