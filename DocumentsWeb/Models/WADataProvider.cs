using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.Security;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Models;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.Analitics.Models;
using DocumentsWeb.Areas.Prices.Models;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.Admins.Models;
using DocumentsWeb.Areas.Kb.Models;
using DocumentsWeb.Areas.Routes.Models;
using DocumentsWeb.Areas.UserPersonal.Models;
using ProductModel = DocumentsWeb.Areas.Products.Models.ProductModel;

namespace DocumentsWeb.Models
{
    internal class LastActionCache
    {
        private Dictionary<string, LastActionCacheItem> _values;

        internal class LastActionCacheItem
        {
            /// <summary>
            /// Дата
            /// </summary>
            public DateTime Date { get; set; }
            /// <summary>
            /// Ключ действия
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// Значение
            /// </summary>
            public string Value { get; set; }
        }

        public LastActionCache()
        {
            Values = new Dictionary<string, LastActionCacheItem>();
        }

        private Dictionary<string, LastActionCacheItem>  Values
        {
            get
            {
                if (_values == null)
                    _values = new Dictionary<string, LastActionCacheItem>();
                return _values;
            }
            set { _values = value; }
        }

        public bool Exists(string key)
        {
            if (string.IsNullOrEmpty(key))
                return false;
            return Values.ContainsKey(key);
        }

