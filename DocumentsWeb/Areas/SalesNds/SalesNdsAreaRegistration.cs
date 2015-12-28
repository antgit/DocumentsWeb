using System.Web.Mvc;
using System.Web.Routing;

namespace DocumentsWeb.Areas.SalesNds
{
    public class SalesNdsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SalesNds";
            }
        }
        
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SalesNds_default",
                "SalesNds/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
