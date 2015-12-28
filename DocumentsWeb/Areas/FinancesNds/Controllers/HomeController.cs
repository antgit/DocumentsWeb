using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;
using System.Web.Mvc;

namespace DocumentsWeb.Areas.FinancesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPFINANCENDS})]
    public class HomeController : CoreController
    {
        public HomeController()
        {
            Name = WebModuleNames.WEB_DOCFINANCENDS;
        }

        public ActionResult Index()
        {
            ViewResult result = View();
            if (!string.IsNullOrEmpty(HelpDefaultLink))
                result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        public ActionResult IndexPartial()
        {
            return PartialView();
        }

        public void SaveLayout()
        {
            HomePageLayoutSettings.CreateFromRequest(HttpContext.Request.Params).SaveLayoutToDatabase(WADataProvider.CurrentUser.Id, "FinancesNdsHome");
        }
        
        public ActionResult ViewBoardFinanceInPartial()
        {
            return PartialView();
        }

       
        public ActionResult ViewBoardFinanceOutPartial()
        {
            return PartialView();
        }

    }
}
