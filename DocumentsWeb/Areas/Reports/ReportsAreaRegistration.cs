using System.Web.Mvc;
using System.Web.Routing;

namespace DocumentsWeb.Areas.Reports
{
    public class ReportsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Reports";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            Route r = context.MapRoute(
                "Reports_Printform",
                "Reports/Printform/{action}/{docid}/{repid}/{docprint}",
                new { controller = "Printform", action = "Index", docid = 0, repid = 0, docprint = "none" }
            );
            r.DataTokens["area"] = "Reports";


            context.MapRoute(
                "Reports_default",
                "Reports/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );



            //r = context.MapRoute(
            //    "Reports_Printform_GetReportSnapshot",
            //    "Reports/Printform/GetReportSnapshot/{id}/{repid}/{docprint}",
            //    new { controller = "Printform", action = "GetReportSnapshot", id = 0, repid = 0, docprint = "none" }
            //);
            //r.DataTokens["area"] = "Reports";
        }
    }
}
