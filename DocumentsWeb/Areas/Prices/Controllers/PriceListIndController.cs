using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Prices.Models;

namespace DocumentsWeb.Areas.Prices.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPPRICES })]
    public class PriceListIndController : DocumentsWeb.Controllers.PriceListController
    {
        public PriceListIndController()
        {
            Name = "WEBЦПРИ";
            FolderCodeFind = Folder.CODE_FIND_PRICES_IND;
            DocumentKindId = DocumentPrices.KINDID_PRICEIND;
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

            if (!ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, model.MainCompanyDepatmentId ?? 0);

            return View(model);
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

        public virtual ActionResult PriceNamePartial(string modelId)
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            DocumentPriceListModel doc = (DocumentPriceListModel)WADataProvider.ModelsCache.Get(modelId);
            doc.MainCompanyDepatmentId = mainCompanyDepatmentId;
            return PartialView(doc);
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
