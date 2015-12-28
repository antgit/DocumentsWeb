using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.SalesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALESNDS})]
    public class SaleActInController : SaleController
    {
        public SaleActInController()
        {
            Name = "WEBТАПNDS";
            FolderCodeFind = Folder.CODE_FIND_SALES_IN_ACT_NDS;
            DocumentKindId = DocumentSales.KINDID_IN;
        }
    }
}
