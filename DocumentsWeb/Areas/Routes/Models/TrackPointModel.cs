using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Routes.Models
{
    /// <summary>
    /// Точка трека
    /// </summary>
    public class TrackPointModel
    {
        /// <summary>
        /// Широта
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        //public DateTime? Date { get; set; }
        
        /// <summary>
        /// Время
        /// </summary>
        //public TimeSpan? Time { get; set; }
        public string stringDate { get; set; }

        /// <summary>
        /// Направление
        /// </summary>
        public int Direction { get; set; }

        /// <summary>
        /// Наименование участника маршрута
        /// </summary>
        public string RouteMemberName { get; set; }

        /// <summary>
        /// Скорость
        /// </summary>
        public double Speed { get; set; }

        public static List<TrackPointModel> GetCollection(string trackId)
        {
            List<TrackPointModel> list = new List<TrackPointModel>();

            DateTime TrackDate = DateTime.Now;
            int RouteMemeberId = 0;

            if (trackId.Length > 0)
            {
                TrackDate = DateTime.Parse(trackId.Split('_')[0]);
                RouteMemeberId = int.Parse(trackId.Split('_')[1]);
            }

            SqlConnection con = new SqlConnection(WADataProvider.WA.ConnectionString);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "Route.XGetTrack";
            cmd.Parameters.AddWithValue("@Date", TrackDate);
            cmd.Parameters.AddWithValue("@RouteMemberId", RouteMemeberId);

            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                DateTime Date = (DateTime)rd["Date"];
                TimeSpan Time = (TimeSpan)rd["Time"];
                //decimal Direction = (decimal)rd["Direction"];
                DateTime date = Date + Time;
                date = date.ToLocalTime();
                list.Add(new TrackPointModel
                {
                    X = Convert.ToDouble(rd["X"]),
                    Y = Convert.ToDouble(rd["Y"]),
                    Speed = rd.IsDBNull(rd.GetOrdinal("Speed")) ? 0.0 : Convert.ToDouble(rd["Speed"]),
                    RouteMemberName = (string)rd["RouteMemberName"],
                    stringDate = String.Format("{0:dd.MM.yyyy HH:mm}", date)
                });
                var pos = list.Count - 1;
                if (pos > 1)
                {
                    var X1 = list[pos - 1].X;
                    var Y1 = list[pos - 1].Y;

                    var X2 = list[pos].X;
                    var Y2 = list[pos].Y;

                    var dX = Math.Round(X1 - X2, 4);
                    var dY = Math.Round(Y1 - Y2, 4);

                    if ((dX > -0.02 && dX <= 0) && (dY < 0.0002 && dY > -0.0002))
                    {
                        list[pos - 1].Direction = 0;
                    }
                    else if ((dX >= -0.02 && dX <= -0.0002) && (dY >= -0.02 && dY <= -0.0002))
                    {
                        list[pos - 1].Direction = 1;
                    }
                    else if ((dX > -0.0002 && dX < 0.0002) && (dY > -0.02 && dY <= 0))
                    {
                        list[pos - 1].Direction = 2;
                    }
                    else if ((dX >= 0.0002 && dX <= 0.02) && (dY >= -0.02 && dY <= -0.0002))
                    {
                        list[pos - 1].Direction = 3;
                    }
                    else if ((dX < 0.02 && dX >= 0) && (dY > -0.0002 && dY < 0.0002))
                    {
                        list[pos - 1].Direction = 4;
                    }
                    else if ((dX >= 0.0002 && dX <= 0.02) && (dY >= 0.0002 && dY <= 0.02))
                    {
                        list[pos - 1].Direction = 5;
                    }
                    else if ((dX < 0.0002 && dX > -0.0002) && (dY < 0.02 && dY >= 0))
                    {
                        list[pos - 1].Direction = 6;
                    }
                    else if ((dX >= -0.02 && dX <= -0.0002) && (dY >= 0.0002 && dY <= 0.02))
                    {
                        list[pos - 1].Direction = 7;
                    }
                }
                else
                {
                    list[pos].Direction = -1;
                }
            }
            if (list.Count > 0)
            {
                list[list.Count - 1].Direction = -1;
            }
            rd.Close();
            con.Close();

            return list;
        }
    }
}