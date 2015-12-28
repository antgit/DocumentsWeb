using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.FinancesNds.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Входящие оплаты"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPFINANCENDS})]
    public class ViewListFinanceInNdsController : CoreDocumentListControler
    {
        public ViewListFinanceInNdsController()
        {
            Name = "WEBФПД";
            FolderCodeFind = Folder.CODE_FIND_FINANCE_IN_NDS;
        }

        public ActionResult Index()
        {
            ViewResult result = View(FinanceHelper.GetDocumentsIn(Folder.CODE_FIND_FINANCE_IN_NDS, true));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial()
        {
            return PartialView(FinanceHelper.GetDocumentsIn(Folder.CODE_FIND_FINANCE_IN_NDS, true));
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
