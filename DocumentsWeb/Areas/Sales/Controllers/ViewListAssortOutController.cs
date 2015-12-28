using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    /// <summary>
    /// ���������� ������ ���������� "�������������� ���� ���������"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class ViewListAssortOutController : CoreDocumentListControler
    {
        public ViewListAssortOutController()
        {
            Name = "WEB����";
            FolderCodeFind = Folder.CODE_FIND_SALES_ASSORT_OUT;
        }

        public ActionResult Index()
        {
            ViewResult result = View(SalesHelper.GetDocumentsAssort(false, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(SalesHelper.GetDocumentsAssort(false, FolderCodeFind, refresh));
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
            return PartialView("IndexPartial", SalesHelper.GetDocumentsAssort(false, FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleAssortOut" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleAssortOut" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
        
    }
}