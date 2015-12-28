using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.SalesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALESNDS})]
    public class SaleMoveController : SaleController
    {
        public SaleMoveController()
        {
            Name = "WEBТВПNDS";
            FolderCodeFind = Folder.CODE_FIND_SALES_MOVE_NDS;
            DocumentKindId = DocumentSales.KINDID_MOVE;
        }
    }
}
