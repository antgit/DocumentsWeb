using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Models;
using BusinessObjects;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Prices.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Прайс лист поставщика"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPPRICES })]
    public class ViewListPriceListSupplierController : CoreDocumentListControler
    {
        public ViewListPriceListSupplierController()
        {
            Name = "WEBЦПРП";
            FolderCodeFind = Folder.CODE_FIND_PRICES_SYPPLYER;
        }

        public ActionResult Index()
        {
            ViewResult result = View();
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial()
        {
            return PartialView();
        }

        public ActionResult ViewListPriceListDeliveryDeletePartial(int Id)
        {
            BusinessObjects.Documents.DocumentPrices price = new BusinessObjects.Documents.DocumentPrices() { Workarea = WADataProvider.WA };
            price.Load(Id);
            price.StateId = State.STATEDELETED;
            price.Save();
            return PartialView("IndexPartial");
        }
        public override ActionResult SelectDocumentTemplate()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "PriceListSupplier" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "PriceListSupplier" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
    }
}
