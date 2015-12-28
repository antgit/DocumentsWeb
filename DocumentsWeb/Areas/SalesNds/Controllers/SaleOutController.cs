using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.SalesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALESNDS})]
    public class SaleOutController : SaleController
    {
        public SaleOutController()
        {
            Name = "WEBТРНNDS";
            FolderCodeFind = Folder.CODE_FIND_SALES_OUT_NDS;
            DocumentKindId = DocumentSales.KINDID_OUT;
        }
    }
}
