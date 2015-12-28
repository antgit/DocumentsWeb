using System.Data;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;
using BusinessObjects;
using System.Web.Mvc;

namespace DocumentsWeb.Areas.ServicesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICESNDS })]
    public class HomeController : CoreController
    {
        public HomeController()
        {
            Name = WebModuleNames.WEB_DOCSERVICENDS;
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
            HomePageLayoutSettings.CreateFromRequest(HttpContext.Request.Params).SaveLayoutToDatabase(WADataProvider.CurrentUser.Id, "ServicesNdsHome");
        }

        public ActionResult ViewBoardAccountInPartial(bool refresh = false)
        {
            DataTable tbl = ServicesHelper.GetDocumentsAccounts(true, Folder.CODE_FIND_SERVICE_IN_ACCOUNT_NDS, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardAccountOutPartial(bool refresh = false)
        {
            DataTable tbl = ServicesHelper.GetDocumentsAccounts(false, Folder.CODE_FIND_SERVICE_OUT_ACCOUNT_NDS, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardOutPartial(bool refresh = false)
        {
            DataTable tbl = ServicesHelper.GetDocuments(false, Folder.CODE_FIND_SERVICE_OUT_NDS, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardInPartial(bool refresh = false)
        {
            DataTable tbl = ServicesHelper.GetDocuments(true, Folder.CODE_FIND_SERVICE_IN_NDS, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";

            return PartialView(tbl.DefaultView.ToTable());
        }
    }
}
