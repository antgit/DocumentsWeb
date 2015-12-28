function OnEditDocumentInit(s, e) {
    if (globalNotReadOnly) {
        if (AgentDepartmentFromId.GetValue() == null) {
            //DocNumber.SetEnabled(false);
//            MainClientDepatmentId.SetEnabled(false);
            RegistratorId.SetEnabled(false);
//            if(typeof MainClientAccountId!='undefined')
//                MainClientAccountId.SetEnabled(false);
//            if (typeof MainCompanyAccountId != 'undefined')
//                MainCompanyAccountId.SetEnabled(false);
            gvFiles.SetEnabled(false);
            ucMultiSelection.SetEnabled(false);
        }
    }
}
function AgentToId_Changed(s, e) {
//    if (!DocNumber.GetEnabled()) {
//        DocNumber.SetEnabled(true);
//    }

//    if (!MainClientDepatmentId.GetEnabled()) {
//        MainClientDepatmentId.SetEnabled(true);
//    }
//    MainClientDepatmentId.PerformCallback();

    if (!RegistratorId.GetEnabled()) {
        RegistratorId.SetEnabled(true);
    }
    RegistratorId.PerformCallback();

    if (!gvFiles.GetEnabled()) {
        gvFiles.SetEnabled(true);
    }
}