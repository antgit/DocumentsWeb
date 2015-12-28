using System.Web.Mvc;

namespace DocumentsWeb.Areas.ServicesNds
{
    public class ServicesNdsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ServicesNds";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ServicesNds_default",
                "ServicesNds/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
