function OnEditDocumentInit(s, e) {
    if (globalNotReadOnly) {
        if (MainCompanyDepatmentId.GetValue() == null) {
            DocNumber.SetEnabled(false);
        }
    }
}
function OnAgentFromIdSelectedIndexChanged(s, e) {
    
}
function AgentToId_Changed(s, e) {
    if (!DocNumber.GetEnabled()) {
        DocNumber.SetEnabled(true);
    }
}