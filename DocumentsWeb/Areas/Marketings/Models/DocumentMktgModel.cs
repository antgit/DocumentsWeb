using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects.Documents;
using BusinessObjects;
using System.ComponentModel.DataAnnotations;
using DocumentsWeb.Areas.Sales.Models;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.Analitics.Models;
using System.Globalization;

namespace DocumentsWeb.Areas.Marketings.Models
{
    /// <summary>
    /// Модель маркетинговой анкеты
    /// </summary>
    public class DocumentMktgModel: DocumentModel
    {
        // Контактное лицо
        public string ContactFace { get; set; }

        // Должность
        [Required(ErrorMessage = "Укажите должность")]
        public int WorkpostId { get; set; }
        public string WorkpostName { get; set; }

        // Контактный телефон
        [Required(ErrorMessage = "Укажите телефон")]
        public string Telephone { get; set; }
        
        // Тип тогровой точки
        [Required(ErrorMessage = "Укажите тип торговой точки")]
        public int TPTypeId { get; set; }
        public string TPtypeName { get; set; }

        // Площадь торговой точки
        [Required(ErrorMessage = "Укажите площадь торговой точки")]
        public int TPAreaId { get; set; }
        public string TPAreaName { get; set; }

        // Расположение торговой точки
        [Required(ErrorMessage = "Укажите размещение торговой точки")]
        public int TPPositionId { get; set; }
        public string TPPositionName { get; set; }

        // График работы
        [Required(ErrorMessage = "Укажите график работы")]
        public int TimePeriodId { get; set; }
        public string TimePeriodName { get; set; }

        // График перерывов
        [Required(ErrorMessage = "Укажите график перерыва")]
        public int BreakPeriodId { get; set; }
        public string BreakPeriodName { get; set; }

        [Required(ErrorMessage = "Укажите клиента")]
        [AgentAddressValidator(ErrorMessage = "Адрес клиента указан с ошибкой")]
        public int MainCompanyId { get; set; }
        /*public int? MainCompanyDepatmentId { get; set; }
        public int? MainClientId { get; set; }
        public int? MainClientDepatmentId { get; set; }*/

        /// <summary>Идентификатор менеджера</summary>
        [Required(ErrorMessage = "Укажите менеджера")]
        public int ManagerId { get; set; }
        public string ManagerName { get; set; }

        /// <summary>Идентификатор старшего менеджера</summary>
        [Required(ErrorMessage = "Укажите супервайзера")]
        public int SupervisorId { get; set; }
        public string SupervisorName { get; set; }

        /// <summary>
        /// Мнимый адрес клиента (используется только для визуализации)
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Общее состояние магазина
        /// </summary>
        public decimal TPCondition { get; set; }

        /// <summary>
        /// Группы товары по которым готова сотрудничать торговая точка
        /// </summary>
        public string WorkingRelationship { get; set; }

        /// <summary>
        /// Отношения торговой точки с Геркулес
        /// </summary>
        public string ReasonTradePoint { get; set; }

        /// <summary>
        /// Группы товарос со слишком высокой ценой
        /// </summary>
        public string Overcharge { get; set; }

        /// <summary>
        /// Коментарий к отношениям с торговой точкой
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Последняя торговая марка указанная в анкете
        /// </summary>
        public int LastTM { get; set; }

        /// <summary>
        /// Последняя группа товаров указанная в анкете
        /// </summary>
        public int LastGT { get; set; }

        // Аналитики
        public List<DocumentAnaliticModel> Analitics { get; set; }

        public DocumentMktgModel()
        {
            Analitics = new List<DocumentAnaliticModel>();
            Name = "Анкета клиета";
            Date = DateTime.Now;
            Time = DateTime.Now;
            StateId = State.STATEACTIVE;
        }

        public override void Save()
        {
            // Сохраняем документ
            DocumentMktg doc = this.ToObject(WADataProvider.WA);
            doc.UserName = WADataProvider.CurrentMembershipUser.UserName;
            doc.Document.UserOwnerId = WADataProvider.CurrentUser.Id;
            foreach (DocumentAnaliticModel m in Analitics)
            {
                if (m.AnaliticId == 677 && m.StateId != State.STATEDELETED)
                {
                    m.StringValue1 = Comment;
                }
                if (m.AnaliticId == 675 && m.StateId != State.STATEDELETED)
                {
                    m.StringValue1 = Comment;
                }
                DocumentMktgModel.SetAnalitic(doc, m);
            }
            doc.Save();
            try
            {
                DocumentAnaliticModel am = Analitics.FirstOrDefault(w => w.GroupId == 2 && w.StateId != State.STATEDELETED);
                if (am != null)
                {
                    Analitic tmp = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item((int)am.AnaliticId);
                    doc.Document.GetStringData().Value1 = tmp.Name;
                }
            }
            catch { }
            doc.Document.GetStringData().Save();
            this.Id = doc.Id;
        }

