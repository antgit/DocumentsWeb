using System;
using System.Data;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;
using BusinessObjects;
using System.Web.Mvc;

namespace DocumentsWeb.Areas.Contracts.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPCONTRACTS})]
    public class HomeController : CoreController
    {
        public HomeController()
        {
            Name = WebModuleNames.WEB_DOCDOGOVOR;
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
            HomePageLayoutSettings.CreateFromRequest(HttpContext.Request.Params).SaveLayoutToDatabase(WADataProvider.CurrentUser.Id, "ContractsHome");
        }

        public ActionResult ViewBoardContractPartial(bool refresh = false)
        {
            DataTable tbl = ContractsHelper.GetDocumentsContracts(refresh, State.STATEACTIVE, 10, new DateTime(2001, 1, 1), new DateTime(2020, 1, 1));
            return View(tbl.DefaultView.ToTable());
        }

        public ActionResult ViewBoardRevisionPartial(bool refresh = false)
        {
            DataTable tbl = ContractsHelper.GetDocumentsRevision(refresh, State.STATEACTIVE, 10, new DateTime(2001, 1, 1), new DateTime(2020, 1, 1));
            return View(tbl.DefaultView.ToTable());
        }

        public ActionResult ViewBoardVerificationPartial(bool refresh = false)
        {
            DataTable tbl = ContractsHelper.GetDocumentsVerification(refresh, State.STATEACTIVE, 10, new DateTime(2001, 1, 1), new DateTime(2020, 1, 1));
            return View(tbl.DefaultView.ToTable());
        }
    }
}
