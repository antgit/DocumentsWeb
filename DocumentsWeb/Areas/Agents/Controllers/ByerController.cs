using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.Mvc;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Agents.Models;

namespace DocumentsWeb.Areas.Agents.Controllers
{
    /// <summary>
    /// Контроллер объектов "Покупатели"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class ByerController : CoreController<Agent>
    {
        public ByerController()
        {
            Name = WebModuleNames.WEB_CLIENTS;
            RootHierachy = Hierarchy.SYSTEM_AGENT_BUYERS;
        }

        public ActionResult Index()
        {
            //LayoutHelper.LoadLayoutFromDatabase(
            //     LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
            //     WADataProvider.WA.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
            //     HttpContext);

            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"] + "Partial";

            List<ClientModel> list = new List<ClientModel>();
            int[] roots = Utils.GetHieRoots(controller, action);
            if (roots.Contains(0))
            {
                list = ClientModel.GetCollection(RootHierachy, false).OfType<ClientModel>().ToList();
                //return View(ClientModel.GetCollection(RootHierachy, false));
                //list = ClientModel.GetCollection(RootHierachy, false);
                //list = ClientModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy).Select(s => s.Code).ToArray<string>());
            }
            else
                list = ClientModel.GetCollectionWONested(roots);

            ViewResult result = View(list);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            string controller = ControllerContext.RouteData.Values["controller"].ToString();
            string action = ControllerContext.RouteData.Values["action"].ToString();
            List<ClientModel> list = new List<ClientModel>();
            int[] roots = Utils.GetHieRoots(controller, action);
            if (roots.Contains(0))
            {
                list = ClientModel.GetCollection(RootHierachy, false).OfType<ClientModel>().ToList();
                //return PartialView(ClientModel.GetCollection(RootHierachy, false));
                //list = ClientModel.GetCollection(HierarchyModel.GetLinkedHierarchies(RootHierachy).Select(s => s.Code).ToArray<string>());
            }
            else
                list = ClientModel.GetCollectionWONested(roots);

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
            ClientModel model = ClientModel.GetObject(id);
            // опеделение текущих разрешений
            if (model.IsReadOnly ||
                !WADataProvider.LibrariesElementRightView.IsAllow(Right.UIEDIT, Name)
                || WADataProvider.CommonRightView.ReadOnly
                || !WADataProvider.IsCompanyIdAllowEditToCurrentUser(model.MyCompanyId))
                return RedirectToAction("Preview", new { Id = model.Id });

            return RedirectToAction("ControlView", new { Id = model.Id });
        }
        /// <summary>
        /// Просмотр данных
        /// </summary>
        /// <param name="id">Идентификатор корреспондента</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Preview(int id)
        {
            return View("Preview", ClientModel.GetObject(id));
        }
        /// <summary>
        /// Управление данными
        /// </summary>
        /// <param name="id">Идентификатор корреспондента</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ControlView(int id)
        {
            ClientModel model = id == 0 ? new ClientModel { Id = 0, KindId = Agent.KINDID_COMPANY } : ClientModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            ViewResult result = View("ControlView", model);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }
        [HttpPost]
        public ActionResult ControlView([ModelBinder(typeof(DevExpressEditorsBinder))] ClientModel model)
        {
            ClientModel modelCashe = (ClientModel)WADataProvider.ModelsCache.Get(model.ModelId);
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
                if (model.Id != 0 && !ClientModel.CanSave(model.Id))
                {
                    return View("ControlView", model);
                }

                Agent obj = model.ToObject(WADataProvider.WA);
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();

                if (obj.Company != null)
                {
                    obj.Company.Save();
                }

                if (model.Id == 0)
                {
                    //New
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_BUYERS);
                    hierarchy.ContentAdd(obj, true);

                    model.Id = obj.Id;
                    model = ClientModel.GetObject(obj.Id);

                    WADataProvider.CacheClientsModelData.AddToCashe(hierarchy.Id, model);
                }
                else
                {
                    model.Id = obj.Id;
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_BUYERS);

                    model = ClientModel.GetObject(obj.Id);
                    WADataProvider.CacheClientsModelData.AddToCashe(hierarchy.Id, model);
                }

                model.Id = obj.Id;