        public DocumentMktg ToObject(Workarea wa)
        {
            DocumentMktg mktg = new DocumentMktg() { Workarea = wa };
            if(Id != 0)
            {
                mktg.Load(Id);
            }
            mktg.Kind = 917505;
            mktg.ContactPhone = Telephone;
            mktg.ContactName = ContactFace;
            mktg.MetricAreaId = TPAreaId;
            mktg.WorkPostId = WorkpostId;
            mktg.OutletTypeId = TPTypeId;
            mktg.OutletLocationId = TPPositionId;
            mktg.TimePeriodId = TimePeriodId;
            mktg.BreakPeriodId = BreakPeriodId;
            mktg.StateId = StateId;
            if (IsReadOnly)
                mktg.FlagsValue |= FlagValue.FLAGREADONLY;
            else
                mktg.FlagsValue &= ~FlagValue.FLAGREADONLY;
            if (mktg.Document == null)
            {
                mktg.Document = new Document
                {
                    Workarea = wa,
                    KindId = DocumentMktg.KINDID_MRAK,
                    FolderId = 107,
                    ProjectItemId = 640
                };
            }
            mktg.Document.AgentFromId = 5;
            mktg.Document.MyCompanyId = MainClientDepatmentId.Value;
            mktg.Document.Date = Date;
            mktg.Document.Time = Time.TimeOfDay;
            mktg.UserName = HttpContext.Current.User.Identity.Name;
            /*if (string.IsNullOrEmpty(DocNumber) && mktg.Id == 0)
            {
                Autonum an = Autonum.GetAutonumByDocumentKind(WADataProvider.WA, 917505, mktg.Document.MyCompany.Code);
                if (an != null)
                {
                    if (string.IsNullOrEmpty(an.Prefix))
                    {
                        mktg.Document.Number = (an.Number + 1).ToString();
                    }
                    else
                    {
                        mktg.Document.Number = an.Prefix + "-" + (an.Number + 1).ToString();    
                    }
                    
                    an.Number++;
                    an.Save();
                }
            }*/
            if (string.IsNullOrEmpty(DocNumber))
            {
                    Autonum an = Autonum.GetAutonumByDocumentKindUser(WADataProvider.WA, mktg.Document.KindId, WADataProvider.CurrentUser.Id);
                    if (an.Id==0 && MainCompanyDepatmentId.HasValue)
                        an = Autonum.GetAutonumByDocumentKind(WADataProvider.WA, mktg.Document.KindId, MainCompanyDepatmentId.Value);
                    if (an.Id == 0 && !string.IsNullOrEmpty(mktg.Document.MyCompany.Code))
                        an = Autonum.GetAutonumByDocumentKind(WADataProvider.WA, mktg.Document.KindId, mktg.Document.MyCompany.Code);
                    if (an.Id == 0 && MainCompanyDepatmentId.HasValue && MainCompanyDepatmentId.Value!=0)
                    {
                        an = new Autonum { Workarea = WADataProvider.WA, DocKind = mktg.Document.KindId, StateId = State.STATEACTIVE, MyCompanyId = MainCompanyDepatmentId.Value };
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
                            mktg.Document.Number = (an.Number + 1).ToString(CultureInfo.InvariantCulture);
                        else
                            mktg.Document.Number = newDocnumber;
                        an.Number++;
                        an.Save();
                    }
            }
            else
            {
                mktg.Document.Number = DocNumber;
            }
            mktg.Document.AgentFromName = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(MainClientDepatmentId ?? 0).Name;
            mktg.Document.AgentToId = MainCompanyId;
            mktg.Document.AgentDepartmentToId = MainCompanyId;
            mktg.Document.AgentDepartmentFromId = MainClientDepatmentId.Value;
            if (MainCompanyId != 0)
            {
                mktg.Document.AgentToName = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(MainCompanyId).Name;
                mktg.Document.AgentDepartmentToName = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(MainCompanyId).Name;
            }
            else
            {
                mktg.Document.AgentToName = string.Empty;
                mktg.Document.AgentDepartmentToName = string.Empty;
            }

            if (MainClientDepatmentId != 0)
            {
                mktg.Document.AgentFromName = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(MainClientDepatmentId ?? 0).Name;
            }
            else
            {
                mktg.Document.AgentFromName = string.Empty;
            }

            if (MainClientDepatmentId != 0)
            {
                mktg.Document.AgentDepartmentFromName = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(MainClientDepatmentId.Value).Name;
            }
            else
            {
                mktg.Document.AgentDepartmentFromName = string.Empty;
            }
            mktg.Document.Name = Name;
            mktg.Document.Memo = Memo;
            mktg.ManagerId = ManagerId;

            UpdateAnaliticsGroup("957", new DocumentAnaliticModel { GroupId = 13, SummValue1 = TPCondition });
            UpdateAnaliticsGroup(WorkingRelationship, new DocumentAnaliticModel { GroupId = 3 });
            UpdateAnaliticsGroup(ReasonTradePoint, new DocumentAnaliticModel { GroupId = 2 });
            UpdateAnaliticsGroup(Overcharge, new DocumentAnaliticModel { GroupId = 16 });
            return mktg;
        }

