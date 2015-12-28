using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using DocumentsWeb.Models;
using System.Globalization;

namespace DocumentsWeb.Areas.Routes.Models
{
    public class LoggerModel
    {
        /// <summary>
        /// Идентификатор устройства
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Дата замера
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Время замера
        /// </summary>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// Время в формате JavaScript
        /// </summary>
        public long JavaScriptTimeStamp { get; set; }

        /// <summary>
        /// Значение 1
        /// </summary>
        public decimal Value1 { get; set; }
        /// <summary>
        /// Значение 2
        /// </summary>
        public decimal Value2 { get; set; }
        /// <summary>
        /// Значение 3
        /// </summary>
        public decimal Value3 { get; set; }
        /// <summary>
        /// Значение 4
        /// </summary>
        public decimal Value4 { get; set; }
        /// <summary>
        /// Значение 5
        /// </summary>
        public decimal Value5 { get; set; }
        /// <summary>
        /// Значение 6
        /// </summary>
        public decimal Value6 { get; set; }
        /// <summary>
        /// Значение 7
        /// </summary>
        public decimal Value7 { get; set; }
        /// <summary>
        /// Значение 8
        /// </summary>
        public decimal Value8 { get; set; }

        /// <summary>
        /// Идентификатор сообщения 1
        /// </summary>
        public int MessageId1 { get; set; }
        /// <summary>
        /// Идентификатор сообщения 2
        /// </summary>
        public int MessageId2 { get; set; }
        /// <summary>
        /// Идентификатор сообщения 3
        /// </summary>
        public int MessageId3 { get; set; }
        /// <summary>
        /// Идентификатор сообщения 4
        /// </summary>
        public int MessageId4 { get; set; }
        /// <summary>
        /// Идентификатор сообщения 5
        /// </summary>
        public int MessageId5 { get; set; }
        /// <summary>
        /// Идентификатор сообщения 6
        /// </summary>
        public int MessageId6 { get; set; }
        /// <summary>
        /// Идентификатор сообщения 7
        /// </summary>
        public int MessageId7 { get; set; }
        /// <summary>
        /// Идентификатор сообщения 8
        /// </summary>
        public int MessageId8 { get; set; }

        /// <summary>
        /// Возвращает заданное число последних замеров
        /// </summary>
        /// <param name="DeviceId">Идентификатор устройтсва</param>
        /// <param name="Count">Кол-во точек</param>
        /// <returns></returns>
        public static List<LoggerModel> GetLastValues(int DeviceId, int Count)
        {
            List<LoggerModel> list = new List<LoggerModel>();

            SqlConnection con = new SqlConnection(WADataProvider.WA.ConnectionString);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "Route.XGetLastLoggerValues";
            cmd.Parameters.Add("@DevId", System.Data.SqlDbType.Int).Value = DeviceId;
            cmd.Parameters.Add("@Count", System.Data.SqlDbType.Int).Value = Count;

            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                LoggerModel model = new LoggerModel
                {
                    Date = (DateTime)rd["Date"],
                    Time = (TimeSpan)rd["Time"],
                    JavaScriptTimeStamp = GetJavascriptTimestamp((DateTime)rd["Date"] + (TimeSpan)rd["Time"]),
                    Value1 = rd.IsDBNull(rd.GetOrdinal("Value1")) ? 0 : (decimal)rd["Value1"],
                    Value2 = rd.IsDBNull(rd.GetOrdinal("Value2")) ? 0 : (decimal)rd["Value2"],
                    Value3 = rd.IsDBNull(rd.GetOrdinal("Value3")) ? 0 : (decimal)rd["Value3"],
                    Value4 = rd.IsDBNull(rd.GetOrdinal("Value4")) ? 0 : (decimal)rd["Value4"],
                    Value5 = rd.IsDBNull(rd.GetOrdinal("Value5")) ? 0 : (decimal)rd["Value5"],
                    Value6 = rd.IsDBNull(rd.GetOrdinal("Value6")) ? 0 : (decimal)rd["Value6"],
                    Value7 = rd.IsDBNull(rd.GetOrdinal("Value7")) ? 0 : (decimal)rd["Value7"],
                    Value8 = rd.IsDBNull(rd.GetOrdinal("Value8")) ? 0 : (decimal)rd["Value8"],
                    MessageId1 = rd.IsDBNull(rd.GetOrdinal("MessageId1")) ? 0 : (int)rd["MessageId1"],
                    MessageId2 = rd.IsDBNull(rd.GetOrdinal("MessageId2")) ? 0 : (int)rd["MessageId2"],
                    MessageId3 = rd.IsDBNull(rd.GetOrdinal("MessageId3")) ? 0 : (int)rd["MessageId3"],
                    MessageId4 = rd.IsDBNull(rd.GetOrdinal("MessageId4")) ? 0 : (int)rd["MessageId4"],
                    MessageId5 = rd.IsDBNull(rd.GetOrdinal("MessageId5")) ? 0 : (int)rd["MessageId5"],
                    MessageId6 = rd.IsDBNull(rd.GetOrdinal("MessageId6")) ? 0 : (int)rd["MessageId6"],
                    MessageId7 = rd.IsDBNull(rd.GetOrdinal("MessageId7")) ? 0 : (int)rd["MessageId7"],
                    MessageId8 = rd.IsDBNull(rd.GetOrdinal("MessageId8")) ? 0 : (int)rd["MessageId8"]
                };
                DateTime date = model.Date + model.Time;
                date = date.ToLocalTime();
                model.JavaScriptTimeStamp = GetJavascriptTimestamp(date);
                list.Add(model);
            }

