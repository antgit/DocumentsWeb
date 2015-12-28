using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Services.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICES})]
    public class ServiceActOutController : ServiceController
    {
        public ServiceActOutController()
        {
            Name = "WEBУАРИ";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_OUT;
            DocumentKindId = DocumentService.KINDID_OUT;
        }
    }
}
