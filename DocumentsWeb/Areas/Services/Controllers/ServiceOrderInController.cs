using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Services.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICES})]
    public class ServiceOrderInController : ServiceController
    {
        public ServiceOrderInController()
        {
            Name = "WEBУЗП";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_IN_ORDER;
            DocumentKindId = DocumentService.KINDID_ORDERIN;
        }
    }
}
