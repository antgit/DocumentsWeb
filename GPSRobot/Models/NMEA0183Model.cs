using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;

namespace GPSRobot.Models
{
    /// <summary>
    /// Структура запроса по стандарту NMEA 0183
    /// </summary>
    public struct NMEA0183Data
    {
        /// <summary>GPS</summary>
        public bool GP;
        /// <summary>Glonass</summary>
        public bool GN;
        /// <summary>Recomended Minimum sentence C</summary>
        public bool RMC;
        /// <summary>Время в нулевом часовом поясе</summary>
        public TimeSpan utcTime;
        /// <summary>A - Данные достоверны, V - Данные не достоверны</summary>
        public char A;
        /// <summary>Широта</summary>
        public decimal Latitude;
        /// <summary>N - Северная, S - Южная</summary>
        public char P;
        /// <summary>Долгота</summary>
        public decimal Longitude;
        /// <summary>E - Восточная, W - Западная</summary>
        public char J;
        /// <summary>Горизонтальная состовляющая скорости относительно земли в узлах</summary>
        public decimal Speed;
        /// <summary>Угол движения 0 - север, 90 - восток, 180 - юг, 270 - запад</summary>
        public decimal Angle;
        /// <summary>Дата</summary>
        public DateTime Date;
        /// <summary>Магнитное склонение</summary>
        public decimal MagneticDeclination;
        /// <summary>Направление магнитного склонения. E - вычисть, W - прибавить к истинному курсу</summary>
        public char n;
        /// <summary>Индикатор режима. A - автономный, D - дифференциальный, E - аппроксимация, N - недостоверные данные</summary>
        public char m;
        /// <summary>Контрольная сумма</summary>
        public string CheckSumm;
    }

    public class NMEA0183Model
    {
        private bool ControlNMEA0183(string data, NMEA0183Data block)
        {
            string sentence = data;
            int checksum = Convert.ToByte(sentence[sentence.IndexOf('$') + 1]);
            for (int i = sentence.IndexOf('$') + 2; i < sentence.IndexOf('*'); i++)
            {
                checksum ^= Convert.ToByte(sentence[i]);
            }
            if (block.CheckSumm.Equals(checksum.ToString("X2")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ParseNMEA0183(string dev, string data)
        {
            NMEA0183Data d = new NMEA0183Data();
            string[] vals = data.Split(',');
            for (int i = 0; i < vals.Length; i++)
            {
                string val = vals[i];
                switch (i)
                {
                    case 0:
                        if (val.StartsWith("$GP"))
                        {
                            d.GP = true;
                            d.GN = false;
                        }
                        else if (val.StartsWith("$GN"))
                        {
                            d.GP = false;
                            d.GN = true;
                        }
                        if (val.EndsWith("RMC"))
                        {
                            d.RMC = true;
                        }
                        else
                        {
                            d.RMC = false;
                        }
                        break;
                    case 1:
                        if (val.Length != 0)
                        {
                            d.utcTime = TimeSpan.ParseExact(val, "hhmmss", CultureInfo.CurrentCulture);
                        }
                        break;
                    case 2:
                        if (val.Length != 0)
                        {
                            d.A = val[0];
                        }
                        break;
                    case 3:
                        if (val.Length != 0)
                        {
                            decimal g = decimal.Parse(val.Substring(0, 2), System.Globalization.CultureInfo.InvariantCulture);
                            decimal m = decimal.Parse(val.Substring(2), System.Globalization.CultureInfo.InvariantCulture);

                            d.Latitude = g + (m / 60);
                        }
                        break;
                    case 4:
                        if (val.Length != 0)
                        {
                            d.P = val[0];
                        }
                        break;
                    case 5:
                        if (val.Length != 0)
                        {
                            decimal g = decimal.Parse(val.Substring(0, 2), System.Globalization.CultureInfo.InvariantCulture);
                            decimal m = decimal.Parse(val.Substring(2), System.Globalization.CultureInfo.InvariantCulture);

                            d.Longitude = g + (m / 60);
                        }
                        break;
                    case 6:
                        if (val.Length != 0)
                        {
                            d.J = val[0];
                        }
                        break;
                    case 7:
                        if (val.Length != 0)
                        {
                            d.Speed = decimal.Parse(val, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        break;
                    case 8:
                        if (val.Length != 0)
                        {
                            d.Angle = decimal.Parse(val, System.Globalization.CultureInfo.InvariantCulture);
                        }
                        break;
                    case 9:
                        if (val.Length != 0)
                        {
                            d.Date = DateTime.ParseExact(val, "ddMMyy", CultureInfo.CurrentCulture);
                        }
                        break;
                    case 10:
                        if (val.Length != 0)
                        {
                            d.MagneticDeclination = decimal.Parse(val, CultureInfo.CurrentCulture);
                        }
                        break;
                    case 11:
                        if (val.Length != 0)
                        {
                            d.n = val[0];
                        }
                        break;
                    /*case 12:
                        if (val.Length != 0)
                        {
                            d.m = val[0];
                        }
                        break;*/
                    case 12:
                        if (val.Length != 0)
                        {
                            d.CheckSumm = val.Substring(1);
                        }
                        break;
                }
            }
            if (ControlNMEA0183(data, d))
            {
                AddToDataBase(dev, d);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddToDataBase(string dev, NMEA0183Data data)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["GPSRobot"].ConnectionString);
            con.Open();

            SqlCommand cmd = con.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText =
                "DECLARE @DevId INT; " +
                "DECLARE @RouteMemberId INT; " +
                "SELECT @DevId = Id FROM Documents2011DMPZ.Route.Devices WHERE Code = @DevCode; " +
                "SELECT @RouteMemberId = Id FROM Documents2011DMPZ.Route.RouteMembers WHERE DeviceId = @DevId; " +
                "INSERT INTO " +
                    "Documents2011DMPZ.Route.RouteEvents(RouteMemberId, DeviceId, X, Y, Speed, Date, Time, Direction, StateId, DatabaseId) " +
                "VALUES (@RouteMemberId, @DevId, @X, @Y, @SPEED, @DATE, @TIME, @DIRECTION, 1, 1); ";
            cmd.Parameters.Add("@DevCode", System.Data.SqlDbType.VarChar, 100).Value = dev;
            cmd.Parameters.Add("@X", System.Data.SqlDbType.Money).Value = data.Latitude;
            cmd.Parameters.Add("@Y", System.Data.SqlDbType.Money).Value = data.Longitude;
            cmd.Parameters.Add("@Speed", System.Data.SqlDbType.Money).Value = data.Speed * (decimal)1.852;
            cmd.Parameters.Add("@DATE", System.Data.SqlDbType.Date).Value = data.Date;
            cmd.Parameters.Add("@TIME", System.Data.SqlDbType.Time, 7).Value = data.utcTime;
            cmd.Parameters.Add("@DIRECTION", System.Data.SqlDbType.Int).Value = Math.Round(data.Angle);
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}