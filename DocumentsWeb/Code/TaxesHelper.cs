using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>����� �������:
    /// Id, Guid, UserName,DateModified, Flags, StateId, [Name], Memo, FlagString,
    /// Date, AgentFromId, AgentToId, AgentDepartmentFromId, AgentDepartmentToId, AgentFromName, AgentToName,
    /// AgentDepartmentFromName, AgentDepartmentToName, 
    /// DocSumma, SummaTax, DocNumber, [Time], Accounting, MyCompanyId, ClientId, 
    /// DeliveryCondition, PaymentMethod, DogovorDate, DogovorNo,
    /// SalesDocDate, SalesDocNumber, SalesDocId, CompanyName,ClientName, StateName
    /// </remarks>
    public static class TaxesHelper
    {
        /// <summary>
        /// ��������� ���������
        /// </summary>
        /// <param name="requestIn">�������� ��� ��������� ���� ���������</param>
        /// <param name="folderCodeFind">��� ������ �����</param>
        /// <returns></returns>
        public static DataTable GetDocuments(bool requestIn, string folderCodeFind, bool refresh=false, int? count = null, int? stateId=null)
        {
            if (requestIn)
                return BusinessObjects.Web.Core.TaxesDocumentsWebView.GetView(WADataProvider.WA,
                                                                                BusinessObjects.Documents.
                                                                                    DocumentTaxes.KINDID_IN,
                                                                                folderCodeFind,
                                                                                HttpContext.Current.User.Identity.Name,
                                                                                WADataProvider.Period.periodStart,
                                                                                WADataProvider.Period.periodEnd,stateId, count, refresh);

            return BusinessObjects.Web.Core.TaxesDocumentsWebView.GetView(WADataProvider.WA,
                                                                                BusinessObjects.Documents.
                                                                                    DocumentTaxes.KINDID_OUT,
                                                                                folderCodeFind,
                                                                                HttpContext.Current.User.Identity.Name,
                                                                                WADataProvider.Period.periodStart,
                                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);

        }
        /// <summary>
        /// ���������������� ���������
        /// </summary>
        /// <param name="requestIn">�������� ��� ��������� ���� ���������</param>
        /// <param name="folderCodeFind">��� ������ �����</param>
        /// <returns></returns>
        public static DataTable GetDocumentsCor(bool requestIn, string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            if (requestIn)
                return BusinessObjects.Web.Core.TaxesDocumentsWebView.GetView(WADataProvider.WA,
                                                                                BusinessObjects.Documents.
                                                                                    DocumentTaxes.KINDID_CORIN,
                                                                                folderCodeFind,
                                                                                HttpContext.Current.User.Identity.Name,
                                                                                WADataProvider.Period.periodStart,
                                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);

            return BusinessObjects.Web.Core.TaxesDocumentsWebView.GetView(WADataProvider.WA,
                                                                                BusinessObjects.Documents.
                                                                                    DocumentTaxes.KINDID_COROUT,
                                                                                folderCodeFind,
                                                                                HttpContext.Current.User.Identity.Name,
                                                                                WADataProvider.Period.periodStart,
                                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);
        }
        /// <summary>
        /// ������� ���� �� ������
        /// </summary>
        /// <returns></returns>
        public static IEnumerable GetProductPrices()
        {
            return ProductView.GetViewCurrentPrices(WADataProvider.WA, 3, HttpContext.Current.User.Identity.Name);
        }


    }
}