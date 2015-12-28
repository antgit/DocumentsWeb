using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Счета исходящие"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class ViewListAccountOutController : CoreDocumentListControler
    {
        public ViewListAccountOutController()
        {
            Name = "WEBТСФИ";
            FolderCodeFind = Folder.CODE_FIND_SALES_ACCOUNT_OUT;
        }

        public ActionResult Index()
        {
            ViewResult result = View(SalesHelper.GetDocumentsAccount(false, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(SalesHelper.GetDocumentsAccount(false, FolderCodeFind, refresh));
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
            return PartialView("IndexPartial", SalesHelper.GetDocumentsAccount(false, FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleAccountOut" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleAccountOut" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
        
    }
}