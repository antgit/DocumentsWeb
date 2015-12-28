using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using System.ComponentModel.DataAnnotations;
using DocumentsWeb.Areas.Services.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;
using System;

//DocumentsWeb.Areas.Services
namespace DocumentsWeb.Areas.Services.Models
{
    /// <summary>
    /// Торговый документ
    /// </summary>
    public class DocumentServiceModel: DocumentModel
    {
        /// <summary>Идентификатор старшего менеджера</summary>
        [Required(ErrorMessage = "Укажите супервайзера")]
        public int? SupervisorId{ get; set; }
        public string SupervisorName { get; set; }
        /// <summary>Идентификатор менеджера</summary>
        [Required(ErrorMessage = "Укажите менеджера")]
        public int? ManagerId { get; set; }
        public string ManagerName { get; set; }
        /// <summary>Идентификатор ценовой политики</summary>
        [Required(ErrorMessage = "Укажите прайс")]
        public int PrcNameId { get; set; }

        /// <summary>Банковский счет нашей компании</summary>
        public int? MainCompanyAccountId { get; set; }
        /// <summary>Банковский счет клиента</summary>
        public int? MainClientAccountId { get; set; }

        public DateTime? DatePay { get; set; }

        /// <summary>Детализация документа</summary>
        public List<DocumentDetailServiceModel> Details { get; set; }

        public DocumentServiceModel()
        {
            Details=new List<DocumentDetailServiceModel>();
        }

        public override void Save()
        {
            DocumentService doc = ToObject(WADataProvider.WA);
            doc.Validate();
            DocumentData.SignDocumentOnSave(doc.Document);
            doc.UserName = WADataProvider.CurrentUserName;
            doc.Save();
            Id = doc.Id;
            this.SaveNotes<Document>(doc.Document.GetLinkedNotes());
            base.Save();
        }

        public DocumentService ToObject(Workarea workarea)
        {
            DocumentService doc = new DocumentService {Workarea = WADataProvider.WA};
            if (Id != 0)
            {
                doc.Load(Id);
            }
            //doc.Load(Id);
            doc.StateId = StateId;
            if (IsReadOnly)
                doc.FlagsValue |= FlagValue.FLAGREADONLY;
            else
                doc.FlagsValue &= ~FlagValue.FLAGREADONLY;

            UserName = HttpContext.Current.User.Identity.Name;
            if(doc.Id==0)
            {
                //new
                doc.IsNew = true;
                doc.StateId = State.STATEACTIVE;
                doc.Kind = KindId;
            }

            if (doc.Document == null)
            {
                doc.Document = new Document();
                doc.Kind = KindId;
            }

            ToObject(doc.Document);

            /////////////////////////////////
            doc.SupervisorId = SupervisorId ?? 0;
            doc.ManagerId = ManagerId ?? 0;
            doc.PriceId = PrcNameId;
            //doc.PrcNameId = PrcNameId;
            //doc.BankAccFromId = BankAccFromId == null ? 0 : (int)BankAccFromId;
            //doc.BankAccToId = BankAccToId;
            doc.DatePay = DatePay;
            //doc.StoreFromId = StoreFromId;
            //doc.StoreToId = StoreToId;

            //Сохранение полей счетов по docKind
            var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == doc.Document.KindId);
            doc.BankAccFromId = StoreAccountField(docKind.AgentFirstFilterId);
            doc.BankAccToId = StoreAccountField(docKind.AgentThirdFilterId);
            
            doc.Details = Details.Select(s => s.ToObject(WADataProvider.WA, doc)).ToList();
            doc.Document.Summa = CalculateSum();
            
            return doc;
        }

        private int StoreAccountField(int agentFilterId)
        {
            switch (agentFilterId)
            {
                case 1:
                case 2:
                    return MainCompanyAccountId ?? 0;
                case 3:
                case 4:
                    return MainClientAccountId ?? 0;
                default:
                    return 0;
            }
        }

        public static DocumentServiceModel ConvertToModel(DocumentService value)
        {
            DocumentServiceModel model = new DocumentServiceModel();
            ConvertToModel(value.Document, model);
            model.KindId = value.Kind;
            model.SupervisorId = value.SupervisorId == 0 ? (int?)null : value.SupervisorId;
            model.SupervisorName = value.Supervisor == null ? "" : value.Supervisor.Name;
            model.ManagerId = value.ManagerId == 0 ? (int?)null : value.ManagerId;
            model.ManagerName = value.Manager == null ? "" : value.Manager.Name;
            model.PrcNameId = value.PriceId;
            model.DatePay = value.DatePay;

            //Заполение значений расчетных счетов по docKind
            var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == value.Document.KindId);
            model.MainCompanyAccountId = FillAccountField(docKind.AgentFirstFilterId, value);
            model.MainClientAccountId = FillAccountField(docKind.AgentThirdFilterId, value);

            //res.StoreFromId = value.StoreFromId;
            //res.StoreToId = value.StoreToId;
            model.Details = value.Details.Where(s => !s.IsStateDeleted).Select(DocumentDetailServiceModel.ConvertToModel).ToList();

            return model;
        }

        private static int? FillAccountField(int agentFilterId, DocumentService doc)
        {
            switch (agentFilterId)
            {
                case 1:
                case 2:
                    return doc.BankAccFromId == 0 ? (int?)null : doc.BankAccFromId;
                case 3:
                case 4:
                    return doc.BankAccToId == 0 ? (int?)null : doc.BankAccToId;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Документ по идентификатору
        /// </summary>
        public static new DocumentServiceModel GetObject(int id)
        {
            DocumentService obj = WADataProvider.WA.GetObject<DocumentService>(id);
            return ConvertToModel(obj);
        }    
   
        public decimal CalculateSum()
        {
            return Details.Where(s => s.StateId != State.STATEDELETED).Sum(s => s.Summa);
        }

        //public override void SwapAgents()
        //{
        //    base.SwapAgents();

        //    /*int tmp = StoreFromId;
        //    StoreFromId = StoreToId;
        //    StoreToId = tmp;*/

        //    int tmp = BankAccFromId == null ? 0 : (int)BankAccFromId;
        //    BankAccFromId = BankAccToId;
        //    BankAccToId = tmp;
        //}

        public static void CreateCopy(int id)
        {
            if (id == 0)
                return;
            DocumentService obj = WADataProvider.WA.Cashe.GetCasheData<DocumentService>().Item(id);
            DocumentService newObj = DocumentService.CreateCopy(obj);
            newObj.Document.Name += " (копия)";
            newObj.Save();
        }

        public override void LoadFromTemplate(Document template)
        {
            base.LoadFromTemplate(template);

            if (template.MyCompanyId != -1)
            {
                DocumentService tmpl = new DocumentService() {Workarea = WADataProvider.WA};
                tmpl.Load(template.Id);
                ManagerId = tmpl.ManagerId == 0 ? (int?) null : tmpl.ManagerId;
                SupervisorId = tmpl.SupervisorId == 0 ? (int?) null : tmpl.SupervisorId;
                PrcNameId = tmpl.PriceId;

                //Заполение значений расчетных счетов по docKind
                var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == tmpl.Document.KindId);
                MainCompanyAccountId = FillAccountField(docKind.AgentFirstFilterId, tmpl);
                MainClientAccountId = FillAccountField(docKind.AgentThirdFilterId, tmpl);
            }
        }
    }
}