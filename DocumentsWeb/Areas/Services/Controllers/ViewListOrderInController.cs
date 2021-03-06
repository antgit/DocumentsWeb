﻿using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Services.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Заказ услуг входящий"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICES })]
    public class ViewListOrderInController : CoreDocumentListControler
    {
        public ViewListOrderInController()
        {
            Name = "WEBУЗК";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_IN_ORDER;
        }
        
        public ActionResult Index()
        {
            ViewResult result = View(ServicesHelper.GetDocumentsOrders(true, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(ServicesHelper.GetDocumentsOrders(true, FolderCodeFind, refresh));
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
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "ServiceOrderIn" });

            return PartialView("SelectDocumentTemplatePartial");
        }

        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "ServiceOrderIn" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }

    }
}
