using System;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Kb.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Kb.Controllers
{
    /// <summary>
    /// Задачи
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPTASK })]
    public class TaskController : CoreController<Task>
    {

        public TaskController()
        {
            RootHierachy = Hierarchy.GetSystemRootCodeValue(WhellKnownDbEntity.Task);
            Name = WebModuleNames.WEB_TASKS;
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

            return Edit(id);
        }
        public ActionResult Create(int? hierarchyId=null)
        {
            DateTime now = DateTime.Now;

            TaskModel model = new TaskModel
            {
                //Name = "Новая задача",
                TaskStateId = WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>("TASK_STATE_INPROGRESS").Id,
                PriorityId = WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>("SYSTEM_PRIORITY_NORMAL").Id,
                UserOwnerId = WADataProvider.CurrentUser.Id,
                UserToId = WADataProvider.CurrentUser.Id,
                DateStart = now,
                DateStartTimeAsDate = now,
                DateStartPlan = now,
                DateStartPlanTimeAsDate = now,
                HierarchyId = hierarchyId.HasValue ? hierarchyId.Value : 0
            };
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("Edit", model);
        }

        public ActionResult Edit(int id)
        {
            //int id_=0;
            //TaskModel tmp =null;
            //if (id.Length == 36)
            //{
            //    tmp = (TaskModel) WADataProvider.ModelsCache.Get(id);

            //     id_ = tmp.Id;
            //}
            //else if (Int32.TryParse(id, out id_))
            //{
            //    tmp = TaskModel.GetObject(id_, false);
            //    WADataProvider.ModelsCache.Add(tmp.ModelId, tmp);
            //    id_ = tmp.Id;
            //}

            if (id == 0)
                return Create();
            TaskModel model = TaskModel.GetObject(id, false);
            //model.ModelId = tmp.ModelId;
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] TaskModel model)
        {
            TaskModel cache = (TaskModel) WADataProvider.ModelsCache.Get(model.ModelId);

            //Если снимается флаг IsReadOnly
            if(cache.IsReadOnly && !model.IsReadOnly)
            {
                // Действие доступно только администраторам, web администраторам
                if (WADataProvider.CurrentUser.Groups.FirstOrDefault(s => s.Name == Uid.GROUP_GROUPSYSTEMADMIN || s.Name == Uid.GROUP_GROUPWEBADMIN) != null)
                {
                    //Снятие флага без проверки валидности модели
                    cache.IsReadOnly = false;
                    Task obj = cache.ToObject();
                    obj.Save();
                    return RedirectToAction("Edit", new {Id = obj.Id});
                }
            }

            model.Memo = HtmlEditorExtension.GetHtml("Memo");
            model.Id = cache.Id;
            model.KindId = cache.KindId;
            model.Files = cache.Files;
            model.Notes = cache.Notes;

            if (ModelState.IsValid)
            {
                Task obj = model.ToObject();
                obj.Save();
                
                if(model.Id==0 &&  model.HierarchyId>0)
                {
                    Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(model.HierarchyId);
                    h.ContentAdd(obj, true);
                }
                model.Id = obj.Id;
                model.SaveFiles(obj.GetLinkedFiles());
                model.SaveNotes(obj.GetLinkedNotes());

                TaskModel tmp = TaskModel.ConvertToModel(obj);
                tmp.ModelId = model.ModelId;
                WADataProvider.ModelsCache.Remove(model.ModelId);
                WADataProvider.ModelsCache.Add(tmp.ModelId, tmp);

                return RedirectToAction("Edit", new { Id = model.Id });
            }

            return View(model);
        }

        public void Delete(int id)
        {
            TaskModel.ToTrash(id, RootHierachy);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    TaskModel.SetStatetDone(id, RootHierachy);
                    break;
                case State.STATENOTDONE:
                    TaskModel.SetStateNotDone(id, RootHierachy);
                    break;
                case State.STATEDENY:
                    TaskModel.SetStateDeny(id, RootHierachy);
                    break;
            }
        }

        public void CreateCopy(int id)
        {
            TaskModel.CreateCopy(id, RootHierachy);
        }

        public ActionResult Preview(int id)
        {
            return View(TaskModel.GetObject(id));
        }

        public ActionResult MemoPartial(int id)
        {
            TaskModel model = TaskModel.GetObject(id);
            model.Memo = HtmlEditorExtension.GetHtml("Memo");
            return View(model);
        }

        public ActionResult ShowUserFlagsPartial(string modelId)
        {
            TaskModel cache = (TaskModel)WADataProvider.ModelsCache.Get(modelId);
            return PartialView("ShowUserFlagsPartial", cache);
        }
        
        public ActionResult UserFlagsPartial()
        {
            return PartialView("UserFlagsPartial");
        }

        
        public ActionResult ShowPeoplesFinderAutorPartial()
        {
            ViewData["Name"] = "PeopleFinderAuthor";
            ViewData["ComboboxName"] = "UserOwnerId";
            ViewData["ComboboxButtonIndex"] = 0;
            ViewData["onlyUsers"] = false;
            return PartialView("PeoplesFinderPartial");
        }
        public ActionResult ShowPeopleFinderCreatorPartial()
        {
            ViewData["Name"] = "PeopleFinderCreator";
            ViewData["ComboboxName"] = "UserToId";
            ViewData["ComboboxButtonIndex"] = 0;
            ViewData["onlyUsers"] = false;
            return PartialView("PeoplesFinderPartial");
        }
        public virtual bool IsValidated()
        {
            //TaskModel cache = (TaskModel)WADataProvider.ModelsCache.Get(modelId);
            //TryValidateModel(model);
            return ModelState.IsValid;
        }
        
    }
}
