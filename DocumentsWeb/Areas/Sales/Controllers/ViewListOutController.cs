using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.XtraReports.Data;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Расходных накладных"
    /// </summary>
    //[System.Web.Mvc.OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")] 
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class ViewListOutController : CoreDocumentListControler
    {
        public ViewListOutController()
        {
            Name = "WEBТРН";
            FolderCodeFind = Folder.CODE_FIND_SALES_OUT;
        }
        
        public ActionResult Index()
        {
            ViewResult result = View(SalesHelper.GetDocuments(false, FolderCodeFind));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView(SalesHelper.GetDocuments(false, FolderCodeFind, refresh));
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
            return PartialView("IndexPartial", SalesHelper.GetDocuments(false, FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleOut" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "SaleOut" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
    }
}