using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class SaleAccountOutController : SaleController
    {
        public SaleAccountOutController()
        {
            Name = "WEBТСФИ";
            FolderCodeFind = Folder.CODE_FIND_SALES_ACCOUNT_OUT;
            DocumentKindId = DocumentSales.KINDID_ACCOUNTOUT;
        }
    }
}
