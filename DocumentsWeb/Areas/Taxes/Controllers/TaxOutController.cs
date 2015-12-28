using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Taxes.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPTAX})]
    public class TaxOutController : TaxController
    {
        public TaxOutController()
        {
            Name = "WEBННИ";
            FolderCodeFind = Folder.CODE_FIND_TAX_OUT;
            DocumentKindId = DocumentTaxes.KINDID_OUT;
        }
    }
}
