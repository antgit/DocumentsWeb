using System;
using System.Data;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Models;

namespace DocumentsWeb.Controllers
{
    [Authorize(Roles = Uid.GROUP_GROUPWEBUSER)]
    public class DocumentController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DocumentFoldersTreePartial()
        {
            return PartialView(WADataProvider.GetViewFolders());
        }
        public ActionResult IndexPartial(bool refresh = false)
        {
            int? folderId = (Request.Params["folderId"] == "null" || Request.Params["folderId"] == null)
                                ? (int?) null
                                : int.Parse(Request.Params["folderId"]);
            if (folderId.HasValue && folderId.Value != 0)
                System.Web.HttpContext.Current.Session["DocumentFolderId"] = folderId;
            if (!folderId.HasValue)
            {
                if (System.Web.HttpContext.Current.Session["DocumentFolderId"] != null)
                    folderId = (int) System.Web.HttpContext.Current.Session["DocumentFolderId"];
                else
                    folderId = 0;
            }
            DataTable tbl = DocumentsWebView.GetView(WADataProvider.WA, 0, folderId.Value,
                                                     System.Web.HttpContext.Current.User.Identity.Name,
                                                     //new DateTime(2001, 1, 1),
                                                     //new DateTime(2014, 1, 1),
                                                     WADataProvider.Period.periodStart,
                                                     WADataProvider.Period.periodEnd, 
                                                     refresh);
            return PartialView(tbl);
            //return PartialView(WADataProvider.GetDocumentsByFolder(folderId));
        }

        public ActionResult DeleteDocument(int id)
        {
            Document value = new Document { Workarea = WADataProvider.WA };
            value.Load(id);
            value.Remove();
            return RedirectToAction("Index", "Document");
        }

        public static object GetRouteValues(string formCode, bool isNds=false, int? id=null, string modelId=null)
        {
            switch (formCode)
            {
                #region Маркетинг
                case "МРАК":
                    return new { Area = "Marketings", Controller = "Mktg", Id = id, ModelId = modelId };
                #endregion

                #region Продажи
                case "ТЗП":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleOrderIn", Id = id, ModelId = modelId };
                case "ТЗПО":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleOrderOut", Id = id, ModelId = modelId };
                case "ТСФВ":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleAccountIn", Id = id, ModelId = modelId };
                case "ТСФИ":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleAccountOut", Id = id, ModelId = modelId };
                case "ТПН":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleIn", Id = id, ModelId = modelId };
                case "ТРН":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleOut", Id = id, ModelId = modelId };
                case "ТАЛВ":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleAssortIn", Id = id, ModelId = modelId };
                case "ТАЛИ":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleAssortOut", Id = id, ModelId = modelId };
                case "ТАП":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleActIn", Id = id, ModelId = modelId };
                case "ТРП":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleActOut", Id = id, ModelId = modelId };
                case "ТВНП":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleReturnIn", Id = id, ModelId = modelId };
                case "ТВНПС":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleReturnOut", Id = id, ModelId = modelId };
                case "ТВП":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleMove", Id = id, ModelId = modelId };
                case "ТИВ":
                    return new { Area = "Sales" + (isNds ? "Nds" : ""), Controller = "SaleInventory", Id = id, ModelId = modelId };
                #endregion

                #region Договора
                case "ДОГ":
                    return new { Area = "Contracts", Controller = "Contract", Id = id, ModelId = modelId };
                case "ДАР":
                    return new { Area = "Contracts", Controller = "Revision", Id = id, ModelId = modelId };
                case "ДАС":
                    return new { Area = "Contracts", Controller = "Verification", Id = id, ModelId = modelId };
                #endregion
                
                #region Налоги
                case "ННВ":
                    return new { Area = "Taxes", Controller = "TaxIn", Id = id, ModelId = modelId };
                case "ННИ":
                    return new { Area = "Taxes", Controller = "TaxOut", Id = id, ModelId = modelId };
                case "ННКВ":
                    return new { Area = "Taxes", Controller = "CorIn", Id = id, ModelId = modelId };
                case "ННКИ":
                    return new { Area = "Taxes", Controller = "CorOut", Id = id, ModelId = modelId };
                #endregion

                #region Цены
                case "ЦПР":
                    return new { Area = "Prices", Controller = "PriceList", Id = id, ModelId = modelId };
                case "ЦПРИ":
                    return new { Area = "Prices", Controller = "PriceListInd", Id = id, ModelId = modelId };
                case "ЦПРП":
                    return new { Area = "Prices", Controller = "PriceListSupplier", Id = id, ModelId = modelId };
                case "ЦПРК":
                    return new { Area = "Prices", Controller = "PriceListCompetitor", Id = id, ModelId = modelId };
                case "ЦПРКИ":
                    return new { Area = "Prices", Controller = "PriceListCompetitorInd", Id = id, ModelId = modelId };
                #endregion

                #region Финансы
                case "ФРД":
                    return new { Area = "Finances" + (isNds ? "Nds" : ""), Controller = "FinanceOut", Id = id, ModelId = modelId };
                case "ФПД":
                    return new { Area = "Finances" + (isNds ? "Nds" : ""), Controller = "FinanceIn", Id = id, ModelId = modelId };
                #endregion
            }
            throw new Exception("Неизвестный код формы документа " + formCode);
        }

        public ActionResult Create(int folderId)
        {
            Folder folder = WADataProvider.WA.Cashe.GetCasheData<Folder>().Item(folderId);
            bool nds = folder.CodeFind.EndsWith("NDS");
            int formId = folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(formId).Code;
            //Document template=WADataProvider.WA.Cashe.GetCasheData<Document>().Item(folder.DocumentId);

            return RedirectToAction("Create", GetRouteValues(formCode, nds));
        }

        public ActionResult Edit(int id)
        {
            DocumentModel doc=DocumentModel.GetObject(id);
            Folder folder = WADataProvider.WA.Cashe.GetCasheData<Folder>().Item(doc.FolderId);
            bool nds = folder.CodeFind.EndsWith("NDS");
            string formCode = doc.FormCode;

            return RedirectToAction("Edit", GetRouteValues(formCode, nds, id));
        }

        #region Договора
        public ActionResult Dogovor()
        {
            return View();
        }

        public ActionResult DogovorPartial()
        {
            return PartialView();
        }

        
        #endregion
    }
}
