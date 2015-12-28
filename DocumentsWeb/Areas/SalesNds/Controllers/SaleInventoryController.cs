using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.SalesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALESNDS})]
    public class SaleInventoryController : SaleController
    {
        public SaleInventoryController()
        {
            Name = "WEBТИВNDS";
            FolderCodeFind = Folder.CODE_FIND_SALES_INVENTORY;
            DocumentKindId = DocumentSales.KINDID_INVENTORY;
        }
    }
}
