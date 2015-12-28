using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Services.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Акт выполненных работ входящий"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICES })]
    public class ViewListActInController : CoreDocumentListControler
    {
        public ViewListActInController()
        {
            Name = "WEBУАРВ";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_IN;
        }

        public ActionResult Index()
        {
            ViewResult result = View(ServicesHelper.GetDocuments(true, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(ServicesHelper.GetDocuments(true, FolderCodeFind, refresh));
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
            return PartialView("IndexPartial", ServicesHelper.GetDocumentsAccounts(true, FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "ServiceActIn" });

            return PartialView("SelectDocumentTemplatePartial");
        }

        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "ServiceActIn" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }

    }
}
