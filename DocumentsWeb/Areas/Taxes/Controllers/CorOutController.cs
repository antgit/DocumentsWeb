using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Taxes.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPTAX})]
    public class CorOutController : TaxController
    {
        public CorOutController()
        {
            Name = "WEBННКИ";
            FolderCodeFind = Folder.CODE_FIND_TAX_COROUT;
            DocumentKindId = DocumentTaxes.KINDID_COROUT;
        }
    }
}
