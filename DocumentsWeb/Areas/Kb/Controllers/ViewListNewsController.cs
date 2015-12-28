using System.Web.Mvc;
using System.Linq;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Areas.Kb.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Kb.Controllers
{
    /// <summary>
    /// Контроллер списка новостей 
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class ViewListNewsController : CoreController<Message>
    {
        public ViewListNewsController()
        {
            RootHierachy = Hierarchy.SYSTEM_MESSAGE_WEBNEWS;
            Name = WebModuleNames.WEB_MODULE_NEWS;
        }

        public ActionResult Index(bool refresh = false)
        {
            LayoutHelper.LoadLayoutFromDatabase(
                   LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
                   WADataProvider.WA.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
                   HttpContext);
            ViewResult result = View();
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }


        public ActionResult IndexPartial(bool refresh = false)
        {
            string root = (string)Request.Params["Root"];
            int root_id = 0;

            try { root_id = int.Parse(root); }
            catch { }

            //CustomViewList list=null;

            if (root_id == 0)
            {
                root_id = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy).Id;
            }
            return PartialView("IndexPartial", NewsModel.GetCollection(root_id, refresh));
        }

        public ActionResult LastFiveNews()
        {
            return PartialView();
        }

        

    }
}