using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.ServicesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICESNDS })]
    public class ServiceActOutController : ServiceController
    {
        public ServiceActOutController()
        {
            Name = "WEBУАРИNDS";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_OUT_NDS;
            DocumentKindId = DocumentService.KINDID_OUT;
        }
    }
}
