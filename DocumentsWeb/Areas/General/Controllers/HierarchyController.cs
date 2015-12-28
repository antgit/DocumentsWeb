using System.Web.Mvc;
using BusinessObjects.Security;

namespace DocumentsWeb.Areas.General.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class HierarchyController : Controller
    {
        public ActionResult CallbacksPartial()
        {
            return PartialView();
        }

        public ActionResult GridFilterPartial()
        {
            return PartialView();
        }

        public ActionResult FilterTreeViewPartial()
        {
            PartialViewResult res = PartialView();
            res.ViewData.Add("GridController", Request.Params["GridController"]);
            res.ViewData.Add("GridAction", Request.Params["GridAction"]);
            res.ViewData.Add("RootList", ((string)Request.Params["RootList"]).Split(','));
            return res;
        }

        public ActionResult GroupControlTreeViewPartial()
        {
            PartialViewResult res = PartialView();
            res.ViewData.Add("RootList", ((string)Request.Params["RootList"]).Split(','));
            res.ViewData.Add("InHies", (string)Request.Params["InHies"]);
            return res;
        }

        public ActionResult HierarchyPartial()
        {
            return PartialView();
        }

        public ActionResult HierarchyTreeViewPartial()
        {
            PartialViewResult res = PartialView();
            res.ViewData.Add("RootList", ((string)Request.Params["RootList"]).Split(','));
            res.ViewData.Add("OnSelectNode", Request.Params["OnSelectNode"]);
            return res;
        }

        public ActionResult GroupsControlPartial()
        {
            return PartialView();
        }

        public void AddFilterRoot(string c, string a, int r)
        {
            Utils.AddHieRoot(c, a, r);
        }

        public void DelFilterRoot(string c, string a, int r)
        {
            Utils.DeleteHieRoot(c, a, r);
        }
    }
}
