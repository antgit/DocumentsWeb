using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.ServicesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICESNDS })]
    public class ServiceOrderInController : ServiceController
    {
        public ServiceOrderInController()
        {
            Name = "WEBУЗПNDS";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_IN_ORDER_NDS;
            DocumentKindId = DocumentService.KINDID_ORDERIN;
        }
    }
}
