using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using System.ComponentModel.DataAnnotations;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Prices.Models
{
    /// <summary>
    /// Цены
    /// </summary>
    public class DocumentPriceListModel : DocumentModel, IValidatableObject
    {
        /// <summary>
        /// Собственная проверка модели 
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(!MainCompanyDepatmentId.HasValue)
            {
                yield return new ValidationResult("Укажите предприятие!", new[] { GlobalPropertyNames.MainCompanyDepatmentId });
            }
            if (MainCompanyDepatmentId.HasValue && MainCompanyDepatmentId==0)
            {
                yield return new ValidationResult("Укажите предприятие!", new[] { GlobalPropertyNames.MainCompanyDepatmentId });
            }
            
            if (DateStart == DateTime.MinValue )
            {
                yield return new ValidationResult("Укажите дату начала действия цен!", new[] { GlobalPropertyNames.DateStart });
            }
            if (DateStart != DateTime.MinValue && DateStart < new DateTime(2000, 1, 1))
            {
                yield return new ValidationResult("Дату начала действия цен меньше 2000 года!", new[] { GlobalPropertyNames.DateStart });
            }
            if (!DateExpire.HasValue)
            {
                yield return new ValidationResult("Укажите дату окончания действия цен!", new[] { GlobalPropertyNames.DateExpire });
            }

            if(DateExpire.HasValue && DateExpire.Value<DateStart)
            {
                yield return new ValidationResult("Дата начала действия цен не может быть меньше даты окончания!", new[] {GlobalPropertyNames.DateStart, GlobalPropertyNames.DateExpire });
            }
            if(PrcNameId==0)
            {
                yield return new ValidationResult("Укажите прайс!", new[] { GlobalPropertyNames.PrcNameId });
            }
            if(KindId== DocumentPrices.KINDID_PRICEIND)
            {
                if (!MainClientDepatmentId.HasValue)
                {
                    yield return new ValidationResult("Укажите контрагента!", new[] { GlobalPropertyNames.MainClientDepatmentId });
                }
                if (MainClientDepatmentId.HasValue && MainClientDepatmentId == 0)
                {
                    yield return new ValidationResult("Укажите контрагента!", new[] { GlobalPropertyNames.MainClientDepatmentId });
                }
            }
            foreach (ValidationResult validationResult in ValidationByRules())
            {
                yield return validationResult;
            }

        }
        //[Required(ErrorMessage = "Укажите дату начала действия цен")]
        /// <summary>Дата начала действия цен</summary>
        public DateTime DateStart { get; set; }

        /// <summary>Дата окончания действия цен</summary>
        public DateTime? DateExpire { get; set; }

        /// <summary>Идентификатор ценовой политики</summary>
        public int PrcNameId { get; set; }

        /// <summary>Детализация документа</summary>
        public List<DocumentDetailPriceListModel> Details { get; set; }

        public DocumentPriceListModel()
        {
            Details=new List<DocumentDetailPriceListModel>();
            Date = DateTime.Today;
        }

        public override void Save()
        {
            DocumentPrices doc = ToObject(WADataProvider.WA);
            doc.UserName = WADataProvider.CurrentMembershipUser.UserName;
            doc.Document.UserOwnerId = WADataProvider.CurrentUser.Id;
            doc.Validate();
            doc.Save();
            Id = doc.Id;
            this.SaveNotes<Document>(doc.Document.GetLinkedNotes());
            base.Save();
        }

        public DocumentPrices ToObject(Workarea workarea)
        {
            DocumentPrices doc = new DocumentPrices {Workarea = WADataProvider.WA};
            if(Id!=0)
            {
                doc.Load(Id);
            }
            doc.StateId = StateId;
            if (IsReadOnly)
                doc.FlagsValue |= FlagValue.FLAGREADONLY;
            else
                doc.FlagsValue &= ~FlagValue.FLAGREADONLY;

            if(doc.Id==0)
            {
                doc.IsNew = true;
                doc.StateId = State.STATEACTIVE;
                UserName = HttpContext.Current.User.Identity.Name;
            }

            doc.DateStart = DateStart;
            doc.ExpireDate = DateExpire;
            if (doc.Document == null)
            {
                doc.Document = new Document();
                doc.Kind = KindId;
            }

            ToObject(doc.Document);

            /////////////////////////////////
            doc.PrcNameId = PrcNameId;
            doc.Details = Details.Select(s => s.ToObject(WADataProvider.WA, doc)).ToList();
            
            return doc;
        }

        public static DocumentPriceListModel ConvertToModel(DocumentPrices value)
        {
            DocumentPriceListModel res = new DocumentPriceListModel();
            ConvertToModel(value.Document, res);

            res.PrcNameId = value.PrcNameId;
            res.DateStart = value.DateStart;
            res.DateExpire = value.ExpireDate;
            res.Details = value.Details.Where(s => !s.IsStateDeleted).Select(DocumentDetailPriceListModel.ConvertToModel).ToList();

            return res;
        }

        /// <summary>
        /// Документ по идентификатору
        /// </summary>
        public static new DocumentPriceListModel GetObject(int id)
        {
            DocumentPrices obj = WADataProvider.WA.GetObject<DocumentPrices>(id);
            return ConvertToModel(obj);
        }

        public static void CreateCopy(int id)
        {
            if (id == 0)
                return;
            DocumentPrices obj = WADataProvider.WA.Cashe.GetCasheData<DocumentPrices>().Item(id);
            DocumentPrices newObj = DocumentPrices.CreateCopy(obj);
            newObj.Document.Name += " (копия)";
            newObj.Save();
        }
    }
}