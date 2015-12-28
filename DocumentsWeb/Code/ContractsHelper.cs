using System;
using System.Data;
using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Models;

namespace DocumentsWeb
{
    public class ContractsHelper
    {
        public static DataTable GetDocumentsContracts(bool refresh = false, int? stateId = null, int? count = null, DateTime? ds = null, DateTime? de = null)
        {
            return BusinessObjects.Web.Core.ContractDocumentsWebView.GetView(WADataProvider.WA,
                                                            DocumentContract.KINDID_DOGOVOR,
                                                            Folder.CODE_FIND_CONTRACTS_CONTRACT,
                                                            HttpContext.Current.User.Identity.Name,
                                                            ds.HasValue ? ds.Value : WADataProvider.Period.periodStart,
                                                                         de.HasValue ? de.Value : WADataProvider.Period.periodEnd,
                                                                         stateId: stateId,
                                                                         count: count,
                                                                         refresh: refresh);
        }

        public static DataTable GetDocumentsAccountingComputers(bool refresh = false, int? stateId = null, int? count = null, DateTime? ds = null, DateTime? de = null)
        {
            return BusinessObjects.Web.Core.ContractDocumentsWebView.GetView(WADataProvider.WA,
                                                            DocumentContract.KINDID_COMPUTER,
                                                            Folder.CODE_FIND_CONTRACTS_COMPUTER,
                                                            HttpContext.Current.User.Identity.Name,
                                                            ds.HasValue ? ds.Value : WADataProvider.Period.periodStart,
                                                                         de.HasValue ? de.Value : WADataProvider.Period.periodEnd,
                                                                         stateId: stateId,
                                                                         count: count,
                                                                         refresh: refresh);
        }

        public static DataTable GetDocumentsAccountingPrinters(bool refresh = false, int? stateId = null, int? count = null, DateTime? ds = null, DateTime? de = null)
        {
            return BusinessObjects.Web.Core.ContractDocumentsWebView.GetView(WADataProvider.WA,
                                                            DocumentContract.KINDID_PRINTER,
                                                            Folder.CODE_FIND_CONTRACTS_PRINTER,
                                                            HttpContext.Current.User.Identity.Name,
                                                            ds.HasValue ? ds.Value : WADataProvider.Period.periodStart,
                                                                         de.HasValue ? de.Value : WADataProvider.Period.periodEnd,
                                                                         stateId: stateId,
                                                                         count: count,
                                                                         refresh: refresh);
        }

        public static DataTable GetDocumentsRevision(bool refresh = false, int? stateId = null, int? count = null, DateTime? ds = null, DateTime? de = null)
        {
            return BusinessObjects.Web.Core.ContractDocumentsWebView.GetView(WADataProvider.WA,
                                                            DocumentContract.KINDID_REVISION,
                                                            Folder.CODE_FIND_CONTRACTS_REVISION,
                                                            HttpContext.Current.User.Identity.Name,
                                                            ds.HasValue ? ds.Value : WADataProvider.Period.periodStart,
                                                                         de.HasValue ? de.Value : WADataProvider.Period.periodEnd,
                                                                         stateId: stateId,
                                                                         count: count,
                                                                         refresh: refresh);
        }

        public static DataTable GetDocumentsVerification(bool refresh = false, int? stateId = null, int? count = null, DateTime? ds = null, DateTime? de = null)
        {
            return BusinessObjects.Web.Core.ContractDocumentsWebView.GetView(WADataProvider.WA,
                                                            DocumentContract.KINDID_VERIFICATION,
                                                            Folder.CODE_FIND_CONTRACTS_VERIFICATION,
                                                            HttpContext.Current.User.Identity.Name,
                                                            ds.HasValue ? ds.Value : WADataProvider.Period.periodStart,
                                                                         de.HasValue ? de.Value : WADataProvider.Period.periodEnd,
                                                                         stateId: stateId,
                                                                         count: count,
                                                                         refresh: refresh);
        }

        public static DataTable GetDocumentsOfficialNote(bool refresh = false, int? stateId = null, int? count = null, DateTime? ds = null, DateTime? de = null)
        {
            return BusinessObjects.Web.Core.ContractDocumentsWebView.GetView(WADataProvider.WA,
                                                            DocumentContract.KINDID_OFFICIALNOTE,
                                                            Folder.CODE_FIND_CONTRACTS_OFFICIALNOTE,
                                                            HttpContext.Current.User.Identity.Name,
                                                            ds.HasValue ? ds.Value : WADataProvider.Period.periodStart,
                                                                         de.HasValue ? de.Value : WADataProvider.Period.periodEnd,
                                                                         stateId: stateId,
                                                                         count: count,
                                                                         refresh: refresh);
        }
    }
}