using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using System.ComponentModel.DataAnnotations;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Contracts.Models
{
    /// <summary>
    /// Договор
    /// </summary>
    public class DocumentContractModel : DocumentModel, IFileOwner
    {
        /// <summary>Идентификатор важности договора</summary>
        public int? ImportanceId { get; set; }
        /// <summary>Идентификатор типа договора</summary>
        [Required(ErrorMessage = "Укажите тип договора")]
        public int? ContractKindId { get; set; }
        /// <summary>Дата начала договора</summary>
        public DateTime? DateStart { get; set; }
        /// <summary>Дата окончания договора</summary>
        public DateTime? DateEnd { get; set; }
        /// <summary>Идентификатор регистратора документа</summary>
        [Required(ErrorMessage = "Укажите регистратора")]
        public int? RegistratorId { get; set; }
        /// <summary>Отправитель</summary>
        //[Required(ErrorMessage = "Укажите отправителя")]
        public int? SenderId { get; set; }
        /// <summary>Получатель</summary>
        //[Required(ErrorMessage = "Укажите получателя")]
        public int? RecipientId { get; set; }
        //[Required(ErrorMessage = "Укажите Состояние")]
        public int? StateCurrentId { get; set; }
        /// <summary>Файлы, связанные с документом</summary>
        public List<FileDataModel> Files { get; set; }

        /// <summary>Подписи</summary>
        public List<DocumentSignModel> Signs { get; set; }

        /// <summary>Детализация документа</summary>
        public List<DocumentDetailContractModel> Details { get; set; }

        public string MemoAdv { get; set; }
        public DocumentContractModel()
        {
            Files = new List<FileDataModel>();
            Signs = new List<DocumentSignModel>();
            Details = new List<DocumentDetailContractModel>();
            Date = DateTime.Now;
        }

        public override void Save()
        {
            DocumentContract doc = ToObject(WADataProvider.WA);
            doc.UserName = WADataProvider.CurrentMembershipUser.UserName;
            doc.Document.UserOwnerId = WADataProvider.CurrentUser.Id;
            doc.Validate();
            DocumentData.SignDocumentOnSave(doc.Document);
            doc.Save();

            //TODO:Убрать цикл
            foreach (DocumentSignModel sm in Signs)
            {
                sm.OwnerId = doc.Id;
                DocumentSign sign = sm.ToObject();
                sign.Id = sm.Id;
                sign.Save();
                sm.Id = sign.Id;
            }

            Id = doc.Id;
            this.SaveFiles();
            this.SaveNotes<Document>(doc.Document.GetLinkedNotes());
            Id = doc.Id;
            base.Save();
        }

        public override void LoadFromTemplate(Document template)
        {
            base.LoadFromTemplate(template);

            DocumentContract doc = new DocumentContract { Workarea = WADataProvider.WA };
            doc.Load(template.Id);
            //doc.RegistratorId
            ContractKindId = doc.ContractKindId;
            ImportanceId = doc.ImportanceId;
            DateStart = doc.DateStart;
            DateEnd = doc.DateEnd;
            RegistratorId = doc.RegistratorId;
            SenderId = doc.Contractors().Exists(s => s.Kind == 0) ? doc.Contractors().FirstOrDefault(s => s.Kind == 0).Id : (int?)null;
            RecipientId = doc.Contractors().Exists(s => s.Kind == 1) ? doc.Contractors().FirstOrDefault(s => s.Kind == 1).Id : (int?)null;
            StateCurrentId = doc.StateCurrentId;
        }
        public DocumentContract ToObject(Workarea workarea)
        {
            DocumentContract doc = new DocumentContract { Workarea = WADataProvider.WA };
            doc.Load(Id);
            doc.StateId = StateId;
            if (IsReadOnly)
                doc.FlagsValue |= FlagValue.FLAGREADONLY;
            else
                doc.FlagsValue &= ~FlagValue.FLAGREADONLY;

            if(doc.Id==0)
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
            doc.ContractKindId = ContractKindId.HasValue ? ContractKindId.Value : 0;
            doc.DateStart = DateStart;
            doc.DateEnd = DateEnd;
            doc.RegistratorId = RegistratorId.HasValue ? RegistratorId.Value : 0;
            doc.StateCurrentId = StateCurrentId.HasValue ? StateCurrentId.Value : 0;
            if (KindId == DocumentContract.KINDID_OFFICIALNOTE)
            {
                doc.Document.GetStringData().Memo = MemoAdv;
                if (SenderId.HasValue)
                {
                    DocumentContractor dc = doc.NewContractorRow();
                    dc.Kind = 0;
                    dc.AgentId = SenderId.Value;
                }
                
                if (RecipientId.HasValue)
                {
                    DocumentContractor dc = doc.NewContractorRow();
                    dc.Kind = 1;
                    dc.AgentId = RecipientId.Value;
                }
            }
            return doc;
        }

        public static DocumentContractModel ConvertToModel(DocumentContract value)
        {
            DocumentContractModel res = new DocumentContractModel();
            ConvertToModel(value.Document, res);
            res.ImportanceId = value.ImportanceId == 0 ? (int?) null : value.ImportanceId;
            res.ContractKindId = value.ContractKindId == 0 ? (int?)null : value.ContractKindId;
            res.DateStart = value.DateStart;
            res.DateEnd = value.DateEnd;
            res.StateCurrentId = value.StateCurrentId;
            res.RegistratorId = value.RegistratorId == 0 ? (int?)null : value.RegistratorId;
            res.SenderId = value.Contractors().Exists(s => s.Kind == 0) ? value.Contractors().FirstOrDefault(s => s.Kind == 0).Id : (int?)null;
            res.RecipientId = value.Contractors().Exists(s => s.Kind == 1) ? value.Contractors().FirstOrDefault(s => s.Kind == 1).Id : (int?)null;
            res.Files = value.Document.GetLinkedFiles().Where(s => s.StateId != State.STATEDELETED).Select(s => FileDataModel.ConvertToModel(s.Right)).ToList();
            res.Signs = value.Document.Signs().Select(DocumentSignModel.ConvertToModel).ToList();
            res.Details = value.Details.Select(DocumentDetailContractModel.ConvertToModel).ToList();
            if(value.Document.KindId == DocumentContract.KINDID_OFFICIALNOTE)
            {
                res.MemoAdv = value.Document.GetStringData().Memo;
            }
            return res;
        }

        /// <summary>
        /// Документ по идентификатору
        /// </summary>
        public static new DocumentContractModel GetObject(int id)
        {
            DocumentContract obj = WADataProvider.WA.GetObject<DocumentContract>(id);
            return ConvertToModel(obj);
        } 
      
        public void SaveFiles()
        {
            DocumentContract documentContract = new DocumentContract {Workarea = WADataProvider.WA};
            documentContract.Load(Id);
            List<IChainAdvanced<Document, FileData>> fileLinks = documentContract.Document.GetLinkedFiles().Where(s=>s.StateId!=State.STATEDELETED).ToList();

            //Insert
            foreach (var fileModel in Files.Where(s=>s.Id==0))
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
            foreach (FileDataModel fileModel in Files.Where(s=>s.IsChanged))
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
            DocumentContract obj = WADataProvider.WA.Cashe.GetCasheData<DocumentContract>().Item(id);
            DocumentContract newObj = DocumentContract.CreateCopy(obj);
            newObj.Document.Name += " (копия)";
            newObj.Save();
        }
    }
}