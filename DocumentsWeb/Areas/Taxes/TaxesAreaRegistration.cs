using System.Web.Mvc;

namespace DocumentsWeb.Areas.Taxes
{
    public class TaxesAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Taxes";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Taxes_default",
                "Taxes/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
