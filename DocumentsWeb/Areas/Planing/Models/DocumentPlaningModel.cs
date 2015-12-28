using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects.Documents;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.General.Models;
using System.ComponentModel.DataAnnotations;
using DocumentsWeb.Areas.Contracts.Models;
using BusinessObjects;

namespace DocumentsWeb.Areas.Planing.Models
{
    /// <summary>
    /// Планирование
    /// </summary>
    public class DocumentPlaningModel : DocumentModel, IFileOwner
    {
        /// <summary>Идентификатор важности документа</summary>
        public int? ImportanceId { get; set; }
        /// <summary>Идентификатор периода планирования</summary>
        [Required(ErrorMessage = "Укажите период планирования")]
        public int? DateregionId { get; set; }

        /// <summary>Идентификатор типа планирования</summary>
        [Required(ErrorMessage = "Укажите тип планирования")]
        public int? PlanKindId { get; set; }
        /// <summary>Дата начала расмоттрения</summary>
        public DateTime? DateStart { get; set; }
        /// <summary>Дата окончания расмоттрения</summary>
        public DateTime? DateEnd { get; set; }
        /// <summary>Идентификатор регистратора документа</summary>
        [Required(ErrorMessage = "Укажите регистратора")]
        public int? RegistratorId { get; set; }
        //[Required(ErrorMessage = "Укажите Состояние")]
        /// <summary>
        /// Текущее состояние
        /// </summary>
        public int? StateCurrentId { get; set; }
        /// <summary>Идентификатор заключения</summary>
        public int? StateResultId { get; set; }

        /// <summary>Идентификатор отдела</summary>
        public int? DepatmentFromId { get; set; }
        /// <summary>Идентификатор отдела</summary>
        public int? DepatmentToId { get; set; }
        /// <summary>Идентификатор сотрудника</summary>
        public int? WorkerFromId { get; set; }
        /// <summary>Идентификатор сотрудника</summary>
        public int? WorkerToId { get; set; }

        /// <summary>Файлы, связанные с документом</summary>
        public List<FileDataModel> Files { get; set; }

        /// <summary>Подписи</summary>
        public List<DocumentSignModel> Signs { get; set; }

        /// <summary>Детализация документа</summary>
        public List<DocumentDetailPlaningModel> Details { get; set; }

        public string MemoAdv { get; set; }
        public DocumentPlaningModel()
        {
            Files = new List<FileDataModel>();
            Signs = new List<DocumentSignModel>();
            Details = new List<DocumentDetailPlaningModel>();
            this.Date = DateTime.Now;
        }

        public override void LoadFromTemplate(Document template)
        {
            base.LoadFromTemplate(template);

            DocumentPlan doc = new DocumentPlan { Workarea = WADataProvider.WA };
            doc.Load(template.Id);
            //doc.RegistratorId
            PlanKindId = doc.PlanKindId;
            ImportanceId = doc.ImportanceId;

            
        }
        public DocumentPlan ToObject(Workarea workarea)
        {
            DocumentPlan doc = new DocumentPlan { Workarea = WADataProvider.WA };
            doc.Load(Id);
            doc.StateId = StateId;
            if (IsReadOnly)
                doc.FlagsValue |= FlagValue.FLAGREADONLY;
            else
                doc.FlagsValue &= ~FlagValue.FLAGREADONLY;

            if (doc.Id == 0)
            {
                //new
                doc.IsNew = true;
                doc.StateId = State.STATEACTIVE;
                UserName = HttpContext.Current.User.Identity.Name;
            }
            if (doc.Document == null)
                doc.Document = new Document();// {Workarea = workarea, StateId = State.STATEACTIVE,UserName = HttpContext.Current.User.Identity.Name};

            ToObject(doc.Document);

            doc.Kind = KindId;
            doc.ImportanceId = ImportanceId.HasValue ? ImportanceId.Value : 0;
            doc.PlanKindId = PlanKindId.HasValue ? PlanKindId.Value : 0;
            doc.DateStart = DateStart;
            doc.DateEnd = DateEnd;
            doc.RegistratorId = RegistratorId.HasValue ? RegistratorId.Value : 0;
            doc.StateCurrentId = StateCurrentId.HasValue ? StateCurrentId.Value : 0;
            doc.StateResultId = StateResultId.HasValue ? StateResultId.Value : 0;
            doc.DepatmentFromId = DepatmentFromId.HasValue ? DepatmentFromId.Value : 0;
            doc.DepatmentToId = DepatmentToId.HasValue ? DepatmentToId.Value : 0;
            doc.WorkerFromId = WorkerFromId.HasValue ? WorkerFromId.Value : 0;
            doc.WorkerToId = WorkerToId.HasValue ? WorkerToId.Value : 0;
            doc.DateregionId = DateregionId.HasValue ? DateregionId.Value : 0;
            
            return doc;
        }