                return RedirectToAction("ControlView", new { Id = model.Id });
                //ViewResult result = View("ControlView", model);
                //return result;
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult Create()
        {
            ClientModel model = new ClientModel { Id = 0, KindId = Agent.KINDID_COMPANY };
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("Edit", model);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id == 0)
                return Create();   
            ClientModel model = ClientModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("Edit", model);    
        }
        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] ClientModel model)
        {
            ClientModel modelCashe = (ClientModel)WADataProvider.ModelsCache.Get(model.ModelId);
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
                if (model.Id != 0 && !ClientModel.CanSave(model.Id))
                {
                    return View("PopupWindowClose", model);
                }

                Agent obj = model.ToObject(WADataProvider.WA);
                obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
                obj.Save();

                if (obj.Company != null)
                    obj.Company.Save();

                if (model.Id == 0)
                {
                    //New
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_BUYERS);
                    hierarchy.ContentAdd(obj, true);

                    model.Id = obj.Id;
                    model = ClientModel.GetObject(obj.Id);

                    WADataProvider.CacheClientsModelData.AddToCashe(hierarchy.Id, model);
                }
                else
                {
                    model.Id = obj.Id;
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_BUYERS);

                    model = ClientModel.GetObject(obj.Id);
                    WADataProvider.CacheClientsModelData.AddToCashe(hierarchy.Id, model);
                }

                model.Id = obj.Id;
                return View("PopupWindowClose", model);
                //return View("Details", model);
            }
            return View(model);
        }

        public void Delete(int id)
        {
            ClientModel.ToTrash(id, Hierarchy.SYSTEM_AGENT_BUYERS);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    ClientModel.SetStatetDone(id, Hierarchy.SYSTEM_AGENT_BUYERS);
                    break;
                case State.STATENOTDONE:
                    ClientModel.SetStateNotDone(id, Hierarchy.SYSTEM_AGENT_BUYERS);
                    break;
                case State.STATEDENY:
                    ClientModel.SetStateDeny(id, Hierarchy.SYSTEM_AGENT_BUYERS);
                    break;
            }
        }

        public void CreateCopy(int id)
        {
            ClientModel.CreateCopy(id, Hierarchy.SYSTEM_AGENT_BUYERS);
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

            IEnumerable coll = ClientModel.GetClients();

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

        public ActionResult GetNameById(int id)
        {
            Agent agent = WADataProvider.WA.GetObject<Agent>(id);
            return Content(agent == null ? string.Empty : agent.Name);
        }

        #region AddressGridPartial
        public ActionResult AddressGridPartial(int agId)
        {
            return PartialView("AddressGridPartial", ClientModel.GetObject(agId));
        }

        public ActionResult AddNewAddressPartial([ModelBinder(typeof(DevExpressEditorsBinder))] AgentAddressModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject().Save();
            }
            return PartialView("AddressGridPartial", ClientModel.GetObject(model.OwnId, true));
        }

        public ActionResult UpdateAddressPartial([ModelBinder(typeof(DevExpressEditorsBinder))] AgentAddressModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject().Save();
            }
            return PartialView("AddressGridPartial", ClientModel.GetObject(model.OwnId, true));
        }

        public ActionResult DeleteAddressPartial(int id)
        {
            AgentAddress address = new AgentAddress { Workarea = WADataProvider.WA };
            address.Load(id);
            address.Delete();

            return PartialView("AddressGridPartial", ClientModel.GetObject(address.OwnId, true));
        }

        public ActionResult cmbTerritoryPartial()
        {
            int countryId = int.Parse(Request.Params["CountryId"]);

            PartialViewResult result = PartialView();
            result.ViewData.Add("CountryId", countryId);
            return result;
        }

        public ActionResult cmbRegionPartial()
        {
            if (Request.Params["TerritoryId"] == null || Request.Params["TerritoryId"] == "null")
                return PartialView();
            int territoryId = int.Parse(Request.Params["TerritoryId"]);

            PartialViewResult result = PartialView();
            result.ViewData.Add("TerritoryId", territoryId);
            return result;
        }

        public ActionResult cmbTownPartial()
        {
            int territoryId = 0;
            int regionId = 0;
            try
            {
                territoryId = int.Parse(Request.Params["TerritoryId"]);
            }
            catch (Exception)
            {
                return PartialView();
            }
            try
            {
                regionId = int.Parse(Request.Params["RegionId"]);
            }
            catch (Exception)
            {
                return PartialView();
            }
            PartialViewResult result = PartialView();
            result.ViewData.Add("TerritoryId", territoryId);
            result.ViewData.Add("RegionId", regionId);
            return result;
        }

        public ActionResult cmbStreetPartial()
        {
            int townId = int.Parse(Request.Params["TownId"]);

            PartialViewResult result = PartialView();
            result.ViewData.Add("TownId", townId);
            return result;
        }
        #endregion

        #region AccountsGridPartial
        public ActionResult AccountsGridPartial(int agId)
        {
            return PartialView("AccountsGridPartial", ClientModel.GetObject(agId));
        }

        public ActionResult AddNewAccountPartial([ModelBinder(typeof(DevExpressEditorsBinder))] AgentBankAccountModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject().Save();
            }
            return PartialView("AccountsGridPartial", ClientModel.GetObject(model.AgentId, true));
        }

        public ActionResult UpdateAccountPartial([ModelBinder(typeof(DevExpressEditorsBinder))] AgentBankAccountModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject().Save();
            }
            return PartialView("AccountsGridPartial", ClientModel.GetObject(model.AgentId, true));
        }

        public ActionResult DeleteAccountPartial(int id)
        {
            AgentBankAccount account = new AgentBankAccount { Workarea = WADataProvider.WA };
            account.Load(id);
            account.Delete();

            return PartialView("AccountsGridPartial", ClientModel.GetObject(account.AgentId, true));
        }
        #endregion

        #region CodesGridPartial
        public ActionResult CodesGridPartial(int agId)
        {
            return PartialView("CodesGridPartial", ClientModel.GetObject(agId));
        }

        public ActionResult AddNewCodePartial([ModelBinder(typeof(DevExpressEditorsBinder))] CodeValueModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject<Agent>().Save();
            }
            return PartialView("CodesGridPartial", ClientModel.GetObject(model.ElementId));
        }

        public ActionResult UpdateCodePartial([ModelBinder(typeof(DevExpressEditorsBinder))] CodeValueModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject<Agent>().Save();
            }
            return PartialView("CodesGridPartial", ClientModel.GetObject(model.ElementId));
        }

        //public ActionResult DeleteCodePartial(int id)
        //{
        //    CodeValue<Agent> code = new CodeValue<Agent> { Workarea = WADataProvider.WA };
        //    code.Load(id);
        //    code.Delete();

        //    return PartialView("CodesGridPartial", ClientModel.GetObject(code.ElementId));
        //}
        #endregion

        public void RefreshByers()
        {
            ClientModel.GetCollection(Hierarchy.SYSTEM_AGENT_BUYERS, true);
        }
    }
}
