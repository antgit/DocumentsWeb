using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.FinancesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPFINANCENDS})]
    public class ReturnOutController : FinanceController
    {
        public ReturnOutController()
        {
            Name = "WEBФРДNDS";
            FolderCodeFind = Folder.CODE_FIND_FINANCE_OUT;
        }
    }
}
