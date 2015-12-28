using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.ServicesNds.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Акт выполненных работ входящий"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICESNDS })]
    public class ViewListActInNdsController : CoreDocumentListControler
    {
        public ViewListActInNdsController()
        {
            Name = "WEBУАРВNDS";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_IN_NDS;
        }
        
        public ActionResult Index()
        {
            ViewResult result = View(ServicesHelper.GetDocuments(true, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial()
        {
            return PartialView(ServicesHelper.GetDocuments(true, FolderCodeFind, true));
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
            return PartialView("IndexPartial", ServicesHelper.GetDocuments(true, FolderCodeFind, true));
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
