using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Services.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Счет входящий"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICES })]
    public class ViewListAccountInController : CoreDocumentListControler
    {
        public ViewListAccountInController()
        {
            Name = "WEBУСФВ";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_IN_ACCOUNT;
        }

        public ActionResult Index()
        {
            ViewResult result = View(ServicesHelper.GetDocumentsAccounts(true, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh=false)
        {
            return PartialView(ServicesHelper.GetDocumentsAccounts(true, FolderCodeFind, refresh));
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
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "ServiceAccountIn" });

            return PartialView("SelectDocumentTemplatePartial");
        }

        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "ServiceAccountIn" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }


    }
}
