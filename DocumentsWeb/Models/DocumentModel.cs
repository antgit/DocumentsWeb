using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Workflow.Activities.Rules;
using System.Xml.Serialization;
using BusinessObjects;
using BusinessObjects.Documents;
using System.ComponentModel.DataAnnotations;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Controllers;

namespace DocumentsWeb.Models
{
    /// <summary>
    /// Модель для списка документов
    /// </summary>
    public class DocumentModel:INotesOwner, BusinessObjects.Models.IModelData, IValidatableObject
    {
        /// <summary>Уникальный идентификатор модели</summary>
        public string ModelId { get; set; }
        /// <summary>Идентификатор пользователя</summary>
        public int ModelUserOwnerId { get; set; }
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        /// <summary>
        /// Ошибки
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, string> Errors
        {
            get { return _errors; }
            set { _errors = value; }
        }
        /// <summary>Идентификатор документа</summary>
        public int Id { get; set; }
        /// <summary>Примечания</summary>
        public List<NoteModel> Notes { get; set; }
        /// <summary>Имя пользователя</summary>
        public string UserName { get; set; }
        /// <summary>Идентификатор состояния</summary>
        public int StateId { get; set; }
        /// <summary>Только для чтения</summary>
        public bool IsReadOnly{get; set; }
        /// <summary>Текущее состояние</summary>
        public string StateName { get; set; }
        /// <summary>Наименование</summary>
        public string Name { get; set; }
        /// <summary>Полное наименование</summary>
        public string NameFull { get; set; }
        /// <summary>Тип</summary>
        public int KindId { get; set; }
        /// <summary>Дата</summary>
        [Required]
        public DateTime Date { get; set; }
        /// <summary> Время </summary>
        public DateTime Time { get; set; }
        /// <summary>Номер</summary>
        /// <remarks>Обратно в Number не переименовывать - глючит редактирование</remarks>
        public string DocNumber { get; set; }
        /// <summary>Примечание</summary>
        public string Memo { get; set; }
        /// <summary>Идентификатор валюты</summary>
        public int CurrencyId { get; set; }
        /// <summary>Цифровой код валюты</summary>
        public int CurrencyCode { get; set; }
        /// <summary>Дата последнего изменения документа</summary>
        public DateTime? DateModified { get; set; }
        public string AgFromCode { get; set; }
        public string AgDepartmentFromCode { get; set; }

        /// <summary>Подразделение</summary>
        [Required(ErrorMessage = "Укажите подразделение")]
        [Obsolete]
        public int AgentDepartmentFromId { get; set; }
        [Obsolete]
        public string AgentDepartmentFromName { get; set; }
        [Obsolete]
        public int AgentDepartmentToId { get; set; }
        [Obsolete]
        public string AgentDepartmentToName { get; set; }

        /// <summary>Id холдинга корреспондента "Наше предприятие"</summary>
        public int? MainCompanyId { get; set; }
        public string MainCompanyName { get; set; }
        /// <summary>Id холдинга корреспондента "Клиент"</summary>
        public int? MainClientId { get; set; }
        public string MainClientName { get; set; }

