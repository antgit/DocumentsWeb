using System;
using System.Web.Mvc;
using BusinessObjects;
using DevExpress.Web.Mvc;
using DocumentsWeb.Models;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.General.Models;

namespace DocumentsWeb.Areas.General.Controllers
{
    /// <summary>
    /// Графики работы и перерыва
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class TimePeriodController : CoreController<TimePeriod>
    {
        public TimePeriodController()
        {
            Name = WebModuleNames.WEB_TIMEPERIODS;
            RootHierachy = Hierarchy.GetSystemRootCodeValue(WhellKnownDbEntity.TimePeriod);
        }

        public ActionResult Index()
        {
            ViewResult result = View(TimePeriodModel.GetCollection());
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial()
        {
            return PartialView(TimePeriodModel.GetCollection());
        }

        public ActionResult Edit(int id)
        {
            TimePeriodModel value = id == 0 ? new TimePeriodModel { Id = 0, KindId = TimePeriod.KINDID_WORK, canChangeType = true } : TimePeriodModel.GetObject(id);
            return View("Edit", value);
        }

        public ActionResult OpenTyped(int id, int type)
        {
            TimePeriodModel value = id == 0 ? new TimePeriodModel { Id = 0, KindId = (type == 0 ? TimePeriod.KINDID_WORK : TimePeriod.KINDID_BREAK), canChangeType = false } : TimePeriodModel.GetObject(id);
            return View("Edit", value);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] TimePeriodModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id != 0 && !TimePeriodModel.CanSave(model.Id))
                {
                    return View("PopupWindowClose", model);
                }

                TimePeriod obj = model.ToObject(WADataProvider.WA);
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();
                model.Id = obj.Id;
                return View("PopupWindowClose", model);
            }
            return View(model);
        }

        public void Delete(int id)
        {
            TimePeriodModel.ToTrash(id);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    TimePeriodModel.SetStatetDone(id);
                    break;
                case State.STATENOTDONE:
                    TimePeriodModel.SetStateNotDone(id);
                    break;
                case State.STATEDENY:
                    TimePeriodModel.SetStateDeny(id);
                    break;
            }
        }

        public void CreateCopy(int id)
        {
            TimePeriodModel.CreateCopy(id);
        }

        /// <summary>
        /// Экспорт таблицы в файл
        /// </summary>
        /// <param name="type">Тип файла (xls, pdf)</param>
        /// <returns></returns>
        public ActionResult ExportTo(string type)
        {
            GridViewSettings settings = new GridViewSettings { Name = "Графики работ" };
            settings.Columns.Add("Name", "Имя");
            settings.Columns.Add("Code", "Код");

            switch (type)
            {
                case "XLSX":
                    return GridViewExtension.ExportToXlsx(settings, TimePeriodModel.GetCollection());
                case "PDF":
                    return GridViewExtension.ExportToPdf(settings, TimePeriodModel.GetCollection());
                default:
                    throw new ArgumentException("Неизвестный тип данных для экспорта");
            }
        }
    }
}
