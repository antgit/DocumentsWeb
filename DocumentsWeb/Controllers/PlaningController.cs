using System;
using System.Linq;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Documents;
using DevExpress.Web.ASPxUploadControl;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.Contracts.Models;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Planing.Models;

namespace DocumentsWeb.Controllers
{
    public class PlaningController : CoreDocumentControler
    {
        public ActionResult Create()
        {
            base.OnCreate();
            //Folder folder = WADataProvider.WA.GetFolderByCodeFind(FolderCodeFind);

            //int formId = Folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(Folder.FormId).Code;
            Document template = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(Folder.DocumentId);


            DocumentPlaningModel documentModel = new DocumentPlaningModel { FormCode = formCode };
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
                DocumentPlaningModel model = DocumentPlaningModel.GetObject(id);
                WADataProvider.ModelsCache.Add(model.ModelId, model);
                return EditModel(model.ModelId);
            }
        }

        public ActionResult EditModel(string modelId)
        {
            DocumentPlaningModel doc = (DocumentPlaningModel)WADataProvider.ModelsCache.Get(modelId);
            if (ClientModel.currentMyCompanies.ContainsKey(System.Web.HttpContext.Current.Session.SessionID))
                ClientModel.currentMyCompanies[System.Web.HttpContext.Current.Session.SessionID] = doc.MainCompanyDepatmentId ?? 0;
            else
                ClientModel.currentMyCompanies.Add(System.Web.HttpContext.Current.Session.SessionID, doc.MainCompanyDepatmentId ?? 0);

            ViewResult result = View("Edit", doc);
            result.ViewData.Add("PrintForms", PrintForms());
            return result;
            //return View("Edit", doc);
        }

        [HttpPost]
        public virtual ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] DocumentPlaningModel model)
        {
            DocumentPlaningModel m = (DocumentPlaningModel)WADataProvider.ModelsCache.Get(model.ModelId);
            //Копирование полей документа, не сохраняющихся на клиенте
            model.Id = m.Id;
            model.Files = m.Files;
            model.FolderId = m.FolderId;
            model.ProjectItemId = m.ProjectItemId;
            model.FormCode = m.FormCode;
            model.KindId = m.KindId;
            //TODO: Переместить на форму
            model.StateCurrentId = 106;

            if (ModelState.IsValid)
            {
                DocumentPlan doc = model.ToObject(WADataProvider.WA);
                doc.UserName = WADataProvider.CurrentMembershipUser.UserName;
                doc.Save();

                foreach (DocumentSignModel sm in m.Signs)
                {
                    sm.OwnerId = doc.Id;
                    DocumentSign sign = sm.ToObject();
                    sign.Id = sm.Id;
                    sign.Save();
                    sm.Id = sign.Id;
                }

                model.Id = doc.Id;
                model.SaveFiles();
                WADataProvider.ModelsCache.Remove(model.ModelId);
                //return View("EditingComplete2");
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

            DocumentPlaningModel documentContractModel = new DocumentPlaningModel();
            return PartialView(documentContractModel);
        }

        public void CreateCopy(int id)
        {
            DocumentPlaningModel.CreateCopy(id);
        }

        public ActionResult RegistratorIdPartial()
        {
            int mainCompanyDepatmentId = int.Parse(Request.Params["MainCompanyDepatmentId"] == null || Request.Params["MainCompanyDepatmentId"] == "null" ? "0" : Request.Params["MainCompanyDepatmentId"]);
            int id = int.Parse(Request.Params["Id"]);

            DocumentPlaningModel documentContractModel = new DocumentPlaningModel();
            PartialViewResult result = PartialView(documentContractModel);
            result.ViewData.Add("MainCompanyDepatmentId", mainCompanyDepatmentId);
            return result;
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
            DocumentPlaningModel doc = (DocumentPlaningModel)WADataProvider.ModelsCache.Get(ownewrModelId);
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
            DocumentPlaningModel doc = (DocumentPlaningModel)WADataProvider.ModelsCache.Get(ModelId);
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
            DocumentPlaningModel doc = (DocumentPlaningModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            doc.Files.RemoveAll(s => s.RowId == rowId);
            return PartialView("FileGridPartial", doc);
        }

        public ActionResult FileGridEdit(string rowId, string ownewrModelId)
        {
            DocumentPlaningModel documentModel = (DocumentPlaningModel)WADataProvider.ModelsCache.Get(ownewrModelId);
            FileDataModel fileDataModel = documentModel.Files.FirstOrDefault(s => s.RowId == rowId);
            return View(fileDataModel);
        }

        public ActionResult FillAgentsListPartial()
        {
            return PartialView();
        }
        #endregion

        public ActionResult ChildDocumentCreate(int id, int chainId, string codeValue)
        {
            return ChildDocumentCreate<DocumentPlan>(id, chainId, codeValue);
        }
    }
}