using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.Finances.Models;
using DocumentsWeb.Areas.Products.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Controllers
{
    public class FinanceController : CoreDocumentControler
    {
        public ActionResult Create()
        {
            Folder folder = WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);

            int formId = folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(formId).Code;
            Document template = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(folder.DocumentId);
            
            DocumentFinanceModel documentModel = new DocumentFinanceModel { FormCode = formCode };
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
            else
            {
                DocumentFinanceModel model = DocumentFinanceModel.GetObject(id);
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
            DocumentFinanceModel documentModel = (DocumentFinanceModel)WADataProvider.ModelsCache.Get(modelId);
            if (!ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, documentModel.MainCompanyDepatmentId ?? 0);
            ViewResult result = View("Edit", documentModel);
            OnEndingEditModel(result, modelId);
            return result;
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentFinanceModel model)
        {
            DocumentFinanceModel documentModel = (DocumentFinanceModel)WADataProvider.ModelsCache.Get(model.ModelId);
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
            ViewResult result = View("Edit", model);
            OnEndingEditModel(result, model.ModelId);
            return result;
        }

        public ActionResult BankAccToPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentFinanceModel doc = (DocumentFinanceModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            return PartialView(doc);
        }

        public ActionResult BankAccFromPartial(string modelId)
        {
            int mainClientDepatmentId = Request.Params["MainClientDepatmentId"] == "null" ? 0 : int.Parse(Request.Params["MainClientDepatmentId"]);
            DocumentFinanceModel doc = (DocumentFinanceModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainClientDepatmentId = mainClientDepatmentId;
            return PartialView(doc);
        }

        public void CreateCopy(int id)
        {
            DocumentFinanceModel.CreateCopy(id);
        }

        public ActionResult DetailsGridPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public ActionResult AgentFromPartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
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

        #region Iline Editing
        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridAddNewPartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailFinanceModel model, string ownewrModelId)
        {
            DocumentFinanceModel documentModel = (DocumentFinanceModel)WADataProvider.ModelsCache.Get(ownewrModelId);
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
        public ActionResult DetailsGridUpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailFinanceModel model, string ownewrModelId)
        {
            DocumentFinanceModel documentModel = (DocumentFinanceModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            if (ModelState.IsValid)
            {
                DocumentDetailFinanceModel documentDetailModel = documentModel.Details.FirstOrDefault(s => s.RowId == model.RowId);
                documentDetailModel.Memo = model.Memo;
                documentDetailModel.Price = model.Price;
                documentDetailModel.ProductId = model.ProductId;
                documentDetailModel.Qty = model.Qty;
                documentDetailModel.Summa = model.Summa;
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("DetailsGridPartial", documentModel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridDeletePartial(string rowId, string ownewrModelId)
        {
            DocumentFinanceModel documentModel = (DocumentFinanceModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            DocumentDetailFinanceModel documentDetailModel = documentModel.Details.FirstOrDefault(s => s.RowId == rowId);

            if (documentDetailModel.Id > 0)
                documentDetailModel.StateId = State.STATEDELETED;
            else
                documentModel.Details.Remove(documentDetailModel);

            return PartialView("DetailsGridPartial", documentModel);
        }
        #endregion

        public ActionResult ChildDocumentCreate(int id, int chainId, string codeValue)
        {
            return ChildDocumentCreate<DocumentFinance>(id, chainId, codeValue);
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
        public ActionResult ProductsFinderPartial(int companyId)
        {
            ViewData["Name"] = "ProductsFinder";
            ViewData["ComboboxName"] = "ProductId";
            ViewData["NomComboboxName"] = "ProductNomenclatureId";
            ViewData["ComboboxButtonIndex"] = 0;
            return PartialView("ProductsFinderPartial");
        }
    }
}
