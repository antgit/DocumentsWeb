using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using BusinessObjects.Security;
using System.Web.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;
using BusinessObjects;
using System.Web.Mvc;
using System.Web.Routing;

namespace DocumentsWeb.Areas.Marketings.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPMKTG, Uid.GROUP_GROUPWEBADMIN })]
    public class HomeController : CoreController
    {
        public HomeController()
        {
            Name = WebModuleNames.WEB_DOCMKTG;
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
            HomePageLayoutSettings.CreateFromRequest(HttpContext.Request.Params).SaveLayoutToDatabase(WADataProvider.CurrentUser.Id, "MarketingsHome");
        }

        public ActionResult ViewBoardMrak(bool refresh = false)
        {
            DataTable tbl = MktgHelper.GetDocumentsMrak(refresh, 1, 10, new DateTime(2011, 1, 1), new DateTime(2020, 1, 1));
//            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl);
        }

        public ActionResult ViewBoardMrakOnControl(bool refresh = false)
        {
            DataTable tbl = MktgHelper.GetDocumentsMrakNeedCorrect(refresh,10);
            return PartialView(tbl);
        }
        
    }
}
