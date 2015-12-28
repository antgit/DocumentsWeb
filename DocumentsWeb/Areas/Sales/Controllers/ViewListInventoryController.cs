﻿using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Инвентаризация"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class ViewListInventoryController : CoreDocumentListControler
    {
        public ViewListInventoryController()
        {
            Name = "WEBТИВ";
            FolderCodeFind = Folder.CODE_FIND_SALES_INVENTORY;
        }

        public ActionResult Index(bool refresh = false)
        {
            ViewResult result = View(SalesHelper.GetDocumentsInventory(FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        public ActionResult IndexPartial(bool refresh=false)
        {
            return PartialView(SalesHelper.GetDocumentsInventory(FolderCodeFind, refresh));
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
            return PartialView("IndexPartial", SalesHelper.GetDocumentsInventory(FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleInventory" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleInventory" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
    }
}