            rd.Close();
            con.Close();

            return list;
        }

        /// <summary>
        /// Возвращает замеры за указанный день
        /// </summary>
        /// <param name="DeviceId">Идентификатор устройтсва</param>
        /// <param name="Date">Дата в строковом виде</param>
        /// <returns></returns>
        public static List<LoggerModel> GetValuesByDate(int DeviceId, string Date)
        {
            List<LoggerModel> list = new List<LoggerModel>();

            SqlConnection con = new SqlConnection(WADataProvider.WA.ConnectionString);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "Route.XGetValuesByDate";
            cmd.Parameters.Add("@DevId", System.Data.SqlDbType.Int).Value = DeviceId;
            cmd.Parameters.Add("@Date", System.Data.SqlDbType.Date).Value = DateTime.ParseExact(Date, "yyyyMMdd", CultureInfo.InvariantCulture);

            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                LoggerModel model = new LoggerModel
                {
                    Date = (DateTime)rd["Date"],
                    Time = (TimeSpan)rd["Time"],
                    JavaScriptTimeStamp = GetJavascriptTimestamp((DateTime)rd["Date"] + (TimeSpan)rd["Time"]),
                    Value1 = rd.IsDBNull(rd.GetOrdinal("Value1")) ? 0 : (decimal)rd["Value1"],
                    Value2 = rd.IsDBNull(rd.GetOrdinal("Value2")) ? 0 : (decimal)rd["Value2"],
                    Value3 = rd.IsDBNull(rd.GetOrdinal("Value3")) ? 0 : (decimal)rd["Value3"],
                    Value4 = rd.IsDBNull(rd.GetOrdinal("Value4")) ? 0 : (decimal)rd["Value4"],
                    Value5 = rd.IsDBNull(rd.GetOrdinal("Value5")) ? 0 : (decimal)rd["Value5"],
                    Value6 = rd.IsDBNull(rd.GetOrdinal("Value6")) ? 0 : (decimal)rd["Value6"],
                    Value7 = rd.IsDBNull(rd.GetOrdinal("Value7")) ? 0 : (decimal)rd["Value7"],
                    Value8 = rd.IsDBNull(rd.GetOrdinal("Value8")) ? 0 : (decimal)rd["Value8"],
                    MessageId1 = rd.IsDBNull(rd.GetOrdinal("MessageId1")) ? 0 : (int)rd["MessageId1"],
                    MessageId2 = rd.IsDBNull(rd.GetOrdinal("MessageId2")) ? 0 : (int)rd["MessageId2"],
                    MessageId3 = rd.IsDBNull(rd.GetOrdinal("MessageId3")) ? 0 : (int)rd["MessageId3"],
                    MessageId4 = rd.IsDBNull(rd.GetOrdinal("MessageId4")) ? 0 : (int)rd["MessageId4"],
                    MessageId5 = rd.IsDBNull(rd.GetOrdinal("MessageId5")) ? 0 : (int)rd["MessageId5"],
                    MessageId6 = rd.IsDBNull(rd.GetOrdinal("MessageId6")) ? 0 : (int)rd["MessageId6"],
                    MessageId7 = rd.IsDBNull(rd.GetOrdinal("MessageId7")) ? 0 : (int)rd["MessageId7"],
                    MessageId8 = rd.IsDBNull(rd.GetOrdinal("MessageId8")) ? 0 : (int)rd["MessageId8"]
                };
                DateTime date = model.Date + model.Time;
                date = date.ToLocalTime();
                model.JavaScriptTimeStamp = GetJavascriptTimestamp(date);
                list.Add(model);
            }

            rd.Close();
            con.Close();

            return list;
        }

        public static long GetJavascriptTimestamp(System.DateTime input)
        {
            System.TimeSpan span = new System.TimeSpan(System.DateTime.Parse("1/1/1970").Ticks);
            System.DateTime time = input.Subtract(span);
            return (long)(time.Ticks / 10000);
        }
    }
}