        private void UpdateAnaliticsGroup(string Analitics, DocumentAnaliticModel value)
        {
            List<DocumentAnaliticModel> Models = this.Analitics.Where(w => w.GroupId == value.GroupId).ToList<DocumentAnaliticModel>();
            if (Analitics != null && Analitics.Length > 0)
            {
                List<int> Ans = Analitics.Split(',').Select(s => int.Parse(s)).ToList<int>();
                foreach (DocumentAnaliticModel model in Models)
                {
                    if (!Ans.Contains((int)model.AnaliticId))
                    {
                        model.StateId = State.STATEDELETED;
                    }
                    else
                    {
                        model.StateId = State.STATEACTIVE;
                        model.SummValue1 = value.SummValue1;
                        Ans.Remove((int)model.AnaliticId);
                    }
                }
                foreach (int id in Ans)
                {
                    this.Analitics.Add(new DocumentAnaliticModel
                    {
                        GroupId = value.GroupId,
                        AnaliticId = id,
                        StateId = State.STATEACTIVE,
                        SummValue1 = value.SummValue1
                    });
                }
            }
            else
            {
                foreach (DocumentAnaliticModel model in Models)
                {
                    model.StateId = State.STATEDELETED;
                }
            }
        }

        private static string BuildAnaliticsString(List<DocumentAnaliticModel> list)
        {
            string str = "";
            foreach (DocumentAnaliticModel an in list)
            {
                if (str.Length > 0)
                {
                    str += ",";
                }
                str += an.AnaliticId.ToString();
            }
            return str;
        }

        public static DocumentMktgModel GetObject(int id)
        {
            DocumentMktg doc = new DocumentMktg() { Workarea = WADataProvider.WA };
            doc.Load(id);
            return ConvertToModel(doc);
        }

        public static void SetAnalitic(DocumentMktg doc, DocumentAnaliticModel aModel)
        {
            if (aModel.Id == 0)
            {
                aModel.ToObject(WADataProvider.WA, doc.NewAnaliticRow());
            }
            else
            {
                foreach (DocumentAnalitic da in doc.Analitics)
                {
                    if (da.Id == aModel.Id)
                    {
                        da.AnaliticId = ((int)aModel.AnaliticId == 0 ? 1 : (int)aModel.AnaliticId);
                        da.AnaliticId2 = (int)aModel.Analitic2Id;
                        da.AnaliticId3 = (int)aModel.Analitic3Id;
                        da.AnaliticId4 = (int)aModel.Analitic4Id;
                        da.AnaliticId5 = (int)aModel.Analitic5Id;
                        da.Memo = aModel.Memo;
                        da.StateId = aModel.StateId;
                        da.IntValue1 = (aModel.IntValue1 == null ? 0 : (int)aModel.IntValue1);
                        da.IntValue2 = (aModel.IntValue2 == null ? 0 : (int)aModel.IntValue2);
                        da.IntValue3 = (aModel.IntValue3 == null ? 0 : (int)aModel.IntValue3);
                        da.IntValue4 = (aModel.IntValue4 == null ? 0 : (int)aModel.IntValue4);
                        da.IntValue5 = (aModel.IntValue5 == null ? 0 : (int)aModel.IntValue5);
                        da.SummValue1 = aModel.SummValue1 == null ? 0 : (decimal)aModel.SummValue1;
                        da.SummValue2 = aModel.SummValue2 == null ? 0 : (decimal)aModel.SummValue2;
                        da.SummValue3 = aModel.SummValue3 == null ? 0 : (decimal)aModel.SummValue3;
                        da.SummValue4 = aModel.SummValue4 == null ? 0 : (decimal)aModel.SummValue4;
                        da.SummValue5 = aModel.SummValue5 == null ? 0 : (decimal)aModel.SummValue5;
                        da.StringValue1 = aModel.StringValue1;
                        da.StringValue2 = aModel.StringValue2;
                        da.StringValue3 = aModel.StringValue3;
                        da.StringValue4 = aModel.StringValue4;
                        da.StringValue5 = aModel.StringValue5;
                        break;
                    }
                }
            }
        }

