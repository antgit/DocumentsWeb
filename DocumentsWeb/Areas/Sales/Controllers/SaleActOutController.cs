using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Sales.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALES})]
    public class SaleActOutController : SaleController
    {
        public SaleActOutController()
        {
            Name = "WEBТРП";
            FolderCodeFind = Folder.CODE_FIND_SALES_ACT_OUT;
            DocumentKindId = DocumentSales.KINDID_OUT;
        }
    }
}
