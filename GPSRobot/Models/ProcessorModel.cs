using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace GPSRobot.Models
{
    public class ProcessorModel
    {
        /// <summary>
        /// Записывает показания GPS в базу данных
        /// </summary>
        /// <param name="devId">Идентификатор устройства IMEI</param>
        /// <param name="dt">Дата и время</param>
        /// <param name="MsgId">Идентификатор сообщения</param>
        /// <param name="GpsData">Коллекция параметров</param>
        public static void WriteGpsData(string devId, DateTime dt, int MsgId, string[] GpsData)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["GPSRobot"].ConnectionString))
            {
                con.Open();

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "Route.XInsertGPSData";

                cmd.Parameters.Add("@DevCode", System.Data.SqlDbType.VarChar, 100).Value = devId;
                cmd.Parameters.Add("@MsgId", System.Data.SqlDbType.Int).Value = MsgId;
                cmd.Parameters.Add("@Date", System.Data.SqlDbType.Date).Value = dt.Date;
                cmd.Parameters.Add("@Time", System.Data.SqlDbType.Time).Value = dt.TimeOfDay;
                cmd.Parameters.Add("@X", System.Data.SqlDbType.Money).Value = decimal.Parse(GpsData[0], System.Globalization.CultureInfo.InvariantCulture);
                cmd.Parameters.Add("@Y", System.Data.SqlDbType.Money).Value = decimal.Parse(GpsData[1], System.Globalization.CultureInfo.InvariantCulture);
                cmd.Parameters.Add("@Z", System.Data.SqlDbType.Money).Value = decimal.Parse(GpsData[2], System.Globalization.CultureInfo.InvariantCulture);
                cmd.Parameters.Add("@Speed", System.Data.SqlDbType.Money).Value = decimal.Parse(GpsData[3], System.Globalization.CultureInfo.InvariantCulture) * (decimal)1.852;
                cmd.Parameters.Add("@Direction", System.Data.SqlDbType.Int).Value = decimal.Parse(GpsData[4], System.Globalization.CultureInfo.InvariantCulture);
                cmd.Parameters.Add("@Odometr", System.Data.SqlDbType.Money).Value = decimal.Parse(GpsData[5], System.Globalization.CultureInfo.InvariantCulture);
                cmd.Parameters.Add("@HDOP", System.Data.SqlDbType.Int).Value = int.Parse(GpsData[6], System.Globalization.CultureInfo.InvariantCulture);
                cmd.Parameters.Add("@Satellites", System.Data.SqlDbType.Int).Value = int.Parse(GpsData[7], System.Globalization.CultureInfo.InvariantCulture);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Записывает показания датчиков в базу данных
        /// </summary>
        /// <param name="devId">Идентификатор устройства</param>
        /// <param name="dt">Дата и время</param>
        /// <param name="GPioData">Коллекция параметров</param>
        public static void WriteGPioData(string devId, DateTime dt, string[] GPioData) {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["GPSRobot"].ConnectionString))
            {
                con.Open();

                SqlCommand cmd = con.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "Route.XInsertGPIOData";

                cmd.Parameters.Add("@DevCode", System.Data.SqlDbType.VarChar, 100).Value = devId;
                cmd.Parameters.Add("@Date", System.Data.SqlDbType.Date).Value = dt.Date;
                cmd.Parameters.Add("@Time", System.Data.SqlDbType.Time).Value = dt.TimeOfDay;

                for (int i = 0; i < GPioData.Length; i++)
                {
                    decimal val;
                    if (decimal.TryParse(GPioData[i], System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out val))
                    {
                        cmd.Parameters.Add("@Value" + (i + 1).ToString(), System.Data.SqlDbType.Money).Value = val;
                    }
                    else
                    {
                        string msgId = GPioData[i];
                        if (msgId.StartsWith("m"))
                        {
                            int id = int.Parse(msgId.Substring(1));
                            cmd.Parameters.Add("@MessageId" + (i + 1).ToString(), System.Data.SqlDbType.Int).Value = id;
                        }
                        else
                        {
                            throw new Exception("Ошибка входного параметра Value" + (i + 1).ToString());
                        }
                    }
                }
                cmd.ExecuteNonQuery();
            }
        }
    }
}