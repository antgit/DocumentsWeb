using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Services.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICES})]
    public class ServiceAccountOutController : ServiceController
    {
        public ServiceAccountOutController()
        {
            Name = "WEBУСФИ";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_OUT_ACCOUNT;
            DocumentKindId = DocumentService.KINDID_ACCOUNTOUT;
        }
    }
}
