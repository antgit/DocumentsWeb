using System.Web.Mvc;
using BusinessObjects;
using DocumentsWeb.Controllers;
using BusinessObjects.Security;

namespace DocumentsWeb.Areas.Kb.Controllers
{
    [MultiAuthorize(Roles = new[] {Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class HomeController : CoreController<Knowledge>
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