        public static DocumentPlaningModel ConvertToModel(DocumentPlan value)
        {
            DocumentPlaningModel res = new DocumentPlaningModel();
            ConvertToModel(value.Document, res);
            res.ImportanceId = value.ImportanceId == 0 ? (int?)null : value.ImportanceId;
            res.PlanKindId = value.PlanKindId == 0 ? (int?)null : value.PlanKindId;
            res.DateStart = value.DateStart;
            res.DateEnd = value.DateEnd;
            res.StateCurrentId = value.StateCurrentId;
            res.StateResultId = value.StateResultId;
            res.DepatmentFromId = value.DepatmentFromId;
            res.DepatmentToId = value.DepatmentToId;
            res.WorkerFromId = value.WorkerFromId;
            res.WorkerToId = value.WorkerToId;
            res.DateregionId = value.DateregionId;

            res.RegistratorId = value.RegistratorId == 0 ? (int?)null : value.RegistratorId;
            res.Files = value.Document.GetLinkedFiles().Where(s => s.StateId != State.STATEDELETED).Select(s => FileDataModel.ConvertToModel(s.Right)).ToList();
            res.Signs = value.Document.Signs().Select(DocumentSignModel.ConvertToModel).ToList();
            res.Details = value.Details.Select(DocumentDetailPlaningModel.ConvertToModel).ToList();
            
            return res;
        }

        /// <summary>
        /// Документ по идентификатору
        /// </summary>
        public static new DocumentPlaningModel GetObject(int id)
        {
            DocumentPlan obj = WADataProvider.WA.GetObject<DocumentPlan>(id);
            return ConvertToModel(obj);
        }

        public void SaveFiles()
        {
            DocumentPlan documentContract = new DocumentPlan { Workarea = WADataProvider.WA };
            documentContract.Load(Id);
            List<IChainAdvanced<Document, FileData>> fileLinks = documentContract.Document.GetLinkedFiles().Where(s => s.StateId != State.STATEDELETED).ToList();

            //Insert
            foreach (var fileModel in Files.Where(s => s.Id == 0))
            {
                //Данные
                FileData fileData = fileModel.ToObject(WADataProvider.WA);
                fileData.Save();

                //Связь
                ChainAdvanced<Document, FileData> link =
                    new ChainAdvanced<Document, FileData>(documentContract.Document) { Right = fileData, StateId = State.STATEACTIVE, KindId = 13 };
                link.Save();

                //Добавление в иерархию
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("CONTRACTSFILES");
                h.ContentAdd(fileData);
            }

            //Update
            foreach (var fileModel in Files.Where(s => s.Id != 0 && s.IsChanged))
            {
                FileData fileData = fileModel.ToObject(WADataProvider.WA);
                fileData.Save();
            }

            //Дополнительные коды
            foreach (FileDataModel fileModel in Files.Where(s => s.IsChanged))
            {
                fileModel.SaveCodes();
            }

            //Delete
            foreach (var link in fileLinks.Where(link => !Files.Exists(s => s.Id == link.Right.Id)))
            {
                link.StateId = State.STATEDELETED;
                link.Save();
            }
        }

        public static void CreateCopy(int id)
        {
            if (id == 0)
                return;
            DocumentPlan obj = WADataProvider.WA.Cashe.GetCasheData<DocumentPlan>().Item(id);
            DocumentPlan newObj = DocumentPlan.CreateCopy(obj);
            newObj.Document.Name += " (копия)";
            newObj.Save();
        }
    }
}