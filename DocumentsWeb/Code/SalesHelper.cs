using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Models;
using System.Data;

namespace DocumentsWeb
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Набор колонок:
    /// Id, Guid, UserName, DateModified, Flags, StateId, [Name], Memo, FlagString,
    /// Date, AgentFromId, AgentToId, AgentDepartmentFromId, AgentDepartmentToId, AgentFromName, AgentToName,
    /// AgentDepartmentFromName, AgentDepartmentToName, 
    /// DocSumma, SummaTax, Number AS DocNumber, [Time], Accounting, MyCompanyId, ClientId, 
    /// ManagerName, SupervisorName, PriceName,
    /// CompanyName,ClientName, StateName, DateShip, DatePay, StoreName, BankAccFromName,
    /// BankAccToName, BankAccFromCode, BankAccToCode
    /// </remarks>
    public static class SalesHelper
    {
        /// <summary>
        /// Счета
        /// </summary>
        /// <param name="requestIn">Входящие или исходящие типы документа</param>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsAccount(bool requestIn, string folderCodeFind, bool refresh = false, int? count = null, int? stateId=null)
        {
            return BusinessObjects.Web.Core.SalesDocumentsWebView.GetView(WADataProvider.WA,
                                                            requestIn ? DocumentSales.KINDID_ACCOUNTIN : DocumentSales.KINDID_ACCOUNTOUT,
                                                            folderCodeFind,
                                                            HttpContext.Current.User.Identity.Name,
                                                            WADataProvider.Period.periodStart,
                                                            WADataProvider.Period.periodEnd, stateId, count, refresh);
        }
        /// <summary>
        /// Основные документы - приход или расход
        /// </summary>
        /// <param name="requestIn">Входящие или исходящие типы документа</param>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocuments(bool requestIn, string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.SalesDocumentsWebView.GetView(WADataProvider.WA,
                                                            requestIn ? DocumentSales.KINDID_IN : DocumentSales.KINDID_OUT,
                                                            folderCodeFind,
                                                            HttpContext.Current.User.Identity.Name,
                                                            WADataProvider.Period.periodStart,
                                                            WADataProvider.Period.periodEnd, stateId, count,
                                                            refresh);
        }
        /// <summary>
        /// Ассортиментный лист
        /// </summary>
        /// <param name="requestIn">Входящие или исходящие типы документа</param>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsAssort(bool requestIn, string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.SalesDocumentsWebView.GetView(WADataProvider.WA,
                                                            requestIn ? DocumentSales.KINDID_ASSORTIN : DocumentSales.KINDID_ASSORTOUT,
                                                            folderCodeFind,
                                                            HttpContext.Current.User.Identity.Name,
                                                            WADataProvider.Period.periodStart,
                                                            WADataProvider.Period.periodEnd, stateId, count,
                                                            refresh);
        }
        /// <summary>
        /// Инвентаризация
        /// </summary>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsInventory(string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.SalesDocumentsWebView.GetView(WADataProvider.WA,
                                                            DocumentSales.KINDID_INVENTORY,
                                                            folderCodeFind,
                                                            HttpContext.Current.User.Identity.Name,
                                                            WADataProvider.Period.periodStart,
                                                            WADataProvider.Period.periodEnd, stateId, count,
                                                            refresh);
        }
        /// <summary>
        /// Внутренее перемещение
        /// </summary>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsMove(string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.SalesDocumentsWebView.GetView(WADataProvider.WA,
                                                            DocumentSales.KINDID_MOVE,
                                                            folderCodeFind,
                                                            HttpContext.Current.User.Identity.Name,
                                                            WADataProvider.Period.periodStart,
                                                            WADataProvider.Period.periodEnd, stateId, count,
                                                            refresh);
        }
        /// <summary>
        /// Заказы
        /// </summary>
        /// <param name="requestIn">Входящие или исходящие типы документа</param>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsOrder(bool requestIn, string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.SalesDocumentsWebView.GetView(WADataProvider.WA,
                                                            requestIn ? DocumentSales.KINDID_ORDERIN : DocumentSales.KINDID_ORDEROUT,
                                                            folderCodeFind,
                                                            HttpContext.Current.User.Identity.Name,
                                                            WADataProvider.Period.periodStart,
                                                            WADataProvider.Period.periodEnd, stateId, count,
                                                            refresh);
        }
        /// <summary>
        /// Возвраты
        /// </summary>
        /// <param name="requestIn">Входящие или исходящие типы документа</param>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsReturn(bool requestIn, string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.SalesDocumentsWebView.GetView(WADataProvider.WA,
                                                            requestIn ? DocumentSales.KINDID_RETURNIN : DocumentSales.KINDID_RETURNOUT,
                                                            folderCodeFind,
                                                            HttpContext.Current.User.Identity.Name,
                                                            WADataProvider.Period.periodStart,
                                                            WADataProvider.Period.periodEnd, stateId, count,
                                                            refresh);
        }
        /// <summary>
        /// Акты
        /// </summary>
        /// <param name="requestIn">Входящие или исходящие типы документа</param>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsAct(bool requestIn, string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.SalesDocumentsWebView.GetView(WADataProvider.WA,
                                                            requestIn ? DocumentSales.KINDID_IN : DocumentSales.KINDID_OUT,
                                                            folderCodeFind,
                                                            HttpContext.Current.User.Identity.Name,
                                                            WADataProvider.Period.periodStart,
                                                            WADataProvider.Period.periodEnd, stateId, count,
                                                            refresh);
        }
        /// <summary>
        /// Текущие цены на товары
        /// </summary>
        /// <returns></returns>
        public static List<ProductView> GetProductPrices()
        {
            return ProductView.GetViewCurrentPrices(WADataProvider.WA, 3, HttpContext.Current.User.Identity.Name);
        }

        /// <summary>
        /// Текущие настроки документов для предприятия
        /// </summary>
        public static ConfigSales CurrentConfig()
        {
            ConfigSales config = ConfigSales.GetConfig(WADataProvider.WA,  WADataProvider.CurrentUser.MyCompanyId);
            return config;
        }

    }
}