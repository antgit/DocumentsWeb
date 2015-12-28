using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.SalesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALESNDS})]
    public class SaleReturnInController : SaleController
    {
        public SaleReturnInController()
        {
            Name = "WEBТВНПNDS";
            FolderCodeFind = Folder.CODE_FIND_SALES_IN_RETURN_NDS;
            DocumentKindId = DocumentSales.KINDID_RETURNIN;
        }
    }
}
