using System.Web.Mvc;

namespace DocumentsWeb.Areas.Routes
{
    public class RoutesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Routes";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Routes_default",
                "Routes/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
