using System.Data;
using System.Linq;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;
using BusinessObjects;
using System.Web.Mvc;

namespace DocumentsWeb.Areas.SalesNds.Controllers
{
    //<%= Html.ActionLink<HomeController>(c => c.Index(), "Home") %> 
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALESNDS})]
    public class HomeController : CoreController
    {
        public HomeController()
        {
            Name = WebModuleNames.WEB_DOCSALENDS;
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
            HomePageLayoutSettings.CreateFromRequest(HttpContext.Request.Params).SaveLayoutToDatabase(WADataProvider.CurrentUser.Id, "SalesNdsHome");
        }

        public ActionResult ViewBoardAccountInPartial(bool refresh = false)
        {
            DataTable tbl = SalesHelper.GetDocumentsAccount(true, Folder.CODE_FIND_SALES_IN_ACCOUNT_NDS, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardAccountOutPartial(bool refresh = false)
        {
            DataTable tbl = SalesHelper.GetDocumentsAccount(false, Folder.CODE_FIND_SALES_OUT_ACCOUNT_NDS, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardOutPartial(bool refresh = false)
        {
            DataTable tbl = SalesHelper.GetDocuments(false, Folder.CODE_FIND_SALES_OUT_NDS, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardInPartial(bool refresh = false)
        {
            DataTable tbl = SalesHelper.GetDocuments(true, Folder.CODE_FIND_SALES_IN_NDS, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }

        public ActionResult ViewSalesNdsProductPrices()
        {
            return View(SalesHelper.GetProductPrices());
        }
        public ActionResult ViewSalesNdsProductPricesPartial()
        {
            return PartialView(SalesHelper.GetProductPrices().Where(s=>s.KindValue== Product.KINDVALUE_PRODUCT && s.StateId== State.STATEACTIVE));
        }

    }
}
