using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Workflow.Activities.Rules;
using BusinessObjects;
using BusinessObjects.Models;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;
using System.ComponentModel.DataAnnotations;



namespace DocumentsWeb.Areas.UserPersonal.Models
{
    public class WebMessageModel : MessageModel, IFileOwner
    {

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult("Тема обязательна", new[] { GlobalPropertyNames.Name });
            }
            if (UserOwnerId == 0)
            {
                yield return new ValidationResult("Автор обязателен", new[] { GlobalPropertyNames.UserOwnerId });
            }
            if (PriorityId == 0)
            {
                yield return new ValidationResult("Приоритет сообщения обязателен", new[] { GlobalPropertyNames.PriorityId });
            }
            if (UserId == 0)
            {
                yield return new ValidationResult("Получатель обязателен", new[] { GlobalPropertyNames.UserId });
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

        }
        
       
        public List<FileDataModel> Files { get; set; }
        
        /// <summary>
        /// Является входящим сообщением для текущего пользователя
        /// </summary>
        public bool IsIncomminMessage
        {
            get { return UserId == WADataProvider.CurrentUser.Id && UserOwnerId != WADataProvider.CurrentUser.Id; }
        }
        /// <summary>
        /// Является исходящим сообщением для текущего пользователя
        /// </summary>
        public bool IsOutcomminMessage
        {
            get { return UserOwnerId == WADataProvider.CurrentUser.Id && UserId != WADataProvider.CurrentUser.Id; }
        }
        /// <summary>
        /// Является собственным сообщением для текущего пользователя
        /// </summary>
        public bool IsSelfMessage
        {
            get { return UserOwnerId == WADataProvider.CurrentUser.Id && UserId == WADataProvider.CurrentUser.Id; }
        }
        /// <summary>
        /// Является черновиком
        /// </summary>
        public bool IsDraft
        {
            get { return !IsSend && UserOwnerId == WADataProvider.CurrentUser.Id; }
        }
        /// <summary>
        /// Является черновиком
        /// </summary>
        public bool IsTodayMessage
        {
            get { return SendDate== DateTime.Today; }
        }
        

        //-------------------------------------------
        public DateTime CreateDate { get; set; }
        //-------------------------------------------
        
        public static Uid currentUser;
        
        static WebMessageModel()
        {
            currentUser = (Membership.Provider as BusinessObjectMembershipProvider).GetUid(HttpContext.Current.User.Identity.Name);           
        }
                
        public WebMessageModel()
        {
            Files = new List<FileDataModel>();            
            ModelId = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// Количество непрочтенных сообщений
        /// </summary>
        /// <returns></returns>
        public static int GetIncomingMessagesCount()
        {
            string key = "GetIncomingMessagesCount" + WADataProvider.CurrentUserName;
            if(WADataProvider.LastActionCashe.Exists(key))
            {
                DateTime? dt = WADataProvider.LastActionCashe.GetLastDate(key);
                if(dt.HasValue && dt.Value.AddMinutes(5)>DateTime.Now)
                {
                    string v = WADataProvider.LastActionCashe.GetValue(key);
                    int retuntValue = 0;
                    if(Int32.TryParse(v, out retuntValue))
                    {
                        return retuntValue;
                    }
                }
                else
                {
                    int currentCount = Message.MessageIncommingCount(WADataProvider.CurrentUser);
                    WADataProvider.LastActionCashe.SetValue(key, currentCount.ToString());
                    return currentCount;
                }
            }
            else
            {
                int currentCount = Message.MessageIncommingCount(WADataProvider.CurrentUser);
                WADataProvider.LastActionCashe.SetValue(key, currentCount.ToString());
                return currentCount;
            }
            return Message.MessageIncommingCount(WADataProvider.CurrentUser);
        }

        /// <summary>
        /// Список последних непрочтенных сообщений
        /// </summary>
        /// <returns></returns>
        public static List<WebMessageModel> GetLastIncomingMessages()
        {
            return Message.MessageIncommingNotRead(WADataProvider.CurrentUser).Select(s => ConvertToModel(s)).ToList();
        }

        /// <summary>
        /// Список всех сообщений для текущего пользователя
        /// </summary>
        /// <returns></returns>
        public static List<WebMessageModel> GetAllMessages(bool refresh)
        {
            // TODO: добавить спец метод
            Hierarchy  hroot =  WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_MESSAGE_USERS);
            return hroot.GetTypeContents<Message>(true, refresh).Where(f => f.UserId == WADataProvider.CurrentUser.Id || f.UserOwnerId == WADataProvider.CurrentUser.Id).Select(s => ConvertToModel(s)).ToList();
        }