        /// <summary>Id корреспондента "Кто"</summary>
        public int? MainClientDepatmentId { get; set; } ////MainClientDepatmentId AgentFromId
        public string MainClientDepatmentName { get; set; }
        /// <summary>Корреспондент "Кто"</summary>
        [Obsolete]
        public string AgentFromName { get; set; }
        /// <summary>Id корреспондента "Кому"</summary>
        //[Required(ErrorMessage = "Укажите клиента")]
        //[Required(ErrorMessage = "Укажите наше предприятие")]
        public int? MainCompanyDepatmentId { get; set; } //MainCompanyDepatmentId AgentToId
        public string MainCompanyDepatmentName { get; set; }
        /// <summary>Корреспондент "Кому"</summary>
        [Obsolete]
        public string AgentToName { get; set; }
        /// <summary>Сумма</summary>
        public decimal DocSumma { get; set; }
        /// <summary>Сумма НДС</summary>
        public decimal DocSummaTax { get; set; }
        /// <summary>Код формы документа</summary>
        public string FormCode { get; set; }
        /// <summary>Компания</summary>
        public string MyCompanyName { get; set; }
        /// <summary>Идентификатор компании владельца документа </summary>
        public int MyCompanyId { get; set; }
        /// <summary>Код компании-владельца</summary>
        public string MyCompanyCode { get; set; }
        /// <summary>Идентификатор формы</summary>
        public int ProjectItemId { get; set; }
        /// <summary>Идентификатор папки</summary>
        public int FolderId { get; set; }
        public int FlagsValue { get; set; }
        /// <summary>Идентификатор шаблона</summary>
        public int TemplateId { get; set; }
        /// <summary>Идентификатор пользователя владельца</summary>
        public int UserOwnerId { get; set; }
        /// <summary>Ид родительского документа, из которого данный документ создан</summary>
        public int DocumentOwnerId { get; set; }
        /// <summary>
        /// Тип документа. 1 - входящий, 2 - исходящий, 3- внутренний
        /// </summary>
        public int DocType { get; set; }

        /// <summary>Список уже связаных документов с текущим, в которых текущий документ является родительским</summary>
        /// <returns></returns>
        public List<DocumentModel> GetChainsDocuments()
        {
           return Document.GetChainSourceList(WADataProvider.WA, Id, 20).Where(f => !f.IsStateDeleted).Select(ConvertToModel).ToList();
        }

        /// <summary>Список уже связаных документов с текущим, в которых текущий документ является подчиненным</summary>
        /// <returns></returns>
        public List<DocumentModel> GetChainsParentDocuments()
        {
            return Document.GetChainDestinationList(WADataProvider.WA, Id, 20).Where(f => !f.IsStateDeleted).Select(ConvertToModel).ToList();
        }
        /// <summary>
        /// Список отчетов для данного типа документов
        /// </summary>
        public void GetReports()
        {
            //WADataProvider.WA.GetReports<EntityDocument,Document> (MainDocument).Where(f => f.IsStateAllow)
        }

        public DocumentModel()
        {
            ModelId = Guid.NewGuid().ToString();
            Notes = new List<NoteModel>();
            DocumentOwnerId = 0;
            ModelUserOwnerId = WADataProvider.CurrentUser.Id;
        }

        /// <summary>
        /// Сохраняет модель в базу данных
        /// </summary>
        public virtual void Save()
        {
            //Если у этого документа есть родительский
            if(DocumentOwnerId!=0)
            {
                //Создаем связь
                Chain<Document> chain = new Chain<Document>()
                {
                    Workarea = WADataProvider.WA,
                    LeftId = DocumentOwnerId,
                    RightId = Id,
                    KindId = 20,
                    StateId = State.STATEACTIVE
                };
                chain.Save();
            }
        }

        /// <summary>
        /// Возвращет ссылку для редактирования данного документа
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string GetEditUrl(RequestContext context)
        {
            Document document = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(Id);
            Folder folder = WADataProvider.WA.Cashe.GetCasheData<Folder>().Item(document.FolderId);
            bool nds = folder.CodeFind.EndsWith("NDS");
            int formId = folder.FormId;
            string formCode = WADataProvider.WA.Cashe.GetCasheData<Library>().Item(formId).Code;
            object route = DocumentController.GetRouteValues(formCode, nds, id:Id);
            UrlHelper h=new UrlHelper(context);
            return h.Action("Edit", route);
        }

        public static DocumentModel ConvertToModel(Document value)
        {
            DocumentModel obj = new DocumentModel();
            ConvertToModel(value, obj);
            return obj;
        }

