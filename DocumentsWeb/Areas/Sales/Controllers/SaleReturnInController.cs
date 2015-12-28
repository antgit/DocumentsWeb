using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class SaleReturnInController : SaleController
    {
        public SaleReturnInController()
        {
            Name = "WEBТВНП";
            FolderCodeFind = Folder.CODE_FIND_SALES_RETURN_IN;
            DocumentKindId = DocumentSales.KINDID_RETURNIN;
        }
    }
}
