using System;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.UserPersonal.Models;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.UserPersonal.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize(Roles = Uid.GROUP_GROUPWEBUSER)]
    public class UserNoteController : CoreController<Note>
    {
        public UserNoteController()
        {
         
            RootHierachy = Hierarchy.SYSTEM_MESSAGE_USERS; 
            Name = WebModuleNames.WEB_USERMESSAGE;
        }

        protected override void OnAuthorizationEditAction<T>(AuthorizationContext filterContext) 
        {
            if (filterContext.ActionDescriptor.ActionName.ToUpper() == "EDIT"
                || filterContext.ActionDescriptor.ActionName.ToUpper() == "CHANGESTATE"
                || filterContext.ActionDescriptor.ActionName.ToUpper() == "CREATECOPY"
                || filterContext.ActionDescriptor.ActionName.ToUpper() == "CREATE")
            {
                int objId = 0;
                //filterContext.RouteData.Values["id"]
                if (filterContext.HttpContext.Request.QueryString.AllKeys.Contains("id"))
                {
                    string valueParam = filterContext.HttpContext.Request.QueryString["Id"];
                    Int32.TryParse(valueParam, out objId);
                }
                if (filterContext.RouteData.Values.ContainsKey("id"))
                {
                    Int32.TryParse(filterContext.RouteData.Values["id"].ToString(), out objId);
                }
                if (objId != 0 && !WADataProvider.LibrariesElementRightView.IsAllow(Right.UIEDIT, Name))
                {
                    throw new SecurityException("Отсутствуют разрешения на изменение данных!");
                    //filterContext.Result = new HttpUnauthorizedResult();
                }
                if (objId == 0 && !WADataProvider.LibrariesElementRightView.IsAllow(Right.UICREATE, Name))
                {
                    throw new SecurityException("Отсутствуют разрешения на создание данных!");
                    //filterContext.Result = new HttpUnauthorizedResult();
                }

                //if (objId != 0)
                //{
                //    T obj = WADataProvider.WA.Cashe.GetCasheData<T>().Item(objId);
                //    if (obj is ICompanyOwner)
                //    {
                //        ICompanyOwner companyObj = obj as ICompanyOwner;
                //        if (!WADataProvider.IsCompanyIdAllowIdToCurrentUser(companyObj.MyCompanyId))
                //        {
                //            throw new SecurityException("Изменение данных вне собственной компании запрещено!");
                //        }
                //    }
                //}
            }
        }
       public ActionResult ViewMyNotes()
       {

            return PartialView(UserNoteModel.GetMyNotes());
       }
       public ActionResult Edit(int id)
       {
           UserNoteModel model = id == 0 ? new UserNoteModel() : UserNoteModel.GetObject(id);
           return View("Edit", model);
       }

       [HttpPost]
       public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] UserNoteModel model)
       {
           if (ModelState.IsValid)
           {
               Note obj = model.ToObject();
               obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
               obj.Save();

               
               model.Id = obj.Id;
               return View("PopupWindowClose", model);
           }
           return View("Edit", model);
       }

       public void Delete(int id)
       {
           UserNoteModel.ToTrash(id);
       }

    }
}
