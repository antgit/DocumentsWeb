using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Security;

namespace DocumentsWeb.Areas.Admins.Controllers
{
    [Authorize(Roles = Uid.GROUP_GROUPWEBADMIN)]
    public class HomeController : Controller
    {
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
