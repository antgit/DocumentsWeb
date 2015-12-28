using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using System.Data;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Contracts.Controllers
{
    /// <summary>
    /// Список документов "Учет прнтеров и копировальной техники"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPCONTRACTS})]
    public class ViewListAccountingPrintersController : CoreDocumentListControler
    {
        public ViewListAccountingPrintersController()
        {
            FolderCodeFind = Folder.CODE_FIND_CONTRACTS_PRINTER;
            Name = "WEBДПРИНТ";
        }
        public ActionResult Index()
        {
            DataTable tbl = ContractsHelper.GetDocumentsAccountingPrinters(true);
            ViewResult result = View(tbl.DefaultView.ToTable());
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            DataTable tbl = ContractsHelper.GetDocumentsAccountingPrinters(refresh);
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
            return PartialView("IndexPartial", ContractsHelper.GetDocumentsAccountingPrinters(true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "AccountingPrinters" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "AccountingPrinters" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }

    }
}
