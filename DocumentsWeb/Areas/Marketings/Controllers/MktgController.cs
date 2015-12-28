using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentsWeb.Areas.Marketings.Models;
using DevExpress.Web.Mvc;
using BusinessObjects;
using DocumentsWeb.Areas.Agents.Models;
using BusinessObjects.Security;
using DocumentsWeb.Controllers;
using BusinessObjects.Documents;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Analitics.Models;

namespace DocumentsWeb.Areas.Marketings.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPMKTG, Uid.GROUP_GROUPWEBADMIN })]
    public class MktgController : CoreDocumentControler
    {
        public MktgController()
        {
            Name = "WEBМРАК";
            FolderCodeFind = Folder.CODE_FIND_MKTG_MRAK;
            HelpHasPopup = true;
        }

        public ActionResult Create()
        {
            Folder folder = WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);

            int formId = folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(formId).Code;
            Document template = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(folder.DocumentId);
            DocumentMktg template2 = WADataProvider.WA.Cashe.GetCasheData<DocumentMktg>().Item(folder.DocumentId);


            DocumentMktgModel documentModel = new DocumentMktgModel { FormCode = formCode };
            documentModel.LoadFromTemplate(template);
            DocumentMktgModel.ImportAnalitics(template2.Analitics, documentModel);
            foreach (DocumentAnaliticModel a in documentModel.Analitics) { a.Id = 0; }
            WADataProvider.ModelsCache.Add(documentModel.ModelId, documentModel);
            return EditModel(documentModel.ModelId);
        }

        public ActionResult AgentFromPartial()
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            int id = int.Parse(Request.Params["Id"]);
            string mId = Request.Params["ModelId"];

            if (!ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, mainCompanyDepatmentId);
            else
                ClientModel.currentMyCompanies[HttpContext.Session.SessionID] = mainCompanyDepatmentId;
            //ClientModel.currentMyCompanies.Add(WADataProvider.WA.CurrentUser.Id, agentToId);            
            PartialViewResult result = PartialView(WADataProvider.ModelsCache.Get(mId));
            result.ViewData.Add("MainCompanyDepatmentId", mainCompanyDepatmentId);
            return result;
        }

        public ActionResult Edit(int Id, int LastTM = 0, int LastGT = 0)
        {
            Models.DocumentMktgModel model = Models.DocumentMktgModel.GetObject(Id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            model.LastTM = LastTM;
            model.LastGT = LastGT;
            ViewResult result = View(model);
            result.ViewData.Add("PrintForms", PrintForms());
            result.ViewData.Add("HelpHasPopup", HelpHasPopup);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        public ActionResult EditModel(string modelId)
        {
            Models.DocumentMktgModel documentModel = (Models.DocumentMktgModel)WADataProvider.ModelsCache.Get(modelId);
            if (!ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, documentModel.MainCompanyId);
            else
                ClientModel.currentMyCompanies[HttpContext.Session.SessionID] = documentModel.MainCompanyId;
            ViewResult result = View("Edit", documentModel);
            
            return result;
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] Models.DocumentMktgModel model)
        {
            Models.DocumentMktgModel m = (Models.DocumentMktgModel)WADataProvider.ModelsCache.Get(model.ModelId);
            model.Analitics = m.Analitics;
            if (ModelState.IsValid)
            {
                model.Save();
                return RedirectToAction("Edit", new { Id = model.Id, LastTM = m.LastTM, LastGT = m.LastGT });
            }
            ViewResult result = View(model);
            result.ViewData.Add("PrintForms", PrintForms());
            result.ViewData.Add("HelpHasPopup", HelpHasPopup);
            result.ViewData.Add("HelpDefaultLink", HelpDefaultLink);
            return result;
        }

        // Аналитики по продукции в холодильниках
        #region Refrigerators
        [HttpPost, ValidateInput(false)]
        public ActionResult RefrigeratorsPartial(string mId)
        {
            Models.DocumentMktgModel model = (Models.DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            return PartialView(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RefrigeratorsAddPartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentAnaliticModel model, string mId)
        {
            model.RowId = Guid.NewGuid().ToString();
            Models.DocumentMktgModel dm = (Models.DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);

            dm.LastTM = model.AnaliticId ?? 0;
            dm.LastGT = model.GroupId ?? 0;

            if (ModelState.IsValid)
            {
                model.StateId = State.STATEACTIVE;
                dm.Analitics.Add(model);

                if (model.GroupId == 9 || model.GroupId == 8)
                {
                    model.SummValue3 = CalcSummSKU(mId, model.GroupId == null ? 0 : (int)model.GroupId, model.AnaliticId == null ? 0 : (int)model.AnaliticId);
                }
                foreach (DocumentAnaliticModel m in dm.Analitics.Where(w => w.StateId != State.STATEDELETED && ((w.GroupId >= 4 && w.GroupId <= 9) || w.GroupId == 14 || w.GroupId == 15)))
                {
                    if (m.GroupId == model.GroupId && m.AnaliticId == model.AnaliticId)
                    {
                        m.SummValue3 = model.SummValue3;
                        m.StringValue1 = model.StringValue1;
                    }
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("RefrigeratorsPartial", dm);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RefrigeratorsUpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentAnaliticModel model, string mId)
        {
            DocumentMktgModel dm = (DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);

            dm.LastTM = model.AnaliticId ?? 0;
            dm.LastGT = model.GroupId ?? 0;

            if (ModelState.IsValid)
            {
                for (int i = 0; i < dm.Analitics.Count; i++)
                {
                    if (dm.Analitics[i].RowId == model.RowId)
                    {
                        model.StateId = State.STATEACTIVE;
                        dm.Analitics[i] = model;
                        if (model.GroupId == 9 || model.GroupId == 8)
                        {
                            model.SummValue3 = CalcSummSKU(mId, model.GroupId == null ? 0 : (int)model.GroupId, model.AnaliticId == null ? 0 : (int)model.AnaliticId);
                        }
                        foreach (DocumentAnaliticModel m in dm.Analitics.Where(w => w.StateId != State.STATEDELETED && ((w.GroupId >= 4 && w.GroupId <= 9) || w.GroupId == 14 || w.GroupId == 15)))
                        {
                            if (m.GroupId == model.GroupId && m.AnaliticId == model.AnaliticId)
                            {
                                m.SummValue3 = model.SummValue3;
                                m.StringValue1 = model.StringValue1;
                            }
                        }
                        break;
                    }
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("RefrigeratorsPartial", dm);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RefrigeratorsDeletePartial(string RowId, string mId)
        {
            DocumentMktgModel dm = (DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            if (ModelState.IsValid)
            {
                for (int i = 0; i < dm.Analitics.Count; i++)
                {
                    if (dm.Analitics[i].RowId == RowId)
                    {
                        dm.Analitics[i].StateId = State.STATEDELETED;
                        if (dm.Analitics[i].GroupId == 9 || dm.Analitics[i].GroupId == 8)
                        {
                            decimal summ_sku = CalcSummSKU(mId, dm.Analitics[i].GroupId == null ? 0 : (int)dm.Analitics[i].GroupId, dm.Analitics[i].AnaliticId == null ? 0 : (int)dm.Analitics[i].AnaliticId);
                            foreach (DocumentAnaliticModel m in dm.Analitics.Where(w => w.StateId != State.STATEDELETED && ((w.GroupId >= 4 && w.GroupId <= 9) || w.GroupId == 14 || w.GroupId == 15)))
                            {
                                if (m.GroupId == dm.Analitics[i].GroupId && m.AnaliticId == dm.Analitics[i].AnaliticId)
                                {
                                    m.SummValue3 = summ_sku;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("MktgRefrigeratorsPartial", dm);
        }

        public ActionResult RefrigeratorsTMPartial()
        {
            PartialViewResult res = PartialView();
            res.ViewData.Add("AnaliticId", Request.Params["AnaliticId"]);
            return res;
        }

        public ActionResult EditGroupTMPartial()
        {
            PartialViewResult res = PartialView();
            res.ViewData.Add("AnaliticId", Request.Params["AnaliticId"]);
            return res;
        }

        public ActionResult EditGroupPartial()
        {
            return PartialView();
        }

        public ActionResult EditGroupListPartial()
        {
            PartialViewResult res = PartialView();
            res.ViewData.Add("subGroup", Request.Params["subGroup"]);
            res.ViewData.Add("GId", Request.Params["GId"]);
            res.ViewData.Add("TM", Request.Params["TM"]);
            res.ViewData.Add("mId", Request.Params["mId"]);
            return res;
        }

        [HttpPost]
        public ActionResult SaveGroupPartial()
        {
            string mId = (string)Request.Params["mId"];
            int GroupId = int.Parse((string)Request.Params["GId"]);
            int TM = int.Parse((string)Request.Params["TM"]);
            int count = int.Parse((string)Request.Params["groupCount"]);
            DocumentMktgModel dm = (DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);

            for (int i = 0; i < count; i++)
            {
                int an_id = int.Parse((string)Request.Params["Analitic2Id_" + i.ToString()]);
                decimal summ = 0;
                try
                {
                    summ = int.Parse((string)Request.Params["SummValue2_" + i.ToString()]);
                }
                catch { }

                bool is_find = false;
                for (int j = 0; j < dm.Analitics.Count; j++)
                {
                    if (dm.Analitics[j].GroupId == GroupId && dm.Analitics[j].AnaliticId == TM && dm.Analitics[j].Analitic2Id == an_id)
                    {
                        if (summ > 0)
                        {
                            dm.Analitics[j].SummValue2 = summ;
                            dm.Analitics[j].StateId = State.STATEACTIVE;
                        }
                        else
                        {
                            dm.Analitics[j].StateId = State.STATEDELETED;
                        }
                        is_find = true;
                        break;
                    }
                }
                if (!is_find)
                {
                    if (summ > 0)
                    {
                        DocumentAnaliticModel m = new DocumentAnaliticModel
                        {
                            GroupId = GroupId,
                            AnaliticId = TM,
                            Analitic2Id = an_id,
                            SummValue2 = summ,
                            Group2Id = 0,
                            SummValue4 = 0
                        };
                        dm.Analitics.Add(m);
                    }
                }
            }

            decimal summ_sku = CalcSummSKU(mId, GroupId, TM);
            foreach (DocumentAnaliticModel m in dm.Analitics.Where(w => w.StateId != State.STATEDELETED && ((w.GroupId >= 4 && w.GroupId <= 9) || w.GroupId == 14 || w.GroupId == 15)))
            {
                if (m.GroupId == GroupId && m.AnaliticId == TM)
                {
                    m.SummValue3 = summ_sku;
                }
            }

            return PartialView("PopupWindowClose", dm);
        }

        public decimal CalcSummSKU(string mId, int GpId, int AnId)
        {
            DocumentMktgModel dm = (DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            decimal summ_sku = 0;
            foreach (DocumentAnaliticModel m in dm.Analitics.Where(w => w.StateId != State.STATEDELETED && ((w.GroupId >= 4 && w.GroupId <= 9) || w.GroupId == 14 || w.GroupId == 15)))
            {
                if (m.GroupId == GpId && m.AnaliticId == AnId)
                {
                    summ_sku += (m.SummValue2 == null ? 0 : (decimal)m.SummValue2);
                }
            }
            return summ_sku;
        }

        public decimal GetSKU(string mId, int GpId, int AnId)
        {
            DocumentMktgModel dm = (DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            decimal sku = 0;
            foreach (DocumentAnaliticModel m in dm.Analitics.Where(w => w.StateId != State.STATEDELETED && ((w.GroupId >= 4 && w.GroupId <= 9) || w.GroupId == 14 || w.GroupId == 15)))
            {
                if (m.GroupId == GpId && m.AnaliticId == AnId)
                {
                    sku = m.SummValue3 == null ? 0 : (decimal)m.SummValue3;
                    break;
                }
            }
            return sku;
        }

        public string GetPFGroups(string mId, int GpId, int AnId)
        {
            DocumentMktgModel dm = (DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            string gps = "";
            foreach (DocumentAnaliticModel m in dm.Analitics.Where(w => w.StateId != State.STATEDELETED && ((w.GroupId >= 4 && w.GroupId <= 9) || w.GroupId == 14 || w.GroupId == 15)))
            {
                if (m.GroupId == GpId && m.AnaliticId == AnId)
                {
                    gps = m.StringValue1;
                    break;
                }
            }
            return gps;
        }

        #endregion

        // Аналитики по холодильникам
        #region RefrigeratorsTypes
        [HttpPost, ValidateInput(false)]
        public ActionResult RefrigeratorsTypesPartial(string mId)
        {
            DocumentMktgModel model = (DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            return PartialView(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RefrigeratorsTypesAddPartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentAnaliticModel model, string mId)
        {
            model.RowId = Guid.NewGuid().ToString();
            DocumentMktgModel dm = (DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            if (ModelState.IsValid)
            {
                model.StateId = State.STATEACTIVE;
                dm.Analitics.Add(model);
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("RefrigeratorsTypesPartial", dm);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RefrigeratorsTypesUpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentAnaliticModel model, string mId)
        {
            DocumentMktgModel dm = (DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            if (ModelState.IsValid)
            {
                for (int i = 0; i < dm.Analitics.Count; i++)
                {
                    if (dm.Analitics[i].RowId == model.RowId)
                    {
                        model.StateId = State.STATEACTIVE;
                        dm.Analitics[i] = model;
                        break;
                    }
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("RefrigeratorsTypesPartial", dm);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RefrigeratorsTypesDeletePartial(string RowId, string mId)
        {
            DocumentMktgModel dm = (DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            if (ModelState.IsValid)
            {
                for (int i = 0; i < dm.Analitics.Count; i++)
                {
                    if (dm.Analitics[i].RowId == RowId)
                    {
                        dm.Analitics[i].StateId = State.STATEDELETED;
                        break;
                    }
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("RefrigeratorsTypesPartial", dm);
        }

        public ActionResult RefrigeratorsTypesTMPartial()
        {
            PartialViewResult res = PartialView();
            res.ViewData.Add("AnaliticId", Request.Params["AnaliticId"]);
            return res;
        }

        #endregion

        public ActionResult ManagerPartial(string mId)
        {
            int MainClientDepatmentId = int.Parse(Request.Params["MainClientDepatmentId"]);
            DocumentMktgModel doc = null;
            if (!WADataProvider.ModelsCache.Exists(mId))
            {
                int id = int.Parse(Request.Params["Id"]);
                doc = DocumentMktgModel.GetObject(id);
            }
            else
            {
                doc = (DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            }
            doc.MainClientDepatmentId = MainClientDepatmentId;
            doc.SupervisorId = 0;
            doc.ManagerId = 0;
            return PartialView(doc);
        }

        public ActionResult MainCompanyPartial()
        {
            int MainClientDepatmentId;
            try
            {
                MainClientDepatmentId = int.Parse(Request.Params["MainClientDepatmentId"]);
            }
            catch
            {
                MainClientDepatmentId = 0;
            }
            int id = int.Parse(Request.Params["Id"]);
            string mId = Request.Params["mId"];

            if (!ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, MainClientDepatmentId);
            else
                ClientModel.currentMyCompanies[HttpContext.Session.SessionID] = MainClientDepatmentId;
            //ClientModel.currentMyCompanies.Add(WADataProvider.WA.CurrentUser.Id, agentToId);
            PartialViewResult result = PartialView(WADataProvider.ModelsCache.Get(mId));
            result.ViewData.Add("MainClientDepatmentId", MainClientDepatmentId);
            return result;
        }

        public ActionResult TrackBarPartial()
        {
            string mId = Request.Params["mId"];
            PartialViewResult result = PartialView(WADataProvider.ModelsCache.Get(mId));
            result.ViewData.Add("AnId", Request.Params["AnId"]);
            result.ViewData.Add("Value", Request.Params["Value"]);
            result.ViewData.Add("Name", Request.Params["Name"]);
            return result;
        }

        public ActionResult ProductSubType()
        {
            PartialViewResult result = PartialView("ProductSubTypePartial");
            result.ViewData.Add("Type", Request.Params["Type"]);
            result.ViewData.Add("Analitic2Id", Request.Params["Analitic2Id"]);
            return result;
        }

        /// <summary>
        /// Последняя введенная группа товаров
        /// </summary>
        /// <returns></returns>
        
        public int GetLastInputGT(string mId)
        {
            Models.DocumentMktgModel dm = (Models.DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            return dm.LastGT;
        }

        public void CreateCopy(int id)
        {
            DocumentMktgModel.CreateCopy(id);
        }

        public void ChangeState(int id, int state)
        {
            DocumentModel.ChangeState(id, state);
        }

        #region Управление клиентами
        public ActionResult LightClient(int id, int MyCompanyId)
        {
            ClientModel model = id == 0 ? new ClientModel
            {
                Id = 0,
                KindId = Agent.KINDID_COMPANY,
                MyCompanyId = MyCompanyId,
            } : ClientModel.GetObject(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult LightClient([ModelBinder(typeof(DevExpressEditorsBinder))] ClientModel model)
        {
            if (ModelState.IsValid)
            {
                Agent obj = model.ToObject(WADataProvider.WA);
                obj.KindId = Agent.KINDID_COMPANY;
                obj.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
                obj.Save();

                if (obj.Company != null)
                    obj.Company.Save();

                if (model.Id == 0)
                {
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("SYSTEM_AGENT_POTENTIALCUSTOMERS");
                    hierarchy.ContentAdd(obj, true);
                    //model = ClientModel.ConvertToModel(obj);
                }

                model.Id = obj.Id;
                return View("PopupWindowClose", model);
            }
            return View(model);
        }
        #endregion

        #region Адрес клиента
        public ActionResult LightAddress(int id)
        {
            AgentAddressModel addr = AgentAddressModel.GetMktgAddressByAgentId(id);
            AgentAddressModel model = (addr == null ? new AgentAddressModel { OwnId = id, AddressKindId = AgentAddress.KINDID_ACTUALADDRESS } : addr);
            return View(model);
        }

        [HttpPost]
        public ActionResult LightAddress([ModelBinder(typeof(DevExpressEditorsBinder))] AgentAddressModel model)
        {
            if (ModelState.IsValid)
            {
                model.ToObject().Save();
                return View("PopupWindowClose", new ClientModel { Name = model.GetActualName(), Id = 0 });
            }
            return View(model);
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

        /// <summary>
        /// Возвращает адрес клиента
        /// </summary>
        /// <param name="id">Идентификатор клиента</param>
        public object GetClientAddress(int id)
        {
            string address = "";
            AgentAddressModel model = AgentAddressModel.GetMktgAddressByAgentId(id);
            if (model != null)
            {
                address = model.GetActualName();
            }
            return address;
        }
        #endregion

        #region Управление торговыми марками
        public ActionResult LightTM(int id)
        {
            AnaliticModel value = id == 0 ? new AnaliticModel { KindId = Analitic.KINDID_BRAND } : AnaliticModel.GetObject(id);
            return View(value);
        }

        [HttpPost]
        public ActionResult LightTM([ModelBinder(typeof(DevExpressEditorsBinder))] AnaliticModel model)
        {
            if (ModelState.IsValid)
            {
                Analitic obj = model.ToObject();
                obj.Save();

                if (model.Id == 0)
                {
                    Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_ANALITIC_BRANDS);
                    h.ContentAdd(obj, true);
                }
                model.Id = obj.Id;
                ClientModel cm = new ClientModel { Id = obj.Id, Name = model.Name, KindId = model.KindId };
                return View("PopupWindowClose", cm);
            }
            return View(model);
        }

        /// <summary>
        /// Последняя введенная торговая марка
        /// </summary>
        /// <returns></returns>
        public int GetLastInputTM(string mId)
        {
            Models.DocumentMktgModel dm = (Models.DocumentMktgModel)WADataProvider.ModelsCache.Get(mId);
            return dm.LastTM;
        }

        /// <summary>
        /// Обновляет список торговых марок в кеше
        /// </summary>
        public void UpdateTMs()
        {
            AnaliticModel.GetCollection(Hierarchy.SYSTEM_ANALITIC_BRANDS, true);
        }
        #endregion

        #region Графики работы

        public ActionResult cmbTimePeriodPartial()
        {
            return PartialView();
        }

        public ActionResult cmbBreakPeriodPartial()
        {
            return PartialView();
        }

        #endregion
    }
}
