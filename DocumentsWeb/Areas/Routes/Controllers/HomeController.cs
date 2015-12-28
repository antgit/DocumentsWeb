using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;

namespace DocumentsWeb.Areas.Routes.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class HomeController : Controller
    {
        //
        // GET: /Routes/Home/

        public ActionResult Index()
        {
            return View();
        }

    }
}
