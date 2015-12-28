using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Areas.Planing.Controllers
{
    /// <summary>Документ "Заявка на приобретение"</summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPCONTRACTS})]
    public class PlanOrderController : PlaningController
    {
        public PlanOrderController()
        {
            Name = "WEBФИНПЛАНЗВК";
            FolderCodeFind = Folder.CODE_FIND_FINPLAN_ORDER;
            PrintModelCode = "FINPLAN";
        }

    }
    /// <summary>Документ "Окончательная заявка на приобретение"</summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPCONTRACTS})]
    public class PlanOrderTotalController : PlaningController
    {
        public PlanOrderTotalController()
        {
            Name = "WEBФИНПЛАНЗВКО";
            FolderCodeFind = Folder.CODE_FIND_FINPLAN_ORDERTOTAL;
            PrintModelCode = "FINPLAN";
        }

    }
    /// <summary>Документ "Финальная заявка на приобретение"</summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPCONTRACTS})]
    public class PlanOrderFinalController : PlaningController
    {
        public PlanOrderFinalController()
        {
            Name = "WEBФИНПЛАНЗВКФ";
            FolderCodeFind = Folder.CODE_FIND_FINPLAN_ORDERFINAL;
            PrintModelCode = "FINPLAN";
        }

    }
}
