using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.ServicesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICESNDS })]
    public class ServiceOrderOutController : ServiceController
    {
        public ServiceOrderOutController()
        {
            Name = "WEBУЗПNDS";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_OUT_ORDER_NDS;
            DocumentKindId = DocumentService.KINDID_ORDEROUT;
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
