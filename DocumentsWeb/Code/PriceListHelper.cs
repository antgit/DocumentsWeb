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
    /// ������ � ����������� � ������� "���������� ������"
    /// </summary>
    public static class PriceListHelper
    {
        /// <summary>
        /// �������� ��������� - ���� �����-�����
        /// </summary>
        /// <param name="folderCodeFind">��� ������ �����</param>
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
        /// �������� ��������� - ���� �������������� �����-�����
        /// </summary>
        /// <param name="folderCodeFind">��� ������ �����</param>
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
        /// �������� ��������� - ���� �����-�����
        /// </summary>
        /// <param name="folderCodeFind">��� ������ �����</param>
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
        /// �������� ��������� - ���� �������������� �����-�����
        /// </summary>
        /// <param name="folderCodeFind">��� ������ �����</param>
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
        /// �������� �������� - �����-���� �����������
        /// </summary>
        /// <param name="folderCodeFind">��� ������ �����</param>
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
        /// �������� �������� - �����-���� �����������
        /// </summary>
        /// <param name="folderCodeFind">��� ������ �����</param>
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
        /// ������� ���� �� ����� 
        /// </summary>
        /// <param name="priceNameId">������������� ���� ����</param>
        /// <param name="productId">������������� ������</param>
        /// <param name="agentId">������������� ��������</param>
        /// <param name="date">����, �� ������� ��������� �������� ����. ���� �� ������� - ������������� �������</param>
        /// <param name="agentToId">������������� ����������</param>
        /// <returns></returns>
        public static decimal GetPriceOut(int priceNameId, int productId, int agentId, DateTime? date, int? agentToId = null)
        {
            Agent holding = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(agentId).GetAgentHolding();
            return PriceValue.GetActualPrice(WADataProvider.WA, priceNameId, productId,
                agentId, date.HasValue ? date.Value : DateTime.Today, agentClientId: agentToId, agentHoldingId: holding.Id);
        }

        /// <summary>���� ���������� ������� ������ ������������ � �������� ����������</summary>
        /// <param name="date">����</param>
        /// <param name="productId">������������� ������</param>
        /// <param name="supplyerId">������������� �������������� ���������� (MainClientDepatmentId)</param>
        /// <param name="agentId">������������� �������� (MainCompanyDepatmentId)</param>
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

        /// <summary>���� ���������� ������� ������ ������������ � ���������� �����</summary>
        /// <param name="date">����</param>
        /// <param name="productId">������������� ������</param>
        /// <param name="supplyerId">������������� �������������� ���������� (MainClientDepatmentId)</param>
        /// <param name="agentId">������������� �������� (MainCompanyDepatmentId)</param>
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
        /// ������� �������� ���������� ��� �����������
        /// </summary>
        public static ConfigPrices CurrentConfig()
        {
            ConfigPrices config = ConfigPrices.GetConfig(WADataProvider.WA, WADataProvider.CurrentUser.MyCompanyId);
            return config;
        }
    }
}