        /// <summary>
        /// Список всех сообщений для текущего пользователя
        /// </summary>
        /// <returns></returns>
        public static List<WebMessageModel> GetNews(bool refresh)
        {
            Hierarchy hroot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_MESSAGE_WEBNEWS);
            return hroot.GetTypeContents<Message>(true, refresh).Where(f => WADataProvider.IsCompanyIdAllowIdToCurrentUser(f.MyCompanyId)).Select(s => ConvertToModel(s)).ToList();
        }

        public static List<WebMessageModel> GetCollection(int HierarchyId, bool Refresh = false, bool nested = false)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(HierarchyId);
            List<Message> coll = h.GetTypeContents<Message>(nested, Refresh);
            if (Refresh) WADataProvider.RefreshLibrariesElementRightView(HttpContext.Current.User.Identity.Name);
            return coll.Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(s => ConvertToModel(s)).OrderBy(o => o.SendDate).ToList();
        }

        public Message ToObject()
        {
            Message obj = new Message { Workarea = WADataProvider.WA};
            if (obj.Id == 0)
            {
                obj.IsNew = true;
                obj.StateId = State.STATEACTIVE;                
                obj.KindId = Message.KINDID_USER;
                obj.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
            }
            obj.Id = Id;
            if(obj.Id!=0)
                obj.Load(Id);
            obj.UserName = WADataProvider.CurrentMembershipUser.UserName;
            obj.Name = Name;
            obj.NameFull = NameFull;

            if(string.IsNullOrEmpty(Memo))
            {
                obj.Memo = "текст отсутствует...";
            }
            else
            {
                obj.Memo = Memo;
            }
            
            obj.Code = Code;
            obj.CodeFind = CodeFind;
            obj.HasRead = HasRead;
            obj.ReadDone = ReadDone;
            obj.IsSend = IsSend;

            
            obj.PriorityId = PriorityId;            
            Analitic p = WADataProvider.WA.GetObject<Analitic>(PriorityId);
            obj.Priority.Name = p.Name;

            obj.ReadDate = ReadDate;
            obj.ReadTime = Period.DateTime2TimeSpan(ReadTimeAsDate);
            obj.SendDate = SendDate;
            obj.SendTime = Period.DateTime2TimeSpan(SendTimeAsDate);

            obj.UserId = UserId;
            Uid u = WADataProvider.WA.GetObject<Uid>(UserId);
            obj.User.Name = u.Name;

            obj.UserOwnerId = UserOwnerId;
            u = WADataProvider.WA.GetObject<Uid>(UserOwnerId);
            obj.UserOwner.Name = u.Name;            

            return obj;
        }
        
        
        public static WebMessageModel ConvertToModel(Message message, bool partialLoad = true)
        {
            WebMessageModel obj = new WebMessageModel();
            obj.GetData(message);
            if (!partialLoad)
            {
                obj.ReloadOnEdit = false;
                obj.Files = message.GetLinkedFiles().Where(s => s.StateId != State.STATEDELETED).Select(s => FileDataModel.ConvertToModel(s.Right)).ToList();
            }
            else
            {
                obj.ReloadOnEdit = true;
            }
            if (message.UserId!=0 && message.User.AgentId!=0)
            {
                obj.AgentToName = message.User.Agent.Name;
            }
            return obj;

        }

        public static WebMessageModel GetObject(int id)
        {            
            Message message = WADataProvider.WA.GetObject<Message>(id);
            return ConvertToModel(message);
        }

        
        public static List<Message> GetIncomingMessages(int userId)
        {
            List<Message> resultList = WADataProvider.WA.GetCollection<Message>().Where(f => f.UserId == userId && f.IsSend && !f.IsStateDeleted).ToList();
            return resultList;
        }

        public static int GetIncomingMessagesCount(int userId)
        {
            List<Message> resultList = WADataProvider.WA.GetCollection<Message>().Where(f => f.UserId == userId && f.IsSend && !f.IsStateDeleted).ToList();
            return resultList.Count;
        }

        public static List<Message> GetOutcomingMessages(int userId)
        {
            List<Message> resultList = WADataProvider.WA.GetCollection<Message>().Where(f => f.UserOwnerId == userId && f.IsSend && !f.IsStateDeleted).ToList();
            return resultList;
        }

        public static int GetOutComingMessagesCount(int userId)
        {
            List<Message> resultList = WADataProvider.WA.GetCollection<Message>().Where(f => f.UserOwnerId == userId && f.IsSend && !f.IsStateDeleted).ToList();
            return resultList.Count;
        }

        public static List<Message> GetDeletedMessages(int userId)
        {
            List<Message> resultList = WADataProvider.WA.GetCollection<Message>().Where((f => f.UserOwnerId == userId && f.IsStateDeleted || f.UserId == userId && f.IsStateDeleted && f.IsSend)).ToList();
            return resultList;
        }

        public static int GetDeletedMessagesCount(int userId)
        {
            List<Message> resultList = WADataProvider.WA.GetCollection<Message>().Where((f => f.UserOwnerId == userId && f.IsStateDeleted || f.UserId == userId && f.IsStateDeleted && f.IsSend)).ToList();
            return resultList.Count;
        }

        public static List<Message> GetRoughCopiesMessages(int userId)
        {
            List<Message> resultList = WADataProvider.WA.GetCollection<Message>().Where(f => !f.IsSend && f.UserOwnerId == userId && !f.IsStateDeleted).ToList();
            return resultList;
        }

        public static int GetRoughCopiesMessagesCount(int userId)
        {
            List<Message> resultList = WADataProvider.WA.GetCollection<Message>().Where(f => !f.IsSend && f.UserOwnerId == userId && !f.IsStateDeleted).ToList();
            return resultList.Count;
        }
        

        public static List<Uid> GetUsers()
        {
            List<Uid> resultList = WADataProvider.WA.GetCollection<Uid>().Where(f => f.Id != currentUser.Id && f.KindId == Uid.KINDID_USER).ToList();
            return resultList;
        }

        public static List<ComboboxModel> UsersListWithoutId(int id)
        {
            List<Uid> resultList = WADataProvider.WA.GetCollection<Uid>().Where(f => f.Id != id && f.KindId == Uid.KINDID_USER && f.Agent != null).ToList();
            foreach (var uid in resultList)
            {
                uid.Name = uid.Agent.Name;
            }
            return resultList.Select(s=>new ComboboxModel(s)).ToList();
        }

        public static List<ComboboxModel> GetFullUsersList()
        {
            List<Uid> resultList = WADataProvider.WA.GetCollection<Uid>().Where(f => f.KindId == Uid.KINDID_USER && f.Agent != null).ToList();
            foreach (var uid in resultList)
            {
                uid.Name = uid.Agent.Name;
            }
            return resultList.Select(s => new ComboboxModel(s)).ToList();
        }

        public static List<ComboboxModel> GetPriority()
        {            
            List<Analitic> resultList = WADataProvider.WA.GetCollection<Analitic>().Where(f => f.KindId == Analitic.KINDID_IMPORTANCE).OrderBy(s=>s.Name).ToList();            
            return resultList.Select(s=>new ComboboxModel(s)).ToList();
        }

        public bool IsValid()
        {
            bool res;
            res = true;
            if (Name == "") res = false;
            if (UserId == 0) res = false;
            if(PriorityId == 0) res = false;            
            if (NameFull == "") res = false;
            return res;            
        }

        public static void SetStateNotDone(int id, string hierarchyCode)
        {
            Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheTaskModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id, string hierarchyCode)
        {
            Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(id);
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
                        obj = WADataProvider.WA.GetObject<Message>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheTaskModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id, string hierarchyCode)
        {
            Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheTaskModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void ToTrash(int id)
        {
            Message obj = WADataProvider.WA.Cashe.GetCasheData<Message>().Item(id);
            obj.Remove();
        }
        /// <summary>
        /// Текущее значение аттрибутов в виде строки
        /// </summary>
        /// <returns></returns>
        public string FlagsValueString()
        {
            return ToObject().FlagsValueString();
        }
    }
}