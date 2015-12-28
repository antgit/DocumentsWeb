using System;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.Kb.Models;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Kb.Controllers
{
    /// <summary>
    /// Новости
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class NewsController : CoreController<Message>
    {
        public NewsController()
        {
            RootHierachy = Hierarchy.SYSTEM_MESSAGE_WEBNEWS;
            Name = WebModuleNames.WEB_MODULE_NEWS;
        }
        public ActionResult Create(int? hierarchyId=null)
        {
            DateTime now = DateTime.Now;

            NewsModel model = new NewsModel
            {
                //Name = "Новая задача",
                //TaskStateId = WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>("TASK_STATE_INPROGRESS").Id,
                PriorityId = WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>("SYSTEM_PRIORITY_NORMAL").Id,
                UserOwnerId = WADataProvider.CurrentUser.Id,
                //UserToId = WADataProvider.CurrentUser.Id,
                //DateStart = now,
                //DateStartTime = now,
                //DateStartPlan = now,
                //DateStartPlanTime = now,
                HierarchyId = hierarchyId.HasValue ? hierarchyId.Value : 0
            };
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("ControlView", model);
        }

        /// <summary>
        /// Универсальный метод просмотра или редактирования данных
        /// </summary>
        /// <remarks>В зависимости от текущих разрешений пользователя и действующих ограничей 
        /// отображается страница управления или страница просмотра данных</remarks>
        /// <param name="id">Идентификатор объекта</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Open(int id)
        {
            KnowledgeModel model = KnowledgeModel.GetObject(id);
            // опеделение текущих разрешений
            if (model.IsReadOnly ||
                !WADataProvider.LibrariesElementRightView.IsAllow(Right.UIEDIT, Name)
                || WADataProvider.CommonRightView.ReadOnly
                || !WADataProvider.IsCompanyIdAllowEditToCurrentUser(model.MyCompanyId))
                return Preview(id);

            return ControlView(id);
        }

        /// <summary>
        /// Просмотр данных
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Preview(int id)
        {
            return View(NewsModel.GetObject(id));
        }

        /// <summary>
        /// Просмотр данных
        /// </summary>
        /// <param name="id">Идентификатор корреспондента</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ControlView(int id)
        {
            NewsModel model = id == 0 ? new NewsModel { Id = 0, KindId = Message.KINDID_USER } : NewsModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            ViewResult result = View("ControlView", model);
            return result;
        }

        [HttpPost]
        public ActionResult ControlView([ModelBinder(typeof(DevExpressEditorsBinder))] NewsModel model)
        {
            NewsModel modelCashe = (NewsModel)WADataProvider.ModelsCache.Get(model.ModelId);
            if (modelCashe != null)
            {
                //Копирование полей документа, не сохраняющихся на клиенте
                model.Id = modelCashe.Id;
                model.TemplateId = modelCashe.TemplateId;
                model.KindId = modelCashe.KindId;
                //model.MyCompanyId = modelCashe.MyCompanyId;
            }
            model.Memo = HtmlEditorExtension.GetHtml(GlobalPropertyNames.Memo);
            model.UserId = model.UserOwnerId;
            model.PriorityId = WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>("SYSTEM_PRIORITY_NORMAL").Id;
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !NewsModel.CanSave(model.Id))
                {
                    return View("ControlView", model);
                }

                Message obj = model.ToObject();
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();

                
                if (model.Id == 0)
                {
                    //New
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);
                    hierarchy.ContentAdd(obj, true);

                    model.Id = obj.Id;
                    model = NewsModel.GetObject(obj.Id);

                    //WADataProvider.CacheClientsModelData.AddToCashe(hierarchy.Id, model);
                }
                else
                {
                    model.Id = obj.Id;
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);

                    model = NewsModel.GetObject(obj.Id);
                    //WADataProvider.CacheClientsModelData.AddToCashe(hierarchy.Id, model);
                }

                model.Id = obj.Id;

                ViewResult result = View("ControlView", model);
                return result;
            }
            return View(model);
        }

        public void Delete(int id)
        {
            NewsModel.ToTrash(id, RootHierachy);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    NewsModel.SetStatetDone(id, RootHierachy);
                    break;
                case State.STATENOTDONE:
                    NewsModel.SetStateNotDone(id, RootHierachy);
                    break;
                case State.STATEDENY:
                    NewsModel.SetStateDeny(id, RootHierachy);
                    break;
            }
        }

        public void CreateCopy(int id)
        {
            NewsModel.CreateCopy(id, RootHierachy);
        }
        
        public ActionResult MemoPartial(int id)
        {
            NewsModel model = NewsModel.GetObject(id);
            model.Memo = HtmlEditorExtension.GetHtml("Memo");
            return View(model);
        }

    }
}