        /// <summary>
        /// Преобразование докумнта в модель. Может быть использовано для классов-потомков
        /// </summary>
        /// <param name="value">Документ</param>
        /// <param name="model">Модель документа</param>
        public static void ConvertToModel(Document value, DocumentModel model)
        {
            model.Id = value.Id;
            model.Name = value.Name;
            model.NameFull = value.NameFull;
            model.KindId = value.KindId;
            model.Date = value.Date;
            model.DocNumber = value.Number;
            //model.MainClientDepatmentId = value.AgentDepartmentFromId;
            model.AgentFromName = value.AgentFromName;
            //model.MainCompanyDepatmentId = value.AgentDepartmentToId;
            model.AgentToName = value.AgentToName;
            model.AgentDepartmentFromId = value.AgentDepartmentFromId;
            model.AgentDepartmentFromName = value.AgentDepartmentFromName;
            model.AgentDepartmentToId = value.AgentDepartmentToId;
            model.AgentDepartmentToName = value.AgentDepartmentToName;

            //Заполнение полей корреспондентов по docKind
            var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == value.KindId);
            model.MainCompanyId = FillAgentField(docKind.AgentFirstFilterId, value);
            model.MainCompanyDepatmentId = FillAgentField(docKind.AgentSecondFilterId, value);
            model.MainClientId = FillAgentField(docKind.AgentThirdFilterId, value);
            model.MainClientDepatmentId = FillAgentField(docKind.AgentFourthFilterId, value);
            
