using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Admins.Models
{
    /// <summary>
    /// Модель документа настройки раздела "Управление продажами"
    /// </summary>
    public class DocumentConfigSalesModel : DocumentModel
    {
        public DocumentConfigSalesModel()
        {
            Config = new ConfigSales();
            Config.Reset();
        }

        public ConfigSales Config { get; set; }
        public override void Save()
        {
            DocumentSalesConfig doc = ToObject(WADataProvider.WA);
            doc.Validate();
            DocumentData.SignDocumentOnSave(doc.Document);
            doc.Save();
            Id = doc.Id;
            this.SaveNotes<Document>(doc.Document.GetLinkedNotes());

            base.Save();
        }

        public DocumentSalesConfig ToObject(Workarea workarea)
        {
            DocumentSalesConfig doc = new DocumentSalesConfig { Workarea = WADataProvider.WA };
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

        public static DocumentConfigSalesModel ConvertToModel(DocumentSalesConfig value)
        {
            DocumentConfigSalesModel model = new DocumentConfigSalesModel();
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
        public static new DocumentConfigSalesModel GetObject(int id)
        {
            DocumentSalesConfig obj = WADataProvider.WA.GetObject<DocumentSalesConfig>(id);
            return ConvertToModel(obj);
        }

        public static void CreateCopy(int id)
        {
            if (id == 0)
                return;
            DocumentSalesConfig obj = WADataProvider.WA.Cashe.GetCasheData<DocumentSalesConfig>().Item(id);
            DocumentSalesConfig newObj = DocumentSalesConfig.CreateCopy(obj);
            newObj.Document.Name += " (копия)";
            newObj.Save();
        }
    }
}