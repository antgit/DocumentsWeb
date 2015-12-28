using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GPSRobot.Models;
using System.Globalization;

namespace GPSRobot.Controllers
{
    public class DataController : Controller
    {
        //
        // GET: /Data/

        public ActionResult Index(string dev, string gprmc)
        {
            try
            {
                NMEA0183Model parser = new NMEA0183Model();
                if (parser.ParseNMEA0183(dev, gprmc))
                {
                    return PartialView("GPRMCMessagePartial", new MessageStateModel { State = true });
                }
                else
                {
                    return PartialView("GPRMCMessagePartial", new MessageStateModel { State = false });
                }
            }
            catch
            {
                return PartialView("GPRMCMessagePartial", new MessageStateModel { State = false });
            }
        }

        public ActionResult Input(string devId, string dt, string MsgId, string GpsData, string GPioData)
        {
            try 
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime date = DateTime.ParseExact(dt, "yyyy.MM.dd-HH.mm.ss", provider);

                if (GpsData != null && GpsData.Length > 0)
                {
                    ProcessorModel.WriteGpsData(devId, date, int.Parse(MsgId), GpsData.Split('|'));
                }

                if (GPioData != null && GPioData.Length > 0)
                {
                    ProcessorModel.WriteGPioData(devId, date, GPioData.Split('|'));
                }

                return PartialView("GPRMCMessagePartial", new MessageStateModel { State = true });
            }
            catch (Exception e)
            {
                string msg = e.Message + "\r\n\r\n" + e.StackTrace;
                return PartialView("GPRMCMessagePartial", new MessageStateModel { State = false, Message = msg });
            }
        }
    }
}
