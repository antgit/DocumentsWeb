using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class SaleInController : SaleController
    {
        public SaleInController()
        {
            Name = "WEBТПН";
            FolderCodeFind = Folder.CODE_FIND_SALES_IN;
            DocumentKindId = DocumentSales.KINDID_IN;
        }
    }
}
