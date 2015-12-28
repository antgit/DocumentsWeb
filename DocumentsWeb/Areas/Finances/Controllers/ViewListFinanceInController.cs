using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Finances.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Входящие оплаты"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPFINANCE })]
    public class ViewListFinanceInController : CoreDocumentListControler
    {
        public ViewListFinanceInController()
        {
            Name = "WEBФПД";
            FolderCodeFind = Folder.CODE_FIND_FINANCE_IN;
        }

        public ActionResult Index()
        {
            ViewResult result = View(FinanceHelper.GetDocumentsIn(FolderCodeFind, true));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial()
        {
            return PartialView(FinanceHelper.GetDocumentsIn(FolderCodeFind, true));
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "FinanceIn" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "FinanceIn" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }

    }
}
