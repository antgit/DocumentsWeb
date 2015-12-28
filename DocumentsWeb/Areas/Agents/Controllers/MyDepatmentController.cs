using System;
using System.Collections;
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
    /// Контроллер объектов "Наше предприятие"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER })]
    public class MyDepatmentController : CoreController<Agent>
    {
        public MyDepatmentController()
        {
            Name = WebModuleNames.WEB_MYCOMPANY;
            RootHierachy = Hierarchy.SYSTEM_AGENT_MYDEPATMENTS;
        }
        public ActionResult Index()
        {
            //LayoutHelper.LoadLayoutFromDatabase(
            //       LayoutHelper.GetCurrentSettingIndex(Url.RequestContext.RouteData.Values["controller"].ToString()),
            //       WADataProvider.WA.CurrentUser.Id, Url.RequestContext.RouteData.Values["controller"].ToString(),
            //       HttpContext);

            ViewResult result = View("Index", ClientModel.GetMyDepatmentsView());
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult IndexPartial(bool refresh = false)
        {
            return PartialView("IndexPartial", ClientModel.GetMyDepatmentsView(refresh));
        }

        /// <summary>
        /// Просмотр данных
        /// </summary>
        /// <param name="id">Идентификатор корреспондента</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Preview(int id)
        {
            return View(ClientModel.GetObject(id));
        }
        [HttpGet]
        public ActionResult Create()
        {
            ClientModel model = new ClientModel { Id = 0, KindId = Agent.KINDID_MYCOMPANY };
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View("Edit", model);
        }
        [HttpGet]
        public ActionResult Edit(int id, bool isreadonly=false)
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
                // Для корреспондента идентификатор компании всегда равен собственному идентификатору.
                //model.MyCompanyId = modelCashe.MyCompanyId;
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

                if (obj.IsCompany && obj.Company != null)
                {
                    obj.Company.Save();
                    if (obj.IsBank && obj.Company.Bank!=null)
                    {
                        obj.Company.Bank.Save();
                    }
                }

                if (obj.IsPeople && obj.People != null)
                    obj.People.Save();

                if (model.Id == 0)
                {
                    //New
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYDEPATMENTS);
                    hierarchy.ContentAdd(obj, true);

                    model.Id = obj.Id;
                    model = ClientModel.GetObject(obj.Id);

                    WADataProvider.CacheClientsModelData.AddToCashe(hierarchy.Id, model);

                }
                else
                {
                    model.Id = obj.Id;
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYDEPATMENTS);

                    model = ClientModel.GetObject(obj.Id);
                    WADataProvider.CacheClientsModelData.AddToCashe(hierarchy.Id, model);
                }
                
                model.Id = obj.Id;
                return View("PopupWindowClose", model);
            }
            return View(model);
        }

        public void Delete(int id)
        {
            ClientModel.ToTrash(id, Hierarchy.SYSTEM_AGENT_MYDEPATMENTS);
        }

        public void ChangeState(int id, int state)
        {
            switch (state)
            {
                case State.STATEACTIVE:
                    ClientModel.SetStatetDone(id, Hierarchy.SYSTEM_AGENT_MYDEPATMENTS);
                    break;
                case State.STATENOTDONE:
                    ClientModel.SetStateNotDone(id, Hierarchy.SYSTEM_AGENT_MYDEPATMENTS);
                    break;
                case State.STATEDENY:
                    ClientModel.SetStateDeny(id, Hierarchy.SYSTEM_AGENT_MYDEPATMENTS);
                    break;
            }
        }

        public void CreateCopy(int id)
        {
            ClientModel.CreateCopy(id, Hierarchy.SYSTEM_AGENT_MYDEPATMENTS);
        }

        /// <summary>
        /// Экспорт таблицы в файл
        /// </summary>
        /// <param name="type">Тип файла (xls, pdf)</param>
        /// <param name="subtype">Подтип данных</param>
        /// <returns></returns>
        public ActionResult ExportTo(string type, string subtype)
        {
            GridViewSettings settings = new GridViewSettings { Name = "Наши предприятия" };
            //settings.Columns.Add("Id", "Ид");
            settings.Columns.Add("Name", "Имя");
            settings.Columns.Add("NameFull", "Печатное наименование");
            settings.Columns.Add("Code", "Код");

            IEnumerable coll = ClientModel.GetMyDepatments();

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
            return PartialView("AddressGridPartial", ClientModel.GetObject(model.OwnId));
        }

        public ActionResult UpdateAddressPartial([ModelBinder(typeof(DevExpressEditorsBinder))] AgentAddressModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject().Save();
            }
            return PartialView("AddressGridPartial", ClientModel.GetObject(model.OwnId));
        }

        public ActionResult DeleteAddressPartial(int id)
        {
            AgentAddress address = new AgentAddress { Workarea = WADataProvider.WA };
            address.Load(id);
            address.Delete();

            return PartialView("AddressGridPartial", ClientModel.GetObject(address.OwnId));
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
            return PartialView("AccountsGridPartial", ClientModel.GetObject(model.AgentId));
        }

        public ActionResult UpdateAccountPartial([ModelBinder(typeof(DevExpressEditorsBinder))] AgentBankAccountModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject().Save();
            }
            return PartialView("AccountsGridPartial", ClientModel.GetObject(model.AgentId));
        }

        public ActionResult DeleteAccountPartial(int id)
        {
            AgentBankAccount account = new AgentBankAccount { Workarea = WADataProvider.WA };
            account.Load(id);
            account.Delete();

            return PartialView("AccountsGridPartial", ClientModel.GetObject(account.AgentId));
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

        #region MyDepatmentControl
        public ActionResult MyDepatmentControl(int id)
        {
            ClientModel model = ClientModel.GetObject(id);
            model.LoadChains();
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MyDepatmentControlWorkersPartial(string mId)
        {
            ClientModel model = (ClientModel)WADataProvider.ModelsCache.Get(mId);
            return PartialView(model);
        }

        public ActionResult MyDepatmentControlWorkersAddNew(string mId)
        {
            ClientModel m = (ClientModel)WADataProvider.ModelsCache.Get(mId);

            string NewAgentName = Request.Params["NewAgentName"];
            string NewAgentINN = Request.Params["NewAgentINN"];
            
            string NewAgentF = Request.Params["NewAgentF"];
            string NewAgentI = Request.Params["NewAgentI"];
            string NewAgentO = Request.Params["NewAgentO"];

            string NewAgentMO = Request.Params["NewAgentMO_VI"];
            string NewAgentTN = Request.Params["NewAgentTN"];

            if (NewAgentName == null | NewAgentName.Length == 0) ModelState.AddModelError("NewAgentName", "Наименование обязателен для заполнения");
            if (NewAgentF == null | NewAgentF.Length == 0) ModelState.AddModelError("NewAgentF", "Фамилия обязательна для заполнения");
            if (NewAgentI == null | NewAgentI.Length == 0) ModelState.AddModelError("NewAgentI", "Имя обязательно для заполнения");
            if (NewAgentO == null | NewAgentO.Length == 0) ModelState.AddModelError("NewAgentO", "Отчество обязательно для заполнения");

            if (ModelState.IsValid)
            {
                ClientModel ag = new ClientModel();
                ag.Name = NewAgentName;

                ag.FirstName = NewAgentI;
                ag.MidleName = NewAgentO;
                ag.LastName = NewAgentF;

                ag.TaxNumber = NewAgentINN;
                ag.Mol = NewAgentMO == null || NewAgentMO.Length == 0 || NewAgentMO == "0" ? false : true;
                ag.TabNo = NewAgentTN;

                ag.StateId = State.STATEACTIVE;
                ag.Id = 0;
                ag.KindId = Agent.KINDID_PEOPLE;
                m.ChainAgents.Add(new WorkerChainModel { Agent = ag, AgentState = ClientChainState.Worker, StateId = State.STATEACTIVE });
            }

            PartialViewResult res = PartialView("MyDepatmentControlWorkersPartial", m);
            res.ViewData.Add("NewAgentName", NewAgentName);
            res.ViewData.Add("NewAgentINN", NewAgentINN);

            res.ViewData.Add("NewAgentF", NewAgentF);
            res.ViewData.Add("NewAgentI", NewAgentI);
            res.ViewData.Add("NewAgentO", NewAgentO);

            res.ViewData.Add("NewAgentMO", NewAgentMO);
            res.ViewData.Add("NewAgentTN", NewAgentTN);
            return res;
        }

        public void MyDepatmentControlWorkersProcessor(string mId, string agId, string act)
        {
            ClientModel model = (ClientModel)WADataProvider.ModelsCache.Get(mId);
            WorkerChainModel agent = model.ChainAgents.First(w => w.Id == agId && w.StateId != State.STATEDELETED);
            switch (act)
            {
                case "delete":
                    if (agent.AgentState == ClientChainState.Worker)
                    {
                        try
                        {
                            WorkerChainModel trader = model.ChainAgents.First(w => w.Agent.Id == agent.Agent.Id && w.AgentState == ClientChainState.Trader && w.StateId != State.STATEDELETED);
                            trader.StateId = State.STATEDELETED;
                        }
                        catch
                        { }
                        agent.AgentState = ClientChainState.Deleted;
                    }
                    if (agent.AgentState == ClientChainState.Dissmised)
                    {
                        agent.AgentState = ClientChainState.Deleted;
                    }
                    agent.StateId = State.STATEDELETED;
                    //agent.AgentState = ClientChainState.Deleted;
                    /*ClientModel copy_del = (ClientModel)agent.Clone();
                    copy_del.ChainState = ClientChainState.Deleted;*/
                    model.ChainAgents.Add(new WorkerChainModel { Agent = agent.Agent, AgentState = ClientChainState.Worker, StateId = State.STATEDELETED });
                    break;
                case "dismiss":
                    try
                    {
                        WorkerChainModel old = null;
                        if (agent.AgentState == ClientChainState.Trader)
                            old = model.ChainAgents.First(w => w.Agent.Id == agent.Agent.Id && w.AgentState == ClientChainState.Worker && w.StateId != State.STATEDELETED);
                        else
                            old = model.ChainAgents.First(w => w.Agent.Id == agent.Agent.Id && w.AgentState == ClientChainState.Trader && w.StateId != State.STATEDELETED);
                        old.StateId = State.STATEDELETED;
                    }
                    catch
                    { }
                    /*ClientModel copy_d = (ClientModel)agent.Clone();
                    copy_d.ChainState = ClientChainState.Dissmised;
                    model.ChainAgents.Add(copy_d);*/
                    agent.StateId = State.STATEDELETED;
                    model.ChainAgents.Add(new WorkerChainModel { Agent = agent.Agent, AgentState = ClientChainState.Dissmised, StateId = State.STATEACTIVE });
                    break;
                case "make_worker":
                    /*ClientModel copy_w = (ClientModel)agent.Clone();
                    copy_w.ChainState = ClientChainState.Worker;
                    model.ChainAgents.Add(copy_w);*/
                    agent.StateId = State.STATEDELETED;
                    model.ChainAgents.Add(new WorkerChainModel { Agent = agent.Agent, AgentState = ClientChainState.Worker, StateId = State.STATEACTIVE });
                    break;
                case "make_trader":
                    try
                    {
                        model.ChainAgents.First(w => w.Agent.Id == agent.Agent.Id && w.AgentState == ClientChainState.Trader && w.StateId != State.STATEDELETED);
                    }
                    catch
                    {
                        /*ClientModel copy = (ClientModel)agent.Clone();
                        copy.ChainState = ClientChainState.Trader;
                        model.ChainAgents.Add(copy);*/
                        model.ChainAgents.Add(new WorkerChainModel { Agent = agent.Agent, AgentState = ClientChainState.Trader, StateId = State.STATEACTIVE });
                    }
                    break;
            }
        }

        [HttpPost]
        public ActionResult MyDepatmentControlSave([ModelBinder(typeof(DevExpressEditorsBinder))] ClientModel model)
        {
            if (ModelState.IsValid)
            {
                model.ChainAgents = ((ClientModel)WADataProvider.ModelsCache.Get(model.ModelId)).ChainAgents;

                Agent ag = model.ToObject(WADataProvider.WA);
                ag.UserName = WADataProvider.CurrentMembershipUser.UserName;
                ag.Save();
                model.SaveChains();

                //return View("EditingComplete2", model);
            }
            return RedirectToAction("MyDepatmentControl", new { Controller = "MyDepatment", Id = model.Id });
        }
        #endregion
    }
}
