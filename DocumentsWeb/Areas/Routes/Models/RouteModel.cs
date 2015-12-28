using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects.Documents;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Routes.Models
{
    public class RouteModel
    {
        private List<ZoneModel> _zones;
        /// <summary>
        /// Коллекция зон
        /// </summary>
        public List<ZoneModel> Zones
        {
            get { return _zones; }
            set { _zones = value; }
        }
        /// <summary>
        /// Маршрут
        /// </summary>
        public string Route { get; set; }

        public RouteModel()
        {
            _zones = new List<ZoneModel>();
        }

        public static RouteModel GetRoute(int RouteId)
        {
            DocumentRoute doc = new DocumentRoute { Workarea = WADataProvider.WA };
            doc.Load(RouteId);
            RouteModel model = new RouteModel
            {
                Zones = ZoneModel.GetZonesFromRoute(RouteId),
                Route = doc.Document.Memo
            };
            return model;
        }
    }
}