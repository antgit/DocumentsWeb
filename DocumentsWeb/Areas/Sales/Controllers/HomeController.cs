using System.Data;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;
using BusinessObjects;
using System.Web.Mvc;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class HomeController : CoreController
    {
        public HomeController()
        {
            Name = WebModuleNames.WEB_DOCSALE;
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
            HomePageLayoutSettings.CreateFromRequest(HttpContext.Request.Params).SaveLayoutToDatabase(WADataProvider.CurrentUser.Id, "SalesHome");
        }

        public ActionResult ViewBoardAccountInPartial(bool refresh=false)
        {
            DataTable tbl = SalesHelper.GetDocumentsAccount(true, Folder.CODE_FIND_SALES_ACCOUNT_IN, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardAccountOutPartial(bool refresh = false)
        {
            DataTable tbl = SalesHelper.GetDocumentsAccount(false, Folder.CODE_FIND_SALES_ACCOUNT_OUT, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardOutPartial(bool refresh = false)
        {
            DataTable tbl = SalesHelper.GetDocuments(false, Folder.CODE_FIND_SALES_OUT, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardInPartial(bool refresh = false)
        {
            DataTable tbl = SalesHelper.GetDocuments(true, Folder.CODE_FIND_SALES_IN, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }

    }
}
