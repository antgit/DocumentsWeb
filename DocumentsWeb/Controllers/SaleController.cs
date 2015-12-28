using System;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.Products.Models;
using DocumentsWeb.Areas.Sales.Models;
using DocumentsWeb.Models;

namespace DocumentsWeb.Controllers
{
    public class SaleController : CoreDocumentControler
    {
        public SaleController()
        {
            PrintModelCode = "SALES";
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
            decimal price =PriceListHelper.GetPriceOut(priceNameId, productId, agentFromId, DateTime.Now, agentToId);
            return price;
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
            return PriceListHelper.GetLastPriceIn(DateTime.Now, productId, agentFromId, agentToId);
        }
        /*public virtual ActionResult Create()
        {
            Folder folder = WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);

            int formId = folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(formId).Code;
            Document template = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(folder.DocumentId);


            DocumentSaleModel documentSaleModel = new DocumentSaleModel { FormCode = formCode };
            documentSaleModel.LoadFromTemplate(template);
            WADataProvider.ModelsCache.Add(documentSaleModel.ModelId, documentSaleModel);
            return EditModel(documentSaleModel.ModelId);
        }*/

        public virtual ActionResult Create(int? tmlId = null)
        {
            Folder folder = WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);

            int formId = folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(formId).Code;
            Document template = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(tmlId.HasValue ? tmlId.Value : folder.DocumentId);

            DocumentSaleModel documentSaleModel = new DocumentSaleModel { FormCode = formCode };
            documentSaleModel.LoadFromTemplate(template);
            WADataProvider.ModelsCache.Add(documentSaleModel.ModelId, documentSaleModel);
            return EditModel(documentSaleModel.ModelId);
        }

