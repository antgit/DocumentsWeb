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
    /// Список документов "Договора"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPCONTRACTS})]
    public class ViewListContractController : CoreDocumentListControler
    {
        public ViewListContractController()
        {
            FolderCodeFind = Folder.CODE_FIND_CONTRACTS_CONTRACT;
            Name = "WEBДОГ";
        }
        public ActionResult Index()
        {
            DataTable tbl = ContractsHelper.GetDocumentsContracts(true);
            ViewResult result = View(tbl.DefaultView.ToTable());
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            DataTable tbl = ContractsHelper.GetDocumentsContracts(refresh);
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
            return PartialView("IndexPartial", ContractsHelper.GetDocumentsContracts(true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "Contract" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "Contract" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
    }
}
