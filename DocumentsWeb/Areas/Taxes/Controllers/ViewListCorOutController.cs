using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Taxes.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Корректировочная налоговая исходящая"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPTAX})]
    public class ViewListCorOutController : CoreDocumentListControler
    {
        public ViewListCorOutController()
        {
            Name = "WEBННКИ";
            FolderCodeFind = Folder.CODE_FIND_TAX_COROUT;
        }

        public ActionResult Index()
        {
            ViewResult result = View(TaxesHelper.GetDocumentsCor(false, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(TaxesHelper.GetDocumentsCor(false, FolderCodeFind, refresh));
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
            return PartialView("IndexPartial", TaxesHelper.GetDocumentsCor(false, FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "CorOut" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "CorOut" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }


    }
}