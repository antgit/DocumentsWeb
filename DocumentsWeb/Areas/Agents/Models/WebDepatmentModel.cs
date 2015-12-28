using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Workflow.Activities.Rules;
using BusinessObjects;
using BusinessObjects.Models;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DevExpress.Web.ASPxEditors;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Agents.Models
{
    public class WebDepatmentModel : DepatmentModel, ICloneable, IModelData, IValidatableObject
    {
        #region Свойства
        public string InHierarchies { get; set; }
        #endregion

        #region Дополнительные модели
        //ClientChainState _chainState;

        /// <summary>Дополнительные модели</summary>
        public List<CodeValueModel> CodesCollection { get; set; }

        public List<WorkerChainModel> ChainAgents { get; set; }

        
        #endregion
        public WebDepatmentModel():base()
        {
            StateId = State.STATEACTIVE;
            ModelUserOwnerId = WADataProvider.CurrentUser.Id;
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

            foreach (WorkerChainModel a in ChainAgents)
            {
                if (a.Agent.Id == 0)
                {
                    Agent new_ag = a.Agent.ToObject(WADataProvider.WA);
                    new_ag.MyCompanyId = Id;
                    // FIXME: Проблема с сохранением в таблицу Employer и People актуальна
                    new_ag.Save();
                    if (new_ag.IsPeople && new_ag.People != null)
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
                    Hierarchy hierarchy = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("SYSTEM_AGENT_MYWORKERS");
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
        }

        public Depatment ToObject(Workarea workarea)
        {
            Depatment agent = new Depatment();
            if (Id == 0)
                agent = new Depatment { Workarea = WADataProvider.WA };
            else
                agent = workarea.Cashe.GetCasheData<Depatment>().Item(Id);

            if (agent.Id == 0)
            {
                agent.IsNew = true;
                agent.KindId = KindId;
                Uid user = (Membership.Provider as BusinessObjectMembershipProvider).GetUid(HttpContext.Current.User.Identity.Name);
                agent.MyCompanyId = MyCompanyId == 0 ? WADataProvider.CurrentUser.MyCompanyId : MyCompanyId;
            }
            agent.StateId = StateId;
            agent.UserName = HttpContext.Current.User.Identity.Name;
            agent.Id = Id;
            agent.Name = Name;
            agent.NameFull = NameFull;
            agent.Code = Code;
            agent.Memo = Memo;
            agent.CodeFind = CodeFind;
            return agent;
        }

        public static WebDepatmentModel GetObject(int id)
        {
            Depatment obj = WADataProvider.WA.Cashe.GetCasheData<Depatment>().Item(id);
            return ConvertToModel(obj);
        }

        public static WebDepatmentModel ConvertToModel(Depatment value)
        {
            WebDepatmentModel obj = new WebDepatmentModel();
            obj.GetData(value);
            obj.DefaultGroup = value.FirstHierarchy() != null ? value.FirstHierarchy().Name : string.Empty;
            
            obj.CodesCollection = value.GetValues(true).Select(CodeValueModel.ConverToModel).ToList();
            obj.InHierarchies = string.Join(",", HierarchyModel.GetHierarchiesWith<Depatment>(value).Select(s => s.Id.ToString()));

            return obj;
        }
        /// <summary>
        /// Список отделов по иерархии. Если иерархия не указана - все отделы, 
        /// в противном случае отделы по указанной иерархии.
        /// </summary>
        /// <param name="rootHierarchyCode">Строковый код иерархии</param>
        /// <param name="refresh">Выполнять обращение к серверу БД за данными</param>
        public static List<WebDepatmentModel> GetCollection(string rootHierarchyCode = null, bool refresh = false)
        {
            if (string.IsNullOrEmpty(rootHierarchyCode))
            {
                List<Depatment> collAll = WADataProvider.WA.GetCollection<Depatment>().Where(f => WADataProvider.IsCompanyIdAllowOpenToCurrentUser(f.MyCompanyId)).ToList();
                return collAll.Select(ConvertToModel).ToList();
            }


            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);

            if (!refresh)
            {
                if (WADataProvider.CacheDepatmentsModelData.ExistInCashe(h.Id))
                    return WADataProvider.CacheDepatmentsModelData.GetDataForUser(h.Id);
            }

            List<Depatment> coll = h.GetTypeContents<Depatment>(refresh: refresh);

            WADataProvider.CacheDepatmentsModelData.AddToCashe(h.Id, coll.Select(ConvertToModel).ToList());
            return WADataProvider.CacheDepatmentsModelData.GetDataForUser(h.Id);
        }

        //TODO: А если в иерархиях не проставлен код?
        public static List<WebDepatmentModel> GetCollection(string[] roots, bool refresh = false)
        {
            List<Depatment> coll = new List<Depatment>();

            foreach (string s in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(s);
                coll.InsertRange(0, h.GetTypeContents<Depatment>(true, refresh));
            }
            return coll.Select(ConvertToModel).Distinct(new DepatmentComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<WebDepatmentModel> GetCollectionWONested(string rootHierarchyCode = null)
        {
            if (string.IsNullOrEmpty(rootHierarchyCode))
            {
                return WADataProvider.WA.GetCollection<Depatment>().Where(s => !s.IsHiden).Select(ConvertToModel).ToList();
            }

            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            List<Depatment> coll = h.GetTypeContents<Depatment>(false);
            return coll.Select(ConvertToModel).Distinct(new DepatmentComparer()).OrderBy(o => o.Name).ToList();
        }

        public static List<WebDepatmentModel> GetCollectionWONested(int[] roots)
        {
            if (roots == null)
            {
                return WADataProvider.WA.GetCollection<Depatment>().Where(s => !s.IsHiden
                                                                           && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel).ToList();
            }

            List<WebDepatmentModel> coll = new List<WebDepatmentModel>();
            foreach (int item in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(item);
                coll.AddRange(h.GetTypeContents<Depatment>(false).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel));
            }
            return coll.Distinct(new DepatmentComparer()).OrderBy(o => o.Name).ToList();

        }

        public static List<WebDepatmentModel> GetWorkersCollectionWONested(int[] roots)
        {
            if (roots == null)
            {
                return WADataProvider.WA.GetCollection<Depatment>().Where(s => !s.IsHiden
                                                                           && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel).ToList();
            }

            List<WebDepatmentModel> coll = new List<WebDepatmentModel>();
            foreach (int item in roots)
            {
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(item);
                //coll.AddRange(h.GetTypeContents<Agent>(false));
                coll.AddRange(h.GetTypeContents<Depatment>(false).Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(ConvertToModel));
            }
            return coll.Distinct(new DepatmentComparer()).OrderBy(o => o.Name).ToList();

        }

        /// <summary>
        /// Список отделов по компании 
        /// </summary>
        public static IEnumerable GetCollectionByCompany(int myCompanyId)
        {
            //TODO:
            //IEnumerable<Agent> coll = WADataProvider.WA.GetCollection<Agent>().Where(s=>s.MyCompanyId==myCompanyId && !s.IsTemplate && !s.IsStateDeleted);
            IEnumerable<Depatment> coll = WADataProvider.WA.Empty<Depatment>().FindBy(myCompanyId: myCompanyId,
                                                                              filter: s => !s.IsTemplate && !s.IsStateDeleted);
            return coll.Select(s => new ComboboxModel(s)).ToList();
        }

        public static Dictionary<string, int> currentMyCompanies = new Dictionary<string, int>();
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
                    IEnumerable<Depatment> coll = WADataProvider.WA.Empty<Depatment>().FindBy(myCompanyId: userScopeView[0], name: name, useAndFilter: true,
                                                                                      filter: s => !s.IsTemplate && s.IsStateActive).Where(s => s.MyCompanyId != 0 && WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId));
                    currentMyCompanies.Remove(HttpContext.Current.Session.SessionID);
                    //return coll.Where(s=>s.StateId==State.STATEACTIVE).Select(s => new ComboboxModel(s)).OrderBy(o => o.Name).ToList();
                    return coll.Select(s => new ComboboxModel(s)).OrderBy(o => o.Name).ToList();
                }
            }
            else
            {
                // Берем по myCompany
                IEnumerable<Depatment> coll = WADataProvider.WA.Empty<Depatment>().FindBy(myCompanyId: agentTo, name: name, useAndFilter: true,
                                                                                  filter: s => !s.IsTemplate && s.IsStateActive, count: int.MaxValue);
                currentMyCompanies.Remove(HttpContext.Current.Session.SessionID);
                return coll.Select(s => new ComboboxModel(s)).OrderBy(o => o.Name).ToList();
            }
            return new List<ComboboxModel>();
           
        }

        public static object GetDepatmentByID(ListEditItemRequestedByValueEventArgs args)
        {
            if (args.Value != null)
            {
                int id = (int)args.Value;
                return GetObject(id);
               
            }
            return null;
        }

        public static List<ComboboxModel> GetCurrentDepatment(int id)
        {
            List<ComboboxModel> list = new List<ComboboxModel>();
            if (id != 0)
            {
                WebDepatmentModel obj = WebDepatmentModel.GetObject(id);
                if (obj.MyCompanyId != 0 & WADataProvider.IsCompanyIdAllowIdToCurrentUser(obj.MyCompanyId))
                    list.Add(new ComboboxModel { Id = id, Name = obj.Name });
            }
            return list;
        }

        private static Dictionary<int, List<WebDepatmentModel>> CacheDepatmentsData = new Dictionary<int, List<WebDepatmentModel>>();
        /// <summary>
        /// Список клиентов в группе
        /// </summary>
        public static IEnumerable GetHierarchyContents(int hierarchyId)
        {
            if (CacheDepatmentsData.ContainsKey(hierarchyId))
                return CacheDepatmentsData[hierarchyId];
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(hierarchyId);

            List<Depatment> coll = hRoot.GetTypeContents<Depatment>();
            CacheDepatmentsData.Remove(hierarchyId);
            CacheDepatmentsData.Add(hierarchyId, coll.Select(WebDepatmentModel.ConvertToModel).ToList());
            return CacheDepatmentsData[hierarchyId];
        }

        /// <summary>
        /// Получение списка работников по идентификатору отдела
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static List<ComboboxModel> GetWorkersByDepatment(int companyId)
        {
            Depatment obj = WADataProvider.WA.Cashe.GetCasheData<Depatment>().Item(companyId);
            ChainKind chKind =
                WADataProvider.WA.CollectionChainKinds.FirstOrDefault(
                    f => f.FromEntityId == (int) WhellKnownDbEntity.Depatment
                         && f.ToEntityId == (int) WhellKnownDbEntity.Agent);

            List<Agent> coll = ChainAdvanced<Depatment, Agent>.GetChainSourceList<Depatment, Agent>(obj, chKind.Id);

            List<ComboboxModel> ret = coll.Select(s => new ComboboxModel(s as IBase)).ToList();
                //coll.Where(s => s.MyCompanyId == companyId).Select(s => new ComboboxModel(s)).ToList();
            return ret;
            
        }

        //public static object GetDepatmentsRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        //{
        //    string name = args.Filter;
        //    List<WebDepatmentModel> list = WebDepatmentModel.GetCollection(Hierarchy.SYSTEM_AGENT_MYWORKERS).Where(w => w.Name.ToLower().Contains(name.ToLower()) && w.StateId == State.STATEACTIVE).ToList();
        //    return list;
        //}

        //public static object GetWorkerByID(ListEditItemRequestedByValueEventArgs args)
        //{
        //    if (args.Value != null)
        //    {
        //        int id = (int)args.Value;
        //        return GetObject(id);
        //    }
        //    return null;
        //}

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public static void ToTrash(int id, string hierarchyCode)
        {
            Depatment obj = WADataProvider.WA.Cashe.GetCasheData<Depatment>().Item(id);
            obj.Remove();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheDepatmentsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }
        public static void CreateCopy(int id, string hierarchyCode)
        {
            if (id != 0)
            {
                Depatment obj = WADataProvider.WA.Cashe.GetCasheData<Depatment>().Item(id);
                Depatment newObj = Depatment.CreateCopy(obj);
                Hierarchy hCurrent = obj.FirstHierarchy();
                if (hCurrent != null)
                {
                    hCurrent.ContentAdd(newObj);
                }
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
                WADataProvider.CacheDepatmentsModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
            }
        }

        public static void SetStateNotDone(int id, string hierarchyCode)
        {
            Depatment obj = WADataProvider.WA.Cashe.GetCasheData<Depatment>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheDepatmentsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id, string hierarchyCode)
        {
            Depatment obj = WADataProvider.WA.Cashe.GetCasheData<Depatment>().Item(id);
            obj.StateId = State.STATEACTIVE;
            try
            {
                obj.Save();
            }
            catch (DatabaseException dbex)
            {
                if (dbex.Id != 0)
                {
                    ErrorLog err = WADataProvider.WA.GetErrorLog(dbex.Id);
                    if (err != null && err.Message.Contains("конфликт версий"))
                    {
                        obj = WADataProvider.WA.GetObject<Depatment>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheDepatmentsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id, string hierarchyCode)
        {
            Depatment obj = WADataProvider.WA.Cashe.GetCasheData<Depatment>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheDepatmentsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static bool CanSave(int id)
        {
            Depatment a = new Depatment { Workarea = WADataProvider.WA };
            a.Load(id);
            return !(a.IsSystem | a.IsReadOnly);
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult("Наименование отдела обязательно", new[] { GlobalPropertyNames.Name });
            }
            if (MyCompanyId==0)
            {
                yield return new ValidationResult("Укажите предприятие", new[] { GlobalPropertyNames.MyCompanyId });
            }

            Type targetType = this.GetType();
            string fulltypename = targetType.FullName;
            IEnumerable<Ruleset> collRules = WADataProvider.WA.GetCollection<Ruleset>().Where(s => s.ActivityName == fulltypename && s.StateId == 1 && s.KindValue == Ruleset.KINDVALUE_WEBRULESET);
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
                            yield return new ValidationResult(Errors[k], new[] { k });                            
                        }
                    }
                }
            }
            //if (StartDate.TimeOfDay > new TimeSpan(22 - DurationInHours, 0, 0))
            //{
            //    yield return new ValidationResult("The party should not exceed after 10.00 PM");
            //}

            //if (NoOfJoinees < 5 && Drinks)
            //{
            //    yield return new ValidationResult("Drinks are only allowed if no. of joinees is 5 or more.");
            //}

            /*
             var grouping = this.ListEntries.GroupBy(listEntry => listEntry.MenuItemName.ToLowerInvariant());
            var duplicates = grouping.Where(group => group.Count() > 1);
            foreach(var duplicate in duplicates)
            {
                yield return new ValidationResult(
                    string.Format("You entered {0} {1} times", duplicate.Key, duplicate.Count()),
                    new [] {"ListEntries"}
                    );
            }
             */
        }

        /// <summary>
        /// Текущее значение аттрибутов в виде строки
        /// </summary>
        /// <returns></returns>
        public string FlagsValueString()
        {
            return ToObject(WADataProvider.WA).FlagsValueString();
        }

    }
    internal class DepatmentComparer : IEqualityComparer<WebDepatmentModel>
    {
        public bool Equals(WebDepatmentModel x, WebDepatmentModel y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(WebDepatmentModel obj)
        {
            return 0;//obj.GetHashCode();
        }
    }
}