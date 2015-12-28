using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Routes.Models
{
    public class RouteMemberLightModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Направление
        /// </summary>
        public decimal Direction { get; set; }

        /// <summary>
        /// X
        /// </summary>
        public decimal X { get; set; }

        /// <summary>
        /// Y
        /// </summary>
        public decimal Y { get; set; }

        /// <summary>
        /// Скорость
        /// </summary>
        public decimal Speed { get; set; }

        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Время
        /// </summary>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// Строковое представление даты и времени
        /// </summary>
        public string Date_Time { get; set; }

        public static RouteMemberLightModel GetLastPosition(int Id) 
        {
            RouteMemberLightModel m = null;
            SqlConnection con = new SqlConnection(WADataProvider.WA.ConnectionString);
            con.Open();
            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "Route.XGetLastRouteMemberPosition";
            cmd.Parameters.AddWithValue("@Id", Id);
            SqlDataReader rd = cmd.ExecuteReader();
            if (rd.Read())
            {
                DateTime date = (DateTime)rd["Date"] + (TimeSpan)rd["Time"];
                date = date.ToLocalTime();
                m = new RouteMemberLightModel
                {
                    Id = (int)rd["Id"],
                    Name = (string)rd["Name"],
                    X = (decimal)rd["X"],
                    Y = (decimal)rd["Y"],
                    Speed = (decimal)rd["Speed"],
                    Date = (DateTime)rd["Date"],
                    Time = (TimeSpan)rd["Time"],
                    Direction = (decimal)rd["Direction"],
                    Date_Time = String.Format("{0:dd.MM.yyyy HH:mm}", date)
                };
            }
            rd.Close();
            con.Close();
            return m;
        }
    }
}