using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Agents.Controllers
{
    /// <summary>
    /// Контроллер объектов "Корреспонденты"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexPartial()
        {
            return PartialView();
        }
        //http://devexpress.com/Support/Center/p/E723.aspx
        //
        public ActionResult EditAgent(int id, string kind)
        {
            //Agent ag = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id);

            string cnt = string.Empty;

            switch (kind.ToLower())
            {
                case "bank":
                    cnt = "Bank";
                    break;
                case "byer":
                    cnt = "Byer";
                    break;
                case "supplyer":
                    cnt = "Supplyer";
                    break;
                case "mydepatment":
                    cnt = "MyDepatment";
                    break;
                case "store":
                    cnt = "Store";
                    break;
                case "worker":
                    cnt = "Worker";
                    break;
                default:
                    throw new Exception("Unavailable kind of agent");
            }

            return RedirectToAction("Index", new { Controller = cnt, Edit = id.ToString(CultureInfo.InvariantCulture) });
        }

        public ActionResult PopupEditAgent(int id, string kind)
        {
            string cnt = string.Empty;

            switch (kind.ToLower())
            {
                case "bank":
                    cnt = "Bank";
                    break;
                case "byer":
                    cnt = "Byer";
                    break;
                case "supplyer":
                    cnt = "Supplyer";
                    break;
                case "mydepatment":
                    cnt = "MyDepatment";
                    break;
                case "store":
                    cnt = "Store";
                    break;
                case "worker":
                    cnt = "Worker";
                    break;
                default:
                    throw new Exception("Unavailable kind of agent");
            }
            

            return RedirectToAction("Edit", new { Controller = cnt, Id = id.ToString(CultureInfo.InvariantCulture) });
        }

        #region PeopleFinder
        public ActionResult PeoplesFinderGridPartial()
        {
            string name = Request.Params["Name"];
            bool onlyUsers = bool.Parse(Request.Params["onlyUsers"]);
            int myCompanyId = Request.Params.AllKeys.Contains("MyCompanyId") ? int.Parse(Request.Params["MyCompanyId"]) : 0;

            PartialViewResult result = PartialView();
            result.ViewData.Add("Name", name);
            if (myCompanyId != 0)
                result.ViewData.Add("MyCompanyId", myCompanyId);
            result.ViewData.Add("onlyUsers", onlyUsers);
            result.ViewData.Add("showAgentsInChains", bool.Parse(Request.Params["showAgentsInChains"]));
            return result;
        }
        #endregion

        #region ClientsFinder
        public ActionResult ClientsFinderGridPartial()
        {
            string name = Request.Params["Name"];
            bool onlySupplyer = bool.Parse(Request.Params["OnlySupplyer"]);
            var dataModel = BusinessObjects.Web.Core.AgentWebView.GetView(WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(onlySupplyer ? Hierarchy.SYSTEM_AGENT_SUPPLIERS : Hierarchy.SYSTEM_AGENT_BUYERS), true).Where(f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId) && f.StateId != State.STATEDENY && f.StateId != State.STATEDELETED).Select(Models.ClientModel.ConvertToModel);
            PartialViewResult result = PartialView(dataModel);
            result.ViewData.Add("Name", name);
            return result;
        }
        #endregion
    }
}
