using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class SaleAssortOutController : SaleController
    {
        public SaleAssortOutController()
        {
            Name = "WEBТАЛИ";
            FolderCodeFind = Folder.CODE_FIND_SALES_ASSORT_OUT;
            DocumentKindId = DocumentSales.KINDID_ASSORTOUT;
        }
        public override ActionResult ClientsFinderPartial()
        {
            ViewData["Name"] = "ClientsFinderAgentFrom";
            ViewData["ComboboxName"] = GlobalPropertyNames.MainClientDepatmentId;
            ViewData["ComboboxClientAcc"] = "MainClientAccountId";
            ViewData["ComboboxButtonIndex"] = 2;
            ViewData["onlyUsers"] = false;
            ViewData["OnlySupplyer"] = false;
            return PartialView("ClientsFinderPartial");
        }
    }
}
