using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Areas.Prices.Models;
using DocumentsWeb.Models;

namespace DocumentsWeb
{
    /// <summary>
    /// Работа с документами в разделе "Управление ценами"
    /// </summary>
    public static class PriceListHelper
    {
        /// <summary>
        /// Основные документы - наши прайс-листы
        /// </summary>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocuments(string folderCodeFind, bool refresh=false, int? count=null, int? stateId=null)
        {
            return BusinessObjects.Web.Core.PricesDocumentsWebView.GetView(WADataProvider.WA,
                                                                                DocumentPrices.KINDID_PRICE,
                                                                                folderCodeFind,
                                                                                HttpContext.Current.User.Identity.Name,
                                                                                WADataProvider.Period.periodStart,
                                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);
        }

        /// <summary>
        /// Основные документы - наши индивидуальные прайс-листы
        /// </summary>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsInd(string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.PricesDocumentsWebView.GetView(WADataProvider.WA,
                                                                                DocumentPrices.KINDID_PRICEIND,
                                                                                folderCodeFind,
                                                                                HttpContext.Current.User.Identity.Name,
                                                                                WADataProvider.Period.periodStart,
                                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);
        }

        /// <summary>
        /// Основные документы - наши прайс-листы
        /// </summary>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsCommand(string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.PricesDocumentsWebView.GetView(WADataProvider.WA,
                                                                                DocumentPrices.KINDID_COMMAND,
                                                                                folderCodeFind,
                                                                                HttpContext.Current.User.Identity.Name,
                                                                                WADataProvider.Period.periodStart,
                                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);
        }

        /// <summary>
        /// Основные документы - наши индивидуальные прайс-листы
        /// </summary>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsCommandInd(string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.PricesDocumentsWebView.GetView(WADataProvider.WA,
                                                                                DocumentPrices.KINDID_COMMANDIND,
                                                                                folderCodeFind,
                                                                                HttpContext.Current.User.Identity.Name,
                                                                                WADataProvider.Period.periodStart,
                                                                                WADataProvider.Period.periodEnd, stateId, count, refresh);
        }
        /// <summary>
        /// Основной документ - прайс-лист конкурентов
        /// </summary>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsCompetitor(string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.PricesDocumentsWebView.GetView(WADataProvider.WA,
                                                                    DocumentPrices.KINDID_COMPETITOR,
                                                                    folderCodeFind,
                                                                    HttpContext.Current.User.Identity.Name,
                                                                    WADataProvider.Period.periodStart,
                                                                    WADataProvider.Period.periodEnd, stateId, count, refresh);
            
        }

        /// <summary>
        /// Основной документ - прайс-лист поставщиков
        /// </summary>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static DataTable GetDocumentsSupplier(string folderCodeFind, bool refresh = false, int? count = null, int? stateId = null)
        {
            return BusinessObjects.Web.Core.PricesDocumentsWebView.GetView(WADataProvider.WA,
                                                                    DocumentPrices.KINDID_SYPPLYER,
                                                                    folderCodeFind,
                                                                    HttpContext.Current.User.Identity.Name,
                                                                    WADataProvider.Period.periodStart,
                                                                    WADataProvider.Period.periodEnd, stateId, count, refresh);
        }

        public static List<PriceNameModel> GetPriceNames()
        {
            var coll = WADataProvider.WA.GetCollection<PriceName>().Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId) && !s.IsHiden).Select(PriceNameModel.ToModel).ToList();
            return coll;
        }


        /// <summary>
        /// Текущая цена на товар 
        /// </summary>
        /// <param name="priceNameId">Идентификатор вида цены</param>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="agentId">Идентификатор компании</param>
        /// <param name="date">Дата, на которую требуется получить цену. Если не указана - соответствует текущей</param>
        /// <param name="agentToId">Идентификатор покупателя</param>
        /// <returns></returns>
        public static decimal GetPriceOut(int priceNameId, int productId, int agentId, DateTime? date, int? agentToId = null)
        {
            Agent holding = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(agentId).GetAgentHolding();
            return PriceValue.GetActualPrice(WADataProvider.WA, priceNameId, productId,
                agentId, date.HasValue ? date.Value : DateTime.Today, agentClientId: agentToId, agentHoldingId: holding.Id);
        }

        /// <summary>Цена последнего прихода товара использовать в торговых документах</summary>
        /// <param name="date">Дата</param>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="supplyerId">Идентификатор корреспондента поставщика (MainClientDepatmentId)</param>
        /// <param name="agentId">Идентификатор компании (MainCompanyDepatmentId)</param>
        /// <returns></returns>
        public static decimal GetLastPriceIn(DateTime date, int productId, int supplyerId, int agentId)
        {
            if (productId == 0) return 0;
            using (SqlConnection cnn = WADataProvider.WA.GetDatabaseConnection())
            {
                if (cnn == null) return 0;

                try
                {
                    using (SqlCommand sqlCmd = cnn.CreateCommand())
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.CommandText = WADataProvider.WA.Empty<Product>().Entity.FindMethod("Sales.GetLastPriceIn").FullName;
                        sqlCmd.Parameters.Add(GlobalSqlParamNames.ProductId, SqlDbType.Int).Value = productId;
                        sqlCmd.Parameters.Add(GlobalSqlParamNames.Date, SqlDbType.DateTime).Value = date;
                        if (supplyerId != 0)
                            sqlCmd.Parameters.Add(GlobalSqlParamNames.AgentFromId, SqlDbType.Int).Value = supplyerId;

                        object val = sqlCmd.ExecuteScalar();
                        return (decimal)val;
                    }
                }
                finally
                {
                    if (cnn.State != ConnectionState.Closed)
                        cnn.Close();
                }
            }
        }

        /// <summary>Цена последнего прихода товара использовать в документах услуг</summary>
        /// <param name="date">Дата</param>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="supplyerId">Идентификатор корреспондента поставщика (MainClientDepatmentId)</param>
        /// <param name="agentId">Идентификатор компании (MainCompanyDepatmentId)</param>
        /// <returns></returns>
        public static decimal GetLastPriceInService(DateTime date, int productId, int supplyerId, int agentId)
        {
            if (productId == 0) return 0;
            using (SqlConnection cnn = WADataProvider.WA.GetDatabaseConnection())
            {
                if (cnn == null) return 0;

                try
                {
                    using (SqlCommand sqlCmd = cnn.CreateCommand())
                    {
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.CommandText = WADataProvider.WA.Empty<Product>().Entity.FindMethod("Services.GetLastPriceIn").FullName;
                        sqlCmd.Parameters.Add(GlobalSqlParamNames.ProductId, SqlDbType.Int).Value = productId;
                        sqlCmd.Parameters.Add(GlobalSqlParamNames.Date, SqlDbType.DateTime).Value = date;
                        if (supplyerId != 0)
                            sqlCmd.Parameters.Add(GlobalSqlParamNames.AgentFromId, SqlDbType.Int).Value = supplyerId;

                        object val = sqlCmd.ExecuteScalar();
                        return (decimal)val;
                    }
                }
                finally
                {
                    if (cnn.State != ConnectionState.Closed)
                        cnn.Close();
                }
            }
        }
        /// <summary>
        /// Текущие настроки документов для предприятия
        /// </summary>
        public static ConfigPrices CurrentConfig()
        {
            ConfigPrices config = ConfigPrices.GetConfig(WADataProvider.WA, WADataProvider.CurrentUser.MyCompanyId);
            return config;
        }
    }
}