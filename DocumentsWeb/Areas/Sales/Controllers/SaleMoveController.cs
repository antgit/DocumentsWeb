using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class SaleMoveController : SaleController
    {
        public SaleMoveController()
        {
            Name = "WEBТВП";
            FolderCodeFind = Folder.CODE_FIND_SALES_MOVE;
            DocumentKindId = DocumentSales.KINDID_MOVE;
        }
    }
}
