using System.Web.Mvc;

namespace DocumentsWeb.Areas.UserPersonal
{
    public class UserPersonalAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "UserPersonal";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
                                    
            //context.MapRoute(
            //    "UserPage",
            //    "UserPersonal/Home/UserPage/{pageName}",
            //    new {controller = "Home", action = "UserPage", pageName = UrlParameter.Optional }                
            //);

           
            context.MapRoute(
                "UserPersonal_default",
                "UserPersonal/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
