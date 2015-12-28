using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DevExpress.Web.ASPxUploadControl;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Contracts.Models;
using DocumentsWeb.Areas.Sales.Models;
using DocumentsWeb.Controllers;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Agents.Models;

namespace DocumentsWeb.Areas.Contracts.Controllers
{
    /// <summary>
    /// Документ "Договор"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPCONTRACTS})]
    public class ContractController : DocumentsWeb.Controllers.ContractController
    {
        public ContractController()
        {
            Name = "WEBДОГ";
            FolderCodeFind = Folder.CODE_FIND_CONTRACTS_CONTRACT;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SignaturesPartial(string mId)
        {
            DocumentContractModel model = (DocumentContractModel)WADataProvider.ModelsCache.Get(mId);
            return PartialView(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SignatureAddPartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentSignModel model, string mId)
        {
            Analitic an = WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>(Analitic.SYSTEM_SIGN_EMPTY);
            model.RowId = Guid.NewGuid().ToString();
            DocumentContractModel dm = (DocumentContractModel)WADataProvider.ModelsCache.Get(mId);
            if (ModelState.IsValid)
            {
                model.StateId = State.STATEACTIVE;
                model.Kind = DocumentSign.KINDID_FIRST;
                model.ResolutionId = an.Id;
                dm.Signs.Add(model);
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("SignaturesPartial", dm);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SignatureUpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentSignModel model, string mId)
        {
            DocumentContractModel dm = (DocumentContractModel)WADataProvider.ModelsCache.Get(mId);
            if (ModelState.IsValid)
            {
                for (int i = 0; i < dm.Signs.Count; i++)
                {
                    if (dm.Signs[i].RowId == model.RowId)
                    {
                        model.StateId = State.STATEACTIVE;
                        dm.Signs[i].AgentId = model.AgentId;
                        dm.Signs[i].AgentSubId = model.AgentSubId;
                        dm.Signs[i].DateTo = model.DateTo;
                        dm.Signs[i].MessageNeed = model.MessageNeed;
                        dm.Signs[i].TaskNeed = model.TaskNeed;
                        dm.Signs[i].PriorityId = model.PriorityId;
                        dm.Signs[i].GroupNo = model.GroupNo;
                        //dm.Signs[i] = model;
                        break;
                    }
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("SignaturesPartial", dm);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SignatureDeletePartial(string RowId, string mId)
        {
            DocumentContractModel dm = (DocumentContractModel)WADataProvider.ModelsCache.Get(mId);
            if (ModelState.IsValid)
            {
                for (int i = 0; i < dm.Signs.Count; i++)
                {
                    if (dm.Signs[i].RowId == RowId)
                    {
                        dm.Signs[i].StateId = State.STATEDELETED;
                        break;
                    }
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("SignaturesPartial", dm);
        }

        public ActionResult WorkerPartial()
        {
            PartialViewResult res = PartialView();
            res.ViewData.Add("WorkerId", Request.Params["WorkerId"]);
            res.ViewData.Add("GridName", Request.Params["GridName"]);
            res.ViewData.Add("ReadOnly", Request.Params["ReadOnly"]);
            return res;
        }

        public ActionResult FillResponsibles(string mId)
        {
            string modelId = Guid.NewGuid().ToString();
            DocumentContractModel dm = (DocumentContractModel)WADataProvider.ModelsCache.Get(mId);
            List<DocumentSignModel> signs = dm.Signs.ToList();
            WADataProvider.ModelsCache.Add(modelId, signs);
            ViewResult res = View();
            res.ViewData.Add("ModelId", modelId);
            res.ViewData.Add("DocModelId", mId);
            return res; 
        }

        public ActionResult SelectedAgentsPartial()
        {
            PartialViewResult res = PartialView();
            res.ViewData.Add("modelId", Request.Params["modelId"]);
            return res;
        }

        public void AddAgentToList(string modelId, int agentId, string dateTo)
        {
            int max = 0;
            List<DocumentSignModel> signs = (List<DocumentSignModel>)WADataProvider.ModelsCache.Get(modelId);
            foreach (DocumentSignModel ss in signs)
            {
                if (ss.StateId != State.STATEDELETED && ss.OrderNo > max)
                {
                    max = ss.OrderNo;
                }
            }
            DateTime dt = DateTime.MinValue;
            if (dateTo != null) dt = DateTime.Parse(dateTo);
            DocumentSignModel sign = new DocumentSignModel { OrderNo = max + 1, AgentId = agentId, MessageNeed = true, TaskNeed = true, DateTo = dt };
            signs.Add(sign);
        }

        public void DeleteAgentsFromList(string modelId, string agents)
        {
            List<DocumentSignModel> signs = (List<DocumentSignModel>)WADataProvider.ModelsCache.Get(modelId);
            if (agents != null)
            {
                List<string> list = agents.Split(',').ToList<string>();
                if (list != null)
                {
                    for (int i = signs.Count - 1; i >= 0; i--)
                    {
                        if (list.Contains(signs[i].RowId))
                        {
                            signs[i].StateId = State.STATEDELETED;
                        }
                    }
                }
            }
        }

        public void UpdateAgentInList(string modelId, int rowNo, int orderNo, bool needMessage = true, bool needTask = true, string dateTo = null, int? agentId = null, int? agentSubId = null)
        {
            List<DocumentSignModel> signs = (List<DocumentSignModel>)WADataProvider.ModelsCache.Get(modelId);
            if (rowNo >= 0 && rowNo <= signs.Count - 1)
            {
                signs[rowNo].OrderNo = orderNo;
                signs[rowNo].AgentId = agentId == null ? 0 : (int)agentId;
                signs[rowNo].AgentSubId = agentSubId == null ? 0 : (int)agentSubId;
                signs[rowNo].DateTo = dateTo == null ? DateTime.MinValue : DateTime.Parse(dateTo);
                signs[rowNo].MessageNeed = needMessage;
                signs[rowNo].TaskNeed = needTask;
            }
        }

        [HttpPost]
        public ActionResult Fill(string docModelId, string modelId)
        {
            if (ModelState.IsValid)
            {
                List<DocumentSignModel> signs = (List<DocumentSignModel>)WADataProvider.ModelsCache.Get(modelId);
                DocumentContractModel dm = (DocumentContractModel)WADataProvider.ModelsCache.Get(docModelId);
                dm.Signs = signs.ToList();
                return View("PopupWindowClose", new DocumentModel { ModelId = docModelId });
            }
            return View("FillResponsibles");
        }

        public ActionResult FillAgentsPartial()
        {
            return PartialView();
        }

        public ActionResult GetContractInfo(int? agentId, DateTime? date)
        {
            if(!agentId.HasValue || !date.HasValue)
                return Json(new { date = string.Empty, number = string.Empty });

            List<Document> coll = WADataProvider.WA.Empty<Document>().FindBy(kindId: DocumentContract.KINDID_DOGOVOR, agentToId: agentId.Value, useAndFilter: true, filter: s=>!s.IsTemplate);

            DocumentContract contract = new DocumentContract {Workarea = WADataProvider.WA};
            bool found = false;

            foreach(Document doc in coll)
            {
                contract.Load(doc.Id);
                if (date >= contract.DateStart && date <= contract.DateEnd)
                { 
                    found = true;
                    break;
                }
            }
           
            if(!found)
            {
                return Json(new { date = string.Empty, number = string.Empty });
            }

            Analitic contractKind = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(contract.ContractKindId);
            return Json(new { date = contract.Document.Date.ToString("MM/dd/yyyy"), number = contract.Document.Number, deliveryCondition=contractKind.Name });
        }

        public ActionResult ClientsFinderPartial()
        {
            ViewData["Name"] = "ClientsFinderAgentFrom";
            ViewData["ComboboxName"] = GlobalPropertyNames.MainClientDepatmentId;
            ViewData["ComboboxClientAcc"] = "MainClientAccountId";
            ViewData["ComboboxButtonIndex"] = 0;
            ViewData["onlyUsers"] = false;
            return PartialView("ClientsFinderPartial");
        }
    }
}
