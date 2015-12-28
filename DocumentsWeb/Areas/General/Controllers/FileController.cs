using System;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Web.ASPxUploadControl;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.Contracts.Models;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.General.Controllers
{
    public class FileController : Controller
    {
        //
        // GET: /FileData/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Редактирование файла в БД
        /// </summary>
        /// <param name="id">Идентификатор файла</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Редктирование фала в памяти
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public ActionResult EditModel(string rowId)
        {
            FileDataModel fileDataModel = FileDataModel.GetByModelId(rowId);
            return View("Edit", fileDataModel);
        }


        public ActionResult CreateNewFileModel()
        {
            FileDataModel fileDataModel = new FileDataModel();
            return View("Edit", fileDataModel);
        }


        [HttpPost]
        public ActionResult EditDocumentContractEditFile([ModelBinder(typeof(DevExpressEditorsBinder))] FileDataModel model)
        {
            if (ModelState.IsValid)
            {
                FileDataModel file = null;
                foreach (var doc in WADataProvider.ModelsCache.Values)
                {
                    if (!(doc.Value is DocumentContractModel))
                        continue;

                    file = ((DocumentContractModel)doc.Value).Files.FirstOrDefault(s => s.RowId == model.RowId);
                    if (file == null)
                        continue;

                }
                if (file != null)
                {
                    file.Category = model.Category;
                    file.ContentType = model.ContentType;
                    file.Theme = model.Theme;
                    file.IsChanged = true;
                }

                //return View("EditingComplete2", model);
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))] FileDataModel model)
        {
            if (ModelState.IsValid)
            {
                //TODO: Не учитывается вариант редактирования фала из БД
                FileDataModel file = FileDataModel.GetByModelId(model.RowId);

                if (file != null)
                {
                    file.Category = model.Category;
                    file.ContentType = model.ContentType;
                    file.Theme = model.Theme;
                    file.AllowVersion = model.AllowVersion;
                    file.IsChanged = true;
                }

                //return View("EditingComplete2", model);
            }
            return View(model);
        }

        /// <summary>
        /// Передача файла клиенту по идентификатору его модели
        /// </summary>
        /// <remarks>Файл должен быть предварительно загружен в память</remarks>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public ActionResult GetFile(string rowId)
        {
            FileDataModel fileDataModel = FileDataModel.GetByModelId(rowId);
            return File(fileDataModel.StreamData, "application/octet-stream", fileDataModel.Name);
        }

        /// <summary>
        /// Передача версии файла клиенту по идентификатору её модели
        /// </summary>
        /// <remarks>Версия файл должен быть предварительно загружен в память</remarks>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public ActionResult GetFileVersion(string rowId)
        {
            FileVersionModel fileVersionDataModel = FileVersionModel.GetByModelId(rowId);
            return File(fileVersionDataModel.StreamData, "application/octet-stream", fileVersionDataModel.Owner.Name);
        }

        /// <summary>
        /// Замена файла
        /// </summary>
        /// <returns></returns>
        public ActionResult FileUpload(string rowId)
        {
            UploadedFile[] files = UploadControlExtension.GetUploadedFiles("ucMultiSelection");

            FileDataModel file = FileDataModel.GetByModelId(rowId);

            if (file != null)
            {
                file.StreamData = files[0].FileBytes;
                file.IsChanged = true;
                if (file.AllowVersion)
                {
                    file.Versions.Add(new FileVersionModel
                    {
                        DateModified = DateTime.Now,
                        Owner = file,
                        UserName = System.Web.HttpContext.Current.User.Identity.Name,
                        StreamData = file.StreamData,
                        RowId = Guid.NewGuid().ToString()
                    });
                }
            }
            return null;
        }

        public ActionResult FileVersionsPartial(string rowId)
        {
            FileDataModel file = FileDataModel.GetByModelId(rowId);
            return PartialView(file);
        }

        #region Грид
        public ActionResult FileGrid(string modelId)
        {
            return View("FileGridPartial", WADataProvider.ModelsCache.Get(modelId));
        }

        public ActionResult FileGridPartial(string modelId)
        {
            return PartialView(WADataProvider.ModelsCache.Get(modelId));
        }

        public ActionResult FileGridWithUploader(string modelId, bool enabled=true)
        {
            PartialViewResult result=PartialView(WADataProvider.ModelsCache.Get(modelId));
            result.ViewData.Add("Enabled", enabled);
            return result;
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
            IFileOwner owner = (IFileOwner)WADataProvider.ModelsCache.Get(ModelId);
            owner.Files.Add(new FileDataModel { Id = 0, Name = e.UploadedFile.FileName, StreamData = e.UploadedFile.FileBytes });
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
