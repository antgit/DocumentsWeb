using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Admins.Controllers
{
    [Authorize(Roles = Uid.GROUP_GROUPWEBADMIN)]
    public class ConnectedUserController : Controller
    {
        public ActionResult Index()
        {
            if (Request.ServerVariables.AllKeys.Contains("REMOTE_ADDR"))
                WADataProvider.OnUserActivity(Request.ServerVariables["REMOTE_ADDR"]);
            else
                WADataProvider.OnUserActivity();
            if (MvcApplication.Sessions.ContainsKey(Session.SessionID))
                MvcApplication.Sessions[Session.SessionID] = WADataProvider.CurrentUserName;
            else
                MvcApplication.Sessions.Add(Session.SessionID, WADataProvider.CurrentUserName);

            var data = MvcApplication.Sessions.Where(s => !string.IsNullOrEmpty(s.Value)).Select(s=> new {Name = s.Value}).Distinct().ToList();
            
            /*
            var query = from n in data
                        join u in WADataProvider.WA.Access.GetAllUsers() on n.Name equals u.Name
                        select new {Id = u.Id, Name = u.Agent.Name};
            */
            //var query = from n in data
            //            join u in WADataProvider.WA.Access.GetAllUsers() on n.Name equals u.Name into gj
            //            from subpet in gj.DefaultIfEmpty()
            //            select new { Id = n.Id, Name = (subpet.AgentId==0? string.Empty: subpet.Agent.Name), UidName=n.Name };
            var query = from n in data
                        join u in WADataProvider.WA.Access.GetAllUsers() on n.Name equals u.Name
                        select new { Id = u.Id, LogName = u.Name, Name = u.Agent.Name, LastActivityDate = u.LastActivityDate, LastPasswordChangedDate = u.LastPasswordChangedDate, RemoteAddr = u.RemoteAddr };
            return View(query);
        }
        public ActionResult IndexPartial()
        {
            var data = MvcApplication.Sessions.Where(s => !string.IsNullOrEmpty(s.Value)).Select(s => new {  Name = s.Value }).Distinct().ToList();
            var query = from n in data
                        join u in WADataProvider.WA.Access.GetAllUsers() on n.Name equals u.Name
                        select new { Id = u.Id, LogName = u.Name, Name = u.Agent.Name, LastActivityDate = u.LastActivityDate, LastPasswordChangedDate = u.LastPasswordChangedDate, RemoteAddr = u.RemoteAddr };
            //var query = from n in data
            //            join u in WADataProvider.WA.Access.GetAllUsers() on n.Name equals u.Name into gj
            //            from subpet in gj.DefaultIfEmpty()
            //            select new { Id = n.Id, Name = (subpet.AgentId == 0 ? string.Empty : subpet.Agent.Name), UidName = n.Name };
            return PartialView(query);
        }

    }
}
