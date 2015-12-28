using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Routes.Models
{
    public class RouteHeaderModel
    {
        /// <summary>
        /// Идентификатор участника маршрута
        /// </summary>
        public int RouteMemberId { get; set; }

        /// <summary>
        /// Наименование участника маршрута
        /// </summary>
        public string RouteMemberName { get; set; }

        /// <summary>
        /// Идентификатор маршрута
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Наименование маршрута
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// Идентификатор компании
        /// </summary>
        public int MyCompanyId { get; set; }

        /// <summary>
        /// Возвращает коллекцию шапок маршрутов
        /// </summary>
        public static List<RouteHeaderModel> GetRoutesHeaders()
        {
            List<RouteHeaderModel> list = new List<RouteHeaderModel>();
            SqlConnection con = new SqlConnection(WADataProvider.WA.ConnectionString);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "Route.XGetRouteHeaders";
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                RouteHeaderModel model = new RouteHeaderModel
                {
                    RouteMemberId = rd.IsDBNull(0) ? 0 : rd.GetInt32(0),
                    RouteMemberName = rd.IsDBNull(1) ? "" : rd.GetString(1),
                    RouteId = rd.IsDBNull(2) ? 0 : rd.GetInt32(2),
                    RouteName = rd.IsDBNull(3) ? "" : rd.GetString(3),
                    MyCompanyId = rd.IsDBNull(4) ? 0 : rd.GetInt32(4)
                };
                list.Add(model);
            }
            rd.Close();
            con.Close();

            return list.Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).ToList();
        }
    }
}