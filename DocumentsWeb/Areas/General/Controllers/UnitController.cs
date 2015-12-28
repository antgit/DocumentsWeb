using System;
using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Areas.General.Models;
using BusinessObjects;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Controllers;
using DocumentsWeb.Code;
using DocumentsWeb.Models;
using System.Collections.Generic;
using System.Linq;

namespace DocumentsWeb.Areas.General.Controllers
{
    /// <summary>
    /// Единицы измерения - список
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class UnitController : CoreController<Unit>
    {
        public UnitController()
        {
            Name = WebModuleNames.WEB_UNIT;
            RootHierachy = Hierarchy.GetSystemRootCodeValue(WhellKnownDbEntity.Unit);
        }

        public ActionResult Index()
        {
            LayoutHelper.LoadLayoutFromDatabase(
                   LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
                   WADataProvider.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
                   HttpContext);

            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"].ToString() + "Partial";

            List<WebUnitModel> list = new List<WebUnitModel>();
            int[] roots = Utils.GetHieRoots(controller, action);
            if (roots.Contains(0))
            {
                list = WebUnitModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy, HierarchyModel.FILTER_HIERARCHY_CHAIN).Select(s => s.Code).ToArray<string>());
            }
            else
                list = WebUnitModel.GetCollectionWONested(roots);

            ViewResult result = View(list);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial()
        {
            //return PartialView(WebUnitModel.GetCollection());
            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"].ToString();
            List<WebUnitModel> list = new List<WebUnitModel>();
            int[] roots = Utils.GetHieRoots(controller, action);
            if (roots.Contains(0))
            {
                list = WebUnitModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy, HierarchyModel.FILTER_HIERARCHY_CHAIN).Select(s => s.Code).ToArray<string>());
            }
            else
                list = WebUnitModel.GetCollectionWONested(roots);

            return PartialView(list);
        }

        /// <summary>
        /// Просмотр данных
        /// </summary>
        /// <param name="id">Идентификатор единицы измерения</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Preview(int id)
        {
            return View(WebUnitModel.GetObject(id));
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            WebUnitModel value = id == 0 ? new WebUnitModel{ Id = 0, Name = "" } : WebUnitModel.GetObject(id);
            return View("Edit", value);
            //return View(value);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] WebUnitModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !WebUnitModel.CanSave(model.Id))
                {
                    return View("PopupWindowClose", model);
                }

                Unit unit = model.ToObject(WADataProvider.WA);
                unit.UserName = WADataProvider.CurrentMembershipUser.UserName;
                unit.Save();
                model.Id = unit.Id;
                return View("PopupWindowClose", model);
            }
            return View(model);
        }

        /// <summary>
        /// Просмотр данных
        /// </summary>
        /// <param name="id">Идентификатор единицы измерения</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ControlView(int id)
        {
            WebUnitModel model = id == 0 ? new WebUnitModel { Id = 0, KindId = Unit.KINDID_UNIT  } : WebUnitModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            ViewResult result = View("ControlView", model);

            return result;
        }
        /// <summary>
        /// Управление данными
        /// </summary>
        /// <param name="model">Модель</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ControlView([ModelBinder(typeof(DevExpressEditorsBinder))] WebUnitModel model)
        {
            WebUnitModel modelCashe = (WebUnitModel)WADataProvider.ModelsCache.Get(model.ModelId);
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
                if (model.Id != 0 && !WebUnitModel.CanSave(model.Id))
                {
                    ViewResult res = View("ControlView", model);
                    return res;
                }

                Unit obj = model.ToObject(WADataProvider.WA);
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();

                if (model.Id == 0)
                {
                    //New
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);
                    hierarchy.ContentAdd(obj, true);

                    model.Id = obj.Id;
                    model = WebUnitModel.GetObject(obj.Id);

                    //WADataProvider.CacheAnaliticsModelData.AddToCashe(hierarchy.Id, model);
                }
                else
                {
                    model.Id = obj.Id;
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHierachy);

                    model = WebUnitModel.GetObject(obj.Id);
                    //WADataProvider.CacheAnaliticsModelData.AddToCashe(hierarchy.Id, model);
                }

                model.Id = obj.Id;

                ViewResult result = View("ControlView", model);
                return result;
            }
            return View(model);
        }

        /// <summary>
        /// Создание новой единицы измерения
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Create()
        {
            WebUnitModel model = new WebUnitModel { Id = 0, KindId = Unit.KINDID_UNIT };
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("Edit", model);
        }
        /// <summary>
        /// Удалить единицу измерения
        /// </summary>
        /// <returns></returns>
        public void Delete(int id)
        {
            WebUnitModel.ToTrash(id);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    WebUnitModel.SetStatetDone(id);
                    break;
                case State.STATENOTDONE:
                    WebUnitModel.SetStateNotDone(id);
                    break;
                case State.STATEDENY:
                    WebUnitModel.SetStateDeny(id);
                    break;
            }
        }

        public void CreateCopy(int id)
        {
            WebUnitModel.CreateCopy(id);
        }

        public ActionResult ExportTo(string type)
        {
            GridViewSettings settings = new GridViewSettings { Name = "Единицы измерения" };
            settings.Columns.Add("Id", "Ид");
            settings.Columns.Add("Name", "Имя");
            settings.Columns.Add("Code", "Код");
            settings.Columns.Add("CodeInternational", "Международн.");

            switch (type)
            {
                case "XLSX":
                    return GridViewExtension.ExportToXlsx(settings, WebUnitModel.GetCollection());
                case "PDF":
                    return GridViewExtension.ExportToPdf(settings, WebUnitModel.GetCollection());
                default:
                    throw new ArgumentException("Неизвестный тип данных для экспорта");
            }
        }
    }
}
