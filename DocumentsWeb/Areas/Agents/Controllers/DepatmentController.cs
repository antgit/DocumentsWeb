using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Agents.Controllers
{
    /// <summary>
    /// Контроллер объектов "Подразделения и отделы"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class DepatmentController : CoreController<Depatment>
    {
        public DepatmentController()
        {
            Name = WebModuleNames.WEB_DEPATMENTS;
            RootHierachy = Hierarchy.GetSystemFavoriteCodeValue(WhellKnownDbEntity.Depatment);
            // "SYSTEM_DEPATMENT_FAVORITE"
        }

        public ActionResult Index()
        {
            //LayoutHelper.LoadLayoutFromDatabase(
            //     LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
            //     WADataProvider.WA.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
            //     HttpContext);

            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"] + "Partial";

            List<WebDepatmentModel> list = new List<WebDepatmentModel>();
            int[] roots = Utils.GetHieRoots(controller, action);
            if (roots.Contains(0))
            {
                list = WebDepatmentModel.GetCollection(RootHierachy, false).OfType<WebDepatmentModel>().ToList();
                //return View(WebDepatmentModel.GetCollection(RootHierachy, false));
                //list = WebDepatmentModel.GetCollection(RootHierachy, false);
                //list = WebDepatmentModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy).Select(s => s.Code).ToArray<string>());
            }
            else
                list = WebDepatmentModel.GetCollectionWONested(roots);
            ViewResult result = View(list);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"].ToString();
            
            int[] roots = Utils.GetHieRoots(controller, action);
            List<WebDepatmentModel> list = roots.Contains(0) ? WebDepatmentModel.GetCollection(RootHierachy, false).OfType<WebDepatmentModel>().ToList() : WebDepatmentModel.GetCollectionWONested(roots);

            return PartialView(list);
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
            WebDepatmentModel model = WebDepatmentModel.GetObject(id);
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
        /// <param name="id">Идентификатор корреспондента</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Preview(int id)
        {
            return View("Preview", WebDepatmentModel.GetObject(id));
        }
        /// <summary>
        /// Управдение данными
        /// </summary>
        /// <param name="id">Идентификатор корреспондента</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ControlView(int id)
        {
            WebDepatmentModel model = id == 0 ? new WebDepatmentModel { Id = 0, KindId = Depatment.KINDID_DEPATMENT } : WebDepatmentModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            ViewResult result = View("ControlView", model);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        [HttpPost]
        public ActionResult ControlView([ModelBinder(typeof(DevExpressEditorsBinder))] WebDepatmentModel model)
        {
            WebDepatmentModel modelCashe = (WebDepatmentModel)WADataProvider.ModelsCache.Get(model.ModelId);
            if (modelCashe != null)
            {
                //Копирование полей документа, не сохраняющихся на клиенте
                model.Id = modelCashe.Id;
                model.TemplateId = modelCashe.TemplateId;
                model.KindId = modelCashe.KindId;
            }
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !ClientModel.CanSave(model.Id))
                {
                    return View("ControlView", model);
                }

                Depatment obj = model.ToObject(WADataProvider.WA);
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();

                if (model.Id == 0)
                {
                    //New
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);
                    hierarchy.ContentAdd(obj, true);

                    model.Id = obj.Id;
                    model = WebDepatmentModel.GetObject(obj.Id);

                    WADataProvider.CacheDepatmentsModelData.AddToCashe(hierarchy.Id, model);
                }
                else
                {
                    model.Id = obj.Id;
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);

                    model = WebDepatmentModel.GetObject(obj.Id);
                    WADataProvider.CacheDepatmentsModelData.AddToCashe(hierarchy.Id, model);
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
            WebDepatmentModel model = new WebDepatmentModel { Id = 0, KindId = Depatment.KINDID_DEPATMENT };
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("Edit", model);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            WebDepatmentModel model = id == 0 ? new WebDepatmentModel { Id = 0, KindId = Depatment.KINDID_DEPATMENT} : WebDepatmentModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("Edit", model);    
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] WebDepatmentModel model)
        {
            ClientModel modelCashe = (ClientModel)WADataProvider.ModelsCache.Get(model.ModelId);
            if (modelCashe != null)
            {
                //Копирование полей документа, не сохраняющихся на клиенте
                model.Id = modelCashe.Id;
                model.TemplateId = modelCashe.TemplateId;
                model.KindId = modelCashe.KindId;
                // Идентификатор компании владельца у отдела задается в окне редактирования!!!
                //model.MyCompanyId = modelCashe.MyCompanyId;
            }
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !WebDepatmentModel.CanSave(model.Id))
                {
                    return View("PopupWindowClose", model);
                }

                Depatment obj = model.ToObject(WADataProvider.WA);
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();

                if (model.Id == 0)
                {
                    //New
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);
                    hierarchy.ContentAdd(obj, true);

                    model.Id = obj.Id;
                    model = WebDepatmentModel.GetObject(obj.Id);

                    WADataProvider.CacheDepatmentsModelData.AddToCashe(hierarchy.Id, model);
                }
                else
                {
                    model.Id = obj.Id;
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);

                    model = WebDepatmentModel.GetObject(obj.Id);
                    WADataProvider.CacheDepatmentsModelData.AddToCashe(hierarchy.Id, model);
                }

                model.Id = obj.Id;
                return View("PopupWindowClose", model);
                //return View("Details", model);
            }
            return View(model);
        }

        public void Delete(int id)
        {
            WebDepatmentModel.ToTrash(id, Hierarchy.SYSTEM_AGENT_BUYERS);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    WebDepatmentModel.SetStatetDone(id, Hierarchy.SYSTEM_AGENT_BUYERS);
                    break;
                case State.STATENOTDONE:
                    WebDepatmentModel.SetStateNotDone(id, Hierarchy.SYSTEM_AGENT_BUYERS);
                    break;
                case State.STATEDENY:
                    WebDepatmentModel.SetStateDeny(id, Hierarchy.SYSTEM_AGENT_BUYERS);
                    break;
            }
        }

        public void CreateCopy(int id)
        {
            WebDepatmentModel.CreateCopy(id, Hierarchy.SYSTEM_AGENT_BUYERS);
        }

        /// <summary>
        /// Экспорт таблицы в файл
        /// </summary>
        /// <param name="type">Тип файла (xls, pdf)</param>
        /// <param name="subtype">Подтип данных</param>
        /// <returns></returns>
        public ActionResult ExportTo(string type, string subtype)
        {
            GridViewSettings settings = new GridViewSettings { Name = "Покупатели" };
            //settings.Columns.Add("Id", "Ид");
            settings.Columns.Add("Name", "Имя");
            settings.Columns.Add("NameFull", "Печатное наименование");
            settings.Columns.Add("Code", "Код");

            IEnumerable coll = WebDepatmentModel.GetCollection();

            switch (type)
            {
                case "XLSX":
                    return GridViewExtension.ExportToXlsx(settings, coll);
                case "PDF":
                    return GridViewExtension.ExportToPdf(settings, coll);
                default:
                    throw new ArgumentException("Неизвестный тип данных для экспорта");
            }
        }


        #region CodesGridPartial
        public ActionResult CodesGridPartial(int agId)
        {
            return PartialView("CodesGridPartial", WebDepatmentModel.GetObject(agId));
        }

        public ActionResult AddNewCodePartial([ModelBinder(typeof(DevExpressEditorsBinder))] CodeValueModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject<Agent>().Save();
            }
            return PartialView("CodesGridPartial", WebDepatmentModel.GetObject(model.ElementId));
        }

        public ActionResult UpdateCodePartial([ModelBinder(typeof(DevExpressEditorsBinder))] CodeValueModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject<Agent>().Save();
            }
            return PartialView("CodesGridPartial", WebDepatmentModel.GetObject(model.ElementId));
        }

        //public ActionResult DeleteCodePartial(int id)
        //{
        //    CodeValue<Agent> code = new CodeValue<Agent> { Workarea = WADataProvider.WA };
        //    code.Load(id);
        //    code.Delete();

        //    return PartialView("CodesGridPartial", WebDepatmentModel.GetObject(code.ElementId));
        //}
        #endregion

        public ActionResult DepatmentHeadPartial(string modelId)
        {
            bool showFindButton = bool.Parse(Request.Params["showFindButton"]);

            int agentToId = int.Parse(Request.Params["MyCompanyId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MyCompanyId"]);
            //int id = int.Parse(Request.Params["Id"]);
            WebDepatmentModel doc = (WebDepatmentModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MyCompanyId = agentToId;
            PartialViewResult result= PartialView(doc);
            result.ViewData.Add("showFindButton", showFindButton);
            return result;
        }

        public ActionResult DepatmentSubHeadPartial(string modelId)
        {
            bool showFindButton = bool.Parse(Request.Params["showFindButton"]);

            int agentToId = int.Parse(Request.Params["MyCompanyId"] == null || Request.Params["MyCompanyId"] == "null" ? "0" : Request.Params["MyCompanyId"]);
            WebDepatmentModel doc = null;
            if (!WADataProvider.ModelsCache.Exists(modelId))
            {
                int id = int.Parse(Request.Params["Id"]);
                doc = WebDepatmentModel.GetObject(id);
            }
            else
            {
                doc = (WebDepatmentModel)WADataProvider.ModelsCache.Get(modelId);
            }
            doc.MyCompanyId = agentToId;
            PartialViewResult result = PartialView(doc);
            result.ViewData.Add("showFindButton", showFindButton);
            return result;
        }

    }
}
