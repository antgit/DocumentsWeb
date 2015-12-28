using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.Products.Models;
using DocumentsWeb.Areas.Services.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Controllers
{
    public class ServiceController : CoreDocumentControler
    {
        public ServiceController()
        {
            PrintModelCode = "SERVICES";
        }
        /// <summary>
        /// Цена по прайс-листу
        /// </summary>
        /// <param name="priceNameId">Идентификатор цены</param>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="agentFromId">Идентификатор корреспондента "Кто" (поставщик)</param>
        /// <param name="agentToId">Идентификатор корреспондента "Кому" (покупатель)</param>
        /// <returns></returns>
        public virtual decimal GetPriceOut(int priceNameId, int productId, int agentFromId, int agentToId)
        {
            return PriceListHelper.GetPriceOut(priceNameId, productId, agentFromId, DateTime.Now, agentToId);
        }

        /// <summary>
        /// Цена по данным последнего прихода
        /// </summary>
        /// <param name="priceNameId">Идентификатор цены</param>
        /// <param name="productId">Идентификатор товара</param>
        /// <param name="agentFromId">Идентификатор корреспондента "Кто" (поставщик)</param>
        /// <param name="agentToId">Идентификатор корреспондента "Кому" (покупатель) </param>
        /// <returns></returns>
        public virtual decimal GetLastPriceIn(int priceNameId, int productId, int agentFromId, int agentToId)
        {
            return PriceListHelper.GetLastPriceInService(DateTime.Now, productId, agentFromId, agentToId);
        }
        public ActionResult Create(int? tmlId = null)
        {
            Folder folder = WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);

            int formId = folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(formId).Code;
            Document template = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(tmlId.HasValue ? tmlId.Value : folder.DocumentId);
            
            DocumentServiceModel documentServiceModel = new DocumentServiceModel { FormCode = formCode };
            documentServiceModel.LoadFromTemplate(template);
            WADataProvider.ModelsCache.Add(documentServiceModel.ModelId, documentServiceModel);
            return EditModel(documentServiceModel.ModelId);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return Create();
            }
            else
            {
                DocumentServiceModel model = DocumentServiceModel.GetObject(id);
                WADataProvider.ModelsCache.Add(model.ModelId, model);
                // если тип документа не соответствует типу документов поддерживаемым контроллером!
                if (model.KindId != DocumentKindId)
                {
                    throw new DocumentKindIdOpenException("Вы пытаетесь открыть документ несоответствующего типа в данной странице!");
                }
                return EditModel(model.ModelId);
            }
        }
        [HttpGet]
        public ActionResult EditModel(string modelId)
        {
            DocumentServiceModel documentModel = (DocumentServiceModel)WADataProvider.ModelsCache.Get(modelId);
            if (!ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, documentModel.MainCompanyDepatmentId ?? 0);

            ViewResult result = View("Edit", documentModel);
            OnEndingEditModel(result, modelId);
            return result;
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentServiceModel model)
        {
            DocumentServiceModel documentServiceModel = (DocumentServiceModel)WADataProvider.ModelsCache.Get(model.ModelId);
            //Копирование полей документа, не сохраняющихся на клиенте
            model.Id = documentServiceModel.Id;
            model.TemplateId = documentServiceModel.TemplateId;
            model.Details = documentServiceModel.Details;
            model.FolderId = documentServiceModel.FolderId;
            model.ProjectItemId = documentServiceModel.ProjectItemId;
            model.FormCode = documentServiceModel.FormCode;
            model.KindId = documentServiceModel.KindId;
            model.Notes = documentServiceModel.Notes;

            //Разблокировка документа
            if (documentServiceModel.IsReadOnly && !model.IsReadOnly)
            {
                //Не проверяем модель на валидность - просто меняем флаг
                documentServiceModel.IsReadOnly = false;
                documentServiceModel.Save();
                return RedirectToAction("Edit", new { Id = model.Id });
            }

            if (ModelState.IsValid)
            {
                model.Save();
                return RedirectToAction("Edit", new { Id = model.Id });
            }
            ViewResult result = View("Edit", model);
            OnEndingEditModel(result, model.ModelId);
            return result;
        }

        /// <summary></summary>
        /// <returns></returns>
        public ActionResult DetailsGridPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public ActionResult ProductPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public virtual ActionResult ProductNomenclaturePartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public ActionResult AgentFromPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            if (ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies[HttpContext.Session.SessionID] = mainCompanyDepatmentId;
            else
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, mainCompanyDepatmentId);
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public ActionResult SupervisorPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            //int id = int.Parse(Request.Params["Id"]);
            DocumentServiceModel doc = (DocumentServiceModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            doc.SupervisorId = 0;
            doc.ManagerId = 0;
            return PartialView(doc);
        }

        public ActionResult ManagerPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentServiceModel doc = null;
            if (!WADataProvider.ModelsCache.Exists(modelId))
            {
                int id = int.Parse(Request.Params["Id"]);
                doc = DocumentServiceModel.GetObject(id);
            }
            else
            {
                doc = (DocumentServiceModel)WADataProvider.ModelsCache.Get(modelId);
            }
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            doc.SupervisorId = 0;
            doc.ManagerId = 0;
            return PartialView(doc);
        }

        public ActionResult BankAccToPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentServiceModel doc = (DocumentServiceModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            return PartialView(doc);
        }

        public ActionResult BankAccFromPartial(string modelId)
        {
            int mainClientDepatmentId = Request.Params["MainClientDepatmentId"] == "null" ? 0 : int.Parse(Request.Params["MainClientDepatmentId"]);
            DocumentServiceModel doc = (DocumentServiceModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainClientDepatmentId = mainClientDepatmentId;
            return PartialView(doc);
        }

        public void CreateCopy(int id)
        {
            DocumentServiceModel.CreateCopy(id);
        }

        #region Iline Editing
        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridAddNewPartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailServiceModel model, string ownewrModelId)
        {
            DocumentServiceModel DocumentServiceModel = (DocumentServiceModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            if (ModelState.IsValid)
            {
                model.RowId = Guid.NewGuid().ToString();
                model.ProductName = ProductModel.GetObject(model.ProductId).Name;
                model.StateId = State.STATEACTIVE;
                DocumentServiceModel.Details.Add(model);
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("DetailsGridPartial", DocumentServiceModel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridUpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailServiceModel model, string ownewrModelId)
        {
            DocumentServiceModel DocumentServiceModel = (DocumentServiceModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            if (ModelState.IsValid)
            {
                DocumentDetailServiceModel DocumentDetailServiceModel = DocumentServiceModel.Details.FirstOrDefault(s => s.RowId == model.RowId);
                DocumentDetailServiceModel.Memo = model.Memo;
                DocumentDetailServiceModel.Price = model.Price;
                DocumentDetailServiceModel.ProductId = model.ProductId;
                DocumentDetailServiceModel.ProductName = ProductModel.GetObject(model.ProductId).Name;
                DocumentDetailServiceModel.Qty = model.Qty;
                DocumentDetailServiceModel.Summa = model.Summa;
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("DetailsGridPartial", DocumentServiceModel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridDeletePartial(string rowId, string ownewrModelId)
        {
            DocumentServiceModel DocumentServiceModel = (DocumentServiceModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            DocumentDetailServiceModel DocumentDetailServiceModel = DocumentServiceModel.Details.FirstOrDefault(s => s.RowId == rowId);

            if (DocumentDetailServiceModel.Id > 0)
                DocumentDetailServiceModel.StateId = State.STATEDELETED;
            else
                DocumentServiceModel.Details.Remove(DocumentDetailServiceModel);

            return PartialView("DetailsGridPartial", DocumentServiceModel);
        }
        #endregion

        public ActionResult ChildDocumentCreate(int id, int chainId, string codeValue)
        {
            return ChildDocumentCreate<DocumentService>(id, chainId, codeValue);
        }

        public ActionResult PriceNamePartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentServiceModel doc = (DocumentServiceModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            return PartialView(doc);
        }

        public virtual ActionResult ClientsFinderPartial()
        {
            ViewData["Name"] = "ClientsFinderAgentFrom";
            ViewData["ComboboxName"] = GlobalPropertyNames.MainClientDepatmentId;
            ViewData["ComboboxClientAcc"] = "MainClientAccountId";
            ViewData["ComboboxButtonIndex"] = 2;
            ViewData["onlyUsers"] = false;
            return PartialView("ClientsFinderPartial");
        }
        public ActionResult ShowPeopleFinderSupervisorPartial(int companyId)
        {
            ViewData["Name"] = "PeopleFinderSupervisor";
            ViewData["ComboboxName"] = "SupervisorId";
            ViewData["ComboboxButtonIndex"] = 0;
            ViewData["onlyUsers"] = false;
            ViewData["ComboboxMyCompany"] = GlobalPropertyNames.MainCompanyDepatmentId;
            ViewData["showAgentsInChains"] = true;
            ViewData["myCompanyId"] = companyId;
            return PartialView("PeoplesFinderPartial");
        }
        public ActionResult ShowPeopleFinderManagerPartial(int companyId)
        {
            ViewData["Name"] = "PeopleFinderManager";
            ViewData["ComboboxName"] = "ManagerId";
            ViewData["ComboboxButtonIndex"] = 0;
            ViewData["onlyUsers"] = false;
            ViewData["ComboboxMyCompany"] = GlobalPropertyNames.MainCompanyDepatmentId;
            ViewData["showAgentsInChains"] = true;
            ViewData["myCompanyId"] = companyId;

            return PartialView("PeoplesFinderPartial");
        }

        public ActionResult ProductsFinderPartial(int companyId)
        {
            ViewData["Name"] = "ProductsFinder";
            ViewData["ComboboxName"] = "ProductId";
            ViewData["NomComboboxName"] = "ProductNomenclatureId";
            ViewData["ComboboxButtonIndex"] = 0;
            ViewData["showOnlyServices"] = true;
            return PartialView("ProductsFinderPartial");
        }
    }
}
