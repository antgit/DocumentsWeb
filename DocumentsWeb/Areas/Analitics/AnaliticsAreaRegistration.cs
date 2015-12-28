using System.Web.Mvc;

namespace DocumentsWeb.Areas.Analitics
{
    public class AnaliticsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Analitics";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Analitics_default",
                "Analitics/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
