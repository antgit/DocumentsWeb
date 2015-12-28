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
namespace DocumentsWeb.Areas.Taxes.Models
{
    /// <summary>
    /// Торговый документ
    /// </summary>
    public class DocumentTaxModel:DocumentModel
    {
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!MainClientDepatmentId.HasValue)
            {
                yield return new ValidationResult("Укажите контрагента!", new[] { GlobalPropertyNames.MainClientDepatmentId });
            }
            if (MainClientDepatmentId.HasValue && MainClientDepatmentId == 0)
            {
                yield return new ValidationResult("Укажите контрагента!", new[] { GlobalPropertyNames.MainClientDepatmentId });
            }
            if (!MainCompanyDepatmentId.HasValue)
            {
                yield return new ValidationResult("Укажите наше предприятие!", new[] { GlobalPropertyNames.MainCompanyDepatmentId });
            }
            if (MainCompanyDepatmentId.HasValue && MainCompanyDepatmentId == 0)
            {
                yield return new ValidationResult("Укажите наше предприятие!", new[] { GlobalPropertyNames.MainCompanyDepatmentId });
            }

            if (string.IsNullOrEmpty(DeliveryCondition))
            {
                yield return new ValidationResult("Укажите усливия поставки!", new[] { GlobalPropertyNames.DeliveryCondition });
            }

            if (string.IsNullOrEmpty(DogovorNo))
            {
                yield return new ValidationResult("Укажите номер договора!", new[] { GlobalPropertyNames.DogovorNo });
            }

            if (string.IsNullOrEmpty(PaymentMethod))
            {
                yield return new ValidationResult("Укажите форму рассчетов!", new[] { GlobalPropertyNames.PaymentMethod });
            }

            if (!DogovorDate.HasValue)
            {
                yield return new ValidationResult("Укажите дату договора!", new[] { GlobalPropertyNames.DogovorDate });
            }

            if (DogovorDate.HasValue && (DogovorDate.Value == DateTime.MinValue || DogovorDate.Value>DateTime.MaxValue))
            {
                yield return new ValidationResult("Дата договора в запрещенном диапазоне!", new[] { GlobalPropertyNames.DogovorDate});
            }

            if (DogovorDate.HasValue && DogovorDate.Value> Date )
            {
                yield return new ValidationResult("Дата договора больше даты документа!", new[] { GlobalPropertyNames.DogovorDate });
            }

            foreach (ValidationResult validationResult in ValidationByRules())
            {
                yield return validationResult;
            }

        }

        /// <summary>Идентификатор условия поставки</summary>
        //[Required(ErrorMessage = "Укажите усливия поставки")]
        public string DeliveryCondition { get; set; }

        /// <summary>Идентификатор формы проведенных расчетов</summary>
        //[Required(ErrorMessage = "Укажите форму рассчетов")]
        public string PaymentMethod { get; set; }

        /// <summary>Номер договора</summary>
        //[Required(ErrorMessage = "Укажите номер договора")]
        public string DogovorNo { get; set; }

        /// <summary>Дата договора</summary>
        //[Required(ErrorMessage = "Укажите дату договора", )]
        public DateTime? DogovorDate { get; set; }
        
        /// <summary>Детализация документа</summary>
        public List<DocumentDetailTaxModel> Details { get; set; }

        public DocumentTaxModel()
        {
            Details=new List<DocumentDetailTaxModel>();
        }

        public override void Save()
        {
            DocumentTaxes doc = ToObject(WADataProvider.WA);           
            doc.Validate();
            DocumentData.SignDocumentOnSave(doc.Document);
            doc.Save();
            Id = doc.Id;
            this.SaveNotes(doc.Document.GetLinkedNotes());
            base.Save();
        }

        public DocumentTaxes ToObject(Workarea workarea)
        {
            DocumentTaxes doc = new DocumentTaxes { Workarea = WADataProvider.WA };
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

            //if (WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == KindId).CorrespondenceId == 2)
            //    SwapAgents();//Если документ иходящий то меняем местами корреспондентов

            doc.Date = Date;
            ToObject(doc.Document);
            
            /////////////////////////////////
            doc.DeliveryCondition=DeliveryCondition;
            doc.PaymentMethod=PaymentMethod;
            doc.DogovorNo = DogovorNo;
            doc.DogovorDate = DogovorDate;
            
            doc.Details = Details.Select(s => s.ToObject(WADataProvider.WA, doc)).ToList();
            doc.Document.Summa = CalculateSum();
            
            return doc;
        }

        public static DocumentTaxModel ConvertToModel(DocumentTaxes value)
        {
            DocumentTaxModel res = new DocumentTaxModel();
            ConvertToModel(value.Document, res);
            res.KindId = value.Kind;

            res.DeliveryCondition = value.DeliveryCondition;
            res.PaymentMethod = value.PaymentMethod;
            res.DogovorNo = value.DogovorNo;
            res.DogovorDate = value.DogovorDate;

            res.Details = value.Details.Where(s => !s.IsStateDeleted).Select(DocumentDetailTaxModel.ConvertToModel).ToList();

            return res;
        }

        /// <summary>
        /// Документ по идентификатору
        /// </summary>
        public static new DocumentTaxModel GetObject(int id)
        {
            DocumentTaxes obj = WADataProvider.WA.GetObject<DocumentTaxes>(id);
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
            DocumentTaxes obj = WADataProvider.WA.Cashe.GetCasheData<DocumentTaxes>().Item(id);
            DocumentTaxes newObj = DocumentTaxes.CreateCopy(obj);
            newObj.Document.Name += " (копия)";
            newObj.Save();
        }

        public override void LoadFromTemplate(Document template)
        {
            base.LoadFromTemplate(template);
            if (template.MyCompanyId != -1)
            {
                DocumentTaxes tmpl = new DocumentTaxes() { Workarea = WADataProvider.WA };
                tmpl.Load(template.Id);

                DeliveryCondition = tmpl.DeliveryCondition;
                PaymentMethod = tmpl.PaymentMethod;
                DogovorNo = tmpl.DogovorNo;
                DogovorDate = tmpl.DogovorDate;
            }
        }
    }
}