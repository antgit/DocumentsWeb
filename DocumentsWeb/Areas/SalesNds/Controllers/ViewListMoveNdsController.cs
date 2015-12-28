using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.SalesNds.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Внутреннее перемещение"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALESNDS})]
    public class ViewListMoveNdsController : CoreDocumentListControler
    {
        public ViewListMoveNdsController()
        {
            Name = "WEBТВПNDS";
            FolderCodeFind = Folder.CODE_FIND_SALES_MOVE_NDS;
        }
        
        public ActionResult Index()
        {
            ViewResult result = View(SalesHelper.GetDocumentsMove(FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(SalesHelper.GetDocumentsMove(FolderCodeFind, refresh));
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
            return PartialView("IndexPartial", SalesHelper.GetDocumentsMove(FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleMove" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleMove" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
        
    }
}