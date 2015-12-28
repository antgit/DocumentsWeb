using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Finances.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPFINANCE })]
    public class FinanceInController : FinanceController
    {
        public FinanceInController()
        {
            Name = "WEBФПД";
            FolderCodeFind = Folder.CODE_FIND_FINANCE_IN;
            DocumentKindId = DocumentFinance.KINDID_IN;
        }
    }
}
