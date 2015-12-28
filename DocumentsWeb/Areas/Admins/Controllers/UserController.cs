using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Security;
using System.Web.Mvc;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Admins.Controllers
{
    [Authorize(Roles = Uid.GROUP_GROUPWEBADMIN)]
    public class UserController : CoreController
    {
        public UserController()
        {
            Name = WebModuleNames.WEB_ADMINUSERS;
        }

        public ActionResult Index()
        {
            return View(UserModel.GetCollection(true));
        }

        public ActionResult IndexPartial()
        {
            //ViewBag.Message = "Главная";

            return PartialView(UserModel.GetCollection());
        }

        /// <summary>
        /// Просмотр данных
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Preview(int id)
        {
            return View(UserModel.GetObject(id));
        }
        /// <summary>
        /// Управление данными
        /// </summary>
        /// <param name="id">Идентификатор корреспондента</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ControlView(int id)
        {
            UserModel model = id == 0 ? new UserModel { Id = 0, KindId = Uid.KINDID_USER, Name = "Новый пользователь", IsActive = true, Email = "user@userdomain.ua" } : UserModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);

            ViewResult result = View("ControlView", model);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        [HttpPost]
        public ActionResult ControlView([ModelBinder(typeof(DevExpressEditorsBinder))] UserModel model)
        {
            UserModel modelCashe = (UserModel)WADataProvider.ModelsCache.Get(model.ModelId);
            if (modelCashe != null)
            {
                //Копирование полей документа, не сохраняющихся на клиенте
                model.Id = modelCashe.Id;
                model.TemplateId = modelCashe.TemplateId;
                model.KindId = modelCashe.KindId;
            }
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !UserModel.CanSave(model.Id))
                {
                    return View("ControlView", model);
                }
                Uid obj = model.ToObject(WADataProvider.WA);
                if (model.Id == 0)
                {
                   
                    obj.Save();
                    Uid grp = WADataProvider.WA.GetCollection<Uid>().FirstOrDefault(s => s.Name == Uid.GROUP_ALLUSERS);
                    if (!obj.Workarea.Access.IsUserExistsInGroup(obj.Name, Uid.GROUP_ALLUSERS))
                        obj.IncludeInGroup(grp);
                    Uid defaultGroup =
                        WADataProvider.WA.Cashe.GetCasheData<Uid>().ItemCode<Uid>(Uid.GROUP_GROUPWEBUSER);
                    obj.IncludeInGroup(defaultGroup);
                    if (model.IsAdmin)
                    {
                        obj.Workarea.Access.IncludeInGroup(obj.Name, Uid.GROUP_GROUPWEBADMIN);
                    }
                    else
                        obj.Workarea.Access.ExcludeFromGroup(obj.Name, Uid.GROUP_GROUPWEBADMIN);


                    obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                    obj.Save();
                }
                else
                {
                    //Old User
                    Uid uid = model.ToObject(WADataProvider.WA);
                    uid.Save();

                    if (model.IsAdmin)
                        uid.Workarea.Access.IncludeInGroup(uid.Name, Uid.GROUP_GROUPWEBADMIN);
                    else
                        uid.Workarea.Access.ExcludeFromGroup(uid.Name, Uid.GROUP_GROUPWEBADMIN);
                }
                if (model.Id == 0)
                {
                    model.Id = obj.Id;
                    model = UserModel.GetObject(obj.Id);
                }
                else
                {
                    model.Id = obj.Id;
                    model = UserModel.GetObject(obj.Id);
                }

                model.Id = obj.Id;

                ViewResult result = View("ControlView", model);
                return result;
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            UserModel model = new UserModel
                {
                    Id = 0, 
                    Name = "Новый пользователь", 
                    IsActive = true, 
                    Email = "user@userdomain.ua",
                    MyCompanyId = WADataProvider.CurrentUser.MyCompanyId
                };
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("Edit", model);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id == 0)
                return Create();   
            UserModel value = UserModel.GetObject(id);
            return View(value);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] UserModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == 0)
                {
                    //New user
                    //Uid uid = new Uid { Workarea = WADataProvider.WA, KindId = Uid.KINDID_USER, Name = model.Name };
                    Uid uid = model.ToObject(WADataProvider.WA);
                    uid.Save();
                    Uid grp = WADataProvider.WA.GetCollection<Uid>().FirstOrDefault(s => s.Name == Uid.GROUP_ALLUSERS);
                    if (!uid.Workarea.Access.IsUserExistsInGroup(uid.Name, Uid.GROUP_ALLUSERS))
                        uid.IncludeInGroup(grp);
                    Uid defaultGroup = WADataProvider.WA.Cashe.GetCasheData<Uid>().ItemCode<Uid>(Uid.GROUP_GROUPWEBUSER);
                    uid.IncludeInGroup(defaultGroup);
                    //uid.Memo = model.Memo;
                    //uid.Email = model.Email;
                    if (model.IsAdmin)
                    {
                        uid.Workarea.Access.IncludeInGroup(uid.Name, Uid.GROUP_GROUPWEBADMIN);
                    }
                    else
                        uid.Workarea.Access.ExcludeFromGroup(uid.Name, Uid.GROUP_GROUPWEBADMIN);

                    uid.Save();
                }
                else
                {
                    //Old User
                    Uid uid = model.ToObject(WADataProvider.WA);
                    uid.Save();

                    if (model.IsAdmin)
                        uid.Workarea.Access.IncludeInGroup(uid.Name, Uid.GROUP_GROUPWEBADMIN);
                    else
                        uid.Workarea.Access.ExcludeFromGroup(uid.Name, Uid.GROUP_GROUPWEBADMIN);

                }
                return View("PopupWindowClose", model);

            }
            return View(model);
        }

        /// <summary>
        /// Экспорт таблицы в файл
        /// </summary>
        /// <param name="type">Тип файла (xls, pdf)</param>
        /// <returns></returns>
        public ActionResult ExportTo(string type)
        {
            GridViewSettings settings = new GridViewSettings { Name = "Пользователи" };
            //settings.Columns.Add("Id", "Ид");
            settings.Columns.Add("Name", "Имя");
            settings.Columns.Add("NameFull", "ФИО");
            settings.Columns.Add("Email", "Email");

            switch (type)
            {
                case "XLSX":
                    return GridViewExtension.ExportToXlsx(settings, UserModel.GetCollection());
                case "PDF":
                    return GridViewExtension.ExportToPdf(settings, UserModel.GetCollection());
                default:
                    throw new ArgumentException("Неизвестный тип данных для экспорта");
            }
        }

        public ActionResult UserGroupsPartial(int id)
        {
            return PartialView(UserModel.GetObject(id));
        }

        public ActionResult WorkerPartial(int id)
        {
            int companyId = int.Parse(Request.Params["MyCompanyId"] == null || Request.Params["MyCompanyId"] == "null" ? "0" : Request.Params["MyCompanyId"]);
            //return PartialView(UserModel.GetObject(id));
            UserModel model = UserModel.GetObject(id);
            model.MyCompanyId = companyId;
            return PartialView(model);
        }

        public void IncludeUserInGroup(int userId, int groupId, bool include)
        {
            Uid user = WADataProvider.WA.GetObject<Uid>(userId);
            Uid group = WADataProvider.WA.GetObject<Uid>(groupId);
            if (include)
                user.IncludeInGroup(group);
            else
                user.ExcludeFromGroup(group);
        }
    }
}
