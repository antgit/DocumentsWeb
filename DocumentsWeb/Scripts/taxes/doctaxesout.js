function OnEditDocumentInit(s, e) {
    if (globalNotReadOnly) {
        if (MainCompanyDepatmentId.GetValue() == null) {
            DocNumber.SetEnabled(false);
            MainClientDepatmentId.SetEnabled(false);
            DeliveryCondition.SetEnabled(false);
            DogovorDate.SetEnabled(false);
            DogovorNo.SetEnabled(false);
            PaymentMethod.SetEnabled(false);
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
    if (!DeliveryCondition.GetEnabled()) {
        DeliveryCondition.SetEnabled(true);
    }
    if (!DogovorDate.GetEnabled()) {
        DogovorDate.SetEnabled(true);
    }
    if (!DogovorNo.GetEnabled()) {
        DogovorNo.SetEnabled(true);
    }
    if (!PaymentMethod.GetEnabled()) {
        PaymentMethod.SetEnabled(true);
    }

    if (!gvDocDetails.GetEnabled()) {
        gvDocDetails.SetEnabled(true);
    }
    $('#lnkAddNewDetailRow').unbind('click', false);
}

//Получение ингформации о договоре (номер, дата) по ид клиента
//http://stackoverflow.com/questions/6076961/pass-date-values-from-ajax-call-to-mvc
function MainClientDepatmentId_SelectedIndexChanged(s,e) {
    var clientId = s.GetValue();
    var docDate=date.GetDate();

    $.ajax({
        type: "POST",
        url: serverRoot + "Contracts/Contract/GetContractInfo",
        data: {
            agentId: clientId,
            date: dateFormat(docDate, "dd/mm/yyyy HH:MM:ss")
        },
        success: function (response) {
            DogovorDate.SetDate(new Date(response.date));
            DogovorNo.SetText(response.number);
            //DeliveryCondition.SetValue(response.deliveryCondition);
        }
    });
}