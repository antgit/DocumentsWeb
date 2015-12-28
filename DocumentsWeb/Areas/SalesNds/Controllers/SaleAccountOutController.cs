using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.SalesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSALESNDS})]
    public class SaleAccountOutController : SaleController
    {
        public SaleAccountOutController()
        {
            Name = "WEBТСФИNDS";
            FolderCodeFind = Folder.CODE_FIND_SALES_OUT_ACCOUNT_NDS;
            DocumentKindId = DocumentSales.KINDID_ACCOUNTOUT;
        }
    }
}
