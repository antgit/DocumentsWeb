using System.Data;
using System.Threading;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;
using BusinessObjects;
using System.Web.Mvc;

namespace DocumentsWeb.Areas.Services.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICES})]
    public class HomeController : CoreController
    {
        public HomeController()
        {
            Name = WebModuleNames.WEB_DOCSERVICE;
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
            HomePageLayoutSettings.CreateFromRequest(HttpContext.Request.Params).SaveLayoutToDatabase(WADataProvider.CurrentUser.Id, "ServicesHome");
        }

        public ActionResult ViewBoardAccountInPartial(bool refresh = false)
        {
            DataTable tbl = ServicesHelper.GetDocumentsAccounts(true, Folder.CODE_FIND_SERVICE_IN_ACCOUNT, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardAccountOutPartial(bool refresh = false)
        {
            if (DevExpressHelper.IsCallback)
                Thread.Sleep(500);
            DataTable tbl = ServicesHelper.GetDocumentsAccounts(false, Folder.CODE_FIND_SERVICE_OUT_ACCOUNT, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardOutPartial(bool refresh = false)
        {
            DataTable tbl = ServicesHelper.GetDocuments(false, Folder.CODE_FIND_SERVICE_OUT, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardInPartial(bool refresh = false)
        {
            DataTable tbl = ServicesHelper.GetDocuments(true, Folder.CODE_FIND_SERVICE_IN, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";

            return PartialView(tbl.DefaultView.ToTable());
        }
    }
}
