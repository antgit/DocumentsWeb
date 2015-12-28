using System;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.UserPersonal.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.UserPersonal.Controllers
{
    [Authorize(Roles = Uid.GROUP_GROUPWEBUSER)]
    public class UserMessageController : CoreController<Message>
    {
        public UserMessageController()
        {
            RootHierachy = Hierarchy.SYSTEM_MESSAGE_USERS; 
            Name = WebModuleNames.WEB_USERMESSAGE;
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            WebMessageModel model = id == 0 ? new WebMessageModel { Id = 0, Name = "" } : WebMessageModel.GetObject(id);
            if(model.Id==0)
            {
                model.UserOwnerId = WADataProvider.CurrentUser.Id;
                model.PriorityId = WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>("SYSTEM_PRIORITY_NORMAL").Id;
            }
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] WebMessageModel model)
        {
            model.NameFull = HtmlEditorExtension.GetHtml("NameFull");
            model.Files = ((WebMessageModel) WADataProvider.ModelsCache.Get(model.ModelId)).Files;
            if (ModelState.IsValid)
            {
                Message message = model.ToObject();
                message.Save();
                if(model.Id==0)
                {
                    Hierarchy hroot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_MESSAGE_USERS);
                    if (hroot != null)
                        hroot.ContentAdd<Message>(message);
                }
                model.Id = message.Id;
                model.SaveFiles(message.GetLinkedFiles());

                //Отчет о прочтении
                if (model.IsIncomminMessage && model.ReadDone)
                    message.CreateMessageReply();

                return RedirectToAction("Edit", new { Id = model.Id });


            }
            return View(model);
        }

        /// <summary>
        /// Быстрый просмотр списка входящих, непрочитанных сообщений
        /// </summary>
        /// <returns></returns>
        public ActionResult FastViewIncommingPartial()
        {
            return PartialView();
        }
        public ActionResult ShowNewUserMessagesPartial()
        {
            return PartialView("ShowNewUserMessagesPartial");
        }
        
        public ActionResult Preview(int id)
        {
            return View("Preview", WebMessageModel.GetObject(id));
        }

        public ActionResult NameFullPartial(string modelId)
        {
            WebMessageModel model = (WebMessageModel) WADataProvider.ModelsCache.Get(modelId);
            model.Memo = HtmlEditorExtension.GetHtml("Memo");
            return View(model);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    WebMessageModel.SetStatetDone(id, RootHierachy);
                    break;
                case State.STATENOTDONE:
                    WebMessageModel.SetStateNotDone(id, RootHierachy);
                    break;
                case State.STATEDENY:
                    WebMessageModel.SetStateDeny(id, RootHierachy);
                    break;
            }
        }
        public void ToTrash(int id)
        {
            WebMessageModel.ToTrash(id);
        }
        protected override void OnAuthorizationDeleteAction<T>(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.ToUpper() == "DELETE")
            {
                if (!WADataProvider.LibrariesElementRightView.IsAllow(Right.UITRASH, Name))
                {
                    throw new SecurityException("Удаление запрещено!");
                    //filterContext.Result = new HttpUnauthorizedResult();
                }
                string valueParam = filterContext.HttpContext.Request.QueryString["Id"];
                int objId = 0;
                Int32.TryParse(valueParam, out objId);
                if (objId != 0)
                {
                    T obj = WADataProvider.WA.Cashe.GetCasheData<T>().Item(objId);
                    if (obj.IsSystem || obj.IsReadOnly)
                    {
                        throw new SecurityException("Запись является системной или предназначена только для чтения!");
                        //filterContext.Result = new HttpUnauthorizedResult();     
                    }
                    if (obj is ICompanyOwner)
                    {
                        ICompanyOwner companyObj = obj as ICompanyOwner;
                        if (!WADataProvider.IsCompanyIdAllowIdToCurrentUser(companyObj.MyCompanyId))
                        {
                            throw new SecurityException("Удаление данных вне собственной компании запрещено!");
                        }
                    }

                }
            }
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
                //    Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(objId);
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

        protected override void OnAuthorizationViewAction<T>(AuthorizationContext filterContext)
        {
            if (filterContext.ActionDescriptor.ActionName.ToUpper() == "OPEN")
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
                if (!(WADataProvider.LibrariesElementRightView.IsAllow(Right.UIEDIT, Name) |
                      WADataProvider.LibrariesElementRightView.IsAllow(Right.UIVIEW, Name)))
                {
                    throw new SecurityException("Отсутствуют разрешения на изменение данных!");
                    filterContext.Result = new HttpUnauthorizedResult();
                }

                //if (objId != 0)
                //{
                //    Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(objId);
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
    }
}
