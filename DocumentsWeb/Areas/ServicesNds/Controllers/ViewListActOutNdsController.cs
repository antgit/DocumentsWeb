﻿using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.ServicesNds.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Акт выполненных работ исходящий"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICESNDS })]
    public class ViewListActOutNdsController : CoreDocumentListControler
    {
        public ViewListActOutNdsController()
        {
            Name = "WEBУАРИNDS";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_OUT_NDS;
        }
        
        public ActionResult Index()
        {
            ViewResult result = View(ServicesHelper.GetDocuments(false, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial()
        {
            return PartialView(ServicesHelper.GetDocuments(false, FolderCodeFind, true));
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
