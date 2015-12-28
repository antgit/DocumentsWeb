using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Finances.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPFINANCE })]
    public class ReturnOutController : FinanceController
    {
        public ReturnOutController()
        {
            Name = "WEBФРД";
            FolderCodeFind = Folder.CODE_FIND_FINANCE_OUT;
        }
    }
}