        public string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
                return string.Empty;
            if (Values.ContainsKey(key))
            {
                return Values[key].Value;
            }
            return string.Empty;
        }
        public DateTime? GetLastDate(string key)
        {
            if (string.IsNullOrEmpty(key))
                return (DateTime?)null;
            if (Values.ContainsKey(key))
            {
                return _values[key].Date;
            }
            return (DateTime?)null;
        }
        public void SetValue(string key, string value)
        {
            if(Values.ContainsKey(key))
            {
                Values[key].Date = DateTime.Now;
                Values[key].Value = value;
            }
            else
            {
                Values.Add(key, new LastActionCacheItem {Date = DateTime.Now, Key = key, Value = value});
            }
        }
    }

    public class WADataProvider
    {
        //@*ElementRightView secureEntityHierarchy = Program.WA.Access.ElementRightView(28);
        //    ElementRightView secureLibrary = Program.WA.Access.ElementRightView(15);
        //    Hierarchy rootUI = Program.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("UI_DOCUMENTS2011");*@
        
        //    <sessionState mode="InProc" cookieless="false" timeout="20"> 
        //</sessionState> 

        //        Session["username"] = userName; 

        // and I retrieve it like this
        //string user_id = (string)Session["username"]; 

        //Yes, you can access Application variables from .NET MVC. Here's how:
        //System.Web.HttpContext.Current.Application.Lock(); 
        //System.Web.HttpContext.Current.Application["Name"] = "Value"; 
        //System.Web.HttpContext.Current.Application.UnLock(); 


        // Глобальный кэш моделей
        //private static ModelCacheDictionary _modelsCache = new ModelCacheDictionary();
        //public static ModelCacheDictionary ModelsCache { get { return _modelsCache; } }

        internal static LastActionCache LastActionCashe
        {
            get
            {
                if (_lastActionCashe == null)
                    _lastActionCashe = new LastActionCache();
                return _lastActionCashe;
            }
            set { _lastActionCashe = value; }
        }

        public static ModelCashe ModelsCache
        {
            get { return WA.Cashe.ModelCashe; }
        }

        

        private static SysConfigModel _sysConfig;

        /// <summary>
        /// Кеш для хранения системных параметров
        /// </summary>
        public static SysConfigModel SysConfig
        {
            get
            {
                if(_sysConfig==null)
                {
                    SysConfigModel sysConfigModel=new SysConfigModel();
                    sysConfigModel.Load();
                    _sysConfig = sysConfigModel;

                }
                return _sysConfig;
            }

            set
            {
                _sysConfig = value;
            }
        }

        private static AccountConfigModel _accountConfig;
        /// <summary>
        /// Кеш для хранения системных параметров
        /// </summary>
        public static AccountConfigModel AccountConfig
        {
            get
            {
                if (_accountConfig == null)
                {
                    AccountConfigModel sysConfigModel = new AccountConfigModel();
                    sysConfigModel.Load();
                    _accountConfig = sysConfigModel;

                }
                return _accountConfig;
            }

            set
            {
                _accountConfig = value;
            }
        }

        private static Dictionary<string, CommonRightView> storedCommonRightView =
            new Dictionary<string, CommonRightView>();
        
        public static CommonRightView CommonRightView
        {
            get
            {
                lock (storedCommonRightView)
                {
                    if (storedCommonRightView.ContainsKey(HttpContext.Current.User.Identity.Name))
                        return storedCommonRightView[HttpContext.Current.User.Identity.Name];

                    RefreshCommonRightView(HttpContext.Current.User.Identity.Name);

                    if (storedCommonRightView.ContainsKey(HttpContext.Current.User.Identity.Name))
                        return storedCommonRightView[HttpContext.Current.User.Identity.Name];
                }
                return new CommonRightView(WA);
            }
        }

        public static void RefreshCommonRightView(string uidName)
        {
            lock (storedCommonRightView)
            {
                if (storedCommonRightView.ContainsKey(uidName))
                    storedCommonRightView.Remove(uidName);
                Uid user = WA.Access.GetAllUsers().FirstOrDefault(w => w.Name == uidName); // WA.GetCollection<Uid>().First(w=>w.Name == uidName);
                if(user!=null)
                {
                    CommonRightView coll = (CommonRightView)WA.Access.GetCommonRights(user);
                    storedCommonRightView.Add(uidName, coll);
                }
                
            }
        }

        #region Пользователь
        /// <summary>
        /// Текущий пользователь системы
        /// </summary>
        public static Uid CurrentUser
        {
            get
            {
                Uid user = WA.Access.GetAllUsers().FirstOrDefault(w => w.Code == HttpContext.Current.User.Identity.Name);//WA.GetCollection<Uid>().First(w => w.Name == HttpContext.Current.User.Identity.Name);
                if(user==null)
                    user = WA.Access.GetAllUsers(true).FirstOrDefault(w => w.Code == HttpContext.Current.User.Identity.Name);
                return user;
            }
        }
        /// <summary>
        /// Системный пользователь
        /// </summary>
        /// <returns></returns>
        public static Uid SystemInternalUser()
        {
            return WA.Access.GetAllUsers().FirstOrDefault(w => w.Code == "SYSTEMINTENAL");
        }
        /// <summary>
        /// Текущий пользователь системы на основе HttpContext
        /// </summary>
        public static string CurrentUserName
        {
            get { return HttpContext.Current.User.Identity.Name; }
        }
        /// <summary>
        /// Текущий пользователь системы на основе HttpContext
        /// </summary>
        public static MembershipUser CurrentMembershipUser
        {
            get
            {
                return Membership.GetUser(true);
            }
        }

        private static Dictionary<string, DateTime> _lastUserActivity = new Dictionary<string,DateTime>();
        /// <summary>
        /// Обновление информации о последней активности пользователя
        /// </summary>
        public static void OnUserActivity(string remoteAddr=null)
        {
            if(CurrentUser!=null)
            {
                if (_lastUserActivity!=null)
                {
                    if (_lastUserActivity.ContainsKey(CurrentUserName))
                    {
                        DateTime v = _lastUserActivity[CurrentUserName];
                        if(v.AddMinutes(5)<DateTime.Now)
                        {
                            CurrentUser.SetLastActivityDate(DateTime.Now, remoteAddr);
                            _lastUserActivity[CurrentUserName] = DateTime.Now;
                        }
                    }
                    else
                    {
                        CurrentUser.SetLastActivityDate(DateTime.Now, remoteAddr);
                        _lastUserActivity.Add(CurrentUserName, DateTime.Now);
                    }
                }
                
            }
        }

        /// <summary>
        /// Список разрешенных компаний для просмотра для текущего пользователя
        /// </summary>
        /// <remarks>Используется только  в списках справочников и документов - пользователь может просматривать данные в списках только для разрешенных компаний</remarks>
        /// <returns></returns>
        private static List<int> GetCompanyScopeViewCurrentUser()
        {
            return WA.Access.GetCompanyScopeView(HttpContext.Current.User.Identity.Name);
        }
        /// <summary>
        /// Список разрешенных компаний для редактирования данных для текущего пользователя
        /// </summary>
        /// <remarks>Используется в справочниках и документах - пользователь может редактировать данные только для разрешенных компаний</remarks>
        /// <returns></returns>
        private static List<int> GetCompanyScopeEditCurrentUser()
        {
            return WA.Access.GetCompanyScopeEdit(HttpContext.Current.User.Identity.Name);
        }
        /// <summary>
        /// Разрешенo ли изменение данных для указанной компании для текущего пользователя
        /// </summary>
        /// <param name="myCompanyId"></param>
        /// <returns></returns>
        public static bool IsCompanyIdAllowEditToCurrentUser(int myCompanyId)
        {
            return GetCompanyScopeEditCurrentUser().Contains(myCompanyId);
        }
        /// <summary>
        /// Список разрешенных компаний для создания данных для текущего пользователя
        /// </summary>
        /// <remarks>Используется в справочниках и документах - пользователь может создавать данные только для разрешенных компаний</remarks>
        /// <returns></returns>
        private static List<int> GetCompanyScopeCreateCurrentUser()
        {
            return WA.Access.GetCompanyScopeCreate(HttpContext.Current.User.Identity.Name);
        }
        /// <summary>
        /// Разрешенo ли создание данных для указанной компании для текущего пользователя
        /// </summary>
        /// <param name="myCompanyId"></param>
        /// <returns></returns>
        public static bool IsCompanyIdAllowCreateToCurrentUser(int myCompanyId)
        {
            return GetCompanyScopeCreateCurrentUser().Contains(myCompanyId);
        }
        /// <summary>
        /// Список разрешенных компаний для открытия данных для текущего пользователя
        /// </summary>
        /// <remarks>Используется в справочниках и документах - пользователь может просматривать данные, свойства и аттрибуты конкретного объекта (может открыть окно или страницу свойств)  только для разрешенных компаний</remarks>
        /// <returns></returns>
        private static List<int> GetCompanyScopeOpenCurrentUser()
        {
            return WA.Access.GetCompanyScopeEdit(HttpContext.Current.User.Identity.Name);
        }
        /// <summary>
        /// Разрешен ли просмотр данных указанной компании для текущего пользователя
        /// </summary>
        /// <param name="myCompanyId"></param>
        /// <returns></returns>
        public static bool IsCompanyIdAllowOpenToCurrentUser(int myCompanyId)
        {
            return GetCompanyScopeOpenCurrentUser().Contains(myCompanyId);
        }
        /// <summary>
        /// Проверка разрешения по области видимости
        /// </summary>
        /// <param name="myCompanyId"></param>
        /// <returns></returns>
        public static bool IsCompanyIdAllowIdToCurrentUser(int myCompanyId)
        {
            return GetCompanyScopeViewCurrentUser().Contains(myCompanyId);
        }
        /// <summary>Наличие нескольких предприятий в области видимости текущего пользователя</summary>
        /// <returns></returns>
        public static bool CurrentUserCompanyMultyCount()
        {
            return GetCompanyScopeViewCurrentUser().Where(f=>f!=-1).ToList().Count > 1;
        }
        /// <summary>
        /// Компания пользователя является плательщиком НДС?
        /// </summary>
        /// <returns></returns>
        public static bool IsDefaultCompanyHasNds()
        {
            if (CurrentUser.MyCompanyId != 0)
            {
                if (CurrentUser.MyCompany.Company != null)
                    return CurrentUser.MyCompany.Company.NdsPayer;
                return false;
            }
            return false;
        }
        /// <summary>
        /// Является ли хотя бы одна компания из доступных пользователю пользователя плательщиком НДС
        /// </summary>
        /// <returns></returns>
        public static bool IsUserCompanyHasNds()
        {
            if (!CurrentUserCompanyMultyCount())
            {
                if (CurrentUser.MyCompanyId != 0)
                    return CurrentUser.MyCompany.Company.NdsPayer;
                else return false;
            }
            else
            {
                List<int> allowedCompany = GetCompanyScopeViewCurrentUser();
                foreach (int id in allowedCompany)
                {
                    if (WA.Cashe.GetCasheData<Agent>().Item(id).Company.NdsPayer)
                        return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Является ли хотя бы одна компания из доступных пользователю пользователя не плательщиком НДС
        /// </summary>
        /// <returns></returns>
        public static bool IsUserCompanyHasNoNds()
        {
            if (!CurrentUserCompanyMultyCount())
            {
                return !CurrentUser.MyCompany.Company.NdsPayer;
            }
            else
            {
                List<int> allowedCompany = GetCompanyScopeViewCurrentUser();
                foreach (int id in allowedCompany)
                {
                    if (!WA.Cashe.GetCasheData<Agent>().Item(id).Company.NdsPayer)
                        return true;
                }
                return false;
            }
        }
        #endregion

        //HttpContext.Current.Session["WADataProvider"]
        private static Dictionary<string, ElementRightView> storedLibrariesElementRightView =
            new Dictionary<string, ElementRightView>();
        public static ElementRightView LibrariesElementRightView
        {
            get
            {
                lock (storedLibrariesElementRightView)
                {
                    if (storedLibrariesElementRightView.ContainsKey(HttpContext.Current.User.Identity.Name))
                        return storedLibrariesElementRightView[HttpContext.Current.User.Identity.Name];

                    RefreshLibrariesElementRightView(HttpContext.Current.User.Identity.Name);

                    if (storedLibrariesElementRightView.ContainsKey(HttpContext.Current.User.Identity.Name))
                        return storedLibrariesElementRightView[HttpContext.Current.User.Identity.Name];
                }
                return new ElementRightView();
            }
        }

        public static void RefreshLibrariesElementRightView(string uidName)
        {
            lock (storedLibrariesElementRightView)
            {
                if (storedLibrariesElementRightView.ContainsKey(uidName))
                    storedLibrariesElementRightView.Remove(uidName);
                ElementRightView coll = WA.Access.ElementRightView((int)WhellKnownDbEntity.Library, uidName);
                storedLibrariesElementRightView.Add(uidName, coll);
            }
        }

        private static Dictionary<string, ElementRightView> storedFolderElementRightView =
            new Dictionary<string, ElementRightView>();
        public static ElementRightView FolderElementRightView
        {
            get
            {
                lock (storedFolderElementRightView)
                {
                    if (storedFolderElementRightView.ContainsKey(HttpContext.Current.User.Identity.Name))
                        return storedFolderElementRightView[HttpContext.Current.User.Identity.Name];

                    RefreshFoldersElementRightView(HttpContext.Current.User.Identity.Name);

                    if (storedFolderElementRightView.ContainsKey(HttpContext.Current.User.Identity.Name))
                        return storedFolderElementRightView[HttpContext.Current.User.Identity.Name];
                }
                return new ElementRightView();
            }
        }

        public static void RefreshFoldersElementRightView(string uidName)
        {
            lock (storedFolderElementRightView)
            {
                if (storedFolderElementRightView.ContainsKey(uidName))
                    storedFolderElementRightView.Remove(uidName);
                ElementRightView coll = WA.Access.ElementRightView((int)WhellKnownDbEntity.Folder, uidName);
                storedFolderElementRightView.Add(uidName, coll);
            }
        }

        private static Dictionary<string, ElementRightView> storedHiearchyElementRightView =
            new Dictionary<string, ElementRightView>();

        public static ElementRightView HiearchyElementRightView
        {
            get
            {
                lock (storedHiearchyElementRightView)
                {
                    if (storedHiearchyElementRightView.ContainsKey(HttpContext.Current.User.Identity.Name))
                        return storedHiearchyElementRightView[HttpContext.Current.User.Identity.Name];

                    RefreshHiearchyElementRightView(HttpContext.Current.User.Identity.Name);

                    if (storedHiearchyElementRightView.ContainsKey(HttpContext.Current.User.Identity.Name))
                        return storedHiearchyElementRightView[HttpContext.Current.User.Identity.Name];
                }
                return new ElementRightView();
            }
        }

        public static void RefreshHiearchyElementRightView(string uidName)
        {
            lock (storedHiearchyElementRightView)
            {
                if (storedHiearchyElementRightView.ContainsKey(uidName))
                    storedHiearchyElementRightView.Remove(uidName);
                ElementRightView coll = WA.Access.ElementRightView((int)WhellKnownDbEntity.Hierarchy, uidName);
                storedHiearchyElementRightView.Add(uidName, coll);
            }
        }

        private static Dictionary<int, PeriodModel> _periods = new Dictionary<int,PeriodModel>();
        /// <summary>
        /// Системный период текущего пользователя
        /// </summary>
        public static PeriodModel Period
        {
            get
            {
                int user_id = CurrentUser.Id;
                if (_periods.ContainsKey(user_id))
                {
                    return _periods[user_id];
                }
                else
                {
                    PeriodModel period = new PeriodModel();
                    _periods.Add(user_id, period);
                    return period;
                }
            }
        }

        static Workarea wa;

        /// <summary>
        /// Рабочая область
        /// </summary>
        public static Workarea WA
        {
            get
            {
                if (wa == null)
                {
                    Uid uid = null;
                    wa = OpenDataBase(out uid);
                    wa.Period.SetYearRange(2011,2012);
                    wa.IsWebApplication = true;
                }
                return wa;
            }

            set
            {
                wa = value;
            }
        }

        /// <summary>
        /// Открыть базу данных
        /// </summary>
        private static Workarea OpenDataBase(out Uid user)
        {
            string cnnStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["Document2011"].ConnectionString;
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(cnnStr);

            user = new Uid { Name = builder.UserID, Password = string.Empty, AuthenticateKind = 0 };


            Workarea WA = new Workarea();
            WA.ConnectionString = builder.ConnectionString;
            try
            {
                if (WA.LogOn(user.Name))
                {
                    return WA;
                }
            }
            catch (SqlException)
            {

            }
            catch (Exception)
            {

            }
            return null;
        }

        // Объект используемый для кеширования корреспондентов по идентификатору иерархии
        internal static HierarchyUserDataCashe<WebDepatmentModel> CacheDepatmentsModelData = new HierarchyUserDataCashe<WebDepatmentModel>();
        internal static HierarchyUserDataCashe<ClientModel> CacheClientsModelData = new HierarchyUserDataCashe<ClientModel>();
        internal static HierarchyUserDataCashe<ProductModel> CacheProductsModelData = new HierarchyUserDataCashe<ProductModel>();
        internal static HierarchyUserDataCashe<DocumentsWeb.Areas.Analitics.Models.AnaliticModel> CacheAnaliticsModelData = new HierarchyUserDataCashe<DocumentsWeb.Areas.Analitics.Models.AnaliticModel>();
        internal static HierarchyUserDataCashe<DeviceModel> CacheDevicesModelData = new HierarchyUserDataCashe<DeviceModel>();
        internal static HierarchyUserDataCashe<RouteMemberModel> CacheRouteMembersModelData = new HierarchyUserDataCashe<RouteMemberModel>();
        internal static HierarchyUserDataCashe<DocumentsWeb.Areas.Kb.Models.KnowledgeModel> CacheKnowledgeModelData = new HierarchyUserDataCashe<DocumentsWeb.Areas.Kb.Models.KnowledgeModel>();
        internal static HierarchyUserDataCashe<DocumentsWeb.Areas.Kb.Models.TaskModel> CacheTaskModelData = new HierarchyUserDataCashe<DocumentsWeb.Areas.Kb.Models.TaskModel>();
        private static LastActionCache _lastActionCashe;

        /// <summary>
        /// Список корреспондентов, являющихся нашим холдингом или объединением
        /// </summary>
        /// <returns></returns>
        public static IEnumerable GetAgentHoldings()
        {
            Hierarchy hRoot = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYCOMPANY);

            if (CacheClientsModelData.ContainsKey(hRoot.Id))
                return CacheClientsModelData.GetDataForUser(hRoot.Id);

            List<AgentWebView> coll = AgentWebView.GetView(hRoot, true);
            //CacheClientsModelData.Remove(hRoot.Id);
            CacheClientsModelData.AddToCashe(hRoot.Id, coll.Select(ClientModel.ConvertToModel).ToList());
            return CacheClientsModelData.GetDataForUser(hRoot.Id);
            //List<Agent> coll = hRoot.GetTypeContents<Agent>();
            //return coll.Select(ConvertToModel).ToList();
        }
        /// <summary>
        /// Список корреспондентов являющихся нашими предприятими для указанного холдинга
        /// </summary>
        /// <param name="holdingId"></param>
        /// <returns></returns>
        public static IEnumerable GetAgentMyCompanies(int holdingId)
        {
            Agent agHolding = WA.Cashe.GetCasheData<Agent>().Item(holdingId);
            int DepatmentChainId = WA.CollectionChainKinds.FirstOrDefault(s => s.Code == ChainKind.TREE && s.FromEntityId == (int)WhellKnownDbEntity.Agent).Id;


            List<Agent> coll = Chain<Agent>.GetChainSourceList(agHolding, DepatmentChainId); //agHolding.GetChainSourceList(DepatmentChainId);
            return coll.Select(ConvertToModel).ToList();
        }

        /// <summary>
        /// Список корреспондентов являющихся нашими предприятими для первого холдинга
        /// </summary>
        /// <returns></returns>
        public static IEnumerable GetAgentMyCompanies()
        {
            Hierarchy hRoot = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_AGENT_MYCOMPANY);


            Agent agHolding = WA.Cashe.GetCasheData<Agent>().Item(hRoot.GetTypeContents<Agent>()[0].Id);
            int DepatmentChainId = WA.CollectionChainKinds.FirstOrDefault(s => s.Code == ChainKind.TREE && s.FromEntityId == (int)WhellKnownDbEntity.Agent).Id;


            List<Agent> coll = Chain<Agent>.GetChainSourceList(agHolding, DepatmentChainId); //agHolding.GetChainSourceList(DepatmentChainId);
            List<ClientModel> collRet = new List<ClientModel>();
            foreach (Agent obj in coll)
            {
                collRet.Add(ConvertToModel(obj));
            }
            return collRet;
        }

        public static HieModel GetHierarchy(int id)
        {
            Hierarchy obj = WA.GetObject<Hierarchy>(id);
            return ConvertToModel(obj);
        }
        
        public static HieModel GetHieByCode(string code) //"SYSTEM_BANKS" , SYSTEM_AGENT_ROOT
        {
            Hierarchy obj = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(code);
            return ConvertToModel(obj);
        }

        public static List<HieModel> GetHieByParentId(int parentId) //"SYSTEM_BANKS"
        {
            List<HieModel> coll = new List<HieModel>();
            Hierarchy objRoot = WA.Cashe.GetCasheData<Hierarchy>().Item(parentId);
            
            foreach(Hierarchy h in objRoot.Children)
            {
                coll.Add(ConvertToModel(h));
            }
            return coll;
        }
        
        /// <summary>
        /// Преобразование корреспондента
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static ClientModel ConvertToModel(Agent value)
        {
            ClientModel obj = new ClientModel { Id = value.Id, Name = value.Name, NameFull= value.NameFull, Code=value.Code, Memo = value.Memo, TaxNumber = value.CodeTax };
            if (value.Memo != null && value.Memo.Length > 100)
                obj.DisplayMemo = value.Memo.Substring(0, 100) +"...";
            else
                obj.DisplayMemo = value.Memo;
            if(value.IsCompany && value.Company!=null)
            {
                obj.Okpo = value.Company.Okpo;
                if (value.Company.Ownership != null)
                    obj.OwnerShip = value.Company.Ownership.Code;
                obj.OwnerShipId = value.Company.OwnershipId;
            }
            return obj;
        }

        private static HieModel ConvertToModel(Hierarchy value)
        {
            HieModel obj = new HieModel { Id = value.Id, Name = value.Name, Memo = value.Memo, Code = value.Code };
            
            return obj;
        }


        /// <summary>
        /// Список документов по папке
        /// </summary>
        /// <remarks> 601 - Оплата от покупателя
        /// 603 - Оплата поставщику
        /// 201 - Расходная накладная
        /// 202 - Приход товара
        /// </remarks>
        /// <param name="id">Идентификатор папки</param>
        /// <returns></returns>
        public static IEnumerable GetDocumentsByFolder(int? id)
        {
            List<int> userScopeView = WADataProvider.WA.Access.GetCompanyScopeView(HttpContext.Current.User.Identity.Name);
            List<Document> coll = id.HasValue
                                      ? Document.GetCollectionDocumentByFolder(WA, id.Value).Where(s => !s.IsStateDeleted && (userScopeView.Contains(s.MyCompanyId) || HttpContext.Current.User.IsInRole(Uid.GROUP_GROUPWEBADMIN))).ToList<Document>()
                                      : new List<Document>();
            return coll.Select(DocumentModel.ConvertToModel);
        }

        /// <summary>
        /// Список документов указанного типа в разделе
        /// </summary>
        /// <param name="typeValue"></param>
        /// <param name="folderCodeFind">Код поиска папки</param>
        /// <returns></returns>
        public static IEnumerable GetDocumentsByKindValue(int typeValue, string folderCodeFind)
        {
            return Document.GetCollectionDocumentByKind(WA, typeValue, Period.periodStart, Period.periodEnd,
                                                      HttpContext.Current.User.Identity.Name, folderCodeFind).Where(w => w.StateId != State.STATEDELETED).Select(DocumentModel.ConvertToModel);
        }
        
        //public static IEnumerable GetDocumentsFinanceIn()
        //{
        //    Folder fld = WA.Cashe.GetCasheData<Folder>().ItemCode<Folder>("601");
        //    return GetDocumentsByFolder(fld.Id);
        //}
        //public static IEnumerable GetDocumentsFinanceOut()
        //{
        //    Folder fld = WA.Cashe.GetCasheData<Folder>().ItemCode<Folder>("603");
        //    return GetDocumentsByFolder(fld.Id);
        //}
        //public static DocumentModel GetDocument(int id)
        //{
        //    Document doc = new Document {Workarea = WA};
        //    doc.Load(id);
        //    return DocumentModel.ConvertToModel(doc);
        //}
        /// <summary>
        /// Список документов по корреспонденту
        /// </summary>
        /// <param name="id">Идентификатор корреспондента</param>
        /// <returns></returns>
        public static IEnumerable GetDocumentsByAgent(int id)
        {
            List<Document> coll = Document.GetCollectionDocumentByAgent(WA, id);
            List<DocumentModel> collRet = new List<DocumentModel>();
            foreach (Document obj in coll)
            {
                collRet.Add(DocumentModel.ConvertToModel(obj));
            }
            return collRet;
        }

        #region Навигатор
        public static List<ViewFolders> GetViewFolders(string rootHierarchyCode)
        {
            Hierarchy rootData = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(rootHierarchyCode);
            List<Folder> folders = rootData.GetTypeContents<Folder>();
            return folders.Select(f => new ViewFolders { Id = f.Id, Name = f.Name, HierarchyName = rootData.Name, Memo = f.Memo, Folder = f, 
                FormCode = f.ProjectItem==null? string.Empty: f.ProjectItem.Code,
                DocumentTemplateId = f.DocumentId,
                Hierarchy = rootData, Kind = 7 }).ToList();
        }
        public static int ScopeViewListId
        {
            get
            {
                return WA.CollectionChainKinds.First(
                s =>
                s.Code == "SCOPEVIEWLIST" && s.FromEntityId == (int)WhellKnownDbEntity.Hierarchy &&
                s.ToEntityId == (int)WhellKnownDbEntity.Agent).Id;
            }
        }
        public static List<ViewFolders> GetViewFolders()
        {
            Uid user = (Membership.Provider as BusinessObjectMembershipProvider).GetUid(
                System.Web.HttpContext.Current.User.Identity.Name);

            //Agent companyOwner = ((IChains<Agent>) user.Agent).DestinationList(WA.WorkresChainId()).FirstOrDefault();

            //UserModel.ConvertToModel(user).CompanyId;
            
            // SCOPEVIEWLIST - тип связи для видимости групп
            

            // Области видимости для пользователя в целов
            //List<IChainAdvanced<Uid, Agent>> userScope = user.GetLinkedAgents();
            List<int> userScopeView = GetCompanyScopeViewCurrentUser();

            #region Финансовый документы
            Hierarchy rootData = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_FOLDER_FINANCE);
            // Область видимости для иерархии
            List<IChainAdvanced<Hierarchy, Agent>> hierarchyScope = rootData.GetLinkedHierarchy().Where(s => s.KindId == ScopeViewListId).ToList();

           // List<int> hierarchyScopeIdList = WA.Access.HierarchyCompanyScopeViewContext(System.Web.HttpContext.Current.User.Identity.Name, rootData.Id);
            

            bool allow = userScopeView.Exists(s => hierarchyScope.Exists(f => f.RightId == s));
            //bool allow = userScope.Exists(s => hierarchyScope.Exists(f => f.RightId == s.RightId));
            List<Folder> folders = new List<Folder>();
            List<ViewFolders> viewFinance = new List<ViewFolders>();
            if (allow)
            {
                folders = rootData.GetTypeContents<Folder>();
                viewFinance = folders.Select(f => new ViewFolders
                                                                        {
                                                                            Id = f.Id,
                                                                            Name = f.Name,
                                                                            HierarchyName = rootData.Name,
                                                                            Memo = f.Memo,
                                                                            Folder = f,
                                                                            FormCode =
                                                                                f.ProjectItem == null
                                                                                    ? string.Empty
                                                                                    : f.ProjectItem.Code,
                                                                            DocumentTemplateId = f.DocumentId,
                                                                            Hierarchy = rootData,
                                                                            Kind = 7
                                                                        }).ToList().Where(s => WADataProvider.FolderElementRightView.IsAllow("VIEW", s.FolderId)).ToList();

            }
            #endregion

            #region Tax
            rootData = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_FOLDER_TAX);
            hierarchyScope = rootData.GetLinkedHierarchy().Where(s => s.KindId == ScopeViewListId).ToList();
            allow = userScopeView.Exists(s => hierarchyScope.Exists(f => f.RightId == s));
            
            List<ViewFolders> viewTax = new List<ViewFolders>();
            if (allow)
            {
                folders = rootData.GetTypeContents<Folder>();
                viewTax = folders.Select(f => new ViewFolders
                                                  {
                                                      Id = f.Id,
                                                      Name = f.Name,
                                                      HierarchyName = rootData.Name,
                                                      Memo = f.Memo,
                                                      Folder = f,
                                                      FormCode =
                                                          f.ProjectItem == null ? string.Empty : f.ProjectItem.Code,
                                                      DocumentTemplateId = f.DocumentId,
                                                      Hierarchy = rootData,
                                                      Kind = 7
                                                  }).ToList().Where(s => WADataProvider.FolderElementRightView.IsAllow("VIEW", s.FolderId)).ToList();
            }
            #endregion

            #region Sales
            rootData = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_FOLDER_SALES);
            hierarchyScope = rootData.GetLinkedHierarchy().Where(s => s.KindId == ScopeViewListId).ToList();
            allow = userScopeView.Exists(s => hierarchyScope.Exists(f => f.RightId == s));
            List<ViewFolders> viewSales = new List<ViewFolders>();
            if (allow)
            {
                folders = rootData.GetTypeContents<Folder>();
                viewSales = folders.Select(f => new ViewFolders
                                                    {
                                                        Id = f.Id,
                                                        Name = f.Name,
                                                        HierarchyName = rootData.Name,
                                                        Memo = f.Memo,
                                                        Folder = f,
                                                        FormCode =
                                                            f.ProjectItem == null ? string.Empty : f.ProjectItem.Code,
                                                        DocumentTemplateId = f.DocumentId,
                                                        Hierarchy = rootData,
                                                        Kind = 7
                                                    }).ToList().Where(s => WADataProvider.FolderElementRightView.IsAllow("VIEW", s.FolderId)).ToList();
            }
            #endregion

            #region SalesNoNDS
            rootData = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_FOLDER_SALESNONDS);
            hierarchyScope = rootData.GetLinkedHierarchy().Where(s => s.KindId == ScopeViewListId).ToList();
            allow = userScopeView.Exists(s => hierarchyScope.Exists(f => f.RightId == s));
            List<ViewFolders> viewSalesNoNds = new List<ViewFolders>();
            if (allow)
            {
                folders = rootData.GetTypeContents<Folder>();
                viewSalesNoNds = folders.Select(f => new ViewFolders
                {
                    Id = f.Id,
                    Name = f.Name,
                    HierarchyName = rootData.Name,
                    Memo = f.Memo,
                    Folder = f,
                    FormCode =
                        f.ProjectItem == null ? string.Empty : f.ProjectItem.Code,
                    DocumentTemplateId = f.DocumentId,
                    Hierarchy = rootData,
                    Kind = 7
                }).ToList().Where(s => WADataProvider.FolderElementRightView.IsAllow("VIEW", s.FolderId)).ToList();
            }
            #endregion

            #region Price
            rootData = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_FOLDER_PRICEPOLICY);
            hierarchyScope = rootData.GetLinkedHierarchy().Where(s => s.KindId == ScopeViewListId).ToList();
            allow = userScopeView.Exists(s => hierarchyScope.Exists(f => f.RightId == s));
            List<ViewFolders> viewPrices = new List<ViewFolders>();
            if (allow)
            {
                folders = rootData.GetTypeContents<Folder>();
                viewPrices = folders.Select(f => new ViewFolders
                {
                    Id = f.Id,
                    Name = f.Name,
                    HierarchyName = rootData.Name,
                    Memo = f.Memo,
                    Folder = f,
                    FormCode =
                        f.ProjectItem == null ? string.Empty : f.ProjectItem.Code,
                    DocumentTemplateId = f.DocumentId,
                    Hierarchy = rootData,
                    Kind = 7
                }).ToList().Where(s => WADataProvider.FolderElementRightView.IsAllow("VIEW", s.FolderId)).ToList();
            }
            #endregion

            #region Service
            rootData = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_FOLDER_SERVICE);
            hierarchyScope = rootData.GetLinkedHierarchy().Where(s => s.KindId == ScopeViewListId).ToList();
            allow = userScopeView.Exists(s => hierarchyScope.Exists(f => f.RightId == s));
            List<ViewFolders> viewService = new List<ViewFolders>();
            if (allow)
            {
                folders = rootData.GetTypeContents<Folder>();
                viewService = folders.Select(f => new ViewFolders
                                                      {
                                                          Id = f.Id,
                                                          Name = f.Name,
                                                          HierarchyName = rootData.Name,
                                                          Memo = f.Memo,
                                                          Folder = f,
                                                          FormCode =
                                                              f.ProjectItem == null ? string.Empty : f.ProjectItem.Code,
                                                          DocumentTemplateId = f.DocumentId,
                                                          Hierarchy = rootData,
                                                          Kind = 7
                                                      }).ToList().Where(s => WADataProvider.FolderElementRightView.IsAllow("VIEW", s.FolderId)).ToList(); ;

            }
            #endregion

            #region ServiceNoDNS
            rootData = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_FOLDER_SERVICENONDS);
            hierarchyScope = rootData.GetLinkedHierarchy().Where(s => s.KindId == ScopeViewListId).ToList();
            allow = userScopeView.Exists(s => hierarchyScope.Exists(f => f.RightId == s));
            List<ViewFolders> viewServiceNoNds = new List<ViewFolders>();
            if (allow)
            {
                folders = rootData.GetTypeContents<Folder>();
                viewServiceNoNds = folders.Select(f => new ViewFolders
                {
                    Id = f.Id,
                    Name = f.Name,
                    HierarchyName = rootData.Name,
                    Memo = f.Memo,
                    Folder = f,
                    FormCode =
                        f.ProjectItem == null ? string.Empty : f.ProjectItem.Code,
                    DocumentTemplateId = f.DocumentId,
                    Hierarchy = rootData,
                    Kind = 7
                }).ToList().Where(s => WADataProvider.FolderElementRightView.IsAllow("VIEW", s.FolderId)).ToList(); ;

            }
            #endregion

            #region Mktg
            rootData = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("SYSTEM_FLD_MKTG");
            hierarchyScope = rootData.GetLinkedHierarchy().Where(s => s.KindId == ScopeViewListId).ToList();
            allow = userScopeView.Exists(s => hierarchyScope.Exists(f => f.RightId == s));
            List<ViewFolders> viewMktg = new List<ViewFolders>();
            if (allow)
            {
                folders = rootData.GetTypeContents<Folder>();
                viewMktg = folders.Select(f => new ViewFolders
                                                   {
                                                       Id = f.Id,
                                                       Name = f.Name,
                                                       HierarchyName = rootData.Name,
                                                       Memo = f.Memo,
                                                       Folder = f,
                                                       FormCode =
                                                           f.ProjectItem == null ? string.Empty : f.ProjectItem.Code,
                                                       DocumentTemplateId = f.DocumentId,
                                                       Hierarchy = rootData,
                                                       Kind = 7
                                                   }).ToList().Where(s => WADataProvider.FolderElementRightView.IsAllow("VIEW", s.FolderId)).ToList();
                //WADataProvider.HiearchyElementRightView.IsAllow("VIEW", hierarchy.Id))
            }
            #endregion

            #region Contract
            rootData = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("DOGOVOR");
            hierarchyScope = rootData.GetLinkedHierarchy().Where(s => s.KindId == ScopeViewListId).ToList();
            //TODO:
            allow = userScopeView.Exists(s => hierarchyScope.Exists(f => f.RightId == s));
            List<ViewFolders> viewContract = new List<ViewFolders>();
            if (allow)
            {
                folders = rootData.GetTypeContents<Folder>();//rootData.GetTypeContents<Folder>().Where(s => s.Code == "401" || s.Code == "403").ToList();
                viewContract = folders.Select(f => new ViewFolders
                {
                    Id = f.Id,
                    Name = f.Name,
                    HierarchyName = rootData.Name,
                    Memo = f.Memo,
                    Folder = f,
                    FormCode = f.ProjectItem == null ? string.Empty : f.ProjectItem.Code,
                    DocumentTemplateId = f.DocumentId,
                    Hierarchy = rootData,
                    Kind = 7
                }).ToList().Where(s => WADataProvider.FolderElementRightView.IsAllow("VIEW", s.FolderId)).ToList();
            }
            #endregion

            //viewTax
            //viewSales
            //viewService
            return viewFinance.Union(viewSales).Union(viewSalesNoNds).Union(viewTax).Union(viewService).Union(viewServiceNoNds).Union(viewMktg).Union(viewContract).Union(viewPrices).ToList();
        }
        
        #endregion

        public static IEnumerable GetCountries()
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("SYSTEM_COUNTRY_FAVORITE");
            List<Country> coll = h.GetTypeContents<Country>(true);
            return coll.Select(s => new ComboboxModel(s)).OrderBy(o => o.Name).ToList();

            //List<Country> coll = WA.GetCollection<Country>();
            //return coll.Select(s=>new ComboboxModel(s));
        }

        public static IEnumerable GetTerritories(int? ownId)
        {
            List<Territory> coll = ownId.HasValue
                                       ? WA.GetCollection<Territory>().Where(s => s.CountryId == ownId && s.KindId == Territory.KINDID_REGION).ToList()
                                       : new List<Territory>();
            return coll.Select(s => new ComboboxModel(s));
        }

        public static IEnumerable GetRegions(int? ownId)
        {
            List<Territory> coll = new List<Territory>();
            if (ownId != null)
            {
                Territory t = new Territory { Workarea = WADataProvider.WA };
                t.Load((int)ownId);
                foreach(IChain<Territory> c_t in t.GetLinks(37))
                {
                    coll.Add(c_t.Right);
                }
            }
            /*List<Territory> coll = ownId.HasValue
                                       ? WA.GetCollection<Territory>().Where(s => s.CountryId == ownId && s.KindId == Territory.KINDID_REGIONALDISTRICT).ToList()
                                       : new List<Territory>();*/
            return coll.Select(s => new ComboboxModel(s));
        }

        public static IEnumerable GetTowns(int? ownId, int? regionId)
        {
            List<Town> list = new List<Town>();
            Territory tr = null;
            Territory rg = null;
            if (ownId != null && ownId > 0)
            {
                tr = new Territory { Workarea = WA };
                tr.Load((int)ownId);
            }
            if (regionId != null && regionId > 0)
            {
                rg = new Territory { Workarea = WA };
                rg.Load((int)regionId);
            }

            if (tr != null && rg != null)
            {
                list.AddRange(tr.GetAllTownsInRegion(tr, rg));
            }

            return list;
        }

        public static IEnumerable GetStreets(int? ownId)
        {
            if(ownId.HasValue)
            {
                Town town = WA.GetObject<Town>(ownId.Value);
                return town.GetStreets();
            }
            return new List<string>();
        }

        public static SystemParameter GetSysProperty(string code)
        {
            try
            {
                SystemParameter sp = WA.Cashe.SystemParameters.ItemCode<SystemParameter>(code);
                return sp;
            }
            catch
            {
                return null;
            }
        }

        public static XmlStorage GetXmlStorage(string code, bool create = false)
        {
            try
            {
                XmlStorage storage = WA.Cashe.GetCasheData<XmlStorage>().Dictionary.Values.FirstOrDefault(
                    w => w.Code == code && w.UserId == CurrentUser.Id);
                
                if(storage==null)
                {
                    List<XmlStorage> collObjs = new List<XmlStorage>();
                    collObjs = WA.Empty<XmlStorage>().FindBy(code: code, userId: CurrentUser.Id, useAndFilter: true);
                    if (collObjs==null)
                    {
                        storage = WA.GetCollection<XmlStorage>().First(w => w.Code == code && w.UserId == CurrentUser.Id);
                        return storage;
                    }
                    else if(collObjs.Count==0)
                    {
                        storage = WA.GetCollection<XmlStorage>().First(w => w.Code == code && w.UserId == CurrentUser.Id);
                        if (storage != null)
                            return storage;
                    }
                    else if (collObjs.Count == 1)
                    {
                        storage = collObjs[0];
                        return storage;
                    }
                    else
                    {
                        throw new ApplicationException();
                    }
                }
                //return collObjs[0];
                // TODO: Срочно!!!
                
                return storage;
            }
            catch
            {
                if (create)
                {
                    XmlStorage storage = new XmlStorage
                    {
                        Workarea = WA,
                        UserId = CurrentUser.Id,
                        StateId = State.STATEACTIVE,
                        Code = code,
                        KindId = XmlStorage.KINDID_LISTDATA,
                        Name = "Параметры пейджеров таблиц в WEB интерфейсе"
                    };
                    storage.Save();
                    return storage;
                }
                else
                {
                    return null;
                }
            }
        }

        public static IEnumerable GetAddressTypes()
        {
            List<ComboboxModel> list = new List<ComboboxModel>();

            AgentAddress addr = new AgentAddress { Workarea = WA };
            foreach (EntityKind k in addr.Entity.EntityKinds)
            {
                list.Add(new ComboboxModel
                {
                    Id = k.Id,
                    Name = k.Name
                });
            }

            return list;
        }
        /// <summary>
        /// Список видов цен
        /// </summary>
        /// <param name="kindId">Идентификатор типа цен (KindId)</param>
        /// <param name="myCompanyId">Идентификатор компании, если указана возвращаются только виды цен данной компании</param>
        /// <param name="refresh">Выполнять обновление из базы данных</param>
        /// <returns></returns>
        public static List<PriceNameModel> GetPriceNames(int kindId, int? myCompanyId=null, int? currentSelectedValue=null, bool refresh=false)
        {
            if (refresh)
                WA.GetCollection<PriceName>(true);
            if(myCompanyId.HasValue && myCompanyId.Value!=0)
            {
                var coll = WA.GetCollection<PriceName>().Where(s => s.KindId == kindId && !s.IsHiden && (s.MyCompanyId == myCompanyId.Value || IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId))).Select(PriceNameModel.ToModel).ToList();
                //var coll = WA.GetCollection<PriceName>().Where(s => s.KindId == kindId && s.MyCompanyId==myCompanyId.Value && IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId) && !s.IsHiden).Select(PriceNameModel.ToModel).ToList();
                if (currentSelectedValue != null && currentSelectedValue.Value!=0)
                {
                    if(!coll.Exists(f=>f.Id==currentSelectedValue.Value))
                    {
                        coll.Add(PriceNameModel.GetObject(currentSelectedValue.Value));
                    }
                }
                return coll;
            }
            else
            {
                var coll = WA.GetCollection<PriceName>().Where(s => s.KindId == kindId && IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId) && !s.IsHiden).Select(PriceNameModel.ToModel).ToList();
                return coll;
            }
        }



        /*public static IEnumerable GetPricesNames()
        {
            List<PriceName> coll = WA.GetCollection<PriceName>();
            return coll.Where(w => w.StateId != State.STATEDELETED).Select(o => PriceNameModel.ToModel(o)).OrderBy(o => o.Name).ToList();
        }*/

        public static IEnumerable GetCurrences()
        {
            Hierarchy objRoot = WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("SYSTEM_CURRENCY_FAVORITE");
            List<Currency> coll = objRoot.GetTypeContents<Currency>(false, true);
            return coll.Where(w => w.StateId != State.STATEDELETED).Select(o => new ComboboxModel(o)).OrderBy(o => o.Name).ToList();
        }
    }
}