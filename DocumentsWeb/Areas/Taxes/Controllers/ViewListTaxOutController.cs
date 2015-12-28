using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Taxes.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Исходящая налоговая"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPTAX})]
    public class ViewListTaxOutController : CoreDocumentListControler
    {
        public ViewListTaxOutController()
        {
            Name = "WEBННИ";
            FolderCodeFind = Folder.CODE_FIND_TAX_OUT;
        }

        public ActionResult Index()
        {
            ViewResult result = View(TaxesHelper.GetDocuments(false, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(TaxesHelper.GetDocuments(false, FolderCodeFind, refresh));
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
            return PartialView("IndexPartial", ServicesHelper.GetDocuments(false, FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "TaxOut" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "TaxOut" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }


    }
}