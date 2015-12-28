using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class SaleInventoryController : SaleController
    {
        public SaleInventoryController()
        {
            Name = "WEBТИВ";
            FolderCodeFind = Folder.CODE_FIND_SALES_INVENTORY;
            DocumentKindId = DocumentSales.KINDID_INVENTORY;
        }
    }
}
