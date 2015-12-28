using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Controllers;
using DocumentsWeb.Areas.Kb.Models;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Kb.Controllers
{
    /// <summary>
    /// База знаний
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class KbBaseController : CoreController<Knowledge>
    {

        public KbBaseController()
        {
            RootHierachy = Hierarchy.GetSystemRootCodeValue(WhellKnownDbEntity.Knowledge);
            Name = WebModuleNames.WEB_KNOWLEDGES;
        }

        public ActionResult Index()
        {
            ViewResult result = View();
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }


        public ActionResult IndexPartial()
        {
            string root = Request.Params["Root"];
            int rootId = 0;

            try { rootId = int.Parse(root); }
            catch { }

            return PartialView(KnowledgeModel.GetCollection(rootId, true));
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
        /// Навигация к данным статьи
        /// </summary>
        /// <param name="id">Идентификатор отчета</param>
        /// <returns></returns>
        public ActionResult NavigateTo(int id)
        {
            KnowledgeModel value = id == 0 ? new KnowledgeModel { Id = 0, Name = "" } : KnowledgeModel.GetObject(id);

            switch(value.KindId)
            {
                case Knowledge.KINDID_ONLINE:
                    return Redirect(value.CodeFind);
                case Knowledge.KINDID_LOCAL:
                    if (value.FileId.HasValue && value.FileId != 0)
                    {
                        FileData file = WADataProvider.WA.GetObject<FileData>(value.FileId.Value);
                        return File(file.StreamData, FileDataModel.GetContentType(file.FileExtention));
                    }
                    else
                    {
                        // TODO: Правильное сообщение о ошибке
                        return Redirect(value.CodeFind);    
                    }
                default:
                    return Redirect(value.CodeFind);
            }
        }

        /// <summary>
        /// Просмотр данных
        /// </summary>
        /// <param name="id">Идентификатор корреспондента</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Preview(int id)
        {
            return View(KnowledgeModel.GetObject(id));
        }
        /// <summary>
        /// Управление данными
        /// </summary>
        /// <param name="id">Идентификатор статьи</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ControlView(int id)
        {
            KnowledgeModel model = id == 0 ? new KnowledgeModel { Id = 0, KindId = Knowledge.KINDID_ONLINE} : KnowledgeModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            ViewResult result = View("ControlView", model);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        [HttpPost]
        public ActionResult ControlView([ModelBinder(typeof(DevExpressEditorsBinder))] KnowledgeModel model)
        {
            KnowledgeModel modelCashe = (KnowledgeModel)WADataProvider.ModelsCache.Get(model.ModelId);

            //Если снимается флаг IsReadOnly
            if (modelCashe.IsReadOnly && !model.IsReadOnly)
            {
                // Действие доступно только администраторам, web администраторам
                if (WADataProvider.CurrentUser.Groups.FirstOrDefault(s => s.Name == Uid.GROUP_GROUPSYSTEMADMIN || s.Name == Uid.GROUP_GROUPWEBADMIN) != null)
                {
                    //Снятие флага без проверки валидности модели
                    modelCashe.IsReadOnly = false;
                    Knowledge obj = modelCashe.ToObject();
                    obj.Save();
                    return RedirectToAction("ControlView", new { Id = obj.Id });
                }
            }
            if (modelCashe != null)
            {
                //Копирование полей документа, не сохраняющихся на клиенте
                model.Id = modelCashe.Id;
                model.TemplateId = modelCashe.TemplateId;
                model.KindId = modelCashe.KindId;
                model.MyCompanyId = modelCashe.MyCompanyId;
            }
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !KnowledgeModel.CanSave(model.Id))
                {
                    return View("ControlView", model);
                }

                Knowledge obj = model.ToObject();
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();

                if (model.Id == 0)
                {
                    //New
                    //Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_BUYERS);
                    //hierarchy.ContentAdd(obj, true);

                    model.Id = obj.Id;
                    model = KnowledgeModel.GetObject(obj.Id);

                    //WADataProvider.CacheClientsModelData.AddToCashe(hierarchy.Id, model);
                }
                else
                {
                    model.Id = obj.Id;
                    //Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_BUYERS);

                    model = KnowledgeModel.GetObject(obj.Id);
                    //WADataProvider.CacheClientsModelData.AddToCashe(hierarchy.Id, model);
                }

                model.Id = obj.Id;

                ViewResult result = View("ControlView", model);
                return result;
            }
            return View(model);
        }

        public ActionResult Edit(int id, int kindId, int hieId = 0)
        {
            KnowledgeModel model = id == 0 ? new KnowledgeModel { Id = 0, KindId = kindId, HierarchyId = hieId } : KnowledgeModel.GetObject(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] KnowledgeModel model)
        {
            if (model.KindId == Knowledge.KINDID_ONLINE && string.IsNullOrEmpty(model.CodeFind))
            {
                ModelState.AddModelError("CodeFind", "URL должен быть задан");
            }
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !KnowledgeModel.CanSave(model.Id))
                {
                    return View("PopupWindowClose", model);
                }

                Knowledge obj = model.ToObject();
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();

                if (model.Id == 0 && model.HierarchyId > 0)
                {
                    Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(model.HierarchyId);
                    h.ContentAdd(obj, true);
                }
                model.Id = obj.Id;
                return View("PopupWindowClose", model);
            }
            return View("Edit", model);
        }
        /// <summary>
        /// Удаление в корзину
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            KnowledgeModel.ToTrash(id, RootHierachy);
        }
        /// <summary>
        /// Изменение состояния
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    KnowledgeModel.SetStatetDone(id, RootHierachy);
                    break;
                case State.STATENOTDONE:
                    KnowledgeModel.SetStateNotDone(id, RootHierachy);
                    break;
                case State.STATEDENY:
                    KnowledgeModel.SetStateDeny(id, RootHierachy);
                    break;
            }
        }
        /// <summary>
        /// Создание копии
        /// </summary>
        /// <param name="id"></param>
        public void CreateCopy(int id)
        {
            KnowledgeModel.CreateCopy(id, RootHierachy);
        }
    }
}
