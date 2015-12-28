using System;
using System.Collections;
using System.Linq;
using System.Web.Security;
using System.Web.Mvc;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using BusinessObjects;
using DocumentsWeb.Models;
using DevExpress.Web.Mvc;

namespace DocumentsWeb.Areas.UserPersonal.Controllers
{
    /// <summary>
    /// Личная страница пользователя
    /// </summary>
    [Authorize(Roles = Uid.GROUP_GROUPWEBUSER)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Uid user = WADataProvider.CurrentUser;// (Membership.Provider as BusinessObjectMembershipProvider).GetUid(System.Web.HttpContext.Current.User.Identity.Name);
            if (user != null)
            {
                user.Refresh(true);
                if(user.AgentId!=0)
                {
                    user.Agent.Refresh(true);
                }
                UserModel model = UserModel.ConvertToModel(user);

                try
                {
                    WADataProvider.ModelsCache.Add(model.ModelId, model);
                }
                catch (ArgumentException)
                {

                }

                string filePath = Request.MapPath("~/Images/" + user.Id + ".png");
                if (System.IO.File.Exists(filePath))
                {
                    model.Avatar = user.Id.ToString();
                }
                else
                {
                    model.Avatar = "noavatar.png";
                }

                ViewResult res = View(model);
                //string selNodeName = !string.IsNullOrEmpty(selectedNodeName) ? selectedNodeName : "Messages";
                //res.ViewData.Add("SelectedNodeName", selNodeName);
                return res;
            }
            else
            {
                return RedirectToAction("LogOn", "Account");
                //return View("Welcome");
            }
        }

        public ActionResult IndexPartial()
        {
            Uid user = WADataProvider.CurrentUser;// (Membership.Provider as BusinessObjectMembershipProvider).GetUid(System.Web.HttpContext.Current.User.Identity.Name);
            user.Refresh(true);
                if (user.AgentId != 0)
                {
                    user.Agent.Refresh(true);
                }
                UserModel model = UserModel.ConvertToModel(user);

                try
                {
                    WADataProvider.ModelsCache.Add(model.ModelId, model);
                }
                catch (ArgumentException)
                {

                }

                string filePath = Request.MapPath("~/Images/" + user.Id + ".png");
                if (System.IO.File.Exists(filePath))
                {
                    model.Avatar = user.Id.ToString();
                }
                else
                {
                    model.Avatar = "noavatar.png";
                }

                return View(model);
        }

        public ActionResult Edit(int? id)
        {
            UserPersonalModel model = UserPersonalModel.ConvertToModel(WADataProvider.CurrentUser);
            return View("Edit", model);
        }
        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] UserPersonalModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !UserPersonalModel.CanSave(model.Id))
                {
                    return View("PopupWindowClose", model);
                }
                Uid uid = model.ToObject(WADataProvider.WA);
                uid.Email = model.Email;
                uid.Save();
                if(uid.AgentId!=0)
                {
                    uid.Agent.People.FirstName = model.WorkerFirstName;
                    uid.Agent.People.LastName = model.WorkerLastName;
                    uid.Agent.People.MidleName = model.WorkerMidleName;
                    uid.Agent.People.Save();
                }
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    uid.Password = model.NewPassword;
                    uid.Save();
                    (Membership.Provider as BusinessObjectMembershipProvider).RefreshUser(uid.Name);
                }
                //Analitic obj = model.ToObject();
                //obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                //obj.Save();

                //if (model.Id == 0)
                //{
                //    Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHie);
                //    h.ContentAdd(obj, true);
                //}
                //model.Id = obj.Id;
                return View("PopupWindowClose", model);
            }
            return View("Edit", model);
        }



        public ActionResult ViewMySections()
        {

            return PartialView(GetMySections());
        }

        public static IEnumerable GetMySections()
        {
            string[] modules = new string[]
                                   {
                                       WebModuleNames.WEB_DOCSERVICENDS,
                                       WebModuleNames.WEB_DOCSERVICE,
                                       WebModuleNames.WEB_DOCSALE,
                                       WebModuleNames.WEB_DOCSALENDS,
                                       WebModuleNames.WEB_DOCTAX,
                                       WebModuleNames.WEB_DOCDOGOVOR,
                                       WebModuleNames.WEB_TASKS,
                                       WebModuleNames.WEB_USERMESSAGE
                                   };

            //CodeName codeController = WADataProvider.WA.GetCollection<CodeName>().FirstOrDefault(f => f.Code == "CONTROLLER" && f.ToEntityId==(int)WhellKnownDbEntity.Library);
            //CodeName codeMethod = WADataProvider.WA.GetCollection<CodeName>().FirstOrDefault(f => f.Code == "METHOD" && f.ToEntityId==(int)WhellKnownDbEntity.Library);

            //int codeMethodId = codeMethod != null ? codeMethod.Id : 0;
            //int codeControllerId = codeController != null ? codeController.Id : 0;

            //CodeValueView viewcodeMethod = (WADataProvider.WA.Cashe.GetCasheData<Library>().ItemCode<Library>("") as ICodes<Library>).GetView(true).
            //    FirstOrDefault(f => f.CodeNameId == codeMethodId);
            //CodeValueView viewcodeController = (WADataProvider.WA.Cashe.GetCasheData<Library>().ItemCode<Library>("") as ICodes<Library>).GetView(true).
            //    FirstOrDefault(f => f.CodeNameId == codeControllerId);

            var query1 = from f in modules
                         select new
                         {
                             Id = WADataProvider.WA.Cashe.GetCasheData<Library>().ItemCode<Library>(f),
                             Code = f,
                             Name = WADataProvider.WA.Cashe.GetCasheData<Library>().ItemCode<Library>(f).Name,
                             IsAllow = WADataProvider.LibrariesElementRightView.IsAllow(Right.VIEW, f),
                             NavigateUrl = WADataProvider.WA.Cashe.GetCasheData<Library>().ItemCode<Library>(f).TypeUrl
                         };

            return query1.Where(f => f.IsAllow).OrderBy(s=>s.Name).ToList();
        }

        
    }
}
