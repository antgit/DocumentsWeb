using System;
using System.Collections;
using System.Data.SqlClient;
using System.Web;
using System.Linq;
using System.Data;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb
{
    public static class ServicesHelper
    {
        /// <summary>
        /// ���� ������/�������� �����
        /// </summary>
        /// <param name="requestIn">�������� ��� ��������� ���� ���������</param>
        /// <param name="folderCodeFind">��� ������ �����</param>
        /// <returns></returns>
        public static DataTable GetDocuments(bool requestIn, string folderCodeFind, bool refresh = false, int? count = null, int? stateId=null)
        {

            if (requestIn)
                return BusinessObjects.Web.Core.ServiceDocumentsWebView.GetView(WADataProvider.WA,
                                                                                BusinessObjects.Documents.
                                                                                    DocumentService.KINDID_IN,
                                                                                folderCodeFind,
                                                                                HttpContext.Current.User.Identity.Name,
                                                                                WADataProvider.Period.periodStart,
                                                                                WADataProvider.Period.periodEnd, stateId, count,  refresh);
            return BusinessObjects.Web.Core.ServiceDocumentsWebView.GetView(WADataProvider.WA,
                                                                                BusinessObjects.Documents.
                                                                                    DocumentService.KINDID_OUT,
                                                                                folderCodeFind,
                                                                                HttpContext.Current.User.Identity.Name,
                                                                                WADataProvider.Period.periodStart,
                                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);
                //return WADataProvider.GetDocumentsByKindValue(BusinessObjects.Documents.DocumentService.KINDID_IN, folderCodeFind);
            //return WADataProvider.GetDocumentsByKindValue(BusinessObjects.Documents.DocumentService.KINDID_OUT, folderCodeFind);
        }
        /// <summary>
        /// �����
        /// </summary>
        /// <param name="requestIn">�������� ��� ��������� ���� ���������</param>
        /// <param name="folderCodeFind">��� ������ �����</param>
        /// <returns></returns>
        public static DataTable GetDocumentsAccounts(bool requestIn, string folderCodeFind, bool refresh=false, int? count=null, int? stateId=null)
        {
            if (requestIn)
                return BusinessObjects.Web.Core.ServiceDocumentsWebView.GetView(WADataProvider.WA,
                                                                                BusinessObjects.Documents.
                                                                                    DocumentService.KINDID_ACCOUNTIN,
                                                                                folderCodeFind,
                                                                                HttpContext.Current.User.Identity.Name,
                                                                                WADataProvider.Period.periodStart,
                                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);

            return BusinessObjects.Web.Core.ServiceDocumentsWebView.GetView(WADataProvider.WA,
                                                                BusinessObjects.Documents.
                                                                    DocumentService.KINDID_ACCOUNTOUT,
                                                                folderCodeFind,
                                                                HttpContext.Current.User.Identity.Name,
                                                                WADataProvider.Period.periodStart,
                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);
        }
        /// <summary>
        /// ������
        /// </summary>
        /// <param name="requestIn">�������� ��� ��������� ���� ���������</param>
        /// <param name="folderCodeFind">��� ������ �����</param>
        /// <returns></returns>
        public static DataTable GetDocumentsOrders(bool requestIn, string folderCodeFind, bool refresh = false, int? count = null, int? stateId=null)
        {
            if (requestIn)
                return BusinessObjects.Web.Core.ServiceDocumentsWebView.GetView(WADataProvider.WA,
                                                                BusinessObjects.Documents.
                                                                    DocumentService.KINDID_ORDERIN,
                                                                folderCodeFind,
                                                                HttpContext.Current.User.Identity.Name,
                                                                WADataProvider.Period.periodStart,
                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);
            return BusinessObjects.Web.Core.ServiceDocumentsWebView.GetView(WADataProvider.WA,
                                                                BusinessObjects.Documents.
                                                                    DocumentService.KINDID_ORDEROUT,
                                                                folderCodeFind,
                                                                HttpContext.Current.User.Identity.Name,
                                                                WADataProvider.Period.periodStart,
                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);
            //    return WADataProvider.GetDocumentsByKindValue(BusinessObjects.Documents.DocumentService.KINDID_ORDERIN, folderCodeFind);
            //return WADataProvider.GetDocumentsByKindValue(BusinessObjects.Documents.DocumentService.KINDID_ORDEROUT, folderCodeFind);
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