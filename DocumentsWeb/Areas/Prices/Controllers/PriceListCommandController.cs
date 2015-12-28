using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Prices.Models;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Prices.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPPRICES })]
    public class PriceListCommandController : DocumentsWeb.Controllers.PriceListController
    {
        public PriceListCommandController()
        {
            Name = "WEBЦПРИКАЗ";
            FolderCodeFind = Folder.CODE_FIND_PRICES_COMMAND;
            DocumentKindId = DocumentPrices.KINDID_COMMAND;
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentPriceListModel model)
        {
            DocumentPriceListModel documentPriceListModel = (DocumentPriceListModel)WADataProvider.ModelsCache.Get(model.ModelId);
            //Копирование полей документа, не сохраняющихся на клиенте
            model.Id = documentPriceListModel.Id;
            model.Details = documentPriceListModel.Details;
            model.FolderId = documentPriceListModel.FolderId;
            model.ProjectItemId = documentPriceListModel.ProjectItemId;
            model.FormCode = documentPriceListModel.FormCode;
            model.KindId = documentPriceListModel.KindId;
            model.Notes = documentPriceListModel.Notes;

            model.MyCompanyId = model.MainCompanyDepatmentId ?? 0;
            model.MainClientDepatmentId = model.MainCompanyDepatmentId;
            model.StateId = State.STATEACTIVE;

            //Разблокировка документа
            if (documentPriceListModel.IsReadOnly && !model.IsReadOnly)
            {
                //Не проверяем модель на валидность - просто меняем флаг
                documentPriceListModel.IsReadOnly = false;
                documentPriceListModel.Save();
                return RedirectToAction("Edit", new { Id = model.Id });
            }

            ModelState.Clear();
            this.TryValidateModel(model);
            if (ModelState.IsValid)
            {
                model.Save();
                return RedirectToAction("Edit", new { Id = model.Id });
            }
            return View(model);
        }

        public virtual ActionResult PriceNamePartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentPriceListModel doc = (DocumentPriceListModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            doc.MainClientDepatmentId = mainCompanyDepatmentId;
            return PartialView(doc);
        }
    }
}