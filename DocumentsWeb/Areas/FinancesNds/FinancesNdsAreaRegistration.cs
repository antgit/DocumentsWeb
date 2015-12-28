using System.Web.Mvc;

namespace DocumentsWeb.Areas.FinancesNds
{
    public class FinancesNdsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "FinancesNds";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "FinancesNds_default",
                "FinancesNds/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
