using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects.Security;
using DocumentsWeb.Areas.Contracts.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;
using BusinessObjects;
using DevExpress.Web.Mvc;
using BusinessObjects.Documents;
using DocumentsWeb.Areas.Products.Models;

namespace DocumentsWeb.Areas.Contracts.Controllers
{
    /// <summary>
    /// Документ "Учет компьютеров"
    /// </summary>
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPCONTRACTS})]
    public class AccountingComputersController : DocumentsWeb.Controllers.ContractController
    {
        public AccountingComputersController()
        {
            Name = "WEBДКОПМ";
            FolderCodeFind = Folder.CODE_FIND_CONTRACTS_COMPUTER;
            PrintModelCode = "CONTRACTORGTEX";
        }

        //
        // GET: /Contracts/AccountingComputers/

        public ActionResult Index()
        {
            return View();
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

        public ActionResult ProductPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        [HttpPost]
        public override ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentContractModel model)
        {
            DocumentContractModel m = (DocumentContractModel)WADataProvider.ModelsCache.Get(model.ModelId);
            //Копирование полей документа, не сохраняющихся на клиенте
            model.Id = m.Id;
            model.TemplateId = m.TemplateId;
            model.Files = m.Files;
            model.Details = m.Details;
            model.FolderId = m.FolderId;
            model.ProjectItemId = m.ProjectItemId;
            model.FormCode = m.FormCode;
            model.KindId = m.KindId;
            model.Notes = m.Notes;

            if (ModelState.IsValid)
            {
                DocumentContract doc = model.ToObject(WADataProvider.WA);
                doc.UserName = WADataProvider.CurrentMembershipUser.UserName;

                foreach (DocumentDetailContractModel dm in model.Details)
                {
                    if (dm.StateId != State.STATEDELETED && dm.ProductId > 0)
                    {
                        Product p = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(dm.ProductId);
                        if (p != null)
                        {
                            p.Memo = dm.Config;
                            p.Save();
                        }
                    }
                }

                doc.Details = model.Details.Select(s => s.ToObject(doc)).ToList();
                doc.Document.AgentDepartmentToId = doc.Document.AgentDepartmentFromId;
                doc.Document.AgentTo = doc.Document.AgentDepartmentFrom.GetAgentHolding();
                doc.Document.AgentFrom = doc.Document.AgentTo;
                doc.Document.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
                doc.Save();

                model.Details = doc.Details.Select(s => DocumentDetailContractModel.ConvertToModel(s)).ToList();

                foreach (DocumentSignModel sm in m.Signs)
                {
                    sm.OwnerId = doc.Id;
                    DocumentSign sign = sm.ToObject();
                    sign.Id = sm.Id;
                    sign.Save();
                    sm.Id = sign.Id;
                }

                model.Id = doc.Id;
                model.SaveFiles();
                model.SaveNotes<Document>(doc.Document.GetLinkedNotes());

                m.Details = model.Details;
                m.Files = model.Files;
            }
            ViewResult result = View("Edit", model);
            result.ViewData.Add("PrintForms", PrintForms());
            return result;
        }

        public ActionResult exRegistratorIdPartial()
        {
            int agentToId = int.Parse(Request.Params["AgentDepartmentFromId"] == null || Request.Params["AgentDepartmentFromId"] == "null" ? "0" : Request.Params["AgentDepartmentFromId"]);
            int id = int.Parse(Request.Params["Id"]);

            DocumentContractModel documentContractModel = new DocumentContractModel();
            PartialViewResult result = PartialView("RegistratorIdPartial", documentContractModel);
            result.ViewData.Add("AgentDepartmentFromId", agentToId);
            return result;
        }

        #region Inline Editing
        public ActionResult DetailsGridPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridAddNewPartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailContractModel model, string ownewrModelId)
        {
            DocumentContractModel documentContractModel = (DocumentContractModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            if (ModelState.IsValid)
            {
                model.RowId = Guid.NewGuid().ToString();
                model.ProductName = ProductModel.GetObject(model.ProductId).Name;
                model.StateId = State.STATEACTIVE;
                documentContractModel.Details.Add(model);
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
                if (model.Qty == 0) model.Qty = 1900;
                ViewData["EditableProduct"] = model;
            }

            return PartialView("DetailsGridPartial", WADataProvider.ModelsCache.Get(ownewrModelId));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridUpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailContractModel model, string ownewrModelId)
        {
            DocumentContractModel documentContractModel = (DocumentContractModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            if (ModelState.IsValid)
            {
                DocumentDetailContractModel documentDetailContractModel = documentContractModel.Details.FirstOrDefault(s => s.RowId == model.RowId);
                documentDetailContractModel.DMemo = model.DMemo;
                documentDetailContractModel.Price = model.Price;
                documentDetailContractModel.ProductId = model.ProductId;
                documentDetailContractModel.Qty = model.Qty;
                documentDetailContractModel.Summa = model.Summa;
                documentDetailContractModel.Config = model.Config;
            }
            else
            {
                ViewData["EditError"] = "Please, correct all errors.";
                if (model.Qty == 0) model.Qty = 1900;
                ViewData["EditableProduct"] = model;
            }

            return PartialView("DetailsGridPartial", documentContractModel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridDeletePartial(string rowId, string ownewrModelId)
        {
            DocumentContractModel documentContractModel = (DocumentContractModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            DocumentDetailContractModel documentDetailContractModel = documentContractModel.Details.FirstOrDefault(s => s.RowId == rowId);

            if (documentDetailContractModel.Id > 0)
                documentDetailContractModel.StateId = State.STATEDELETED;
            else
                documentContractModel.Details.Remove(documentDetailContractModel);

            return PartialView("DetailsGridPartial", documentContractModel);
        }
        #endregion

        #region Signatures
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
        #endregion

        #region Signatures agents table
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
        #endregion

        public string GetProductMemo(int Id)
        {
            string value = "";
            if (Id > 0)
            {
                Product p = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(Id);
                value = p.Memo;
            }
            return value;
        }
    }
}
