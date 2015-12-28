using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using DevExpress.Web.ASPxUploadControl;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.Contracts.Models;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Controllers
{
    public class ContractController : CoreDocumentControler
    {
        public ActionResult Create(int? tmlId = null)
        {
            base.OnCreate();
            //Folder folder = WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);

            //int formId = Folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(Folder.FormId).Code;
            Document template = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(tmlId.HasValue ? tmlId.Value : Folder.DocumentId);


            DocumentContractModel documentModel = new DocumentContractModel { FormCode = formCode };
            documentModel.LoadFromTemplate(template);
            WADataProvider.ModelsCache.Add(documentModel.ModelId, documentModel);
            return EditModel(documentModel.ModelId);
        }

        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return Create();
            }
            else
            {
                DocumentContractModel model = DocumentContractModel.GetObject(id);
                WADataProvider.ModelsCache.Add(model.ModelId, model);
                return EditModel(model.ModelId);
            }
        }

        public ActionResult EditModel(string modelId)
        {
            DocumentContractModel doc = (DocumentContractModel)WADataProvider.ModelsCache.Get(modelId);
            if (ClientModel.currentMyCompanies.ContainsKey(System.Web.HttpContext.Current.Session.SessionID))
                ClientModel.currentMyCompanies[System.Web.HttpContext.Current.Session.SessionID] = doc.MainCompanyDepatmentId ?? 0;
            else
                ClientModel.currentMyCompanies.Add(System.Web.HttpContext.Current.Session.SessionID, doc.MainCompanyDepatmentId ?? 0);

            ViewResult result = View("Edit", doc);
            result.ViewData.Add("PrintForms", PrintForms());
            OnEndingEditModel(result);
            return result;
            //return View("Edit", doc);
        }

        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentContractModel model)
        {
            DocumentContractModel m = (DocumentContractModel)WADataProvider.ModelsCache.Get(model.ModelId);
            //Копирование полей документа, не сохраняющихся на клиенте
            model.Id = m.Id;
            model.TemplateId = m.TemplateId;
            model.Files = m.Files;
            model.FolderId = m.FolderId;
            model.ProjectItemId = m.ProjectItemId;
            model.FormCode = m.FormCode;
            model.KindId = m.KindId;
            model.Notes = m.Notes;

            if (ModelState.IsValid)
            {
                model.Save();
                return RedirectToAction("Edit", new { Id = model.Id });
            }
            return View(model);
        }

        public ActionResult AgentFromPartial()
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            int id = int.Parse(Request.Params["Id"]);

            if (!ClientModel.currentMyCompanies.ContainsKey(HttpContext.Session.SessionID))
                ClientModel.currentMyCompanies.Add(HttpContext.Session.SessionID, mainCompanyDepatmentId);
            else
                ClientModel.currentMyCompanies[HttpContext.Session.SessionID] = mainCompanyDepatmentId;

            DocumentContractModel documentContractModel = new DocumentContractModel();
            return PartialView(documentContractModel);
        }

        public void CreateCopy(int id)
        {
            DocumentContractModel.CreateCopy(id);
        }

        public ActionResult RegistratorIdPartial()
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            int id = int.Parse(Request.Params["Id"]);

            DocumentContractModel documentContractModel = new DocumentContractModel();
            PartialViewResult result = PartialView(documentContractModel);
            result.ViewData.Add("MainCompanyDepatmentId", mainCompanyDepatmentId);
            return result;
        }

        public ActionResult ChildDocumentCreate(int id, int chainId, string codeValue)
        {
            return ChildDocumentCreate<DocumentContract>(id, chainId, codeValue);
        }

        #region Файлы
        public ActionResult FileGridPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public ActionResult MultiFileSelection()
        {
            return null;
        }

        /// <summary>
        /// Загрузка нового файла
        /// </summary>
        /// <returns></returns>
        public ActionResult MultiSelectionImageUpload(string ownewrModelId)
        {
            /*UploadedFile[] files =  UploadControlExtension.GetUploadedFiles("ucMultiSelection", ValidationSettings);
            DocumentContractModel doc = (DocumentContractModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            doc.Files.AddRange(files.Select(uploadedFile => new FileDataModel { Id = 0, Name = uploadedFile.FileName, StreamData = uploadedFile.FileBytes }));*/
            HttpContext.Session.Add("MyModelId", ownewrModelId);
            UploadControlExtension.GetUploadedFiles("ucMultiSelection", ValidationSettings, ucContractRevisionMultiSelection_FileUploadComplete);
            return null;
        }

        public static readonly ValidationSettings ValidationSettings = new ValidationSettings
        {
            AllowedFileExtensions = new string[] { ".docx", ".doc", ".jpg", ".png" },
            MaxFileSize = 104857600,
        };

        private void ucContractRevisionMultiSelection_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            string ModelId = Convert.ToString(HttpContext.Session["MyModelId"]);
            DocumentContractModel doc = (DocumentContractModel)WADataProvider.ModelsCache.Get(ModelId);
            doc.Files.Add(new FileDataModel { Id = 0, Name = e.UploadedFile.FileName, StreamData = e.UploadedFile.FileBytes });
            /*string resultFilePath = UploadDirectory + e.UploadedFile.FileName;
            e.UploadedFile.SaveAs(HttpContext.Current.Request.MapPath(resultFilePath));

            UploadingUtils.RemoveFileWithDelay(e.UploadedFile.FileName, HttpContext.Current.Request.MapPath(resultFilePath), 5);

            IUrlResolutionService urlResolver = sender as IUrlResolutionService;
            if (urlResolver != null)
            {
                string file = string.Format("{0} ({1}) {2}KB", e.UploadedFile.FileName, e.UploadedFile.ContentType, e.UploadedFile.ContentLength / 1024);
                string url = urlResolver.ResolveClientUrl(resultFilePath);
                e.CallbackData = file + "|" + url;
            }*/
        }

        public ActionResult FileGridDelete(string rowId, string ownewrModelId)
        {
            DocumentContractModel doc = (DocumentContractModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            doc.Files.RemoveAll(s => s.RowId == rowId);
            return PartialView("FileGridPartial", doc);
        }

        public ActionResult FileGridEdit(string rowId, string ownewrModelId)
        {
            DocumentContractModel documentModel = (DocumentContractModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            FileDataModel fileDataModel = documentModel.Files.FirstOrDefault(s => s.RowId == rowId);
            return View(fileDataModel);
        }

        public ActionResult FillAgentsListPartial()
        {
            return PartialView();
        }
        #endregion
    }
}
