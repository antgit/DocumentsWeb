using System;
using System.Data;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Contracts.Controllers
{
    /// <summary>
    /// Список документов "Акт сверки"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPCONTRACTS})]
    public class ViewListVerificationController : CoreDocumentListControler
    {
        public ViewListVerificationController()
        {
            FolderCodeFind = Folder.CODE_FIND_CONTRACTS_VERIFICATION;
            Name = "WEBДАС";
        }
        public ActionResult Index()
        {
            DataTable tbl = ContractsHelper.GetDocumentsVerification(true);
            tbl.DefaultView.RowFilter = "StateId=1";
            ViewResult result = View(tbl.DefaultView.ToTable());
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            DataTable tbl = ContractsHelper.GetDocumentsVerification(refresh);
            tbl.DefaultView.RowFilter = "StateId=1";
            return PartialView(tbl.DefaultView.ToTable());
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ToTrash(int id)
        {
            if (id != 0)
            {
                try
                {
                    DocumentModel.Remove(id);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("IndexPartial", ContractsHelper.GetDocumentsVerification(true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "Verification" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "Verification" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
    }
}
