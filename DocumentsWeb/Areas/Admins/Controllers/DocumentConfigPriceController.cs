using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Admins.Models;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Admins.Controllers
{
    /// <summary>
    /// Контроллер настройки раздела "Управление ценами"
    /// </summary>
    [Authorize(Roles = Uid.GROUP_GROUPWEBADMIN)]
    public class DocumentConfigPriceController : BaseDocumentConfigController
    {
        public DocumentConfigPriceController()
        {
            Name = "WEBЦПНАСТРОЙКА";
            FolderCodeFind = Folder.CODE_FIND_PRICE_CONFIG;
        }

        public virtual ActionResult Create(int? tmlId = null)
        {
            Folder folder = WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);

            int formId = folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(formId).Code;
            Document template = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(tmlId.HasValue ? tmlId.Value : folder.DocumentId);

            DocumentConfigPricesModel documentSaleModel = new DocumentConfigPricesModel { FormCode = formCode };
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
            DocumentConfigPricesModel model = DocumentConfigPricesModel.GetObject(id);
            WADataProvider.ModelsCache.Add(model.ModelId, model);
            return EditModel(model.ModelId);
        }
        [HttpGet]
        public virtual ActionResult EditModel(string modelId)
        {
            DocumentConfigPricesModel documentModel = (DocumentConfigPricesModel)WADataProvider.ModelsCache.Get(modelId);
            if (!ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, documentModel.MainCompanyDepatmentId ?? 0);
            ViewResult result = View("Edit", documentModel);

            OnEndingEditModel(result, modelId);
            return result;
        }

        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentConfigPricesModel model)
        {
            DocumentConfigPricesModel documentSaleModel = (DocumentConfigPricesModel)WADataProvider.ModelsCache.Get(model.ModelId);
            //Копирование полей документа, не сохраняющихся на клиенте
            model.Id = documentSaleModel.Id;
            model.TemplateId = documentSaleModel.TemplateId;
            model.FolderId = documentSaleModel.FolderId;
            model.ProjectItemId = documentSaleModel.ProjectItemId;
            model.FormCode = documentSaleModel.FormCode;
            model.KindId = documentSaleModel.KindId;
            model.Notes = documentSaleModel.Notes;

            //Разблокировка документа
            if (documentSaleModel.IsReadOnly && !model.IsReadOnly)
            {
                //Не проверяем модель на валидность - просто меняем флаг
                documentSaleModel.IsReadOnly = false;
                documentSaleModel.Save();
                return RedirectToAction("Edit", new { Id = model.Id });
            }

            model.MainClientDepatmentId = model.MainCompanyDepatmentId;

            ModelState.Clear();
            TryValidateModel(model);
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


        public virtual void CreateCopy(int id)
        {
            DocumentConfigPricesModel.CreateCopy(id);
        }
        public virtual ActionResult ChildDocumentCreate(int id, int chainId, string codeValue)
        {
            return ChildDocumentCreate<DocumentPrices>(id, chainId, codeValue);
        }
    }
}