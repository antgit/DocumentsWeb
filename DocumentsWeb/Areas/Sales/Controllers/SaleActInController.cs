using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class SaleActInController : SaleController
    {
        public SaleActInController()
        {
            Name = "WEBТАП";
            FolderCodeFind = Folder.CODE_FIND_SALES_ACT_IN;
            DocumentKindId = DocumentSales.KINDID_IN;
        }
    }
}
