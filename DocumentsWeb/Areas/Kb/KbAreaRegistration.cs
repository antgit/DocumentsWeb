using System.Web.Mvc;

namespace DocumentsWeb.Areas.Kb
{
    public class KbAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Kb";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Kb_default",
                "Kb/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
