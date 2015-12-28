using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Finances.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPFINANCE })]
    public class FinanceOutController : FinanceController
    {
        public FinanceOutController()
        {
            Name = "WEBФРД";
            FolderCodeFind = Folder.CODE_FIND_FINANCE_OUT;
            DocumentKindId = DocumentFinance.KINDID_OUT;
        }
    }
}
