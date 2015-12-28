using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class SaleOutController : SaleController
    {
        public SaleOutController()
        {
            Name = "WEBТРН";
            FolderCodeFind = Folder.CODE_FIND_SALES_OUT;
            DocumentKindId = DocumentSales.KINDID_OUT;
        }
    }
}
