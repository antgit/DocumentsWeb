using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.SalesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALESNDS})]
    public class SaleAccountInController : SaleController
    {
        public SaleAccountInController()
        {
            Name = "WEBТСФВNDS";
            FolderCodeFind = Folder.CODE_FIND_SALES_IN_ACCOUNT_NDS;
            DocumentKindId = DocumentSales.KINDID_ACCOUNTIN;
        }
        public override ActionResult ClientsFinderPartial()
        {
            ViewData["Name"] = "ClientsFinderAgentFrom";
            ViewData["ComboboxName"] = GlobalPropertyNames.MainClientDepatmentId;
            ViewData["ComboboxClientAcc"] = "MainClientAccountId";
            ViewData["ComboboxButtonIndex"] = 2;
            ViewData["onlyUsers"] = false;
            ViewData["OnlySupplyer"] = true;
            return PartialView("ClientsFinderPartial");
        }
    }
}
