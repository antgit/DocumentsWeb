using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.UserPersonal.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.UserPersonal.Controllers
{
    /// <summary>
    /// Список сообщений пользователя
    /// </summary>
    [Authorize(Roles = Uid.GROUP_GROUPWEBUSER)]
    public class ViewListUserMessageController : CoreController<Message>
    {
        public ViewListUserMessageController()
        {
            RootHierachy = Hierarchy.SYSTEM_MESSAGE_USERS;
            Name = WebModuleNames.WEB_USERMESSAGE;
        }

        public ActionResult Index()
        {
            //LayoutHelper.LoadLayoutFromDatabase(
            //       LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
            //       WADataProvider.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
            //       HttpContext);

            //string controller = ControllerContext.RouteData.Values["controller"].ToString();
            //string action = ControllerContext.RouteData.Values["action"] + "Partial";
            

            ViewResult result = View(WebMessageModel.GetAllMessages(true));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            PartialViewResult result = PartialView(WebMessageModel.GetAllMessages(true));
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
            //return PartialView(WebMessageModel.GetAllMessages(refresh));
        }

    }
}
