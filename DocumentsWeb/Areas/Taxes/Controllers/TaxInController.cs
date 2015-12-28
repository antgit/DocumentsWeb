using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Taxes.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPTAX})]
    public class TaxInController : TaxController
    {
        public TaxInController()
        {
            Name = "WEBННВ";
            FolderCodeFind = Folder.CODE_FIND_TAX_IN;
            DocumentKindId = DocumentTaxes.KINDID_IN;
        }
    }
}