        public static DocumentMktgModel ConvertToModel(DocumentMktg value)
        {
            DocumentMktgModel res = new DocumentMktgModel
            {
                Name = value.Document.Name,
                Date = value.Document.Date,
                Time = value.Document.Date + value.Document.Time,
                DocNumber = value.Document.Number,
                Id = value.Id,
                MainClientId = value.Document.AgentFromId,
                MainClientName = value.Document.AgentFromName,
                MainCompanyId = value.Document.AgentToId,
                MainCompanyName = value.Document.AgentToName,

                MainClientDepatmentId = value.Document.AgentDepartmentFromId,
                MainCompanyDepatmentId = value.Document.AgentDepartmentToId,

                CurrencyId = value.Document.CurrencyId,
                CurrencyCode = value.Document.Currency.IntCode,
                Memo = value.Document.Memo,
                DocSumma = value.Document.Summa,
                DocSummaTax = value.Document.SummaTax,
                StateName = value.State.Name,
                UserName = value.UserName,
                SupervisorId = value.SupervisorId,
                SupervisorName = value.Supervisor == null ? "" : value.Supervisor.Name,
                ManagerId = value.ManagerId,
                ManagerName = value.Manager == null ? "" : value.Manager.Name,

                Telephone = value.ContactPhone,
                ContactFace = value.ContactName,
                TPAreaId = value.MetricAreaId,
                WorkpostId = value.WorkPostId,
                TPTypeId = value.OutletTypeId,
                TPPositionId = value.OutletLocationId,

                TimePeriodId = value.TimePeriodId,
                BreakPeriodId = value.BreakPeriodId,

                StateId = value.StateId,
                IsReadOnly = value.IsReadOnly,

                MyCompanyId = value.Document.MyCompanyId,
                MyCompanyName = value.Document.MyCompanyId == 0 ? string.Empty : value.Document.MyCompany.Name
            };
            if (value.Document != null)
            {
                Agent ag = new Agent { Workarea = WADataProvider.WA };
                ag.Load(value.Document.AgentToId);
                if (ag.AddressCollection.Count > 0)
                {
                    res.Address = AgentAddressModel.ConvertToModel(ag.AddressCollection[0]).GetActualName();
                }
            }
            ImportAnalitics(value.Analitics, res);

            try { res.TPCondition = (decimal)res.Analitics.First(w => w.GroupId == 13 && w.AnaliticId == 957 && w.StateId != State.STATEDELETED).SummValue1; } catch { }
            try { res.WorkingRelationship = BuildAnaliticsString(res.Analitics.Where(w => w.GroupId == 3 && w.StateId != State.STATEDELETED).ToList<DocumentAnaliticModel>()); } catch { }
            try { res.ReasonTradePoint = BuildAnaliticsString(res.Analitics.Where(w => w.GroupId == 2 && w.StateId != State.STATEDELETED).ToList<DocumentAnaliticModel>()); } catch { }
            try { res.Overcharge = BuildAnaliticsString(res.Analitics.Where(w => w.GroupId == 16 && w.StateId != State.STATEDELETED).ToList<DocumentAnaliticModel>()); } catch { }
            try { res.Comment = res.Analitics.First(w => w.AnaliticId == 677 && w.StateId != State.STATEDELETED).StringValue1; } catch { }
            try { res.Comment = res.Analitics.First(w => w.AnaliticId == 675 && w.StateId != State.STATEDELETED).StringValue1; } catch { }

            return res;
        }

        public static void ImportAnalitics(List<DocumentAnalitic> Analitics, DocumentMktgModel res) 
        {
            res.Analitics.Clear();
            foreach (DocumentAnalitic da in Analitics)
            {
                res.Analitics.Add(DocumentAnaliticModel.ConvertToModel(da));
            }
        }

        /// <summary>
        /// Возвращает коллекцию опросниокв
        /// </summary>
        /*public static List<AnaliticGroupModel> GetQuestionnaireList()
        {
            return AnaliticGroupModel.GetGroups().Where(w => w.KindId == AnaliticGroupKind.MktgQuestionnaire).ToList<AnaliticGroupModel>();
        }*/

        public static void CreateCopy(int id)
        {
            if (id == 0)
                return;
            DocumentMktg obj = WADataProvider.WA.Cashe.GetCasheData<DocumentMktg>().Item(id);
            DocumentMktg newObj = DocumentMktg.CreateCopy(obj);
            newObj.Document.Name += " (копия)";
            newObj.Save();
        }
    }
}