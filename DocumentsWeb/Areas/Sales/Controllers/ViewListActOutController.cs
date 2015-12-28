using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    /// <summary>
    /// ���������� ������ ���������� "��� ������ ������ ���������"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class ViewListActOutController : CoreDocumentListControler
    {
        public ViewListActOutController()
        {
            Name = "WEB���";
            FolderCodeFind = Folder.CODE_FIND_SALES_ACT_OUT;
        }

        public ActionResult Index()
        {
            ViewResult result = View(SalesHelper.GetDocumentsAct(false, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(SalesHelper.GetDocumentsAct(false, FolderCodeFind, refresh));
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
            return PartialView("IndexPartial", SalesHelper.GetDocumentsAct(false, FolderCodeFind, true));
        }

        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleActOut" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleActOut" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
        
    }
}