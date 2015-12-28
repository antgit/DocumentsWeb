function OnEditDocumentInit(s, e) {
    if (globalNotReadOnly) {
        if (MainCompanyDepatmentId.GetValue() == null) {

            if(typeof MainClientDepatmentId != 'undefined')
                MainClientDepatmentId.SetEnabled(false);

            RegistratorId.SetEnabled(false);

            if (typeof SenderId != 'undefined')
                SenderId.SetEnabled(false);

            if (typeof RecipientId != 'undefined')
                RecipientId.SetEnabled(false);

            if (typeof StateCurrentId != 'undefined')
                StateCurrentId.SetEnabled(false);

            if (typeof ImportanceId != 'undefined')
                ImportanceId.SetEnabled(false);

            if (typeof ContractKindId != 'undefined')
                ContractKindId.SetEnabled(false);

            gvFiles.SetEnabled(false);
            ucMultiSelection.SetEnabled(false);
        }
    }
}
function AgentToId_Changed(s, e) {
    if (typeof MainClientDepatmentId != 'undefined') {
        if (!MainClientDepatmentId.GetEnabled()) {
            MainClientDepatmentId.SetEnabled(true);
        }
        MainClientDepatmentId.PerformCallback();
    }

    if (!RegistratorId.GetEnabled()) {
        RegistratorId.SetEnabled(true);
    }
    RegistratorId.PerformCallback();

    if(typeof SenderId != 'undefined'){
        if (!SenderId.GetEnabled()) {
            SenderId.SetEnabled(true);
        }
        SenderId.PerformCallback();
    }

    if (typeof RecipientId != 'undefined') {
        if (!RecipientId.GetEnabled()) {
            RecipientId.SetEnabled(true);
        }
        RecipientId.PerformCallback();
    }

    if (typeof StateCurrentId != 'undefined') {
        if (!StateCurrentId.GetEnabled()) {
            StateCurrentId.SetEnabled(true);
        }
        //StateCurrentId.PerformCallback();
    }

    if (typeof ImportanceId != 'undefined') {
        if (!ImportanceId.GetEnabled()) {
            ImportanceId.SetEnabled(true);
        }
        //ImportanceId.PerformCallback();
    }

    if (typeof ContractKindId != 'undefined') {
        if (!ContractKindId.GetEnabled()) {
            ContractKindId.SetEnabled(true);
        }
        //RecipientId.PerformCallback();
    }

    if (!gvFiles.GetEnabled()) {
        gvFiles.SetEnabled(true);
    }
}