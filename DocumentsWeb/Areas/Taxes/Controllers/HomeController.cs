using System.Data;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;
using BusinessObjects;
using System.Web.Mvc;

namespace DocumentsWeb.Areas.Taxes.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPTAX })]
    public class HomeController : CoreController
    {
        public HomeController()
        {
            Name = WebModuleNames.WEB_DOCTAX;
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
            HomePageLayoutSettings.CreateFromRequest(HttpContext.Request.Params).SaveLayoutToDatabase(WADataProvider.CurrentUser.Id, "TaxesHome");
        }
        public ActionResult ViewBoardTaxCorInPartial(bool refresh = false)
        {
            DataTable tbl = TaxesHelper.GetDocumentsCor(true, Folder.CODE_FIND_TAX_CORIN, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardTaxCorOutPartial(bool refresh = false)
        {
            DataTable tbl = TaxesHelper.GetDocumentsCor(false, Folder.CODE_FIND_TAX_COROUT, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardTaxOutPartial(bool refresh = false)
        {
            DataTable tbl = TaxesHelper.GetDocuments(false, Folder.CODE_FIND_TAX_OUT, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        public ActionResult ViewBoardTaxInPartial(bool refresh = false)
        {
            DataTable tbl = TaxesHelper.GetDocuments(true, Folder.CODE_FIND_TAX_IN, refresh, 10, State.STATEACTIVE);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }

    }
}
