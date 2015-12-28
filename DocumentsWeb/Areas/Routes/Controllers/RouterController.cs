using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentsWeb.Models;
using System.Web.Script.Serialization;
using DocumentsWeb.Areas.Routes.Models;
using BusinessObjects.Security;
using BusinessObjects;
using BusinessObjects.Documents;
using System.Net;
using System.IO;
using System.Text;

namespace DocumentsWeb.Areas.Routes.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class RouterController : Controller
    {
        //
        // GET: /Routes/Router/

        public ActionResult Index()
        {
            return View();
        }

        #region Треки

        public JsonResult GetTrack(string trackId)
        {
            return this.Json(TrackPointModel.GetCollection(trackId), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Зоны
        public JsonResult GetZone(int zoneId)
        {
            return this.Json(ZoneModel.GetZone(zoneId), JsonRequestBehavior.AllowGet);
        }

        public void SetLatLngToAddr(int AddrId, decimal Lat, decimal Lng, int Radius)
        {
            AgentAddress addr = new AgentAddress { Workarea = WADataProvider.WA };
            addr.Load(AddrId);
            addr.X = Lat;
            addr.Y = Lng;
            addr.ZoneRadius = Radius;
            addr.Save();
        }
        #endregion

        #region Маршруты

        public JsonResult GetRoutePoints(int RouteId)
        {
            JsonResult res = this.Json(RouteModel.GetRoute(RouteId), JsonRequestBehavior.AllowGet);
            return res;
        }

        [HttpPost]
        public void SaveRoute(int RouteId, string Steps)
        {
            string json = Uri.UnescapeDataString(Steps);
            DocumentRoute doc = new DocumentRoute { Workarea = WADataProvider.WA };
            doc.Load(RouteId);
            doc.Document.Memo = json;
            doc.Save();
        }

        //http://localhost:4053/dgmz/Routes/Router/ReqGoogleRoute?start=48.022998,37.801106&end=50.474113,30.52256
        //waypoints=optimize:true|Barossa+Valley,SA|Clare,SA|Connawarra,SA|McLaren+Vale,SA
        public ContentResult ReqGoogleRoute(string start, string end, string waypoints = null)
        {
            byte[] buff = new byte[1024];
            int len = 0;
            string url = "";

            if (waypoints == null || waypoints.Length == 0)
            {
                url = "http://maps.googleapis.com/maps/api/directions/json?origin=" + start + "&destination=" + end + "&sensor=true";
            }
            else
            {
                url = "http://maps.googleapis.com/maps/api/directions/json?origin=" + start + "&destination=" + end + "&waypoints=" + waypoints + "&sensor=true";
            }

            WebRequest req = WebRequest.Create(url);
            req.Proxy.Credentials = CredentialCache.DefaultCredentials;
            WebResponse rsp = req.GetResponse();
            Stream s = rsp.GetResponseStream();
            string value = "";
            while((len = s.Read(buff, 0, buff.Length)) > 0) {
                value += Encoding.UTF8.GetString(buff, 0, len);
            }
            return new ContentResult { Content = value, ContentType = "application/json" };
        }

        #endregion

        #region Участник маршрута

        public JsonResult GetRouteMemberPosition(int Id)
        {
            return this.Json(RouteMemberLightModel.GetLastPosition(Id), JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}