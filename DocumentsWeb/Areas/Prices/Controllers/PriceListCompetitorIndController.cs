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
    public class PriceListCompetitorIndController : DocumentsWeb.Controllers.PriceListController
    {
        public PriceListCompetitorIndController()
        {
            Name = "WEBЦПРКИ";
            FolderCodeFind = Folder.CODE_FIND_PRICES_COMPETITORIND;
            DocumentKindId = DocumentPrices.KINDID_COMPETITORIND;
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

            //model.MyCompanyId = model.AgentFromId;
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
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, model.MyCompanyId);

            return View(model);
        }

        public ActionResult AgentFromPartial(string modelId)
        {
            int myCompanyId = int.Parse(Request.Params["MyCompanyId"] == null || Request.Params["MyCompanyId"] == "null" ? "0" : Request.Params["MyCompanyId"]);

            if (ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies[HttpContext.Session.SessionID] = myCompanyId;
            else
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, myCompanyId);

            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public ActionResult AgentToPartial(string modelId)
        {
            int myCompanyId = int.Parse(Request.Params["MyCompanyId"] == null || Request.Params["MyCompanyId"] == "null" ? "0" : Request.Params["MyCompanyId"]);

            if (ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies[HttpContext.Session.SessionID] = myCompanyId;
            else
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, myCompanyId);

            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }
    }
}
