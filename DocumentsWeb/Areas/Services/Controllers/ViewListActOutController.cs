using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Services.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Акт выполненных работ исходящий"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICES })]
    public class ViewListActOutController : CoreDocumentListControler
    {
        public ViewListActOutController()
        {
            Name = "WEBУАРИ";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_OUT;
        }
        
        public ActionResult Index()
        {
            ViewResult result = View(ServicesHelper.GetDocuments(false, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(ServicesHelper.GetDocuments(false, FolderCodeFind, refresh));
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
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "ServiceActOut" });

            return PartialView("SelectDocumentTemplatePartial");
        }

        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "ServiceActOut" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
    }
}
