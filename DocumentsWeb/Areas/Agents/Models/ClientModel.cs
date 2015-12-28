using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Workflow.Activities.Rules;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.ASPxEditors;
using System;
using DocumentsWeb.Code;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.General.Models;

namespace DocumentsWeb.Areas.Agents.Models
{
    /// <summary>
    /// Состояние агента по связям
    /// </summary>
    public enum ClientChainState
    {
        /// <summary>
        /// Сотрудник
        /// </summary>
        Worker,
        /// <summary>
        /// Торговый
        /// </summary>
        Trader,
        /// <summary>
        /// Уволенный
        /// </summary>
        Dissmised,
        /// <summary>
        /// Удаленный
        /// </summary>
        Deleted
    }

    public class ClientModel : BusinessObjects.Models.AgentModel, 
        ICloneable, 
        IModelData, 
        IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult("Наименование обязательно", new[] { GlobalPropertyNames.Name });
            }
            if (!string.IsNullOrEmpty(Name) && Name.Length>255)
            {
                yield return new ValidationResult("Наименование не более 255 символов", new[] { GlobalPropertyNames.Name });
            }
            if (Id!=0 && MyCompanyId == 0)
            {
                yield return new ValidationResult("Укажите предприятие", new[] { GlobalPropertyNames.MyCompanyId });
            }

            if(KindId== Agent.KINDID_PEOPLE)
            {
                if (string.IsNullOrEmpty(FirstName))
                {
                    yield return new ValidationResult("Фамилия обязательна", new[] { GlobalPropertyNames.FirstName });
                }
                if (string.IsNullOrEmpty(LastName))
                {
                    yield return new ValidationResult("Имя обязательно", new[] { GlobalPropertyNames.LastName });
                }
                if (string.IsNullOrEmpty(MidleName))
                {
                    yield return new ValidationResult("Отчество обязательно", new[] { GlobalPropertyNames.MidleName });
                }

                if (!string.IsNullOrEmpty(FirstName) && FirstName.Length > 255)
                {
                    yield return new ValidationResult("Наименование не более 255 символов", new[] { GlobalPropertyNames.FirstName });
                }

                if (!string.IsNullOrEmpty(LastName) && LastName.Length > 255)
                {
                    yield return new ValidationResult("Наименование не более 255 символов", new[] { GlobalPropertyNames.LastName });
                }
                if (!string.IsNullOrEmpty(MidleName) && FirstName.Length > 255)
                {
                    yield return new ValidationResult("Наименование не более 255 символов", new[] { GlobalPropertyNames.MidleName });
                }
            }

            Type targetType = GetType();
            string fulltypename = targetType.FullName;
            IEnumerable<Ruleset> collRules = WADataProvider.WA.GetCollection<Ruleset>().Where(s => s.ActivityName == fulltypename && s.StateId == 1 && s.KindValue == Ruleset.KINDVALUE_WEBRULESET);
            
            
            foreach (Ruleset rule in collRules)
            {
                RuleSet ruleSetToValidate = rule.DeserializeRuleSet();
                if (ruleSetToValidate != null)
                {
                    RuleEngine engine = new RuleEngine(ruleSetToValidate, targetType);
                    engine.Execute(this);
                    if (Errors.Count > 0)
                    {
                        foreach (string k in Errors.Keys)
                        {
                            yield return new ValidationResult(Errors[k], new[] { k });
                        }
                    }
                }
            }
            
        }
         
        public string InHierarchies { get; set; }
        
        
        /// <summary>Адреса</summary>
        public List<AgentAddressModel> AddressCollection { get; set; }
        /// <summary>Банковские счета</summary>
        public List<AgentBankAccountModel> AccountsCollection { get; set; }
        /// <summary>Дополнительные модели</summary>
        public List<CodeValueModel> CodesCollection { get; set; }

        //public List<ClientModel> ChainAgents { get; set; }
        public List<WorkerChainModel> ChainAgents { get; set; }
        
        public ClientModel()
        {
            StateId = State.STATEACTIVE;
        }

        public void LoadChains()
        {
            ChainAgents = new List<WorkerChainModel>();
            //Agent myCompany = new Agent { Workarea = WADataProvider.WA };
            //myCompany.Load(Id);
            Agent myCompany = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(Id);
            

            List<Agent> coll = Chain<Agent>.GetChainSourceList(myCompany, WADataProvider.WA.WorkresChainId(), State.STATEACTIVE).ToList<Agent>();
            List<ClientModel> w = coll.Select(ClientModel.ConvertToModel).OrderBy(o => o.Name).ToList();
            foreach (ClientModel w_m in w) { ChainAgents.Add(new WorkerChainModel { Agent = w_m, AgentState = ClientChainState.Worker, StateId = State.STATEACTIVE }); }

            coll = Chain<Agent>.GetChainSourceList(myCompany, WADataProvider.WA.TradersChainId(), State.STATEACTIVE).ToList<Agent>();
            List<ClientModel> t = coll.Select(ClientModel.ConvertToModel).OrderBy(o => o.Name).ToList();
            foreach (ClientModel t_m in t) { ChainAgents.Add(new WorkerChainModel { Agent = t_m, AgentState = ClientChainState.Trader, StateId = State.STATEACTIVE }); }

            coll = Chain<Agent>.GetChainSourceList(myCompany, WADataProvider.WA.WorkreDissmisedChainId(), State.STATEACTIVE).ToList<Agent>();
            List<ClientModel> d = coll.Select(ClientModel.ConvertToModel).OrderBy(o => o.Name).ToList();
            foreach (ClientModel d_m in d) { ChainAgents.Add(new WorkerChainModel { Agent = d_m, AgentState = ClientChainState.Dissmised, StateId = State.STATEACTIVE }); }
        }

        public void SaveChains()
        {
            Agent myCompany = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(Id);
            
            //Agent myCompany = new Agent { Workarea = WADataProvider.WA };
            //myCompany.Load(Id);

            foreach(WorkerChainModel a in ChainAgents)
            {
                if (a.Agent.Id == 0)
                {
                    Agent new_ag = a.Agent.ToObject(WADataProvider.WA);
                    new_ag.MyCompanyId = Id;
                    new_ag.Save();
                    if(new_ag.IsPeople && new_ag.People!=null)
                    {
                        new_ag.People.Save();
                        if (new_ag.People.Employer != null)
                        {
                            new_ag.People.Employer.DateStart = DateTime.Now;
                            new_ag.People.Employer.DateEnd = DateTime.Now;
                            new_ag.People.Employer.Save();
                        }
                    }
                    a.Agent.Id = new_ag.Id;
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYWORKERS);
                    hierarchy.ContentAdd(new_ag, true);
                }

                if (a.AgentState != ClientChainState.Deleted)
                {
                    Chain<Agent> c = new Chain<Agent>();
                    c.Workarea = WADataProvider.WA;
                    c.LeftId = myCompany.Id;
                    c.Left = myCompany;
                    c.RightId = a.Agent.Id;
                    c.KindId = (a.AgentState == ClientChainState.Worker ? WADataProvider.WA.WorkresChainId() : (a.AgentState == ClientChainState.Trader ? WADataProvider.WA.TradersChainId() : (a.AgentState == ClientChainState.Dissmised ? WADataProvider.WA.WorkreDissmisedChainId() : 0)));
                    c.StateId = a.StateId;
                    c.Save();
                }
                else
                {
                    Agent ag = (Agent)WADataProvider.WA.Cashe.GetCasheItem(3, a.Agent.Id);
                    ag.StateId = State.STATEDELETED;
                    ag.Save();
                }
            }
            Chain<Agent>.GetChainSourceList(myCompany, WADataProvider.WA.WorkresChainId(), State.STATEACTIVE, true);

            Chain<Agent>.GetChainSourceList(myCompany, WADataProvider.WA.TradersChainId(), State.STATEACTIVE, true);

            Chain<Agent>.GetChainSourceList(myCompany, WADataProvider.WA.WorkreDissmisedChainId(), State.STATEACTIVE, true);
        }

        public Agent ToObject(Workarea workarea)
        {
            Agent agent = new Agent { Workarea = WADataProvider.WA };
            if(Id!=0)
                agent = workarea.Cashe.GetCasheData<Agent>().Item(Id);
            
            
            if (agent.Id == 0)
            {
                agent.IsNew = true;
                //agent.StateId = State.STATEACTIVE;
                //agent.KindId = Agent.KINDID_COMPANY;
                agent.KindId = KindId;
                Uid user = (Membership.Provider as BusinessObjectMembershipProvider).GetUid(HttpContext.Current.User.Identity.Name);
                agent.MyCompanyId = MyCompanyId == 0 ? WADataProvider.CurrentUser.MyCompanyId: MyCompanyId;
            }
            agent.People.Sex = Sex;
            agent.StateId = StateId;
            agent.UserName = HttpContext.Current.User.Identity.Name;
            agent.Id = Id;
            agent.Name = Name;
            agent.NameFull = NameFull;
            agent.AddressLegal = AddressLegal;
            agent.AddressPhysical = AddressPhysical;
            agent.Code = Code;
            agent.CodeTax = TaxNumber;
            agent.Memo = Memo;
            agent.Phone = Phone;
            agent.CodeFind = CodeFind;
            if (agent.IsCompany && agent.Company != null)
            {
                agent.Company.Okpo = Okpo;
                agent.Company.NdsPayer = NdsPayer;
                agent.Company.RegNumber = RegNumber;

                agent.Company.OwnershipId = OwnerShipId??0;
                agent.Company.TypeOutletId = TypeOutletId??0;
                agent.Company.MetricAreaId = MetricAreaId??0;
                agent.Company.CategoryId = CategoryId??0;
                agent.Company.InternationalName = InternationalName;

                agent.Company.RegPensionFund = RegPensionFund;
                agent.Company.RegEmploymentService = RegEmploymentService;
                agent.Company.RegSocialInsuranceNesch = RegSocialInsuranceNesch;
                agent.Company.RegSocialInsuranceDisability = RegSocialInsuranceDisability;
                agent.Company.RegPfu = RegPfu;
                agent.Company.RegOpfg = RegOpfg;
                agent.Company.RegKoatu = RegKoatu;
                agent.Company.RegKfv = RegKfv;
                agent.Company.RegZkgng = RegZkgng;
                agent.Company.RegKved = RegKved;

                if(agent.IsBank)
                {
                    agent.Company.Bank.Mfo = Mfo;
                    agent.Company.Bank.LicenseNo = LicenseNo;
                    agent.Company.Bank.LicenseDate = LicenseDate;
                    agent.Company.Bank.SertificateNo = SertificateNo;
                    agent.Company.Bank.SertificateDate = SertificateDate;
                    agent.Company.Bank.Swift = Swift;
                    agent.Company.Bank.CorrBankAccount = CorrBankAccount;
                }
            }
            if (agent.IsStore && agent.Store != null)
            {
                agent.Store.StorekeeperId = StorekeeperId == null ? 0 : (int)StorekeeperId;
            }
            if (agent.IsPeople && agent.People != null)
            {
                agent.People.FirstName = FirstName;
                agent.People.MidleName = MidleName;
                agent.People.LastName = LastName;
                agent.People.StateId = agent.StateId;
                agent.People.InsuranceNumber = InsuranceNumber;
                agent.People.InsuranceSeries = InsuranceSeries;
                agent.People.Invalidity = Invalidity;
                agent.People.Pension = Pension;
                agent.People.LegalWorker = LegalWorker;
                agent.People.PlaceEmploymentBookId = PlaceEmploymentBookId ?? 0;
                if (agent.People.Employer != null)
                {
                    agent.People.Employer.Mol = Mol;
                    agent.People.Employer.TabNo = TabNo;
                    agent.People.Employer.StateId = agent.StateId;
                    agent.People.Employer.DateStart = DateStart;
                    agent.People.Employer.DateEnd = DateEnd;
                    agent.People.TaxSocialPrivilege = TaxSocialPrivilege;
                }
            }
            return agent;
        }

        /// <summary>
        /// Получить корреспондента
        /// </summary>
        /// <param name="id"></param>
        /// <param name="refresh"></param>
        /// <returns></returns>
        public static ClientModel GetObject(int id, bool refresh=false)
        {
            Agent obj = refresh? WADataProvider.WA.GetObject<Agent>(id): WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id);
            
            return ConvertToModel(obj);
        }

        //public static List<ClientModel> GetObjectAsList(int? id)
        //{
        //    if (!id.HasValue)
        //        return null;
        //    List<ClientModel> res = new List<ClientModel>(1) {GetObject(id.Value)};
        //    return res;
        //}

        public static ClientModel ConvertToModel(Agent value)
        {
            ClientModel obj = new ClientModel();
            obj.GetData(value);
            //ClientModel obj = new ClientModel { 
            //    Id = value.Id, 
            //    Name = value.Name, 
            //    NameFull = value.NameFull, 
            //    Code = value.Code,
            //    CodeFind = value.CodeFind,
            //    Memo = value.Memo,
            //    UserName = value.UserName,
            //    TaxNumber = value.CodeTax, 
            //    KindId = value.KindId,
            //    StateName = value.State.Name,
            //    StateId = value.StateId,
            //    FlagsValue = value.FlagsValue,
            //    Phone = value.Phone,
            //    AddressPhysical = value.AddressPhysical,
            //    AddressLegal = value.AddressLegal,
            //    DateModified = value.DateModified,
            //    ReloadOnEdit = false
            //};
            //obj.IsReadOnly = value.IsReadOnly;
            //obj.IsSystem = value.IsSystem;
            //if (value.Memo != null && value.Memo.Length > 100)
            //    obj.DisplayMemo = value.Memo.Substring(0, 100) + "...";
            //else
            //    obj.DisplayMemo = value.Memo;
            //if (value.IsCompany && value.Company != null)
            //{
            //    obj.NdsPayer = value.Company.NdsPayer;
            //    obj.InternationalName = value.Company.InternationalName;
            //    obj.Okpo = value.Company.Okpo;
            //    if (value.Company.Ownership != null)
            //        obj.OwnerShip = value.Company.Ownership.Code;
            //    obj.OwnerShipId = value.Company.OwnershipId == 0 ? (int?)null : value.Company.OwnershipId;

            //    obj.TypeOutletId = value.Company.TypeOutletId == 0 ? (int?) null : value.Company.TypeOutletId;
            //    obj.MetricAreaId = value.Company.MetricAreaId == 0 ? (int?) null : value.Company.MetricAreaId;
            //    obj.CategoryId = value.Company.CategoryId == 0 ? (int?) null : value.Company.CategoryId;

            //    obj.RegPensionFund = value.Company.RegPensionFund;
            //    obj.RegEmploymentService = value.Company.RegEmploymentService;
            //    obj.RegSocialInsuranceNesch = value.Company.RegSocialInsuranceNesch;
            //    obj.RegSocialInsuranceDisability = value.Company.RegSocialInsuranceDisability;
            //    obj.RegPfu = value.Company.RegPfu;
            //    obj.RegOpfg = value.Company.RegOpfg;
            //    obj.RegKoatu = value.Company.RegKoatu;
            //    obj.RegKfv = value.Company.RegKfv;
            //    obj.RegZkgng = value.Company.RegZkgng;
            //    obj.RegKved = value.Company.RegKved;
            //}
            //if (value.IsStore && value.Store != null)
            //{
            //    obj.StorekeeperId = value.Store.StorekeeperId;
            //}
            //if(value.IsPeople)
            //{
            //    obj.FirstName = value.People.FirstName;
            //    obj.LastName = value.People.LastName;
            //    obj.MidleName = value.People.MidleName;
            //    obj.Sex = value.People.Sex;
            //    obj.TaxSocialPrivilege = value.People.TaxSocialPrivilege;
            //    obj.InsuranceNumber = value.People.InsuranceNumber;
            //    obj.InsuranceSeries = value.People.InsuranceSeries;
            //    obj.LegalWorker = value.People.LegalWorker;
            //    obj.Invalidity = value.People.Invalidity;
            //    obj.Pension = value.People.Pension;
            //    obj.PlaceEmploymentBookId = value.People.PlaceEmploymentBookId;
            //    if (value.People.Employer != null)
            //    {
            //        obj.TabNo = value.People.Employer.TabNo;
            //        obj.Mol = value.People.Employer.Mol;
            //        obj.DateStart = value.People.Employer.DateStart;
            //        obj.DateEnd = value.People.Employer.DateEnd;
            //    }
            //}
            //obj.MyCompanyId = value.MyCompanyId;
            //if (value.MyCompanyId!=0)
            //    obj.MyCompanyName = value.MyCompany.Name;
            if (value.FirstHierarchy() != null)
                obj.DefaultGroup = value.FirstHierarchy().Name;

            obj.AddressCollection = value.AddressCollection.Select(AgentAddressModel.ConvertToModel).ToList();
            obj.AccountsCollection = value.BankAccounts.Select(AgentBankAccountModel.ConvertToModel).ToList();
            obj.CodesCollection = value.GetValues(true).Select(CodeValueModel.ConverToModel).ToList();
            obj.InHierarchies = string.Join(",", HierarchyModel.GetHierarchiesWith(value).Select(s => s.Id.ToString(CultureInfo.InvariantCulture)));

            return obj;
        }

        public static ClientModel ConvertToModelLight(Agent value)
        {
            ClientModel obj = new ClientModel { 
                Id = value.Id, 
                Name = value.Name, 
                NameFull = value.NameFull, 
                Code = value.Code,
                CodeFind = value.CodeFind,
                Memo = value.Memo, 
                TaxNumber = value.CodeTax,
                Phone = value.Phone,
                KindId = value.KindId,
                StateId = value.StateId,
                FlagsValue = value.FlagsValue,
                AddressPhysical = value.AddressPhysical,
                AddressLegal = value.AddressLegal
            };
            obj.MyCompanyId = value.MyCompanyId;
            if(value.MyCompanyId!=0)
            {
                obj.MyCompanyName = value.MyCompany.Name;
            }
            obj.IsReadOnly = value.IsReadOnly;
            obj.IsSystem = value.IsSystem;
            if (value.Memo != null && value.Memo.Length > 100)
                obj.DisplayMemo = value.Memo.Substring(0, 100) + "...";
            else
                obj.DisplayMemo = value.Memo;
            if (value.IsCompany && value.Company != null)
            {
                obj.InternationalName = value.Company.InternationalName;
                obj.Okpo = value.Company.Okpo;
                if (value.Company.Ownership != null)
                    obj.OwnerShip = value.Company.Ownership.Code;
                obj.OwnerShipId = value.Company.OwnershipId == 0 ? (int?)null : value.Company.OwnershipId;

                obj.TypeOutletId = value.Company.TypeOutletId == 0 ? (int?)null : value.Company.TypeOutletId;
                obj.MetricAreaId = value.Company.MetricAreaId == 0 ? (int?)null : value.Company.MetricAreaId;
                obj.CategoryId = value.Company.CategoryId == 0 ? (int?)null : value.Company.CategoryId;
            }
            if (value.IsPeople)
            {
                obj.FirstName = value.People.FirstName;
                obj.LastName = value.People.LastName;
                obj.MidleName = value.People.MidleName;
                obj.Sex = value.People.Sex;
                obj.TaxSocialPrivilege = value.People.TaxSocialPrivilege;
                obj.InsuranceNumber = value.People.InsuranceNumber;
                obj.InsuranceSeries = value.People.InsuranceSeries;
                obj.LegalWorker = value.People.LegalWorker;
                obj.Invalidity = value.People.Invalidity;
                obj.Pension = value.People.Pension;
                obj.PlaceEmploymentBookId = value.People.PlaceEmploymentBookId;
                if (value.People.Employer != null)
                {
                    obj.TabNo = value.People.Employer.TabNo;
                    obj.Mol = value.People.Employer.Mol;
                    obj.DateStart = value.People.Employer.DateStart;
                    obj.DateEnd = value.People.Employer.DateEnd;
                }
            }
            //obj.AddressCollection = value.AddressCollection.Select(AgentAddressModel.ConvertToModel).ToList();
            //obj.AccountsCollection = value.BankAccounts.Select(AgentBankAccountModel.ConvertToModel).ToList();
            //obj.CodesCollection = value.GetValues(true).Select(CodeValueModel.ConverToModel).ToList();

            return obj;
        }

        public static ClientModel ConvertToModel(AgentWebView value)
        {
            ClientModel obj = new ClientModel
            {
                Id = value.Id,
                KindId = value.KindId,
                Name = value.Name,
                NameFull = value.NameFull,
                Code = value.Code,
                Memo = value.Memo,
                TaxNumber = value.TaxNumber,
                DisplayMemo = value.DisplayMemo,
                Okpo = value.Okpo,
                OwnerShip = value.OwnerShip,
                OwnerShipId = value.OwnerShipId == 0 ? (int?)null : value.OwnerShipId,
                MyCompanyId = value.MyCompanyId,
                MyCompanyName = value.MyCompanyName,
                StateName = value.StateName,
                TypeOutletId = value.TypeOutletId == 0 ? (int?)null : value.TypeOutletId,
                MetricAreaId = value.MetricAreaId == 0 ? (int?)null : value.MetricAreaId,
                CategoryId = value.CategoryId == 0 ? (int?)null : value.CategoryId,
                StateId = value.StateId,
                FlagsValue = value.FlagsValue,
                Phone = value.Phone,
                ReloadOnEdit = true
            };
            obj.IsReadOnly = (value.FlagsValue & FlagValue.FLAGREADONLY) == FlagValue.FLAGREADONLY;
            obj.IsSystem = (value.FlagsValue & FlagValue.FLAGSYSTEM) == FlagValue.FLAGSYSTEM;
            if (value.FirstHierarchy != null)
                obj.DefaultGroup = value.FirstHierarchy;
            return obj;
        }

        public static ClientModel ConvertToModel(AgentBankWebView value)
        {
            ClientModel obj = new ClientModel
            {
                Id = value.Id,
                KindId = value.KindId,
                Name = value.Name,
                NameFull = value.NameFull,
                Code = value.Code,
                Memo = value.Memo,
                TaxNumber = value.TaxNumber,
                DisplayMemo = value.DisplayMemo,
                Okpo = value.Okpo,
                OwnerShip = value.OwnerShip,
                OwnerShipId = value.OwnerShipId == 0 ? (int?)null : value.OwnerShipId,
                MyCompanyId = value.MyCompanyId,
                MyCompanyName = value.MyCompanyName,
                StateName = value.StateName,
                TypeOutletId = value.TypeOutletId == 0 ? (int?)null : value.TypeOutletId,
                MetricAreaId = value.MetricAreaId == 0 ? (int?)null : value.MetricAreaId,
                CategoryId = value.CategoryId == 0 ? (int?)null : value.CategoryId,
                StateId = value.StateId,
                FlagsValue = value.FlagsValue,
                Phone = value.Phone,
                Mfo = value.Mfo,
                SertificateNo = value.SertificateNo,
                LicenseNo = value.LicenseNo,
                Swift= value.Swift,
                CorrBankAccount = value.CorrBankAccount,
                SertificateDate=value.SertificateDate,
                LicenseDate = value.LicenseDate,
                ReloadOnEdit = true
            };
            obj.IsReadOnly = (value.FlagsValue & FlagValue.FLAGREADONLY) == FlagValue.FLAGREADONLY;
            obj.IsSystem = (value.FlagsValue & FlagValue.FLAGSYSTEM) == FlagValue.FLAGSYSTEM;
            if (value.FirstHierarchy != null)
                obj.DefaultGroup = value.FirstHierarchy;
            return obj;
        }

        public static ClientModel ConvertToModel(PeopleWebView value)
        {
            ClientModel obj = new ClientModel
            {
                Id = value.Id,
                KindId = value.KindId,
                Name = value.Name,
                NameFull = value.NameFull,
                Code = value.Code,
                Memo = value.Memo,
                TaxNumber = value.TaxNumber,
                DisplayMemo = value.DisplayMemo,
                FirstName = value.FirstName,
                LastName = value.LastName,
                MidleName = value.MidleName,
                MyCompanyId = value.MyCompanyId,
                MyCompanyName = value.MyCompanyName,
                StateName = value.StateName,
                StateId = value.StateId,
                FlagsValue = value.FlagsValue,
                Phone = value.Phone,
                ReloadOnEdit = true
            };
            obj.IsReadOnly = (value.FlagsValue & FlagValue.FLAGREADONLY) == FlagValue.FLAGREADONLY;
            obj.IsSystem = (value.FlagsValue & FlagValue.FLAGSYSTEM) == FlagValue.FLAGSYSTEM;
            if (value.FirstHierarchy != null)
                obj.DefaultGroup = value.FirstHierarchy;
            return obj;
        }
        /// <summary>
        /// Список торговых для связывания
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentSelectedId"></param>
        /// <returns></returns>
        public static IEnumerable GetChainSourceList(int? id, int? currentSelectedId = null)
        {
            if (id.HasValue && id!=0)
            {
                int tradersChainId = WorkareaExtention.TradersChainId(WADataProvider.WA); //WADataProvider.WA.TradersChainId();
                //WADataProvider.WA.CollectionChainKinds.FirstOrDefault(s => s.Code == ChainKind.TRADERS && s.FromEntityId == (int)WhellKnownDbEntity.Agent).Id;
                Agent ag = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id.Value);
                List<Agent> coll = Chain<Agent>.GetChainSourceList(ag, tradersChainId).ToList();
                if (currentSelectedId.HasValue && currentSelectedId.Value != 0)
                {
                    if (!coll.Exists(f => f.Id == currentSelectedId.Value))
                    {
                        Agent objCurrent = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(currentSelectedId.Value);
                        coll.Add(objCurrent);
                    }
                }
                return coll.Select(ConvertToModelLight).OrderBy(o => o.Name).ToList();
            }
            return new List<ClientModel>();
        }

        /// <summary>
        /// Список всех сотрудников предприятия, используется для связывания
        /// </summary>
        /// <param name="id">Идентификатор компании</param>
        /// <param name="currentSelectedId">Идентификатор текущего сотрудника </param>
        /// <returns></returns>
        public static IEnumerable GetChainWorker(int? id, int? currentSelectedId = null)
        {
            if (id.HasValue && id != 0)
            {
                int tradersChainId = WADataProvider.WA.WorkresChainId();
                //WADataProvider.WA.CollectionChainKinds.FirstOrDefault(s => s.Code == ChainKind.TRADERS && s.FromEntityId == (int)WhellKnownDbEntity.Agent).Id;
                Agent ag = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id.Value);
                List<Agent> coll = Chain<Agent>.GetChainSourceList(ag, tradersChainId).ToList();
                
                if (currentSelectedId.HasValue && currentSelectedId.Value != 0)
                {
                    if (!coll.Exists(f => f.Id == currentSelectedId.Value))
                    {
                        Agent objCurrent = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(currentSelectedId.Value);
                        coll.Add(objCurrent);
                    }
                }
                return coll.Select(ConvertToModelLight).OrderBy(o => o.Name).ToList();
            }
            if(currentSelectedId.HasValue && currentSelectedId.Value!=0)
            {
                List<Agent> coll = new List<Agent>();
                if (currentSelectedId.HasValue && currentSelectedId.Value != 0)
                {
                    if (!coll.Exists(f => f.Id == currentSelectedId.Value))
                    {
                        Agent objCurrent = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(currentSelectedId.Value);
                        coll.Add(objCurrent);
                    }
                }
                return coll.Select(ConvertToModelLight).OrderBy(o => o.Name).ToList();
            }
            return new List<ClientModel>();
        }

        /// <summary>
        /// Список всех сотрудников предприятия
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IEnumerable GetChainWorker(int? id)
        {
            if (id.HasValue && id != 0)
            {
                int tradersChainId = WADataProvider.WA.WorkresChainId();
                //WADataProvider.WA.CollectionChainKinds.FirstOrDefault(s => s.Code == ChainKind.TRADERS && s.FromEntityId == (int)WhellKnownDbEntity.Agent).Id;
                Agent ag = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id.Value);
                List<Agent> coll = Chain<Agent>.GetChainSourceList(ag, tradersChainId).ToList();
                return coll.Select(ConvertToModelLight).OrderBy(o => o.Name).ToList();
            }
            return new List<ClientModel>();
        }

        /// <summary>
        /// Список расчетных счетов для связывания
        /// </summary>
        /// <param name="id">Идентификатор компании</param>
        /// <param name="currentSelectedId">Текущее значение</param>
        /// <returns></returns>
        public static List<ComboboxModel>GetAccounts(int? id, int? currentSelectedId=null)
        {
            if (id.HasValue)
            {
                Agent ag = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id.Value);

                List<AgentBankAccount> coll = ag.BankAccounts;
                if (currentSelectedId.HasValue && currentSelectedId.Value != 0)
                {
                    if (!coll.Exists(f => f.Id == currentSelectedId.Value))
                    {
                        AgentBankAccount objCurrent = WADataProvider.WA.Cashe.GetCasheData<AgentBankAccount>().Item(currentSelectedId.Value);
                        coll.Add(objCurrent);
                    }
                }
                return coll.Select(s => new ComboboxModel(s)).ToList();
            }
            return new List<ComboboxModel>();
        }
        /// <summary>
        /// Используется для дополнительного связывания в комбобоксах
        /// </summary>
        /// <param name="value">Текущее значение</param>
        /// <returns></returns>
        public static object GetAccountsByValue(object value)
        {
            return value != null ? new List<ComboboxModel> { new ComboboxModel(WADataProvider.WA.GetObject<AgentBankAccount>((int)value)) }.Take(1).ToList() : null;
        }

        /// <summary>
        /// Используется для дополнительного связывания в комбобоксах
        /// </summary>
        /// <param name="value">Текущее значение</param>
        /// <returns></returns>
        public static object GetStoresByValue(object value)
        {
            return value != null ? new List<ComboboxModel> { new ComboboxModel(WADataProvider.WA.GetObject<Agent>((int)value) as IBase) }.Take(1).ToList() : null;
        }
        /// <summary>
        /// Список складов для биндинга
        /// </summary>
        /// <param name="id">Идентификатор компании</param>
        /// <param name="currentSelectedId">Текущее значение</param>
        /// <returns></returns>
        public static List<ComboboxModel> GetStores(int? id, int? currentSelectedId=null)
        {
            if (id.HasValue && id!=0)
            {
                int chainId = WADataProvider.WA.StoreChainId();
                Agent ag = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id.Value);
                List<Agent> coll = Chain<Agent>.GetChainSourceList(ag, chainId).ToList();
                if(currentSelectedId.HasValue && currentSelectedId.Value!=0)
                {
                    if(!coll.Exists(f=>f.Id==currentSelectedId.Value))
                    {
                        Agent objCurrent = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(currentSelectedId.Value);
                        coll.Add(objCurrent);
                    }
                }
                return coll.Select(s => new ComboboxModel(s as IBase)).OrderBy(o => o.Name).ToList();
            }
            return new List<ComboboxModel>();
        }
        /// <summary>
        /// Метод используемый для связывания в списках
        /// </summary>
        /// <param name="value">Текущее значение, int</param>
        /// <returns></returns>
        public static object GetAgentsByValue(object value)
        {
            return value != null ? new List<ClientModel> { GetObject((int)value) }.Take(1).ToList() : null;
        }
        /// <summary>
        /// Метод используемый для связывания в списках
        /// </summary>
        /// <param name="value">Текущее значение, int</param>
        /// <returns></returns>
        public static object GetAgentsLightByValue(object value)
        {
            return value != null && (int)value!=0 ? new List<ClientModel> {ConvertToModelLight(WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(((int)value)))}.Take(1).ToList() : null;
        }

        /// <summary>
        /// Список корреспондентов по иерархии. Если иерархия не указана - все корреспонденты, 
        /// в противном случае корреспонденты по указанной иерархии.
        /// </summary>
        /// <param name="rootHierarchyCode">Строковый код иерархии</param>
        /// <param name="refresh">Выполнять обращение к серверу БД за данными</param>
        public static List<ClientModel> GetCollection(string rootHierarchyCode = null, bool refresh = false)
        {
            if (string.IsNullOrEmpty(rootHierarchyCode))
            {
                List<Agent> collAll = WADataProvider.WA.GetCollection<Agent>();
                return collAll.Select(ConvertToModel).ToList();
            }

            
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);

            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ExistInCashe(h.Id))
                    return WADataProvider.CacheClientsModelData.GetDataForUser(h.Id);
            }

            List<AgentWebView> coll = AgentWebView.GetView(h, true, refresh);

            WADataProvider.CacheClientsModelData.AddToCashe(h.Id, coll.Select(ConvertToModel).ToList());
            return WADataProvider.CacheClientsModelData.GetDataForUser(h.Id);
        }

        /// <summary>
        /// Список корреспондентов по иерархии. Если иерархия не указана - все корреспонденты, 
        /// в противном случае корреспонденты по указанной иерархии.
        /// </summary>
        /// <param name="rootHierarchyCode">Строковый код иерархии</param>
        /// <param name="refresh">Выполнять обращение к серверу БД за данными</param>
        public static List<ClientModel> GetCollectionBanks(string rootHierarchyCode = null, bool refresh = false)
        {
            if (string.IsNullOrEmpty(rootHierarchyCode))
            {
                List<Agent> collAll = WADataProvider.WA.GetCollection<Agent>();
                return collAll.Select(ConvertToModel).ToList();
            }


            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);

            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ExistInCashe(h.Id))
                    return WADataProvider.CacheClientsModelData.GetDataForUser(h.Id);
            }

            List<AgentBankWebView> coll = AgentBankWebView.GetView(h, true);

            WADataProvider.CacheClientsModelData.AddToCashe(h.Id, coll.Select(ConvertToModel).ToList());
            return WADataProvider.CacheClientsModelData.GetDataForUser(h.Id);
        }

        //TODO: А если в иерархиях не проставлен код?
        public static List<ClientModel> GetCollection(string[] roots, bool refresh = false)
        {
            List<Agent> coll = new List<Agent>();

            foreach (string s in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(s);
                coll.InsertRange(0, h.GetTypeContents<Agent>(true, refresh));
            }
            return coll.Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel).Distinct(new AgentComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<ClientModel> GetCollectionWONested(string rootHierarchyCode = null)
        {
            if (string.IsNullOrEmpty(rootHierarchyCode))
            {
                return WADataProvider.WA.GetCollection<Agent>().Where(s => !s.IsHiden).Select(ConvertToModel).ToList();
            }

            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            List<Agent> coll = h.GetTypeContents<Agent>(false);
            return coll.Where(s=>WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel).Distinct(new AgentComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<ClientModel> GetCollectionWONested(int[] roots)
        {
            if (roots == null)
            {
                return WADataProvider.WA.GetCollection<Agent>().Where(s => !s.IsHiden 
                    && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel).ToList();
            }

            List<ClientModel> coll = new List<ClientModel>();
            foreach (int item in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(item);
                //coll.AddRange(h.GetTypeContents<Agent>(false));
                coll.AddRange(AgentWebView.GetView(h, false).Where(s=>WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel));
            }
            return coll.Distinct(new AgentComparer()).OrderBy(o => o.Name).ToList();
            
        }

        public static List<ClientModel> GetCollectionBankWONested(int[] roots)
        {
            if (roots == null)
            {
                return WADataProvider.WA.GetCollection<Agent>().Where(s => !s.IsHiden
                    && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel).ToList();
            }

            List<ClientModel> coll = new List<ClientModel>();
            foreach (int item in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(item);
                //coll.AddRange(h.GetTypeContents<Agent>(false));
                coll.AddRange(AgentBankWebView.GetView(h).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel));
            }
            return coll.Distinct(new AgentComparer()).OrderBy(o => o.Name).ToList();
        }
        /// <summary>
        /// Список поставщиков
        /// </summary>
        public static IEnumerable GetSuppliers(bool refresh = false)
        {

            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_SUPPLIERS);

            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ExistInCashe(hRoot.Id))
                    return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
            }

            List<AgentWebView> coll = AgentWebView.GetView(hRoot, true);
            //if (WADataProvider.CacheClientsModelDataNew.AddToCashe.ContainsKey(hRoot.Id))
            //    WADataProvider.CacheClientsModelData.Remove(hRoot.Id);
            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, coll.Select(ConvertToModel).ToList());
            return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
        }

        /// <summary>
        /// Список складов
        /// </summary>
        public static IEnumerable GetStores(bool refresh = false)
        {

            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYSTORES);

            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ExistInCashe(hRoot.Id))
                    return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
            }

            List<AgentWebView> coll = AgentWebView.GetView(hRoot, true);
            //if (WADataProvider.CacheClientsModelDataNew.AddToCashe.ContainsKey(hRoot.Id))
            //    WADataProvider.CacheClientsModelData.Remove(hRoot.Id);
            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, coll.Select(ConvertToModel).ToList());
            return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
        }

        /// <summary>
        /// Список сотрудников
        /// </summary>
        public static IEnumerable GetWorkers(bool refresh = false)
        {

            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYWORKERS);

            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ExistInCashe(hRoot.Id))
                    return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
            }

            List<PeopleWebView> coll = PeopleWebView.GetView(hRoot, true);
            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, coll.Select(ConvertToModel).ToList());
            return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
        }

        public static List<ClientModel> GetWorkersCollectionWONested(int[] roots)
        {
            if (roots == null)
            {
                return WADataProvider.WA.GetCollection<Agent>().Where(s => !s.IsHiden
                    && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel).ToList();
            }

            List<ClientModel> coll = new List<ClientModel>();
            foreach (int item in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(item);
                //coll.AddRange(h.GetTypeContents<Agent>(false));
                coll.AddRange(PeopleWebView.GetView(h, false).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel));
            }
            return coll.Distinct(new AgentComparer()).OrderBy(o => o.Name).ToList();

        }
        /// <summary>
        /// Список сотрудников
        /// </summary>
        public static IEnumerable GetWorkersCollection(string rootHierarchyCode = null, bool refresh = false)
        {
            if (string.IsNullOrEmpty(rootHierarchyCode))
            {
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYWORKERS);

                if (!refresh)
                {
                    if (WADataProvider.CacheClientsModelData.ExistInCashe(hRoot.Id))
                        return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
                }

                List<PeopleWebView> _coll = PeopleWebView.GetView(hRoot, true);
                WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, _coll.Select(ConvertToModel).ToList());
                return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
            }

            //List<int> userScopeView = WADataProvider.WA.Access.GetCompanyScopeView(HttpContext.Current.User.Identity.Name);


            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);

            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ExistInCashe(h.Id))
                    return WADataProvider.CacheClientsModelData.GetDataForUser(h.Id);
                //return WADataProvider.CacheClientsModelData[h.Id].Where(f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId));
            }

            List<PeopleWebView> coll = PeopleWebView.GetView(h, true);
            //if (WADataProvider.CacheClientsModelDataNew.ExistInCashe(h.Id))
            //    WADataProvider.CacheClientsModelDataNew.Remove(h.Id);
            WADataProvider.CacheClientsModelData.AddToCashe(h.Id, coll.Select(ConvertToModel).ToList());
            return WADataProvider.CacheClientsModelData.GetDataForUser(h.Id);

            //if (CacheClientsModelData.ContainsKey(h.Id))
            //    return CacheClientsModelData[h.Id].Where(s => userScopeView.Contains(s.MyCompanyId));

            //List<AgentWebView> collNew = AgentWebView.GetView(hRoot, true);
            //CacheClientsModelData.Remove(h.Id);
            //CacheClientsModelData.Add(h.Id, collNew.Select(ClientModel.ConvertToModel).ToList().OrderBy(o=>o.Name).ToList());
            //return CacheClientsModelData[h.Id].Where(s => userScopeView.Contains(s.MyCompanyId));


            //List<Agent> coll2 = h.GetTypeContents<Agent>(true).Where(s=>userScopeView.Contains(s.MyCompanyId)).ToList();
            //return coll2.Select(ConvertToModel).OrderBy(o => o.Name).ToList();
        }
        /// <summary>
        /// Список клиентов
        /// </summary>
        public static List<ClientModel> GetClients(bool refresh = false)
        {
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_BUYERS);
            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ContainsKey(hRoot.Id))
                    return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
            }
            //if (WADataProvider.CacheClientsModelData.ContainsKey(hRoot.Id))
            //    return WADataProvider.CacheClientsModelData[hRoot.Id].Where(f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId));

            List<AgentWebView> coll = AgentWebView.GetView(hRoot, true);
            //if (WADataProvider.CacheClientsModelData.ContainsKey(hRoot.Id))
            //    WADataProvider.CacheClientsModelData.Remove(hRoot.Id);
            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, coll.Select(ConvertToModel).ToList());
            return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
        }


        /// <summary>
        /// Список банков
        /// </summary>
        public static IEnumerable GetBanks(bool refresh = false)
        {
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_BANKS);
            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ContainsKey(hRoot.Id))
                    return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
            }
            //if (WADataProvider.CacheClientsModelData.ContainsKey(hRoot.Id))
            //    return WADataProvider.CacheClientsModelData[hRoot.Id].Where(f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId));

            List<AgentWebView> coll = AgentWebView.GetView(hRoot, true);
            //if (WADataProvider.CacheClientsModelData.ContainsKey(hRoot.Id))
            //    WADataProvider.CacheClientsModelData.Remove(hRoot.Id);
            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, coll.Select(ConvertToModel).ToList());
            return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
        }

        /// <summary>
        /// Список моих предприятий (в области создания)
        /// </summary>
        public static List<ClientModel> GetMyDepatments(bool refresh = false)
        {
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYDEPATMENTS);
            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ContainsKey(hRoot.Id))
                    return WADataProvider.CacheClientsModelData.GetDataForUserInCreateScope(hRoot.Id);
                    //return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
            }

            List<AgentWebView> coll = AgentWebView.GetView(hRoot, true);

            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, coll.Select(ConvertToModel).ToList());
            //return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id).OrderBy(o => o.Name).ToList();
            return WADataProvider.CacheClientsModelData.GetDataForUserInCreateScope(hRoot.Id).OrderBy(o => o.Name).ToList();
        }
        
        /// <summary>
        /// Список моих предприятий (в области создания)
        /// </summary>
        public static List<ClientModel> GetMyDepatmentsBind(int selectedValue, bool refresh = false)
        {
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYDEPATMENTS);
            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ContainsKey(hRoot.Id))
                {
                    List<ClientModel> coll = WADataProvider.CacheClientsModelData.GetDataForUserInCreateScope(hRoot.Id);

                    if(selectedValue!=0 && !coll.Exists(f=>f.Id== selectedValue))
                    {
                        coll.Add(ClientModel.GetObject(selectedValue));
                    }
                    return coll.OrderBy(o => o.Name).ToList(); ;
                }
            }

            List<AgentWebView> collData = AgentWebView.GetView(hRoot, true);

            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, collData.Select(ConvertToModel).ToList());

            List<ClientModel> coll2 = WADataProvider.CacheClientsModelData.GetDataForUserInCreateScope(hRoot.Id);

            if (selectedValue != 0 && !coll2.Exists(f => f.Id == selectedValue))
            {
                coll2.Add(ClientModel.GetObject(selectedValue));
            }
            return coll2.OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Список моих предприятий (в области просмотра)
        /// </summary>
        public static List<ClientModel> GetMyDepatmentsViewBind(int selectedValue, bool refresh = false)
        {
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYDEPATMENTS);
            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ContainsKey(hRoot.Id))
                {
                    List<ClientModel> coll = WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);

                    if (selectedValue != 0 && !coll.Exists(f => f.Id == selectedValue))
                    {
                        coll.Add(ClientModel.GetObject(selectedValue));
                    }
                    return coll.OrderBy(o => o.Name).ToList(); ;
                }
            }

            List<AgentWebView> collData = AgentWebView.GetView(hRoot, true);

            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, collData.Select(ConvertToModel).ToList());

            List<ClientModel> coll2 = WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);

            if (selectedValue != 0 && !coll2.Exists(f => f.Id == selectedValue))
            {
                coll2.Add(ClientModel.GetObject(selectedValue));
            }
            return coll2.OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Список моих предприятий (в области видисомти)
        /// </summary>
        public static List<ClientModel> GetMyDepatmentsView(bool refresh = false)
        {
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYDEPATMENTS);
            if (!refresh)
            {
                if (WADataProvider.CacheClientsModelData.ContainsKey(hRoot.Id))
                    return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
                //return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id);
            }

            List<AgentWebView> coll = AgentWebView.GetView(hRoot, true);

            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, coll.Select(ConvertToModel).ToList());
            //return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id).OrderBy(o => o.Name).ToList();
            return WADataProvider.CacheClientsModelData.GetDataForUser(hRoot.Id).OrderBy(o => o.Name).ToList();
        }
        /// <summary>
        /// Список моих предприятий включая системный холдинг
        /// </summary>
        public static List<ClientModel> GetMyDepatmentsWithHolding(bool refresh = false)
        {
            List<ClientModel> coll = GetMyDepatments(refresh);
            if(WADataProvider.CommonRightView.AdminEnterprize)
                coll.Add(GetObject(-1));
            return coll;
        }

        /// <summary>
        /// Список корреспондентов по компании 
        /// </summary>
        public static IEnumerable GetCollectionByCompany(int myCompanyId)
        {
            //TODO:
            //IEnumerable<Agent> coll = WADataProvider.WA.GetCollection<Agent>().Where(s=>s.MyCompanyId==myCompanyId && !s.IsTemplate && !s.IsStateDeleted);
            IEnumerable<Agent> coll = WADataProvider.WA.Empty<Agent>().FindBy(myCompanyId: myCompanyId,
                                                                              filter: s =>!s.IsTemplate && !s.IsStateDeleted);
            return coll.Select(s => new ComboboxModel(s as IBase)).ToList();
        }

        /// <summary>
        /// Список банков
        /// </summary>
        /*public static IEnumerable GetBanks()
        {
            //Hierarchy hRoot = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_BANKS);
            List<Agent> coll = WADataProvider.WA.GetBanks();//hRoot.GetTypeContents<Agent>(); 
            return coll.Select(ConvertToModel).ToList();
        }*/

        /// <summary>
        /// Список моих предприятий
        /// </summary>
        /*public static IEnumerable GetMyDepatments()
        {
            //Hierarchy hRoot = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_BANKS);
            List<Agent> coll = WADataProvider.WA.GetMyDepatments();//hRoot.GetTypeContents<Agent>(); 
            return coll.Select(ConvertToModel).ToList();
        }*/

        public static Dictionary<string, int> currentMyCompanies = new Dictionary<string, int>();
        [Obsolete("Возвращает клиентов только из текущей выбранной компании, а не из выбранной и родительских + не кеширует запросы. Используйте GetAgentsRangeFromView")]
        public static object GetAgentsRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            int agentTo = (currentMyCompanies.ContainsKey(HttpContext.Current.Session.SessionID) ? currentMyCompanies[HttpContext.Current.Session.SessionID] : 0);
            string name = string.IsNullOrEmpty(args.Filter) ? null : "%" + args.Filter + "%";

            if (agentTo == 0)
            {
                // Моя компания не задана, берем все доступные
                List<int> userScopeView = WADataProvider.WA.Access.GetCompanyScopeView(HttpContext.Current.User.Identity.Name);

                if (userScopeView.Count > 0)
                {
                    IEnumerable<Agent> coll = WADataProvider.WA.Empty<Agent>().FindBy(myCompanyId: userScopeView[0], name: name, useAndFilter: true,
                                                                                      filter:s =>!s.IsTemplate && s.IsStateActive).Where(s=>s.MyCompanyId!=0 && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId));
                    currentMyCompanies.Remove(HttpContext.Current.Session.SessionID);
                    //return coll.Where(s=>s.StateId==State.STATEACTIVE).Select(s => new ComboboxModel(s)).OrderBy(o => o.Name).ToList();
                    return coll.Select(s => new ComboboxModel(s as IBase)).OrderBy(o => o.Name).ToList();
                }
            }
            else
            {
                // Берем по myCompany
                IEnumerable<Agent> coll = WADataProvider.WA.Empty<Agent>().FindBy(myCompanyId: agentTo, name: name, useAndFilter: true,
                                                                                  filter:s =>!s.IsTemplate && s.IsStateActive, count: int.MaxValue);
                currentMyCompanies.Remove(HttpContext.Current.Session.SessionID);
                return coll.Select(s => new ComboboxModel(s as IBase)).OrderBy(o => o.Name).ToList();
            }
            return new List<ComboboxModel>();
        }

        public static object GetAgentsRangeFromView(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            int agentTo = (currentMyCompanies.ContainsKey(HttpContext.Current.Session.SessionID) ? currentMyCompanies[HttpContext.Current.Session.SessionID] : 0);
            string name = /*string.IsNullOrEmpty(args.Filter) ? null : "%" + args.Filter + "%";*/ args.Filter;
            if (string.IsNullOrEmpty(args.Filter))
                return new List<ComboboxModel>();

            if (agentTo == 0)
            {
                // Моя компания не задана, берем все доступные
                List<int> userScopeView = WADataProvider.WA.Access.GetCompanyScopeView(HttpContext.Current.User.Identity.Name);

                if (userScopeView.Count > 0)
                {
                    List<AgentWebView> coll = AgentWebView.GetView(WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_BUYERS), true).
                        Where(f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId) && f.StateId != State.STATEDENY && f.StateId != State.STATEDELETED && f.Name.Contains(name)).ToList();

                    currentMyCompanies.Remove(HttpContext.Current.Session.SessionID);
                    return coll.Select(s => new ComboboxModel(s)).OrderBy(o => o.Name).ToList();
                }
            }
            else
            {
                // Берем по myCompany
                //IEnumerable<Agent> coll = WADataProvider.WA.Empty<Agent>().FindBy(myCompanyId: agentTo, name: name, useAndFilter: true, filter: s => !s.IsTemplate && s.IsStateActive, count: int.MaxValue);
                List<AgentWebView> coll = AgentWebView.GetView(WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_BUYERS), true).
                    Where(f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId) && f.StateId != State.STATEDENY && f.StateId != State.STATEDELETED && f.Name.ToUpper().Contains(name.ToUpper())).ToList();
                currentMyCompanies.Remove(HttpContext.Current.Session.SessionID);
                return coll.Select(s => new ComboboxModel(s)).OrderBy(o => o.Name).ToList();
            }
            return new List<ComboboxModel>();
           
        }
        
        public static object GetAgentByID(ListEditItemRequestedByValueEventArgs args)
        {
            if (args.Value != null)
            {
                int id = (int)args.Value;
                return GetObject(id);
                //return (from person in DB.Persons
                //        where person.ID == id
                //        select person).SingleOrDefault();
            }
            return null;
        }
        /// <summary>
        /// Используется для дополнительного связывания в комбобоксах
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static List<ComboboxModel> GetCurrentClient(int id)
        {
            List<ComboboxModel> list = new List<ComboboxModel>();
            if (id != 0)
            {
                ClientModel obj = GetObject(id);
                if (obj.MyCompanyId!=0 & WADataProvider.IsCompanyIdAllowIdToCurrentUser(obj.MyCompanyId))
                    list.Add(new ComboboxModel { Id = id, Name = obj.Name });
            }
            return list;
        }

        private static Dictionary<int, List<ClientModel>> CacheClientsData = new Dictionary<int, List<ClientModel>>();
        /// <summary>
        /// Список клиентов в группе
        /// </summary>
        public static IEnumerable GetHierarchyContents(int hierarchyId)
        {
            if (CacheClientsData.ContainsKey(hierarchyId))
                return CacheClientsData[hierarchyId];
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(hierarchyId);

            List<AgentWebView> coll = AgentWebView.GetView(hRoot);
            CacheClientsData.Remove(hierarchyId);
            CacheClientsData.Add(hierarchyId, coll.Select(ConvertToModel).ToList());
            return CacheClientsData[hierarchyId];
        }

        /// <summary>
        /// Получение списка работников по идентификатору компании
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static List<ComboboxModel> GetWorkersByCompany(int companyId)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYWORKERS);
            List<Agent> coll = h.GetTypeContents<Agent>();
            List<ComboboxModel> ret = coll.Where(s => s.MyCompanyId == companyId).Select(s => new ComboboxModel(s as IBase)).ToList();
            return ret;
            //return h.GetTypeContents<Agent>().Where(s => s.MyCompanyId == companyId).Select(s => new ComboboxModel(s)).ToList();
        }

        public static object GetWorkersRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            string name = args.Filter;
            List<ClientModel> list = GetCollection(Hierarchy.SYSTEM_AGENT_MYWORKERS).Where(w => w.Name.ToLower().Contains(name.ToLower()) && w.StateId == State.STATEACTIVE).ToList();
            return list;
        }

        public static object GetWorkerByID(ListEditItemRequestedByValueEventArgs args)
        {
            if (args.Value != null)
            {
                int id = (int)args.Value;
                return GetObject(id);
            }
            return null;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public static void ToTrash(int id, string hierarchyCode)
        {
            Agent obj = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id);
            obj.Remove();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }
        public static void CreateCopy(int id, string hierarchyCode)
        {
            if (id != 0)
            {
                Agent obj = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id);
                Agent newObj = Agent.CreateCopy(obj);
                Hierarchy hCurrent = obj.FirstHierarchy();
                if (hCurrent != null)
                {
                    hCurrent.ContentAdd(newObj);
                }
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
                WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
            }
        }

        public static void SetStateNotDone(int id, string hierarchyCode)
        {
            Agent obj = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id, string hierarchyCode)
        {
            Agent obj = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id);
            obj.StateId = State.STATEACTIVE;
            try
            {
                obj.Save();    
            }
            catch(DatabaseException dbex)
            {
                if (dbex.Id != 0)
                {
                    ErrorLog err = WADataProvider.WA.GetErrorLog(dbex.Id);
                    if (err != null && err.Message.Contains("конфликт версий"))
                    {
                        obj = WADataProvider.WA.GetObject<Agent>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id, string hierarchyCode)
        {
            Agent obj = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheClientsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static bool CanSave(int id)
        {
            Agent a = new Agent { Workarea = WADataProvider.WA };
            a.Load(id);
            return !(a.IsSystem | a.IsReadOnly);
        }
        /// <summary>
        /// Текущее значение аттрибутов в виде строки
        /// </summary>
        /// <returns></returns>
        public string FlagsValueString()
        {
            if (Id != 0)
                return WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(Id).FlagsValueString();
            return string.Empty;
            //return ToObject(WADataProvider.WA).FlagsValueString();
        }
    }

    internal class AgentComparer : IEqualityComparer<ClientModel>
    {
        public bool Equals(ClientModel x, ClientModel y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(ClientModel obj)
        {
            return 0;//obj.GetHashCode();
        }
    }
}