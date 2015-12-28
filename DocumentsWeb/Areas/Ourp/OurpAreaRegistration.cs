using System.Web.Mvc;

namespace DocumentsWeb.Areas.OURP
{
	public class OurpAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Ourp";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Ourp_default",
                "Ourp/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
