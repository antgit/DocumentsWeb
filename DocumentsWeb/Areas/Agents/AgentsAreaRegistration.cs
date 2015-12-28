using System.Web.Mvc;

namespace DocumentsWeb.Areas.Agents
{
    public class AgentsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Agents";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Agents_default",
                "Agents/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
