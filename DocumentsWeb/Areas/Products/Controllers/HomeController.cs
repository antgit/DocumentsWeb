using System.Web.Mvc;
using BusinessObjects.Security;

namespace DocumentsWeb.Areas.Products.Controllers
{
    /// <summary>
    /// Контроллер раздела "Справочники объектов учета"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER})]
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

        #region ProductsFinder
        public ActionResult ProductsFinderGridPartial()
        {
            string name = Request.Params["Name"];
            bool showOnlyServices = bool.Parse(Request.Params["showOnlyServices"]);
            bool showOnlyProducts = bool.Parse(Request.Params["showOnlyProducts"]);

            PartialViewResult result = PartialView();
            result.ViewData.Add("Name", name);
            result.ViewData.Add("showOnlyServices", showOnlyServices);
            result.ViewData.Add("showOnlyProducts", showOnlyProducts);
            return result;
        }
        #endregion
    }
}
