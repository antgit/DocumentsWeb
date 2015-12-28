using System.Web.Mvc;

namespace DocumentsWeb.Areas.Marketings
{
    public class MarketingsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Marketings";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Marketings_default",
                "Marketings/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