            model.CurrencyId = value.CurrencyId;
            model.CurrencyCode = value.Currency.IntCode;
            model.Memo = value.Memo;
            model.DocSumma = value.Summa;
            model.DocSummaTax = value.SummaTax;
            model.StateName = value.State.Name;
            model.UserName = value.UserName;
            model.FlagsValue = value.FlagsValue;
            model.IsReadOnly = (value.FlagsValue & FlagValue.FLAGREADONLY) == FlagValue.FLAGREADONLY;
            model.StateId = value.StateId;
            model.MyCompanyId = value.MyCompanyId;
            if (value.MyCompanyId != 0)
            {
                model.MyCompanyName = value.MyCompany.Name;
                model.MyCompanyCode = value.MyCompany.Code;
            }
            model.FolderId = value.FolderId;
            model.ProjectItemId = value.ProjectItemId;
            model.DateModified = value.DateModified;
            model.AgFromCode = (value.AgentFrom != null ? value.AgentFrom.Code : string.Empty);
            model.AgDepartmentFromCode = (value.AgentDepartmentFrom != null ? value.AgentDepartmentFrom.Code : string.Empty);
            model.FormCode = value.ProjectItemId != 0 ? value.ProjectItem.Code : string.Empty;
            model.Notes = NoteModel.GetCollection(value);
            model.Notes.ForEach(s => s.NoteOwner = model);
            model.TemplateId = value.TemplateId;
            model.UserOwnerId = value.UserOwnerId;
            model.DocType = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == model.KindId).CorrespondenceId;
        }

        private static int? FillAgentField(int agentFilterId, Document doc)
        {
            switch (agentFilterId)
            {
                case 1:
                    return doc.AgentFromId == 0 ? (int?)null : doc.AgentFromId;
                case 2:
                    return doc.AgentDepartmentFromId == 0 ? (int?)null : doc.AgentDepartmentFromId;
                case 3:
                    return doc.AgentToId == 0 ? (int?)null : doc.AgentToId;
                case 4:
                    return doc.AgentDepartmentToId == 0 ? (int?)null : doc.AgentDepartmentToId;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Документ по идентификатору
        /// </summary>
        public static DocumentModel GetObject(int id)
        {
            Document obj = WADataProvider.WA.GetObject<Document>(id);
            return ConvertToModel(obj);
        }

        public virtual void LoadFromTemplate(Document template)
        {
            Date = DateTime.Now;
            FolderId = template.FolderId;
            ProjectItemId = template.ProjectItemId;
            StateId = template.StateId;
            Name = template.Name;
            KindId = template.KindId;
            CurrencyId = template.CurrencyId;
            TemplateId = template.Id;
            DocType = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == KindId).CorrespondenceId;

            if (template.MyCompanyId != -1)
            {
                //Корреспонденты
                var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == template.KindId);
                MainCompanyId = FillAgentField(docKind.AgentFirstFilterId, template);
                MainCompanyDepatmentId = FillAgentField(docKind.AgentSecondFilterId, template);
                MainClientId = FillAgentField(docKind.AgentThirdFilterId, template);
                MainClientDepatmentId = FillAgentField(docKind.AgentFourthFilterId, template);
            }
        }

        

        /// <summary>
        /// Заполнение полей Документа 
        /// </summary>
        /// <param name="doc">Документ</param>
        public void ToObject(Document doc)
        {
            doc.Workarea = WADataProvider.WA;
            doc.StateId = State.STATEACTIVE;
            doc.UserName = HttpContext.Current.User.Identity.Name;
            doc.KindId = KindId;
            doc.UserName = HttpContext.Current.User.Identity.Name;
            doc.Name = Name;
            doc.Date = Date;
            doc.Number = DocNumber;
            doc.Memo = Memo;
            doc.Summa = DocSumma;
            doc.SummaTax = DocSummaTax;
            doc.FolderId = FolderId;
            doc.ProjectItemId = ProjectItemId;
            doc.TemplateId = TemplateId;

            if (IsReadOnly)
                doc.FlagsValue |= FlagValue.FLAGREADONLY;
            else
                doc.FlagsValue &= ~FlagValue.FLAGREADONLY;

            doc.MyCompanyId = MainCompanyDepatmentId ?? 0;
            doc.ClientId = MainClientDepatmentId ?? 0;

            if (doc.Id == 0)
            {
                doc.UserOwnerId = WADataProvider.CurrentUser.Id;
                //Автонумерация документа...
                if (string.IsNullOrEmpty(DocNumber) )
                {
                    Autonum an = Autonum.GetAutonumByDocumentKindUser(WADataProvider.WA, doc.KindId, WADataProvider.CurrentUser.Id);
                    if (an.Id==0 && MainCompanyDepatmentId.HasValue)
                        an = Autonum.GetAutonumByDocumentKind(WADataProvider.WA, doc.KindId, MainCompanyDepatmentId.Value);
                    if (an.Id == 0 && !string.IsNullOrEmpty(doc.MyCompany.Code))
                        an = Autonum.GetAutonumByDocumentKind(WADataProvider.WA, doc.KindId, doc.MyCompany.Code);
                    if (an.Id == 0 && MainCompanyDepatmentId.HasValue && MainCompanyDepatmentId.Value!=0)
                    {
                        an = new Autonum { Workarea = WADataProvider.WA, DocKind = doc.KindId, StateId = State.STATEACTIVE, MyCompanyId = MainCompanyDepatmentId.Value };
                    }
                    if (an != null)
                    {
                        string newDocnumber = string.Empty;
                        if(!string.IsNullOrEmpty(an.Prefix))
                        {
                            newDocnumber = an.Prefix + "-" + (an.Number + 1).ToString(CultureInfo.InvariantCulture);
                        }
                        if (!string.IsNullOrEmpty(an.Suffix))
                        {
                            newDocnumber = newDocnumber  + "-" + an.Suffix;
                        }
                        if (string.IsNullOrEmpty(newDocnumber))
                            doc.Number = (an.Number + 1).ToString(CultureInfo.InvariantCulture);
                        else
                            doc.Number = newDocnumber;
                        an.Number++;
                        an.Save();
                    }
                }
            }
            if (doc.UserOwnerId == 0)
                doc.UserOwnerId = WADataProvider.CurrentUser.Id;

            

            //Заполнение ид корреспондентов по ид подразделений
            MainClientId = MainClientDepatmentId != 0 ? WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(MainClientDepatmentId ?? 0).GetAgentHolding().Id : 0;  //MainClientId;
            MainCompanyId = MainCompanyDepatmentId != 0 ? WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(MainCompanyDepatmentId ?? 0).GetAgentHolding().Id : 0;  //MainClientId;

            var docKind = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == doc.KindId);

            //Заполнение полей документа по docKind
            doc.AgentFromId = StoreDocumentField(docKind.AgentFirstFilterId);
            doc.AgentDepartmentFromId = StoreDocumentField(docKind.AgentSecondFilterId);
            doc.AgentToId = StoreDocumentField(docKind.AgentThirdFilterId);
            doc.AgentDepartmentToId = StoreDocumentField(docKind.AgentFourthFilterId);

            //Заполенение имен корреспондентов и подразделений
            doc.AgentFromName = doc.AgentFromId == 0 ? "" : WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(doc.AgentFromId).Name;
            doc.AgentDepartmentFromName = doc.AgentDepartmentFromId == 0 ? "" : WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(doc.AgentDepartmentFromId).Name;
            doc.AgentToName = doc.AgentToId == 0 ? "" : WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(doc.AgentToId).Name;
            doc.AgentDepartmentToName = doc.AgentDepartmentToId == 0 ? "" : WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(doc.AgentDepartmentToId).Name;
        }

        private int StoreDocumentField(int agentFilterId)
        {
            switch (agentFilterId)
            {
                case 1:
                    return MainCompanyId ?? 0;
                case 2:
                    return MainCompanyDepatmentId ?? 0;
                case 3:
                    return MainClientId ?? 0;
                case 4:
                    return MainClientDepatmentId ?? 0;
                default:
                    return 0;
            }
        }

        public static void Remove(int id)
        {
            Document obj = WADataProvider.WA.GetObject<Document>(id);
            if (obj != null) obj.Remove();
        }

        public static void ChangeState(int id, int stateId)
        {
            Document obj = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(id);
            obj.ChangeState(stateId);
        }

        public static List<ClientModel> GetMyDepatmentsForDocumet(int? idSelected)
        {
            List<ClientModel> coll = ClientModel.GetMyDepatments();
            if (idSelected.HasValue && idSelected != 0)
            {
                if (!coll.Exists(s => s.Id == idSelected))
                {
                    ClientModel obj = ClientModel.GetObject(idSelected.Value);
                    coll.Add(obj);
                }
            }
            return coll.OrderBy(s=>s.Name).ToList();
        }

        public virtual IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(!MainClientDepatmentId.HasValue)
            {
                yield return new ValidationResult("Укажите контрагента!", new[] { GlobalPropertyNames.MainClientDepatmentId });
            }
            if(MainClientDepatmentId.HasValue && MainClientDepatmentId==0)
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
            
            foreach (ValidationResult validationResult in ValidationByRules())
            {
                yield return validationResult;
            }
        }

        /// <summary>
        /// Проверка на основе правил
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<ValidationResult> ValidationByRules()
        {
            Type targetType = this.GetType();
            string fulltypename = targetType.FullName;
            IEnumerable<Ruleset> collRules =
                WADataProvider.WA.GetCollection<Ruleset>().Where(
                    s => s.ActivityName == fulltypename && s.StateId == 1 && s.KindValue == Ruleset.KINDVALUE_WEBRULESET);
            //if (collRules == null) return true;

            foreach (Ruleset rule in collRules)
            {
                RuleSet ruleSetToValidate = rule.DeserializeRuleSet();
                if (ruleSetToValidate != null)
                {
                    RuleEngine engine = new RuleEngine(ruleSetToValidate, targetType);
                    engine.Execute(this);
                    if (this.Errors.Count > 0)
                    {
                        foreach (string k in this.Errors.Keys)
                        {
                            yield return new ValidationResult(Errors[k], new[] {k});
                        }
                    }
                }
            }
        }
    }
}