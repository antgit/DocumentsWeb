using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.Prices.Models;
using DocumentsWeb.Areas.Products.Models;
using DocumentsWeb.Models;

namespace DocumentsWeb.Controllers
{
    public class PriceListController : CoreDocumentControler
    {
        public PriceListController()
        {
            PrintModelCode = "PRICES";
        }
        public ActionResult Create()
        {
            Folder folder = WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);

            int formId = folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(formId).Code;
            Document template = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(folder.DocumentId);


            DocumentPriceListModel documentSaleModel = new DocumentPriceListModel { FormCode = formCode };
            documentSaleModel.LoadFromTemplate(template);
            WADataProvider.ModelsCache.Add(documentSaleModel.ModelId, documentSaleModel);
            return EditModel(documentSaleModel.ModelId);
        }

        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return Create();
            }
            else
            {
                DocumentPriceListModel model = DocumentPriceListModel.GetObject(id);
                WADataProvider.ModelsCache.Add(model.ModelId, model);
                // если тип документа не соответствует типу документов поддерживаемым контроллером!
                if (model.KindId != DocumentKindId)
                {
                    throw new DocumentKindIdOpenException("Вы пытаетесь открыть документ несоответствующего типа в данной странице!");
                }
                return EditModel(model.ModelId);
            }
        }

        public ActionResult EditModel(string modelId)
        {
            DocumentPriceListModel documentPriceListModel = (DocumentPriceListModel)WADataProvider.ModelsCache.Get(modelId);
            if (!ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, documentPriceListModel.MainCompanyDepatmentId ?? 0);
            ViewResult result = View("Edit", documentPriceListModel);
            OnEndingEditModel(result, modelId);
            return result;
        }

        public ActionResult DetailsGridPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public ActionResult PriceListPriceNamePartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentPriceListModel doc = (DocumentPriceListModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            return PartialView(doc);
        }

        #region Iline Editing
        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridAddNewPartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailPriceListModel model, string ownewrModelId)
        {
            DocumentPriceListModel documentPriceListModel = (DocumentPriceListModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            if (ModelState.IsValid)
            {
                model.RowId = Guid.NewGuid().ToString();
                model.ProductName = ProductModel.GetObject(model.ProductId).Name;
                model.StateId = State.STATEACTIVE;
                documentPriceListModel.Details.Add(model);
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("DetailsGridPartial", WADataProvider.ModelsCache.Get(ownewrModelId));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridUpdatePartial([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentDetailPriceListModel model, string ownewrModelId)
        {
            DocumentPriceListModel documentPriceListModel = (DocumentPriceListModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            if (ModelState.IsValid)
            {
                DocumentDetailPriceListModel documentDetailPriceListModel = documentPriceListModel.Details.FirstOrDefault(s => s.RowId == model.RowId);
                documentDetailPriceListModel.DetailMemo = model.DetailMemo;
                documentDetailPriceListModel.Price = model.Price;
                documentDetailPriceListModel.ProductId = model.ProductId;
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("DetailsGridPartial", documentPriceListModel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DetailsGridDeletePartial(string rowId, string ownewrModelId)
        {
            DocumentPriceListModel documentPriceListModel = (DocumentPriceListModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            DocumentDetailPriceListModel documentDetailPriceListModel = documentPriceListModel.Details.FirstOrDefault(s => s.RowId == rowId);

            if (documentDetailPriceListModel.Id > 0)
                documentDetailPriceListModel.StateId = State.STATEDELETED;
            else
                documentPriceListModel.Details.Remove(documentDetailPriceListModel);

            return PartialView("DetailsGridPartial", documentPriceListModel);
        }
        #endregion

        public void CreateCopy(int id)
        {
            DocumentPriceListModel.CreateCopy(id);
        }

        public ActionResult ChildDocumentCreate(int id, int chainId, string codeValue)
        {
            return ChildDocumentCreate<DocumentPrices>(id, chainId, codeValue);
        }

        public virtual ActionResult ProductPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public virtual ActionResult ProductNomenclaturePartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }
    }
}
