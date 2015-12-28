using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Routes.Models
{
    /// <summary>
    /// Трек
    /// </summary>
    public class TrackModel
    {
        /// <summary>
        /// Идентификатор трека
        /// </summary>
        public string TrackId { get; set; }

        /// <summary>
        /// Идентификатор участника маршрута
        /// </summary>
        public int RouteMemberId { get; set; }

        /// <summary>
        /// Наименование участника маршрута
        /// </summary>
        public string RouteMemberName { get; set; }

        /// <summary>
        /// Дата трека
        /// </summary>
        public DateTime TrackDate { get; set; }

        /// <summary>
        /// Моя компания
        /// </summary>
        public int MyCompanyId { get; set; }

        public static List<TrackModel> GetCollection()
        {
            List<TrackModel> list = new List<TrackModel>();

            SqlConnection con = new SqlConnection(WADataProvider.WA.ConnectionString);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "Route.XGetTracksList";

            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                TrackModel model = new TrackModel
                {
                    TrackDate = rd.GetDateTime(0),
                    RouteMemberId = rd.GetInt32(1),
                    RouteMemberName = rd.GetString(2),
                    TrackId = String.Format("{0:yyyy-MM-dd}", rd.GetDateTime(0)) + "_" + rd.GetInt32(1).ToString(),
                    MyCompanyId = rd.GetInt32(3)
                };
                list.Add(model);
            }

            rd.Close();
            con.Close();

            return list.Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).ToList();
        }
    }
}