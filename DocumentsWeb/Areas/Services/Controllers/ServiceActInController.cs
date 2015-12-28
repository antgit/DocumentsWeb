using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Services.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICES})]
    public class ServiceActInController : ServiceController
    {
        public ServiceActInController()
        {
            Name = "WEBУАРВ";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_IN;
            DocumentKindId = DocumentService.KINDID_IN;
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
