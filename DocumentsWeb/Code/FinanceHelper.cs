using System.Collections;
using System.Data;
using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Models;

namespace DocumentsWeb
{
    /// <summary>
    /// ������ � ����������� � ������� "���������� ���������"
    /// </summary>
    /// <remarks>
    /// ����� �������:
    /// Id, Guid, UserName, DateModified, Flags, StateId, [Name], Memo, FlagString,
    /// Date, AgentFromId, AgentToId,AgentDepartmentFromId, AgentDepartmentToId, AgentFromName, AgentToName,
    /// AgentDepartmentFromName, AgentDepartmentToName, 
    /// DocSumma, SummaTax, DocNumber, [Time], Accounting, MyCompanyId, ClientId, 
    /// PaymentTypeName, AgFromBankAccName, AgFromBankAccCode,
    /// AgToBankAccName, AgToBankAccCode,CompanyName,ClientName, StateName
    /// </remarks>
    public static class FinanceHelper
    {
        /// <summary>
        /// ��������� ����������� �������� �������  - ���������, �� ������������
        /// </summary>
        /// <param name="folderCodeFind">��� ������ �����</param>
        /// <returns></returns>
        public static DataTable GetDocumentsIn(string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.FinancesDocumentsWebView.GetView(WADataProvider.WA,
                                                                               DocumentFinance.KINDID_IN,
                                                                               folderCodeFind,
                                                                               HttpContext.Current.User.Identity.Name,
                                                                               WADataProvider.Period.periodStart,
                                                                               WADataProvider.Period.periodEnd, stateId, count, refresh);

            //return WADataProvider.GetDocumentsByKindValue(DocumentFinance.KINDID_IN, folderCodeFind);
        }

        /// <summary>
        /// ��������� ����������� �������� �������
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDocumentsIn(bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.FinancesDocumentsWebView.GetView(WADataProvider.WA,
                                                                               DocumentFinance.KINDID_IN,
                                                                               Folder.CODE_FIND_FINANCE_IN,
                                                                               HttpContext.Current.User.Identity.Name,
                                                                               WADataProvider.Period.periodStart,
                                                                               WADataProvider.Period.periodEnd, stateId, count, refresh);
            //return WADataProvider.GetDocumentsByKindValue(DocumentFinance.KINDID_IN, Folder.CODE_FIND_FINANCE_IN);
        }

        /// <summary>
        /// ��������� ������� �������� ������� - ���������, �� ������������
        /// </summary>
        /// <param name="folderCodeFind"></param>
        /// <returns></returns>
        public static DataTable GetDocumentsOut(string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.FinancesDocumentsWebView.GetView(WADataProvider.WA,
                                                                   DocumentFinance.KINDID_OUT,
                                                                   folderCodeFind,
                                                                   HttpContext.Current.User.Identity.Name,
                                                                   WADataProvider.Period.periodStart,
                                                                   WADataProvider.Period.periodEnd, stateId, count, refresh);

            //return WADataProvider.GetDocumentsByKindValue(DocumentFinance.KINDID_OUT, folderCodeFind);
        }

        /// <summary>
        /// ��������� ������� �������� �������
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDocumentsOut(bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.FinancesDocumentsWebView.GetView(WADataProvider.WA,
                                                                               DocumentFinance.KINDID_OUT,
                                                                               Folder.CODE_FIND_FINANCE_OUT,
                                                                               HttpContext.Current.User.Identity.Name,
                                                                               WADataProvider.Period.periodStart,
                                                                               WADataProvider.Period.periodEnd, stateId, count, refresh);
        }
    }
}