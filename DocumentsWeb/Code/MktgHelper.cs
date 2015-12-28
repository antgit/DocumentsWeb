using System;
using System.Data;
using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Models;

namespace DocumentsWeb
{
    public static class MktgHelper
    {
        /// <summary>
        /// Основные документы - анкеты
        /// </summary>
        /// <remarks>Набор колонок:
        /// Id, Guid, UserName, DateModified, Flags, StateId, [Name], Memo, FlagString,
        /// Date, AgentFromId, AgentToId,
        /// AgentDepartmentFromId, AgentDepartmentToId, AgentFromName, AgentToName,
        /// AgentDepartmentFromName, AgentDepartmentToName, 
        /// Summa AS DocSumma, SummaTax, Number AS DocNumber, [Time], Accounting, MyCompanyId, ClientId, 
        /// ManagerName, CompanyName,ClientName, StateName
        /// </remarks>
        /// <returns></returns>
        public static DataTable GetDocumentsMrak(bool refresh = false, int? stateId=null, int? count=null, DateTime? ds=null, DateTime? de=null)
        {
            //bool requestIn, 
            //string folderCodeFind, 
            return BusinessObjects.Web.Core.MktgDocumentsWebView.GetView(WADataProvider.WA,
                                                                         DocumentMktg.KINDID_MRAK,
                                                                         Folder.CODE_FIND_MKTG_MRAK,
                                                                         HttpContext.Current.User.Identity.Name,
                                                                         ds.HasValue? ds.Value: WADataProvider.Period.periodStart,
                                                                         de.HasValue ? de.Value : WADataProvider.Period.periodEnd,
                                                                         stateId: stateId,
                                                                         count: count,
                                                                         refresh: refresh);
        }

        public static DataTable GetDocumentsMrakNeedCorrect(bool refresh = false, int? count = null)
        {
            
            DataTable tbl = BusinessObjects.Web.Core.MktgDocumentsWebView.GetView(WADataProvider.WA,
                                                                          DocumentMktg.KINDID_MRAK,
                                                                          Folder.CODE_FIND_MKTG_MRAK,
                                                                          HttpContext.Current.User.Identity.Name,
                                                                          WADataProvider.Period.periodStart,
                                                                          WADataProvider.Period.periodEnd,
                                                                          stateId:2,
                                                                          count: count,
                                                                          refresh: refresh);

            //tbl.DefaultView.RowFilter = "StateId=2";


            return tbl.DefaultView.ToTable(); 
        }
    }
}