using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Admins.Models
{
    /// <summary>
    /// Модель документа настройки раздела "Управление финансами"
    /// </summary>
    public class DocumentConfigFinancesModel : DocumentModel
    {
        public DocumentConfigFinancesModel()
        {
            Config = new ConfigFinances();
            Config.Reset();
        }

        public ConfigFinances Config { get; set; }
        public override void Save()
        {
            DocumentFinanceConfig doc = ToObject(WADataProvider.WA);
            doc.Validate();
            DocumentData.SignDocumentOnSave(doc.Document);
            doc.Save();
            Id = doc.Id;
            this.SaveNotes<Document>(doc.Document.GetLinkedNotes());

            base.Save();
        }

        public DocumentFinanceConfig ToObject(Workarea workarea)
        {
            DocumentFinanceConfig doc = new DocumentFinanceConfig { Workarea = WADataProvider.WA };
            if (Id != 0)
            {
                doc.Load(Id);
            }
            doc.StateId = StateId;
            if (IsReadOnly)
                doc.FlagsValue |= FlagValue.FLAGREADONLY;
            else
                doc.FlagsValue &= ~FlagValue.FLAGREADONLY;

            UserName = HttpContext.Current.User.Identity.Name;
            if (doc.Id == 0)
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
            doc.Config = Config;
            ToObject(doc.Document);

            //Сохранение полей складов по docKind
            var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == doc.Document.KindId);

            return doc;
        }

        public static DocumentConfigFinancesModel ConvertToModel(DocumentFinanceConfig value)
        {
            DocumentConfigFinancesModel model = new DocumentConfigFinancesModel();
            ConvertToModel(value.Document, model);
            model.Config = value.Config;
            model.KindId = value.Kind;

            //Заполение значений расчетных счетов по docKind
            var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == value.Document.KindId);
            return model;
        }

        /// <summary>
        /// Документ по идентификатору
        /// </summary>
        public static new DocumentConfigFinancesModel GetObject(int id)
        {
            DocumentFinanceConfig obj = WADataProvider.WA.GetObject<DocumentFinanceConfig>(id);
            return ConvertToModel(obj);
        }

        public static void CreateCopy(int id)
        {
            if (id == 0)
                return;
            DocumentFinanceConfig obj = WADataProvider.WA.Cashe.GetCasheData<DocumentFinanceConfig>().Item(id);
            DocumentFinanceConfig newObj = DocumentFinanceConfig.CreateCopy(obj);
            newObj.Document.Name += " (копия)";
            newObj.Save();
        }
    }
}