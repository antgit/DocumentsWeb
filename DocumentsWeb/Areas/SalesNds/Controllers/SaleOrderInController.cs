using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.SalesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALESNDS})]
    public class SaleOrderInController : SaleController
    {
        public SaleOrderInController()
        {
            Name = "WEBТЗПNDS";
            FolderCodeFind = Folder.CODE_FIND_SALES_IN_ORDER_NDS;
            DocumentKindId = DocumentSales.KINDID_ORDERIN;
        }
    }
}
