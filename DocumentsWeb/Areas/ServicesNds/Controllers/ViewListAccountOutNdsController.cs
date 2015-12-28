using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.ServicesNds.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Счет исходящий"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICESNDS })]
    public class ViewListAccountOutNdsController : CoreDocumentListControler
    {
        public ViewListAccountOutNdsController()
        {
            Name = "WEBУСФИNDS";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_OUT_ACCOUNT_NDS;
        }
        
        public ActionResult Index()
        {
            ViewResult result = View(ServicesHelper.GetDocumentsAccounts(false, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial()
        {
            return PartialView(ServicesHelper.GetDocumentsAccounts(false, FolderCodeFind, true));
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
            return PartialView("IndexPartial", ServicesHelper.GetDocumentsAccounts(false, FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "ServiceAccountOut" });

            return PartialView("SelectDocumentTemplatePartial");
        }

        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "ServiceAccountOut" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
    }
}
