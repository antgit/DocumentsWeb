using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class SaleOrderInController : SaleController
    {
        public SaleOrderInController()
        {
            Name = "WEBТЗП";
            FolderCodeFind = Folder.CODE_FIND_SALES_ORDER_IN;
            DocumentKindId = DocumentSales.KINDID_ORDERIN;
        }
    }
}
