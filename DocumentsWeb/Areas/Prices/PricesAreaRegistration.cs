using System.Web.Mvc;

namespace DocumentsWeb.Areas.Prices
{
    public class PricesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Prices";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Prices_default",
                "Prices/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
