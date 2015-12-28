using System.Web.Mvc;
using BusinessObjects.Security;

namespace DocumentsWeb.Areas.Analitics.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
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

        #region AnaliticsFinder
        public ActionResult AnaliticsFinderGridPartial()
        {
            string Name = (string)Request.Params["Name"];
            string hierarchyCode = (string)Request.Params["hierarchyCode"];

            PartialViewResult result = PartialView();
            result.ViewData.Add("Name", Name);
            result.ViewData.Add("hierarchyCode", hierarchyCode);
            return result;
        }
        #endregion
    }
}
