using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Areas.Routes.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DevExpress.Web.Mvc;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Routes.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class DeviceController : CoreController<Device>
    {
        /// <summary>
        /// Корневая иерархия
        /// </summary>
        private const string RootHie = "SYSTEM_DEVICE_ROOT";

        /// <summary>
        /// Тип 
        /// </summary>
        private const int KindId = Device.KINDID_DEVICE;

        public DeviceController()
        {
            Name = WebModuleNames.WEB_DEVICES;
            RootHierachy = RootHie;
        }

        public ActionResult Index()
        {
            LayoutHelper.LoadLayoutFromDatabase(
                   LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
                   WADataProvider.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
                   HttpContext);

            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"].ToString() + "Partial";

            List<DeviceModel> list = new List<DeviceModel>();
            int[] roots = Utils.GetHieRoots(controller, action);
            if (roots.Contains(0))
            {
                list = DeviceModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy, HierarchyModel.FILTER_HIERARCHY_CHAIN).Select(s => s.Code).ToArray<string>());
            }
            else
                list = DeviceModel.GetCollectionWONested(roots);

            return View(list);
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"].ToString();
            List<DeviceModel> list = new List<DeviceModel>();
            int[] roots = Utils.GetHieRoots(controller, action);
            if (roots.Contains(0))
            {
                list = DeviceModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy, HierarchyModel.FILTER_HIERARCHY_CHAIN).Select(s => s.Code).ToArray<string>());
            }
            else
                list = DeviceModel.GetCollectionWONested(roots);

            return PartialView(list);
        }
        public ActionResult Edit(int id)
        {
            DeviceModel model = id == 0 ? new DeviceModel { Id = 0, KindId = KindId } : DeviceModel.GetObject(id);
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] DeviceModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !DeviceModel.CanSave(model.Id))
                {
                    return View("PopupWindowClose", model);
                }

                Device obj = model.ToObject();
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();

                if (model.Id == 0)
                {
                    Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(RootHie);
                    h.ContentAdd(obj, true);
                }
                model.Id = obj.Id;
                return View("PopupWindowClose", model);
            }
            return View("Edit", model);
        }

        /// <summary>
        /// Удалить аналитику
        /// </summary>
        public void Delete(int id)
        {
            DeviceModel.ToTrash(id, RootHie);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    DeviceModel.SetStatetDone(id, RootHie);
                    break;
                case State.STATENOTDONE:
                    DeviceModel.SetStateNotDone(id, RootHie);
                    break;
                case State.STATEDENY:
                    DeviceModel.SetStateDeny(id, RootHie);
                    break;
            }
        }

        public void CreateCopy(int id)
        {
            DeviceModel.CreateCopy(id, RootHie);
        }

        /// <summary>
        /// Экспорт таблицы в файл
        /// </summary>
        /// <param name="type">Тип файла (xls, pdf)</param>
        /// <param name="subtype">Подтип данных</param>
        /// <returns></returns>
        public ActionResult ExportTo(string type, string subtype)
        {
            GridViewSettings settings = new GridViewSettings { Name = "Аналитика" };
            settings.Columns.Add("Name", "Имя");
            settings.Columns.Add("NameFull", "Печатное наименование");
            settings.Columns.Add("Code", "Код");

            List<DeviceModel> coll = DeviceModel.GetCollection(RootHie);

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
    }
}