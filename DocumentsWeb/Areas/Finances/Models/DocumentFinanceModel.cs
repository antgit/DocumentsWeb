using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using System.ComponentModel.DataAnnotations;
using DocumentsWeb.Areas.Finances.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;
using System;

//DocumentsWeb.Areas.Finances
namespace DocumentsWeb.Areas.Finances.Models
{
    /// <summary>
    /// Торговый документ
    /// </summary>
    public class DocumentFinanceModel: DocumentModel
    {
        /// <summary>Банковский счет нашей компании</summary>
        public int? MainCompanyAccountId { get; set; }
        /// <summary>Банковский счет клиента</summary>
        public int? MainClientAccountId { get; set; }

        [Obsolete]
        public int? BankAccFromId { get; set; }
        [Obsolete]
        public int BankAccToId { get; set; }
        public int PaymentTypeId { get; set; }

        public DateTime? DatePay { get; set; }

        /// <summary>Детализация документа</summary>
        public List<DocumentDetailFinanceModel> Details { get; set; }

        public DocumentFinanceModel()
        {
            Details=new List<DocumentDetailFinanceModel>();
        }

        public override void Save()
        {
            DocumentFinance doc = ToObject(WADataProvider.WA);
            doc.Validate();
            DocumentData.SignDocumentOnSave(doc.Document);
            doc.Save();
            this.SaveNotes<Document>(doc.Document.GetLinkedNotes());
            base.Save();
        }

        public DocumentFinance ToObject(Workarea workarea)
        {
            DocumentFinance doc = new DocumentFinance {Workarea = WADataProvider.WA};
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

            if(doc.Id==0)
            {
                //new
                doc.IsNew = true;
                doc.StateId = State.STATEACTIVE;
                UserName = HttpContext.Current.User.Identity.Name;
                doc.Kind = KindId;
            }

            doc.PaymentTypeId = PaymentTypeId;

            if (doc.Document == null)
            {
                doc.Document = new Document();
                doc.Kind = KindId;
            }

            //Сохранение полей счетов по docKind
            var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == doc.Document.KindId);
            doc.AgFromBankAccId = StoreAccountField(docKind.AgentFirstFilterId);
            doc.AgToBankAccId = StoreAccountField(docKind.AgentThirdFilterId);

            ToObject(doc.Document);

            /////////////////////////////////
            
            doc.AgFromBankAccId = BankAccFromId == null ? 0 : (int)BankAccFromId;
            doc.AgToBankAccId = BankAccToId;
            
            
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

        public static DocumentFinanceModel ConvertToModel(DocumentFinance value)
        {
            DocumentFinanceModel model = new DocumentFinanceModel();
            ConvertToModel(value.Document, model);
            model.KindId = value.Kind;
            
            model.BankAccFromId = value.AgFromBankAccId;
            model.BankAccToId = value.AgToBankAccId;
            model.PaymentTypeId = value.PaymentTypeId;

            //Заполение значений расчетных счетов по docKind
            var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == value.Document.KindId);
            model.MainCompanyAccountId = FillAccountField(docKind.AgentFirstFilterId, value);
            model.MainClientAccountId = FillAccountField(docKind.AgentThirdFilterId, value);
            
            model.Details = value.Details.Where(s => !s.IsStateDeleted).Select(DocumentDetailFinanceModel.ConvertToModel).ToList();

            return model;
        }

        private static int? FillAccountField(int agentFilterId, DocumentFinance doc)
        {
            switch (agentFilterId)
            {
                case 1:
                case 2:
                    return doc.AgFromBankAccId == 0 ? (int?)null : doc.AgFromBankAccId;
                case 3:
                case 4:
                    return doc.AgToBankAccId == 0 ? (int?)null : doc.AgToBankAccId;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Документ по идентификатору
        /// </summary>
        public static new DocumentFinanceModel GetObject(int id)
        {
            DocumentFinance obj = WADataProvider.WA.GetObject<DocumentFinance>(id);
            return ConvertToModel(obj);
        }    
   
        public decimal CalculateSum()
        {
            return Details.Where(s => s.StateId != State.STATEDELETED).Sum(s => s.Summa);
        }

        public static void CreateCopy(int id)
        {
            if (id == 0)
                return;
            DocumentFinance obj = WADataProvider.WA.Cashe.GetCasheData<DocumentFinance>().Item(id);
            DocumentFinance newObj = DocumentFinance.CreateCopy(obj);
            newObj.Document.Name += " (копия)";
            newObj.Save();
        }
    }
}