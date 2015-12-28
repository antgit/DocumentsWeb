using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.FinancesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPFINANCENDS})]
    public class FinanceOutController : FinanceController
    {
        public FinanceOutController()
        {
            Name = "WEBФРДNDS";
            FolderCodeFind = Folder.CODE_FIND_FINANCE_OUT_NDS;
            DocumentKindId = DocumentFinance.KINDID_OUT;
        }
    }
}
