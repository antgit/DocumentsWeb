using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;

namespace DocumentsWeb.Areas.OURP.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class HomeController : Controller
    {
        //
        // GET: /OURP/Home/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult IndexPartial()
		{
			return PartialView();
		}
    }
}
