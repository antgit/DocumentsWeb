using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.ServicesNds.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPSERVICESNDS })]
    public class ServiceAccountOutController : ServiceController
    {
        public ServiceAccountOutController()
        {
            Name = "WEBУСФИNDS";
            FolderCodeFind = Folder.CODE_FIND_SERVICE_OUT_ACCOUNT_NDS;
            DocumentKindId = DocumentService.KINDID_ACCOUNTOUT;
        }
    }
}