        [HttpGet]
        public virtual ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return Create();
            }
            DocumentSaleModel model = DocumentSaleModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            // если тип документа не соответствует типу документов поддерживаемым контроллером!
            if(model.KindId!= DocumentKindId)
            {
                throw new DocumentKindIdOpenException("Вы пытаетесь открыть документ несоответствующего типа в данной странице!");
            }
            return EditModel(model.ModelId);
        }
        [HttpGet]
        public virtual ActionResult EditModel(string modelId)
        {
            //ViewData["IsDinamicScriptRegister"] = true;
            //ViewData["GETSCRIPTS.SALESDOCUMENT"] = true;
            DocumentSaleModel documentModel = (DocumentSaleModel)WADataProvider.ModelsCache.Get(modelId);
            if (!ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, documentModel.MainCompanyDepatmentId ?? 0);
            ViewResult result = View("Edit", documentModel);

            OnEndingEditModel(result, modelId);
            return result;
        }

        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentSaleModel model)
        {
            DocumentSaleModel documentSaleModel = (DocumentSaleModel)WADataProvider.ModelsCache.Get(model.ModelId);
            //Копирование полей документа, не сохраняющихся на клиенте
            model.Id = documentSaleModel.Id;
            model.TemplateId = documentSaleModel.TemplateId;
            model.Details = documentSaleModel.Details;
            model.FolderId = documentSaleModel.FolderId;
            model.ProjectItemId = documentSaleModel.ProjectItemId;
            model.FormCode = documentSaleModel.FormCode;
            model.KindId = documentSaleModel.KindId;
            model.Notes = documentSaleModel.Notes;

            //Разблокировка документа
            if(documentSaleModel.IsReadOnly && !model.IsReadOnly)
            {
                //Не проверяем модель на валидность - просто меняем флаг
                documentSaleModel.IsReadOnly = false;
                documentSaleModel.Save();
                return RedirectToAction("Edit", new { Id = model.Id });
            }

            if (ModelState.IsValid)
            {
                model.Save();
                return RedirectToAction("Edit", new { Id = model.Id });
            }

            if (!ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, model.MainCompanyDepatmentId ?? 0);
            ViewResult result = View("Edit", model);
            OnEndingEditModel(result, model.ModelId);
            return result;
        }

        /// <summary></summary>
        /// <returns></returns>
        public virtual ActionResult DetailsGridPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public virtual ActionResult SupervisorPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentSaleModel doc = (DocumentSaleModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            doc.SupervisorId = 0;
            doc.ManagerId = 0;
            return PartialView(doc);
        }

        public virtual ActionResult ManagerPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentSaleModel doc = null;
            if (!WADataProvider.ModelsCache.Exists(modelId))
            {
                int id = int.Parse(Request.Params["Id"]);
                doc = DocumentSaleModel.GetObject(id);
            }
            else
            {
                doc = (DocumentSaleModel)WADataProvider.ModelsCache.Get(modelId);
            }
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            doc.SupervisorId = 0;
            doc.ManagerId = 0;
            return PartialView(doc);
        }

        public virtual ActionResult AgentFromPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);

            if (ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies[HttpContext.Session.SessionID] = mainCompanyDepatmentId;
            else
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, mainCompanyDepatmentId);

            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public virtual ActionResult ProductPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public virtual ActionResult ProductNomenclaturePartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public virtual ActionResult PriceNamePartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentSaleModel doc = (DocumentSaleModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
// ReSharper disable Mvc.PartialViewNotResolved
            return PartialView(doc);
// ReSharper restore Mvc.PartialViewNotResolved
        }

        public virtual ActionResult BankAccToPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentSaleModel doc = (DocumentSaleModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
// ReSharper disable Mvc.PartialViewNotResolved
            return PartialView(doc);
// ReSharper restore Mvc.PartialViewNotResolved
        }

        public virtual ActionResult BankAccFromPartial(string modelId)
        {
            int mainClientDepatmentId = Request.Params["MainClientDepatmentId"] == "null" ? 0 : int.Parse(Request.Params["MainClientDepatmentId"]);
            DocumentSaleModel doc = (DocumentSaleModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainClientDepatmentId = mainClientDepatmentId;
// ReSharper disable Mvc.PartialViewNotResolved
            return PartialView(doc);
// ReSharper restore Mvc.PartialViewNotResolved
        }

        public virtual ActionResult StoreToPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentSaleModel doc = (DocumentSaleModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            return PartialView(doc);
        }

        public virtual ActionResult StoreFromPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentSaleModel doc = (DocumentSaleModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            return PartialView(doc);
        }

        public virtual void CreateCopy(int id)
        {
            DocumentSaleModel.CreateCopy(id);
        }

        #region Iline Editing
        [HttpPost, ValidateInput(false)]
        public virtual ActionResult DetailsGridAddNewPartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailSaleModel model, string ownewrModelId)
        {
            DocumentSaleModel documentSaleModel = (DocumentSaleModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            if (ModelState.IsValid)
            {
                model.RowId = Guid.NewGuid().ToString();
                model.ProductName = ProductModel.GetObject(model.ProductId).Name;
                model.StateId = State.STATEACTIVE;
                documentSaleModel.Details.Add(model);
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("DetailsGridPartial", documentSaleModel);
        }

        [HttpPost, ValidateInput(false)]
        public virtual ActionResult DetailsGridUpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailSaleModel model, string ownewrModelId)
        {
            DocumentSaleModel documentSaleModel = (DocumentSaleModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            if (ModelState.IsValid)
            {
                DocumentDetailSaleModel documentDetailSaleModel = documentSaleModel.Details.FirstOrDefault(s => s.RowId == model.RowId);
                documentDetailSaleModel.Memo = model.Memo;
                documentDetailSaleModel.Price = model.Price;
                documentDetailSaleModel.ProductId = model.ProductId;
                documentDetailSaleModel.ProductName = ProductModel.GetObject(model.ProductId).Name;
                documentDetailSaleModel.Qty = model.Qty;
                documentDetailSaleModel.Summa = model.Summa;
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("DetailsGridPartial", documentSaleModel);
        }

        [HttpPost, ValidateInput(false)]
        public virtual ActionResult DetailsGridDeletePartial(string rowId, string ownewrModelId)
        {
            DocumentSaleModel documentSaleModel = (DocumentSaleModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            DocumentDetailSaleModel documentDetailSaleModel = documentSaleModel.Details.FirstOrDefault(s => s.RowId == rowId);

            if (documentDetailSaleModel.Id > 0)
                documentDetailSaleModel.StateId = State.STATEDELETED;
            else
                documentSaleModel.Details.Remove(documentDetailSaleModel);

            return PartialView("DetailsGridPartial", documentSaleModel);
        }
        #endregion

        public virtual ActionResult ChildDocumentCreate(int id, int chainId, string codeValue)
        {
            return ChildDocumentCreate<DocumentSales>(id, chainId, codeValue);
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
            ViewData["showOnlyProducts"] = true;
            return PartialView("ProductsFinderPartial");
        }

    }
}
