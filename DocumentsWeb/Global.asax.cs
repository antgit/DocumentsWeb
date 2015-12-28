using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DevExpress.Web.Mvc;

namespace DocumentsWeb
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            /*
             filters.Add(new HandleErrorAttribute
        {
            ExceptionType = typeof(DbException),
            // DbError.cshtml is a view in the Shared folder.
            View = "DbError",
            Order = 2
        });
             */
            filters.Add(new HandleErrorAttribute
            {
                ExceptionType = typeof(DocumentKindIdOpenException),
                View = "ErrorDocumentKindIdOpen",
                Order = 2
            });
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.ashx/{*pathInfo}");

            /*routes.MapRoute(
                "UserPage",
                "Home/UserPage/{pageName}",
                new { controller = "Home", action = "UserPage", pageName = UrlParameter.Optional },
                new[] { "DocumentsWeb.Controllers" }
            );*/

            /*
            routes.MapRoute(
                "UserPage",
                "UserPersonal/Home/UserPage/{pageName}",
                new { controller = "Home", action = "UserPage", pageName = UrlParameter.Optional },
                new[] { "DocumentsWeb.Areas.UserPersonal.Controllers" }
            );*/

            /*Route r = routes.MapRoute(
                "GPSDataReader", // Route name
                "Data", // URL with parameters
                new { area = "Routes", controller = "Data", action = "Index" }, // Parameter defaults
                new[] { "DocumentsWeb.Areas.Routes.Controllers" }
            );
            r.DataTokens["area"] = "Routes";*/

            //Route r = routes.MapRoute(
            //    "Printform", // Route name
            //    "Reports/Printform/Index/{id}/{repId}/{docprint}",
            //    new { area = "Reports", controller = "Printform", action = "Index" }, // Parameter defaults
            //    new[] { "DocumentsWeb.Areas.Reports.Controllers" }
            //);
            //r.DataTokens["area"] = "Reports";

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional  }, // Parameter defaults
                new[] { "DocumentsWeb.Controllers" }
            );

            
        }
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            DevExpressHelper.Theme = Utils.CurrentTheme;
            //DevExpressHelper.Theme = "Aqua";
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        private static Dictionary<string, string> _sessionInfo;
        private static readonly object padlock = new object();
        public static Dictionary<string, string> Sessions
        {
            get
            {
                lock (padlock)
                {
                    if (_sessionInfo == null)
                    {
                        _sessionInfo = new Dictionary<string, string>();
                    }
                    return _sessionInfo;
                }
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (!Sessions.ContainsKey(Session.SessionID))
                Sessions.Add(Session.SessionID, "");
        }
        protected void Session_End(object sender, EventArgs e)
        {
            if (Sessions.ContainsKey(Session.SessionID))
            {
                string value = Sessions[Session.SessionID];

                List<string> itemsToRemove = new List<string>();

                foreach (var pair in Sessions)
                {
                    if (pair.Value.Equals(value))
                        itemsToRemove.Add(pair.Key);
                }

                foreach (string item in itemsToRemove)
                {
                    Sessions.Remove(item);
                } 

                
                //Sessions.Remove(Session.SessionID);
            }
        } 
        

    }
}