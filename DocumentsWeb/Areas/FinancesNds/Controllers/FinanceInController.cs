using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.FinancesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPFINANCENDS})]
    public class FinanceInController : FinanceController
    {
        public FinanceInController()
        {
            Name = "WEBФПДNDS";
            FolderCodeFind = Folder.CODE_FIND_FINANCE_IN_NDS;
            DocumentKindId = DocumentFinance.KINDID_IN;
        }
    }
}
