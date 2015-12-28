using System.Web.Mvc;

namespace DocumentsWeb.Areas.Planing
{
    public class PlaningAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Planing";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Planing_default",
                "Planing/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
