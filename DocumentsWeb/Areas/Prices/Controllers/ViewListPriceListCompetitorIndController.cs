using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Models;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Prices.Controllers
{
    /// <summary>
    /// Контроллер списка документов "Прайс лист конкурента индивидуальный"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPPRICES })]
    public class ViewListPriceListCompetitorIndController : CoreDocumentListControler
    {
        public ViewListPriceListCompetitorIndController()
        {
            Name = "WEBЦПРКИ";
            FolderCodeFind = Folder.CODE_FIND_PRICES_COMPETITORIND;
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

        public ActionResult ViewListPriceListCompetitorsDeletePartial(int Id)
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
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "PriceListCompetitorInd" });

            return PartialView("SelectDocumentTemplatePartial");
        }
        public override ActionResult DocumentTemplateSelectPartial()
        {
            ViewData["FolderCode"] = FolderCodeFind;
            ViewData["CreateUrl"] = Url.Action("Create", new { controller = "PriceListCompetitorInd" });
            return PartialView("SelectDocumentTemplateGridPartial");
        }
    }
}
