using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Taxes.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPTAX})]
    public class CorInController : TaxController
    {
        public CorInController()
        {
            Name = "WEBННКВ";
            FolderCodeFind = Folder.CODE_FIND_TAX_CORIN;
            DocumentKindId = DocumentTaxes.KINDID_CORIN;
        }
    }
}
