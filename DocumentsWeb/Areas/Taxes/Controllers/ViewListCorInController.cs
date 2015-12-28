using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Taxes.Controllers
{
    /// <summary>
    /// ���������� ������ ���������� "���������������� ��������� ��������"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPTAX})]
    public class ViewListCorInController : CoreDocumentListControler
    {
        public ViewListCorInController()
        {
            Name = "WEB����";
            FolderCodeFind = Folder.CODE_FIND_TAX_CORIN;
        }

        public ActionResult Index()
        {
            ViewResult result = View(TaxesHelper.GetDocumentsCor(true, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(TaxesHelper.GetDocumentsCor(true, FolderCodeFind, refresh));
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
            return PartialView("IndexPartial", TaxesHelper.GetDocumentsCor(true, FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "CorIn" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "CorIn" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }


    }
}