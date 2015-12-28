using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using System.ComponentModel.DataAnnotations;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

//DocumentsWeb.Areas.Sales
namespace DocumentsWeb.Areas.Sales.Models
{
    /// <summary>
    /// Торговый документ
    /// </summary>
    public class DocumentSaleModel:DocumentModel
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
        /// <summary>Склад нашей компании</summary>
        public int? MainCompanyStoreId { get; set; }
        /// <summary>Склад клиента</summary>
        public int? MainClientStoreId { get; set; }
        /// <summary>Факт отгрузки</summary>
        public DateTime? DateShip { get; set; }

        /// <summary>Детализация документа</summary>
        public List<DocumentDetailSaleModel> Details { get; set; }

        public DocumentSaleModel()
        {
            Details=new List<DocumentDetailSaleModel>();
        }

        public override void Save()
        {
            DocumentSales doc = ToObject(WADataProvider.WA);
            doc.Validate();
            DocumentData.SignDocumentOnSave(doc.Document);
            doc.Save();
            Id = doc.Id;
            this.SaveNotes<Document>(doc.Document.GetLinkedNotes());

            base.Save();
        }

        public DocumentSales ToObject(Workarea workarea)
        {
            DocumentSales doc = new DocumentSales {Workarea = WADataProvider.WA};
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

            //Сохранение полей складов по docKind
            var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == doc.Document.KindId);
            doc.StoreFromId = StoreStoreField(docKind.AgentFirstFilterId);
            doc.StoreToId = StoreStoreField(docKind.AgentThirdFilterId);

            //Сохранение полей счетов по docKind
            doc.BankAccFromId = StoreAccountField(docKind.AgentFirstFilterId);
            doc.BankAccToId = StoreAccountField(docKind.AgentThirdFilterId);

            /////////////////////////////////
            doc.SupervisorId = SupervisorId ?? 0;
            doc.ManagerId = ManagerId ?? 0;
            doc.PrcNameId = PrcNameId;
            doc.DateShip = DateShip;
            
            doc.Details = Details.Select(s => s.ToObject(WADataProvider.WA, doc)).ToList();
            doc.Document.Summa = CalculateSum();
            
            return doc;
        }

        private int StoreStoreField(int agentFilterId)
        {
            switch (agentFilterId)
            {
                case 1:
                case 2:
                    return MainCompanyStoreId ?? 0;
                case 3:
                case 4:
                    return MainClientStoreId ?? 0;
                default:
                    return 0;
            }
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

        public static DocumentSaleModel ConvertToModel(DocumentSales value)
        {
            DocumentSaleModel model = new DocumentSaleModel();
            ConvertToModel(value.Document, model);
            model.KindId = value.Kind;
            model.SupervisorId = value.SupervisorId == 0 ? (int?)null : value.SupervisorId;
            model.SupervisorName = value.Supervisor == null ? "" : value.Supervisor.Name;
            model.ManagerId = value.ManagerId == 0 ? (int?)null : value.ManagerId;
            model.ManagerName = value.Manager == null ? "" : value.Manager.Name;
            model.PrcNameId = value.PrcNameId;
            model.DateShip = value.DateShip;

            //Заполение значений расчетных счетов по docKind
            var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == value.Document.KindId);
            model.MainCompanyAccountId = FillAccountField(docKind.AgentFirstFilterId, value);
            model.MainClientAccountId = FillAccountField(docKind.AgentThirdFilterId, value);

            //Заполение значений складов по docKind
            model.MainCompanyStoreId = FillStoreField(docKind.AgentFirstFilterId, value);
            model.MainClientStoreId = FillStoreField(docKind.AgentThirdFilterId, value);

            model.Details = value.Details.Where(s => !s.IsStateDeleted).Select(DocumentDetailSaleModel.ConvertToModel).ToList();
            return model;
        }

        private static int? FillAccountField(int agentFilterId, DocumentSales doc)
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

        private static int? FillStoreField(int agentFilterId, DocumentSales doc)
        {
            switch (agentFilterId)
            {
                case 1:
                case 2:
                    return doc.StoreFromId == 0 ? (int?)null : doc.StoreFromId;
                case 3:
                case 4:
                    return doc.StoreToId == 0 ? (int?)null : doc.StoreToId;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Документ по идентификатору
        /// </summary>
        public static new DocumentSaleModel GetObject(int id)
        {
            DocumentSales obj = WADataProvider.WA.GetObject<DocumentSales>(id);
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
            DocumentSales obj = WADataProvider.WA.Cashe.GetCasheData<DocumentSales>().Item(id);
            DocumentSales newObj = DocumentSales.CreateCopy(obj);
            newObj.Document.Name += " (копия)";
            newObj.Save();
        }

        public override void LoadFromTemplate(Document template)
        {
            base.LoadFromTemplate(template);
            if (template.MyCompanyId != -1)
            {
                DocumentSales tmpl = new DocumentSales() {Workarea = WADataProvider.WA};
                tmpl.Load(template.Id);
                ManagerId = tmpl.ManagerId == 0 ? (int?) null : tmpl.ManagerId;
                SupervisorId = tmpl.SupervisorId == 0 ? (int?) null : tmpl.SupervisorId;
                PrcNameId = tmpl.PrcNameId;

                //Заполение значений расчетных счетов по docKind
                var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == tmpl.Document.KindId);
                MainCompanyAccountId = FillAccountField(docKind.AgentFirstFilterId, tmpl);
                MainClientAccountId = FillAccountField(docKind.AgentThirdFilterId, tmpl);

                //Заполение значений складов по docKind
                MainCompanyStoreId = FillStoreField(docKind.AgentFirstFilterId, tmpl);
                MainClientStoreId = FillStoreField(docKind.AgentThirdFilterId, tmpl);
            }
        }
    }
}