using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Agents.Models;
using System.Data.SqlClient;
using DevExpress.Web.ASPxEditors;

namespace DocumentsWeb.Areas.Routes.Models
{
    public class ZoneModel
    {
        /// <summary>
        /// Компания
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// Широта
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        /// Радиус в метрах
        /// </summary>
        public int Radius { get; set; }

        /// <summary>
        /// Адрес
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор корреспондента
        /// </summary>
        public int AgentId { get; set; }
	                
        /// <summary>
        /// Идентификатор адреса
        /// </summary>
	    public int AddressId { get; set; }
	                
        /// <summary>
        /// Страна
        /// </summary>
        public string Country { get; set; }
	    
        /// <summary>
        /// Город
        /// </summary>
        public string Town { get; set; }

        /// <summary>
        /// Область
        /// </summary>
        public string Territory { get; set; }

        /// <summary>
        /// Улица
        /// </summary>
        public string Street { get; set; }

        /// <summary>
        /// Номер дома
        /// </summary>
        public string HouseNumber { get; set; }

        public int Number { get; set; }

        /// <summary>
        /// Загружает зону из базы данных
        /// </summary>
        /// <param name="zoneId">Идентификатор зоны</param>
        /// <returns>Зона</returns>
        public static ZoneModel GetZone(int zoneId) {
            if (zoneId == 0) return null;

            ZoneModel zone = new ZoneModel();
            Agent ag = (Agent)WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(zoneId);
            /*AgentAddress ad = ag.AddressCollection.Count > 0 ? ag.AddressCollection[0] : null;*/
            AgentAddressModel adm = AgentAddressModel.GetMktgAddressByAgentId(zoneId);

            zone.Name = ag.Name;
            if (adm != null)
            {
                zone.X = adm.X ?? 0;
                zone.Y = adm.Y ?? 0;
                zone.Radius = adm.ZoneRadius;
                zone.Address = adm.GetShortName();
                zone.AddressId = adm.Id;
            }

            return zone;
        }

        /// <summary>
        /// Коллекция геозон
        /// </summary>
        /// <returns>Коллекция геозон</returns>
        public static List<ZoneModel> GetZonesList()
        {
            List<ZoneModel> list = new List<ZoneModel>();
            SqlConnection con = new SqlConnection(WADataProvider.WA.ConnectionString);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "Route.XGetZonesList";
            cmd.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar, 255).Value = WADataProvider.CurrentUser.Name;
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                ZoneModel model = new ZoneModel
                {
                    AgentId = rd.GetInt32(0),
                    Name = rd.GetString(1),
                    AddressId = rd.IsDBNull(3) ? 0 : rd.GetInt32(3),
                    Country = rd.IsDBNull(4) ? null : rd.GetString(4),
                    Town = rd.IsDBNull(5) ? null : rd.GetString(5),
                    Territory = rd.IsDBNull(6) ? null : rd.GetString(6),
                    Street = rd.IsDBNull(7) ? null : rd.GetString(7),
                    HouseNumber = rd.IsDBNull(8) ? null : rd.GetString(8),
                    X = rd.IsDBNull(9) ? 0 : rd.GetDecimal(9),
                    Y = rd.IsDBNull(10) ? 0 : rd.GetDecimal(10),
                    Radius = rd.IsDBNull(11) ? 0 : rd.GetInt32(11),
                    CompanyName = rd.IsDBNull(12) ? null : rd.GetString(12)
                };
                list.Add(model);
            }
            rd.Close();
            con.Close();
            return list;
        }

        #region Селектор
        /// <summary>
        /// Возвращает выборку зон для поиска по имени
        /// </summary>
        /// <param name="args">Параметры выборки</param>
        /// <returns>Коллекция зон</returns>
        public static object GetZonesRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            string name = args.Filter;
            if (name == null || name.Length == 0) return null;
            List<ZoneModel> list = new List<ZoneModel>();
            SqlConnection con = new SqlConnection(WADataProvider.WA.ConnectionString);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "Route.XGetZonesListByName";
            cmd.Parameters.Add("@UserName", System.Data.SqlDbType.VarChar, 255).Value = WADataProvider.WA.CurrentUser.Name;
            cmd.Parameters.Add("@Name", System.Data.SqlDbType.VarChar, 255).Value = name;

            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                ZoneModel m = new ZoneModel
                {
                    AgentId = rd.IsDBNull(0) ? 0 : rd.GetInt32(0),
                    Name = rd.GetString(1),
                    AddressId = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                    Address = rd.GetString(3)
                };
                list.Add(m);
            }

            rd.Close();
            con.Close();
            return list;
        }

        /// <summary>
        /// Возвращает зону по идентификатору
        /// </summary>
        /// <param name="args">Параметры фильра</param>
        /// <returns></returns>
        public static object GetZoneById(ListEditItemRequestedByValueEventArgs args)
        {
            if (args.Value != null)
            {
                int id = (int)args.Value;
                ZoneModel zone = ZoneModel.GetZone(id);
                return zone;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// Возвращает коллекцию зон из маршрута
        /// </summary>
        /// <param name="RouteId"></param>
        /// <returns></returns>
        public static List<ZoneModel> GetZonesFromRoute(int RouteId)
        {
            List<ZoneModel> list = new List<ZoneModel>();

            SqlConnection con = new SqlConnection(WADataProvider.WA.ConnectionString);
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "Route.XGetZonesFromRoute";
            cmd.Parameters.AddWithValue("@RouteId", RouteId);
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                ZoneModel m = ZoneModel.GetZone(Convert.ToInt32(rd["AgentId"]));
                m.Number = Convert.ToInt32(rd["OrderNo"]) + 1;
                list.Add(m);
            }
            rd.Close();
            con.Close();

            return list;
        }
    }
}