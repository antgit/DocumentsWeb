using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using BusinessObjects;
using System.Web.Mvc;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Taxes.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Входящая налоговая"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPTAX})]
    public class ViewListTaxInController : CoreDocumentListControler
    {
        public ViewListTaxInController()
        {
            Name = "WEBННВ";
            FolderCodeFind = Folder.CODE_FIND_TAX_IN;
        }

        public ActionResult Index()
        {
            ViewResult result = View(TaxesHelper.GetDocuments(true, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(TaxesHelper.GetDocuments(true, FolderCodeFind, refresh));
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
            return PartialView("IndexPartial", TaxesHelper.GetDocuments(true, FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "TaxIn" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "TaxIn" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }


    }
}