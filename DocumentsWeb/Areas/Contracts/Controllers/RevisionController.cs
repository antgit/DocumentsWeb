using BusinessObjects;
using BusinessObjects.Security;

namespace DocumentsWeb.Areas.Contracts.Controllers
{
    /// <summary>
    /// Документ "Акт ревизии"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPCONTRACTS})]
    public class RevisionController : DocumentsWeb.Controllers.ContractController
    {
        public RevisionController()
        {
            Name = "WEBДАР";
            FolderCodeFind = Folder.CODE_FIND_CONTRACTS_REVISION;
        }
    }
}
