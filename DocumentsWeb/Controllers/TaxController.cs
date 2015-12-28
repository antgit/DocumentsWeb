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
using DocumentsWeb.Areas.Taxes.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Controllers
{
    public class TaxController : CoreDocumentControler
    {
        public TaxController()
        {
            PrintModelCode = "TAXES";
        }

        public ActionResult Create(int? tmlId = null)
        {
            Folder folder = WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);

            int formId = folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(formId).Code;
            Document template = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(tmlId.HasValue ? tmlId.Value : folder.DocumentId);


            DocumentTaxModel documentModel = new DocumentTaxModel { FormCode = formCode };
            documentModel.LoadFromTemplate(template);
            WADataProvider.ModelsCache.Add(documentModel.ModelId, documentModel);
            return EditModel(documentModel.ModelId);
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return Create();
            }
            DocumentTaxModel model = DocumentTaxModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            // если тип документа не соответствует типу документов поддерживаемым контроллером!
            if (model.KindId != DocumentKindId)
            {
                throw new DocumentKindIdOpenException("Вы пытаетесь открыть документ несоответствующего типа в данной странице!");
            }
            return EditModel(model.ModelId);
        }
        [HttpGet]
        public ActionResult EditModel(string modelId)
        {
            DocumentTaxModel documentModel = (DocumentTaxModel)WADataProvider.ModelsCache.Get(modelId);
            
            if (ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies[HttpContext.Session.SessionID] = documentModel.MainCompanyDepatmentId ?? 0;
            else
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, documentModel.MainCompanyDepatmentId ?? 0);

            ViewResult result = View("Edit", documentModel);
            OnEndingEditModel(result, modelId);
            return result;
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
            return PriceListHelper.GetLastPriceIn(DateTime.Now, productId, agentFromId, agentToId);
        }
        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentTaxModel model)
        {
            DocumentTaxModel documentModel = (DocumentTaxModel)WADataProvider.ModelsCache.Get(model.ModelId);
            //Копирование полей документа, не сохраняющихся на клиенте
            model.Id = documentModel.Id;
            model.TemplateId = documentModel.TemplateId;
            model.Details = documentModel.Details;
            model.FolderId = documentModel.FolderId;
            model.ProjectItemId = documentModel.ProjectItemId;
            model.FormCode = documentModel.FormCode;
            model.KindId = documentModel.KindId;
            model.Notes = documentModel.Notes;
            
            //Разблокировка документа
            if (documentModel.IsReadOnly && !model.IsReadOnly)
            {
                //Не проверяем модель на валидность - просто меняем флаг
                documentModel.IsReadOnly = false;
                documentModel.Save();
                return RedirectToAction("Edit", new { Id = model.Id });
            }

            if (ModelState.IsValid)
            {
                model.Save();
                return RedirectToAction("Edit", new { Id = model.Id });
            }

            if (ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies[HttpContext.Session.SessionID] = documentModel.MainCompanyDepatmentId ?? 0;
            else
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, documentModel.MainCompanyDepatmentId ?? 0);

            return View(model);
        }

        /// <summary></summary>
        /// <returns></returns>
        public ActionResult DetailsGridPartial(string modelId)
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

        public void CreateCopy(int id)
        {
            DocumentTaxModel.CreateCopy(id);
        }

        #region Iline Editing
        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridAddNewPartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailTaxModel model, string ownewrModelId)
        {
            DocumentTaxModel documentModel = (DocumentTaxModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            if (ModelState.IsValid)
            {
                model.RowId = Guid.NewGuid().ToString();
                model.ProductName = ProductModel.GetObject(model.ProductId).Name;
                model.StateId = State.STATEACTIVE;
                documentModel.Details.Add(model);
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("DetailsGridPartial", WADataProvider.ModelsCache.Get(ownewrModelId));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridUpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailTaxModel model, string ownewrModelId)
        {
            DocumentTaxModel documentModel = (DocumentTaxModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            if (ModelState.IsValid)
            {
                DocumentDetailTaxModel documentDetailTaxModel = documentModel.Details.FirstOrDefault(s => s.RowId == model.RowId);
                documentDetailTaxModel.Memo = model.Memo;
                documentDetailTaxModel.Price = model.Price;
                documentDetailTaxModel.ProductId = model.ProductId;
                documentDetailTaxModel.Qty = model.Qty;
                documentDetailTaxModel.Summa = model.Summa;
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("DetailsGridPartial", documentModel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridDeletePartial(string rowId, string ownewrModelId)
        {
            DocumentTaxModel documentModel = (DocumentTaxModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            DocumentDetailTaxModel documentDetailTaxModel = documentModel.Details.FirstOrDefault(s => s.RowId == rowId);

            if (documentDetailTaxModel.Id > 0)
                documentDetailTaxModel.StateId = State.STATEDELETED;
            else
                documentModel.Details.Remove(documentDetailTaxModel);

            return PartialView("DetailsGridPartial", documentModel);
        }
        #endregion

        public ActionResult ChildDocumentCreate(int id, int chainId, string codeValue)
        {
            return ChildDocumentCreate<DocumentTaxes>(id, chainId, codeValue);
        }

        public virtual ActionResult ProductPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public virtual ActionResult ProductNomenclaturePartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public ActionResult ClientsFinderPartial()
        {
            ViewData["Name"] = "ClientsFinderAgentFrom";
            ViewData["ComboboxName"] = GlobalPropertyNames.MainClientDepatmentId;
            ViewData["ComboboxClientAcc"] = "MainClientAccountId";
            ViewData["ComboboxButtonIndex"] = 2;
            ViewData["onlyUsers"] = false;
            return PartialView("ClientsFinderPartial");
        }
    }
}
