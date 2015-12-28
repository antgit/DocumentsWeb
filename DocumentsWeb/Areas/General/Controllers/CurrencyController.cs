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
    /// Валюты
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class CurrencyController : CoreController<Currency>
    {
        public CurrencyController()
        {
            Name = WebModuleNames.WEB_CURRENCY;
            RootHierachy = Hierarchy.GetSystemRootCodeValue(WhellKnownDbEntity.Currency);
        }

        public ActionResult Index()
        {
            /*ViewResult result = View(WebCurrencyModel.GetCollection());
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;*/
            LayoutHelper.LoadLayoutFromDatabase(
                   LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
                   WADataProvider.WA.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
                   HttpContext);

            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"].ToString() + "Partial";

            List<WebCurrencyModel> list = new List<WebCurrencyModel>();
            int[] roots = Utils.GetHieRoots(controller, action);
            if (roots.Contains(0))
            {
                list = WebCurrencyModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy, HierarchyModel.FILTER_HIERARCHY_CHAIN).Select(s => s.Code).ToArray());
            }
            else
                list = WebCurrencyModel.GetCollectionWONested(roots);

            ViewResult result = View(list);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial()
        {
            //return PartialView("IndexPartial", WebCurrencyModel.GetCollection());
            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"].ToString();

            List<WebCurrencyModel> list = new List<WebCurrencyModel>();
            int[] roots = Utils.GetHieRoots(controller, action);
            if (roots.Contains(0))
            {
                list = WebCurrencyModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy, HierarchyModel.FILTER_HIERARCHY_CHAIN).Select(s => s.Code).ToArray<string>());
            }
            else
                list = WebCurrencyModel.GetCollectionWONested(roots);

            return PartialView(list);
        }
        
        [HttpGet]
        public ActionResult Edit(int id)
        {
            WebCurrencyModel value = id == 0 ? new WebCurrencyModel{ Id = 0, Name = "" } : WebCurrencyModel.GetObject(id);
            return View(value);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] WebCurrencyModel model)
        {
            if (ModelState.IsValid)
            {
                Currency obj = model.ToObject();
                obj.Save();
                model.Id = obj.Id;
                return View("PopupWindowClose", model);
            }
            else
                return View(model);
        }
        
        /// <summary>
        /// Удалить валюту
        /// </summary>
        /// <returns></returns>
        public void Delete(int id)
        {
            WebCurrencyModel.ToTrash(id);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    WebCurrencyModel.SetStatetDone(id);
                    break;
                case State.STATENOTDONE:
                    WebCurrencyModel.SetStateNotDone(id);
                    break;
                case State.STATEDENY:
                    WebCurrencyModel.SetStateDeny(id);
                    break;
            }
        }

        public void CreateCopy(int id)
        {
            WebCurrencyModel.CreateCopy(id);
        }

        /// <summary>
        /// Экспорт таблицы в файл
        /// </summary>
        /// <param name="type">Тип файла (xls, pdf)</param>
        /// <returns></returns>
        public ActionResult ExportTo(string type)
        {
            GridViewSettings settings = new GridViewSettings { Name = "Валюты" };
            settings.Columns.Add("Name", "Имя");
            settings.Columns.Add("Code", "Код");
            settings.Columns.Add("IntCode", "Интернац. код");

            switch (type)
            {
                case "XLSX":
                    return GridViewExtension.ExportToXlsx(settings, WebCurrencyModel.GetCollection());
                case "PDF":
                    return GridViewExtension.ExportToPdf(settings, WebCurrencyModel.GetCollection());
                default:
                    throw new ArgumentException("Неизвестный тип данных для экспорта");
            }
        }
    }
}
