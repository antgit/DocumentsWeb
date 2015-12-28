using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.FinancesNds.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Исходящие оплаты"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPFINANCENDS})]
    public class ViewListFinanceOutNdsController : CoreDocumentListControler
    {
        public ViewListFinanceOutNdsController()
        {
            Name = "WEBФРД";
            FolderCodeFind = Folder.CODE_FIND_FINANCE_OUT_NDS;
        }

        public ActionResult Index()
        {
            ViewResult result = View(FinanceHelper.GetDocumentsOut(FolderCodeFind, true));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial()
        {
            return PartialView(FinanceHelper.GetDocumentsOut(FolderCodeFind, true));
        }

        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "FinanceOut" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "FinanceOut" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
    }
}
