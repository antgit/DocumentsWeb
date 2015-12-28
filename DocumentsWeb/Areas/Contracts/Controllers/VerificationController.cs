using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;

namespace DocumentsWeb.Areas.Contracts.Controllers
{
    /// <summary>
    /// Документ "Акт сверки"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPCONTRACTS})]
    public class VerificationController : DocumentsWeb.Controllers.ContractController
    {
        public VerificationController()
        {
            Name = "WEBДАС";
            FolderCodeFind = Folder.CODE_FIND_CONTRACTS_VERIFICATION;
        }
        public ActionResult ClientsFinderPartial()
        {
            ViewData["Name"] = "ClientsFinderAgentFrom";
            ViewData["ComboboxName"] = GlobalPropertyNames.MainClientDepatmentId;
            ViewData["ComboboxClientAcc"] = "MainClientAccountId";
            ViewData["ComboboxButtonIndex"] = 0;
            ViewData["onlyUsers"] = false;
            return PartialView("ClientsFinderPartial");
        }
    }
}
