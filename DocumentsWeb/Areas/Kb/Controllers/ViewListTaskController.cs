using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.Kb.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Kb.Controllers
{
    /// <summary>
    /// Контроллер списка задач
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPTASK })]
    public class ViewListTaskController:CoreController<Task>
    {
        public ViewListTaskController()
        {
            RootHierachy = Hierarchy.GetSystemRootCodeValue(WhellKnownDbEntity.Task);
            Name = WebModuleNames.WEB_TASKS;
        }

        public ActionResult Index(bool refresh = false)
        {
            //LayoutHelper.LoadLayoutFromDatabase(
            //       LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
            //       WADataProvider.WA.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
            //       HttpContext);
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

            if(root_id!=0)
            {
                //Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(root_id);

                ////Если это иерархия поиска
                //if (hierarchy != null && hierarchy.ViewList != null && hierarchy.ViewListId!=0) 
                //{
                //    LayoutHelper.DeleteCookies(Url.RequestContext.RouteData.Values["controller"].ToString(), HttpContext);
                //    PartialViewResult result = PartialView(hierarchy.GetCustomView());
                //    result.ViewData.Add("CustomViewList", hierarchy.ViewList);
                //    return result;
                //}
                //else
                //{
                //    //Восстанавливаем сохраненые настройки
                //    LayoutHelper.LoadLayoutFromDatabase(
                //        LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
                //        WADataProvider.WA.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
                //        HttpContext);
                //}
            }
            else
            {
                root_id = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy).Id;
            }
            return PartialView(TaskModel.GetCollection(root_id, refresh));
        }

        public ActionResult FastViewIncomeTask()
        {
            return PartialView();
        }

        public ActionResult FastViewOutcomeTask()
        {
            return PartialView();
        }
        
    }
}
