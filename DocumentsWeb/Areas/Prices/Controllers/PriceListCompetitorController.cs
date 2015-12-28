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
    public class PriceListCompetitorController : DocumentsWeb.Controllers.PriceListController
    {
        public PriceListCompetitorController()
        {
            Name = "WEBЦПРК";
            FolderCodeFind = Folder.CODE_FIND_PRICES_COMPETITOR;
            DocumentKindId = DocumentPrices.KINDID_COMPETITOR;
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
            model.FormCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(model.FolderId).Code;
            model.Notes = documentPriceListModel.Notes;

            model.MyCompanyId = model.MainClientDepatmentId ?? 0;
            model.AgentDepartmentFromId = model.MainClientDepatmentId ?? 0;
            model.StateId = State.STATEACTIVE;

            //Разблокировка документа
            if (documentPriceListModel.IsReadOnly && !model.IsReadOnly)
            {
                //Не проверяем модель на валидность - просто меняем флаг
                documentPriceListModel.IsReadOnly = false;
                documentPriceListModel.Save();
                return RedirectToAction("Edit", new { Id = model.Id });
            }

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
